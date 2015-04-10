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
using CFE;

public partial class facturas_nota_cargo_preimpresion : BasePagePopUp
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
    string strComentarios;
	string strPorcDescuento;

    protected void Page_Load(object sender, EventArgs e)
    {
        strOrdenCompra = strProveedor = strNota = string.Empty;
        if (Request.QueryString["notaID"] != null)
        {
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
                strNotaID = Request.QueryString["notaID"];
                strNota = "CFDI_" + objRowResult["nota"].ToString() + ".xml";
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
            else
                this.lblMessage.Text = "Nota no existe";
        }
        else
            this.lblMessage.Text = "Nota no existe";
    }

    private void Generar_Factura()
    {
        Obtener_Datos();
        int intTotalPaginas;
        try
        {
            intTotalPaginas = Crear_Archivo_Temporal();
        }
        catch
        {
            this.lblMessage.Text = "Nota no tiene productos";
            return;
        }
        Crear_Archivo(intTotalPaginas);
    }

    private void Obtener_Datos()
    {
        objFactura = new CFacturaElectronica();

        CNotaCargoDB objNotaDB = new CNotaCargoDB();
        if (!objNotaDB.Leer(int.Parse(Request.QueryString["notaID"])))
            return;

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT rfc, razonsocial, " +
                   " Dom_Calle, Dom_NumExt, Dom_NumInt, Dom_Colonia, " +
                   " Dom_Localidad, Dom_Referencia, Dom_Municipio, Dom_Estado, " +
                   " Dom_Pais, Dom_CP, Exp_Calle, Exp_NumExt, Exp_NumInt, " +
                   " Exp_Colonia, Exp_Localidad, Exp_Municipio, Exp_Estado, " +
                   " Exp_Pais, Exp_CP, Lugar_Expedicion, Regimen, comentarios_en_factura" +
                   " FROM sistema_apps " +
                   " WHERE ID = " +  Session["SIANAppID"];

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);
        }
        catch
        {
            return;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        string strSucID = objNotaDB.sucursal_ID.ToString();
        decimal dcmTotal = objNotaDB.total;
        decimal dcmPorISR_Ret = objNotaDB.isr_ret;
        decimal dcmISR_Ret = objNotaDB.monto_isr_ret;
        decimal dcmPorIVA_Ret = objNotaDB.iva_ret;
        decimal dcmIVA_Ret = objNotaDB.monto_iva_ret;
        decimal dcmTotalRetenidos = dcmISR_Ret + dcmIVA_Ret;
        decimal dcmPorIva = objNotaDB.iva;
        decimal dcmIva = objNotaDB.monto_iva;
        decimal dcmPorcDescuento = objNotaDB.descuento;
        decimal dcmPorcDescuento2 = objNotaDB.descuento2;
        decimal dcmSubtotal = objNotaDB.monto_subtotal;

        objFactura.StrMoneda = objNotaDB.moneda;
        objFactura.AmtTipoCambio = objNotaDB.tipo_cambio;
        decimal dcmDescuento = 0;
        if (dcmPorcDescuento > 0)
            dcmDescuento = dcmSubtotal - objNotaDB.monto_descuento;

        objFactura.DtFecha = objNotaDB.fecha;
        objFactura.AmtTotal = dcmTotal;
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

        strQuery = "SELECT sucursales.sucursal as sucursal, " +
                "sucursales.numero_codificacion as codificacion, " +
                "establecimientos.razonsocial as negocio, " +
                "establecimientos.direccionfiscal as direccionfiscal, " +
                "establecimientos.num_exterior as num_exterior, " +
                "establecimientos.num_interior as num_interior, " +
                "establecimientos.colonia as colonia, " +
                "establecimientos.cp as cp, " +
                "establecimientos.poblacion as poblacion, " +
                "establecimientos.municipio as municipio, " +
                "establecimientos.estado as estado, " +
                "establecimientos.pais as pais, " +
                "establecimientos.rfc as rfc " +
                "FROM sucursales INNER JOIN establecimientos " +
                "ON sucursales.establecimiento_ID = establecimientos.ID " +
                "WHERE sucursales.ID = " + strSucID;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
            return;
        }

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

        strQuery = "SELECT F.cantidad as cantidad, " +
                        " F.consecutivo as consecutivo, " +
                        " F.costo_unitario as costo_unitario, " +
                        " F.costo as costo, " +
                        " F.detalle as detalle, " +
                        " P.clave as clave, " +
                        " P.nombre as nombre, " +
                        " P.unimed as unimed, " +
                        " P.piezasporcaja as piezasporcaja " +
                        " FROM notas_cargo_prod F" +
                        " INNER JOIN productos P" +
                        " ON F.producto_ID = P.ID " +
                        " WHERE F.nota_ID = " + Request.QueryString["notaID"] +
                        " ORDER BY consecutivo, nombre";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
            return;
        }

        List<CProducto> lstProductos = new List<CProducto>();
        foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
        {
            CProducto objProducto = new CProducto();
            objProducto.StrDescripcion = objRowResult2["nombre"].ToString();
            objProducto.AmtCantidad = (decimal)objRowResult2["cantidad"];
            objProducto.AmtValorUnitario = (decimal)objRowResult2["costo_unitario"];
            objProducto.AmtImporte = (decimal)objRowResult2["costo"];
            objProducto.StrUnidad = objRowResult2["unimed"].ToString();
            objProducto.StrDetalle = objRowResult2["detalle"].ToString();
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
        events._strNegocioID = strNegocioID;
        events._strNotaID = strNotaID;
        events._strCertificado = objFactura.StrCertificadoSAT;
        events._strFolioFiscal = objFactura.StrFolioFiscal;
        events._strSelloCFDI = objFactura.StrSelloCFDI;
        events._strSelloSAT = objFactura.StrSelloSAT;
        events._strCadenaOriginal = "N/A";
        events._intFolio = objFactura.IntFolio;
        events._dtFechaCertificado = objFactura.DtFecha;
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
        events._intTotalPaginas = intTotalPaginas;
        events._strOrdenCompra = strOrdenCompra;
        events._strProveedor = strProveedor;
        events._strNotas = strNotas.ToString();
        events._strBanco = strBanco;
        events._strSustituye = strSustituye;
        events._strVendedor = strVendedor;
        events._strComentarios = strComentarios;
        events._strMoneda = objFactura.StrMoneda;
        events._strMonedaAbrev = (objFactura.StrMoneda.Equals("MXN") ? " M.N." : objFactura.StrMoneda);

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f);

        Response.ContentType = "application/pdf";
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        writer.CloseStream = false;
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
        Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
        Response.OutputStream.Flush();
        Response.OutputStream.Close();
        Response.End();
        m.Close();
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

            texto = new Paragraph(string.Empty, ftHeaderB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter2.AddCell(celda);

            tblFooterB.AddCell(tblFooter2);

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
