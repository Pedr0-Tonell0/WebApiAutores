using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await _context.Libros.Include(libro => libro.AutoresLibros)
                                              .ThenInclude(autorLibro => autorLibro.Autor)
                                              .Include(x => x.Comentarios).FirstOrDefaultAsync(x => x.Id == id);
                if (libro == null)
            {
                return NotFound();
            }
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return _mapper.Map<LibroDTOConAutores>(libro);

        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if(libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            var autores = await _context.Autores.Where(autores => libroCreacionDTO.AutoresIds.Contains(autores.Id)).Select(x => x.Id).ToListAsync();
            if(libroCreacionDTO.AutoresIds.Count != autores.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }
            var libro = _mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libro = await _context.Libros.Include(libro => libro.AutoresLibros)
                                  .ThenInclude(autorLibro => autorLibro.Autor)
                                  .Include(x => x.Comentarios).FirstOrDefaultAsync(x => x.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            libro = _mapper.Map(libroCreacionDTO, libro);
            AsignarOrdenAutores(libro);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {

            if (patchDocument == null)
            {
                return NotFound();
            }
            var libro = await _context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            var libroDTO = _mapper.Map<LibroPatchDTO>(libro);
            patchDocument.ApplyTo(libroDTO, ModelState);
            var esValido = TryValidateModel(libroDTO);
            if(!esValido)
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(libroDTO, libro);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _context.Libros.Remove(new Libro() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
