﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaTelefonica.Models
{
    public class Contexto : DbContext
    {
        //atributos para referencia das tabelas 

        public DbSet <Pessoa> Pessoas { get; set; }

        public Contexto(DbContextOptions<Contexto> opcoes) : base(opcoes)
        {

        }

    }
}