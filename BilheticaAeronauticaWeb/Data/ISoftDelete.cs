namespace BilheticaAeronauticaWeb.Data
{
    public interface ISoftDelete
    {
        bool WasDeleted { get; set; }
    }
}
