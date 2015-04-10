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

public partial class facturas_nota_impresion : BasePagePopUp
{
    CFacturaElectronica objFactura;
    string strProveedor;
    string strContacto;
    string strNotaID;
    string strVendedor;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["notID"] != null)
        {
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT F.nota, F.nota_suf, E.proveedor, " +
                    " E.contacto, P.nombre, P.apellidos " +
                    " FROM notas F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN personas P " +
                    " ON F.vendedorID = P.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " WHERE F.ID = " + Request.QueryString["notID"];
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
                strNotaID = objRowResult["nota"].ToString() + " " +
                            objRowResult["nota_suf"].ToString();
                strProveedor = objRowResult["proveedor"].ToString();
                strContacto = objRowResult["contacto"].ToString();
                strVendedor = objRowResult["nombre"].ToString() + " " +
                              objRowResult["apellidos"].ToString();
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

        CNotaDB objNotaDB = new CNotaDB();
        if (!objNotaDB.Leer(int.Parse(Request.QueryString["notID"])))
            return;

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT rfc, razonsocial, " +
                   "Dom_Calle, Dom_NumExt, Dom_NumInt, Dom_Colonia, " +
                   "Dom_Localidad, Dom_Referencia, Dom_Municipio, Dom_Estado, " +
                   "Dom_Pais, Dom_CP, Exp_Calle, Exp_NumExt, Exp_NumInt, " +
                   "Exp_Colonia, Exp_Localidad, Exp_Municipio, Exp_Estado, " +
                   "Exp_Pais, Exp_CP, Lugar_Expedicion, Regimen " +
                   "FROM sistema_apps " +
                   "WHERE ID = " +  Session["SIANAppID"];

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

        decimal dcmDescuento = 0;
        if (dcmPorcDescuento > 0)
            dcmDescuento = dcmSubtotal - objNotaDB.monto_descuento;


        objFactura.AmtTotal = dcmTotal;
        objFactura.AmtSubTotal = dcmSubtotal;
        if (dcmDescuento != 0)
            objFactura.AmtDescuento = dcmDescuento;

        objFactura.StrNombreEmisor = objRowResult["razonsocial"].ToString();
        objFactura.StrRFCEmisor = objRowResult["rfc"].ToString();

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

        objFactura.StrNombreCliente = objRowResult["negocio"].ToString() + " - " +
                                    objRowResult["sucursal"].ToString();
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

        strQuery = "SELECT L.* " +
                    ",P.clave as clave " +
                    ",P.nombre as nombre " +
                    ",P.unimed as unimed " +
                    ",P.piezasporcaja as piezasporcaja " +
                    ",P.unimed2" +
                    ",P.relacion1" +
                    ",P.relacion2" +
                    ",P.unimed_original" +
                    ",P2.nombre as producto_grupo" +
                    " FROM notas_prod L" +
                    " INNER JOIN productos P " +
                    " ON L.producto_ID = P.ID " +
                    " AND L.nota_ID = " + Request.QueryString["notID"] +
                    " LEFT JOIN productos P2" +
                    " ON P2.ID = L.grupoID" +
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
        decimal dcmCantidadAlterno;
        string strGrupoConsecutivo = string.Empty;

        foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
        {
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

            CProducto objProducto = new CProducto();
            objProducto.StrDescripcion = objRowResult2["nombre"].ToString();
            objProducto.AmtCantidad = (decimal)objRowResult2["cantidad"];

            if (!string.IsNullOrEmpty(objRowResult2["grupoID"].ToString()))
                objProducto.IntGrupoID = (int)objRowResult2["grupoID"];

            if (string.IsNullOrEmpty(objRowResult2["unimed2"].ToString()))
            {
                objProducto.StrDescripcion += " (" + objRowResult2["unimed"].ToString() + ")";
            }
            else
            {
                if ((bool)objRowResult2["unimed_original"])
                {
                    if ((decimal)objRowResult2["relacion1"] == 1)
                        dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objRowResult2["relacion2"], 2);
                    else
                        dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objRowResult2["relacion1"], 2);

                    objProducto.StrDescripcion += " (" + objRowResult2["unimed"].ToString() + ")\n" +
                                                "(Equivalente a " +
                                                dcmCantidadAlterno.ToString("0.##") + " " +
                                                objRowResult2["unimed2"].ToString() + ")";
                }
                else
                {
                    if ((bool)objRowResult2["usar_unimed2"])
                    {
                        if ((decimal)objRowResult2["relacion1"] == 1)
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objRowResult2["relacion2"], 2);
                        else
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objRowResult2["relacion1"], 2);

                        objProducto.StrDescripcion += " (" + objRowResult2["unimed2"].ToString() + ")\n" +
                                                    "(Equivalente a " +
                                                    dcmCantidadAlterno.ToString("0.##") + " " +
                                                    objRowResult2["unimed"].ToString() + ")";
                    }
                    else
                    {
                        if ((decimal)objRowResult2["relacion1"] == 1)
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad / (decimal)objRowResult2["relacion2"], 2);
                        else
                            dcmCantidadAlterno = Math.Round(objProducto.AmtCantidad * (decimal)objRowResult2["relacion1"], 2);

                        objProducto.StrDescripcion += " (" + objRowResult2["unimed"].ToString() + ")\n" +
                                                    "(Equivalente a " +
                                                    dcmCantidadAlterno.ToString("0.##") + " " +
                                                    objRowResult2["unimed2"].ToString() + ")";
                    }
                }
            }

            objProducto.AmtValorUnitario = (decimal)objRowResult2["costo_unitario"];
            objProducto.AmtImporte = (decimal)objRowResult2["costo"];
            objProducto.StrUnidad = objRowResult2["unimed"].ToString();
            lstProductos.Add(objProducto);
        }
        objFactura.LstProductos = lstProductos;

        if (dcmPorIva > 0)
        {
            objFactura.AmtTasa = dcmPorIva;
            objFactura.AmtImpuesto = dcmIva;
        }

        objFactura.StrVersionCFDI = "3.2";
        objFactura.StrFolioFiscal = "N/A";
        objFactura.StrCertificadoSAT = "N/A";
        objFactura.StrSelloCFDI = "N/A";
        objFactura.StrSelloSAT = "N/A";
        objFactura.DtFechaCertificado = objNotaDB.fecha;

    }

    private int Crear_Archivo_Temporal()
    {
        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 240.0f, 270.0f);

        MemoryStream m = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        document.Open();

        float[] ancho_columnas = new float[5];
        ancho_columnas[0] = 60;
        ancho_columnas[1] = 10;
        ancho_columnas[2] = 335;
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

            texto = new Paragraph(string.Empty, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
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

        Font ftProductos = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));

        MyPageEvents events = new MyPageEvents();
        events._strNotaID = strNotaID;
        events._strContacto = strContacto;
        events._strCertificado = objFactura.StrCertificadoSAT;
        events._strFolioFiscal = objFactura.StrFolioFiscal;
        events._strSelloCFDI = objFactura.StrSelloCFDI;
        events._strSelloSAT = objFactura.StrSelloSAT;
        events._strCadenaOriginal = "N/A";
        events._intFolio = objFactura.IntFolio;
        events._dtFechaCertificado = objFactura.DtFechaCertificado;
        events._intNoAprobacion = objFactura.IntNoAprobacion;
        events._intAnoAprobacion = objFactura.IntAnoAprobacion;
        events._strMetodoDePago = objFactura.StrMetodoDePago;
        events._strCondicionesDePago = objFactura.StrCondicionesDePago;
        events._amtSubTotal = objFactura.AmtSubTotal;
        events._amtDescuento = objFactura.AmtDescuento;
        events._amtTotal = objFactura.AmtTotal;
        events._strTotalLetras = CRutinas.ObtenerImporteLetras(objFactura.AmtTotal, objFactura.StrMoneda);
        events._strRFCEmisor = objFactura.StrRFCEmisor;
        events._strNombreEmisor = objFactura.StrNombreEmisor;
        events._dirDomicilioFiscal = objFactura.DirDomicilioFiscal;
        events._dirDomicilioExpedicion = objFactura.DirDomicilioExpedicion;
        events._strRFCCliente = objFactura.StrRFCCliente;
        events._strNombreCliente = objFactura.StrNombreCliente;
        events._strCedulaFiscalCliente = objFactura.StrCedulaFiscalCliente;
        events._strCedulaIEPSCliente = objFactura.StrCedulaIEPSCliente;
        events._dirDomicilioCliente = objFactura.DirDomicilioCliente;
        events._amtTotalImpuestosRetenidos = objFactura.AmtTotalImpuestosRetenidos;
        events._amtImpuesto = objFactura.AmtImpuesto;
        events._amtTasa = objFactura.AmtTasa;
        events._amtImporte = objFactura.AmtImporte;
        events._amtTotalImpuestos = objFactura.AmtTotalImpuestos;
        events._strMapPath = Server.MapPath("../imagenes") + "/";
        events._strLogoEmpresa = ckSIAN["ck_logo"].Substring(intNombreLogo);
        events._flEscala = flRelacion * 100;
        events._intTotalPaginas = intTotalPaginas;
        events._strOrdenCompra = string.Empty;
        events._strProveedor = strProveedor;
        events._strVendedor = strVendedor;

        MemoryStream m = new MemoryStream();
        Document document = new Document(
            new Rectangle(PageSize.LETTER.Width, PageSize.LETTER.Height));
        // 1 in = 25.4 mm = 72 points
        document.SetMargins(32.0f, 32.0f, 205.0f, 150.0f);

        Response.ContentType = "application/pdf";
        PdfWriter writer = PdfWriter.GetInstance(document, m);
        writer.CloseStream = false;
        writer.PageEvent = events;

        document.Open();

        float[] ancho_columnas = new float[5];
        ancho_columnas[0] = 60;
        ancho_columnas[1] = 10;
        ancho_columnas[2] = 335;
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

            texto = new Paragraph(string.Empty, ftProductos);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
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
        public string _strNotaID;
        public string _strMapPath;
        public string _strLogoEmpresa;
        public float _flEscala;
        public string _strFolioFiscal;
        public string _strCertificado;
        public string _strSelloCFDI;
        public string _strSelloSAT;
        public string _strCadenaOriginal;
        public string _strProveedor;
        public string _strOrdenCompra;
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
        public decimal _amtImpuestoRetenido;
        public decimal _amtImporteRetenido;
        public decimal _amtTotalImpuestosRetenidos;
        public decimal _amtImpuesto;
        public decimal _amtTasa;
        public decimal _amtImporte;
        public decimal _amtTotalImpuestos;
        public int _intTotalPaginas;
        public string _strContacto;
        public string _strVendedor;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            StringBuilder strCadena = new StringBuilder();
            Font ftHeader = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL));
            Font ftFooter = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.NORMAL));
            Font ftHeaderB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD));
            Font ftFooterB = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 6, Font.BOLD));
            Font ftHeader30 = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 30, Font.NORMAL));
            Font ftHeader11 = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 11, Font.NORMAL));

            Rectangle page = document.PageSize;

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

            float[] ancho_columnas = new float[2];
            ancho_columnas[0] = 120;
            ancho_columnas[1] = tblHeaderA.TotalWidth - 120;
            PdfPTable tblHeader1 = new PdfPTable(ancho_columnas);

            iTextSharp.text.Image imgEmpresa = iTextSharp.text.Image.GetInstance(_strMapPath + _strLogoEmpresa);
            imgEmpresa.SetAbsolutePosition(45, 700);
            imgEmpresa.ScalePercent(_flEscala, _flEscala);
            document.Add(imgEmpresa);

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

            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);

            tblHeaderA.AddCell(tblHeader1);

            //Datos Expedición
            float[] ancho_columnas2 = new float[4];
            ancho_columnas2[0] = 100;
            ancho_columnas2[1] = 200;
            ancho_columnas2[3] = 100;
            ancho_columnas2[2] = tblHeaderB.TotalWidth - 400;
            PdfPTable tblHeader2 = new PdfPTable(ancho_columnas2);
            texto = new Paragraph("AT'N:", ftHeaderB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph(_strContacto, ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph("FECHA:", ftHeaderB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph(CRutinas.Fecha_DD_MMM_YYYY(_dtFechaCertificado), ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph(string.Empty, ftHeaderB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph(_strNombreCliente, ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph("REMISIÓN:", ftHeaderB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            texto = new Paragraph(_strNotaID, ftHeader);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_LEFT;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader2.AddCell(celda);

            tblHeaderB.AddCell(tblHeader2);

            // Cuádricula para los productos

            PdfContentByte cb = writer.DirectContent;

            // x, y, ancho, alto
            cb.Rectangle(30, 600, 550, 15);
            cb.Stroke();

            cb.MoveTo(100, 600);
            cb.LineTo(100, 615);
            cb.Stroke();

            cb.MoveTo(440, 600);
            cb.LineTo(440, 615);
            cb.Stroke();

            cb.MoveTo(520, 600);
            cb.LineTo(520, 615);
            cb.Stroke();

            cb.Rectangle(30, 150, 550, 440);
            cb.Stroke();

            cb.MoveTo(100, 150);
            cb.LineTo(100, 590);
            cb.Stroke();

            cb.MoveTo(440, 150);
            cb.LineTo(440, 590);
            cb.Stroke();

            cb.MoveTo(520, 150);
            cb.LineTo(520, 590);
            cb.Stroke();

            // Encabezados de los productos
            float[] ancho_columnas3 = new float[4];
            ancho_columnas3[0] = 55;
            ancho_columnas3[1] = 345;
            ancho_columnas3[2] = 68;
            ancho_columnas3[3] = 60;
            PdfPTable tblHeader3 = new PdfPTable(ancho_columnas3);
            texto = new Paragraph("CANTIDAD", ftHeader);
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

            texto = new Paragraph("TOTAL", ftHeader);
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
                document.LeftMargin, 670,
                writer.DirectContent);

            tblHeaderC.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 617,
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

            texto = new Paragraph(_amtSubTotal.ToString("c"), ftFooter);
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
                texto = new Paragraph(amtConDescuento.ToString("c"), ftFooter);
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

            texto = new Paragraph(_amtImpuesto.ToString("c"), ftFooter);
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

            texto = new Paragraph("TOTAL:", ftFooterB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(_amtTotal.ToString("c"), ftFooterB);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Border = Rectangle.BOTTOM_BORDER;
            tblFooter1.AddCell(celda);

            texto = new Paragraph(" \n\n\n ", ftFooter);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 3;
            tblFooter1.AddCell(celda);


            tblFooterA.AddCell(tblFooter1);

            float[] ancho_columnasFB = new float[4];
            ancho_columnasFB[0] = 200;
            ancho_columnasFB[1] = 200;
            ancho_columnasFB[2] = 200;
            ancho_columnasFB[3] = 200;
            PdfPTable tblFooter2 = new PdfPTable(ancho_columnasFB);

            texto = new Paragraph(string.Empty, ftHeaderB);
            celda = new PdfPCell(texto);
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Colspan = 2;
            tblFooter2.AddCell(celda);

            texto = new Paragraph(_strVendedor, ftHeader);
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
            celda.Colspan = 1;
            tblFooter2.AddCell(celda);

            texto = new Paragraph("CLIENTE", ftHeader);
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

            texto = new Paragraph("VENDEDOR", ftHeaderB);
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

            tblFooterB.AddCell(tblFooter2);

            #endregion

            tblFooterA.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 150,
                writer.DirectContent);

            tblFooterB.WriteSelectedRows(0, -1, 0, -1,
                document.LeftMargin, 90,
                writer.DirectContent);
        }
    }
}
