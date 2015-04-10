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
using System.IO;

public partial class catalogos_catproveedores : BasePage
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
            if (!CComunDB.CCommun.ValidarAcceso(1500, out swVer, out swTot))
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

            HttpCookie ckSIAN = Request.Cookies["userCng"];
            ListItem liIva0 = new ListItem("0%", "0.00");
            decimal intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva1"]) * 100, 2);
            ListItem liIva1 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva2"]) * 100, 2);
            ListItem liIva2 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            this.rdIVA.Items.Add(liIva0);
            this.rdIVA.Items.Add(liIva1);
            this.rdIVA.Items.Add(liIva2);
            this.rdIVA.Items[1].Selected = true;

            Llenar_Grid();

            Llenar_Catalogos();

            this.hdID.Value = "";
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("COMPRAS");
        this.dlListaPrecios.DataBind();
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerDatos();
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

    private DataTable ObtenerDatos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("contacto", typeof(string)));
        dt.Columns.Add(new DataColumn("telefono", typeof(string)));
        dt.Columns.Add(new DataColumn("rfc", typeof(string)));

        string strQuery = "CALL leer_proveedores_consulta(" +
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
            dr[1] = objRowResult["proveedor"].ToString();
            dr[2] = objRowResult["contacto"].ToString();
            dr[3] = objRowResult["telefono"].ToString();
            dr[4] = objRowResult["rfc"].ToString();
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
                case "2":
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
            this.txtNumero_Proveedor.Text = string.Empty;
            this.txtRazonSocial.Text = string.Empty;
            this.txtRFC.Text = string.Empty;
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
            this.txtDatos_Bancarios.Text = string.Empty;
            this.txtContacto.Text = string.Empty;
            this.txtContacto_Tel.Text = string.Empty;
            this.txtContacto_Email.Text = string.Empty;
            this.txtContacto2.Text = string.Empty;
            this.txtContacto2_Tel.Text = string.Empty;
            this.txtContacto2_Email.Text = string.Empty;
            this.txtEspecialidad.Text = string.Empty;
            this.txtDiasEntrega.Text = string.Empty;
            this.rdIVA.ClearSelection();
            this.rdIVA.SelectedIndex = 1;
            this.txtDescuento.Text = "0";
            this.txtUtilidad.Text = "0";
            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.SelectedIndex = 0;

            this.rdTipo.ClearSelection();
            this.rdTipo.SelectedIndex = 0;

            this.rdCobraPaq.ClearSelection();
            this.rdCobraPaq.SelectedIndex = 0;

            this.btnModificar.Visible = true;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * " +
                    " FROM proveedores " +
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

            this.txtNombre.Text = objRowResult["proveedor"].ToString();
            this.txtNumero_Proveedor.Text = objRowResult["numero_proveedor"].ToString();
            this.txtRazonSocial.Text = objRowResult["razonsocial"].ToString();
            this.txtRFC.Text = objRowResult["rfc"].ToString();
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
            this.txtDatos_Bancarios.Text = objRowResult["datos_bancarios"].ToString();
            this.txtContacto.Text = objRowResult["contacto"].ToString();
            this.txtContacto_Tel.Text = objRowResult["contacto_tel"].ToString();
            this.txtContacto_Email.Text = objRowResult["contacto_email"].ToString();
            this.txtContacto2.Text = objRowResult["contacto2"].ToString();
            this.txtContacto2_Tel.Text = objRowResult["contacto2_tel"].ToString();
            this.txtContacto2_Email.Text = objRowResult["contacto2_email"].ToString();
            this.txtEspecialidad.Text = objRowResult["especialidad"].ToString();

            if (!objRowResult.IsNull("tiempo_entrega"))
                this.txtDiasEntrega.Text = ((byte)objRowResult["tiempo_entrega"]).ToString("##0");
            else
                this.txtDiasEntrega.Text = string.Empty;
            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["iva"]).ToString("0.00")).Selected = true;
            this.txtDescuento.Text = objRowResult["descuento"].ToString();
            this.txtUtilidad.Text = objRowResult["utilidad"].ToString();
            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_precios_ID"].ToString()).Selected = true;
            this.rdTipo.ClearSelection();
            this.rdTipo.Items.FindByValue(((bool)objRowResult["contado"]).ToString()).Selected = true;
            this.rdCobraPaq.ClearSelection();
            this.rdCobraPaq.Items.FindByValue(((bool)objRowResult["cobra_paqueteria"]).ToString()).Selected = true;
            this.btnModificar.Visible = true;
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        this.txtRFC.Text = this.txtRFC.Text.Trim().ToUpper();

        if (!CRutinas.Validar_RFC(this.txtRFC.Text))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("RFC no tiene el formato correcto");
            return;
        }

        if (!string.IsNullOrEmpty(this.txtDiasEntrega.Text))
        {
            byte btDias;
            if (!byte.TryParse(this.txtDiasEntrega.Text, out btDias))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Días de entrega debe ser numérico y menor a 255 días");
                return;
            }
            this.txtDiasEntrega.Text = btDias.ToString();
        }

        if (this.hdID.Value.Equals("0"))
            Agregar_Proveedor();
        else
            Modificar_Proveedor();
    }

    protected void Agregar_Proveedor()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 " +
                    " FROM proveedores " +
                    " WHERE proveedor = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'";
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Proveedor ya existe");
            return;
        }

        this.txtRFC.Text = this.txtRFC.Text.Trim().ToUpper();
        decimal dcmDescuento1, dcmUtilidad;

        decimal.TryParse(this.txtDescuento.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtUtilidad.Text.Trim(), out dcmUtilidad);

        this.txtDescuento.Text = dcmDescuento1.ToString("0.##");
        this.txtUtilidad.Text = dcmUtilidad.ToString("0.##");

        strQuery = "INSERT INTO proveedores (" +
                "proveedor, razonsocial, rfc, telefono, direccionfiscal" +
                ",num_exterior, num_interior, colonia, municipio" +
                ",estado, pais, cp, ciudad, iva, descuento " +
                ", email, fax, notas, lista_precios_ID " +
                ", contacto, contado, tiempo_entrega, cobra_paqueteria" +
                ", contacto_tel, contacto_email, contacto2, contacto2_tel, contacto2_email" +
                ", numero_proveedor, datos_bancarios, utilidad" +
                ", especialidad" +
                ") VALUES (" +
                "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtRazonSocial.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtRFC.Text.Trim().ToUpper() + "'" +
                ", '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtPais.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtCP.Text.Trim() + "'" +
                ", '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.rdIVA.SelectedValue + "'" +
                ", '" + this.txtDescuento.Text.Trim() + "'" +
                ", '" + this.txtEmail.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtFax.Text.Trim() + "'" +
                ", '" + this.txtNotas.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.dlListaPrecios.SelectedValue + "'" +
                ", '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                ", '" + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") + "'" +
                ", " + (string.IsNullOrEmpty(this.txtDiasEntrega.Text) ? "null" : this.txtDiasEntrega.Text) +
                ", " + (Convert.ToBoolean(this.rdCobraPaq.SelectedValue) ? "1" : "0") +
                ", '" + this.txtContacto_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Email.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2_Email.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumero_Proveedor.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtDatos_Bancarios.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtUtilidad.Text.Trim() + "'" +
                ", '" + this.txtEspecialidad.Text.Replace("'", "''") + "'" +
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
                  " FROM proveedores " +
                  " WHERE proveedor = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'";
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Proveedor ha sido creado");

        Llenar_Grid();

        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    private void Modificar_Proveedor()
    {
        decimal dcmDescuento1, dcmUtilidad;

        decimal.TryParse(this.txtDescuento.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtUtilidad.Text.Trim(), out dcmUtilidad);

        this.txtDescuento.Text = dcmDescuento1.ToString("0.##");
        this.txtUtilidad.Text = dcmUtilidad.ToString("0.##");

        string strQuery = "UPDATE proveedores SET " +
                  "proveedor = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                  ",razonsocial = '" + this.txtRazonSocial.Text.Trim().Replace("'", "''") + "'" +
                  ",rfc = '" + this.txtRFC.Text.Trim().Replace("'", "''") + "'" +
                  ",telefono = '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                  ",direccionfiscal = '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                  ",num_exterior = '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                  ",num_interior = '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                  ",colonia = '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                  ",municipio = '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                  ",estado = '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                  ",pais = '" + this.txtPais.Text.Trim().Replace("'", "''") + "'" +
                  ",cp = '" + this.txtCP.Text.Trim() + "'" +
                  ",ciudad = '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                  ",iva = " + this.rdIVA.SelectedValue +
                  ",descuento = " + this.txtDescuento.Text.Trim() +
                  ",email = '" + this.txtEmail.Text.Trim().Replace("'", "''") + "'" +
                  ",fax = '" + this.txtFax.Text.Trim().Replace("'", "''") + "'" +
                  ",notas = '" + this.txtNotas.Text.Trim().Replace("'", "''") + "'" +
                  ",lista_precios_ID = '" + this.dlListaPrecios.SelectedValue + "'" +
                  ",contacto = '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                  ",contado = " + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") +
                  ",tiempo_entrega = " + (string.IsNullOrEmpty(this.txtDiasEntrega.Text) ? "null" : this.txtDiasEntrega.Text) +
                  ",cobra_paqueteria = " + (Convert.ToBoolean(this.rdCobraPaq.SelectedValue) ? "1" : "0") +
                  ",contacto_tel = '" + this.txtContacto_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_email = '" + this.txtContacto_Email.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2 = '" + this.txtContacto2.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2_tel = '" + this.txtContacto2_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2_email = '" + this.txtContacto2_Email.Text.Trim().Replace("'", "''") + "'" +
                  ",numero_proveedor = '" + this.txtNumero_Proveedor.Text.Trim().Replace("'", "''") + "'" +
                  ",datos_bancarios = '" + this.txtDatos_Bancarios.Text.Trim().Replace("'", "''") + "'" +
                  ",utilidad = '" + this.txtUtilidad.Text.Trim() + "'" +
                  ",especialidad = '" + this.txtEspecialidad.Text.Replace("'", "''") + "'" +
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Proveedor ha sido modificado");

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