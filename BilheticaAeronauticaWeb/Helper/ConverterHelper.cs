using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace BilheticaAeronauticaWeb.Helper
{
    public class ConverterHelper : IConverterHelper
    {
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

            };
        }

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

        public Bilhete UpdateBilheteFromViewModel(Bilhete bilhete, BilheteViewModel model)
        {
            bilhete.VooId = model.VooId;
            bilhete.LugarId = model.LugarId;
            bilhete.BagagemExtra = model.BagagemExtra;
            bilhete.Refeicao = model.Refeicao;
            bilhete.Valor = model.Valor;
            return bilhete;
        }
    }
}
