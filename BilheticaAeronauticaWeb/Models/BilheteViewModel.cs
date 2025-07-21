using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class BilheteViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Voo")]
        public int VooId { get; set; }
        public string VooNumero { get; set; }


        [Required]
        [Display(Name = "Passageiro")]
        public int PassageiroId { get; set; }

        public string PassageiroNome { get; set; }

        public string OrigemNome { get; set; }
        public string DestinoNome { get; set; }


        [Required]
        [Display(Name = "Lugar")]
        public int LugarId { get; set; }
        public string LugarCodigo { get; set; } = string.Empty;

        [Display(Name = "Data de Compra")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataCompra { get; set; }

        [Display(Name = "Valor Pago")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Valor { get; set; }


        public string NomeCliente { get; set; }


        [Display(Name = "Bagagem Extra")]
        public bool BagagemExtra { get; set; }

        public bool Anulado { get; set; }


        [Display(Name = "Refeição")]
        public bool Refeicao { get; set; }

        public bool WasDeleted { get; set; }

        [Display(Name = "Data Partida")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataPartida { get; set; }

        public IEnumerable<SelectListItem> Voos { get; set; }
        public IEnumerable<SelectListItem> Passageiros { get; set; }
        public IEnumerable<SelectListItem> Lugares { get; set; }
    }

}
