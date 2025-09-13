namespace ProductosHexagonal.Domain.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }

        public Producto()
        {
            FechaCreacion = DateTime.Now;
            Activo = true;
        }

        // Métodos de dominio
        public void ActualizarStock(int cantidad)
        {
            if (Stock + cantidad < 0)
                throw new InvalidOperationException("El stock no puede ser negativo");
            Stock += cantidad;
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero");
            Precio = nuevoPrecio;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public void Activar()
        {
            Activo = true;
        }
    }
}
