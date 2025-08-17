using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.BLL.Validations;
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
        public long Incluir(DML.Beneficiario beneficiario)
        {
            beneficiario.CPF = CPFNormalizer.NormalizeCPF(beneficiario.CPF);
            DAL.DaoBeneficiario dal = new DAL.DaoBeneficiario();
            if (!dal.VerificarExistencia(beneficiario.CPF, beneficiario.IdCliente))
                return dal.Incluir(beneficiario);

            return -1;
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public bool Alterar(DML.Beneficiario beneficiario)
        {
            //Normalizar os dados (do CPF)
            beneficiario.CPF = CPFNormalizer.NormalizeCPF(beneficiario.CPF);
            DAL.DaoBeneficiario dal = new DAL.DaoBeneficiario();
            //verificar se existe outro
            //if (!dal.VerificarExistencia(beneficiario.CPF,beneficiario.IdCliente))
            //{
                //verificar se não é ele mesmo
                if (dal.VerificarExistenciaAlteracao(beneficiario.CPF, beneficiario.IdCliente, beneficiario.Id))
                {
                    dal.Alterar(beneficiario);
                    return true;
                }
            //}
            return false;
        }

        /// <summary>
        /// Lista os beneficiarios
        /// </summary>
        public List<DML.Beneficiario> Pesquisa(long idCliente)
        {
            DAL.DaoBeneficiario dal = new DAL.DaoBeneficiario();
            return dal.Pesquisa(idCliente);
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
        public void Excluir(long ID, long IdCliente)
        {
            DAL.DaoBeneficiario dal = new DAL.DaoBeneficiario();
            dal.Excluir(ID, IdCliente);
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
    }
}
