using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using SuperShop.Models;
using System;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Interface que define métodos para converter entre entidades do domínio e respetivos ViewModels,
    /// facilitando o mapeamento bidirecional entre modelos e view models da aplicação.
    /// </summary>
    public interface IConverterHelper
    {
        // <summary>
        /// Converte um <see cref="AvioesViewModel"/> para a entidade <see cref="Aviao"/>.
        /// </summary>
        /// <param name="model">ViewModel com os dados do avião.</param>
        /// <param name="ImageId">Identificador da imagem associada ao avião.</param>
        /// <param name="isNew">Indica se é uma nova entidade (define o Id).</param>
        /// <returns>Entidade <see cref="Aviao"/> preenchida a partir do ViewModel.</returns>
        Aviao ToAviao(AvioesViewModel model, Guid ImageId, bool isNew);


        /// <summary>
        /// Converte um <see cref="AeroportosViewModel"/> para a entidade <see cref="Aeroporto"/>.
        /// </summary>
        /// <param name="model">ViewModel do aeroporto.</param>
        /// <param name="ImageId">Identificador da imagem associada ao aeroporto.</param>
        /// <param name="isNew">Define se o objeto é novo para tratamento do Id.</param>
        /// <returns>Entidade <see cref="Aeroporto"/> criada.</returns>
        AvioesViewModel ToAvioesViewModel(Aviao aviao);


        /// <summary>
        /// Converte uma entidade <see cref="Aeroporto"/> para a respetiva <see cref="AeroportosViewModel"/>.
        /// </summary>
        /// <param name="aeroporto">Entidade aeroporto.</param>
        /// <returns>ViewModel preenchido com os dados do aeroporto.</returns>

        Aeroporto ToAeroporto(AeroportosViewModel model, Guid ImageId, bool isNew);



        /// <summary>
        /// Converte um <see cref="PassageiroViewModel"/> para a entidade <see cref="Passageiro"/>.
        /// </summary>
        /// <param name="model">ViewModel do passageiro.</param>
        /// <param name="userId">Identificador do utilizador associado.</param>
        /// <param name="isNew">Indica se a entidade é nova (define Id).</param>
        /// <returns>Entidade <see cref="Passageiro"/> criada a partir do ViewModel.</returns>
        AeroportosViewModel ToAeroportosViewModel(Aeroporto aeroporto);


        /// <summary>
        /// Converte uma entidade <see cref="Passageiro"/> para o respetivo <see cref="PassageiroViewModel"/>.
        /// </summary>
        /// <param name="passageiro">Entidade passageiro.</param>
        /// <returns>ViewModel preenchido com os dados do passageiro.</returns>
        Passageiro ToPassageiro(PassageiroViewModel model, string userId, bool isNew);


        /// <summary>
        /// Converte um <see cref="VooViewModel"/> para a entidade <see cref="Voo"/>.
        /// </summary>
        /// <param name="model">ViewModel do voo.</param>
        /// <param name="isNew">Indica se o voo é novo (define Id).</param>
        /// <returns>Entidade <see cref="Voo"/> criada a partir do ViewModel.</returns>
        PassageiroViewModel ToPassageirosViewModel(Passageiro passageiro);


        /// <summary>
        /// Converte uma entidade <see cref="Voo"/> para um <see cref="VooViewModel"/>.
        /// </summary>
        /// <param name="voo">Entidade voo.</param>
        /// <returns>ViewModel preenchido com dados do voo.</returns>
        Voo TooVoo(VooViewModel model, bool isNew);


        /// <summary>
        /// Converte um <see cref="UserViewModel"/> para a entidade <see cref="User"/> (funcionário).
        /// </summary>
        /// <param name="model">ViewModel do funcionário.</param>
        /// <param name="isNew">Indica se é uma nova entidade (define Id).</param>
        /// <returns>Entidade <see cref="User"/> criada.</returns>
        public VooViewModel ToVooViewModel(Voo voo);

        /// <summary>
        /// Converte uma entidade <see cref="User"/> (funcionário) para o respetivo <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="user">Entidade funcionário.</param>
        /// <returns>ViewModel com os dados do funcionário.</returns>
        User ToFuncionario(UserViewModel model, bool isNew);



        UserViewModel ToUserViewModel(User user);

        /// <summary>
        /// Converte um <see cref="BilheteViewModel"/> em entidade temporária <see cref="BilheteTemp"/>.
        /// </summary>
        /// <param name="model">ViewModel do bilhete.</param>
        /// <param name="userId">Identificador do utilizador que cria a reserva temporária.</param>
        /// <returns>Entidade <see cref="BilheteTemp"/> criada.</returns>
        public BilheteTemp ToBilheteTemp(BilheteViewModel model, string userId);


        /// <summary>
        /// Converte uma entidade <see cref="Bilhete"/> para o respetivo <see cref="BilheteViewModel"/>.
        /// </summary>
        /// <param name="bilhete">Entidade bilhete.</param>
        /// <returns>ViewModel preenchido com os dados do bilhete.</returns>
        BilheteViewModel ToBilheteViewModel(Bilhete bilhete);


        /// <summary>
        /// Atualiza uma entidade <see cref="User"/> (funcionário) com os dados de um <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="funcionario">Entidade funcionário a actualizar.</param>
        /// <param name="model">Dados no ViewModel para atualizar o funcionário.</param>
        public void UpdateFuncionarioFromViewModel(User funcionario, UserViewModel model);

        /// <summary>
        /// Preenche um <see cref="UserViewModel"/> com os dados de um utilizador administrador (<see cref="User"/>).
        /// </summary>
        /// <param name="admin">Entidade administrador.</param>
        /// <param name="model">ViewModel a ser preenchido.</param>
        public void ToAdminViewModel(User admin, UserViewModel model);


        /// <summary>
        /// Atualiza uma entidade <see cref="Bilhete"/> com os dados de um <see cref="BilheteViewModel"/>.
        /// </summary>
        /// <param name="bilhete">Entidade bilhete a ser atualizada.</param>
        /// <param name="model">ViewModel que contém os dados atualizados.</param>
        /// <returns>Bilhete atualizado.</returns>
        Bilhete UpdateBilheteFromViewModel(Bilhete bilhete, BilheteViewModel model);

        public RegisterNewUserViewModel ToRegisterNewUserViewModel(RegisterFuncionarioViewModel funcModel);
    }
}
