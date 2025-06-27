using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace BilheticaAeronauticaWeb.Models
{
    public class BilheteViewModel
    {
        public int Id { get; set; }
        public int VooId { get; set; }
        public int LugarId { get; set; }
        public int PassageiroId { get; set; }
        public DateTime? DataCompra { get; set; }
        public decimal Preco { get; set; }
        public IEnumerable<SelectListItem> Voos { get; set; }
        public IEnumerable<SelectListItem> Passageiros { get; set; }
        public IEnumerable<SelectListItem> Lugares { get; set; } // Added property to fix CS1061  
    }
}
