using Application.Batch.Application.Contracts.Persistence;
using Application.Batch.Application.Exceptions;
using Application.Batch.Domain.Entities;
using AutoMapper;
using FluentValidation.Results;
using MediatR;

namespace Application.Batch.Application.Features.Customers.Commands.CreateCustomer;

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