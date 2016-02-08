using Authorization.Models;
using Authorization.Services;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;

namespace Authorization.Infrastructure.Authorization
{
    public class ProductAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        private readonly IDiscountPermissionService _discountPermissionService;

        public ProductAuthorizationHandler(IDiscountPermissionService discountPermissionService)
        {
            _discountPermissionService = discountPermissionService;
        }

        protected override void Handle(
            AuthorizationContext context,
            OperationAuthorizationRequirement requirement,
            Product resource)
        {
            // Products can be handled only by sales people
            if (!context.User.HasClaim("department", "sales"))
            {
                return;
            }

            // Special products can be edited only by senior sales people
            if (requirement == ProductOperations.Edit)
            {
                if (resource.ProductType == ProductType.Special)
                {
                    if (!context.User.HasClaim("status", "senior"))
                    {
                        return;
                    }
                }
            }

            // Discount operations needs an external service to validate
            // the discount ammount for a given product
            var discountRequirement = requirement as DiscountOperationAuthorizationRequirement;

            if (discountRequirement != null)
            {
                if (resource.ProductType == ProductType.Special)
                {
                    if (!context.User.HasClaim("status", "senior"))
                    {
                        return;
                    }
                }

                HandleDiscountOperation(context, discountRequirement, resource);

                return;
            }

            // Other operations are allowed
            context.Succeed(requirement);
        }

        private void HandleDiscountOperation(
            AuthorizationContext context,
            DiscountOperationAuthorizationRequirement requirement,
            Product resource)
        {
            var result = _discountPermissionService.IsDiscountAllowed(resource.Id, requirement.Amount);

            if (result)
            {
                context.Succeed(requirement);
            }
        }
    }
}
