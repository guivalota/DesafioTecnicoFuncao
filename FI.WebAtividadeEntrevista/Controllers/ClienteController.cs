using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Net;
using System.Reflection;
using FI.AtividadeEntrevista.BLL.Validations;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private const string SessionBeneficiario = "SessionBeneficiario";
        private BoBeneficiario boBeneficiario
        {
            get
            {
                if (Session[SessionBeneficiario] == null)
                {
                    Session[SessionBeneficiario] = new BoBeneficiario();
                }
                return (BoBeneficiario)Session[SessionBeneficiario];
            }
            set { Session[SessionBeneficiario] = value; }
        }
        private BoCliente boCliente = new BoCliente();
        public ActionResult Index()
        {
            Session[SessionBeneficiario] = null;
            return View();
        }

        public ActionResult Incluir()
        {
            Session[SessionBeneficiario] = null;
            ViewBag.IsEdit = false;
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!ValidadorCPF.ValidarCPF(model.CPF))
                {
                        Response.StatusCode = 400;
                        return Json("Informe um CPF valido");   
                }

                var retorno = boCliente.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                }, boBeneficiario.getListaTemp());

                if (retorno)
                {
                    Session[SessionBeneficiario] = null;
                    Response.StatusCode = 200;
                    return Json("Cadastro efetuado com sucesso");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json("Este CPF já está cadastrado para outro usuário");
                }
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            ViewBag.IsEdit = true;
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!ValidadorCPF.ValidarCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("Informe um CPF valido");
                }
                var retorno = boCliente.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                }
                , boBeneficiario.getListaTemp());

                if (retorno)
                {
                    return Json(new { Success = true, Message = "Cadastro efetuado com sucesso" });
                }
                else
                    return Json(new { Success = false, Message = "Este CPF já está cadastrado para outro usuário" });
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            ViewBag.IsEdit = true;
            Cliente cliente = boCliente.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = CPFNormalizer.FormatCPF(cliente.CPF)
                };

                BeneficiarioListBanco(model.Id);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);
                foreach (Cliente cliente in clientes)
                    cliente.CEP = CPFNormalizer.FormatCPF(cliente.CPF);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult IncluirBeneficiarioTemp(BeneficiarioModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();

                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }
                else
                {
                    model.Id = boBeneficiario.AdicionarOuAtualizarBeneficiario(new Beneficiario()
                    {
                        Id = model.Id,
                        Nome = model.Nome,
                        CPF = model.CPF
                    });

                    if (model.Id != -1)
                    {
                        Response.StatusCode = 200;
                        return Json("Beneficiario adicionado efetuado com sucesso");
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return Json("Este CPF já está cadastrado para outro usuário");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ExcluirBeneficiarioTemp(int id)
        {
            boBeneficiario.RemoverListaTemp(id);
            return Json(new { sucesso = true });
        }

        private void BeneficiarioListBanco(long idCliente)
        {
            try
            {
                // Buscar beneficiários do cliente usando sua BLL
                List<Beneficiario> beneficiarios = new BoBeneficiario()
                    .Pesquisa(idCliente);
                boBeneficiario.PreencherListaBanco(beneficiarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro:{ex}");
            }
        }

        [HttpPost]
        public JsonResult BeneficiarioListTemp()
        {

            // Buscar beneficiários do cliente usando sua BLL
            List<Beneficiario> beneficiarios = boBeneficiario.PesquisaTemp();

            // Retorna no padrão jTable
            return Json(new { Result = "OK", Records = beneficiarios });

        }
    }
}