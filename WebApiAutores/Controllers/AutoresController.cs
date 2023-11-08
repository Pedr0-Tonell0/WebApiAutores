using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AutoresController(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet ("{id:int}", Name = "obtenerAutor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor =  await _context.Autores
                .Include(libro => libro.AutoresLibros)
                .ThenInclude(autorLibro => autorLibro.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(autor == null){
                return NotFound();
            }
            return _mapper.Map<AutorDTOConLibros>(autor);
        }


        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await _context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            return _mapper.Map<List<AutorDTO>>(autores);

        }


        [HttpGet]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await _context.Autores.ToListAsync();
            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();
            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }


        [HttpPut ("{id:int}")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            _context.Autores.Update(autor);
           await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Autores.AnyAsync(x=>x.Id == id);
            if(!existe)
            {
                return NotFound();
            }

            _context.Autores.Remove(new Autor() { Id = id});
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
