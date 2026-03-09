using Xunit;
using QuestionService.Controllers;

namespace QuestionService.Tests;

public class BasicTests
{
    [Fact]
    public void TestExplorer_ShouldDiscoverThisTest()
    {
        // Arrange
        int expected = 42;
        
        // Act
        int actual = 21 * 2;
        
        // Assert
        Assert.Equal(expected, actual);
    }
}
