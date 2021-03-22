using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TokenGenerator.Models;

namespace TokenGenerator.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ApiContext _context;

        public TokensController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a token number for the informed card
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /tokens
        ///     {
        ///        "customerId": 1,
        ///        "cardNumber": 1234567812345678,
        ///        "cvv": 123
        ///     }
        ///
        /// </remarks>
        /// <param name="card">The card to be generated the token</param>
        /// <returns>A newly created token</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Card>> GetToken(Card card)
        {
            if (card == null)
            {
                return BadRequest();
            }

            try
            {
                var customer = await _context.Customers.FindAsync(card.CustomerId);

                if (customer == null)
                {
                    //return NotFound();

                    customer = new Customer
                    {
                        CustomerId = card.CustomerId
                    };
                    
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                var savedCard = _context.Cards.Where(c => c.CardNumber == card.CardNumber && c.CVV == card.CVV && c.CustomerId == card.CustomerId).FirstOrDefault();

                if (savedCard == null)
                {
                    card.Customer = customer;
                    _context.Cards.Add(card);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetToken), new { id = card.CardId }, new
                    {
                        registrationDate = card.RegistrationDate,
                        token = card.Token,
                        cardId = card.CardId
                    });
                }
                else
                {
                    savedCard.RegistrationDate = DateTime.UtcNow;
                    _context.Update(savedCard);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetToken), new { id = savedCard.CardId }, new
                    {
                        registrationDate = savedCard.RegistrationDate,
                        token = savedCard.Token,
                        cardId = savedCard.CardId
                    });
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        /// <summary>
        /// Validate the token for a informed card
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /tokens
        ///     {
        ///        "customerId": 1,
        ///        "cardId": 1,
        ///        "token": 8419,
        ///        "cvv": 123
        ///     }
        ///
        /// </remarks>
        /// <param name="card">The card to be validated</param>
        /// <returns>The validation result</returns>
        /// <response code="202">Returns if the token is valid</response>
        /// <response code="401">Returns if the token is invalid</response>
        /// <response code="400">If the item is null</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Card>> ValidateToken(Card card)
        {
            if (card == null)
            {
                return BadRequest();
            }

            try
            {
                var savedCard = await _context.Cards.FindAsync(card.CardId);

                if (savedCard == null || !TokenIsValid(savedCard, card))
                {
                    return Unauthorized(new { Validated = false });
                }

                Console.WriteLine(savedCard.CardNumber);

                return Accepted(new { Validated = true });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        private bool TokenIsValid(Card requestCard, Card savedCard)
        {
            return
                savedCard.RegistrationDate > DateTime.UtcNow.AddMinutes(-30) &&
                savedCard.CustomerId ==  requestCard.CustomerId &&
                savedCard.CVV == requestCard.CVV &&
                savedCard.Token == requestCard.Token;
        }
    }
}
