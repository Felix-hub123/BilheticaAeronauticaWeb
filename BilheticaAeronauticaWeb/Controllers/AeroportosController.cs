using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AeroportosController : Controller
    {
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public AeroportosController(
            IAeroportoRepository aeroportoRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _aeroportoRepository = aeroportoRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Aeroportos
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var aeroportos = _aeroportoRepository
                .GetAll()
                .OrderBy(p => p.Cidade)
                .ToList();

            foreach (var aeroporto in aeroportos)
            {
                aeroporto.FoiUsadoEmVoos =
                    await _aeroportoRepository
                        .TemVoosAssociadosAsync(aeroporto.Id);
            }

            return View(aeroportos);
        }

        // GET: Aeroportos/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            var aeroporto =
                await _aeroportoRepository
                    .GetByIdAsync(id.Value);

            if (aeroporto == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            return View(aeroporto);
        }

        // GET: Aeroportos/Create
        [Authorize(Roles = "Funcionario,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aeroportos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create(
            AeroportosViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null &&
                    model.ImageFile.Length > 0)
                {
                    imageId =
                        await _imageHelper.UploadImageAsync(
                            model.ImageFile,
                            "aeroportos");
                }

                var aeroporto =
                    _converterHelper.ToAeroporto(
                        model,
                        imageId,
                        true);

                await _aeroportoRepository
                    .CreateAsync(aeroporto);

                TempData["Success"] =
                    "Aeroporto criado com sucesso.";

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Aeroportos/Edit/5
        [HttpGet]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            var aeroporto =
                await _aeroportoRepository
                    .GetByIdAsync(id.Value);

            if (aeroporto == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            bool temVoos =
                await _aeroportoRepository
                    .TemVoosAssociadosAsync(id.Value);

            if (temVoos)
            {
                TempData["Error"] =
                    "Este aeroporto já foi utilizado em um voo e não pode mais ser editado.";

                return RedirectToAction(nameof(Index));
            }

            var model =
                _converterHelper
                    .ToAeroportosViewModel(aeroporto);

            return View(model);
        }

        // POST: Aeroportos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(
            AeroportosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _aeroportoRepository
                .TemVoosAssociadosAsync(model.Id))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Este aeroporto não pode ser editado porque está associado a um voo ativo.");

                return View(model);
            }

            try
            {
                Guid imageId = model.ImageId;

                if (model.ImageFile != null &&
                    model.ImageFile.Length > 0)
                {
                    // Elimina a imagem antiga
                    if (imageId != Guid.Empty)
                    {
                        await _imageHelper.DeleteImageAsync(
                            imageId,
                            "aeroportos");
                    }

                    // Faz upload da nova imagem
                    imageId =
                        await _imageHelper.UploadImageAsync(
                            model.ImageFile,
                            "aeroportos");
                }

                var aeroporto =
                    _converterHelper.ToAeroporto(
                        model,
                        imageId,
                        false);

                await _aeroportoRepository
                    .UpdateAsync(aeroporto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _aeroportoRepository
                    .ExistsAsync(model.Id))
                {
                    return new NotFoundViewResult(
                        "AeroportoNotFound");
                }

                throw;
            }

            TempData["Success"] =
                "Aeroporto atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTaxa(int id)
        {
            var aeroporto =
                await _aeroportoRepository
                    .GetByIdAsync(id);

            if (aeroporto == null)
            {
                return Json(new { taxa = 0 });
            }

            return Json(new
            {
                taxa = aeroporto.TaxaAeroportoPadrao
            });
        }

        // GET: Aeroportos/Delete/5
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            var aeroporto =
                await _aeroportoRepository
                    .GetByIdAsync(id.Value);

            if (aeroporto == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            var model =
                _converterHelper
                    .ToAeroportosViewModel(aeroporto);

            return View(model);
        }

        // POST: Aeroportos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var aeroporto =
                await _aeroportoRepository
                    .GetByIdAsync(id);

            if (aeroporto == null)
            {
                return new NotFoundViewResult(
                    "AeroportoNotFound");
            }

            if (await _aeroportoRepository
                .TemVoosAssociadosAsync(id))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não é possível apagar este aeroporto porque ele está associado a um voo ativo.");

                var model =
                    _converterHelper
                        .ToAeroportosViewModel(aeroporto);

                return View("Delete", model);
            }

            try
            {
                // Apaga a imagem antes de eliminar
                if (aeroporto.ImageId != Guid.Empty)
                {
                    await _imageHelper.DeleteImageAsync(
                        aeroporto.ImageId,
                        "aeroportos");
                }

                await _aeroportoRepository
                    .DeleteAeroportoComValidacaoAsync(
                        aeroporto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                var model =
                    _converterHelper
                        .ToAeroportosViewModel(aeroporto);

                return View("Delete", model);
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Erro de concorrência ao tentar apagar o aeroporto. Tente novamente.");

                var model =
                    _converterHelper
                        .ToAeroportosViewModel(aeroporto);

                return View("Delete", model);
            }

            TempData["Success"] =
                "Aeroporto eliminado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AeroportoNotFound()
        {
            return View();
        }
    }
}