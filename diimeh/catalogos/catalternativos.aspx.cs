using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public partial class catalogos_catalternativos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtDescripcion.Attributes["onKeyPress"] = "javascript:return Limite(event, this, 500)";
        this.txtDescripcion.Attributes["onpaste"] = "javascript:return prevenirPaste(this, 500)";
        this.btnAgregar.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnAgregar.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            if (!CComunDB.CCommun.ValidarAcceso(1000, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.hdAT.Value = "0";
            }

            ViewState["SortCampo"] = "0";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;
            Llenar_Grid();

            this.hdID.Value = "";

            Llenar_Catalogos();
        }
    }

    private void Llenar_Grid()
    {
        DataTable dtResultado = ObtenerDatos();

        this.btnFirstPage.Visible = false;
        this.btnLastPage.Visible = false;
        this.btnNextPage.Visible = false;
        this.btnPrevPage.Visible = false;
        if (dtResultado.Rows.Count == 0)
        {
            this.grdvLista.DataSource = null;
            this.grdvLista.DataBind();
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

            this.grdvLista.DataSource = dtResultado;
            this.grdvLista.DataBind();
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
        this.dlListaPrecios.DataBind();
    }

    private DataTable ObtenerDatos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("nombreProducto", typeof(string)));

        string strQuery = "CALL leer_alternativos_consulta(" +
            ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30)";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["alternativoID"].ToString();
            dr[1] = objRowResult["nombreAlternativo"].ToString();
            dr[2] = objRowResult["nombreProducto"].ToString();
            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32(decimal.Truncate((decimal)objRowResult2["paginas"]));


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
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos();
            if (!this.hdAT.Value.Equals("1"))
                this.btnModificar.Visible = false;
        }
    }

    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {
        string strMensaje = string.Empty;
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()))
        {
            if (string.IsNullOrEmpty(strMensaje))
            {
                StringBuilder strCriterio = new StringBuilder();
                strCriterio.Append("%");
                strCriterio.Append(this.txtCriterio.Text.Trim());
                strCriterio.Append("%");
                ViewState["SortCampo"] = "0";
                ViewState["SortOrden"] = 1;
                ViewState["CriterioCampo"] = this.dlBusqueda.SelectedValue;
                ViewState["Criterio"] = strCriterio.ToString();
                ViewState["PagActual"] = 1;
                Llenar_Grid();
                this.lblMostrar.Visible = true;
            }
        }
        else
            strMensaje = "Ingrese el criterio de búsqueda";

        if (!string.IsNullOrEmpty(strMensaje))
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
    }

    protected void lblMostrar_Click(object sender, EventArgs e)
    {
        this.lblMostrar.Visible = false;
        this.txtCriterio.Text = string.Empty;

        ViewState["SortCampo"] = "0";
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

    protected void btnCancelar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    private void Mostrar_Datos()
    {
        if (this.hdID.Value.Equals("0"))
        {
            this.txtNombre.Text = string.Empty;
            this.txtDescripcion.Text = string.Empty;
            this.txtProducto.Text = string.Empty;
            this.txtProductoSus.Text = string.Empty;
            this.hdProductoID.Value = string.Empty;
            this.hdProductoSusID.Value = string.Empty;
            this.txtRelacion.Text = string.Empty;
            this.lblPrecioProducto.Text = "$0.00";
            this.lblPrecioOriginal.Text = "$0.00";
            this.lblPrecio.Text = "$0.00";
            this.btnAgregar.Visible = true;
            this.btnModificar.Visible = false;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT A.nombre as nombreA, productoID, O.nombre as nombreO, " +
                    "producto_A_SustituirID, P.nombre as nombreP, " +
                    "A.descripcion, relacion " +
                    " FROM alternativos A " +
                    " INNER JOIN productos O " +
                    " ON A.productoID = O.ID " +
                    " INNER JOIN productos P " +
                    " ON A.producto_A_SustituirID = P.ID " +
                    " WHERE A.ID = " + this.hdID.Value;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtNombre.Text = objRowResult["nombreA"].ToString();
            this.txtDescripcion.Text = objRowResult["descripcion"].ToString();
            this.txtProducto.Text = objRowResult["nombreO"].ToString();
            this.txtProductoSus.Text = objRowResult["nombreP"].ToString();
            this.hdProductoID.Value = objRowResult["productoID"].ToString();
            this.hdProductoSusID.Value = objRowResult["producto_A_SustituirID"].ToString();
            this.txtRelacion.Text = objRowResult["relacion"].ToString();

            Llenar_Precios();

            this.btnAgregar.Visible = false;
            this.btnModificar.Visible = true;

        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    protected void btnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        string strQuery = "SELECT 1 " +
                    " FROM alternativos " +
                    " WHERE nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'";
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Nombre ya existe para otro artículo");
            return;
        }
        if (string.IsNullOrEmpty(this.hdProductoID.Value) ||
            string.IsNullOrEmpty(this.hdProductoSusID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione los artículos de la lista");
            return;
        }

        if (this.hdProductoID.Value.Equals(this.hdProductoSusID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Producto a sustituir no puede ser el mismo que el equivalente");
            return;
        }

        Guardar_Producto();
    }

    private void Guardar_Producto()
    {
        string strQuery = "INSERT INTO alternativos (nombre, descripcion, " +
                    "productoID, relacion, producto_A_SustituirID ) VALUES (" +
                    "'" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    ", '" + this.txtDescripcion.Text.Trim().Replace("'","''") + "'" +
                    ", " + this.hdProductoID.Value +
                    ", " + this.txtRelacion.Text.Trim() +
                    ", " + this.hdProductoSusID.Value +
                    ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (Exception ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(strQuery + " " + ex.Message);
        }

        ViewState["SortCampo"] = "0";
        ViewState["CriterioCampo"] = "0";
        ViewState["Criterio"] = "";
        ViewState["SortOrden"] = 1;
        ViewState["PagActual"] = 1;
        Llenar_Grid();
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        string strQuery = "SELECT 1 " +
                    " FROM alternativos " +
                    " WHERE nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    " AND ID <> " + intProdID.ToString();
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Nombre ya existe para otro artículo");
            return;
        }

        if (this.hdProductoID.Value.Equals(this.hdProductoSusID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Producto a sustituir no puede ser el mismo que el equivalente");
            return;
        }

        Modificar_Producto(intProdID);
    }

    private void Modificar_Producto(int intProdID)
    {
        string strQuery = "UPDATE alternativos SET " +
                    "nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    ",descripcion = '" + this.txtDescripcion.Text.Trim().Replace("'","''") + "'" +
                    ",productoID = " + this.hdProductoID.Value +
                    ",producto_A_SustituirID = " + this.hdProductoSusID.Value +
                    ",relacion = " + this.txtRelacion.Text.Trim() +
                    " WHERE ID = " + intProdID.ToString();
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        ViewState["SortCampo"] = "0";
        ViewState["CriterioCampo"] = "0";
        ViewState["Criterio"] = "";
        ViewState["SortOrden"] = 1;
        ViewState["PagActual"] = 1;
        Llenar_Grid();
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
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
                if(intPagina > int.Parse(ViewState["PagTotal"].ToString()))
                    ViewState["PagActual"] = ViewState["PagTotal"];
                else
                    ViewState["PagActual"] = intPagina;
                Llenar_Grid();
            }
        }
        this.txtPag.Focus();
    }

    protected void dlListaPrecios_SelectedIndexChanged(object sender, EventArgs e)
    {
        Llenar_Precios();
    }

    protected void txtRelacion_TextChanged(object sender, EventArgs e)
    {
        Llenar_Precios();
        string Clientscript = "setTimeout('setProductoSusFoco()',500);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", Clientscript, true);
    }

    public void Llenar_Precios()
    {
        string[] strPrecios = ObtenerPrecios(this.dlListaPrecios.SelectedValue + "|" +
                                            this.hdProductoSusID.Value + "|" +
                                            this.hdProductoID.Value + "|" +
                                            this.txtRelacion.Text.Trim()).Split('|');

        this.lblPrecioProducto.Text = strPrecios[0];
        this.lblPrecioOriginal.Text = strPrecios[1];
        this.lblPrecio.Text = strPrecios[2];
    }

    [System.Web.Services.WebMethod]
    public static string ObtenerPrecios(string strParametros)
    {
        string[] strParametro = strParametros.Split('|');
        decimal dcmPrecioProducto = 0;
        decimal dcmPrecioOriginal = 0;
        decimal dcmPrecio = 0;

        DataSet objDataResult = new DataSet();
        string strQuery = string.Empty;

        if (!string.IsNullOrEmpty(strParametro[1]))
        {
            strQuery = "SELECT precio_caja as precio FROM precios " +
                       "WHERE producto_ID = " + strParametro[1] +
                       " AND lista_precios_ID = " + strParametro[0] +
                       " AND validez = '2099-12-31'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                dcmPrecioProducto = Math.Round((decimal)objRowResult["precio"], 2);
            }
        }

        if (!string.IsNullOrEmpty(strParametro[2]))
        {
            strQuery = "SELECT precio_caja as precio FROM precios " +
                       "WHERE producto_ID = " + strParametro[2] +
                       " AND lista_precios_ID = " + strParametro[0] +
                       " AND validez = '2099-12-31'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                DataRow objRowResult = objDataResult.Tables[0].Rows[0];
                dcmPrecioOriginal = Math.Round((decimal)objRowResult["precio"], 2);
            }
        }

        if (!string.IsNullOrEmpty(strParametro[3]))
        {
            decimal dcmRelacion = 0;
            decimal.TryParse(strParametro[3], out dcmRelacion);
            dcmPrecio = Math.Round(dcmPrecioOriginal * dcmRelacion, 2);
        }

        return dcmPrecioProducto.ToString("c") + "|" + 
                dcmPrecioOriginal.ToString("c") + "|" + 
                dcmPrecio.ToString("c");
    }
}