using Microsoft.EntityFrameworkCore;
using System;

namespace BilheticaAeronauticaWeb.Data.Entities
{
#nullable enable


    /// <summary>
    /// Representa um bilhete de voo reservado ou comprado por um passageiro,
    /// contendo informações sobre o voo, lugar, preço, opções adicionais (bagagem extra, refeição),
    /// estado da reserva/pagamento, datas relevantes e histórico de criação e soft delete.
    /// </summary>
    public class Bilhete : IEntity
    {
        /// <summary>
        /// Identificador único do bilhete.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data e hora em que a transação de compra do bilhete foi finalizada.
        /// </summary>
        public DateTime DataCompra { get; set; }

        /// <summary>
        /// Chave estrangeira de associação ao passageiro titular.
        /// </summary>
        public int PassageiroId { get; set; }

        /// <summary>
        /// Propriedade de navegação do passageiro titular do bilhete.
        /// </summary>
        public Passageiro? Passageiro { get; set; }

        /// <summary>
        /// Chave estrangeira de associação ao voo planeado.
        /// </summary>
        public int VooId { get; set; }

        /// <summary>
        /// Propriedade de navegação para obter dados do voo associado.
        /// </summary>
        public Voo? Voo { get; set; }

        /// <summary>
        /// Chave estrangeira de associação ao lugar físico escolhido.
        /// </summary>
        public int LugarId { get; set; }

        /// <summary>
        /// Propriedade de navegação do assento específico mapeado.
        /// </summary>
        public Lugar? Lugar { get; set; }

        /// <summary>
        /// Valor monetário final faturado e pago pelo bilhete.
        /// </summary>
        [Precision(18, 2)]
        public decimal Valor { get; set; }

        /// <summary>
        /// Indica se o passageiro contratou o serviço adicional de franquia de bagagem extra.
        /// </summary>
        public bool BagagemExtra { get; set; }

        /// <summary>
        /// Indica se o passageiro contratou serviço de bordo ou refeição especial para o voo.
        /// </summary>
        public bool Refeicao { get; set; }

        /// <summary>
        /// Data e hora em que a reserva de lugar no voo foi iniciada no sistema.
        /// </summary>
        public DateTime DataReserva { get; set; } = DateTime.Now;

        /// <summary>
        /// Identificador do utilizador ou operador do sistema que efetuou a emissão da reserva.
        /// </summary>
        public string? CriadoPorUserId { get; set; }

        /// <summary>
        /// Flag que indica se o registo foi logicamente eliminado (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }

        /// <summary>
        /// Sinaliza se a transação financeira ligada ao bilhete foi deferida com sucesso.
        /// </summary>
        public bool PagamentoConfirmado { get; set; } = false;

        /// <summary>
        /// Data e hora de efetivação e emissão formal do cartão de embarque/bilhete líquido.
        /// </summary>
        public DateTime? DataEmissao { get; set; }

        /// <summary>
        /// Estado atual em que a reserva se encontra (Ex: "Reservado", "Pago", "Cancelado").
        /// </summary>
        public string Estado { get; set; } = "Reservado";
    }
}


