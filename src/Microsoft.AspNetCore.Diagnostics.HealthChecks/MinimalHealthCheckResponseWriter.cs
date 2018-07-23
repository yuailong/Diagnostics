// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    /// <summary>
    /// A <see cref="HealthCheckResponseWriter"/> that writes the <see cref="HealthCheckStatus"/>
    /// as <c>text/plain</c> content.
    /// </summary>
    public sealed class MinimalHealthCheckResponseWriter : HealthCheckResponseWriter
    {
        public override Task WriteResponseAsync(HttpContext httpContext, CompositeHealthCheckResult result)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync(result.Status.ToString());
        }
    }
}
