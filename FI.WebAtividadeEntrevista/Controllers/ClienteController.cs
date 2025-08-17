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
                model.Id = boCliente.Incluir(new Cliente()
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
                });

                if (model.Id != -1)
                {
                    //incluir os beneficiarios
                    List<Beneficiario> beneficiarios = boBeneficiario.PesquisaTemp();
                    foreach(Beneficiario b in beneficiarios)
                    {
                        b.IdCliente = model.Id;
                        if (IncluirBeneficiarioInterno(b))
                        {
                            //Faria algum tipo de tratamento se controlasse a conection, daria um rollback
                        }
                    }
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
                return Json(new { Success = false, Message = string.Join(Environment.NewLine, erros) });
            }
            else
            {
                boCliente.Alterar(new Cliente()
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
                });

                if (model.Id != -1)
                    return Json(new { Success = true, Message = "Cadastro efetuado com sucesso" });
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
                    CPF = cliente.CPF
                };

            
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

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool IncluirBeneficiarioInterno(Beneficiario b)
        {
            b.Id = boBeneficiario.Incluir(b);
            return b.Id != -1;
        }

        [HttpPost]
        public ActionResult IncluirBeneficiario(BeneficiarioModel model)
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
                    model.Id = boBeneficiario.Incluir(new Beneficiario()
                    {
                        Nome = model.Nome,
                        CPF = model.CPF,
                        IdCliente = model.IdCliente
                    });

                    if (model.Id != -1)
                    {
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
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
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
        public JsonResult AlterarBeneficiario(Beneficiario model)
        {
            ViewBag.IsEdit = true;
            BoBeneficiario bo = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(new { Success = false, Message = string.Join(Environment.NewLine, erros) });
            }
            else
            {
                if (bo.Alterar(new Beneficiario()
                {
                    Id = model.Id,
                    Nome = model.Nome,
                    CPF = model.CPF,
                    IdCliente = model.IdCliente,
                }))
                {
                    Response.StatusCode = 200;
                    return Json("Cadastro efetuado com sucesso");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json("Este CPF já está cadastrado para outro beneficiário");
                }
            }
        }

        [HttpPost]
        public JsonResult ExcluirBeneficiario(long id, long idCliente)
        {
            try
            {
                BoBeneficiario bo = new BoBeneficiario();
                bo.Excluir(id, idCliente);
                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult BeneficiarioList(long idCliente)
        {
            try
            {
                // Buscar beneficiários do cliente usando sua BLL
                List<Beneficiario> beneficiarios = new BoBeneficiario()
                    .Pesquisa(idCliente);

                // Retorna no padrão jTable
                return Json(new { Result = "OK", Records = beneficiarios });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
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