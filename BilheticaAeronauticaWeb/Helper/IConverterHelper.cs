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

        Voo TooVoo(VooViewModel model, bool isNew);

        public VooViewModel ToVooViewModel(Voo voo);


        User ToFuncionario(UserViewModel model, bool isNew);

        UserViewModel ToUserViewModel(User user);

        //ApplicationUser ToAdmin(AdminViewModel model, bool isNew);
        //AdminViewModel ToAdminViewModel(ApplicationUser admin);
        public BilheteTemp ToBilheteTemp(BilheteViewModel model, string userId);

        BilheteViewModel ToBilheteViewModel(Bilhete bilhete);

        public void UpdateFuncionarioFromViewModel(User funcionario, UserViewModel model);
        public void ToAdminViewModel(User admin, UserViewModel model);
    }
}
