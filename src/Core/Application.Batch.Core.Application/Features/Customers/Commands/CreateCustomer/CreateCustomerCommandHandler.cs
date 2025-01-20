using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Exceptions;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using FluentValidation.Results;
using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository) : IRequestHandler<CreateCustomerCommand, int>
{
	public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
	{
		Customer? customer = mapper.Map<Customer>(request);

		CreateCustomerCommandValidator validator = new(customerRepository);
		ValidationResult? validationResult = await validator.ValidateAsync(request, cancellationToken);

		if (validationResult.Errors.Count > 0)
		{
			throw new ValidationException(validationResult);
		}

		customer = await customerRepository.AddAsync(customer);
		return customer.Id;
	}
}