namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Interface base que define propriedades comuns para todas as entidades do sistema,
    /// garantindo um identificador único e suporte para eliminação lógica (soft delete).
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }


        bool WasDeleted { get; set; }

    }
}
