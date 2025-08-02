using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class PagamentoMbwayViewModel
    {
        /// <summary>
        /// ViewModel que representa os dados necessários para efetuar o pagamento de um bilhete,
        /// contendo informações do bilhete e dados do cartão para processamento do pagamento.
        /// </summary>
        public int VooId { get; set; }

        public int LugarId { get; set; }

        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O número de telemóvel MB WAY é obrigatório")]
        [Phone(ErrorMessage = "Insira um número de telemóvel válido")]

        public string NumeroTelemovel { get; set; }

        public bool BagagemExtra { get; set; }

        public bool Refeicao { get; set; }
    }
}
