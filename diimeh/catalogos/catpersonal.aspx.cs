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
using System.Configuration;

public partial class catalogos_catpersonal : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnAgregar.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnAgregar.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            if (!CComunDB.CCommun.ValidarAcceso(1300, out swVer, out swTot))
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

            Llenar_Catalogos();

            this.hdID.Value = "";

        }
    }

    private void Llenar_Catalogos()
    {
        this.dlRoles.DataSource = CComunDB.CCommun.ObtenerRoles(false);
        this.dlRoles.DataBind();

        StringBuilder strRoles = new StringBuilder();
        for (int i = 0; i < this.dlRoles.Items.Count; i++)
        {
            if (strRoles.Length > 0)
                strRoles.Append(", ");
            strRoles.Append(this.dlRoles.Items[i].Text);
        }
        this.hdRoles.Value = strRoles.ToString();
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

    private DataTable ObtenerDatos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("apellidos", typeof(string)));
        dt.Columns.Add(new DataColumn("telefono", typeof(string)));
        dt.Columns.Add(new DataColumn("rol", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));

        string strQuery = "CALL leer_personas_consulta(" +
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
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["referencia"].ToString();
            dr[1] = objRowResult["nombre"].ToString();
            dr[2] = objRowResult["apellidos"].ToString();
            dr[3] = objRowResult["telefono"].ToString();
            dr[4] = objRowResult["rol"].ToString();
            if ((bool)objRowResult["activo"])
                dr[5] = "Activo";
            else
                dr[5] = "No activo";
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
        if (e.CommandName.Equals("Modificar"))
        {
            int index = Convert.ToInt32(e.CommandArgument);

            // Si es el vendedor de Mostrador
            if (this.grdvLista.DataKeys[index].Value.ToString().Equals(ConfigurationManager.AppSettings["vendedorPuntoVenta"].ToString()) &&
                !Session["SIANID"].ToString().Equals("1"))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Este usuario sólo puede ser modificado por el administrador");
                return;
            }

            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos();
            if (!this.hdAT.Value.Equals("1"))
            {
                this.btnModificar.Visible = false;
            }
        }
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
            StringBuilder strCriterio = new StringBuilder();
            switch (this.dlBusqueda.SelectedValue)
            {
                case "0":
                case "1":
                    strCriterio.Append("%");
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    strCriterio.Append("%");
                    break;
                case "2":
                    if (this.dlRoles.Items.FindByText(CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim())) == null)
                        return "Criterio de búsqueda debe ser " + this.hdRoles.Value;
                    strCriterio.Append(this.dlRoles.Items.FindByText(CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim())).Value);
                    break;
                case "3":
                    if(!CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim()).Equals("Activo") &&
                       !CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim()).Equals("No activo"))
                        return "Criterio de búsqueda debe ser Activo, No activo";
                    if (CRutinas.PrimeraLetraMayuscula(this.txtCriterio.Text.Trim()).Equals("Activo"))
                        strCriterio.Append("1");
                    else
                        strCriterio.Append("0");
                    break;
            }
            ViewState["SortCampo"] = "1";
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
            this.txtApellidos.Text = string.Empty;
            this.txtTelefono.Text = string.Empty;
            this.txtCelular.Text = string.Empty;
            this.txtCalle.Text = string.Empty;
            this.txtColonia.Text = string.Empty;
            this.txtCiudad.Text = "Juarez";
            this.txtCP.Text = string.Empty;
            this.chkActivo.Checked = true;
            this.dlRoles.ClearSelection();
            this.dlRoles.SelectedIndex = 0;

            this.btnAgregar.Visible = true;
            this.btnModificar.Visible = false;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * " +
                    " FROM personas " +
                    " WHERE ID = " + this.hdID.Value;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtNombre.Text = objRowResult["nombre"].ToString();
            this.txtApellidos.Text = objRowResult["apellidos"].ToString();
            this.txtTelefono.Text = objRowResult["telefono"].ToString();
            this.txtCelular.Text = objRowResult["celular"].ToString();
            this.txtCalle.Text = objRowResult["direccion"].ToString();
            this.txtColonia.Text = objRowResult["colonia"].ToString();
            this.txtCiudad.Text = objRowResult["ciudad"].ToString();
            this.txtCP.Text = objRowResult["cp"].ToString();
            this.dlRoles.ClearSelection();
            this.dlRoles.Items.FindByValue(objRowResult["rol_ID"].ToString()).Selected = true;
            this.chkActivo.Checked = (bool)objRowResult["activo"];
            this.btnAgregar.Visible = false;
            this.btnModificar.Visible = true;
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    protected void btnAgregar_Click(object sender, ImageClickEventArgs e)
    {

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 " +
                    " FROM personas " +
                    " WHERE nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    " AND apellidos = '" + this.txtApellidos.Text.Trim().Replace("'","''") + "'" +
                    " AND rol_ID = " + this.dlRoles.SelectedValue;
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Persona ya existe con ese rol");
            return;
        }

        int intCP;
        int.TryParse(this.txtCP.Text.Trim(), out intCP);
        this.txtCP.Text = intCP.ToString();

        strQuery = "INSERT INTO personas (" +
                "nombre, apellidos, telefono, celular, direccion, colonia " +
                ",CP, ciudad, rol_ID, activo) VALUES (" +
                "'" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtApellidos.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtTelefono.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtCelular.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtCalle.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtColonia.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtCP.Text.Trim() + "'" +
                ", '" + this.txtCiudad.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.dlRoles.SelectedValue + "'" +
                ", '" + (this.chkActivo.Checked ? "1" : "0") + "'" +
                ")";

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al generar el cliente: " + ex.Message);
            return;
        }

        strQuery = "SELECT ID " +
                    " FROM personas " +
                    " WHERE nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    " AND apellidos = '" + this.txtApellidos.Text.Trim().Replace("'","''") + "'" +
                    " AND rol_ID = " + this.dlRoles.SelectedValue;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        this.hdID.Value = objRowResult["ID"].ToString();

        this.btnAgregar.Visible = false;
        this.btnModificar.Visible = true;

        ((master_MasterPage)Page.Master).MostrarMensajeError("Persona ha sido creada");

        Llenar_Grid();

        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 " +
                    " FROM personas " +
                    " WHERE nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                    " AND apellidos = '" + this.txtApellidos.Text.Trim().Replace("'","''") + "'" +
                    " AND rol_ID = " + this.dlRoles.SelectedValue +
                    " AND ID <> " + this.hdID.Value;
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Persona ya existe con ese rol");
            return;
        }

        int intCP;
        int.TryParse(this.txtCP.Text.Trim(), out intCP);
        this.txtCP.Text = intCP.ToString();

        strQuery = "UPDATE personas SET " +
                  "nombre = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'" +
                  ",apellidos = '" + this.txtApellidos.Text.Trim().Replace("'","''") + "'" +
                  ",telefono = '" + this.txtTelefono.Text.Trim().Replace("'","''") + "'" +
                  ",celular = '" + this.txtCelular.Text.Trim().Replace("'","''") + "'" +
                  ",direccion = '" + this.txtCalle.Text.Trim().Replace("'","''") + "'" +
                  ",colonia = '" + this.txtColonia.Text.Trim().Replace("'","''") + "'" +
                  ",cp = '" + this.txtCP.Text.Trim() + "'" +
                  ",ciudad = '" + this.txtCiudad.Text.Trim().Replace("'","''") + "'" +
                  ",rol_ID = '" + this.dlRoles.SelectedValue + "'" +
                  ",activo = " +(this.chkActivo.Checked ? "1" : "0") +
                  " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        Llenar_Grid();

        ((master_MasterPage)Page.Master).MostrarMensajeError("Persona ha sido modificada");

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
}