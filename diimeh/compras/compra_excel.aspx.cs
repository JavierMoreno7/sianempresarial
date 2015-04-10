using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class compras_compra_excel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool swVer, swTot;
            if(Request.QueryString["t"].ToString().Equals("o") &&
                !CComunDB.CCommun.ValidarAcceso(10000, out swVer, out swTot))
            {
                Response.Redirect("../inicio/error.aspx");
                return;
            }
            else
                if (Request.QueryString["t"].ToString().Equals("c") &&
                !CComunDB.CCommun.ValidarAcceso(10010, out swVer, out swTot))
                {
                    Response.Redirect("../inicio/error.aspx");
                    return;
                }
                else
                    if (Request.QueryString["t"].ToString().Equals("d") &&
                        !CComunDB.CCommun.ValidarAcceso(10020, out swVer, out swTot))
                    {
                        Response.Redirect("../inicio/error.aspx");
                        return;
                    }

            if (Request.QueryString["t"] != null)
            {
                Generar_Reporte(Request.QueryString["t"].ToString());
            }
        }
    }

    private void Generar_Reporte(string strTipo)
    {
        if (Request.QueryString["m"] == null)
            Response.Clear();

        DataSet objDataResult = new DataSet();
        string strQuery = string.Empty;
        switch (strTipo)
        {
            case "o":
                strQuery = "SELECT ID, monto_subtotal, descuento, monto_descuento " +
                    ",porc_iva, monto_iva, total " +
                    " FROM compra_orden " +
                    " WHERE ID = " + Request.QueryString["c"].ToString();
                break;
            case "c":
                strQuery = "SELECT ID, monto_subtotal, descuento, monto_descuento " +
                    ",porc_iva, monto_iva, total " +
                    " FROM compra " +
                    " WHERE ID = " + Request.QueryString["c"].ToString();
                break;
            case "d":
                strQuery = "SELECT ID, monto_subtotal, descuento, monto_descuento " +
                    ",porc_iva, monto_iva, total " +
                    " FROM compra_dev " +
                    " WHERE ID = " + Request.QueryString["c"].ToString();
                break;
        }

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResultC = objDataResult.Tables[0].Rows[0];

        switch (strTipo)
        {
            case "o":
                strQuery = "SELECT P.proveedor, P.contacto, C.fecha_creacion " +
                   " FROM compra_orden C" +
                   " INNER JOIN proveedores P " +
                   " ON C.proveedorID = P.ID " +
                   " WHERE C.ID = " + Request.QueryString["c"].ToString();
                break;
            case "c":
                strQuery = "SELECT P.proveedor, P.contacto, C.fecha_creacion " +
                    " FROM compra C" +
                    " INNER JOIN proveedores P " +
                    " ON C.proveedorID = P.ID " +
                    " WHERE C.ID = " + Request.QueryString["c"].ToString();
                break;
            case "d":
                strQuery = "SELECT P.proveedor, P.contacto, C.fecha_creacion " +
                    " FROM compra_dev C" +
                    " INNER JOIN proveedores P " +
                    " ON C.proveedorID = P.ID " +
                    " WHERE C.ID = " + Request.QueryString["c"].ToString();
                break;
        }

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResultE = objDataResult.Tables[0].Rows[0];

        switch (strTipo)
        {
            case "o":
                strQuery = "SELECT * FROM (" +
                           "SELECT C.productoID as productoID " +
                           ", C.cantidad as cantidad " +
                           ", C.consecutivo as consecutivo " +
                           ", C.costo_unitario as costo_unitario " +
                           ", C.costo as costo " +
                           ", C.exento as exento " +
                           ", P.nombre as producto " +
                           ", P.codigo " +
                           ", TRIM(P.ubicacion) as ubicacion " +
                           " FROM compra_orden_productos C " +
                           " INNER JOIN productos P " +
                           " ON C.productoID = P.ID " +
                           " AND compra_ordenID = " + Request.QueryString["c"].ToString() +
                           ") AS AA ORDER BY consecutivo, producto";
                break;
            case "c":
                strQuery = "SELECT * FROM (" +
                           "SELECT C.productoID as productoID " +
                           ", C.cantidad as cantidad " +
                           ", C.consecutivo as consecutivo " +
                           ", C.costo_unitario as costo_unitario " +
                           ", C.costo as costo " +
                           ", C.exento as exento " +
                           ", P.nombre as producto " +
                           ", P.codigo " +
                           ", TRIM(P.ubicacion) as ubicacion " +
                           " FROM compra_productos C " +
                           " INNER JOIN productos P " +
                           " ON C.productoID = P.ID " +
                           " AND compraID = " + Request.QueryString["c"].ToString() +
                           ") AS AA ORDER BY consecutivo, producto";
                break;
            case "d":
                strQuery = "SELECT * FROM (" +
                           "SELECT C.productoID as productoID " +
                           ", C.cantidad as cantidad " +
                           ", C.consecutivo as consecutivo " +
                           ", C.costo_unitario as costo_unitario " +
                           ", C.costo as costo " +
                           ", C.exento as exento " +
                           ", P.nombre as producto " +
                           ", P.codigo " +
                           ", TRIM(P.ubicacion) as ubicacion " +
                           " FROM compra_dev_productos C " +
                           " INNER JOIN productos P " +
                           " ON C.productoID = P.ID " +
                           " AND compra_devID = " + Request.QueryString["c"].ToString() +
                           ") AS AA ORDER BY consecutivo, producto";
                break;
        }

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        FileInfo newFile;

        int intRegistros = objDataResult.Tables[0].Rows.Count;

        DataRow[] dtUbicacion = objDataResult.Tables[0].Select("ubicacion <> ''");

        intRegistros += dtUbicacion.Length;

        int intMaxRow;
        if (intRegistros < 31)
        {
            newFile = new FileInfo(Server.MapPath(@"./formatocompra.xlsx"));
            intMaxRow = 42;
        }
        else
            if (intRegistros < 61)
            {
                newFile = new FileInfo(Server.MapPath(@"./formatocompra60.xlsx"));
                intMaxRow = 72;
            }
            else
            {
                newFile = new FileInfo(Server.MapPath(@"./formatocompra100.xlsx"));
                intMaxRow = 112;
            }

        ExcelPackage xlsFile = new ExcelPackage(newFile);
        ExcelWorksheet xlsWorkSheet = xlsFile.Workbook.Worksheets.First();

        xlsWorkSheet.Cells["E9"].Value = CRutinas.PrimeraLetraMayuscula(((DateTime)objRowResultE["fecha_creacion"]).ToString("ddd dd MMM yy", CultureInfo.CreateSpecificCulture("es-MX")));
        switch (strTipo)
        {
            case "o":
                xlsWorkSheet.Cells["D10"].Value = "ORDEN DE COMPRA " +
                                                  objRowResultC["ID"].ToString();
                break;
            case "c":
                xlsWorkSheet.Cells["D10"].Value = "COMPRA " +
                                                  objRowResultC["ID"].ToString();
                break;
            case "d":
                xlsWorkSheet.Cells["D10"].Value = "DEVOLUCIÓN " +
                                                  objRowResultC["ID"].ToString();
                break;
        }
        xlsWorkSheet.Cells["B9"].Value = objRowResultE["contacto"].ToString();
        xlsWorkSheet.Cells["B10"].Value = objRowResultE["proveedor"].ToString();

        int intRow = 12;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = string.Empty;
            if (intRow < intMaxRow)
            {
                if (Convert.ToBoolean(objRowResult["exento"]))
                    strProducto = "(" + objRowResult["codigo"].ToString() + ") " +
                                    objRowResult["producto"].ToString();
                else
                    strProducto = "(" + objRowResult["codigo"].ToString() + ") " +
                                    objRowResult["producto"].ToString() + 
                                    "*";
                xlsWorkSheet.Cells["A" + intRow.ToString()].Value = (decimal)objRowResult["cantidad"];
                xlsWorkSheet.Cells["B" + intRow.ToString()].Value = strProducto;
                xlsWorkSheet.Cells["D" + intRow.ToString()].Value = (decimal)objRowResult["costo_unitario"];
            }

            intRow++;

            if (!string.IsNullOrEmpty(objRowResult["ubicacion"].ToString()) && intRow < intMaxRow)
            {
                xlsWorkSheet.Cells["B" + intRow.ToString()].Value = objRowResult["ubicacion"].ToString();
                intRow++;
            }
        }

        if (intRegistros < 31)
        {
            xlsWorkSheet.Cells["D45"].Value = "IVA " + ((decimal)objRowResultC["porc_iva"]).ToString("0.##") + "%";
            xlsWorkSheet.Cells["E45"].Value = ((decimal)objRowResultC["monto_iva"]).ToString("c");
            xlsWorkSheet.Cells["E46"].Value = ((decimal)objRowResultC["total"]).ToString("c");
            if ((decimal)objRowResultC["monto_descuento"] != 0)
            {
                xlsWorkSheet.Cells["D44"].Value = "MONTO DESCTO " + ((decimal)objRowResultC["descuento"]).ToString("0.##") + "%";
                xlsWorkSheet.Cells["E44"].Value = ((decimal)objRowResultC["monto_descuento"]).ToString("c");
            }
            else
                xlsWorkSheet.DeleteRow(44, 1);
        }
        else
            if (intRegistros < 61)
            {
                xlsWorkSheet.Cells["D75"].Value = "IVA " + ((decimal)objRowResultC["porc_iva"]).ToString("0.##") + "%";
                xlsWorkSheet.Cells["E75"].Value = ((decimal)objRowResultC["monto_iva"]).ToString("c");
                xlsWorkSheet.Cells["E76"].Value = ((decimal)objRowResultC["total"]).ToString("c");
                if ((decimal)objRowResultC["monto_descuento"] != 0)
                {
                    xlsWorkSheet.Cells["D74"].Value = "MONTO DESCTO " + ((decimal)objRowResultC["descuento"]).ToString("0.##") + "%";
                    xlsWorkSheet.Cells["E74"].Value = ((decimal)objRowResultC["monto_descuento"]).ToString("c");
                }
                else
                    xlsWorkSheet.DeleteRow(74, 1);
            }
            else
            {
                xlsWorkSheet.Cells["D115"].Value = "IVA " + ((decimal)objRowResultC["porc_iva"]).ToString("0.##") + "%";
                xlsWorkSheet.Cells["E115"].Value = ((decimal)objRowResultC["monto_iva"]).ToString("c");
                xlsWorkSheet.Cells["E116"].Value = ((decimal)objRowResultC["total"]).ToString("c");
                if ((decimal)objRowResultC["monto_descuento"] != 0)
                {
                    xlsWorkSheet.Cells["D114"].Value = "MONTO DESCTO " + ((decimal)objRowResultC["descuento"]).ToString("0.##") + "%";
                    xlsWorkSheet.Cells["E114"].Value = ((decimal)objRowResultC["monto_descuento"]).ToString("c");
                }
                else
                    xlsWorkSheet.DeleteRow(114, 1);
            }

        Regex regexInValidChars = new Regex(@"[\\\/:\*\?""'<>|]");

        string strFile = string.Empty;
        switch (strTipo)
        {
            case "o":
                strFile = regexInValidChars.Replace(objRowResultE["proveedor"].ToString(), "").Replace(" ", "_") +
                         "_ORD_" + Request.QueryString["c"].ToString() + ".xlsx";
                break;
            case "c":
                strFile = regexInValidChars.Replace(objRowResultE["proveedor"].ToString(), "").Replace(" ", "_") +
                         "_COM_" + Request.QueryString["c"].ToString() + ".xlsx";
                break;
            case "d":
                strFile = regexInValidChars.Replace(objRowResultE["proveedor"].ToString(), "").Replace(" ", "_") +
                         "_DEV_" + Request.QueryString["c"].ToString() + ".xlsx";
                break;
        }

        if (Request.QueryString["m"] == null)
        {
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; " +
                    "filename=" + strFile);
            Response.BinaryWrite(xlsFile.GetAsByteArray());
            Response.End();
        }
        else
        {
            try
            {
                Byte[] btArchivo = xlsFile.GetAsByteArray();
                File.WriteAllBytes(Server.MapPath("../xml_facturas") + "/" + strFile, btArchivo);
            }
            catch
            { }
            Response.Redirect("compras_correo.aspx?t=" + strTipo +
                                                 "&c=" + Request.QueryString["c"].ToString() +
                                                 "&f=" + strFile);
        }
    }
}
