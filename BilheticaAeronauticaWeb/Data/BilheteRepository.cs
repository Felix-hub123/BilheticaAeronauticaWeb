using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class BilheteRepository : GenericRepository<Bilhete>, IBilheteRepository
    {
        public BilheteRepository(DataContext context) : base(context)
        {
            
        }


    }
}
