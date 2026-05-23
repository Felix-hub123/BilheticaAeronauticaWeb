using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um detalhe associado a um bilhete, como extras ou serviços adicionais,
    /// por exemplo bagagem extra ou refeição, contendo descrição, preço unitário, quantidade,
    /// valor calculado total e indicador de eliminação lógica (soft delete).
    /// </summary>
    public class BilheteDetail : IEntity
    {
        /// <summary>
        /// Identificador único do detalhe de custos do bilhete.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição legível do item ou extra faturado (Ex: "Bagagem extra 23kg", "Assento VIP").
        /// </summary>
        [Required(ErrorMessage = "A descrição do item detalhado é obrigatória.")]
        public string Descricao { get; set; }

        /// <summary>
        /// Preço unitário atribuído ao serviço complementar contratado.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Quantidade adquirida do item ou serviço em questão.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantidade { get; set; }

        /// <summary>
        /// Valor monetário total derivado da multiplicação do preço unitário pela quantidade.
        /// </summary>
        public decimal Valor => Preco * (decimal)Quantidade;

        /// <summary>
        /// Flag visual e de persistência que dita se o item extra foi anulado do carrinho ou bilhete.
        /// </summary>
        [Required]
        [Display(Name = "Foi Eliminado?")]
        public bool WasDeleted { get; set; }
    }
}
