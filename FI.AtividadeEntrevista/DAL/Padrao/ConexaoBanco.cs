using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.DAL.Padrao
{
    public class ConexaoBanco : IDisposable
    {
        private readonly SqlConnection _conexao;
        private SqlTransaction _transacao;

        public SqlConnection Conexao => _conexao;
        public SqlTransaction Transacao => _transacao;

        public ConexaoBanco()
        {
            var conn = ConfigurationManager.ConnectionStrings["BancoDeDados"];
            if (conn == null)
                throw new Exception("String de conexão não encontrada em app.config/web.config");

            _conexao = new SqlConnection(conn.ConnectionString);
        }

        public void Abrir()
        {
            if (_conexao.State != ConnectionState.Open)
                _conexao.Open();
        }

        public void Fechar()
        {
            if (_conexao.State != ConnectionState.Closed)
                _conexao.Close();
        }

        public void BeginTransaction()
        {
            if (_conexao.State != ConnectionState.Open)
                _conexao.Open();

            _transacao = _conexao.BeginTransaction();
        }

        public void Commit()
        {
            _transacao?.Commit();
            _transacao = null;
        }

        public void Rollback()
        {
            _transacao?.Rollback();
            _transacao = null;
        }

        public void Dispose()
        {
            if (_transacao != null)
            {
                _transacao.Dispose();
                _transacao = null;
            }

            if (_conexao != null)
            {
                _conexao.Dispose();
            }
        }
    }
}
