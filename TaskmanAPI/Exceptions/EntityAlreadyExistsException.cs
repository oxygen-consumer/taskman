namespace TaskmanAPI.Exceptions;

public class EntityAlreadyExistsException(string message) : ArgumentException(message);