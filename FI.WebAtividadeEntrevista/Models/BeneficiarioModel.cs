using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }


        /// <summary>
        /// CPF
        /// </summary>
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [Cpf(ErrorMessage = "Digite um CPF válido")]
        public string CPF { get; set; }

        public long IdCliente { get; set; }

    }
}