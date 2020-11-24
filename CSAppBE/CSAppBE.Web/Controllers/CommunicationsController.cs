
namespace CSAppBE.Web.Controllers
{
    using CSAppBE.Web.Data;
    using CSAppBE.Web.Data.Entities;
    using CSAppBE.Web.Data.Repositories;
    using CSAppBE.Web.Helpers;
    using CSAppBE.Web.Models;
    using CSWebAfip;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    [Authorize]
    public class CommunicationsController : Controller
    {
        private readonly ICommunicationRepository communicRepo;
        private readonly IClientRepository clientRepo;
        private readonly IUserHelper userHelper;

        public CommunicationsController(ICommunicationRepository communicRepo, IClientRepository clientRepo, IUserHelper userHelper)
        {
            this.communicRepo = communicRepo;
            this.clientRepo = clientRepo;
            this.userHelper = userHelper;
        }
        public IActionResult Index(string cuit)
        {
            return View(this.communicRepo.GetByClientId(cuit));
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> ConsultarComunicaciones(string cuit)
        {
            if (ModelState.IsValid)
            {
                var oWsP = new VEConsumer();
                var user = this.userHelper.GetUserWithCertificateByEmail(this.User.Identity.Name);
                if (user != null)
                {
                    var certificateData = user.Certificate.Data;
                    string lstrPassword = "catedral";
                    string lstrCuit = cuit;
                    try
                    {
                        var paginatedResponse = oWsP.consultarComunicaciones(false, certificateData, lstrPassword, lstrCuit);
                        foreach (var communic in paginatedResponse.items)
                        {
                            if (this.communicRepo.GetByCommunicationId(communic.idComunicacion) == null)
                            {
                                var communication = new Communication
                                {
                                    CommunicationId = communic.idComunicacion,
                                    Cuit = communic.cuitDestinatario.ToString(),
                                    PublishedDate = Convert.ToDateTime(communic.fechaPublicacion),
                                    DueDate = Convert.ToDateTime(communic.fechaVencimiento),
                                    Status = communic.estado,
                                    Attach = communic.tieneAdjunto,
                                    StatusDesc = communic.estadoDesc,
                                    PublicSystemId = communic.sistemaPublicador.ToString(),
                                    PublicSystemDesc = communic.sistemaPublicadorDesc,
                                    Ref1 = communic.referencia1,
                                    Ref2 = communic.referencia2,
                                    Subject = communic.asunto,
                                    Client = this.clientRepo.GetByCUIT(cuit)
                                };

                                await this.communicRepo.CreateAsync(communication);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }                    
                }

                return RedirectToAction("Index", new { cuit = cuit });
            }
            return View("Error");
        }

        public IActionResult Detail(long idCom)
        {
            if (ModelState.IsValid)
            {
                var com = this.communicRepo.GetByCommunicationId(idCom);
                if (com != null)
                {
                    var oWsP = new VEConsumer();
                    var user = this.userHelper.GetUserWithCertificateByEmail(this.User.Identity.Name);
                    if (user != null)
                    {
                        var certificateData = user.Certificate.Data;
                        string lstrPassword = "catedral";
                        string lstrCuit = com.Cuit;
                        var fileName = string.Empty;
                        byte[] data = null;
                        try
                        {
                            var communication = oWsP.ConsumirComunicacion(certificateData, lstrPassword, lstrCuit, idCom);
                            if (communication.adjuntos.Length != 0)
                            {
                                var adj = communication.adjuntos[0];
                                fileName = adj.filename;
                                data = adj.content;
                            }

                            var comViewModel = new CommunicationViewModel
                            {
                                Data = data,
                                FileName = fileName,
                                Message = communication.mensaje,
                                Subject = communication.asunto
                            };
                            return View(comViewModel);

                        }
                        catch (Exception e)
                        {
                            
                        }
                        
                        
                    }
                }
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult Detail(CommunicationViewModel comm)
        {
            return File(comm.Data, "application/force-download", comm.FileName);
        }
    }
}