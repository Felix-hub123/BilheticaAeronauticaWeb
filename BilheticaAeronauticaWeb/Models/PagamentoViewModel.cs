using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class PagamentoViewModel
    {
        public int BilheteId { get; set; }
        public decimal Valor { get; set; }
        [Required]
        public string NumeroCartao { get; set; }
        [Required]
        public string Validade { get; set; }
        [Required]
        public string CVV { get; set; }
    }
}
