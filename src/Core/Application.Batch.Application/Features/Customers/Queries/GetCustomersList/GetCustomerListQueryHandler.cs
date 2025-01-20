using Application.Batch.Application.Contracts.Persistence;
using Application.Batch.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Application.Features.Customers.Queries.GetCustomersList;

public class GetCustomerListQueryHandler(IMapper mapper, IRepository<Customer> customerRepository) : IRequestHandler<GetCustomerListQuery, List<CustomerListViewModel>>
{
	public async Task<List<CustomerListViewModel>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
	{
		List<Customer> allCustomers = (await customerRepository.GetAllAsync()).OrderBy(c => c.LastName).ToList();
		return mapper.Map<List<CustomerListViewModel>>(allCustomers);
	}
}