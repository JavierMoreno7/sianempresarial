using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ConsultaString
{
    /// <summary>
    /// Summary description for Consultas
    /// </summary>
    public class Consultas
    {
        string query = string.Empty;
        string condicion = string.Empty;
        string datos = string.Empty;

        #region catalogos
        public string GenerarQuery(string catalogo, string campo, string criterio, ListItemCollection selvalues)
        {
            datos = string.Empty;
            query = string.Empty;
            condicion = string.Empty;
            condicion = "LIKE";
            switch (campo)
            {
                case "CONCAT(P.nombre,' ',P.apellidos)":
                    campo = "P.nombre";criterio = LimpiarString(criterio);
                    break;
                case "if(IFNULL(L.porcentaje,0)=0,#No#,#Si#)":
                    campo = "L.porcentaje";criterio = LimpiarString(criterio);
                    break;
                case "if(IFNULL(L.fijo,0)=0,#No#,#Si#)":
                    campo = "L.fijo";criterio = LimpiarString(criterio);
                    break;
                case "DATE_FORMAT(r.fecha_asignacion,'%m/%d/%Y')":
                    campo = "r.fecha_asignacion";
                    if (criterio != "")
                       criterio = "STR_TO_DATE('"+criterio.Replace("/", ",")+"','%m,%d,%Y')";
                    break;
                default :
                    criterio = LimpiarString(criterio);
                    break;
            }

            switch (catalogo)
            {
                case "Articulos":
                    datos += "P.ID as ID,";
                    break;
                case "Personal":
                    datos += "A.ID as ID,";
                    break;
                case "Rutas":
                    datos += "R.ID as ID,";
                    break;
                case "Precios":
                    datos += "L.ID as ID,";
                    break;
                case "Proveedores":
                case "Comisiones":
                case "Clientes":
                    datos += "ID as ID,";
                    break;
            }

            foreach (ListItem item in selvalues)
                datos += string.Format("{0} as {1},", item.Value, item.Text);
            datos = datos.Substring(0, datos.Length - 1);

            switch (catalogo)
            {
                case "Articulos":
                    query = "SELECT {0} FROM PRODUCTOS P LEFT JOIN FAMILIAS F ON P.FAMILIA_ID=F.ID LEFT JOIN CLASES C ON P.CLASE_ID=C.ID LEFT JOIN LINEAS L ON P.LINEA_ID=L.ID LEFT JOIN INVENTARIO I ON P.ID = I.producto_ID WHERE {1} {2} '%{3}%' ORDER BY NOMBRE";
                    break;
                case "Proveedores":
                    query = "SELECT {0} FROM PROVEEDORES WHERE {1} {2} '%{3}%' ORDER BY RAZONSOCIAL";
                    break;
                case "Personal":
                    query = "SELECT {0} FROM PERSONAS A INNER JOIN TIPOS_ROLES B ON A.ROL_ID = B.ID WHERE {1} {2} '%{3}%' ORDER BY A.APELLIDOS";
                    break;
                case "Clientes":
                    query = "SELECT {0} FROM ESTABLECIMIENTOS WHERE {1} {2} '%{3}%' ORDER BY NEGOCIO";
                    break;
                case "Comisiones":
                    query = "SELECT {0} FROM COMISIONES_CONCEPTO WHERE {1} {2} '%{3}%' ORDER BY CONCEPTO";
                    break;
                case "Rutas":
                    if ((campo == "r.fecha_asignacion") && (criterio != ""))
                    {
                    	 condicion="=";
                       query = "SELECT {0} FROM RUTAS R LEFT JOIN PERSONAS P ON R.vendedor_ID = P.ID WHERE {1} {2} {3} ORDER BY R.CLAVE_RUTA";
                    }
                    else
                       query = "SELECT {0} FROM RUTAS R LEFT JOIN PERSONAS P ON R.vendedor_ID = P.ID WHERE {1} {2} '%{3}%' ORDER BY R.CLAVE_RUTA";
                    break;
                case "Precios":
                    query = "SELECT {0} FROM LISTAS_PRECIOS L WHERE {1} {2} '%{3}%' ORDER BY NOMBRE_LISTA";
                    break;
                case "OrdenesCompra":
                    query = "SELECT {0} FROM PROCESO_ORDENCOMPRA A INNER JOIN CAT_ESTATUS_COMPRAS B ON A.POC_ESTATUSID=B.ID INNER JOIN PROVEEDORES C ON A.POC_ProveedorID=C.ID WHERE {1} {2} '%{3}%' AND B.ID IN (1,2,3,4) ORDER BY A.POC_REFERENCIA";
                    break;
                case "Compras":
                    query = "SELECT {0} FROM PROCESO_COMPRA A INNER JOIN CAT_ESTATUS_COMPRAS B ON A.COM_ESTATUSID=B.ID INNER JOIN PROVEEDORES C ON A.COM_ProveedorID=C.ID WHERE {1} {2} '%{3}%' AND B.ID IN (5,6,7) ORDER BY A.COM_REFERENCIA";
                    break;
                case "Devoluciones":
                    query = "SELECT {0} FROM PROCESO_DEVOLUCION A INNER JOIN CAT_ESTATUS_COMPRAS B on A.DEV_ESTATUSID=B.ID INNER JOIN PROVEEDORES C ON A.DEV_ProveedorID=C.ID WHERE {1} {2} '%{3}%' AND B.ID IN (8,9,10) ORDER BY A.DEV_REFERENCIA";
                    break;
            }
            query = string.Format(query, datos.Replace("#", "'"), campo, condicion, criterio);

            return query;
        }
        #endregion catalogos

        #region compras
        public string GenerarQueryProv(string campo, string criterio, int proceso)
        {
            string condicion = string.Empty;
            string campos = string.Empty;
            string buscar = string.Empty;

            criterio = LimpiarString(criterio);

            if (proceso == 0)
            {
                condicion = "LIKE";
                campos = "proveedor";
                buscar = string.Format("'%{0}%'", criterio);
                campo = "proveedor";

            }
            else
            {
                condicion = "=";
                campos = "ID,LOWER(proveedor),direccion";

                if (campo == "proveedor")
                {
                    buscar = string.Format("'{0}'", criterio);
                }
                else
                {
                    buscar = criterio;
                }
            }


            switch (campo)
            {
                case "proveedor":
                    query = string.Format("SELECT {0} FROM PROVEEDORES WHERE {1} {2} {3} ORDER BY {0}", campos, campo, condicion, buscar);
                    break;
                case "ID":
                    query = string.Format("SELECT {0} FROM PROVEEDORES WHERE {1} {2} {3} ORDER BY {0}", campos, campo, condicion, buscar);
                    break;
            }

            return query;
        }

        public string GenerarQueryArt(string campo, string criterio, int proceso)
        {
            string condicion = string.Empty;
            string campos = string.Empty;
            string buscar = string.Empty;

            criterio = LimpiarString(criterio);

            if (proceso == 0)
            {
                condicion = "LIKE";
                campos = "nombre";
                buscar = string.Format("'%{0}%'", criterio);
                campo = "nombre";
            }
            else
            {
                condicion = "=";
                campos = "ID,LOWER(nombre),clave,existencia";

                if (campo == "nombre")
                {
                    buscar = string.Format("'{0}'", criterio);
                }
                else
                {
                    buscar = criterio;
                }
            }


            switch (campo)
            {
                case "nombre":
                    query = string.Format("SELECT {0} FROM PRODUCTOS WHERE {1} {2} {3} ORDER BY {0}", campos, campo, condicion, buscar);
                    break;
                case "clave":
                    query = string.Format("SELECT {0} FROM PRODUCTOS WHERE {1} {2} {3} ORDER BY {0}", campos, campo, condicion, buscar);
                    break;
            }

            return query;
        }

        #endregion compras

        #region basicas
        public string LimpiarString(string valor)
        {
            valor = repchars(valor, 33, 47);
            valor = repchars(valor, 58, 64);
            valor = repchars(valor, 91, 96);
            valor = repchars(valor, 123, 126);
            valor = repchars(valor, 161, 191);
            valor = valor.Replace("SELECT", "");
            valor = valor.Replace("UPDATE", "");
            valor = valor.Replace("INSERT", "");
            valor = valor.Replace("DROP", "");
            valor = valor.Replace("NEW", "");
            valor = valor.Replace("DELETE", "");
            valor = valor.Trim();
            return valor;
        }

        public string repchars(string valor, int vlini, int vlfin)
        {
            char c;
            string repvar = string.Empty;
            for (int x = vlini; x <= vlfin; x++)
            {
                c = Convert.ToChar(x);
                repvar = "" + c;
                valor = valor.Replace(repvar, "");
            }
            return valor;
        }
        #endregion basicas

    }


}
