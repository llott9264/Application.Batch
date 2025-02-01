using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using AutoMapper;

namespace Application.Batch.Core.Application.Features.Utilities.Mapper;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<(string, string, string), CustomerViewModel>()
			.ForMember(d => d.FirstName,
				opt => opt.MapFrom(s => s.Item1))
			.ForMember(d => d.LastName,
				opt => opt.MapFrom(s => s.Item2))
			.ForMember(d => d.SocialSecurityNumber,
				opt => opt.MapFrom(s => s.Item3));

		CreateMap<(string, string), RevokeViewModel>()
			.ForMember(d => d.SocialSecurityNumber,
				opt => opt.MapFrom(s => s.Item1))
			.ForMember(d => d.IsRevoked,
				opt => opt.MapFrom(s => s.Item2 == "1"));
	}
}