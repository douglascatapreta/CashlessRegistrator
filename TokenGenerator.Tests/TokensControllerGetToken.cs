using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenGenerator.Controllers;
using TokenGenerator.Models;
using Xunit;

namespace TokenGenerator.Tests
{
    public class TokensControllerGetToken
    {
        [Fact]
        public async void GetTokenSendingValidCard()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("CashlessRegistratorGT")
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

            Assert.IsType<CreatedAtActionResult> (response.Result);
        }

        [Fact]
        public async void GetErrorSendingInvalidCard()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("CashlessRegistratorGT")
                .Options;

            var context = new ApiContext(options);

            var controller = new TokensController(context);

            var response = await controller.GetToken(null);

            Assert.IsType<BadRequestResult>(response.Result);
        }
    }
}
