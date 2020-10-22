namespace CSWebAfip
{
    using System;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.Pkcs;
    using System.Security.Cryptography.X509Certificates;
    using ar.gov.afip.servicios1;
    using ar.gov.afip.servicios1bonos;
    using ar.gob.afip.serviciosjava;
    using VEConsumerService;
    using System.ServiceModel;
    using WcfCoreMtomEncoder;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Consume los siguientes metodos del web service de factura electrónica de la Afip:
    /// 
    /// FEAutRequest 
    /// Dado un lote de comprobantes retorna el mismo autorizado con el CAE otorgado. 
    ///
    /// FEConsultaCAERequest 
    /// Consulta el CAE. 
    ///
    /// FEDummy 
    /// Metodo dummy para verificacion basica de funcionamiento. 
    ///
    /// FERecuperaLastCMPRequest 
    /// Retorna el ultimo comprobante autorizado para el tipo de comprobante /cuit / punto de venta ingresado. 
    ///
    /// FERecuperaQTYRequest 
    /// Retorna la cantidad maxima de registros de detalle que puede tener una invocacion al FEAutorizarRequest. 
    ///
    /// FEUltNroRequest 
    /// Retorna el ultimo número de Request.         
    ///</summary>     

    [ProgId("CatedralSoftware.wsAfipFE")]
    [Guid("3BBFFA7F-37E6-4aa7-B3CD-2D2C58689548")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipFE
    {
        #region Fields

        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEHomologacion = "https://wswhomo.afip.gov.ar/wsfe/service.asmx?WSDL";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEProduccion = "https://servicios1.afip.gov.ar/wsfe/service.asmx?WSDL";
        private string strWebServiceName = "wsfe";

        private string strCuit;
        private string strCertificateFileName;
        private byte[] dataCertificate;
        private string strPassword;
        private bool bIsHomologacion = false;

        private ar.gov.afip.servicios1.ServiceSoapClient oCSwsAfipFE = new ar.gov.afip.servicios1.ServiceSoapClient("https://wswhomo.afip.gov.ar/wsfe/service.asmx?WSDL");
        private ar.gov.afip.servicios1.FEAuthRequest oCSwsAfipAuthRequest = new ar.gov.afip.servicios1.FEAuthRequest();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        private bool bIsConnect = false;

        private int iPerCode;
        private string strPerrMsg;

        private ar.gov.afip.servicios1.FEDetalleRequest[] arrayFEDetalleRequest = null;
        private ar.gov.afip.servicios1.FEResponse oCSwsFEResponse;
        private string sAppServer;
        private string sAuthServer;
        private string sDBServer;
        private int iCantidadReg = 0;
        private long lId = 0;
        private bool bPrestaServ = false;
        private string sFechaCae;
        private string sCabResultado;
        private string sCabMotivo;
        private string sReproceso;
        private string sDetCae;
        private string sDetResultado;
        private string sDetMotivo;
        private string sDetFecVtoCae;
        private int iDetIndex = 0;



        #endregion

        #region Properties

        public string AppServer
        {
            get { return sAppServer; }
        }

        public string AuthServer
        {
            get { return sAuthServer; }
        }

        public string DBServer
        {
            get { return sDBServer; }
        }

        public int DetalleIndice
        {
            get { return iDetIndex; }
        }

        public int CantidadReg
        {
            get { return iCantidadReg; }
        }

        public int Id
        {
            get { return Convert.ToInt32(lId); }
        }

        public bool PrestaServicio
        {
            get { return bPrestaServ; }
        }

        public string FechaCae
        {
            get { return sFechaCae; }
        }

        public string CabeceraResultado
        {
            get { return sCabResultado; }
        }

        public string CabeceraMotivo
        {
            get { return sCabMotivo; }
        }

        public string Reproceso
        {
            get { return sReproceso; }
        }

        public string cae
        {
            get { return sDetCae; }
        }

        public string DetalleResultado
        {
            get { return sDetResultado; }
        }

        public string DetalleMotivo
        {
            get { return sDetMotivo; }
        }

        public string FechaVtoCae
        {
            get { return sDetFecVtoCae; }
        }

        public string URLWebServiceAAHomologacion
        {
            get { return strURLWebServiceAAHomologacion; }
            set { strURLWebServiceAAHomologacion = value; }
        }

        public string URLWebServiceFEHomologacion
        {
            get { return strURLWebServiceFEHomologacion; }
            set { strURLWebServiceFEHomologacion = value; }
        }

        public string WebServiceName
        {
            get { return strWebServiceName; }
            set { strWebServiceName = value; }
        }

        public string URLWebServiceAAProduccion
        {
            get { return strURLWebServiceAAProduccion; }
            set { strURLWebServiceAAProduccion = value; }
        }

        public string URLWebServiceFEProduccion
        {
            get { return strURLWebServiceFEProduccion; }
            set { strURLWebServiceFEProduccion = value; }
        }

        public string CertificateFileName
        {
            get { return strCertificateFileName; }
        }

        public byte[] DataCertificate
        {
            get { return dataCertificate; }
        }

        public string Password
        {
            get { return strPassword; }
        }

        public bool IsHomologacion
        {
            get { return bIsHomologacion; }
        }

        public string Cuit
        {
            get { return strCuit; }
        }

        public int PerCode
        {
            get { return iPerCode; }
            set { iPerCode = value; }
        }

        public string PerrMsg
        {
            get { return strPerrMsg; }
            set { strPerrMsg = value; }
        }

        public bool IsConnect
        {
            get { return bIsConnect; }
        }

        #endregion

        #region Methods

        public CSwsAfipFE() { } // Constructor

        /// <summary> 
        /// Se conecta WSFE de la Afip. 
        /// </summary> 
        /// <param name="artbIsHomologacion">Indica si la conexión se efectúa sobre el web service de homologación.</param> 
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <param name="argStrPassword">Password para obtener la clave privada desde el certificado(PKCS#12 = ((Certificate + Clave Pública + Firma AC) + Clave Privada).</param>
        /// <returns>Verdadero si la conexión fue exitosa o ya estaba conectado, falso si no lo fue.</returns> 
        /// <remarks>Lanza excepciones en caso de errores.</remarks> 
        [ComVisible(true)]
        public bool CSwsAfipFEConnect(bool argbIsHomologacion, string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            // Check parameters and asigns to fields
            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                throw new Exception("Deben especificarse los parámetros bIsHomologación, strCuit y strCertificateFileName.");
            }

            strCuit = argStrCuit;
            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;
            bIsHomologacion = argbIsHomologacion;

            //if (bIsHomologacion)
            //{
            //    oCSwsAfipFE.Url = strURLWebServiceFEHomologacion;
            //}
            //else
            //{
            //    oCSwsAfipFE.Url = strURLWebServiceFEProduccion;
            //}

            // Get Tickets Access Requirement
            try
            {
                bIsConnect = CSwsAfipAAGetTRA();
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar conectarce al servicio: " + ex.Message);
                bIsConnect = false;
            }

            return bIsConnect;
        }

        /// <summary>
        /// Se conecta WSFE de la Afip.
        /// </summary>
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <returns></returns>
        public bool CSwsAfipFEConnect(string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            return CSwsAfipFEConnect(false, argStrCuit, argStrCertificateFileName, argStrPassword);
        }

        /// <summary>
        /// Verifica el estado de los servicios del web service de factura electrónica.
        /// </summary>
        /// <returns>Retorna Verdadero si todo esta Ok, falso en caso contrario.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEDummy()
        {
            ar.gov.afip.servicios1.DummyResponse dummyResponse = new ar.gov.afip.servicios1.DummyResponse();
            bool bResult = false;

            try
            {
                dummyResponse = oCSwsAfipFE.FEDummy();
                sAppServer = dummyResponse.appserver;
                sAuthServer = dummyResponse.authserver;
                sDBServer = dummyResponse.dbserver;

                bResult = (dummyResponse.appserver == "OK" && dummyResponse.authserver == "OK" && dummyResponse.dbserver == "OK");
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar los estados de los servicios de la Afip: " + ex.Message);
            }
            return bResult;

        }

        /// <summary>
        /// Retorna el último ID enviado por el contribuyente.. 
        /// </summary>
        /// <returns>Ultimo número de transacción, o -1 si hubo errores.</returns>
        [ComVisible(true)]
        public int CSwsAfipFEUltNroRequest()
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1.FEUltNroResponse oFEUltNroResponse = new ar.gov.afip.servicios1.FEUltNroResponse();
            int iResult = -1;

            try
            {
                oFEUltNroResponse = oCSwsAfipFE.FEUltNroRequest(oCSwsAfipAuthRequest);
                iPerCode = oFEUltNroResponse.RError.percode;
                strPerrMsg = oFEUltNroResponse.RError.perrmsg;
                iResult = Convert.ToInt32(oFEUltNroResponse.nro.value);

            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return iResult;

        }

        /// <summary>
        /// Consulta de validez de comprobante electrónico recibido.
        /// </summary>
        /// <param name="argStrCuitEmisor"> Cuit del emisor del comprobante.</param>
        /// <param name="argiTipoCbte">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <param name="argiNroComprob">Número de comprobante.</param>
        /// <param name="argdImpTotal">Importe total del comprobante.</param>
        /// <param name="argStrCAE">CAE del comprobante.</param>
        /// <param name="argStrFechaComprob">Fecha del comprobante.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEConsultaCAERequest(string argStrCuitEmisor, int argiTipoCbte, int argiPuntoVta, int argiNroComprob, double argdImpTotal, string argStrCAE, string argStrFechaComprob)
        {
            if (!CSwsAfipCheck()) { return false; }

            ar.gov.afip.servicios1.FEConsultaCAEResponse oFEConsultaCAEResponse = new ar.gov.afip.servicios1.FEConsultaCAEResponse();
            ar.gov.afip.servicios1.FEConsultaCAEReq oargCAERequest = new ar.gov.afip.servicios1.FEConsultaCAEReq();
            bool bResult = false;

            try
            {
                oargCAERequest.cuit_emisor = Convert.ToInt64(argStrCuitEmisor);
                oargCAERequest.tipo_cbte = argiTipoCbte;
                oargCAERequest.punto_vta = argiPuntoVta;
                oargCAERequest.cbt_nro = Convert.ToInt64(argiNroComprob);
                oargCAERequest.fecha_cbte = argStrFechaComprob;
                oargCAERequest.imp_total = argdImpTotal;
                oargCAERequest.cae = argStrCAE;

                oFEConsultaCAEResponse = oCSwsAfipFE.FEConsultaCAERequest(oCSwsAfipAuthRequest, oargCAERequest);
                iPerCode = oFEConsultaCAEResponse.RError.percode;
                strPerrMsg = oFEConsultaCAEResponse.RError.perrmsg;
                bResult = (oFEConsultaCAEResponse.Resultado == 1);
            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return bResult;
        }

        /// <summary>
        /// Retorna el último número otorgado para el comprobante. En caso de no poseer ningún comprobante autorizado se devuelve un 0.
        /// </summary>
        /// <param name="argiTipoCbte">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public int CSwsAfipFERecuperaLastCMPRequest(int argiTipoCbte, int argiPuntoVta)
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1.FERecuperaLastCMPResponse oFERecuperaLastCMPResponse = new ar.gov.afip.servicios1.FERecuperaLastCMPResponse();
            ar.gov.afip.servicios1.FELastCMPtype oargTCMP = new ar.gov.afip.servicios1.FELastCMPtype();
            int iResult = -1;

            try
            {
                oargTCMP.TipoCbte = argiTipoCbte;
                oargTCMP.PtoVta = argiPuntoVta;
                oFERecuperaLastCMPResponse = oCSwsAfipFE.FERecuperaLastCMPRequest(oCSwsAfipAuthRequest, oargTCMP);
                iPerCode = oFERecuperaLastCMPResponse.RError.percode;
                strPerrMsg = oFERecuperaLastCMPResponse.RError.perrmsg;
                iResult = oFERecuperaLastCMPResponse.cbte_nro;
            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return iResult;
        }

        /// <summary>
        /// Retorna la cantidad máxima de registros a incluir en el detalle del servicio Facturador.
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        public int CSwsAfipFERecuperaQTYRequest()
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1.FERecuperaQTYResponse oFERecuperaQTYResponse = new ar.gov.afip.servicios1.FERecuperaQTYResponse();
            int iResult = -1;

            try
            {
                oFERecuperaQTYResponse = oCSwsAfipFE.FERecuperaQTYRequest(oCSwsAfipAuthRequest);
                iPerCode = oFERecuperaQTYResponse.RError.percode;
                strPerrMsg = oFERecuperaQTYResponse.RError.perrmsg;
                iResult = oFERecuperaQTYResponse.qty.value;
            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return iResult;
        }

        /// <summary>
        /// Inicializa la cabecera del comprobante/lote de la solicitud.
        /// </summary>
        /// <param name="argiId">Id de la transacción.</param>
        /// <param name="argiCdadReg">Cantidad de registros que formarán el detalle.</param>
        /// <param name="argbPrestaServ">Indica si la empresa es prestadora de servicios.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFECabeceraRequest(int argiId, int argiCdadReg, bool argbPrestaServ)
        {
            if (!CSwsAfipCheck()) { return false; }

            bool bResult = false;

            try
            {
                if (arrayFEDetalleRequest != null)
                {
                    arrayFEDetalleRequest = null;
                }
                arrayFEDetalleRequest = new ar.gov.afip.servicios1.FEDetalleRequest[argiCdadReg];
                iDetIndex = 0;
                iCantidadReg = argiCdadReg;
                lId = Convert.ToInt64(argiId);
                bPrestaServ = argbPrestaServ;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al inicializar la cabecera de la solicitud: " + ex.Message);
            }

            return bResult;

        }

        /// <summary>
        /// Ingresa un registro en el detalle de la solicitud de comprobante/lote.
        /// </summary>
        /// <param name="argiIndex">Indice de registro del detalle.</param>
        /// <param name="argiTipoDoc">Código de documento identificatorio del comprador.</param>
        /// <param name="argiNroDoc">Número de identificación del comprador.</param>
        /// <param name="argiTipoCbte">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <param name="argiCbtDesde">Número de comprobante desde.</param>
        /// <param name="argiCbtHasta">Número de comprobante hasta.</param>
        /// <param name="argdImpTotal">Importe total de la operación o lote.</param>
        /// <param name="argdImpTotConc">Importe total de conceptos que no integran el precio neto gravado.</param>
        /// <param name="argdImpNeto">Importe neto gravado.</param>
        /// <param name="argdImptoLiq">Importe liquidado.</param>
        /// <param name="argdImptoLiqRni">Impuesto liquidado a RNI o percepción a no categorizados.</param>
        /// <param name="argdImpOpEx">Importe de operaciones exentas.</param>
        /// <param name="argStrFechaCbte">Fecha del comprobante (yyyymmdd).</param>
        /// <param name="argStrFechaServDesde">Fecha de inicio del servicio a facturar (yyyymmdd).</param>
        /// <param name="argStrFechaServHasta">Fecha de fin del servicio a facturar (yyyymmdd).</param>
        /// <param name="argStrFechaVencPago">Fecha de vto de la factura (no es el vencimiento del CAE). (yyyymmdd).</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEDetalleRequest(int argiIndex, int argiTipoDoc, long arglNroDoc, int argiTipoCbte, int argiPuntoVta,
                                            int argiCbtDesde, int argiCbtHasta, double argdImpTotal, double argdImpTotConc,
                                            double argdImpNeto, double argdImptoLiq, double argdImptoLiqRni, double argdImpOpEx,
                                            string argStrFechaCbte, string argStrFechaServDesde, string argStrFechaServHasta,
                                            string argStrFechaVencPago)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCantidadReg == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
            }

            if (argiIndex > iCantidadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle.");
            }

            bool bResult = false;

            try
            {
                ar.gov.afip.servicios1.FEDetalleRequest oFEDetalleRequest = new ar.gov.afip.servicios1.FEDetalleRequest();

                oFEDetalleRequest.tipo_doc = argiTipoDoc;
                oFEDetalleRequest.nro_doc = arglNroDoc;
                oFEDetalleRequest.tipo_cbte = argiTipoCbte;
                oFEDetalleRequest.punto_vta = argiPuntoVta;
                oFEDetalleRequest.cbt_desde = Convert.ToInt64(argiCbtDesde);
                oFEDetalleRequest.cbt_hasta = Convert.ToInt64(argiCbtHasta);
                oFEDetalleRequest.imp_total = argdImpTotal;
                oFEDetalleRequest.imp_tot_conc = argdImpTotConc;
                oFEDetalleRequest.imp_neto = argdImpNeto;
                oFEDetalleRequest.impto_liq = argdImptoLiq;
                oFEDetalleRequest.impto_liq_rni = argdImptoLiqRni;
                oFEDetalleRequest.imp_op_ex = argdImpOpEx;
                oFEDetalleRequest.fecha_cbte = argStrFechaCbte;
                oFEDetalleRequest.fecha_serv_desde = argStrFechaServDesde;
                oFEDetalleRequest.fecha_serv_hasta = argStrFechaServHasta;
                oFEDetalleRequest.fecha_venc_pago = argStrFechaVencPago;
                arrayFEDetalleRequest[(argiIndex - 1)] = oFEDetalleRequest;
                bResult = true;

            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al agregar el comprobante: " + ex.Message);
            }

            return bResult;

        }

        /// <summary>
        /// Dado un lote de comprobantes retorna el mismo autorizado con el CAE otorgado
        /// </summary>
        /// <returns>Verdadero si la operación es exitosa, caso contrario Falso.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEAutRequest()
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCantidadReg == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
                return false;
            }

            bool bResult = false;

            try
            {
                ar.gov.afip.servicios1.FERequest oFERequest = new ar.gov.afip.servicios1.FERequest();
                ar.gov.afip.servicios1.FECabeceraRequest oFECabeceraRequest = new ar.gov.afip.servicios1.FECabeceraRequest();
                oCSwsFEResponse = new ar.gov.afip.servicios1.FEResponse();

                // Header
                oFECabeceraRequest.id = lId;
                oFECabeceraRequest.cantidadreg = iCantidadReg;
                oFECabeceraRequest.presta_serv = Convert.ToInt32(bPrestaServ);
                oFERequest.Fecr = oFECabeceraRequest;

                // Details
                oFERequest.Fedr = arrayFEDetalleRequest;

                // Request
                oCSwsFEResponse = oCSwsAfipFE.FEAutRequest(oCSwsAfipAuthRequest, oFERequest);
                iPerCode = oCSwsFEResponse.RError.percode;
                strPerrMsg = oCSwsFEResponse.RError.perrmsg;

                if (oCSwsFEResponse.FecResp != null)
                {
                    iDetIndex = 1;
                    iCantidadReg = oCSwsFEResponse.FecResp.cantidadreg;
                    lId = oCSwsFEResponse.FecResp.id;
                    bPrestaServ = Convert.ToBoolean(oCSwsFEResponse.FecResp.presta_serv);
                    sCabResultado = oCSwsFEResponse.FecResp.resultado;
                    sCabMotivo = oCSwsFEResponse.FecResp.motivo;
                    sReproceso = oCSwsFEResponse.FecResp.reproceso;
                    sFechaCae = oCSwsFEResponse.FecResp.fecha_cae;
                    sDetCae = oCSwsFEResponse.FedResp[0].cae;
                    sDetResultado = oCSwsFEResponse.FedResp[0].resultado;
                    sDetMotivo = oCSwsFEResponse.FedResp[0].motivo;
                    sDetFecVtoCae = oCSwsFEResponse.FedResp[0].fecha_vto;
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al enviar la solicitud de autorización del comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Setea las propiedades internas al comprobante en el lote, indicado por argIndex.
        /// </summary>
        /// <param name="argIndex">Indice dentro del lote solicitado.</param>
        /// <returns>Verdadero si la operación fue exitosa.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEAutRequest(int argIndex)
        {
            if (iDetIndex == 0)
            {
                throw new Exception("No se ha enviado la solicitud");
                return false;
            }

            if (argIndex > iCantidadReg)
            {
                throw new Exception("El índice indicado se encuentra fuera de los intervalos del lote de comprobantes.");
                return false;
            }

            bool bResult = false;

            try
            {
                // Request

                if (oCSwsFEResponse.FecResp != null)
                {
                    iDetIndex = argIndex;
                    sDetResultado = oCSwsFEResponse.FedResp[(iDetIndex - 1)].resultado;
                    sDetMotivo = oCSwsFEResponse.FedResp[(iDetIndex - 1)].motivo;
                    sDetFecVtoCae = oCSwsFEResponse.FedResp[(iDetIndex - 1)].fecha_vto;
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al setear el indice del lote de comprobantes: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Check interno de la clase
        /// </summary>
        /// <returns>Verdadero si todo Ok.</returns>
        [ComVisible(false)]
        private bool CSwsAfipCheck()
        {
            if (!bIsConnect)
            {
                throw new Exception("No esta conectado al web service de negocio.");
                return false;
            }
            if (!IsValidAuthRequest())
            {
                throw new Exception("El Ticket de Requerimiento de Acceso ha expirado.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene un TRA del ws de autenticación y autorización de la Afip.
        /// </summary>
        /// <returns>Verdadero si se obtuvo con éxito un Ticket de requermiento de acceso.</returns>    
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = bIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    tAuthRequestExpiration = wsAfipAA.ExpirationDatetime;
                    oCSwsAfipAuthRequest.cuit = Convert.ToInt64(strCuit);
                    oCSwsAfipAuthRequest.Sign = wsAfipAA.Sign;
                    oCSwsAfipAuthRequest.Token = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    oCSwsAfipAuthRequest.cuit = 0;
                    oCSwsAfipAuthRequest.Sign = "";
                    oCSwsAfipAuthRequest.Token = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }

        }

        /// <summary>
        /// Chequea el Ticket de requerimiento de acceso al ws de negocio.
        /// </summary>
        /// <returns>Verdadero si el Ticket de Requerimiento de acceso se ha obtenido y no ha expirado.</returns>
        [ComVisible(true)]
        public bool IsValidAuthRequest()
        {
            return (tAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10));
        }

        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it

            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // And create the 'Control' key - this allows it to show up in 

            // the ActiveX control container 

            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry - needed if not string named and GACced.

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            // Finally close the main key

            k.Close();
        }

        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open HKCR\CLSID\{guid} for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control' key, but don't throw an exception if it does not exist

            k.DeleteSubKey("Control", false);

            // Next open up InprocServer32

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key, again not throwing if missing 

            k.DeleteSubKey("CodeBase", false);

            // Finally close the main key 

            k.Close();
        }

        #endregion

    }


    [ProgId("CatedralSoftware.wsAfipFEX")]
    [Guid("E87B628A-0B18-4e62-B8FF-6CA1A64FFD1B")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipFEX
    {
        #region Fields

        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEHomologacion = "https://wswhomo.afip.gov.ar/wsfex/service.asmx?WSDL ";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEProduccion = "https://servicios1.afip.gov.ar/wsfex/service.asmx?WSDL";
        private string strWebServiceName = "wsfex";

        private string strCuit;
        private string strCertificateFileName;
        private string strPassword;
        private bool bIsHomologacion = false;

        private ar.gov.afip.servicios1fex.ServiceSoapClient oCSwsAfipFEX = new ar.gov.afip.servicios1fex.ServiceSoapClient();
        private ar.gov.afip.servicios1fex.ClsFEXAuthRequest oCSwsAfipAuthRequest = new ar.gov.afip.servicios1fex.ClsFEXAuthRequest();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        private bool bIsConnect = false;

        private int iPerCode;
        private string strPerrMsg;

        private ar.gov.afip.servicios1fex.Item[] arrayFEXDetalleItems = null;
        private ar.gov.afip.servicios1fex.Permiso[] arrayFEXPermisos = null;
        private ar.gov.afip.servicios1fex.Cmp_asoc[] arrayFEXCmp_asoc = null;

        private int iCountItem = 0;
        private int iCountPermiso = 0;
        private int iCountCmp_Asoc = 0;

        private string sAppServer;
        private string sAuthServer;
        private string sDBServer;

        private long lId = 0;
        private string sFechaComprobante;
        private int iPuntoVenta;
        private long lNroComprob;
        private int iTipo_Expo;
        private string sPermisoExistente;
        private int iPaisDestino;
        private string sCliente;
        private long lCuitPaisCliente;
        private string sDomicilioCliente;
        private string sNroDocumCliente;
        private string sMonedaId;
        private Double dCotizacMon;
        private string sObsComerciales;
        private Double dImporteTotal;
        private string sObs;
        private string sFormaPago;
        private string sIncoTerms;
        private int iIdiomaComprob;
        private string sCae;
        private string sFechaCae;
        private string sResultado;
        private string sMotivo;
        private string sReproceso;
        private int iEventCode = 0;
        private string sEventMsg;
        private string sFecha_pago = "";


        #endregion

        #region Properties

        public string AppServer
        {
            get { return sAppServer; }
        }

        public string AuthServer
        {
            get { return sAuthServer; }
        }

        public string DBServer
        {
            get { return sDBServer; }
        }

        public int Id
        {
            get { return Convert.ToInt32(lId); }
        }

        public string FechaComprobante
        {
            get { return sFechaComprobante; }
        }

        public int PuntoDeVenta
        {
            get { return iPuntoVenta; }
        }

        public long NroComprobante
        {
            get { return lNroComprob; }
        }

        public int TipoExportacion
        {
            get { return iTipo_Expo; }
        }

        public string PermisoExistente
        {
            get { return sPermisoExistente; }
        }

        public int PaisDestino
        {
            get { return iPaisDestino; }
        }

        public string Cliente
        {
            get { return sCliente; }
        }

        public long CuitPais
        {
            get { return lCuitPaisCliente; }
        }

        public string Domicilio
        {
            get { return sDomicilioCliente; }
        }

        public string NroDocumCliente
        {
            get { return sNroDocumCliente; }
        }

        public string MonedaId
        {
            get { return sMonedaId; }
        }

        public Double Cotizacion
        {
            get { return dCotizacMon; }
        }

        public string ObsComerciales
        {
            get { return sObsComerciales; }
        }

        public Double Total
        {
            get { return dImporteTotal; }
        }

        public string Observaciones
        {
            get { return sObs; }
        }

        public string FormaPago
        {
            get { return sFormaPago; }
        }

        public string IncoTerms
        {
            get { return sIncoTerms; }
        }

        public int IdiomaComprobante
        {
            get { return iIdiomaComprob; }
        }

        public string Cae
        {
            get { return sCae; }
        }

        public string FechaCAE
        {
            get { return sFechaCae; }
        }

        public string Resultado
        {
            get { return sResultado; }
        }

        public string Motivo
        {
            get { return sMotivo; }
        }

        public string Reproceso
        {
            get { return sReproceso; }
        }

        public string URLWebServiceAAHomologacion
        {
            get { return strURLWebServiceAAHomologacion; }
            set { strURLWebServiceAAHomologacion = value; }
        }

        public string URLWebServiceFEHomologacion
        {
            get { return strURLWebServiceFEHomologacion; }
            set { strURLWebServiceFEHomologacion = value; }
        }

        public string WebServiceName
        {
            get { return strWebServiceName; }
            set { strWebServiceName = value; }
        }

        public string URLWebServiceAAProduccion
        {
            get { return strURLWebServiceAAProduccion; }
            set { strURLWebServiceAAProduccion = value; }
        }

        public string URLWebServiceFEProduccion
        {
            get { return strURLWebServiceFEProduccion; }
            set { strURLWebServiceFEProduccion = value; }
        }

        public string CertificateFileName
        {
            get { return strCertificateFileName; }
        }

        public string Password
        {
            get { return strPassword; }
        }

        public bool IsHomologacion
        {
            get { return bIsHomologacion; }
        }

        public string Cuit
        {
            get { return strCuit; }
        }

        public int PerCode
        {
            get { return iPerCode; }
            set { iPerCode = value; }
        }

        public string PerrMsg
        {
            get { return strPerrMsg; }
            set { strPerrMsg = value; }
        }

        public bool IsConnect
        {
            get { return bIsConnect; }
        }

        public int EventCode
        {
            get { return iEventCode; }
        }

        public string EventMsg
        {
            get { return sEventMsg; }
        }

        public string SFecha_pago
        {
            get { return sFecha_pago; }
            set { sFecha_pago = value; }
        }

        #endregion

        #region Methods

        public CSwsAfipFEX() { } // Constructor

        /// <summary> 
        /// Se conecta WSFEX de la Afip. 
        /// </summary> 
        /// <param name="artbIsHomologacion">Indica si la conexión se efectúa sobre el web service de homologación.</param> 
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <param name="argStrPassword">Password para obtener la clave privada desde el certificado(PKCS#12 = ((Certificate + Clave Pública + Firma AC) + Clave Privada).</param>
        /// <returns>Verdadero si la conexión fue exitosa o ya estaba conectado, falso si no lo fue.</returns> 
        /// <remarks>Lanza excepciones en caso de errores.</remarks> 
        [ComVisible(true)]
        public bool CSwsAfipFEXConnect(bool argbIsHomologacion, string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            // Check parameters and asigns to fields
            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                throw new Exception("Deben especificarse los parámetros bIsHomologación, strCuit y strCertificateFileName.");
                return false;
            }

            strCuit = argStrCuit;
            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;
            bIsHomologacion = argbIsHomologacion;

            //if (bIsHomologacion)
            //{
            //    oCSwsAfipFEX.Url = strURLWebServiceFEHomologacion;
            //}
            //else
            //{
            //    oCSwsAfipFEX.Url = strURLWebServiceFEProduccion;
            //}

            // Get Tickets Access Requirement
            try
            {
                bIsConnect = CSwsAfipAAGetTRA();
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al conectarce al servicio: " + ex.Message);
                bIsConnect = false;
            }

            return bIsConnect;
        }

        /// <summary>
        /// Se conecta WSFE de la Afip.
        /// </summary>
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <returns></returns>
        public bool CSwsAfipFEXConnect(string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            return CSwsAfipFEXConnect(false, argStrCuit, argStrCertificateFileName, argStrPassword);
        }

        /// <summary>
        /// Verifica el estado de los servicios del web service de factura electrónica.
        /// </summary>
        /// <returns>Retorna Verdadero si todo esta Ok, falso en caso contrario.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEXDummy()
        {
            ar.gov.afip.servicios1fex.DummyResponse dummyResponse = new ar.gov.afip.servicios1fex.DummyResponse();
            bool bResult = false;

            try
            {
                dummyResponse = oCSwsAfipFEX.FEXDummy();
                sAppServer = dummyResponse.AppServer;
                sAuthServer = dummyResponse.AuthServer;
                sDBServer = dummyResponse.DbServer;

                bResult = (dummyResponse.AppServer == "OK" && dummyResponse.AuthServer == "OK" && dummyResponse.DbServer == "OK");
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar los estados de los servicios de la Afip: " + ex.Message);
            }
            return bResult;

        }

        /// <sumary>
        /// Retorna el último ID enviado por el contribuyente
        /// </sumary>
        /// <return>Ultimo número de transacción, o -1 si hubo errores</return>
        [ComVisible(true)]
        public int CSWsAfipFEXGetLast_ID()
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fex.FEXResponse_LastID oFEXResponse_LastID = new ar.gov.afip.servicios1fex.FEXResponse_LastID();
            int iResult = -1;

            try
            {
                oFEXResponse_LastID = oCSwsAfipFEX.FEXGetLast_ID(oCSwsAfipAuthRequest);
                iPerCode = oFEXResponse_LastID.FEXErr.ErrCode;
                strPerrMsg = oFEXResponse_LastID.FEXErr.ErrMsg;

                if (oFEXResponse_LastID.FEXEvents != null)
                {
                    iEventCode = oFEXResponse_LastID.FEXEvents.EventCode;
                    sEventMsg = oFEXResponse_LastID.FEXEvents.EventMsg;
                }

                iResult = Convert.ToInt32(oFEXResponse_LastID.FEXResultGet.Id);
            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return iResult;

        }

        /// <summary>
        /// Recupera el CAE un comprobante autorizado. 
        /// </summary>
        /// <param name="argiTipoCbte">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <param name="argiNroComprob">Número de comprobante.</param>        
        /// <returns>Retorna verdadero si la operación es exitosa, falso si hubo errores.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEXGetCMP(int argsTipoCbte, int argsPuntoVta, int argiNroComprob)
        {

            if (!CSwsAfipCheck()) { return false; }

            ar.gov.afip.servicios1fex.ClsFEXGetCMP oFEXGetCMP = new ar.gov.afip.servicios1fex.ClsFEXGetCMP();
            ar.gov.afip.servicios1fex.FEXGetCMPResponse oFEXGetCMPResponse = new ar.gov.afip.servicios1fex.FEXGetCMPResponse();

            bool bResult = false;

            try
            {
                oFEXGetCMP.Tipo_cbte = (short)argsTipoCbte;
                oFEXGetCMP.Punto_vta = (short)argsPuntoVta;
                oFEXGetCMP.Cbte_nro = argiNroComprob;
                oFEXGetCMPResponse = oCSwsAfipFEX.FEXGetCMP(oCSwsAfipAuthRequest, oFEXGetCMP);
                iPerCode = oFEXGetCMPResponse.FEXErr.ErrCode;
                strPerrMsg = oFEXGetCMPResponse.FEXErr.ErrMsg;

                if (oFEXGetCMPResponse.FEXResultGet != null)
                {
                    lId = oFEXGetCMPResponse.FEXResultGet.Id;
                    sFechaComprobante = oFEXGetCMPResponse.FEXResultGet.Fecha_cbte;
                    iPuntoVenta = oFEXGetCMPResponse.FEXResultGet.Punto_vta;
                    lNroComprob = oFEXGetCMPResponse.FEXResultGet.Cbte_nro;
                    iTipo_Expo = oFEXGetCMPResponse.FEXResultGet.Tipo_expo;
                    sPermisoExistente = oFEXGetCMPResponse.FEXResultGet.Permiso_existente;
                    iPaisDestino = oFEXGetCMPResponse.FEXResultGet.Dst_cmp;
                    sCliente = oFEXGetCMPResponse.FEXResultGet.Cliente;
                    lCuitPaisCliente = oFEXGetCMPResponse.FEXResultGet.Cuit_pais_cliente;
                    sDomicilioCliente = oFEXGetCMPResponse.FEXResultGet.Domicilio_cliente;
                    sNroDocumCliente = oFEXGetCMPResponse.FEXResultGet.Id_impositivo;
                    sMonedaId = oFEXGetCMPResponse.FEXResultGet.Moneda_Id;
                    dCotizacMon = oFEXGetCMPResponse.FEXResultGet.Moneda_ctz;
                    sObsComerciales = oFEXGetCMPResponse.FEXResultGet.Obs_comerciales;
                    dImporteTotal = oFEXGetCMPResponse.FEXResultGet.Imp_total;
                    sObs = oFEXGetCMPResponse.FEXResultGet.Obs;
                    sFormaPago = oFEXGetCMPResponse.FEXResultGet.Forma_pago;
                    sIncoTerms = oFEXGetCMPResponse.FEXResultGet.Incoterms;
                    iIdiomaComprob = oFEXGetCMPResponse.FEXResultGet.Idioma_cbte;
                    sCae = oFEXGetCMPResponse.FEXResultGet.Cae;
                    sFechaCae = oFEXGetCMPResponse.FEXResultGet.Fecha_cbte_cae;
                    sResultado = oFEXGetCMPResponse.FEXResultGet.Resultado;
                    sMotivo = oFEXGetCMPResponse.FEXResultGet.Motivos_Obs;
                    SFecha_pago = oFEXGetCMPResponse.FEXResultGet.Fecha_pago;

                    if (oFEXGetCMPResponse.FEXEvents != null)
                    {
                        iEventCode = oFEXGetCMPResponse.FEXEvents.EventCode;
                        sEventMsg = oFEXGetCMPResponse.FEXEvents.EventMsg;
                    }

                    bResult = true;
                }

            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return bResult;
        }

        /// <summary>
        /// Retorna el último número otorgado para el comprobante. En caso de no poseer ningún comprobante autorizado se devuelve un 0.
        /// </summary>
        /// <param name="argiTipoCbte">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public int CSwsAfipFEXGetLast_CMP(int argiTipoCbte, int argiPuntoVta)
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fex.ClsFEX_LastCMP oFEX_LastCMP = new ar.gov.afip.servicios1fex.ClsFEX_LastCMP();
            ar.gov.afip.servicios1fex.FEXResponseLast_CMP oFEX_ResponseLast_CMP = new ar.gov.afip.servicios1fex.FEXResponseLast_CMP();

            int iResult = -1;

            try
            {
                oFEX_LastCMP.Cuit = oCSwsAfipAuthRequest.Cuit;
                oFEX_LastCMP.Sign = oCSwsAfipAuthRequest.Sign;
                oFEX_LastCMP.Token = oCSwsAfipAuthRequest.Token;
                oFEX_LastCMP.Tipo_cbte = (short)argiTipoCbte;
                oFEX_LastCMP.Pto_venta = (short)argiPuntoVta;
                oFEX_ResponseLast_CMP = oCSwsAfipFEX.FEXGetLast_CMP(oFEX_LastCMP);
                iPerCode = oFEX_ResponseLast_CMP.FEXErr.ErrCode;
                strPerrMsg = oFEX_ResponseLast_CMP.FEXErr.ErrMsg;

                if (oFEX_ResponseLast_CMP.FEXEvents != null)
                {
                    iEventCode = oFEX_ResponseLast_CMP.FEXEvents.EventCode;
                    sEventMsg = oFEX_ResponseLast_CMP.FEXEvents.EventMsg;
                }

                if (oFEX_ResponseLast_CMP.FEXResult_LastCMP != null)
                    iResult = Convert.ToInt32(oFEX_ResponseLast_CMP.FEXResult_LastCMP.Cbte_nro);

            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }

            return iResult;
        }

        /// <summary>
        /// Recupera la cotizacion de la moneda consultada
        /// </summary>
        /// <param name="argsMonedaId"> Id de la moneda a consultar</param>
        /// <return> Cotización de la moneda, -1 en caso de error</return>
        public Double CSwsAfipFEXGetPARAM_Ctz(string argsMonedaId)
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fex.FEXResponse_Ctz oFEXResponse_Ctz = new ar.gov.afip.servicios1fex.FEXResponse_Ctz();
            Double dResult = -1;

            try
            {
                oFEXResponse_Ctz = oCSwsAfipFEX.FEXGetPARAM_Ctz(oCSwsAfipAuthRequest, argsMonedaId);
                iPerCode = oFEXResponse_Ctz.FEXErr.ErrCode;
                strPerrMsg = oFEXResponse_Ctz.FEXErr.ErrMsg;

                if (oFEXResponse_Ctz.FEXEvents != null)
                {
                    iEventCode = oFEXResponse_Ctz.FEXEvents.EventCode;
                    sEventMsg = oFEXResponse_Ctz.FEXEvents.EventMsg;
                }

                if (oFEXResponse_Ctz.FEXResultGet != null)
                    dResult = oFEXResponse_Ctz.FEXResultGet.Mon_ctz;

            }
            catch (Exception ex)
            {
                iPerCode = 0;
                strPerrMsg = ex.Message;
            }


            return dResult;

        }

        /// <summary>
        /// Verifica la existencia del permiso de embarque.
        /// </summary>
        /// <param name="argsCodDespacho">Código de despacho.</param>
        /// <param name="argiPaisId">Id de pais.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEXCheck_Permiso(string argsCodDespacho, int argiPaisId)
        {
            if (!CSwsAfipCheck()) { return false; }

            ar.gov.afip.servicios1fex.FEXResponse_CheckPermiso oFEXResponse_CheckPermiso = new ar.gov.afip.servicios1fex.FEXResponse_CheckPermiso();
            bool bResult = false;

            try
            {
                oFEXResponse_CheckPermiso = oCSwsAfipFEX.FEXCheck_Permiso(oCSwsAfipAuthRequest, argsCodDespacho, argiPaisId);
                iPerCode = oFEXResponse_CheckPermiso.FEXErr.ErrCode;
                strPerrMsg = oFEXResponse_CheckPermiso.FEXErr.ErrMsg;

                if (oFEXResponse_CheckPermiso.FEXEvents != null)
                {
                    iEventCode = oFEXResponse_CheckPermiso.FEXEvents.EventCode;
                    sEventMsg = oFEXResponse_CheckPermiso.FEXEvents.EventMsg;
                }

                bResult = (oFEXResponse_CheckPermiso.FEXResultGet.Status == "OK");
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar la existencia del permiso de embarque: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Inicializa la cabecera del comprobante.
        /// </summary>
        /// <param name="argiId">Id de la transacción.</param>
        /// <param name="argiItem">Cantidad de item que formarán el detalle.</param>
        /// <param name="argiPermiso">Cantidad de permisos asociados al comprobante.</param>
        /// <param name="argiCmp_Asoc">Cantidad de comprobantes asociados al comprobante.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEXInit(int argiId, int argiItem, int argiPermiso, int argiCmp_Asoc)
        {
            if (!CSwsAfipCheck()) { return false; }

            bool bResult = false;

            try
            {
                if (arrayFEXDetalleItems != null)
                {
                    arrayFEXDetalleItems = null;
                }
                arrayFEXDetalleItems = new ar.gov.afip.servicios1fex.Item[argiItem];
                iCountItem = argiItem;

                if (arrayFEXPermisos != null)
                {
                    arrayFEXPermisos = null;
                }

                if (argiPermiso > 0)
                {
                    arrayFEXPermisos = new ar.gov.afip.servicios1fex.Permiso[argiPermiso];
                    iCountPermiso = argiPermiso;
                }

                if (arrayFEXCmp_asoc != null)
                {
                    arrayFEXCmp_asoc = null;
                }

                if (argiCmp_Asoc > 0)
                {
                    arrayFEXCmp_asoc = new ar.gov.afip.servicios1fex.Cmp_asoc[argiCmp_Asoc];
                    iCountCmp_Asoc = argiCmp_Asoc;
                }

                lId = Convert.ToInt64(argiId);
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inicializando la cabecera de la solicitud: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un registro en el detalle de la solicitud de comprobante/lote.
        /// </summary>
        /// <param name="argiIndex">Indice de registro del detalle.</param>
        /// <param name="argsProductoId">Código de producto.</param>
        /// <param name="argsDescrip">Descripción del producto.</param>
        /// <param name="argdCantidad">Cantidad.</param>
        /// <param name="argiUnidMed">Unidad de medida.</param>
        /// <param name="argdImpUnit">Importe unitario.</param>
        /// <param name="argdTotal">Total.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEX_Add_ItemDetalle(int argiIndex, string argsProductoId, string argsDescrip, Double argdCantidad, int argiUnidMed, Double argdImpUnit, Double argdTotal)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCountItem == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
                return false;
            }

            if (argiIndex > iCountItem)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle.");
                return false;
            }

            ar.gov.afip.servicios1fex.Item oFEXItem = new ar.gov.afip.servicios1fex.Item();
            bool bResult = false;

            try
            {
                oFEXItem.Pro_codigo = argsProductoId;
                oFEXItem.Pro_ds = argsDescrip;
                oFEXItem.Pro_umed = argiUnidMed;
                oFEXItem.Pro_precio_uni = argdImpUnit;
                oFEXItem.Pro_qty = argdCantidad;
                oFEXItem.Pro_total_item = argdTotal;

                arrayFEXDetalleItems[(argiIndex - 1)] = oFEXItem;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el item de la factura: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa Permiso a la colección de permisos.
        /// </summary>
        /// <param name="argiIndex">Indice en el detalle de permisos.</param>
        /// <param name="argsCodDespacho">Código de despacho.</param>
        /// <param name="argiPaisId">Id de pais.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEX_Add_Permiso(int argiIndex, string argsCodDespacho, int argiPaisId)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCountItem == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
                return false;
            }

            if (argiIndex > iCountPermiso)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de los permisos.");
                return false;
            }

            ar.gov.afip.servicios1fex.Permiso oFEXPermiso = new ar.gov.afip.servicios1fex.Permiso();
            ar.gov.afip.servicios1fex.FEXResponse_CheckPermiso oFEXResponse_CheckPermiso = new ar.gov.afip.servicios1fex.FEXResponse_CheckPermiso();
            bool bResult = false;

            try
            {
                oFEXResponse_CheckPermiso = oCSwsAfipFEX.FEXCheck_Permiso(oCSwsAfipAuthRequest, argsCodDespacho, argiPaisId);
                iPerCode = oFEXResponse_CheckPermiso.FEXErr.ErrCode;
                strPerrMsg = oFEXResponse_CheckPermiso.FEXErr.ErrMsg;

                if (oFEXResponse_CheckPermiso.FEXEvents != null)
                {
                    iEventCode = oFEXResponse_CheckPermiso.FEXEvents.EventCode;
                    sEventMsg = oFEXResponse_CheckPermiso.FEXEvents.EventMsg;
                }

                if (oFEXResponse_CheckPermiso.FEXResultGet.Status == "OK")
                {
                    oFEXPermiso.Id_permiso = argsCodDespacho;
                    oFEXPermiso.Dst_merc = argiPaisId;
                    arrayFEXPermisos[(argiIndex - 1)] = oFEXPermiso;
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el permiso en la factura: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiIndex">Indice en el detalle comprobantes asociados.</param>
        /// <param name="argiCbte_Tipo">Tipo de comprobante.</param>
        /// <param name="argiPunto_Vta">Punto de venta.</param>
        /// <param name="argiNroComprob">Nro de comprobante.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEX_Add_Cmp_Asoc(int argiIndex, int argiCbte_Tipo, int argiPunto_Vta, int argiNroComprob)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCountItem == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
                return false;
            }

            if (argiIndex > iCountCmp_Asoc)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de los comprobantes asociados.");
                return false;
            }

            ar.gov.afip.servicios1fex.Cmp_asoc oFEXCmp_Asoc = new ar.gov.afip.servicios1fex.Cmp_asoc();
            bool bResult = false;

            try
            {
                oFEXCmp_Asoc.CBte_tipo = (short)argiCbte_Tipo;
                oFEXCmp_Asoc.Cbte_punto_vta = (short)argiPunto_Vta;
                oFEXCmp_Asoc.Cbte_nro = argiNroComprob;
                arrayFEXCmp_asoc[(argiIndex - 1)] = oFEXCmp_Asoc;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el comprobante asociado en la factura: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Autoriza un comprobante, devolviendo su CAE correspondiente
        /// <param name="argiTipo_Cpte">Tipo de comprobante</param>
        /// <param name="argsFecha_Cpte">Fecha del comprobante</param>
        /// <param name="argiPunto_Vta">Punto de Venta</param>
        /// <param name="argiNroComprob">Número de comprobante</param>
        /// <param name="argiTipo_Exp">Tipo de exportación. Valores permitidos (1, 2, 4)</param>
        /// <param name="argsPermiso">Permiso existente</param>
        /// <param name="argiPais_Dst">País destino del comprobante</param>
        /// <param name="argsCliente">Razón social del comprador</param>
        /// <param name="argiCuit_Pais">Cuit del país destino/contribuyente</param>
        /// <param name="argsDomicilio">Domicilio comercial del cliente</param>
        /// <param name="argsId_Impositivo">Clave de identificación tributaria del comprador. No es obligatorio si se ingreso argiCuit_Pais</param>
        /// <param name="argsMoneda_Id">Id Moneda</param>
        /// <param name="argdMoneda_Ctz">Cotización de moneda</param>
        /// <param name="argsObs_Comerciales">Observaciones comerciales</param>
        /// <param name="argdImp_Total">Importe total</param>
        /// <param name="argsObs">Observaciones</param>
        /// <param name="argsForma_Pago">Forma de pago</param>
        /// <param name="argsIncoterms">Cláusula de venta</param>
        /// <param name="argsIncoTerms_Ds">Información complementaria del incoterms</param>
        /// <param name="argiIdioma_Cbte">Idioma del documento, valore posibles: 1=Español, 2=Ingles, 3=Portugués</param>
        /// <param name="argFecha_pago">Fecha de pago (yyyymmdd)</param>
        /// </summary>
        /// <returns>Verdadero si la operación es exitosa, caso contrario Falso.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEXAuthorize(int argiTipo_Cbte, string argsFecha_Cpte, int argiPunto_Vta, int argiNroComprob, int argiTipo_Exp, string argsPermiso, int argiPais_Dst,
                                            string argsCliente, long arglCuit_Pais, string argsDomicilio, string argsId_Impositivo, string argsMoneda_Id, Double argdMoneda_Ctz,
                                            string argsObs_Comerciales, Double argdImp_Total, string argsObs, string argsForma_Pago, string argsIncoterms, string argsIncoterms_Ds,
                                            int argiIdioma_Cbte, string argFecha_pago)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (iCountItem == 0)
            {
                throw new Exception("No se han indicado los datos de cabecera de la solicitud.");
                return false;
            }

            bool bResult = false;

            try
            {
                ar.gov.afip.servicios1fex.ClsFEXRequest oFEXRequest = new ar.gov.afip.servicios1fex.ClsFEXRequest();
                ar.gov.afip.servicios1fex.FEXResponseAuthorize oFEXResponse = new ar.gov.afip.servicios1fex.FEXResponseAuthorize();

                oFEXRequest.Id = lId;
                oFEXRequest.Tipo_cbte = (short)argiTipo_Cbte;
                oFEXRequest.Fecha_cbte = argsFecha_Cpte;
                oFEXRequest.Punto_vta = (short)argiPunto_Vta;
                oFEXRequest.Cbte_nro = argiNroComprob;
                oFEXRequest.Tipo_expo = (short)argiTipo_Exp;
                oFEXRequest.Permiso_existente = argsPermiso;
                oFEXRequest.Dst_cmp = (short)argiPais_Dst;
                oFEXRequest.Cliente = argsCliente;
                oFEXRequest.Cuit_pais_cliente = arglCuit_Pais;
                oFEXRequest.Domicilio_cliente = argsDomicilio;
                oFEXRequest.Id_impositivo = argsId_Impositivo;
                oFEXRequest.Moneda_Id = argsMoneda_Id;
                oFEXRequest.Moneda_ctz = argdMoneda_Ctz;
                oFEXRequest.Obs_comerciales = argsObs_Comerciales;
                oFEXRequest.Imp_total = argdImp_Total;
                oFEXRequest.Obs = argsObs;
                oFEXRequest.Forma_pago = argsForma_Pago;
                oFEXRequest.Incoterms = argsIncoterms;
                oFEXRequest.Incoterms_Ds = argsIncoterms_Ds;
                oFEXRequest.Idioma_cbte = (short)argiIdioma_Cbte;
                oFEXRequest.Cmps_asoc = arrayFEXCmp_asoc;
                oFEXRequest.Permisos = arrayFEXPermisos;
                oFEXRequest.Items = arrayFEXDetalleItems;

                oFEXRequest.Fecha_pago = argFecha_pago;

                oFEXResponse = oCSwsAfipFEX.FEXAuthorize(oCSwsAfipAuthRequest, oFEXRequest);
                iPerCode = oFEXResponse.FEXErr.ErrCode;
                strPerrMsg = oFEXResponse.FEXErr.ErrMsg;

                if (oFEXResponse.FEXEvents != null)
                {
                    iEventCode = oFEXResponse.FEXEvents.EventCode;
                    sEventMsg = oFEXResponse.FEXEvents.EventMsg;
                }

                if (oFEXResponse.FEXResultAuth != null)
                {
                    lId = oFEXResponse.FEXResultAuth.Id;
                    sResultado = oFEXResponse.FEXResultAuth.Resultado;
                    sMotivo = oFEXResponse.FEXResultAuth.Motivos_Obs;
                    sReproceso = oFEXResponse.FEXResultAuth.Reproceso;
                    sFechaCae = oFEXResponse.FEXResultAuth.Fch_venc_Cae;
                    sCae = oFEXResponse.FEXResultAuth.Cae;
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al solicitar la autorización del comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Check interno de la clase
        /// </summary>
        /// <returns>Verdadero si todo Ok.</returns>
        [ComVisible(false)]
        private bool CSwsAfipCheck()
        {
            if (!bIsConnect)
            {
                throw new Exception("No esta conectado al web service de negocio.");
                return false;
            }
            if (!IsValidAuthRequest())
            {
                throw new Exception("El Ticket de Requerimiento de Acceso ha expirado.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene un TRA del ws de autenticación y autorización de la Afip.
        /// </summary>
        /// <returns>Verdadero si se obtuvo con éxito un Ticket de requermiento de acceso.</returns>    
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = bIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    tAuthRequestExpiration = wsAfipAA.ExpirationDatetime;
                    oCSwsAfipAuthRequest.Cuit = Convert.ToInt64(strCuit);
                    oCSwsAfipAuthRequest.Sign = wsAfipAA.Sign;
                    oCSwsAfipAuthRequest.Token = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    oCSwsAfipAuthRequest.Cuit = 0;
                    oCSwsAfipAuthRequest.Sign = "";
                    oCSwsAfipAuthRequest.Token = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }

        }

        /// <summary>
        /// Chequea el Ticket de requerimiento de acceso al ws de negocio.
        /// </summary>
        /// <returns>Verdadero si el Ticket de Requerimiento de acceso se ha obtenido y no ha expirado.</returns>
        [ComVisible(true)]
        public bool IsValidAuthRequest()
        {
            return (tAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10));
        }

        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it

            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // And create the 'Control' key - this allows it to show up in 

            // the ActiveX control container 

            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry - needed if not string named and GACced.

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            // Finally close the main key

            k.Close();
        }

        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open HKCR\CLSID\{guid} for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control' key, but don't throw an exception if it does not exist

            k.DeleteSubKey("Control", false);

            // Next open up InprocServer32

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key, again not throwing if missing 

            k.DeleteSubKey("CodeBase", false);

            // Finally close the main key 

            k.Close();
        }

        #endregion

    }


    [ProgId("CatedralSoftware.wsAfipFEV2")]
    [Guid("BB196957-02C8-458b-A040-95F3CD32F5EB")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipFEV2
    {
        #region Fields

        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEHomologacion = "https://wswhomo.afip.gov.ar/wsfev1/service.asmx?WSDL ";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEProduccion = "https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL";
        private string strWebServiceName = "wsfe";

        private string strCuit;
        private string strCertificateFileName;
        private string strPassword;
        private bool bIsHomologacion = false;

        private ar.gov.afip.servicios1fev2.ServiceSoapClient oCSwsAfipFE = new ar.gov.afip.servicios1fev2.ServiceSoapClient();
        private ar.gov.afip.servicios1fev2.FEAuthRequest oCSwsAfipAuthRequest = new ar.gov.afip.servicios1fev2.FEAuthRequest();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        private bool bIsConnect = false;

        private ar.gov.afip.servicios1fev2.Err[] arrayErrors = null;
        private ar.gov.afip.servicios1fev2.Evt[] arrayEvents = null;
        private ar.gov.afip.servicios1fev2.Obs[] arrayObs = null;
        private ar.gov.afip.servicios1fev2.FECAERequest oFECAERequest = null;
        private ar.gov.afip.servicios1fev2.FECAEResponse oFECAEResponse = null;

        private string sAppServer;
        private string sAuthServer;
        private string sDBServer;

        private int iConcepto;
        private int iDocTipo;
        private long lDocNro;
        private long lCbteDesde;
        private long lCbteHasta;
        private string sCbteFecha;
        private string sCae;
        private string sCaeFecVto;
        private string sResultado;

        #endregion

        #region Properties

        public bool Error
        {
            get
            {
                return (arrayErrors != null && arrayErrors.GetLength(0) > 0);
            }
        }

        public string Errors
        {
            get
            {
                string Message = "";

                if (Error)
                {
                    foreach (ar.gov.afip.servicios1fev2.Err oError in arrayErrors)
                    {
                        Message = Message + oError.Code.ToString() + " " + oError.Msg + "\r";
                    }
                }
                return Message;
            }
        }

        public bool Event
        {
            get
            {
                return (arrayEvents != null && arrayEvents.GetLength(0) > 0);
            }
        }

        public string Events
        {
            get
            {
                string Message = "";

                if (Event)
                {
                    foreach (ar.gov.afip.servicios1fev2.Evt oEvent in arrayEvents)
                    {
                        Message = Message + oEvent.Code.ToString() + " " + oEvent.Msg + "\r";
                    }
                }
                return Message;
            }
        }

        public bool Obs
        {
            get
            {
                return (arrayObs != null && arrayObs.GetLength(0) > 0);
            }
        }

        public string Observaciones
        {
            get
            {
                string Message = "";

                if (Obs)
                {
                    foreach (ar.gov.afip.servicios1fev2.Obs oObs in arrayObs)
                    {
                        Message = Message + oObs.Code.ToString() + " " + oObs.Msg + "\r";
                    }
                }
                return Message;
            }
        }

        public string AppServer
        {
            get { return sAppServer; }
        }

        public string AuthServer
        {
            get { return sAuthServer; }
        }

        public string DBServer
        {
            get { return sDBServer; }
        }

        public int Concepto
        {
            get { return Convert.ToInt32(iConcepto); }
        }

        public int DocumentoTipo
        {
            get { return iDocTipo; }
        }

        public long DocumentoNumero
        {
            get { return lDocNro; }
        }

        public long DesdeNroComprobante
        {
            get { return lCbteDesde; }
        }

        public long HastaNroComprobante
        {
            get { return lCbteHasta; }
        }

        public string FechaComprobante
        {
            get { return sCbteFecha; }
        }

        public string CAE
        {
            get { return sCae; }
        }

        public string VencimientoCAE
        {
            get { return sCaeFecVto; }
        }

        public string Resultado
        {
            get { return sResultado; }
        }

        public string URLWebServiceAAHomologacion
        {
            get { return strURLWebServiceAAHomologacion; }
            set { strURLWebServiceAAHomologacion = value; }
        }

        public string URLWebServiceFEHomologacion
        {
            get { return strURLWebServiceFEHomologacion; }
            set { strURLWebServiceFEHomologacion = value; }
        }

        public string WebServiceName
        {
            get { return strWebServiceName; }
            set { strWebServiceName = value; }
        }

        public string URLWebServiceAAProduccion
        {
            get { return strURLWebServiceAAProduccion; }
            set { strURLWebServiceAAProduccion = value; }
        }

        public string URLWebServiceFEProduccion
        {
            get { return strURLWebServiceFEProduccion; }
            set { strURLWebServiceFEProduccion = value; }
        }

        public string CertificateFileName
        {
            get { return strCertificateFileName; }
        }

        public string Password
        {
            get { return strPassword; }
        }

        public bool IsHomologacion
        {
            get { return bIsHomologacion; }
        }

        public string Cuit
        {
            get { return strCuit; }
        }

        public bool IsConnect
        {
            get { return bIsConnect; }
        }

        #endregion

        #region Methods

        public CSwsAfipFEV2() { } // Constructor

        /// <summary> 
        /// Se conecta WSFEX de la Afip. 
        /// </summary> 
        /// <param name="artbIsHomologacion">Indica si la conexión se efectúa sobre el web service de homologación.</param> 
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <param name="argStrPassword">Password para obtener la clave privada desde el certificado(PKCS#12 = ((Certificate + Clave Pública + Firma AC) + Clave Privada).</param>
        /// <returns>Verdadero si la conexión fue exitosa o ya estaba conectado, falso si no lo fue.</returns> 
        /// <remarks>Lanza excepciones en caso de errores.</remarks> 
        [ComVisible(true)]
        public bool CSwsAfipFEConnect(bool argbIsHomologacion, string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            // Check parameters and asigns to fields
            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                throw new Exception("Deben especificarse los parámetros bIsHomologación, strCuit y strCertificateFileName.");
                return false;
            }

            strCuit = argStrCuit;
            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;
            bIsHomologacion = argbIsHomologacion;

            //if (bIsHomologacion)
            //{
            //    oCSwsAfipFE.Url = strURLWebServiceFEHomologacion;
            //}
            //else
            //{
            //    oCSwsAfipFE.Url = strURLWebServiceFEProduccion;
            //}

            // Get Tickets Access Requirement
            try
            {
                bIsConnect = CSwsAfipAAGetTRA();
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al conectarce al servicio: " + ex.Message);
                bIsConnect = false;
            }

            return bIsConnect;
        }

        /// <summary>
        /// Se conecta WSFE de la Afip.
        /// </summary>
        /// <param name="argStrCuit">Cuit del ente externo.</param>
        /// <param name="argStrCertificateFileName">Ruta y nombre de archivo del certificado del ente externo. Debe ser en formato PKCS#12.</param>
        /// <returns></returns>
        public bool CSwsAfipFEConnect(string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            return CSwsAfipFEConnect(false, argStrCuit, argStrCertificateFileName, argStrPassword);
        }

        /// <summary>
        /// Verifica el estado de los servicios del web service de factura electrónica.
        /// </summary>
        /// <returns>Retorna Verdadero si todo esta Ok, falso en caso contrario.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFEDummy()
        {
            ar.gov.afip.servicios1fev2.DummyResponse dummyResponse = new ar.gov.afip.servicios1fev2.DummyResponse();
            bool bResult = false;

            try
            {
                dummyResponse = oCSwsAfipFE.FEDummy();
                sAppServer = dummyResponse.AppServer;
                sAuthServer = dummyResponse.AuthServer;
                sDBServer = dummyResponse.DbServer;

                bResult = (dummyResponse.AppServer == "OK" && dummyResponse.AuthServer == "OK" && dummyResponse.DbServer == "OK");
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar los estados de los servicios de la Afip: " + ex.Message);
            }
            return bResult;

        }

        /// <sumary>
        /// Esta operación permite consultar mediante tipo, numero de comprobante y punto de venta los datos de un comprobante ya emitido.
        /// </sumary>
        /// <param name="argiCbteTipo">Tipo de comprobante.</param>
        /// <param name="argiPtoVta">Punto de venta.</param>
        /// <param name="arglCbteNro">Número de comprobante.</param>
        /// <returns>Ultimo número de transacción, o -1 si hubo errores</returns>
        [ComVisible(true)]
        public bool CSWsAfipFECompConsultar(int argiCbteTipo, int argiPtoVta, long arglCbteNro)
        {
            if (!CSwsAfipCheck()) { return false; }

            ar.gov.afip.servicios1fev2.FECompConsultaReq oFECompConsultaReq = new ar.gov.afip.servicios1fev2.FECompConsultaReq();
            ar.gov.afip.servicios1fev2.FECompConsultaResponse oFECompConsultaResponse = new ar.gov.afip.servicios1fev2.FECompConsultaResponse();

            bool bResult = false;

            try
            {
                oFECompConsultaReq.CbteTipo = argiCbteTipo;
                oFECompConsultaReq.PtoVta = argiPtoVta;
                oFECompConsultaReq.CbteNro = arglCbteNro;
                oFECompConsultaResponse = oCSwsAfipFE.FECompConsultar(oCSwsAfipAuthRequest, oFECompConsultaReq);
                arrayErrors = oFECompConsultaResponse.Errors;
                arrayEvents = oFECompConsultaResponse.Events;

                if (oFECompConsultaResponse.ResultGet != null)
                {
                    arrayObs = oFECompConsultaResponse.ResultGet.Observaciones;
                    iConcepto = oFECompConsultaResponse.ResultGet.Concepto;
                    iDocTipo = oFECompConsultaResponse.ResultGet.DocTipo;
                    lDocNro = oFECompConsultaResponse.ResultGet.DocNro;
                    lCbteDesde = oFECompConsultaResponse.ResultGet.CbteDesde;
                    lCbteHasta = oFECompConsultaResponse.ResultGet.CbteHasta;
                    sCbteFecha = oFECompConsultaResponse.ResultGet.CbteFch;
                    sCae = oFECompConsultaResponse.ResultGet.CodAutorizacion;
                    sCaeFecVto = oFECompConsultaResponse.ResultGet.FchVto;
                    sResultado = oFECompConsultaResponse.ResultGet.Resultado;
                    bResult = sResultado == "A";

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar el comprobante: " + ex.Message);
            }

            return bResult;
        }


        /// <sumary>
        /// Retorna el ultimo comprobante autorizado para el tipo de comprobante / punto de venta ingresado. 
        /// </sumary>
        /// <param name="argiCbteTipo">Tipo de comprobante.</param>
        /// <param name="argiPuntoVta">Punto de venta.</param>
        /// <returns>Ultimo número de transacción, o -1 si hubo errores</returns>
        [ComVisible(true)]
        public int CSWsAfipFECompUltimoAutorizado(int argiCbteTipo, int argiPtoVta)
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fev2.FERecuperaLastCbteResponse oFECompUltimoAutorizadoResponse = new ar.gov.afip.servicios1fev2.FERecuperaLastCbteResponse();

            int iResult = -1;

            try
            {
                oFECompUltimoAutorizadoResponse = oCSwsAfipFE.FECompUltimoAutorizado(oCSwsAfipAuthRequest, argiPtoVta, argiCbteTipo);
                arrayErrors = oFECompUltimoAutorizadoResponse.Errors;
                arrayEvents = oFECompUltimoAutorizadoResponse.Events;
                iResult = oFECompUltimoAutorizadoResponse.CbteNro;

            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar el último número autorizado: " + ex.Message);
            }

            return iResult;

        }

        /// <summary>
        /// Retorna la cantidad maxima de registros que puede tener una invocacion al metodo FECAESolicitar / FECAEARegInformativo.
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        public int CSwsAfipFECompTotXRequest()
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fev2.FERegXReqResponse oFERegXReqResponse = new ar.gov.afip.servicios1fev2.FERegXReqResponse();
            int iResult = -1;

            try
            {
                oFERegXReqResponse = oCSwsAfipFE.FECompTotXRequest(oCSwsAfipAuthRequest);
                iResult = oFERegXReqResponse.RegXReq;
                arrayErrors = oFERegXReqResponse.Errors;
                arrayEvents = oFERegXReqResponse.Events;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar la cantidad máxima posible de autorización de un lote de comprobantes: " + ex.Message);
            }

            return iResult;
        }

        /// <summary>
        /// Recupera la cotizacion de la moneda consultada
        /// </summary>
        /// <param name="argsMonedaId"> Id de la moneda a consultar</param>
        /// <return> Cotización de la moneda, -1 en caso de error</return>
        public Double CSwsAfipFEParamGetCotizacion(string argsMonedaId)
        {
            if (!CSwsAfipCheck()) { return -1; }

            ar.gov.afip.servicios1fev2.FECotizacionResponse oFECotizacionResponse = new ar.gov.afip.servicios1fev2.FECotizacionResponse();
            Double dResult = -1;

            try
            {
                oFECotizacionResponse = oCSwsAfipFE.FEParamGetCotizacion(oCSwsAfipAuthRequest, argsMonedaId);
                arrayErrors = oFECotizacionResponse.Errors;
                arrayEvents = oFECotizacionResponse.Events;

                if (oFECotizacionResponse.ResultGet != null)
                    dResult = oFECotizacionResponse.ResultGet.MonCotiz;

            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al consultar la cotización de la moneda indicada: " + ex.Message);
            }


            return dResult;

        }

        /// <summary>
        /// Inicializa la cabecera del comprobante.
        /// </summary>
        /// <param name="argiCmp_Asoc">Cantidad de comprobantes asociados.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFEInit(int argiCbteTipo, int argiPtoVta, int argiCdadReg)
        {
            if (!CSwsAfipCheck()) { return false; }

            bool bResult = false;

            try
            {
                oFECAERequest = null;
                oFECAERequest = new ar.gov.afip.servicios1fev2.FECAERequest();
                oFECAERequest.FeCabReq = new ar.gov.afip.servicios1fev2.FECAECabRequest();
                oFECAERequest.FeCabReq.CbteTipo = argiCbteTipo;
                oFECAERequest.FeCabReq.PtoVta = argiPtoVta;
                oFECAERequest.FeCabReq.CantReg = argiCdadReg;
                oFECAERequest.FeDetReq = new ar.gov.afip.servicios1fev2.FECAEDetRequest[argiCdadReg];
                oFECAEResponse = null;

                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inicializando la cabecera de la solicitud: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Agrega un comprobante al Array de comprobantes a autorizar.
        /// </summary>
        /// <param name="argiIndex">Indice correspondiente al Array de comprobantes a autorizar.</param>
        /// <param name="argiConcepto">Concepto 1=Productos, 2=Productos y Servicios, 3=Servicios.</param>
        /// <param name="argiDocTipo">Tipo de documento del comprador.</param>
        /// <param name="argStrDocNro">Nº de identificación del comprador.</param>
        /// <param name="argiCbteDesde">Nº de comprobante desde.</param>
        /// <param name="argiCbteHasta">Nº de comprobante hasta.</param>
        /// <param name="argStrFechaCbte">Fecha del comprobante.</param>
        /// <param name="argdImpTotal">Importe Total del comprobante. Debe ser igual a: neto no gravado + exento + neto gravado + iva + tributos.</param>
        /// <param name="argdImpTotConc">Importe neto no gravado. Debe ser menor o igual al Importe Total y no puede ser negativo. Para comprobantes tipo C debe ser cero (0).</param>
        /// <param name="argdImpNeto">Importe neto gravado. Para comprobantes tipo C sera el subtotal.</param>
        /// <param name="argdImpOpEx">Importe exento. Debe ser menor o igual al Importe Total y no puede ser negativo. Para comprobantes tipo C debe ser cero (0).</param>
        /// <param name="argdImpIVA">Suma de los importes de IVA. Para comprobantes tipo C debe ser cero (0).</param>
        /// <param name="argdImpTrib">Suma de los importes de tributos.</param>
        /// <param name="argStrFechaServDesde">Fecha de inicio del servicio o abono. Obligatorio para Concepto = 2 o 3.</param>
        /// <param name="argStrFechaServHasta">Fecha de fin del servicio o abono. Obligatorio para Concepto = 2 o 3.</param>
        /// <param name="argStrFechaVencPago">Fecha de vencimiento del pago de servicio. Obligatorio para Concepto = 2 o 3.</param>
        /// <param name="argStrMonedaId">Identificador de moneda.</param>
        /// <param name="argdMonCotiz">Cotización de la moneda informada.</param>        /// 
        /// <returns>Verdadero si la operación es exitosa, caso contrario Falso.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_DetRequest(int argiIndex, int argiConcepto, int argiDocTipo, long arglDocNro,
                                                long arglCbteDesde, long arglCbteHasta, string argStrFechaCbte, double argdImpTotal,
                                                double argdImpTotConc, double argdImpNeto, double argdImpOpEx, double argdImpIVA,
                                                double argdImpTrib, string argStrFechaServDesde, string argStrFechaServHasta,
                                                string argStrFechaVencPago, string argStrMonedaId, double argdMonCotiz,
                                                int argiCbteAsoc, int argiTributo, int argiIva, int argiOpcional)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado la cantidad de comprobantes que integrara el lote.");
                return false;
            }

            if (argiIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            bool bResult = false;

            try
            {
                ar.gov.afip.servicios1fev2.FECAEDetRequest oFECAEDetRequest = new ar.gov.afip.servicios1fev2.FECAEDetRequest();

                if (argiCbteAsoc > 0) { oFECAEDetRequest.CbtesAsoc = new ar.gov.afip.servicios1fev2.CbteAsoc[argiCbteAsoc]; }
                if (argiTributo > 0) { oFECAEDetRequest.Tributos = new ar.gov.afip.servicios1fev2.Tributo[argiTributo]; }
                if (argiIva > 0) { oFECAEDetRequest.Iva = new ar.gov.afip.servicios1fev2.AlicIva[argiIva]; }
                if (argiOpcional > 0) { oFECAEDetRequest.Opcionales = new ar.gov.afip.servicios1fev2.Opcional[argiOpcional]; }

                oFECAEDetRequest.Concepto = argiConcepto;
                oFECAEDetRequest.DocTipo = argiDocTipo;
                oFECAEDetRequest.DocNro = arglDocNro;
                oFECAEDetRequest.CbteDesde = arglCbteDesde;
                oFECAEDetRequest.CbteHasta = arglCbteHasta;
                oFECAEDetRequest.CbteFch = argStrFechaCbte;
                oFECAEDetRequest.ImpTotal = argdImpTotal;
                oFECAEDetRequest.ImpTotConc = argdImpTotConc;
                oFECAEDetRequest.ImpNeto = argdImpNeto;
                oFECAEDetRequest.ImpOpEx = argdImpOpEx;
                oFECAEDetRequest.ImpIVA = argdImpIVA;
                oFECAEDetRequest.ImpTrib = argdImpTrib;
                oFECAEDetRequest.FchServDesde = argStrFechaServDesde;
                oFECAEDetRequest.FchServHasta = argStrFechaServHasta;
                oFECAEDetRequest.FchVtoPago = argStrFechaVencPago;
                oFECAEDetRequest.MonId = argStrMonedaId;
                oFECAEDetRequest.MonCotiz = argdMonCotiz;
                oFECAERequest.FeDetReq[(argiIndex - 1)] = oFECAEDetRequest;

                bResult = true;

            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al agregar el comprobante: " + ex.Message);
            }

            return bResult;

        }
        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiCbteIndex">Total de filas de comprobantes asociados.</param>
        /// <param name="argiCbteAsocIndex">Filta del comprobante que se esta agregando.</param>
        /// <param name="argiCbte_Tipo">Tipo de comprobante.</param>
        /// <param name="argiPunto_Vta">Punto de venta.</param>
        /// <param name="argiNroComprob">Nro de comprobante.</param>
        /// <returns>Verdadero si todo salio bien.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_CbteAsoc(int argiCbteIndex, int argiCbteAsocIndex, int argiCbte_Tipo, int argiPunto_Vta, int argiNroComprob)
        {
            try
            {
                return CSwsAfipFE_Add_CbteAsoc(argiCbteIndex, argiCbteAsocIndex, argiCbte_Tipo, argiPunto_Vta, argiNroComprob, "", "");
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }


        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiCbteIndex">Total de filas de comprobantes asociados.</param>
        /// <param name="argiCbteAsocIndex">Filta del comprobante que se esta agregando.</param>
        /// <param name="argiCbte_Tipo">Tipo de comprobante.</param>
        /// <param name="argiPunto_Vta">Punto de venta.</param>
        /// <param name="argiNroComprob">Nro de comprobante.</param>
        /// <param name="argCuit">Cuit del receptor del comprobante asociado</param>
        /// <param name="argFechaComprobante">Fecha del comprobante asociado</param>
        /// <returns>Verdadero si todo salio bien.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_CbteAsoc(int argiCbteIndex, int argiCbteAsocIndex, int argiCbte_Tipo, int argiPunto_Vta, int argiNroComprob, string argCuit, string argFechaComprobante)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado la cantidad de comprobantes que integrara el lote.");
                return false;
            }

            if (argiCbteIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            ar.gov.afip.servicios1fev2.CbteAsoc[] arrayFECbteAsoc = oFECAERequest.FeDetReq[(argiCbteIndex - 1)].CbtesAsoc;
            int iCount = arrayFECbteAsoc.GetLength(0);

            if (iCount == 0)
            {
                throw new Exception("No se ha indicado detalle de comprobantes asociados en el comprobante indicado.");
                return false;
            }

            if (argiCbteAsocIndex > iCount)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de los comprobantes asociados.");
                return false;
            }

            ar.gov.afip.servicios1fev2.CbteAsoc oFECbteAsoc = new ar.gov.afip.servicios1fev2.CbteAsoc();
            bool bResult = false;

            try
            {
                oFECbteAsoc.Tipo = argiCbte_Tipo;
                oFECbteAsoc.PtoVta = argiPunto_Vta;
                oFECbteAsoc.Nro = argiNroComprob;
                arrayFECbteAsoc[(argiCbteAsocIndex - 1)] = oFECbteAsoc;
                oFECbteAsoc.CbteFch = argFechaComprobante;
                oFECbteAsoc.Cuit = argCuit;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el comprobante asociado al comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un tributo a la factura.
        /// </summary>
        /// <param name="argiCbteIndex">Indice del comprobante en el lote.</param>
        /// <param name="argiTribIndex">Indice en el detalle comprobantes asociados.</param>
        /// <param name="argiId">Id del tributo.</param>
        /// <param name="argsDescrip">Descripción del tributo.</param>
        /// <param name="argdBaseImp">Base imponible del tributo.</param>
        /// <param name="argdAlicuota">Alícuota del tributo.</param>
        /// <param name="argdImporte">Importe del tributo.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_Tributo(int argiCbteIndex, int argiTribIndex, int argiId, string argsDescrip, Double argdBaseImp, Double argdAlicuota, Double argdImporte)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado la cantidad de comprobantes que integrara el lote.");
                return false;
            }

            if (argiCbteIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            ar.gov.afip.servicios1fev2.Tributo[] arrayFETributo = oFECAERequest.FeDetReq[(argiCbteIndex - 1)].Tributos;
            int iCount = arrayFETributo.GetLength(0);

            if (iCount == 0)
            {
                throw new Exception("No se ha indicado detalle de Tributos en el comprobante indicado.");
                return false;
            }

            if (argiTribIndex > iCount)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de los Tributos.");
                return false;
            }

            ar.gov.afip.servicios1fev2.Tributo oFETributo = new ar.gov.afip.servicios1fev2.Tributo();
            bool bResult = false;

            try
            {
                oFETributo.Id = (short)argiId;
                oFETributo.Desc = argsDescrip;
                oFETributo.BaseImp = argdBaseImp;
                oFETributo.Alic = argdAlicuota;
                oFETributo.Importe = argdImporte;
                arrayFETributo[(argiTribIndex - 1)] = oFETributo;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el tributo asociado al comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiIndex">Indice en el detalle comprobantes asociados.</param>
        /// <param name="argiAlicId">Indentificador de la alicuota.</param>
        /// <param name="argdBaseImp">Base imponible.</param>
        /// <param name="argdImporte">Importe.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_AlicIva(int argiCbteIndex, int argiAlicIndex, int argiAlicId, Double argdBaseImp, Double argdImporte)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado la cantidad de comprobantes que integrara el lote.");
                return false;
            }

            if (argiCbteIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            ar.gov.afip.servicios1fev2.AlicIva[] arrayFEAlicIva = oFECAERequest.FeDetReq[(argiCbteIndex - 1)].Iva;
            int iCount = arrayFEAlicIva.GetLength(0);

            if (iCount == 0)
            {
                throw new Exception("No se ha indicado detalle de alícuotas en el comprobante indicado.");
                return false;
            }

            if (argiAlicIndex > iCount)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de alícuotas.");
                return false;
            }

            ar.gov.afip.servicios1fev2.AlicIva oFEAlicIva = new ar.gov.afip.servicios1fev2.AlicIva();
            bool bResult = false;

            try
            {
                oFEAlicIva.Id = argiAlicId;
                oFEAlicIva.BaseImp = argdBaseImp;
                oFEAlicIva.Importe = argdImporte;
                arrayFEAlicIva[(argiAlicIndex - 1)] = oFEAlicIva;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar la alícuota asociada  al comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiIndex">Indice en el detalle comprobantes asociados.</param>
        /// <param name="argsId">Identificador del opcional.</param>
        /// <param name="argsObs">Valor del opcional.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Add_Opcional(int argiCbteIndex, int argiOpcIndex, string argsId, string argsObs)
        {
            if (!CSwsAfipCheck()) { return false; }

            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado la cantidad de comprobantes que integrara el lote.");
                return false;
            }

            if (argiCbteIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            ar.gov.afip.servicios1fev2.Opcional[] arrayFEOpcional = oFECAERequest.FeDetReq[(argiCbteIndex - 1)].Opcionales;
            int iCount = arrayFEOpcional.GetLength(0);

            if (iCount == 0)
            {
                throw new Exception("No se ha indicado detalle de Opcionales en el comprobante indicado.");
                return false;
            }

            if (argiOpcIndex > iCount)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de Opcionales.");
                return false;
            }

            ar.gov.afip.servicios1fev2.Opcional oFEOpcional = new ar.gov.afip.servicios1fev2.Opcional();
            bool bResult = false;

            try
            {
                oFEOpcional.Id = argsId;
                oFEOpcional.Valor = argsObs;
                arrayFEOpcional[(argiOpcIndex - 1)] = oFEOpcional;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al intentar agregar el opcional asociado al comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Autoriza un comprobante/lote de comprobantes, devolviendo su CAE correspondiente.
        /// </summary>
        /// <returns>Verdadero si la operación es exitosa, caso contrario Falso.</returns>
        [ComVisible(true)]
        public bool CSwsAfipFECAESolicitar()
        {
            if (!CSwsAfipCheck()) { return false; }


            if (oFECAERequest == null)
            {
                throw new Exception("No se ha inicializado la solicitud.");
                return false;
            }

            int iCdadReg = oFECAERequest.FeDetReq.GetLength(0);
            if (iCdadReg == 0)
            {
                throw new Exception("No se ha especificado comprobantes a autorizar.");
                return false;
            }


            bool bResult = false;

            try
            {

                oFECAEResponse = new ar.gov.afip.servicios1fev2.FECAEResponse();
                oFECAEResponse = oCSwsAfipFE.FECAESolicitar(oCSwsAfipAuthRequest, oFECAERequest);
                arrayErrors = oFECAEResponse.Errors;
                arrayEvents = oFECAEResponse.Events;

                if (oFECAEResponse.FeDetResp != null)
                {
                    CSwsAfipFE_Get_DetRequest(1);
                    bResult = oFECAEResponse.FeCabResp.Resultado == "A";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al solicitar la autorización del comprobante: " + ex.Message);
            }

            return bResult;
        }

        /// <summary>
        /// Ingresa un comprobante asociado a la factura.
        /// </summary>
        /// <param name="argiIndex">Indice en el detalle comprobantes asociados.</param>
        /// <param name="argiCbte_Tipo">Tipo de comprobante.</param>
        /// <param name="argiPunto_Vta">Punto de venta.</param>
        /// <param name="argiNroComprob">Nro de comprobante.</param>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CSwsAfipFE_Get_DetRequest(int argiCbteIndex)
        {

            if (oFECAEResponse == null) { return false; }

            int iCdadReg = oFECAEResponse.FeDetResp.GetLength(0);
            if (iCdadReg == 0) { return false; }

            if (argiCbteIndex > iCdadReg)
            {
                throw new Exception("El indice indicado esta fuera del intervalo definido en el detalle de comprobantes.");
                return false;
            }

            bool bResult = false;

            try
            {
                ar.gov.afip.servicios1fev2.FECAEDetResponse oFECAEDetResponse = oFECAEResponse.FeDetResp[(argiCbteIndex - 1)];

                iConcepto = oFECAEDetResponse.Concepto;
                iDocTipo = oFECAEDetResponse.DocTipo;
                lDocNro = oFECAEDetResponse.DocNro;
                lCbteDesde = oFECAEDetResponse.CbteDesde;
                lCbteHasta = oFECAEDetResponse.CbteHasta;
                sCbteFecha = oFECAEDetResponse.CbteFch;
                sResultado = oFECAEDetResponse.Resultado;
                sCae = oFECAEDetResponse.CAE;
                sCaeFecVto = oFECAEDetResponse.CAEFchVto;
                arrayObs = oFECAEDetResponse.Observaciones;
                bResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Se genero una excepción al obtener el comprobante indicado: " + ex.Message);
            }

            return bResult;
        }


        /// <summary>
        /// Check interno de la clase
        /// </summary>
        /// <returns>Verdadero si todo Ok.</returns>
        [ComVisible(false)]
        private bool CSwsAfipCheck()
        {
            if (!bIsConnect)
            {
                throw new Exception("No esta conectado al web service de negocio.");
                return false;
            }
            if (!IsValidAuthRequest())
            {
                throw new Exception("El Ticket de Requerimiento de Acceso ha expirado.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene un TRA del ws de autenticación y autorización de la Afip.
        /// </summary>
        /// <returns>Verdadero si se obtuvo con éxito un Ticket de requermiento de acceso.</returns>    
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = bIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    tAuthRequestExpiration = wsAfipAA.ExpirationDatetime;
                    oCSwsAfipAuthRequest.Cuit = Convert.ToInt64(strCuit);
                    oCSwsAfipAuthRequest.Sign = wsAfipAA.Sign;
                    oCSwsAfipAuthRequest.Token = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    oCSwsAfipAuthRequest.Cuit = 0;
                    oCSwsAfipAuthRequest.Sign = "";
                    oCSwsAfipAuthRequest.Token = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }

        }

        /// <summary>
        /// Chequea el Ticket de requerimiento de acceso al ws de negocio.
        /// </summary>
        /// <returns>Verdadero si el Ticket de Requerimiento de acceso se ha obtenido y no ha expirado.</returns>
        [ComVisible(true)]
        public bool IsValidAuthRequest()
        {
            return (tAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10));
        }

        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it

            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // And create the 'Control' key - this allows it to show up in 

            // the ActiveX control container 

            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry - needed if not string named and GACced.

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            // Finally close the main key

            k.Close();
        }

        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open HKCR\CLSID\{guid} for write access

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control' key, but don't throw an exception if it does not exist

            k.DeleteSubKey("Control", false);

            // Next open up InprocServer32

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key, again not throwing if missing 

            k.DeleteSubKey("CodeBase", false);

            // Finally close the main key 

            k.Close();
        }

        #endregion

    }


    [ComVisible(false)]
    class CSwsAfipAA
    {

        #region Fields

        private string strURLWebServiceAAHomologacion = "";
        private string strURLWebServiceAAProduccion = "";
        private string strWebServiceName = "";
        private bool bIsHomologacion = false;
        private UInt32 intUniqueId;
        private DateTime tGeneration;
        private DateTime tExpiration;
        private string strSign = "";
        private string strToken = "";

        private static UInt32 _globalUniqueID = 0;


        #endregion

        #region Properties

        public string URLWebServiceAAHomologacion
        {
            get { return strURLWebServiceAAHomologacion; }
            set { strURLWebServiceAAHomologacion = value; }
        }

        public string URLWebServiceAAProduccion
        {
            get { return strURLWebServiceAAProduccion; }
            set { strURLWebServiceAAProduccion = value; }
        }

        public string WebServiceName
        {
            get { return strWebServiceName; }
            set { strWebServiceName = value; }
        }

        public bool IsHomologacion
        {
            get { return bIsHomologacion; }
            set { bIsHomologacion = value; }
        }

        public UInt32 UniqueId
        {
            get { return intUniqueId; }
            set { intUniqueId = value; }
        }

        public DateTime GenerationDateTime
        {
            get { return tGeneration; }
        }

        public DateTime ExpirationDatetime
        {
            get { return tExpiration; }
        }

        public string Sign
        {
            get { return strSign; }
        }

        public string Token
        {
            get { return strToken; }
        }

        #endregion

        #region Methods

        // Define methods
        private XmlDocument CreateTicketAccessRequest()
        {
            string sXmlTicketAccessRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";

            XmlDocument XmlTicketAccessRequest = new XmlDocument();
            XmlNode xmlNodoUniqueId;
            XmlNode xmlNodoGenerationTime;
            XmlNode xmlNodoExpirationTime;
            XmlNode xmlNodoService;

            try
            {
                XmlTicketAccessRequest.LoadXml(sXmlTicketAccessRequestTemplate);

                xmlNodoUniqueId = XmlTicketAccessRequest.SelectSingleNode("//uniqueId");
                xmlNodoGenerationTime = XmlTicketAccessRequest.SelectSingleNode("//generationTime");
                xmlNodoExpirationTime = XmlTicketAccessRequest.SelectSingleNode("//expirationTime");
                xmlNodoService = XmlTicketAccessRequest.SelectSingleNode("//service");

                // Las horas son UTC formato yyyy-MM-ddTHH:mm:ssZ
                xmlNodoGenerationTime.InnerText = DateTime.UtcNow.AddMinutes(-10).ToString("s") + "Z";
                xmlNodoExpirationTime.InnerText = DateTime.UtcNow.AddMinutes(+10).ToString("s") + "Z";
                xmlNodoUniqueId.InnerText = _globalUniqueID.ToString();
                xmlNodoService.InnerText = strWebServiceName;
                _globalUniqueID += 1;

                return XmlTicketAccessRequest;

            }

            catch (Exception excepcionCreateTicketAccessRequest)
            {
                throw new Exception("Se genero un excepción no controlada creando el ticket de solicitud de acceso: " + excepcionCreateTicketAccessRequest.Message);
            }

        }

        private string SignTicketAccessRequest(string argStrCertificateFileName, string argStrPassword, XmlDocument argXmlTicketAccessRequest)
        {
            string cmsFirmadoBase64;

            try
            {
                X509Certificate2 oX509Certificate = CSX509Certificate.LoadCertificate(argStrCertificateFileName, argStrPassword);

                // Convierto el Ticket Access Request a bytes, para poder firmarlo
                Encoding EncodedMsg = Encoding.UTF8;
                byte[] msgBytes = EncodedMsg.GetBytes(argXmlTicketAccessRequest.OuterXml);

                // Firmo el msg y paso a Base64 
                byte[] encodedSignedCms = CSX509Certificate.SignBytesMessage(msgBytes, oX509Certificate);

                cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);

                return cmsFirmadoBase64;
            }

            catch (Exception excepcionSignTicketRequest)
            {
                throw new Exception("Se genero una excepción firmando el ticket de solicitud de acceso: " + excepcionSignTicketRequest.Message);
            }
        }

        private string SignTicketAccessRequest(byte[] argDataCertificate, string argStrPassword, XmlDocument argXmlTicketAccessRequest)
        {
            string cmsFirmadoBase64;

            try
            {
                X509Certificate2 oX509Certificate = CSX509Certificate.LoadCertificate(argDataCertificate, argStrPassword);

                // Convierto el Ticket Access Request a bytes, para poder firmarlo
                Encoding EncodedMsg = Encoding.UTF8;
                byte[] msgBytes = EncodedMsg.GetBytes(argXmlTicketAccessRequest.OuterXml);

                // Firmo el msg y paso a Base64 
                byte[] encodedSignedCms = CSX509Certificate.SignBytesMessage(msgBytes, oX509Certificate);

                cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);

                return cmsFirmadoBase64;
            }

            catch (Exception excepcionSignTicketRequest)
            {
                throw new Exception("Se genero una excepción firmando el ticket de solicitud de acceso: " + excepcionSignTicketRequest.Message);
            }
        }
        private bool GetTicketAccessResponse(string argStrCmsSignedBase64)
        {
            string strTicketAccessResponse;
            bool bResult = false;

            try
            {
                if (bIsHomologacion)
                {
                    ar.gov.afip.wsaahomo.LoginCMSClient CSwsaaHomo = new ar.gov.afip.wsaahomo.LoginCMSClient("https://wsaa.afip.gov.ar/ws/services/LoginCms");
                    strTicketAccessResponse = CSwsaaHomo.loginCms(argStrCmsSignedBase64);
                }
                else
                {
                    System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(new Uri("https://wsaa.afip.gov.ar/ws/services/LoginCms"));
                    ar.gov.afip.wsaa.LoginCMSClient CSwsaa = new ar.gov.afip.wsaa.LoginCMSClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);

                    strTicketAccessResponse = CSwsaa.loginCms(argStrCmsSignedBase64);
                }

            }
            catch (Exception excepcionTicketAccessResponse)
            {
                throw new Exception("Se genero una excepción invocando al Web Service de Autenticación: " + excepcionTicketAccessResponse.Message);
            }

            try
            {
                XmlDocument XmlTicketAccessResponse = new XmlDocument();
                XmlTicketAccessResponse.LoadXml(strTicketAccessResponse);

                intUniqueId = UInt32.Parse(XmlTicketAccessResponse.SelectSingleNode("//uniqueId").InnerText);
                tGeneration = DateTime.Parse(XmlTicketAccessResponse.SelectSingleNode("//generationTime").InnerText);
                tExpiration = DateTime.Parse(XmlTicketAccessResponse.SelectSingleNode("//expirationTime").InnerText);
                strSign = XmlTicketAccessResponse.SelectSingleNode("//sign").InnerText;
                strToken = XmlTicketAccessResponse.SelectSingleNode("//token").InnerText;
                bResult = true;
            }
            catch (Exception excepcionAlAnalizarTicketResponse)
            {
                throw new Exception("Se genero una excepción al analizar la respuesta del WSAA: " + excepcionAlAnalizarTicketResponse.Message);
            }
            return bResult;

        }

        public bool GetTicketAccessRequirement(string argStrCertificateFileName, string argStrPassword)
        {
            XmlDocument XmlTicketAccessRequest = null;
            string StrCmsFirmadoBase64;
            bool bResult = false;

            try
            {
                // Create ticket access request
                XmlTicketAccessRequest = CreateTicketAccessRequest();

                // Sign ticket acces request
                StrCmsFirmadoBase64 = SignTicketAccessRequest(argStrCertificateFileName, argStrPassword, XmlTicketAccessRequest);

                // View response
                return GetTicketAccessResponse(StrCmsFirmadoBase64);

            }
            catch (Exception excepcionConnect)
            {
                throw new Exception("Se genero una excepción firmar el Ticket de Acceso: " + excepcionConnect.Message);
            }
            return bResult;
        }

        public bool GetTicketAccessRequirement(byte[] argDataCertificate, string argStrPassword)
        {
            XmlDocument XmlTicketAccessRequest = null;
            string StrCmsFirmadoBase64;
            bool bResult = false;

            try
            {
                // Create ticket access request
                XmlTicketAccessRequest = CreateTicketAccessRequest();

                // Sign ticket acces request
                StrCmsFirmadoBase64 = SignTicketAccessRequest(argDataCertificate, argStrPassword, XmlTicketAccessRequest);

                // View response
                return GetTicketAccessResponse(StrCmsFirmadoBase64);

            }
            catch (Exception excepcionConnect)
            {
                throw new Exception("GetTicketAccessRequirement Se genero una excepción firmar el Ticket de Acceso: " + excepcionConnect.Message);
            }
        }

        #endregion

    }

    [ComVisible(false)]
    class CSX509Certificate
    {

        #region Methods
        /// <summary> 
        /// Firma mensaje 
        /// </summary> 
        /// <param name="argBytesMsg">Bytes del mensaje</param> 
        /// <param name="argCertificate">Certificado usado para firmar</param> 
        /// <returns>Bytes del mensaje firmado</returns> 
        /// <remarks></remarks> 
        public static byte[] SignBytesMessage(byte[] argBytesMsg, X509Certificate2 argCertificate)
        {
            try
            {
                // Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms) 
                ContentInfo infoContenido = new ContentInfo(argBytesMsg);
                SignedCms cmsFirmado = new SignedCms(infoContenido);

                // Creo objeto CmsSigner que tiene las caracteristicas del firmante 
                CmsSigner cmsFirmante = new CmsSigner(argCertificate);
                cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;

                // Firmo el mensaje PKCS #7 
                cmsFirmado.ComputeSignature(cmsFirmante);

                // Encodeo el mensaje PKCS #7. 
                return cmsFirmado.Encode();

            }
            catch (Exception excepcionSign)
            {
                throw new Exception("Se genero una excepción al firmar la solicitud de acceso: " + excepcionSign.Message);
            }
        }

        /// <summary> 
        /// Lee certificado de disco 
        /// </summary> 
        /// <param name="argFileName">Ruta del certificado a leer.</param> 
        /// <param name="argStrPassword">Password con la que se encripto la clave privada del certificado.<param/>
        /// <returns>Un objeto certificado X509</returns> 
        /// <remarks></remarks> 
        public static X509Certificate2 LoadCertificate(string argStrFileName, string argStrPassword)
        {
            X509Certificate2 oCertificate = new X509Certificate2();

            try
            {
                oCertificate.Import(File.ReadAllBytes(argStrFileName), argStrPassword, X509KeyStorageFlags.Exportable);
                return oCertificate;
            }
            catch (Exception excepcionImportCertificate)
            {
                throw new Exception("Se genero una excepción al leer el certificado desde: " + argStrFileName + ". " + excepcionImportCertificate.Message + " " + excepcionImportCertificate.StackTrace);
            }
        }

        public static X509Certificate2 LoadCertificate(byte[] argDataCertificate, string argStrPassword)
        {
            try
            {
                var oCertificate = new X509Certificate2(argDataCertificate, argStrPassword, X509KeyStorageFlags.Exportable);
                //oCertificate.Import(argDataCertificate, argStrPassword, X509KeyStorageFlags.Exportable);
                return oCertificate;
            }
            catch (Exception excepcionImportCertificate)
            {
                throw new Exception("Se genero una excepción al leer el certificado:" + excepcionImportCertificate.Message + " " + excepcionImportCertificate.StackTrace);
            }
        }

        #endregion

    }

    [ProgId("CatedralSoftware.wsAfipPersonaV2")]
    [Guid("AB1B718D-DE29-45FD-9B67-15E53DD01909")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipPersonaV2
    {
        #region Fields

        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServicePersonaHomologacion = "https://awshomo.afip.gov.ar/sr-padron/webservices/personaServiceA5?WSDL";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServicePersonaProduccion = "https://aws.afip.gov.ar/sr-padron/webservices/personaServiceA5?WSDL";
        private string strWebServiceName = "ws_sr_constancia_inscripcion";
        private bool bIsConnect = false;

        private string strCuit;
        private string strCertificateFileName;
        private string strPassword;
        private string strToken = "";
        private string strSign = "";
        private string strIdPersona = "";
        private bool bIsHomologacion = false;

        private ar.gov.afip.aws.PersonaServiceA5Client oCSwsAfipPersona = new ar.gov.afip.aws.PersonaServiceA5Client();
        private ar.gov.afip.aws.personaReturn oPersonaReturn = new ar.gov.afip.aws.personaReturn();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        //Datos de respuesta
        private string strNombre = "";
        private string strApellido = "";
        private string strRazonSocial = "";
        private string strtipoClave = "";
        private string strtipoPersona = "";
        private string strXmlRespuesta = "";
        private string strXmlRespuestaDatosGeneral = "";
        private string strXmlRespuestaRegimen = "";
        private string strXmlRespuestaActividad = "";
        private string strXmlRespuestaImpuesto = "";
        private string strestadoClave = "";
        private string strfechaContrato = "";
        private string strmesCierre = "";
        private string stridPersona = "";
        private string strXmldomicilioFiscal = "";
        private string strXmlRespuestaMonotributo = "";
        private string strXmErrorConstancia = "";
        private string strXmlErrorMonotributo = "";
        private string strXmlErrorRegimenGeneral = "";

        #endregion

        #region Properties
        public string StrToken
        {
            get { return strToken; }
            set { strToken = value; }
        }
        public string StrSign
        {
            get { return strSign; }
            set { strSign = value; }
        }
        public long NumCuit
        {
            get { return Convert.ToInt64(strCuit); }
            set { strCuit = Convert.ToString(value); }
        }
        public long NumIdPersona
        {
            get { return Convert.ToInt64(strIdPersona); }
            set { strIdPersona = Convert.ToString(value); }
        }
        public string StrCertificateFileName
        {
            get { return strCertificateFileName; }
            set { strCertificateFileName = value; }
        }
        public string StrPassword
        {
            get { return strPassword; }
            set { strPassword = value; }
        }

        /// <summary>
        /// Nombre del contribuyente.
        /// </summary>
        public string StrNombre
        {
            get { return strNombre; }
            set { strNombre = value; }
        }

        /// <summary>
        /// Apellido del contribuyente.
        /// </summary>
        public string StrApellido
        {
            get { return strApellido; }
            set { strApellido = value; }
        }

        /// <summary>
        /// Razón Social del contribuyente, para personas juridicas.
        /// </summary>
        public string StrRazonSocial
        {
            get { return strRazonSocial; }
            set { strRazonSocial = value; }
        }
        /// <summary>
        /// Tipo de la clave, CUIT, DNI, etc.
        /// </summary>
        public string StrtipoClave
        {
            get { return strtipoClave; }
            set { strtipoClave = value; }
        }
        /// <summary>
        /// Tipo de contribuyente, Fisica, Juridica.
        /// </summary>
        public string StrtipoPersona
        {
            get { return strtipoPersona; }
            set { strtipoPersona = value; }
        }
        public bool BIsHomologacion
        {
            get { return bIsHomologacion; }
            set { bIsHomologacion = value; }
        }
        public bool BIsConnect
        {
            get { return bIsConnect; }
            set { bIsConnect = value; }
        }
        public DateTime TAuthRequestExpiration
        {
            get { return tAuthRequestExpiration; }
            set { tAuthRequestExpiration = value; }
        }

        /// <summary>
        /// Respuesta general del web service de Personas, en formato XML.
        /// </summary>
        public string StrXmlRespuesta
        {
            get { return strXmlRespuesta; }
            set { strXmlRespuesta = value; }
        }
        /// <summary>
        /// Respuesta de los Datos generales del contribuyente, en formato XML.
        /// </summary>
        public string StrXmlRespuestaDatosGeneral
        {
            get { return strXmlRespuestaDatosGeneral; }
            set { strXmlRespuestaDatosGeneral = value; }
        }
        /// <summary>
        /// Respuesta del régimen al que esta inscripto el contribuyente, en formato XML.
        /// </summary>
        public string StrXmlRespuestaRegimen
        {
            get { return strXmlRespuestaRegimen; }
            set { strXmlRespuestaRegimen = value; }
        }

        /// <summary>
        /// Actividades de AFIP a las cuales esta inscripto el contribuyente, en formato XML.
        /// </summary>
        public string StrXmlRespuestaActividad
        {
            get { return strXmlRespuestaActividad; }
            set { strXmlRespuestaActividad = value; }
        }

        /// <summary>
        /// Lista de inpuestos a los que esta inscripto el contribuyente en AFIP, en formato XML.
        /// </summary>
        public string StrXmlRespuestaImpuesto
        {
            get { return strXmlRespuestaImpuesto; }
            set { strXmlRespuestaImpuesto = value; }

        }

        /// <summary>
        /// Estado de la clave, Activo o Inactivo.
        /// </summary>
        public string StrestadoClave
        {
            get { return strestadoClave; }
            set { strestadoClave = value; }
        }
        /// <summary>
        /// Fecha del contrato social.
        /// </summary>
        public string StrfechaContrato
        {
            get { return strfechaContrato; }
            set { strfechaContrato = value; }
        }
        /// <summary>
        /// Mes de cierre del ejercicio contable (Número del mes).
        /// </summary>
        public string StrmesCierre
        {
            get { return strmesCierre; }
            set { strmesCierre = value; }
        }
        /// <summary>
        /// ID de Persona, usualmente CUIT.
        /// </summary>
        public string StridPersona
        {
            get { return stridPersona; }
            set { stridPersona = value; }
        }
        /// <summary>
        /// Lista de los domicilios fiscales, en formato XML.
        /// </summary>
        public string StrXmldomicilioFiscal
        {
            get { return strXmldomicilioFiscal; }
            set { strXmldomicilioFiscal = value; }
        }
        /// <summary>
        /// Respuesta de los datos crudos del contribuyente, si esta inscripto en Monotributo, en formto XML.
        /// </summary>
        public string StrXmlRespuestaMonotributo
        {
            get { return strXmlRespuestaMonotributo; }
            set { strXmlRespuestaMonotributo = value; }
        }
        /// <summary>
        /// Error devuelto por AFIP en la constancia de inscripción, en formato XML.
        /// </summary>
        public string StrXmErrorConstancia
        {
            get { return strXmErrorConstancia; }
            set { strXmErrorConstancia = value; }
        }
        /// <summary>
        ///  Error devuelto por AFIP en la constancia de inscripción en Monotributo, en formato XML.
        /// </summary>
        public string StrXmlErrorMonotributo
        {
            get { return strXmlErrorMonotributo; }
            set { strXmlErrorMonotributo = value; }
        }
        /// <summary>
        ///  Error devuelto por AFIP en la constancia de inscripción del regimen general, en formato XML.
        /// </summary>
        public string StrXmlErrorRegimenGeneral
        {
            get { return strXmlErrorRegimenGeneral; }
            set { strXmlErrorRegimenGeneral = value; }
        }

        #endregion

        #region Methods

        public CSwsAfipPersonaV2() { } //Constructor

        /// <summary>
        /// Obtiene los datos de la constacia de CUIT, correspondiente a la CUIT solicitada en argStrIdPersona.
        /// </summary>
        /// <param name="argbIsHomologacion">Verdadero si es un certificado de Homologacion y Falso si es de producción.</param>
        /// <param name="argStrCertificateFileName">Certificado digital Pk12</param>
        /// <param name="argStrPassword">Clave del certificado.</param>
        /// <param name="argStrCuit">CUIT del solicitante, tiene que coincidir con el del certificado.</param>
        /// <param name="argStrIdPersona">CUIT de la perona que se quiere consultar.</param>
        /// <returns>Verdadero, si fue exitoso</returns>
        public bool CSwsAfipGetPersona(bool argbIsHomologacion, string argStrCertificateFileName, string argStrPassword, string argStrCuit, string argStrIdPersona)
        {
            bool lbReturn = false;
            XmlSerializer lcsXmlReturn = new XmlSerializer(oPersonaReturn.GetType());
            StringWriter lstrXmlReturn = new StringWriter();

            ar.gov.afip.aws.domicilio loDomicilioFiscal = new ar.gov.afip.aws.domicilio();

            BIsHomologacion = argbIsHomologacion;
            StrCertificateFileName = argStrCertificateFileName;
            StrPassword = argStrPassword;
            strCuit = argStrCuit;
            strIdPersona = argStrIdPersona;

            try
            {
                if (!CSwsAfipChecktTAPersona())
                {
                    BIsConnect = CSwsAfipAAGetTRA();
                }

                //if (BIsHomologacion)
                //{
                //    oCSwsAfipPersona.Url = strURLWebServicePersonaHomologacion;
                //}
                //else
                //{
                //    oCSwsAfipPersona.Url = strURLWebServicePersonaProduccion;
                //}

                oPersonaReturn = oCSwsAfipPersona.getPersona(strToken, StrSign, NumCuit, NumIdPersona);
                if (oPersonaReturn == null)
                {
                    return lbReturn;
                }

                if (oPersonaReturn.datosGenerales != null)
                {
                    StrApellido = oPersonaReturn.datosGenerales.apellido;
                    StrNombre = oPersonaReturn.datosGenerales.nombre;
                    StrRazonSocial = oPersonaReturn.datosGenerales.razonSocial;
                    StrtipoClave = oPersonaReturn.datosGenerales.tipoClave;
                    StrtipoPersona = oPersonaReturn.datosGenerales.tipoPersona;
                    StrestadoClave = oPersonaReturn.datosGenerales.estadoClave;
                    StrfechaContrato = oPersonaReturn.datosGenerales.fechaContratoSocial.ToString();
                    StrmesCierre = oPersonaReturn.datosGenerales.mesCierre.ToString();
                    StridPersona = oPersonaReturn.datosGenerales.idPersona.ToString();
                }


                if (oPersonaReturn.errorConstancia != null)
                {
                    StrXmErrorConstancia = CSGetError(oPersonaReturn.errorConstancia.error);
                }

                if (oPersonaReturn.errorMonotributo != null)
                {
                    StrXmlErrorMonotributo = CSGetError(oPersonaReturn.errorMonotributo.error);
                }

                if (oPersonaReturn.errorRegimenGeneral != null)
                {
                    StrXmlErrorRegimenGeneral = CSGetError(oPersonaReturn.errorRegimenGeneral.error);
                }

                lcsXmlReturn.Serialize(lstrXmlReturn, oPersonaReturn);
                StrXmlRespuesta = lstrXmlReturn.ToString();

                if (oPersonaReturn.datosGenerales != null)
                {
                    XmlSerializer lcXmlDatosenGeneral = new XmlSerializer(oPersonaReturn.datosGenerales.GetType());
                    lcXmlDatosenGeneral.Serialize(lstrXmlReturn, oPersonaReturn.datosGenerales);
                    StrXmlRespuestaDatosGeneral = lstrXmlReturn.ToString();
                    loDomicilioFiscal = oPersonaReturn.datosGenerales.domicilioFiscal;
                }

                if (oPersonaReturn.datosMonotributo != null)
                {
                    XmlSerializer lcsXmlMonotributo = new XmlSerializer(oPersonaReturn.datosMonotributo.GetType());
                    lcsXmlMonotributo.Serialize(lstrXmlReturn, oPersonaReturn.datosMonotributo);

                    // Se obtiene los datos del final del XML.
                    StrXmlRespuestaMonotributo = CSgetXml(lstrXmlReturn.ToString(), "<datosMonotributo xmlns:xsi", "</datosMonotributo>");
                    StrXmlRespuestaActividad = CSgetXml(StrXmlRespuestaMonotributo, "<actividadMonotributista>", "</actividadMonotributista>");
                    StrXmlRespuestaRegimen = CSgetXml(StrXmlRespuestaMonotributo, "<categoriaMonotributo>", "</categoriaMonotributo>");
                    StrXmlRespuestaImpuesto = CSgetXml(StrXmlRespuestaMonotributo, "<impuesto>", "</impuesto>");
                    StrXmldomicilioFiscal = CSgetXml(StrXmlRespuesta, "<domicilioFiscal>", "</domicilioFiscal>");
                }

                if (oPersonaReturn.datosRegimenGeneral != null)
                {
                    XmlSerializer lcXmlDatosRegimen = new XmlSerializer(oPersonaReturn.datosRegimenGeneral.GetType());
                    lcXmlDatosRegimen.Serialize(lstrXmlReturn, oPersonaReturn.datosRegimenGeneral);
                    StrXmlRespuestaRegimen = lstrXmlReturn.ToString().Substring(lstrXmlReturn.ToString().IndexOf("<datosRegimenGeneral xmlns:xsi") - 41);

                    // Separa los XML referentes a Actividades comerciales, Impuestos inscriptos y Regímenes de retencion y otros.
                    StrXmlRespuestaActividad = CSgetXml(StrXmlRespuestaRegimen, "<actividad>", "</actividad>");
                    StrXmlRespuestaImpuesto = CSgetXml(StrXmlRespuestaRegimen, "<impuesto>", "</impuesto>");
                    StrXmlRespuestaRegimen = CSgetXml(StrXmlRespuestaRegimen, "<regimen>", "</regimen>");

                    // Obtengo el XML correspondiente al domicilio.
                    XmlSerializer lcXmlDomicilio = new XmlSerializer(loDomicilioFiscal.GetType());
                    lcXmlDomicilio.Serialize(lstrXmlReturn, loDomicilioFiscal);
                    //StrXmldomicilioFiscal = lstrXmlReturn.ToString().Substring(lstrXmlReturn.ToString().IndexOf(value: "<domicilio xmlns:xsi") - 41);
                    StrXmldomicilioFiscal = CSgetXml(lstrXmlReturn.ToString(), "<domicilio xmlns:xsi", "</domicilio>");
                }

                lbReturn = true;
            }
            catch (System.Exception oEx)
            {

                throw new Exception(oEx.Message);
            }

            return lbReturn;
        }

        /// <summary>
        /// Obtiene un TRA del ws de autenticación y autorización de la Afip para la consulta de Personas.
        /// </summary>
        /// <returns>Verdadero si se obtuvo con éxito un Ticket de requermiento de acceso.</returns>    
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = BIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    TAuthRequestExpiration = wsAfipAA.ExpirationDatetime;

                    StrSign = wsAfipAA.Sign;
                    StrToken = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    StrToken = "";
                    StrToken = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }
        }

        /// <summary>
        /// Verifica si el tiket de acceso TAR es válido o existe.
        /// </summary>
        /// <returns>Verdadero si existe un TAR, Falso si no.</returns>
        private bool CSwsAfipChecktTAPersona()
        {
            bool lbReturn = false;

            if (BIsConnect && StrToken != "" && TAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10))
            {
                lbReturn = true;
            }

            return lbReturn;

        }

        /// <summary>
        /// Particiona un XML segun los string desde y hasta.
        /// </summary>
        /// <param name="argStrxXml">XML a particionar</param>
        /// <param name="argStrFirstChar">Cadena a partir de dónde se empieza la partición.</param>
        /// <param name="argStrLastChar">Cadena a hasta dónde se realiza la partición.</param>
        /// <returns>XML particionado</returns>
        private string CSgetXml(string argStrxXml, string argStrFirstChar, string argStrLastChar)
        {
            string lcXml = "";
            lcXml = argStrxXml;
            Int32 lnFirstIndex = lcXml.IndexOf(argStrFirstChar);
            Int32 lnLastIndex = lcXml.LastIndexOf(argStrLastChar) - lnFirstIndex + argStrLastChar.Length;
            string lcHeader = "";
            string lcFooter = "";
            string lcXmlResturn = "";

            if (lnFirstIndex > 0 && lnLastIndex >= 0)
            {
                lcHeader = "<?xml version='1.0' encoding='utf-16'?>" + "\r\n";
                lcHeader = lcHeader + "<Respuesta xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>" + "\r\n";
                lcFooter = "\r\n" + "</Respuesta>";

                // se cambian el 1 y 0 para que no sea tomado como true y false.
                lcXmlResturn = lcXml.Substring(lnFirstIndex, lnLastIndex);
                lcXmlResturn = lcXmlResturn.Replace(">1<", ">01<");
                lcXmlResturn = lcXmlResturn.Replace(">0<", ">00<");
                lcXmlResturn = lcXmlResturn.Replace(">.<", "> <");

                return lcHeader + lcXmlResturn + lcFooter;
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// Transforma a XML la respuesta de error de la CUIT consultada.
        /// </summary>
        /// <param name="argError">Array con el string de los errores.</param>
        /// <returns>XML con la respuesta del error.</returns>
        private string CSGetError(string[] argError)
        {
            string strError = "";
            string lcHeader = "";
            string lcFooter = "";

            lcHeader = "<?xml version='1.0' encoding='utf-16'?>" + "\r\n";
            lcHeader = lcHeader + "<Respuesta xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>" + "\r\n";
            lcFooter = "</Respuesta>";

            if (argError != null)
            {
                foreach (string line in argError)
                {
                    strError = strError + "<errorLine>\r\n<error>" + line + "</error>\r\n</errorLine>\r\n";
                }
                strError = lcHeader + strError + lcFooter;
            }

            return strError;
        }

        #endregion

    }

    [ProgId("CatedralSoftware.wsAfipBFEv1")]
    [Guid("CE8789AB-6A19-465C-BE01-5969D5E113AD")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipBFEv1
    {

        #region Fields
        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEBonosHomologacion = "https://wswhomo.afip.gov.ar/wsbfev1/service.asmx?WSDL";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFEBonosProduccion = "https://servicios1.afip.gov.ar/wsbfev1/service.asmx?WSDL";
        private string strWebServiceName = "wsbfe";


        private string strCuit;
        private string strCertificateFileName;
        private string strPassword;
        private bool bIsHomologacion = false;

        private ar.gov.afip.servicios1bonos.ServiceSoapClient oCSwsAfipBFE = new ar.gov.afip.servicios1bonos.ServiceSoapClient();
        private ar.gov.afip.servicios1bonos.ClsBFEAuthRequest oCSwsAfipAuthRequest = new ar.gov.afip.servicios1bonos.ClsBFEAuthRequest();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        private bool bIsConnect = false;

        private ar.gov.afip.servicios1bonos.ClsBFEErr arrayErrors = null;
        private ar.gov.afip.servicios1bonos.ClsBFEEvents arrayEvents = null;
        private ar.gov.afip.servicios1bonos.Opcional[] arrayOpc = null;
        private ar.gov.afip.servicios1bonos.Item[] arrayItem = null;
        private ar.gov.afip.servicios1bonos.CbteAsoc[] arrayCbtesAsoc = null;
        private ar.gov.afip.servicios1bonos.ClsBFERequest oClsBFERequest = null;


        #endregion

        #region Properties

        private string sAppServer;
        private string sAuthServer;
        private string sDBServer;
        private int iDocTipo;
        private long lDocNro;

        private long lId;               // Identificador del requerimiento
        private string sCae;
        private string sResultado;
        private string sReproceso;      //Indica si es un reproceso “S” o “N”
        private string sObs;            // Observaciones, motivo de rechazo según tabla de motivos.

        private short nZona;            //Código de zona
        private int iTipo_cbte;         //Tipo de comprobante (ver anexo A)
        private int iPunto_vta;         //Punto de venta
        private long lCbte_nro;          //Nro. de comprobante
        private double dImp_total;      // double Importe total de la operación S;
        private double dImp_tot_conc;   //Importe total de conceptos que no integran el precio neto gravado
        private double dImp_neto;       //Importe neto gravado S
        private double dImpto_liq;      //Importe liquidado S
        private double dImpto_liq_rni;  //Impuesto liquidado a RNI o percepción a no categorizados
        private double dImp_op_ex;      //Importe de operaciones exentas
        private double dImp_perc;       //Importe de percepciones 
        private double dImp_internos;   //Importe de impuestos internos 
        private string sImp_moneda_Id;  //Código de moneda(ver anexo A) 
        private double dImp_moneda_ctz; //Cotización de moneda 
        private string sFecha_cbte_orig;//Fecha de comprobante ingreso(yyyymmdd) 
        private string sFecha_cbte_cae; //Fecha de comprobante otorgado en caso de omitirla en la presentación(yyyymmdd)
        private string sFecha_vto_pago; //Fecha de vencimiento de pago.Solo para Factura de Credito MiPyme (yyyymmdd)
        private string sFecha_cae;      // string Fecha de autorización (yyyymmdd) 




        public string SCae
        {
            get
            {
                return sCae;
            }
            set
            {
                sCae = value;
            }
        }
        public string SResultado
        {
            get
            {
                return sResultado;
            }
            set
            {
                sResultado = value;
            }
        }

        public short NZona
        {
            get
            {
                return nZona;
            }
            set
            {
                nZona = value;
            }
        }

        public int ITipo_cbte
        {
            get
            {
                return iTipo_cbte;
            }
            set
            {
                iTipo_cbte = value;
            }
        }

        public int IPunto_vta
        {
            get
            {
                return iPunto_vta;
            }
            set
            {
                iPunto_vta = value;
            }
        }

        public long LCbte_nro
        {
            get
            {
                return lCbte_nro;
            }
            set
            {
                lCbte_nro = value;
            }
        }

        public double DImp_total
        {
            get
            {
                return dImp_total;
            }
            set
            {
                dImp_total = value;
            }
        }

        public double DImp_tot_conc
        {
            get
            {
                return dImp_tot_conc;
            }
            set
            {
                dImp_tot_conc = value;
            }
        }

        public double DImp_neto
        {
            get
            {
                return dImp_neto;
            }
            set
            {
                dImp_neto = value;
            }
        }
        public double DImpto_liq
        {
            get
            {
                return dImpto_liq;
            }
            set
            {
                dImpto_liq = value;
            }
        }
        public double DImpto_liq_rni
        {
            get
            {
                return dImpto_liq_rni;
            }
            set
            {
                dImpto_liq_rni = value;
            }
        }

        public double DImp_op_ex
        {
            get
            {
                return dImp_op_ex;
            }
            set
            {
                dImp_op_ex = value;
            }
        }

        public double DImp_perc
        {
            get
            {
                return dImp_perc;
            }
            set
            {
                dImp_perc = value;
            }
        }

        public double DImp_internos
        {
            get
            {
                return dImp_internos;
            }
            set
            {
                dImp_internos = value;
            }
        }

        public string SImp_moneda_Id
        {
            get
            {
                return sImp_moneda_Id;
            }
            set
            {
                sImp_moneda_Id = value;
            }
        }

        public double DImp_moneda_ctz
        {
            get
            {
                return dImp_moneda_ctz;
            }
            set
            {
                dImp_moneda_ctz = value;
            }
        }

        public string SFecha_cbte_orig
        {
            get
            {
                return sFecha_cbte_orig;
            }
            set
            {
                sFecha_cbte_orig = value;
            }
        }

        public string SFecha_cbte_cae
        {
            get
            {
                return sFecha_cbte_cae;
            }
            set
            {
                sFecha_cbte_cae = value;
            }
        }

        public string SFecha_cae
        {
            get
            {
                return sFecha_cae;
            }
            set
            {
                sFecha_cae = value;
            }
        }

        public string SFecha_vto_pago
        {
            get { return sFecha_vto_pago; }
            set { sFecha_vto_pago = value; }
        }

        public string SReproceso
        {
            get { return sReproceso; }
            set { sReproceso = value; }
        }

        public long LId
        {
            get { return lId; }
            set { lId = value; }
        }

        public string SObs
        {
            get { return sObs; }
            set { sObs = value; }

        }

        public Item[] ArrayItem
        {
            get { return arrayItem; }
            set { arrayItem = value; }
        }

        public CbteAsoc[] ArrayCbtesAsoc
        {
            get { return arrayCbtesAsoc; }
            set { arrayCbtesAsoc = value; }
        }

        public ClsBFERequest OClsBFERequest
        {
            get { return oClsBFERequest; }
            set { oClsBFERequest = value; }
        }

        public Opcional[] ArrayOpc
        {
            get { return arrayOpc; }
            set { arrayOpc = value; }
        }

        public int IDocTipo
        {
            get { return iDocTipo; }
            set { iDocTipo = value; }
        }

        public long LDocNro
        {
            get { return lDocNro; }
            set { lDocNro = value; }
        }




        #endregion

        #region Methods

        public CSwsAfipBFEv1() { } //Cosntructor


        /// <summary>
        /// Establece la conexión con el webservice BFE.
        /// </summary>
        /// <param name="argbIsHomologacion">True si es de prueba, Fase si es de producción.</param>
        /// <param name="argStrCuit">CUIT del contribuyente del certificado</param>
        /// <param name="argStrCertificateFileName">Certificado para autenticación</param>
        /// <param name="argStrPassword">Clve del certificado</param>
        /// <returns>True si se conecto y False si no</returns>
        public bool CSwsAfipBFEConnec(bool argbIsHomologacion, string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {

            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                return false;
            }

            bIsHomologacion = argbIsHomologacion;
            strCuit = argStrCuit;
            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;

            //if (argbIsHomologacion)
            //{
            //    oCSwsAfipBFE.Url = strURLWebServiceFEBonosHomologacion;
            //}
            //else
            //{
            //    oCSwsAfipBFE.Url = strURLWebServiceFEBonosProduccion;
            //}

            try
            {
                bIsConnect = CSwsAfipAAGetTRA();
                return bIsConnect;
            }
            catch (Exception oEx)
            {

                throw new Exception("No se pudo obtener un tiket de acceso para el web service de Bonos " + oEx.Message);

            }

        }

        /// <summary>
        /// Establece la conexión con el webservice BFE.
        /// </summary>
        /// <param name="argStrCuit">CUIT del ontribuyente del certificado</param>
        /// <param name="argStrCertificateFileName">Certificado para autenticación</param>
        /// <param name="argStrPassword">Clve del certificado</param>
        /// <returns>True si se conecto y False si no</returns>
        public bool CSwsAfipBFEConnec(string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {

            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                return false;
            }

            return CSwsAfipBFEConnec(true, argStrCuit, argStrCertificateFileName, argStrPassword);
        }

        /// <summary>
        /// Obtiene el tiket de acceso para acceder al webservice de Bonos Filscal Electrónico.
        /// </summary>
        /// <returns>True si las credenciales son válidas, False si no.</returns>
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = bIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    tAuthRequestExpiration = wsAfipAA.ExpirationDatetime;
                    oCSwsAfipAuthRequest.Cuit = Convert.ToInt64(strCuit);
                    oCSwsAfipAuthRequest.Sign = wsAfipAA.Sign;
                    oCSwsAfipAuthRequest.Token = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    oCSwsAfipAuthRequest.Cuit = 0;
                    oCSwsAfipAuthRequest.Sign = "";
                    oCSwsAfipAuthRequest.Token = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }

        }

        /// <summary>
        /// Testeo del servidor de AFIP.
        /// </summary>
        /// <returns>True si esta todo bien, False si no</returns>
        [ComVisible(true)]
        public bool CSwsAfipBFEDummy()
        {
            ar.gov.afip.servicios1bonos.DummyResponse dummyResponse = new ar.gov.afip.servicios1bonos.DummyResponse();
            bool bResult = false;

            try
            {
                dummyResponse = oCSwsAfipBFE.BFEDummy();
                sAppServer = dummyResponse.AppServer;
                sAuthServer = dummyResponse.AuthServer;
                sDBServer = dummyResponse.DbServer;

                bResult = (dummyResponse.AppServer == "OK" && dummyResponse.AuthServer == "OK" && dummyResponse.DbServer == "OK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bResult;
        }

        /// <summary>
        /// Consulta de un comprobante de Bono Electrónico específico.
        /// </summary>
        /// <param name="argiCbteTipo">Tipo de coprobante</param>
        /// <param name="argiPtoVta">Punto de venta</param>
        /// <param name="arglCbteNro">Número de comprobante</param>
        /// <returns>Devuelve true si se pudo consultar, false si no.</returns>
        [ComVisible(true)]
        public bool CSWsAfipBFECompConsultar(short argiCbteTipo, int argiPtoVta, long arglCbteNro)
        {

            ar.gov.afip.servicios1bonos.BFEGetCMPResponse oBFEGetCMPResponse = new ar.gov.afip.servicios1bonos.BFEGetCMPResponse();
            ar.gov.afip.servicios1bonos.ClsBFEGetCMP oclsBFEGetCMP = new ar.gov.afip.servicios1bonos.ClsBFEGetCMP();

            bool bReturn = false;

            try
            {
                oclsBFEGetCMP.Tipo_cbte = argiCbteTipo;
                oclsBFEGetCMP.Punto_vta = argiPtoVta;
                oclsBFEGetCMP.Cbte_nro = arglCbteNro;
                arrayErrors = oBFEGetCMPResponse.BFEErr;
                arrayEvents = oBFEGetCMPResponse.BFEEvents;

                oBFEGetCMPResponse = oCSwsAfipBFE.BFEGetCMP(oCSwsAfipAuthRequest, oclsBFEGetCMP);

                if (oBFEGetCMPResponse.BFEResultGet != null)
                {
                    LId = oBFEGetCMPResponse.BFEResultGet.Id;
                    SCae = oBFEGetCMPResponse.BFEResultGet.Cae;
                    IDocTipo = oBFEGetCMPResponse.BFEResultGet.Tipo_doc;
                    LDocNro = oBFEGetCMPResponse.BFEResultGet.Nro_doc;
                    ITipo_cbte = oBFEGetCMPResponse.BFEResultGet.Tipo_cbte;
                    IPunto_vta = oBFEGetCMPResponse.BFEResultGet.Punto_vta;
                    LCbte_nro = oBFEGetCMPResponse.BFEResultGet.Cbte_nro;
                    DImp_total = oBFEGetCMPResponse.BFEResultGet.Imp_total;
                    DImp_tot_conc = oBFEGetCMPResponse.BFEResultGet.Imp_tot_conc;
                    DImp_neto = oBFEGetCMPResponse.BFEResultGet.Imp_neto;
                    DImpto_liq = oBFEGetCMPResponse.BFEResultGet.Impto_liq;
                    DImpto_liq_rni = oBFEGetCMPResponse.BFEResultGet.Impto_liq_rni;
                    DImp_op_ex = oBFEGetCMPResponse.BFEResultGet.Imp_op_ex;
                    DImp_perc = oBFEGetCMPResponse.BFEResultGet.Imp_perc;
                    DImp_internos = oBFEGetCMPResponse.BFEResultGet.Imp_internos;
                    SImp_moneda_Id = oBFEGetCMPResponse.BFEResultGet.Imp_moneda_Id;
                    DImp_moneda_ctz = oBFEGetCMPResponse.BFEResultGet.Imp_moneda_ctz;
                    SFecha_cbte_orig = oBFEGetCMPResponse.BFEResultGet.Fecha_cbte_orig;
                    SFecha_vto_pago = oBFEGetCMPResponse.BFEResultGet.Fecha_vto_pago;
                    SFecha_cbte_cae = oBFEGetCMPResponse.BFEResultGet.Fecha_cbte_cae;
                    SFecha_cae = oBFEGetCMPResponse.BFEResultGet.Fch_venc_Cae;

                    arrayItem = oBFEGetCMPResponse.BFEResultGet.Items;
                    arrayCbtesAsoc = oBFEGetCMPResponse.BFEResultGet.CbtesAsoc;
                    arrayOpc = oBFEGetCMPResponse.BFEResultGet.Opcionales;

                    SResultado = oBFEGetCMPResponse.BFEResultGet.Resultado;

                    bReturn = sResultado == "A";
                }

            }
            catch (Exception oEx)
            {
                throw oEx;
            }

            return bReturn;
        }

        /// <summary>
        /// Obtiene el último número de comprobante.
        /// </summary>
        /// <param name="argTipoComprobante">Tipo de comprobante</param>
        /// <param name="argPuntoVenta">Punto de venta del comprobante</param>
        /// <returns>Devuelve el último número de comprobante solicitado</returns>
        [ComVisible(true)]
        public int CSWsAfipBFECompUltimoAutorizado(short argTipoComprobante, int argPuntoVenta)
        {
            ar.gov.afip.servicios1bonos.BFEResponseLast_CMP oFEResponseLast_CMP = new BFEResponseLast_CMP();
            ar.gov.afip.servicios1bonos.ClsBFE_LastCMP oclsBFE_LastCMP = new ClsBFE_LastCMP();

            try
            {

                oclsBFE_LastCMP.Cuit = oCSwsAfipAuthRequest.Cuit;
                oclsBFE_LastCMP.Pto_venta = argPuntoVenta;
                oclsBFE_LastCMP.Tipo_cbte = argTipoComprobante;
                oclsBFE_LastCMP.Sign = oCSwsAfipAuthRequest.Sign;
                oclsBFE_LastCMP.Token = oCSwsAfipAuthRequest.Token;

                oFEResponseLast_CMP = oCSwsAfipBFE.BFEGetLast_CMP(oclsBFE_LastCMP);

                if (oFEResponseLast_CMP.BFEErr != null && oFEResponseLast_CMP.BFEErr.ErrCode != 0)
                {
                    throw new Exception("No se pudo obtener el último número de comprobante para el punto de venta:" + argPuntoVenta.ToString() + ". Excepción:" + oFEResponseLast_CMP.BFEErr.ErrCode + " " + oFEResponseLast_CMP.BFEErr.ErrMsg);
                }

                return Convert.ToInt32(oFEResponseLast_CMP.BFEResult_LastCMP.Cbte_nro);
            }
            catch (Exception oEx)
            {
                throw oEx;
            }

        }


        /// <summary>
        /// Chequea el Ticket de requerimiento de acceso al ws de negocio.
        /// </summary>
        /// <returns>Verdadero si el Ticket de Requerimiento de acceso se ha obtenido y no ha expirado.</returns>
        [ComVisible(true)]
        public bool IsValidAuthRequest()
        {
            return (tAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10));
        }

        /// <summary>
        /// Obtiene el último ID de transacción con AFIP.
        /// </summary>
        /// <returns>Número de ID</returns>
        [ComVisible(true)]
        public int CSWsAfipBFEGetLast_ID()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_LastID oBFEResponse_LastID = new BFEResponse_LastID();

            try
            {
                oBFEResponse_LastID = oCSwsAfipBFE.BFEGetLast_ID(oCSwsAfipAuthRequest);

                if (oBFEResponse_LastID.BFEErr != null && oBFEResponse_LastID.BFEErr.ErrCode != 0)
                {
                    throw new Exception("No se pudo obtener el último ID, Excepción: " + oBFEResponse_LastID.BFEErr.ErrCode + " " + oBFEResponse_LastID.BFEErr.ErrMsg);
                }
                else
                {
                    return Convert.ToInt32(oBFEResponse_LastID.BFEResultGet.Id);
                }
            }
            catch (Exception oEx)
            {
                throw oEx;
            }

        }

        /// <summary>
        /// Check interno de la clase
        /// </summary>
        /// <returns>Verdadero si todo Ok.</returns>
        [ComVisible(false)]
        private bool CSwsAfipCheck()
        {
            if (!bIsConnect)
            {
                throw new Exception("No esta conectado al web service de negocio.");
            }
            if (!IsValidAuthRequest())
            {
                throw new Exception("El Ticket de Requerimiento de Acceso ha expirado.");
            }

            return true;
        }


        /// <summary>
        /// Inicializa los Array del Bono Fiscal Electrónico.
        /// </summary>
        /// <param name="argTotalItems">Cantidad total de ítems de productos que tendrá la factura.</param>
        /// <param name="argTotalDocAsociados">Catidad total de documentos asociados que tendrá la factura.</param>
        /// <param name="argTotalOpcionales">Catidad total de parámetros opcionales que tendrá la factura.</param>
        /// <returns>true si se pudieron setear, false si no</returns>
        public bool CSwsAfipBFEInit(int argTotalItems, int argTotalDocAsociados, int argTotalOpcionales)
        {
            if (!CSwsAfipCheck()) { return false; }

            bool bReturn = false;

            try
            {
                if (argTotalItems >= 0)
                {
                    ArrayItem = new Item[argTotalItems];
                    bReturn = bReturn || true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalDocAsociados > 0)
                {
                    ArrayCbtesAsoc = new CbteAsoc[argTotalDocAsociados];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalOpcionales > 0)
                {
                    ArrayOpc = new Opcional[argTotalOpcionales];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }


                SCae = "";
                SResultado = "";
                NZona = 0;
                ITipo_cbte = 0;
                IPunto_vta = 0;
                LCbte_nro = 0;
                DImp_total = 0;
                DImp_tot_conc = 0;
                DImp_neto = 0;
                DImpto_liq = 0;
                DImpto_liq_rni = 0;
                DImp_op_ex = 0;
                DImp_perc = 0;
                DImp_internos = 0;
                SImp_moneda_Id = "";
                DImp_moneda_ctz = 0;
                SFecha_cbte_orig = "";
                SFecha_cbte_cae = "";
                SFecha_cae = "";
                SFecha_vto_pago = "";
                SReproceso = "";
                LId = 0;
                SObs = "";
            }

            catch (Exception oEx)
            {
                throw new Exception("No se pudo inicializar las variables. " + oEx.Message);
            }

            return bReturn;

        }

        /// <summary>
        /// Crea el comprobante para luego solicitar el CAE.
        /// </summary>
        /// <param name="argId">ID del comprobante, debe ser único</param>
        /// <param name="argTipoDoc">Código de documento identificatorio del comprador</param>
        /// <param name="argNroDoc">Nro. De identificación del comprador</param>
        /// <param name="argZona">Código de zona</param>
        /// <param name="argtipoCbte">Tipo de comprobante</param>
        /// <param name="argPuntoVta">Punto de venta</param>
        /// <param name="argCbtNro">Nro. De comprobante</param>
        /// <param name="argImpTotal">Importe total de la operación</param>
        /// <param name="argImpTotConc">Importe total de conceptos que no integran el precio neto gravado</param>
        /// <param name="argImpNeto">Importe neto gravado</param>
        /// <param name="argImptoLiq">Importe liquidado</param>
        /// <param name="argImptoLiqRni">Impuesto liquidado a RNI o percepción a no categorizados</param>
        /// <param name="argImpOpEx">Importe de operaciones exentas</param>
        /// <param name="argImpPerc">Importe de percepciones</param>
        /// <param name="argImpInternos">Importe de impuestos internos</param>
        /// <param name="argImpMonedaId">Código de moneda(BFEGetPARAM_MON)</param>
        /// <param name="argImpMonedaCot">Cotización de moneda</param>
        /// <param name="argImpiibb">Importe de percepción de Ingresos Brutos</param>
        /// <param name="argImpPercMun">Impporte de percepciones Municipales</param>
        /// <param name="argFechaCbte">Fecha de comprobante (yyyymmdd)</param>
        /// <param name="argFechaVtoPago">Fecha de comprobante (yyyymmdd), si es MiPyMEs argFechaVtoPago debe ser mayor a argFechaCbte</param>
        /// <returns>true si todo salio bien, false si no.</returns>
        public bool CSwsAfipBFEComprobante(int argId,
                                            short argTipoDoc,
                                            string argNroDoc,
                                            short argZona,
                                            short argtipoCbte,
                                            int argPuntoVta,
                                            int argCbtNro,
                                            double argImpTotal,
                                            double argImpTotConc,
                                            double argImpNeto,
                                            double argImptoLiq,
                                            double argImptoLiqRni,
                                            double argImpOpEx,
                                            double argImpPerc,
                                            double argImpInternos,
                                            string argImpMonedaId,
                                            double argImpMonedaCot,
                                            double argImpiibb,
                                            double argImpPercMun,
                                            string argFechaCbte,
                                            string argFechaVtoPago)
        {

            oClsBFERequest = new ClsBFERequest();

            try
            {
                oClsBFERequest.Id = argId;
                oClsBFERequest.Cbte_nro = Convert.ToInt64(argCbtNro);   // Por compatibilidad con VFP9
                oClsBFERequest.Fecha_cbte = argFechaCbte;
                oClsBFERequest.Impto_liq = argImptoLiq;
                oClsBFERequest.Impto_liq_rni = argImptoLiqRni;
                oClsBFERequest.Imp_iibb = argImpiibb;
                oClsBFERequest.Imp_internos = argImpInternos;
                oClsBFERequest.Imp_moneda_ctz = argImpMonedaCot;
                oClsBFERequest.Imp_moneda_Id = argImpMonedaId;
                oClsBFERequest.Imp_neto = argImpNeto;
                oClsBFERequest.Imp_op_ex = argImpOpEx;
                oClsBFERequest.Imp_perc = argImpPerc;
                oClsBFERequest.Imp_perc_mun = argImpPercMun;
                oClsBFERequest.Imp_total = argImpTotal;
                oClsBFERequest.Imp_tot_conc = argImpTotConc;
                oClsBFERequest.Nro_doc = Convert.ToInt64(argNroDoc);    // Por compatibilidad con VFP9
                oClsBFERequest.Punto_vta = argPuntoVta;
                oClsBFERequest.Tipo_cbte = argtipoCbte;
                oClsBFERequest.Tipo_doc = argTipoDoc;
                oClsBFERequest.Zona = argZona;
                oClsBFERequest.Fecha_cbte = argFechaCbte;
                oClsBFERequest.Fecha_vto_pago = argFechaVtoPago;

                if (this.ArrayItem != null && this.ArrayItem.Length > 0)
                {

                    oClsBFERequest.Items = this.ArrayItem;
                }
                else
                {
                    throw new Exception("Falta definir al menos un Ítem del comprobante.");
                }

                if (this.ArrayOpc != null && this.ArrayOpc.Length > 0)
                {
                    oClsBFERequest.Opcionales = this.ArrayOpc;
                }

                if (this.arrayCbtesAsoc != null && this.ArrayCbtesAsoc.Length > 0)
                {
                    oClsBFERequest.CbtesAsoc = this.ArrayCbtesAsoc;
                }

                return true;
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo definir el comprobante. " + oEx.Message);
            }

        }


        /// <summary>
        /// Agrega un ítem al detalle del comprobante.
        /// </summary>
        /// <param name="argTotalFilas">Total de ítems que se agregaran</param>
        /// <param name="argFila">Ítem que se esta agregando</param>
        /// <param name="argCodigo_ncm"></param>
        /// <param name="argCodigo_sec"></param>
        /// <param name="argDescrip"></param>
        /// <param name="argCdad"></param>
        /// <param name="argUmed"></param>
        /// <param name="argpPecio_uni"></param>
        /// <param name="argImp_bonif"></param>
        /// <param name="argImp_total"></param>
        /// <param name="argIva_id"></param>
        /// <returns>True o False</returns>
        public bool CSwsAfipBFE_Add_Item(int argTotalFilas, int argFila, string argCodigo_ncm, string argCodigo_sec, string argDescrip,
                                         double argCdad, int argUmed, double argpPecio_uni,
                                         double argImp_bonif, double argImp_total, short argIva_id)
        {
            ar.gov.afip.servicios1bonos.Item aItem = new ar.gov.afip.servicios1bonos.Item();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    //arrayItem = new Item[argFila];
                    aItem.Pro_codigo_ncm = argCodigo_ncm;
                    aItem.Pro_codigo_ncm = argCodigo_sec;
                    aItem.Pro_ds = argDescrip;
                    aItem.Pro_qty = argCdad;
                    aItem.Pro_umed = argUmed;
                    aItem.Pro_precio_uni = argpPecio_uni;
                    aItem.Imp_bonif = argImp_bonif;
                    aItem.Imp_total = argImp_total;
                    aItem.Iva_id = argIva_id;
                    arrayItem[argFila - 1] = aItem;

                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Items es cero o es mayor que el total de items del comprobante.");
                }


            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem " + oEx.Message);
            }

        }

        /// <summary>
        /// Agrega valores aopcionales al comprobante
        /// </summary>
        /// <param name="argTotalFilas">Total de filas que se agregarán</param>
        /// <param name="argFila">Ítem que se esta agregando</param>
        /// <param name="argId">ID del ítem, según tabla de AFIP</param>
        /// <param name="argValor">Valor correspondiente al ID, según tabla AFIP</param>
        /// <returns>true si se pudo agregar, false si no</returns>
        public bool CSwsAfipBFE_Add_Opcional(int argTotalFilas, int argFila, string argId, string argValor)
        {
            ar.gov.afip.servicios1bonos.Opcional aOpcional = new ar.gov.afip.servicios1bonos.Opcional();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aOpcional.Id = argId;
                    aOpcional.Valor = argValor;
                    arrayOpc[argFila - 1] = aOpcional;

                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Opcionales es cero o es mayor que el total de ítems opcionales indicados.");
                }
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el valor opcional. " + oEx.Message);
            }

        }

        /// <summary>
        /// Agrega Comprobantes Asociados al Bono Fiscal Electronico.
        /// </summary>
        /// <param name="argTotalFilas">Total de comprobantes que se agregarán</param>
        /// <param name="argFila">Ítem que se esta agregando a la lista.</param>
        /// <param name="argTipo_cbte">Tipo de comprobante según tabla de AFIP</param>
        /// <param name="argPunto_vta">Número de punto de veta</param>
        /// <param name="argCbte_nro">Número de comprobante </param>
        /// <param name="argCuit">CUIT del cliente del comprobante asociado.</param>
        /// <param name="argFecha_cbte">Fecha del comprobante</param>
        /// <returns></returns>
        public bool CSwsAfipBFE_Add_CbteAsoc(int argTotalFilas, int argFila, short argTipo_cbte, int argPunto_vta,
                                             long argCbte_nro, string argCuit, string argFecha_cbte)
        {
            ar.gov.afip.servicios1bonos.CbteAsoc aCbteAsoc = new ar.gov.afip.servicios1bonos.CbteAsoc();
            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aCbteAsoc.Cbte_nro = argCbte_nro;
                    aCbteAsoc.Punto_vta = argPunto_vta;
                    aCbteAsoc.Tipo_cbte = argTipo_cbte;
                    aCbteAsoc.Cuit = argCuit;
                    aCbteAsoc.Fecha_cbte = argFecha_cbte;
                    arrayCbtesAsoc[argFila - 1] = aCbteAsoc;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Comprobantes Asociados es cero o es mayor que el total de ítems indicados.");
                }
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el comprobante asociado. " + oEx.Message);
            }
        }

        /// <summary>
        /// Solicita el CAE del comprobante indicado en CSwsAfipBFEComprobante
        /// </summary>
        /// <returns>True si todo estuvo bien y False si no</returns>
        public bool CSwsAfipBFECAESolicitar()
        {
            ar.gov.afip.servicios1bonos.ClsBFEAuthRequest oclsBFEAuthRequest = new ar.gov.afip.servicios1bonos.ClsBFEAuthRequest();
            ar.gov.afip.servicios1bonos.BFEResponseAuthorize obFEResponseAuthorize = new BFEResponseAuthorize();

            try
            {
                obFEResponseAuthorize = oCSwsAfipBFE.BFEAuthorize(oCSwsAfipAuthRequest, oClsBFERequest);

                sFecha_cbte_orig = obFEResponseAuthorize.BFEResultAuth.Fch_cbte;
                sFecha_cae = obFEResponseAuthorize.BFEResultAuth.Fch_venc_Cae;
                sCae = obFEResponseAuthorize.BFEResultAuth.Cae;
                sResultado = obFEResponseAuthorize.BFEResultAuth.Resultado;
                sReproceso = obFEResponseAuthorize.BFEResultAuth.Reproceso;
                sObs = obFEResponseAuthorize.BFEResultAuth.Obs;

                arrayErrors = obFEResponseAuthorize.BFEErr;
                arrayEvents = obFEResponseAuthorize.BFEEvents;

                if (arrayErrors != null && arrayErrors.ErrCode != 0)
                {
                    throw new Exception("Excepcion al solicitar el CAE: Código " + arrayErrors.ErrCode.ToString() + " " + arrayErrors.ErrMsg);
                }

                return true;
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo obtener el CAE del bono. " + oEx.Message);
            }
        }

        /// <summary>
        /// Lista de valores referenciales de códigos de opcionales para los Bonos de Factura Electrónica.
        /// </summary>
        /// <returns>String con formato XML</returns>
        public string BFEGetParam_Tipo_Opc()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_Opc obFEResponse_Opc = new BFEResponse_Opc();
            StringWriter strxmlOpcionales = new StringWriter();

            try
            {
                obFEResponse_Opc = oCSwsAfipBFE.BFEGetPARAM_Tipo_Opc(oCSwsAfipAuthRequest);
                XmlSerializer xmlReturn = new XmlSerializer(obFEResponse_Opc.GetType());
                xmlReturn.Serialize(strxmlOpcionales, obFEResponse_Opc);

                return strxmlOpcionales.ToString();

            }
            catch (Exception oEx)
            {
                throw oEx;
            }

        }

        /// <summary>
        /// Lista de los códigos de alícuota de IVA.
        /// </summary>
        /// <returns>String con formato XML</returns>
        public string BFEGetParam_Tipo_Iva()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_Tipo_IVA obFEResponse_Tipo_IVA = new BFEResponse_Tipo_IVA();

            StringWriter strxmlIva = new StringWriter();

            try
            {
                obFEResponse_Tipo_IVA = oCSwsAfipBFE.BFEGetPARAM_Tipo_IVA(oCSwsAfipAuthRequest);
                XmlSerializer xmlReturn = new XmlSerializer(obFEResponse_Tipo_IVA.GetType());
                xmlReturn.Serialize(strxmlIva, obFEResponse_Tipo_IVA);

                return strxmlIva.ToString();

            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene la lista de códigos de monedas habilitadas.
        /// </summary>
        /// <returns>String con formto XML</returns>
        public string BFEGetParam_MON()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_Mon obFEResponse_Mon = new BFEResponse_Mon();
            StringWriter strxmlMoneda = new StringWriter();

            try
            {
                obFEResponse_Mon = oCSwsAfipBFE.BFEGetPARAM_MON(oCSwsAfipAuthRequest);
                XmlSerializer olRespuesta = new XmlSerializer(obFEResponse_Mon.GetType());
                olRespuesta.Serialize(strxmlMoneda, obFEResponse_Mon);

                return strxmlMoneda.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene los valores referenciales de códigos de productos
        /// </summary>
        /// <returns>String con formto XML</returns>
        public string BFEGetParam_NCM()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_NCM obFEResponse_NCM = new BFEResponse_NCM();
            StringWriter strxmlCodProducto = new StringWriter();

            try
            {
                obFEResponse_NCM = oCSwsAfipBFE.BFEGetPARAM_NCM(oCSwsAfipAuthRequest);
                XmlSerializer oRespuesta = new XmlSerializer(obFEResponse_NCM.GetType());
                oRespuesta.Serialize(strxmlCodProducto, obFEResponse_NCM);

                return strxmlCodProducto.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }

        }

        /// <summary>
        /// Obtiene los valores referenciales de códigos de Tipos de comprobante
        /// </summary>
        /// <returns>String con formto XML</returns>
        public string BFEGetParam_Tipo_cbte()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_Tipo_Cbte obFEResponse_Tipo_Cbte = new BFEResponse_Tipo_Cbte();
            StringWriter strxmlComprobante = new StringWriter();

            try
            {
                obFEResponse_Tipo_Cbte = oCSwsAfipBFE.BFEGetPARAM_Tipo_Cbte(oCSwsAfipAuthRequest);
                XmlSerializer oRespuesta = new XmlSerializer(obFEResponse_Tipo_Cbte.GetType());
                oRespuesta.Serialize(strxmlComprobante, obFEResponse_Tipo_Cbte);

                return strxmlComprobante.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene los valores referenciales de códigos de zona
        /// </summary>
        /// <returns>String con formto XML</returns>
        public string BFEGetParam_Zonas()
        {
            ar.gov.afip.servicios1bonos.BFEResponse_Zon obFEResponse_Zon = new BFEResponse_Zon();
            StringWriter lstrXmlComprobante = new StringWriter();

            try
            {
                obFEResponse_Zon = oCSwsAfipBFE.BFEGetPARAM_Zonas(oCSwsAfipAuthRequest);

                XmlSerializer oRespuesta = new XmlSerializer(obFEResponse_Zon.GetType());
                oRespuesta.Serialize(lstrXmlComprobante, obFEResponse_Zon);

                return lstrXmlComprobante.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        #endregion
    }

    /// <summary>
    /// Clase de Factura Electrónica Comprobantes de Turismo
    /// </summary>
    [ProgId("CatedralSoftware.wsAfipFET")]
    [Guid("03A16655-3677-4404-A236-6BB69137B462")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class CSwsAfipFET
    {
        #region Fields
        private string strURLWebServiceAAHomologacion = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFETHomologacion = "https://fwshomo.afip.gov.ar/wsct/CTService?wsdl";
        private string strURLWebServiceAAProduccion = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";
        private string strURLWebServiceFETProduccion = "https://serviciosjava.afip.gob.ar/wsct/CTService?wsdl";
        private string strWebServiceName = "wsct";

        private string strCuit;
        private string strCertificateFileName;
        private string strPassword;
        private bool bIsHomologacion = false;

        private ar.gob.afip.serviciosjava.CTServicePortTypeClient oCTService = new ar.gob.afip.serviciosjava.CTServicePortTypeClient();
        private ar.gob.afip.serviciosjava.AuthRequestType oCSwsAfipAuthRequest = new ar.gob.afip.serviciosjava.AuthRequestType();
        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        private bool bIsConnect = false;

        private ItemType[] arrayItems = null;
        private OtroTributoType[] arrayOtrosTributos = null;
        private ComprobanteAsociadoType[] arrayComprobantesAsociados = null;
        private TipoDatoAdicionalType[] arrayDatosAdicionales = null;
        private FormaPagoType[] arrayFormasPago = null;
        private CodigoDescripcionType[] arrayError = null;
        private CodigoDescripcionType[] arrayObs = null;
        private CodigoDescripcionStringType[] arrayErrorFormato = null;
        private SubtotalIVAType[] arraySubtotalesIVA = null;

        private ComprobanteType oComprobanteRequest = null;

        private string sCae;
        private string sResultado;          // Resultado { 'A', 'O', 'R' }
        private string sReproceso;          //Indica si es un reproceso “S” o “N”
        private string sObs;                // Observaciones, motivo de rechazo según tabla de motivos.

        private string fechaVencimientoCAE; //Fecha de vencimiento del CAE de la factura.
        private string fechaEmision;        //Fecha de emisión del comprobante.

        private string sCodigoMoneda;
        private short iCodigoPais;
        private short iCodigoRelacionEmisorReceptor;
        private string sCodigoTipoAutorizacion;
        private short iCodigoTipoComprobante;
        private short iCodigoTipoDocumento;
        private double dCotizacionMoneda;
        private string sDomicilioReceptor;

        private string sIdImpositivo;
        private double dImporteExento;
        private double dImporteGravado;
        private double dImporteNoGravado;
        private double dImporteOtrosTributos;
        private double dImporteReintegro;
        private double dImporteTotal;
        private string sNumeroComprobante;
        private string sNumeroDocumento;
        private short iNumeroPuntoVenta;
        #endregion

        #region Properties
        private string sAppServer = "";
        private string sAuthServer = "";
        private string sDBServer = "";

        public ItemType[] ArrayItems { get { return arrayItems; } set { arrayItems = value; } }
        public OtroTributoType[] ArrayOtrosTributos { get { return arrayOtrosTributos; } set { arrayOtrosTributos = value; } }
        public ComprobanteAsociadoType[] ArrayComprobantesAsociados { get { return arrayComprobantesAsociados; } set { arrayComprobantesAsociados = value; } }
        public CodigoDescripcionType[] ArrayError { get { return arrayError; } set { arrayError = value; } }
        public CodigoDescripcionType[] ArrayObs { get { return arrayObs; } set { arrayObs = value; } }
        public CodigoDescripcionStringType[] ArrayErrorFormato { get { return arrayErrorFormato; } set { arrayErrorFormato = value; } }
        public SubtotalIVAType[] ArraySubtotalesIVA { get { return arraySubtotalesIVA; } set { arraySubtotalesIVA = value; } }
        public TipoDatoAdicionalType[] ArrayDatosAdicionales { get { return arrayDatosAdicionales; } set { arrayDatosAdicionales = value; } }
        public FormaPagoType[] ArrayFormasPago { get { return arrayFormasPago; } set { arrayFormasPago = value; } }
        public string SCae { get { return sCae; } set { sCae = value; } }
        public string SResultado { get { return sResultado; } set { sResultado = value; } }
        public string SReproceso { get { return sReproceso; } set { sReproceso = value; } }
        public string SObs { get { return sObs; } set { sObs = value; } }
        public string FechaVencimientoCAE { get { return fechaVencimientoCAE; } set { fechaVencimientoCAE = value; } }
        public string FechaEmision { get { return fechaEmision; } set { fechaEmision = value; } }
        public string SCodigoMoneda { get { return sCodigoMoneda; } set { sCodigoMoneda = value; } }
        public short ICodigoPais { get { return iCodigoPais; } set { iCodigoPais = value; } }
        public short ICodigoRelacionEmisorReceptor { get { return iCodigoRelacionEmisorReceptor; } set { iCodigoRelacionEmisorReceptor = value; } }
        public string SCodigoTipoAutorizacion { get { return sCodigoTipoAutorizacion; } set { sCodigoTipoAutorizacion = value; } }
        public short ICodigoTipoComprobante { get { return iCodigoTipoComprobante; } set { iCodigoTipoComprobante = value; } }
        public short ICodigoTipoDocumento { get { return iCodigoTipoDocumento; } set { iCodigoTipoDocumento = value; } }
        public double DCotizacionMoneda { get { return dCotizacionMoneda; } set { dCotizacionMoneda = value; } }
        public string SDomicilioReceptor { get { return sDomicilioReceptor; } set { sDomicilioReceptor = value; } }
        public string SIdImpositivo { get { return sIdImpositivo; } set { sIdImpositivo = value; } }
        public double DImporteExento { get { return dImporteExento; } set { dImporteExento = value; } }
        public double DImporteGravado { get { return dImporteGravado; } set { dImporteGravado = value; } }
        public double DImporteNoGravado { get { return dImporteNoGravado; } set { dImporteNoGravado = value; } }
        public double DImporteOtrosTributos { get { return dImporteOtrosTributos; } set { dImporteOtrosTributos = value; } }
        public double DImporteReintegro { get { return dImporteReintegro; } set { dImporteReintegro = value; } }
        public double DImporteTotal { get { return dImporteTotal; } set { dImporteTotal = value; } }

        public string SNumeroDocumento { get { return sNumeroDocumento; } set { sNumeroDocumento = value; } }
        public short INumeroPuntoVenta { get { return iNumeroPuntoVenta; } set { iNumeroPuntoVenta = value; } }
        public string SNumeroComprobante { get { return sNumeroComprobante; } set { sNumeroComprobante = value; } }


        #endregion

        #region Method

        public CSwsAfipFET() { } //Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argbIsHomologacion"></param>
        /// <param name="argStrCuit"></param>
        /// <param name="argStrCertificateFileName"></param>
        /// <param name="argStrPassword"></param>
        /// <returns></returns>
        public bool CSwsAfipFETConnec(bool argbIsHomologacion, string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                return false;
            }

            bIsHomologacion = argbIsHomologacion;
            strCuit = argStrCuit;
            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;

            //if (argbIsHomologacion)
            //{
            //    oCTService.Url = strURLWebServiceFETHomologacion;
            //}
            //else
            //{
            //    oCTService.Url = strURLWebServiceFETProduccion;
            //}

            try
            {
                bIsConnect = CSwsAfipAAGetTRA();
                return bIsConnect;
            }
            catch (Exception oEx)
            {

                throw new Exception("No se pudo obtener un tiket de acceso para el web service de comprobante clase T " + oEx.Message);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argStrCuit"></param>
        /// <param name="argStrCertificateFileName"></param>
        /// <param name="argStrPassword"></param>
        /// <returns></returns>
        public bool CSwsAfipFETConnec(string argStrCuit, string argStrCertificateFileName, string argStrPassword)
        {
            if (argStrCuit == "" || argStrCertificateFileName == "")
            {
                return false;
            }

            return CSwsAfipFETConnec(true, argStrCuit, argStrCertificateFileName, argStrPassword);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();

            bool bReturn = false;

            wsAfipAA.IsHomologacion = bIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAHomologacion = strURLWebServiceAAHomologacion;
            wsAfipAA.URLWebServiceAAProduccion = strURLWebServiceAAProduccion;

            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                {
                    tAuthRequestExpiration = wsAfipAA.ExpirationDatetime;
                    oCSwsAfipAuthRequest.cuitRepresentada = Convert.ToInt64(strCuit);
                    oCSwsAfipAuthRequest.sign = wsAfipAA.Sign;
                    oCSwsAfipAuthRequest.token = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    oCSwsAfipAuthRequest.cuitRepresentada = 0;
                    oCSwsAfipAuthRequest.sign = "";
                    oCSwsAfipAuthRequest.token = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw exceptionwsAfipAA;
            }

        }

        /// <summary>
        /// Permite verificar el funcionamiento del presente WS
        /// </summary>
        /// <returns>true si funciona OK</returns>
        [ComVisible(true)]
        public bool CSwsAfipFETDummy()
        {
            ar.gob.afip.serviciosjava.DummyReturnType dummyResponse = new ar.gob.afip.serviciosjava.DummyReturnType();
            bool bResult = false;

            //try
            //{
            //    dummyResponse = oCTService.dummy();
            //    sAppServer = dummyResponse.dummyReturn.appserver;
            //    sAuthServer = dummyResponse.dummyReturn.authserver;
            //    sDBServer = dummyResponse.dummyReturn.authserver;

            //    bResult = (dummyResponse.dummyReturn.appserver == "OK" && dummyResponse.dummyReturn.authserver == "OK" && dummyResponse.dummyReturn.dbserver == "OK");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return bResult;
        }


        [ComVisible(true)]
        public int CSWsAfipFETCompUltimoAutorizado(short argTipoComprobante, short argPuntoVenta)
        {
            ar.gob.afip.serviciosjava.ConsultarUltimoComprobanteAutorizadoReturnType oConsultarUltimoComprobante = new ar.gob.afip.serviciosjava.ConsultarUltimoComprobanteAutorizadoReturnType();

            try
            {
                oConsultarUltimoComprobante = oCTService.consultarUltimoComprobanteAutorizado(oCSwsAfipAuthRequest, argTipoComprobante, argPuntoVenta);

                if (oConsultarUltimoComprobante.arrayErrores != null && oConsultarUltimoComprobante.arrayErrores[0].codigo != 0)
                {
                    throw new Exception("No se pudo obtener el último número de comprobante para el punto de venta:" + argPuntoVenta.ToString() + ". Excepción:" + oConsultarUltimoComprobante.arrayErrores[0].codigo + " " + oConsultarUltimoComprobante.arrayErrores[0].descripcion);
                }

                return Convert.ToInt32(oConsultarUltimoComprobante.numeroComprobante);
            }
            catch (Exception oEx)
            {
                throw oEx;
            }

        }



        /// <summary>
        /// Consulta un comprobante por numero tipo y punto de venta.
        /// </summary>
        /// <param name="argiCbteTipo">Tipo de comprobante, ej Factura T = 195</param>
        /// <param name="argiPtoVta">Punto de venta a consultar</param>
        /// <param name="argsCbteNro">Número de comprobante a consultar</param>
        /// <returns>Verdadero si la consulta fue exitosa</returns>
        [ComVisible(true)]
        public bool CSWsAfipFETCompConsultar(short argiCbteTipo, short argiPtoVta, string argsCbteNro)
        {
            ConsultarComprobanteReturnType oConsultaComprobante = new ConsultarComprobanteReturnType();

            try
            {
                oConsultaComprobante = oCTService.consultarComprobanteTipoPVentaNro(oCSwsAfipAuthRequest, argiCbteTipo, argiPtoVta, Convert.ToInt64(argsCbteNro));
                ArrayError = oConsultaComprobante.arrayErrores;
                ArrayErrorFormato = oConsultaComprobante.arrayErroresFormato;
                ArrayObs = oConsultaComprobante.arrayObservaciones;

                ArrayComprobantesAsociados = oConsultaComprobante.comprobante.arrayComprobantesAsociados;
                ArrayDatosAdicionales = oConsultaComprobante.comprobante.arrayDatosAdicionales;
                ArrayFormasPago = oConsultaComprobante.comprobante.arrayFormasPago;
                ArrayItems = oConsultaComprobante.comprobante.arrayItems;
                ArrayOtrosTributos = oConsultaComprobante.comprobante.arrayOtrosTributos;
                ArraySubtotalesIVA = oConsultaComprobante.comprobante.arraySubtotalesIVA;
                SCae = Convert.ToString(oConsultaComprobante.comprobante.codigoAutorizacion);
                SCodigoMoneda = oConsultaComprobante.comprobante.codigoMoneda;
                ICodigoPais = oConsultaComprobante.comprobante.codigoPais;
                ICodigoRelacionEmisorReceptor = oConsultaComprobante.comprobante.codigoRelacionEmisorReceptor;
                SCodigoTipoAutorizacion = oConsultaComprobante.comprobante.codigoTipoAutorizacion.ToString();
                ICodigoTipoComprobante = oConsultaComprobante.comprobante.codigoTipoComprobante;
                ICodigoTipoDocumento = oConsultaComprobante.comprobante.codigoTipoDocumento;
                DCotizacionMoneda = Convert.ToDouble(oConsultaComprobante.comprobante.cotizacionMoneda);
                SDomicilioReceptor = oConsultaComprobante.comprobante.domicilioReceptor;
                FechaEmision = oConsultaComprobante.comprobante.fechaEmision.ToShortDateString();
                FechaVencimientoCAE = oConsultaComprobante.comprobante.fechaVencimiento.ToShortDateString();
                SIdImpositivo = oConsultaComprobante.comprobante.idImpositivo;
                DImporteExento = Convert.ToDouble(oConsultaComprobante.comprobante.importeExento);
                DImporteGravado = Convert.ToDouble(oConsultaComprobante.comprobante.importeGravado);
                DImporteNoGravado = Convert.ToDouble(oConsultaComprobante.comprobante.importeNoGravado);
                DImporteOtrosTributos = Convert.ToDouble(oConsultaComprobante.comprobante.importeOtrosTributos);
                DImporteReintegro = Convert.ToDouble(oConsultaComprobante.comprobante.importeReintegro);
                DImporteTotal = Convert.ToDouble(oConsultaComprobante.comprobante.importeTotal);
                SNumeroComprobante = Convert.ToString(oConsultaComprobante.comprobante.numeroComprobante);
                SNumeroDocumento = oConsultaComprobante.comprobante.numeroDocumento;
                INumeroPuntoVenta = oConsultaComprobante.comprobante.numeroPuntoVenta;
                SObs = oConsultaComprobante.comprobante.observaciones;
                return true;
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Consulta el último comprobante autorizado.
        /// </summary>
        /// <param name="argiCbteTipo">Codigo de Tipo de comprobante ejemplo 195</param>
        /// <param name="argiPtoVta">Número del punto de venta</param>
        /// <returns>Verdadero si se pudo consultar</returns>
        public bool CSWsAfipFETUltimoCompConsultar(short argiCbteTipo, short argiPtoVta)
        {
            ConsultarUltimoComprobanteAutorizadoReturnType oConsultarUltimoComprobante = new ConsultarUltimoComprobanteAutorizadoReturnType();

            try
            {
                oConsultarUltimoComprobante = oCTService.consultarUltimoComprobanteAutorizado(oCSwsAfipAuthRequest, argiCbteTipo, argiPtoVta);
                ArrayError = oConsultarUltimoComprobante.arrayErrores;
                ArrayErrorFormato = oConsultarUltimoComprobante.arrayErroresFormato;
                FechaEmision = oConsultarUltimoComprobante.fechaEmision.ToShortDateString();
                SNumeroComprobante = Convert.ToString(oConsultarUltimoComprobante.numeroComprobante);
                return true;
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        [ComVisible(true)]
        public bool IsValidAuthRequest()
        {
            return (tAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10));
        }

        [ComVisible(false)]
        private bool CSwsAfipCheck()
        {
            if (!bIsConnect)
            {
                throw new Exception("No esta conectado al web service de negocio.");
            }
            if (!IsValidAuthRequest())
            {
                throw new Exception("El Ticket de Requerimiento de Acceso ha expirado.");
            }

            return true;
        }


        /// <summary>
        /// Inicializa las variables y los Arrays.
        /// </summary>
        /// <param name="argTotalItems">Total de ítems que se agregarán a la factura T.</param>
        /// <param name="argTotalDocAsociados">Total de documentos asociados que se agregarán a la factura T.</param>
        /// <param name="argTotalDatosAdicionales">Total de datos adicionales que se agregarán a la factura T.</param>
        /// <param name="argTotalOtrosTributos">Total de otros tributos que se agregarán a la factura T.</param>
        /// <param name="argTotalSubtotalesIVA">Total de alícuotas de IVA que se agregarán a la factura T.</param>
        /// <param name="argTotalFormasPago">Total de formas de pago que se agregarán a la factura T.</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFETInit(int argTotalItems, int argTotalDocAsociados, int argTotalDatosAdicionales,
                                    int argTotalOtrosTributos,
                                    int argTotalSubtotalesIVA,
                                    int argTotalFormasPago)
        {
            if (!CSwsAfipCheck()) { return false; }

            bool bReturn = false;

            try
            {
                if (argTotalItems >= 0)
                {
                    ArrayItems = new ItemType[argTotalItems];
                    bReturn = bReturn || true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalDocAsociados > 0)
                {
                    ArrayComprobantesAsociados = new ComprobanteAsociadoType[argTotalDocAsociados];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalDatosAdicionales > 0)
                {
                    ArrayDatosAdicionales = new TipoDatoAdicionalType[argTotalDatosAdicionales];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalOtrosTributos > 0)
                {
                    ArrayOtrosTributos = new OtroTributoType[argTotalOtrosTributos];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalSubtotalesIVA > 0)
                {
                    ArraySubtotalesIVA = new SubtotalIVAType[argTotalSubtotalesIVA];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                if (argTotalFormasPago > 0)
                {
                    ArrayFormasPago = new FormaPagoType[argTotalFormasPago];
                    bReturn = bReturn && true;
                }
                else
                {
                    bReturn = false;
                }

                SCae = "";
                SResultado = "";
                SCodigoMoneda = "";
                ICodigoPais = 0;
                ICodigoRelacionEmisorReceptor = 0;
                SCodigoTipoAutorizacion = "";
                ICodigoTipoComprobante = 0;
                ICodigoTipoDocumento = 0;
                DCotizacionMoneda = 0;
                SDomicilioReceptor = "";
                FechaEmision = "";
                FechaVencimientoCAE = "";
                SIdImpositivo = "";
                DImporteExento = 0;
                DImporteGravado = 0;
                DImporteNoGravado = 0;
                DImporteOtrosTributos = 0;
                DImporteReintegro = 0;
                DImporteTotal = 0;
                SNumeroComprobante = "";
                SNumeroDocumento = "";
                INumeroPuntoVenta = 0;
                SObs = "";
            }

            catch (Exception oEx)
            {
                throw new Exception("No se pudo inicializar las variables. " + oEx.Message);
            }

            return bReturn;
        }

        /// <summary>
        /// Agrega un ítem de producto a la matriz ArrayItems
        /// </summary>
        /// <param name="argTotalFilas">Total de filas de productos a agregar</param>
        /// <param name="argFila">Fila que se esta agregando</param>
        /// <param name="argCodigo">Código de producto personal</param>
        /// <param name="argCodigoAlicIva">Código de alícuota de IVA 5=21%</param>
        /// <param name="argDescrip">Descripción del producto</param>
        /// <param name="argCodTurismo">Código de Turismo: 1=alojamiento sin desayuno, 2=alojamiento con desayuno y 5=Excedente</param>
        /// <param name="argpImporteItem">Importe Neto del ítem</param>
        /// <param name="argImpIva">Importe de IVA.</param>
        /// <param name="argTipo">Tipo de ítem: 0=Item general, 97=Anticipo, 99=Descuento General</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_Item(int argTotalFilas, int argFila, string argCodigo, short argCodigoAlicIva, string argDescrip,
                                         short argCodTurismo, double argpImporteItem,
                                         double argImpIva, short argTipo)
        {
            ar.gob.afip.serviciosjava.ItemType aItem = new ar.gob.afip.serviciosjava.ItemType();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aItem.codigo = argCodigo;
                    aItem.codigoAlicuotaIVA = argCodigoAlicIva;
                    aItem.codigoTurismo = argCodTurismo;
                    aItem.codigoTurismoSpecified = (argCodTurismo > 0);
                    aItem.descripcion = argDescrip;
                    aItem.importeItem = Convert.ToDecimal(argpImporteItem);
                    aItem.importeIVA = Convert.ToDecimal(argImpIva);
                    aItem.tipo = argTipo;
                    ArrayItems[argFila - 1] = aItem;

                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Items es cero o es mayor que el total de items del comprobante.");
                }


            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem " + oEx.Message);
            }

        }

        /// <summary>
        /// Agrega comprobantes asociados al comprobante T.
        /// </summary>
        /// <param name="argTotalFilas">Total de comrobantes que se agregarán.</param>
        /// <param name="argFila">Fila que se está agregando.</param>
        /// <param name="argTipo_cbte">Tipo de comprobante: 196=Nota de Débito T, 197=Nota de Crédito T.</param>
        /// <param name="argPunto_vta">Punto de venta del comprobante.</param>
        /// <param name="argCbte_nro">Número del comprobante.</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_CbteAsoc(int argTotalFilas, int argFila, short argTipo_cbte, short argPunto_vta,
                                             int argCbte_nro)
        {
            ComprobanteAsociadoType aCbteAsoc = new ar.gob.afip.serviciosjava.ComprobanteAsociadoType();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aCbteAsoc.codigoTipoComprobante = argTipo_cbte;
                    aCbteAsoc.numeroComprobante = Convert.ToInt32(argCbte_nro);
                    aCbteAsoc.numeroPuntoVenta = argPunto_vta;
                    ArrayComprobantesAsociados[argFila - 1] = aCbteAsoc;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Comprobantes Asociados es cero o es mayor que el total de ítems indicados.");
                }
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el comprobante asociado. " + oEx.Message);
            }
        }

        /// <summary>
        /// Agrega otros tributos al comprobante, ejemploimpuestos Municipales.
        /// </summary>
        /// <param name="argTotalFilas">Total de tributos que se agregarán.</param>
        /// <param name="argFila">Fila del tributo que se está agregando.</param>
        /// <param name="argCodigo">Código del tributo: 1=Impuestos Nacionales, 2=Impuestos Provinciales, 3=Impuestos Municipales, 4=Impuestos Internos, 99=Otros</param>
        /// <param name="argDescripcion">Descripción del tributo.</param>
        /// <param name="argBaseImponible">Base imponible sobre la que se calcula el impuesto.</param>
        /// <param name="argimporte">Importe del impuesto.</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_OtrosTributos(int argTotalFilas, int argFila, short argCodigo, string argDescripcion, double argBaseImponible, double argimporte)
        {
            OtroTributoType aOtroTributo = new OtroTributoType();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aOtroTributo.codigo = argCodigo;
                    aOtroTributo.descripcion = argDescripcion;
                    aOtroTributo.baseImponible = Convert.ToDecimal(argBaseImponible);
                    aOtroTributo.baseImponibleSpecified = (argBaseImponible != 0);
                    aOtroTributo.importe = Convert.ToDecimal(argimporte);
                    ArrayOtrosTributos[argFila - 1] = aOtroTributo;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Otros Tributos es cero o es mayor que el total de ítems indicados.");
                }
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem de Otros Tributos. " + oEx.Message);
            }
        }

        /// <summary>
        /// Agrega el subtotal de IVA correspondiente a cada alícuota.
        /// </summary>
        /// <param name="argTotalFilas">Total de alícuotas que se agregarán.</param>
        /// <param name="argFila">Fila de la alícuota que se está agregando.</param>
        /// <param name="argCodigo">Código de alícuota de IVA 5=21%.</param>
        /// <param name="argImporte">Importe de IVA.</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_SubTotIva(int argTotalFilas, int argFila, short argCodigo, double argImporte)
        {
            SubtotalIVAType aSubtotalIVA = new SubtotalIVAType();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aSubtotalIVA.codigo = argCodigo;
                    aSubtotalIVA.importe = Convert.ToDecimal(argImporte);
                    ArraySubtotalesIVA[argFila - 1] = aSubtotalIVA;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Otros Tributos es cero o es mayor que el total de ítems indicados.");
                }

            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem de Sub Total de IVA. " + oEx.Message);
            }
        }

        /// <summary>
        /// Datos adicionales al comprobante
        /// </summary>
        /// <param name="argTotalFilas">Total de filas de datos adicionales que se agregarán.</param>
        /// <param name="argFila">Fila que se está agregando.</param>
        /// <param name="argTipoId">0=NO HABILITADO - RESERVADO PARA USO FUTURO</param>
        /// <param name="argC1">Dato adicional 1</param>
        /// <param name="argC2">Dato adicional 2</param>
        /// <param name="argC3">Dato adicional 3</param>
        /// <param name="argC4">Dato adicional 4</param>
        /// <param name="argC5">Dato adicional 5</param>
        /// <param name="argC6">Dato adicional 6</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_DatoAdicional(int argTotalFilas, int argFila, short argTipoId, string argC1, string argC2, string argC3,
                                                  string argC4, string argC5, string argC6)
        {
            TipoDatoAdicionalType aTipoDatoAdicional = new TipoDatoAdicionalType();
            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aTipoDatoAdicional.t = argTipoId;
                    aTipoDatoAdicional.c1 = argC1;
                    aTipoDatoAdicional.c2 = argC2;
                    aTipoDatoAdicional.c3 = argC3;
                    aTipoDatoAdicional.c4 = argC4;
                    aTipoDatoAdicional.c5 = argC5;
                    aTipoDatoAdicional.c6 = argC6;
                    ArrayDatosAdicionales[argFila - 1] = aTipoDatoAdicional;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Datos Adicionales es cero o es mayor que el total de ítems indicados.");
                }

            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem de Datos Adicionales. " + oEx.Message);
            }
        }

        /// <summary>
        /// Forma de pago que se utilizó para pagar la factura T.
        /// </summary>
        /// <param name="argTotalFilas">Total de filas que tiene la matriz de pagos</param>
        /// <param name="argFila">Número de fila que se esta agregando</param>
        /// <param name="argCodigo">Código de pago 68=Tarjeta de Credito, 69=Tarjeta de Debito, 9=Transferencia bancaria.</param>
        /// <param name="argSwiftCode">Código del Banco Swift, ejemplo "ANFEBRRJ"</param>
        /// <param name="argTipoCuenta">Tipo de la cuenta bancaria 1=Caja de ahorro,2=Cuenta corriente, 3=Cuenta única</param>
        /// <param name="argNumeroCuenta">Número de cuanta bancaria ejemplo: 99999</param>
        /// <param name="argNumeroTarjeta">Número de tarjeta de crédito ejemplo: "0", "999999"</param>
        /// <param name="argTipoTarjeta">Código de tipo de tarjeta, varia según sea Debito o Crédito, 99=Otras</param>
        /// <param name="argImporte">Importe total de la factura.</param>
        /// <returns>true si todo salió bien</returns>
        public bool CSwsAfipFET_Add_FormaPago(int argTotalFilas, int argFila, short argCodigo, string argSwiftCode, short argTipoCuenta,
                                              int argNumeroCuenta, string argNumeroTarjeta, short argTipoTarjeta, double argImporte)
        {
            FormaPagoType aFormaPago = new FormaPagoType();

            try
            {
                if (argFila > 0 && argTotalFilas >= argFila)
                {
                    aFormaPago.codigo = argCodigo;
                    if (argSwiftCode != "")
                    {
                        aFormaPago.swiftCode = argSwiftCode;
                    }
                    aFormaPago.tipoCuenta = argTipoCuenta;
                    aFormaPago.tipoCuentaSpecified = (argTipoCuenta != 0);
                    aFormaPago.numeroCuenta = argNumeroCuenta;
                    aFormaPago.numeroCuentaSpecified = (argNumeroCuenta != 0);
                    aFormaPago.numeroTarjeta = Convert.ToInt64(argNumeroTarjeta);
                    aFormaPago.numeroTarjetaSpecified = (Convert.ToInt64(argNumeroTarjeta) != 0);
                    // aFormaPago. IMPORTE???
                    aFormaPago.tipoTarjeta = argTipoTarjeta;
                    aFormaPago.tipoTarjetaSpecified = (argTipoTarjeta != 0);

                    ArrayFormasPago[argFila - 1] = aFormaPago;
                    return true;
                }
                else
                {
                    throw new Exception("El índice de la matriz de Forma de Pago es cero o es mayor que el total de ítems indicados.");
                }

            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo agregar el ítem de Forma de Pago. " + oEx.Message);
            }
        }

        /// <summary>
        /// Setea los datos de la cabecera del comprobante T
        /// </summary>
        /// <param name="argCodAutorizacion"></param>
        /// <param name="argCodMoneda">Código de moneda, ej.: "PES", "DOL"</param>
        /// <param name="argCodigoPais">Código de pís, ej: 200=ARGENTINA, 203=BRASIL </param>
        /// <param name="argCodRelacionEmisorReceptor">Código de relación Emisor y receptor: 1=Alojamiento Directo a Turista No Residente, 2=Alojamiento a Agencia de Viaje Residente, 3=Alojamiento a Agencia de Viaje No Residente,4=Agencia de Viaje Residente a Agencia de Viaje No Residente,5=Agencia de Viaje Residente a Turista No Residente, 6=Agencia de Viaje Residente a Agencia de Viaje Residente</param>
        /// <param name="argCodTipoAutorizacion">Tipo de autorización "A" o "E" (E = CAE)</param>
        /// <param name="argCodTipoComprobante">Tipo de comprobante, 195=Factura T, 196=Nota de Débito T, 197=Nota de Crédito T</param>
        /// <param name="argCodTipoDocumento">Tipo de documento, ej.:80=CUIT</param>
        /// <param name="argCotizacionMoneda">Importe de la cotizción de la moneda.</param>
        /// <param name="argDomicilioReceptor">Domicilio del receptor de la factura T</param>
        /// <param name="argFechaEmision">Formato "AAAAMMDD"</param>
        /// <param name="argFechaVencimiento">Formato "AAAAMMDD"</param>
        /// <param name="argIdImpositivo">ID impositivo ej.: "9"=Cliente del exterior</param>
        /// <param name="argImporteExento">Importe exento en IVA</param>
        /// <param name="argImporteGravado">Importe Gravado en IVA</param>
        /// <param name="argImporteNoGravado">Importe No Gravado en IVA</param>
        /// <param name="argImporteOtrosTributos">Importe total de otros tributos</param>
        /// <param name="argImporteReintegro">Importe de reintegro, usualmente el valor de IVA negativo (debe ser menor que 0)</param>
        /// <param name="argImporteTotal">Importe total (ya tiene que estar descontad el descuento)</param>
        /// <param name="argNumeroComprobante">Número del comprobante que se está emitiendo (Ultimo + 1)</param>
        /// <param name="argNumeroDocumento">Número del documento del comprador (Pasaporte, CUIT, etc)</param>
        /// <param name="argNumeroPuntoVenta">Número del punto de venta del comprobante T</param>
        /// <param name="argObservaciones">Texto para indicar cualquier observación que sea necesaria</param>
        /// <returns>true si todo esta bien , false si no</returns>
        public bool CSwsAfipFETComprobante(int argCodAutorizacion,
                                            string argCodMoneda,
                                            short argCodigoPais,
                                            short argCodRelacionEmisorReceptor,
                                            string argCodTipoAutorizacion,
                                            short argCodTipoComprobante,
                                            short argCodTipoDocumento,
                                            double argCotizacionMoneda,
                                            string argDomicilioReceptor,
                                            string argFechaEmision,
                                            string argFechaVencimiento,
                                            string argIdImpositivo,
                                            double argImporteExento,
                                            double argImporteGravado,
                                            double argImporteNoGravado,
                                            double argImporteOtrosTributos,
                                            double argImporteReintegro,
                                            double argImporteTotal,
                                            int argNumeroComprobante,
                                            string argNumeroDocumento,
                                            short argNumeroPuntoVenta,
                                            string argObservaciones)
        {

            oComprobanteRequest = new ComprobanteType();

            try
            {
                if (ArrayComprobantesAsociados != null && ArrayComprobantesAsociados.Length > 0)
                {
                    oComprobanteRequest.arrayComprobantesAsociados = ArrayComprobantesAsociados;
                }

                if (ArrayDatosAdicionales != null && ArrayDatosAdicionales.Length > 0)
                {
                    oComprobanteRequest.arrayDatosAdicionales = ArrayDatosAdicionales;
                }

                if (ArrayFormasPago != null && ArrayFormasPago.Length > 0)
                {
                    oComprobanteRequest.arrayFormasPago = ArrayFormasPago;
                }

                if (ArrayItems != null && ArrayItems.Length > 0 && ArrayItems[0] != null)
                {
                    oComprobanteRequest.arrayItems = ArrayItems;
                }
                else
                {
                    throw new Exception("No hay ítems de producctos definidos.");
                }

                if (ArrayOtrosTributos != null && ArrayOtrosTributos.Length > 0)
                {
                    oComprobanteRequest.arrayOtrosTributos = ArrayOtrosTributos;
                }
                if (ArraySubtotalesIVA != null && ArraySubtotalesIVA.Length > 0)
                {
                    oComprobanteRequest.arraySubtotalesIVA = ArraySubtotalesIVA;
                }

                oComprobanteRequest.codigoAutorizacion = Convert.ToInt64(argCodAutorizacion);
                oComprobanteRequest.codigoAutorizacionSpecified = (argCodAutorizacion > 0);

                oComprobanteRequest.codigoMoneda = argCodMoneda;

                oComprobanteRequest.codigoPais = argCodigoPais;
                oComprobanteRequest.codigoPaisSpecified = (argCodigoPais > 0);

                oComprobanteRequest.codigoRelacionEmisorReceptor = argCodRelacionEmisorReceptor;

                if (argCodTipoAutorizacion == "A")
                {
                    oComprobanteRequest.codigoTipoAutorizacion = CodigoTipoAutorizacionSimpleType.A;
                }
                else
                {
                    oComprobanteRequest.codigoTipoAutorizacion = CodigoTipoAutorizacionSimpleType.E;
                }

                oComprobanteRequest.codigoTipoAutorizacionSpecified = (argCodTipoAutorizacion != "");

                oComprobanteRequest.codigoTipoComprobante = argCodTipoComprobante;
                oComprobanteRequest.codigoTipoDocumento = argCodTipoDocumento;
                oComprobanteRequest.cotizacionMoneda = Convert.ToDecimal(argCotizacionMoneda);
                oComprobanteRequest.domicilioReceptor = argDomicilioReceptor;


                oComprobanteRequest.fechaEmision = DateTime.ParseExact(argFechaEmision, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.CurrentInfo);
                oComprobanteRequest.fechaEmisionSpecified = (argFechaEmision != "");

                if (argFechaVencimiento != "")
                {
                    oComprobanteRequest.fechaVencimiento = DateTime.ParseExact(argFechaVencimiento, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.CurrentInfo);
                    oComprobanteRequest.fechaVencimientoSpecified = true;
                }

                oComprobanteRequest.idImpositivo = argIdImpositivo;

                oComprobanteRequest.importeExento = Convert.ToDecimal(argImporteExento);
                oComprobanteRequest.importeExentoSpecified = (argImporteExento != 0);

                oComprobanteRequest.importeGravado = Convert.ToDecimal(argImporteGravado);
                oComprobanteRequest.importeGravadoSpecified = (argImporteGravado != 0);

                oComprobanteRequest.importeNoGravado = Convert.ToDecimal(argImporteNoGravado);
                oComprobanteRequest.importeNoGravadoSpecified = (argImporteNoGravado != 0);

                oComprobanteRequest.importeOtrosTributos = Convert.ToDecimal(argImporteOtrosTributos);
                oComprobanteRequest.importeOtrosTributosSpecified = (argImporteOtrosTributos != 0);

                oComprobanteRequest.importeReintegro = Convert.ToDecimal(argImporteReintegro);
                oComprobanteRequest.importeReintegroSpecified = (argImporteReintegro != 0);

                oComprobanteRequest.importeTotal = Convert.ToDecimal(argImporteTotal);
                oComprobanteRequest.numeroComprobante = Convert.ToInt64(argNumeroComprobante);
                oComprobanteRequest.numeroDocumento = argNumeroDocumento;
                oComprobanteRequest.numeroPuntoVenta = argNumeroPuntoVenta;
                oComprobanteRequest.observaciones = argObservaciones;

                return true;
            }
            catch (Exception oEx)
            {
                throw new Exception("No se pudo crear el comprobante T. " + oEx.Message);
            }
        }

        /// <summary>
        /// Solicita el CAE del comprobante que fue definido en CSwsAfipFETComprobante
        /// </summary>
        /// <returns>true si todo salio bien</returns>
        public bool CSwsAfipFETCAESolicitar()
        {

            AutorizarComprobanteReturnType oAutorizarComprobanteReturnType = new AutorizarComprobanteReturnType();
            string strMsg = "";

            try
            {
                oAutorizarComprobanteReturnType = oCTService.autorizarComprobante(oCSwsAfipAuthRequest, oComprobanteRequest);
                ArrayError = oAutorizarComprobanteReturnType.arrayErrores;
                ArrayErrorFormato = oAutorizarComprobanteReturnType.arrayErroresFormato;
                ArrayObs = oAutorizarComprobanteReturnType.arrayObservaciones;

                SResultado = oAutorizarComprobanteReturnType.resultado.ToString();

                if (SResultado == "O")
                {
                    if (ArrayObs != null && ArrayObs.Length > 0)
                    {
                        SObs = "Observaciones en la Autorización del comprobante:" + "\n";

                        for (int i = 1; i < ArrayObs.Length; i++)
                        {
                            SObs = SObs + "Código: " + ArrayObs[i].codigo + ArrayObs[i].descripcion + "\n";
                        }
                    }
                }

                if (SResultado == "R")
                {
                    if (ArrayError != null && ArrayError.Length > 0)
                    {
                        strMsg = "Exepción al solicitar la autorización del comprobante T." + "\n";

                        for (int i = 0; i < ArrayError.Length; i++)
                        {
                            strMsg = strMsg + " Código: " + ArrayError[i].codigo.ToString() + " " + ArrayError[i].descripcion + "\n";
                        }

                        throw new Exception(strMsg);
                    }

                    if (ArrayErrorFormato != null && ArrayErrorFormato.Length > 0)
                    {
                        strMsg = "Error en formato del comprobante:" + "\n";

                        for (int i = 0; i < ArrayErrorFormato.Length; i++)
                        {
                            strMsg = strMsg + "Código: " + ArrayErrorFormato[i].codigo + " " + ArrayErrorFormato[i].descripcion + "\n";
                        }

                        throw new Exception(strMsg);
                    }
                }

                SCae = Convert.ToString(oAutorizarComprobanteReturnType.comprobanteResponse.CAE);
                FechaVencimientoCAE = oAutorizarComprobanteReturnType.comprobanteResponse.fechaVencimientoCAE.ToShortDateString();
                FechaEmision = oAutorizarComprobanteReturnType.comprobanteResponse.fechaEmision.ToShortDateString();

                return true;
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene la lista de los Puntos de Ventas habilitados para el contribuyente.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de puntos de ventas.</returns>
        public string CSwsAfipFETConsultarPuntosVenta()
        {
            ConsultarPuntosVentaReturnType oPuntoVentas = new ConsultarPuntosVentaReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oPuntoVentas = oCTService.consultarPuntosVenta(oCSwsAfipAuthRequest);
                ArrayError = oPuntoVentas.arrayErrores;
                ArrayErrorFormato = oPuntoVentas.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de punto de ventas."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oPuntoVentas.arrayPuntosVenta.GetType());
                olRespuesta.Serialize(strReturn, oPuntoVentas.arrayPuntosVenta);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene la lista de códigos de ítems para Turismo
        /// </summary>
        /// <returns>Devuelve un XML con la lista de códigos.</returns>
        public string CSwsAfipFETConsultarCodigosItemTurismo()
        {
            ConsultarCodigosItemTurismoReturnType oCodItemT = new ConsultarCodigosItemTurismoReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oCodItemT = oCTService.consultarCodigosItemTurismo(oCSwsAfipAuthRequest);
                ArrayError = oCodItemT.arrayErrores;
                ArrayErrorFormato = oCodItemT.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Códigos de ítems de Turismo."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oCodItemT.arrayCodigosItem.GetType());
                olRespuesta.Serialize(strReturn, oCodItemT.arrayCodigosItem);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Obtiene la lista de Tipos de comprobantes habilitados para factura T.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de tipos de comprobantes.</returns>
        public string CSwsAfipFETConsultarTiposComprobantes()
        {
            ConsultarTiposComprobantesReturnType oTipoComprob = new ConsultarTiposComprobantesReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTipoComprob = oCTService.consultarTiposComprobantes(oCSwsAfipAuthRequest);
                ArrayError = oTipoComprob.arrayErrores;
                ArrayErrorFormato = oTipoComprob.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Comprobantes habilitados para Turismo."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTipoComprob.arrayTiposComprobantes.GetType());
                olRespuesta.Serialize(strReturn, oTipoComprob.arrayTiposComprobantes);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar los tipos de documentos de receptores de comprobantes habilitados a ser informados en el presente ws.
        /// </summary>
        /// <returns>Deuelve un XML con la lista de documentos.</returns>
        public string CSwsAfipFETConsultarTiposDocumento()
        {
            ConsultarTiposDocumentoReturnType oTipoDoc = new ConsultarTiposDocumentoReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTipoDoc = oCTService.consultarTiposDocumento(oCSwsAfipAuthRequest);
                ArrayError = oTipoDoc.arrayErrores;
                ArrayErrorFormato = oTipoDoc.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de Documentos de receptores."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTipoDoc.arrayTiposDocumento.GetType());
                olRespuesta.Serialize(strReturn, oTipoDoc.arrayTiposDocumento);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }


        /// <summary>
        /// Este método obtiene la lista de los Tipos de códigos de ítems.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de códigos.</returns>
        public string CSwsAfipFETConsultarTiposItem()
        {
            ConsultarTiposItemReturnType oTipoItem = new ConsultarTiposItemReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTipoItem = oCTService.consultarTiposItem(oCSwsAfipAuthRequest);
                ArrayError = oTipoItem.arrayErrores;
                ArrayErrorFormato = oTipoItem.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de Códigos de ítems para Turismo."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTipoItem.arrayTiposItem.GetType());
                olRespuesta.Serialize(strReturn, oTipoItem.arrayTiposItem);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar los tipos de IVA habilitados a informar al momento de identificar el detalle del comprobante.
        /// </summary>
        /// <returns>Delvuelve un XML con la lista de tipos de IVA.</returns>
        public string CSwsAfipFETConsultarTiposIVA()
        {
            ConsultarTiposIVAReturnType oTipoIva = new ConsultarTiposIVAReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTipoIva = oCTService.consultarTiposIVA(oCSwsAfipAuthRequest);
                ArrayError = oTipoIva.arrayErrores;
                ArrayErrorFormato = oTipoIva.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de IVA habilitados para Turismo."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTipoIva.arrayTiposIVA.GetType());
                olRespuesta.Serialize(strReturn, oTipoIva.arrayTiposIVA);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar los tipos de tributos habilitados a informar al momento de identificar el detalle del comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con los tipos de tributo.</returns>
        public string CSwsAfipFETConsultarTiposTributo()
        {
            ConsultarTiposTributoReturnType oTipoTributo = new ConsultarTiposTributoReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTipoTributo = oCTService.consultarTiposTributo(oCSwsAfipAuthRequest);
                ArrayError = oTipoTributo.arrayErrores;
                ArrayErrorFormato = oTipoTributo.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de Tributos habilitados para Turismo."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTipoTributo.arrayTiposTributo.GetType());
                olRespuesta.Serialize(strReturn, oTipoTributo.arrayTiposTributo);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar diferentes relaciones a informar entre el emisor y el receptor del comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de relaciones.</returns>
        public string CSwsAfipFETConsultarRelacionEmisorReceptor()
        {
            ConsultarRelacionEmisorReceptorReturnType oRelaciones = new ConsultarRelacionEmisorReceptorReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oRelaciones = oCTService.consultarRelacionEmisorReceptor(oCSwsAfipAuthRequest);
                ArrayError = oRelaciones.arrayErrores;
                ArrayErrorFormato = oRelaciones.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de las relaciones entre emisor y receptor."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oRelaciones.arrayRelacionesEmisorReceptor.GetType());
                olRespuesta.Serialize(strReturn, oRelaciones.arrayRelacionesEmisorReceptor);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar diferentes países disponibles a informar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de países.</returns>
        public string CSwsAfipFETConsultarPaises()
        {
            ConsultarPaisesReturnType oPaises = new ConsultarPaisesReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oPaises = oCTService.consultarPaises(oCSwsAfipAuthRequest);
                ArrayError = oPaises.arrayErrores;
                ArrayErrorFormato = oPaises.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de países."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oPaises.arrayPaises.GetType());
                olRespuesta.Serialize(strReturn, oPaises.arrayPaises);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar las diferentes monedas disponibles a informar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de monedas.</returns>
        public string CSwsAfipFETConsultarMonedas()
        {
            ConsultarMonedasReturnType oMonedas = new ConsultarMonedasReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oMonedas = oCTService.consultarMonedas(oCSwsAfipAuthRequest);
                ArrayError = oMonedas.arrayErrores;
                ArrayErrorFormato = oMonedas.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Monedas."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oMonedas.arrayTiposMoneda.GetType());
                olRespuesta.Serialize(strReturn, oMonedas.arrayTiposMoneda);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar las diferentes formas de pago disponibles a informar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con las formas de pago.</returns>
        public string CSwsAfipFETConsultarFormasPago()
        {
            ConsultarFormasPagoReturnType oFormasPago = new ConsultarFormasPagoReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oFormasPago = oCTService.consultarFormasPago(oCSwsAfipAuthRequest);
                ArrayError = oFormasPago.arrayErrores;
                ArrayErrorFormato = oFormasPago.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Formas de Pago."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oFormasPago.arrayFormasPago.GetType());
                olRespuesta.Serialize(strReturn, oFormasPago.arrayFormasPago);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar las diferentes condiciones de IVA disponibles a informar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de las condiciones de IVA.</returns>
        public string CSwsAfipFETConsultarCondicionesIVA()
        {
            ConsultarCondicionesIVAReturnType oCondicionIva = new ConsultarCondicionesIVAReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oCondicionIva = oCTService.consultarCondicionesIVA(oCSwsAfipAuthRequest);
                ArrayError = oCondicionIva.arrayErrores;
                ArrayErrorFormato = oCondicionIva.arrayErroresFormato;


                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de las Condiciones de IVA."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oCondicionIva.arrayCondicionesIVA.GetType());
                olRespuesta.Serialize(strReturn, oCondicionIva.arrayCondicionesIVA);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Procesa el array de error que devuelve el WS
        /// </summary>
        /// <param name="argMsg"></param>
        /// <returns>Devuelve un sring con el error.</returns>
        private string ProcesarArrayError(string argMsg)
        {
            string strMsg = "";

            try
            {
                if (ArrayError != null && ArrayError.Length > 0)
                {
                    strMsg = argMsg + "\n";

                    for (int i = 0; i < ArrayError.Length; i++)
                    {
                        strMsg = strMsg + " Código: " + ArrayError[i].codigo.ToString() + " " + ArrayError[i].descripcion + "\n";
                    }

                    return strMsg;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }


        /// <summary>
        /// Mediante este método se podrá consultar los diferentes CUIT de países habilitados a informar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de CUITs.</returns>
        public string CSwsAfipFETConsultarCUITsPaises()
        {
            ConsultarCuitPaisesReturnType oCuitPaises = new ConsultarCuitPaisesReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oCuitPaises = oCTService.consultarCUITsPaises(oCSwsAfipAuthRequest);
                ArrayError = oCuitPaises.arrayErrores;
                ArrayErrorFormato = oCuitPaises.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de las CUITs de Países."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oCuitPaises.arrayCuitPaises.GetType());
                olRespuesta.Serialize(strReturn, oCuitPaises.arrayCuitPaises);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar la cotización de la moneda al momento de invocar al método público.
        /// </summary>
        /// <param name="argCodMoneda">String en dónde se indica el código de moneda, por ejempl Dolares="DOL"</param>
        /// <returns>Devuelve el importe de la cotización.</returns>
        public double CSwsAfipFETConsultarCotizacion(string argCodMoneda)
        {
            ConsultarCotizacionReturnType oCotizacion = new ConsultarCotizacionReturnType();

            try
            {
                oCotizacion = oCTService.consultarCotizacion(oCSwsAfipAuthRequest, argCodMoneda);
                ArrayError = oCotizacion.arrayErrores;
                ArrayErrorFormato = oCotizacion.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Cotización de Moneda."));
                }

                return Convert.ToDouble(oCotizacion.cotizacionMoneda);
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar todas las notificaciones referentes al servicio en cuestión.
        /// </summary>
        /// <returns>Devuelve un XML con las novedades.</returns>
        public string CSwsAfipFETConsultarNovedades()
        {
            ConsultarNovedadesReturnType oNovedades = new ConsultarNovedadesReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oNovedades = oCTService.consultarNovedades(oCSwsAfipAuthRequest);
                ArrayError = oNovedades.arrayErrores;
                ArrayErrorFormato = oNovedades.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de las Novedades del WS de Factura T."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oNovedades.arrayNovedades.GetType());
                olRespuesta.Serialize(strReturn, oNovedades.arrayNovedades);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar las diferentes tipos de tarjetas a utilizar al momento de autorizar un comprobante dependiendo de su forma de pago.
        /// </summary>
        /// <param name="argFormPago">Forma de pagos ejemplo: 1=débito, 2=crédito</param>
        /// <returns>Devuelve un XML con la lista de tipos de tarjeta.</returns>
        public string CSwsAfipFETConsultarTiposTarjeta(short argFormPago)
        {
            ConsultarTiposTarjetaReturnType oTarjetas = new ConsultarTiposTarjetaReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oTarjetas = oCTService.consultarTiposTarjeta(oCSwsAfipAuthRequest, argFormPago);
                ArrayError = oTarjetas.arrayErrores;
                ArrayErrorFormato = oTarjetas.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de tarjeta."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oTarjetas.arrayTiposTarjeta.GetType());
                olRespuesta.Serialize(strReturn, oTarjetas.arrayTiposTarjeta);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar las diferentes tipos de cuenta a utilizar al momento de autorizar el comprobante.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de los tipos de cuenta.</returns>
        public string CSwsAfipFETConsultarTiposCuenta()
        {
            ConsultarTiposCuentaReturnType oCuentas = new ConsultarTiposCuentaReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oCuentas = oCTService.consultarTiposCuenta(oCSwsAfipAuthRequest);
                ArrayError = oCuentas.arrayErrores;
                ArrayErrorFormato = oCuentas.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de cuenta."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oCuentas.arrayTiposCuenta.GetType());
                olRespuesta.Serialize(strReturn, oCuentas.arrayTiposCuenta);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }

        /// <summary>
        /// Mediante este método se podrá consultar todos los datos adicionales a informar sobre un comprobante según RG.
        /// </summary>
        /// <returns>Devuelve un XML con la lista de datos adicionales.</returns>
        public string CSwsAfipFETConsultarTiposDatosAdicionales()
        {
            ConsultarTiposDatosAdicionalesReturnType oDatosAdicionales = new ConsultarTiposDatosAdicionalesReturnType();
            StringWriter strReturn = new StringWriter();

            try
            {
                oDatosAdicionales = oCTService.consultarTiposDatosAdicionales(oCSwsAfipAuthRequest);
                ArrayError = oDatosAdicionales.arrayErrores;
                ArrayErrorFormato = oDatosAdicionales.arrayErroresFormato;

                if (ArrayError != null && ArrayError.Length > 0)
                {
                    throw new Exception(ProcesarArrayError("Exepción al solicitar la consulta de Tipos de Datos Adicionales."));
                }

                XmlSerializer olRespuesta = new XmlSerializer(oDatosAdicionales.arrayTiposDatosAdicionales.GetType());
                olRespuesta.Serialize(strReturn, oDatosAdicionales.arrayTiposDatosAdicionales);
                return strReturn.ToString();
            }
            catch (Exception oEx)
            {
                throw oEx;
            }
        }
        #endregion

    }

    public class VEConsumer
    {
        #region Fields

        private string urlWSAA = "https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL";

        private string strPassword;

        private string strSign = "";

        private string strWebServiceName = "veconsumerws";

        private string strCertificateFileName;

        private byte[] dataCertificate;

        private bool bIsConnect;

        private bool bIsHomologacion;

        private string strToken = string.Empty;

        private DateTime tAuthRequestExpiration = DateTime.UtcNow.AddMinutes(-10);

        #endregion

        #region Properties

        public bool BIsHomologacion
        {
            get { return bIsHomologacion; }
            set { bIsHomologacion = value; }
        }

        public string StrToken
        {
            get { return strToken; }
            set { strToken = value; }
        }

        public string StrCertificateFileName
        {
            get { return strCertificateFileName; }
            set { strCertificateFileName = value; }
        }

        public byte[] DataCertificate
        {
            get { return dataCertificate; }
            set { dataCertificate = value; }
        }

        public bool BIsConnect
        {
            get
            {
                return bIsConnect;
            }
            set
            {
                bIsConnect = value;
            }
        }

        public string StrPassword
        {
            get { return strPassword; }
            set { strPassword = value; }
        }

        public DateTime TAuthRequestExpiration
        {
            get { return tAuthRequestExpiration; }
            set { tAuthRequestExpiration = value; }
        }

        public string StrSign
        {
            get { return strSign; }
            set { strSign = value; }
        }

        #endregion

        public RespuestaPaginada consultarComunicaciones(bool argbIsHomologacion, string argStrCertificateFileName, string argStrPassword, string argStrCuit)
        {

            strCertificateFileName = argStrCertificateFileName;
            strPassword = argStrPassword;
            var response = new RespuestaPaginada();
            try
            {
                if (!CSwsAfipChecktTAPersona())
                {
                    BIsConnect = CSwsAfipAAGetTRA();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Convertir string a long
            long cuit;
            long.TryParse(argStrCuit, out cuit);

            var authRequest = new AuthRequest
            {
                cuitRepresentada = cuit,
                sign = this.StrSign,
                token = this.StrToken
            };
            var filter = new Filter
            {
                fechaDesde = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd")
            };

            var veConsumer = new VEConsumerClient();
            try
            {
                return veConsumer.consultarComunicaciones(authRequest, filter);

                // TEST PURPOSE
                // var numCom = 1;
                //foreach (var comunication in response.items)
                //{
                //    throw new Exception("Mensaje :" + numCom);
                //    numCom++;
                //    throw new Exception("Asunto: " + comunication.asunto);
                //    throw new Exception("Estado: " + comunication.estadoDesc);
                //    throw new Exception("Fecha: " + comunication.fechaPublicacion);
                //    var comunicationObj = veConsumer.consumirComunicacion(authRequest, comunication.idComunicacion, false);
                //    throw new Exception("Mensaje: " + comunicationObj.mensaje);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void consultarComunicaciones(bool argbIsHomologacion, byte[] argdataCertificate, string argStrPassword, string argStrCuit)
        {

            dataCertificate = argdataCertificate;
            strPassword = argStrPassword;
            var response = new RespuestaPaginada();
            try
            {
                if (!CSwsAfipChecktTAPersona())
                {
                    BIsConnect = CSwsAfipAAGetTRA();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Convertir string a long
            long cuit;
            long.TryParse(argStrCuit, out cuit);

            var authRequest = new AuthRequest
            {
                cuitRepresentada = cuit,
                sign = this.StrSign,
                token = this.StrToken
            };
            var filter = new Filter
            {
                fechaDesde = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")
            };

            var encoding = new MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement())
            {
                MessageVersion = MessageVersion.Soap12
            };
            var transport = new HttpsTransportBindingElement();
            transport.AllowCookies = true;
            var customBinding = new CustomBinding(encoding, transport)
            {
                Name = "BindingAuthBasicMtom",
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
            };

            EndpointAddress remoteAddress = new EndpointAddress(new Uri("https://infraestructura.afip.gob.ar/ve-ws/services/veconsumer/"));
            var veConsumer = new VEConsumerClient(customBinding, remoteAddress);

            response = veConsumer.consultarComunicaciones(authRequest, filter);

            foreach (var comunication in response.items)
            {
                //MessageBox.Show(comunication.asunto);
                //MessageBox.Show(comunication.estadoDesc);
                //MessageBox.Show(comunication.fechaPublicacion);
                var comunicationObj = veConsumer.consumirComunicacion(authRequest, comunication.idComunicacion, false);
                //MessageBox.Show(comunicationObj.mensaje);
            }
        }

        private bool CSwsAfipChecktTAPersona()
        {
            bool lbReturn = false;

            if (BIsConnect && StrToken != "" && TAuthRequestExpiration > DateTime.UtcNow.AddMinutes(+10))
            {
                lbReturn = true;
            }

            return lbReturn;
        }

        [ComVisible(false)]
        private bool CSwsAfipAAGetTRA()
        {
            CSwsAfipAA wsAfipAA = new CSwsAfipAA();
            bool bReturn = false;

            wsAfipAA.IsHomologacion = BIsHomologacion;
            wsAfipAA.WebServiceName = strWebServiceName;
            wsAfipAA.URLWebServiceAAProduccion = urlWSAA;

            if (!string.IsNullOrEmpty(strCertificateFileName))
            {
                try
                {
                    if (wsAfipAA.GetTicketAccessRequirement(strCertificateFileName, strPassword))
                    {
                        TAuthRequestExpiration = wsAfipAA.ExpirationDatetime;

                        StrSign = wsAfipAA.Sign;
                        StrToken = wsAfipAA.Token;
                        bReturn = true;
                    }
                    else
                    {
                        StrToken = "";
                    }

                    return bReturn;
                }
                catch (Exception exceptionwsAfipAA)
                {
                    throw new Exception("Exception: " + exceptionwsAfipAA.Message);
                }
            }
            try
            {
                if (wsAfipAA.GetTicketAccessRequirement(dataCertificate, this.strPassword))
                {
                    this.TAuthRequestExpiration = wsAfipAA.ExpirationDatetime;

                    this.StrSign = wsAfipAA.Sign;
                    this.StrToken = wsAfipAA.Token;
                    bReturn = true;
                }
                else
                {
                    this.StrToken = "";
                }

                return bReturn;
            }
            catch (Exception exceptionwsAfipAA)
            {
                throw new Exception("Concha tu madre abuela" + exceptionwsAfipAA.Message);
            }
        }
    }

    //public class VEConsumerObject
    //{
    //    public string 
    //}

}
