using FluentValidation;
using Metro.Application.Auth.Commands;
using Metro.Domain.Auth;

namespace Metro.Application.Auth.Validators
{
    public sealed class RequestPasswordRecoveryCommandValidator : AbstractValidator<RequestPasswordRecoveryCommand>
    {
        public RequestPasswordRecoveryCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");
        }
    }

    public sealed class VerifyPasswordRecoveryCodeCommandValidator : AbstractValidator<VerifyPasswordRecoveryCodeCommand>
    {
        public VerifyPasswordRecoveryCodeCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");

            RuleFor(command => command.Code)
                .NotEmpty().WithMessage("O código é obrigatório.")
                .Length(6).WithMessage("O código deve ter 6 dígitos.");
        }
    }

    public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");

            RuleFor(command => command.Code)
                .NotEmpty().WithMessage("O código é obrigatório.")
                .Length(6).WithMessage("O código deve ter 6 dígitos.");

            RuleFor(command => command.NewPassword)
                .NotEmpty().WithMessage("A nova senha é obrigatória.")
                .Must(PasswordPolicy.IsValid).WithMessage(PasswordPolicy.RequirementsMessage);
        }
    }
}
