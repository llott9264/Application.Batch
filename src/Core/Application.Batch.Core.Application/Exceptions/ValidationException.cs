using FluentValidation.Results;

namespace Application.Batch.Core.Application.Exceptions;

public class ValidationException : Exception
{
	public List<string> ValidationErrors { get; set; }

	public ValidationException(ValidationResult validationResult)
	{
		ValidationErrors = [];

		foreach (ValidationFailure? validationError in validationResult.Errors)
		{
			ValidationErrors.Add(validationError.ErrorMessage);
		}
	}
}
