using System;

namespace BilheticaAeronauticaWeb.Models
{
    public class VooIndexViewModel
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public string OrigemNome { get; set; } = string.Empty;

        public string DestinoNome { get; set; } = string.Empty;

        public string OrigemImageUrl { get; set; } = string.Empty;

        public string DestinoImageUrl { get; set; } = string.Empty;

        public string AviaoModelo { get; set; } = string.Empty;

        public DateTime DataHoraPartida { get; set; }

        public DateTime DataHoraChegada { get; set; }
    }
}