using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class AviaoRepository : GenericRepository<Aviao>, IAviaoRepository
    {
        public AviaoRepository(DataContext context) : base(context)
        {
        }
    }


}
