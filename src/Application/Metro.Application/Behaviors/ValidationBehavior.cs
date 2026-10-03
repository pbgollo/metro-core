using System.Reflection;
using FluentValidation;
using MediatR;
using Metro.Application.Results;

namespace Metro.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(
                    _validators.Select(validator => validator.ValidateAsync(context, cancellationToken))))
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToList();

            if (failures.Count == 0)
            {
                return await next();
            }

            var message = string.Join(" ", failures.Select(failure => failure.ErrorMessage).Distinct());
            return CreateBadRequest(message);
        }

        private static TResponse CreateBadRequest(string message)
        {
            var responseType = typeof(TResponse);
            if (!responseType.IsGenericType
                || responseType.GetGenericTypeDefinition() != typeof(ApiResult<>))
            {
                throw new ValidationException(message);
            }

            var badRequest = responseType.GetMethod(
                nameof(ApiResult<object>.BadRequest),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (badRequest is null)
            {
                throw new InvalidOperationException(
                    $"Could not create BadRequest result for {responseType.Name}.");
            }

            return (TResponse)badRequest.Invoke(null, [message])!;
        }
    }
}
