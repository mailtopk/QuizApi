using Xunit;
using System;
using System.Linq.Expressions;
using QuizHelper;
using FluentAssertions;

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

            var result = Helper.ExtractBindingsAndValues(inputValue);
            Assert.True(result.Count == 2);
            result.Should().ContainKey("Description").WhichValue.ShouldBeEquivalentTo("testingDescription");
            result.Should().ContainKey("Notes").WhichValue.ShouldBeEquivalentTo("testingNotes");
        }

        [Fact]
        public void CanRetunNullWhenExpressionHasEmptyMember()
        {
            Expression<Func<DataEntity.Topic>> inputValue = () => new DataEntity.Topic();
            var result = Helper.ExtractBindingsAndValues(inputValue);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CanThrowExceptionWhenPassedNullToExtractBindings()
        {
            Expression<Func<DataEntity.Topic>> nullInput = null;
            var ex = Assert.Throws<ArgumentException>( () => Helper.ExtractBindingsAndValues(nullInput));
            ex.Message.Should().BeEquivalentTo("Invalid Expression");
        }

        [Fact]
        public void CanReturnEmptywhenNullInitExpressionIsPassed()
        {
            Expression<Func<DataEntity.Topic>> nullInput = () => null;
            var actualResult =  Helper.ExtractBindingsAndValues(nullInput);
            Assert.True(actualResult.Count == 0);
            actualResult.Should().BeEmpty();
        }

        [Fact]
        public void CanReturnPropertiesAndValueWhenConstantExpressionIsAnObjectInstance()
        {
            var input = new DataEntity.Topic {
                Id = "mockId",
                Description = "mockDescription"
            };
            var result = Helper.ExtractBindingsAndValues( 
                () => new DataEntity.Topic { Id = input.Id, Description = input.Description });
            
            result.Should().NotBeNull("Expected properties and values");
            result.Should().ContainKey("Id").WhichValue.ShouldBeEquivalentTo("mockId");
            result.Should().ContainKey("Description").WhichValue.ShouldBeEquivalentTo("mockDescription");
        }
    }
}