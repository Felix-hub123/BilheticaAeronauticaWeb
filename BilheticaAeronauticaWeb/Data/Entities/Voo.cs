using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um voo agendado na aplicação, incluindo número do voo,
    /// aeroportos de origem e destino, avião associado, datas/hora de partida e chegada,
    /// preço base para o bilhete e lista de lugares disponíveis.
    /// Implementa soft delete para remoção lógica.
    /// </summary>
    public class Voo : IEntity
    {
        /// <summary>
        /// Identificador de índice sequencial interno para o voo planeado.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código identificador comercial visível do Voo (Ex: "TP1402", "FR2241").
        /// </summary>
        [Required(ErrorMessage = "O número de identificação comercial do voo é obrigatório.")]
        public string Numero { get; set; }

        /// <summary>
        /// Chave estrangeira que referencia o Aeroporto onde se inicia a rota do voo.
        /// </summary>
        [Required]
        public int OrigemId { get; set; }

        /// <summary>
        /// Instância de dados do Aeroporto de Origem do voo.
        /// </summary>
        public Aeroporto Origem { get; set; }

        /// <summary>
        /// Chave estrangeira que referencia o Aeroporto onde termina a rota do voo.
        /// </summary>
        [Required]
        public int DestinoId { get; set; }

        /// <summary>
        /// Instância de dados do Aeroporto de Destino de término do voo.
        /// </summary>
        public Aeroporto Destino { get; set; }

        /// <summary>
        /// Chave estrangeira que referencia a aeronave escalada para executar o percurso.
        /// </summary>
        [Required]
        public int AviaoId { get; set; }

        /// <summary>
        /// Instância de dados do avião designado para o voo corrente.
        /// </summary>
        public Aviao Aviao { get; set; }

        /// <summary>
        /// Data combinada com a hora precisa programada para a descolagem do avião.
        /// </summary>
        [Required(ErrorMessage = "A especificação da data e hora de partida é obrigatória.")]
        public DateTime DataHoraPartida { get; set; }

        /// <summary>
        /// Data combinada com a hora prevista calculada para a aterragem em destino.
        /// </summary>
        [Required(ErrorMessage = "A especificação da data e hora de chegada é obrigatória.")]
        public DateTime DataHoraChegada { get; set; }

        /// <summary>
        /// Custo tarifário inicial cru do bilhete neste voo, sem taxas ou extras acrescidos.
        /// </summary>
        [Required(ErrorMessage = "O preço de custo base do voo é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoBase { get; set; }

        /// <summary>
        /// Grade completa e pontual com o estado de ocupação de cada assento exclusivo deste voo.
        /// </summary>
        public ICollection<Lugar> Lugares { get; set; } = new List<Lugar>();

        /// <summary>
        /// Flag de exclusão lógica para preservação de integridade de dados históricos relacionais (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }
    }
   
}
