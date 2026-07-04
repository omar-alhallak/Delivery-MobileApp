using System.Reflection;
using DeliveryApp.API.Controllers;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DeliveryApp.API.Tests.Orders
{
    public sealed class OrdersControllerSecurityTests
    {
        [Fact]
        public void Controller_RequiresAuthorization()
        {
            typeof(OrdersController)
                .GetCustomAttribute<AuthorizeAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public void Controller_DoesNotExposeHardDeleteOrUnpaidEndpoints()
        {
            var routes = typeof(OrdersController)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
                .ToList();

            routes.Should().NotContain(route => route.HttpMethods.Contains("DELETE"));
            routes.Should().NotContain(route => route.Template != null && route.Template.EndsWith("/unpaid", StringComparison.Ordinal));
        }

        [Fact]
        public void CreateRequest_DoesNotAcceptCallerOrPaymentState()
        {
            typeof(CreateOrderRequest).GetProperty("CustomerId").Should().BeNull();
            typeof(CreateOrderRequest).GetProperty("PaymentStatus").Should().BeNull();
            typeof(CreateOrderRequest).GetProperty("PaymentMethod").Should().BeNull();
            typeof(CreateOrderRequest).GetProperty("DeliveryFee").Should().BeNull();
        }

        [Fact]
        public void OrderItemRequest_DoesNotAcceptCatalogSnapshotsFromClient()
        {
            typeof(OrderItemRequest).GetProperty("ProductName").Should().BeNull();
            typeof(OrderItemRequest).GetProperty("VariantName").Should().BeNull();
            typeof(OrderItemRequest).GetProperty("UnitPrice").Should().BeNull();
        }
    }
}
