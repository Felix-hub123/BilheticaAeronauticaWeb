using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Repositório para operações específicas de dados relacionadas aos aviões,
    /// baseado no repositório genérico para herdar operações CRUD padrão.
    /// Pode ser extendido para incluir métodos personalizados do domínio de aviões.
    /// </summary>
    public class AviaoRepository : GenericRepository<Aviao>, IAviaoRepository
    {
        public AviaoRepository(DataContext context) : base(context)
        {
        }
    }


}
