using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;

namespace CComunDB
{
    /// <summary>
    /// Summary description for CCommun
    /// </summary>
    public class CCommun
    {
        private static MySqlConnection Abrir_Conexion()
        {
            string strConnection = ConfigurationManager.ConnectionStrings["SIANDB"].ConnectionString;

            MySqlConnection objConnection = new MySqlConnection(strConnection);
            try
            {
                objConnection.Open();
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return objConnection;
        }

        private static void Cerrar_Conexion(MySqlConnection objConnection)
        {
            if (objConnection.State != ConnectionState.Closed)
            {
                objConnection.Close();
            }
        }

        public static DataSet Ejecutar_SP(string str_SProc)
        {
            MySqlConnection objConnection;
            try
            {
                objConnection = Abrir_Conexion();
            }
            catch (MySqlException ex)
            {
                LogBD("BD", ex.Message, str_SProc);
                throw new ApplicationException("Error: " + ex.Message);
            }

            MySqlCommand objCommand = new MySqlCommand(str_SProc, objConnection);

            MySqlDataAdapter objAdapter = null;
            DataSet objDataSet = new DataSet();
            try
            {
                objAdapter = new MySqlDataAdapter(objCommand);
                objAdapter.Fill(objDataSet);
            }
            catch (MySqlException ex)
            {
                LogBD("BD", ex.Message, str_SProc);
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (objAdapter != null)
                    objAdapter.Dispose();
                Cerrar_Conexion(objConnection);
            }
            return objDataSet;
        }

        private static MySqlConnection Abrir_ConexionUSU()
        {
            string strConnection = ConfigurationManager.ConnectionStrings["UsuariosDB"].ConnectionString;

            MySqlConnection objConnection = new MySqlConnection(strConnection);
            try
            {
                objConnection.Open();
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return objConnection;
        }

        public static DataSet Ejecutar_SP_Usu(string str_SProc)
        {
            MySqlConnection objConnection;
            try
            {
                objConnection = Abrir_ConexionUSU();
            }
            catch (MySqlException ex)
            {
                LogBD("Usuarios", ex.Message, str_SProc);
                throw new ApplicationException("Error: " + ex.Message);
            }

            MySqlCommand objCommand = new MySqlCommand(str_SProc, objConnection);

            MySqlDataAdapter objAdapter = null;
            DataSet objDataSet = new DataSet();
            try
            {
                objAdapter = new MySqlDataAdapter(objCommand);
                objAdapter.Fill(objDataSet);
            }
            catch (MySqlException ex)
            {
                LogBD("Usuarios", ex.Message, str_SProc);
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (objAdapter != null)
                    objAdapter.Dispose();
                Cerrar_Conexion(objConnection);
            }
            return objDataSet;
        }

        public static bool ValidarAcceso(int intPantalla, out bool swVer, out bool swTot)
        {
            swVer = swTot = false;
            bool swAccesoPermitido = false;
            if (System.Web.HttpContext.Current.Session["SIANID"] != null)
            {
                if (int.Parse(System.Web.HttpContext.Current.Session["SIANID"].ToString()) != 1)
                {
                    string strQuery = "SELECT F.accion" +
                                     " FROM usuario_perfiles U" +
                                     " INNER JOIN perfil P" +
                                     " ON P.ID = U.perfil_ID" +
                                     " AND U.usuario_ID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString() +
                                     " INNER JOIN perfil_funciones F" +
                                     " ON F.perfil_ID = P.ID" +
                                     " AND F.funcion_ID = " + intPantalla.ToString() +
                                     " ORDER BY F.accion DESC";

                    DataSet objDataResult = Ejecutar_SP_Usu(strQuery);

                    if (objDataResult.Tables[0].Rows.Count > 0)
                    {
                        switch ((byte)objDataResult.Tables[0].Rows[0]["accion"])
                        {
                            case 1:
                                swVer = true;
                                break;
                            case 2:
                                swTot = true;
                                break;
                        }
                        swAccesoPermitido = true;
                    }
                }
                else
                {
                    swVer = swTot = true;
                    swAccesoPermitido = true;
                }
            }

            if (swAccesoPermitido)
                ((Page)System.Web.HttpContext.Current.Handler).Title = ObtenerTitulo(intPantalla);
            else
                ((Page)System.Web.HttpContext.Current.Handler).Title = "SIAN - Acceso no permitido";

            return swAccesoPermitido;
        }

        public static bool ValidarAccesoReportes(int intPantalla)
        {
            bool swAccesoPermitido = false;
            if (System.Web.HttpContext.Current.Session["SIANID"] != null)
            {
                if (int.Parse(System.Web.HttpContext.Current.Session["SIANID"].ToString()) != 1)
                {
                    int intPantallaFin = intPantalla + 100;

                    string strQuery = "SELECT F.accion" +
                                     " FROM usuario_perfiles U" +
                                     " INNER JOIN perfil P" +
                                     " ON P.ID = U.perfil_ID" +
                                     " AND U.usuario_ID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString() +
                                     " INNER JOIN perfil_funciones F" +
                                     " ON F.perfil_ID = P.ID" +
                                     " AND F.funcion_ID >= " + intPantalla +
                                     " AND F.funcion_ID < " + intPantallaFin +
                                     " ORDER BY F.accion DESC";

                    DataSet objDataResult = Ejecutar_SP_Usu(strQuery);

                    if (objDataResult.Tables[0].Rows.Count > 0)
                        swAccesoPermitido = true;
                }
                else
                {
                    swAccesoPermitido = true;
                }
            }

            return swAccesoPermitido;
        }

        private static string ObtenerTitulo(int intPantalla)
        {
            string strEtiqueta = string.Empty;
            if (!CacheManager.ObtenerValor("F" + intPantalla, out strEtiqueta))
            {
                string strQuery = "SELECT titulo " +
                                 " FROM funciones " +
                                 " WHERE ID = " + intPantalla;

                DataSet objDataResult = Ejecutar_SP_Usu(strQuery);
                if (objDataResult.Tables[0].Rows.Count > 0 &&
                   !string.IsNullOrEmpty(objDataResult.Tables[0].Rows[0][0].ToString()))
                    strEtiqueta = objDataResult.Tables[0].Rows[0][0].ToString();
                else
                    strEtiqueta = "SIAN";
                CacheManager.ColocarValor("F" + intPantalla, strEtiqueta);
            }
            return strEtiqueta;
        }

        public static bool Validar_Horario_Acceso()
        {
            bool swValido = false;

            DateTime dtHoy = DateTime.Now;
            TimeSpan tmHora = new TimeSpan(dtHoy.Hour, dtHoy.Minute, 0);

            string strQuery = "SELECT hora_inicio, hora_fin " +
                             " FROM usuario_perfiles_horario U" +
                             " INNER JOIN  perfil_horario_dia P" +
                             " ON P.perfil_ID = U.perfil_ID" +
                             " AND P.dia = " + (int)dtHoy.DayOfWeek +
                             " AND U.usuario_ID = " + System.Web.HttpContext.Current.Session["SIANID"].ToString() +
                             " ORDER BY hora_inicio";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP_Usu(strQuery);

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                if (tmHora >= (TimeSpan)objRowResult["hora_inicio"] &&
                   tmHora < (TimeSpan)objRowResult["hora_fin"])
                {
                    swValido = true;
                    break;
                }
            }

            return swValido;
        }

        public static DataTable ObtenerFamilias(bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("familiaID", typeof(int)));
            dt.Columns.Add(new DataColumn("familia", typeof(string)));

            string strQuery = "SELECT ID, familia " +
                " FROM familias " +
                " ORDER BY familia";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas las familias";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["familia"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerClases(string familiaID, bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("claseID", typeof(int)));
            dt.Columns.Add(new DataColumn("clase", typeof(string)));

            string strQuery = "SELECT ID, clase " +
                " FROM clases " +
                " WHERE familiaID = " + familiaID +
                " ORDER BY clase";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas las clases";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["clase"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerLineas(string claseID, bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("lineaID", typeof(int)));
            dt.Columns.Add(new DataColumn("linea", typeof(string)));

            string strQuery = "SELECT ID, linea " +
                " FROM lineas " +
                " WHERE claseID = " + claseID +
                " ORDER BY linea";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas las líneas";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["linea"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerRacks()
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("rackID", typeof(int)));
            dt.Columns.Add(new DataColumn("rack", typeof(string)));

