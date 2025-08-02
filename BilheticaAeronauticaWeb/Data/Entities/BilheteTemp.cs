using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa uma reserva temporária de bilhete,
    /// contendo informações do passageiro, voo, lugar, preço unitário, quantidade,
    /// extras opcionais (bagagem extra, refeição), data de criação,
    /// indicações para exclusão lógica (soft delete) e identificadores de criação.
    /// Utilizado para operações intermediárias antes da confirmação definitiva do bilhete.
    /// </summary>
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
        public bool BagagemExtra { get; set; }
        public bool Refeicao { get; set; }

        public DateTime DataCriacao { get;  set; }

        public DateTime DataReserva { get;  set; }
    }
}
