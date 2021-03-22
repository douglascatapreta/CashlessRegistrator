using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenGenerator.Controllers;
using TokenGenerator.Models;
using Xunit;

namespace TokenGenerator.Tests
{
    public class TokensControllerValidateToken
    {
        [Fact]
        public async void ValidationIsTrueWithValidToken()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("CashlessRegistratorVT")
                .Options;

            var context = new ApiContext(options);

            var controller = new TokensController(context);
            var model = new Card
            {
                CardNumber = 4687334569974198,
                CVV = 233,
                CustomerId = 1
            };

            var response = await controller.GetToken(model);

            model = new Card
            {
                CVV = 233,
                CustomerId = 1,
                Token = 8419,
                CardId = 1
            };

            response = await controller.ValidateToken(model);

            Assert.IsType<AcceptedResult>(response.Result);
        }

        [Fact]
        public async void ValidationIsFalseWithInvalidToken()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("CashlessRegistratorVT")
                .Options;

            var context = new ApiContext(options);

            var controller = new TokensController(context);

            var model = new Card
            {
                CVV = 233,
                CustomerId = 1,
                Token = 8419,
                CardId = 1
            };

            var response = await controller.ValidateToken(model);

            Assert.IsType<UnauthorizedObjectResult>(response.Result);
        }
    }
}
