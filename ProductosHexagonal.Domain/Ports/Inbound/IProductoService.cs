using ProductosHexagonal.Domain.Models;

namespace ProductosHexagonal.Domain.Ports.Inbound
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto> CrearAsync(Producto producto);
        Task<Producto> ActualizarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<Producto>> BuscarPorNombreAsync(string nombre);
        Task<bool> ActualizarStockAsync(int id, int cantidad);
    }
}