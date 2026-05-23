using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// <summary>
        /// Identificador único da linha de reserva temporária no carrinho/checkout.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira do passageiro associado à intenção de compra.
        /// </summary>
        [Required]
        public int PassageiroId { get; set; }

        /// <summary>
        /// Objeto de navegação contendo os dados do Passageiro em retenção de checkout.
        /// </summary>
        public Passageiro Passageiro { get; set; }

        /// <summary>
        /// Chave estrangeira do voo selecionado na reserva em progresso.
        /// </summary>
        [Required]
        public int VooId { get; set; }

        /// <summary>
        /// Objeto de navegação contendo a instância de dados do Voo.
        /// </summary>
        public Voo Voo { get; set; }

        /// <summary>
        /// Chave estrangeira do assento que se encontra bloqueado temporariamente no fluxo de checkout.
        /// </summary>
        [Required]
        public int LugarId { get; set; }

        /// <summary>
        /// Objeto de navegação contendo dados detalhados do assento.
        /// </summary>
        public Lugar Lugar { get; set; }

        /// <summary>
        /// Preço corrente de tabela associado ao bilhete no instante em que foi adicionado ao carrinho.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Quantidade de assentos iguais agrupados nesta intenção de compra.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantidade { get; set; }

        /// <summary>
        /// Código do utilizador em sessão responsável pela criação do processo temporário.
        /// </summary>
        public string CriadoPorUserId { get; set; }

        /// <summary>
        /// Custo acumulado total da reserva provisória.
        /// </summary>
        public decimal Valor => Preco * (decimal)Quantidade;

        /// <summary>
        /// Identifica se a intenção de reserva foi descartada ou expirada pelo sistema por timeout.
        /// </summary>
        [Required]
        [Display(Name = "Foi Eliminado?")]
        public bool WasDeleted { get; set; }

        /// <summary>
        /// Flag provisória indicando se foi pedida bagagem extra para esta futura emissão.
        /// </summary>
        public bool BagagemExtra { get; set; }

        /// <summary>
        /// Flag provisória indicando se foi encomendada refeição de bordo para esta futura emissão.
        /// </summary>
        public bool Refeicao { get; set; }

        /// <summary>
        /// Data de criação do registo em tabela temporária (útil para rotinas de limpeza de carrinhos abandonados).
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Limite temporal estipulado em que os lugares se manterão reservados/retidos para o cliente.
        /// </summary>
        public DateTime DataReserva { get; set; }
    }
}
