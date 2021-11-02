using Xunit;
using learn_and_code;
using System;

namespace learn_and_code.Tests
{
    public class UnitTests
    {
        public int Add(int y, int x)
        {
            return y + x;
        }
        public bool IsOdd(int x)
        {
            return x % 2 == 1;
        }

        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        [Fact]
        public void IsValidCard()
        {
            var testCard = new Card(Card.FacetValue.One, Card.FacetValue.Green, Card.FacetValue.Outlined, Card.FacetValue.Oval);
            var testCardFacetValues = testCard.FacetValues();

            Console.WriteLine("These are the Facet Values: " + testCardFacetValues);
            Assert.True(testCard.IsValid());
        }

        [Theory]
        [InlineData("")]
        [InlineData("card")]
        [InlineData("12222")]
        [InlineData("2222")]
        [InlineData("2121")]
        [InlineData("1231")]
        public void IsValidCardInput(string cardInput)
        {
            Assert.False(cardInput.Length != 4);
            Assert.True(Int32.TryParse(cardInput, out int x));
        }
    }
}
