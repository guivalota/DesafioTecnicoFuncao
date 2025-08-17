using Newtonsoft.Json.Linq;
using System.Linq;

public static class ValidadorCPF
{
    public static bool ValidarCPF(string value)
    {
        if (value == null) return true; // N�o valida campo vazio (use [Required] junto se necess�rio)

        string cpf = new string(value.ToString().Where(char.IsDigit).ToArray());

        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        // Calcular primeiro d�gito
        int sum = 0;
        for (int i = 0, weight = 10; i < 9; i++, weight--)
            sum += (cpf[i] - '0') * weight;

        int remainder = sum % 11;
        int dv1 = remainder < 2 ? 0 : 11 - remainder;

        // Calcular segundo d�gito
        sum = 0;
        for (int i = 0, weight = 11; i < 10; i++, weight--)
            sum += (cpf[i] - '0') * weight;

        remainder = sum % 11;
        int dv2 = remainder < 2 ? 0 : 11 - remainder;

        return cpf[9] - '0' == dv1 && cpf[10] - '0' == dv2;
    }
}