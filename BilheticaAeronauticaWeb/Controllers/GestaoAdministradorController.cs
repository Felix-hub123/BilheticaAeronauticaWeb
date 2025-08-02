using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{/// <summary>
 /// Controlador para gestão dos administradores da plataforma.
 /// Permite listar, criar, editar e eliminar utilizadores com role "Admin".
 /// Aplica regras de upload de imagem e usa UserManager para gestão segura de contas.
 /// </summary>
    public class GestaoAdministradorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;

        public GestaoAdministradorController(
            UserManager<User> userManager,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
        }

        /// <summary>
        /// Lista todos os administradores registados.
        /// </summary>
        /// <returns>View com a lista dos administradores.</returns>
        // GET: GestaoAdministradorController
        public async Task<ActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var model = admins.Select(a => _converterHelper.ToUserViewModel(a)).ToList();
            return View(model);
        }


        /// <summary>
        /// Mostra detalhes de um administrador específico.
        /// </summary>
        /// <param name="id">ID do utilizador administrador.</param>
        /// <returns>View com os detalhes ou NotFound se não existir.</returns>
        // GET: GestaoAdministradorController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null)
                return NotFound();

            var model = _converterHelper.ToUserViewModel(admin);
            return View(model);
        }


        /// <summary>
        /// Exibe o formulário para criação de um novo administrador.
        /// </summary>
        /// <returns>View para criar administrador.</returns>
        // GET: GestaoAdministradorController/Create
        public ActionResult Create()
        {
            return View();
        }




        /// <summary>
        /// Recebe dados do formulário para criar novo administrador.
        /// Faz upload da imagem e atribui o role "Admin" ao utilizador criado.
        /// </summary>
        /// <param name="model">ViewModel com dados do novo administrador.</param>
        /// <returns>Redireciona para Index em sucesso ou retorna ao formulário 
        // POST: GestaoAdministradorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _converterHelper.ToFuncionario(model, isNew: true);

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        user.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao criar o administrador. Tente novamente.");
            }
            return View(model);
        }




        /// <summary>
        /// Exibe o formulário para editar administrador existente.
        /// </summary>
        /// <param name="id">ID do administrador.</param>
        /// <returns>View para edição ou página de erro personalizada se não encontrado.</returns>
        // GET: GestaoAdministradorController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null)
            {
                return new NotFoundViewResult("AdministradorNotFound");
            }

            var model = _converterHelper.ToUserViewModel(admin);
            return View(model);
        }



        /// <summary>
        /// Recebe dados para atualizar informações do administrador.
        /// Faz upload da nova imagem se fornecida.
        /// </summary>
        /// <param name="model">ViewModel com dados atualizados.</param>
        /// <returns>Redireciona para index ou retorna view com erros.</returns>
        // POST: GestaoAdministradorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> Edit(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var admin = await _userManager.FindByIdAsync(model.Id);
                    if (admin == null)
                    {
                        return new NotFoundViewResult("AdministradorNotFound");
                    }

                    _converterHelper.UpdateFuncionarioFromViewModel(admin, model);

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        admin.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }
                    var result = await _userManager.UpdateAsync(admin);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao editar o administrador.");
                return View(model);
            }
        }


        /// <summary>
        /// Exibe confirmação para eliminar administrador.
        /// </summary>
        /// <param name="id">ID do administrador.</param>
        /// <returns>View de confirmação ou página de erro se não encontrado.</returns>
        // GET: GestaoAdministradorController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("AdministradorNotFound");
                }

                var admin = await _userManager.FindByIdAsync(id);
                if (admin == null)
                {
                    return new NotFoundViewResult("AdministradorNotFound");
                }

                var model = _converterHelper.ToUserViewModel(admin);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao carregar o administrador para eliminação.");
                return View("Error");
            }
        }


        /// <summary>
        /// Remove o administrador da base de dados.
        /// </summary>
        /// <param name="id">ID do administrador a eliminar.</param>
        /// <returns>Redireciona para index ou retorna view com erro.</returns>
        // POST: GestaoAdministradorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var admin = await _userManager.FindByIdAsync(id);
                if (admin != null)
                {
                    await _userManager.DeleteAsync(admin);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao eliminar o administrador.");
                var admin = await _userManager.FindByIdAsync(id);
                var model = admin != null ? _converterHelper.ToUserViewModel(admin) : null;
                return View("Delete", model);
            }
        }
    }
}
    

