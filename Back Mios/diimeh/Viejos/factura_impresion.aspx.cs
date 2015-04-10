using System;
using System.Collections.Generic;
using System.Globalization;
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

public partial class facturas_factura_impresion : System.Web.UI.Page
{
    CFacturaElectronica objFactura;
    float flAjusteOficio;
    string strRenglones;
    string strDireccion_Entrega;
    string strRepartidor;
    string strNegocioID;
    string strOrdenCompra;
    string strReferencia;
    string strProveedor;
    string strFact;
    string strFactID;
    StringBuilder strNotas;
    string strNumCtaPago;
    string strBanco;
    string strMetodoPago;
    string strSustituye;
    string strVendedor;
    string strQRCodeData;
    string strCertificadoEmisor;
    string strTelefono;
    string strCobro;
    string strContrarecibo;
    string strTotalFactura;

    protected void Page_Load(object sender, EventArgs e)
    {
        strOrdenCompra = strProveedor = strFact = string.Empty;
        if (Request.QueryString["fact"] != null)
        {
            strFact = "CFDI_" + Request.QueryString["fact"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.ID, F.orden_compra, F.referencia, E.proveedor, E.cuenta_bancaria " +
                    " ,E.banco, M.metodo_pago, E.ID as negocioID " +
                    " ,E.direccion_entrega1, E.direccion_entrega2, E.direccion_entrega3, E.telefono" +
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
                    " WHERE F.factura = '" + Request.QueryString["fact"].Replace(".xml", "") + "'" + ";" +
                    " SELECT CONCAT(C.nombre, ' ' , C.apellidos) as cobrador" +                 // Table 1
                    " ,A.direccion_ID" +
                    " FROM facturas_liq F " +
                    " INNER JOIN facturas_liq_anexo A" +
                    " ON F.ID = A.factura_ID " +
                    " AND F.factura = '" + Request.QueryString["fact"].Replace(".xml", "") + "'" +
                    " LEFT JOIN personas C" +
                    " ON A.cobrador_ID = C.ID;" +
                    " SELECT R.tipo, R.dia_semana, R.dia, R.fecha, R.comentarios" +             // Tabla 2
                    ",R.frecuencia, S.frecuencia as especial" +
                    " FROM" +
                    " (" +
                    "    SELECT C.*" +
                    "    FROM cobranza_dias C" +
                    "    WHERE establecimientoID =" +
                    "    (" +
                    "       SELECT S.establecimiento_ID" +
                    "       FROM facturas_liq F" +
                    "       INNER JOIN sucursales S" +
                    "       ON F.sucursal_ID = S.ID" +
                    "       AND F.factura = '" + Request.QueryString["fact"].Replace(".xml", "") + "'" +
                    "     )" +
                    " ) AS R" +
                    " INNER JOIN cobranza_frecuencia_especial S" +
                    " ON S.ID = R.especial" +
                    " ORDER BY tipo, R.frecuencia, dia_semana, dia, fecha";
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
                strOrdenCompra = objRowResult["orden_compra"].ToString();
                strReferencia = objRowResult["referencia"].ToString();
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                strTelefono = objRowResult["telefono"].ToString();
                if ((int)objRowResult["vendID"] == 5)
                    strVendedor = objRowResult["vendedor"].ToString();
                else
                    strVendedor = string.Empty;

                if (objDataResult.Tables[1].Rows.Count > 0)
                {
                    strRepartidor = objDataResult.Tables[1].Rows[0]["cobrador"].ToString();

                    switch ((byte)objDataResult.Tables[1].Rows[0]["direccion_ID"])
                    {
                        case 1:
                            strDireccion_Entrega = objRowResult["direccion_entrega1"].ToString();
                            break;
                        case 2:
                            strDireccion_Entrega = objRowResult["direccion_entrega2"].ToString();
                            break;
                        case 3:
                            strDireccion_Entrega = objRowResult["direccion_entrega3"].ToString();
                            break;
                    }
                }

                if (objDataResult.Tables[2].Rows.Count > 0)
                {
                    StringBuilder strCobro = new StringBuilder();
                    StringBuilder strContrarecibo = new StringBuilder();
                    StringBuilder strTemp = new StringBuilder();

                    foreach (DataRow objRow in objDataResult.Tables[2].Rows)
                    {
                        strTemp.Clear();
                        switch ((byte)objRow["frecuencia"])
                        {
                            case 1: //Diario
                                strTemp.Append("Diario L-V");
                                break;
                            case 2: //Semanal
                                strTemp.Append("Cada " + CultureInfo.CreateSpecificCulture("es-MX").DateTimeFormat.GetDayName((DayOfWeek)(byte)objRow["dia_semana"]));
                                break;
                            case 3: //Mensual
                                strTemp.Append("El día " + (byte)objRow["dia"] + " de cada mes");
                                break;
                            case 4: //Especial
                                strTemp.Append(objRow["especial"].ToString() + " " +
                                    CultureInfo.CreateSpecificCulture("es-MX").DateTimeFormat.GetDayName((DayOfWeek)(byte)objRow["dia_semana"]) +
                                    " de cada mes");
                                break;
                            case 5: //N días
                                strTemp.Append("Cada " + (byte)objRow["dia"] +
                                               " " + objRow["especial"].ToString());
                                break;
                            case 6: //Calendario
                                strTemp.Append(((DateTime)objRow["fecha"]).ToString("ddd dd MMM yy", CultureInfo.CreateSpecificCulture("es-MX")));
                                break;
                        }
                        if (!string.IsNullOrEmpty(objRow["comentarios"].ToString()))
                        {
                            strTemp.Append(", " + objRow["comentarios"].ToString());
                        }
                        if ((byte)objRow["tipo"] == 1)
                        {
                            if (strContrarecibo.Length > 0)
                                strContrarecibo.Append("; ");
                            strContrarecibo.Append(strTemp.ToString());
                        }
                        else
                        {
                            if (strCobro.Length > 0)
                                strCobro.Append("; ");
                            strCobro.Append(strTemp.ToString());
                        }
                    }

                    this.strCobro = strCobro.ToString();
                    this.strContrarecibo = strContrarecibo.ToString();
                }
            }
        }
        else
            if (Request.QueryString["factID"] != null)
            {
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT F.factura, F.orden_compra, F.referencia, E.proveedor " +
                        " ,E.cuenta_bancaria, E.banco, M.metodo_pago, E.ID as negocioID " +
                        " ,E.direccion_entrega1, E.direccion_entrega2, E.direccion_entrega3" +
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
                        " WHERE F.ID = " + Request.QueryString["factID"] + ";" +
                        " SELECT CONCAT(C.nombre, ' ' , C.apellidos) as cobrador" +                 // Tabla 1
                        " ,F.direccion_ID" +
                        " FROM facturas_liq_anexo F" +
                        " INNER JOIN personas C" +
                        " ON F.cobrador_ID = C.ID" +
                        " AND F.factura_ID = " + Request.QueryString["factID"] + ";" +
                        " SELECT R.tipo, R.dia_semana, R.dia, R.fecha, R.comentarios" +             // Tabla 2
                        ",R.frecuencia, S.frecuencia as especial" +
                        " FROM" +
                        " (" +
                        "    SELECT C.*" +
                        "    FROM cobranza_dias C" +
                        "    WHERE establecimientoID =" +
                        "    (" +
                        "       SELECT S.establecimiento_ID" +
                        "       FROM facturas_liq F" +
                        "       INNER JOIN sucursales S" +
                        "       ON F.sucursal_ID = S.ID" +
                        "       AND F.ID = " + Request.QueryString["factID"] +
                        "     )" +
                        " ) AS R" +
                        " INNER JOIN cobranza_frecuencia_especial S" +
                        " ON S.ID = R.especial" +
                        " ORDER BY tipo, R.frecuencia, dia_semana, dia, fecha";
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
                    strOrdenCompra = objRowResult["orden_compra"].ToString();
                    strReferencia = objRowResult["referencia"].ToString();
                    strProveedor = objRowResult["proveedor"].ToString();
                    strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                    strBanco = objRowResult["banco"].ToString();
                    strMetodoPago = objRowResult["metodo_pago"].ToString();
                    strNegocioID = objRowResult["negocioID"].ToString();
                    if ((int)objRowResult["vendID"] == 5)
                        strVendedor = objRowResult["vendedor"].ToString();
                    else
                        strVendedor = string.Empty;

                    if (objDataResult.Tables[1].Rows.Count > 0)
                    {
                        strRepartidor = objDataResult.Tables[1].Rows[0]["cobrador"].ToString();

                        switch ((byte)objDataResult.Tables[1].Rows[0]["direccion_ID"])
                        {
                            case 1:
                                strDireccion_Entrega = objRowResult["direccion_entrega1"].ToString();
                                break;
                            case 2:
                                strDireccion_Entrega = objRowResult["direccion_entrega2"].ToString();
                                break;
                            case 3:
                                strDireccion_Entrega = objRowResult["direccion_entrega3"].ToString();
                                break;
                        }
                    }

                    if (objDataResult.Tables[2].Rows.Count > 0)
                    {
                        StringBuilder strCobro = new StringBuilder();
                        StringBuilder strContrarecibo = new StringBuilder();
                        StringBuilder strTemp = new StringBuilder();

                        foreach (DataRow objRow in objDataResult.Tables[2].Rows)
                        {
                            strTemp.Clear();
                            switch ((byte)objRow["frecuencia"])
                            {
                                case 1: //Diario
                                    strTemp.Append("Diario L-V");
                                    break;
                                case 2: //Semanal
                                    strTemp.Append("Cada " + CultureInfo.CreateSpecificCulture("es-MX").DateTimeFormat.GetDayName((DayOfWeek)(byte)objRow["dia_semana"]));
                                    break;
                                case 3: //Mensual
                                    strTemp.Append("El día " + (byte)objRow["dia"] + " de cada mes");
                                    break;
                                case 4: //Especial
                                    strTemp.Append(objRow["especial"].ToString() + " " +
                                        CultureInfo.CreateSpecificCulture("es-MX").DateTimeFormat.GetDayName((DayOfWeek)(byte)objRow["dia_semana"]) +
                                        " de cada mes");
                                    break;
                                case 5: //N días
                                    strTemp.Append("Cada " + (byte)objRow["dia"] +
                                                   " " + objRow["especial"].ToString());
                                    break;
                                case 6: //Calendario
                                    strTemp.Append(((DateTime)objRow["fecha"]).ToString("ddd dd MMM yy", CultureInfo.CreateSpecificCulture("es-MX")));
                                    break;
                            }
                            if (!string.IsNullOrEmpty(objRow["comentarios"].ToString()))
                            {
                                strTemp.Append(", " + objRow["comentarios"].ToString());
                            }
                            if ((byte)objRow["tipo"] == 1)
                            {
                                if (strContrarecibo.Length > 0)
                                    strContrarecibo.Append("; ");
                                strContrarecibo.Append(strTemp.ToString());
                            }
                            else
                            {
                                if (strCobro.Length > 0)
                                    strCobro.Append("; ");
                                strCobro.Append(strTemp.ToString());
                            }
                        }

                        this.strCobro = strCobro.ToString();
                        this.strContrarecibo = strContrarecibo.ToString();
                    }
                }
            }
        if (File.Exists(Server.MapPath("../xml_facturas" + "/" + strFact)))
            Generar_Factura();
        else
            this.lblMessage.Text = "Factura Electrónica no existe";
    }

    private void Generar_Factura()
    {
        if (Request.QueryString["m"] != null)   // Se está enviado por correo
            flAjusteOficio = 0;
        else
            flAjusteOficio = 172.8f;    // 2.4 in
        Obtener_Datos();
        int intTotalPaginas = Crear_Archivo_Temporal();
        Crear_Archivo(intTotalPaginas);
    }

    private void Obtener_Datos()
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT isr_ret, iva_ret, monto_isr_ret, monto_iva_ret " +
                " FROM facturas_liq " +
                " WHERE factura = '" + strFact.Replace("CFDI_", "").Replace(".xml", "") + "'";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        objFactura = new CFacturaElectronica();

        XmlDocument xmlFactura = new XmlDocument();
        xmlFactura.Load(Server.MapPath("../xml_facturas" + "/" + strFact));

        XmlNodeList xmlComprobante = xmlFactura.GetElementsByTagName("cfdi:Comprobante");
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

        List<string> lstProdDetalle = new List<string>();
        strQuery = "SELECT F.usar_cve_gob, F.detalle " +
                  " FROM facturas_liq_prod F" +
                  " INNER JOIN productos P" +
                  " ON F.producto_ID = P.ID " +
                  " WHERE F.factura_ID = " + strFactID +
                  " ORDER BY consecutivo, nombre";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
        {
            if((bool)objRowResult2["usar_cve_gob"])
                lstProdDetalle.Add(objRowResult2["detalle"].ToString());
            else
                lstProdDetalle.Add(string.Empty);
        }

        List<CProducto> lstProductos = new List<CProducto>();
        for (int i = 0; i < xmlProductos.Count; i++)
        {
            CProducto objProducto = new CProducto();
            objProducto.StrDescripcion = xmlProductos[i].Attributes["descripcion"].Value;
            objProducto.AmtCantidad = Convert.ToDecimal(xmlProductos[i].Attributes["cantidad"].Value);
            objProducto.AmtValorUnitario = Convert.ToDecimal(xmlProductos[i].Attributes["valorUnitario"].Value);
            objProducto.AmtImporte = Convert.ToDecimal(xmlProductos[i].Attributes["importe"].Value);
            if (xmlProductos[i].Attributes["unidad"] != null)
                objProducto.StrUnidad = xmlProductos[i].Attributes["unidad"].Value;
            if (i < lstProdDetalle.Count)
                objProducto.StrDetalle = lstProdDetalle[i];
            lstProductos.Add(objProducto);
        }
        objFactura.LstProductos = lstProductos;
        strRenglones = xmlProductos.Count.ToString("#,##0");

        XmlNodeList xmlImpuestos = xmlFactura.GetElementsByTagName("cfdi:Traslado");
        if (xmlImpuestos.Count > 0)
        {
            objFactura.AmtTasa = Convert.ToDecimal(xmlImpuestos[0].Attributes["tasa"].Value);
            objFactura.AmtImpuesto = Convert.ToDecimal(xmlImpuestos[0].Attributes["importe"].Value);
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

        strQuery = "SELECT nota, nota_suf " +
                  " FROM nota_facturas F" +
                  " INNER JOIN notas N" +
                  " ON N.ID = F.nota_ID" +
                  " AND F.factura_ID = " + strFactID +
                  " ORDER BY nota, nota_suf";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        strNotas = new StringBuilder();
        foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
        {
            if (strNotas.Length > 0)
                strNotas.Append(", ");
            strNotas.Append(objRowResult2["nota"].ToString());
            if (!string.IsNullOrEmpty(objRowResult2["nota_suf"].ToString()))
                strNotas.Append("-" + objRowResult2["nota_suf"].ToString());
        }

        strQuery = "SELECT factura_IDAnt" +
                  " FROM facturas_reemplazo" +
                  " WHERE factura_ID = " + strFactID;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        if (objDataResult.Tables[0].Rows.Count > 0)
            strSustituye = "Sustituye a la factura " + objDataResult.Tables[0].Rows[0]["factura_IDAnt"].ToString();
        else
            strSustituye = string.Empty;
    }

    private int Crear_Archivo_Temporal()
    {
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height + flAjusteOficio));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f + flAjusteOficio);

        MemoryStream m = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        document.Open();

        float[] ancho_columnas = new float[5];
        ancho_columnas[0] = 55;
        ancho_columnas[1] = 95;
        ancho_columnas[2] = 245;
        ancho_columnas[3] = 73;
        ancho_columnas[4] = 60;

        foreach (CProducto objProducto in objFactura.LstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = 548;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;
            Paragraph texto = new Paragraph(objProducto.AmtCantidad.ToString("#.###"), ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrUnidad, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrDescripcion, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.AmtValorUnitario.ToString("C"), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.AmtImporte.ToString("C"), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            document.Add(tblLinea);

            if (!string.IsNullOrEmpty(objProducto.StrDetalle))
            {
                tblLinea = new PdfPTable(ancho_columnas);
                tblLinea.TotalWidth = 548;
                tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
                tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
                tblLinea.LockedWidth = true;

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_CENTER;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                char tab = '\u0009';
                texto = new Paragraph(objProducto.StrDetalle.Replace(tab.ToString(), "    "), ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                document.Add(tblLinea);
            }
        }

        document.Close();

        m.Close();

        return writer.PageNumber - 1;
    }

    private void Crear_Archivo(int intTotalPaginas)
    {
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        MyPageEvents events = new MyPageEvents();
        events._flAjusteOficio = flAjusteOficio;
        events._strRenglones = strRenglones;
        events._strDireccion_Entrega = strDireccion_Entrega;
        events._strRepartidor = strRepartidor;
        events._strNegocioID = strNegocioID;
        events._strFactID = strFactID;
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
        events._strFormaDePago = objFactura.StrMetodoDePago;
        events._strNumCtaPago = objFactura.StrNumCtaPago;
        events._strCondicionesDePago = objFactura.StrCondicionesDePago;
        events._amtSubTotal = objFactura.AmtSubTotal;
        events._amtDescuento = objFactura.AmtDescuento;
        events._amtTotal = objFactura.AmtTotal;
        events._dtFecha = objFactura.DtFecha;
        events._strTotalLetras = CRutinas.ObtenerImporteLetras(objFactura.AmtTotal);
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
        events._intTotalPaginas = intTotalPaginas;
        events._strOrdenCompra = strOrdenCompra;
        events._strReferencia = strReferencia;
        events._strProveedor = strProveedor;
        events._strNotas = strNotas.ToString();
        events._strBanco = strBanco;
        events._strSustituye = strSustituye;
        events._strVendedor = strVendedor;
        events._strQRCodeData = strQRCodeData;
        events._strTelefono = strTelefono;
        events._strCobro = strCobro;
        events._strContrarecibo = strContrarecibo;
        events._strTotalFactura = strTotalFactura;

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height + flAjusteOficio));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f + flAjusteOficio);

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
            if (File.Exists(Server.MapPath("../xml_facturas" + "/" + strFact)))
            {
                Response.Redirect("facturas_correo.aspx?t=0&ID=" + strFactID);
            }
            FileStream flArchivo = new FileStream(Server.MapPath("../xml_facturas" + "/" + strFact), FileMode.Create);
            writer = PdfWriter.GetInstance(document, flArchivo);
        }
        writer.PageEvent = events;

        document.Open();

        float[] ancho_columnas = new float[5];
        ancho_columnas[0] = 55;
        ancho_columnas[1] = 95;
        ancho_columnas[2] = 245;
        ancho_columnas[3] = 73;
        ancho_columnas[4] = 60;

        foreach (CProducto objProducto in objFactura.LstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = 548;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;
            Paragraph texto = new Paragraph(objProducto.AmtCantidad.ToString("#.###"), ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrUnidad, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrDescripcion, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.AmtValorUnitario.ToString("C"), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.AmtImporte.ToString("C"), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            document.Add(tblLinea);

            if (!string.IsNullOrEmpty(objProducto.StrDetalle))
            {
                tblLinea = new PdfPTable(ancho_columnas);
                tblLinea.TotalWidth = 548;
                tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
                tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
                tblLinea.LockedWidth = true;

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_CENTER;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                char tab = '\u0009';
                texto = new Paragraph(objProducto.StrDetalle.Replace(tab.ToString(), "    "), ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                texto = new Paragraph(string.Empty, ftProductos);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.Border = Rectangle.NO_BORDER;
                tblLinea.AddCell(celda);

                document.Add(tblLinea);
            }
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
            Response.Redirect("facturas_correo.aspx?t=0&ID=" + strFactID);
    }

    class MyPageEvents : PdfPageEventHelper
    {
        public float _flAjusteOficio;
        public string _strRenglones;
        public string _strDireccion_Entrega;
        public string _strRepartidor;
        public string _strNegocioID;
        public string _strFactID;
        public string _strMapPath;
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
        public int _intFolio;
        public DateTime _dtFechaCertificado;
        public int _intNoAprobacion;
        public int _intAnoAprobacion;
        public string _strFormaDePago;
        public string _strCondicionesDePago;
        public decimal _amtSubTotal;
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
        public DateTime _dtFecha;
        public string _strTelefono;
        public string _strCobro;
        public string _strContrarecibo;
        public string _strTotalFactura;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            StringBuilder strCadena = new StringBuilder();
            Font ftHeader = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL));
            Font ftFooter = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));
            Font ftHeaderB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD));
            Font ftFooterB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.BOLD));

            Rectangle page = document.PageSize;

            #region iTextSharp.text.Imagenes Fijas
            iTextSharp.text.Image imgFactura = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_grande.png");
            imgFactura.SetAbsolutePosition(25, 40 + _flAjusteOficio);
            imgFactura.ScalePercent(25.5f, 25f);
            document.Add(imgFactura);

            iTextSharp.text.Image imgProductos = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prods.png");
            imgProductos.SetAbsolutePosition(30, 270 + _flAjusteOficio);
            imgProductos.ScalePercent(25.2f, 25f);
            document.Add(imgProductos);

            iTextSharp.text.Image imgProductos2 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodshor.png");
            imgProductos2.SetAbsolutePosition(30, 560 + _flAjusteOficio);
            imgProductos2.ScalePercent(25.1f, 25f);
            document.Add(imgProductos2);

            iTextSharp.text.Image imgProductos3 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos3.SetAbsolutePosition(90, 270 + _flAjusteOficio);
            imgProductos3.ScalePercent(25.2f, 25f);
            document.Add(imgProductos3);

            iTextSharp.text.Image imgProductos4 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos4.SetAbsolutePosition(180, 270 + _flAjusteOficio);
            imgProductos4.ScalePercent(25.2f, 25f);
            document.Add(imgProductos4);

            iTextSharp.text.Image imgProductos5 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos5.SetAbsolutePosition(450, 270 + _flAjusteOficio);
            imgProductos5.ScalePercent(25.2f, 25f);
            document.Add(imgProductos5);

            iTextSharp.text.Image imgProductos6 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos6.SetAbsolutePosition(520, 270 + _flAjusteOficio);
            imgProductos6.ScalePercent(25.2f, 25f);
            document.Add(imgProductos6);

            #endregion

            #region Header
            PdfPTable tblHeaderA = new PdfPTable(1);
            PdfPTable tblHeaderB = new PdfPTable(1);
            PdfPTable tblHeaderC = new PdfPTable(1);
            tblHeaderA.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblHeaderA.DefaultCell.Border = Rectangle.NO_BORDER;
            tblHeaderA.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            tblHeaderB.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblHeaderB.DefaultCell.Border = Rectangle.NO_BORDER;
            tblHeaderB.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            tblHeaderC.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblHeaderC.DefaultCell.Border = Rectangle.NO_BORDER;
            tblHeaderC.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            float[] ancho_columnas = new float[3];
            ancho_columnas[0] = 120;
            ancho_columnas[1] = tblHeaderA.TotalWidth - 280;
            ancho_columnas[2] = 160;
            PdfPTable tblHeader1 = new PdfPTable(ancho_columnas);

            // iTextSharp.text.Imagen de la empresa
            iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(_strMapPath + "LogoEmpresaFactura.png");
            PdfPCell celdaImg = new PdfPCell(img1);
            celdaImg.Border = Rectangle.NO_BORDER;
            celdaImg.VerticalAlignment = Element.ALIGN_LEFT;
            celdaImg.HorizontalAlignment = Element.ALIGN_MIDDLE;
            tblHeader1.AddCell(celdaImg);

            // Datos Fiscales

            Paragraph texto = new Paragraph("D I I M E H" + "\n", ftHeader);
            texto.Add(new Paragraph("Distribuidora de Insumos Médicos y Hospitalarios", ftHeader));
            texto.Add(new Paragraph(_strNombreEmisor, ftHeader));
            strCadena.Append(_dirDomicilioFiscal.StrCalle);
            strCadena.Append(" ");
            strCadena.Append(_dirDomicilioFiscal.StrNumeroExterior);
            if (!string.IsNullOrEmpty(_dirDomicilioFiscal.StrNumeroInterior))
            {
                strCadena.Append("-");
                strCadena.Append(_dirDomicilioFiscal.StrNumeroInterior);
            }
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioFiscal.StrColonia);
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));

            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioFiscal.StrLocalidad);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioFiscal.StrEstado);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioFiscal.StrPais);
            strCadena.Append(", C.P. ");
            strCadena.Append(_dirDomicilioFiscal.StrCP);
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));

            texto.Add(new Paragraph("RFC " + _strRFCEmisor, ftHeader));
            texto.Add(new Paragraph("Cuenta Banamex 142/339573 ", ftHeader));
            texto.Add(new Paragraph("Clabe 002164014203395730", ftHeader));
            texto.Add(new Paragraph("Actividad empresarial y profesional", ftHeader));

            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);

            // Datos de la factura
            texto = new Paragraph("Folio: ", ftHeaderB);
            texto.Add(new Paragraph(_strFactID, ftHeader));
            texto.Add(new Paragraph("Folio Fiscal:", ftHeaderB));
            texto.Add(new Paragraph(_strFolioFiscal, ftHeader));
            texto.Add(new Paragraph("No de Serie del Certificado del SAT:", ftHeaderB));
            texto.Add(new Paragraph(_strCertificado, ftHeader));
            texto.Add(new Paragraph("Fecha y hora de certificación:", ftHeaderB));
            texto.Add(new Paragraph(_dtFechaCertificado.ToString("yyyy-MM-dd HH:mm:ss"), ftHeader));
            texto.Add(new Paragraph("No de Serie del Certificado del CSD:", ftHeaderB));
            texto.Add(new Paragraph(_strCertificadoEmisor, ftHeader));
            texto.Add(new Paragraph("Hoja: " + document.PageNumber.ToString() +
                                "/" + _intTotalPaginas.ToString(), ftHeader));
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblHeader1.AddCell(celda);

            tblHeaderA.AddCell(tblHeader1);

            //Datos Expedición
            float[] ancho_columnas2 = new float[3];
            ancho_columnas2[0] = 200;
            ancho_columnas2[1] = tblHeaderB.TotalWidth - 350;
            ancho_columnas2[2] = 150;
            PdfPTable tblHeader2 = new PdfPTable(ancho_columnas2);
            texto = new Paragraph("Lugar de expedición:\n", ftHeaderB);
            texto.Add(new Paragraph(_dirDomicilioExpedicion.StrReferencia, ftHeader));
            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioExpedicion.StrCalle);
            strCadena.Append(" ");
            strCadena.Append(_dirDomicilioExpedicion.StrNumeroExterior);
            if (!string.IsNullOrEmpty(_dirDomicilioExpedicion.StrNumeroInterior))
            {
                strCadena.Append("-");
                strCadena.Append(_dirDomicilioExpedicion.StrNumeroInterior);
            }
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));
            texto.Add(new Paragraph(_dirDomicilioExpedicion.StrColonia, ftHeader));

            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioExpedicion.StrLocalidad);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioExpedicion.StrEstado);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioExpedicion.StrPais);
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));

            texto.Add(new Paragraph("C.P. " + _dirDomicilioExpedicion.StrCP, ftHeader));

            texto.Add(new Paragraph(string.Empty, ftHeader));
            texto.Add(new Paragraph("Fecha de elaboración:", ftHeaderB));
            texto.Add(new Paragraph(_dtFecha.ToString("dd-MMMM-yyyy",
                   System.Globalization.CultureInfo.CreateSpecificCulture("es-MX")), ftHeader));

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            //Datos del cliente
            texto = new Paragraph("Receptor:\n", ftHeaderB);
            texto.Add(new Paragraph(_strNombreCliente, ftHeader));
            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioCliente.StrCalle);
            strCadena.Append(" ");
            strCadena.Append(_dirDomicilioCliente.StrNumeroExterior);
            if (!string.IsNullOrEmpty(_dirDomicilioCliente.StrNumeroInterior))
            {
                strCadena.Append("-");
                strCadena.Append(_dirDomicilioCliente.StrNumeroInterior);
            }
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));
            texto.Add(new Paragraph(_dirDomicilioCliente.StrColonia, ftHeader));

            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioCliente.StrLocalidad);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioCliente.StrEstado);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioCliente.StrPais);
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));

            texto.Add(new Paragraph("C.P. " + _dirDomicilioCliente.StrCP, ftHeader));

            texto.Add(new Paragraph("RFC " + _strRFCCliente, ftHeader));

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph("Forma de pago:\n", ftHeaderB);
            texto.Add(new Paragraph(_strFormaDePago, ftHeader));

            if (!string.IsNullOrEmpty(_strProveedor) ||
                !string.IsNullOrEmpty(_strOrdenCompra) ||
                !string.IsNullOrEmpty(_strReferencia) ||
                !string.IsNullOrEmpty(_strNotas) ||
                !string.IsNullOrEmpty(_strNumCtaPago) ||
                !string.IsNullOrEmpty(_strBanco) ||
                !string.IsNullOrEmpty(_strSustituye) ||
                !string.IsNullOrEmpty(_strVendedor))
            {
                if (!string.IsNullOrEmpty(_strNumCtaPago))
                    texto.Add(new Paragraph("Terminación cuenta bancaria: " + _strNumCtaPago, ftHeader));
                if (!string.IsNullOrEmpty(_strBanco))
                    texto.Add(new Paragraph("Banco: " + _strBanco, ftHeader));
                texto.Add(new Paragraph(" ", ftHeader));
                if (!string.IsNullOrEmpty(_strProveedor))
                    texto.Add(new Paragraph("Proveedor: " + _strProveedor, ftHeader));
                if (!string.IsNullOrEmpty(_strOrdenCompra))
                    texto.Add(new Paragraph("Orden de Compra: " + _strOrdenCompra, ftHeader));
                if (!string.IsNullOrEmpty(_strReferencia))
                    texto.Add(new Paragraph("At'n: " + _strReferencia, ftHeader));
                if (!string.IsNullOrEmpty(_strNotas))
                    texto.Add(new Paragraph("Remisión: " + _strNotas, ftHeader));
                if (!string.IsNullOrEmpty(_strSustituye))
                    texto.Add(new Paragraph(_strSustituye, ftHeader));
                if (!string.IsNullOrEmpty(_strVendedor))
                    texto.Add(new Paragraph(_strVendedor, ftHeader));
            }

            texto.Add(new Paragraph("Cliente: " + _strNegocioID, ftHeader));

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            tblHeaderB.AddCell(tblHeader2);

            // Encabezados de los productos
            float[] ancho_columnas3 = new float[5];
            ancho_columnas3[0] = 55;
            ancho_columnas3[1] = 90;
            ancho_columnas3[2] = 255;
            ancho_columnas3[3] = 68;
            ancho_columnas3[4] = 60;
            PdfPTable tblHeader3 = new PdfPTable(ancho_columnas3);
            texto = new Paragraph("CANTIDAD", ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader3.AddCell(celda);

            texto = new Paragraph("U. DE MEDIDA", ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader3.AddCell(celda);

            texto = new Paragraph("DESCRIPCIÓN", ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader3.AddCell(celda);

            texto = new Paragraph("P.UNITARIO", ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader3.AddCell(celda);

            texto = new Paragraph("IMPORTE", ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader3.AddCell(celda);

            tblHeaderC.AddCell(tblHeader3);
            #endregion


            tblHeaderA.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 760 + _flAjusteOficio,
                writer.DirectContent);

            tblHeaderB.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 675 + _flAjusteOficio,
                writer.DirectContent);

            tblHeaderC.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 580 + _flAjusteOficio,
                writer.DirectContent);


            #region Footer
            PdfPTable tblFooterTot = new PdfPTable(1);
            PdfPTable tblFooterCad = new PdfPTable(1);
            PdfPTable tblFooterFir = new PdfPTable(1);
            tblFooterTot.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooterTot.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooterTot.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            tblFooterCad.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooterCad.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooterCad.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            tblFooterFir.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooterFir.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooterFir.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            float[] ancho_columnasFA = new float[3];
            ancho_columnasFA[0] = 380;
            ancho_columnasFA[1] = 88;
            ancho_columnasFA[2] = 60;
            PdfPTable tblFooter1 = new PdfPTable(ancho_columnasFA);

            texto = new Paragraph(_strTotalLetras.ToUpper(), ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Rowspan = 2;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("SUBTOTAL:", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtSubTotal.ToString("C"), ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.BOTTOM_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            if (_amtDescuento > 0)
            {
                texto = new Paragraph("MONTO CON DESCUENTO:", ftFooter);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                decimal amtConDescuento = _amtSubTotal - _amtDescuento;
                texto = new Paragraph(amtConDescuento.ToString("C"), ftFooter);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.BOTTOM_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(string.Empty, ftFooter);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);
            }

            texto = new Paragraph("IVA " + _amtTasa + "%:", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtImpuesto.ToString("C"), ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.BOTTOM_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            if (_amtIVA_Retenido > 0)
            {
                texto = new Paragraph(string.Empty, ftFooter);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph("RETENCIÓN IVA:", ftFooter);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(_amtIVA_Retenido.ToString("C"), ftFooter);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                celda.Border = Rectangle.BOTTOM_BORDER;
                tblFooter1.AddCell(celda);
            }

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("TOTAL:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtTotal.ToString("C"), ftFooterB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Border = Rectangle.BOTTOM_BORDER;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(" \n ", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            tblFooterTot.AddCell(tblFooter1);

            PdfPTable tblFooter2 = new PdfPTable(ancho_columnasFA);
            texto = new Paragraph("Pago en una sola exhibición", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            //texto = new Paragraph("Régimen Fiscal: " + _strRegimen, ftFooterB);
            //celda = new PdfPCell(texto);
            //celda.Border = Rectangle.NO_BORDER;
            //celda.HorizontalAlignment = Element.ALIGN_CENTER;
            //celda.VerticalAlignment = Element.ALIGN_TOP;
            //celda.Colspan = 3;
            //tblFooter2.AddCell(celda);

            texto = new Paragraph(" \n ", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            texto = new Paragraph("Sello digital del CFDI:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(_strSelloCFDI, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter2.AddCell(celda);

            texto = new Paragraph("Sello del SAT:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(_strSelloSAT, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter2.AddCell(celda);

            texto = new Paragraph("Cadena original del complemento de certificación digital del SAT:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(_strCadenaOriginal, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter2.AddCell(celda);

            tblFooterCad.AddCell(tblFooter2);

            float[] ancho_columnasFB = new float[3];
            ancho_columnasFB[0] = 164;
            ancho_columnasFB[1] = 200;
            ancho_columnasFB[2] = 164;
            tblFooter2 = new PdfPTable(ancho_columnasFB);

            texto = new Paragraph(string.Empty, ftHeaderB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 1;
            tblFooter2.AddCell(celda);
            texto = new Paragraph("Firma de conformidad", ftHeader);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.TOP_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 1;
            tblFooter2.AddCell(celda);
            texto = new Paragraph(string.Empty, ftHeaderB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 1;
            tblFooter2.AddCell(celda);

            texto = new Paragraph("Esta es una representación impresa de un CFDI", ftHeaderB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            tblFooterFir.AddCell(tblFooter2);

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeVersion = 7;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            iTextSharp.text.Image imgCodigo = iTextSharp.text.Image.GetInstance(qrCodeEncoder.Encode(_strQRCodeData), BaseColor.BLACK);
            imgCodigo.SetAbsolutePosition(450, 100 + _flAjusteOficio);
            document.Add(imgCodigo);

            #endregion

            if (document.PageNumber == _intTotalPaginas)
            {
                tblFooterTot.WriteSelectedRows(0, -1, 0, -1,
                    document.LeftMargin, 270 + _flAjusteOficio,
                    writer.DirectContent);
            }

            tblFooterCad.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 220 + _flAjusteOficio,
                writer.DirectContent);

            tblFooterFir.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 70 + _flAjusteOficio,
                writer.DirectContent);

            #region Anexo-Pagaré
            if (_flAjusteOficio != 0)
            {
                PdfContentByte cb = writer.DirectContent;

                // x, y, ancho, alto
                // Rectángulo general
                cb.SetLineWidth(0.5f);
                cb.Rectangle(25, 25, 560, 115);
                cb.Stroke();

                int intX = 127;
                // Renglones
                cb.MoveTo(100, intX);
                cb.LineTo(170, intX);
                cb.Stroke();

                //Factura
                cb.MoveTo(235, intX);
                cb.LineTo(305, intX);
                cb.Stroke();

                //Fecha
                cb.MoveTo(360, intX);
                cb.LineTo(430, intX);
                cb.Stroke();

                //Fecha Entrega
                cb.MoveTo(505, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;
                //Cliente
                cb.MoveTo(172, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;
                //Direccion 1
                cb.MoveTo(172, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;
                //Contrarecibo
                cb.MoveTo(172, intX);
                cb.LineTo(305, intX);
                cb.Stroke();

                cb.MoveTo(410, intX); //Total Factura
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;

                //Cobro
                cb.MoveTo(172, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;
                //Repartidor
                cb.MoveTo(100, intX);
                cb.LineTo(305, intX);
                cb.Stroke();

                cb.MoveTo(410, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                intX = intX - 12;
                //Efectivo
                cb.MoveTo(100, intX);
                cb.LineTo(305, intX);
                cb.Stroke();

                cb.MoveTo(410, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                //Persona que recibe
                intX = intX - 12;
                cb.MoveTo(172, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                //Recibe 1
                intX = intX - 12;
                cb.MoveTo(100, intX);
                cb.LineTo(305, intX);
                cb.Stroke();

                //Recibe 2
                cb.MoveTo(360, intX);
                cb.LineTo(575, intX);
                cb.Stroke();

                float[] ancho_anexo = new float[8];
                ancho_anexo[0] = 66;
                ancho_anexo[1] = 66;
                ancho_anexo[2] = 66;
                ancho_anexo[3] = 66;
                ancho_anexo[4] = 66;
                ancho_anexo[5] = 66;
                ancho_anexo[6] = 66;
                ancho_anexo[7] = 66;

                PdfPTable tblFooterAnexo = new PdfPTable(1);
                tblFooterAnexo.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
                tblFooterAnexo.DefaultCell.Border = Rectangle.NO_BORDER;
                tblFooterAnexo.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPTable tblAnexo = new PdfPTable(ancho_anexo);

                // Línea 1
                texto = new Paragraph("Renglones: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strRenglones, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Factura:", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strFactID, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Fecha: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(DateTime.Today.ToString("dd/MMM/yyyy",
                   System.Globalization.CultureInfo.CreateSpecificCulture("es-MX")).ToUpper(), ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Fecha Entrega: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                tblAnexo.AddCell(celda);

                // Línea 2
                texto = new Paragraph("Nombre del cliente: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strNombreCliente, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 6;
                tblAnexo.AddCell(celda);

                // Línea 3
                texto = new Paragraph("Domicilio Entrega: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strDireccion_Entrega, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 6;
                tblAnexo.AddCell(celda);

                // Línea 3
                texto = new Paragraph("Contrarecibo: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 1;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strContrarecibo, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 3;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Total Factura: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 1;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strTotalFactura, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 3;
                tblAnexo.AddCell(celda);

                // Línea 3
                texto = new Paragraph("Cobro: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strCobro, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 6;
                tblAnexo.AddCell(celda);

                // Línea 6
                texto = new Paragraph("Telefono: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 1;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strTelefono, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 3;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Repartidor: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 1;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(_strRepartidor, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 3;
                tblAnexo.AddCell(celda);

                // Línea 7
                texto = new Paragraph("Efectivo: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Tarjeta Crédito N° Autoriza: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                // Línea 8
                texto = new Paragraph("Persona que recibe: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 6;
                tblAnexo.AddCell(celda);

                // Línea 9
                texto = new Paragraph("Recibe 1°: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph("Recibe 2°: ", ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                texto = new Paragraph(string.Empty, ftHeader);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_LEFT;
                celda.Colspan = 2;
                tblAnexo.AddCell(celda);

                tblFooterAnexo.AddCell(tblAnexo);

                tblFooterAnexo.WriteSelectedRows(0, -1, 0, -1,
                                                 document.LeftMargin, 140,
                                                 writer.DirectContent);

                // Divisor
                float[] dash1 = { 9, 6, 0, 6 };
                cb.SetLineDash(dash1, 5);
                cb.MoveTo(10, 152);
                cb.LineTo(602, 152);
                cb.Stroke();
            }
            #endregion
        }

    }
}
