using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjetsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET /projets → retourne tous les projets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Projet>>> GetProjets()
        {
            return await _context.Projets.ToListAsync();
        }

        // ✅ POST /projets → ajoute un nouveau projet
        [HttpPost]
        public async Task<ActionResult<Projet>> PostProjet(Projet projet)
        {
            _context.Projets.Add(projet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProjet), new { id = projet.Id }, projet);
        }

        // ✅ GET /projets/5 → retourne un seul projet par id
        [HttpGet("{id}")]
        public async Task<ActionResult<Projet>> GetProjet(int id)
        {
            var projet = await _context.Projets.FindAsync(id);
            if (projet == null)
                return NotFound();
            return projet;
        }
    }
}
