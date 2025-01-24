using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand>
{
	public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
	{
		Customer? customerToUpdate = await unitOfWork.Customer.GetByIdAsync(request.Id);
		mapper.Map<UpdateCustomerCommand, Customer>(request, customerToUpdate);
		await unitOfWork.Customer.UpdateAsync(customerToUpdate);
	}
}