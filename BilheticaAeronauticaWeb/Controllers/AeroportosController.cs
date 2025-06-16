using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AeroportosController : Controller
    {
        
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public AeroportosController( IAeroportoRepository aeroportoRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {
           
            _aeroportoRepository = aeroportoRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
        }

        // GET: Aeroportos
        public IActionResult Index()
        {
            return View(_aeroportoRepository.GetAll().OrderBy(p => p.Cidade));
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
        public async Task<IActionResult> Create([Bind("Id,Nome,Cidade,Pais,IATA,ImageId,WasDeleted")] AeroportosViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {


                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");

                }

                var aeroporto = _converterHelper.ToAeroporto(model, imageId, true);

                await _aeroportoRepository.CreateAsync(aeroporto);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
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

            var model = _converterHelper.ToAeroportosViewModel(aeroporto);
            return View(model);
        }

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( AeroportosViewModel model)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {

                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");

                    }
                    var aeroporto = _converterHelper.ToAeroporto(model, imageId, false);
                    await _aeroportoRepository.UpdateAsync(aeroporto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aeroportoRepository.ExistsAsync(model.Id))
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
            return View(model);
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

        // Fix for CS4014: Added 'await' to the DeleteAsync call to ensure proper asynchronous execution.
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