            string strQuery = "SELECT ID, rack " +
                " FROM racks " +
                " ORDER BY rack";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["rack"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCuartos()
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("cuartoID", typeof(int)));
            dt.Columns.Add(new DataColumn("cuarto", typeof(string)));

            string strQuery = "SELECT ID, cuarto " +
                " FROM cuartos " +
                " ORDER BY cuarto";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["cuarto"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerNiveles()
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("nivelID", typeof(int)));
            dt.Columns.Add(new DataColumn("nivel", typeof(string)));

            string strQuery = "SELECT ID, nivel " +
                " FROM niveles " +
                " ORDER BY nivel";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nivel"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerRutas(bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("rutaID", typeof(int)));
            dt.Columns.Add(new DataColumn("clave_ruta", typeof(string)));

            string strQuery = "SELECT ID, clave_ruta " +
                " FROM rutas " +
                " ORDER BY clave_ruta";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas las rutas";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["clave_ruta"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerListasPrecios(string strTipoLista)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("listaprecioID", typeof(int)));
            dt.Columns.Add(new DataColumn("nombrelista", typeof(string)));
            dt.Columns.Add(new DataColumn("tipolista", typeof(string)));

            string strQuery = "SELECT ID, nombre_lista, tipo_lista " +
                " FROM listas_precios ";
            if (!string.IsNullOrEmpty(strTipoLista))
                strQuery += " WHERE tipo_lista = '" + strTipoLista + "'";
            strQuery += " ORDER BY tipo_lista, nombre_lista";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                if (string.IsNullOrEmpty(strTipoLista))
                    dr[1] = objRowResult["tipo_lista"].ToString() + " - " + objRowResult["nombre_lista"].ToString();
                else
                    dr[1] = objRowResult["nombre_lista"].ToString();
                dr[2] = objRowResult["tipo_lista"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerAlmacenes(bool swConPrincipal, bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("subalmacenID", typeof(int)));
            dt.Columns.Add(new DataColumn("nombre_subalmacen", typeof(string)));

            string strQuery = "SELECT ID, nombre_subalmacen " +
                " FROM subalmacenes ";
            if (!swConPrincipal)
                strQuery += " WHERE ID > 100 AND ID < 200  ";

            strQuery += " ORDER BY ID";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas los almacenes";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre_subalmacen"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerAlmacenesBodegas(bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("subalmacenID", typeof(int)));
            dt.Columns.Add(new DataColumn("nombre_subalmacen", typeof(string)));

            string strQuery = "SELECT ID, nombre_subalmacen " +
                " FROM subalmacenes " +
                " WHERE ID IN (100, 200)";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas los almacenes";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre_subalmacen"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerAlmacenesTodos(bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("subalmacenID", typeof(int)));
            dt.Columns.Add(new DataColumn("nombre_subalmacen", typeof(string)));

            string strQuery = "SELECT ID, nombre_subalmacen " +
                " FROM subalmacenes";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas los almacenes";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre_subalmacen"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;

        }

        public static DataTable ObtenerTransferenciaBodegasEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM transf_bodega_estatus";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerNegocios(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("negocioID", typeof(int)));
            dt.Columns.Add(new DataColumn("negocio", typeof(string)));

            string strQuery = "SELECT ID, negocio " +
                " FROM establecimientos " +
                " ORDER BY negocio";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los negocios";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["negocio"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCobranzaFrecuencia(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("frecuenciaID", typeof(int)));
            dt.Columns.Add(new DataColumn("frecuencia", typeof(string)));

