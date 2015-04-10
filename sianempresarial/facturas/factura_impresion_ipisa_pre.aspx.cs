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

public partial class facturas_factura_impresion_ipisa_pre : BasePagePopUp
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
        strOrdenCompra = strProveedor = strFact = string.Empty;
        if (Request.QueryString["factID"] != null)
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

                Generar_Factura();
            }
        }
        // Nota Cargo
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
                Generar_Factura();
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
                Generar_Factura();
            }
        }
        else
            this.lblMessage.Text = "Documento no existe";
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

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        this.strTelefono = objRowResult["telefono_impresion"].ToString();
        this.strEmail = objRowResult["email_impresion"].ToString();

        CFacturaDB objFacturaDB = new CFacturaDB();
        CNotaCreditoDB objNotaCRDB = new CNotaCreditoDB();
        CNotaCargoDB objNotaDB = new CNotaCargoDB();
        if (swNotaCredito)
        {
            if (!objNotaCRDB.Leer(int.Parse(Request.QueryString["notaCRID"])))
                return;
        }
        else if (swNotaCargo)
        {
            if (!objNotaDB.Leer(int.Parse(Request.QueryString["notaID"])))
                return;
        }
        else
        {
            if (!objFacturaDB.Leer(int.Parse(Request.QueryString["factID"])))
                return;
        }

        string strSucID;
        decimal dcmTotal, dcmPorISR_Ret, dcmISR_Ret, dcmPorIVA_Ret, dcmIVA_Ret, dcmTotalRetenidos, dcmPorIva, dcmIva;
        decimal dcmDescuento, dcmPorcDescuento, dcmPorcDescuento2, dcmSubtotal;

        dcmDescuento = 0;
        strPorcDescuento = string.Empty;
        if (swNotaCredito)
        {
            objFactura.IntFolio = objNotaCRDB.folio;
            if (objNotaCRDB.contado)
                strContadoCredito = "Contado";
            else
                strContadoCredito = "Crédito";
            strSucID = objNotaCRDB.sucursal_ID.ToString();
            dcmTotal = objNotaCRDB.total;
            dcmPorISR_Ret = objNotaCRDB.isr_ret;
            dcmISR_Ret = objNotaCRDB.monto_isr_ret;
            dcmPorIVA_Ret = objNotaCRDB.iva_ret;
            dcmIVA_Ret = objNotaCRDB.monto_iva_ret;
            dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            dcmPorIva = objNotaCRDB.iva;
            dcmIva = objNotaCRDB.monto_iva;
            dcmPorcDescuento = objNotaCRDB.descuento;
            dcmPorcDescuento2 = objNotaCRDB.descuento2;
            dcmSubtotal = objNotaCRDB.monto_subtotal;

            objFactura.StrMoneda = objNotaCRDB.moneda;
            objFactura.AmtTipoCambio = objNotaCRDB.tipo_cambio;

            if (dcmPorcDescuento > 0)
            {
                dcmDescuento = dcmSubtotal - objNotaCRDB.monto_descuento;
                strPorcDescuento = dcmPorcDescuento.ToString("0.##") + "%";
                if (dcmPorcDescuento2 > 0)
                    strPorcDescuento += " y " + dcmPorcDescuento2.ToString("0.##") + "%";
                strPorcDescuento += " descuento";
            }

            objFactura.DtFecha = objNotaCRDB.fecha;
        }
        else if (swNotaCargo)
        {
            objFactura.IntFolio = objNotaDB.folio;
            if (objNotaDB.contado)
                strContadoCredito = "Contado";
            else
                strContadoCredito = "Crédito";
            strSucID = objNotaDB.sucursal_ID.ToString();
            dcmTotal = objNotaDB.total;
            dcmPorISR_Ret = objNotaDB.isr_ret;
            dcmISR_Ret = objNotaDB.monto_isr_ret;
            dcmPorIVA_Ret = objNotaDB.iva_ret;
            dcmIVA_Ret = objNotaDB.monto_iva_ret;
            dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            dcmPorIva = objNotaDB.iva;
            dcmIva = objNotaDB.monto_iva;
            dcmPorcDescuento = objNotaDB.descuento;
            dcmPorcDescuento2 = objNotaDB.descuento2;
            dcmSubtotal = objNotaDB.monto_subtotal;

            objFactura.StrMoneda = objNotaDB.moneda;
            objFactura.AmtTipoCambio = objNotaDB.tipo_cambio;

            if (dcmPorcDescuento > 0)
            {
                dcmDescuento = dcmSubtotal - objNotaDB.monto_descuento;
                strPorcDescuento = dcmPorcDescuento.ToString("0.##") + "%";
                if (dcmPorcDescuento2 > 0)
                    strPorcDescuento += " y " + dcmPorcDescuento2.ToString("0.##") + "%";
                strPorcDescuento += " descuento";
            }

            objFactura.DtFecha = objNotaDB.fecha;
        }
        else
        {
            objFactura.IntFolio = objFacturaDB.folio;
            objFactura.StrSerie = objFacturaDB.factura_suf;
            if (objFacturaDB.contado)
                strContadoCredito = "Contado";
            else
                strContadoCredito = "Crédito";
            strSucID = objFacturaDB.sucursal_ID.ToString();
            dcmTotal = objFacturaDB.total;
            dcmPorISR_Ret = objFacturaDB.isr_ret;
            dcmISR_Ret = objFacturaDB.monto_isr_ret;
            dcmPorIVA_Ret = objFacturaDB.iva_ret;
            dcmIVA_Ret = objFacturaDB.monto_iva_ret;
            dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
            dcmPorIva = objFacturaDB.iva;
            dcmIva = objFacturaDB.monto_iva;
            dcmPorcDescuento = objFacturaDB.descuento;
            dcmPorcDescuento2 = objFacturaDB.descuento2;
            dcmSubtotal = objFacturaDB.monto_subtotal;

            objFactura.StrMoneda = objFacturaDB.moneda;
            objFactura.AmtTipoCambio = objFacturaDB.tipo_cambio;

            if (dcmPorcDescuento > 0)
            {
                dcmDescuento = dcmSubtotal - objFacturaDB.monto_descuento;
                strPorcDescuento = dcmPorcDescuento.ToString("0.##") + "%";
                if (dcmPorcDescuento2 > 0)
                    strPorcDescuento += " y " + dcmPorcDescuento2.ToString("0.##") + "%";
                strPorcDescuento += " descuento";
            }

            objFactura.DtFecha = objFacturaDB.fecha;
        }

        objFactura.AmtTotal = dcmTotal;

        if (dcmPorIva != 5)
            objFactura.AmtSubTotal = dcmSubtotal;

        if (dcmDescuento != 0)
            objFactura.AmtDescuento = dcmDescuento;

        objFactura.AmtTasa_ISR_Retenido = dcmPorISR_Ret;
        objFactura.AmtTasa_IVA_Retenido = dcmPorIVA_Ret;

        objFactura.AmtISR_Retenido = dcmISR_Ret;
        objFactura.AmtIVA_Retenido = dcmIVA_Ret;

        objFactura.StrNombreEmisor = objRowResult["razonsocial"].ToString();
        objFactura.StrRFCEmisor = objRowResult["rfc"].ToString();
        objFactura.StrRegimen = objRowResult["regimen"].ToString();

        objFactura.DirDomicilioFiscal.StrCalle = objRowResult["Dom_Calle"].ToString();
        objFactura.DirDomicilioFiscal.StrNumeroExterior = objRowResult["Dom_NumExt"].ToString();
        if (!string.IsNullOrEmpty(objRowResult["Dom_NumInt"].ToString()))
            objFactura.DirDomicilioFiscal.StrNumeroInterior = objRowResult["Dom_NumInt"].ToString();
        objFactura.DirDomicilioFiscal.StrColonia = objRowResult["Dom_Colonia"].ToString();
        objFactura.DirDomicilioFiscal.StrLocalidad = objRowResult["Dom_Localidad"].ToString();
        objFactura.DirDomicilioFiscal.StrMunicipio = objRowResult["Dom_Municipio"].ToString();
        objFactura.DirDomicilioFiscal.StrEstado = objRowResult["Dom_Estado"].ToString();
        objFactura.DirDomicilioFiscal.StrPais = objRowResult["Dom_Pais"].ToString();
        objFactura.DirDomicilioFiscal.StrCP = objRowResult["Dom_CP"].ToString();

        objFactura.DirDomicilioExpedicion.StrCalle = objRowResult["Exp_Calle"].ToString();
        objFactura.DirDomicilioExpedicion.StrNumeroExterior = objRowResult["Exp_NumExt"].ToString();
        if (!string.IsNullOrEmpty(objRowResult["Exp_NumInt"].ToString()))
            objFactura.DirDomicilioExpedicion.StrNumeroInterior = objRowResult["Exp_NumInt"].ToString();
        objFactura.DirDomicilioExpedicion.StrColonia = objRowResult["Exp_Colonia"].ToString();
        objFactura.DirDomicilioExpedicion.StrLocalidad = objRowResult["Exp_Localidad"].ToString();
        objFactura.DirDomicilioExpedicion.StrMunicipio = objRowResult["Exp_Municipio"].ToString();
        objFactura.DirDomicilioExpedicion.StrEstado = objRowResult["Exp_Estado"].ToString();
        objFactura.DirDomicilioExpedicion.StrPais = objRowResult["Exp_Pais"].ToString();
        objFactura.DirDomicilioExpedicion.StrCP = objRowResult["Exp_CP"].ToString();

        if (!(bool)objRowResult["comentarios_en_factura"])
            strComentarios = string.Empty;

        objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT S.sucursal as sucursal, " +
                                                    "S.numero_codificacion as codificacion, " +
                                                    "E.razonsocial as negocio, " +
                                                    "E.direccionfiscal as direccionfiscal, " +
                                                    "E.num_exterior as num_exterior, " +
                                                    "E.num_interior as num_interior, " +
                                                    "E.colonia as colonia, " +
                                                    "E.cp as cp, " +
                                                    "E.poblacion as poblacion, " +
                                                    "E.municipio as municipio, " +
                                                    "E.estado as estado, " +
                                                    "E.pais as pais, " +
                                                    "E.rfc as rfc " +
                                                    "FROM sucursales S " +
                                                    "INNER JOIN establecimientos E " +
                                                    "ON S.establecimiento_ID = E.ID " +
                                                    "WHERE S.ID = " + strSucID);

        objRowResult = objDataResult.Tables[0].Rows[0];

        objFactura.StrNombreCliente = objRowResult["negocio"].ToString();
        objFactura.StrRFCCliente = objRowResult["rfc"].ToString();

        objFactura.DirDomicilioCliente.StrCalle = objRowResult["direccionfiscal"].ToString();
        objFactura.DirDomicilioCliente.StrNumeroExterior = objRowResult["num_exterior"].ToString();
        if (!string.IsNullOrEmpty(objRowResult["num_interior"].ToString()))
            objFactura.DirDomicilioCliente.StrNumeroInterior = objRowResult["num_interior"].ToString();
        objFactura.DirDomicilioCliente.StrColonia = objRowResult["colonia"].ToString();
        objFactura.DirDomicilioCliente.StrLocalidad = objRowResult["poblacion"].ToString();
        objFactura.DirDomicilioCliente.StrMunicipio = objRowResult["municipio"].ToString();
        objFactura.DirDomicilioCliente.StrEstado = objRowResult["estado"].ToString();
        objFactura.DirDomicilioCliente.StrPais = objRowResult["pais"].ToString();
        objFactura.DirDomicilioCliente.StrCP = objRowResult["CP"].ToString();

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
                                                        " AND F.nota_ID = " + Request.QueryString["notaID"] +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.nota_ID = " + Request.QueryString["notaID"] +
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
                                                        " AND F.nota_ID = " + Request.QueryString["notaCRID"] +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.nota_ID = " + Request.QueryString["notaCRID"] +
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
                                                        " AND F.factura_ID = " + Request.QueryString["factID"] +
                                                        " LEFT JOIN productos P2" +
                                                        " ON P2.ID = F.grupoID" +
                                                        " LEFT JOIN establecimiento_producto S" +
                                                        " ON S.productoID = F.producto_ID" +
                                                        " AND S.establecimientoID = " + strEstabID +
                                                        " WHERE F.factura_ID = " + Request.QueryString["factID"] +
                                                        " ORDER BY consecutivo, nombre" +
                                                        ";" +
                                                        " SELECT consecutivo, producto_ID, lote, fecha_caducidad" +
                                                        " FROM facturas_prod_lote" +
                                                        " WHERE factura_ID = " + Request.QueryString["factID"] +
                                                        " ORDER BY consecutivo, producto_ID");

        List<CProducto> lstProductos = new List<CProducto>();
        StringBuilder strLotes = new StringBuilder();
        string strUniMed = string.Empty;
        string strGrupoConsecutivo = string.Empty;

        foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
        {
            strUniMed = objRowResult2["unimed"].ToString();

            // Si es factura
            if (!swNotaCargo && !swNotaCredito)
            {
                // Se puede usar la unidad original o la alterna
                if (!(bool)objRowResult2["unimed_original"])
                {
                    if ((bool)objRowResult2["usar_unimed2"])
                        strUniMed = objRowResult2["unimed2"].ToString();
                }

                if (!string.IsNullOrEmpty(objRowResult2["grupoID"].ToString()))
                {
                    if (!strGrupoConsecutivo.Equals(objRowResult2["grupoID"].ToString() + "_" + objRowResult2["grupo_consecutivo"].ToString()))
                    {
                        CProducto objProductoKit = new CProducto();
                        objProductoKit.StrDescripcion = objRowResult2["producto_grupo"].ToString() + ", Cant: " +
                                                        ((decimal)objRowResult2["grupo_cantidad"]).ToString("0.##");
                        objProductoKit.SwKit = true;
                        strGrupoConsecutivo = objRowResult2["grupoID"].ToString() + "_" + objRowResult2["grupo_consecutivo"].ToString();
                        lstProductos.Add(objProductoKit);
                    }
                }
            }

            CProducto objProducto = new CProducto();
            objProducto.StrDescripcion = objRowResult2["nombre"].ToString();
            objProducto.AmtCantidad = (decimal)objRowResult2["cantidad"];
            objProducto.AmtValorUnitario = (decimal)objRowResult2["costo_unitario"];
            objProducto.AmtImporte = (decimal)objRowResult2["costo"];
            objProducto.StrUnidad = strUniMed;

            if (!swNotaCargo && !swNotaCredito)
            {
                if (!string.IsNullOrEmpty(objRowResult2["grupoID"].ToString()))
                    objProducto.IntGrupoID = (int)objRowResult2["grupoID"];
            }

            if ((bool)objRowResult2["usar_detalle"])
            {
                objProducto.StrDescripcion = objRowResult2["detalle"].ToString();
            }
            else
            {
                if ((bool)objRowResult2["imprimir_detalle"])
                    objProducto.StrDetalle = objRowResult2["detalle"].ToString();
            }

            if (!swNotaCargo && !swNotaCredito)
            {
                DataRow[] dtLotes = objDataResult.Tables[1].Select(
                                                        "consecutivo = " + objRowResult2["consecutivo"].ToString());
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

            objProducto.StrCodigo = objRowResult2["codigo"].ToString();
            objProducto.StrClaveCliente = objRowResult2["clave"].ToString();

            lstProductos.Add(objProducto);
        }
        objFactura.LstProductos = lstProductos;

        if (dcmPorIva > 0)
        {
            objFactura.AmtTasa = dcmPorIva;
            objFactura.AmtImpuesto = dcmIva;
        }

        objFactura.StrMetodoDePago = strMetodoPago;
        objFactura.StrNumCtaPago = strNumCtaPago;
        objFactura.StrVersionCFDI = "3.2";
        objFactura.StrFolioFiscal = "N/A";
        objFactura.StrCertificadoSAT = "N/A";
        objFactura.StrSelloCFDI = "N/A";
        objFactura.StrSelloSAT = "N/A";
        objFactura.DtFechaCertificado = DateTime.Now;

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

            texto = new Paragraph(objProducto.AmtValorUnitario.ToString("c"), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.AmtImporte.ToString("c"), ftProductos);
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
        events._strCadenaOriginal = "N/A";
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
                                                    180, 720, 410, 764, 10, Element.ALIGN_CENTER);
            ct.Go();

            strTemp.Clear();
            strTemp.Append(_dirDomicilioFiscal.Direccion_Lineas_SinPais);
            strTemp.Append(" RFC: " + CRutinas.RFC_Guiones(_strRFCEmisor));
            if (!string.IsNullOrEmpty(_strTelefono))
                strTemp.Append("\nTel. " + _strTelefono);
            if (!string.IsNullOrEmpty(_strEmail))
                strTemp.Append("\nEmail: " + _strEmail);
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    200, 620, 440, 750, 10, Element.ALIGN_CENTER);
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
            if (!string.IsNullOrEmpty(_strFactSerie))
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

            //QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //qrCodeEncoder.QRCodeScale = 2;
            //qrCodeEncoder.QRCodeVersion = 7;
            //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //iTextSharp.text.Image imgCodigo = iTextSharp.text.Image.GetInstance(qrCodeEncoder.Encode(_strQRCodeData), BaseColor.BLACK);
            //imgCodigo.SetAbsolutePosition(55, 270);
            //document.Add(imgCodigo);

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
