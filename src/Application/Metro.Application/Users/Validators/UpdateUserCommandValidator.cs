using FluentValidation;
using Metro.Application.Users.Commands;
using Metro.Domain.Auth;

namespace Metro.Application.Users.Validators
{
    public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("O id do usuário é obrigatório.");

            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.")
                .MaximumLength(256).WithMessage("O e-mail deve ter no máximo 256 caracteres.");

            RuleFor(command => command.Document)
                .NotEmpty().WithMessage("O documento é obrigatório.")
                .MaximumLength(50).WithMessage("O documento deve ter no máximo 50 caracteres.");

            RuleFor(command => command.Phone)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .MaximumLength(30).WithMessage("O telefone deve ter no máximo 30 caracteres.");

            RuleFor(command => command.Password)
                .Must(password => string.IsNullOrWhiteSpace(password) || PasswordPolicy.IsValid(password))
                .WithMessage(PasswordPolicy.RequirementsMessage);

            RuleFor(command => command.Role)
                .Must(role => role is "master" or "client")
                .WithMessage("O perfil deve ser master ou client.");
        }
    }
}
