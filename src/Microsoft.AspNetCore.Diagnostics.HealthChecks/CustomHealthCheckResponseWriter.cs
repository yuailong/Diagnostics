// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    public sealed class CustomHealthCheckResponseWriter : HealthCheckResponseWriter
    {
        private readonly Func<HttpContext, CompositeHealthCheckResult, Task> _writer;

        public CustomHealthCheckResponseWriter(Func<HttpContext, CompositeHealthCheckResult,Task> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            _writer = writer;
        }

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

            return _writer(httpContext, result);
        }
    }
}
