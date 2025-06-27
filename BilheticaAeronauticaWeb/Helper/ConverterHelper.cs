using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using System;

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
                DataNascimento = passageiro.DataNascimento
            };
        }

        public Bilhete ToBilhete(BilheteViewModel model, bool isNew)
        {
            return new Bilhete
            {
                Id = isNew ? 0 : model.Id,
                LugarId = model.LugarId,
                PassageiroId = model.PassageiroId,
                DataCompra = model.DataCompra ?? DateTime.UtcNow,
                Preco = model.Preco,
                WasDeleted = false
            };
        }

        public BilheteViewModel ToBilheteViewModel(Bilhete bilhete)
        {
            return new BilheteViewModel
            {
                Id = bilhete.Id,
                LugarId = bilhete.LugarId,
                PassageiroId = bilhete.PassageiroId,
                DataCompra = bilhete.DataCompra,
                Preco = bilhete.Preco
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

        public User ToFuncionario(FuncionarioViewModel model, bool isNew)
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

        public FuncionarioViewModel ToFuncionarioViewModel(User funcionario)
        {
            return new FuncionarioViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Email = funcionario.Email
            };


        }

        public void UpdateFuncionarioFromViewModel( User funcionario, FuncionarioViewModel model)
        {
            funcionario.Nome = model.Nome;
            funcionario.Email = model.Email;
            funcionario.UserName = model.Email;
           
        }

    }
}
