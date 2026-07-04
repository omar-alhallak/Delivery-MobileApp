using System.Reflection;
using DeliveryApp.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DeliveryApp.API.Tests.Notifications
{
    public sealed class NotificationsControllerSecurityTests
    {
        [Fact]
        public void Controller_RequiresAuthorization()
        {
            typeof(NotificationsController)
                .GetCustomAttribute<AuthorizeAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public void Controller_DoesNotAcceptUserIdInRoutes()
        {
            var routes = typeof(NotificationsController)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
                .Select(route => route.Template)
                .ToList();

            routes.Should().Contain("my");
            routes.Should().Contain("read-all");
            routes.Should().NotContain(route => route != null && route.Contains("userId", StringComparison.OrdinalIgnoreCase));
        }
    }
}
