using FluentValidation;
using Metro.Application.Users.Commands;

namespace Metro.Application.Users.Validators
{
    public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("O id do usuário é obrigatório.");
        }
    }
}
