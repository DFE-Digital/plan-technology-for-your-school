using Dfe.PlanTech.Web.UnitTests.Models;
using Xunit;

namespace Dfe.PlanTech.Web.UnitTests.ViewComponents
{
    public class WarningTest
    {
        private readonly ComponentBuilder _compBuilder = new ComponentBuilder();
        private const string _value = "Test Value"; 
        private const string _nullValue = null;

        [Fact]
        public void WarningComponent_Text_Is_Populated()
        {
            var Warningtest = _compBuilder.Warning(_value);

            Assert.NotNull(Warningtest);
            Assert.Equal(_value, Warningtest.Text.RichText.Value);
        }

        [Fact]
        public void WarningComponent_Text_Is_Empty()
        {
            var Warningtest = _compBuilder.Warning(string.Empty);

            Assert.NotNull(Warningtest);
            Assert.Equal(string.Empty, Warningtest.Text.RichText.Value);
        }

        [Fact]
        public void WarningComponent_Text_Is_Null()
        {
            var Warningtest = _compBuilder.Warning(_nullValue);

            Assert.Null(Warningtest.Text.RichText.Value);
        }
    }
}
