using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Xml;
using WSTimbradoFS;
using SianFE;

namespace CFE
{
    public class CFacturaElectronica
    {
        #region Miembros
        private string _strVersion;
        private string _strSerie;
        private int _intFolio;
        private DateTime _dtFecha;
        private int _intNoAprobacion;
        private int _intAnoAprobacion;
        private string _strMetodoDePago;
        private string _strNumCtaPago;
        private string _strCondicionesDePago;
        private decimal _amtSubTotal;
        private decimal _amtDescuento;
        private decimal _amtTotal;
        private string _strRFCEmisor;
        private string _strNombreEmisor;
        private string _strRegimen;
        private CDireccion _dirDomicilioFiscal;
        private CDireccion _dirDomicilioExpedicion;
        private string _strRFCCliente;
        private string _strNombreCliente;
        private string _strCedulaFiscalCliente;
        private string _strCedulaIEPSCliente;
        private CDireccion _dirDomicilioCliente;
        private decimal _amtISR_Retenido;
        private decimal _amtTasa_ISR_Retenido;
        private decimal _amtIVA_Retenido;
        private decimal _amtTasa_IVA_Retenido;
        private decimal _amtTotalImpuestosRetenidos;
        private decimal _amtImpuesto;
        private decimal _amtTasa;
        private decimal _amtImporte;
        private decimal _amtTotalImpuestos;
        private string _strFolioFiscal;
        private string _strCertificadoSAT;
        private DateTime _dtFechaCertificado;
        private string _strSelloCFDI;
        private string _strSelloSAT;
        private string _strVersionCFDI;
        List<CProducto> _lstProductos;
        #endregion

        #region Propiedades
        public string StrVersion
        {
            get { return _strVersion; }
            set { _strVersion = value.Trim(); }
        }

        public string StrSerie
        {
            get { return _strSerie; }
            set { _strSerie = value.Trim(); }
        }

        public int IntFolio
        {
            get { return _intFolio; }
            set { _intFolio = value; }
        }

        public DateTime DtFecha
        {
            get { return _dtFecha; }
            set { _dtFecha = value; }
        }

        public DateTime DtFechaCertificado
        {
            get { return _dtFechaCertificado; }
            set { _dtFechaCertificado = value; }
        }

        public int IntNoAprobacion
        {
            get { return _intNoAprobacion; }
            set { _intNoAprobacion = value; }
        }

        public int IntAnoAprobacion
        {
            get { return _intAnoAprobacion; }
            set { _intAnoAprobacion = value; }
        }

        public string StrMetodoDePago
        {
            get { return _strMetodoDePago; }
            set { _strMetodoDePago = value.Trim(); }
        }

        public string StrNumCtaPago
        {
            get { return _strNumCtaPago; }
            set { _strNumCtaPago = value.Trim(); }
        }

        public string StrCondicionesDePago
        {
            get { return _strCondicionesDePago; }
            set { _strCondicionesDePago = value.Trim(); }
        }

        public decimal AmtSubTotal
        {
            get { return _amtSubTotal; }
            set { _amtSubTotal = value; }
        }

        public decimal AmtDescuento
        {
            get { return _amtDescuento; }
            set { _amtDescuento = value; }
        }

        public decimal AmtTotal
        {
            get { return _amtTotal; }
            set { _amtTotal = value; }
        }

        public string StrRFCEmisor
        {
            get { return _strRFCEmisor; }
            set { _strRFCEmisor = value.Trim(); }
        }

        public string StrNombreEmisor
        {
            get { return _strNombreEmisor; }
            set { _strNombreEmisor = value.Trim(); }
        }

        public string StrRegimen
        {
            get { return _strRegimen; }
            set { _strRegimen = value.Trim(); }
        }

        public CDireccion DirDomicilioFiscal
        {
            get { return _dirDomicilioFiscal; }
            set { _dirDomicilioFiscal = value; }
        }

        public CDireccion DirDomicilioExpedicion
        {
            get { return _dirDomicilioExpedicion; }
            set { _dirDomicilioExpedicion = value; }
        }

        public string StrRFCCliente
        {
            get { return _strRFCCliente; }
            set { _strRFCCliente = value.Trim(); }
        }

        public string StrNombreCliente
        {
            get { return _strNombreCliente; }
            set { _strNombreCliente = value.Trim(); }
        }

        public string StrCedulaFiscalCliente
        {
            get { return _strCedulaFiscalCliente; }
            set { _strCedulaFiscalCliente = value.Trim(); }
        }

        public string StrCedulaIEPSCliente
        {
            get { return _strCedulaIEPSCliente; }
            set { _strCedulaIEPSCliente = value.Trim(); }
        }

        public CDireccion DirDomicilioCliente
        {
            get { return _dirDomicilioCliente; }
            set { _dirDomicilioCliente = value; }
        }

        public decimal AmtISR_Retenido
        {
            get { return _amtISR_Retenido; }
            set { _amtISR_Retenido = value; }
        }

        public decimal AmtTasa_ISR_Retenido
        {
            get { return _amtTasa_ISR_Retenido; }
            set { _amtTasa_ISR_Retenido = value; }
        }

        public decimal AmtIVA_Retenido
        {
            get { return _amtIVA_Retenido; }
            set { _amtIVA_Retenido = value; }
        }

        public decimal AmtTasa_IVA_Retenido
        {
            get { return _amtTasa_IVA_Retenido; }
            set { _amtTasa_IVA_Retenido = value; }
        }

        public decimal AmtTotalImpuestosRetenidos
        {
            get { return _amtTotalImpuestosRetenidos; }
            set { _amtTotalImpuestosRetenidos = value; }
        }

        public decimal AmtImpuesto
        {
            get { return _amtImpuesto; }
            set { _amtImpuesto = value; }
        }

        public decimal AmtTasa
        {
            get { return _amtTasa; }
            set { _amtTasa = value; }
        }

        public decimal AmtImporte
        {
            get { return _amtImporte; }
            set { _amtImporte = value; }
        }

        public decimal AmtTotalImpuestos
        {
            get { return _amtTotalImpuestos; }
            set { _amtTotalImpuestos = value; }
        }

        public List<CProducto> LstProductos
        {
            get { return _lstProductos; }
            set { _lstProductos = value; }
        }

        public string StrFolioFiscal
        {
            get { return _strFolioFiscal; }
            set { _strFolioFiscal = value.Trim(); }
        }

        public string StrCertificadoSAT
        {
            get { return _strCertificadoSAT; }
            set { _strCertificadoSAT = value.Trim(); }
        }

        public string StrSelloCFDI
        {
            get { return _strSelloCFDI; }
            set { _strSelloCFDI = value.Trim(); }
        }

        public string StrSelloSAT
        {
            get { return _strSelloSAT; }
            set { _strSelloSAT = value.Trim(); }
        }

        public string StrVersionCFDI
        {
            get { return _strVersionCFDI; }
            set { _strVersionCFDI = value.Trim(); }
        }

        public string StrCadenaOriginalCertificacion
        {
            get
            {
                return "||" + _strVersionCFDI +
                        "|" + _strFolioFiscal +
                        "|" + _dtFechaCertificado.ToString("yyyy-MM-ddTHH:mm:ss") +
                        "|" + _strSelloCFDI +
                        "|" + _strCertificadoSAT +
                        "||";
            }
        }

        #endregion

        public CFacturaElectronica()
        {
            this._strVersion = string.Empty;
            this._strSerie = string.Empty;
            this._intFolio = 0;
            this._dtFecha = DateTime.Now;
            this._dtFechaCertificado = DateTime.Now;
            this._intNoAprobacion = 0;
            this._intAnoAprobacion = 0;
            this._strMetodoDePago = string.Empty;
            this._strNumCtaPago = string.Empty;
            this._strCondicionesDePago = string.Empty;
            this._amtSubTotal = 0;
            this._amtDescuento = 0;
            this._amtTotal = 0;
            this._strRFCEmisor = string.Empty;
            this._strNombreEmisor = string.Empty;
            this._strRegimen = string.Empty;
            this._dirDomicilioFiscal = new CDireccion();
            this._dirDomicilioExpedicion = new CDireccion();
            this._strRFCCliente = string.Empty;
            this._strNombreCliente = string.Empty;
            this._strCedulaFiscalCliente = string.Empty;
            this._strCedulaIEPSCliente = string.Empty;
            this._dirDomicilioCliente = new CDireccion();
            this._amtISR_Retenido = 0;
            this._amtTasa_ISR_Retenido = 0;
            this._amtIVA_Retenido = 0;
            this._amtTasa_IVA_Retenido = 0;
            this._amtTotalImpuestosRetenidos = 0;
            this._amtImpuesto = 0;
            this._amtTasa = 0;
            this._amtImporte = 0;
            this._amtTotalImpuestos = 0;
            this._strSelloCFDI = string.Empty;
            this._strSelloSAT = string.Empty;
            this._strVersionCFDI = string.Empty;
            this._strCertificadoSAT = string.Empty;
            this._strFolioFiscal = string.Empty;
            this._lstProductos = new List<CProducto>();
        }
    }

    public class CProducto
    {
        #region Miembros
        private string strCodigo;
        private string strDescripcion;
        private decimal amtCantidad;
        private string strUnidad;
        private decimal amtValorUnitario;
        private decimal amtImporte;
        private string strDetalle;
        #endregion

        #region Propiedades
        public string StrCodigo
        {
            get { return strCodigo; }
            set { strCodigo = value.Trim(); }
        }

        public string StrDescripcion
        {
            get { return strDescripcion; }
            set { strDescripcion = value.Trim(); }
        }

        public decimal AmtCantidad
        {
            get { return amtCantidad; }
            set { amtCantidad = value; }
        }

        public string StrUnidad
        {
            get { return strUnidad; }
            set { strUnidad = value.Trim(); }
        }

        public decimal AmtValorUnitario
        {
            get { return amtValorUnitario; }
            set { amtValorUnitario = value; }
        }

        public decimal AmtImporte
        {
            get { return amtImporte; }
            set { amtImporte = value; }
        }
        public string StrDetalle
        {
            get { return strDetalle; }
            set { strDetalle = value.Trim(); }
        }
        #endregion

        public CProducto()
        {
            this.strCodigo = string.Empty;
            this.strDescripcion = string.Empty;
            this.strDetalle = string.Empty;
            this.amtCantidad = 0;
            this.strUnidad = string.Empty;
            this.amtValorUnitario = 0;
            this.amtImporte = 0;
        }
    }

    public class CDireccion
    {
        #region Miembros
        private string strCalle;
        private string strNumeroExterior;
        private string strNumeroInterior;
        private string strColonia;
        private string strLocalidad;
        private string strReferencia;
        private string strMunicipio;
        private string strEstado;
        private string strPais;
        private string strCP;
        #endregion

        #region Propiedades
        public string StrCalle
        {
            get { return strCalle; }
            set { strCalle = value.Trim(); }
        }

        public string StrNumeroExterior
        {
            get { return strNumeroExterior; }
            set { strNumeroExterior = value.Trim(); }
        }

        public string StrNumeroInterior
        {
            get { return strNumeroInterior; }
            set { strNumeroInterior = value.Trim(); }
        }

        public string StrColonia
        {
            get { return strColonia; }
            set { strColonia = value.Trim(); }
        }

        public string StrLocalidad
        {
            get { return strLocalidad; }
            set { strLocalidad = value.Trim(); }
        }

        public string StrReferencia
        {
            get { return strReferencia; }
            set { strReferencia = value.Trim(); }
        }

        public string StrMunicipio
        {
            get { return strMunicipio; }
            set { strMunicipio = value.Trim(); }
        }

        public string StrEstado
        {
            get { return strEstado; }
            set { strEstado = value.Trim(); }
        }

        public string StrPais
        {
            get { return strPais; }
            set { strPais = value.Trim(); }
        }

        public string StrCP
        {
            get { return strCP; }
            set { strCP = value.Trim(); }
        }

        #endregion

        public CDireccion()
        {
            this.strCalle = string.Empty;
            this.strNumeroExterior = string.Empty;
            this.strNumeroInterior = string.Empty;
            this.strColonia = string.Empty;
            this.strLocalidad = string.Empty;
            this.strReferencia = string.Empty;
            this.strMunicipio = string.Empty;
            this.strEstado = string.Empty;
            this.strPais = string.Empty;
            this.strCP = string.Empty;
        }
    }

