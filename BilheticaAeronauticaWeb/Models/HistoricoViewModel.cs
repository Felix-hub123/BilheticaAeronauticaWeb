using System.Collections.Generic;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel que representa o histórico de bilhetes de um cliente,
    /// separando-os em bilhetes de voos futuros e bilhetes de voos passados.
    /// </summary>
    public class HistoricoViewModel
    {

      

        /// <summary>
        /// Lista de bilhetes correspondentes a voos futuros que ainda vão ocorrer.
        /// </summary>
        public List<BilheteViewModel> Futuros { get; set; } = new();

        /// <summary>
        /// Lista de bilhetes correspondentes a voos já realizados no passado.
        /// </summary>
        public List<BilheteViewModel> Passados { get; set; } = new();
    }
}
