﻿using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Core.Application.Features.Customers.Queries.GetCustomersList;

public class GetCustomerListQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<GetCustomerListQuery, List<CustomerListViewModel>>
{
	public async Task<List<CustomerListViewModel>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
	{
		List<Customer> allCustomers = (await unitOfWork.Customer.GetAllAsync()).OrderBy(c => c.LastName).ToList();
		return mapper.Map<List<CustomerListViewModel>>(allCustomers);
	}
}