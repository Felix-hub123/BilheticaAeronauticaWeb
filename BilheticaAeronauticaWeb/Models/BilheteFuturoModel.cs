using System;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// Modelo usado para representar informações detalhadas de um bilhete relacionado a um voo futuro.
    /// Contém dados essenciais para identificação do bilhete, detalhes do voo, lugar reservado e estado da reserva.
    /// </summary>
    public class BilheteFuturoModel
    {
        public int BilheteId { get; set; }
        public int VooId { get; set; }
        public string NumeroVoo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataPartida { get; set; }
        public string Lugar { get; set; }
        public string Estado { get; set; }
    }
}
