using Microsoft.EntityFrameworkCore;
using ProductosHexagonal.Domain.Models;
using ProductosHexagonal.Domain.Ports.Outbound;
using ProductosHexagonal.Infrastructure.Data;

namespace ProductosHexagonal.Infrastructure.Adapters.Outbound
{
    public class SqlServerProductoRepository : IProductoRepository
    {
        private readonly ProductosDbContext _context;

        public SqlServerProductoRepository(ProductosDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> UpdateAsync(Producto producto)
        {
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            // Soft delete - solo desactivar
            producto.Desactivar();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Producto>> SearchByNameAsync(string nombre)
        {
            return await _context.Productos
                .Where(p => p.Activo && p.Nombre.Contains(nombre))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Productos
                .AnyAsync(p => p.Id == id);
        }
    }
}
