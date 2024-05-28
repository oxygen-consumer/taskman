namespace TaskmanAPI.Exceptions;

public class EntityNotFoundException(string message) : ArgumentException(message);