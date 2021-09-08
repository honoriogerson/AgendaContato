
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaTelefonica.Models
{
    public class Pessoa
    {
        public int PessoaId { get; set; }
       
        [Required(ErrorMessage = "CPF obrigatório")]
        public string CPF { get; set; }

        public string Telefone { get; set; }

        [Required (ErrorMessage = "{0} é obrigatório")]
        [StringLength(60, ErrorMessage = "Nome muito extenso") ]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(60, ErrorMessage = "sobrenome muito extenso")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "{0} é obrigatório")]
        public Cidade Cidade { get; set; }

        public DateTime DataHoraCadastro { get; set; }

        public DateTime DataNascimento { get; set; }

        //Para termos mais de um telfone 
        public TipoTelefone TipoTelefone { get; set; }
          
    }

    public enum TipoTelefone
    {
        Pessoal, Comercial 
    }
    public enum Cidade
    {
        AC, AL, AP, AM, BA, CE, ES, GO, MA, MT, MS, MG, PA, PB, PR, PE, PI, RJ, RN, RS, RO, RR, SC, SP, SE, TO, DF
    }
}
