using System.Reflection;
using DeliveryApp.API.Controllers;
using DeliveryApp.Application.Features.Addresses.CreateAddressLocation;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DeliveryApp.API.Tests.Addresses
{
    public sealed class AddressesControllerSecurityTests
    {
        [Fact]
        public void Controller_RequiresAuthorization()
        {
            typeof(AddressesController)
                .GetCustomAttribute<AuthorizeAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public void Controller_ExposesOnlyCurrentUsersAddressList()
        {
            var getRoutes = typeof(AddressesController)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
                .Where(route => route.HttpMethods.Contains("GET"))
                .Select(route => route.Template)
                .ToList();

            getRoutes.Should().Contain("my");
            getRoutes.Should().NotContain(route => route != null && route.StartsWith("users/", StringComparison.Ordinal));
        }

        [Fact]
        public void CreateLocationRequest_DoesNotAcceptUserId()
        {
            typeof(CreateAddressLocationRequest)
                .GetProperty("UserId")
                .Should().BeNull();
        }
    }
}
