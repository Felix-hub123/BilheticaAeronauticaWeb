using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class LugarRepository : GenericRepository<Lugar>, ILugarRepository
    {
        public LugarRepository(DataContext context) : base(context)
        {
            
        }
    }
}
