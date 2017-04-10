using Xunit;
using System;
using System.Linq.Expressions;
using QuizHealper;
namespace HelperTest
{
    public class HelperTest
    {
        [Fact]
        public void CanRetunBindingsAndValueWhenExpressionIsPassed()
        {
            var description = "testingDescription";
            var notes = "testingNotes";
            Expression<Func<DataEntity.Topic>> inputValue = () => new DataEntity.Topic {
                Description = description,
                Notes = notes
            };

            var result = Healper.ExtractBindingsAndValues(inputValue);
            Assert.True(result.Count == 2);
        }

        [Fact]
        public void CanRetunNullWhenExpressionHasEmptyMember()
        {
            Expression<Func<DataEntity.Topic>> inputValue = () => new DataEntity.Topic();

            var result = Healper.ExtractBindingsAndValues(inputValue);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CanThrowExceptionWhenPassedNullToExtractBindings()
        {
            Expression<Func<DataEntity.Topic>> nullInput = null;
            var ex = Assert.Throws<ArgumentException>( () => Healper.ExtractBindingsAndValues(nullInput));
            Assert.Equal( "Invalid Expression", ex.Message );
        }

        [Fact]
        public void CanReturnEmptywhenNullInitExpressionIsPassed()
        {
            Expression<Func<DataEntity.Topic>> nullInput = () => null;
            var actualResult =  Healper.ExtractBindingsAndValues(nullInput);
            Assert.True(actualResult.Count == 0);
        }
    }
}