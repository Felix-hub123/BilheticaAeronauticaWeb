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

        Passageiro ToPassageiro(PassageiroViewModel model, string userId, bool isNew);

        PassageiroViewModel ToPassageirosViewModel(Passageiro passageiro);

        Bilhete ToBilhete(BilheteViewModel model, bool isNew);
        BilheteViewModel ToBilheteViewModel(Bilhete bilhete);

        Voo TooVoo(VooViewModel model, bool isNew);

        public VooViewModel ToVooViewModel(Voo voo);


        User ToFuncionario(FuncionarioViewModel model, bool isNew);

        FuncionarioViewModel ToFuncionarioViewModel(User funcionario);

        //ApplicationUser ToAdmin(AdminViewModel model, bool isNew);
        //AdminViewModel ToAdminViewModel(ApplicationUser admin);

       void UpdateFuncionarioFromViewModel(User funcionario, FuncionarioViewModel model);


    }
}
