using FluentValidation;
using Metro.Application.Users.Queries;

namespace Metro.Application.Users.Validators
{
    public sealed class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(query => query.Id)
                .NotEmpty().WithMessage("O id do usuário é obrigatório.");
        }
    }

    public sealed class ListUserQueryValidator : AbstractValidator<ListUserQuery>
    {
        public ListUserQueryValidator()
        {
            RuleFor(query => query.Page)
                .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}
