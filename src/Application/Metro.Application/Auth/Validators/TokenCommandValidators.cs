using FluentValidation;
using Metro.Application.Auth.Commands;

namespace Metro.Application.Auth.Validators
{
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(command => command.RefreshToken)
                .NotEmpty().WithMessage("O refresh token é obrigatório.");
        }
    }
}
