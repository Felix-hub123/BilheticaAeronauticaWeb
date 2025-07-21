using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Authorize(Roles = "Cliente")]
    [ApiController]
    [Route("api/[controller]")]
    public class BilhetesController : Controller
    {

        private readonly IBilheteRepository _bilheteRepository;
        private readonly IUserHelper _userHelper;

        public BilhetesController(IBilheteRepository bilheteRepository, IUserHelper userHelper)
        {
            _bilheteRepository = bilheteRepository;
            _userHelper = userHelper;
        }

        [HttpGet("Futuros")]
        public async Task<IActionResult> GetVoosFuturos()
        {
            var user = await _userHelper.GetUserAsync(User);
            var bilhetesFuturos = await _bilheteRepository.GetBilhetesFuturosByUserAsync(user.Id);

            var lista = bilhetesFuturos.Select(b => new
            {
                BilheteId = b.Id,
                VooId = b.Voo.Id,
                NumeroVoo = b.Voo.Numero,
                Origem = b.Voo.Origem.Nome,
                Destino = b.Voo.Destino.Nome,
                DataPartida = b.Voo.DataHoraPartida,
                Lugar = b.Lugar.Codigo,
                Estado = b.WasDeleted ? "Anulado" : "Ativo"
            }).ToList();

            return Ok(lista);
        }
    }
}
