using System;
using Authorization.Services;

namespace Authorization.Tests
{
    internal class FakeDiscountPermissionService : IDiscountPermissionService
    {
        private readonly Func<int, decimal, bool> _discountAllowedDelegate;

        public FakeDiscountPermissionService(Func<int, decimal, bool> discountAllowedDelegate)
        {
            _discountAllowedDelegate = discountAllowedDelegate;
        }

        public bool IsDiscountAllowed(int productId, decimal amount)
        {
            return _discountAllowedDelegate(productId, amount);
        }
    }
}