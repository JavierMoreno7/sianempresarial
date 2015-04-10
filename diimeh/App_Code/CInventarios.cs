using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for CInventarios
/// </summary>
public static class CInventarios
{
    /// <summary>
    /// Obtiene la cantidad en existencia en el inventario
    /// </summary>
    /// <param name="strProdID"></param>
    /// <param name="strSubalmacenID"></param>
    /// <param name="strLote"></param>
    /// <returns></returns>
    public static decimal Obtener_Existencia(string strProdID, string strSubalmacenID, string strLote)
    {
        decimal dcmExistencia = 0;

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT existencia, existencia_unidad FROM inventario " +
                        " WHERE producto_ID = " + strProdID +
                        "   AND subalmacen_ID = " + strSubalmacenID +
                        "   AND lote = '" + strLote + "'";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        dcmExistencia = Math.Round(dcmExistencia, 2);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            dcmExistencia = Math.Round(decimal.Parse(objRowResult["existencia"].ToString()), 2);
        }

        return dcmExistencia;
    }

    /// <summary>
    /// Modifica la cantidad en el inventario, reemplaza lo que exista, si no existe lo crea
    /// </summary>
    /// <param name="strProdID"></param>
    /// <param name="strSubalmacenID"></param>
    /// <param name="strLote"></param>
    /// <param name="strFechaCaducidad"></param>
    /// <param name="dcmExistencia"></param>
    public static void Modificar_Crear(string strProdID, string strSubalmacenID,
                                       string strLote, DateTime? dtFechaCaducidad, decimal dcmExistencia,
                                       string strLog)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT ID, existencia, existencia_unidad FROM inventario " +
                        " WHERE producto_ID = " + strProdID +
                        "   AND subalmacen_ID = " + strSubalmacenID +
                        "   AND lote = '" + strLote + "'";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        dcmExistencia = Math.Round(dcmExistencia, 2);

        decimal dcmExistenciaAnt = 0;

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            dcmExistenciaAnt = (decimal)objDataResult.Tables[0].Rows[0]["existencia"];
            strQuery = "UPDATE inventario SET " +
                    " existencia = " + dcmExistencia.ToString();
            if (dtFechaCaducidad.HasValue)
                strQuery += ",fecha_caducidad = '" + dtFechaCaducidad.Value.ToString("yyyy-MM-dd") + "'";
            strQuery += " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
        }
        else
        {
            strQuery = "INSERT INTO inventario (subalmacen_ID, producto_ID, division, " +
                        " existencia, existencia_unidad, prestamo, prestamo_unidad, lote";
            if (dtFechaCaducidad.HasValue)
                strQuery += ",fecha_caducidad";
            strQuery += ") VALUES (" +
                        "'" + strSubalmacenID + "'" +
                        ", '" + strProdID + "'" +
                        ", '0'" +
                        ", '" + dcmExistencia.ToString() + "'" +
                        ", '0'" +
                        ", '0'" +
                        ", '0'" +
                        ", '" + strLote + "'";
            if (dtFechaCaducidad.HasValue)
                strQuery += ", '" + dtFechaCaducidad.Value.ToString("yyyy-MM-dd") + "'";
            strQuery += ")";
        }
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        Registrar_Log(strSubalmacenID, strProdID, dcmExistenciaAnt, dcmExistencia, strLote, dtFechaCaducidad, strLog);

        Recalcular_Existencia(strProdID);
    }

    /// <summary>
    /// Suma productos al inventario, sino hay inventario lo genera
    /// </summary>
    /// <param name="strProdID"></param>
    /// <param name="strSubalmacenID"></param>
    /// <param name="strLote"></param>
    /// <param name="dcmExistencia"></param>
    public static void Sumar(string strProdID, string strSubalmacenID,
                             string strLote, DateTime? dtFechaCaducidad, decimal dcmExistencia,
                             string strLog)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT ID, existencia, existencia_unidad FROM inventario " +
                        " WHERE producto_ID = " + strProdID +
                        "   AND subalmacen_ID = " + strSubalmacenID +
                        "   AND lote = '" + strLote + "'";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        dcmExistencia = Math.Round(dcmExistencia, 2);
        decimal dcmExistenciaAnt = 0;

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            dcmExistenciaAnt = (decimal)objDataResult.Tables[0].Rows[0]["existencia"];
            dcmExistencia = Math.Round((decimal)objDataResult.Tables[0].Rows[0]["existencia"] + dcmExistencia, 2);
            strQuery = "UPDATE inventario SET " +
                       " existencia = " + dcmExistencia.ToString() +
                       " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
        }
        else
        {
            strQuery = "INSERT INTO inventario (subalmacen_ID, producto_ID, division, " +
                        " existencia, existencia_unidad, prestamo, prestamo_unidad, lote";
            if (dtFechaCaducidad.HasValue)
                strQuery += ",fecha_caducidad";
            strQuery += ") VALUES (" +
                        "'" + strSubalmacenID + "'" +
                        ", '" + strProdID + "'" +
                        ", '0'" +
                        ", '" + dcmExistencia.ToString() + "'" +
                        ", '0'" +
                        ", '0'" +
                        ", '0'" +
                        ", '" + strLote + "'";
            if (dtFechaCaducidad.HasValue)
                strQuery += ", '" + dtFechaCaducidad.Value.ToString("yyyy-MM-dd") + "'";
            strQuery += ")";
        }

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        Registrar_Log(strSubalmacenID, strProdID, dcmExistenciaAnt, dcmExistencia, strLote, dtFechaCaducidad, strLog);

        Recalcular_Existencia(strProdID);
    }

    /// <summary>
    /// Resta productos del inventario
    /// </summary>
    /// <param name="strProdID"></param>
    /// <param name="strSubalmacenID"></param>
    /// <param name="strLote"></param>
    /// <param name="dcmExistencia"></param>
    public static void Restar(string strProdID, string strSubalmacenID,
                              string strLote, decimal dcmExistencia,
                              string strLog)
    {
        DataSet objDataResult = new DataSet();

        DateTime? dtFechaCaducidad = null;
        string strQuery = "SELECT ID, fecha_caducidad, existencia, existencia_unidad FROM inventario " +
                        " WHERE producto_ID = " + strProdID +
                        "   AND subalmacen_ID = " + strSubalmacenID +
                        "   AND lote = '" + strLote + "'";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        dcmExistencia = Math.Round(dcmExistencia, 2);
        decimal dcmExistenciaAnt = 0;

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_caducidad"))
            {
                dtFechaCaducidad = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_caducidad"];
            }

            dcmExistenciaAnt = (decimal)objDataResult.Tables[0].Rows[0]["existencia"];
            dcmExistencia = Math.Round((decimal)objDataResult.Tables[0].Rows[0]["existencia"] - dcmExistencia, 2);
            if (dcmExistencia > 0)
            {
                strQuery = "UPDATE inventario SET " +
                        " existencia = " + dcmExistencia.ToString() +
                        " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
            }
            else
            {
                dcmExistencia = 0;
                if ((decimal)objDataResult.Tables[0].Rows[0]["existencia_unidad"] == 0)
                    strQuery = "DELETE FROM inventario " +
                            " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
                else
                    strQuery = "UPDATE inventario SET " +
                        " existencia = 0" +
                        " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
            }

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }
        }
        Registrar_Log(strSubalmacenID, strProdID, dcmExistenciaAnt, dcmExistencia, strLote, dtFechaCaducidad, strLog);

        Recalcular_Existencia(strProdID);
    }

    /// <summary>
    /// Borrar productos del inventario
    /// </summary>
    /// <param name="strInvID"></param>
    public static void Borrar(string strInvID,
                              string strLog)
    {
        DataSet objDataResult = new DataSet();

        DateTime? dtFechaCaducidad = null;
        string strQuery = "SELECT fecha_caducidad, subalmacen_ID, producto_ID, existencia, existencia_unidad, lote" +
                        " FROM inventario " +
                        " WHERE ID = " + strInvID;

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        decimal dcmExistenciaAnt = 0;

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_caducidad"))
            {
                dtFechaCaducidad = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_caducidad"];
            }

            dcmExistenciaAnt = (decimal)objDataResult.Tables[0].Rows[0]["existencia"];
            strQuery = "DELETE FROM inventario " +
                      " WHERE ID = " + strInvID;
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            Registrar_Log(objDataResult.Tables[0].Rows[0]["subalmacen_ID"].ToString(), 
                          objDataResult.Tables[0].Rows[0]["producto_ID"].ToString(), 
                          dcmExistenciaAnt, 
                          0, 
                          objDataResult.Tables[0].Rows[0]["lote"].ToString(),
                          dtFechaCaducidad,
                          strLog);
        }
    }

    private static void Recalcular_Existencia(string strProdID)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT IFNULL(SUM(existencia), 0)" +
                         " FROM inventario" +
                         " WHERE producto_ID = " + strProdID;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        CProducto_Datos objProd_Datos = new CProducto_Datos();
        objProd_Datos.intProductoID = int.Parse(strProdID);
        objProd_Datos.Leer();
        objProd_Datos.dcmExistencia = (decimal)objDataResult.Tables[0].Rows[0][0];
        objProd_Datos.Guardar();
    }

    private static void Registrar_Log(string strSubalmacenID, string strProdID, decimal dcmExistenciaAnt,
                                      decimal dcmExistencia, string strLote, DateTime? dtFechaCaducidad, 
                                      string strLog)
    {
        string strQuery = "INSERT INTO inventario_hist_prod (subalmacen_ID, producto_ID, " +
                        " existencia_ant, existencia, lote, fecha_caducidad," +
                        " descripcion, fecha, usuario_ID" +
                        ") VALUES (" +
                        strSubalmacenID +
                        ", " + strProdID +
                        ", " + dcmExistenciaAnt +
                        ", " + dcmExistencia +
                        ", '" + strLote + "'" +
                        ", " + (dtFechaCaducidad.HasValue ? "'" + dtFechaCaducidad.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "null") +
                        ", '" + strLog + "'" +
                        ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                        ", " + System.Web.HttpContext.Current.Session["SIANID"].ToString() +
                        ")";
        CComunDB.CCommun.Ejecutar_SP(strQuery);
    }
}
