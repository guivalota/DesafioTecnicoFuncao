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

        /// <summary>
        /// Formata uma string de 11 dígitos como CPF (###.###.###-##).
        /// </summary>
        /// <param name="cpf">String contendo apenas números, com 11 caracteres</param>
        /// <returns>CPF formatado (ex.: 123.456.789-01)</returns>
        public static string FormatCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            cpf = cpf.Trim();

            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }
    }
}
