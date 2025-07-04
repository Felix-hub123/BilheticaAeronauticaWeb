using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class BilheteTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        public int PassageiroId { get; set; }
        public Passageiro Passageiro { get; set; }

        [Required]
        public int VooId { get; set; }
        public Voo Voo { get; set; }

        [Required]
        public int LugarId { get; set; }
        public Lugar Lugar { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Preco { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantidade { get; set; }

        public string CriadoPorUserId { get; set; }

        public decimal Valor => Preco * (decimal)Quantidade;

        [Required]
        [Display(Name = "Foi Eliminado?")]
        public bool WasDeleted { get; set; }
        public bool BagagemExtra { get; internal set; }
        public bool Refeicao { get; internal set; }
        public DateTime DataCriacao { get; internal set; }
    }
}
