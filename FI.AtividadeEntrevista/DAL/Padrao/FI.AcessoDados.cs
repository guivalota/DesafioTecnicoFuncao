using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FI.AtividadeEntrevista.DAL.Padrao;

namespace FI.AtividadeEntrevista.DAL
{
    public class AcessoDados
    {
        private readonly ConexaoBanco _conexaoBanco;

        public AcessoDados(ConexaoBanco conexaoBanco)
        {
            _conexaoBanco = conexaoBanco;
        }

        internal void Executar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlCommand comando = new SqlCommand(NomeProcedure, _conexaoBanco.Conexao))
            {
                comando.CommandType = CommandType.StoredProcedure;

                if (_conexaoBanco.Transacao != null)
                    comando.Transaction = _conexaoBanco.Transacao;

                if (parametros != null)
                    comando.Parameters.AddRange(parametros.ToArray());

                comando.ExecuteNonQuery();
            }
        }

        internal DataSet Consultar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlCommand comando = new SqlCommand(NomeProcedure, _conexaoBanco.Conexao))
            {
                comando.CommandType = CommandType.StoredProcedure;

                if (_conexaoBanco.Transacao != null)
                    comando.Transaction = _conexaoBanco.Transacao;

                if (parametros != null)
                    comando.Parameters.AddRange(parametros.ToArray());

                SqlDataAdapter adapter = new SqlDataAdapter(comando);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}
