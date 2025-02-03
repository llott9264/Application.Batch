namespace Application.Batch.Core.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);