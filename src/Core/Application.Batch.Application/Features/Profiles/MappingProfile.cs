using Application.Batch.Application.Features.Customers.Commands.CreateCustomer;
using Application.Batch.Application.Features.Customers.Commands.UpdateCustomer;
using Application.Batch.Application.Features.Customers.Queries.GetCustomersList;
using Application.Batch.Domain.Entities;
using AutoMapper;

namespace Application.Batch.Application.Features.Profiles;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Customer, CustomerListViewModel>().ReverseMap();
		CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
		CreateMap<Customer, UpdateCustomerCommand>().ReverseMap();
	}
}