using BilheticaAeronauticaWeb.Data.Entities;
using System.Linq;

namespace BilheticaAeronauticaWeb.Data
{
    public class AeroportoRepository : GenericRepository<Aeroporto>, IAeroportoRepository
    {
        public AeroportoRepository(DataContext context) : base(context) 
        { 

        }
  
    }
}
