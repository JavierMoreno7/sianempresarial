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

public partial class compras_compra_bon : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='CancelFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='CancelFormat2'";
        this.btnRegresar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.imgInfo.Attributes["onmouseout"] = "javascript:esconder();";
        this.txtFechaCreacion.Attributes["readonly"] = "true";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            ViewState["SortCampo"] = "4";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;

            if (!CComunDB.CCommun.ValidarAcceso(10020, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.hdAT.Value = "0";
            }

            Llenar_Catalogos();

            Llenar_Grid();

            this.hdID.Value = "";
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerCompra_BonEstatus(false);
        this.dlEstatus.DataBind();

        StringBuilder strEstatus = new StringBuilder();
        for (int i = 0; i < this.dlEstatus.Items.Count; i++)
        {
            if (strEstatus.Length > 0)
                strEstatus.Append(", ");
            strEstatus.Append(this.dlEstatus.Items[i].Text);
        }
        this.hdEstatus.Value = strEstatus.ToString();
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerBonificaciones();
        this.grdvLista.DataBind();

        this.btnFirstPage.Visible = false;
        this.btnLastPage.Visible = false;
        this.btnNextPage.Visible = false;
        this.btnPrevPage.Visible = false;
        if (grdvLista.Rows.Count == 0)
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

    private DataTable ObtenerBonificaciones()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("compra_bonID", typeof(string)));
        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_creacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("resto", typeof(string)));

        string strQuery = "CALL leer_compras_bon_consulta(" +
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
            dr[0] = objRowResult["compra_bonID"].ToString();
            dr[1] = objRowResult["proveedor"].ToString();
            dr[2] = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy HH:mm");
            if (objRowResult.IsNull("fecha_cancelacion"))
                dr[3] = string.Empty;
            else
                dr[3] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[4] = this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Text;
            dr[5] = ((decimal)objRowResult["monto"]).ToString("c");
            dr[6] = ((decimal)objRowResult["monto"]-
                     (decimal)objRowResult["monto_usado"]).ToString("c");
            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32((decimal)objRowResult2["paginas"]);

        return dt;
    }

    private DataTable ObtenerCompras()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("compraID", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("cubierto", typeof(string)));

        string strQuery = "SELECT P.compraID " +
                         ", P.monto_pago " +
                         ", C.total " +
                         ", C.fecha_creacion " +
                         " FROM compra_bon_compras B " +
                         " INNER JOIN pago_compras P" +
                         " ON B.pagoID = P.pagoID"+
                         "    AND B.compra_bonID = " + this.hdID.Value +
                         " INNER JOIN compra C" +
                         " ON C.ID = P.compraID" +
                         " ORDER BY P.compraID";
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
            dr[0] = objRowResult["compraID"].ToString();
            dr[1] = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy");
            dr[2] = ((decimal)objRowResult["total"]).ToString("c");
            dr[3] = ((decimal)objRowResult["monto_pago"]).ToString("c");
            dt.Rows.Add(dr);
        }

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
                if(intPagina > int.Parse(ViewState["PagTotal"].ToString()))
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
            long valor = 0;
            switch (this.dlBusqueda.SelectedValue)
            {
                case "0":
                    if (!long.TryParse(this.txtCriterio.Text.Trim(), out valor))
                        return "Criterio de búsqueda debe ser numérico";
                    break;
                case "2":
                case "3":
                    if (!CRutinas.FechaValida(this.txtCriterio.Text.Trim(), out dtFecha))
                        return "Criterio de búsqueda no es una fecha válida, usar formato dd/mm/yyyy";
                    break;
                case "4":
                    if (this.dlEstatus.Items.FindByText(CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim())) == null)
                        return "Criterio de búsqueda debe ser " + this.hdEstatus.Value;
                    break;
            }
            StringBuilder strCriterio = new StringBuilder();
            switch (this.dlBusqueda.SelectedValue)
            {
                case "0":
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    break;
                case "1":
                case "5":
                    strCriterio.Append("%");
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    strCriterio.Append("%");
                    break;
                case "2":
                case "3":
                    strCriterio.Append(dtFecha.ToString("yyyy-MM-dd"));
                    break;
                case "4":
                    strCriterio.Append(this.dlEstatus.Items.FindByText(CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim())).Value);
                    break;
            }
            ViewState["SortCampo"] = "4";
            ViewState["SortOrden"] = 1;
            ViewState["CriterioCampo"] = this.dlBusqueda.SelectedValue;
            ViewState["Criterio"] = strCriterio.ToString();
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

        ViewState["SortCampo"] = "4";
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
        this.lblMensaje.Text = string.Empty;
        this.txtMotivoCancelacion.Text = string.Empty;
        if (this.hdID.Value.Equals("0"))
        {
            this.lblCompra_Bon.Text = string.Empty;
            this.txtProveedor.Text = string.Empty;
            this.hdProveedorID.Value = string.Empty;
            this.txtFechaCreacion.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtBonificacion.Text = string.Empty;
            this.txtComentarios.Text = string.Empty;
            this.txtMonto.Text = "0.00";
            this.lblDisponible.Text = string.Empty;
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue("0").Selected = true;
            this.dlEstatus.Enabled = false;
            this.lblNotas.Text = string.Empty;
            this.btnModificar.Visible = true;
            this.btnCancelar.Visible = false;
            this.gvCompras.DataSource = null;
            this.gvCompras.DataBind();
            Habilitar_Campos(true);
        }
        else
        {
            this.lblCompra_Bon.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT proveedorID, proveedor, bonificacion, " +
                    " estatus, fecha_creacion, fecha_cancelacion, " +
                    " motivo_cancelacion, comentarios, monto, monto_usado" +
                    " FROM compra_bon O" +
                    " INNER JOIN proveedores S " +
                    " ON O.proveedorID = S.ID " +
                    " AND O.ID = " + this.hdID.Value;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtProveedor.Text = objRowResult["proveedor"].ToString();
            this.hdProveedorID.Value = objRowResult["proveedorID"].ToString();

            this.txtBonificacion.Text = objRowResult["bonificacion"].ToString();
            this.txtComentarios.Text = objRowResult["comentarios"].ToString();
            this.txtMonto.Text = ((decimal)objRowResult["monto"]).ToString();
            this.lblDisponible.Text = ((decimal)objRowResult["monto"] -
                                       (decimal)objRowResult["monto_usado"]).ToString("c");

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Selected = true;

            Llenar_Compras();

            if (this.gvCompras.Rows.Count == 0)
                this.btnCancelar.Visible = true;
            else
                this.btnCancelar.Visible = false;

            Habilitar_Campos(false);
            this.btnModificar.Visible = false;

            if (objRowResult["estatus"].ToString().Equals("9"))
            {
                this.lblMensaje.Text = "Devolución cancelada: " + objRowResult["motivo_cancelacion"].ToString();
                this.btnCancelar.Visible = false;
            }

            string[] strValores = Obtener_Notas(this.hdProveedorID.Value).Split('|');
            this.lblNotas.Text = strValores[0];
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    private void Habilitar_Campos(bool swHabilitado)
    {
        this.txtProveedor.Enabled = swHabilitado;
        this.txtBonificacion.Enabled = swHabilitado;
        this.btnFechaCreacion.Enabled = swHabilitado;
        this.txtMonto.Enabled = swHabilitado;
        this.txtComentarios.Enabled = swHabilitado;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        Agregar_Bonificacion();
    }

    private void Agregar_Bonificacion()
    {
        if (string.IsNullOrEmpty(this.hdProveedorID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un proveedor de la lista");
            return;
        }

        decimal dcmMonto = 0;

        if (!decimal.TryParse(this.txtMonto.Text.Trim(), out dcmMonto))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Monto debe ser numérico");
            return;
        }

        dcmMonto = Math.Round(dcmMonto, 2);

        if(dcmMonto <= 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Monto debe ser mayor a cero");
            return;
        }

        this.txtMonto.Text = dcmMonto.ToString();

        if (Crear_Bonificacion())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("La bonificación ha sido creada, folio: " + this.hdID.Value);
            this.pnlDatos.Visible = false;
            this.pnlListado.Visible = true;
            Llenar_Grid();
        }
    }

    private bool Crear_Bonificacion()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM compra_bon" +
                    " WHERE proveedorID = " + this.hdProveedorID.Value +
                    " AND fecha_creacion = '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'";
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una bonificación para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra_bon (proveedorID, bonificacion, fecha_creacion, " +
                   "monto, monto_usado, estatus," +
                   "fecha_cancelacion, motivo_cancelacion, comentarios) VALUES (" +
                   "'" + this.hdProveedorID.Value + "'" +
                   ", '" + this.txtBonificacion.Text.Trim().Replace("'", "''") + "'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '" + this.txtMonto.Text + "'" +
                   ", '0'" +
                   ", '0'" +
                   ", null" +
                   ", ''" +
                   ", '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                   ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "SELECT ID " +
                " FROM compra_bon" +
                " WHERE proveedorID = " + this.hdProveedorID.Value +
                " AND fecha_creacion = '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'";
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
            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            this.hdID.Value = objRowResult["ID"].ToString();
            this.lblCompra_Bon.Text = this.hdID.Value;
            return true;
        }
        return false;
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    private void Llenar_Compras()
    {
        this.gvCompras.DataSource = ObtenerCompras();
        this.gvCompras.DataBind();
    }

    protected void btnCancelarContinuar_Click(object sender, EventArgs e)
    {
        string strQuery = "UPDATE compra_bon SET " +
                    "estatus = 9" +
                    ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    ",motivo_cancelacion = '" + this.txtMotivoCancelacion.Text.Trim().Replace("'", "''") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("La bonificación ha sido cancelada");
        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    [System.Web.Services.WebMethod]
    public static string Obtener_Notas(string strParametros)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.notas, E.descuento, " +
                    " E.iva, E.lista_precios_ID, E.contado " +
                    " FROM proveedores E " +
                    " WHERE E.ID = " + strParametros;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        return objRowResult["notas"].ToString().Replace("\r\n", "<br />").Replace("\n", "<br />") +
               "|" + objRowResult["descuento"].ToString() +
               "|" + ((decimal)objRowResult["iva"]).ToString("0.00") +
               "|" + objRowResult["lista_precios_ID"].ToString() +
               "|" + (Convert.ToBoolean(objRowResult["contado"]) ? "1" : "0");
    }

}