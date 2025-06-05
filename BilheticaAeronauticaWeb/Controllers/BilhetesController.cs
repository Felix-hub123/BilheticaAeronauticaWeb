using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class BilhetesController : Controller
    {
        private readonly DataContext _context;

        public BilhetesController(DataContext context)
        {
            _context = context;
        }

        // GET: Bilhetes
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Bilhetes.Include(b => b.Lugar).Include(b => b.Passageiro).Include(b => b.User).Include(b => b.Voo);
            return View(await dataContext.ToListAsync());
        }

        // GET: Bilhetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bilhete = await _context.Bilhetes
                .Include(b => b.Lugar)
                .Include(b => b.Passageiro)
                .Include(b => b.User)
                .Include(b => b.Voo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bilhete == null)
            {
                return NotFound();
            }

            return View(bilhete);
        }

        // GET: Bilhetes/Create
        public IActionResult Create()
        {
            ViewData["LugarId"] = new SelectList(_context.Lugares, "Id", "Classe");
            ViewData["PassageiroId"] = new SelectList(_context.Passageiros, "Id", "Apelido");
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id");
            ViewData["VooId"] = new SelectList(_context.Voos, "Id", "NumeroVoo");
            return View();
        }

        // POST: Bilhetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tarifa,Classe,PodeAlterar,DataCompra,VooId,LugarId,UserId,PassageiroId")] Bilhete bilhete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bilhete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LugarId"] = new SelectList(_context.Lugares, "Id", "Classe", bilhete.LugarId);
            ViewData["PassageiroId"] = new SelectList(_context.Passageiros, "Id", "Apelido", bilhete.PassageiroId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", bilhete.UserId);
            ViewData["VooId"] = new SelectList(_context.Voos, "Id", "NumeroVoo", bilhete.VooId);
            return View(bilhete);
        }

        // GET: Bilhetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bilhete = await _context.Bilhetes.FindAsync(id);
            if (bilhete == null)
            {
                return NotFound();
            }
            ViewData["LugarId"] = new SelectList(_context.Lugares, "Id", "Classe", bilhete.LugarId);
            ViewData["PassageiroId"] = new SelectList(_context.Passageiros, "Id", "Apelido", bilhete.PassageiroId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", bilhete.UserId);
            ViewData["VooId"] = new SelectList(_context.Voos, "Id", "NumeroVoo", bilhete.VooId);
            return View(bilhete);
        }

        // POST: Bilhetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tarifa,Classe,PodeAlterar,DataCompra,VooId,LugarId,UserId,PassageiroId")] Bilhete bilhete)
        {
            if (id != bilhete.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bilhete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BilheteExists(bilhete.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LugarId"] = new SelectList(_context.Lugares, "Id", "Classe", bilhete.LugarId);
            ViewData["PassageiroId"] = new SelectList(_context.Passageiros, "Id", "Apelido", bilhete.PassageiroId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", bilhete.UserId);
            ViewData["VooId"] = new SelectList(_context.Voos, "Id", "NumeroVoo", bilhete.VooId);
            return View(bilhete);
        }

        // GET: Bilhetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bilhete = await _context.Bilhetes
                .Include(b => b.Lugar)
                .Include(b => b.Passageiro)
                .Include(b => b.User)
                .Include(b => b.Voo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bilhete == null)
            {
                return NotFound();
            }

            return View(bilhete);
        }

        // POST: Bilhetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bilhete = await _context.Bilhetes.FindAsync(id);
            _context.Bilhetes.Remove(bilhete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BilheteExists(int id)
        {
            return _context.Bilhetes.Any(e => e.Id == id);
        }
    }
}
