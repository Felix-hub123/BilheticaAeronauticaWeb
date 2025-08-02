using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um detalhe associado a um bilhete, como extras ou serviços adicionais,
    /// por exemplo bagagem extra ou refeição, contendo descrição, preço unitário, quantidade,
    /// valor calculado total e indicador de eliminação lógica (soft delete).
    /// </summary>

    public class BilheteDetail : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Descricao { get; set; } // Ex: "Bagagem extra", "Refeição"

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Preco { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantidade { get; set; }

        public decimal Valor => Preco * (decimal)Quantidade;

        [Required]
        [Display(Name = "Foi Eliminado?")]
        public bool WasDeleted { get; set; }
    }
}
