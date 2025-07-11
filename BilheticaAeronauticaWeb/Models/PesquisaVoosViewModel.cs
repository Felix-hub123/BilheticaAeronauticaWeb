using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace BilheticaAeronauticaWeb.Models
{
    public class PesquisaVoosViewModel
    {
        public DateTime? DataPartida { get; set; }
        public int? OrigemId { get; set; }
        public int? DestinoId { get; set; }
        public List<SelectListItem> Aeroportos { get; set; }
        public List<Voo> Resultados { get; set; }
    }
}
