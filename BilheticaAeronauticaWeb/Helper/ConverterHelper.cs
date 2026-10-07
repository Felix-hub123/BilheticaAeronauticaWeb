using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Helper para converter entre entidades do domínio e ViewModels,
    /// facilitando o mapeamento em ambos os sentidos.
    /// </summary>
    public class ConverterHelper : IConverterHelper
    {
        private readonly IImageHelper _imageHelper;

        public ConverterHelper(IImageHelper imageHelper)
        {
            _imageHelper = imageHelper;
        }

        /// <summary>
        /// Converte um <see cref="AvioesViewModel"/> em entidade <see cref="Aviao"/>.
        /// </summary>
        /// <param name="model">ViewModel com dados do avião.</param>
        /// <param name="ImageId">Identificador da imagem associada.</param>
        /// <param name="isNew">Indica se a entidade é nova (para definir Id).</param>
        /// <returns>Nova instância de <see cref="Aviao"/> com os dados do VM.</returns>
        public Aviao ToAviao(AvioesViewModel model, Guid ImageId, bool isNew)
        {

            return new Aviao
            {
                Id = isNew ? 0 : model.Id,
                Marca = model.Marca,
                Modelo = model.Modelo,
                LugaresEconomica = model.LugaresEconomica,
                LugaresExecutiva = model.LugaresExecutiva,
                ImageId = ImageId,
                Disponivel = model.Disponivel
            };

        }



        public AvioesViewModel ToAvioesViewModel(Aviao aviao)
        {
            return new AvioesViewModel
            {
                Id = aviao.Id,
                Marca = aviao.Marca,
                Modelo = aviao.Modelo,
                LugaresEconomica = aviao.LugaresEconomica,
                LugaresExecutiva = aviao.LugaresExecutiva,
                ImageId = aviao.ImageId,
                Disponivel = aviao.Disponivel
            };

        }

        /// <summary>
        /// Converte uma <see cref="AeroportosViewModel"/> em entidade <see cref="Aeroporto"/>.
        /// </summary>
        /// <param name="model">ViewModel do aeroporto.</param>
        /// <param name="ImageId">Identificador da imagem associada.</param>
        /// <param name="isNew">Indica se é nova entidade para definir Id.</param>
        /// <returns>Entidade <see cref="Aeroporto"/> criada a partir do ViewModel.</returns>
        public Aeroporto ToAeroporto(AeroportosViewModel model, Guid ImageId, bool isNew)
        {
            return new Aeroporto
            {
                Id = isNew ? 0 : model.Id,
                Nome = model.Nome,
                Cidade = model.Cidade,
                Pais = model.Pais,
                IATA = model.IATA,
                ImageId = ImageId
            };
        }


        /// <summary>
        /// Converte entidade <see cref="Aeroporto"/> em <see cref="AeroportosViewModel"/>.
        /// </summary>
        /// <param name="aeroporto">Entidade aeroporto.</param>
        /// <returns>ViewModel criado a partir da entidade.</returns>
        public AeroportosViewModel ToAeroportosViewModel(Aeroporto aeroporto)
        {
            return new AeroportosViewModel
            {
                Id = aeroporto.Id,
                Nome = aeroporto.Nome,
                Cidade = aeroporto.Cidade,
                Pais = aeroporto.Pais,
                IATA = aeroporto.IATA,
                ImageId = aeroporto.ImageId
            };

        }

        public Passageiro ToPassageiro(PassageiroViewModel model, string userId, bool isNew)
        {
            return new Passageiro
            {
                Id = isNew ? 0 : model.Id,
                Nome = model.Nome,
                Apelido = model.Apelido,
                DocumentoIdentificacao = model.DocumentoIdentificacao,
                NumeroDocumento = model.NumeroDocumento,
                DataNascimento = model.DataNascimento,
                DataRegisto = DateTime.UtcNow,
                ImageId = model.ImageId,
                UserId = userId,
                WasDeleted = false
            };
        }


        /// <summary>
        /// Converte um <see cref="PassageiroViewModel"/> em entidade <see cref="Passageiro"/>.
        /// </summary>
        /// <param name="model">ViewModel do passageiro.</param>
        /// <param name="userId">Identificador do utilizador associado.</param>
        /// <param name="isNew">Indica se é uma nova entidade para definir Id.</param>
        /// <returns>Entidade Passageiro pronta para persistência.</returns>
        public PassageiroViewModel ToPassageirosViewModel(Passageiro passageiro)
        {
            return new PassageiroViewModel
            {
                Id = passageiro.Id,
                Nome = passageiro.Nome,
                Apelido = passageiro.Apelido,
                DocumentoIdentificacao = passageiro.DocumentoIdentificacao,
                NumeroDocumento = passageiro.NumeroDocumento,
                DataNascimento = passageiro.DataNascimento,
                ImageId = passageiro.ImageId,
      

            };
        }



        /// <summary>
        /// Converte um <see cref="VooViewModel"/> em entidade <see cref="Voo"/>.
        /// </summary>
        /// <param name="model">ViewModel do voo.</param>
        /// <param name="isNew">Indica se é uma nova entidade para definir Id.</param>
        /// <returns>Entidade Voo pronta para persistência.</returns>
        public Voo TooVoo(VooViewModel model, bool isNew)
        {
            return new Voo
            {
                Id = isNew ? 0 : model.Id,
                OrigemId = model.OrigemId,
                DestinoId = model.DestinoId,
                DataHoraPartida = model.DataHoraPartida

            };
        }


        /// <summary>
        /// Converte entidade <see cref="Voo"/> para o respetivo <see cref="VooViewModel"/>.
        /// </summary>
        /// <param name="voo">Entidade voo.</param>
        /// <returns>ViewModel contendo dados do voo.</returns>
        public VooViewModel ToVooViewModel(Voo voo)
        {
            return new VooViewModel
            {
                Id = voo.Id,
                OrigemNome = voo.Origem?.Nome,
                DestinoNome = voo.Destino?.Nome,
                DataHoraPartida = voo.DataHoraPartida

            };
        }


        /// <summary>
        /// Converte um <see cref="UserViewModel"/> em entidade <see cref="User"/>.
        /// </summary>
        /// <param name="model">ViewModel do utilizador (Funcionário).</param>
        /// <param name="isNew">Indica se é uma nova entidade para gerar Id.</param>
        /// <returns>Instância de <see cref="User"/> com os dados do ViewModel.</returns>
        public User ToFuncionario(UserViewModel model, bool isNew)
        {
            var user = new User
            {
                Id = isNew ? Guid.NewGuid().ToString() : model.Id,
                Nome = model.Nome,
                Email = model.Email,
                UserName = model.Email

            };
            return user;
        }


        /// <summary>
        /// Converte entidade <see cref="User"/> (Funcionário) em <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="funcionario">Entidade funcionário.</param>
        /// <returns>ViewModel com os dados do funcionário.</returns>
        public UserViewModel ToUserViewModel(User funcionario)
        {
            return new UserViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Email = funcionario.Email
            };


        }




        public void UpdateFuncionarioFromViewModel(User funcionario, UserViewModel model)
        {
            funcionario.Nome = model.Nome;
            funcionario.Email = model.Email;
            funcionario.UserName = model.Email;

        }





        /// <summary>
        /// Atualiza os dados do funcionário com base nos dados fornecidos no ViewModel.
        /// </summary>
        /// <param name="funcionario">Entidade funcionário a atualizar.</param>
        /// <param name="model">ViewModel com os dados atualizados.</param>
        public Bilhete ToBilhete(BilheteViewModel model, bool isNew)
        {
            return new Bilhete
            {
                Id = isNew ? 0 : model.Id,
                VooId = model.VooId,
                LugarId = model.LugarId,
                PassageiroId = model.PassageiroId, 
                Valor = model.Valor,
                BagagemExtra = model.BagagemExtra,
                Refeicao = model.Refeicao,
                DataReserva = model.DataCompra ?? DateTime.Now,
                WasDeleted = model.WasDeleted
        
            };
        }


        /// <summary>
        /// Converte uma entidade <see cref="Bilhete"/> em <see cref="BilheteViewModel"/>.
        /// </summary>
        /// <param name="bilhete">Entidade bilhete.</param>
        /// <returns>ViewModel correspondente ao bilhete.</returns>
        public BilheteViewModel ToBilheteViewModel(Bilhete bilhete)
        {
            return new BilheteViewModel
            {
                Id = bilhete.Id,
                VooId = bilhete.VooId,
                LugarId = bilhete.LugarId,
                PassageiroId = bilhete.PassageiroId,
                Valor = bilhete.Valor,
                BagagemExtra = bilhete.BagagemExtra,
                Refeicao = bilhete.Refeicao,
                DataCompra = bilhete.DataReserva,
                WasDeleted = bilhete.WasDeleted,
                DataPartida = bilhete.Voo?.DataHoraPartida ?? DateTime.MinValue,
                VooNumero = bilhete.Voo?.Numero,
                OrigemNome = bilhete.Voo?.Origem?.Nome,
                DestinoNome = bilhete.Voo?.Destino?.Nome,
                LugarCodigo = bilhete.Lugar?.Codigo

            };
        }


        /// <summary>
        /// Converte um <see cref="BilheteViewModel"/> em entidade temporária <see cref="BilheteTemp"/>.
        /// </summary>
        /// <param name="model">ViewModel do bilhete.</param>
        /// <param name="userId">Identificador do utilizador que criou a reserva temporária.</param>
        /// <returns>Nova instância de <see cref="BilheteTemp"/>.</returns>
        public BilheteTemp ToBilheteTemp(BilheteViewModel model, string userId)
        {
            return new BilheteTemp
            {
                PassageiroId = model.PassageiroId, 
                VooId = model.VooId,
                LugarId = model.LugarId,
                Preco = model.Valor,
                BagagemExtra = model.BagagemExtra,
                Refeicao = model.Refeicao,
                CriadoPorUserId = userId,
                DataCriacao = DateTime.Now
            };
        }


        /// <summary>
        /// Atualiza o ViewModel de um administrador com os dados da entidade <see cref="User"/>.
        /// </summary>
        /// <param name="admin">Entidade administrador.</param>
        /// <param name="model">ViewModel do administrador a atualizar.</param>
        public void ToAdminViewModel(User admin, UserViewModel model)
        {
            model.Id = admin.Id;
            model.Nome = admin.Nome;
            model.Apelido = admin.Apelido;
            model.Email = admin.Email;
            model.UserName = admin.UserName;
            model.PhoneNumber = admin.PhoneNumber;
            model.Endereço = admin.Endereço;
            model.ImageId = admin.ImageId;

        }


        /// <summary>
        /// Atualiza a entidade <see cref="Bilhete"/> com os dados fornecidos no <see cref="BilheteViewModel"/>.
        /// </summary>
        /// <param name="bilhete">Bilhete a ser atualizado.</param>
        /// <param name="model">ViewModel com dados atualizados.</param>
        /// <returns>Bilhete atualizado.</returns>
        public Bilhete UpdateBilheteFromViewModel(Bilhete bilhete, BilheteViewModel model)
        {
            bilhete.VooId = model.VooId;
            bilhete.LugarId = model.LugarId;
            bilhete.BagagemExtra = model.BagagemExtra;
            bilhete.Refeicao = model.Refeicao;
            bilhete.Valor = model.Valor;
            return bilhete;
        }




        public RegisterNewUserViewModel ToRegisterNewUserViewModel(RegisterFuncionarioViewModel funcModel)
        {
            if (funcModel == null)
                return null;

            return new RegisterNewUserViewModel
            {
                Nome = funcModel.Nome,
                Apelido = funcModel.Apelido,
                Username = funcModel.Email,        
                PhoneNumber = funcModel.PhoneNumber,

                Password = null,
                ConfirmPassword = null
            };
        }


        /// <summary>
        /// Converte uma entidade Voo no ViewModel utilizado
        /// na listagem pública de voos.
        /// </summary>
        public VooIndexViewModel ToVooIndexViewModel(Voo voo)
        {
            return new VooIndexViewModel
            {
                Id = voo.Id,

                Numero = voo.Numero,

                OrigemNome =
                    voo.Origem?.Nome ?? string.Empty,

                DestinoNome =
                    voo.Destino?.Nome ?? string.Empty,

                OrigemImageUrl =
                    voo.Origem != null
                        ? _imageHelper.GetImageUrl(
                            voo.Origem.ImageId,
                            "aeroportos")
                        : string.Empty,

                DestinoImageUrl =
                    voo.Destino != null
                        ? _imageHelper.GetImageUrl(
                            voo.Destino.ImageId,
                            "aeroportos")
                        : string.Empty,

                AviaoModelo =
                    voo.Aviao?.Modelo ?? string.Empty,

                DataHoraPartida =
                    voo.DataHoraPartida,

                DataHoraChegada =
                    voo.DataHoraChegada
            };
        }


        /// <summary>
        /// Converte uma coleção de voos para a coleção
        /// utilizada pela página de listagem.
        /// </summary>
        public List<VooIndexViewModel> ToVooIndexViewModels(
            IEnumerable<Voo> voos)
        {
            return voos
                .Select(ToVooIndexViewModel)
                .ToList();
        }

    }
}
