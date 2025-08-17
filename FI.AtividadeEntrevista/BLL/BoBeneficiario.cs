using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.BLL.Validations;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
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

        /// <summary>
        /// Excluir um beneficiario
        /// </summary>
        /// <param name="ID">Objeto de beneficiario</param>
        public void Excluir(long ID, long IdCliente)
        {
            DAL.DaoBeneficiario dal = new DAL.DaoBeneficiario();
            dal.Excluir(ID, IdCliente);
        }
    }
}
