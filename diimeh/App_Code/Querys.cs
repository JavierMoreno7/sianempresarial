using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ConectDB
{
    /// <summary>
    /// Summary description for Conectar_DB
    /// </summary>
    public class Conectar_DB    {

        public MySqlConnection testdb;

        public MySqlCommand CrearComando()
        {
            string _cadenaConexion = ConfigurationManager.ConnectionStrings["SIANDB"].ConnectionString;

            testdb = new MySqlConnection();
            testdb.ConnectionString = _cadenaConexion;

            MySqlCommand proceso = testdb.CreateCommand();
            proceso.CommandType = CommandType.Text;
            return proceso;
        }

        public DataTable EjecutarComandoSelect(MySqlCommand proceso)
        {

            DataTable _tabla;
            _tabla = new DataTable();

            try
            {
                proceso.Connection.Open();
                MySqlDataReader _lector = proceso.ExecuteReader();
                
                _tabla.Load(_lector);
                _lector.Close();

                proceso.Connection.Close();

                return _tabla;
            }
            catch
            {
                return _tabla;
            }
        }

        public string EjecutarComandoInsert(MySqlCommand proceso)
        {
            try
            {
                proceso.Connection.Open();
                proceso.ExecuteNonQuery();
                proceso.Connection.Close();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}