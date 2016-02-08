using Authorization.Services;
using FluentAssertions;
using Xunit;

namespace Authorization.Tests.Services
{
    public class DefaultDiscountPermissionServiceTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(0.1, true)]
        [InlineData(10, true)]
        [InlineData(10.1, false)]
        public void IsDiscountAllowed(decimal amount, bool expectedResult)
        {
            var sut = new DefaultDiscountPermissionService();

            var result = sut.IsDiscountAllowed(1, amount);

            result.Should().Be(expectedResult);
        }
    }
}
