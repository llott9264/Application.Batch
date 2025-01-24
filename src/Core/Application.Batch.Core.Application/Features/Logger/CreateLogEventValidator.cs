using FluentValidation;

namespace Application.Batch.Core.Application.Features.Logger;

public class CreateLogEventValidator : AbstractValidator<CreateLogEvent>
{
	public CreateLogEventValidator()
	{
		RuleFor(p => p.Message)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
	}
}