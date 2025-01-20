using Application.Batch.Application.Contracts.Persistence;
using FluentValidation;

namespace Application.Batch.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
	private readonly ICustomerRepository _customerRepository;
	public CreateCustomerCommandValidator(ICustomerRepository customerRepository)
	{
		_customerRepository = customerRepository;

		RuleFor(p => p.LastName)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull()
			.MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

		RuleFor(p => p.FirstName)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull()
			.MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

		RuleFor(p => p.SocialSecurityNumber)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull()
			.MinimumLength(9).WithMessage("{PropertyName} must not be less than 9 characters.")
			.MaximumLength(9).WithMessage("{PropertyName} must not be greater than 9 characters.");

		RuleFor(e=> e)
			.MustAsync(CustomerSsnUnique).WithMessage("A Customer with the same Social Security Number already exists.");
	}

	private async Task<bool> CustomerSsnUnique(CreateCustomerCommand e, CancellationToken token)
	{
		return !await _customerRepository.IsCustomerSocialSecurityNumberUnique(e.SocialSecurityNumber);
	}
}