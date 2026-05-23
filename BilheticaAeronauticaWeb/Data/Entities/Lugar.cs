using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um lugar/assento em um avião, com código identificador, preço base, disponibilidade,
    /// associação ao avião e opcionalmente a um voo específico.
    /// Inclui propriedade para soft delete para remoção lógica do registo.
    /// </summary>
    public class Lugar : IEntity
    {
        /// <summary>
        /// Identificador único sequencial do assento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código alfanumérico identificador da fileira e posição do assento (Ex: "14A", "02C").
        /// </summary>
        [Required(ErrorMessage = "O código do assento é obrigatório.")]
        [MaxLength(10, ErrorMessage = "O código do assento não pode exceder 10 caracteres.")]
        public string Codigo { get; set; }

        /// <summary>
        /// Acréscimo financeiro ou preço tarifário base para ocupação deste assento específico.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoBase { get; set; } = 100.00M;

        /// <summary>
        /// Dita se o lugar se encontra vago para venda ou livre de reservas no contexto do avião/voo.
        /// </summary>
        public bool Disponivel { get; set; } = true;

        /// <summary>
        /// Chave estrangeira de ligação física ao avião ao qual o assento pertence por configuração de fábrica.
        /// </summary>
        public int AviaoId { get; set; }

        /// <summary>
        /// Propriedade de navegação para a aeronave proprietária deste assento.
        /// </summary>
        public Aviao Aviao { get; set; }

        /// <summary>
        /// Chave estrangeira opcional usada caso o assento mude dinamicamente de estado em um voo em particular.
        /// </summary>
        public int? VooId { get; set; }

        /// <summary>
        /// Propriedade de navegação para obter o Voo associado a este assento.
        /// </summary>
        public Voo Voo { get; set; }

        /// <summary>
        /// Flag que indica se o assento foi removido das configurações lógicas (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }
    }
}
