using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.BLL.Validations;
using FI.AtividadeEntrevista.DAL.Padrao;
using FI.AtividadeEntrevista.DAL;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public bool Incluir(DML.Cliente cliente, List<Beneficiario> beneficiarios)
        {
            cliente.CPF = CPFNormalizer.NormalizeCPF(cliente.CPF);
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    if (!dal.VerificarExistencia(cliente.CPF))
                    {
                        var retorno = dal.Incluir(cliente);
                        if (retorno != -1)
                        {
                            var dalBeneficiario = new DaoBeneficiario(acesso);
                            foreach (Beneficiario b in beneficiarios)
                            {
                                b.IdCliente = retorno;
                                dalBeneficiario.Incluir(b);
                            }
                            conexao.Commit();
                            return true;
                        }
                        else
                        {
                            conexao.Rollback();
                            return false;
                        }
                    }
                    else
                        return false;
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }finally { conexao.Dispose(); }
            }
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public bool Alterar(DML.Cliente cliente, List<Beneficiario> beneficiarios)
        {
            //Normalizar os dados (do CPF)
            //Verificaria os padrões utilizados no projeto para normalizar os dados antes da inclusão (como telefones, etc)
            cliente.CPF = CPFNormalizer.NormalizeCPF(cliente.CPF);
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    if (dal.Alterar(cliente))
                    {
                        var boBeneficiario = new BoBeneficiario();
                        if (boBeneficiario.AlterarListaBeneficiarios(cliente.Id, beneficiarios, acesso))
                        {
                            conexao.Commit();
                            return true;
                        }
                        else
                            conexao.Rollback();
                    }
                    else
                    {
                        conexao.Rollback();
                    }

                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }
                finally {  conexao.Dispose(); }
                return false;
            }
        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    return dal.Consultar(id);
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }finally { conexao.Dispose(); }
            }
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    dal.Excluir(id);
                    conexao.Commit();
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }
                finally {  conexao.Dispose(); }
            }
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    return dal.Listar();
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }finally { conexao.Dispose(); }
            }
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    return dal.Pesquisa(iniciarEm,  quantidade, campoOrdenacao, crescente, out qtd);
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }
                finally {  conexao.Dispose(); }
            }
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoCliente(acesso);
                    return dal.VerificarExistencia(CPF);
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }finally { conexao.Dispose(); }
            }
        }
    }
}
