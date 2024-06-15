namespace TaskmanAPI.Exceptions;

public class InvalidEntityStateException(string message) : ArgumentException(message);