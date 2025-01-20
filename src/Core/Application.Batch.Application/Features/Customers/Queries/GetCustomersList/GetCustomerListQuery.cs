using MediatR;

namespace Application.Batch.Application.Features.Customers.Queries.GetCustomersList
{
	public class GetCustomerListQuery : IRequest<List<CustomerListViewModel>>
	{
	}
}
