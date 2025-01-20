﻿using Application.Batch.Application.Contracts.Persistence;
using Application.Batch.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository) : IRequestHandler<UpdateCustomerCommand>
{
	public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
	{
		Customer? customerToUpdate = await customerRepository.GetByIdAsync(request.Id);
		mapper.Map<UpdateCustomerCommand, Customer>(request, customerToUpdate);
		await customerRepository.UpdateAsync(customerToUpdate);
	}
}