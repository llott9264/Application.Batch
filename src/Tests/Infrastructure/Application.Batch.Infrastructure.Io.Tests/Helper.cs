using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;

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