using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using System.IO;
using BilheticaAeronauticaWeb.Helper;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AvioesController : Controller
    {
       
        private readonly IAviaoRepository _aviaoRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public AvioesController(
            IAviaoRepository aviaoRepository,
            IImageHelper imageHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {
          
            _aviaoRepository = aviaoRepository;
            _imageHelper = imageHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
        }

        // GET: Avioes
        public IActionResult Index()
        {
            return View(_aviaoRepository.GetAll().OrderBy(p => p.Marca));
        }

        // GET: Avioes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _aviaoRepository.GetByIdAsync(id.Value);
            if (aviao == null)
            {
                return NotFound();
            }

            return View(aviao);
        }

        // GET: Avioes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Avioes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AvioesViewModel model )
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "avioes");
                }

                var aviao = _converterHelper.ToAviao(model, imageId, true);
                await _aviaoRepository.CreateAsync(aviao);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
       
       

     
        // GET: Avioes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _aviaoRepository.GetByIdAsync(id.Value);
            if (aviao == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToAvioesViewModel(aviao);
            return View(model);
        }

       



        // POST: Avioes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AvioesViewModel model)
        {
          

            if (ModelState.IsValid)
            {
                try
                {

                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "avioes");
                    }

                    var aviao = _converterHelper.ToAviao(model, imageId, false);
                    await _aviaoRepository.UpdateAsync(aviao);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aviaoRepository.ExistsAsync(model.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Avioes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _aviaoRepository.GetByIdAsync(id.Value);
            if (aviao == null)
            {
                return NotFound();
            }

            return View(aviao);
        }

        // POST: Avioes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aviao = await _aviaoRepository.GetByIdAsync(id);
            await _aviaoRepository.DeleteAsync(aviao);
            return RedirectToAction(nameof(Index));
        }

        private bool AviaoExists(int id)
        {
            return _aviaoRepository.GetAll().Any(e => e.Id == id);
        }
    }
}
