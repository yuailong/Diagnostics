// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public static class HealthCheckAppBuilderExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder applicationBuilder, PathString path)
        {
            return applicationBuilder.UseMiddleware<HealthCheckMiddleware>(new HealthCheckOptions()
            {
                Path = path
            });
        }
    }
}
