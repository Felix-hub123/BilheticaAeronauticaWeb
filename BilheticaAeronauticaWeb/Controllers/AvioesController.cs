using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{

    /// <summary>
    /// Controller para gerir os aviões/aparelhos (CRUD).
    /// Acesso restrito a funcionários e administradores para alterações.
    /// </summary>
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
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_aviaoRepository.GetAll().OrderBy(p => p.Marca));
        }

        // GET: Avioes/Details/5
        [AllowAnonymous]
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
        [Authorize(Roles = "Funcionario,Admin")]
        public IActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// Cria um novo avião com upload de imagem obrigatório para cumprimento do requisito visual.
        /// </summary>
        /// 
        // POST: Avioes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
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



        /// <summary>
        /// Atualiza um avião, tratando concorrência e upload opcional de nova imagem.
        /// </summary>
        // GET: Avioes/Edit/5
        [Authorize(Roles = "Funcionario,Admin")]
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
        [Authorize(Roles = "Funcionario,Admin")]
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


        /// <summary>
        /// Confirma e efetua remoção do avião.
        /// </summary>
        // GET: Avioes/Delete/5
        [Authorize(Roles = "Funcionario,Admin")]
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
        [Authorize(Roles = "Funcionario,Admin")]
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
