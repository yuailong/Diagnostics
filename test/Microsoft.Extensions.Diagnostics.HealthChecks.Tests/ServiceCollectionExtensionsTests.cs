using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Extensions.Diagnostics.HealthChecks.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddHealthChecks_RegistersSingletonHealthCheckServiceIdempotently()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddHealthChecks();
            services.AddHealthChecks();

            // Assert
            Assert.Collection(services,
                actual =>
                {
                    Assert.Equal(ServiceLifetime.Singleton, actual.Lifetime);
                    Assert.Equal(typeof(IHealthCheckService), actual.ServiceType);
                    Assert.Equal(typeof(HealthCheckService), actual.ImplementationType);
                    Assert.Null(actual.ImplementationInstance);
                    Assert.Null(actual.ImplementationFactory);
                });
        }
    }
}
