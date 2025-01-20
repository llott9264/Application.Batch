using MediatR;

namespace Application.Batch.Application.Features.Customers.Commands.RemoveCustomer;

public class RemoveCustomerCommand : IRequest
{
	public int Id { get; set; }
}