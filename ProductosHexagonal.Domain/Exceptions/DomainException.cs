namespace ProductosHexagonal.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    public class ProductoNotFoundException : DomainException
    {
        public ProductoNotFoundException(int id) 
            : base($"Producto con ID {id} no encontrado") { }
    }

    public class InvalidProductoDataException : DomainException
    {
        public InvalidProductoDataException(string message) 
            : base($"Datos de producto inválidos: {message}") { }
    }
}