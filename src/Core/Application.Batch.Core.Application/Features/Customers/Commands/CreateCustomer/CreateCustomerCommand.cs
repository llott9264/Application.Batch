using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<int>
{
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string SocialSecurityNumber { get; set; } = string.Empty;
}