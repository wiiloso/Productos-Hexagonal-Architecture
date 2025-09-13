using Microsoft.AspNetCore.Mvc;
using ProductosHexagonal.Application.DTOs;
using ProductosHexagonal.Domain.Exceptions;
using ProductosHexagonal.Domain.Models;
using ProductosHexagonal.Domain.Ports.Inbound;

namespace ProductosHexagonal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            IProductoService productoService,
            ILogger<ProductosController> logger)
        {
            _productoService = productoService ?? 
                throw new ArgumentNullException(nameof(productoService));
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los productos");
                var productos = await _productoService.ObtenerTodosAsync();
                var productosDto = productos.Select(MapToDto);
                return Ok(productosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo producto con ID: {Id}", id);
                var producto = await _productoService.ObtenerPorIdAsync(id);
                
                if (producto == null)
                    return NotFound(new { error = $"Producto con ID {id} no encontrado" });

                return Ok(MapToDto(producto));
            }
            catch (ProductoNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto no encontrado");
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo producto: {Nombre}", createDto.Nombre);
                
                var producto = new Producto
                {
                    Nombre = createDto.Nombre,
                    Descripcion = createDto.Descripcion,
                    Precio = createDto.Precio,
                    Stock = createDto.Stock
                };

                var productoCreado = await _productoService.CrearAsync(producto);
                var productoDto = MapToDto(productoCreado);

                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = productoDto.Id }, 
                    productoDto);
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos de producto inválidos");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> Update(int id, [FromBody] UpdateProductoDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest(new { error = "El ID de la ruta no coincide con el ID del cuerpo" });

                _logger.LogInformation("Actualizando producto con ID: {Id}", id);

                var producto = new Producto
                {
                    Id = updateDto.Id,
                    Nombre = updateDto.Nombre,
                    Descripcion = updateDto.Descripcion,
                    Precio = updateDto.Precio,
                    Stock = updateDto.Stock,
                    Activo = updateDto.Activo
                };

                var productoActualizado = await _productoService.ActualizarAsync(producto);
                return Ok(MapToDto(productoActualizado));
            }
            catch (ProductoNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto no encontrado");
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos de producto inválidos");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina (desactiva) un producto
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando producto con ID: {Id}", id);
                var resultado = await _productoService.EliminarAsync(id);
                
                if (resultado)
                    return NoContent();
                
                return NotFound(new { error = $"Producto con ID {id} no encontrado" });
            }
            catch (ProductoNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto no encontrado");
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Busca productos por nombre
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> SearchByName([FromQuery] string nombre)
        {
            try
            {
                _logger.LogInformation("Buscando productos con nombre: {Nombre}", nombre);
                var productos = await _productoService.BuscarPorNombreAsync(nombre);
                var productosDto = productos.Select(MapToDto);
                return Ok(productosDto);
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos de búsqueda inválidos");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos por nombre: {Nombre}", nombre);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        [HttpPatch("{id}/stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            try
            {
                if (id != stockDto.ProductoId)
                    return BadRequest(new { error = "El ID de la ruta no coincide con el ID del cuerpo" });

                _logger.LogInformation("Actualizando stock del producto {Id} en {Cantidad}", id, stockDto.Cantidad);
                var resultado = await _productoService.ActualizarStockAsync(id, stockDto.Cantidad);
                
                if (resultado)
                    return Ok(new { message = "Stock actualizado correctamente" });
                
                return BadRequest(new { error = "No se pudo actualizar el stock" });
            }
            catch (ProductoNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto no encontrado");
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidProductoDataException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos para actualizar stock");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar stock del producto {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        private static ProductoDto MapToDto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                FechaCreacion = producto.FechaCreacion,
                Activo = producto.Activo
            };
        }
    }
}