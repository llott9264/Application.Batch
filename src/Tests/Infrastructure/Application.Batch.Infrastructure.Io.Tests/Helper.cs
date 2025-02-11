using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using AutoMapper;
using MediatR;
using Moq;

namespace Application.Batch.Infrastructure.Io.Tests;

public class Helper
{
	internal static Mock<IMediator> MockMediator()
	{
		{
			Mock<IMediator> mockMediator = new();
			mockMediator.Setup(static m => m.Send(It.IsAny<GetConfigurationByKeyQuery>(), CancellationToken.None))
				.ReturnsAsync("MyFolderPath\\");

			return mockMediator;
		}
	}

	internal static Mock<IMapper> MockMapper()
	{
		Mock<IMapper> mockMapper = new();
		return mockMapper;
	}
}