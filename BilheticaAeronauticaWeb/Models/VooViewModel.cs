using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel que representa os dados usados para a criação e edição de voos.
    /// Inclui propriedades para identificar origem, destino, avião, datas do voo e preço base.
    /// Aplica uma validação customizada para garantir coerência nas datas de partida e chegada.
    /// </summary>
    [CustomValidation(typeof(VooViewModelValidator), nameof(VooViewModelValidator.ValidarDatas))]

    public class VooViewModel
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        [Display(Name = "Origem")]
        [Required(ErrorMessage = "Selecione a origem.")]
        public int OrigemId { get; set; }


        [Display(Name = "Destino")]
        [Required(ErrorMessage = "Selecione o destino.")]
        public int DestinoId { get; set; }


        [Display(Name = "Avião")]
        [Required(ErrorMessage = "Selecione o avião.")]
        public int AviaoId { get; set; }        

        public string OrigemNome { get; set; }
        public string DestinoNome { get; set; }

        [Required(ErrorMessage = "Indique a data/hora de partida.")]
        [DataType(DataType.DateTime)]
        public DateTime DataHoraPartida { get; set; }

        [Required(ErrorMessage = "Indique a data/hora de chegada.")]
        [DataType(DataType.DateTime)]
        public DateTime DataHoraChegada { get; set; }

        [Required(ErrorMessage = "Indique o preço base do voo.")]
        [Range(1, 99999, ErrorMessage = "Introduza um valor de preço válido.")]
        public decimal PrecoBase { get; set; }

        /// <summary>
        /// Texto formatado para exibição resumida do voo, incluindo origem, destino e data/hora de partida.
        /// </summary>
        public string DisplayName =>
            $"{OrigemNome} - {DestinoNome} ({DataHoraPartida:g})";
    }

}

