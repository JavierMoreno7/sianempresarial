using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using CFE;

public partial class facturas_nota_impresion_ipisa : BasePagePopUp
{
    string strRazonSocial;
    string strRFC;
    string strTelefono;
    string strEmail;
    CDireccion objDirEmisor = new CDireccion();
    string strNumeroCliente;
    string strCliente;
    string strClienteRFC;
    string strDiasCredito;
    string strContacto;
    CDireccion objDirReceptor = new CDireccion();
    CDireccion objEnvio = new CDireccion();
    string strNota;
    string strEjecutivoVentas;
    string strCotizacion;
    string strOrdenCompra;
    DateTime dtFecha;

    decimal dcmSubtotal;
    decimal dcmDescuento;
    decimal dcmIVA;
    decimal dcmTotal;
    string strMoneda;

    // Productos
    List<CProducto> lstProductos = new List<CProducto>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["notID"] == null)
            return;

        if (!IsPostBack)
        {
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT 1" +
                                                               " FROM notas" +
                                                               " WHERE ID = " + Request.QueryString["notID"]);

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                this.lblMessage.Text = "Remisión no existe";
                return;
            }

            Generar_PDF();
        }
    }

    private void Generar_PDF()
    {
        Obtener_Datos();

        if (this.lstProductos.Count == 0)
        {
            this.lblMessage.Text = "Remisión no tiene productos";
            return;
        }

        int intTotalPaginas = Crear_Archivo_Temporal();
        Crear_Archivo(intTotalPaginas);
    }


    private void Obtener_Datos()
    {
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT establecimiento_ID" +
                                                            " FROM sucursales S" +
                                                            " INNER JOIN notas C" +
                                                            " ON C.sucursal_ID = S.ID" +
                                                            " AND C.ID = " + Request.QueryString["notID"]);

        string strEstabID = objDataResult.Tables[0].Rows[0]["establecimiento_ID"].ToString();

        // Tabla 0 - Cotizacion
        // Tabla 1 - Sucursal/Cliente
        // Tabla 2 - Productos
        objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT *" +
                                                    " FROM notas" +
                                                    " WHERE ID = " + Request.QueryString["notID"] +
                                                    ";" +   // 1
                                                    "SELECT S.direccion as calle_envio" +
                                                    ",S.num_exterior as num_exterior_envio" +
                                                    ",S.num_interior as num_interior_envio" +
                                                    ",S.colonia as colonia_envio" +
                                                    ",S.municipio as municipio_envio" +
                                                    ",S.estado as estado_envio" +
                                                    ",S.pais as pais_envio" +
                                                    ",S.cp as cp_envio" +
                                                    ",S.poblacion as poblacion_envio" +
                                                    ",E.razonsocial" +
                                                    ",E.rfc" +
                                                    ",E.dias_credito" +
                                                    ",E.direccionfiscal" +
                                                    ",E.num_exterior" +
                                                    ",E.num_interior" +
                                                    ",E.colonia" +
                                                    ",E.municipio" +
                                                    ",E.estado" +
                                                    ",E.pais" +
                                                    ",E.cp" +
                                                    ",E.poblacion" +
                                                    ",E.dias_credito" +
                                                    ",E.contacto_compras" +
                                                    ",CONCAT(P.nombre, ' ' , P.apellidos) as vendedor" +
                                                    " FROM sucursales S" +
                                                    " INNER JOIN establecimientos E" +
                                                    " ON E.ID = S.establecimiento_ID" +
                                                    " AND E.ID = " + strEstabID +
                                                    " LEFT JOIN personas P" +
                                                    " ON P.ID = E.vendedorID" +
                                                    ";" +   // 2
                                                    " SELECT C.*" +
                                                    ",P.nombre" +
                                                    ",P.codigo" +
                                                    ",P.unimed" +
                                                    ",S.clave" +
                                                    ",P2.nombre as producto_grupo" +
                                                    " FROM notas_prod  C " +
                                                    " INNER JOIN productos P " +
                                                    " ON C.producto_ID = P.ID " +
                                                    " AND nota_ID = " + Request.QueryString["notID"] +
                                                    " LEFT JOIN productos P2" +
                                                    " ON P2.ID = C.grupoID" +
                                                    " LEFT JOIN establecimiento_producto S" +
                                                    " ON S.productoID = C.producto_ID" +
                                                    " AND S.establecimientoID = " + strEstabID +
                                                    " WHERE nota_ID = " + Request.QueryString["notID"] +
                                                    " ORDER BY consecutivo" +
                                                    ";" +   // 3
                                                    " SELECT orden_compraID" +
                                                    " FROM orden_compra_nota" +
                                                    " WHERE notaID = " + Request.QueryString["notID"] +
                                                    " ORDER BY orden_compraID" +
                                                    ";" +   //4
                                                    " SELECT C.cotizacionID" +
                                                    " FROM cotizacion_orden_compra C" +
                                                    " INNER JOIN orden_compra_nota O" +
                                                    " ON C.orden_compraID = O.orden_compraID" +
                                                    " AND O.notaID = " + Request.QueryString["notID"] +
                                                    " ORDER BY C.cotizacionID");

        DataSet objDataResult2 = CComunDB.CCommun.Ejecutar_SP_Usu("SELECT *" +
                                                                 " FROM sistema_apps " +
                                                                 " WHERE ID = " + Session["SIANAppID"]);

        this.strNota = Request.QueryString["notID"];
        this.dtFecha = (DateTime)objDataResult.Tables[0].Rows[0]["fecha"];

        this.dcmSubtotal = (decimal)objDataResult.Tables[0].Rows[0]["monto_subtotal"];
        this.dcmDescuento = (decimal)objDataResult.Tables[0].Rows[0]["monto_descuento"];
        this.dcmIVA = (decimal)objDataResult.Tables[0].Rows[0]["monto_iva"];
        this.dcmTotal = (decimal)objDataResult.Tables[0].Rows[0]["total"];
        this.strMoneda = objDataResult.Tables[0].Rows[0]["moneda"].ToString();

        StringBuilder strTemp = new StringBuilder();
        foreach (DataRow objRow in objDataResult.Tables[3].Rows)
        {
            if (strTemp.Length != 0)
                strTemp.Append(", ");
            strTemp.Append(objRow[0].ToString());
        }
        this.strOrdenCompra = strTemp.ToString();

        strTemp.Clear();
        foreach (DataRow objRow in objDataResult.Tables[4].Rows)
        {
            if (strTemp.Length != 0)
                strTemp.Append(", ");
            strTemp.Append(objRow[0].ToString());
        }
        this.strCotizacion = strTemp.ToString();

        this.strNumeroCliente = "C" + int.Parse(strEstabID).ToString("0000");
        this.strCliente = objDataResult.Tables[1].Rows[0]["razonsocial"].ToString();
        this.strClienteRFC = objDataResult.Tables[1].Rows[0]["rfc"].ToString();
        this.strDiasCredito = objDataResult.Tables[1].Rows[0]["dias_credito"].ToString();
        this.strContacto = objDataResult.Tables[1].Rows[0]["contacto_compras"].ToString();
        objDirReceptor.StrCalle = objDataResult.Tables[1].Rows[0]["direccionfiscal"].ToString();
        objDirReceptor.StrNumeroExterior = objDataResult.Tables[1].Rows[0]["num_exterior"].ToString();
        objDirReceptor.StrNumeroInterior = objDataResult.Tables[1].Rows[0]["num_interior"].ToString();
        objDirReceptor.StrColonia = objDataResult.Tables[1].Rows[0]["colonia"].ToString();
        objDirReceptor.StrCP = objDataResult.Tables[1].Rows[0]["cp"].ToString();
        objDirReceptor.StrLocalidad = objDataResult.Tables[1].Rows[0]["poblacion"].ToString();
        objDirReceptor.StrMunicipio = objDataResult.Tables[1].Rows[0]["municipio"].ToString();
        objDirReceptor.StrEstado = objDataResult.Tables[1].Rows[0]["estado"].ToString();
        objDirReceptor.StrPais = objDataResult.Tables[1].Rows[0]["pais"].ToString();

        objEnvio.StrCalle = objDataResult.Tables[1].Rows[0]["calle_envio"].ToString();
        objEnvio.StrNumeroExterior = objDataResult.Tables[1].Rows[0]["num_exterior_envio"].ToString();
        objEnvio.StrNumeroInterior = objDataResult.Tables[1].Rows[0]["num_interior_envio"].ToString();
        objEnvio.StrColonia = objDataResult.Tables[1].Rows[0]["colonia_envio"].ToString();
        objEnvio.StrCP = objDataResult.Tables[1].Rows[0]["cp_envio"].ToString();
        objEnvio.StrLocalidad = objDataResult.Tables[1].Rows[0]["poblacion_envio"].ToString();
        objEnvio.StrMunicipio = objDataResult.Tables[1].Rows[0]["municipio_envio"].ToString();
        objEnvio.StrEstado = objDataResult.Tables[1].Rows[0]["estado_envio"].ToString();
        objEnvio.StrPais = objDataResult.Tables[1].Rows[0]["pais_envio"].ToString();

        this.strEjecutivoVentas = objDataResult.Tables[1].Rows[0]["vendedor"].ToString();

        string strGrupoConsecutivo = string.Empty;
        //for(int i=0; i<=10;i++)
       
        foreach (DataRow objRow in objDataResult.Tables[2].Rows)
        {
            if (!string.IsNullOrEmpty(objRow["grupoID"].ToString()))
            {
                if (!strGrupoConsecutivo.Equals(objRow["grupoID"].ToString() + "_" + objRow["grupo_consecutivo"].ToString()))
                {
                    CProducto objProductoKit = new CProducto();
                    objProductoKit.StrDescripcion = objRow["producto_grupo"].ToString() + ", Cant: " +
                                                    ((decimal)objRow["grupo_cantidad"]).ToString("0.##");
                    objProductoKit.SwKit = true;
                    strGrupoConsecutivo = objRow["grupoID"].ToString() + "_" + objRow["grupo_consecutivo"].ToString();
                    lstProductos.Add(objProductoKit);
                }
            }

            CProducto objProducto = new CProducto();

            objProducto.StrDescripcion = objRow["nombre"].ToString();
            objProducto.AmtCantidad = (decimal)objRow["cantidad"];
            objProducto.AmtValorUnitario = (decimal)objRow["costo_unitario"];
            objProducto.AmtImporte = (decimal)objRow["costo"];
            objProducto.StrClaveCliente = objRow["clave"].ToString();
            objProducto.StrCodigo = objRow["codigo"].ToString();
            objProducto.StrUnidad = objRow["unimed"].ToString();
            if ((bool)objRow["imprimir_detalle"])
                objProducto.StrDetalle = objRow["detalle"].ToString();

            if (!string.IsNullOrEmpty(objRow["grupoID"].ToString()))
                objProducto.IntGrupoID = (int)objRow["grupoID"];

            lstProductos.Add(objProducto);
        }

        this.strRazonSocial = objDataResult2.Tables[0].Rows[0]["razonsocial"].ToString();
        this.strRFC = objDataResult2.Tables[0].Rows[0]["rfc"].ToString();
        objDirEmisor.StrCalle = objDataResult2.Tables[0].Rows[0]["Dom_Calle"].ToString();
        objDirEmisor.StrNumeroExterior = objDataResult2.Tables[0].Rows[0]["Dom_NumExt"].ToString();
        objDirEmisor.StrNumeroInterior = objDataResult2.Tables[0].Rows[0]["Dom_NumInt"].ToString();
        objDirEmisor.StrColonia = objDataResult2.Tables[0].Rows[0]["Dom_Colonia"].ToString();
        objDirEmisor.StrLocalidad = objDataResult2.Tables[0].Rows[0]["Dom_Localidad"].ToString();
        objDirEmisor.StrMunicipio = objDataResult2.Tables[0].Rows[0]["Dom_Municipio"].ToString();
        objDirEmisor.StrEstado = objDataResult2.Tables[0].Rows[0]["Dom_Estado"].ToString();
        objDirEmisor.StrPais = objDataResult2.Tables[0].Rows[0]["Dom_Pais"].ToString();
        objDirEmisor.StrCP = objDataResult2.Tables[0].Rows[0]["Dom_CP"].ToString();
        this.strTelefono = objDataResult2.Tables[0].Rows[0]["telefono_impresion"].ToString();
        this.strEmail = objDataResult2.Tables[0].Rows[0]["email_impresion"].ToString();
    }

    private int Crear_Archivo_Temporal()
    {
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));

        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 290.0f, 135.0f);

        MemoryStream m = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        document.Open();

        float[] ancho_columnas = new float[6];
        ancho_columnas[0] = 50;
        ancho_columnas[1] = 75;
        ancho_columnas[2] = 242;
        ancho_columnas[3] = 63;
        ancho_columnas[4] = 62;
        ancho_columnas[5] = 74;

        char tab = '\u0009';

        foreach (CProducto objProducto in this.lstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = 532;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;

            Paragraph texto = new Paragraph(string.Empty, ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrCodigo, ftProductos);
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

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
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
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL));

        int intMaxWidth = 190, intMaxHeight = 50, intLogoWidth, intLogoHeight;
        HttpCookie ckSIAN = Request.Cookies["userCng"];
        int intNombreLogo = ckSIAN["ck_logo"].LastIndexOf("/") + 1;
        string[] strSize = ckSIAN["ck_logo_size"].Split('x');
        intLogoWidth = int.Parse(strSize[0]);
        intLogoHeight = int.Parse(strSize[1]);
        float flRelacion = CRutinas.Calcular_Relacion(intMaxWidth, intMaxHeight, intLogoWidth, intLogoHeight);

        MyPageEvents events = new MyPageEvents();
        events.strMapPath = Server.MapPath("../imagenes") + "/";
        events.strLogoEmpresa = ckSIAN["ck_logo"].Substring(intNombreLogo);
        events.flEscala = flRelacion * 100;

        events.strCotizacion = this.strCotizacion;

        events.strRazonSocial = this.strRazonSocial;
        events.strRFC = this.strRFC;
        events.strTelefono = this.strTelefono;
        events.strEmail = this.strEmail;
        events.objDirEmisor = this.objDirEmisor;
        events.strNumeroCliente = this.strNumeroCliente;
        events.strCliente = this.strCliente;
        events.strClienteRFC = this.strClienteRFC;
        events.strDiasCredito = this.strDiasCredito;
        events.objDirReceptor = this.objDirReceptor;
        events.objEnvio = this.objEnvio;
        events.strEjecutivoVentas = this.strEjecutivoVentas;
        events.dtFecha = this.dtFecha;
        events.strNota = this.strNota;
        events.strOrdenCompra = this.strOrdenCompra;
        events.strContacto = this.strContacto;

        events.dcmSubtotal = this.dcmSubtotal;
        events.dcmDescuento = this.dcmDescuento;
        events.dcmIVA = this.dcmIVA;
        events.dcmTotal = this.dcmTotal;
        events.strMoneda = this.strMoneda;

        events.intTotalPaginas = intTotalPaginas;

        string strArchivoFormato = Server.MapPath("remision_ipisa.pdf");
        PdfReader pdfEntrada = new PdfReader(strArchivoFormato);

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 290.0f, 135.0f);

        Response.ContentType = "application/pdf";
        PdfWriter writer;

        string strArchivo = "remision_" + Request.QueryString["notID"] + ".pdf";
        if (Request.QueryString["m"] == null)
        {
            writer = PdfWriter.GetInstance(document, m);
            writer.CloseStream = false;
        }
        else
        {
            FileStream flArchivo = new FileStream(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/" + strArchivo), FileMode.Create);
            writer = PdfWriter.GetInstance(document, flArchivo);
        }

        writer.PageEvent = events;

        events.pdfPage1 = writer.GetImportedPage(pdfEntrada, 1);

        document.Open();

        float[] ancho_columnas = new float[6];
        ancho_columnas[0] = 50;
        ancho_columnas[1] = 75;
        ancho_columnas[2] = 242;
        ancho_columnas[3] = 63;
        ancho_columnas[4] = 62;
        ancho_columnas[5] = 74;

        char tab = '\u0009';

        foreach (CProducto objProducto in this.lstProductos)
        {
            PdfPTable tblLinea = new PdfPTable(ancho_columnas);
            tblLinea.TotalWidth = 532;
            tblLinea.DefaultCell.Border = Rectangle.NO_BORDER;
            tblLinea.HorizontalAlignment = Element.ALIGN_LEFT;
            tblLinea.LockedWidth = true;

            Paragraph texto = new Paragraph(string.Empty, ftProductos);
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblLinea.AddCell(celda);

            texto = new Paragraph(objProducto.StrCodigo, ftProductos);
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

            texto = new Paragraph((objProducto.SwKit ? string.Empty : objProducto.AmtCantidad.ToString("0.##")), ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
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
            Response.Redirect("cotizacion_correo.aspx?t=c&c=" + Request.QueryString["notID"] +
                                                     "&f=" + strArchivo);
    }

    class MyPageEvents : PdfPageEventHelper
    {
        public string strNota;
        public string strRazonSocial;
        public string strRFC;
        public string strTelefono;
        public string strEmail;
        public CDireccion objDirEmisor = new CDireccion();
        public string strNumeroCliente;
        public string strCliente;
        public string strClienteRFC;
        public string strDiasCredito;
        public CDireccion objDirReceptor = new CDireccion();
        public CDireccion objEnvio = new CDireccion();
        public string strEjecutivoVentas;
        public string strCotizacion;
        public string strOrdenCompra;
        public DateTime dtFecha;
        public string strContacto;

        public string strMapPath;
        public string strLogoEmpresa;
        public float flEscala;

        public decimal dcmSubtotal;
        public decimal dcmDescuento;
        public decimal dcmIVA;
        public decimal dcmTotal;
        public string strMoneda;
        public int intTotalPaginas;

        public PdfImportedPage pdfPage1;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;

            cb.AddTemplate(pdfPage1, 0, 0);

            BaseFont fDetB = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont fDet = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            iTextSharp.text.Image imgEmpresa = iTextSharp.text.Image.GetInstance(strMapPath + strLogoEmpresa);
            imgEmpresa.SetAbsolutePosition(45, 700);
            imgEmpresa.ScalePercent(flEscala, flEscala);
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
            ct.SetSimpleColumn(new Phrase(new Chunk(strRazonSocial,
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 8))),
                                                    150, 700, 410, 744, 10, Element.ALIGN_LEFT);
            ct.Go();

            strTemp.Clear();
            //strTemp.Append(objDirEmisor.Direccion_Lineas_SinPais);
            strTemp.Append(objDirEmisor.Direccion_Lineas);
            strTemp.Append(" RFC: " + CRutinas.RFC_Guiones(strRFC));
            if (!string.IsNullOrEmpty(strTelefono))
                strTemp.Append("\nTel. " + strTelefono);
            if (!string.IsNullOrEmpty(strEmail))
                strTemp.Append("\nEmail: " + strEmail);
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    150, 600, 390, 730, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strNota,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 10))),
                                                    425, 720, 567, 740, 10, Element.ALIGN_CENTER);
            ct.Go();


            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(dtFecha.ToString("dd"),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 8))),
                                                    425, 670, 472, 685, 10, Element.ALIGN_CENTER);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(dtFecha.ToString("MM"),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 8))),
                                                    472, 670, 519, 685, 10, Element.ALIGN_CENTER);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(dtFecha.ToString("yyyy"),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 8))),
                                                    519, 670, 567, 685, 10, Element.ALIGN_CENTER);
            ct.Go();

            strTemp.Clear();
            strTemp.Append(strCliente + "\n");
            strTemp.Append(objDirReceptor.Direccion_Lineas + "\n");
            strTemp.Append("RFC: " + CRutinas.RFC_Guiones(strClienteRFC) + "\n");

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    50, 550, 315, 645, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(objEnvio.Direccion_Lineas,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    310, 550, 567, 645, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strContacto,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    310, 500, 567, 575, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strOrdenCompra,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    150, 500, 300, 560, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strNumeroCliente,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    365, 500, 567, 560, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strCotizacion,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    150, 500, 300, 546, 10, Element.ALIGN_LEFT);
            ct.Go();

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strEjecutivoVentas,
                                                    FontFactory.GetFont(BaseFont.HELVETICA, 7))),
                                                    400, 500, 567, 546, 10, Element.ALIGN_LEFT);
            ct.Go();

            strTemp.Clear();
            strTemp.Append("SUBTOTAL:\n\n");
            if (dcmDescuento != 0)
                strTemp.Append("SUBTOTAL CON DESCUENTO:\n\n");
            strTemp.Append("IVA:\n\n");
            strTemp.Append("TOTAL:");
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    410, 40, 490, 120, 10, Element.ALIGN_RIGHT);
            ct.Go();

            strTemp.Clear();
            strTemp.Append(dcmSubtotal.ToString("c") + " " + strMoneda + "\n\n");
            if (dcmDescuento != 0)
                strTemp.Append(dcmDescuento.ToString("c") + " " + strMoneda + "\n\n");
            strTemp.Append(dcmIVA.ToString("c") + " " + strMoneda + "\n\n");
            strTemp.Append(dcmTotal.ToString("c") + " " + strMoneda + "");
            ct = new ColumnText(cb);
            ct.SetSimpleColumn(new Phrase(new Chunk(strTemp.ToString(),
                                                    FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 7))),
                                                    490, 40, 566, 120, 10, Element.ALIGN_RIGHT);
            ct.Go();

            cb.BeginText();
            cb.SetFontAndSize(fDet, 7);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Hoja " + writer.PageNumber + " de " + intTotalPaginas, 580, 20, 0);
            cb.EndText();
        }
    }
}