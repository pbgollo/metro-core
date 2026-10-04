using FluentValidation.TestHelper;
using Metro.Application.Auth.Validators;
using Metro.Application.Users.Validators;
using Metro.Domain.Auth;
using Shouldly;

namespace Metro.Application.Tests.Validators;

public class CommandValidatorTests
{
    [Fact]
    public void CreateUserCommand_WhenPasswordInvalid_HasError()
    {
        var validator = new CreateUserCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Users.Commands.CreateUserCommand
        {
            Name = "Ana",
            Email = "ana@example.com",
            Document = "123",
            Phone = "1199",
            Password = "123",
            Role = "client"
        });

        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(PasswordPolicy.RequirementsMessage);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("")]
    [InlineData("CLIENT")]
    public void CreateUserCommand_WhenRoleInvalid_HasError(string role)
    {
        var validator = new CreateUserCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Users.Commands.CreateUserCommand
        {
            Name = "Ana",
            Email = "ana@example.com",
            Document = "123",
            Phone = "1199",
            Password = "Secret123!",
            Role = role
        });

        result.ShouldHaveValidationErrorFor(command => command.Role)
            .WithErrorMessage("O perfil deve ser master ou client.");
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("")]
    [InlineData("CLIENT")]
    public void UpdateUserCommand_WhenRoleInvalid_HasError(string role)
    {
        var validator = new UpdateUserCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Users.Commands.UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Name = "Ana",
            Email = "ana@example.com",
            Document = "123",
            Phone = "1199",
            Role = role
        });

        result.ShouldHaveValidationErrorFor(command => command.Role)
            .WithErrorMessage("O perfil deve ser master ou client.");
    }

    [Fact]
    public void CreateUserCommand_WhenValid_HasNoErrors()
    {
        var validator = new CreateUserCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Users.Commands.CreateUserCommand
        {
            Name = "Ana",
            Email = "ana@example.com",
            Document = "123",
            Phone = "1199",
            Password = "Secret123!",
            Role = "client"
        });

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ResetPasswordCommand_WhenPasswordInvalid_HasError()
    {
        var validator = new ResetPasswordCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Auth.Commands.ResetPasswordCommand
        {
            Email = "ana@example.com",
            Code = "123456",
            NewPassword = "123"
        });

        result.ShouldHaveValidationErrorFor(command => command.NewPassword)
            .WithErrorMessage(PasswordPolicy.RequirementsMessage);
    }

    [Fact]
    public void LoginCommand_WhenEmailMissing_HasError()
    {
        var validator = new LoginCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Auth.Commands.LoginCommand
        {
            Email = "",
            Password = "Secret123!"
        });

        result.ShouldHaveValidationErrorFor(command => command.Email);
    }

    [Fact]
    public void RefreshTokenCommand_WhenMissing_HasError()
    {
        var validator = new RefreshTokenCommandValidator();
        var result = validator.TestValidate(new Metro.Application.Auth.Commands.RefreshTokenCommand
        {
            RefreshToken = " "
        });

        result.ShouldHaveValidationErrorFor(command => command.RefreshToken);
    }
}
