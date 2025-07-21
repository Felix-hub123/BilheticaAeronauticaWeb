using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    [CustomValidation(typeof(VooViewModelValidator), nameof(VooViewModelValidator.ValidarDatas))]

    public class VooViewModel
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        [Required(ErrorMessage = "Selecione a origem.")]
        public int OrigemId { get; set; }

        [Required(ErrorMessage = "Selecione o destino.")]
        public int DestinoId { get; set; }

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

      
        public string DisplayName =>
            $"{OrigemNome} - {DestinoNome} ({DataHoraPartida:g})";
    }

}

