using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Bilhete : IEntity
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }

        public int PassageiroId { get; set; }

        public Passageiro Passageiro { get; set; }

        public int VooId { get; set; }  
        public Voo Voo { get; set; }

        public int LugarId { get; set; } 
        public Lugar Lugar { get; set; }

        [Precision(18, 2)]
        public decimal Valor { get; set; }

        public bool BagagemExtra { get; set; }

        public bool Refeicao { get; set; }

        public DateTime DataReserva { get; set; } = DateTime.Now;

        public string CriadoPorUserId { get; set; }
        public bool WasDeleted { get; set; }


    }
    
}
