using Application.Batch.Core.Application.Features.Customers.Commands.CreateCustomer;
using Application.Batch.Core.Application.Features.Customers.Commands.UpdateCustomer;
using Application.Batch.Core.Application.Features.Customers.Queries.GetCustomersList;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;

namespace Application.Batch.Core.Application.Features.Profiles;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Customer, CustomerListViewModel>().ReverseMap();
		CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
		CreateMap<Customer, UpdateCustomerCommand>().ReverseMap();
	}
}