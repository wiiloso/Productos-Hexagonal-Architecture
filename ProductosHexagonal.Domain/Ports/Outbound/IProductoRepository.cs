using ProductosHexagonal.Domain.Models;

namespace ProductosHexagonal.Domain.Ports.Outbound
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Producto>> SearchByNameAsync(string nombre);
        Task<bool> ExistsAsync(int id);
    }
}