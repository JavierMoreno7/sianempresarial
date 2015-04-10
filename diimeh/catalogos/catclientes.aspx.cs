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

public partial class catalogos_catclientes : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.btnRegresar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;

            this.hdRV.Value = "1";
            if (!CComunDB.CCommun.ValidarAcceso(1406, out swVer, out swTot))    //Resumen ventas
                this.hdRV.Value = "0";

            this.hdUsuVentas.Value = "1";
            if (!CComunDB.CCommun.ValidarAcceso(1100, out swVer, out swTot))    //Listas de precios de ventas
                this.hdUsuVentas.Value = "0";

            if (!CComunDB.CCommun.ValidarAcceso(1403, out swVer, out swTot))
                this.chkCompletos.Enabled = false;

            if (!CComunDB.CCommun.ValidarAcceso(1400, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            HttpCookie ckSIAN = Request.Cookies["userCng"];

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.btnAgregarSucursal.Visible = false;
                this.btnGuardarSucursal.Visible = false;
                this.hdAT.Value = "0";
            }

            ViewState["SortCampo"] = "0";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;

            ListItem liIva0 = new ListItem("0%", "0.00");
            decimal intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva1"]) * 100, 2);
            ListItem liIva1 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva2"]) * 100, 2);
            ListItem liIva2 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            this.rdIVA.Items.Add(liIva0);
            this.rdIVA.Items.Add(liIva1);
            this.rdIVA.Items.Add(liIva2);
            this.rdIVA.Items[1].Selected = true;

            Llenar_Catalogos();

            if (Request.QueryString["s"] != null &&
                Request.QueryString["s"].Equals("9"))
            {
                this.dlBusqueda.ClearSelection();
                this.dlBusqueda.Items.FindByValue("9").Selected = true;
                this.txtCriterio.Text = string.Empty;
                Validar_Campos();
            }
            else
                Llenar_Grid();

            this.hdID.Value = "";
        }

        string strClientScript = "setTimeout('iva11()',500);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private void Llenar_Catalogos()
    {
        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
        this.dlListaPrecios.DataBind();

        this.dlMetodoPago.DataSource = CComunDB.CCommun.ObtenerMetodos_Pago(false);
        this.dlMetodoPago.DataBind();
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
        dt.Columns.Add(new DataColumn("rfc", typeof(string)));

        string strQuery = "CALL leer_negocios_consulta(" +
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
            if(!string.IsNullOrEmpty(objRowResult["negocio"].ToString()))
                dr[1] = objRowResult["negocio"].ToString();
            else
                dr[1] = objRowResult["nombre"].ToString();
            dr[2] = objRowResult["contacto"].ToString();
            dr[3] = objRowResult["telefono"].ToString();
            dr[4] = objRowResult["rfc"].ToString();
            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32(decimal.Truncate((decimal)objRowResult2["paginas"]));
        return dt;
    }

    private DataTable ObtenerSucursales()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("sucursalID", typeof(string)));
        dt.Columns.Add(new DataColumn("sucursal", typeof(string)));
        dt.Columns.Add(new DataColumn("telefono", typeof(string)));
        dt.Columns.Add(new DataColumn("direccion", typeof(string)));

        string strQuery = "SELECT ID, sucursal, telefono, direccion" +
                        " FROM sucursales " +
                        " WHERE establecimiento_ID = " + this.hdID.Value;
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
            dr[0] = objRowResult["ID"].ToString();
            dr[1] = objRowResult["sucursal"].ToString();
            dr[2] = objRowResult["telefono"].ToString();
            dr[3] = objRowResult["direccion"].ToString();
            dt.Rows.Add(dr);
        }
        return dt;
    }

    private DataTable ObtenerProductos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("minimo", typeof(string)));
        dt.Columns.Add(new DataColumn("maximo", typeof(string)));
        dt.Columns.Add(new DataColumn("punto_reorden", typeof(string)));

        string strQuery = "SELECT S.producto_ID " +
                        ", S.minimo, S.maximo, S.punto_reorden " +
                        ", P.nombre " +
                        " FROM sucursales_productos S " +
                        " INNER JOIN productos P " +
                        " ON S.producto_ID = P.ID " +
                        " AND S.sucursal_ID = " + this.hdSucursalID.Value;
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
            dr[0] = objRowResult["producto_ID"].ToString();
            dr[1] = objRowResult["nombre"].ToString();
            dr[2] = objRowResult["minimo"].ToString();
            dr[3] = objRowResult["maximo"].ToString();
            dr[4] = objRowResult["punto_reorden"].ToString();
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
        if (e.CommandName.Equals("Modificar"))
        {
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos();
            if (!this.hdAT.Value.Equals("1"))
                this.btnModificar.Visible = false;
        }
        else
            if (e.CommandName.Equals("Sucursales"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
                this.lblNegocio.Text = this.grdvLista.Rows[index].Cells[1].Text;
                Mostrar_Sucursales();
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
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()) ||
            (string.IsNullOrEmpty(this.txtCriterio.Text.Trim()) &&
             this.dlBusqueda.SelectedValue.Equals("9")))
        {
            StringBuilder strCriterio = new StringBuilder();
            switch (this.dlBusqueda.SelectedValue)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
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
        this.pnlVentas.Visible = false;
    }

    private void Mostrar_Datos()
    {
        if (this.hdID.Value.Equals("0"))
        {
            this.lblClienteID.Text = string.Empty;
            this.txtNegocio.Text = string.Empty;
            this.txtRazonSocial.Text = string.Empty;
            this.txtRFC.Text = string.Empty;
            this.txtTelefono.Text = string.Empty;
            this.txtCalle.Text = string.Empty;
            this.txtNumExt.Text = string.Empty;
            this.txtNumInt.Text = string.Empty;
            this.txtColonia.Text = string.Empty;
            this.txtLocalidad.Text = "Juarez";
            this.txtReferencia.Text = string.Empty;
            this.txtMunicipio.Text = "Juarez";
            this.txtEstado.Text = "Chihuahua";
            this.txtPais.Text = "Mexico";
            this.txtCP.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtFacturas_Email.Text = string.Empty;
            this.txtDirEntrega1.Text = string.Empty;
            this.txtDirEntrega2.Text = string.Empty;
            this.txtDirEntrega3.Text = string.Empty;
            this.txtNotas.Text = string.Empty;
            this.txtContacto.Text = string.Empty;
            this.txtProveedor.Text = string.Empty;
            this.rdIVA.ClearSelection();
            this.rdIVA.SelectedIndex = 2;
            this.txtDescuento1.Text = "0";
            this.txtDescuento2.Text = "0";
            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.SelectedIndex = 0;
            this.txtCuenta_Bancaria.Text = string.Empty;
            this.txtBanco.Text = string.Empty;
            this.txtNombre.Text = string.Empty;
            this.txtContacto_Tel.Text = string.Empty;
            this.txtContacto2.Text = string.Empty;
            this.txtContacto2_Tel.Text = string.Empty;
            this.txtContacto2_Email.Text = string.Empty;
            this.lblFechaModificacion.Text = string.Empty;
            this.txtLimiteCR.Text = string.Empty;
            this.txtDiasCR.Text = string.Empty;

            this.txtContacto_Dr1.Text = string.Empty;
            this.txtContacto_Dr1_Tel.Text = string.Empty;
            this.txtContacto_Dr1_Cel.Text = string.Empty;
            this.txtContacto_Enf1.Text = string.Empty;
            this.txtContacto_Enf1_Tel.Text = string.Empty;
            this.txtContacto_Enf1_Cel.Text = string.Empty;
            this.txtContacto_Dr2.Text = string.Empty;
            this.txtContacto_Dr2_Tel.Text = string.Empty;
            this.txtContacto_Dr2_Cel.Text = string.Empty;
            this.txtContacto_Enf2.Text = string.Empty;
            this.txtContacto_Enf2_Tel.Text = string.Empty;
            this.txtContacto_Enf2_Cel.Text = string.Empty;
            this.chkCompletos.Checked = false;
            this.chkSeparar_Facturas.Checked = false;

            this.dlMetodoPago.ClearSelection();
            this.dlMetodoPago.SelectedIndex = 0;

            this.rdTipo.ClearSelection();
            this.rdTipo.SelectedIndex = 0;

            this.pnlVentas.Visible = false;
        }
        else
        {
            this.lblClienteID.Text = " Cliente: " + this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * " +
                    " FROM establecimientos " +
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

            this.txtNegocio.Text = objRowResult["negocio"].ToString();
            this.txtRazonSocial.Text = objRowResult["razonsocial"].ToString();
            this.txtRFC.Text = objRowResult["rfc"].ToString();
            this.txtTelefono.Text = objRowResult["telefono"].ToString();
            this.txtCalle.Text = objRowResult["direccionfiscal"].ToString();
            this.txtNumExt.Text = objRowResult["num_exterior"].ToString();
            this.txtNumInt.Text = objRowResult["num_interior"].ToString();
            this.txtColonia.Text = objRowResult["colonia"].ToString();
            this.txtLocalidad.Text = objRowResult["poblacion"].ToString();
            this.txtReferencia.Text = objRowResult["referencia"].ToString();
            this.txtMunicipio.Text = objRowResult["municipio"].ToString();
            this.txtEstado.Text = objRowResult["estado"].ToString();
            this.txtPais.Text = objRowResult["pais"].ToString();
            this.txtCP.Text = objRowResult["cp"].ToString();
            this.txtEmail.Text = objRowResult["email"].ToString();
            this.txtFacturas_Email.Text = objRowResult["facturas_email"].ToString();
            this.txtDirEntrega1.Text = objRowResult["direccion_entrega1"].ToString();
            this.txtDirEntrega2.Text = objRowResult["direccion_entrega2"].ToString();
            this.txtDirEntrega3.Text = objRowResult["direccion_entrega3"].ToString();
            this.txtNotas.Text = objRowResult["notas"].ToString();
            this.txtContacto.Text = objRowResult["contacto"].ToString();
            this.txtProveedor.Text = objRowResult["proveedor"].ToString();
            this.txtCuenta_Bancaria.Text = objRowResult["cuenta_bancaria"].ToString();
            this.txtBanco.Text = objRowResult["banco"].ToString();
            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((double)objRowResult["iva"]).ToString("0.00")).Selected = true;
            this.txtDescuento1.Text = objRowResult["descuento"].ToString();
            this.txtDescuento2.Text = objRowResult["descuento2"].ToString();
            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_precios_ID"].ToString()).Selected = true;
            this.dlMetodoPago.ClearSelection();
            this.dlMetodoPago.Items.FindByValue(objRowResult["metodo_pago"].ToString()).Selected = true;
            this.rdTipo.ClearSelection();
            this.rdTipo.Items.FindByValue(Convert.ToBoolean(objRowResult["contado"]).ToString()).Selected = true;

            this.txtNombre.Text = objRowResult["nombre"].ToString();
            this.txtContacto_Tel.Text = objRowResult["contacto_tel"].ToString();
            this.txtContacto2.Text = objRowResult["contacto2"].ToString();
            this.txtContacto2_Tel.Text = objRowResult["contacto2_tel"].ToString();
            this.txtContacto2_Email.Text = objRowResult["contacto2_email"].ToString();
            if (objRowResult.IsNull("fecha_modificacion"))
                this.lblFechaModificacion.Text = string.Empty;
            else
                this.lblFechaModificacion.Text = ((DateTime)objRowResult["fecha_modificacion"]).ToString("dd/MM/yyyy HH:mm:ss");
            if (objRowResult.IsNull("limite_credito"))
                this.txtLimiteCR.Text = string.Empty;
            else
                this.txtLimiteCR.Text = objRowResult["limite_credito"].ToString();
            if (objRowResult.IsNull("dias_credito"))
                this.txtDiasCR.Text = string.Empty;
            else
                this.txtDiasCR.Text = objRowResult["dias_credito"].ToString();
            this.txtContacto_Dr1.Text = objRowResult["contacto_dr1"].ToString();
            this.txtContacto_Dr1_Tel.Text = objRowResult["contacto_dr1_tel"].ToString();
            this.txtContacto_Dr1_Cel.Text = objRowResult["contacto_dr1_cel"].ToString();
            this.txtContacto_Enf1.Text = objRowResult["contacto_enf1"].ToString();
            this.txtContacto_Enf1_Tel.Text = objRowResult["contacto_enf1_tel"].ToString();
            this.txtContacto_Enf1_Cel.Text = objRowResult["contacto_enf1_cel"].ToString();
            this.txtContacto_Dr2.Text = objRowResult["contacto_dr2"].ToString();
            this.txtContacto_Dr2_Tel.Text = objRowResult["contacto_dr2_tel"].ToString();
            this.txtContacto_Dr2_Cel.Text = objRowResult["contacto_dr2_cel"].ToString();
            this.txtContacto_Enf2.Text = objRowResult["contacto_enf2"].ToString();
            this.txtContacto_Enf2_Tel.Text = objRowResult["contacto_enf2_tel"].ToString();
            this.txtContacto_Enf2_Cel.Text = objRowResult["contacto_enf2_cel"].ToString();
            this.chkCompletos.Checked = (bool)objRowResult["datos_completos"];
            this.chkSeparar_Facturas.Checked = (bool)objRowResult["separar_facturas"];
            if (this.hdRV.Value.Equals("1"))
                Llenar_Ventas();
            else
                this.pnlVentas.Visible = false;
        }
        this.hdLimiteCR.Value = this.txtLimiteCR.Text;
        this.hdDiasCR.Value = this.txtDiasCR.Text;
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    private void Llenar_Ventas()
    {
        StringBuilder strTemp = new StringBuilder();
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT ID, F.fecha" +
                         " FROM facturas_liq F" +
                         " INNER JOIN" +
                         " (" +
                         " SELECT MAX(fecha) as fecha" +
                         " FROM facturas_liq F" +
                         " INNER JOIN sucursales S" +
                         " ON F.sucursal_ID = S.ID" +
                         " AND S.establecimiento_ID = " + this.hdID.Value +
                         " AND F.status <> 9" +
                         " ) AS R" +
                         " ON R.fecha = F.fecha;" +
                         " SELECT ID, F.fecha" +
                         " FROM notas F" +
                         " INNER JOIN" +
                         " (" +
                         " SELECT MAX(fecha) as fecha" +
                         " FROM notas F" +
                         " INNER JOIN sucursales S" +
                         " ON F.sucursal_ID = S.ID" +
                         " AND S.establecimiento_ID = " + this.hdID.Value +
                         " AND F.status <> 9" +
                         " ) AS R" +
                         " ON R.fecha = F.fecha;";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            strTemp.Append(((DateTime)objDataResult.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy") +
                           " (Factura " + objDataResult.Tables[0].Rows[0][0].ToString() +
                           ")" );

        if (objDataResult.Tables[1].Rows.Count > 0)
        {
            if (strTemp.Length > 0)
                strTemp.Append(", ");
            strTemp.Append(((DateTime)objDataResult.Tables[1].Rows[0][1]).ToString("dd/MM/yyyy") +
                           " (Remisión " + objDataResult.Tables[1].Rows[0][0].ToString() +
                           ")");
        }

        this.lblUltimaVenta.Text = strTemp.ToString();

        strQuery = "SELECT IFNULL(SUM(total), 0) as total" +
                  " FROM" +
                  " (" +
                  "    SELECT SUM(total) as total" +
                  "    FROM facturas_liq F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status <> 9" +
                  "    AND YEAR(F.fecha) = " + DateTime.Today.Year +
                  "    UNION ALL" +
                  "    SELECT SUM(total) as total" +
                  "    FROM notas F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status <> 9" +
                  "    AND YEAR(F.fecha) = " + DateTime.Today.Year +
                  " ) AS R;" +
                  " SELECT IFNULL(SUM(total), 0) as total" +
                  " FROM" +
                  " (" +
                  "    SELECT SUM(total) as total" +
                  "    FROM facturas_liq F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status <> 9" +
                  "    AND YEAR(F.fecha) = " + DateTime.Today.AddYears(-1).Year +
                  "    UNION ALL" +
                  "    SELECT SUM(total) as total" +
                  "    FROM notas F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status <> 9" +
                  "    AND YEAR(F.fecha) = " + DateTime.Today.AddYears(-1).Year +
                  " ) AS R";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        strTemp.Length = 0;
        strTemp.Append(((decimal)objDataResult.Tables[1].Rows[0][0]).ToString("c") +
                       " (" + DateTime.Today.AddYears(-1).Year + ")");
        strTemp.Append(", " + ((decimal)objDataResult.Tables[0].Rows[0][0]).ToString("c") +
                       " (" + DateTime.Today.Year + ")");

        this.lblVentas1.Text = strTemp.ToString();

        strQuery = "SELECT IFNULL(SUM(total), 0) as total" +
                  " FROM" +
                  " (" +
                  "    SELECT SUM(F.total) as total" +
                  "    FROM facturas_liq F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status in (1, 2, 7)" +
                  "    UNION ALL" +
                  "    SELECT SUM(total) as total" +
                  "    FROM notas F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status in (1, 2, 3)" +
                  " ) AS R;" +
                  " SELECT IFNULL(SUM(monto_pago), 0) as monto_pago" +
                  " FROM" +
                  " (" +
                  "    SELECT SUM(P.monto_pago) as monto_pago" +
                  "    FROM facturas_liq F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status in (1, 2, 7)" +
                  "    LEFT JOIN pago_facturas P" +
                  "    ON P.facturaID = F.ID" +
                  "    UNION ALL" +
                  "    SELECT SUM(P.monto_pago) as monto_pago" +
                  "    FROM notas F" +
                  "    INNER JOIN sucursales S" +
                  "    ON F.sucursal_ID = S.ID" +
                  "    AND S.establecimiento_ID = " + this.hdID.Value +
                  "    AND F.status in (1, 2, 3)" +
                  "    LEFT JOIN pago_notas P" +
                  "    ON P.notaID = F.ID" +
                  " ) AS R";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        decimal dcmSaldo = (decimal)objDataResult.Tables[0].Rows[0][0] -
                           (decimal)objDataResult.Tables[1].Rows[0][0];

        this.lblSaldo.Text = dcmSaldo.ToString("c");

        this.pnlVentas.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (!Validar_Datos())
            return;

        if (this.hdID.Value.Equals("0"))
            Agregar_Cliente();
        else
        {
            decimal dcmNuevoLimite, dcmAntLimite;
            short shNuevoDias, shAntDias;
            decimal.TryParse(this.txtLimiteCR.Text, out dcmNuevoLimite);
            decimal.TryParse(this.hdLimiteCR.Value, out dcmAntLimite);
            short.TryParse(this.txtDiasCR.Text, out shNuevoDias);
            short.TryParse(this.hdDiasCR.Value, out shAntDias);
            if (dcmNuevoLimite != dcmAntLimite || shNuevoDias != shAntDias)
            {
                this.mdLimiteCR.Show();
                return;
            }
            Modificar_Cliente();
        }
    }

    protected void btnLimiteCRContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if (DateTime.Parse(strValores[1]) >= DateTime.Now)
                strCodigo = strValores[0];
        }

        if (string.IsNullOrEmpty(this.txtCodigoLimiteCR.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigoLimiteCR.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
        }

        StringBuilder strTexto = new StringBuilder();
        try
        {
            StringWriter swTexto = new StringWriter(strTexto);
            HtmlTextWriter twTexto = new HtmlTextWriter(swTexto);
            this.pnlLimiteCR.RenderControl(twTexto);
        }
        catch
        {
            strTexto = new StringBuilder();
        }

        CRutinas.Enviar_Correo("6",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Cliente: " + this.txtNegocio.Text + "<br />" +
                               "Razón: Límite de crédito modificado" + "<br /><br />"+
                               strTexto.ToString().Replace("display:none;",""));

        Modificar_Cliente();
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

    private bool Validar_Datos()
    {
        this.txtRFC.Text = this.txtRFC.Text.Trim().ToUpper();

        if (!string.IsNullOrEmpty(this.txtRFC.Text) &&
            !CRutinas.Validar_RFC(this.txtRFC.Text))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("RFC no tiene el formato correcto");
            return false;
        }

        if (!string.IsNullOrEmpty(this.txtEmail.Text) &&
            !CRutinas.Validar_Email(this.txtEmail.Text))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Email del contacto de compras no es válido");
            return false;
        }

        if (!string.IsNullOrEmpty(this.txtContacto2_Email.Text) &&
            !CRutinas.Validar_Email(this.txtContacto2_Email.Text))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Email del contacto de finanzas no es válido");
            return false;
        }

        if (!string.IsNullOrEmpty(this.txtFacturas_Email.Text))
        {
            char[] chSeparador = new char[] { ' ' };
            if (this.txtFacturas_Email.Text.Contains(","))
                chSeparador[0] = ',';
            else
                if (this.txtFacturas_Email.Text.Contains(";"))
                    chSeparador[0] = ';';

            string[] strEmails = this.txtFacturas_Email.Text.Split(chSeparador);

            int intValidos = 0;
            foreach (string strEmail in strEmails)
            {
                if (!string.IsNullOrEmpty(strEmail))
                    if (!CRutinas.Validar_Email(strEmail))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Email de facturas no es válido");
                        return false;
                    }
                    else
                        intValidos++;
            }

            if (intValidos == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Email de facturas no es válido");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(this.txtLimiteCR.Text))
        {
            decimal dcmLimiteCR = 0;
            if(!decimal.TryParse(this.txtLimiteCR.Text, out dcmLimiteCR))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Límite de crédito debe ser numérico");
                return false;
            }
            this.txtLimiteCR.Text = Math.Round(dcmLimiteCR, 2).ToString();
        }

        if (!string.IsNullOrEmpty(this.txtDiasCR.Text))
        {
            short shDiasCR = 0;
            if (!short.TryParse(this.txtDiasCR.Text, out shDiasCR))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Días de crédito debe ser numérico");
                return false;
            }
            this.txtDiasCR.Text = shDiasCR.ToString();
        }

        return true;
    }

    protected void Agregar_Cliente()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 " +
                    " FROM establecimientos " +
                    " WHERE negocio = '" + this.txtNegocio.Text.Trim().Replace("'", "''") + "'";

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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Negocio ya existe");
            return;
        }

        this.txtRFC.Text = this.txtRFC.Text.Trim().ToUpper();
        decimal dcmDescuento1, dcmDescuento2;

        decimal.TryParse(this.txtDescuento1.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtDescuento2.Text.Trim(), out dcmDescuento2);

        this.txtDescuento1.Text = dcmDescuento1.ToString("0.##");
        this.txtDescuento2.Text = dcmDescuento2.ToString("0.##");

        strQuery = "INSERT INTO establecimientos (" +
                "nombre, negocio, razonsocial, rfc, telefono, direccionfiscal" +
                ",num_exterior, num_interior, referencia, colonia, municipio" +
                ",estado, pais, cp, poblacion, iva, descuento, descuento2 " +
                ", email, facturas_email, credito " +
                ", direccion_entrega1, direccion_entrega2, direccion_entrega3" +
                ", notas, lista_precios_ID " +
                ", contacto, contacto_tel, contacto2, contacto2_tel, contacto2_email" +
                ", contacto_dr1, contacto_dr1_tel, contacto_dr1_cel" +
                ", contacto_enf1, contacto_enf1_tel, contacto_enf1_cel" +
                ", contacto_dr2, contacto_dr2_tel, contacto_dr2_cel" +
                ", contacto_enf2, contacto_enf2_tel, contacto_enf2_cel" +
                ", datos_completos" +
                ", proveedor, metodo_pago, cuenta_bancaria, banco" +
                ", contado, fecha_modificacion, limite_credito, dias_credito" +
                ", separar_facturas" +
                ") VALUES (" +
                "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNegocio.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtRazonSocial.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtRFC.Text.Trim().ToUpper() + "'" +
                ", '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtPais.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtCP.Text.Trim() + "'" +
                ", '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.rdIVA.SelectedValue + "'" +
                ", '" + this.txtDescuento1.Text.Trim() + "'" +
                ", '" + this.txtDescuento2.Text.Trim() + "'" +
                ", '" + this.txtEmail.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtFacturas_Email.Text.Trim().Replace("'", "''") + "'" +
                ", '0'" +
                ", '" + this.txtDirEntrega1.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtDirEntrega2.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtDirEntrega3.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtNotas.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.dlListaPrecios.SelectedValue + "'" +
                ", '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto2_Email.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr1.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr1_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr1_Cel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf1.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf1_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf1_Cel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr2.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr2_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Dr2_Cel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf2.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf2_Tel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtContacto_Enf2_Cel.Text.Trim().Replace("'", "''") + "'" +
                ", '" + (this.chkCompletos.Checked ? "1" : "0") + "'" +
                ", '" + this.txtProveedor.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.dlMetodoPago.SelectedValue + "'" +
                ", '" + this.txtCuenta_Bancaria.Text.Trim() + "'" +
                ", '" + this.txtBanco.Text.Trim().Replace("'", "''") + "'" +
                ", '" + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") + "'" +
                ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                ", " + (string.IsNullOrEmpty(this.txtLimiteCR.Text) ? "null" : this.txtLimiteCR.Text) +
                ", " + (string.IsNullOrEmpty(this.txtDiasCR.Text) ? "null" : this.txtDiasCR.Text) +
                ", '" + (this.chkSeparar_Facturas.Checked ? "1" : "0") + "'" +
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
                    " FROM establecimientos " +
                    " WHERE negocio = '" + this.txtNegocio.Text.Trim().Replace("'", "''") + "'";
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Cliente ha sido creado<br/><br/>NO OLVIDE AGREGAR AL MENOS UNA SUCURSAL");

        Llenar_Grid();

        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlVentas.Visible = false;
    }

    private void Modificar_Cliente()
    {
        decimal dcmDescuento1, dcmDescuento2;

        decimal.TryParse(this.txtDescuento1.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtDescuento2.Text.Trim(), out dcmDescuento2);

        this.txtDescuento1.Text = dcmDescuento1.ToString("0.##");
        this.txtDescuento2.Text = dcmDescuento2.ToString("0.##");

        string strQuery = "UPDATE establecimientos SET " +
                  "nombre = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                  ",negocio = '" + this.txtNegocio.Text.Trim().Replace("'", "''") + "'" +
                  ",razonsocial = '" + this.txtRazonSocial.Text.Trim().Replace("'", "''") + "'" +
                  ",rfc = '" + this.txtRFC.Text.Trim().Replace("'", "''") + "'" +
                  ",telefono = '" + this.txtTelefono.Text.Trim().Replace("'", "''") + "'" +
                  ",direccionfiscal = '" + this.txtCalle.Text.Trim().Replace("'", "''") + "'" +
                  ",num_exterior = '" + this.txtNumExt.Text.Trim().Replace("'", "''") + "'" +
                  ",num_interior = '" + this.txtNumInt.Text.Trim().Replace("'", "''") + "'" +
                  ",referencia = '" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                  ",colonia = '" + this.txtColonia.Text.Trim().Replace("'", "''") + "'" +
                  ",municipio = '" + this.txtMunicipio.Text.Trim().Replace("'", "''") + "'" +
                  ",estado = '" + this.txtEstado.Text.Trim().Replace("'", "''") + "'" +
                  ",pais = '" + this.txtPais.Text.Trim().Replace("'", "''") + "'" +
                  ",cp = '" + this.txtCP.Text.Trim() + "'" +
                  ",poblacion = '" + this.txtLocalidad.Text.Trim().Replace("'", "''") + "'" +
                  ",iva = " + this.rdIVA.SelectedValue +
                  ",descuento = " + this.txtDescuento1.Text.Trim() +
                  ",descuento2 = " + this.txtDescuento2.Text.Trim() +
                  ",email = '" + this.txtEmail.Text.Trim().Replace("'", "''") + "'" +
                  ",facturas_email = '" + this.txtFacturas_Email.Text.Trim().Replace("'", "''") + "'" +
                  ",direccion_entrega1 = '" + this.txtDirEntrega1.Text.Trim().Replace("'", "''") + "'" +
                  ",direccion_entrega2 = '" + this.txtDirEntrega2.Text.Trim().Replace("'", "''") + "'" +
                  ",direccion_entrega3 = '" + this.txtDirEntrega3.Text.Trim().Replace("'", "''") + "'" +
                  ",notas = '" + this.txtNotas.Text.Trim().Replace("'", "''") + "'" +
                  ",lista_precios_ID = '" + this.dlListaPrecios.SelectedValue + "'" +
                  ",contacto = '" + this.txtContacto.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_tel = '" + this.txtContacto_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2 = '" + this.txtContacto2.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2_tel = '" + this.txtContacto2_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto2_email = '" + this.txtContacto2_Email.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr1 = '" + this.txtContacto_Dr1.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr1_tel = '" + this.txtContacto_Dr1_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr1_cel = '" + this.txtContacto_Dr1_Cel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf1 = '" + this.txtContacto_Enf1.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf1_tel = '" + this.txtContacto_Enf1_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf1_cel = '" + this.txtContacto_Enf1_Cel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr2 = '" + this.txtContacto_Dr2.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr2_tel = '" + this.txtContacto_Dr2_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_dr2_cel = '" + this.txtContacto_Dr2_Cel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf2 = '" + this.txtContacto_Enf2.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf2_tel = '" + this.txtContacto_Enf2_Tel.Text.Trim().Replace("'", "''") + "'" +
                  ",contacto_enf2_cel = '" + this.txtContacto_Enf2_Cel.Text.Trim().Replace("'", "''") + "'" +
                  ",datos_completos = " + (this.chkCompletos.Checked ? "1" : "0") +
                  ",proveedor = '" + this.txtProveedor.Text.Trim().Replace("'", "''") + "'" +
                  ",metodo_pago = " + this.dlMetodoPago.SelectedValue +
                  ",cuenta_bancaria = '" + this.txtCuenta_Bancaria.Text.Trim() + "'" +
                  ",banco = '" + this.txtBanco.Text.Trim().Replace("'", "''") + "'" +
                  ",contado = " + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") +
                  ",fecha_modificacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                  ",limite_credito = " + (string.IsNullOrEmpty(this.txtLimiteCR.Text) ? "null" : this.txtLimiteCR.Text) +
                  ",dias_credito = " + (string.IsNullOrEmpty(this.txtDiasCR.Text) ? "null" : this.txtDiasCR.Text) +
                  ",separar_facturas = " + (this.chkSeparar_Facturas.Checked ? "1" : "0") +
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

        ((master_MasterPage)Page.Master).MostrarMensajeError("Cliente ha sido modificado");

        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlVentas.Visible = false;
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
        ViewState["PagActual"] = int.Parse(ViewState["PagActual"].ToString()) - 1;
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

    private void Mostrar_Sucursales()
    {
        this.gvSucursales.DataSource = ObtenerSucursales();
        this.gvSucursales.DataBind();

        this.pnlListado.Visible = false;
        this.pnlSucursales.Visible = true;
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlSucursales.Visible = false;
        this.pnlListado.Visible = true;
    }

    protected void gvSucursales_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Modificar"))
        {
            if (this.hdAT.Value.Equals("1"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                this.hdSucursalID.Value = this.gvSucursales.DataKeys[index].Value.ToString();
                Mostrar_Datos_Sucursal();
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
        }
    }

    protected void btnAgregarSucursal_Click(object sender, EventArgs e)
    {
        this.hdSucursalID.Value = "0";
        Mostrar_Datos_Sucursal();
    }

    private void Mostrar_Datos_Sucursal()
    {
        this.txtProducto.Text = string.Empty;
        this.txtMinimo.Text = string.Empty;
        this.txtMaximo.Text = string.Empty;
        this.txtPunto_Reorden.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        if (this.hdSucursalID.Value.Equals("0"))
        {
            this.txtSucursal.Text = string.Empty;
            this.txtSucursal_Dirección.Text = string.Empty;
            this.txtSucursal_Telefono.Text = string.Empty;
            this.dlFrecuencia.ClearSelection();
            this.dlFrecuencia.SelectedIndex = 0;
            this.btnAgregarProd.Enabled = false;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * " +
                    " FROM sucursales " +
                    " WHERE ID = " + this.hdSucursalID.Value;
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtSucursal.Text = objRowResult["sucursal"].ToString();
            this.txtSucursal_Dirección.Text = objRowResult["direccion"].ToString();
            this.txtSucursal_Telefono.Text = objRowResult["telefono"].ToString();
            this.dlFrecuencia.ClearSelection();
            this.dlFrecuencia.Items.FindByValue(objRowResult["periodo_visita"].ToString()).Selected = true;
            this.btnAgregarProd.Enabled = true;
        }

        Mostrar_Productos();

        this.pnlSucursales.Visible = false;
        this.pnlSucursal.Visible = true;
    }

    protected void btnRegresarSucursal_Click(object sender, EventArgs e)
    {
        this.pnlSucursal.Visible = false;
        this.pnlSucursales.Visible = true;
    }

    protected void btnCopiar_Click(object sender, EventArgs e)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT * " +
                " FROM establecimientos " +
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

        this.txtSucursal_Telefono.Text = objRowResult["telefono"].ToString();
        this.txtSucursal_Dirección.Text = objRowResult["direccionfiscal"].ToString() +
                    " " + objRowResult["num_exterior"].ToString() +
                    " " + objRowResult["num_interior"].ToString();
    }

    protected void btnGuardarSucursal_Click(object sender, EventArgs e)
    {
        string strQuery;

        if (this.hdSucursalID.Value.Equals("0"))
        {
            strQuery = "INSERT INTO sucursales (" +
                "establecimiento_ID, sucursal, tipo_sucursal, telefono " +
                ", direccion, inactivo, razon_inactivo " +
                ", numero_codificacion, ultima_compra, periodo_visita) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + this.txtSucursal.Text.Trim().Replace("'", "''") + "'" +
                ", '0'" +
                ", '" + this.txtSucursal_Telefono.Text.Trim().Replace("'", "''") + "'" +
                ", '" + this.txtSucursal_Dirección.Text.Trim().Replace("'", "''") + "'" +
                ", '0'" +
                ", ''" +
                ", ''" +
                ", '1990-01-01'" +
                ", '" + this.dlFrecuencia.SelectedValue + "'" +
                ")";
        }
        else
        {
            strQuery = "UPDATE sucursales SET " +
                  "sucursal = '" + this.txtSucursal.Text.Trim().Replace("'", "''") + "'" +
                  ",telefono = '" + this.txtSucursal_Telefono.Text.Trim().Replace("'", "''") + "'" +
                  ",direccion = '" + this.txtSucursal_Dirección.Text.Trim().Replace("'", "''") + "'" +
                  ",periodo_visita = " + this.dlFrecuencia.SelectedValue +
                  " WHERE ID = " + this.hdSucursalID.Value;
        }

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al generar el cliente: " + ex.Message);
            return;
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Datos fueron guardados");
        Mostrar_Sucursales();
        this.pnlSucursal.Visible = false;
        this.pnlSucursales.Visible = true;
    }

    private void Mostrar_Productos()
    {
        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Borrar")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            string strQuery = "DELETE " +
                    " FROM sucursales_productos " +
                    " WHERE sucursal_ID = " + this.hdSucursalID.Value +
                    " AND producto_ID = " + this.gvProductos.DataKeys[index].Value.ToString();
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }
            Mostrar_Productos();
        }
    }

    protected void btnAgregarProd_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdProductoID.Value))
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un producto de la lista");
        else
        {
            int intMinimo, intMaximo, intPunto_Reorden;
            intMaximo = intMaximo = intPunto_Reorden = 0;
            string strMensaje = string.Empty;
            if (int.TryParse(this.txtMinimo.Text.Trim(), out intMinimo) &&
                int.TryParse(this.txtMaximo.Text.Trim(), out intMaximo) &&
                int.TryParse(this.txtPunto_Reorden.Text.Trim(), out intPunto_Reorden))
            {
                if (intMinimo < 0)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Mínimo no puede ser menor a 0");
                    return;
                }

                if (intMaximo <= intMinimo)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Máximo no puede ser menor o igual al mínimo");
                    return;
                }

                if (intPunto_Reorden <= 0)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Punto de reorden debe ser mayor a 0");
                    return;
                }

                if (!Agregar_Producto(out strMensaje))
                    ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidades deben ser numéricas");
        }

        string strClientScript = "setTimeout('setProductoFoco()',500);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private bool Agregar_Producto(out string strMensaje)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1 FROM sucursales_productos" +
                    " WHERE sucursal_ID = " + this.hdSucursalID.Value +
                    " AND producto_ID = " + this.hdProductoID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }

        catch (ApplicationException ex)
        {
            strMensaje = "Error: " + ex.Message;
            return false;
        }

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            strMensaje = "Producto ya existe para la sucursal";
            return false;
        }

        strQuery = "INSERT INTO sucursales_productos (sucursal_ID, " +
                "producto_ID, minimo, maximo, punto_reorden) VALUES (" +
                "'" + this.hdSucursalID.Value + "'" +
                ", '" + this.hdProductoID.Value + "'" +
                ", '" + this.txtMinimo.Text + "'" +
                ", '" + this.txtMaximo.Text + "'" +
                ", '" + this.txtPunto_Reorden.Text + "'" +
                ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            strMensaje = "Error: " + ex.Message;
            return false;
        }

        this.txtProducto.Text = string.Empty;
        this.txtMinimo.Text = string.Empty;
        this.txtMaximo.Text = string.Empty;
        this.txtPunto_Reorden.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        Mostrar_Productos();
        strMensaje = string.Empty;
        return true;
    }
}