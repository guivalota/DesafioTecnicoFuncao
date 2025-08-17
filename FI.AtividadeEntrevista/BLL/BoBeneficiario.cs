using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.BLL.Validations;
using FI.AtividadeEntrevista.DAL;
using FI.AtividadeEntrevista.DAL.Padrao;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        private List<Beneficiario> listaBeneficiarios;
        public BoBeneficiario()
        {
            listaBeneficiarios = new List<Beneficiario>();
        }
        /// <summary>
        /// Inclui um novo beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public long Incluir(DML.Beneficiario beneficiario, AcessoDados acesso)
        {
            beneficiario.CPF = CPFNormalizer.NormalizeCPF(beneficiario.CPF);
            using(var conexao = new ConexaoBanco())
            {
                try
                {
                    var dal = new DaoBeneficiario(acesso);
                    if (!dal.VerificarExistencia(beneficiario.CPF, beneficiario.IdCliente))
                    {
                        var retorno = dal.Incluir(beneficiario);
                        return retorno;
                    }
                    return -1;
                }
                catch
                {
                    conexao.Rollback();
                    throw;
                }
            }


        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public bool Alterar(DML.Beneficiario beneficiario, AcessoDados acesso)
        {
            //Normalizar os dados (do CPF)
            beneficiario.CPF = CPFNormalizer.NormalizeCPF(beneficiario.CPF);
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    var dal = new DaoBeneficiario(acesso);
                    if (dal.VerificarExistenciaAlteracao(beneficiario.CPF, beneficiario.IdCliente, beneficiario.Id))
                    {
                        dal.Alterar(beneficiario);
                        return true;
                    }
                    return false;
                }
                catch
                {
                    throw;
                }
             }
        }

        /// <summary>
        /// Lista os beneficiarios
        /// </summary>
        public List<DML.Beneficiario> Pesquisa(long idCliente)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    conexao.Abrir();
                    conexao.BeginTransaction();
                    var acesso = new AcessoDados(conexao);
                    var dal = new DaoBeneficiario(acesso);
                    return dal.Pesquisa(idCliente);
                }
                catch
                {
                    conexao.Rollback(); 
                    throw;
                }
            }
        }

        public List<DML.Beneficiario>PesquisaTemp()
        {
            for (int i=0; i< listaBeneficiarios.Count; i++)
            {
                listaBeneficiarios[i].Id = i;
            }
            return listaBeneficiarios;
        }

        /// <summary>
        /// Excluir um beneficiario
        /// </summary>
        /// <param name="ID">Objeto de beneficiario</param>
        public void Excluir(long ID, long IdCliente, AcessoDados acesso)
        {
            using (var conexao = new ConexaoBanco())
            {
                try
                {
                    var dal = new DaoBeneficiario(acesso);
                    dal.Excluir(ID, IdCliente);
                }
                catch
                {
                    throw;
                }
            }
        }

        public int AdicionarOuAtualizarBeneficiario(Beneficiario novoBeneficiario)
        {
            if (novoBeneficiario == null || string.IsNullOrWhiteSpace(novoBeneficiario.CPF))
                throw new ArgumentException("Beneficiário ou CPF inválido.");

            if (novoBeneficiario.Id == -1)
            {
                // Caso seja novo beneficiário, procura CPF duplicado
                bool cpfDuplicado = listaBeneficiarios.Any(b => b.CPF == novoBeneficiario.CPF);
                if (cpfDuplicado)
                    return -1; // não permite inserir com CPF duplicado

                // Adiciona novo beneficiário
                listaBeneficiarios.Add(novoBeneficiario);
                return listaBeneficiarios.Count - 1; // posição na lista
            }
            else
            {
                // Caso seja alteração, procura CPF duplicado exceto ele mesmo
                bool cpfDuplicado = listaBeneficiarios
                    .Any(b => b.CPF == novoBeneficiario.CPF && b.Id != novoBeneficiario.Id);
                if (cpfDuplicado)
                    return -1; // não permite atualizar com CPF duplicado

                // Procura o beneficiário pelo ID para atualizar
                var existente = listaBeneficiarios.FirstOrDefault(b => b.Id == novoBeneficiario.Id);
                if (existente != null)
                {
                    existente.CPF = novoBeneficiario.CPF;
                    existente.Nome = novoBeneficiario.Nome;
                    return listaBeneficiarios.IndexOf(existente);
                }
                else
                {
                    // Caso não encontre pelo ID, adiciona como novo
                    listaBeneficiarios.Add(novoBeneficiario);
                    return listaBeneficiarios.Count - 1;
                }
            }
        }
        public void RemoverListaTemp(int id)
        {
            listaBeneficiarios.RemoveAt(id);
        }

        public List<Beneficiario> PreencherListaBanco(List<Beneficiario> beneficiarios)
        {
            return listaBeneficiarios = beneficiarios.ToList();
        }

        public List<Beneficiario> getListaTemp()
        {
            return listaBeneficiarios;
        }


        public bool AlterarListaBeneficiarios(long idCliente, List<Beneficiario> listaNova, AcessoDados acesso)
        {
            try
            {
                //nomalizar cpf na nova lista
                foreach(Beneficiario item in listaNova) 
                {
                    item.CPF = CPFNormalizer.NormalizeCPF(item.CPF);
                }
                //buscar todos os Beneficiarios
                List<Beneficiario> beneficiariosAtual = Pesquisa(idCliente);
                var inserir = listaNova
                .Where(n => !beneficiariosAtual.Any(a => a.CPF == n.CPF))
                .Select(n =>
                {
                    n.IdCliente = idCliente;
                    return n;
                })
                .ToList();
                var atualizar = listaNova
                .Where(n => beneficiariosAtual.Any(a => a.CPF == n.CPF))
                .Select(n =>
                {
                    var existente = beneficiariosAtual.First(a => a.CPF == n.CPF);
                    n.Id = existente.Id;
                    n.IdCliente = existente.IdCliente;
                    return n;
                })
                .ToList();
                var deletar = beneficiariosAtual.Where(a => !listaNova.Any(n => n.CPF == a.CPF)).ToList();

                foreach (Beneficiario b in deletar)
                    Excluir(b.Id, b.IdCliente, acesso);
                foreach (Beneficiario b in atualizar)
                    Alterar(b, acesso);
                foreach (Beneficiario b in inserir)
                    Incluir(b, acesso);

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
