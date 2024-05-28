namespace TaskmanAPI.Exceptions;

public class InsufficientPrivilegesException(string message) : ArgumentException(message);