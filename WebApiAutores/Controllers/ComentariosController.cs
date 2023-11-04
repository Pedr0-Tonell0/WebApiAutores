using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/libros/{libroId:int}/comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var libro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if (!libro)
            {
                return NotFound();
            }
            var comentarios = await _context.Comentarios.Where(x => x.LibroId == libroId).ToListAsync();
            return _mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet ("{id:int}", Name = "ObtenerComentarioPorId")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }
            return _mapper.Map<ComentarioDTO>(comentario);
        }


        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {

            var libro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if (!libro)
            {
                return NotFound();
            }
            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();
            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentarioPorId", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, ComentarioCreacionDTO comentarioCreacionDTO, int id)
        {
            var libro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if (!libro)
            {
                return NotFound();
            }
            var existeComentario = await _context.Comentarios.AnyAsync(x => x.Id == id);
            if (!existeComentario)
            {
                return NotFound();
            }
            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            _context.Comentarios.Update(comentario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
