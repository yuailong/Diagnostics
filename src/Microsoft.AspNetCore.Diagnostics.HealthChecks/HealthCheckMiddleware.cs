// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckMiddleware(
            RequestDelegate next,
            IOptions<HealthCheckOptions> healthCheckOptions,
            IHealthCheckService healthCheckService)
        {
            _next = next;
            _healthCheckOptions = healthCheckOptions.Value;
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path != _healthCheckOptions.Path)
            {
                await _next(httpContext);
                return;
            }

            // Get results
            var result = await _healthCheckService.CheckHealthAsync(httpContext.RequestAborted);

            // Map status to response code - this is done before calling the response writer
            // this lets the writer customize the status code.
            switch (result.Status)
            {
                case HealthCheckStatus.Failed:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
                case HealthCheckStatus.Unhealthy:
                    httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    break;
                case HealthCheckStatus.Degraded:
                    // Degraded doesn't mean unhealthy so we return 200, but the content will contain more details
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    break;
                case HealthCheckStatus.Healthy:
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    break;
                default:
                    // This will only happen when we change HealthCheckStatus and we don't update this.
                    Debug.Fail($"Unrecognized HealthCheckStatus value: {result.Status}");
                    throw new InvalidOperationException($"Unrecognized HealthCheckStatus value: {result.Status}");
            }

            if (_healthCheckOptions.ResponseWriter != null)
            {
                await _healthCheckOptions.ResponseWriter.WriteResponseAsync(httpContext, result);
            }
        }
    }
}
