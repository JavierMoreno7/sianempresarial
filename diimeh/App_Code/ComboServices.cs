using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AjaxControlToolkit;
using System.Data;
using System.Text;
using System.Globalization;

/// <summary>
/// Summary description for ComboServices
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
[System.Web.Script.Services.ScriptService]
public class ComboServices : System.Web.Services.WebService
{

    public ComboServices()
    {

        //Uncomment the following line if using designed components
        //InitializeComponent();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerProductos(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia" +
            " FROM (" +
            " SELECT ID, concat(nombre, ' - ', sales) as nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR sales LIKE '%" + prefixText + "%'" +
            " OR descripcion LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " OR codigo2 = '" + prefixText + "'" +
            " OR codigo3 = '" + prefixText + "'" +
            " ) AND tipo = 0" +
            " AND activo = 1" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString() +
            " ) R" +
            " LEFT JOIN producto_datos D" +
            " ON D.productoID = R.ID";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString() +
                            "(" + ((decimal)objRowResult["existencia"]).ToString("0.##") + ")",
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerProductosConceptos(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia" +
            " FROM (" +
            " SELECT ID, concat(nombre, ' - ', sales) as nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR sales LIKE '%" + prefixText + "%'" +
            " OR descripcion LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " OR codigo2 = '" + prefixText + "'" +
            " OR codigo3 = '" + prefixText + "'" +
            " ) AND activo = 1" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString() +
            " ) R" +
            " LEFT JOIN producto_datos D" +
            " ON D.productoID = R.ID";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString() +
                            "(" + ((decimal)objRowResult["existencia"]).ToString("0.##") + ")",
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerConceptos(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT ID, nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " ) AND tipo = 2" +
            " AND activo = 1" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString(),
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    // Incluye los no activos
    public string[] ObtenerProductosALL(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia" +
            " FROM (" +
            " SELECT ID, concat(nombre, ' - ', sales) as nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR sales LIKE '%" + prefixText + "%'" +
            " OR descripcion LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " OR codigo2 = '" + prefixText + "'" +
            " OR codigo3 = '" + prefixText + "'" +
            " ) AND tipo = 0" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString() +
            " ) R" +
            " LEFT JOIN producto_datos D" +
            " ON D.productoID = R.ID";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString() +
                            "(" + ((decimal)objRowResult["existencia"]).ToString("0.##") + ")",
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    // Incluye los no activos
    public string[] ObtenerProductosConceptosALL(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia" +
            " FROM (" +
            " SELECT ID, concat(nombre, ' - ', sales) as nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR sales LIKE '%" + prefixText + "%'" +
            " OR descripcion LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " OR codigo2 = '" + prefixText + "'" +
            " OR codigo3 = '" + prefixText + "'" +
            " )" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString() +
            " ) R" +
            " LEFT JOIN producto_datos D" +
            " ON D.productoID = R.ID";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString() +
                            "(" + ((decimal)objRowResult["existencia"]).ToString("0.##") + ")",
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    // Incluye los no activos
    public string[] ObtenerConceptosALL(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT ID, nombre  " +
            " FROM productos " +
            " WHERE (nombre LIKE '%" + prefixText + "%'" +
            " OR codigo = '" + prefixText + "'" +
            " ) AND tipo = 2" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString(),
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerSucursales(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT S.ID, concat(negocio, ' - ', sucursal) as nombre  " +
            " FROM sucursales S " +
            " INNER JOIN establecimientos E " +
            " ON S.establecimiento_ID = E.ID " +
            " WHERE (E.negocio LIKE '%" + prefixText + "%'" +
            " OR S.sucursal LIKE '%" + prefixText + "%')" +
            "   AND E.ID <> 0" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString(),
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerNegocios(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.ID, E.negocio as nombre  " +
            " FROM establecimientos E " +
            " WHERE E.negocio LIKE '%" + prefixText + "%'" +
            "   AND E.ID <> 0" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString(),
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public string[] ObtenerProveedores(string prefixText, int count)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.ID, E.proveedor as nombre  " +
            " FROM proveedores E " +
            " WHERE E.proveedor LIKE '%" + prefixText + "%'" +
            " ORDER BY nombre " +
            " LIMIT " + count.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        List<string> items = new List<string>();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            string strProducto = AutoCompleteExtender.CreateAutoCompleteItem(
                            objRowResult["nombre"].ToString(),
                            objRowResult["ID"].ToString());
            items.Add(strProducto);
        }

        return items.ToArray();
    }

    [WebMethod]
    public string ObtenerProdDatos(string contextKey)
    {
        // strParams[0] - Producto ID
        // strParams[1] - Usuario Ventas
        // strParams[2] - Usuario Compras
        // strParams[3] - Nombre
        // strParams[4] - Imagen Principal
        // strParams[5] - Código
        // strParams[6] - Esconder precios
        // strParams[7] - Sales
        string[] strParms = contextKey.Split('~');
        CProducto_Datos objProd_Datos = new CProducto_Datos();
        objProd_Datos.intProductoID = int.Parse(strParms[0]);
        objProd_Datos.Leer();
        StringBuilder strTemp = new StringBuilder();
        strTemp.Append("<br/><br/><br/><br/><br/><br/><br/><table style='border-collapse: collapse;overflow:hidden; word-wrap:break-word;'>" +
                       "<tr><td style='width:50px'></td><td style='width:50px'></td><td style='width:50px'></td><td style='width:50px'></td><td style='width:50px'></td><td style='width:50px'></td></tr>" +
                       "<tr style='height:130px'><td class='CellInfoB' colspan='4' valign='middle' align='center' style='height:130px'>");
        if (!string.IsNullOrEmpty(strParms[4]))
            strTemp.Append("<img src='../fotos/" + strParms[4] + "' height='100%'>");
        else
            strTemp.Append("No image");
        strTemp.Append("</td><td class='CellInfoB' colspan='4' valign='middle' align='left' style='height:130px'>" +
                       strParms[3] +
                       "<br/><br/>" +
                       strParms[7] +
                       "<br/><br/>" +
                       "Código: " + strParms[5] + "<br/>" +
                       "Existencia: " + objProd_Datos.dcmExistencia.Value.ToString("0.##") +
                       "</td></tr>");
        strTemp.Append("<tr style='height:80px'><td class='CellInfo' colspan='3' style='height:80px' valign='middle' align='left'>");

        if (strParms[6].Equals("0") && strParms[1].Equals("1"))
        {
            if ((objProd_Datos.intFacturaID.HasValue || objProd_Datos.intNotaID.HasValue) && strParms[1].Equals("1"))
            {
                if (objProd_Datos.intFacturaID.HasValue && objProd_Datos.intNotaID.HasValue)
                    if (objProd_Datos.dtFactura_fecha >= objProd_Datos.dtNota_fecha)
                        strTemp.Append("Precio: " + objProd_Datos.dcmFactura_costo.Value.ToString("c") + "<br/>");
                    else
                        strTemp.Append("Precio: " + objProd_Datos.dcmNota_costo.Value.ToString("c") + "<br/>");
                else
                    if (objProd_Datos.intFacturaID.HasValue)
                        strTemp.Append("Precio: " + objProd_Datos.dcmFactura_costo.Value.ToString("c") + "<br/>");
                    else
                        strTemp.Append("Precio: " + objProd_Datos.dcmNota_costo.Value.ToString("c") + "<br/>");
            }
            else
                strTemp.Append("Precio: $0.00<br/>");
            if(objProd_Datos.dcmVenta_promedio.HasValue)
                strTemp.Append("Precio promedio: " + objProd_Datos.dcmVenta_promedio.Value.ToString("c"));
            else
                strTemp.Append("Precio promedio: $0.00");
        }

        strTemp.Append("</td><td class='CellInfo' colspan='3' style='height:80px' valign='middle' align='left'>");

        if (strParms[6].Equals("0") && strParms[2].Equals("1"))
        {
            if (objProd_Datos.intCompraID.HasValue)
                strTemp.Append("Costo: " + objProd_Datos.dcmCompra_costo.Value.ToString("c") + "<br/>");
            else
                strTemp.Append("Costo: $0.00<br/>");
            if (objProd_Datos.dcmCompra_promedio.HasValue)
                strTemp.Append("Costo promedio: " + objProd_Datos.dcmCompra_promedio.Value.ToString("c"));
            else
                strTemp.Append("Costo promedio: $0.00");
            if(objProd_Datos.dtCompra_fecha.HasValue)
                strTemp.Append("<br/>Últ. compra: " +
                               objProd_Datos.dtCompra_fecha.Value.ToString("dd/MMM/yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper());

            string strQuery = "SELECT 1" +
                             " FROM precios P" +
                             " INNER JOIN proveedores V" +
                             " ON V.lista_precios_ID = P.lista_precios_ID" +
                             " AND P.producto_ID = " + strParms[0] +
                             " AND V.cobra_paqueteria = 1" +
                             " LIMIT 1";
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult.Tables[0].Rows.Count > 0)
                strTemp.Append("<br/>Cobra Paq: Sí");
            else
                strTemp.Append("<br/>Cobra Paq: No");
        }

        strTemp.Append("</td></tr>" +
                       "</table>");

        return strTemp.ToString();
    }
}
