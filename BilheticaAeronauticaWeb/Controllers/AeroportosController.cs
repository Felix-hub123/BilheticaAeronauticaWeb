using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AeroportosController : Controller
    {

        private readonly IAeroportoRepository _aeroportoRepository;

        public AeroportosController(IAeroportoRepository aeroportoRepository)
        {
            _aeroportoRepository = aeroportoRepository;
        }

        // GET: Aeroportos
        public async Task<IActionResult> Index()
        {
            return View(_aeroportoRepository.GetAll());
        }

        // GET: Aeroportos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return NotFound();
            }

            return View(aeroporto);
        }

        // GET: Aeroportos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aeroportos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aeroporto aeroporto)
        {
            if (ModelState.IsValid)
            {
                await _aeroportoRepository.CreateAsync(aeroporto);
                return RedirectToAction(nameof(Index));
            }
            return View(aeroporto);
        }

        // GET: Aeroportos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return NotFound();
            }
            return View(aeroporto);
        }

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Aeroporto aeroporto)
        {
            if (id != aeroporto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _aeroportoRepository.UpdateAsync(aeroporto);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aeroportoRepository.ExistsAsync(aeroporto.Id))
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
            return View(aeroporto);
        }

        // GET: Aeroportos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return NotFound();
            }

            return View(aeroporto);
        }

        // POST: Aeroportos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aeroporto = await _aeroportoRepository.GetByIdAsync(id);
            await _aeroportoRepository.DeleteAsync(aeroporto); 
            return RedirectToAction(nameof(Index));
        }


    }
}
