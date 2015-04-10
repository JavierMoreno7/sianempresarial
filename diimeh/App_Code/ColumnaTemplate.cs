using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace ColumnaTemplate
{
    // Create a template class to represent a dynamic template column.
    public class GridViewTemplate : ITemplate
    {
        private DataControlRowType templateType;
        private string columnName;
        private string catalogo;

        public GridViewTemplate(DataControlRowType type, string colname, string cat)
        {
            templateType = type;
            columnName = colname;
            catalogo = cat;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            // Create the content for the different row types.
            switch (templateType)
            {
                case DataControlRowType.Header:
                    // Create the controls to put in the header
                    // section and set their properties.
                    Literal lc = new Literal();
                    lc.Text = "<b>" + columnName + "</b>";

                    // Add the controls to the Controls collection
                    // of the container.
                    container.Controls.Add(lc);
                    break;
                case DataControlRowType.DataRow:
                    // Create the controls to put in a data row
                    // section and set their properties.
                    Label lbinfo = new Label();

                    // To support data binding, register the event-handling methods
                    // to perform the data binding. Each control needs its own event
                    // handler.
                    lbinfo.DataBinding += new EventHandler(this.lbinfo_DataBinding);

                    // Add the controls to the Controls collection
                    // of the container.
                    container.Controls.Add(lbinfo);
                    break;

                // Insert cases to create the content for the other 
                // row types, if desired.

                default:
                    // Insert code to handle unexpected values.
                    break;
            }
        }

        private void lbinfo_DataBinding(Object sender, EventArgs e)
        {
            // Get the Label control to bind the value. The Label control
            // is contained in the object that raised the DataBinding 
            // event (the sender parameter).
            Label l = (Label)sender;

            // Get the GridViewRow object that contains the Label control. 
            GridViewRow row = (GridViewRow)l.NamingContainer;

            // Get the field value from the GridViewRow object and 
            // assign it to the Text property of the Label control.

            string script2 = string.Empty;
            string vlpage = string.Empty;
            string script = "javascript:";
            string vpath= string.Empty;
            string c=string.Empty;

            switch (catalogo)
            {
                case "Articulos":
                    //vlpage = "../conge/productos.aspx?parentpage=catalogos&accion=modificar&ID=" + DataBinder.Eval(row.DataItem, "ID").ToString();
                    //l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Precios":
                    if (columnName == "Imagen")
                    {
                    	 c=""+Convert.ToChar(92);
                       vpath = "D:"+c+"inetpub"+c+"vhosts"+c+"siansystem.com"+c+"httpdocs"+c+"test"+c+""+c+"fotos"+c+DataBinder.Eval(row.DataItem, columnName).ToString();
                       if (File.Exists(vpath))
                          vpath=DataBinder.Eval(row.DataItem, columnName).ToString();
                       else
                          vpath="no_disponible.jpg";
                       l.Text = "<IMG align=&quot;middle&quot; src='../Fotos/" + vpath + "' height=30 width=30>";
                    }
                    else
                    {
                        script2 = "window.open(&quot;../conge/preciosLista_crear.asp?accion=2&listaID=" + DataBinder.Eval(row.DataItem, "ID").ToString() + "&quot;,&quot;Modifica_Precios&quot;,&quot;width=500, height=330&quot;); return false;";
                        l.Text = "<a href='" + script + "' target='_blank' onClick='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    }
                    break;
                case "Proveedores":
                    script2 = "../prov/proveedor_proc.asp?accion=3&provID=" + DataBinder.Eval(row.DataItem, "ID").ToString();
                    l.Text = "<a href='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Personal":
                    vlpage = "../conge/vendedores.asp?parentpage=catalogos&accion=modificar&ID=" + DataBinder.Eval(row.DataItem, "ID").ToString();
                    l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Clientes":
                    if (columnName == "Referencia")
                    {
                        script2 = "../estab/establecimiento_proc.asp?accion=3&estID=" + DataBinder.Eval(row.DataItem, "ID").ToString();
                        l.Text = "<a href='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    }
                    else
                    {
                        script2 = "window.open(&quot;../catalogos/PopWindow.aspx?ID=" + DataBinder.Eval(row.DataItem, "ID").ToString() + "&Nombre=" + DataBinder.Eval(row.DataItem, "Nombre").ToString().Replace("'", "@apos") + "&quot;,&quot;Modifica_Sucursales&quot;,&quot;width=510, height=450, scrollbars=1&quot;); return false;";
                        l.Text = "<a href='" + script + "' target='_blank' onClick='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    }
                    break;
                case "Comisiones":
                    script2 = "../conge/comisiones_crear.asp?accion=2&comisionID=" + DataBinder.Eval(row.DataItem, "ID").ToString();
                    l.Text = "<a href='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Rutas":
                    script2 = "window.open(&quot;../rutas/rutas_estab.asp?accion=3&rutID=" + DataBinder.Eval(row.DataItem, "ID").ToString() + "&quot;,&quot;Modifica_Rutas&quot;,&quot;width=750, height=300, scrollbars=1&quot;); return false;";
                    l.Text = "<a href='" + script + "' target='_blank' onClick='" + script2 + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Sucursal":
                    vlpage = "../estab/sucursal_proc.asp?accion=3&estID=0&sucID=" + DataBinder.Eval(row.DataItem, "ID_Sucursal").ToString() + "&lastid=" + DataBinder.Eval(row.DataItem, "EID").ToString() + "&Nombre=" + DataBinder.Eval(row.DataItem, "ENOM").ToString();
                    l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "OrdenesCompra":
                    vlpage = "../compras/ProcesoCompras.aspx?Proceso=OrdenDeCompra&Accion=MOD&ID=" + DataBinder.Eval(row.DataItem, "ID_Referencia").ToString();
                    l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Compras":
                    vlpage = "../compras/ProcesoCompras.aspx?Proceso=Compras&Accion=MOD&ID=" + DataBinder.Eval(row.DataItem, "ID_Referencia").ToString();
                    l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
                case "Devoluciones":
                    vlpage = "../compras/ProcesoCompras.aspx?Proceso=Devoluciones&Accion=MOD&ID=" + DataBinder.Eval(row.DataItem, "ID_Referencia").ToString();
                    l.Text = "<a href='" + vlpage + "'>" + DataBinder.Eval(row.DataItem, columnName).ToString() + "</a>";
                    break;
            }
        }

    }

}