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
using CFE;
using System.Configuration;
using System.IO;
using System.Xml;

public partial class notas_notas_cargo : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.imgInfo.Attributes["onmouseout"] = "javascript:esconder();";
        this.txtFecha.Attributes["readonly"] = "true";
        this.txtPago.Attributes["readonly"] = "true";
        this.txtFechaPago.Attributes["readonly"] = "true";
        this.txtPag.Attributes["onkeypress"] = "javascript:return isNumber(event, this);";
        this.txtCambiarCantidad.Attributes["onkeyup"] = "javascript:return validateCambiar(this);";
        this.txtCambiarCantidad.Attributes["onblur"] = "javascript:return validateCambiar(this);";
        this.txtTipoCambio.Attributes["readonly"] = "true";

        this.txtCantidad.Attributes["onkeypress"] =
            this.txtCambiarCantidad.Attributes["onkeypress"] =
            this.txtPrecioUnitario.Attributes["onkeypress"] =
            this.txtCambiarUnitario.Attributes["onkeypress"] =
            this.txtPrecioUnitarioCambio.Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";

        this.txtSubtotal.Attributes["readonly"] = "true";
        this.txtTotal.Attributes["readonly"] = "true";
        this.txtIVA.Attributes["onkeypress"] =
        this.txtISRRet.Attributes["onkeypress"] =
        this.txtIVARet.Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
        this.txtIVA.Attributes["onblur"] =
        this.txtIVARet.Attributes["onblur"] =
        this.txtISRRet.Attributes["onblur"] =
        this.btnTAXCalcular.Attributes["onclick"] = "javascript:sumarPago('" + this.txtSubtotal.ClientID + "'" +
                                                                        ",'" + this.txtIVA.ClientID + "'" +
                                                                        ",'" + this.txtIVARet.ClientID + "'" +
                                                                        ",'" + this.txtISRRet.ClientID + "'" +
                                                                        ",'" + this.txtTotal.ClientID + "'" +
                                                                        ");return false;";

        this.txtTipoCambio.Attributes["readonly"] = "true";
        if (!IsPostBack)
        {
            bool swVer, swTot;
            ViewState["SortCampo"] = "4";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;

            this.hdUsuPr.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1105, out swVer, out swTot))
                this.hdUsuPr.Value = "1";
            else
            {
                this.chkCambiarPrecios.Checked = false;
                this.chkCambiarPrecios.Enabled = false;
                this.txtCambiarUnitario.Enabled = false;
            }

            if (!CComunDB.CCommun.ValidarAcceso(6110, out swVer, out swTot))
            {
                Response.Redirect("../inicio/error.aspx");
                return;
            }

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.hdAT.Value = "0";
            }

            HttpCookie ckSIAN = Request.Cookies["userCng"];
            if (ckSIAN["ck_honorarios"].Equals("1"))
            {
                this.hdHonorarios.Value = "1";
                this.hdPorcISRRet.Value = ckSIAN["ck_isr_ret"];
                this.hdPorcIVARet.Value = ckSIAN["ck_iva_ret"];
            }

            ListItem liIva0 = new ListItem("0%", "0.00");
            decimal intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva1"]) * 100, 2);
            ListItem liIva1 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva2"]) * 100, 2);
            ListItem liIva2 = new ListItem(intIva.ToString("#.##") + "%", intIva.ToString("0.00"));
            this.rdIVA.Items.Add(liIva0);
            //this.rdIVA.Items.Add(liIva1);
            this.rdIVA.Items.Add(liIva2);
            this.rdIVA.Items[1].Selected = true;

            this.txtFecha.Text = DateTime.Today.ToString("dd/MM/yyyy");

            Llenar_Catalogos();

            Llenar_Grid();

            this.hdInventarios.Value = CComunDB.CCommun.Obtener_Valor_CatParametros(20);
            this.hdInvAlmacen.Value = CComunDB.CCommun.Obtener_Valor_CatParametros(21);

            if (Request.QueryString["ID"] != null)
            {
                this.hdID.Value = Request.QueryString["ID"];
                Mostrar_Datos();
            }
            else
                this.hdID.Value = "";
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlTiposPagos.DataSource = CComunDB.CCommun.ObtenerTipos_Pago(false);
        this.dlTiposPagos.DataBind();

        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerNotaCargoEstatus(false);
        this.dlEstatus.DataBind();

        StringBuilder strEstatus = new StringBuilder();
        for (int i = 0; i < this.dlEstatus.Items.Count; i++)
        {
            if (strEstatus.Length > 0)
                strEstatus.Append(", ");
            strEstatus.Append(this.dlEstatus.Items[i].Text);
        }
        this.hdEstatus.Value = strEstatus.ToString();

        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
        this.dlListaPrecios.DataBind();

        Llenar_Personas(true, 0);

        if (this.dlVendedor.Items.Count == 0)
            this.btnModificar.Enabled = false;
        else
            this.btnModificar.Enabled = true;

        this.dlMoneda.DataSource = CComunDB.CCommun.ObtenerMonedas(false);
        this.dlMoneda.DataBind();

        this.dlMonedaPago.DataSource = CComunDB.CCommun.ObtenerMonedas(false);
        this.dlMonedaPago.DataBind();

        this.dlCuentaBancaria.DataSource = CComunDB.CCommun.ObtenerCtasBancarias_CtaContable(false);
        this.dlCuentaBancaria.DataBind();
    }

    private void Llenar_Personas(bool swLlenarActivos, int intPersonaID)
    {
        if (swLlenarActivos)
        {
            this.dlVendedor.DataSource = CComunDB.CCommun.ObtenerVendedores(false, true, 0);
            this.dlVendedor.DataBind();
        }
        else
        {
            if (this.dlVendedor.Items.FindByValue(intPersonaID.ToString()) == null)
            {
                this.dlVendedor.DataSource = CComunDB.CCommun.ObtenerVendedores(false, true, intPersonaID);
                this.dlVendedor.DataBind();
            }
        }
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerNotas();
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

    private DataTable ObtenerNotas()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("notaID", typeof(string)));
        dt.Columns.Add(new DataColumn("negocio", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("estatusID", typeof(string)));
        dt.Columns.Add(new DataColumn("nota", typeof(string)));

        string strQuery = "CALL leer_notas_cargo_consulta(" +
            Session["SIANAppID"] +
            ", " + ViewState["SortCampo"].ToString() +
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
            dr[0] = objRowResult["notaID"].ToString();
            dr[1] = objRowResult["negocio"].ToString();
            if ((DateTime)objRowResult["fecha"] == DateTime.Parse("1901-01-01"))
                dr[2] = string.Empty;
            else
                dr[2] = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy HH:mm");
            if (objRowResult.IsNull("fecha_cancelacion"))
                dr[3] = string.Empty;
            else
                dr[3] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[4] = objRowResult["estatus"].ToString();
            dr[5] = ((decimal)objRowResult["monto"]).ToString("c") + " " +objRowResult["moneda"].ToString();
            dr[6] = objRowResult["estatusID"].ToString();
            dr[7] = objRowResult["nota"].ToString();
            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32((decimal)objRowResult2["paginas"]);

        return dt;
    }

    private DataTable ObtenerProductos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();
        decimal dcmCosto = 0;
        decimal dcmCostoIVA = 0;

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));
        dt.Columns.Add(new DataColumn("exento", typeof(string)));
        dt.Columns.Add(new DataColumn("id", typeof(string)));
        dt.Columns.Add(new DataColumn("con", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_original", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_original_moneda", typeof(string)));

        List<string> lstProductosFE = new List<string>();

        if (!string.IsNullOrEmpty(this.lblNota.Text))
        {
            if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                          this.lblNota.Text + ".xml")))
            {
                XmlDocument xmlNota = new XmlDocument();
                xmlNota.Load(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                               this.lblNota.Text + ".xml"));
                XmlNodeList xmlProductos = xmlNota.GetElementsByTagName("cfdi:Concepto");
                for (int i = 0; i < xmlProductos.Count; i++)
                {
                    lstProductosFE.Add(xmlProductos[i].Attributes["descripcion"].Value);
                }
            }
        }

        string strQuery = "SELECT * FROM (" +
            "SELECT C.producto_ID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
            ", C.exento as exento " +
            ", C.detalle as detalle " +
            ", C.costo_original " +
            ", C.costo_original_moneda " +
            " FROM notas_cargo_prod C " +
            " INNER JOIN productos P " +
            " ON C.producto_ID = P.ID " +
            " AND nota_ID = " + this.hdID.Value +
            ") AS AA ORDER BY consecutivo, producto";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        int intId = 0;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString() + "_" +
                    objRowResult["consecutivo"].ToString();
            if (lstProductosFE.Count > intId)
                dr[1] = lstProductosFE[intId];
            else
                dr[1] = objRowResult["producto"].ToString();
            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("0.##");
            dr[3] = ((decimal)objRowResult["costo_unitario"]).ToString("c");
            decimal dblCosto = (decimal)objRowResult["costo"];
            dr[4] = dblCosto.ToString("c");
            dcmCosto += dblCosto;
            if (Convert.ToBoolean(objRowResult["exento"]))
                dr[5] = "1";
            else
            {
                dcmCostoIVA += dblCosto;
                dr[1] += "*";
                dr[5] = "0";
            }
            if (!objRowResult.IsNull("detalle") && objRowResult["detalle"].ToString().Length > 0)
            {
                if (objRowResult["detalle"].ToString().Length < 50)
                    dr[1] += "<br/>" + objRowResult["detalle"].ToString();
                else
                    dr[1] += "<br/>" + objRowResult["detalle"].ToString().Substring(0, 50);
            }
            if (intId == 0)
                this.hdConsMin.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMax.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMaxID.Value = intId.ToString();
            intId++;
            dr[6] = intId.ToString();
            dr[7] = objRowResult["consecutivo"].ToString();
            dr[8] = ((decimal)objRowResult["costo_original"]).ToString("0.00");
            dr[9] = objRowResult["costo_original_moneda"].ToString();
            dt.Rows.Add(dr);
        }

        dcmCosto = Math.Round(dcmCosto, 2);
        dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

        this.hdCosto.Value = dcmCosto.ToString();
        this.hdCostoIVA.Value = dcmCostoIVA.ToString();

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
            {
                this.btnModificar.Visible = false;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.gvProductos.Enabled = false;
                this.btnFE.Visible = false;
            }
        }
        else
            if (e.CommandName == "Pagos")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                Mostrar_Pagos(this.grdvLista.DataKeys[index].Value.ToString());
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
                case "5":
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    break;
                case "1":
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
        if (this.hdID.Value.Equals("0") && !Obtener_Tipo_Cambio_Dia())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No se ha definido el tipo del cambio del día actual, es necesario que se haga esto antes de poder continuar");
            return;
        }

        this.hdCosto.Value = this.hdCostoIVA.Value = this.hdCostoDescuento.Value = this.hdIVA.Value =
            this.hdSubtotal.Value = this.hdISRRet.Value = this.hdIVARet.Value = this.hdTotal.Value = "0";

        this.btnTAX.Visible = false;
        this.lnkSAT.Visible = false;
        this.lblMensaje.Text = string.Empty;
        this.lblCreado.Text = string.Empty;
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lblNotaID.Text = string.Empty;
        this.lblCorreoEnvio.Text = string.Empty;
        this.lnkPagos.Text = "$0.00";
        this.rdIVA.Enabled = true;
        this.txtMotivoCancelacion.Text = string.Empty;

        this.hdPorcIVARet.Value = "10.67";
        this.hdPorcISRRet.Value = "10";
        this.chkIVARet.Checked = false;
        this.chkISRRet.Checked = false;

        this.hdTAX.Value = "0";
        this.txtIVA.Text = this.txtIVARet.Text = this.txtISRRet.Text = string.Empty;

        this.dlMoneda.ClearSelection();
        this.dlMoneda.SelectedIndex = 0;
        this.btnRefrescarTipoCambio.Visible = false;

        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.btnModificar.ImageUrl = "~/imagenes/modificar.png";

        if (this.hdID.Value.Equals("0"))
        {
            Estado_Campos(true);
            this.hdBorrar.Value = "1";
            this.txtFecha.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtSucursal.Text = string.Empty;
            this.txtComentarios.Text = string.Empty;
            this.txtDescuento1.Text = "0";
            this.txtDescuento2.Text = "0";
            this.txtProducto.Enabled = false;
            this.txtCantidad.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.lblNota.Text = string.Empty;
            this.lblFecha_Timbrado.Text = string.Empty;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = false;
            this.btnCancelar.Visible = false;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = false;
            this.btnEmail.Visible = false;
            this.btnXML.Visible = false;
            this.btnFE.Visible = false;
            this.dlVendedor.Enabled = true;
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue("8").Selected = true;
            this.dlListaPrecios.Enabled = true;
            this.hdCosto.Value = "0";
            this.hdCostoIVA.Value = "0";
            this.hdIVA.Value = "0";
            this.hdTotal.Value = "0";
            this.hdSucursalID.Value = string.Empty;
            this.rdIVA.ClearSelection();
            this.rdIVA.SelectedIndex = 1;
            this.gvProductos.DataSource = null;
            this.gvProductos.DataBind();
            Llenar_Personas(true, 0);

            this.pnlDatos2.Visible = false;
            this.btnModificar.ImageUrl = "~/imagenes/salida.png";
        }
        else
        {
            this.lblNotaID.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT sucursal_ID, concat(negocio, ' - ', sucursal) as nombre, " +
                    " nota, fecha_timbrado, fecha, F.status, F.contado, comentarios, " +
                    " F.descuento, F.descuento2, F.iva, F.lista_precios_ID, F.vendedorID, " +
                    " F.monto_iva_ret, F.total, F.monto_iva, F.tax_manual, F.monto_isr_ret, F.isr_ret, F.iva_ret," +
                    " C.razon as motivo_cancelacion " +
                    ",F.moneda, F.tipo_cambio" +
                    " FROM notas_cargo F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " LEFT JOIN notas_cargo_cancelacion C " +
                    " ON F.ID = C.nota_ID " +
                    " WHERE F.ID = " + this.hdID.Value + ";" +
                    " SELECT fecha_envio" +
                    " FROM correo_envio" +
                    " WHERE ID = " + this.hdID.Value +
                    "   AND tipo = 1";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            if (!objRowResult["status"].ToString().Equals("8"))
                Estado_Campos(false);
            else
                Estado_Campos(true);

            this.lblNota.Text = objRowResult["nota"].ToString();
            if (!objRowResult.IsNull("fecha_timbrado"))
                this.lblFecha_Timbrado.Text = CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objRowResult["fecha_timbrado"]);
            else
                this.lblFecha_Timbrado.Text = string.Empty;

            this.txtFecha.Text = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy");

            this.txtSucursal.Text = objRowResult["nombre"].ToString();
            this.hdSucursalID.Value = objRowResult["sucursal_ID"].ToString();

            this.rdTipo.ClearSelection();
            this.rdTipo.Items.FindByValue(Convert.ToBoolean(objRowResult["contado"]).ToString()).Selected = true;

            this.txtComentarios.Text = objRowResult["comentarios"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["status"].ToString()).Selected = true;

            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_precios_ID"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["iva"]).ToString("0.00")).Selected = true;

            Llenar_Personas(false, (int)objRowResult["vendedorID"]);
            this.dlVendedor.ClearSelection();
            this.dlVendedor.Items.FindByValue(objRowResult["vendedorID"].ToString()).Selected = true;

            this.txtDescuento1.Text = objRowResult["descuento"].ToString();
            this.txtDescuento2.Text = objRowResult["descuento2"].ToString();

            if ((bool)objRowResult["tax_manual"])
            {
                this.hdTAX.Value = "1";
                this.hdIVA.Value = objRowResult["monto_iva"].ToString();
                this.hdIVARet.Value = objRowResult["monto_iva_ret"].ToString();
                this.hdISRRet.Value = objRowResult["monto_isr_ret"].ToString();
            }

            if ((decimal)objRowResult["monto_iva_ret"] > 0)
            {
                this.chkIVARet.Checked = true;
                this.hdPorcIVARet.Value = objRowResult["iva_ret"].ToString();
            }

            if ((decimal)objRowResult["monto_isr_ret"] > 0)
            {
                this.chkISRRet.Checked = true;
                this.hdPorcISRRet.Value = objRowResult["isr_ret"].ToString();
            }

            this.dlMoneda.ClearSelection();
            this.dlMoneda.Items.FindByValue(objRowResult["moneda"].ToString()).Selected = true;
            this.txtTipoCambio.Text = ((decimal)objRowResult["tipo_cambio"]).ToString("0.00##");

            if (!objRowResult["status"].ToString().Equals("8"))
            {
                if (objRowResult["status"].ToString().Equals("9"))
                {
                    this.hdBorrar.Value = "0";
                    this.txtProducto.Enabled = false;
                    this.txtCantidad.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnFE.Visible = false;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = false;
                    this.btnAgregarProd.Visible = false;
                    this.btnEmail.Visible = false;
                    this.lblMensaje.Text = "NOTA CANCELADA: " + objRowResult["motivo_cancelacion"].ToString();
                }
                else
                {
                    this.hdBorrar.Value = "0";
                    this.txtProducto.Enabled = false;
                    this.txtCantidad.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnFE.Visible = false;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = true;
                    this.btnAgregarProd.Visible = false;
                    this.btnEmail.Visible = true;
                }
            }
            else
            {
                Estado_Campos(true);
                this.hdBorrar.Value = "1";
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                if (this.hdUsuPr.Value.Equals("0"))
                    this.txtPrecioUnitario.Enabled = false;
                else
                    this.txtPrecioUnitario.Enabled = true;
                this.btnModificar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnXML.Visible = false;
                this.btnFE.Visible = true;
                this.btnCancelar.Visible = true;
                this.btnAgregarProd.Visible = true;
                this.btnAgregarProd.Enabled = true;
                this.dlListaPrecios.Enabled = true;
                this.btnEmail.Visible = false;
                this.btnRefrescarTipoCambio.Visible = true;

                if (!objRowResult.IsNull("fecha_timbrado"))
                {
                    this.rdIVA.Enabled = false;
                    this.txtDescuento1.Enabled = false;
                    this.txtDescuento2.Enabled = false;
                    this.chkIVARet.Enabled = false;
                    this.hdBorrar.Value = "0";
                    this.txtProducto.Enabled = false;
                    this.txtCantidad.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = true;
                    this.btnAgregarProd.Visible = false;
                    this.btnRefrescarTipoCambio.Visible = false;
                }
            }

            if (objDataResult.Tables[1].Rows.Count > 0)
                this.lblCorreoEnvio.Text = "Correo enviado: " + CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objDataResult.Tables[1].Rows[0]["fecha_envio"]);

            strQuery = "SELECT SUM(F.monto_pago) AS monto_pago" +
                       " FROM pago_notas_cargo F" +
                       " WHERE F.notaID = " + this.hdID.Value +
                       " AND F.aplicado = 1";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (!objDataResult.Tables[0].Rows[0].IsNull("monto_pago"))
                this.lnkPagos.Text = ((decimal)objDataResult.Tables[0].Rows[0]["monto_pago"]).ToString("c");

            Llenar_Productos(false);
            Obtener_Notas();
        }

        Obtener_Moneda();

        this.dlMoneda.Enabled = false;
    }

    private void Estado_Campos(bool sw_estado)
    {
        this.dlVendedor.Enabled = sw_estado;
        this.txtSucursal.Enabled = sw_estado;
        this.dlListaPrecios.Enabled = sw_estado;
        this.rdIVA.Enabled = sw_estado;
        this.rdTipo.Enabled = sw_estado;
        this.txtDescuento1.Enabled = sw_estado;
        this.txtDescuento2.Enabled = sw_estado;
        this.txtComentarios.Enabled = sw_estado;
        this.chkIVARet.Enabled = sw_estado;
        this.chkISRRet.Enabled = sw_estado;
        this.dlMoneda.Enabled = sw_estado;
        this.dlMoneda.Enabled = sw_estado;
    }

    private void Agregar_Nota()
    {
        if (string.IsNullOrEmpty(this.hdSucursalID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return;
        }

        if (Crear_Nota(int.Parse(this.hdSucursalID.Value),
                          DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")),
                          decimal.Parse(this.rdIVA.SelectedValue),
                          Convert.ToBoolean(this.rdTipo.SelectedValue),
                          decimal.Parse(this.txtDescuento1.Text),
                          decimal.Parse(this.txtDescuento2.Text),
                          this.txtComentarios.Text.Trim(),
                          int.Parse(this.dlListaPrecios.SelectedValue),
                          int.Parse(this.dlVendedor.SelectedValue),
                          this.dlMoneda.SelectedValue,
                          decimal.Parse(this.txtTipoCambio.Text)))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("La nota ha sido creada, folio interno: " + this.hdID.Value);
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = true;
            this.btnXML.Visible = false;
            this.btnFE.Visible = true;
            this.btnCancelar.Visible = true;
            this.btnAgregarProd.Enabled = true;
            this.dlMoneda.Enabled = true;
            this.btnRefrescarTipoCambio.Visible = true;
            Obtener_CreadoPor();

            this.pnlDatos2.Visible = true;
            this.btnModificar.ImageUrl = "~/imagenes/modificar.png";
        }
    }

    private bool Crear_Nota(int intSucursalID, DateTime dtFecha,
                               decimal dcmIVA, bool esContado,
                               decimal dcmDescuento1, decimal dcmDescuento2,
                               string strComentarios, int intListaPreciosID,
                               int intVendedorID,
                               string strMoneda, decimal dcmTipo_Cambio)
    {
        DataSet objDataResult = new DataSet();
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        CNotaCargoDB objNota = new CNotaCargoDB();

        objNota.fecha = dtFecha;
        objNota.sucursal_ID = intSucursalID;
        objNota.iva = dcmIVA;
        objNota.contado = esContado;
        objNota.descuento = dcmDescuento1;
        objNota.descuento2 = dcmDescuento2;
        objNota.comentarios = strComentarios;
        objNota.status = 8;
        objNota.lista_precios_ID = intListaPreciosID;
        objNota.vendedorID = intVendedorID;
        objNota.isr_ret = (this.chkISRRet.Checked ? decimal.Parse(this.hdPorcISRRet.Value) : 0);
        objNota.iva_ret = (this.chkIVARet.Checked ? decimal.Parse(this.hdPorcIVARet.Value) : 0);
        objNota.tax_manual = (this.hdTAX.Value.Equals("1") ? true : false);
        objNota.moneda = strMoneda;
        objNota.tipo_cambio = dcmTipo_Cambio;

        objNota.fecha_contrarecibo = null;
        objNota.fecha_cobranza = null;
        if (!objNota.contado)
        {
            // Busca fecha de contrarecibo
            DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza_SucursalID(1, intSucursalID);
            if (dtFechaCobranza.HasValue)
                objNota.fecha_contrarecibo = dtFechaCobranza.Value;
            else
            {
                // Busca fecha de cobro
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza_SucursalID(2, intSucursalID);
                if (dtFechaCobranza.HasValue)
                    objNota.fecha_cobranza = dtFechaCobranza.Value;
                else
                    objNota.fecha_cobranza = DateTime.Today;
            }
        }

        if (objNota.Guardar())
        {
            this.hdID.Value = objNota.ID.ToString();
            this.lblNotaID.Text = this.hdID.Value;
            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objNota.Mensaje);
            return false;
        }
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Nota();
        }
        else
            if (Actualizar_Nota(8))
                ((master_MasterPage)Page.Master).MostrarMensajeError("La nota ha sido modificada");
    }

    private bool Actualizar_Nota(byte btStatus)
    {
        DateTime dtFecha = DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        decimal dcmDescuento1, dcmDescuento2;
        decimal.TryParse(this.txtDescuento1.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtDescuento2.Text.Trim(), out dcmDescuento2);

        CNotaCargoDB objNota = new CNotaCargoDB();

        objNota.Leer(int.Parse(this.hdID.Value));

        this.txtDescuento1.Text = dcmDescuento1.ToString("0.00");
        this.txtDescuento2.Text = dcmDescuento2.ToString("0.00");

        objNota.fecha = dtFecha;
        objNota.sucursal_ID = int.Parse(this.hdSucursalID.Value);
        objNota.iva = decimal.Parse(this.rdIVA.SelectedValue);
        objNota.contado = Convert.ToBoolean(this.rdTipo.SelectedValue);
        objNota.descuento = dcmDescuento1;
        objNota.descuento2 = dcmDescuento2;
        objNota.comentarios = this.txtComentarios.Text.Trim();
        objNota.status = btStatus;
        objNota.lista_precios_ID = int.Parse(this.dlListaPrecios.SelectedValue);
        objNota.vendedorID = int.Parse(this.dlVendedor.SelectedValue);
        objNota.tax_manual = (this.hdTAX.Value.Equals("1") ? true : false);
        objNota.moneda = this.dlMoneda.SelectedValue;
        objNota.tipo_cambio = decimal.Parse(this.txtTipoCambio.Text);

        if (!objNota.contado)
        {
            // Busca fecha de contrarecibo
            DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(1, objNota.ID);
            if (dtFechaCobranza.HasValue)
                objNota.fecha_contrarecibo = dtFechaCobranza.Value;
            else
            {
                // Busca fecha de cobro
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranzaNotaCargo(2, objNota.ID);
                if (dtFechaCobranza.HasValue)
                    objNota.fecha_cobranza = dtFechaCobranza.Value;
                else
                    objNota.fecha_cobranza = DateTime.Today;
            }
        }
        else
        {
            objNota.fecha_contrarecibo = null;
            objNota.fecha_cobranza = null;
        }

        if (objNota.Guardar())
        {
            Obtener_CreadoPor();
            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objNota.Mensaje);
            return false;
        }
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    protected void btnAgregarProd_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdProductoID.Value))
        {
            if (!Buscar_Producto())
                ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un producto de la lista");
            return;
        }

        decimal dcmCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        string strMensaje = string.Empty;
        if (decimal.TryParse(this.txtCantidad.Text.Trim(), out dcmCantidad) &&
            decimal.TryParse(this.txtPrecioUnitario.Text.Trim(), out dcmPrecioUnitario))
        {
            dcmCantidad = Math.Round(dcmCantidad, 2);
			this.txtCantidad.Text = dcmCantidad.ToString();
            if (dcmCantidad <= 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad no puede ser menor o igual a cero");
                return;
            }
            if (dcmPrecioUnitario <= 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Precio no puede ser menor o igual a cero");
                return;
            }

            if (!string.IsNullOrEmpty(this.hdMinimo.Value))
            {
                if (dcmCantidad % decimal.Parse(this.hdMinimo.Value) != 0)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser un múltiplo de " + this.hdMinimo.Value);
                    return;
                }
            }

            this.lblPrecioProducto.Text = this.txtProducto.Text;
            if (this.lblPrecioProducto.Text.Length > 25)
                this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 25);
            this.hdProductoPrecioID.Value = this.hdProductoID.Value;
            if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                    dcmCantidad, Math.Round(dcmPrecioUnitario, 2),
                                    this.txtDetalle.Text.Trim(),
                                    this.chkImpDetalle.Checked,
                                    out strMensaje))
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
            else
                if (this.chkCambiarPrecios.Checked)
                {
                    decimal dcmPrecioUnitarioOrig = 0;
                    decimal.TryParse(this.hdPrecioUnitario.Value, out dcmPrecioUnitarioOrig);
                    if (dcmPrecioUnitario != dcmPrecioUnitarioOrig)
                    {
                        this.lblPrecioLista.Text = this.dlListaPrecios.SelectedItem.Text;
                        this.lblPrecioAnterior.Text = dcmPrecioUnitarioOrig.ToString("c");
                        this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                        this.txtPrecioUnitarioCambio.Focus();
                        this.mdCambiarPrecio.Show();
                        return;
                    }
                }
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad y precio unitario deben ser numéricos");

        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private bool Buscar_Producto()
    {
        if (!string.IsNullOrEmpty(this.txtProducto.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, nombre," +
                             "           V.precio_caja as precio " +
                             "          ,P.minimo_compra" +
                             "    FROM productos P " +
                             "    LEFT JOIN precios V " +
                             "    ON P.ID = V.producto_ID" +
                             "    AND lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                             "    AND validez = '2099-12-31'" +
                             "    WHERE (codigo = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo2 = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo3 = '" + this.txtProducto.Text.Trim() + "'" +
                             "    ) AND tipo = 2" +
                             " ) R" +
                             " LEFT JOIN producto_datos D" +
                             " ON D.productoID = R.ID" +
							 " AND D.appID = " + Session["SIANAppID"];

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult.Tables[0].Rows.Count == 1)
            {
                this.hdProductoID.Value = objDataResult.Tables[0].Rows[0][0].ToString();
                this.txtProducto.Text = objDataResult.Tables[0].Rows[0][1].ToString() +
                                        "(" + ((decimal)objDataResult.Tables[0].Rows[0]["existencia"]).ToString("0.##") + ")";
                if (objDataResult.Tables[0].Rows[0].IsNull(2))
                    this.txtPrecioUnitario.Text = "0.00";
                else
                    this.txtPrecioUnitario.Text = objDataResult.Tables[0].Rows[0][2].ToString();
                if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
                    this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##");
                else
                    this.hdMinimo.Value = string.Empty;
                this.hdPrecioUnitario.Value = this.txtPrecioUnitario.Text;
                string strClientScript = "setTimeout('setCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Agregar_Producto(string strProductoID,
                                string strProducto,
                                decimal dcmCantidad,
                                decimal dcmCosto_unitario,
                                string strDetalle,
                                bool swImprimirDetalle,
                                out string strMensaje)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        decimal dcmCosto_Original = dcmCosto_unitario;

        if (!this.hdMoneda.Value.Equals(this.dlMoneda.SelectedValue))
        {
            // A pesos
            if (this.dlMoneda.SelectedValue.Equals("MXN"))
                dcmCosto_unitario = Math.Round(decimal.Parse(this.txtTipoCambio.Text) * dcmCosto_unitario, 2);
            else
                dcmCosto_unitario = Math.Round(dcmCosto_unitario / decimal.Parse(this.txtTipoCambio.Text), 2);
        }

        string strQuery = "SELECT exento " +
                    " FROM productos " +
                    " WHERE ID = " + strProductoID;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }

        catch (ApplicationException ex)
        {
            strMensaje = "Error: " + ex.Message;
            return false;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        string strExento = "0";
        if (Convert.ToBoolean(objRowResult["exento"]))
            strExento = "1";

        decimal dcmCosto;
        dcmCosto = Math.Round(dcmCosto_unitario * dcmCantidad, 2);

        strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                    " FROM notas_cargo_prod " +
                    " WHERE nota_ID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        string strImprimirDetalle = "0";
        if (string.IsNullOrEmpty(strDetalle))
            strDetalle = "null";
        else
        {
            strImprimirDetalle = (swImprimirDetalle ? "1" : "0");
            if (strDetalle.Length > 5000)
                strDetalle = strDetalle.Substring(0, 5000);
            strDetalle = "'" + strDetalle + "'";
        }

        strQuery = "INSERT INTO notas_cargo_prod (nota_ID, " +
                "producto_ID, exento, cantidad, costo_unitario" +
                ",costo_original, costo_original_moneda" +
                ",costo, consecutivo, detalle, imprimir_detalle) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + dcmCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto_Original.ToString() + "'" +
                ", '" + this.hdMoneda.Value + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", " + strDetalle +
                ", " + strImprimirDetalle +
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
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.txtDetalle.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        Llenar_Productos(true);
        strMensaje = string.Empty;
        return true;
    }

    protected void gvProductos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (this.hdBorrar.Value.Equals("0"))
            {
                e.Row.Cells[0].Enabled = false;
                e.Row.Cells[1].Controls.Clear();
                e.Row.Cells[6].Controls.Clear();
            }
            else
            {
                if (e.Row.RowIndex == 0)
                    ((ImageButton)e.Row.Cells[1].Controls[1]).Enabled = false;
                if (e.Row.RowIndex.ToString().Equals(this.hdConsMaxID.Value))
                    ((ImageButton)e.Row.Cells[1].Controls[3]).Enabled = false;
            }
        }
        else
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                StringBuilder strLeyenda = new StringBuilder();
                StringBuilder strValores = new StringBuilder();
                decimal dcmPorcDescuento1, dcmPorcDescuento2;
                decimal dcmCosto, dcmCostoDescuento, dcmCostoIVA;
                decimal dcmIVA;
                decimal dcmISRRet, dcmIVARet, dcmTotal;
                decimal.TryParse(this.txtDescuento1.Text, out dcmPorcDescuento1);
                decimal.TryParse(this.txtDescuento2.Text, out dcmPorcDescuento2);
                decimal.TryParse(this.hdCosto.Value, out dcmCosto);
                decimal.TryParse(this.hdCostoIVA.Value, out dcmCostoIVA);

                //Subtotal antes de descuento, es la suma de todos los productos
                dcmCosto = Math.Round(dcmCosto, 2);
                strLeyenda.Append("Subtotal:");
                strValores.Append(dcmCosto.ToString("c") + " " + this.dlMoneda.SelectedValue);

                dcmCostoDescuento = dcmCosto;
                dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

                // Si hay descuento, se calcula el subtotal con descuento
                if (dcmPorcDescuento1 != 0 || dcmPorcDescuento2 != 0)
                {
                    dcmCostoDescuento = Math.Round(dcmCostoDescuento * (1 - (dcmPorcDescuento1 / 100)), 2);
                    dcmCostoDescuento = Math.Round(dcmCostoDescuento * (1 - (dcmPorcDescuento2 / 100)), 2);
                    this.hdCostoDescuento.Value = dcmCostoDescuento.ToString();
                    dcmCostoIVA = Math.Round(dcmCostoIVA * (1 - (dcmPorcDescuento1 / 100)), 2);
                    dcmCostoIVA = Math.Round(dcmCostoIVA * (1 - (dcmPorcDescuento2 / 100)), 2);
                    strLeyenda.Append("<br />Subtotal con descuento:");
                    strValores.Append("<br />" + dcmCostoDescuento.ToString("c") + " " + this.dlMoneda.SelectedValue);
                }
                else
                    this.hdCostoDescuento.Value = "0";

                // IVA
                if (this.hdTAX.Value.Equals("0"))
                    dcmIVA = Math.Round(dcmCostoIVA * (decimal.Parse(this.rdIVA.SelectedValue)) / 100, 2);
                else
                    dcmIVA = decimal.Parse(this.hdIVA.Value);
                this.hdIVA.Value = dcmIVA.ToString();
                strLeyenda.Append("<br />IVA " + (decimal.Parse(this.rdIVA.SelectedValue)).ToString("0.##") + "%:");
                strValores.Append("<br />" + dcmIVA.ToString("c") + " " + this.dlMoneda.SelectedValue);

                this.hdSubtotal.Value = "0";

                dcmTotal = Math.Round(dcmCostoDescuento + dcmIVA, 2);

                if (this.chkIVARet.Checked)
                {
                    if (this.hdTAX.Value.Equals("0"))
                        dcmIVARet = Math.Round(dcmCostoDescuento * decimal.Parse(this.hdPorcIVARet.Value) / 100, 2);
                    else
                        dcmIVARet = decimal.Parse(this.hdIVARet.Value);

                    this.hdIVARet.Value = dcmIVARet.ToString();
                    dcmTotal -= dcmIVARet;
                    strLeyenda.Append("<br />Retención IVA:");
                    strValores.Append("<br />" + dcmIVARet.ToString("c") + " " + this.dlMoneda.SelectedValue);
                }
                else
                    this.hdIVARet.Value = "0";

                if (this.chkISRRet.Checked)
                {
                    if (this.hdTAX.Value.Equals("0"))
                        dcmISRRet = Math.Round(dcmCostoDescuento * decimal.Parse(this.hdPorcISRRet.Value) / 100, 2);
                    else
                        dcmISRRet = decimal.Parse(this.hdISRRet.Value);

                    this.hdISRRet.Value = dcmISRRet.ToString();
                    dcmTotal -= dcmISRRet;
                    strLeyenda.Append("<br />Retención ISR:");
                    strValores.Append("<br />" + dcmISRRet.ToString("c") + " " + this.dlMoneda.SelectedValue);
                }
                else
                    this.hdISRRet.Value = "0";

                this.hdTotal.Value = dcmTotal.ToString();
                strLeyenda.Append("<br />TOTAL:");
                strValores.Append("<br />" + dcmTotal.ToString("c") + " " + this.dlMoneda.SelectedValue);

                e.Row.Cells.RemoveAt(3);
                e.Row.Cells.RemoveAt(4);

                e.Row.Cells[0].ColumnSpan = 4;
                e.Row.Cells[0].Text = strLeyenda.ToString();
                e.Row.Cells[1].ColumnSpan = 2;
                e.Row.Cells[1].Text = strValores.ToString();
                e.Row.Cells[2].Text = string.Empty;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
            }
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("mv"))
            return;

        if (this.dlEstatus.SelectedValue.Equals("8"))
        {
            int index = Convert.ToInt32(e.CommandArgument);
            string[] strID_Consecutivo = this.gvProductos.DataKeys[index].Value.ToString().Split('_');

            if (e.CommandName == "Borrar")
            {
                string strQuery = "DELETE " +
                        " FROM notas_cargo_prod " +
                        " WHERE nota_ID = " + this.hdID.Value +
                        " AND producto_ID = " + strID_Consecutivo[0] +
                        " AND consecutivo = " + strID_Consecutivo[1];
                try
                {
                    CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch (ApplicationException ex)
                {
                    throw new ApplicationException("Error: " + ex.Message);
                }

                strQuery = "UPDATE notas_cargo_prod SET " +
                          " consecutivo = consecutivo - 1 " +
                          " WHERE nota_ID = " + this.hdID.Value +
                          " AND consecutivo > " + strID_Consecutivo[1];

                CComunDB.CCommun.Ejecutar_SP(strQuery);

                Llenar_Productos(true);
            }
            else
                if (e.CommandName == "Modificar")
                {
                    this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                    this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[3].Text;
                    this.txtCambiarUnitario.Text = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginal")).Value;
                    this.lblMonedaCambiar.Text =
                        this.hdMonedaTemp.Value = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginalMoneda")).Value;
                    string strQuery = "SELECT P.nombre, C.detalle, C.imprimir_detalle, P.minimo_compra " +
                              " FROM notas_cargo_prod C" +
                              " INNER JOIN productos P " +
                              " ON C.producto_ID = P.ID " +
                              " AND C.nota_ID = " + this.hdID.Value +
                              " AND C.producto_ID = " + strID_Consecutivo[0] +
                              " AND C.consecutivo = " + strID_Consecutivo[1];
                    DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

                    if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
                        this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##");
                    else
                        this.hdMinimo.Value = string.Empty;
                    this.lblCambiarProducto.Text = objDataResult.Tables[0].Rows[0]["nombre"].ToString();
					this.txtCambiarDetalle.Text = objDataResult.Tables[0].Rows[0]["detalle"].ToString();
                    this.chkCambiarImpDet.Checked = (bool)objDataResult.Tables[0].Rows[0]["imprimir_detalle"];
                    this.mdCambiarProducto.Show();
                    string strClientScript = "setTimeout('setProductoCantidad()',100);";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                }
        }
    }

    private void Llenar_Productos(bool swGuardarMod)
    {
        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        if (this.gvProductos.Rows.Count > 0 &&
            this.dlEstatus.SelectedValue.Equals("8"))
            this.btnTAX.Visible = true;
        else
            this.btnTAX.Visible = false;

        CNotaCargoDB objNota = new CNotaCargoDB();

        objNota.Leer(int.Parse(this.hdID.Value));

        objNota.registrarMod = swGuardarMod;
        objNota.isr_ret = (this.chkISRRet.Checked ? decimal.Parse(this.hdPorcISRRet.Value) : 0);
        objNota.iva_ret = (this.chkIVARet.Checked ? decimal.Parse(this.hdPorcIVARet.Value) : 0);
        objNota.monto_subtotal = Math.Round(decimal.Parse(this.hdCosto.Value), 2);
        objNota.monto_descuento = Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2);
        objNota.monto_iva = Math.Round(decimal.Parse(this.hdIVA.Value), 2);
        objNota.monto_subtotal2 = Math.Round(decimal.Parse(this.hdSubtotal.Value), 2);
        objNota.monto_isr_ret = Math.Round(decimal.Parse(this.hdISRRet.Value), 2);
        objNota.monto_iva_ret = Math.Round(decimal.Parse(this.hdIVARet.Value), 2);
        objNota.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
        objNota.total_real = objNota.total;
        objNota.descuento = decimal.Parse(this.txtDescuento1.Text);
        objNota.descuento2 = decimal.Parse(this.txtDescuento2.Text);
        objNota.iva = decimal.Parse(this.rdIVA.SelectedValue);
        objNota.moneda = this.dlMoneda.SelectedValue;
        objNota.tipo_cambio = decimal.Parse(this.txtTipoCambio.Text);

        if (!objNota.Guardar())
            ((master_MasterPage)Page.Master).MostrarMensajeError(objNota.Mensaje);

        Obtener_CreadoPor();
    }

    protected void btnCancelarContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if (DateTime.Parse(strValores[1]) >= DateTime.Now)
                strCodigo = strValores[0];
        }

        if (string.IsNullOrEmpty(this.txtCodigo_Ver_Canc.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigo_Ver_Canc.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
        }

        Cancelar_Nota();
    }

    private void Cancelar_Nota()
    {

        if (!string.IsNullOrEmpty(this.lblFecha_Timbrado.Text))  // Factura timbrada
        {
            if (!Cancelar_Nota_WS())
                return;
        }

        Cancelar_Nota_Validada();
    }

    protected void btnCancelarWSContinuar_Click(object sender, EventArgs e)
    {
        Cancelar_Nota_Validada();
    }

    private void Cancelar_Nota_Validada()
    {
        string strQuery = "UPDATE notas_cargo SET " +
                         " status = 9" +
                         ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         ",modificadoPorID = " + Session["SIANID"].ToString() +
                         ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        strQuery = "INSERT INTO notas_cargo_cancelacion (" +
                   "nota_ID" +
                   ",razon" +
                   ") VALUES(" +
                   "'" + this.hdID.Value + "'" +
                   ",'" + this.txtMotivoCancelacion.Text.Trim().Replace("'", "''") + "'" +
                   ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataSet objDataResult = new DataSet();

        strQuery = "SELECT producto_ID " +
                  " FROM notas_cargo_prod " +
                  " WHERE nota_ID = " + this.hdID.Value;
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
            CProducto_Datos objProd_Datos = new CProducto_Datos();
            objProd_Datos.intProductoID = (int)objRowResult[0];
			objProd_Datos.intAppID = int.Parse(Session["SIANAppID"].ToString());
            objProd_Datos.Leer();
            objProd_Datos.Recalcular_Notas();
            objProd_Datos.Guardar();
        }

        this.txtProducto.Enabled = false;
        this.txtCantidad.Enabled = false;
        this.txtPrecioUnitario.Enabled = false;
        this.btnModificar.Visible = false;
        this.btnImprimir.Visible = false;
        this.btnCancelar.Visible = false;
        this.btnAgregarProd.Enabled = false;
    }

    private bool Cancelar_Nota_WS()
    {
        string strMensaje;

        if (!CFDI.Cancelar_Nota_Cargo(int.Parse(this.hdID.Value), out strMensaje))
        {
            this.lblMessageCancelarWS.Text = strMensaje;
            this.mdCancelarWS.Show();
            return false;
        }

        return true;
    }

    protected void btnCambiarContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad, dcmPrecio;

        decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
        decimal.TryParse(this.txtCambiarUnitario.Text, out dcmPrecio);

        dcmCantidad = Math.Round(dcmCantidad, 2);
        dcmPrecio = Math.Round(dcmPrecio, 2);

        if (dcmCantidad > 0 && dcmPrecio > 0)
        {
            if (!string.IsNullOrEmpty(this.hdMinimo.Value))
            {
                if (dcmCantidad % decimal.Parse(this.hdMinimo.Value) != 0)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser un múltiplo de " + this.hdMinimo.Value);
                    return;
                }
            }

            this.hdTAX.Value = "0";
            string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');
	        string strDetalle = "null";
            string strImprimirDetalle = "0";

	        if (!string.IsNullOrEmpty(this.txtCambiarDetalle.Text.Trim()))
	        {
                strImprimirDetalle = (this.chkCambiarImpDet.Checked ? "1" : "0");
	            if (this.txtCambiarDetalle.Text.Trim().Length > 5000)
	                strDetalle = "'" + this.txtCambiarDetalle.Text.Trim().Substring(0, 5000) + "'";
	            else
	                strDetalle = "'" + this.txtCambiarDetalle.Text.Trim() + "'";
	        }

            decimal dcmCosto_Original = dcmPrecio;
            if (!this.hdMonedaTemp.Value.Equals(this.dlMoneda.SelectedValue))
            {
                // A pesos
                if (this.dlMoneda.SelectedValue.Equals("MXN"))
                    dcmPrecio = Math.Round(decimal.Parse(this.txtTipoCambio.Text) * dcmPrecio, 2);
                else
                    dcmPrecio = Math.Round(dcmPrecio / decimal.Parse(this.txtTipoCambio.Text), 2);
            }

            string strQuery = "UPDATE notas_cargo_prod SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo_original = " + dcmCosto_Original.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                        ",detalle = " + strDetalle +
                        ",imprimir_detalle = " + strImprimirDetalle +
                        " WHERE nota_ID = " + this.hdID.Value +
                        " AND producto_ID = " + strID_Consecutivo[0] +
                        " AND consecutivo = " + strID_Consecutivo[1];
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }
            Llenar_Productos(true);
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad/Precio no puede ser menor o igual a cero");
    }

    [System.Web.Services.WebMethod]
    public static string ObtenerPrecio(string strParametros)
    {
        string[] strParametro = strParametros.Split('|');

        return CComunDB.CCommun.ObtenerPrecioProductoCliente(strParametro[0],
                                                             strParametro[1],
                                                             strParametro[2],
                                                               strParametro[3]);
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {
        if (System.IO.File.Exists(Server.MapPath("../facturas" + "/factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx")))
        {
            if (string.IsNullOrEmpty(this.lblFecha_Timbrado.Text))
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + "_pre.aspx?notaID=" + this.hdID.Value + "')",
                                                    true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx?notaID=" + this.hdID.Value + "')",
                                                    true);
        }
        else
        {
            if (string.IsNullOrEmpty(this.lblFecha_Timbrado.Text))
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_preimpresion.aspx?notaID=" + this.hdID.Value + "')",
                                                    true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion.aspx?notaID=" + this.hdID.Value + "')",
                                                    true);
        }
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    protected void dlNegocios_SelectedIndexChanged(object sender, EventArgs e)
    {
        Obtener_Notas();
    }

    private void Obtener_Notas()
    {
        string[] strValores = Obtener_NotasWeb(this.hdSucursalID.Value).Split('|');
        this.lblNotas.Text = strValores[0];
		if (string.IsNullOrEmpty(strValores[6]))
				this.dlMoneda.Enabled = true;
			else
				this.dlMoneda.Enabled = false;

        this.txtSucursal.Focus();
    }

    [System.Web.Services.WebMethod]
    public static string Obtener_NotasWeb(string strParametros)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.notas, E.descuento, E.descuento2, " +
                    " E.iva, E.lista_precios_ID, E.contado, E.moneda " +
                    ",E.vendedorID " +
                    " FROM establecimientos E " +
                    " INNER JOIN sucursales S " +
                    " ON S.establecimiento_ID = E.ID " +
                    " AND S.ID = " + strParametros;
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
               "|" + objRowResult["descuento2"].ToString() +
               "|" + ((double)objRowResult["iva"]).ToString("0.00") +
               "|" + objRowResult["lista_precios_ID"].ToString() +
               "|" + (Convert.ToBoolean(objRowResult["contado"]) ? "1" : "0") +
               "|" + objRowResult["moneda"].ToString() +
               "|" + objRowResult["vendedorID"].ToString();
    }

    protected void btnPrecioContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmPrecioUnitario = 0;
        if (decimal.TryParse(this.txtPrecioUnitarioCambio.Text.Trim(), out dcmPrecioUnitario))
        {
            CComunDB.CCommun.Actualizar_Precio(this.hdProductoPrecioID.Value,
                                               this.dlListaPrecios.SelectedValue,
                                               dcmPrecioUnitario);
        }
    }

    protected void txtDescuento1_TextChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
        {
            decimal dcmDescuento;
            decimal.TryParse(this.txtDescuento1.Text.Trim(), out dcmDescuento);
            dcmDescuento = Math.Round(dcmDescuento, 2);
            this.txtDescuento1.Text = dcmDescuento.ToString();
            Llenar_Productos(true);
        }
        this.txtDescuento2.Focus();
    }

    protected void txtDescuento2_TextChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
        {
            decimal dcmDescuento;
            decimal.TryParse(this.txtDescuento2.Text.Trim(), out dcmDescuento);
            dcmDescuento = Math.Round(dcmDescuento, 2);
            this.txtDescuento2.Text = dcmDescuento.ToString();
            Llenar_Productos(true);
        }
        this.txtComentarios.Focus();
    }
    protected void btnFE_Click(object sender, ImageClickEventArgs e)
    {
        Actualizar_Nota(8);

        decimal dcmResto = Math.Round(decimal.Parse(this.hdTotal.Value) -
                                      decimal.Parse(this.lnkPagos.Text.Remove(0, 1)),
                                      2);

        if (dcmResto < 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Monto total de la nota menor a los pagos ya registrados");
            return;
        }

        if (this.rdTipo.SelectedIndex == 0)  //Contado
        {
            if (dcmResto > 0)
            {
                this.txtFechaPago.Text = DateTime.Today.ToString("dd/MM/yyyy");
                this.txtReferencia.Text = string.Empty;
                this.txtPago.Text = this.hdPago.Value = dcmResto.ToString();
                this.dlMonedaPago.ClearSelection();
                this.dlMonedaPago.Items.FindByValue(this.dlMoneda.SelectedValue).Selected = true;
                this.mdPago.Show();
            }
            else
                Timbrar_Nota();
        }
        else
        {
            if (dcmResto == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Nota ya se encuentra pagada por lo que no se puede poner mandar a cobranza, cambie el tipo a Contado");
                return;
            }
            Timbrar_Nota();
        }
    }

    private bool Timbrar_Nota()
    {
        if (!CComunDB.CCommun.Validar_Folios_Electronicos())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Se ha alcanzado el máximo número de folios electrónicos permitidos para timbrar, favor de contactar al administrador");
            return false;
        }

        string strMensaje, strUUID;
        DateTime dtFechaTimbrado;
        if (CFE.CFDI.Generar_Nota_Cargo(int.Parse(this.hdID.Value),
                                        out strMensaje,
                                        out strUUID,
                                        out dtFechaTimbrado))
        {
            Mostrar_Datos();

            this.lblCorreo.Text = strMensaje;
            this.mdCorreo.Show();

            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al generar la nota eléctrónica:" + strMensaje);
            return false;
        }
    }

    protected void btnEmail_Click(object sender, ImageClickEventArgs e)
    {
        Abrir_Correo();
    }

    protected void btnCorreoContinuar_Click(object sender, EventArgs e)
    {
        Abrir_Correo();
    }

    private void Abrir_Correo()
    {
        if (System.IO.File.Exists(Server.MapPath("../facturas" + "/factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx")))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                "SIANRPT",
                                                "mostrarMailPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx?m=Y&nota=" + this.lblNota.Text + ".xml')",
                                                true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                "SIANRPT",
                                                "mostrarMailPopUp('factura_impresion.aspx?m=Y&nota=" + this.lblNota.Text + ".xml')",
                                                true);
        }
    }

    protected void btnXML_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarPopUp('factura_xml.aspx?fact=" +
                                   this.lblNota.Text + ".xml')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void chkIVARet_CheckedChanged(object sender, EventArgs e)
    {
        this.hdTAX.Value = "0";

        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    protected void chkISRRet_CheckedChanged(object sender, EventArgs e)
    {
        this.hdTAX.Value = "0";

        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    private void Mostrar_Pagos(string strNotaID)
    {
        this.lblPagos.Text = "Pagos de la nota " + strNotaID;
        this.gvPagos.DataSource = ObtenerPagos(strNotaID);
        this.gvPagos.DataBind();
        this.mdCambiarPagos.Show();
    }

    private DataTable ObtenerPagos(string strNotaID)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("tipo", typeof(string)));
        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("notaID", typeof(string)));

        string strQuery = "SELECT T.tipo_pago, P.referencia, P.fecha_pago, F.monto_pago, S.moneda" +
                         ",F.notaID" +
                         " FROM pago_notas_cargo F" +
                         " INNER JOIN pagos P" +
                         " ON P.ID = F.pagoID" +
                         " AND F.notaID = " + strNotaID +
                         " AND F.aplicado = 1" +
                         " INNER JOIN notas_cargo S" +
                         " ON S.ID = F.notaID" +
                         " INNER JOIN tipos_pagos T" +
                         " ON T.ID = P.tipo_pago" +
                         " ORDER BY P.fecha_pago";
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
            dr[0] = objRowResult["tipo_pago"].ToString();
            dr[1] = objRowResult["referencia"].ToString();
            dr[2] = CRutinas.Fecha_DD_MMM_YYYY((DateTime)objRowResult["fecha_pago"]);
            dr[3] = ((decimal)objRowResult["monto_pago"]).ToString("c") + " " + objRowResult["moneda"].ToString();
            dr[4] = objRowResult["notaID"].ToString();
            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected void btnPagoContinuar_Click(object sender, EventArgs e)
    {
        if (!Timbrar_Nota())
            return;

        DateTime dtFecha = DateTime.Parse(this.txtFechaPago.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        string strCuentaBancaria = "0";

        if (this.dlCuentaBancaria.Items.Count > 0)
        {
            string[] strBancoIDs = this.dlCuentaBancaria.SelectedValue.Split('_');
            strCuentaBancaria = strBancoIDs[0];
        }

        string strQuery = "INSERT INTO pagos (tipo_pago, fecha_pago, referencia, " +
                         "monto_pago, moneda, tipo_cambio, cuenta_bancariaID" +
                         ", creadoPorID, creadoPorFecha, appID) VALUES(" +
                         "'" + this.dlTiposPagos.SelectedValue + "'" +
                         ",'" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         ",'" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                         ",'" + this.txtPago.Text + "'" +
                         ",'" + this.dlMoneda.SelectedValue + "'" +
                         ",'" + this.txtTipoCambio.Text + "'" +
                         "," + strCuentaBancaria +
                         ",'" + Session["SIANID"].ToString() + "'" +
                         ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
				         ", " + Session["SIANAppID"] +
                         ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        DataSet objDataResult = new DataSet();
        strQuery = "SELECT ID" +
                   " FROM pagos " +
                   " WHERE fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   " AND monto_pago = " + this.txtPago.Text;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        strQuery = "INSERT INTO pago_notas_cargo (pagoID, notaID, monto_pago) VALUES(" +
                   "'" + objDataResult.Tables[0].Rows[0]["ID"].ToString() + "'" +
                   ",'" + this.hdID.Value + "'" +
                   ",'" + this.hdPago.Value + "'" +
                   ")";

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        this.lnkPagos.Text = decimal.Parse(this.hdTotal.Value).ToString("c");

        strQuery = "UPDATE notas_cargo SET " +
                  " fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd") + "'" +
                  ",modificadoPorID = " + Session["SIANID"].ToString() +
                  ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                  " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        strQuery = "SELECT establecimiento_ID as estabID, fecha_ultimo_pago " +
                  " FROM sucursales S" +
                  " INNER JOIN establecimientos E" +
                  " ON S.establecimiento_ID = E.ID" +
                  "    AND S.ID = " + this.hdSucursalID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows[0].IsNull("fecha_ultimo_pago"))
        {
            strQuery = "UPDATE establecimientos SET" +
                      " fecha_ultimo_pago = '" + dtFecha.ToString("yyyy-MM-dd") + "'" +
                      " WHERE ID = " + objDataResult.Tables[0].Rows[0]["estabID"].ToString();
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        else
            if (((DateTime)objDataResult.Tables[0].Rows[0]["fecha_ultimo_pago"]) < dtFecha)
            {
                strQuery = "UPDATE establecimientos SET" +
                          " fecha_ultimo_pago = '" + dtFecha.ToString("yyyy-MM-dd") + "'" +
                          " WHERE ID = " + objDataResult.Tables[0].Rows[0]["estabID"].ToString();
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
    }

    protected void btnPosponer_Click(object sender, EventArgs e)
    {
        if (!Timbrar_Nota())
            return;

        // Se pone como No pagada
        string strQuery = "UPDATE notas_cargo SET " +
                    "status = 7" +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.dlEstatus.ClearSelection();
        this.dlEstatus.Items.FindByValue("7").Selected = true;
        Obtener_CreadoPor();
    }

    protected void lnkPagos_Click(object sender, EventArgs e)
    {
        Mostrar_Pagos(this.hdID.Value);
    }

    protected void btnUP_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((ImageButton)sender).CommandArgument))
            return;

        if (!((ImageButton)sender).CommandArgument.Equals(this.hdConsMin.Value))
        {
            Mover(((ImageButton)sender).CommandArgument, true);
        }
    }

    protected void btnMV_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((ImageButton)sender).CommandArgument))
            return;

        this.hdPos.Value = ((ImageButton)sender).CommandArgument;
        this.lblProdPos.Text = "Posición actual: " + this.hdPos.Value;
        this.txtPosicion.Text = string.Empty;

        this.mdMV.Show();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", "setTimeout('setPos()',50);", true);
    }

    protected void btnMVContinuar_Click(object sender, EventArgs e)
    {
        int intNvaPos;

        //Si no es numérico no mueve
        if (!int.TryParse(this.txtPosicion.Text, out intNvaPos))
            return;

        //Si es menor o igual a cero no mueve
        if (intNvaPos <= 0)
            return;

        if (intNvaPos > int.Parse(this.hdConsMax.Value))
            intNvaPos = int.Parse(this.hdConsMax.Value);

        //Si es la misma posición no se mueve
        if (intNvaPos == int.Parse(this.hdPos.Value))
            return;

        int intTemp = int.Parse(this.hdPos.Value);
        if (intNvaPos < intTemp)    //Se mueve hacia arriba
        {
            while (intTemp != intNvaPos)
            {
                intTemp = Mover(intTemp.ToString(), false);
            }
        }
        else                        //Se mueve hacia abajo
        {
            do
            {
                intTemp += 1;
                Mover(intTemp.ToString(), false);
            } while (intTemp != intNvaPos);
        }

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();
    }

    private int Mover(string btnUPID, bool swRefrescar)
    {
        int intAntInicio = int.Parse(btnUPID) - 1;

        //Mueve el producto de abajo temporalmente
        string strQuery = "UPDATE notas_cargo_prod SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND nota_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE notas_cargo_prod SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND nota_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE notas_cargo_prod SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND nota_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (swRefrescar)
        {
            this.gvProductos.DataSource = ObtenerProductos();
            this.gvProductos.DataBind();
        }

        return intAntInicio;
    }

    protected void btnDN_Click(object sender, EventArgs e)
    {
        ImageButton btnDN = (ImageButton)sender;

        if (!btnDN.CommandArgument.Equals(this.hdConsMax.Value))
            Mover((int.Parse(btnDN.CommandArgument) + 1).ToString(), true);
    }

    private void Obtener_CreadoPor()
    {
        this.lblCreado.Text = string.Empty;
        string strQuery = "SELECT U.usuario, F.creadoPorFecha" +
                         " FROM notas_cargo F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.creadoPorID" +
                         " AND F.ID = " + this.hdID.Value + ";" +
                         "SELECT U.usuario, F.modificadoPorFecha" +
                         " FROM notas_cargo F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.modificadoPorID" +
                         " AND F.ID = " + this.hdID.Value;
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            this.lblCreado.Text = "Creación: " + objDataResult.Tables[0].Rows[0]["usuario"].ToString() +
                                  "(" + CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objDataResult.Tables[0].Rows[0]["creadoPorFecha"]) +
                                  ")";
        }

        if (objDataResult.Tables[1].Rows.Count > 0)
        {
            this.lblCreado.Text += " Última modificación: " + objDataResult.Tables[1].Rows[0]["usuario"].ToString() +
                                  "(" + CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objDataResult.Tables[1].Rows[0]["modificadoPorFecha"]) +
                                  ")";
        }
    }
	
	protected void btnTAXContinuar_Click(object sender, EventArgs e)
    {
        this.hdTAX.Value = "1";

        this.hdIVA.Value = this.txtIVA.Text;
        this.hdIVARet.Value = this.txtIVARet.Text;
        this.hdISRRet.Value = this.txtISRRet.Text;

        Llenar_Productos(true);
    }

    protected void btnTAX_Click(object sender, EventArgs e)
    {
        if (this.hdCostoDescuento.Value.Equals("0"))
            this.txtSubtotal.Text = this.hdCosto.Value;
        else
            this.txtSubtotal.Text = this.hdCostoDescuento.Value;
        this.txtIVA.Text = this.hdIVA.Value;
        this.txtIVARet.Text = this.hdIVARet.Value;
        this.txtISRRet.Text = this.hdISRRet.Value;
        this.txtTotal.Text = this.hdTotal.Value;

        if (this.chkIVARet.Checked)
            this.txtIVARet.Enabled = true;
        else
            this.txtIVARet.Enabled = false;

        if (this.chkISRRet.Checked)
            this.txtISRRet.Enabled = true;
        else
            this.txtISRRet.Enabled = false;

        this.mdTAX.Show();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                "setTimeout('setFoco(\"" + this.txtIVA.ClientID + "\")',50);",
                                                true);
    }

    protected void dlMoneda_SelectedIndexChanged(object sender, EventArgs e)
    {
        Recalcular_Productos();
    }

    private void Recalcular_Productos()
    {
        decimal dcmTipoCambio = decimal.Parse(this.txtTipoCambio.Text);

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT *" +
                                                            " FROM notas_cargo_prod" +
                                                            " WHERE nota_ID = " + this.hdID.Value);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {

            decimal dcmPrecio, dcmCantidad;
            foreach (DataRow dtRow in objDataResult.Tables[0].Rows)
            {
                dcmCantidad = (decimal)dtRow["cantidad"];
                dcmPrecio = (decimal)dtRow["costo_original"];
                if (!dtRow["costo_original_moneda"].ToString().Equals(this.dlMoneda.SelectedValue))
                {
                    if (this.dlMoneda.SelectedValue.Equals("MXN"))
                        dcmPrecio = Math.Round(dcmTipoCambio * dcmPrecio, 2);
                    else
                        dcmPrecio = Math.Round(dcmPrecio / dcmTipoCambio, 2);
                }

                CComunDB.CCommun.Ejecutar_SP("UPDATE notas_cargo_prod SET" +
                                            " costo_unitario = " + dcmPrecio +
                                            ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                                            " WHERE nota_ID = " + this.hdID.Value +
                                            " AND producto_ID = " + dtRow["producto_ID"] +
                                            " AND consecutivo = " + dtRow["consecutivo"]);
            }

            Llenar_Productos(true);
        }
    }

    private bool Obtener_Tipo_Cambio_Dia()
    {
        decimal dcmTipoCambio = CRutinas.Obtener_TipoCambio();

        if (dcmTipoCambio == 0)
            return false;

        this.txtTipoCambio.Text = dcmTipoCambio.ToString("0.00##");

        return true;
    }

    private void Obtener_Moneda()
    {
        this.lblMoneda.Text =
            this.lblPrecioCambiarMoneda.Text =
            this.hdMoneda.Value = CComunDB.CCommun.ObtenerMonedaListasPrecios(this.dlListaPrecios.SelectedValue);
    }

    protected void dlListaPrecios_SelectedIndexChanged(object sender, EventArgs e)
    {
        Obtener_Moneda();
    }

    protected void btnRefrescarTipoCambio_Command(object sender, CommandEventArgs e)
    {
        if (Obtener_Tipo_Cambio_Dia())
        {
            Recalcular_Productos();
        }
    }

    protected void dlMonedaPago_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.dlMoneda.SelectedValue.Equals(this.dlMonedaPago.SelectedValue))
        {
            decimal dcmTipoCambio = decimal.Parse(this.txtTipoCambio.Text);
            decimal dcmMontoPago = decimal.Parse(this.txtPago.Text);

            if (this.dlMonedaPago.SelectedValue.Equals("MXN"))
                dcmMontoPago = Math.Round(dcmTipoCambio * dcmMontoPago, 2);
            else
                dcmMontoPago = Math.Round(dcmMontoPago / dcmTipoCambio, 2);

            this.txtPago.Text = dcmMontoPago.ToString("0.00");
        }
        else
            this.txtPago.Text = this.hdPago.Value;

        this.mdPago.Show();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                   "setTimeout('setFoco(\"" + this.txtPago.ClientID + "\")',50);",
                                                   true);
    }
}
