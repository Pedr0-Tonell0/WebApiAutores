﻿using AutoMapper;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<AutorCreacionDTO, Autor>();

            CreateMap<Autor, AutorDTO>();

            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autor => autor.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros)); 

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>();

            CreateMap<Libro, LibroPatchDTO>().ReverseMap();

            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libro => libro.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

            CreateMap<ComentarioCreacionDTO, Comentario>();

            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreacionDTO.AutoresIds == null) { return resultado; }
            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }
            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTOConAutores libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibros == null) { return resultado; }
            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO() { 
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }
            return resultado;
        }
        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTOConLibros AutorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutoresLibros == null) { return resultado; }
            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return resultado;

        }
    }
}
