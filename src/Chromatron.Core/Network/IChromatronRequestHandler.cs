﻿// Copyright © 2024 Greeana LLC. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace Chromatron.Core.Network;

/// <summary>
/// <see cref="IChromatronRequestHandler"/> specify interface functionalities for Chromatron request handling.
/// </summary>
public interface IChromatronRequestHandler
{
    /// <summary>
    /// Execute fire and forget requests. 
    /// </summary>
    /// <remarks>
    /// This expects requests that do not have responses.
    /// </remarks>
    /// <param name="url">The controller route URL path.</param>
    void Execute(string url);

    /// <summary>
    /// Execute synchronously requests that have responses.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="routeUrl">The controller route URL path.</param>
    /// <param name="parameters">A dictionary of request parameters.</param>
    /// <param name="postData">Post data object.</param>
    /// <param name="requestData">The json data of the entire request object.</param>
    /// <returns>Instance of <see cref="IChromatronResponse"/>.</returns>
    IChromatronResponse Execute(string requestId, string routeUrl, IDictionary<string, object>? parameters,
        object? postData, string? requestData);

    /// <summary>
    /// Execute asynchronously requests that have responses.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="routeUrl">The controller route URL path.</param>
    /// <param name="parameters">A dictionary of request parameters.</param>
    /// <param name="postData">Post data object.</param>
    /// <param name="requestData">The json data of the entire request object.</param>
    /// <returns>Instance of <see cref="IChromatronResponse"/>.</returns>
    Task<IChromatronResponse> ExecuteAsync(string requestId, string routeUrl, IDictionary<string, object>? parameters, object? postData, string? requestData);
}