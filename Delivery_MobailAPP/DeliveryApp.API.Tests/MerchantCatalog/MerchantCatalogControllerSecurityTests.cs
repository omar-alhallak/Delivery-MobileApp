using System.Reflection;
using DeliveryApp.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DeliveryApp.API.Tests.MerchantCatalog
{
    public sealed class MerchantCatalogControllerSecurityTests
    {
        [Fact]
        public void Controller_RequiresAuthorization()
        {
            typeof(MerchantCatalogController)
                .GetCustomAttribute<AuthorizeAttribute>()
                .Should().NotBeNull();
        }

        [Theory]
        [InlineData(nameof(MerchantCatalogController.GetSystemCategories))]
        [InlineData(nameof(MerchantCatalogController.GetPublicMerchantCatalog))]
        public void PublicReadEndpoints_AllowAnonymous(string methodName)
        {
            typeof(MerchantCatalogController)
                .GetMethod(methodName)!
                .GetCustomAttribute<AllowAnonymousAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public void SystemCategoryApi_ExposesReadOnlyEndpoint()
        {
            var endpoints = typeof(MerchantCatalogController)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
                .Where(attribute => attribute.Template?.StartsWith("system-categories", StringComparison.Ordinal) == true)
                .ToList();

            endpoints.Should().ContainSingle();
            endpoints.Single().HttpMethods.Should().ContainSingle().Which.Should().Be("GET");
        }
    }
}
