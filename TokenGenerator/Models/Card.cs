using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace TokenGenerator.Models
{
    /// <summary>
    /// The Card class models a credit card ownered by a customer
    /// </summary>
    public class Card
    {
        [Key]
        public int CardId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [RequiredIf("CardId == 0", ErrorMessage = "Card number must have between 1 to 16 digits")]
        [Range(1, 9999999999999999, ErrorMessage = "Card number must have between 1 to 16 digits")]
        public ulong? CardNumber { get; set; }

        [Required(ErrorMessage = "CVV must have between 1 to 5 digits")]
        [Range(1, 99999, ErrorMessage = "CVV must have between 1 to 5 digits")]
        public int? CVV { get; set; }

        public DateTime RegistrationDate { get; set; }

        private ulong? token;
        
        [NotMapped]
        public ulong? Token 
        {
            get
            {
                return token != null ? token : GenerateToken();
            }

            set 
            {
                token = value;
            }
        }

        public Card()
        {
            RegistrationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Generate and return a token for the informed credit card
        /// </summary>
        /// <returns>A nullable ulong with the generated token</returns>
        private ulong? GenerateToken()
        {
            if (CardNumber > 0 && CVV > 0)
            {
                int[] digits = CardNumber.ToString().ToCharArray().Select(x => (int)Char.GetNumericValue(x)).ToArray();

                if (digits.Length > 4)
                {
                    digits = new List<int>(digits).GetRange(digits.Length - 4, 4).ToArray();
                }

                for (int i = 0; i < CVV; i++)
                {
                    int j, last;

                    last = digits[^1];

                    for (j = digits.Length - 1; j > 0; j--)
                    {
                        digits[j] = digits[j - 1];
                    }

                    digits[0] = last;
                }

                var token = UInt64.Parse(string.Join("", digits));

                return token;
            }

            return null;
        }
    }
}
