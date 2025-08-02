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


        /// <summary>
        /// Lista todos os aeroportos, disponível publicamente para consulta.
        /// </summary>
        /// <returns>View com lista de aeroportos ordenada por cidade.</returns>
        // GET: Aeroportos
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var aeroportos = _aeroportoRepository.GetAll().OrderBy(p => p.Cidade).ToList();

            foreach (var aeroporto in aeroportos)
            {
                aeroporto.FoiUsadoEmVoos = await _aeroportoRepository.TemVoosAssociadosAsync(aeroporto.Id);
            }

            return View(aeroportos);
        }


        /// <summary>
        /// Mostra detalhes de um aeroporto específico.
        /// </summary>
        /// <param name="id">ID do aeroporto a consultar.</param>
        /// <returns>View dos detalhes ou página de erro personalizado se não encontrado.</returns>
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



        /// <summary>
        /// Exibe formulário para criar novo aeroporto.
        /// Apenas acessível a funcionários e administradores.
        /// </summary>
        /// <returns>View para criação de aeroporto.</returns>
        // GET: Aeroportos/Create
        [Authorize(Roles = "Funcionario,Admin")]
        public IActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// Regista novo aeroporto com upload de imagem obrigatório.
        /// </summary>
        /// <param name="model">Dados do aeroporto a criar.</param>
        /// <returns>Redireciona para lista ou retorna ao formulário em caso de erro.</returns>
        /// 
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



        /// <summary>
        /// Exibe formulário para edição de aeroporto existente.
        /// Acesso restrito a funcionários e administradores.
        /// </summary>
        /// <param name="id">ID do aeroporto a editar.</param>
        /// <returns>View de edição ou página de erro personalizado se não encontrado.</returns>
        // GET: Aeroportos/Edit/5
        [HttpGet]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return new NotFoundViewResult("AeroportoNotFound");

            var aeroporto = await _aeroportoRepository.GetByIdAsync(id.Value);
            if (aeroporto == null)
                return new NotFoundViewResult("AeroportoNotFound");

            bool temVoos = await _aeroportoRepository.TemVoosAssociadosAsync(id.Value);
            if (temVoos)
            {
                TempData["Error"] = "Este aeroporto já foi utilizado em um voo e não pode mais ser editado.";
                return RedirectToAction(nameof(Index));
            }

            var model = _converterHelper.ToAeroportosViewModel(aeroporto);
            return View(model);
        }


        /// <summary>
        /// Atualiza dados do aeroporto após edição.
        /// Inclui tratamento de concorrência e upload de nova imagem opcional.
        /// </summary>
        /// <param name="model">Dados atualizados do aeroporto.</param>
        /// <returns>Redireciona para lista ou retorna ao formulário com erros.</returns>

        // POST: Aeroportos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(AeroportosViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _aeroportoRepository.TemVoosAssociadosAsync(model.Id))
            {
                ModelState.AddModelError(string.Empty, "Este aeroporto não pode ser editado porque está associado a um voo ativo.");
                return View(model);
            }

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
                    return new NotFoundViewResult("AeroportoNotFound");
                else
                    throw;
            }

            TempData["Success"] = "Aeroporto atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }




        /// <summary>
        /// Obtém a taxa padrão associada a um aeroporto para uso em cálculos.
        /// </summary>
        /// <param name="id">ID do aeroporto.</param>
        /// <returns>Taxa padrão como JSON, ou zero se aeroporto não existir.</returns>

        [HttpGet]
        public async Task<IActionResult> ObterTaxa(int id)
        {
            var aeroporto = await _aeroportoRepository.GetByIdAsync(id); // usa o método do repositório!
            if (aeroporto == null)
                return Json(new { taxa = 0 });

            // Garante que o campo existe e não é nulo
            return Json(new { taxa = aeroporto.TaxaAeroportoPadrao });


        }



        /// <summary>
        /// Remove aeroporto permanentemente da base de dados.
        /// Delete explícito e controlado, evitando erros de cascata.
        /// </summary>
        /// <param name="id">ID do aeroporto a eliminar.</param>
        /// <returns>Redireciona para a lista após remoção.</returns>

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

            var model = _converterHelper.ToAeroportosViewModel(aeroporto);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aeroporto = await _aeroportoRepository.GetByIdAsync(id);
            if (aeroporto == null)
                return new NotFoundViewResult("AeroportoNotFound");

            // Verifica se há voos associados (ativos)
            if (await _aeroportoRepository.TemVoosAssociadosAsync(id))
            {
                ModelState.AddModelError(string.Empty, "Não é possível apagar este aeroporto porque ele está associado a um voo ativo.");
                var model = _converterHelper.ToAeroportosViewModel(aeroporto);
                return View("Delete", model);
            }

            try
            {
                await _aeroportoRepository.DeleteAeroportoComValidacaoAsync(aeroporto);
            }
            catch (InvalidOperationException ex)
            {
                
                ModelState.AddModelError(string.Empty, ex.Message);
                var model = _converterHelper.ToAeroportosViewModel(aeroporto);
                return View("Delete", model);
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Erro de concorrência ao tentar apagar o aeroporto. Tente novamente.");
                var model = _converterHelper.ToAeroportosViewModel(aeroporto);
                return View("Delete", model);
            }

            TempData["Success"] = "Aeroporto eliminado com sucesso.";
            return RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// Página amigável apresentada quando o aeroporto não é encontrado.
        /// </summary>
        /// <returns>View personalizada de erro not found.</returns>
        public IActionResult AeroportoNotFound()
       {
            return View(); 
       }
    }
}