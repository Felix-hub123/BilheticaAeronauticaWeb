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
    }
}
