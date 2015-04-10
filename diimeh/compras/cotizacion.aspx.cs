using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

public partial class compras_cotizacion : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnRegresar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.txtFecha.Attributes["readonly"] = "true";
        this.txtFechaRespuesta.Attributes["readonly"] = "true";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            ViewState["SortCampo"] = "3";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;

            if (!CComunDB.CCommun.ValidarAcceso(10040, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.hdAT.Value = "0";
            }

            Llenar_Grid();
        }
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerCotizaciones();
        this.grdvLista.DataBind();

        this.btnFirstPage.Visible = false;
        this.btnLastPage.Visible = false;
        this.btnNextPage.Visible = false;
        this.btnPrevPage.Visible = false;
        if (this.grdvLista.Rows.Count == 0)
        {
            this.lblPagina.Text = "";
            this.lblPaginatot.Text = "";
            this.txtPag.Visible = false;
            if (!string.IsNullOrEmpty(ViewState["Criterio"].ToString()))
                ((master_MasterPage)Page.Master).MostrarMensajeError("No se encontraron datos con dicho criterio");
        }
        else
        {
            this.lblPagina.Text = "Página";
            this.txtPag.Text = ViewState["PagActual"].ToString();
            this.txtPag.Visible = true;
            this.lblPaginatot.Text = " de " + ViewState["PagTotal"].ToString();
            if (int.Parse(ViewState["PagActual"].ToString()) != 1)
            {
                this.btnFirstPage.Visible = true;
                this.btnPrevPage.Visible = true;
            }
            if (ViewState["PagActual"].ToString() != ViewState["PagTotal"].ToString())
            {
                this.btnLastPage.Visible = true;
                this.btnNextPage.Visible = true;
            }
        }
    }

    private DataTable ObtenerCotizaciones()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("cotizacionID", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("negocio", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("tiempo_entrega", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_respuesta", typeof(string)));
        dt.Columns.Add(new DataColumn("contacto", typeof(string)));

        string strQuery = "CALL leer_cotizacion_proveedor(" +
            ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30" +
            ")";
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
            dr[0] = objRowResult["cotizacionID"].ToString();
            dr[1] = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy");
            dr[2] = objRowResult["negocio"].ToString();
            dr[3] = objRowResult["producto"].ToString();
            dr[4] = ((decimal)objRowResult["cantidad"]).ToString("#,##0.##");
            dr[5] = objRowResult["proveedor"].ToString();
            dr[6] = objRowResult["descripcion"].ToString();
            dr[7] = ((decimal)objRowResult["precio"]).ToString("c");
            dr[8] = objRowResult["tiempo_entrega"].ToString();
            if(!objRowResult.IsNull("fecha_respuesta"))
                dr[9] = ((DateTime)objRowResult["fecha_Respuesta"]).ToString("dd/MM/yyyy");
            dr[10] = objRowResult["contacto"].ToString();
            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32((decimal)objRowResult2["paginas"]);

        return dt;
    }

    protected void grdvLista_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression.Equals(ViewState["SortCampo"].ToString()))
        {
            if (ViewState["SortOrden"].ToString().Equals("1"))
                ViewState["SortOrden"] = 2;
            else
                ViewState["SortOrden"] = 1;
        }
        else
            ViewState["SortOrden"] = 1;

        ViewState["SortCampo"] = e.SortExpression;

        Llenar_Grid();
    }

    protected void grdvLista_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Modificar")
        {
            if (this.hdAT.Value.Equals("1"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
                Mostrar_Datos();
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
        }
    }

    protected void btnFirstPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = 1;
        Llenar_Grid();
    }

    protected void btnNextPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = int.Parse(ViewState["PagActual"].ToString()) + 1;
        Llenar_Grid();
    }

    protected void btnPrevPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = int.Parse(ViewState["PagActual"].ToString()) - 1; ;
        Llenar_Grid();
    }

    protected void btnLastPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = ViewState["PagTotal"];
        Llenar_Grid();
    }

    protected void txtPag_TextChanged(object sender, EventArgs e)
    {
        int intPagina;
        if (!int.TryParse(this.txtPag.Text.Trim(), out intPagina))
            this.txtPag.Text = ViewState["PagActual"].ToString();
        else
        {
            if (intPagina < 1 || intPagina == int.Parse(ViewState["PagActual"].ToString()))
                this.txtPag.Text = ViewState["PagActual"].ToString();
            else
            {
                if (intPagina > int.Parse(ViewState["PagTotal"].ToString()))
                    ViewState["PagActual"] = ViewState["PagTotal"];
                else
                    ViewState["PagActual"] = intPagina;
                Llenar_Grid();
            }
        }
        this.txtPag.Focus();
    }

    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {
        string strMensaje = Validar_Campos();
        if (!string.IsNullOrEmpty(strMensaje))
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
    }

    private string Validar_Campos()
    {
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()))
        {
            DateTime dtFecha = DateTime.Today;
            StringBuilder strCriterio = new StringBuilder();
            switch (this.dlBusqueda.SelectedValue)
            {
                case "0":
                case "1":
                case "2":
                    strCriterio.Append("%");
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    strCriterio.Append("%");
                    break;
            }
            ViewState["SortCampo"] = "3";
            ViewState["SortOrden"] = 1;
            ViewState["CriterioCampo"] = this.dlBusqueda.SelectedValue;
            ViewState["Criterio"] = strCriterio.ToString();
            ViewState["PagActual"] = 1;
            Llenar_Grid();
            this.lblMostrar.Visible = true;
        }
        else
            return "Ingrese el criterio de búsqueda";

        return string.Empty;
    }

    protected void lblMostrar_Click(object sender, EventArgs e)
    {
        this.lblMostrar.Visible = false;
        this.txtCriterio.Text = string.Empty;

        ViewState["SortCampo"] = "3";
        ViewState["CriterioCampo"] = "0";
        ViewState["Criterio"] = "";
        ViewState["SortOrden"] = 1;
        ViewState["PagActual"] = 1;
        Llenar_Grid();
    }

    protected void lblAgregar_Click(object sender, EventArgs e)
    {
        this.hdID.Value = "0";
        Mostrar_Datos();
    }

    private void Mostrar_Datos()
    {
        if (this.hdID.Value.Equals("0"))
        {
            this.txtFecha.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtNegocio.Text = string.Empty;
            this.hdClienteID.Value = string.Empty;
            this.txtProducto.Text = string.Empty;
            this.hdProductoID.Value = string.Empty;
            this.txtCantidad.Text = string.Empty;
            this.txtProveedor.Text = string.Empty;
            this.txtContacto.Text = string.Empty;
            this.hdProveedorID.Value = string.Empty;
            this.txtDescripcion.Text = string.Empty;
            this.txtPrecio.Text = string.Empty;
            this.txtTiempoEntrega.Text = string.Empty;
            this.txtFechaRespuesta.Text = string.Empty;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT C.*, V.proveedor, E.negocio, P.nombre as producto " +
                             " FROM cotizacion_proveedor C" +
                             " INNER JOIN productos P " +
                             " ON C.producto_ID = P.ID " +
                             " INNER JOIN establecimientos E " +
                             " ON C.establecimiento_ID = E.ID " +
					         " INNER JOIN proveedores V " +
                             " ON C.proveedor_ID = V.ID " +
                             " WHERE C.ID = " + this.hdID.Value;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtFecha.Text = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy"); ;
            this.txtNegocio.Text = objRowResult["negocio"].ToString();
            this.hdClienteID.Value = objRowResult["establecimiento_ID"].ToString();
            this.txtProducto.Text = objRowResult["producto"].ToString();
            this.txtContacto.Text = objRowResult["contacto"].ToString();
            this.hdProductoID.Value = objRowResult["producto_ID"].ToString();
            this.txtCantidad.Text = ((decimal)objRowResult["cantidad"]).ToString("0.##");
            this.txtProveedor.Text = objRowResult["proveedor"].ToString();
            this.hdProveedorID.Value = objRowResult["proveedor_ID"].ToString();
            this.txtDescripcion.Text = objRowResult["descripcion"].ToString();
            this.txtPrecio.Text = objRowResult["precio"].ToString();
            this.txtTiempoEntrega.Text = objRowResult["producto_ID"].ToString();
            if(objRowResult.IsNull("fecha_respuesta"))
                this.txtFechaRespuesta.Text = string.Empty;
            else
                this.txtFechaRespuesta.Text = ((DateTime)objRowResult["fecha_respuesta"]).ToString("dd/MM/yyyy");

        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdClienteID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return;
        }

        if (string.IsNullOrEmpty(this.hdProductoID.Value))
        {
            if (!Buscar_Producto())
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un producto de la lista");
                return;
            }
        }

        if (string.IsNullOrEmpty(this.hdProveedorID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un proveedor de la lista");
            return;
        }

        int intCantidad = 0;
        decimal dcmPrecio = 0;
        if(string.IsNullOrEmpty(this.txtPrecio.Text.Trim()))
            this.txtPrecio.Text = "0";
        if (!int.TryParse(this.txtCantidad.Text.Trim(), out intCantidad))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad debe ser numérica");
            return;
        }
        if (!decimal.TryParse(this.txtPrecio.Text.Trim(), out dcmPrecio))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Precio debe ser numérico");
            return;
        }

        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Cotizacion();
        }
        else
            Modificar_Cotizacion();

        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    private void Agregar_Cotizacion()
    {
        string strQuery = "INSERT INTO cotizacion_proveedor (proveedor_ID, establecimiento_ID, " +
                         "contacto, fecha, producto_ID, cantidad, precio, descripcion, " +
                         "tiempo_entrega, fecha_respuesta" +
                         ") VALUES (" +
                         "'" + this.hdProveedorID.Value + "'" +
                         ", '" + this.hdClienteID.Value + "'" +
                         ", '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                         ", '" + DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'" +
                         ", '" + this.hdProductoID.Value + "'" +
                         ", '" + this.txtCantidad.Text.Trim() + "'" +
                         ", '" + this.txtPrecio.Text.Trim() + "'" +
                         ", '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                         ", '" + this.txtTiempoEntrega.Text.Trim().Replace("'", "''") + "'" +
                         ", " + (string.IsNullOrEmpty(this.txtFechaRespuesta.Text) ? "null" : "'" + DateTime.Parse(this.txtFechaRespuesta.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'") +
                         ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("La cotización ha sido creada");
    }

    private bool Modificar_Cotizacion()
    {
        string strQuery = "UPDATE cotizacion_proveedor SET " +
                    "proveedor_ID = " + this.hdProveedorID.Value +
                    ",establecimiento_ID = " + this.hdClienteID.Value +
                    ",contacto = '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                    ",fecha = '" + DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'" +
                    ",producto_ID = " + this.hdProductoID.Value +
                    ",cantidad = " + this.txtCantidad.Text.Trim() +
                    ",precio = " + this.txtPrecio.Text.Trim() +
                    ",descripcion = '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                    ",tiempo_entrega = '" + this.txtTiempoEntrega.Text.Trim().Replace("'", "''") + "'" +
                    ",fecha_respuesta = " + (string.IsNullOrEmpty(this.txtFechaRespuesta.Text) ? "null" : "'" + DateTime.Parse(this.txtFechaRespuesta.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'") +
                    " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }
        ((master_MasterPage)Page.Master).MostrarMensajeError("La cotización ha sido modificada");

        return true;
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
    }

    private bool Buscar_Producto()
    {
        if (!string.IsNullOrEmpty(this.txtProducto.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, CONCAT(nombre, ' - ', sales) as nombre" +
                             "    FROM productos P " +
                             "    WHERE (codigo = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo2 = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo3 = '" + this.txtProducto.Text.Trim() + "'" +
							 "       )" +
							 "       AND activo = 1" +
                             " ) R" +
                             " LEFT JOIN producto_datos D" +
                             " ON D.productoID = R.ID";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult.Tables[0].Rows.Count == 1)
            {
                this.hdProductoID.Value = objDataResult.Tables[0].Rows[0][0].ToString();
                this.txtProducto.Text = objDataResult.Tables[0].Rows[0][1].ToString() +
                                        "(" + ((decimal)objDataResult.Tables[0].Rows[0]["existencia"]).ToString("0.##") + ")";
                string strClientScript = "setTimeout('setCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

}
