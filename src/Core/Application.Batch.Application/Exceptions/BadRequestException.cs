namespace Application.Batch.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);