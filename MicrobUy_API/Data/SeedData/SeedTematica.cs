using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data.SeedData
{
    public static class SeedTematica
    {
        private static TenantAplicationDbContext _context;

        public static void RunSeedTematica(TenantAplicationDbContext context)
        {
            _context = context;

            if (!_context.Tematica.Any())
            {
                var tematicas = new List<TematicaModel>
                {
                new TematicaModel { Name = "Opiniones" },
                new TematicaModel { Name = "Reseñas" },
                new TematicaModel { Name = "Instructivos" },
                new TematicaModel { Name = "Tutoriales" },
                new TematicaModel { Name = "Guías para principiantes" },
                new TematicaModel { Name = "Recetas de cocina" },
                new TematicaModel { Name = "Noticias" },
                new TematicaModel { Name = "Consejos" },
                new TematicaModel { Name = "Viajes" },
                new TematicaModel { Name = "Fitness" },
                new TematicaModel { Name = "Defensa personal" },
                new TematicaModel { Name = "Proyectos de bricolaje" },
                new TematicaModel { Name = "Tutoriales de videojuegos" },
                new TematicaModel { Name = "Jardinería" },
                new TematicaModel { Name = "Fotografía" },
                new TematicaModel { Name = "Lista de sitios interesantes" },
                new TematicaModel { Name = "Aprendizaje de un instrumento" },
                new TematicaModel { Name = "Aprendizaje de idiomas" },
                new TematicaModel { Name = "Diseño web" },
                new TematicaModel { Name = "Diseño de interiores" },
                new TematicaModel { Name = "Deportes" },
                new TematicaModel { Name = "Animales" },
                new TematicaModel { Name = "Superación personal" },
                new TematicaModel { Name = "Autoempleo" },
                new TematicaModel { Name = "Hablar en público" },
                new TematicaModel { Name = "Estilo de vida económico" },
                new TematicaModel { Name = "Compartir historias" },
                new TematicaModel { Name = "Tecnología" },
                new TematicaModel { Name = "Política" },
                new TematicaModel { Name = "Deportes" },
            };

                context.AddRange(tematicas);
                context.SaveChanges();
            }
        }
    }
}
