using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Commands.RemoveCustomer;

public class RemoveCustomerCommand : IRequest
{
	public int Id { get; set; }
}