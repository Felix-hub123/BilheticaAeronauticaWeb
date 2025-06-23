using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class VooRepository : GenericRepository<Voo>, IVooRepository
    {
        public VooRepository(DataContext context) : base(context)
        {
        }
    }
}
