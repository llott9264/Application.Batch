using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using AutoMapper;

namespace Application.Batch.Core.Application.Tests.Mapper;

public class MapperTests
{
	[Fact]
	public void MappingProfile_ShouldHaveValidConfiguration()
	{
		// Arrange
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		AutoMapper.Mapper mapper = new(configuration);

		// Act & Assert
		mapper.ConfigurationProvider.AssertConfigurationIsValid();
	}

	[Fact]
	public void MappingProfile_ValidCustomerModel_ReturnsValidCustomerViewModel()
	{
		// Arrange
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		AutoMapper.Mapper mapper = new(configuration);

		// Act
		CustomerViewModel customerViewModel = mapper.Map<CustomerViewModel>((firstName: "John", lastName: "Doe", socialSecurityNumber: "123456789"));

		// Assert
		Assert.Equal("John", customerViewModel.FirstName);
		Assert.Equal("Doe", customerViewModel.LastName);
		Assert.Equal("123456789", customerViewModel.SocialSecurityNumber);
	}

	[Fact]
	public void MappingProfile_ValidRevokeModel_ReturnsValidRevokeViewModel()
	{
		// Arrange
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		AutoMapper.Mapper mapper = new(configuration);
		// Act
		RevokeViewModel revokeViewModel = mapper.Map<RevokeViewModel>((socialSecurityNumber: "123456789", isRevoked: "1"));
		// Assert
		Assert.Equal("123456789", revokeViewModel.SocialSecurityNumber);
		Assert.True(revokeViewModel.IsRevoked);
	}
}
