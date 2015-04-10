using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;
using CFE;

public partial class facturas_factura_impresion_ipisa : BasePagePopUp
{
    CFacturaElectronica objFactura;
    bool swNotaCredito = false;
    bool swNotaCargo = false;
    string strNegocioID;
    string strReferencia;
    string strProveedor;
    string strFact;
    string strFactID;
    string strTelefono;
    string strEmail;
    StringBuilder strNotas;
    string strNumCtaPago;
    string strBanco;
    string strMetodoPago;
    string strSustituye;
    string strVendedor;
    string strQRCodeData;
    string strComentarios;
    string strPorcDescuento;
    string strCertificadoEmisor;
    string strAutorizacionPAC;
    string strDiasCredito;
    string strCertificadoSAT;
    string strContadoCredito;
    CDireccion objDirEnvio = new CDireccion();
    string strNumeroCliente;
    string strEjecutivoVentas;
    string strContacto;
    string strCotizacion;
    // Este es la orden de compra del sistema
    string strOrdenCompra;
    // Esta es la orden de compra del cliente
    string strOC_Cliente;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["SIANID"] == null)
        {
            this.lblMessage.Text = "La sesión ha expirado, ingrese de nuevo al sistema";
            return;
        }

        strOrdenCompra = strProveedor = strFact = string.Empty;
        if (Request.QueryString["fact"] != null)
        {
            strFact = "CFDI_" + Request.QueryString["fact"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.ID, F.orden_compra, F.referencia, F.comentarios, E.proveedor, E.cuenta_bancaria " +
                    " ,E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM facturas_liq F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " LEFT JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.factura = '" + Request.QueryString["fact"].Replace(".xml", "") + "'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = objRowResult["ID"].ToString();
                strOC_Cliente = objRowResult["orden_compra"].ToString();
                strReferencia = objRowResult["referencia"].ToString();
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }
        else if (Request.QueryString["factID"] != null)
        {
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.factura, F.orden_compra, F.referencia, F.comentarios, E.proveedor " +
                    " ,E.cuenta_bancaria, E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM facturas_liq F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " INNER JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.ID = " + Request.QueryString["factID"];
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = Request.QueryString["factID"];
                strFact = "CFDI_" + objRowResult["factura"].ToString() + ".xml";
                strOC_Cliente = objRowResult["orden_compra"].ToString();
                strReferencia = objRowResult["referencia"].ToString();
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }
        // Nota Cargo
        else if (Request.QueryString["nota"] != null)
        {
            swNotaCargo = true;
            strFact = "CFDI_" + Request.QueryString["nota"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.ID, F.comentarios, E.proveedor, E.cuenta_bancaria " +
                    " ,E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM notas_cargo F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " LEFT JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.nota = '" + Request.QueryString["nota"].Replace(".xml", "") + "'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = objRowResult["ID"].ToString();
                strOrdenCompra = string.Empty;
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }
        else if (Request.QueryString["notaID"] != null)
        {
            swNotaCargo = true;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.nota, F.comentarios, E.proveedor " +
                    " ,E.cuenta_bancaria, E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM notas_cargo F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " INNER JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.ID = " + Request.QueryString["notaID"];
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = Request.QueryString["notaID"];
                strFact = "CFDI_" + objRowResult["nota"].ToString() + ".xml";
                strOrdenCompra = string.Empty;
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }
        else if (Request.QueryString["notaCR"] != null)
        {
            swNotaCredito = true;
            strFact = "CFDI_" + Request.QueryString["notaCR"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.ID, F.comentarios, E.proveedor, E.cuenta_bancaria " +
                    " ,E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM notas_credito F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " LEFT JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.nota = '" + Request.QueryString["notaCR"].Replace(".xml", "") + "'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = objRowResult["ID"].ToString();
                strOrdenCompra = string.Empty;
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }
        else if (Request.QueryString["notaCRID"] != null)
        {
            swNotaCredito = true;
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.nota, F.comentarios, E.proveedor " +
                    " ,E.cuenta_bancaria, E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,V.ID as vendID" +
                    " ,CONCAT(V.nombre, ' ' , V.apellidos) as vendedor" +
                    " FROM notas_credito F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " INNER JOIN metodos_pago M " +
                    " ON E.metodo_pago = M.ID " +
                    " INNER JOIN personas V " +
                    " ON F.vendedorID = V.ID " +
                    " WHERE F.ID = " + Request.QueryString["notaCRID"];
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                strFactID = Request.QueryString["notaCRID"];
                strFact = "CFDI_" + objRowResult["nota"].ToString() + ".xml";
                strOrdenCompra = string.Empty;
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strVendedor = objRowResult["vendedor"].ToString();
                strComentarios = objRowResult["comentarios"].ToString();
            }
        }

        if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact)))
            Generar_Factura();
        else
        {
            if (swNotaCargo)
                this.lblMessage.Text = "Nota de cargo no existe";
            else if (swNotaCredito)
                this.lblMessage.Text = "Nota de crédito no existe";
            else
                this.lblMessage.Text = "Factura electrónica no existe";
        }
    }

    private void Generar_Factura()
    {
        Obtener_Datos();
        int intTotalPaginas = Crear_Archivo_Temporal();
        Crear_Archivo(intTotalPaginas);
    }

    private void Obtener_Datos()
    {
        objFactura = new CFacturaElectronica();
        DataSet objDataResult = new DataSet();
        //strComentarios = string.Empty;
        strAutorizacionPAC = CComunDB.CCommun.Obtener_Valor_CatParametros(7);
        strCertificadoSAT = CComunDB.CCommun.Obtener_Valor_CatParametros(8);

        if (swNotaCredito)
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT establecimiento_ID" +
                                                        ",S.direccion as calle_envio" +
                                                        ",S.num_exterior as num_exterior_envio" +
                                                        ",S.num_interior as num_interior_envio" +
                                                        ",S.colonia as colonia_envio" +
                                                        ",S.municipio as municipio_envio" +
                                                        ",S.estado as estado_envio" +
                                                        ",S.pais as pais_envio" +
                                                        ",S.cp as cp_envio" +
                                                        ",S.poblacion as poblacion_envio" +
                                                        " FROM sucursales S" +
                                                        " INNER JOIN notas_credito F" +
                                                        " ON F.sucursal_ID = S.ID" +
                                                        " AND F.ID = " + strFactID);
        else if (swNotaCargo)
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT establecimiento_ID" +
                                                        ",S.direccion as calle_envio" +
                                                        ",S.num_exterior as num_exterior_envio" +
                                                        ",S.num_interior as num_interior_envio" +
                                                        ",S.colonia as colonia_envio" +
                                                        ",S.municipio as municipio_envio" +
                                                        ",S.estado as estado_envio" +
                                                        ",S.pais as pais_envio" +
                                                        ",S.cp as cp_envio" +
                                                        ",S.poblacion as poblacion_envio" +
                                                        " FROM sucursales S" +
                                                        " INNER JOIN notas_cargo F" +
                                                        " ON F.sucursal_ID = S.ID" +
                                                        " AND F.ID = " + strFactID);
        else
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT establecimiento_ID" +
                                                        ",S.direccion as calle_envio" +
                                                        ",S.num_exterior as num_exterior_envio" +
                                                        ",S.num_interior as num_interior_envio" +
                                                        ",S.colonia as colonia_envio" +
                                                        ",S.municipio as municipio_envio" +
                                                        ",S.estado as estado_envio" +
                                                        ",S.pais as pais_envio" +
                                                        ",S.cp as cp_envio" +
                                                        ",S.poblacion as poblacion_envio" +
                                                        " FROM sucursales S" +
                                                        " INNER JOIN facturas_liq F" +
                                                        " ON F.sucursal_ID = S.ID" +
                                                        " AND F.ID = " + strFactID);

        string strEstabID = objDataResult.Tables[0].Rows[0]["establecimiento_ID"].ToString();
        this.strNumeroCliente = "C" + int.Parse(strEstabID).ToString("0000");
        objDirEnvio.StrCalle = objDataResult.Tables[0].Rows[0]["calle_envio"].ToString();
        objDirEnvio.StrNumeroExterior = objDataResult.Tables[0].Rows[0]["num_exterior_envio"].ToString();
        objDirEnvio.StrNumeroInterior = objDataResult.Tables[0].Rows[0]["num_interior_envio"].ToString();
        objDirEnvio.StrColonia = objDataResult.Tables[0].Rows[0]["colonia_envio"].ToString();
        objDirEnvio.StrCP = objDataResult.Tables[0].Rows[0]["cp_envio"].ToString();
        objDirEnvio.StrLocalidad = objDataResult.Tables[0].Rows[0]["poblacion_envio"].ToString();
        objDirEnvio.StrMunicipio = objDataResult.Tables[0].Rows[0]["municipio_envio"].ToString();
        objDirEnvio.StrEstado = objDataResult.Tables[0].Rows[0]["estado_envio"].ToString();
        objDirEnvio.StrPais = objDataResult.Tables[0].Rows[0]["pais_envio"].ToString();

        objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT E.contacto_compras" +
                                                    ",CONCAT(P.nombre, ' ' , P.apellidos) as vendedor" +
                                                    ",E.dias_credito" +
                                                    " FROM establecimientos E" +
                                                    " LEFT JOIN personas P" +
                                                    " ON P.ID = E.vendedorID" +
                                                    " WHERE E.ID = " + strEstabID +
                                                    ";" +   // 1
                                                    " SELECT orden_compraID" +
                                                    " FROM orden_compra_factura" +
                                                    " WHERE facturaID = " + strFactID +
                                                    " ORDER BY orden_compraID" +
                                                    ";" +   //2
                                                    " SELECT C.cotizacionID" +
                                                    " FROM cotizacion_orden_compra C" +
                                                    " INNER JOIN orden_compra_factura O" +
                                                    " ON C.orden_compraID = O.orden_compraID" +
                                                    " AND O.facturaID = " + strFactID +
                                                    " ORDER BY C.cotizacionID");

        this.strEjecutivoVentas = objDataResult.Tables[0].Rows[0]["vendedor"].ToString();
        this.strContacto = objDataResult.Tables[0].Rows[0]["contacto_compras"].ToString();
        this.strDiasCredito = objDataResult.Tables[0].Rows[0]["dias_credito"].ToString();

        if (swNotaCargo || swNotaCredito)
        {
            this.strOrdenCompra = this.strCotizacion = string.Empty;
        }
        else
        {
            StringBuilder strTemp = new StringBuilder();
            foreach (DataRow objRow in objDataResult.Tables[1].Rows)
            {
                if (strTemp.Length != 0)
                    strTemp.Append(", ");
                strTemp.Append(objRow[0].ToString());
            }
            this.strOrdenCompra = strTemp.ToString();

            strTemp.Clear();
            foreach (DataRow objRow in objDataResult.Tables[2].Rows)
            {
                if (strTemp.Length != 0)
                    strTemp.Append(", ");
                strTemp.Append(objRow[0].ToString());
            }
            this.strCotizacion = strTemp.ToString();
        }
        
        objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu("SELECT *" +
                                                        " FROM sistema_apps " +
                                                        " WHERE ID = " + Session["SIANAppID"]);

        this.strTelefono = objDataResult.Tables[0].Rows[0]["telefono_impresion"].ToString();
        this.strEmail = objDataResult.Tables[0].Rows[0]["email_impresion"].ToString();

        string strQuery = "SELECT folio, isr_ret, iva_ret, monto_isr_ret, monto_iva_ret" +
                         ",descuento, descuento2, detalle_incluido, contado ";

        if (swNotaCredito)
            strQuery += " FROM notas_credito " +
                        " WHERE nota = '" + strFact.Replace("CFDI_", "").Replace(".xml", "") + "'";
        else if (swNotaCargo)
            strQuery += " FROM notas_cargo " +
                        " WHERE nota = '" + strFact.Replace("CFDI_", "").Replace(".xml", "") + "'";
        else
            strQuery += " FROM facturas_liq " +
                        " WHERE factura = '" + strFact.Replace("CFDI_", "").Replace(".xml", "") + "'";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        objFactura.IntFolio = (int)objRowResult["folio"];

        if ((bool)objRowResult["contado"])
            strContadoCredito = "Contado";
        else
            strContadoCredito = "Crédito";

        bool swDetalleIncluido = (bool)objRowResult["detalle_incluido"];

        XmlDocument xmlFactura = new XmlDocument();
        xmlFactura.Load(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact));

        XmlNodeList xmlComprobante = xmlFactura.GetElementsByTagName("cfdi:Comprobante");
        if (xmlComprobante[0].Attributes["serie"] != null)
            objFactura.StrSerie = xmlComprobante[0].Attributes["serie"].Value;
        objFactura.DtFecha = Convert.ToDateTime(xmlComprobante[0].Attributes["fecha"].Value);
        objFactura.AmtTotal = Convert.ToDecimal(xmlComprobante[0].Attributes["total"].Value);
        objFactura.AmtSubTotal = Convert.ToDecimal(xmlComprobante[0].Attributes["subTotal"].Value);
        if (xmlComprobante[0].Attributes["descuento"] != null)
            objFactura.AmtDescuento = Convert.ToDecimal(xmlComprobante[0].Attributes["descuento"].Value);
        if (xmlComprobante[0].Attributes["metodoDePago"] != null)
            objFactura.StrMetodoDePago = xmlComprobante[0].Attributes["metodoDePago"].Value;
        if (xmlComprobante[0].Attributes["NumCtaPago"] != null)
            objFactura.StrNumCtaPago = xmlComprobante[0].Attributes["NumCtaPago"].Value;
        this.strCertificadoEmisor = xmlComprobante[0].Attributes["noCertificado"].Value;
        objFactura.StrMoneda = xmlComprobante[0].Attributes["Moneda"].Value;
        if (xmlComprobante[0].Attributes["TipoCambio"] != null)
            objFactura.AmtTipoCambio = Convert.ToDecimal(xmlComprobante[0].Attributes["TipoCambio"].Value);

        XmlNodeList xmlEmisor = xmlFactura.GetElementsByTagName("cfdi:Emisor");
        objFactura.StrNombreEmisor = xmlEmisor[0].Attributes["nombre"].Value;
        objFactura.StrRFCEmisor = xmlEmisor[0].Attributes["rfc"].Value;

        XmlNodeList xmlEmisorDom = xmlFactura.GetElementsByTagName("cfdi:DomicilioFiscal");
        objFactura.DirDomicilioFiscal.StrCalle = xmlEmisorDom[0].Attributes["calle"].Value;
        objFactura.DirDomicilioFiscal.StrNumeroExterior = xmlEmisorDom[0].Attributes["noExterior"].Value;
        if (xmlEmisorDom[0].Attributes["noInterior"] != null)
            objFactura.DirDomicilioFiscal.StrNumeroInterior = xmlEmisorDom[0].Attributes["noInterior"].Value;
        objFactura.DirDomicilioFiscal.StrColonia = xmlEmisorDom[0].Attributes["colonia"].Value;
        objFactura.DirDomicilioFiscal.StrLocalidad = xmlEmisorDom[0].Attributes["localidad"].Value;
        objFactura.DirDomicilioFiscal.StrMunicipio = xmlEmisorDom[0].Attributes["municipio"].Value;
        objFactura.DirDomicilioFiscal.StrEstado = xmlEmisorDom[0].Attributes["estado"].Value;
        objFactura.DirDomicilioFiscal.StrPais = xmlEmisorDom[0].Attributes["pais"].Value;
        objFactura.DirDomicilioFiscal.StrCP = xmlEmisorDom[0].Attributes["codigoPostal"].Value;

        XmlNodeList xmlEmisorDomExp = xmlFactura.GetElementsByTagName("cfdi:ExpedidoEn");
        objFactura.DirDomicilioExpedicion.StrCalle = xmlEmisorDomExp[0].Attributes["calle"].Value;
        objFactura.DirDomicilioExpedicion.StrNumeroExterior = xmlEmisorDomExp[0].Attributes["noExterior"].Value;
        if (xmlEmisorDomExp[0].Attributes["noInterior"] != null)
            objFactura.DirDomicilioExpedicion.StrNumeroInterior = xmlEmisorDomExp[0].Attributes["noInterior"].Value;
        objFactura.DirDomicilioExpedicion.StrColonia = xmlEmisorDomExp[0].Attributes["colonia"].Value;
        objFactura.DirDomicilioExpedicion.StrLocalidad = xmlEmisorDomExp[0].Attributes["localidad"].Value;
        objFactura.DirDomicilioExpedicion.StrMunicipio = xmlEmisorDomExp[0].Attributes["municipio"].Value;
        objFactura.DirDomicilioExpedicion.StrEstado = xmlEmisorDomExp[0].Attributes["estado"].Value;
        objFactura.DirDomicilioExpedicion.StrPais = xmlEmisorDomExp[0].Attributes["pais"].Value;
        objFactura.DirDomicilioExpedicion.StrCP = xmlEmisorDomExp[0].Attributes["codigoPostal"].Value;

        XmlNodeList xmlRegimen = xmlFactura.GetElementsByTagName("cfdi:RegimenFiscal");
        objFactura.StrRegimen = xmlRegimen[0].Attributes["Regimen"].Value;

        XmlNodeList xmlReceptor = xmlFactura.GetElementsByTagName("cfdi:Receptor");
        objFactura.StrRFCCliente = xmlReceptor[0].Attributes["rfc"].Value;
        if (xmlReceptor[0].Attributes["nombre"] != null)
            objFactura.StrNombreCliente = xmlReceptor[0].Attributes["nombre"].Value;

        if (xmlFactura.GetElementsByTagName("cfdi:Domicilio") != null)
        {
            XmlNodeList xmlReceptorDom = xmlFactura.GetElementsByTagName("cfdi:Domicilio");
            if (xmlReceptorDom[0].Attributes["calle"] != null)
                objFactura.DirDomicilioCliente.StrCalle = xmlReceptorDom[0].Attributes["calle"].Value;
            if (xmlReceptorDom[0].Attributes["noExterior"] != null)
                objFactura.DirDomicilioCliente.StrNumeroExterior = xmlReceptorDom[0].Attributes["noExterior"].Value;
            if (xmlReceptorDom[0].Attributes["noInterior"] != null)
                objFactura.DirDomicilioCliente.StrNumeroInterior = xmlReceptorDom[0].Attributes["noInterior"].Value;
            if (xmlReceptorDom[0].Attributes["colonia"] != null)
                objFactura.DirDomicilioCliente.StrColonia = xmlReceptorDom[0].Attributes["colonia"].Value;
            if (xmlReceptorDom[0].Attributes["localidad"] != null)
                objFactura.DirDomicilioCliente.StrLocalidad = xmlReceptorDom[0].Attributes["localidad"].Value;
            if (xmlReceptorDom[0].Attributes["municipio"] != null)
                objFactura.DirDomicilioCliente.StrMunicipio = xmlReceptorDom[0].Attributes["municipio"].Value;
            if (xmlReceptorDom[0].Attributes["estado"] != null)
                objFactura.DirDomicilioCliente.StrEstado = xmlReceptorDom[0].Attributes["estado"].Value;
            objFactura.DirDomicilioCliente.StrPais = xmlReceptorDom[0].Attributes["pais"].Value;
            if (xmlReceptorDom[0].Attributes["codigoPostal"] != null)
                objFactura.DirDomicilioCliente.StrCP = xmlReceptorDom[0].Attributes["codigoPostal"].Value;
        }
        XmlNodeList xmlProductos = xmlFactura.GetElementsByTagName("cfdi:Concepto");

        if (swNotaCargo)
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT F.* " +
                                                        ",S.clave as clave " +
                                                        ",P.nombre as nombre " +
                                                        ",P.unimed as unimed " +
                                                        ",P.usar_detalle" +
                                                        ",P.codigo" +
                                                        ",F.imprimir_detalle" +
                                                        ",S.clave" +
                                                        " FROM notas_cargo_prod F" +
                                                        " INNER JOIN productos P" +
                                                        " ON F.producto_ID = P.ID " +
                                                        " AND F.nota_ID = " + strFactID +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.nota_ID = " + strFactID +
                                                        " ORDER BY consecutivo, nombre");
        else if (swNotaCredito)
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT F.* " +
                                                        ",S.clave as clave " +
                                                        ",P.nombre as nombre " +
                                                        ",P.unimed as unimed " +
                                                        ",P.usar_detalle" +
                                                        ",P.codigo" +
                                                        ",S.clave" +
                                                        " FROM notas_credito_prod F" +
                                                        " INNER JOIN productos P" +
                                                        " ON F.producto_ID = P.ID " +
                                                        " AND F.nota_ID = " + strFactID +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.nota_ID = " + strFactID +
                                                        " ORDER BY consecutivo, nombre");
        else
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT F.*" +
                                                        ",S.clave as clave " +
                                                        ",P.nombre as nombre " +
                                                        ",P.unimed as unimed " +
                                                        ",P.unimed2 " +
                                                        ",P.unimed_original" +
                                                        ",P.usar_detalle" +
                                                        ",P.codigo" +
                                                        ",S.clave" +
                                                        ",P2.nombre as producto_grupo" +
                                                        " FROM facturas_liq_prod F" +
                                                        " INNER JOIN productos P" +
                                                        " ON F.producto_ID = P.ID " +
                                                        " AND F.factura_ID = " + strFactID +
                                                        " LEFT JOIN productos P2" +
                                                        " ON P2.ID = F.grupoID" +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.factura_ID = " + strFactID +
                                                        " ORDER BY consecutivo, nombre" +
                                                        ";" +
                                                        " SELECT consecutivo, producto_ID, lote, fecha_caducidad" +
                                                        " FROM facturas_prod_lote" +
                                                        " WHERE factura_ID = " + strFactID +
                                                        " ORDER BY consecutivo, producto_ID");

        List<CProducto> lstProductos = new List<CProducto>();
        StringBuilder strLotes = new StringBuilder();
        string strGrupoConsecutivo = string.Empty;
        for (int i = 0; i < xmlProductos.Count; i++)
        {
            CProducto objProducto = new CProducto();

            if (i < objDataResult.Tables[0].Rows.Count)
            {
                // Si es factura
                if (!swNotaCargo && !swNotaCredito)
                {
                    if (!string.IsNullOrEmpty(objDataResult.Tables[0].Rows[i]["grupoID"].ToString()))
                    {
                        if (!strGrupoConsecutivo.Equals(objDataResult.Tables[0].Rows[i]["grupoID"].ToString() + "_" + objDataResult.Tables[0].Rows[i]["grupo_consecutivo"].ToString()))
                        {
                            CProducto objProductoKit = new CProducto();
                            objProductoKit.StrDescripcion = objDataResult.Tables[0].Rows[i]["producto_grupo"].ToString() + ", Cant: " +
                                                            ((decimal)objDataResult.Tables[0].Rows[i]["grupo_cantidad"]).ToString("0.##");
                            objProductoKit.SwKit = true;
                            strGrupoConsecutivo = objDataResult.Tables[0].Rows[i]["grupoID"].ToString() + "_" + objDataResult.Tables[0].Rows[i]["grupo_consecutivo"].ToString();
                            lstProductos.Add(objProductoKit);
                        }
                    }
                }

                if (!swNotaCargo && !swNotaCredito)
                {
                    if (!string.IsNullOrEmpty(objDataResult.Tables[0].Rows[i]["grupoID"].ToString()))
                        objProducto.IntGrupoID = (int)objDataResult.Tables[0].Rows[i]["grupoID"];
                }

                if ((bool)objDataResult.Tables[0].Rows[i]["usar_detalle"])
                {
                    objProducto.StrDescripcion = objDataResult.Tables[0].Rows[i]["detalle"].ToString();
                }
                else
                {
                    if (swDetalleIncluido)
                        objProducto.StrDescripcion = objDataResult.Tables[0].Rows[i]["nombre"].ToString();
                    else
                        objProducto.StrDescripcion = xmlProductos[i].Attributes["descripcion"].Value;

                    if ((bool)objDataResult.Tables[0].Rows[i]["imprimir_detalle"])
                       objProducto.StrDetalle = objDataResult.Tables[0].Rows[i]["detalle"].ToString();
                }

                objProducto.StrCodigo = objDataResult.Tables[0].Rows[i]["codigo"].ToString();
                objProducto.StrClaveCliente = objDataResult.Tables[0].Rows[i]["clave"].ToString();
            }
            else
                objProducto.StrDescripcion = xmlProductos[i].Attributes["descripcion"].Value;

            // Factura
            if (!swNotaCargo && !swNotaCredito)
            {
                DataRow[] dtLotes = objDataResult.Tables[1].Select(
                                                        "consecutivo = " + objDataResult.Tables[0].Rows[i]["consecutivo"].ToString());
                strLotes.Clear();
                foreach (DataRow dtLote in dtLotes)
                {
                    if (strLotes.Length > 0)
                        strLotes.Append(", ");
                    strLotes.Append(dtLote["lote"].ToString());
                }

                if (strLotes.Length > 0)
                {
                    if (!string.IsNullOrEmpty(objProducto.StrDetalle))
                        objProducto.StrDetalle += "\u000D" + CRutinas.GetLocalizedMsg("lblLote") + ": " + strLotes.ToString();
                    else
                        objProducto.StrDetalle = CRutinas.GetLocalizedMsg("lblLote") + ": " + strLotes.ToString();
                }
            }

            objProducto.AmtCantidad = Convert.ToDecimal(xmlProductos[i].Attributes["cantidad"].Value);
            objProducto.AmtValorUnitario = Convert.ToDecimal(xmlProductos[i].Attributes["valorUnitario"].Value);
            objProducto.AmtImporte = Convert.ToDecimal(xmlProductos[i].Attributes["importe"].Value);
            if (xmlProductos[i].Attributes["unidad"] != null)
                objProducto.StrUnidad = xmlProductos[i].Attributes["unidad"].Value;
            lstProductos.Add(objProducto);
        }
        objFactura.LstProductos = lstProductos;

        XmlNodeList xmlImpuestos = xmlFactura.GetElementsByTagName("cfdi:Traslado");
        if (xmlImpuestos.Count > 0)
        {
            objFactura.AmtTasa = Convert.ToDecimal(xmlImpuestos[0].Attributes["tasa"].Value);
            objFactura.AmtImpuesto = Convert.ToDecimal(xmlImpuestos[0].Attributes["importe"].Value);
        }

        strPorcDescuento = string.Empty;
        if ((decimal)objRowResult["descuento"] != 0)
        {
            strPorcDescuento = ((decimal)objRowResult["descuento"]).ToString("0.##") + "%";
            if ((decimal)objRowResult["descuento2"] != 0)
                strPorcDescuento += " y " + ((decimal)objRowResult["descuento2"]).ToString("0.##") + "%";
            strPorcDescuento += " descuento";
        }

        objFactura.AmtTasa_ISR_Retenido = (decimal)objRowResult["isr_ret"];
        objFactura.AmtTasa_IVA_Retenido = (decimal)objRowResult["iva_ret"];

        objFactura.AmtISR_Retenido = (decimal)objRowResult["monto_isr_ret"];
        objFactura.AmtIVA_Retenido = (decimal)objRowResult["monto_iva_ret"];

        XmlNodeList xmlSello = xmlFactura.GetElementsByTagName("tfd:TimbreFiscalDigital");
        objFactura.StrVersionCFDI = xmlSello[0].Attributes["version"].Value;
        objFactura.StrFolioFiscal = xmlSello[0].Attributes["UUID"].Value;
        objFactura.StrCertificadoSAT = xmlSello[0].Attributes["noCertificadoSAT"].Value;
        objFactura.StrSelloCFDI = xmlSello[0].Attributes["selloCFD"].Value;
        objFactura.StrSelloSAT = xmlSello[0].Attributes["selloSAT"].Value;
        objFactura.DtFechaCertificado = DateTime.Parse(xmlSello[0].Attributes["FechaTimbrado"].Value);

        strQRCodeData = "?re=" + objFactura.StrRFCEmisor +
                        "&rr=" + objFactura.StrRFCCliente +
                        "&tt=" + objFactura.AmtTotal.ToString("0000000000.000000") +
                        "&id=" + objFactura.StrFolioFiscal;

        strNotas = new StringBuilder();
        if (!swNotaCredito && !swNotaCargo)
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT nota, nota_suf " +
                                                        " FROM nota_facturas F" +
                                                        " INNER JOIN notas N" +
                                                        " ON N.ID = F.nota_ID" +
                                                        " AND F.factura_ID = " + strFactID +
                                                        " ORDER BY nota, nota_suf");

            foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
            {
                if (strNotas.Length > 0)
                    strNotas.Append(", ");
                strNotas.Append(objRowResult2["nota"].ToString());
                if (!string.IsNullOrEmpty(objRowResult2["nota_suf"].ToString()))
                    strNotas.Append("-" + objRowResult2["nota_suf"].ToString());
            }

            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT factura_IDAnt" +
                      " FROM facturas_reemplazo" +
                                                        " WHERE factura_ID = " + strFactID);

            if (objDataResult.Tables[0].Rows.Count > 0)
                strSustituye = "Sustituye a la factura " + objDataResult.Tables[0].Rows[0]["factura_IDAnt"].ToString();
            else
                strSustituye = string.Empty;
        }
    }

    private int Crear_Archivo_Temporal()
    {
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(45.0f, 45.0f, 225.0f, 425.0f);

        MemoryStream m = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        document.Open();

        float[] ancho_columnas = new float[8];
        ancho_columnas[0] = 30;
        ancho_columnas[1] = 20;
        ancho_columnas[2] = 73;
        ancho_columnas[3] = 92;
        ancho_columnas[4] = 45;
        ancho_columnas[5] = 204;
        ancho_columnas[6] = 60;
        ancho_columnas[7] = 63;

        char tab = '\u0009';
        foreach (CProducto objProducto in objFactura.LstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;

            Paragraph texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(string.Empty, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrCodigo, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrClaveCliente, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrUnidad, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            if (!string.IsNullOrEmpty(objProducto.StrDetalle))
                texto = new Paragraph(objProducto.StrDescripcion + "\n" + objProducto.StrDetalle.Replace(tab.ToString(), "    "), ftProductos);
            else
                texto = new Paragraph(objProducto.StrDescripcion, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            if (objProducto.IntGrupoID == 0)
                celda.PaddingLeft = 0;
            else
                celda.PaddingLeft = 10;
            tblLinea.AddCell(celda);

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtValorUnitario.ToString("c")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtImporte.ToString("c")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            document.Add(tblLinea);
        }

        document.Close();

        m.Close();

        return writer.PageNumber - 1;
    }

    private void Crear_Archivo(int intTotalPaginas)
    {
        int intMaxWidth = 190, intMaxHeight = 50, intLogoWidth, intLogoHeight;
        HttpCookie ckSIAN = Request.Cookies["userCng"];
        int intNombreLogo = ckSIAN["ck_logo"].LastIndexOf("/") + 1;
        string[] strSize = ckSIAN["ck_logo_size"].Split('x');
        intLogoWidth = int.Parse(strSize[0]);
        intLogoHeight = int.Parse(strSize[1]);
        float flRelacion = CRutinas.Calcular_Relacion(intMaxWidth, intMaxHeight, intLogoWidth, intLogoHeight);

        MyPageEvents events = new MyPageEvents();
        events._swNotaCredito = swNotaCredito;
        events._swNotaCargo = swNotaCargo;
        events._strNegocioID = strNegocioID;
        events._strFactID = objFactura.IntFolio.ToString();
        events._strFactSerie = objFactura.StrSerie;
        events._strCertificado = objFactura.StrCertificadoSAT;
        events._strCertificadoEmisor = this.strCertificadoEmisor;
        events._strFolioFiscal = objFactura.StrFolioFiscal;
        events._strSelloCFDI = objFactura.StrSelloCFDI;
        events._strSelloSAT = objFactura.StrSelloSAT;
        events._strCadenaOriginal = objFactura.StrCadenaOriginalCertificacion;
        events._intFolio = objFactura.IntFolio;
        events._dtFechaCertificado = objFactura.DtFechaCertificado;
        events._intNoAprobacion = objFactura.IntNoAprobacion;
        events._intAnoAprobacion = objFactura.IntAnoAprobacion;
        events._strMetodoDePago = objFactura.StrMetodoDePago;
        events._strNumCtaPago = objFactura.StrNumCtaPago;
        events._strCondicionesDePago = objFactura.StrCondicionesDePago;
        events._amtSubTotal = objFactura.AmtSubTotal;
        events._strPorcDescuento = strPorcDescuento;
        events._amtDescuento = objFactura.AmtDescuento;
        events._amtTotal = objFactura.AmtTotal;
        events._dtFecha = objFactura.DtFecha;
        events._strTotalLetras = CRutinas.ObtenerImporteLetras(objFactura.AmtTotal, objFactura.StrMoneda);
        events._strRFCEmisor = objFactura.StrRFCEmisor;
        events._strNombreEmisor = objFactura.StrNombreEmisor;
        events._strRegimen = objFactura.StrRegimen;
        events._dirDomicilioFiscal = objFactura.DirDomicilioFiscal;
        events._dirDomicilioExpedicion = objFactura.DirDomicilioExpedicion;
        events._strRFCCliente = objFactura.StrRFCCliente;
        events._strNombreCliente = objFactura.StrNombreCliente;
        events._strCedulaFiscalCliente = objFactura.StrCedulaFiscalCliente;
        events._strCedulaIEPSCliente = objFactura.StrCedulaIEPSCliente;
        events._dirDomicilioCliente = objFactura.DirDomicilioCliente;
        events._amtISR_Retenido = objFactura.AmtISR_Retenido;
        events._amtTasa_ISR_Retenido = objFactura.AmtTasa_ISR_Retenido;
        events._amtIVA_Retenido = objFactura.AmtIVA_Retenido;
        events._amtTasa_IVA_Retenido = objFactura.AmtTasa_IVA_Retenido;
        events._amtTotalImpuestosRetenidos = objFactura.AmtTotalImpuestosRetenidos;
        events._amtImpuesto = objFactura.AmtImpuesto;
        events._amtTasa = objFactura.AmtTasa;
        events._amtImporte = objFactura.AmtImporte;
        events._amtTotalImpuestos = objFactura.AmtTotalImpuestos;

        events._strMapPath = Server.MapPath("../imagenes") + "/";
        events._strLogoEmpresa = ckSIAN["ck_logo"].Substring(intNombreLogo);
        events._flEscala = flRelacion * 100;

        events._intTotalPaginas = intTotalPaginas;
        events._strOrdenCompra = strOrdenCompra;
        events._strReferencia = strReferencia;
        events._strOC_Cliente = strOC_Cliente;
        events._strProveedor = strProveedor;
        events._strNotas = strNotas.ToString();
        events._strBanco = strBanco;
        events._strSustituye = strSustituye;
        events._strVendedor = strVendedor;
        events._strQRCodeData = strQRCodeData;
        events._strComentarios = strComentarios;
        events._strMoneda = (objFactura.StrMoneda.Equals("Dolar") ? "Dólar" : "M.N.");
        events._strMonedaAbrev = objFactura.StrMoneda;

        events._strTelefono = this.strTelefono;
        events._strEmail = this.strEmail;
        events._strAutorizacionPAC = strAutorizacionPAC;
        events._strDiasCredito = strDiasCredito;
        events._strCertificadoSAT = strCertificadoSAT;
        events._strContadoCredito = strContadoCredito;
        events._objDirEnvio = objDirEnvio;
        events._strEjecutivoVentas = strEjecutivoVentas;
        events._strNumeroCliente = strNumeroCliente;
        events._strContacto = strContacto;
        events._strCotizacion = this.strCotizacion;
        events._strOrdenCompra = this.strOrdenCompra;

        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        string strArchivoFormato = Server.MapPath("factura_ipisa.pdf");

        PdfReader pdfEntrada = new PdfReader(strArchivoFormato);

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(45.0f, 45.0f, 225.0f, 425.0f);

        Response.ContentType = "application/pdf";
        PdfWriter writer;
        if (Request.QueryString["m"] == null)
        {
            writer = PdfWriter.GetInstance(document, m);
            writer.CloseStream = false;
        }
        else
        {
            strFact = strFact.Replace("CFDI_", "PDF_").Replace(".xml", ".pdf");
            if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact)))
            {
                if (swNotaCredito)
                    Response.Redirect("facturas_correo.aspx?t=2&ID=" + strFactID);
                else if (swNotaCargo)
                    Response.Redirect("facturas_correo.aspx?t=1&ID=" + strFactID);
                else
                    Response.Redirect("facturas_correo.aspx?t=0&ID=" + strFactID);
            }
            FileStream flArchivo = new FileStream(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact), FileMode.Create);
            writer = PdfWriter.GetInstance(document, flArchivo);
        }

        events.pdfPage1 = writer.GetImportedPage(pdfEntrada, 1);

        writer.PageEvent = events;

        document.Open();

        float[] ancho_columnas = new float[8];
        ancho_columnas[0] = 30;
        ancho_columnas[1] = 20;
        ancho_columnas[2] = 73; //73
        ancho_columnas[3] = 91; //92
        ancho_columnas[4] = 45;
        ancho_columnas[5] = 225;
        ancho_columnas[6] = 60;
        ancho_columnas[7] = 63;

        char tab = '\u0009';
        foreach (CProducto objProducto in objFactura.LstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;
            Paragraph texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(string.Empty, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrCodigo, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrClaveCliente, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrUnidad, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            if (!string.IsNullOrEmpty(objProducto.StrDetalle))
                texto = new Paragraph(objProducto.StrDescripcion + "\n" + objProducto.StrDetalle.Replace(tab.ToString(), "    "), ftProductos);
            else
                texto = new Paragraph(objProducto.StrDescripcion, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            if (objProducto.IntGrupoID == 0)
                celda.PaddingLeft = 0;
            else
                celda.PaddingLeft = 10;
            tblLinea.AddCell(celda);

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtValorUnitario.ToString("c")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtImporte.ToString("c")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            document.Add(tblLinea);
        }

        document.Close();
        if (Request.QueryString["m"] == null)
        {
            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
            m.Close();
        }
        else
            if (swNotaCredito)
                Response.Redirect("facturas_correo.aspx?t=2&ID=" + strFactID);
            else if (swNotaCargo)
                Response.Redirect("facturas_correo.aspx?t=1&ID=" + strFactID);
            else
                Response.Redirect("facturas_correo.aspx?t=0&ID=" + strFactID);
    }

    class MyPageEvents : PdfPageEventHelper
    {
        public bool _swNotaCredito;
        public bool _swNotaCargo;
        public string _strNegocioID;
        public string _strFactID;
        public string _strFactSerie;
        public string _strMapPath;
        public float _flEscala;
        public string _strLogoEmpresa;
        public string _strFolioFiscal;
        public string _strCertificado;
        public string _strCertificadoEmisor;
        public string _strSelloCFDI;
        public string _strSelloSAT;
        public string _strCadenaOriginal;
        public string _strProveedor;
        public string _strOrdenCompra;
        public string _strReferencia;
        public string _strRegimen;
        public string _strTelefono;
        public string _strEmail;
        public int _intFolio;
        public DateTime _dtFechaCertificado;
        public int _intNoAprobacion;
        public int _intAnoAprobacion;
        public string _strMetodoDePago;
        public string _strCondicionesDePago;
        public decimal _amtSubTotal;
        public string _strPorcDescuento;
        public decimal _amtDescuento;
        public decimal _amtTotal;
        public string _strTotalLetras;
        public string _strRFCEmisor;
        public string _strNombreEmisor;
        public CDireccion _dirDomicilioFiscal;
        public CDireccion _dirDomicilioExpedicion;
        public string _strRFCCliente;
        public string _strNombreCliente;
        public string _strCedulaFiscalCliente;
        public string _strCedulaIEPSCliente;
        public CDireccion _dirDomicilioCliente;
        public decimal _amtISR_Retenido;
        public decimal _amtTasa_ISR_Retenido;
        public decimal _amtIVA_Retenido;
        public decimal _amtTasa_IVA_Retenido;
        public decimal _amtTotalImpuestosRetenidos;
        public decimal _amtImpuesto;
        public decimal _amtTasa;
        public decimal _amtImporte;
        public decimal _amtTotalImpuestos;
        public int _intTotalPaginas;
        public string _strNotas;
        public string _strNumCtaPago;
        public string _strBanco;
        public string _strSustituye;
        public string _strVendedor;
        public string _strQRCodeData;
        public string _strComentarios;
        public DateTime _dtFecha;
        public PdfImportedPage pdfPage1;
        public string _strMoneda;
        public string _strMonedaAbrev;
        public string _strAutorizacionPAC;
        public string _strDiasCredito;
        public string _strCertificadoSAT;
        public string _strContadoCredito;
        public CDireccion _objDirEnvio;
        public string _strNumeroCliente;
        public string _strContacto;
        public string _strEjecutivoVentas;
        public string _strCotizacion;
        public string _strOC_Cliente;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;

            cb.AddTemplate(pdfPage1, 0, 0);

            BaseFont fDetB = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont fDet = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            float flX = 67;
            float flY = 750;
            float flEspacio = 10;

            iTextSharp.text.Image imgEmpresa = iTextSharp.text.Image.GetInstance(_strMapPath + _strLogoEmpresa);
            imgEmpresa.SetAbsolutePosition(45, 720);
            imgEmpresa.ScalePercent(_flEscala, _flEscala);
            document.Add(imgEmpresa);

            StringBuilder strTemp = new StringBuilder();

            //          -------------------------- urx, ury
            //          |                        |
            //          |                        |
            //          |                        |
            //          |________________________|
            //       llx, lly

            //Dibujar linea para ver de donde a donde es
            //cb.MoveTo(50, 550);  
            //cb.LineTo(360, 660);
            //cb.Stroke();
            
            ColumnText ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(_strNombreEmisor,
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 8))),
                                                    140, 720, 410, 764, 10, Element.ALIGN_LEFT);
            ct.Go();
            
            strTemp.Clear();
            //strTemp.Append(_dirDomicilioFiscal.Direccion_Lineas_SinPais);
            strTemp.Append(_dirDomicilioFiscal.Direccion_Lineas);
            strTemp.Append(" RFC: " + CRutinas.RFC_Guiones(_strRFCEmisor));
            if (!string.IsNullOrEmpty(_strTelefono))
                strTemp.Append("\nTel. " + _strTelefono);
            if (!string.IsNullOrEmpty(_strEmail))
                strTemp.Append("\nEmail: " + _strEmail);
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    140, 620, 440, 750, 10, Element.ALIGN_LEFT); 
            ct.Go();

            ct = new ColumnText(cb);
            if (_swNotaCredito)
                ct.SetSimpleColumn(new Phrase(new Chunk("NOTA DE CRÉDITO",
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    430, 745, 567, 762, 10, Element.ALIGN_CENTER);
            else if (_swNotaCargo)
                ct.SetSimpleColumn(new Phrase(new Chunk("NOTA DE CARGO",
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    430, 745, 567, 762, 10, Element.ALIGN_CENTER);
            else
                ct.SetSimpleColumn(new Phrase(new Chunk("FACTURA",
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    430, 745, 567, 762, 10, Element.ALIGN_CENTER);
            ct.Go();

            strTemp.Clear();
            if(!string.IsNullOrEmpty(_strFactSerie))
                strTemp.Append("SERIE:\n\n");
            strTemp.Append("FOLIO:\n\n");
            strTemp.Append("FECHA:");
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    430, 620, 460, 750, 10, Element.ALIGN_RIGHT);
            ct.Go();

            strTemp.Clear();
            if (!string.IsNullOrEmpty(_strFactSerie))
                strTemp.Append(_strFactSerie + "\n\n");
            strTemp.Append(_strFactID + "\n\n");
            strTemp.Append(CRutinas.Fecha_MMMM_DD_YYYY_HoraSeg(_dtFecha));
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    465, 620, 567, 750, 10, Element.ALIGN_LEFT);
            ct.Go();

            strTemp.Clear();
            strTemp.Append("FACTURAR A:\n");
            strTemp.Append(_strNombreCliente + "\n");
            strTemp.Append(_dirDomicilioCliente.Direccion_Lineas + "\n");
            strTemp.Append("RFC: " + CRutinas.RFC_Guiones(_strRFCCliente) + "\n\n");
            if (_strContadoCredito.Equals("Contado"))
                strTemp.Append("CONDICIONES DE PAGO: " + _strContadoCredito + "\n");
            else
                strTemp.Append("CONDICIONES DE PAGO: " + _strContadoCredito + "  " + _strDiasCredito + " DÍAS\n");
            //strTemp.Append("NO. ORDEN DE COMPRA: " + _strOrdenCompra + "\n");
            strTemp.Append("OC Cliente: " + _strOC_Cliente + "\n");
            strTemp.Append("NO. COTIZACIÓN: " + _strCotizacion + "\n");

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    50, 580, 315, 685, 8, Element.ALIGN_LEFT);
            ct.Go();

            strTemp.Clear();
            strTemp.Append("ENVIAR A:\n");
            strTemp.Append(_objDirEnvio.Direccion_Lineas + "\n\n");
           // strTemp.Append("ATENCIÓN A: " + _strContacto + "\n");
            strTemp.Append("ATENCIÓN A: " + _strReferencia + "\n");
            strTemp.Append("NO. DE CLIENTE: " + _strNumeroCliente + "\n");
            strTemp.Append("EJECUTIVO DE VENTAS: " + _strEjecutivoVentas);

            strTemp.Append("\n\n" + _strComentarios);

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    320, 600, 567, 685, 8, Element.ALIGN_LEFT);

            ct.Go();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strFolioFiscal, 212, 335, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, CRutinas.Fecha_MMMM_DD_YYYY_HoraSeg(_dtFechaCertificado), 448, 335, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strCertificadoSAT, 270, 321, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strCertificadoEmisor, 455, 321, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strCertificado, 270, 307, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strAutorizacionPAC, 455, 307, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strRegimen, 225, 294, 0);
            cb.EndText();

            // Condiciones de pago
            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strContadoCredito, 455, 294, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strMetodoDePago, 230, 281, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strNumCtaPago, 455, 281, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 6.0f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _dirDomicilioExpedicion.StrLocalidad + "," + _dirDomicilioExpedicion.StrEstado, 355, 268, 0);
            cb.EndText();

            // TOTALES
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(_strTotalLetras.ToUpper(),
                                                          FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                          40, 380, 400, 425, 9, Element.ALIGN_LEFT);
            ct.Go();
            if (writer.PageNumber == _intTotalPaginas)
            {
                flX = 450;
                flY = 415;
                flEspacio = 8;
                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Subtotal:", flX, flY, 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, _amtSubTotal.ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                cb.EndText();

                if (_amtDescuento > 0)
                {
                    flY -= flEspacio;
                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Subtotal con descuento:", flX, flY, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, (_amtSubTotal - _amtDescuento).ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                    cb.EndText();
                }

                flY -= flEspacio;
                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "IVA " + _amtTasa.ToString("0.##") + "%:", flX, flY, 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, _amtImpuesto.ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                cb.EndText();

                if (_amtIVA_Retenido > 0)
                {
                    flY -= flEspacio;
                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Retención IVA:", flX, flY, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, _amtIVA_Retenido.ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                    cb.EndText();
                }

                if (_amtISR_Retenido > 0)
                {
                    flY -= flEspacio;
                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Retención ISR:", flX, flY, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(fDetB, 7f);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, _amtISR_Retenido.ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                    cb.EndText();
                }

                flY -= flEspacio;
                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total:", flX, flY, 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(fDetB, 7f);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, _amtTotal.ToString("c") + _strMonedaAbrev, flX + 110, flY, 0);
                cb.EndText();
            }

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(_strSelloCFDI,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 6))),
                                                    45, 230, 560, 250, 8, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(_strSelloSAT,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 6))),
                                                    45, 200, 560, 222, 8, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(_strCadenaOriginal,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 6))),
                                                    45, 150, 560, 195, 8, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            ct.Go();

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeVersion = 7;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            iTextSharp.text.Image imgCodigo = iTextSharp.text.Image.GetInstance(qrCodeEncoder.Encode(_strQRCodeData), BaseColor.BLACK);
            imgCodigo.SetAbsolutePosition(55, 270);
            document.Add(imgCodigo);

            cb.BeginText();
            cb.SetFontAndSize(fDet, 5f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _amtTotal.ToString("c") + _strMonedaAbrev, 115, 104, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 5f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                                            _dirDomicilioExpedicion.StrLocalidad + "," + _dirDomicilioExpedicion.StrEstado +
                                            ", " + _dtFecha.ToString("dd/MM/yyyy"),
                                            400, 104, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 5f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strTotalLetras.ToUpper(), 90, 90, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 5f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, _strNombreCliente, 45, 70, 0);
            cb.EndText();

            flX = 580;
            flY = 20;
            cb.BeginText();
            cb.SetFontAndSize(fDet, 7);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Hoja " + writer.PageNumber + " de " + _intTotalPaginas, flX, flY, 0);
            cb.EndText();

        }
    }
}
