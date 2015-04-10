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
using ConectDB;

namespace Operaciones
{
    /// <summary>
    /// Summary description for Seleccionar
    /// </summary>
    public class Seleccionar
    {
        public DataTable Obtener(string consulta)
        {
            ConectDB.Conectar_DB CONNECT = new ConectDB.Conectar_DB();
            MySqlCommand _comando = CONNECT.CrearComando();
            _comando.CommandText = consulta;
            return CONNECT.EjecutarComandoSelect(_comando);
        }

        public string Insertar(string insert)
        {
            ConectDB.Conectar_DB CONNECT = new ConectDB.Conectar_DB();
            MySqlCommand _comando = CONNECT.CrearComando();
            _comando.CommandText = insert;
            return CONNECT.EjecutarComandoInsert(_comando);
        }


    }
}