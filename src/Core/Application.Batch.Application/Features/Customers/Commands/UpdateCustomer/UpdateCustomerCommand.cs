using MediatR;

namespace Application.Batch.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest
{
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string SocialSecurityNumber { get; set; } = string.Empty;
}