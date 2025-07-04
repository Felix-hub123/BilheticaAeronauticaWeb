using System;

namespace BilheticaAeronauticaWeb.Models
{
    public class VooViewModel
    {
        public int Id { get; set; }
        public int OrigemId { get; set; }
        public int DestinoId { get; set; }
        public string OrigemNome { get; set; }
        public string DestinoNome { get; set; }
        public DateTime DataHoraPartida { get; set; }

        public DateTime DataHoraChegada { get; set; }

        public decimal TaxaAeroporto { get; set; }


        public string DisplayName
        {
            get
            {
                return $"{OrigemNome} - {DestinoNome} ({DataHoraPartida:g})";
            }
        }

    }
}
