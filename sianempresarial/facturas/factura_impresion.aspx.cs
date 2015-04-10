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

public partial class facturas_factura_impresion : BasePagePopUp
{
    CFacturaElectronica objFactura;
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
    string strComentarios;
    string strPorcDescuento;
    string strCertificadoEmisor;

    protected void Page_Load(object sender, EventArgs e)
    {
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
                    " INNER JOIN personas V " +
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
                strOrdenCompra = objRowResult["orden_compra"].ToString();
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
        else
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
                    strOrdenCompra = objRowResult["orden_compra"].ToString();
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
        if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact)))
            Generar_Factura();
        else
            this.lblMessage.Text = "Factura Electrónica no existe";
    }

    private void Generar_Factura()
    {
        Obtener_Datos();
        int intTotalPaginas = Crear_Archivo_Temporal();
        Crear_Archivo(intTotalPaginas);
    }

    private void Obtener_Datos()
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT comentarios_en_factura" +
                         " FROM sistema_apps " +
                         " WHERE ID = " +  Session["SIANAppID"];
        objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

        if (!(bool)objDataResult.Tables[0].Rows[0]["comentarios_en_factura"])
            strComentarios = string.Empty;

        strQuery = "SELECT folio, isr_ret, iva_ret" +
                  ", monto_isr_ret, monto_iva_ret" +
                  ", descuento, descuento2 " +
                  ", detalle_incluido" +
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

        bool swDetalleIncluido = (bool)objRowResult["detalle_incluido"];

        XmlDocument xmlFactura = new XmlDocument();
        xmlFactura.Load(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact));

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

        strQuery = "SELECT P.nombre, F.* " +
                    ", P.usar_detalle" +
                    ", P.unimed" +
                    ", P.unimed2" +
                    ", P.relacion1" +
                    ", P.relacion2" +
                    ", P.unimed_original" +
                    ",P2.nombre as producto_grupo" +
                    " FROM facturas_liq_prod F" +
                    " INNER JOIN productos P" +
                    " ON F.producto_ID = P.ID " +
                    " AND F.factura_ID = " + strFactID +
                    " LEFT JOIN productos P2" +
                    " ON P2.ID = F.grupoID" +
                    " ORDER BY consecutivo, nombre";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        decimal dcmCantidadAlterno;

        List<CProducto> lstProductos = new List<CProducto>();
        string strGrupoConsecutivo = string.Empty;
        for (int i = 0; i < xmlProductos.Count; i++)
        {
            CProducto objProducto = new CProducto();

            objProducto.AmtCantidad = Convert.ToDecimal(xmlProductos[i].Attributes["cantidad"].Value);

            if (i < objDataResult.Tables[0].Rows.Count)
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

                if (!string.IsNullOrEmpty(objDataResult.Tables[0].Rows[i]["grupoID"].ToString()))
                    objProducto.IntGrupoID = (int)objDataResult.Tables[0].Rows[i]["grupoID"];

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

                    objProducto.StrDetalle = objDataResult.Tables[0].Rows[i]["detalle"].ToString();
                }
            }
            else
                objProducto.StrDescripcion = xmlProductos[i].Attributes["descripcion"].Value;

            if (!string.IsNullOrEmpty(objDataResult.Tables[0].Rows[i]["unimed2"].ToString()))
            {
                if ((bool)objDataResult.Tables[0].Rows[i]["unimed_original"])
                {
                    if ((decimal)objDataResult.Tables[0].Rows[i]["relacion1"] == 1)
                        dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objDataResult.Tables[0].Rows[i]["relacion2"], 2);
                    else
                        dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objDataResult.Tables[0].Rows[i]["relacion1"], 2);

                    objProducto.StrDescripcion += "\n" +
                                                "(Equivalente a " +
                                                dcmCantidadAlterno.ToString("0.##") + " " +
                                                objDataResult.Tables[0].Rows[i]["unimed2"].ToString() + ")";
                }
                else
                {
                    if ((bool)objDataResult.Tables[0].Rows[i]["usar_unimed2"])
                    {
                        if ((decimal)objDataResult.Tables[0].Rows[i]["relacion1"] == 1)
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objDataResult.Tables[0].Rows[i]["relacion2"], 2);
                        else
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objDataResult.Tables[0].Rows[i]["relacion1"], 2);

                        objProducto.StrDescripcion += "\n" +
                                                    "(Equivalente a " +
                                                    dcmCantidadAlterno.ToString("0.##") + " " +
                                                    objDataResult.Tables[0].Rows[i]["unimed"].ToString() + ")";
                    }
                    else
                    {
                        if ((decimal)objDataResult.Tables[0].Rows[i]["relacion1"] == 1)
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objDataResult.Tables[0].Rows[i]["relacion2"], 2);
                        else
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objDataResult.Tables[0].Rows[i]["relacion1"], 2);

                        objProducto.StrDescripcion += "\n" +
                                                    "(Equivalente a " +
                                                    dcmCantidadAlterno.ToString("0.##") + " " +
                                                    objDataResult.Tables[0].Rows[i]["unimed2"].ToString() + ")";
                    }
                }
            }

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
            return;
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
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));

        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f);

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
            Paragraph texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
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
        int intMaxWidth = 190, intMaxHeight = 50, intLogoWidth, intLogoHeight;
        HttpCookie ckSIAN = Request.Cookies["userCng"];
        int intNombreLogo = ckSIAN["ck_logo"].LastIndexOf("/") + 1;
        string[] strSize = ckSIAN["ck_logo_size"].Split('x');
        intLogoWidth = int.Parse(strSize[0]);
        intLogoHeight = int.Parse(strSize[1]);
        float flRelacion = CRutinas.Calcular_Relacion(intMaxWidth, intMaxHeight, intLogoWidth, intLogoHeight);

        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));

        MyPageEvents events = new MyPageEvents();
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
        events._strProveedor = strProveedor;
        events._strNotas = strNotas.ToString();
        events._strBanco = strBanco;
        events._strSustituye = strSustituye;
        events._strVendedor = strVendedor;
        events._strQRCodeData = strQRCodeData;
        events._strComentarios = strComentarios;
        events._strMoneda = objFactura.StrMoneda;
        events._strMonedaAbrev = (objFactura.StrMoneda.Equals("MXN") ? " M.N." : objFactura.StrMoneda);

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f);

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
                Response.Redirect("facturas_correo.aspx?t=0&ID=" + strFactID);
            }
            FileStream flArchivo = new FileStream(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strFact), FileMode.Create);
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
            Paragraph texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
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
        public string _strNegocioID;
        public string _strFactID;
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
        public string _strMoneda;
        public string _strMonedaAbrev;
		public DateTime _dtFecha;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            StringBuilder strCadena = new StringBuilder();
            Font ftHeader = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL));
            Font ftFooter = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));
            Font ftFooter7 = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));
            Font ftHeaderB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD));
            Font ftFooterB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.BOLD));

            Rectangle page = document.PageSize;

            #region iTextSharp.text.Imagenes Fijas
            iTextSharp.text.Image imgFactura = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_grande.png");
            imgFactura.SetAbsolutePosition(25, 40);
            imgFactura.ScalePercent(25.5f, 25f);
            document.Add(imgFactura);

            iTextSharp.text.Image imgProductos = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prods.png");
            imgProductos.SetAbsolutePosition(30, 270);
            imgProductos.ScalePercent(25.2f, 25f);
            document.Add(imgProductos);

            iTextSharp.text.Image imgProductos2 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodshor.png");
            imgProductos2.SetAbsolutePosition(30, 560);
            imgProductos2.ScalePercent(25.1f, 25f);
            document.Add(imgProductos2);

            iTextSharp.text.Image imgProductos3 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos3.SetAbsolutePosition(90, 270);
            imgProductos3.ScalePercent(25.2f, 25f);
            document.Add(imgProductos3);

            iTextSharp.text.Image imgProductos4 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos4.SetAbsolutePosition(180, 270);
            imgProductos4.ScalePercent(25.2f, 25f);
            document.Add(imgProductos4);

            iTextSharp.text.Image imgProductos5 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos5.SetAbsolutePosition(450, 270);
            imgProductos5.ScalePercent(25.2f, 25f);
            document.Add(imgProductos5);

            iTextSharp.text.Image imgProductos6 = iTextSharp.text.Image.GetInstance(_strMapPath + "fe_prodsver.png");
            imgProductos6.SetAbsolutePosition(520, 270);
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

            iTextSharp.text.Image imgEmpresa = iTextSharp.text.Image.GetInstance(_strMapPath + _strLogoEmpresa);
            imgEmpresa.SetAbsolutePosition(45, 700);
            imgEmpresa.ScalePercent(_flEscala, _flEscala);
            document.Add(imgEmpresa);

            float[] ancho_columnas = new float[3];
            ancho_columnas[0] = 140; //180
            ancho_columnas[1] = tblHeaderA.TotalWidth - 250; //360
            ancho_columnas[2] = 180;
            PdfPTable tblHeader1 = new PdfPTable(ancho_columnas);

            // iTextSharp.text.Imagen de la empresa
            PdfPCell celda = new PdfPCell();
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);

            // Datos Fiscales
            Paragraph texto = new Paragraph(_strNombreEmisor + "\n", ftHeader);
            strCadena.Append(_dirDomicilioFiscal.StrCalle);
            strCadena.Append(" ");
            strCadena.Append(_dirDomicilioFiscal.StrNumeroExterior);
            if (!string.IsNullOrEmpty(_dirDomicilioFiscal.StrNumeroInterior))
            {
                strCadena.Append("-");
                strCadena.Append(_dirDomicilioFiscal.StrNumeroInterior);
            }
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));
            texto.Add(new Paragraph(_dirDomicilioFiscal.StrColonia, ftHeader));

            strCadena.Remove(0, strCadena.Length);
            strCadena.Append(_dirDomicilioFiscal.StrLocalidad);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioFiscal.StrEstado);
            strCadena.Append(", ");
            strCadena.Append(_dirDomicilioFiscal.StrPais);
            texto.Add(new Paragraph(strCadena.ToString(), ftHeader));

            texto.Add(new Paragraph("C.P. " + _dirDomicilioFiscal.StrCP, ftHeader));

            texto.Add(new Paragraph("RFC " + _strRFCEmisor, ftHeader));

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
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
            texto.Add(new Paragraph(_strNombreCliente + " (" + _strNegocioID + ")", ftHeader));
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
            texto.Add(new Paragraph(_strMetodoDePago, ftHeader));

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

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            celda = new PdfPCell();
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            if(!string.IsNullOrEmpty(_strComentarios))
                texto = new Paragraph(_strComentarios, ftHeader);
            else
                texto = new Paragraph(string.Empty, ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            celda.Colspan = 2;
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
                document.LeftMargin, 760,
                writer.DirectContent);

            tblHeaderB.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 680,
                writer.DirectContent);

            tblHeaderC.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 580,
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

            texto = new Paragraph(_strTotalLetras.ToUpper(), ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Rowspan = 2;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("SUBTOTAL:", ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtSubTotal.ToString("c") + _strMonedaAbrev, ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.BOTTOM_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            if (_amtDescuento > 0)
            {
                texto = new Paragraph(_strPorcDescuento, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(_amtDescuento.ToString("c") + _strMonedaAbrev, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.BOTTOM_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(string.Empty, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph("SUBTOTAL:", ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                decimal amtConDescuento = _amtSubTotal - _amtDescuento;
                texto = new Paragraph(amtConDescuento.ToString("c") + _strMonedaAbrev, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.BOTTOM_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(string.Empty, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);
            }

            texto = new Paragraph("IVA " + _amtTasa.ToString("0.##") + "%:", ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtImpuesto.ToString("c") + _strMonedaAbrev, ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.BOTTOM_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            if (_amtIVA_Retenido > 0)
            {
                texto = new Paragraph(string.Empty, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph("RETENCIÓN IVA:", ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(_amtIVA_Retenido.ToString("c") + _strMonedaAbrev, ftFooter7);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                celda.Border = Rectangle.BOTTOM_BORDER;
                tblFooter1.AddCell(celda);
            }

            if (_amtISR_Retenido > 0)
            {
                texto = new Paragraph(string.Empty, ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph("RETENCIÓN ISR:", ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                texto = new Paragraph(_amtISR_Retenido.ToString("c") + _strMonedaAbrev, ftFooter7);
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

            texto = new Paragraph("TOTAL:", ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtTotal.ToString("c") + _strMonedaAbrev, ftFooter7);
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

            texto = new Paragraph("Régimen Fiscal: " + _strRegimen, ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

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
            imgCodigo.SetAbsolutePosition(450, 100);
            document.Add(imgCodigo);

            #endregion

            if (document.PageNumber == _intTotalPaginas)
            {
                tblFooterTot.WriteSelectedRows(0, -1, 0, -1,
                    document.LeftMargin, 270,
                    writer.DirectContent);
            }

            tblFooterCad.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 220,
                writer.DirectContent);

            tblFooterFir.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 70,
                writer.DirectContent);
        }

    }
}
