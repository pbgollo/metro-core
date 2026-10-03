using FluentValidation;
using Metro.Application.Auth.Commands;

namespace Metro.Application.Auth.Validators
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");

            RuleFor(command => command.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
