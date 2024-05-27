using RecAll.Infrastructure.Ddd.Domain.Exceptions;

namespace RecAll.Core.List.Domain.Exceptions;

public class ListDomainException : DomainException {
    public ListDomainException() { }

    public ListDomainException(string message) : base(message) { }

    public ListDomainException(string message, Exception innerException) : base(
        message, innerException) { }
}