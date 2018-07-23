// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension methods for the <see cref="HealthCheckMiddleware"/>.
    /// </summary>
    public static class HealthCheckAppBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// The health check middleware will use the configured <see cref="HealthCheckOptions"/> to determine
        /// its settings. By default the middleware will respond to requests on the URL path <c>/health</c>.
        /// </remarks>
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HealthCheckMiddleware>();
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// The health check middleware will use default settings other than the provided <paramref name="path"/>.
        /// </remarks>
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app, PathString path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(path.Value))
            {
                throw new ArgumentException("A URL path must be provided", nameof(path));
            }

            return UseHealthChecks(app, new HealthCheckOptions() { Path = path, });
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HealthCheckMiddleware>(Options.Create(options));
        }
    }
}
