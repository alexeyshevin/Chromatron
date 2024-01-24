﻿// Copyright © 2024 Greeana LLC. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

#nullable disable
#pragma warning disable IDE0057

namespace Chromatron.Core.Network;

public class PathAndQuery
{
    public string Path { get; set; }
    public IDictionary<string, string> QueryParameters { get; set; }

    public void Parse(string requestUrl)
    {
        var uri = CreateUri(requestUrl);
        Path = uri?.AbsolutePath;

        if (uri is not null)
        {
            QueryParameters = GetParameters(uri.PathAndQuery);
        }
    }

    public static Uri CreateUri(string requestUrl)
    {
        if (string.IsNullOrWhiteSpace(requestUrl))
        {
            return default;
        }

        if (Uri.IsWellFormedUriString(requestUrl, UriKind.Absolute)
            && Uri.TryCreate(requestUrl, UriKind.Absolute, out var result))
        {
            return result;
        }

        try
        {
            if (!requestUrl.Trim().StartsWith("/", StringComparison.Ordinal))
            {
                requestUrl = $"/{requestUrl.Trim()}";
            }

            var dummyScheme = "http";
            var dummyHost = "dummy.com";
            var wellFormedUrl = $"{dummyScheme}://{dummyHost}{requestUrl}";
            return new Uri(wellFormedUrl);
        }
        catch { }

        return default;
    }

    public static IDictionary<string, string> GetParameters(string querypath)
    {
        if (string.IsNullOrWhiteSpace(querypath))
        {
            return null;
        }

        var nameValueCollection = new NameValueCollection();

        string querystring = string.Empty;
        int index = querypath.IndexOf('?');
        if (index > 0)
        {
            querystring = querypath.Substring(querypath.IndexOf('?'));
            nameValueCollection = HttpUtility.ParseQueryString(querystring);
        }

        if (string.IsNullOrEmpty(querystring))
        {
            return null;
        }

        return nameValueCollection.AllKeys.ToDictionary(x => x, x => nameValueCollection[x]);
    }
}