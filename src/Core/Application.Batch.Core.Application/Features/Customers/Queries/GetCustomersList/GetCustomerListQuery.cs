using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Queries.GetCustomersList
{
	public class GetCustomerListQuery : IRequest<List<CustomerListViewModel>>
	{
	}
}
