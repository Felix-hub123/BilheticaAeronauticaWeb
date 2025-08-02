using System;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel utilizado para representar informações de erro,
    /// incluindo o identificador da requisição para rastreamento.
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
