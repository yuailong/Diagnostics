// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    /// <summary>
    /// Writes a <see cref="CompositeHealthCheckResult"/> as an HTTP response.
    /// </summary>
    public abstract class HealthCheckResponseWriter
    {
        /// <summary>
        /// Writes the provided <see cref="CompositeHealthCheckResult"/> as the HTTP response assoicated
        /// with the provided <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with the current request.</param>
        /// <param name="result">The <see cref="CompositeHealthCheckResult"/> to serialize.</param>
        /// <returns>A <see cref="Task"/> while will asynchronously complete when the response is written.</returns>
        public abstract Task WriteResponseAsync(HttpContext httpContext, CompositeHealthCheckResult result);
    }
}
