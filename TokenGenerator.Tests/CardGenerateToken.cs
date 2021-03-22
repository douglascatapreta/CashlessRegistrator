using TokenGenerator.Models;
using Xunit;

namespace TokenGenerator.Tests
{
    public class CardGenerateToken
    {
        [Theory]
        [InlineData((ulong)4062874567753448, 957, (ulong)8344)]
        [InlineData((ulong)4564132378904650, 233, (ulong)465)]
        [InlineData((ulong)4997855246479002, 175, (ulong)29)]
        [InlineData((ulong)45641323, 15732, (ulong)1323)]
        [InlineData((ulong)176, 3, (ulong)176)]
        [InlineData((ulong)7, 44972, (ulong)7)]
        public void GenerateTokenWithValidCardNumberAndCVV(ulong? cardNumber, int? cvv, ulong? expected)
        {
            var card = new Card
            {
                CardNumber = cardNumber,
                CVV = cvv
            };

            var actual = card.Token;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ulong)0000000000000000, 333, null)]
        [InlineData((ulong)7764258960448874, 0, null)]
        [InlineData((ulong)0000000000000000, 0, null)]
        [InlineData(null, 123, null)]
        [InlineData((ulong)123, null, null)]
        [InlineData(null, null, null)]
        public void ReturnNullWhemGenerateTokenWithInvalidCardNumberAndCVV(ulong? cardNumber, int? cvv, ulong? expected)
        {
            var card = new Card
            {
                CardNumber = cardNumber,
                CVV = cvv
            };

            var actual = card.Token;

            Assert.Equal(expected, actual);
        }
    }
}
