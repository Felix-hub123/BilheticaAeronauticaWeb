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
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AeroportosController : Controller
    {
        
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public AeroportosController( IAeroportoRepository aeroportoRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
           
            _aeroportoRepository = aeroportoRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Aeroportos
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_aeroportoRepository.GetAll().OrderBy(p => p.Cidade));
        }

        // GET: Aeroportos/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return  new NotFoundViewResult("AeroportoNotFound");
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create( AeroportosViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {


                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "aeroportos");

                }

                var aeroporto = _converterHelper.ToAeroporto(model, imageId, true);

                await _aeroportoRepository.CreateAsync(aeroporto);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Aeroportos/Edit/5
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var model = _converterHelper.ToAeroportosViewModel(aeroporto);
            return View(model);
        }

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit( AeroportosViewModel model)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {

                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "aeroportos");

                    }
                    var aeroporto = _converterHelper.ToAeroporto(model, imageId, false);
                    await _aeroportoRepository.UpdateAsync(aeroporto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aeroportoRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("AeroportoNotFound");
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
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
            }

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
            {
                return new NotFoundViewResult("AeroportoNotFound");
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

       public IActionResult AeroportoNotFound()
       {
            return View(); 
       }
    }
}