    public static class CFDI
    {
        public static bool Generar(int intFactID,
                                    out string strMensaje,
                                    out string strUUID,
                                    out DateTime dtFechaTimbrado)
        {
            string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
            string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");

            string strRFCEmisor, strRFCReceptor, strArchivoXMLSinFirma;
            strMensaje = strUUID = strRFCEmisor = strRFCReceptor = string.Empty;
            dtFechaTimbrado = DateTime.Now;

            HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
            bool swBorrarXML = true;

            #region PASO 1 ---> Generando XML

            #region Leyendo datos de la BD

            DataSet objDataResult = new DataSet();

            CFacturaDB objFactura = new CFacturaDB();
            if (!objFactura.Leer(intFactID))
            {
                strMensaje = objFactura.Mensaje;
                return false;
            }

            if (objFactura.fecha_timbrado.HasValue)  // Ya fue facturada
            {
                strMensaje = "Factura ya ha sido timbrada, únicamente se actualizaron los datos y su estatus";
                if (!objFactura.contado)
                {
                    // Busca fecha de contrarecibo
                    DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(1, objFactura.ID);
                    if (dtFechaCobranza.HasValue)
                    {
                        objFactura.status = 1;
                        objFactura.fecha_contrarecibo = dtFechaCobranza.Value;
                    }
                    else
                    {
                        objFactura.status = 2;
                        // Busca fecha de cobro
                        dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(2, objFactura.ID);
                        if (dtFechaCobranza.HasValue)
                            objFactura.fecha_cobranza = dtFechaCobranza.Value;
                        else
                            objFactura.fecha_cobranza = DateTime.Today;
                    }
                }
                else
                    objFactura.status = 0;

                objFactura.Guardar();
                return true;
            }

            string strSucID = objFactura.sucursal_ID.ToString();
            decimal dcmTotal = objFactura.total;
            decimal dcmPorISR_Ret = objFactura.isr_ret;
            decimal dcmISR_Ret = objFactura.monto_isr_ret;
            decimal dcmPorIVA_Ret = objFactura.iva_ret;
            decimal dcmIVA_Ret = objFactura.monto_iva_ret;
            decimal dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            decimal dcmPorIva = objFactura.iva;
            decimal dcmIva = objFactura.monto_iva;
            decimal dcmPorcDescuento = objFactura.descuento;
            decimal dcmPorcDescuento2 = objFactura.descuento2;
            decimal dcmSubtotal =  objFactura.monto_subtotal;

            decimal dcmDescuento = 0;
            if (dcmPorcDescuento > 0)
                dcmDescuento = dcmSubtotal - objFactura.monto_descuento;
            #endregion

            #region Preparando documento electrónico
            CComprobante objFactElectronica = new CComprobante();

            objFactElectronica.DtFecha = objFactura.fecha;
            objFactElectronica.StrFormaDePago = "Pago en una sola exhibicion";
            objFactElectronica.DcmSubTotal = dcmSubtotal;
            if (dcmDescuento != 0)
            {
                objFactElectronica.DcmDescuento = dcmDescuento;
                if (!string.IsNullOrEmpty(objFactura.comentarios))
                    objFactElectronica.StrMotivoDescuento = objFactura.comentarios;
            }
            objFactElectronica.DcmTotal = dcmTotal;
            if (objFactura.ID > 0)
                objFactElectronica.strFolio = objFactura.ID.ToString();
            #endregion

            #region Datos del Emisor
            string strQuery = "SELECT rfc, razonsocial," +
                             " Dom_Calle, Dom_NumExt, Dom_NumInt, Dom_Colonia," +
                             " Dom_Localidad, Dom_Referencia, Dom_Municipio, Dom_Estado," +
                             " Dom_Pais, Dom_CP, Exp_Calle, Exp_NumExt, Exp_NumInt," +
                             " Exp_Colonia, Exp_Localidad, Exp_Municipio, Exp_Estado," +
                             " Exp_Pais, Exp_CP, Lugar_Expedicion, Regimen," +
                             " Certificado, Certificado_Key, Certificado_Pwd" +
                             ",facturacion_prueba, incluir_detalle" +
                             " FROM sistema_apps" +
                             " WHERE ID = " + ckSIAN["ck_app"];

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del cliente ID[{0}]: {1}", ckSIAN["ck_app"], ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                strMensaje = String.Format("No se encontraron los datos del cliente ID[{0}]", ckSIAN["ck_app"]);
                Log(strDirXML, strMensaje);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            if (string.IsNullOrEmpty(objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no existe: " + objRowResult["Certificado"].ToString();
                return false;
            }

            if (string.IsNullOrEmpty(objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave de certificado no existe: " + objRowResult["Certificado_Key"].ToString();
                return false;
            }

            bool WSPrueba = (bool)objRowResult["facturacion_prueba"];
            bool swIncluirDetalle = (bool)objRowResult["incluir_detalle"];

            objFactElectronica.Certificado.StrPathCertificado = strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString();
            objFactElectronica.StrPathLlave = strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString();
            objFactElectronica.StrLlavePwd = objRowResult["Certificado_Pwd"].ToString();
            objFactElectronica.StrPathXSD = strDirSAT + "/Schemas/cfdv32.xsd";

            objFactElectronica.StrLugarExpedicion = objRowResult["Lugar_Expedicion"].ToString();

            objFactElectronica.Emisor.StrRFC = objRowResult["rfc"].ToString();
            objFactElectronica.Emisor.StrNombre = objRowResult["razonsocial"].ToString();

            strRFCEmisor = objRowResult["rfc"].ToString();

            // Domicilio Fiscal
            objFactElectronica.Emisor.Agregar_DomicilioFiscal();
            objFactElectronica.Emisor.DomicilioFiscal.StrCalle = objRowResult["Dom_Calle"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrNumeroExterior = objRowResult["Dom_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_NumInt"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrNumeroInterior = objRowResult["Dom_NumInt"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrColonia = objRowResult["Dom_Colonia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrLocalidad = objRowResult["Dom_Localidad"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_Referencia"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrReferencia = objRowResult["Dom_Referencia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrMunicipio = objRowResult["Dom_Municipio"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrEstado = objRowResult["Dom_Estado"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrPais = objRowResult["Dom_Pais"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrCP = objRowResult["Dom_CP"].ToString();

            // Lugar donde se expidio el documento
            objFactElectronica.Emisor.Agregar_ExpedidoEn();
            objFactElectronica.Emisor.ExpedidoEn.StrCalle = objRowResult["Exp_Calle"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrNumeroExterior = objRowResult["Exp_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Exp_NumInt"].ToString()))
                objFactElectronica.Emisor.ExpedidoEn.StrNumeroInterior = objRowResult["Exp_NumInt"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrColonia = objRowResult["Exp_Colonia"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrLocalidad = objRowResult["Exp_Localidad"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrMunicipio = objRowResult["Exp_Municipio"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrEstado = objRowResult["Exp_Estado"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrPais = objRowResult["Exp_Pais"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrCP = objRowResult["Exp_CP"].ToString();

            objFactElectronica.Emisor.Regimenes.Add(new CRegimen(objRowResult["Regimen"].ToString()));

            #endregion

            #region Datos del Receptor
            strQuery = " SELECT S.sucursal as sucursal, " +
                       " S.numero_codificacion as codificacion, " +
                       " E.razonsocial as negocio, " +
                       " E.direccionfiscal as direccionfiscal, " +
                       " E.num_exterior as num_exterior, " +
                       " E.num_interior as num_interior, " +
                       " E.colonia as colonia, " +
                       " E.cp as cp, " +
                       " E.poblacion as poblacion, " +
                       " E.municipio as municipio, " +
                       " E.estado as estado, " +
                       " E.pais as pais, " +
                       " E.pais as pais, " +
                       " E.rfc as rfc, " +
                       " E.cuenta_bancaria as cuenta_bancaria, " +
                       " M.metodo_pago as metodo_pago " +
                       " FROM sucursales S" +
                       " INNER JOIN establecimientos E" +
                       " ON S.establecimiento_ID = E.ID " +
                       " AND S.ID = " + strSucID +
                       " INNER JOIN metodos_pago M" +
                       " ON M.ID = E.metodo_pago ";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del receptor ID[{0}]: {1}", strSucID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            objRowResult = objDataResult.Tables[0].Rows[0];

            objFactElectronica.StrMetodoDePago = objRowResult["metodo_pago"].ToString();
            if (string.IsNullOrEmpty(objRowResult["cuenta_bancaria"].ToString()))
                objFactElectronica.StrNumCtaPago = "No identificado";
            else
                objFactElectronica.StrNumCtaPago = objRowResult["cuenta_bancaria"].ToString();

            objFactElectronica.EnumTipoDeComprobante = ComprobanteTipoDeComprobante.ingreso;
            objFactElectronica.StrMoneda = "MXN";

            objFactElectronica.Receptor.StrRFC = objRowResult["rfc"].ToString();
            strRFCReceptor = objRowResult["rfc"].ToString();
            objFactElectronica.Receptor.StrNombre = objRowResult["negocio"].ToString();

            objFactElectronica.Receptor.Agregar_Domicilio();
            objFactElectronica.Receptor.Domicilio.StrCalle = objRowResult["direccionfiscal"].ToString();
            objFactElectronica.Receptor.Domicilio.StrNumeroExterior = objRowResult["num_exterior"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["num_interior"].ToString()))
                objFactElectronica.Receptor.Domicilio.StrNumeroInterior = objRowResult["num_interior"].ToString();
            objFactElectronica.Receptor.Domicilio.StrColonia = objRowResult["colonia"].ToString();
            objFactElectronica.Receptor.Domicilio.StrLocalidad = objRowResult["poblacion"].ToString();
            objFactElectronica.Receptor.Domicilio.StrMunicipio = objRowResult["municipio"].ToString();
            objFactElectronica.Receptor.Domicilio.StrEstado = objRowResult["estado"].ToString();
            objFactElectronica.Receptor.Domicilio.StrPais = objRowResult["pais"].ToString();
            objFactElectronica.Receptor.Domicilio.StrCP = objRowResult["CP"].ToString();
            #endregion

            #region Conceptos
            strQuery = "SELECT F.cantidad as cantidad, " +
                        " F.consecutivo as consecutivo, " +
                        " F.costo_unitario as costo_unitario, " +
                        " F.costo as costo, " +
                        " F.usar_sal as usar_sal, " +
                        " F.usar_cve_gob as usar_cve_gob, " +
                        " P.clave as clave, " +
                        " P.nombre as nombre, " +
                        " P.sales as sales, " +
                        " P.clave_gobierno as clave_gobierno, " +
                        " P.unimed as unimed, " +
                        " P.piezasporcaja as piezasporcaja " +
                        " FROM facturas_liq_prod F" +
                        " INNER JOIN productos P" +
                        " ON F.producto_ID = P.ID " +
                        " WHERE F.factura_ID = " + intFactID.ToString() +
                        " ORDER BY consecutivo, nombre";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error los productos de la factura ID[{0}]: {1}", intFactID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            string strNombre = string.Empty;
            foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
            {
                if ((bool)objRowResult2["usar_sal"])
                    strNombre = objRowResult2["sales"].ToString();
                else
                    if ((bool)objRowResult2["usar_cve_gob"])
                        strNombre = objRowResult2["clave_gobierno"].ToString();
                    else
                        strNombre = objRowResult2["nombre"].ToString();

                objFactElectronica.LstConceptos.Add(new CConcepto((decimal)objRowResult2["cantidad"],
                                                                  objRowResult2["unimed"].ToString(),
                                                                  string.Empty,
                                                                  strNombre,
                                                                  (decimal)objRowResult2["costo_unitario"],
                                                                  (decimal)objRowResult2["costo"]
                                                                  ));
            }
            #endregion

            #region Impuestos
            if (dcmPorISR_Ret > 0 || dcmIVA_Ret > 0)
            {
                if (dcmPorISR_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.ISR,
                                                                dcmISR_Ret));

                if (dcmIVA_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.IVA,
                                                                dcmIVA_Ret));

                objFactElectronica.Impuestos.DcmTotalImpuestosRetenidos = dcmTotalRetenidos;
            }

            if (dcmPorIva > 0)
            {
                objFactElectronica.Impuestos.LstTraslados.Add(
                                             new CTraslado(ComprobanteImpuestosTrasladoImpuesto.IVA,
                                                           dcmPorIva,
                                                           dcmIva));
                objFactElectronica.Impuestos.DcmTotalImpuestosTrasladados = dcmIva;
            }
            #endregion

            #region Generando Archivo XML
            strArchivoXMLSinFirma = ckSIAN["ck_app"] + "_" + intFactID.ToString() + ".xml";

            if (!objFactElectronica.GenerarCFD())
            {
                strMensaje = String.Format("Error al generar la factura electrónica ID[{0}]: {1}", intFactID, objFactElectronica.StrMensaje);
                Log(strDirXML, strMensaje);
                return false;
            }
            #endregion

            #endregion

            #region Firmando XML

            #region Leyendo archivo XML
            Stream mXML = new MemoryStream();
            objFactElectronica.XmlCFDI.Save(mXML);
            byte[] fsBytes = ((MemoryStream)mXML).ToArray();
            mXML.Close();
            mXML.Dispose();
            #endregion

            wsGenericResp timbreFiscalDigital = Timbrar(fsBytes, WSPrueba, out strMensaje);

            if (!string.IsNullOrEmpty(strMensaje))
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intFactID, strMensaje);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }

            if (timbreFiscalDigital.isError)
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intFactID, timbreFiscalDigital.errorMessage);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }

            #endregion

            #region Generando XML Firmado
            string strNoCertificadoSAT = string.Empty;
            string strSelloCFD = string.Empty;
            string strSelloSAT = string.Empty;
            try
            {
                System.Xml.XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(new MemoryStream(timbreFiscalDigital.XML));
                XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");
                dtFechaTimbrado = Convert.ToDateTime(xmlSello[0].Attributes["FechaTimbrado"].Value);
                strNoCertificadoSAT = xmlSello[0].Attributes["noCertificadoSAT"].Value;
                strUUID = xmlSello[0].Attributes["UUID"].Value;
                strSelloCFD = xmlSello[0].Attributes["selloCFD"].Value;
                strSelloSAT = xmlSello[0].Attributes["selloSAT"].Value;
            }
            catch
            {
            }

            objFactElectronica.Agregar_TimbreFiscal();
            objFactElectronica.TimbreFiscalDigital.DtFechaTimbrado = dtFechaTimbrado;
            objFactElectronica.TimbreFiscalDigital.StrUUID = strUUID;
            objFactElectronica.TimbreFiscalDigital.StrSelloCFD = strSelloCFD;
            objFactElectronica.TimbreFiscalDigital.StrNoCertificadoSAT = strNoCertificadoSAT;
            objFactElectronica.TimbreFiscalDigital.StrSelloSAT = strSelloSAT;

            if (!objFactElectronica.GenerarCFDI(strDirXML +
                                                "/CFDI_" + strUUID + ".xml") ||
                string.IsNullOrEmpty(strNoCertificadoSAT))
            {
                strMensaje = String.Format("***ADMIN*** Error al guardar factura timbrada ID[{0}]: {1}, " +
                                           "UUID[{2}], FechaTimbrado[{3}], noCertificadoSAT[{4}], " +
                                           "selloSAT[{5}], selloCFD[{6}], version[{7}]",
                                           intFactID,
                                           objFactElectronica.StrMensaje,
                                           strUUID,
                                           dtFechaTimbrado,
                                           strNoCertificadoSAT,
                                           strSelloSAT,
                                           strSelloCFD,
                                           "3.2");
                Log(strDirXML, strMensaje);
                strMensaje = "Factura electrónica creada, pero hubo un error al generar el archivo XML";
                swBorrarXML = false;
            }
            else
            {
                strMensaje = "Factura Generada UUID: " + timbreFiscalDigital.folioUUID;
                Log(strDirXML, strMensaje);
            }
            #endregion

            objFactura.factura = timbreFiscalDigital.folioUUID;
            objFactura.fecha_timbrado = timbreFiscalDigital.fechaHoraTimbrado;

            objFactura.fecha_contrarecibo = null;
            objFactura.fecha_cobranza = null;
            if (!objFactura.contado)
            {
                // Busca fecha de contrarecibo
                DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(1, objFactura.ID);
                if (dtFechaCobranza.HasValue)
                {
                    objFactura.status = 1;
                    objFactura.fecha_contrarecibo = dtFechaCobranza.Value;
                }
                else
                {
                    objFactura.status = 2;
                    // Busca fecha de cobro
                    dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(2, objFactura.ID);
                    if (dtFechaCobranza.HasValue)
                        objFactura.fecha_cobranza = dtFechaCobranza.Value;
                    else
                        objFactura.fecha_cobranza = DateTime.Today;
                }
            }
            else
                objFactura.status = 0;

            if(objFactura.Guardar())
            {
                if(swBorrarXML)
                    File.Delete(strDirXML + "/" + strArchivoXMLSinFirma);
            }
            else
            {
                Log(strDirXML, objFactura.Mensaje);
            }

            return true;
        }

        public static wsGenericResp Timbrar(byte[] fsBytes,
                                            bool WSPrueba,
                                            out string strMensaje)
        {
            wsGenericResp timbreFiscalDigital = null;
            strMensaje = string.Empty;

            InterconectaWsClient timbradoWS;
            string strUser, strPwd;
            if (WSPrueba)
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
                strUser = ConfigurationManager.AppSettings["PACTestUser"];
                strPwd = ConfigurationManager.AppSettings["PACTestPwd"];
            }
            else
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");
                strUser = ConfigurationManager.AppSettings["PACUser"];
                strPwd = ConfigurationManager.AppSettings["PACPwd"];
            }

            try
            {
                timbreFiscalDigital = timbradoWS.timbraEnviaCFDIBytes(strUser,
                                                                      strPwd,
                                                                       null,
                                                                       null,
                                                                       string.Empty,
                                                                       fsBytes,
                                                                       "3.2"
                                                                       );
            }
            catch (Exception ex)
            {
                strMensaje = ex.Message;
            }

            return timbreFiscalDigital;
        }


        public static bool Generar_Nota_Cargo(int intNotaID,
                                    out string strMensaje,
                                    out string strUUID,
                                    out DateTime dtFechaTimbrado)
        {
            string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
            string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");

            string strRFCEmisor, strRFCReceptor, strArchivoXMLSinFirma;
            strMensaje = strUUID = strRFCEmisor = strRFCReceptor = string.Empty;
            dtFechaTimbrado = DateTime.Now;

            HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
            bool swBorrarXML = true;

            #region PASO 1 ---> Generando XML

            #region Leyendo datos de la BD

            DataSet objDataResult = new DataSet();

            CNotaCargoDB objNota = new CNotaCargoDB();
            if (!objNota.Leer(intNotaID))
            {
                strMensaje = objNota.Mensaje;
                return false;
            }

            if (objNota.fecha_timbrado.HasValue)  // Ya fue timbrada
            {
                strMensaje = "Nota ya ha sido timbrada, únicamente se actualizaron los datos y su estatus";
                if (!objNota.contado)
                {
                    // Busca fecha de contrarecibo
                    DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(1, objNota.ID);
                    if (dtFechaCobranza.HasValue)
                    {
                        objNota.status = 1;
                        objNota.fecha_contrarecibo = dtFechaCobranza.Value;
                    }
                    else
                    {
                        objNota.status = 2;
                        // Busca fecha de cobro
                        dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(2, objNota.ID);
                        if (dtFechaCobranza.HasValue)
                            objNota.fecha_cobranza = dtFechaCobranza.Value;
                        else
                            objNota.fecha_cobranza = DateTime.Today;
                    }
                }
                else
                    objNota.status = 0;

                objNota.Guardar();
                return true;
            }

            string strSucID = objNota.sucursal_ID.ToString();
            decimal dcmTotal = objNota.total;
            decimal dcmPorISR_Ret = objNota.isr_ret;
            decimal dcmISR_Ret = objNota.monto_isr_ret;
            decimal dcmPorIVA_Ret = objNota.iva_ret;
            decimal dcmIVA_Ret = objNota.monto_iva_ret;
            decimal dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            decimal dcmPorIva = objNota.iva;
            decimal dcmIva = objNota.monto_iva;
            decimal dcmPorcDescuento = objNota.descuento;
            decimal dcmPorcDescuento2 = objNota.descuento2;
            decimal dcmSubtotal = objNota.monto_subtotal;

            decimal dcmDescuento = 0;
            if (dcmPorcDescuento > 0)
                dcmDescuento = dcmSubtotal - objNota.monto_descuento;
            #endregion

            #region Preparando documento electrónico
            CComprobante objFactElectronica = new CComprobante();

            objFactElectronica.DtFecha = objNota.fecha;
            objFactElectronica.StrFormaDePago = "Pago en una sola exhibicion";
            objFactElectronica.DcmSubTotal = dcmSubtotal;
            if (dcmDescuento != 0)
            {
                objFactElectronica.DcmDescuento = dcmDescuento;
                if (!string.IsNullOrEmpty(objNota.comentarios))
                    objFactElectronica.StrMotivoDescuento = objNota.comentarios;
            }
            objFactElectronica.DcmTotal = dcmTotal;
            if (objNota.ID > 0)
                objFactElectronica.strFolio = objNota.ID.ToString();
            #endregion

            #region Datos del Emisor
            string strQuery = "SELECT rfc, razonsocial," +
                             " Dom_Calle, Dom_NumExt, Dom_NumInt, Dom_Colonia," +
                             " Dom_Localidad, Dom_Referencia, Dom_Municipio, Dom_Estado," +
                             " Dom_Pais, Dom_CP, Exp_Calle, Exp_NumExt, Exp_NumInt," +
                             " Exp_Colonia, Exp_Localidad, Exp_Municipio, Exp_Estado," +
                             " Exp_Pais, Exp_CP, Lugar_Expedicion, Regimen," +
                             " Certificado, Certificado_Key, Certificado_Pwd" +
                             ",facturacion_prueba, incluir_detalle" +
                             " FROM sistema_apps" +
                             " WHERE ID = " + ckSIAN["ck_app"];

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del cliente ID[{0}]: {1}", ckSIAN["ck_app"], ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                strMensaje = String.Format("No se encontraron los datos del cliente ID[{0}]", ckSIAN["ck_app"]);
                Log(strDirXML, strMensaje);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            if (string.IsNullOrEmpty(objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no existe: " + objRowResult["Certificado"].ToString();
                return false;
            }

            if (string.IsNullOrEmpty(objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave de certificado no existe: " + objRowResult["Certificado_Key"].ToString();
                return false;
            }

            bool WSPrueba = (bool)objRowResult["facturacion_prueba"];
            bool swIncluirDetalle = (bool)objRowResult["incluir_detalle"];

            objFactElectronica.Certificado.StrPathCertificado = strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString();
            objFactElectronica.StrPathLlave = strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString();
            objFactElectronica.StrLlavePwd = objRowResult["Certificado_Pwd"].ToString();
            objFactElectronica.StrPathXSD = strDirSAT + "/Schemas/cfdv32.xsd";

            objFactElectronica.StrLugarExpedicion = objRowResult["Lugar_Expedicion"].ToString();

            objFactElectronica.Emisor.StrRFC = objRowResult["rfc"].ToString();
            objFactElectronica.Emisor.StrNombre = objRowResult["razonsocial"].ToString();

            strRFCEmisor = objRowResult["rfc"].ToString();

            // Domicilio Fiscal
            objFactElectronica.Emisor.Agregar_DomicilioFiscal();
            objFactElectronica.Emisor.DomicilioFiscal.StrCalle = objRowResult["Dom_Calle"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrNumeroExterior = objRowResult["Dom_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_NumInt"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrNumeroInterior = objRowResult["Dom_NumInt"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrColonia = objRowResult["Dom_Colonia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrLocalidad = objRowResult["Dom_Localidad"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_Referencia"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrReferencia = objRowResult["Dom_Referencia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrMunicipio = objRowResult["Dom_Municipio"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrEstado = objRowResult["Dom_Estado"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrPais = objRowResult["Dom_Pais"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrCP = objRowResult["Dom_CP"].ToString();

            // Lugar donde se expidio el documento
            objFactElectronica.Emisor.Agregar_ExpedidoEn();
            objFactElectronica.Emisor.ExpedidoEn.StrCalle = objRowResult["Exp_Calle"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrNumeroExterior = objRowResult["Exp_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Exp_NumInt"].ToString()))
                objFactElectronica.Emisor.ExpedidoEn.StrNumeroInterior = objRowResult["Exp_NumInt"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrColonia = objRowResult["Exp_Colonia"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrLocalidad = objRowResult["Exp_Localidad"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrMunicipio = objRowResult["Exp_Municipio"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrEstado = objRowResult["Exp_Estado"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrPais = objRowResult["Exp_Pais"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrCP = objRowResult["Exp_CP"].ToString();

            objFactElectronica.Emisor.Regimenes.Add(new CRegimen(objRowResult["Regimen"].ToString()));

            #endregion

            #region Datos del Receptor
            strQuery = " SELECT sucursales.sucursal as sucursal, " +
                " sucursales.numero_codificacion as codificacion, " +
                " establecimientos.razonsocial as negocio, " +
                " establecimientos.direccionfiscal as direccionfiscal, " +
                " establecimientos.num_exterior as num_exterior, " +
                " establecimientos.num_interior as num_interior, " +
                " establecimientos.colonia as colonia, " +
                " establecimientos.cp as cp, " +
                " establecimientos.poblacion as poblacion, " +
                " establecimientos.municipio as municipio, " +
                " establecimientos.estado as estado, " +
                " establecimientos.pais as pais, " +
                " establecimientos.pais as pais, " +
                " establecimientos.rfc as rfc, " +
                " establecimientos.cuenta_bancaria as cuenta_bancaria, " +
                " metodos_pago.metodo_pago as metodo_pago " +
                " FROM sucursales " +
                " INNER JOIN establecimientos " +
                " ON sucursales.establecimiento_ID = establecimientos.ID " +
                " AND sucursales.ID = " + strSucID +
                " INNER JOIN metodos_pago " +
                " ON metodos_pago.ID = establecimientos.metodo_pago ";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del receptor ID[{0}]: {1}", strSucID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            objRowResult = objDataResult.Tables[0].Rows[0];

            objFactElectronica.StrMetodoDePago = objRowResult["metodo_pago"].ToString();
            if (string.IsNullOrEmpty(objRowResult["cuenta_bancaria"].ToString()))
                objFactElectronica.StrNumCtaPago = "No identificado";
            else
                objFactElectronica.StrNumCtaPago = objRowResult["cuenta_bancaria"].ToString();

            objFactElectronica.EnumTipoDeComprobante = ComprobanteTipoDeComprobante.ingreso;
            objFactElectronica.StrMoneda = "MX";

            objFactElectronica.Receptor.StrRFC = objRowResult["rfc"].ToString();
            strRFCReceptor = objRowResult["rfc"].ToString();
            objFactElectronica.Receptor.StrNombre = objRowResult["negocio"].ToString();

            objFactElectronica.Receptor.Agregar_Domicilio();
            objFactElectronica.Receptor.Domicilio.StrCalle = objRowResult["direccionfiscal"].ToString();
            objFactElectronica.Receptor.Domicilio.StrNumeroExterior = objRowResult["num_exterior"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["num_interior"].ToString()))
                objFactElectronica.Receptor.Domicilio.StrNumeroInterior = objRowResult["num_interior"].ToString();
            objFactElectronica.Receptor.Domicilio.StrColonia = objRowResult["colonia"].ToString();
            objFactElectronica.Receptor.Domicilio.StrLocalidad = objRowResult["poblacion"].ToString();
            objFactElectronica.Receptor.Domicilio.StrMunicipio = objRowResult["municipio"].ToString();
            objFactElectronica.Receptor.Domicilio.StrEstado = objRowResult["estado"].ToString();
            objFactElectronica.Receptor.Domicilio.StrPais = objRowResult["pais"].ToString();
            objFactElectronica.Receptor.Domicilio.StrCP = objRowResult["CP"].ToString();
            #endregion

            #region Conceptos
            strQuery = "SELECT F.cantidad as cantidad, " +
                        " F.consecutivo as consecutivo, " +
                        " F.costo_unitario as costo_unitario, " +
                        " F.costo as costo, " +
                        " P.clave as clave, " +
                        " P.nombre as nombre, " +
                        " P.unimed as unimed, " +
                        " P.piezasporcaja as piezasporcaja " +
                        " FROM notas_cargo_prod F" +
                        " INNER JOIN productos P" +
                        " ON F.producto_ID = P.ID " +
                        " AND F.nota_ID = " + intNotaID.ToString() +
                        " ORDER BY consecutivo, nombre";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error los productos de la nota ID[{0}]: {1}", intNotaID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
            {
                objFactElectronica.LstConceptos.Add(new CConcepto((decimal)objRowResult2["cantidad"],
                                                                  objRowResult2["unimed"].ToString(),
                                                                  string.Empty,
                                                                  objRowResult2["nombre"].ToString(),
                                                                  (decimal)objRowResult2["costo_unitario"],
                                                                  (decimal)objRowResult2["costo"]
                                                                  ));
            }
            #endregion

            #region Impuestos
            if (dcmPorISR_Ret > 0 || dcmIVA_Ret > 0)
            {
                if (dcmPorISR_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.ISR,
                                                                dcmISR_Ret));

                if (dcmIVA_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.IVA,
                                                                dcmIVA_Ret));

                objFactElectronica.Impuestos.DcmTotalImpuestosRetenidos = dcmTotalRetenidos;
            }

            if (dcmPorIva > 0)
            {
                objFactElectronica.Impuestos.LstTraslados.Add(
                                             new CTraslado(ComprobanteImpuestosTrasladoImpuesto.IVA,
                                                           dcmPorIva,
                                                           dcmIva));
                objFactElectronica.Impuestos.DcmTotalImpuestosTrasladados = dcmIva;
            }
            #endregion

            #region Generando Archivo XML
            strArchivoXMLSinFirma = ckSIAN["ck_app"] + "_" + intNotaID.ToString() + ".xml";

            if (!objFactElectronica.GenerarCFD())
            {
                strMensaje = String.Format("Error al generar la nota electrónica ID[{0}]: {1}", intNotaID, objFactElectronica.StrMensaje);
                Log(strDirXML, strMensaje);
                return false;
            }
            #endregion

            #endregion

            #region Firmando XML

            #region Leyendo archivo XML
            Stream mXML = new MemoryStream();
            objFactElectronica.XmlCFDI.Save(mXML);
            byte[] fsBytes = ((MemoryStream)mXML).ToArray();
            mXML.Close();
            mXML.Dispose();
            #endregion

            wsGenericResp timbreFiscalDigital = Timbrar(fsBytes, WSPrueba, out strMensaje);

            if (!string.IsNullOrEmpty(strMensaje))
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intNotaID, strMensaje);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }

            if (timbreFiscalDigital.isError)
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intNotaID, timbreFiscalDigital.errorMessage);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }

            #endregion

            #region Generando XML Firmado
            string strNoCertificadoSAT = string.Empty;
            try
            {
                System.Xml.XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(new MemoryStream(timbreFiscalDigital.XML));
                XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");
                strNoCertificadoSAT = xmlSello[0].Attributes["noCertificadoSAT"].Value;
            }
            catch
            {
            }

            objFactElectronica.Agregar_TimbreFiscal();
            objFactElectronica.TimbreFiscalDigital.DtFechaTimbrado = new DateTime(timbreFiscalDigital.fechaHoraTimbrado.Year,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Month,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Day,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Hour,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Minute,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Second);
            objFactElectronica.TimbreFiscalDigital.StrUUID = timbreFiscalDigital.folioUUID;
            objFactElectronica.TimbreFiscalDigital.StrSelloCFD = timbreFiscalDigital.selloDigitalEmisor;
            objFactElectronica.TimbreFiscalDigital.StrNoCertificadoSAT = strNoCertificadoSAT;
            objFactElectronica.TimbreFiscalDigital.StrSelloSAT = timbreFiscalDigital.selloDigitalTimbreSAT;

            if (!objFactElectronica.GenerarCFDI(strDirXML +
                                                "/CFDI_" + timbreFiscalDigital.folioUUID + ".xml") ||
                string.IsNullOrEmpty(strNoCertificadoSAT))
            {
                strMensaje = String.Format("***ADMIN*** Error al guardar nota timbrada ID[{0}]: {1}, " +
                                           "UUID[{2}], FechaTimbrado[{3}], noCertificadoSAT[{4}], " +
                                           "selloSAT[{5}], selloCFD[{6}], version[{7}]",
                                           intNotaID,
                                           objFactElectronica.StrMensaje,
                                           timbreFiscalDigital.folioUUID,
                                           timbreFiscalDigital.fechaHoraTimbrado,
                                           strNoCertificadoSAT,
                                           timbreFiscalDigital.selloDigitalTimbreSAT,
                                           timbreFiscalDigital.selloDigitalEmisor,
                                           "3.2");
                Log(strDirXML, strMensaje);
                strMensaje = "Nota electrónica creada, pero hubo un error al generar el archivo XML";
                swBorrarXML = false;
            }
            else
            {
                strMensaje = "Nota Generada UUID: " + timbreFiscalDigital.folioUUID;
                Log(strDirXML, strMensaje);
            }
            #endregion

            objNota.nota = timbreFiscalDigital.folioUUID;
            objNota.fecha_timbrado = timbreFiscalDigital.fechaHoraTimbrado;

            objNota.fecha_contrarecibo = null;
            objNota.fecha_cobranza = null;
            if (!objNota.contado)
            {
                // Busca fecha de contrarecibo
                DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(1, objNota.ID);
                if (dtFechaCobranza.HasValue)
                {
                    objNota.status = 1;
                    objNota.fecha_contrarecibo = dtFechaCobranza.Value;
                }
                else
                {
                    objNota.status = 2;
                    // Busca fecha de cobro
                    dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(2, objNota.ID);
                    if (dtFechaCobranza.HasValue)
                        objNota.fecha_cobranza = dtFechaCobranza.Value;
                    else
                        objNota.fecha_cobranza = DateTime.Today;
                }
            }
            else
                objNota.status = 0;

            if (objNota.Guardar())
            {
                if (swBorrarXML)
                    File.Delete(strDirXML + "/" + strArchivoXMLSinFirma);
            }
            else
            {
                Log(strDirXML, objNota.Mensaje);
            }

            return true;
        }

        public static bool Generar_Nota_Credito(int intNotaID,
                                    out string strMensaje,
                                    out string strUUID,
                                    out DateTime dtFechaTimbrado)
        {
            string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
            string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");

            string strRFCEmisor, strRFCReceptor, strArchivoXMLSinFirma;
            strMensaje = strUUID = strRFCEmisor = strRFCReceptor = string.Empty;
            dtFechaTimbrado = DateTime.Now;

            HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
            bool swBorrarXML = true;

            #region PASO 1 ---> Generando XML

            #region Leyendo datos de la BD

            DataSet objDataResult = new DataSet();

            CNotaCreditoDB objNota = new CNotaCreditoDB();
            if (!objNota.Leer(intNotaID))
            {
                strMensaje = objNota.Mensaje;
                return false;
            }

            if (objNota.fecha_timbrado.HasValue)  // Ya fue timbrada
            {
                strMensaje = "Nota ya ha sido timbrada, únicamente se actualizaron los datos y su estatus";
                if (!objNota.contado)
                    objNota.status = 1;
                else
                    objNota.status = 0;

                objNota.Guardar();
                return true;
            }

            string strSucID = objNota.sucursal_ID.ToString();
            decimal dcmTotal = objNota.total;
            decimal dcmPorISR_Ret = objNota.isr_ret;
            decimal dcmISR_Ret = objNota.monto_isr_ret;
            decimal dcmPorIVA_Ret = objNota.iva_ret;
            decimal dcmIVA_Ret = objNota.monto_iva_ret;
            decimal dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            decimal dcmPorIva = objNota.iva;
            decimal dcmIva = objNota.monto_iva;
            decimal dcmPorcDescuento = objNota.descuento;
            decimal dcmPorcDescuento2 = objNota.descuento2;
            decimal dcmSubtotal = objNota.monto_subtotal;

            decimal dcmDescuento = 0;
            if (dcmPorcDescuento > 0)
                dcmDescuento = dcmSubtotal - objNota.monto_descuento;
            #endregion

            #region Preparando documento electrónico
            CComprobante objFactElectronica = new CComprobante();

            objFactElectronica.DtFecha = objNota.fecha;
            objFactElectronica.StrFormaDePago = "Pago en una sola exhibicion";
            objFactElectronica.DcmSubTotal = dcmSubtotal;
            if (dcmDescuento != 0)
            {
                objFactElectronica.DcmDescuento = dcmDescuento;
                if (!string.IsNullOrEmpty(objNota.comentarios))
                    objFactElectronica.StrMotivoDescuento = objNota.comentarios;
            }
            objFactElectronica.DcmTotal = dcmTotal;
            if (objNota.ID > 0)
                objFactElectronica.strFolio = objNota.ID.ToString();
            #endregion

            #region Datos del Emisor
            string strQuery = "SELECT rfc, razonsocial," +
                             " Dom_Calle, Dom_NumExt, Dom_NumInt, Dom_Colonia," +
                             " Dom_Localidad, Dom_Referencia, Dom_Municipio, Dom_Estado," +
                             " Dom_Pais, Dom_CP, Exp_Calle, Exp_NumExt, Exp_NumInt," +
                             " Exp_Colonia, Exp_Localidad, Exp_Municipio, Exp_Estado," +
                             " Exp_Pais, Exp_CP, Lugar_Expedicion, Regimen," +
                             " Certificado, Certificado_Key, Certificado_Pwd" +
                             ",facturacion_prueba, incluir_detalle" +
                             " FROM sistema_apps" +
                             " WHERE ID = " + ckSIAN["ck_app"];

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del cliente ID[{0}]: {1}", ckSIAN["ck_app"], ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                strMensaje = String.Format("No se encontraron los datos del cliente ID[{0}]", ckSIAN["ck_app"]);
                Log(strDirXML, strMensaje);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            if (string.IsNullOrEmpty(objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString()))
            {
                strMensaje = "Archivo del certificado no existe: " + objRowResult["Certificado"].ToString();
                return false;
            }

            if (string.IsNullOrEmpty(objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave del certificado no ha sido definido";
                return false;
            }

            if (!File.Exists(strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString()))
            {
                strMensaje = "Archivo de la llave de certificado no existe: " + objRowResult["Certificado_Key"].ToString();
                return false;
            }

            bool WSPrueba = (bool)objRowResult["facturacion_prueba"];
            bool swIncluirDetalle = (bool)objRowResult["incluir_detalle"];

            objFactElectronica.Certificado.StrPathCertificado = strDirSAT + "/Certificados/" + objRowResult["Certificado"].ToString();
            objFactElectronica.StrPathLlave = strDirSAT + "/Certificados/" + objRowResult["Certificado_Key"].ToString();
            objFactElectronica.StrLlavePwd = objRowResult["Certificado_Pwd"].ToString();
            objFactElectronica.StrPathXSD = strDirSAT + "/Schemas/cfdv32.xsd";

            objFactElectronica.StrLugarExpedicion = objRowResult["Lugar_Expedicion"].ToString();

            objFactElectronica.Emisor.StrRFC = objRowResult["rfc"].ToString();
            objFactElectronica.Emisor.StrNombre = objRowResult["razonsocial"].ToString();

            strRFCEmisor = objRowResult["rfc"].ToString();

            // Domicilio Fiscal
            objFactElectronica.Emisor.Agregar_DomicilioFiscal();
            objFactElectronica.Emisor.DomicilioFiscal.StrCalle = objRowResult["Dom_Calle"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrNumeroExterior = objRowResult["Dom_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_NumInt"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrNumeroInterior = objRowResult["Dom_NumInt"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrColonia = objRowResult["Dom_Colonia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrLocalidad = objRowResult["Dom_Localidad"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Dom_Referencia"].ToString()))
                objFactElectronica.Emisor.DomicilioFiscal.StrReferencia = objRowResult["Dom_Referencia"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrMunicipio = objRowResult["Dom_Municipio"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrEstado = objRowResult["Dom_Estado"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrPais = objRowResult["Dom_Pais"].ToString();
            objFactElectronica.Emisor.DomicilioFiscal.StrCP = objRowResult["Dom_CP"].ToString();

            // Lugar donde se expidio el documento
            objFactElectronica.Emisor.Agregar_ExpedidoEn();
            objFactElectronica.Emisor.ExpedidoEn.StrCalle = objRowResult["Exp_Calle"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrNumeroExterior = objRowResult["Exp_NumExt"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["Exp_NumInt"].ToString()))
                objFactElectronica.Emisor.ExpedidoEn.StrNumeroInterior = objRowResult["Exp_NumInt"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrColonia = objRowResult["Exp_Colonia"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrLocalidad = objRowResult["Exp_Localidad"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrMunicipio = objRowResult["Exp_Municipio"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrEstado = objRowResult["Exp_Estado"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrPais = objRowResult["Exp_Pais"].ToString();
            objFactElectronica.Emisor.ExpedidoEn.StrCP = objRowResult["Exp_CP"].ToString();

            objFactElectronica.Emisor.Regimenes.Add(new CRegimen(objRowResult["Regimen"].ToString()));

            #endregion

            #region Datos del Receptor
            strQuery = " SELECT sucursales.sucursal as sucursal, " +
                " sucursales.numero_codificacion as codificacion, " +
                " establecimientos.razonsocial as negocio, " +
                " establecimientos.direccionfiscal as direccionfiscal, " +
                " establecimientos.num_exterior as num_exterior, " +
                " establecimientos.num_interior as num_interior, " +
                " establecimientos.colonia as colonia, " +
                " establecimientos.cp as cp, " +
                " establecimientos.poblacion as poblacion, " +
                " establecimientos.municipio as municipio, " +
                " establecimientos.estado as estado, " +
                " establecimientos.pais as pais, " +
                " establecimientos.pais as pais, " +
                " establecimientos.rfc as rfc, " +
                " establecimientos.cuenta_bancaria as cuenta_bancaria, " +
                " metodos_pago.metodo_pago as metodo_pago " +
                " FROM sucursales " +
                " INNER JOIN establecimientos " +
                " ON sucursales.establecimiento_ID = establecimientos.ID " +
                " AND sucursales.ID = " + strSucID +
                " INNER JOIN metodos_pago " +
                " ON metodos_pago.ID = establecimientos.metodo_pago ";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error al leer los datos del receptor ID[{0}]: {1}", strSucID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            objRowResult = objDataResult.Tables[0].Rows[0];

            objFactElectronica.StrMetodoDePago = "No identificado";
            objFactElectronica.StrNumCtaPago = "No identificado";

            objFactElectronica.EnumTipoDeComprobante = ComprobanteTipoDeComprobante.egreso;
            objFactElectronica.StrMoneda = "MX";

            objFactElectronica.Receptor.StrRFC = objRowResult["rfc"].ToString();
            strRFCReceptor = objRowResult["rfc"].ToString();
            objFactElectronica.Receptor.StrNombre = objRowResult["negocio"].ToString();

            objFactElectronica.Receptor.Agregar_Domicilio();
            objFactElectronica.Receptor.Domicilio.StrCalle = objRowResult["direccionfiscal"].ToString();
            objFactElectronica.Receptor.Domicilio.StrNumeroExterior = objRowResult["num_exterior"].ToString();
            if (!string.IsNullOrEmpty(objRowResult["num_interior"].ToString()))
                objFactElectronica.Receptor.Domicilio.StrNumeroInterior = objRowResult["num_interior"].ToString();
            objFactElectronica.Receptor.Domicilio.StrColonia = objRowResult["colonia"].ToString();
            objFactElectronica.Receptor.Domicilio.StrLocalidad = objRowResult["poblacion"].ToString();
            objFactElectronica.Receptor.Domicilio.StrMunicipio = objRowResult["municipio"].ToString();
            objFactElectronica.Receptor.Domicilio.StrEstado = objRowResult["estado"].ToString();
            objFactElectronica.Receptor.Domicilio.StrPais = objRowResult["pais"].ToString();
            objFactElectronica.Receptor.Domicilio.StrCP = objRowResult["CP"].ToString();
            #endregion

            #region Conceptos
            strQuery = "SELECT F.cantidad as cantidad, " +
                        " F.consecutivo as consecutivo, " +
                        " F.costo_unitario as costo_unitario, " +
                        " F.costo as costo, " +
                        " P.clave as clave, " +
                        " P.nombre as nombre, " +
                        " P.unimed as unimed, " +
                        " P.piezasporcaja as piezasporcaja " +
                        " FROM notas_credito_prod F" +
                        " INNER JOIN productos P" +
                        " ON F.producto_ID = P.ID " +
                        " AND F.nota_ID = " + intNotaID.ToString() +
                        " ORDER BY consecutivo, nombre";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                strMensaje = String.Format("Error los productos de la nota ID[{0}]: {1}", intNotaID, ex.Message);
                Log(strDirXML, strMensaje);
                return false;
            }

            foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
            {
                objFactElectronica.LstConceptos.Add(new CConcepto((decimal)objRowResult2["cantidad"],
                                                                  objRowResult2["unimed"].ToString(),
                                                                  string.Empty,
                                                                  objRowResult2["nombre"].ToString(),
                                                                  (decimal)objRowResult2["costo_unitario"],
                                                                  (decimal)objRowResult2["costo"]
                                                                  ));
            }
            #endregion

            #region Impuestos
            if (dcmPorISR_Ret > 0 || dcmIVA_Ret > 0)
            {
                if (dcmPorISR_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.ISR,
                                                                dcmISR_Ret));

                if (dcmIVA_Ret > 0)
                    objFactElectronica.Impuestos.LstRetenciones.Add(
                                                 new CRetencion(ComprobanteImpuestosRetencionImpuesto.IVA,
                                                                dcmIVA_Ret));

                objFactElectronica.Impuestos.DcmTotalImpuestosRetenidos = dcmTotalRetenidos;
            }

            if (dcmPorIva > 0)
            {
                objFactElectronica.Impuestos.LstTraslados.Add(
                                             new CTraslado(ComprobanteImpuestosTrasladoImpuesto.IVA,
                                                           dcmPorIva,
                                                           dcmIva));
                objFactElectronica.Impuestos.DcmTotalImpuestosTrasladados = dcmIva;
            }
            #endregion

            #region Generando Archivo XML
            strArchivoXMLSinFirma = ckSIAN["ck_app"] + "_" + intNotaID.ToString() + ".xml";

            if (!objFactElectronica.GenerarCFD())
            {
                strMensaje = String.Format("Error al generar la nota electrónica ID[{0}]: {1}", intNotaID, objFactElectronica.StrMensaje);
                Log(strDirXML, strMensaje);
                return false;
            }
            #endregion

            #endregion

            #region Firmando XML
            #region Leyendo archivo XML
            Stream mXML = new MemoryStream();
            objFactElectronica.XmlCFDI.Save(mXML);
            byte[] fsBytes = ((MemoryStream)mXML).ToArray();
            mXML.Close();
            mXML.Dispose();
            #endregion

            wsGenericResp timbreFiscalDigital = Timbrar(fsBytes, WSPrueba, out strMensaje);

            if (!string.IsNullOrEmpty(strMensaje))
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intNotaID, strMensaje);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }

            if (timbreFiscalDigital.isError)
            {
                strMensaje = String.Format("Error al timbrar la factura electrónica ID[{0}]: {1}", intNotaID, timbreFiscalDigital.errorMessage);
                Log(strDirXML, strMensaje);
                objFactElectronica.XmlCFDI.Save(strDirXML + "/ErrorTimbrado.xml");
                return false;
            }
            #endregion

            #region Generando XML Firmado
            string strNoCertificadoSAT = string.Empty;
            try
            {
                System.Xml.XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(new MemoryStream(timbreFiscalDigital.XML));
                XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");
                strNoCertificadoSAT = xmlSello[0].Attributes["noCertificadoSAT"].Value;
            }
            catch
            {
            }

            objFactElectronica.Agregar_TimbreFiscal();
            objFactElectronica.TimbreFiscalDigital.DtFechaTimbrado = new DateTime(timbreFiscalDigital.fechaHoraTimbrado.Year,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Month,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Day,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Hour,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Minute,
                                                                                  timbreFiscalDigital.fechaHoraTimbrado.Second);
            objFactElectronica.TimbreFiscalDigital.StrUUID = timbreFiscalDigital.folioUUID;
            objFactElectronica.TimbreFiscalDigital.StrSelloCFD = timbreFiscalDigital.selloDigitalEmisor;
            objFactElectronica.TimbreFiscalDigital.StrNoCertificadoSAT = strNoCertificadoSAT;
            objFactElectronica.TimbreFiscalDigital.StrSelloSAT = timbreFiscalDigital.selloDigitalTimbreSAT;

            if (!objFactElectronica.GenerarCFDI(strDirXML +
                                                "/CFDI_" + timbreFiscalDigital.folioUUID + ".xml") ||
                string.IsNullOrEmpty(strNoCertificadoSAT))
            {
                strMensaje = String.Format("***ADMIN*** Error al guardar nota timbrada ID[{0}]: {1}, " +
                                           "UUID[{2}], FechaTimbrado[{3}], noCertificadoSAT[{4}], " +
                                           "selloSAT[{5}], selloCFD[{6}], version[{7}]",
                                           intNotaID,
                                           objFactElectronica.StrMensaje,
                                           timbreFiscalDigital.folioUUID,
                                           timbreFiscalDigital.fechaHoraTimbrado,
                                           strNoCertificadoSAT,
                                           timbreFiscalDigital.selloDigitalTimbreSAT,
                                           timbreFiscalDigital.selloDigitalEmisor,
                                           "3.2");
                Log(strDirXML, strMensaje);
                strMensaje = "Nota electrónica creada, pero hubo un error al generar el archivo XML";
                swBorrarXML = false;
            }
            else
            {
                strMensaje = "Nota Generada UUID: " + timbreFiscalDigital.folioUUID;
                Log(strDirXML, strMensaje);
            }
            #endregion

            objNota.nota = timbreFiscalDigital.folioUUID;
            objNota.fecha_timbrado = timbreFiscalDigital.fechaHoraTimbrado;

            objNota.fecha_contrarecibo = null;
            objNota.fecha_cobranza = null;
            if (!objNota.contado)
                objNota.status = 1;
            else
                objNota.status = 0;

            if (objNota.Guardar())
            {
                if (swBorrarXML)
                    File.Delete(strDirXML + "/" + strArchivoXMLSinFirma);
            }
            else
            {
                Log(strDirXML, objNota.Mensaje);
            }

            return true;
        }

        public static bool Cancelar_Factura(int intFactID, out string strMensaje)
        {
            strMensaje = string.Empty;

            CFacturaDB objFactura = new CFacturaDB();
            if (!objFactura.Leer(intFactID))
            {
                strMensaje = objFactura.Mensaje;
                return false;
            }

            if (objFactura.fecha_timbrado.HasValue)
            {
                string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
                string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
                string strFact = "CFDI_" + objFactura.factura + ".xml";
                if (!File.Exists(strDirXML + "/" + strFact))
                {
                    strMensaje = "Archivo XML de la factura no existe";
                    return false;
                }

                HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT facturacion_prueba" +
                                 " FROM sistema_apps" +
                                 " WHERE ID = " + ckSIAN["ck_app"];

                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

                DataRow objRowResult = objDataResult.Tables[0].Rows[0];

                bool WSPrueba = (bool)objRowResult["facturacion_prueba"];

                XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(strDirXML + "/" + strFact);

                XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");

                wsGenericResp timbreFiscalDigital = Cancelar_CFDI(xmlFactura,
                                                                  WSPrueba,
                                                                  out strMensaje);

                if (timbreFiscalDigital.isError)
                {
                    strMensaje = timbreFiscalDigital.errorMessage;
                }
                else
                {
                    System.Xml.XmlDocument xmlAcuse = new XmlDocument();
                    xmlAcuse.Load(new MemoryStream(timbreFiscalDigital.acuse));
                    xmlAcuse.Save(strDirXML + "/CFDI_" + xmlSello[0].Attributes["UUID"].Value + "_CANC.xml");
                    objFactura.fecha_cancelacion = DateTime.Now;
                    objFactura.status = 9;
                    objFactura.Guardar();
                    strMensaje = "Factura ha sido cancelada";
                    Log(strDirXML, "Factura cancelada: " + xmlSello[0].Attributes["UUID"].Value);
                    return true;
                }
            }
            else
            {
                strMensaje = "Factura no ha sido timbrada";
            }

            return false;
        }

        public static bool Valida_Factura(int intFactID, out string strMensaje)
        {
            strMensaje = string.Empty;

            CFacturaDB objFactura = new CFacturaDB();
            if (!objFactura.Leer(intFactID))
            {
                strMensaje = objFactura.Mensaje;
                return false;
            }

            if (objFactura.fecha_timbrado.HasValue)
            {
                string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
                string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
                string strFact = "CFDI_" + objFactura.factura + ".xml";
                if (!File.Exists(strDirXML + "/" + strFact))
                {
                    strMensaje = "Archivo XML de la factura no existe";
                    return false;
                }

                HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT facturacion_prueba" +
                                 " FROM sistema_apps" +
                                 " WHERE ID = " + ckSIAN["ck_app"];

                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

                DataRow objRowResult = objDataResult.Tables[0].Rows[0];

                bool WSPrueba = (bool)objRowResult["facturacion_prueba"];

                XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(strDirXML + "/" + strFact);

                XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");

                wsValidaResp validacionCFDI = Valida_Comprobante(xmlFactura,
                                                                    WSPrueba,
                                                                    out strMensaje);

                StringBuilder strErrores = new StringBuilder();
                if (validacionCFDI.isError)
                {
                    strErrores.Append(validacionCFDI.errorMessage);
                }
                else
                {
                    if (validacionCFDI.comprobanteValido)
                        strErrores.Append("Comprobante VALIDO!!!");
                }

                if (validacionCFDI.listaErrores != null)
                {
                    strErrores.Append("<br/>ERRORES");
                    foreach (wsInformaValidacion msg in validacionCFDI.listaErrores)
                    {
                        strErrores.Append("<br/>");
                        strErrores.Append("Codigo[" + msg.codigo.ToString() + "]");
                        strErrores.Append("Nombre[" + msg.nombre.ToString() + "]");
                        strErrores.Append("Valor[" + msg.valor.ToString() + "]");
                    }
                }

                if (validacionCFDI.listaAdvertencias != null)
                {
                    strErrores.Append("<br/>ADVERTENCIAS");
                    foreach (wsInformaValidacion msg in validacionCFDI.listaAdvertencias)
                    {
                        strErrores.Append("<br/>");
                        strErrores.Append("Codigo[" + msg.codigo.ToString() + "]");
                        strErrores.Append("Nombre[" + msg.nombre.ToString() + "]");
                        strErrores.Append("Valor[" + msg.valor.ToString() + "]");
                    }
                }

                if (validacionCFDI.listaInformacion != null)
                {
                    strErrores.Append("<br/>INFORMACION");
                    foreach (wsInformaValidacion msg in validacionCFDI.listaInformacion)
                    {
                        strErrores.Append("<br/>");
                        strErrores.Append("Codigo[" + msg.codigo.ToString() + "]");
                        strErrores.Append("Nombre[" + msg.nombre.ToString() + "]");
                        strErrores.Append("Valor[" + msg.valor.ToString() + "]");
                    }
                }

                strMensaje = strErrores.ToString();
            }
            else
            {
                strMensaje = "Factura no ha sido timbrada";
            }

            return false;
        }

        public static bool Cancelar_Nota_Cargo(int intNotaID, out string strMensaje)
        {
            strMensaje = string.Empty;

            CNotaCargoDB objNota_Cargo = new CNotaCargoDB();
            if (!objNota_Cargo.Leer(intNotaID))
            {
                strMensaje = objNota_Cargo.Mensaje;
                return false;
            }

            if (objNota_Cargo.fecha_timbrado.HasValue)
            {
                string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
                string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
                string strFact = "CFDI_" + objNota_Cargo.nota + ".xml";
                if (!File.Exists(strDirXML + "/" + strFact))
                {
                    strMensaje = "Archivo XML de la nota de cargo no existe";
                    return false;
                }

                HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT facturacion_prueba" +
                                 " FROM sistema_apps" +
                                 " WHERE ID = " + ckSIAN["ck_app"];

                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

                DataRow objRowResult = objDataResult.Tables[0].Rows[0];

                bool WSPrueba = (bool)objRowResult["facturacion_prueba"];

                XmlDocument xmlNota_Cargo = new XmlDocument();
                xmlNota_Cargo.Load(strDirXML + "/" + strFact);

                XmlNodeList xmlSello = xmlNota_Cargo.GetElementsByTagName("tfd:TimbreFiscalDigital");

                wsGenericResp timbreFiscalDigital = Cancelar_CFDI(xmlNota_Cargo,
                                                                  WSPrueba,
                                                                  out strMensaje);

                if (timbreFiscalDigital.isError)
                {
                    strMensaje = timbreFiscalDigital.errorMessage;
                }
                else
                {
                    System.Xml.XmlDocument xmlAcuse = new XmlDocument();
                    xmlAcuse.Load(new MemoryStream(timbreFiscalDigital.acuse));
                    xmlAcuse.Save(strDirXML + "/CFDI_" + xmlSello[0].Attributes["UUID"].Value + "_CANC.xml");
                    objNota_Cargo.fecha_cancelacion = DateTime.Now;
                    objNota_Cargo.status = 9;
                    objNota_Cargo.Guardar();
                    strMensaje = "Nota de cargo ha sido cancelada";
                    Log(strDirXML, "Nota de cargo cancelada: " + xmlSello[0].Attributes["UUID"].Value);
                    return true;
                }
            }
            else
            {
                strMensaje = "Nota de cargo no ha sido timbrada";
            }

            return false;
        }

        public static bool Cancelar_Nota_Credito(int intNotaID, out string strMensaje)
        {
            strMensaje = string.Empty;

            CNotaCreditoDB objNota_Credito = new CNotaCreditoDB();
            if (!objNota_Credito.Leer(intNotaID))
            {
                strMensaje = objNota_Credito.Mensaje;
                return false;
            }

            if (objNota_Credito.fecha_timbrado.HasValue)
            {
                string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
                string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
                string strFact = "CFDI_" + objNota_Credito.nota + ".xml";
                if (!File.Exists(strDirXML + "/" + strFact))
                {
                    strMensaje = "Archivo XML de la nota de crédito no existe";
                    return false;
                }

                HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT facturacion_prueba" +
                                 " FROM sistema_apps" +
                                 " WHERE ID = " + ckSIAN["ck_app"];

                objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

                DataRow objRowResult = objDataResult.Tables[0].Rows[0];

                bool WSPrueba = (bool)objRowResult["facturacion_prueba"];

                XmlDocument xmlNota_Credito = new XmlDocument();
                xmlNota_Credito.Load(strDirXML + "/" + strFact);

                XmlNodeList xmlSello = xmlNota_Credito.GetElementsByTagName("tfd:TimbreFiscalDigital");

                wsGenericResp timbreFiscalDigital = Cancelar_CFDI(xmlNota_Credito,
                                                                  WSPrueba,
                                                                  out strMensaje);

                if (timbreFiscalDigital.isError)
                {
                    strMensaje = timbreFiscalDigital.errorMessage;
                }
                else
                {
                    System.Xml.XmlDocument xmlAcuse = new XmlDocument();
                    xmlAcuse.Load(new MemoryStream(timbreFiscalDigital.acuse));
                    xmlAcuse.Save(strDirXML + "/CFDI_" + xmlSello[0].Attributes["UUID"].Value + "_CANC.xml");
                    objNota_Credito.fecha_cancelacion = DateTime.Now;
                    objNota_Credito.status = 9;
                    objNota_Credito.Guardar();
                    Log(strDirXML, "Nota de crédito cancelada: " + xmlSello[0].Attributes["UUID"].Value);
                    strMensaje = "Nota de crédito ha sido cancelada";
                    return true;
                }

            }
            else
            {
                strMensaje = "Nota de crédito no ha sido timbrada";
            }

            return false;
        }

        private static wsGenericResp Cancelar_CFDI(XmlDocument xmlFact,
                                                   bool WSPrueba,
                                                   out string strMensaje)
        {
            string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
            string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
            strMensaje = string.Empty;

            HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT Certificado, Certificado_Key, Certificado_Pwd" +
                              ",facturacion_prueba" +
                              " FROM sistema_apps" +
                              " WHERE ID = " + ckSIAN["ck_app"];

            objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

            byte[] btCertificado = FileToByteArray(strDirSAT + "/Certificados/" + objDataResult.Tables[0].Rows[0]["Certificado"].ToString());
            byte[] btCertificadoLlave = FileToByteArray(strDirSAT + "/Certificados/" + objDataResult.Tables[0].Rows[0]["Certificado_Key"].ToString());

            wsGenericResp timbreFiscalDigital = null;

            InterconectaWsClient timbradoWS;
            string strUser, strPwd;
            if (WSPrueba)
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
                strUser = ConfigurationManager.AppSettings["PACTestUser"];
                strPwd = ConfigurationManager.AppSettings["PACTestPwd"];
            }
            else
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");
                strUser = ConfigurationManager.AppSettings["PACUser"];
                strPwd = ConfigurationManager.AppSettings["PACPwd"];
            }

            if (WSPrueba)
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
            else
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");

            try
            {
                timbreFiscalDigital = timbradoWS.cancelaCFDI32(strUser,
                                                               strPwd,
                                                               btCertificado,
                                                               btCertificadoLlave,
                                                               objDataResult.Tables[0].Rows[0]["Certificado_Pwd"].ToString(),
                                                               xmlFact.OuterXml
                                                               );
            }
            catch (Exception ex)
            {
                strMensaje = ex.Message;
            }

            return timbreFiscalDigital;
        }

        private static wsGenericRespExt Reexpedir_CFDI(XmlDocument xmlFact,
                                                   bool WSPrueba,
                                                   out string strMensaje)
        {
            string strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas");
            string strDirSAT = System.Web.HttpContext.Current.Server.MapPath("../SAT");
            strMensaje = string.Empty;

            HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT Certificado, Certificado_Key, Certificado_Pwd" +
                              ",facturacion_prueba" +
                              " FROM sistema_apps" +
                              " WHERE ID = " + ckSIAN["ck_app"];

            objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

            byte[] btCertificado = FileToByteArray(strDirSAT + "/Certificados/" + objDataResult.Tables[0].Rows[0]["Certificado"].ToString());
            byte[] btCertificadoLlave = FileToByteArray(strDirSAT + "/Certificados/" + objDataResult.Tables[0].Rows[0]["Certificado_Key"].ToString());

            wsGenericRespExt timbreFiscalDigital = null;
            

            InterconectaWsClient timbradoWS;
            string strUser, strPwd;
            if (WSPrueba)
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
                strUser = ConfigurationManager.AppSettings["PACTestUser"];
                strPwd = ConfigurationManager.AppSettings["PACTestPwd"];
            }
            else
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");
                strUser = ConfigurationManager.AppSettings["PACUser"];
                strPwd = ConfigurationManager.AppSettings["PACPwd"];
            }

            if (WSPrueba)
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
            else
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");

            try
            {
                timbreFiscalDigital = timbradoWS.timbraEnviaCFDIReexpide(strUser,
                                                               strPwd,
                                                               btCertificado,
                                                               btCertificadoLlave,
                                                               objDataResult.Tables[0].Rows[0]["Certificado_Pwd"].ToString(),
                                                               xmlFact.OuterXml,
                                                               "3.2"
                                                               );
            }
            catch (Exception ex)
            {
                strMensaje = ex.Message;
            }

            return timbreFiscalDigital;
        }

        private static wsValidaResp Valida_Comprobante(XmlDocument xmlFact,
                                                   bool WSPrueba,
                                                   out string strMensaje)
        {
            strMensaje = string.Empty;
            wsValidaResp validacionCFDI = null;

            InterconectaWsClient timbradoWS;
            string strUser, strPwd;
            if (WSPrueba)
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
                strUser = ConfigurationManager.AppSettings["PACTestUser"];
                strPwd = ConfigurationManager.AppSettings["PACTestPwd"];
            }
            else
            {
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");
                strUser = ConfigurationManager.AppSettings["PACUser"];
                strPwd = ConfigurationManager.AppSettings["PACPwd"];
            }

            if (WSPrueba)
                timbradoWS = new InterconectaWsClient("InterconectaWsTESTPort");
            else
                timbradoWS = new InterconectaWsClient("InterconectaWsPort");


            try
            {
                validacionCFDI = timbradoWS.validaComprobante(strUser,
                                                               strPwd,
                                                               xmlFact.OuterXml,
                                                               Encoding.Default.GetBytes(xmlFact.OuterXml),
                                                               "3.2"
                                                               );
            }
            catch (Exception ex)
            {
                strMensaje = ex.Message;
            }

            return validacionCFDI;
        }

        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }         

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate,
                                               X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        private static void Log(string strDirXML, string strMensaje)
        {
            StreamWriter archLog;

            if (File.Exists(strDirXML + "/errores.log"))
                archLog = File.AppendText(strDirXML + "/errores.log");
            else
                archLog = new StreamWriter(strDirXML + "/errores.log");

            archLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + strMensaje);

            archLog.Close();
        }
    }

    public class CFacturaDB
    {
        public int ID { set; get; }
        public bool electronica { set; get; }
        public string factura { set; get; }
        public string factura_suf { set; get; }
        public DateTime fecha { set; get; }
        public DateTime? fecha_timbrado { set; get; }
        public bool contado { set; get; }
        public byte division { set; get; }
        public int control_carga_ID { set; get; }
        public int ruta_ID { set; get; }
        public int sucursal_ID { set; get; }
        public decimal monto_subtotal { set; get; }
        public decimal monto_descuento { set; get; }
        public decimal monto_iva { set; get; }
        public decimal monto_subtotal2 { set; get; }
        public decimal monto_isr_ret { set; get; }
        public decimal monto_iva_ret { set; get; }
        public decimal total { set; get; }
        public decimal descuento { set; get; }
        public decimal descuento2 { set; get; }
        public byte status { set; get; }
        public DateTime? fecha_contrarecibo { set; get; }
        public DateTime? fecha_cobranza { set; get; }
        public DateTime? fecha_pago { set; get; }
        public DateTime? fecha_cancelacion { set; get; }
        public decimal total_cajas { set; get; }
        public string comentarios { set; get; }
        public decimal total_real { set; get; }
        public decimal total_sin_nota { set; get; }
        public decimal iva { set; get; }
        public bool liquidada { set; get; }
        public byte tipo_factura { set; get; }
        public DateTime start_date { set; get; }
        public DateTime end_date { set; get; }
        public int lista_precios_ID { set; get; }
        public decimal isr_ret { set; get; }
        public decimal iva_ret { set; get; }
        public int vendedorID { set; get; }
        public string orden_compra { set; get; }
        public string referencia { set; get; }
        public int creadoPorID { set; get; }
        public DateTime? creadoPorFecha { set; get; }
        public bool registrarMod { set; get; }
        public int modificadoPorID { set; get; }
        public DateTime? modificadoPorFecha { set; get; }
        private string mensaje;

        public string Mensaje
        {
            get { return this.mensaje; }
        }

        public bool Guardar()
        {
            this.mensaje = string.Empty;
            if (this.ID == 0)
                return Crear();
            else
                return Actualizar();
        }

        private bool Crear()
        {
            bool swGuid = false;

            if (string.IsNullOrEmpty(this.factura))
            {
                Guid guidID = System.Guid.NewGuid();
                this.factura = guidID.ToString();
                swGuid = true;
            }

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("INSERT INTO facturas_liq (");
            strQuery.Append("electronica");
            strQuery.Append(",factura");
            strQuery.Append(",factura_suf");
            strQuery.Append(",fecha");
            if(this.fecha_timbrado.HasValue)
                strQuery.Append(",fecha_timbrado");
            strQuery.Append(",contado");
            strQuery.Append(",division");
            strQuery.Append(",control_carga_ID");
            strQuery.Append(",ruta_ID");
            strQuery.Append(",sucursal_ID");
            strQuery.Append(",monto_subtotal");
            strQuery.Append(",monto_descuento");
            strQuery.Append(",monto_iva");
            strQuery.Append(",monto_subtotal2");
            strQuery.Append(",monto_isr_ret");
            strQuery.Append(",monto_iva_ret");
            strQuery.Append(",total");
            strQuery.Append(",descuento");
            strQuery.Append(",descuento2");
            strQuery.Append(",status");
            if(this.fecha_contrarecibo.HasValue)
                strQuery.Append(",fecha_contrarecibo");
            if(this.fecha_cobranza.HasValue)
                strQuery.Append(",fecha_cobranza");
            if(this.fecha_pago.HasValue)
                strQuery.Append(",fecha_pago");
            if(this.fecha_cancelacion.HasValue)
                strQuery.Append(",fecha_cancelacion");
            strQuery.Append(",total_cajas");
            strQuery.Append(",comentarios");
            strQuery.Append(",total_real");
            strQuery.Append(",total_sin_nota");
            strQuery.Append(",iva");
            strQuery.Append(",liquidada");
            strQuery.Append(",tipo_factura");
            strQuery.Append(",start_date");
            strQuery.Append(",end_date");
            strQuery.Append(",lista_precios_ID");
            strQuery.Append(",isr_ret");
            strQuery.Append(",iva_ret");
            strQuery.Append(",vendedorID");
            strQuery.Append(",orden_compra");
            strQuery.Append(",referencia");
            strQuery.Append(",creadoPorID");
            strQuery.Append(",creadoPorFecha");
            strQuery.Append(",modificadoPorID");
            strQuery.Append(",modificadoPorFecha");
            strQuery.Append(") VALUES(");
            strQuery.Append("'" + (this.electronica ? "1" : "0") + "'");
            strQuery.Append(",'" + this.factura + "'");
            strQuery.Append(",'" + this.factura_suf + "'");
            strQuery.Append(",'" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(",'" + this.fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + (this.contado ? "1" : "0") + "'");
            strQuery.Append(",'" + this.division.ToString() + "'");
            strQuery.Append(",'" + this.control_carga_ID.ToString() + "'");
            strQuery.Append(",'" + this.ruta_ID.ToString() + "'");
            strQuery.Append(",'" + this.sucursal_ID.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal.ToString() + "'");
            strQuery.Append(",'" + this.monto_descuento.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal2.ToString() + "'");
            strQuery.Append(",'" + this.monto_isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.total.ToString() + "'");
            strQuery.Append(",'" + this.descuento.ToString() + "'");
            strQuery.Append(",'" + this.descuento2.ToString() + "'");
            strQuery.Append(",'" + this.status.ToString() + "'");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",'" + this.fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",'" + this.fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",'" + this.fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",'" + this.fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");
            strQuery.Append(",'" + this.total_cajas.ToString() + "'");
            strQuery.Append(",'" + this.comentarios.ToString() + "'");
            strQuery.Append(",'" + this.total_real.ToString() + "'");
            strQuery.Append(",'" + this.total_sin_nota.ToString() + "'");
            strQuery.Append(",'" + this.iva.ToString() + "'");
            strQuery.Append(",'" + (this.liquidada ? "1" : "0") + "'");
            strQuery.Append(",'" + this.tipo_factura.ToString() + "'");
            strQuery.Append(",'" + this.start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.lista_precios_ID.ToString() + "'");
            strQuery.Append(",'" + this.isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.vendedorID.ToString() + "'");
            strQuery.Append(",'" + this.orden_compra.ToString() + "'");
            strQuery.Append(",'" + this.referencia.ToString() + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(")");

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al generar la factura en la base de datos [{0}]", ex.Message);
                return false;
            }

            strQuery.Length = 0;
            strQuery.Append("SELECT ID " +
                    " FROM facturas_liq " +
                    " WHERE fecha = '" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "   AND factura = '" + this.factura + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la factura generada [{0}]", ex.Message);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            this.ID = (int)objRowResult["ID"];

            if (swGuid)
            {
                strQuery.Clear();
                strQuery = new StringBuilder("UPDATE facturas_liq SET ");
                strQuery.Append(" factura = ''");
                strQuery.Append(" WHERE ID = " + this.ID.ToString());
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }

            return true;
        }

        private bool Actualizar()
        {
            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("UPDATE facturas_liq SET ");
            strQuery.Append(" electronica = " + (this.electronica ? "1" : "0"));
            strQuery.Append(", factura = '" + factura + "'");
            strQuery.Append(", factura_suf = '" + factura_suf + "'");
            strQuery.Append(", fecha = '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(", fecha_timbrado = '" + fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", contado = " + (this.contado ? "1" : "0"));
            strQuery.Append(", division = " + division.ToString());
            strQuery.Append(", control_carga_ID = " + control_carga_ID.ToString());
            strQuery.Append(", ruta_ID = " + ruta_ID.ToString());
            strQuery.Append(", sucursal_ID = " + sucursal_ID.ToString());
            strQuery.Append(", monto_subtotal = " + monto_subtotal.ToString());
            strQuery.Append(", monto_descuento = " + monto_descuento.ToString());
            strQuery.Append(", monto_iva = " + monto_iva.ToString());
            strQuery.Append(", monto_subtotal2 = " + monto_subtotal2.ToString());
            strQuery.Append(", monto_isr_ret = " + monto_isr_ret.ToString());
            strQuery.Append(", monto_iva_ret = " + monto_iva_ret.ToString());
            strQuery.Append(", total = " + total.ToString());
            strQuery.Append(", descuento = " + descuento.ToString());
            strQuery.Append(", descuento2 = " + descuento2.ToString());
            strQuery.Append(", status = " + status.ToString());
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(", fecha_contrarecibo = '" + fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(", fecha_cobranza = '" + fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(", fecha_pago = '" + fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(", fecha_cancelacion = '" + fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", total_cajas = " + total_cajas.ToString());
            strQuery.Append(", comentarios = '" + comentarios + "'");
            strQuery.Append(", total_real = " + total_real.ToString());
            strQuery.Append(", total_sin_nota = " + total_sin_nota.ToString());
            strQuery.Append(", iva = " + iva.ToString());
            strQuery.Append(", liquidada = " + (this.liquidada ? "1" : "0"));
            strQuery.Append(", tipo_factura = " + tipo_factura.ToString());
            strQuery.Append(", start_date = '" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", end_date = '" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", lista_precios_ID = " + lista_precios_ID.ToString());
            strQuery.Append(", isr_ret = " + isr_ret.ToString());
            strQuery.Append(", iva_ret = " + iva_ret.ToString());
            strQuery.Append(", vendedorID = " + vendedorID.ToString());
            strQuery.Append(", orden_compra = '" + orden_compra.ToString() + "'");
            strQuery.Append(", referencia = '" + referencia.ToString() + "'");
            if (registrarMod)
            {
                strQuery.Append(", modificadoPorID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString());
                strQuery.Append(", modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            }
            strQuery.Append(" WHERE ID = " + this.ID.ToString());

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al actualizar la factura en la base de datos [{0}]: {1}", this.ID, ex.Message);
                return false;
            }

            return true;
        }

        public bool Leer(int intID)
        {
            this.ID = 0;
            this.mensaje = string.Empty;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT * " +
                    " FROM facturas_liq " +
                    " WHERE ID = " + intID.ToString();
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                this.mensaje = String.Format("Error al leer la factura ID[{0}]: {1}", intID, ex.Message);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                this.mensaje = String.Format("No existe la factura ID[{0}]", intID);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            ID = intID;
            electronica = Convert.ToBoolean(objRowResult["electronica"]);
            factura = objRowResult["factura"].ToString();
            factura_suf = objRowResult["factura_suf"].ToString();
            fecha = (DateTime)objRowResult["fecha"];
            if (objRowResult.IsNull("fecha_timbrado"))
                fecha_timbrado = null;
            else
                fecha_timbrado = (DateTime)objRowResult["fecha_timbrado"];
            contado = Convert.ToBoolean(objRowResult["contado"]);
            division = (byte)objRowResult["division"];
            control_carga_ID = (int)objRowResult["control_carga_ID"];
            ruta_ID = (int)objRowResult["ruta_ID"];
            sucursal_ID = (int)objRowResult["sucursal_ID"];
            monto_subtotal = (decimal)objRowResult["monto_subtotal"];
            monto_descuento = (decimal)objRowResult["monto_descuento"];
            monto_iva = (decimal)objRowResult["monto_iva"];
            monto_subtotal2 = (decimal)objRowResult["monto_subtotal2"];
            monto_isr_ret = (decimal)objRowResult["monto_isr_ret"];
            monto_iva_ret = (decimal)objRowResult["monto_iva_ret"];
            total = (decimal)objRowResult["total"];
            descuento = (decimal)objRowResult["descuento"];
            descuento2 = (decimal)objRowResult["descuento2"];
            status = (byte)objRowResult["status"];
            if (objRowResult.IsNull("fecha_contrarecibo"))
                fecha_contrarecibo = null;
            else
                fecha_contrarecibo = (DateTime)objRowResult["fecha_contrarecibo"];
            if (objRowResult.IsNull("fecha_cobranza"))
                fecha_cobranza = null;
            else
                fecha_cobranza = (DateTime)objRowResult["fecha_cobranza"];
            if (objRowResult.IsNull("fecha_pago"))
                fecha_pago = null;
            else
                fecha_pago = (DateTime)objRowResult["fecha_pago"];
            if (objRowResult.IsNull("fecha_cancelacion"))
                fecha_cancelacion = null;
            else
                fecha_cancelacion = (DateTime)objRowResult["fecha_cancelacion"];
            total_cajas = (decimal)objRowResult["total_cajas"];
            comentarios = objRowResult["comentarios"].ToString();
            total_real = (decimal)objRowResult["total_real"];
            total_sin_nota = (decimal)objRowResult["total_sin_nota"];
            iva = (decimal)objRowResult["iva"];
            liquidada = Convert.ToBoolean(objRowResult["liquidada"]);
            tipo_factura = (byte)objRowResult["tipo_factura"];
            start_date = (DateTime)objRowResult["start_date"];
            end_date = (DateTime)objRowResult["end_date"];
            lista_precios_ID = (int)objRowResult["lista_precios_ID"];
            isr_ret = (decimal)objRowResult["isr_ret"];
            iva_ret = (decimal)objRowResult["iva_ret"];
            vendedorID = (int)objRowResult["vendedorID"];
            orden_compra = objRowResult["orden_compra"].ToString();
            referencia = objRowResult["referencia"].ToString();
            creadoPorID = (int)objRowResult["creadoPorID"];
            if (objRowResult.IsNull("creadoPorFecha"))
                creadoPorFecha = null;
            else
                creadoPorFecha = (DateTime)objRowResult["creadoPorFecha"];
            modificadoPorID = (int)objRowResult["modificadoPorID"];
            if (objRowResult.IsNull("modificadoPorFecha"))
                modificadoPorFecha = null;
            else
                modificadoPorFecha = (DateTime)objRowResult["modificadoPorFecha"];
            return true;
        }

        public CFacturaDB()
        {
            ID = 0;
            electronica = false;
            factura = string.Empty;
            factura_suf = string.Empty;
            fecha = DateTime.Now;
            fecha_timbrado = null;
            contado = false;
            division = 0;
            control_carga_ID = 0;
            ruta_ID = 0;
            sucursal_ID = 0;
            monto_subtotal = 0;
            monto_descuento = 0;
            monto_iva = 0;
            monto_subtotal2 = 0;
            monto_isr_ret = 0;
            monto_iva_ret = 0;
            total = 0;
            descuento = 0;
            descuento2 = 0;
            status = 0;
            fecha_contrarecibo = null;
            fecha_cobranza = null;
            fecha_pago = null;
            fecha_cancelacion = null;
            total_cajas = 0;
            comentarios = string.Empty;
            total_real = 0;
            total_sin_nota = 0;
            iva = 0;
            liquidada = false;
            tipo_factura = 0;
            start_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            end_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            lista_precios_ID = 0;
            isr_ret = 0;
            iva_ret = 0;
            vendedorID = 0;
            orden_compra = string.Empty;
            referencia = string.Empty;
            creadoPorID = 0;
            creadoPorFecha = null;
            registrarMod = true;
            modificadoPorID = 0;
            modificadoPorFecha = null;
            mensaje = string.Empty;
        }
    }

    public class CNotaDB
    {
        public int ID { set; get; }
        public string nota { set; get; }
        public string nota_suf { set; get; }
        public bool contado { set; get; }
        public DateTime fecha { set; get; }
        public byte division { set; get; }
        public int control_carga_ID { set; get; }
        public int ruta_ID { set; get; }
        public int sucursal_ID { set; get; }
        public decimal monto_subtotal { set; get; }
        public decimal monto_descuento { set; get; }
        public decimal monto_iva { set; get; }
        public decimal monto_subtotal2 { set; get; }
        public decimal monto_isr_ret { set; get; }
        public decimal monto_iva_ret { set; get; }
        public decimal total { set; get; }
        public decimal descuento { set; get; }
        public decimal descuento2 { set; get; }
        public byte status { set; get; }
        public DateTime? fecha_contrarecibo { set; get; }
        public DateTime? fecha_cobranza { set; get; }
        public DateTime? fecha_pago { set; get; }
        public DateTime? fecha_cancelacion { set; get; }
        public decimal total_cajas { set; get; }
        public string comentarios { set; get; }
        public string dirigidoa { set; get; }
        public decimal total_real { set; get; }
        public decimal total_sin_nota { set; get; }
        public decimal iva { set; get; }
        public DateTime start_date { set; get; }
        public DateTime end_date { set; get; }
        public int lista_precios_ID { set; get; }
        public decimal isr_ret { set; get; }
        public decimal iva_ret { set; get; }
        public int vendedorID { set; get; }
        public string orden_compra { set; get; }
        public int creadoPorID { set; get; }
        public DateTime? creadoPorFecha { set; get; }
        public bool registrarMod { set; get; }
        public int modificadoPorID { set; get; }
        public DateTime? modificadoPorFecha { set; get; }
        private string mensaje;

        public string Mensaje
        {
            get { return this.mensaje; }
        }

        public bool Guardar()
        {
            this.mensaje = string.Empty;
            if (this.ID == 0)
                return Crear();
            else
                return Actualizar();
        }

        private bool Existe()
        {
            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("SELECT ID " +
                    " FROM notas " +
                    " WHERE nota = '" + this.nota + "'" +
                    " AND nota_suf = '" + this.nota_suf + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota [{0}]", ex.Message);
                return true;
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                this.mensaje = String.Format("Nota ya existe");
                return true;
            }
            else
                return false;
        }

        private bool Crear()
        {
            if (string.IsNullOrEmpty(this.nota))
            {
                Guid guidID = System.Guid.NewGuid();
                this.nota = guidID.ToString();
            }

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("INSERT INTO notas (");
            strQuery.Append("nota");
            strQuery.Append(",nota_suf");
            strQuery.Append(",fecha");
            strQuery.Append(",contado");
            strQuery.Append(",division");
            strQuery.Append(",control_carga_ID");
            strQuery.Append(",ruta_ID");
            strQuery.Append(",sucursal_ID");
            strQuery.Append(",monto_subtotal");
            strQuery.Append(",monto_descuento");
            strQuery.Append(",monto_iva");
            strQuery.Append(",monto_subtotal2");
            strQuery.Append(",monto_isr_ret");
            strQuery.Append(",monto_iva_ret");
            strQuery.Append(",total");
            strQuery.Append(",descuento");
            strQuery.Append(",descuento2");
            strQuery.Append(",status");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",fecha_contrarecibo");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",fecha_cobranza");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",fecha_pago");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",fecha_cancelacion");
            strQuery.Append(",total_cajas");
            strQuery.Append(",comentarios");
            strQuery.Append(",dirigidoa");
            strQuery.Append(",total_real");
            strQuery.Append(",total_sin_nota");
            strQuery.Append(",iva");
            strQuery.Append(",start_date");
            strQuery.Append(",end_date");
            strQuery.Append(",lista_precios_ID");
            strQuery.Append(",isr_ret");
            strQuery.Append(",iva_ret");
            strQuery.Append(",vendedorID");
            strQuery.Append(",orden_compra");
            strQuery.Append(",creadoPorID");
            strQuery.Append(",creadoPorFecha");
            strQuery.Append(",modificadoPorID");
            strQuery.Append(",modificadoPorFecha");
            strQuery.Append(") VALUES(");
            strQuery.Append("'" + this.nota + "'");
            strQuery.Append(",'" + this.nota_suf + "'");
            strQuery.Append(",'" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + (this.contado ? "1" : "0") + "'");
            strQuery.Append(",'" + this.division.ToString() + "'");
            strQuery.Append(",'" + this.control_carga_ID.ToString() + "'");
            strQuery.Append(",'" + this.ruta_ID.ToString() + "'");
            strQuery.Append(",'" + this.sucursal_ID.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal.ToString() + "'");
            strQuery.Append(",'" + this.monto_descuento.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal2.ToString() + "'");
            strQuery.Append(",'" + this.monto_isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.total.ToString() + "'");
            strQuery.Append(",'" + this.descuento.ToString() + "'");
            strQuery.Append(",'" + this.descuento2.ToString() + "'");
            strQuery.Append(",'" + this.status.ToString() + "'");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",'" + this.fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",'" + this.fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",'" + this.fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",'" + this.fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.total_cajas.ToString() + "'");
            strQuery.Append(",'" + this.comentarios.ToString() + "'");
            strQuery.Append(",'" + this.dirigidoa.ToString() + "'");
            strQuery.Append(",'" + this.total_real.ToString() + "'");
            strQuery.Append(",'" + this.total_sin_nota.ToString() + "'");
            strQuery.Append(",'" + this.iva.ToString() + "'");
            strQuery.Append(",'" + this.start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.lista_precios_ID.ToString() + "'");
            strQuery.Append(",'" + this.isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.vendedorID.ToString() + "'");
            strQuery.Append(",'" + this.orden_compra.ToString() + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(")");

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al generar la nota en la base de datos [{0}]", ex.Message);
                return false;
            }

            strQuery.Length = 0;
            strQuery.Append("SELECT ID " +
                    " FROM notas " +
                    " WHERE fecha = '" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "   AND nota = '" + this.nota + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota generada [{0}]", ex.Message);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            this.ID = (int)objRowResult["ID"];
            this.nota = this.ID.ToString();

            if (this.status == 1 || this.status == 2)
                Calcular_Fecha_Cobranza();

            Actualizar();
            return true;
        }

        private void Calcular_Fecha_Cobranza()
        {
            DateTime? dtFechaCobranza = null;
            if (this.status == 1)
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNota(1, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_contrarecibo = dtFechaCobranza.Value;
                else
                {
                    this.status = 2;
                    // Busca fecha de cobro
                    dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNota(2, this.ID);
                    if (dtFechaCobranza.HasValue)
                        this.fecha_cobranza = dtFechaCobranza.Value;
                    else
                        this.fecha_cobranza = DateTime.Today;
                }
            }
            else
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNota(2, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_cobranza = dtFechaCobranza.Value;
                else
                    this.fecha_cobranza = DateTime.Today;
            }
        }

        private bool Actualizar()
        {
            if (this.status == 1 || this.status == 2)
                Calcular_Fecha_Cobranza();

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("UPDATE notas SET ");
            strQuery.Append("nota = '" + nota + "'");
            strQuery.Append(", nota_suf = '" + nota_suf + "'");
            strQuery.Append(", fecha = '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", contado = " + (this.contado ? "1" : "0"));
            strQuery.Append(", division = " + division.ToString());
            strQuery.Append(", control_carga_ID = " + control_carga_ID.ToString());
            strQuery.Append(", ruta_ID = " + ruta_ID.ToString());
            strQuery.Append(", sucursal_ID = " + sucursal_ID.ToString());
            strQuery.Append(", monto_subtotal = " + monto_subtotal.ToString());
            strQuery.Append(", monto_descuento = " + monto_descuento.ToString());
            strQuery.Append(", monto_iva = " + monto_iva.ToString());
            strQuery.Append(", monto_subtotal2 = " + monto_subtotal2.ToString());
            strQuery.Append(", monto_isr_ret = " + monto_isr_ret.ToString());
            strQuery.Append(", monto_iva_ret = " + monto_iva_ret.ToString());
            strQuery.Append(", total = " + total.ToString());
            strQuery.Append(", descuento = " + descuento.ToString());
            strQuery.Append(", descuento2 = " + descuento2.ToString());
            strQuery.Append(", status = " + status.ToString());
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(", fecha_contrarecibo = '" + fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(", fecha_cobranza = '" + fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(", fecha_pago = '" + fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(", fecha_cancelacion = '" + fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", total_cajas = " + total_cajas.ToString());
            strQuery.Append(", comentarios = '" + comentarios + "'");
            strQuery.Append(", dirigidoa = '" + dirigidoa + "'");
            strQuery.Append(", total_real = " + total_real.ToString());
            strQuery.Append(", total_sin_nota = " + total_sin_nota.ToString());
            strQuery.Append(", iva = " + iva.ToString());
            strQuery.Append(", start_date = '" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", end_date = '" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", lista_precios_ID = " + lista_precios_ID.ToString());
            strQuery.Append(", isr_ret = " + isr_ret.ToString());
            strQuery.Append(", iva_ret = " + iva_ret.ToString());
            strQuery.Append(", vendedorID = " + vendedorID.ToString());
            strQuery.Append(", orden_compra = '" + orden_compra.ToString() + "'");
            if (registrarMod)
            {
                strQuery.Append(", modificadoPorID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString());
                strQuery.Append(", modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            }
            strQuery.Append(" WHERE ID = " + this.ID.ToString());

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al actualizar la factura en la base de datos [{0}]: {1}", this.ID, ex.Message);
                return false;
            }

            return true;
        }

        public bool Leer(int intID)
        {
            this.ID = 0;
            this.mensaje = string.Empty;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT * " +
                    " FROM notas " +
                    " WHERE ID = " + intID.ToString();
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                this.mensaje = String.Format("Error al leer la nota ID[{0}]: {1}", intID, ex.Message);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                this.mensaje = String.Format("No existe la factura ID[{0}]", intID);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            ID = intID;
            nota = objRowResult["nota"].ToString();
            nota_suf = objRowResult["nota_suf"].ToString();
            fecha = (DateTime)objRowResult["fecha"];
            contado = Convert.ToBoolean(objRowResult["contado"]);
            division = (byte)objRowResult["division"];
            control_carga_ID = (int)objRowResult["control_carga_ID"];
            ruta_ID = (int)objRowResult["ruta_ID"];
            sucursal_ID = (int)objRowResult["sucursal_ID"];
            monto_subtotal = (decimal)objRowResult["monto_subtotal"];
            monto_descuento = (decimal)objRowResult["monto_descuento"];
            monto_iva = (decimal)objRowResult["monto_iva"];
            monto_subtotal2 = (decimal)objRowResult["monto_subtotal2"];
            monto_isr_ret = (decimal)objRowResult["monto_isr_ret"];
            monto_iva_ret = (decimal)objRowResult["monto_iva_ret"];
            total = (decimal)objRowResult["total"];
            descuento = (decimal)objRowResult["descuento"];
            descuento2 = (decimal)objRowResult["descuento2"];
            status = (byte)objRowResult["status"];
            if (objRowResult.IsNull("fecha_contrarecibo"))
                fecha_contrarecibo = null;
            else
                fecha_contrarecibo = (DateTime)objRowResult["fecha_contrarecibo"];
            if (objRowResult.IsNull("fecha_cobranza"))
                fecha_cobranza = null;
            else
                fecha_cobranza = (DateTime)objRowResult["fecha_cobranza"];
            if (objRowResult.IsNull("fecha_pago"))
                fecha_pago = null;
            else
                fecha_pago = (DateTime)objRowResult["fecha_pago"];
            if (objRowResult.IsNull("fecha_cancelacion"))
                fecha_cancelacion = null;
            else
                fecha_cancelacion = (DateTime)objRowResult["fecha_cancelacion"];
            total_cajas = (decimal)objRowResult["total_cajas"];
            comentarios = objRowResult["comentarios"].ToString();
            dirigidoa = objRowResult["dirigidoa"].ToString();
            total_real = (decimal)objRowResult["total_real"];
            total_sin_nota = (decimal)objRowResult["total_sin_nota"];
            iva = (decimal)objRowResult["iva"];
            start_date = (DateTime)objRowResult["start_date"];
            end_date = (DateTime)objRowResult["end_date"];
            lista_precios_ID = (int)objRowResult["lista_precios_ID"];
            isr_ret = (decimal)objRowResult["isr_ret"];
            iva_ret = (decimal)objRowResult["iva_ret"];
            vendedorID = (int)objRowResult["vendedorID"];
            orden_compra = objRowResult["orden_compra"].ToString();
            creadoPorID = (int)objRowResult["creadoPorID"];
            if (objRowResult.IsNull("creadoPorFecha"))
                creadoPorFecha = null;
            else
                creadoPorFecha = (DateTime)objRowResult["creadoPorFecha"];
            modificadoPorID = (int)objRowResult["modificadoPorID"];
            if (objRowResult.IsNull("modificadoPorFecha"))
                modificadoPorFecha = null;
            else
                modificadoPorFecha = (DateTime)objRowResult["modificadoPorFecha"];

            return true;
        }

        public CNotaDB()
        {
            ID = 0;
            nota = string.Empty;
            nota_suf = string.Empty;
            fecha = DateTime.Now;
            contado = false;
            division = 0;
            control_carga_ID = 0;
            ruta_ID = 0;
            sucursal_ID = 0;
            monto_subtotal = 0;
            monto_descuento = 0;
            monto_iva = 0;
            monto_subtotal2 = 0;
            monto_isr_ret = 0;
            monto_iva_ret = 0;
            total = 0;
            descuento = 0;
            descuento2 = 0;
            status = 0;
            fecha_contrarecibo = null;
            fecha_cobranza = null;
            fecha_pago = null;
            fecha_cancelacion = null;
            total_cajas = 0;
            comentarios = string.Empty;
            dirigidoa = string.Empty;
            total_real = 0;
            total_sin_nota = 0;
            iva = 0;
            start_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            end_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            lista_precios_ID = 0;
            isr_ret = 0;
            iva_ret = 0;
            vendedorID = 0;
            orden_compra = string.Empty;
            creadoPorID = 0;
            creadoPorFecha = null;
            registrarMod = true;
            modificadoPorID = 0;
            modificadoPorFecha = null;
            mensaje = string.Empty;
        }
    }

    public class CNotaCargoDB
    {
        public int ID { set; get; }
        public string nota { set; get; }
        public string nota_suf { set; get; }
        public bool contado { set; get; }
        public DateTime fecha { set; get; }
        public DateTime? fecha_timbrado { set; get; }
        public byte division { set; get; }
        public int control_carga_ID { set; get; }
        public int ruta_ID { set; get; }
        public int sucursal_ID { set; get; }
        public decimal monto_subtotal { set; get; }
        public decimal monto_descuento { set; get; }
        public decimal monto_iva { set; get; }
        public decimal monto_subtotal2 { set; get; }
        public decimal monto_isr_ret { set; get; }
        public decimal monto_iva_ret { set; get; }
        public decimal total { set; get; }
        public decimal descuento { set; get; }
        public decimal descuento2 { set; get; }
        public byte status { set; get; }
        public DateTime? fecha_contrarecibo { set; get; }
        public DateTime? fecha_cobranza { set; get; }
        public DateTime? fecha_pago { set; get; }
        public DateTime? fecha_cancelacion { set; get; }
        public decimal total_cajas { set; get; }
        public string comentarios { set; get; }
        public decimal total_real { set; get; }
        public decimal total_sin_nota { set; get; }
        public decimal iva { set; get; }
        public DateTime start_date { set; get; }
        public DateTime end_date { set; get; }
        public int lista_precios_ID { set; get; }
        public decimal isr_ret { set; get; }
        public decimal iva_ret { set; get; }
        public int vendedorID { set; get; }
        public int creadoPorID { set; get; }
        public DateTime? creadoPorFecha { set; get; }
        public bool registrarMod { set; get; }
        public int modificadoPorID { set; get; }
        public DateTime? modificadoPorFecha { set; get; }
        private string mensaje;

        public string Mensaje
        {
            get { return this.mensaje; }
        }

        public bool Guardar()
        {
            this.mensaje = string.Empty;
            if (this.ID == 0)
                return Crear();
            else
                return Actualizar();
        }

        private bool Existe()
        {
            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("SELECT ID " +
                    " FROM notas_cargo " +
                    " WHERE nota = '" + this.nota + "'" +
                    " AND nota_suf = '" + this.nota_suf + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota [{0}]", ex.Message);
                return true;
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                this.mensaje = String.Format("Nota ya existe");
                return true;
            }
            else
                return false;
        }

        private bool Crear()
        {
            if (string.IsNullOrEmpty(this.nota))
            {
                Guid guidID = System.Guid.NewGuid();
                this.nota = guidID.ToString();
            }

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("INSERT INTO notas_cargo (");
            strQuery.Append("nota");
            strQuery.Append(",nota_suf");
            strQuery.Append(",fecha");
            if(this.fecha_timbrado.HasValue)
                strQuery.Append(",fecha_timbrado");
            strQuery.Append(",contado");
            strQuery.Append(",division");
            strQuery.Append(",control_carga_ID");
            strQuery.Append(",ruta_ID");
            strQuery.Append(",sucursal_ID");
            strQuery.Append(",monto_subtotal");
            strQuery.Append(",monto_descuento");
            strQuery.Append(",monto_iva");
            strQuery.Append(",monto_subtotal2");
            strQuery.Append(",monto_isr_ret");
            strQuery.Append(",monto_iva_ret");
            strQuery.Append(",total");
            strQuery.Append(",descuento");
            strQuery.Append(",descuento2");
            strQuery.Append(",status");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",fecha_contrarecibo");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",fecha_cobranza");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",fecha_pago");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",fecha_cancelacion");
            strQuery.Append(",total_cajas");
            strQuery.Append(",comentarios");
            strQuery.Append(",total_real");
            strQuery.Append(",total_sin_nota");
            strQuery.Append(",iva");
            strQuery.Append(",start_date");
            strQuery.Append(",end_date");
            strQuery.Append(",lista_precios_ID");
            strQuery.Append(",isr_ret");
            strQuery.Append(",iva_ret");
            strQuery.Append(",vendedorID");
            strQuery.Append(",creadoPorID");
            strQuery.Append(",creadoPorFecha");
            strQuery.Append(",modificadoPorID");
            strQuery.Append(",modificadoPorFecha");
            strQuery.Append(") VALUES(");
            strQuery.Append("'" + this.nota + "'");
            strQuery.Append(",'" + this.nota_suf + "'");
            strQuery.Append(",'" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(",'" + this.fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + (this.contado ? "1" : "0") + "'");
            strQuery.Append(",'" + this.division.ToString() + "'");
            strQuery.Append(",'" + this.control_carga_ID.ToString() + "'");
            strQuery.Append(",'" + this.ruta_ID.ToString() + "'");
            strQuery.Append(",'" + this.sucursal_ID.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal.ToString() + "'");
            strQuery.Append(",'" + this.monto_descuento.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal2.ToString() + "'");
            strQuery.Append(",'" + this.monto_isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.total.ToString() + "'");
            strQuery.Append(",'" + this.descuento.ToString() + "'");
            strQuery.Append(",'" + this.descuento2.ToString() + "'");
            strQuery.Append(",'" + this.status.ToString() + "'");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",'" + this.fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",'" + this.fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",'" + this.fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",'" + this.fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.total_cajas.ToString() + "'");
            strQuery.Append(",'" + this.comentarios.ToString() + "'");
            strQuery.Append(",'" + this.total_real.ToString() + "'");
            strQuery.Append(",'" + this.total_sin_nota.ToString() + "'");
            strQuery.Append(",'" + this.iva.ToString() + "'");
            strQuery.Append(",'" + this.start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.lista_precios_ID.ToString() + "'");
            strQuery.Append(",'" + this.isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.vendedorID.ToString() + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(")");

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al generar la nota en la base de datos [{0}]", ex.Message);
                return false;
            }

            strQuery.Length = 0;
            strQuery.Append("SELECT ID " +
                    " FROM notas_cargo " +
                    " WHERE fecha = '" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "   AND nota = '" + this.nota + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota generada [{0}]", ex.Message);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            this.ID = (int)objRowResult["ID"];
            this.nota = this.ID.ToString();

            if (this.status == 1 || this.status == 2)
                Calcular_Fecha_Cobranza();

            Actualizar();
            return true;
        }

        private void Calcular_Fecha_Cobranza()
        {
            DateTime? dtFechaCobranza = null;
            if (this.status == 1)
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(1, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_contrarecibo = dtFechaCobranza.Value;
                else
                {
                    this.status = 2;
                    // Busca fecha de cobro
                    dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(2, this.ID);
                    if (dtFechaCobranza.HasValue)
                        this.fecha_cobranza = dtFechaCobranza.Value;
                    else
                        this.fecha_cobranza = DateTime.Today;
                }
            }
            else
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(2, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_cobranza = dtFechaCobranza.Value;
                else
                    this.fecha_cobranza = DateTime.Today;
            }
        }

        private bool Actualizar()
        {
            if (this.status == 1 || this.status == 2)
                Calcular_Fecha_Cobranza();

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("UPDATE notas_cargo SET ");
            strQuery.Append("nota = '" + nota + "'");
            strQuery.Append(", nota_suf = '" + nota_suf + "'");
            strQuery.Append(", fecha = '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(", fecha_timbrado = '" + fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", contado = " + (this.contado ? "1" : "0"));
            strQuery.Append(", division = " + division.ToString());
            strQuery.Append(", control_carga_ID = " + control_carga_ID.ToString());
            strQuery.Append(", ruta_ID = " + ruta_ID.ToString());
            strQuery.Append(", sucursal_ID = " + sucursal_ID.ToString());
            strQuery.Append(", monto_subtotal = " + monto_subtotal.ToString());
            strQuery.Append(", monto_descuento = " + monto_descuento.ToString());
            strQuery.Append(", monto_iva = " + monto_iva.ToString());
            strQuery.Append(", monto_subtotal2 = " + monto_subtotal2.ToString());
            strQuery.Append(", monto_isr_ret = " + monto_isr_ret.ToString());
            strQuery.Append(", monto_iva_ret = " + monto_iva_ret.ToString());
            strQuery.Append(", total = " + total.ToString());
            strQuery.Append(", descuento = " + descuento.ToString());
            strQuery.Append(", descuento2 = " + descuento2.ToString());
            strQuery.Append(", status = " + status.ToString());
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(", fecha_contrarecibo = '" + fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(", fecha_cobranza = '" + fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(", fecha_pago = '" + fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(", fecha_cancelacion = '" + fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", total_cajas = " + total_cajas.ToString());
            strQuery.Append(", comentarios = '" + comentarios + "'");
            strQuery.Append(", total_real = " + total_real.ToString());
            strQuery.Append(", total_sin_nota = " + total_sin_nota.ToString());
            strQuery.Append(", iva = " + iva.ToString());
            strQuery.Append(", start_date = '" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", end_date = '" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", lista_precios_ID = " + lista_precios_ID.ToString());
            strQuery.Append(", isr_ret = " + isr_ret.ToString());
            strQuery.Append(", iva_ret = " + iva_ret.ToString());
            strQuery.Append(", vendedorID = " + vendedorID.ToString());
            if (registrarMod)
            {
                strQuery.Append(", modificadoPorID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString());
                strQuery.Append(", modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            }
            strQuery.Append(" WHERE ID = " + this.ID.ToString());

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al actualizar la nota en la base de datos [{0}]: {1}", this.ID, ex.Message);
                return false;
            }

            return true;
        }

        public bool Leer(int intID)
        {
            this.ID = 0;
            this.mensaje = string.Empty;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT * " +
                    " FROM notas_cargo " +
                    " WHERE ID = " + intID.ToString();
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                this.mensaje = String.Format("Error al leer la nota ID[{0}]: {1}", intID, ex.Message);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                this.mensaje = String.Format("No existe la nota ID[{0}]", intID);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            ID = intID;
            nota = objRowResult["nota"].ToString();
            nota_suf = objRowResult["nota_suf"].ToString();
            fecha = (DateTime)objRowResult["fecha"];
            if (objRowResult.IsNull("fecha_timbrado"))
                fecha_timbrado = null;
            else
                fecha_timbrado = (DateTime)objRowResult["fecha_timbrado"];
            contado = Convert.ToBoolean(objRowResult["contado"]);
            division = (byte)objRowResult["division"];
            control_carga_ID = (int)objRowResult["control_carga_ID"];
            ruta_ID = (int)objRowResult["ruta_ID"];
            sucursal_ID = (int)objRowResult["sucursal_ID"];
            monto_subtotal = (decimal)objRowResult["monto_subtotal"];
            monto_descuento = (decimal)objRowResult["monto_descuento"];
            monto_iva = (decimal)objRowResult["monto_iva"];
            monto_subtotal2 = (decimal)objRowResult["monto_subtotal2"];
            monto_isr_ret = (decimal)objRowResult["monto_isr_ret"];
            monto_iva_ret = (decimal)objRowResult["monto_iva_ret"];
            total = (decimal)objRowResult["total"];
            descuento = (decimal)objRowResult["descuento"];
            descuento2 = (decimal)objRowResult["descuento2"];
            status = (byte)objRowResult["status"];
            if (objRowResult.IsNull("fecha_contrarecibo"))
                fecha_contrarecibo = null;
            else
                fecha_contrarecibo = (DateTime)objRowResult["fecha_contrarecibo"];
            if (objRowResult.IsNull("fecha_cobranza"))
                fecha_cobranza = null;
            else
                fecha_cobranza = (DateTime)objRowResult["fecha_cobranza"];
            if (objRowResult.IsNull("fecha_pago"))
                fecha_pago = null;
            else
                fecha_pago = (DateTime)objRowResult["fecha_pago"];
            if (objRowResult.IsNull("fecha_cancelacion"))
                fecha_cancelacion = null;
            else
                fecha_cancelacion = (DateTime)objRowResult["fecha_cancelacion"];
            total_cajas = (decimal)objRowResult["total_cajas"];
            comentarios = objRowResult["comentarios"].ToString();
            total_real = (decimal)objRowResult["total_real"];
            total_sin_nota = (decimal)objRowResult["total_sin_nota"];
            iva = (decimal)objRowResult["iva"];
            start_date = (DateTime)objRowResult["start_date"];
            end_date = (DateTime)objRowResult["end_date"];
            lista_precios_ID = (int)objRowResult["lista_precios_ID"];
            isr_ret = (decimal)objRowResult["isr_ret"];
            iva_ret = (decimal)objRowResult["iva_ret"];
            vendedorID = (int)objRowResult["vendedorID"];
            creadoPorID = (int)objRowResult["creadoPorID"];
            if (objRowResult.IsNull("creadoPorFecha"))
                creadoPorFecha = null;
            else
                creadoPorFecha = (DateTime)objRowResult["creadoPorFecha"];
            modificadoPorID = (int)objRowResult["modificadoPorID"];
            if (objRowResult.IsNull("modificadoPorFecha"))
                modificadoPorFecha = null;
            else
                modificadoPorFecha = (DateTime)objRowResult["modificadoPorFecha"];

            return true;
        }

        public CNotaCargoDB()
        {
            ID = 0;
            nota = string.Empty;
            nota_suf = string.Empty;
            fecha = DateTime.Now;
            fecha_timbrado = null;
            contado = false;
            division = 0;
            control_carga_ID = 0;
            ruta_ID = 0;
            sucursal_ID = 0;
            monto_subtotal = 0;
            monto_descuento = 0;
            monto_iva = 0;
            monto_subtotal2 = 0;
            monto_isr_ret = 0;
            monto_iva_ret = 0;
            total = 0;
            descuento = 0;
            descuento2 = 0;
            status = 0;
            fecha_contrarecibo = null;
            fecha_cobranza = null;
            fecha_pago = null;
            fecha_cancelacion = null;
            total_cajas = 0;
            comentarios = string.Empty;
            total_real = 0;
            total_sin_nota = 0;
            iva = 0;
            start_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            end_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            lista_precios_ID = 0;
            isr_ret = 0;
            iva_ret = 0;
            vendedorID = 0;
            creadoPorID = 0;
            creadoPorFecha = null;
            registrarMod = true;
            modificadoPorID = 0;
            modificadoPorFecha = null;
            mensaje = string.Empty;
        }
    }

    public class CNotaCreditoDB
    {
        public int ID { set; get; }
        public string nota { set; get; }
        public string nota_suf { set; get; }
        public bool contado { set; get; }
        public DateTime fecha { set; get; }
        public DateTime? fecha_timbrado { set; get; }
        public byte division { set; get; }
        public int control_carga_ID { set; get; }
        public int ruta_ID { set; get; }
        public int sucursal_ID { set; get; }
        public decimal monto_subtotal { set; get; }
        public decimal monto_descuento { set; get; }
        public decimal monto_iva { set; get; }
        public decimal monto_subtotal2 { set; get; }
        public decimal monto_isr_ret { set; get; }
        public decimal monto_iva_ret { set; get; }
        public decimal total { set; get; }
        public decimal total_usado { set; get; }
        public decimal descuento { set; get; }
        public decimal descuento2 { set; get; }
        public byte status { set; get; }
        public bool parcial { set; get; }
        public DateTime? fecha_contrarecibo { set; get; }
        public DateTime? fecha_cobranza { set; get; }
        public DateTime? fecha_pago { set; get; }
        public DateTime? fecha_cancelacion { set; get; }
        public decimal total_cajas { set; get; }
        public string comentarios { set; get; }
        public decimal total_real { set; get; }
        public decimal total_sin_nota { set; get; }
        public decimal iva { set; get; }
        public DateTime start_date { set; get; }
        public DateTime end_date { set; get; }
        public int lista_precios_ID { set; get; }
        public decimal isr_ret { set; get; }
        public decimal iva_ret { set; get; }
        public int vendedorID { set; get; }
        public int creadoPorID { set; get; }
        public DateTime? creadoPorFecha { set; get; }
        public bool registrarMod { set; get; }
        public int modificadoPorID { set; get; }
        public DateTime? modificadoPorFecha { set; get; }
        private string mensaje;

        public string Mensaje
        {
            get { return this.mensaje; }
        }

        public bool Guardar()
        {
            this.mensaje = string.Empty;
            if (this.ID == 0)
                return Crear();
            else
                return Actualizar();
        }

        private bool Existe()
        {
            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("SELECT ID " +
                    " FROM notas_credito " +
                    " WHERE nota = '" + this.nota + "'" +
                    " AND nota_suf = '" + this.nota_suf + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota [{0}]", ex.Message);
                return true;
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                this.mensaje = String.Format("Nota ya existe");
                return true;
            }
            else
                return false;
        }

        private bool Crear()
        {
            if (string.IsNullOrEmpty(this.nota))
            {
                Guid guidID = System.Guid.NewGuid();
                this.nota = guidID.ToString();
            }

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("INSERT INTO notas_credito (");
            strQuery.Append("nota");
            strQuery.Append(",nota_suf");
            strQuery.Append(",fecha");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(",fecha_timbrado");
            strQuery.Append(",contado");
            strQuery.Append(",division");
            strQuery.Append(",control_carga_ID");
            strQuery.Append(",ruta_ID");
            strQuery.Append(",sucursal_ID");
            strQuery.Append(",monto_subtotal");
            strQuery.Append(",monto_descuento");
            strQuery.Append(",monto_iva");
            strQuery.Append(",monto_subtotal2");
            strQuery.Append(",monto_isr_ret");
            strQuery.Append(",monto_iva_ret");
            strQuery.Append(",total");
            strQuery.Append(",total_usado");
            strQuery.Append(",descuento");
            strQuery.Append(",descuento2");
            strQuery.Append(",status");
            strQuery.Append(",parcial");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",fecha_contrarecibo");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",fecha_cobranza");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",fecha_pago");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",fecha_cancelacion");
            strQuery.Append(",total_cajas");
            strQuery.Append(",comentarios");
            strQuery.Append(",total_real");
            strQuery.Append(",total_sin_nota");
            strQuery.Append(",iva");
            strQuery.Append(",start_date");
            strQuery.Append(",end_date");
            strQuery.Append(",lista_precios_ID");
            strQuery.Append(",isr_ret");
            strQuery.Append(",iva_ret");
            strQuery.Append(",vendedorID");
            strQuery.Append(",creadoPorID");
            strQuery.Append(",creadoPorFecha");
            strQuery.Append(",modificadoPorID");
            strQuery.Append(",modificadoPorFecha");
            strQuery.Append(") VALUES(");
            strQuery.Append("'" + this.nota + "'");
            strQuery.Append(",'" + this.nota_suf + "'");
            strQuery.Append(",'" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(",'" + this.fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + (this.contado ? "1" : "0") + "'");
            strQuery.Append(",'" + this.division.ToString() + "'");
            strQuery.Append(",'" + this.control_carga_ID.ToString() + "'");
            strQuery.Append(",'" + this.ruta_ID.ToString() + "'");
            strQuery.Append(",'" + this.sucursal_ID.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal.ToString() + "'");
            strQuery.Append(",'" + this.monto_descuento.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva.ToString() + "'");
            strQuery.Append(",'" + this.monto_subtotal2.ToString() + "'");
            strQuery.Append(",'" + this.monto_isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.monto_iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.total.ToString() + "'");
            strQuery.Append(",'" + this.total_usado.ToString() + "'");
            strQuery.Append(",'" + this.descuento.ToString() + "'");
            strQuery.Append(",'" + this.descuento2.ToString() + "'");
            strQuery.Append(",'" + this.status.ToString() + "'");
            strQuery.Append(",'" + (this.parcial ? "1" : "0") + "'");
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(",'" + this.fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(",'" + this.fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(",'" + this.fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(",'" + this.fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.total_cajas.ToString() + "'");
            strQuery.Append(",'" + this.comentarios.ToString() + "'");
            strQuery.Append(",'" + this.total_real.ToString() + "'");
            strQuery.Append(",'" + this.total_sin_nota.ToString() + "'");
            strQuery.Append(",'" + this.iva.ToString() + "'");
            strQuery.Append(",'" + this.start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + this.lista_precios_ID.ToString() + "'");
            strQuery.Append(",'" + this.isr_ret.ToString() + "'");
            strQuery.Append(",'" + this.iva_ret.ToString() + "'");
            strQuery.Append(",'" + this.vendedorID.ToString() + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(",'" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'");
            strQuery.Append(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(")");

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al generar la nota en la base de datos [{0}]", ex.Message);
                return false;
            }

            strQuery.Length = 0;
            strQuery.Append("SELECT ID " +
                    " FROM notas_credito " +
                    " WHERE fecha = '" + this.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "   AND nota = '" + this.nota + "'");
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al leer la nota generada [{0}]", ex.Message);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            this.ID = (int)objRowResult["ID"];
            this.nota = this.ID.ToString();

            Actualizar();
            return true;
        }

        private void Calcular_Fecha_Cobranza()
        {
            DateTime? dtFechaCobranza = null;
            if (this.status == 1)
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCredito(1, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_contrarecibo = dtFechaCobranza.Value;
                else
                {
                    this.status = 2;
                    // Busca fecha de cobro
                    dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCredito(2, this.ID);
                    if (dtFechaCobranza.HasValue)
                        this.fecha_cobranza = dtFechaCobranza.Value;
                    else
                        this.fecha_cobranza = DateTime.Today;
                }
            }
            else
            {
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCredito(2, this.ID);
                if (dtFechaCobranza.HasValue)
                    this.fecha_cobranza = dtFechaCobranza.Value;
                else
                    this.fecha_cobranza = DateTime.Today;
            }
        }

        private bool Actualizar()
        {
            //if (this.status == 1 || this.status == 2)
            //    Calcular_Fecha_Cobranza();

            DataSet objDataResult = new DataSet();
            StringBuilder strQuery = new StringBuilder("UPDATE notas_credito SET ");
            strQuery.Append("nota = '" + nota + "'");
            strQuery.Append(", nota_suf = '" + nota_suf + "'");
            strQuery.Append(", fecha = '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (this.fecha_timbrado.HasValue)
                strQuery.Append(", fecha_timbrado = '" + fecha_timbrado.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", contado = " + (this.contado ? "1" : "0"));
            strQuery.Append(", division = " + division.ToString());
            strQuery.Append(", control_carga_ID = " + control_carga_ID.ToString());
            strQuery.Append(", ruta_ID = " + ruta_ID.ToString());
            strQuery.Append(", sucursal_ID = " + sucursal_ID.ToString());
            strQuery.Append(", monto_subtotal = " + monto_subtotal.ToString());
            strQuery.Append(", monto_descuento = " + monto_descuento.ToString());
            strQuery.Append(", monto_iva = " + monto_iva.ToString());
            strQuery.Append(", monto_subtotal2 = " + monto_subtotal2.ToString());
            strQuery.Append(", monto_isr_ret = " + monto_isr_ret.ToString());
            strQuery.Append(", monto_iva_ret = " + monto_iva_ret.ToString());
            strQuery.Append(", total = " + total.ToString());
            strQuery.Append(", total_usado = " + total_usado.ToString());
            strQuery.Append(", descuento = " + descuento.ToString());
            strQuery.Append(", descuento2 = " + descuento2.ToString());
            strQuery.Append(", status = " + status.ToString());
            strQuery.Append(", parcial = " + (this.parcial ? "1" : "0"));
            if (this.fecha_contrarecibo.HasValue)
                strQuery.Append(", fecha_contrarecibo = '" + fecha_contrarecibo.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cobranza.HasValue)
                strQuery.Append(", fecha_cobranza = '" + fecha_cobranza.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_pago.HasValue)
                strQuery.Append(", fecha_pago = '" + fecha_pago.Value.ToString("yyyy-MM-dd") + "'");
            if (this.fecha_cancelacion.HasValue)
                strQuery.Append(", fecha_cancelacion = '" + fecha_cancelacion.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", total_cajas = " + total_cajas.ToString());
            strQuery.Append(", comentarios = '" + comentarios + "'");
            strQuery.Append(", total_real = " + total_real.ToString());
            strQuery.Append(", total_sin_nota = " + total_sin_nota.ToString());
            strQuery.Append(", iva = " + iva.ToString());
            strQuery.Append(", start_date = '" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", end_date = '" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQuery.Append(", lista_precios_ID = " + lista_precios_ID.ToString());
            strQuery.Append(", isr_ret = " + isr_ret.ToString());
            strQuery.Append(", iva_ret = " + iva_ret.ToString());
            strQuery.Append(", vendedorID = " + vendedorID.ToString());
            if (registrarMod)
            {
                strQuery.Append(", modificadoPorID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString());
                strQuery.Append(", modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            }
            strQuery.Append(" WHERE ID = " + this.ID.ToString());

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
            }
            catch (ApplicationException ex)
            {
                this.mensaje = String.Format("Error al actualizar la nota en la base de datos [{0}]: {1}", this.ID, ex.Message);
                return false;
            }

            return true;
        }

        public bool Leer(int intID)
        {
            this.ID = 0;
            this.mensaje = string.Empty;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT * " +
                    " FROM notas_credito " +
                    " WHERE ID = " + intID.ToString();
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                this.mensaje = String.Format("Error al leer la nota ID[{0}]: {1}", intID, ex.Message);
                return false;
            }

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                this.mensaje = String.Format("No existe la nota ID[{0}]", intID);
                return false;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            ID = intID;
            nota = objRowResult["nota"].ToString();
            nota_suf = objRowResult["nota_suf"].ToString();
            fecha = (DateTime)objRowResult["fecha"];
            if (objRowResult.IsNull("fecha_timbrado"))
                fecha_timbrado = null;
            else
                fecha_timbrado = (DateTime)objRowResult["fecha_timbrado"];
            contado = Convert.ToBoolean(objRowResult["contado"]);
            division = (byte)objRowResult["division"];
            control_carga_ID = (int)objRowResult["control_carga_ID"];
            ruta_ID = (int)objRowResult["ruta_ID"];
            sucursal_ID = (int)objRowResult["sucursal_ID"];
            monto_subtotal = (decimal)objRowResult["monto_subtotal"];
            monto_descuento = (decimal)objRowResult["monto_descuento"];
            monto_iva = (decimal)objRowResult["monto_iva"];
            monto_subtotal2 = (decimal)objRowResult["monto_subtotal2"];
            monto_isr_ret = (decimal)objRowResult["monto_isr_ret"];
            monto_iva_ret = (decimal)objRowResult["monto_iva_ret"];
            total = (decimal)objRowResult["total"];
            total_usado = (decimal)objRowResult["total_usado"];
            descuento = (decimal)objRowResult["descuento"];
            descuento2 = (decimal)objRowResult["descuento2"];
            status = (byte)objRowResult["status"];
            parcial = (bool)objRowResult["parcial"];
            if (objRowResult.IsNull("fecha_contrarecibo"))
                fecha_contrarecibo = null;
            else
                fecha_contrarecibo = (DateTime)objRowResult["fecha_contrarecibo"];
            if (objRowResult.IsNull("fecha_cobranza"))
                fecha_cobranza = null;
            else
                fecha_cobranza = (DateTime)objRowResult["fecha_cobranza"];
            if (objRowResult.IsNull("fecha_pago"))
                fecha_pago = null;
            else
                fecha_pago = (DateTime)objRowResult["fecha_pago"];
            if (objRowResult.IsNull("fecha_cancelacion"))
                fecha_cancelacion = null;
            else
                fecha_cancelacion = (DateTime)objRowResult["fecha_cancelacion"];
            total_cajas = (decimal)objRowResult["total_cajas"];
            comentarios = objRowResult["comentarios"].ToString();
            total_real = (decimal)objRowResult["total_real"];
            total_sin_nota = (decimal)objRowResult["total_sin_nota"];
            iva = (decimal)objRowResult["iva"];
            start_date = (DateTime)objRowResult["start_date"];
            end_date = (DateTime)objRowResult["end_date"];
            lista_precios_ID = (int)objRowResult["lista_precios_ID"];
            isr_ret = (decimal)objRowResult["isr_ret"];
            iva_ret = (decimal)objRowResult["iva_ret"];
            vendedorID = (int)objRowResult["vendedorID"];
            creadoPorID = (int)objRowResult["creadoPorID"];
            if (objRowResult.IsNull("creadoPorFecha"))
                creadoPorFecha = null;
            else
                creadoPorFecha = (DateTime)objRowResult["creadoPorFecha"];
            modificadoPorID = (int)objRowResult["modificadoPorID"];
            if (objRowResult.IsNull("modificadoPorFecha"))
                modificadoPorFecha = null;
            else
                modificadoPorFecha = (DateTime)objRowResult["modificadoPorFecha"];

            return true;
        }

        public CNotaCreditoDB()
        {
            ID = 0;
            nota = string.Empty;
            nota_suf = string.Empty;
            fecha = DateTime.Now;
            fecha_timbrado = null;
            contado = false;
            division = 0;
            control_carga_ID = 0;
            ruta_ID = 0;
            sucursal_ID = 0;
            monto_subtotal = 0;
            monto_descuento = 0;
            monto_iva = 0;
            monto_subtotal2 = 0;
            monto_isr_ret = 0;
            monto_iva_ret = 0;
            total = 0;
            total_usado = 0;
            descuento = 0;
            descuento2 = 0;
            status = 0;
            parcial = false;
            fecha_contrarecibo = null;
            fecha_cobranza = null;
            fecha_pago = null;
            fecha_cancelacion = null;
            total_cajas = 0;
            comentarios = string.Empty;
            total_real = 0;
            total_sin_nota = 0;
            iva = 0;
            start_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            end_date = DateTime.Parse("1990/01/01 11:00:00 PM");
            lista_precios_ID = 0;
            isr_ret = 0;
            iva_ret = 0;
            vendedorID = 0;
            creadoPorID = 0;
            creadoPorFecha = null;
            registrarMod = true;
            modificadoPorID = 0;
            modificadoPorFecha = null;
            mensaje = string.Empty;
        }
    }
}