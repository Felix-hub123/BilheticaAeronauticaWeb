using System.Collections.Generic;

namespace BilheticaAeronauticaWeb.Models
{
    public class HistoricoViewModel
    {
        public List<BilheteViewModel> Futuros { get; set; } = new();
        public List<BilheteViewModel> Passados { get; set; } = new();
    }
}
