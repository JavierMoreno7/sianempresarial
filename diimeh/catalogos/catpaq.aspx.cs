using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;

public partial class catalogos_catpaq : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            if (!CComunDB.CCommun.ValidarAcceso(1510, out swVer, out swTot))
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

    private DataTable ObtenerDatos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("contacto", typeof(string)));
        dt.Columns.Add(new DataColumn("telefono", typeof(string)));

        string strQuery = "CALL leer_paqueterias_consulta(" +
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
            dr[1] = objRowResult["paqueteria"].ToString();
            dr[2] = objRowResult["contacto"].ToString();
            dr[3] = objRowResult["telefono"].ToString();
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
                    strCriterio.Append(this.txtCriterio.Text.Trim().Replace("'", "''"));
                    strCriterio.Append("%");
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
            this.txtTelefono.Text = string.Empty;
            this.txtCalle.Text = string.Empty;
            this.txtNumExt.Text = string.Empty;
            this.txtNumInt.Text = string.Empty;
            this.txtColonia.Text = string.Empty;
            this.txtLocalidad.Text = "Juarez";
            this.txtMunicipio.Text = "Juarez";
            this.txtEstado.Text = "Chihuahua";
            this.txtPais.Text = "Mexico";
            this.txtCP.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtFax.Text = string.Empty;
            this.txtNotas.Text = string.Empty;
            this.txtContacto.Text = string.Empty;
            this.txtChofer.Text = string.Empty;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * " +
                    " FROM paqueterias " +
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

            this.txtNombre.Text = objRowResult["paqueteria"].ToString();
            this.txtTelefono.Text = objRowResult["telefono"].ToString();
            this.txtCalle.Text = objRowResult["direccionfiscal"].ToString();
            this.txtNumExt.Text = objRowResult["num_exterior"].ToString();
            this.txtNumInt.Text = objRowResult["num_interior"].ToString();
            this.txtColonia.Text = objRowResult["colonia"].ToString();
            this.txtLocalidad.Text = objRowResult["ciudad"].ToString();
            this.txtMunicipio.Text = objRowResult["municipio"].ToString();
            this.txtEstado.Text = objRowResult["estado"].ToString();
            this.txtPais.Text = objRowResult["pais"].ToString();
            this.txtCP.Text = objRowResult["cp"].ToString();
            this.txtEmail.Text = objRowResult["email"].ToString();
            this.txtFax.Text = objRowResult["fax"].ToString();
            this.txtNotas.Text = objRowResult["notas"].ToString();
            this.txtContacto.Text = objRowResult["contacto"].ToString();
            this.txtChofer.Text = objRowResult["chofer"].ToString();
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
            Agregar_Paqueteria();
        else
            Modificar_Paqueteria();
    }

    private void Agregar_Paqueteria()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 " +
                    " FROM paqueterias " +
                    " WHERE paqueteria = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'";
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Paqueteria ya existe");
            return;
        }

        strQuery = "INSERT INTO paqueterias (" +
                "paqueteria, telefono, direccionfiscal" +
                ",num_exterior, num_interior, colonia, municipio" +
                ",estado, pais, cp, ciudad " +
                ", email, fax, notas " +
                ", contacto, chofer) VALUES (" +
                "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtPais.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtCP.Text.Trim() + "'" +
                ", '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtEmail.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtFax.Text.Trim() + "'" +
                ", '" + this.txtNotas.Text.Trim().Replace("'","''") + "'" +
                ", '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtChofer.Text.Trim().Replace("'", "''") + "'" +
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
                    " FROM paqueterias " +
                    " WHERE paqueteria = '" + this.txtNombre.Text.Trim().Replace("'","''") + "'";
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Paquetería ha sido creada");

        Llenar_Grid();

        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    private void Modificar_Paqueteria()
    {
        string strQuery = "UPDATE paqueterias SET " +
                  "paqueteria = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                  ",telefono = '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                  ",direccionfiscal = '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                  ",num_exterior = '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                  ",num_interior = '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                  ",colonia = '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                  ",municipio = '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                  ",estado = '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                  ",pais = '" + this.txtPais.Text.Trim().Replace("'","''") + "'" +
                  ",cp = '" + this.txtCP.Text.Trim() + "'" +
                  ",ciudad = '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                  ",email = '" + this.txtEmail.Text.Trim().Replace("'","''") + "'" +
                  ",fax = '" + this.txtFax.Text.Trim() + "'" +
                  ",notas = '" + this.txtNotas.Text.Trim().Replace("'","''") + "'" +
                  ",contacto = '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                  ",chofer = '" + this.txtChofer.Text.Trim().Replace("'", "''") + "'" +
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Paquetería ha sido modificada");

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
                if (intPagina > int.Parse(ViewState["PagTotal"].ToString()))
                    ViewState["PagActual"] = ViewState["PagTotal"];
                else
                    ViewState["PagActual"] = intPagina;
                Llenar_Grid();
            }
        }
        this.txtPag.Focus();
    }
}