            string strQuery = "SELECT ID, frecuencia " +
                " FROM cobranza_frecuencia " +
                " ORDER BY ID";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos las frecuencias";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["frecuencia"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCobranzaFrecuenciaEspecial(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("frecuenciaID", typeof(int)));
            dt.Columns.Add(new DataColumn("frecuencia", typeof(string)));

            string strQuery = "SELECT ID, frecuencia " +
                " FROM cobranza_frecuencia_especial " +
                " ORDER BY ID";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos las frecuencias";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["frecuencia"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerSucursales(int intNegocioID,
                                                    bool swNombreNegocio,
                                                    bool swOpcionTodas)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("sucursalID", typeof(int)));
            dt.Columns.Add(new DataColumn("sucursal", typeof(string)));

            string strQuery;

            if(!swNombreNegocio)
                strQuery = "SELECT ID, sucursal " +
                    " FROM sucursales " +
                    " WHERE establecimiento_ID = " + intNegocioID.ToString() +
                    " ORDER BY sucursal";
            else
                strQuery = "SELECT S.ID, concat(negocio, ' - ', sucursal) as sucursal " +
                    " FROM sucursales S " +
                    " INNER JOIN establecimientos E" +
                    " ON S.establecimiento_ID = E.ID " +
                    "  AND establecimiento_ID = " + intNegocioID.ToString() +
                    " ORDER BY sucursal";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodas)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todas los sucursales";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["sucursal"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerVendedores(bool swOpcionTodos, bool swSoloActivos, int intPersonaID)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("vendedorID", typeof(int)));
            dt.Columns.Add(new DataColumn("vendedor", typeof(string)));

            string strQuery = "SELECT ID, nombre, apellidos " +
                " FROM personas";

            if (swSoloActivos)
                strQuery += " WHERE (rol_ID = 1 AND activo = 1)" +
                                  " OR ID = " + intPersonaID;
            else
                strQuery += " WHERE rol_ID = 1 ";
            strQuery += " ORDER BY nombre, apellidos";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los vendedores";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre"].ToString() + " " +
                        objRowResult["apellidos"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCobradores(bool swOpcionTodos, bool swSoloActivos, int intPersonaID)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("cobradorID", typeof(int)));
            dt.Columns.Add(new DataColumn("cobrador", typeof(string)));

            string strQuery = "SELECT ID, nombre, apellidos " +
                " FROM personas";

            if (swSoloActivos)
                strQuery += " WHERE (rol_ID = 4 AND activo = 1)" +
                                  " OR ID = " + intPersonaID;
            else
                strQuery += " WHERE rol_ID = 4 ";
            strQuery += " ORDER BY nombre, apellidos";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los cobradores";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre"].ToString() + " " +
                        objRowResult["apellidos"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCajeros(bool swOpcionTodos, bool swSoloActivos, int intPersonaID)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("cajeroID", typeof(int)));
            dt.Columns.Add(new DataColumn("cajero", typeof(string)));

            string strQuery = "SELECT ID, nombre, apellidos " +
                 " FROM personas";

            if (swSoloActivos)
                strQuery += " WHERE (rol_ID = 5 AND activo = 1)" +
                                  " OR ID = " + intPersonaID;
            else
                strQuery += " WHERE rol_ID = 5 ";
            strQuery += " ORDER BY nombre, apellidos";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los cajeros";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = objRowResult["nombre"].ToString() + " " +
                        objRowResult["apellidos"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCotizacionEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM cotizacion_estatus";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerOrden_CompraEstatus(bool swOpcionTodos, string strAlmacen)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM orden_compra_estatus ";

            if (strAlmacen.Equals("1"))
                strQuery += " WHERE ID in (2, 3, 4, 5)";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCompra_OrdenEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM compra_orden_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCompra_DevEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM compra_dev_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCompra_BonEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM compra_bon_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerCompraEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM compra_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerFacturaEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM facturas_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerNotaEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM notas_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerNotaCargoEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM notas_cargo_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerNotaCreditoEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM notas_credito_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerRoles(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("rolID", typeof(byte)));
            dt.Columns.Add(new DataColumn("rol", typeof(string)));

            string strQuery = "SELECT ID, rol " +
                " FROM tipos_roles ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los roles";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["rol"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerMetodos_Pago(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("metodo_pagoID", typeof(byte)));
            dt.Columns.Add(new DataColumn("metodo_pago", typeof(string)));

            string strQuery = "SELECT ID, metodo_pago " +
                " FROM metodos_pago ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los metodos de pagos";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["metodo_pago"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerTipos_Pago(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("tipo_pagoID", typeof(byte)));
            dt.Columns.Add(new DataColumn("tipo_pago", typeof(string)));

            string strQuery = "SELECT ID, tipo_pago " +
                             " FROM tipos_pagos " +
                             " ORDER BY consecutivo, ID";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los tipos de pago";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["tipo_pago"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerSalidaEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM salida_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerSalidaTipos(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("tipoID", typeof(byte)));
            dt.Columns.Add(new DataColumn("tipo", typeof(string)));

            string strQuery = "SELECT ID, tipo " +
                " FROM salida_tipos";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los tipos";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["tipo"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerEntradaEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM entrada_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerEntradaTipos(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("tipoID", typeof(byte)));
            dt.Columns.Add(new DataColumn("tipo", typeof(string)));

            string strQuery = "SELECT ID, tipo " +
                " FROM entrada_tipos";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los tipos";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["tipo"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerUsuarios(bool swOpcionTodos, bool swAdmin, bool swActivos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("usuarioID", typeof(int)));
            dt.Columns.Add(new DataColumn("usuario", typeof(string)));

            StringBuilder strWHERE = new StringBuilder();
            if (swActivos)
                strWHERE.Append(" WHERE activo <> 0");

            if(!swAdmin)
                if(strWHERE.Length == 0)
                    strWHERE.Append(" WHERE ID <> 1");
                else
                    strWHERE.Append(" AND ID <> 1");


            string strQuery = "SELECT ID, usuario, persona " +
                             " FROM usuarios " + strWHERE.ToString();

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los usuarios";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToInt32(objRowResult["ID"]);
                dr[1] = "(" + objRowResult["usuario"].ToString() + ") " +
                            objRowResult["persona"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerTicketEstatus(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("estatusID", typeof(byte)));
            dt.Columns.Add(new DataColumn("estatus", typeof(string)));

            string strQuery = "SELECT ID, estatus " +
                " FROM ticket_estatus ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los estatus";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["estatus"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerTicketTipo(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("tipoID", typeof(byte)));
            dt.Columns.Add(new DataColumn("tipo", typeof(string)));

            string strQuery = "SELECT ID, tipo " +
                " FROM ticket_tipo ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los tipos";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["tipo"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerTicketOrigen(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("origenID", typeof(byte)));
            dt.Columns.Add(new DataColumn("origen", typeof(string)));

            string strQuery = "SELECT ID, origen " +
                " FROM ticket_origen ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos los origenes";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["origen"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerTicketPrioridad(bool swOpcionTodos)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("prioridadID", typeof(byte)));
            dt.Columns.Add(new DataColumn("prioridad", typeof(string)));

            string strQuery = "SELECT ID, prioridad " +
                " FROM ticket_prioridad ";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "Todos las prioridades";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = Convert.ToByte(objRowResult["ID"]);
                dr[1] = objRowResult["prioridad"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable ObtenerPaqueterias(bool swOpcionTodos, bool swPrimerBlanco)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataSet objDataResult = new DataSet();

            dt.Columns.Add(new DataColumn("paqueteriaID", typeof(string)));
            dt.Columns.Add(new DataColumn("paqueteria", typeof(string)));

            string strQuery = "SELECT ID, paqueteria" +
                            " FROM paqueterias" +
                            " ORDER BY paqueteria";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (swPrimerBlanco)
            {
                dr = dt.NewRow();
                dr[0] = string.Empty;
                dr[1] = string.Empty;
                dt.Rows.Add(dr);
            }

            if (swOpcionTodos)
            {
                dr = dt.NewRow();
                dr[0] = "0";
                dr[1] = "Todos las paqueterías";
                dt.Rows.Add(dr);
            }

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                dr = dt.NewRow();
                dr[0] = objRowResult["ID"].ToString();
                dr[1] = objRowResult["paqueteria"].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DateTime? Obtener_FechaCobranza(byte btTipo, int intFacturaID)
        {
            DateTime dtFecha_Calculo;

            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT fecha, fecha_contrarecibo" +
                             " FROM facturas_liq F" +
                             " WHERE F.ID = " + intFacturaID;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (btTipo == 1)
            {
                dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha"];
                if (dtFecha_Calculo < DateTime.Today)
                    dtFecha_Calculo = DateTime.Today;
            }
            else
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    if ((DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"] >= DateTime.Today)
                        dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"];
                    else
                        dtFecha_Calculo = DateTime.Today;
                else
                    dtFecha_Calculo = DateTime.Today;

            strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial from cobranza_dias " +
                              " WHERE tipo = " + btTipo +
                              " AND (fecha IS NULL OR " +
                              "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')" +
                              " AND establecimientoID = (" +
                              "     SELECT S.establecimiento_ID" +
                              "     FROM facturas_liq F" +
                              "     INNER JOIN sucursales S" +
                              "     ON F.sucursal_ID = S.ID" +
                              "     AND F.ID = " + intFacturaID + ")";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, dtFecha_Calculo);
        }

        public static DateTime? Obtener_FechaCobranzaNota(byte btTipo, int intNotaID)
        {
            DateTime dtFecha_Calculo;

            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT fecha, fecha_contrarecibo" +
                             " FROM notas F" +
                             " WHERE F.ID = " + intNotaID;

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (btTipo == 1)
                if (objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha"];
                else
                    dtFecha_Calculo = DateTime.Today;
            else
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    if ((DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"] >= DateTime.Today)
                        dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"];
                    else
                        dtFecha_Calculo = DateTime.Today;
                else
                    dtFecha_Calculo = DateTime.Today;

            strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial from cobranza_dias " +
                              " WHERE tipo = " + btTipo +
                              " AND (fecha IS NULL OR " +
                              "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')" +
                              " AND establecimientoID = (" +
                              "     SELECT S.establecimiento_ID" +
                              "     FROM notas F" +
                              "     INNER JOIN sucursales S" +
                              "     ON F.sucursal_ID = S.ID" +
                              "     AND F.ID = " + intNotaID + ")";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, dtFecha_Calculo);
        }

        public static DateTime? Obtener_FechaCobranza_SucursalID(byte btTipo, int intSucursalID)
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial from cobranza_dias " +
                              " WHERE tipo = " + btTipo +
                              " AND (fecha IS NULL OR " +
                              "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')" +
                              " AND establecimientoID = (" +
                              "     SELECT S.establecimiento_ID" +
                              "     FROM sucursales S" +
                              "     WHERE S.ID = " + intSucursalID + ")";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, DateTime.Today);
        }

        public static DateTime? Obtener_FechaCobranzaNotaCargo(byte btTipo, int intNotaID)
        {
            DateTime dtFecha_Calculo;

            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT fecha, fecha_contrarecibo" +
                             " FROM notas_cargo F" +
                             " WHERE F.ID = " + intNotaID;

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (btTipo == 1)
                if (objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha"];
                else
                    dtFecha_Calculo = DateTime.Today;
            else
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    if ((DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"] >= DateTime.Today)
                        dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"];
                    else
                        dtFecha_Calculo = DateTime.Today;
                else
                    dtFecha_Calculo = DateTime.Today;

            strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial from cobranza_dias " +
                              " WHERE tipo = " + btTipo +
                              " AND (fecha IS NULL OR " +
                              "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')" +
                              " AND establecimientoID = (" +
                              "     SELECT S.establecimiento_ID" +
                              "     FROM notas_cargo F" +
                              "     INNER JOIN sucursales S" +
                              "     ON F.sucursal_ID = S.ID" +
                              "     AND F.ID = " + intNotaID + ")";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, dtFecha_Calculo);
        }

        public static DateTime? Obtener_FechaCobranzaNotaCredito(byte btTipo, int intNotaID)
        {
            DateTime dtFecha_Calculo;

            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT fecha, fecha_contrarecibo" +
                             " FROM notas_credito F" +
                             " WHERE F.ID = " + intNotaID;

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (btTipo == 1)
                if (objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha"];
                else
                    dtFecha_Calculo = DateTime.Today;
            else
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    if ((DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"] >= DateTime.Today)
                        dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"];
                    else
                        dtFecha_Calculo = DateTime.Today;
                else
                    dtFecha_Calculo = DateTime.Today;

            strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial from cobranza_dias " +
                              " WHERE tipo = " + btTipo +
                              " AND (fecha IS NULL OR " +
                              "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')" +
                              " AND establecimientoID = (" +
                              "     SELECT S.establecimiento_ID" +
                              "     FROM notas_credito F" +
                              "     INNER JOIN sucursales S" +
                              "     ON F.sucursal_ID = S.ID" +
                              "     AND F.ID = " + intNotaID + ")";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, dtFecha_Calculo);
        }

        private static DateTime? Calcula_Fecha_Cobranza(DataSet objDataResult, DateTime dtFecha_Calculo)
        {
            DateTime? dtFecha = null;

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                switch ((byte)objRowResult["frecuencia"])
                {
                    case 1: //Diario
                        // Hoy es sábado, así que suma dos días para obtener el lunes
                        if (dtFecha_Calculo.DayOfWeek.Equals(DayOfWeek.Saturday))
                        {
                            if (!dtFecha.HasValue ||
                                (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(2)))
                                dtFecha = dtFecha_Calculo.AddDays(2);
                        }
                        else
                            // Hoy es viernes, así que suma dos días para obtener el lunes
                            if (dtFecha_Calculo.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                if (!dtFecha.HasValue ||
                                    (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(3)))
                                    dtFecha = dtFecha_Calculo.AddDays(3);
                            }
                            // Es Domingo - Jueves, sólo suma 1 día
                            else
                            {
                                if (!dtFecha.HasValue ||
                                    (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(1)))
                                    dtFecha = dtFecha_Calculo.AddDays(1);
                            }
                        break;
                    case 2: //Semanal
                        // El día de la semana es mayor al día de la semana actual
                        // La fecha es dentro de la misma semana, así que suma la diferencia
                        if ((byte)objRowResult["dia_semana"] > (byte)dtFecha_Calculo.DayOfWeek)
                        {
                            int intDiferencia = (byte)objRowResult["dia_semana"] - (byte)dtFecha_Calculo.DayOfWeek;
                            if (!dtFecha.HasValue ||
                                (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(intDiferencia)))
                                dtFecha = dtFecha_Calculo.AddDays(intDiferencia);
                        }
                        // Ya pasó el día en esta semana, calcula en la siguiente semana
                        else
                        {
                            int intDiferencia = 7 - (byte)dtFecha_Calculo.DayOfWeek + (byte)objRowResult["dia_semana"];
                            if (!dtFecha.HasValue ||
                                (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(intDiferencia)))
                                dtFecha = dtFecha_Calculo.AddDays(intDiferencia);
                        }
                        break;
                    case 3: //Mensual
                        // Día del mes no ha pasado este mes
                        DateTime dtTemp = new DateTime(dtFecha_Calculo.Year, dtFecha_Calculo.Month, (byte)objRowResult["dia"]);
                        if (dtTemp > dtFecha_Calculo)
                        {
                            if (!dtFecha.HasValue ||
                                (dtFecha.HasValue && dtFecha.Value > dtTemp))
                                dtFecha = dtTemp;
                        }
                        else
                        {
                            if (!dtFecha.HasValue ||
                                (dtFecha.HasValue && dtFecha.Value > dtTemp.AddMonths(1)))
                                dtFecha = dtTemp.AddMonths(1);
                        }
                        break;
                    case 4: //Especial
                        switch ((byte)objRowResult["especial"])
                        {
                            case 1:     // Primer dia x del mes
                                // Calcula el primer día de este mes
                                DateTime dtPrimerDia = new DateTime(dtFecha_Calculo.Year, dtFecha_Calculo.Month, 1);

                                // El día de la semana es en la misma semana del primer dia del mes
                                if ((byte)objRowResult["dia_semana"] >= (byte)dtPrimerDia.DayOfWeek)
                                    dtPrimerDia = dtPrimerDia.AddDays((byte)objRowResult["dia_semana"] - (byte)dtPrimerDia.DayOfWeek);
                                else
                                    dtPrimerDia = dtPrimerDia.AddDays(7 - (byte)dtPrimerDia.DayOfWeek + (byte)objRowResult["dia_semana"]);

                                // Verifica si el día ya pasó este mes, calcula el del próximo mes
                                if(dtPrimerDia <= dtFecha_Calculo)
                                {
                                    dtPrimerDia = new DateTime(dtFecha_Calculo.AddMonths(1).Year, dtFecha_Calculo.AddMonths(1).Month, 1);
                                    if ((byte)objRowResult["dia_semana"] > (byte)dtPrimerDia.DayOfWeek)
                                        dtPrimerDia = dtPrimerDia.AddDays((byte)objRowResult["dia_semana"] - (byte)dtPrimerDia.DayOfWeek);
                                    else
                                        dtPrimerDia = dtPrimerDia.AddDays(7 - (byte)dtPrimerDia.DayOfWeek + (byte)objRowResult["dia_semana"]);
                                }

                                if (!dtFecha.HasValue ||
                                    (dtFecha.HasValue && dtFecha.Value > dtPrimerDia))
                                    dtFecha = dtPrimerDia;

                                break;
                            case 2:     // Ultimo dia del mes
                                // Calcula el primer día de este mes
                                DateTime dtUltimoDia = new DateTime(dtFecha_Calculo.AddMonths(1).Year, dtFecha_Calculo.AddMonths(1).Month, 1).AddDays(-1);

                                // El día de la semana es en la misma semana del Ultimo dia del mes
                                if ((byte)objRowResult["dia_semana"] <= (byte)dtUltimoDia.DayOfWeek)
                                    dtUltimoDia = dtUltimoDia.AddDays((byte)objRowResult["dia_semana"] - (byte)dtUltimoDia.DayOfWeek);
                                else
                                    dtUltimoDia = dtUltimoDia.AddDays((byte)objRowResult["dia_semana"] - 7 - (byte)dtUltimoDia.DayOfWeek);

                                // Verifica si el día ya pasó este mes, calcula el del próximo mes
                                if (dtUltimoDia <= dtFecha_Calculo)
                                {
                                    dtUltimoDia = new DateTime(dtFecha_Calculo.AddMonths(2).Year, dtFecha_Calculo.AddMonths(2).Month, 1).AddDays(-1);
                                    if ((byte)objRowResult["dia_semana"] <= (byte)dtUltimoDia.DayOfWeek)
                                        dtUltimoDia = dtUltimoDia.AddDays((byte)objRowResult["dia_semana"] - (byte)dtUltimoDia.DayOfWeek);
                                    else
                                        dtUltimoDia = dtUltimoDia.AddDays((byte)objRowResult["dia_semana"] - 7 - (byte)dtUltimoDia.DayOfWeek);
                                }

                                if (!dtFecha.HasValue ||
                                    (dtFecha.HasValue && dtFecha.Value > dtUltimoDia))
                                    dtFecha = dtUltimoDia;

                                break;
                        }
                        break;
                    case 5: //Cada n días
                        switch ((byte)objRowResult["especial"])
                        {
                            case 6:     // Cada n días naturales
                                dtFecha_Calculo = dtFecha_Calculo.AddDays((byte)objRowResult["dia"]);
                                if(dtFecha_Calculo.DayOfWeek.Equals(DayOfWeek.Sunday))
                                    dtFecha_Calculo = dtFecha_Calculo.AddDays(1);
                                if (!dtFecha.HasValue ||
                                   (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo))
                                    dtFecha = dtFecha_Calculo;
                                break;
                            case 7:   // Calcula n días laborables (L a V)
                                byte btDia = 1;
                                int i = 1;

                                for (; btDia <= (byte)objRowResult["dia"]; i++)
                                {
                                    if (!dtFecha_Calculo.AddDays(i).DayOfWeek.Equals(DayOfWeek.Saturday) &&
                                        !dtFecha_Calculo.AddDays(i).DayOfWeek.Equals(DayOfWeek.Sunday))
                                        btDia++;
                                }

                                if (!dtFecha.HasValue ||
                                   (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(i - 1)))
                                    dtFecha = dtFecha_Calculo.AddDays(i - 1);

                                break;
                            case 8:   // Calcula n días laborables (L a S)
                                byte btDia2 = 1;
                                int i2 = 1;

                                for (; btDia2 <= (byte)objRowResult["dia"]; i2++)
                                {
                                    if (!dtFecha_Calculo.AddDays(i2).DayOfWeek.Equals(DayOfWeek.Sunday))
                                        btDia2++;
                                }

                                if (!dtFecha.HasValue ||
                                   (dtFecha.HasValue && dtFecha.Value > dtFecha_Calculo.AddDays(i2-1)))
                                    dtFecha = dtFecha_Calculo.AddDays(i2-1);

                                break;
                        }
                        break;
                    case 6: //Día calendario
                        if (!dtFecha.HasValue ||
                           (dtFecha.HasValue && dtFecha.Value > (DateTime)objRowResult["fecha"]))
                            dtFecha = (DateTime)objRowResult["fecha"];
                        break;
                }
            }
            return dtFecha;
        }

        public static DateTime? Obtener_FechaPago(byte btTipo, int intCompraID)
        {
            DateTime dtFecha_Calculo;

            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT fecha_creacion, fecha_contrarecibo" +
                             " FROM compra F" +
                             " WHERE F.ID = " + intCompraID;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (btTipo == 1)
                if (objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_creacion"];
                else
                    dtFecha_Calculo = DateTime.Today;
            else
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_contrarecibo"))
                    if ((DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"] >= DateTime.Today)
                        dtFecha_Calculo = (DateTime)objDataResult.Tables[0].Rows[0]["fecha_contrarecibo"];
                    else
                        dtFecha_Calculo = DateTime.Today;
                else
                    dtFecha_Calculo = DateTime.Today;

            strQuery = "SELECT frecuencia, dia_semana, dia, fecha, especial FROM pagos_dias " +
                      " WHERE tipo = " + btTipo +
                      " AND (fecha IS NULL OR " +
                      "      fecha >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "')";

            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            return Calcula_Fecha_Cobranza(objDataResult, dtFecha_Calculo);
        }

        public static string Validar_Limite(int establecimientoID, int sucursalID, decimal dcmTotal)
        {
            string strMensaje = string.Empty;

            DataSet objDataResult = new DataSet();
            string strQuery;

            if (sucursalID != 0)
                strQuery = "SELECT E.limite_credito, E.dias_credito" +
                          " FROM sucursales S" +
                          " INNER JOIN establecimientos E" +
                          " ON E.ID = S.establecimiento_ID" +
                          " AND S.ID = " + sucursalID;
            else
                strQuery = "SELECT E.limite_credito, E.dias_credito" +
                          " FROM establecimientos E" +
                          " WHERE ID = " + establecimientoID;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            decimal? dcmLimite = null;
            short? shDias = null;
            if (!objDataResult.Tables[0].Rows[0].IsNull("dias_credito"))
                shDias = (short)objDataResult.Tables[0].Rows[0]["dias_credito"];

            if (!objDataResult.Tables[0].Rows[0].IsNull("limite_credito"))
                dcmLimite = (decimal)objDataResult.Tables[0].Rows[0]["limite_credito"];

            //Días de crédito
            if (shDias.HasValue)
            {
                DateTime dcmDias = DateTime.Today.AddDays((double)shDias * -1);
                if (establecimientoID == 0)
                {
                    strQuery = "SELECT establecimiento_ID" +
                              " FROM sucursales" +
                              " WHERE ID = " + sucursalID;
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                    establecimientoID = (int)objDataResult.Tables[0].Rows[0][0];
                }

                strQuery = "SELECT MIN(fecha) AS fecha" +
                          " FROM (" +
                          "    SELECT MIN(fecha) AS fecha" +
                          "    FROM facturas_liq F" +
                          "    INNER JOIN sucursales S" +
                          "    ON F.sucursal_ID = S.ID" +
                          "    AND F.status IN (1, 2, 7)" +
                          "    AND S.establecimiento_ID = " + establecimientoID +
                          "    UNION ALL" +
                          "    SELECT MIN(fecha) AS fecha" +
                          "    FROM notas F" +
                          "    INNER JOIN sucursales S" +
                          "    ON F.sucursal_ID = S.ID" +
                          "    AND F.status IN (1, 2)" +
                          "    AND S.establecimiento_ID = " + establecimientoID +
                          "    UNION ALL" +
                          "    SELECT MIN(fecha) AS fecha" +
                          "    FROM notas_cargo F" +
                          "    INNER JOIN sucursales S" +
                          "    ON F.sucursal_ID = S.ID" +
                          "    AND F.status IN (1, 2, 7)" +
                          "    AND S.establecimiento_ID = " + establecimientoID +
                          " ) R";
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                if (!objDataResult.Tables[0].Rows[0].IsNull("fecha") &&
                    dcmDias > (DateTime)objDataResult.Tables[0].Rows[0]["fecha"])
                    return "Se han excedido los días de crédito del negocio: " + shDias +
                           ", la venta más antigua sin pago es del día " + ((DateTime)objDataResult.Tables[0].Rows[0]["fecha"]).ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX")).Replace(".", "").ToUpper();
            }

            // Limite de Crédito
            if (dcmLimite.HasValue)
            {
                if (dcmLimite < dcmTotal)
                    strMensaje = "Se ha excedido el límite de crédito del negocio: " + dcmLimite.Value.ToString("c");
                else
                {
                    if (establecimientoID == 0)
                    {
                        strQuery = "SELECT establecimiento_ID" +
                                  " FROM sucursales" +
                                  " WHERE ID = " + sucursalID;
                        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                        establecimientoID = (int)objDataResult.Tables[0].Rows[0][0];
                    }

                    strQuery = "SELECT SUM(total) AS total" +
                             " FROM (" +
                             "    SELECT IFNULL(SUM(F.total), 0) as total" +
                             "    FROM facturas_liq F" +
                             "    INNER JOIN sucursales S" +
                             "    ON F.sucursal_ID = S.ID" +
                             "    AND F.status IN (1, 2, 7)" +
                             "    AND S.establecimiento_ID = " + establecimientoID +
                             "    UNION ALL" +
                             "    SELECT IFNULL(SUM(F.total), 0) as total" +
                             "    FROM notas F" +
                             "    INNER JOIN sucursales S" +
                             "    ON F.sucursal_ID = S.ID" +
                             "    AND F.status IN (1, 2)" +
                             "    AND S.establecimiento_ID = " + establecimientoID +
                             "    UNION ALL" +
                             "    SELECT IFNULL(SUM(F.total), 0) as total" +
                             "    FROM notas_cargo F" +
                             "    INNER JOIN sucursales S" +
                             "    ON F.sucursal_ID = S.ID" +
                             "    AND F.status IN (1, 2, 7)" +
                             "    AND S.establecimiento_ID = " + establecimientoID +
                             " ) R";
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                    if (dcmLimite < ((decimal)objDataResult.Tables[0].Rows[0][0] + dcmTotal))
                        strMensaje = "Se ha excedido el límite de crédito del negocio: " + dcmLimite.Value.ToString("c") +
                                     ", adeudo actual: " + ((decimal)objDataResult.Tables[0].Rows[0][0]).ToString("c");
                }
            }

            return strMensaje;
        }

        public static string Validar_Ultimo_Precio_Venta(int productoID, decimal dcmCosto)
        {
            string strMensaje = string.Empty;

            DataSet objDataResult = new DataSet();
            string strQuery =  "SELECT facturaID, factura_fecha, factura_costo" +
                              " ,notaID, nota_fecha, nota_costo" +
                              " FROM producto_datos" +
                              " WHERE productoID = " + productoID;

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                if (!objDataResult.Tables[0].Rows[0].IsNull("factura_fecha") ||
                    !objDataResult.Tables[0].Rows[0].IsNull("nota_fecha"))
                {
                    if (!objDataResult.Tables[0].Rows[0].IsNull("factura_fecha") &&
                        !objDataResult.Tables[0].Rows[0].IsNull("nota_fecha"))
                    {
                        if ((DateTime)objDataResult.Tables[0].Rows[0]["factura_fecha"] >=
                            (DateTime)objDataResult.Tables[0].Rows[0]["nota_fecha"])
                        {
                            if(dcmCosto < (decimal)objDataResult.Tables[0].Rows[0]["factura_costo"])
                                strMensaje = "Precio de venta del producto es menor al último precio usado en la factura " +
                                            objDataResult.Tables[0].Rows[0]["facturaID"].ToString() + " de " +
                                            ((decimal)objDataResult.Tables[0].Rows[0]["factura_costo"]).ToString("c");
                        }
                        else
                        {
                            if (dcmCosto < (decimal)objDataResult.Tables[0].Rows[0]["nota_costo"])
                                strMensaje = "Precio de venta del producto es menor al último precio usado en la remisión " +
                                            objDataResult.Tables[0].Rows[0]["notaID"].ToString() + " de " +
                                            ((decimal)objDataResult.Tables[0].Rows[0]["nota_costo"]).ToString("c");
                        }
                    }
                    else
                        if (!objDataResult.Tables[0].Rows[0].IsNull("factura_fecha"))
                        {
                            if (dcmCosto < (decimal)objDataResult.Tables[0].Rows[0]["factura_costo"])
                                strMensaje = "Precio de venta del producto es menor al último precio usado en la factura " +
                                            objDataResult.Tables[0].Rows[0]["facturaID"].ToString() + " de " +
                                            ((decimal)objDataResult.Tables[0].Rows[0]["factura_costo"]).ToString("c");
                        }
                        else
                        {
                            if (dcmCosto < (decimal)objDataResult.Tables[0].Rows[0]["nota_costo"])
                                strMensaje = "Precio de venta del producto es menor al último precio usado en la remisión " +
                                            objDataResult.Tables[0].Rows[0]["notaID"].ToString() + " de " +
                                            ((decimal)objDataResult.Tables[0].Rows[0]["nota_costo"]).ToString("c");
                        }
                }
            }

            return strMensaje;
        }

        public static string Validar_Ultimo_Precio_Compra(int productoID, decimal dcmCosto)
        {
            string strMensaje = string.Empty;

            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT compraID, compra_fecha, compra_costo" +
                              " FROM producto_datos" +
                              " WHERE productoID = " + productoID;

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                if (!objDataResult.Tables[0].Rows[0].IsNull("compra_fecha"))
                {
                    if (dcmCosto < (decimal)objDataResult.Tables[0].Rows[0]["compra_costo"])
                        strMensaje = "Precio de venta del producto es menor al último precio de compra, compra: " +
                                     objDataResult.Tables[0].Rows[0]["compraID"].ToString();
                }
            }

            return strMensaje;
        }

        public static decimal Actualizar_Precio(string strProdID, string strListaPreciosID, decimal dcmPrecioUnitario)
        {
            decimal dcmPrecio = 0;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT piezasporcaja " +
                             " FROM productos " +
                             " WHERE ID = " + strProdID + ";" +
                             " SELECT precio_unitario" +
                             " FROM precios" +
                             " WHERE lista_precios_ID = " + strListaPreciosID +
                             " AND producto_ID = " + strProdID +
                             " AND validez = '2099-12-31'";
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            decimal dcmPiezaPorCaja = decimal.Parse(objDataResult.Tables[0].Rows[0]["piezasporcaja"].ToString());
            dcmPrecio = Math.Round(dcmPrecioUnitario * dcmPiezaPorCaja, 2);

            if (objDataResult.Tables[1].Rows.Count > 0)
            {
                if ((decimal)objDataResult.Tables[1].Rows[0]["precio_unitario"] == dcmPrecioUnitario)
                    return dcmPrecio;
            }

            strQuery = "UPDATE precios " +
                      " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                      " WHERE lista_precios_ID = " + strListaPreciosID +
                      " AND producto_ID = " + strProdID +
                      " AND validez = '2099-12-31'";
            CComunDB.CCommun.Ejecutar_SP(strQuery);

            strQuery = "INSERT INTO precios (producto_ID, lista_precios_ID, precio_caja, " +
                      "precio_unitario, validez_desde, validez, creadoPorID, creadoPorFecha) VALUES (" +
                      "'" + strProdID + "'" +
                      ", '" + strListaPreciosID + "'" +
                      ", '" + dcmPrecio.ToString() + "'" +
                      ", '" + dcmPrecioUnitario.ToString() + "'" +
                      ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                      ", '2099-12-31'" +
                      ", '" + System.Web.HttpContext.Current.Session["SIANID"].ToString() + "'" +
                      ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                      ")";
            CComunDB.CCommun.Ejecutar_SP(strQuery);

            return dcmPrecio;
        }

        public static string Obtener_Valor_CatParametros(int intParametroID)
        {
            string strValor = string.Empty;

            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT valor" +
                              " FROM cat_parametros" +
                              " WHERE ID = " + intParametroID;

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                strValor = objDataResult.Tables[0].Rows[0]["valor"].ToString();
            }

            return strValor;
        }

        public static void Ejecutar_Async_SP(string strSP, List<string> lstParams)
        {
            Thread thQuery = new Thread(() => Ejecutar_SP(strSP, lstParams));
            thQuery.Start();
        }

        private static void Ejecutar_SP(string strSP, List<string> lstParams)
        {
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("CALL " + strSP);
            if (lstParams.Count > 0)
            {
                strQuery.Append("(");
                bool swPrimerParam = true;
                foreach (string strParam in lstParams)
                {
                    if (swPrimerParam)
                        swPrimerParam = false;
                    else
                        strQuery.Append(",");
                    strQuery.Append("'" + strParam + "'");
                }
                strQuery.Append(")");
            }
            CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
        }

        private static void LogBD(string strBD, string strError, string strQuery)
        {
            StreamWriter archLog;

            try
            {
                string strDirXML = string.Empty;

                try
                {
                    strDirXML = System.Web.HttpContext.Current.Server.MapPath("../xml_facturas") + "/";
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(strDirXML))
                {
                    try
                    {
                        strDirXML = System.Web.HttpContext.Current.Server.MapPath("./xml_facturas") + "/";
                    }
                    catch
                    {
                    }
                }

                if (string.IsNullOrEmpty(strDirXML))
                    return;

                if (File.Exists(strDirXML + "/bd.log"))
                {
                    FileInfo flInfo = new FileInfo(strDirXML + "/bd.log");
                    if (flInfo.Length > 102400)  //100KB - 102400
                    {
                        File.Move(strDirXML + "/bd.log", strDirXML + "/bd_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + ".log");
                        archLog = new StreamWriter(strDirXML + "/bd.log");
                    }
                    else
                        archLog = File.AppendText(strDirXML + "/bd.log");
                }
                else
                {
                    archLog = new StreamWriter(strDirXML + "/bd.log");
                }

                archLog.WriteLine("****************************************************");
                archLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                archLog.WriteLine("BD    : " + strBD);
                archLog.WriteLine("Error : " + strError);
                archLog.WriteLine("Query : [" + strQuery + "]");

                archLog.Close();
            }
            catch
            {

            }
        }
    }
}