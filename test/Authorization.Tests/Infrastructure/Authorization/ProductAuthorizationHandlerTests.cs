using System.Linq;
using System.Security.Claims;
using Authorization.Infrastructure.Authorization;
using Authorization.Models;
using FluentAssertions;
using Microsoft.AspNet.Authorization;
using Xunit;

namespace Authorization.Tests.Infrastructure.Authorization
{
    public class ProductAuthorizationHandlerTests
    {
        private readonly ProductAuthorizationHandler _handler;
        private bool _isDiscountAllowedResult = true;

        public ProductAuthorizationHandlerTests()
        {
            _handler = new ProductAuthorizationHandler(
                new FakeDiscountPermissionService((productId, amount) => _isDiscountAllowedResult));
        }

        [Fact]
        public void Not_Senior_Not_Sales_Department_User_NOT_Allowed_To_Edit_Product()
        {
            var requirement = ProductOperations.Edit;
            var user = new ClaimsPrincipal(new ClaimsIdentity(Enumerable.Empty<Claim>(), "custom"));
            var product = new Product { Id = 1, ProductType = ProductType.Standard };
            var context = new AuthorizationContext(new[] { requirement }, user, product);

            _handler.Handle(context);

            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public void Not_Senior_Sales_Department_User_Allowed_To_Edit_Standard_Product()
        {
            var requirement = ProductOperations.Edit;
            var user = CreateSalesDepartmentPrincipal();
            var product = new Product { Id = 1, ProductType = ProductType.Standard };
            var context = new AuthorizationContext(new[] { requirement }, user, product);

            _handler.Handle(context);

            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public void Not_Senior_Sales_Department_User_NOT_Allowed_To_Edit_Special_Product()
        {
            var requirement = ProductOperations.Edit;
            var user = CreateSalesDepartmentPrincipal();
            var product = new Product { Id = 1, ProductType = ProductType.Special };
            var context = new AuthorizationContext(new[] { requirement }, user, product);

            _handler.Handle(context);

            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public void Senior_Sales_Department_User_Allowed_To_Edit_Special_Product()
        {
            var requirement = ProductOperations.Edit;
            var user = CreateSalesDepartmentPrincipal();
            user.Identities.First().AddClaim(new Claim("status", "senior"));
            var product = new Product { Id = 1, ProductType = ProductType.Special };
            var context = new AuthorizationContext(new[] { requirement }, user, product);

            _handler.Handle(context);

            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public void Not_Senior_Sales_Department_User_Allowed_To_Discount_Standard_Product_If_External_Service_Allows()
        {
            var requirement = ProductOperations.GiveDiscount(10);
            var user = CreateSalesDepartmentPrincipal();
            var product = new Product { Id = 1, ProductType = ProductType.Standard };
            var context = new AuthorizationContext(new[] { requirement }, user, product);
            _isDiscountAllowedResult = true;

            _handler.Handle(context);

            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public void Not_Senior_Sales_Department_User_NOT_Allowed_To_Discount_Standard_Product_If_External_Service_NOT_Allows()
        {
            var requirement = ProductOperations.GiveDiscount(10);
            var user = CreateSalesDepartmentPrincipal();
            var product = new Product { Id = 1, ProductType = ProductType.Standard };
            var context = new AuthorizationContext(new[] { requirement }, user, product);
            _isDiscountAllowedResult = false;

            _handler.Handle(context);

            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public void Not_Senior_Sales_Department_User_NOT_Allowed_To_Discount_Special_Product_If_External_Service_Allows()
        {
            var requirement = ProductOperations.GiveDiscount(10);
            var user = CreateSalesDepartmentPrincipal();
            var product = new Product { Id = 1, ProductType = ProductType.Special };
            var context = new AuthorizationContext(new[] { requirement }, user, product);
            _isDiscountAllowedResult = true;

            _handler.Handle(context);

            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public void Senior_Sales_Department_User_Allowed_To_Discount_Special_Product_If_External_Service_Allows()
        {
            var requirement = ProductOperations.GiveDiscount(10);
            var user = CreateSalesDepartmentPrincipal();
            user.Identities.First().AddClaim(new Claim("status", "senior"));
            var product = new Product { Id = 1, ProductType = ProductType.Special };
            var context = new AuthorizationContext(new[] { requirement }, user, product);
            _isDiscountAllowedResult = true;

            _handler.Handle(context);

            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public void Senior_Sales_Department_User_NOT_Allowed_To_Discount_Special_Product_If_External_Service_NOT_Allows()
        {
            var requirement = ProductOperations.GiveDiscount(10);
            var user = CreateSalesDepartmentPrincipal();
            user.Identities.First().AddClaim(new Claim("status", "senior"));
            var product = new Product { Id = 1, ProductType = ProductType.Special };
            var context = new AuthorizationContext(new[] { requirement }, user, product);
            _isDiscountAllowedResult = false;

            _handler.Handle(context);

            context.HasSucceeded.Should().BeFalse();
        }

        private static ClaimsPrincipal CreateSalesDepartmentPrincipal()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim("department", "sales"),
                },
                "custom"));
            return user;
        }
    }
}
