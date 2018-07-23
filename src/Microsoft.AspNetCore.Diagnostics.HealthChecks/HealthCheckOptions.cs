// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    /// <summary>
    /// Contains options for the <see cref="HealthCheckMiddleware"/>.
    /// </summary>
    public class HealthCheckOptions
    {
        /// <summary>
        /// Gets or sets the path at which the Health Check results will be available. Defaults
        /// to <c>/health</c>.
        /// </summary>
        public PathString Path { get; set; } = new PathString("/health");

        /// <summary>
        /// Gets or sets the <see cref="HealthCheckResponseWriter"/> used to write the response.
        /// </summary>
        /// <remarks>
        /// The default value is an instance of <see cref="MinimalHealthCheckResponseWriter"/> which
        /// will write <see cref="HealthCheckStatus"/> as <c>text/plain</c> content.
        /// </remarks>
        public HealthCheckResponseWriter ResponseWriter { get; set; } = new MinimalHealthCheckResponseWriter();
    }
}
