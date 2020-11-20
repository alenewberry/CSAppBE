
namespace CSAppBE.Web.Controllers
{
    using CSAppBE.Web.Data;
    using CSAppBE.Web.Data.Entities;
    using CSAppBE.Web.Data.Repositories;
    using CSAppBE.Web.Helpers;
    using CSWebAfip;
    using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Index()
        {
            return View(this.communicRepo.GetAll());
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
                var certificateData = user.Certificate.Data;
                string lstrPassword = "catedral";
                string lstrCuit = cuit;
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
                            StatusDesc = communic.estadoDesc,
                            PublicSystemId = communic.sistemaPublicador.ToString(),
                            PublicSystemDesc = communic.sistemaPublicadorDesc,
                            Ref1 = communic.referencia1,
                            Ref2 = communic.referencia2,
                            Subject = communic.asunto,
                            Client = this.clientRepo.GetByCUIT(cuit)
                        };
                        
                        //com = oWsP.ConsumirComunicacion(certificateData, lstrPassword, lstrCuit, communic.idComunicacion);

                        await this.communicRepo.CreateAsync(communication);
                    }

                }

                return RedirectToAction(nameof(Index));
            }
            return View("Error");
        }

        public IActionResult ConsumirComunicacion(long idCom)
        {
            if (ModelState.IsValid)
            {
                var com = this.communicRepo.GetByCommunicationId(idCom);
                var oWsP = new VEConsumer();
                var user = this.userHelper.GetUserWithCertificateByEmail(this.User.Identity.Name);
                var certificateData = user.Certificate.Data;
                string lstrPassword = "catedral";
                string lstrCuit = com.Cuit;
                var paginatedResponse = oWsP.ConsumirComunicacion(certificateData, lstrPassword, lstrCuit, idCom);
                var msj = paginatedResponse.mensaje;
                var adj = paginatedResponse.adjuntos;
                var jsonResponse = JsonConvert.SerializeObject(paginatedResponse);
            }

                return View("Index");
        }
    }
}