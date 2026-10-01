using Metro.Domain.Users.Authentication;
using Shouldly;

namespace Metro.Domain.Tests.Users.Authentication;

public class PasswordPolicyTests
{
    [Theory]
    [InlineData("Abcdef1")]
    [InlineData("abcdefgh")]
    [InlineData("ABCDEFGH")]
    [InlineData("12345678")]
    [InlineData("Abcdefgh")]
    [InlineData("ABCD1234")]
    [InlineData("abcd1234")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_WhenPasswordDoesNotMeetRequirements_ReturnsFalse(string? password)
    {
        PasswordPolicy.IsValid(password).ShouldBeFalse();
    }

    [Theory]
    [InlineData("Abcdefg1")]
    [InlineData("Secret123")]
    [InlineData("NovaSenha1")]
    public void IsValid_WhenPasswordMeetsRequirements_ReturnsTrue(string password)
    {
        PasswordPolicy.IsValid(password).ShouldBeTrue();
    }
}
