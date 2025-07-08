using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WebApiPrueba.Context;
using WebApiPrueba.Models;

namespace WebApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesPsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesPsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ClientesPs
        [HttpGet]
        public async Task<ActionResult<PagedResult<ClientesPs>>> GetClientes([FromQuery] ClientesPsQueryParams queryParams)
        {
            IQueryable<ClientesPs> clientesQuery = _context.ClientesPs;

            // 1. Aplicar Filtros
            if (!string.IsNullOrWhiteSpace(queryParams.Nombres))
            {
                // Busca en Nombres o Apellidos
                clientesQuery = clientesQuery.Where(c =>
                    c.Nombres.Contains(queryParams.Nombres) ||
                    c.Apellidos.Contains(queryParams.Nombres)
                );
            }

            if (!string.IsNullOrWhiteSpace(queryParams.NumId))
            {
                clientesQuery = clientesQuery.Where(c => c.NumId.Contains(queryParams.NumId));
            }

            if (queryParams.Estado.HasValue)
            {
                clientesQuery = clientesQuery.Where(c => c.Estado == queryParams.Estado.Value);
            }
            else
            {
                // Por defecto, solo mostrar clientes activos si no se especifica el estado
                clientesQuery = clientesQuery.Where(c => c.Estado == true);
            }


            // 2. Obtener el Total de Registros (antes de paginar)
            var totalCount = await clientesQuery.CountAsync();

            // 3. Aplicar Ordenamiento
            if (!string.IsNullOrWhiteSpace(queryParams.SortField))
            {
                // Para ordenamiento dinámico:
                var parameter = Expression.Parameter(typeof(ClientesPs), "x");
                PropertyInfo? property = typeof(ClientesPs).GetProperty(queryParams.SortField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string method = queryParams.SortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), method,
                                        new Type[] { typeof(ClientesPs), property.PropertyType },
                                        clientesQuery.Expression, Expression.Quote(orderByExp));
                    clientesQuery = clientesQuery.Provider.CreateQuery<ClientesPs>(resultExp);
                }
                else
                {
                    // Si el campo de ordenamiento no es válido, usa un orden por defecto
                    clientesQuery = clientesQuery.OrderBy(c => c.Id);
                }
            }
            else
            {
                // Orden por defecto si no se especifica campo
                clientesQuery = clientesQuery.OrderBy(c => c.Id);
            }


            // 4. Aplicar Paginación
            var items = await clientesQuery
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // 5. Construir el resultado paginado
            var pagedResult = new PagedResult<ClientesPs>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };

            return Ok(pagedResult);
        }

        // GET: api/ClientesPs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientesPs>> GetClientesPs(int id)
        {
            var clientesPs = await _context.ClientesPs.FindAsync(id);

            if (clientesPs == null)
            {
                return NotFound();
            }

            return clientesPs;
        }

        // PUT: api/ClientesPs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientesPs(int id, ClientesPs clientesPs)
        {
            if (id != clientesPs.Id)
            {
                return BadRequest();
            }

            _context.Entry(clientesPs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientesPsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClientesPs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClientesPs>> PostClientesPs(ClientesPs clientesPs)
        {
            _context.ClientesPs.Add(clientesPs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientesPs", new { id = clientesPs.Id }, clientesPs);
        }

        // DELETE: api/ClientesPs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientesPs(int id)
        {
            var clientesPs = await _context.ClientesPs.FindAsync(id);
            if (clientesPs == null)
            {
                return NotFound();
            }

            _context.ClientesPs.Remove(clientesPs);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientesPsExists(int id)
        {
            return _context.ClientesPs.Any(e => e.Id == id);
        }

        [HttpGet("Exists/{numId}")] // Ejemplo de URL: GET api/ClientesPs/Exists/12345
        public async Task<ActionResult<bool>> CheckNumIdExists(string numId)
        {
            if (string.IsNullOrWhiteSpace(numId))
            {
                return BadRequest("El número de identificación no puede estar vacío.");
            }

            // Verifica si existe algún cliente con el NumId proporcionado en tu base de datos
            // Asegúrate de que 'NumId' sea el nombre correcto de la propiedad en tu modelo ClienteP de C#
            var exists = await _context.ClientesPs.AnyAsync(c => c.NumId == numId);

            return Ok(exists); // Retorna true si existe, false si no
        }

        [HttpPut("SoftDelete/{id}")]
        public async Task<IActionResult> SoftDeleteCliente(int id)
        {
            var cliente = await _context.ClientesPs.FindAsync(id);

            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado.");
            }

            // Asegúrate de que 'Estado' sea el nombre correcto de la propiedad en tu modelo ClienteP de C#
            // Y que el tipo de dato sea compatible (bool o int)
            cliente.Estado = false; // O cliente.Estado = 0; si tu campo es int
            cliente.FechaModificacion = DateTime.Now; // O DateTime.UtcNow, si manejas UTC

            try
            {
                _context.Entry(cliente).State = EntityState.Modified; // Marca el objeto como modificado
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientesPsExists(id))
                {
                    return NotFound($"Cliente con ID {id} no encontrado durante la actualización.");
                }
                else
                {
                    throw; // Relanza la excepción si es otro tipo de error de concurrencia
                }
            }
            catch (Exception ex)
            {
                // Loguea el error para depuración
                Console.WriteLine($"Error al realizar soft delete del cliente {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al intentar realizar soft delete.");
            }

            return NoContent(); // Retorna 204 No Content para indicar éxito sin cuerpo de respuesta
        }

        [HttpPut("Activate/{id}")]
        public async Task<IActionResult> ActivateCliente(int id)
        {
            var cliente = await _context.ClientesPs.FindAsync(id);

            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado.");
            }

            cliente.Estado = true; // Establece el estado a activo
            cliente.FechaModificacion = DateTime.Now; // Actualiza la fecha de modificación

            try
            {
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientesPsExists(id))
                {
                    return NotFound($"Cliente con ID {id} no encontrado durante la activación.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar activar el cliente {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al intentar activar el cliente.");
            }

            return NoContent();
        }
    }
}
