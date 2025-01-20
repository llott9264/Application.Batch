using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Commands.RemoveCustomer;

public class RemoveCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository) : IRequestHandler<RemoveCustomerCommand>
{
	public async Task Handle(RemoveCustomerCommand request, CancellationToken cancellationToken)
	{
		Customer? customerToDelete = await customerRepository.GetByIdAsync(request.Id);
		await customerRepository.RemoveAsync(customerToDelete);
	}
}