using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using System;

namespace BilheticaAeronauticaWeb.Helper
{
    public interface IConverterHelper
    {
        Aviao ToAviao(AvioesViewModel model, Guid ImageId, bool isNew);

        AvioesViewModel ToAvioesViewModel(Aviao aviao);

        Aeroporto ToAeroporto(AeroportosViewModel model, Guid ImageId, bool isNew);

        AeroportosViewModel ToAeroportosViewModel(Aeroporto aeroporto);
    }
}
