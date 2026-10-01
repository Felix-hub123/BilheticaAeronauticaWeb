using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão dos aviões.
    /// </summary>
    public class AvioesController : Controller
    {
        private readonly IAviaoRepository _aviaoRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AvioesController(
            IAviaoRepository aviaoRepository,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _aviaoRepository = aviaoRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [AllowAnonymous]
        public IActionResult Index()
        {
            var avioes = _aviaoRepository
                .GetAll()
                .OrderBy(a => a.Marca)
                .ToList();

            var models = new List<AvioesViewModel>();

            foreach (var aviao in avioes)
            {
                var model =
                    _converterHelper.ToAvioesViewModel(aviao);

                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        aviao.ImageId,
                        "avioes",
                        "aviao/noimage");

                models.Add(model);
            }

            return View(models);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao =
                await _aviaoRepository
                    .GetByIdAsync(id.Value);

            if (aviao == null)
            {
                return NotFound();
            }

            var model =
                _converterHelper
                    .ToAvioesViewModel(aviao);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    aviao.ImageId,
                    "avioes",
                    "aviao/noimage");

            return View(model);
        }

        // =========================================================
        // CREATE
        // =========================================================

        [Authorize(Roles = "Funcionario,Admin")]
        public IActionResult Create()
        {
            return View(
                new AvioesViewModel
                {
                    Disponivel = true
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create(
            AvioesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Guid imageId = Guid.Empty;

            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                imageId =
                    await _imageHelper
                        .UploadImageAsync(
                            model.ImageFile,
                            "avioes");
            }

            var aviao =
                _converterHelper.ToAviao(
                    model,
                    imageId,
                    true);

            await _aviaoRepository
                .CreateAsync(aviao);

            TempData["Success"] =
                "Avião criado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // EDIT
        // =========================================================

        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao =
                await _aviaoRepository
                    .GetByIdAsync(id.Value);

            if (aviao == null)
            {
                return NotFound();
            }

            var model =
                _converterHelper
                    .ToAvioesViewModel(aviao);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    aviao.ImageId,
                    "avioes",
                    "aviao/noimage");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(
            AvioesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        model.ImageId,
                        "avioes",
                        "aviao/noimage");

                return View(model);
            }

            try
            {
                Guid imageId = model.ImageId;

                if (model.ImageFile != null &&
                    model.ImageFile.Length > 0)
                {
                    if (imageId != Guid.Empty)
                    {
                        await _imageHelper
                            .DeleteImageAsync(
                                imageId,
                                "avioes");
                    }

                    imageId =
                        await _imageHelper
                            .UploadImageAsync(
                                model.ImageFile,
                                "avioes");
                }

                var aviao =
                    _converterHelper.ToAviao(
                        model,
                        imageId,
                        false);

                await _aviaoRepository
                    .UpdateAsync(aviao);

                TempData["Success"] =
                    "Avião atualizado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _aviaoRepository
                    .ExistsAsync(model.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // DELETE
        // =========================================================

        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao =
                await _aviaoRepository
                    .GetByIdAsync(id.Value);

            if (aviao == null)
            {
                return NotFound();
            }

            var model =
                _converterHelper
                    .ToAvioesViewModel(aviao);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    aviao.ImageId,
                    "avioes",
                    "aviao/noimage");

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var aviao =
                await _aviaoRepository
                    .GetByIdAsync(id);

            if (aviao == null)
            {
                return NotFound();
            }

            if (aviao.ImageId != Guid.Empty)
            {
                await _imageHelper
                    .DeleteImageAsync(
                        aviao.ImageId,
                        "avioes");
            }

            await _aviaoRepository
                .DeleteAsync(aviao);

            TempData["Success"] =
                "Avião eliminado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
    }
}