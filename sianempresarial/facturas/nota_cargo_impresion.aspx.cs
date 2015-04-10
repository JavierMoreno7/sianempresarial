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

public partial class facturas_nota_cargo_impresion : BasePagePopUp
{
    CFacturaElectronica objFactura;
    string strNegocioID;
    string strOrdenCompra;
    string strProveedor;
    string strNota;
    string strNotaID;
    StringBuilder strNotas;
    string strNumCtaPago;
    string strBanco;
    string strMetodoPago;
    string strSustituye;
    string strVendedor;
    string strQRCodeData;

    protected void Page_Load(object sender, EventArgs e)
    {
        strOrdenCompra = strProveedor = strNota = string.Empty;
        if (Request.QueryString["nota"] != null)
        {
            strNota = "CFDI_" + Request.QueryString["nota"];
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.ID, E.proveedor, E.cuenta_bancaria " +
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
                    " INNER JOIN personas V " +
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
                strNotaID = objRowResult["ID"].ToString();
                strOrdenCompra = string.Empty;
                strProveedor = objRowResult["proveedor"].ToString();
                strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                strBanco = objRowResult["banco"].ToString();
                strMetodoPago = objRowResult["metodo_pago"].ToString();
                strNegocioID = objRowResult["negocioID"].ToString();
                if ((int)objRowResult["vendID"] == 5)
                    strVendedor = objRowResult["vendedor"].ToString();
                else
                    strVendedor = string.Empty;
            }
        }
        else
            if (Request.QueryString["notaID"] != null)
            {
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT F.nota, E.proveedor " +
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
                    strNotaID = Request.QueryString["notaID"];
                    strNota = "CFDI_" + objRowResult["nota"].ToString() + ".xml";
                    strOrdenCompra = string.Empty;
                    strProveedor = objRowResult["proveedor"].ToString();
                    strNumCtaPago = objRowResult["cuenta_bancaria"].ToString();
                    strBanco = objRowResult["banco"].ToString();
                    strMetodoPago = objRowResult["metodo_pago"].ToString();
                    strNegocioID = objRowResult["negocioID"].ToString();
                    if ((int)objRowResult["vendID"] == 5)
                        strVendedor = objRowResult["vendedor"].ToString();
                    else
                        strVendedor = string.Empty;
                }
            }
        if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strNota)))
            Generar_Factura();
        else
            this.lblMessage.Text = "Nota no existe";
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
        string strQuery = "SELECT isr_ret, iva_ret, monto_isr_ret, monto_iva_ret " +
                " FROM notas_cargo " +
                " WHERE nota = '" + strNota.Replace("CFDI_", "").Replace(".xml", "") + "'";
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
        xmlFactura.Load(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strNota));

        XmlNodeList xmlComprobante = xmlFactura.GetElementsByTagName("cfdi:Comprobante");
        objFactura.AmtTotal = Convert.ToDecimal(xmlComprobante[0].Attributes["total"].Value);
        objFactura.AmtSubTotal = Convert.ToDecimal(xmlComprobante[0].Attributes["subTotal"].Value);
        if (xmlComprobante[0].Attributes["descuento"] != null)
            objFactura.AmtDescuento = Convert.ToDecimal(xmlComprobante[0].Attributes["descuento"].Value);
        if (xmlComprobante[0].Attributes["metodoDePago"] != null)
            objFactura.StrMetodoDePago = xmlComprobante[0].Attributes["metodoDePago"].Value;
        if (xmlComprobante[0].Attributes["NumCtaPago"] != null)
            objFactura.StrNumCtaPago = xmlComprobante[0].Attributes["NumCtaPago"].Value;

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
            lstProductos.Add(objProducto);
        }
        objFactura.LstProductos = lstProductos;

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

        strNotas = new StringBuilder();
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
            Paragraph texto = new Paragraph(objProducto.AmtCantidad.ToString("0.##"), ftProductos);
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
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));

        MyPageEvents events = new MyPageEvents();
        events._strNegocioID = strNegocioID;
        events._strNotaID = strNotaID;
        events._strCertificado = objFactura.StrCertificadoSAT;
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
        events._amtDescuento = objFactura.AmtDescuento;
        events._amtTotal = objFactura.AmtTotal;
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
        events._intTotalPaginas = intTotalPaginas;
        events._strOrdenCompra = strOrdenCompra;
        events._strProveedor = strProveedor;
        events._strNotas = strNotas.ToString();
        events._strBanco = strBanco;
        events._strSustituye = strSustituye;
        events._strVendedor = strVendedor;
        events._strQRCodeData = strQRCodeData;

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
            strNota = strNota.Replace("CFDI_", "PDF_").Replace(".xml", ".pdf");
            if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strNota)))
            {
                Response.Redirect("facturas_correo.aspx?t=1&ID=" + strNotaID);
            }
            FileStream flArchivo = new FileStream(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strNota), FileMode.Create);
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
            Paragraph texto = new Paragraph(objProducto.AmtCantidad.ToString("0.##"), ftProductos);
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
        if (Request.QueryString["m"] == null)
        {
            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
            m.Close();
        }
        else
            Response.Redirect("facturas_correo.aspx?t=1&ID=" + strNotaID);
    }

    class MyPageEvents : PdfPageEventHelper
    {
        public string _strNegocioID;
        public string _strNotaID;
        public string _strMapPath;
        public string _strFolioFiscal;
        public string _strCertificado;
        public string _strSelloCFDI;
        public string _strSelloSAT;
        public string _strCadenaOriginal;
        public string _strProveedor;
        public string _strOrdenCompra;
        public string _strRegimen;
        public int _intFolio;
        public DateTime _dtFechaCertificado;
        public int _intNoAprobacion;
        public int _intAnoAprobacion;
        public string _strMetodoDePago;
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

            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);

            // Datos de la nota
            texto = new Paragraph("NOTA DE CARGO, FOLIO: ", ftHeaderB);
            texto.Add(new Paragraph(_strNotaID, ftHeader));
            texto.Add(new Paragraph("Folio Fiscal:", ftHeaderB));
            texto.Add(new Paragraph(_strFolioFiscal, ftHeader));
            texto.Add(new Paragraph("No de Serie del Certificado del SAT:", ftHeaderB));
            texto.Add(new Paragraph(_strCertificado, ftHeader));
            texto.Add(new Paragraph("Fecha y hora de certificación:", ftHeaderB));
            texto.Add(new Paragraph(_dtFechaCertificado.ToString("yyyy-MM-dd HH:mm:ss"), ftHeader));
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
            texto.Add(new Paragraph(DateTime.Today.ToString("dd-MMMM-yyyy",
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
                if (!string.IsNullOrEmpty(_strNotas))
                    texto.Add(new Paragraph("Remisión: " + _strNotas, ftHeader));
                if (!string.IsNullOrEmpty(_strSustituye))
                    texto.Add(new Paragraph(_strSustituye, ftHeader));
                if (!string.IsNullOrEmpty(_strVendedor))
                    texto.Add(new Paragraph(_strVendedor, ftHeader));
            }

            //texto.Add(new Paragraph("Cédula Fiscal " + _strCedulaFiscalCliente, ftHeader));

            //texto.Add(new Paragraph("Cédula IEPS " + _strCedulaIEPSCliente, ftHeader));

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
                document.LeftMargin, 760,
                writer.DirectContent);

            tblHeaderB.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 675,
                writer.DirectContent);

            tblHeaderC.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 580,
                writer.DirectContent);

            #region Footer
            PdfPTable tblFooterA = new PdfPTable(1);
            PdfPTable tblFooterB = new PdfPTable(1);
            tblFooterA.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooterA.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooterA.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            tblFooterB.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooterB.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooterB.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

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

            texto = new Paragraph(_amtSubTotal.ToString("c"), ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.BOTTOM_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            if (_amtDescuento > 0)
            {
                texto = new Paragraph("MONTO CON DESCUENTO:", ftFooter7);
                celda = new PdfPCell(texto);
                celda.Border = Rectangle.NO_BORDER;
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                tblFooter1.AddCell(celda);

                decimal amtConDescuento = _amtSubTotal - _amtDescuento;
                texto = new Paragraph(amtConDescuento.ToString("c"), ftFooter7);
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

            texto = new Paragraph("IVA " + _amtTasa + "%:", ftFooter7);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtImpuesto.ToString("c"), ftFooter7);
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

                texto = new Paragraph(_amtIVA_Retenido.ToString("c"), ftFooter7);
                celda = new PdfPCell(texto);
                celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                celda.VerticalAlignment = Element.ALIGN_TOP;
                celda.Border = Rectangle.BOTTOM_BORDER;
                tblFooter1.AddCell(celda);
            }

            texto = new Paragraph(string.Empty, ftFooter7);
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

            texto = new Paragraph(_amtTotal.ToString("c"), ftFooter7);
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

            texto = new Paragraph("Pago en una sola exhibición", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("Régimen Fiscal: " + _strRegimen, ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(" \n ", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("Sello digital del CFDI:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_strSelloCFDI, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("Sello del SAT:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_strSelloSAT, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter1.AddCell(celda);

            texto = new Paragraph("Cadena original del complemento de certificación digital del SAT:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_strCadenaOriginal, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(string.Empty, ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter1.AddCell(celda);

            tblFooterA.AddCell(tblFooter1);

            float[] ancho_columnasFB = new float[3];
            ancho_columnasFB[0] = 164;
            ancho_columnasFB[1] = 200;
            ancho_columnasFB[2] = 164;
            PdfPTable tblFooter2 = new PdfPTable(ancho_columnasFB);

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

            tblFooterB.AddCell(tblFooter2);

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeVersion = 7;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            iTextSharp.text.Image imgCodigo = iTextSharp.text.Image.GetInstance(qrCodeEncoder.Encode(_strQRCodeData), BaseColor.BLACK);
            imgCodigo.SetAbsolutePosition(450, 100);
            document.Add(imgCodigo);

            #endregion

            tblFooterA.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 270,
                writer.DirectContent);

            tblFooterB.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 70,
                writer.DirectContent);
        }
    }
}
