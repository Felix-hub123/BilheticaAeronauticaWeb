using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class PassageiroRepository : GenericRepository<Passageiro>, IPassageiroRepository
    {
        public PassageiroRepository(DataContext context) : base(context)
        {
        }
    }
}
