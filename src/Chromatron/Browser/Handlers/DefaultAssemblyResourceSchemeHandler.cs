﻿// Copyright © 2024 Greeana LLC. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace Chromatron.Browser;

/// <summary>
/// Default implementation of <see cref="CefResourceHandler"/> for assembly resource handling.
/// </summary>
public class DefaultAssemblyResourceSchemeHandler : CefResourceHandler
{
    protected readonly IChromatronConfiguration _config;
    protected readonly IChromatronErrorHandler _chromatronErrorHandler;

    protected IChromatronResource _chromatronResource;
    protected FileInfo? _fileInfo;
    protected bool _completed;
    protected int _totalBytesRead;
    protected Regex _regex = new("[/]");

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultAssemblyResourceSchemeHandler"/>.
    /// </summary>
    /// <param name="config">Instance of <see cref="IChromatronConfiguration"/>.</param>
    /// <param name="chromatronErrorHandler">Instance of <see cref="IChromatronErrorHandler"/>.</param>
    public DefaultAssemblyResourceSchemeHandler(IChromatronConfiguration config, IChromatronErrorHandler chromatronErrorHandler)
    {
        _config = config;
        _chromatronResource = new ChromatronResource();
        _chromatronErrorHandler = chromatronErrorHandler;
        _fileInfo = null;
    }

    /// <inheritdoc/>
    [Obsolete("ProcessRequest is obsolete.")]
    protected override bool ProcessRequest(CefRequest request, CefCallback callback)
    {
        var u = new Uri(request.Url);
        var fileAbsolutePath = u.AbsolutePath;
        var file = u.Authority + fileAbsolutePath;
        if (string.IsNullOrEmpty(Path.GetFileName(file)))
        {
            file = Path.Combine(file, "index.html");
        }

        _totalBytesRead = 0;
        _chromatronResource.Content = null;
        _completed = false;

        if (ProcessAssmblyEmbeddedFile(request.Url, file, fileAbsolutePath, callback))
        {
            return true;
        }

        if (ProcessLocalFile(file, callback))
        {
            return true;
        }

        callback.Dispose();
        return false;
    }

    /// <inheritdoc/>
    protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
    {
        // unknown content-length
        // no-redirect
        responseLength = -1;
        redirectUrl = string.Empty;

        try
        {
            var headers = response.GetHeaderMap();
            headers.Add("Access-Control-Allow-Origin", "*");
            response.SetHeaderMap(headers);

            response.Status = (int)_chromatronResource.StatusCode;
            response.MimeType = _chromatronResource.MimeType;
            response.StatusText = _chromatronResource.StatusText;
        }
        catch (Exception exception)
        {
            _chromatronResource = _chromatronErrorHandler.HandleError(_fileInfo, exception);
            response.Status = (int)_chromatronResource.StatusCode;
            response.MimeType = _chromatronResource.MimeType;
            response.StatusText = _chromatronResource.StatusText;
        }
    }

    /// <inheritdoc/>
    [Obsolete("ReadResponse is obsolete.")]
    protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
    {
        int currBytesRead = 0;

        try
        {
            if (_completed)
            {
                bytesRead = 0;
                _totalBytesRead = 0;
                _chromatronResource.Content = null;
                return false;
            }
            else
            {
                if (_chromatronResource.Content is not null)
                {
                    var fileBytes = _chromatronResource.Content.ToArray();
                    currBytesRead = Math.Min(fileBytes.Length - _totalBytesRead, bytesToRead);
                    response.Write(fileBytes, _totalBytesRead, currBytesRead);
                    _totalBytesRead += currBytesRead;

                    if (_totalBytesRead >= fileBytes.Length)
                    {
                        _completed = true;
                    }
                }
                else
                {
                    bytesRead = 0;
                    _completed = true;
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Instance.Log.LogError(exception);
        }

        bytesRead = currBytesRead;
        return true;
    }

    /// <inheritdoc/>
    protected override void Cancel()
    {
    }

    /// <inheritdoc/>
    protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
    {
        handleRequest = false;
        return false;
    }

    /// <inheritdoc/>
    protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
    {
        bytesSkipped = 0;
        return true;
    }

    /// <inheritdoc/>
    protected override bool Read(IntPtr dataOut, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
    {
        bytesRead = -1;
        return false;
    }

    private bool ProcessLocalFile(string file, CefCallback callback)
    {
        _fileInfo = new FileInfo(file);

        // Check if file exists
        if (!_fileInfo.Exists)
        {
            _chromatronResource = _chromatronErrorHandler.HandleError(_fileInfo);
            callback.Continue();
        }
        // Check if file exists but empty
        else if (_fileInfo.Length == 0)
        {
            _chromatronResource = _chromatronErrorHandler.HandleError(_fileInfo);
            callback.Continue();
        }
        else
        {
            Task.Run(() =>
            {
                using (callback)
                {
                    try
                    {
                        var fileBytes = File.ReadAllBytes(file);
                        _chromatronResource.Content = new MemoryStream(fileBytes);

                        string extension = Path.GetExtension(file);
                        _chromatronResource.MimeType = MimeMapper.GetMimeType(extension);
                        _chromatronResource.StatusCode = ResourceConstants.StatusOK;
                        _chromatronResource.StatusText = ResourceConstants.StatusOKText;
                    }
                    catch (Exception exception)
                    {
                        _chromatronResource = _chromatronErrorHandler.HandleError(_fileInfo, exception);
                    }
                    finally
                    {
                        callback.Continue();
                    }
                }
            });

            return true;
        }

        return false;
    }

    private bool ProcessAssmblyEmbeddedFile(string url, string file, string fileAbsolutePath, CefCallback callback)
    {
        var urlScheme = _config?.UrlSchemes?.GetScheme(url, UrlSchemeType.AssemblyResource);
        AssemblyOptions? option = urlScheme?.AssemblyOptions;
        if (option is null || option.TargetAssembly is null)
        {
            return false;
        }

        var manifestName = string.Join(".", option.DefaultNamespace, option.RootFolder, _regex.Replace(fileAbsolutePath, ".")).Replace("..", ".").Replace("..", ".");
        Stream? stream = option.TargetAssembly.GetManifestResourceStream(manifestName);

        // Check if file exists
        if (stream is null)
        {
            _chromatronResource = _chromatronErrorHandler.HandleError(stream);
            callback.Continue();
        }
        // Check if file exists but empty
        else if (stream.Length == 0)
        {
            _chromatronResource = _chromatronErrorHandler.HandleError(stream);
            stream.Dispose();

            callback.Continue();
        }
        else
        {
            Task.Run(() =>
            {
                using (callback)
                {
                    try
                    {
                        var fileBytes = new byte[stream.Length];
                        stream.Read(fileBytes, 0, (int)stream.Length);
                        stream.Flush();
                        stream.Dispose();

                        _chromatronResource.Content = new MemoryStream(fileBytes);
                        string extension = Path.GetExtension(file);
                        _chromatronResource.MimeType = MimeMapper.GetMimeType(extension);
                        _chromatronResource.StatusCode = ResourceConstants.StatusOK;
                        _chromatronResource.StatusText = ResourceConstants.StatusOKText;
                    }
                    catch (Exception exception)
                    {
                        _chromatronResource = _chromatronErrorHandler.HandleError(_fileInfo, exception);
                    }
                    finally
                    {
                        callback.Continue();
                    }
                }
            });

            return true;
        }

        return false;
    }
}