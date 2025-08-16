using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL.Validations
{
    public static class CPFNormalizer
    {
        /// <summary>
        /// Remove todos os caracteres não numéricos.
        /// </summary>
        /// <param name="documento">CPF com ou sem máscara.</param>
        /// <returns>Somente os dígitos numéricos, ou string vazia se entrada for nula.</returns>
        public static string OnlyNumbers(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            return new string(documento.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Normaliza CPF (remove máscara e mantém 11 dígitos).
        /// </summary>
        public static string NormalizeCPF(string cpf)
        {
            string onlyNumbers = OnlyNumbers(cpf);
            return onlyNumbers.Length == 11 ? onlyNumbers : string.Empty;
        }
    }
}
