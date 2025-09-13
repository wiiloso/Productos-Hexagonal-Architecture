using ProductosHexagonal.Domain.Models;
using ProductosHexagonal.Domain.Ports.Inbound;
using ProductosHexagonal.Domain.Ports.Outbound;
using ProductosHexagonal.Domain.Exceptions;

namespace ProductosHexagonal.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository ?? 
                throw new ArgumentNullException(nameof(productoRepository));
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            try
            {
                return await _productoRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new DomainException("Error al obtener todos los productos", ex);
            }
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductoDataException("El ID debe ser mayor a cero");

            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);
                if (producto == null)
                    throw new ProductoNotFoundException(id);
                
                return producto;
            }
            catch (ProductoNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException($"Error al obtener el producto con ID {id}", ex);
            }
        }

        public async Task<Producto> CrearAsync(Producto producto)
        {
            ValidarProducto(producto);

            try
            {
                return await _productoRepository.CreateAsync(producto);
            }
            catch (Exception ex)
            {
                throw new DomainException("Error al crear el producto", ex);
            }
        }

        public async Task<Producto> ActualizarAsync(Producto producto)
        {
            if (producto.Id <= 0)
                throw new InvalidProductoDataException("El ID debe ser mayor a cero");

            ValidarProducto(producto);

            try
            {
                var existe = await _productoRepository.ExistsAsync(producto.Id);
                if (!existe)
                    throw new ProductoNotFoundException(producto.Id);

                return await _productoRepository.UpdateAsync(producto);
            }
            catch (ProductoNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException($"Error al actualizar el producto con ID {producto.Id}", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductoDataException("El ID debe ser mayor a cero");

            try
            {
                var existe = await _productoRepository.ExistsAsync(id);
                if (!existe)
                    throw new ProductoNotFoundException(id);

                return await _productoRepository.DeleteAsync(id);
            }
            catch (ProductoNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException($"Error al eliminar el producto con ID {id}", ex);
            }
        }

        public async Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new InvalidProductoDataException("El nombre de búsqueda no puede estar vacío");

            try
            {
                return await _productoRepository.SearchByNameAsync(nombre);
            }
            catch (Exception ex)
            {
                throw new DomainException($"Error al buscar productos por nombre: {nombre}", ex);
            }
        }

        public async Task<bool> ActualizarStockAsync(int id, int cantidad)
        {
            if (id <= 0)
                throw new InvalidProductoDataException("El ID debe ser mayor a cero");

            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);
                if (producto == null)
                    throw new ProductoNotFoundException(id);

                producto.ActualizarStock(cantidad);
                await _productoRepository.UpdateAsync(producto);
                return true;
            }
            catch (ProductoNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidProductoDataException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DomainException($"Error al actualizar stock del producto con ID {id}", ex);
            }
        }

        private void ValidarProducto(Producto producto)
        {
            if (producto == null)
                throw new InvalidProductoDataException("El producto no puede ser nulo");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new InvalidProductoDataException("El nombre del producto es requerido");

            if (producto.Nombre.Length > 100)
                throw new InvalidProductoDataException("El nombre no puede exceder los 100 caracteres");

            if (producto.Precio <= 0)
                throw new InvalidProductoDataException("El precio debe ser mayor a cero");

            if (producto.Stock < 0)
                throw new InvalidProductoDataException("El stock no puede ser negativo");

            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                producto.Descripcion = "Sin descripción";
        }
    }
}
