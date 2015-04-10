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

public partial class facturas_punto_venta : BasePage
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

        this.txtCantidad.Attributes["onkeypress"] =
            this.txtCambiarCantidad.Attributes["onkeypress"] =
            this.txtPrecioUnitario.Attributes["onkeypress"] =
            this.txtCambiarUnitario.Attributes["onkeypress"] =
            this.txtPrecioUnitarioCambio.Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";

        this.acProducto.ContextKey = Session["SIANAppID"].ToString();

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

            if (!CComunDB.CCommun.ValidarAcceso(6115, out swVer, out swTot))
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

            ListItem liIva0 = new ListItem("Sin IVA", "0.00");
            decimal intIva = Math.Round(Convert.ToDecimal(ckSIAN["ck_iva2"]) * 100, 2);
            ListItem liIva1 = new ListItem("Con IVA", intIva.ToString("0.00"));
            this.rdIVA.Items.Add(liIva0);
            this.rdIVA.Items.Add(liIva1);
            this.rdIVA.Items[1].Selected = true;

            this.rdIVA.Visible = false;

            this.txtFecha.Text = DateTime.Today.ToString("dd/MM/yyyy");

            this.txtNota.Enabled = false;
            this.txtNota_Suf.Enabled = false;

            Llenar_Catalogos();

            Llenar_Grid();

            this.hdInventarios.Value = CComunDB.CCommun.Obtener_Valor_CatParametros(20);
            this.hdInvAlmacen.Value = CComunDB.CCommun.Obtener_Valor_CatParametros(21);

            string strQuery = "SELECT E.lista_precios_ID, P.nombre_lista " +
                             " FROM establecimientos E " +
                             " INNER JOIN listas_precios P " +
                             " ON E.lista_precios_ID = P.ID " +
                             " AND E.ID = 0";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            this.hdVentaLista.Value = objDataResult.Tables[0].Rows[0]["lista_precios_ID"].ToString();
            this.hdVentaListaNombre.Value = objDataResult.Tables[0].Rows[0]["nombre_lista"].ToString();
            this.hdVentaVendedor.Value = CComunDB.CCommun.Obtener_Valor_CatParametros(22);

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

        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerNotaEstatus(false);
        this.dlEstatus.DataBind();

        this.dlCuentaBancaria.DataSource = CComunDB.CCommun.ObtenerCtasBancarias_CtaContable(false);
        this.dlCuentaBancaria.DataBind();

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
        this.grdvLista.DataSource = ObtenerNotas();
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

        string strQuery = "CALL leer_notas_consulta(" +
            Session["SIANAppID"] +
            ", " + ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", 1" +
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
            dr[5] = ((decimal)objRowResult["monto"]).ToString("c") + " " + objRowResult["moneda"].ToString();
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

        string strQuery = "SELECT * FROM (" +
            "SELECT C.producto_ID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
            ", C.exento as exento " +
            ", C.costo_original " +
            ", C.costo_original_moneda " +
            " FROM notas_prod C " +
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

    private DataTable ObtenerLotes(string strProdID)
    {
        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("loteID", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));

        DataSet objDataResult = CInventarios.Obtener_Existencia_Lotes(strProdID, CComunDB.CCommun.ObtenerAlmacenPrincipal());

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            if (objRowResult.IsNull("fecha_caducidad"))
                dr[0] = objRowResult["lote"].ToString() + "_null";
            else
                dr[0] = objRowResult["lote"].ToString() + "_'" +
                        ((DateTime)objRowResult["fecha_caducidad"]).ToString("yyyy-MM-dd") +
                        "'";
            string strLote = "S/N";
            if (!string.IsNullOrEmpty(objRowResult["lote"].ToString()))
                strLote = objRowResult["lote"].ToString();
            dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0.##") + ")";
            dr[2] = ((decimal)objRowResult["existencia"]).ToString("0.##");
            dt.Rows.Add(dr);
        }
        return dt;
    }

    private DataTable ObtenerLotesModificar(string strID, string strConsecutivo)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("loteID", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad_inv", typeof(string)));

        string strQuery = "SELECT I.existencia, I.lote, I.fecha_caducidad, L.cantidad" +
                          " FROM inventario I " +
                          " LEFT JOIN facturas_prod_lote L" +
                          " ON I.producto_ID = L.producto_ID" +
                          "   AND I.lote = L.lote" +
                          "   AND L.consecutivo = " + strConsecutivo +
                          "   AND L.factura_ID = " + this.hdID.Value +
                          " WHERE I.producto_ID = " + strID +
                          "   AND I.subalmacen_ID = " + CComunDB.CCommun.ObtenerAlmacenPrincipal() +
                          " ORDER BY I.existencia desc, fecha_caducidad, lote";
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
            if (objRowResult.IsNull("fecha_caducidad"))
                dr[0] = objRowResult["lote"].ToString() + "_null";
            else
                dr[0] = objRowResult["lote"].ToString() + "_'" +
                        ((DateTime)objRowResult["fecha_caducidad"]).ToString("yyyy-MM-dd") +
                        "'";
            string strLote = "S/N";
            if (!string.IsNullOrEmpty(objRowResult["lote"].ToString()))
                strLote = objRowResult["lote"].ToString();
            dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0.##") + ")";
            if (objRowResult.IsNull("cantidad"))
                dr[2] = "0";
            else
                dr[2] = ((decimal)objRowResult["cantidad"]).ToString("0.##");
            dr[3] = ((decimal)objRowResult["existencia"]).ToString("0.##");
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
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos();
            if (!this.hdAT.Value.Equals("1"))
            {
                this.btnModificar.Visible = false;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.gvProductos.Enabled = false;
                //this.btnFacturar.Visible = false;
                this.btnFinalizar.Visible = false;
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
        if (this.hdID.Value.Equals("0") && CRutinas.Obtener_TipoCambio() == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No se ha definido el tipo del cambio del día actual, es necesario que se haga esto antes de poder continuar");
            return;
        }

        //this.btnFacturar.Visible = false;
        this.lblMensaje.Text = string.Empty;
        this.lblCreado.Text = string.Empty;
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lnkPagos.Text = "$0.00";

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
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.txtNota.Text = string.Empty;
            this.txtNota_Suf.Text = string.Empty;
            this.btnModificar.Visible = true;
            this.btnCancelar.Visible = false;
            this.btnImprimir.Visible = false;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = false;
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue("8").Selected = true;
            this.hdCosto.Value = "0";
            this.hdCostoIVA.Value = "0";
            this.hdIVA.Value = "0";
            this.hdTotal.Value = "0";
            this.rdIVA.ClearSelection();
            this.rdIVA.SelectedIndex = 1;

            string strQuery = "SELECT S.ID, concat(negocio, ' - ', sucursal) as nombre " +
                    " FROM sucursales S " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " AND E.ID = 0";
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            this.txtSucursal.Text = objDataResult.Tables[0].Rows[0]["nombre"].ToString();
            this.hdSucursalID.Value = objDataResult.Tables[0].Rows[0]["ID"].ToString();

            this.txtSucursal.Enabled = false;

            this.gvProductos.DataSource = null;
            this.gvProductos.DataBind();
            this.btnFinalizar.Visible = false;
            this.txtProducto.Focus();

            this.pnlDatos2.Visible = false;
            this.btnModificar.ImageUrl = "~/imagenes/salida.png";
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT sucursal_ID, concat(negocio, ' - ', sucursal) as nombre, " +
                    " nota, nota_suf, fecha, F.status, F.contado, comentarios, " +
                    " F.descuento, F.descuento2, F.iva, F.lista_precios_ID, F.vendedorID, " +
                    " C.razon as motivo_cancelacion " +
                    " FROM notas F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " LEFT JOIN notas_cancelacion C " +
                    " ON F.ID = C.nota_ID " +
                    " WHERE F.ID = " + this.hdID.Value + ";" +
                    " SELECT factura_ID " +
                    " FROM nota_facturas " +
                    " WHERE nota_ID = " + this.hdID.Value +
                    " ORDER BY factura_ID;" +
                    " SELECT orden_compraID" +
                    " FROM orden_compra_nota" +
                    " WHERE notaID = " + this.hdID.Value +
                    " ORDER BY orden_compraID";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtNota.Text = objRowResult["nota"].ToString();
            this.txtNota_Suf.Text = objRowResult["nota_suf"].ToString();

            this.txtFecha.Text = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy");

            this.txtSucursal.Text = objRowResult["nombre"].ToString();
            this.hdSucursalID.Value = objRowResult["sucursal_ID"].ToString();

            this.txtComentarios.Text = objRowResult["comentarios"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["status"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["iva"]).ToString("0.00")).Selected = true;

            if (!objRowResult["status"].ToString().Equals("8"))
            {
                this.btnFinalizar.Visible = false;
                Estado_Campos(false);
                this.hdBorrar.Value = "0";
                this.txtProducto.Enabled = false;
                this.txtCantidad.Enabled = false;
                this.txtPrecioUnitario.Enabled = false;
                this.btnModificar.Visible = false;
                this.btnCancelar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnAgregarProd.Visible = false;
                switch (objRowResult["status"].ToString())
                {
                    case "1":   //Contrarecibo
                    case "2":   //A Cobrar
                        this.lblMensaje.Text = "Nota en cobranza";
                        break;
                    case "0":   //Pagada
                    case "3":   //A Facturar
                        //this.btnFacturar.Visible = true;
                        break;
                    case "4":   //Facturada
                        this.btnCancelar.Visible = false;
                        //this.btnFacturar.Visible = false;
                        StringBuilder strFacturas = new StringBuilder();
                        foreach (DataRow objRowResult2 in objDataResult.Tables[1].Rows)
                        {
                            if (strFacturas.Length > 0)
                                strFacturas.Append(", ");
                            strFacturas.Append(objRowResult2[0].ToString());
                        }
                        this.lblMensaje.Text = "Nota facturada, factura(s): " + strFacturas;
                        this.lnkPagos.Text = string.Empty;
                        break;
                    case "9":   //Cancelada
                        this.btnCancelar.Visible = false;
                        this.lblMensaje.Text = "NOTA CANCELADA: " + objRowResult["motivo_cancelacion"].ToString();
                        break;
                }
            }
            else
            {
                this.btnFinalizar.Visible = true;
                Estado_Campos(true);
                this.hdBorrar.Value = "1";
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                if (this.hdUsuPr.Value.Equals("0"))
                    this.txtPrecioUnitario.Enabled = false;
                else
                    this.txtPrecioUnitario.Enabled = true;
                this.btnModificar.Visible = true;
                this.btnCancelar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnAgregarProd.Visible = true;
                this.btnAgregarProd.Enabled = true;
            }

            if (objDataResult.Tables[2].Rows.Count > 0)
            {
                StringBuilder strOrdenes = new StringBuilder("Pedido(s): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[2].Rows)
                {
                    if (strOrdenes.Length != 11)
                        strOrdenes.Append(", ");
                    strOrdenes.Append(objRowResult2[0].ToString());
                }
                this.lblMensaje.Text += " " + strOrdenes.ToString();
            }

            strQuery = "SELECT SUM(F.monto_pago) AS monto_pago" +
                       " FROM pago_notas F" +
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

            this.txtSucursal.Enabled = false;

            Llenar_Productos(false);
            Obtener_CreadoPor();
        }

        Obtener_Moneda();
    }

    private void Estado_Campos(bool sw_estado)
    {
        this.txtSucursal.Enabled = sw_estado;
        this.txtComentarios.Enabled = sw_estado;
        this.rdIVA.Enabled = sw_estado;
    }

    private void Agregar_Nota()
    {
        if (string.IsNullOrEmpty(this.hdSucursalID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return;
        }

        if (Crear_Nota(this.txtNota.Text.Trim(),
                       this.txtNota_Suf.Text.Trim(),
                       int.Parse(this.hdSucursalID.Value),
                       DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")),
                       decimal.Parse(this.rdIVA.SelectedValue),
                       true,
                       0,
                       0,
                       this.txtComentarios.Text.Trim(),
                       int.Parse(this.hdVentaLista.Value),
                       int.Parse(this.hdVentaVendedor.Value),
                       "MXN",
                       1))
        {
            //((master_MasterPage)Page.Master).MostrarMensajeError("La nota ha sido creada, folio interno: " + this.hdID.Value);
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.btnModificar.Visible = true;
            this.btnCancelar.Visible = true;
            this.btnImprimir.Visible = true;
            this.btnAgregarProd.Enabled = true;
            this.btnFinalizar.Visible = true;
            Obtener_CreadoPor();

            this.pnlDatos2.Visible = true;
            this.btnModificar.ImageUrl = "~/imagenes/modificar.png";
        }
    }

    private bool Crear_Nota(string strNota, string strNota_Suf,
                            int strSucursalID, DateTime dtFecha,
                            decimal dcmIVA, bool esContado,
                            decimal dcmDescuento1, decimal dcmDescuento2,
                            string strComentarios,
                            int intListaPreciosID,
                            int intVendedorID,
                            string strMoneda, decimal dcmTipo_Cambio)
    {
        DataSet objDataResult = new DataSet();
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        CNotaDB objNota = new CNotaDB();

        objNota.nota = strNota;
        objNota.nota_suf = strNota_Suf;
        objNota.fecha = dtFecha;
        objNota.sucursal_ID = strSucursalID;
        objNota.iva = dcmIVA;
        objNota.descuento = dcmDescuento1;
        objNota.descuento2 = dcmDescuento2;
        objNota.comentarios = strComentarios;
        objNota.status = 8;
        objNota.contado = esContado;
        objNota.lista_precios_ID = intListaPreciosID;
        objNota.vendedorID = intVendedorID;
        objNota.isr_ret = decimal.Parse(this.hdPorcISRRet.Value);
        objNota.iva_ret = decimal.Parse(this.hdPorcIVARet.Value);
        objNota.moneda = strMoneda;
        objNota.tipo_cambio = CRutinas.Obtener_TipoCambio();

        if (objNota.Guardar())
        {
            this.hdID.Value = objNota.ID.ToString();
            this.txtNota.Text = objNota.ID.ToString();
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objNota.status.ToString()).Selected = true;
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
             Agregar_Nota();
        else
            if (Actualizar_Nota())
                ((master_MasterPage)Page.Master).MostrarMensajeError("La nota ha sido modificada");
    }

    protected void btnVerificacionContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if (DateTime.Parse(strValores[1]) >= DateTime.Now)
                strCodigo = strValores[0];
        }

        if (string.IsNullOrEmpty(this.txtCodigo_Verificacion.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigo_Verificacion.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
        }
        Agregar_Nota();
    }

    private bool Actualizar_Nota()
    {
        DateTime dtFecha = DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        decimal dcmDescuento1 = 0, dcmDescuento2 = 0;

        CNotaDB objNota = new CNotaDB();

        objNota.Leer(int.Parse(this.hdID.Value));

        objNota.fecha = dtFecha;
        objNota.sucursal_ID = int.Parse(this.hdSucursalID.Value);
        objNota.iva = decimal.Parse(this.rdIVA.SelectedValue);
        objNota.descuento = dcmDescuento1;
        objNota.descuento2 = dcmDescuento2;
        objNota.contado = true;
        objNota.lista_precios_ID = int.Parse(this.hdVentaLista.Value);
        objNota.vendedorID = int.Parse(this.hdVentaVendedor.Value);
        objNota.comentarios = this.txtComentarios.Text.Trim();
        objNota.moneda = "MXN";
        objNota.tipo_cambio = CRutinas.Obtener_TipoCambio();

        if (objNota.Guardar())
        {
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objNota.status.ToString()).Selected = true;
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

            string strMensaje = CComunDB.CCommun.Validar_Ultimo_Precio_Compra(int.Parse(this.hdProductoID.Value), dcmPrecioUnitario, this.hdMoneda.Value, CRutinas.Obtener_TipoCambio());
            if (!string.IsNullOrEmpty(strMensaje))
            {
                this.hdVentaAccion.Value = "1";
                this.lblProdVenta.Text = strMensaje;
                this.mdProdVenta.Show();
                return;
            }
            Continuar_Agregar_Producto();
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad y precio unitario deben ser numéricos");
    }

    protected void btnProdVentaContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if (DateTime.Parse(strValores[1]) >= DateTime.Now)
                strCodigo = strValores[0];
        }

        if (string.IsNullOrEmpty(this.txtCodigoProdVenta.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigoProdVenta.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
        }

        if (this.hdVentaAccion.Value.Equals("1"))
            Continuar_Agregar_Producto();
        else
            Continuar_Cambiar_Producto();

        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private void Continuar_Agregar_Producto()
    {
        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Nota();
            if (this.hdID.Value.Equals("0"))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al generar la venta");
                return;
            }
        }

        decimal dcmCantidad = decimal.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());
        string strMensaje = string.Empty;

        this.lblPrecioProducto.Text = this.txtProducto.Text;
        if (this.lblPrecioProducto.Text.Length > 25)
            this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 25);
        this.hdProductoPrecioID.Value = this.hdProductoID.Value;
        if (Validar_Lote())
        {
            if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                  dcmCantidad, Math.Round(dcmPrecioUnitario, 2),
                                  this.hdVentaLista.Value,
                                  true,
                                  out strMensaje))
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
            else
                if (this.chkCambiarPrecios.Checked)
                {
                    decimal dcmPrecioUnitarioOrig = 0;
                    decimal.TryParse(this.hdPrecioUnitario.Value, out dcmPrecioUnitarioOrig);
                    if (dcmPrecioUnitario != dcmPrecioUnitarioOrig)
                    {
                        this.lblPrecioLista.Text = this.hdVentaListaNombre.Value;
                        this.lblPrecioAnterior.Text = dcmPrecioUnitarioOrig.ToString("c");
                        this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                        this.txtPrecioUnitarioCambio.Focus();
                        this.mdCambiarPrecio.Show();
                        return;
                    }
                }
        }
        else
            return;

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
                             "    AND lista_precios_ID = " + this.hdVentaLista.Value +
                             "    AND validez = '2099-12-31'" +
                             "    WHERE codigo = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo2 = '" + this.txtProducto.Text.Trim() + "'" +
                             "       OR codigo3 = '" + this.txtProducto.Text.Trim() + "'" +
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

    private bool Validar_Lote()
    {
        this.gvLote.DataSource = ObtenerLotes(this.hdProductoID.Value);
        this.gvLote.DataBind();

        if (gvLote.Rows.Count > 0)
        {
            if (gvLote.Rows.Count == 1)
            {
                if (this.hdInventarios.Value.Equals("1"))
                {
                    if (decimal.Parse(((HiddenField)gvLote.Rows[0].FindControl("hdCantidadLote")).Value) >=
                        decimal.Parse(this.txtCantidad.Text.Trim()))
                    {
                        ((TextBox)gvLote.Rows[0].FindControl("txtCantidadLote")).Text = this.txtCantidad.Text.Trim();
                        return true;
                    }
                }
                else
                {
                    ((TextBox)gvLote.Rows[0].FindControl("txtCantidadLote")).Text = this.txtCantidad.Text.Trim();
                    return true;
                }
            }
            this.btnLoteContinuar.Enabled = false;
            bool swPrimero = true;
            foreach (GridViewRow gvRow in this.gvLote.Rows)
            {
                ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
                ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeyup"] = "javascript:return validateLote(this);";
                ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onblur"] = "javascript:return validateLote(this);";
                ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onfocus"] = "this.select();";
                if (swPrimero)
                {
                    ((TextBox)gvRow.FindControl("txtCantidadLote")).Focus();
                    swPrimero = false;
                }
            }
            this.mdSeleccionarLote.Show();
            return false;
        }
        else
        {
            if (this.hdInventarios.Value.Equals("1"))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("No hay inventario de este producto");
                return false;
            }
            else
                return true;
        }
    }

    protected void btnLoteContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad = decimal.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());
        string strMensaje = string.Empty;

        if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                      dcmCantidad, Math.Round(dcmPrecioUnitario, 2),
                                      this.hdVentaLista.Value,
                                      true,
                                      out strMensaje))
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
        else
            if (this.chkCambiarPrecios.Checked)
            {
                decimal dcmPrecioUnitarioOrig = 0;
                decimal.TryParse(this.hdPrecioUnitario.Value, out dcmPrecioUnitarioOrig);
                if (dcmPrecioUnitario != dcmPrecioUnitarioOrig)
                {
                    this.lblPrecioLista.Text = this.hdVentaListaNombre.Value;
                    this.lblPrecioAnterior.Text = dcmPrecioUnitarioOrig.ToString("c");
                    this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                    this.txtPrecioUnitarioCambio.Focus();
                    this.mdCambiarPrecio.Show();
                    return;
                }
            }
        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private bool Agregar_Producto(string strProductoID, string strProducto,
                                decimal dcmCantidad, decimal dcmCosto_unitario,
                                string listaPrecios, bool llenarProds,
                                out string strMensaje)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        decimal dcmCosto_Original = dcmCosto_unitario;

        //if (!this.hdMoneda.Value.Equals(this.dlMoneda.SelectedValue))
        //{
        //    // A pesos
        //    if (this.dlMoneda.SelectedValue.Equals("MXN"))
        //        dcmCosto_unitario = Math.Round(decimal.Parse(this.txtTipoCambio.Text) * dcmCosto_unitario, 2);
        //    else
        //        dcmCosto_unitario = Math.Round(dcmCosto_unitario / decimal.Parse(this.txtTipoCambio.Text), 2);
        //}

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
                    " FROM notas_prod " +
                    " WHERE nota_ID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO notas_prod (nota_ID, " +
                "producto_ID, exento, cantidad, costo_unitario" +
                ",costo_original, costo_original_moneda" +
                ",costo, consecutivo) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + dcmCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto_Original.ToString() + "'" +
                ", '" + this.hdMoneda.Value + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
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

        if (this.hdInventarios.Value.Equals("1"))
        {
            foreach (GridViewRow gvRow in this.gvLote.Rows)
            {
                if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim()) > 0)
                {
                    Agregar_Lote_Inventariar(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString(),
                                             strProductoID,
                                             this.gvLote.DataKeys[gvRow.RowIndex].Value.ToString(),
                                             ((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim());
                }
            }
        }

        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        if (llenarProds)
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
                decimal dcmPorcDescuento1 = 0, dcmPorcDescuento2 = 0;
                decimal dcmCosto, dcmCostoDescuento, dcmCostoIVA;
                decimal dcmIVA, dcmSubtotal;
                decimal dcmISRRet, dcmIVARet, dcmTotal;
                decimal.TryParse(this.hdCosto.Value, out dcmCosto);
                decimal.TryParse(this.hdCostoIVA.Value, out dcmCostoIVA);

                //Subtotal antes de descuento, es la suma de todos los productos
                dcmCosto = Math.Round(dcmCosto, 2);
                strLeyenda.Append("Subtotal:");
                strValores.Append(dcmCosto.ToString("c") + " MXN");

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
                    strValores.Append("<br />" + dcmCostoDescuento.ToString("c"));
                }
                else
                    this.hdCostoDescuento.Value = "0";

                // IVA
                dcmIVA = Math.Round(dcmCostoIVA * decimal.Parse(this.rdIVA.SelectedValue) / 100, 2);
                this.hdIVA.Value = dcmIVA.ToString();
                strLeyenda.Append("<br />IVA " + this.rdIVA.SelectedValue + "%:");
                strValores.Append("<br />" + dcmIVA.ToString("c") + " MXN");

                this.hdSubtotal.Value = "0";
                this.hdISRRet.Value = "0";
                this.hdIVARet.Value = "0";

                dcmTotal = Math.Round(dcmCostoDescuento + dcmIVA, 2);
                this.hdTotal.Value = dcmTotal.ToString();
                strLeyenda.Append("<br />TOTAL:");
                strValores.Append("<br />" + dcmTotal.ToString("c") + " MXN");

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
                        " FROM notas_prod " +
                        " WHERE nota_ID = " + this.hdID.Value +
                        " AND producto_ID = " + strID_Consecutivo[0] +
                        " AND consecutivo = " + strID_Consecutivo[1] + ";" +
                        "DELETE " +
                        " FROM notas_prod_lote " +
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

                strQuery = "UPDATE notas_prod SET " +
                          " consecutivo = consecutivo - 1 " +
                          " WHERE nota_ID = " + this.hdID.Value +
                          " AND consecutivo > " + strID_Consecutivo[1] + ";" +
                          "UPDATE notas_prod_lote SET " +
                          " consecutivo = consecutivo - 1 " +
                          " WHERE nota_ID = " + this.hdID.Value +
                          " AND consecutivo > " + strID_Consecutivo[1] + ";";

                CComunDB.CCommun.Ejecutar_SP(strQuery);

                Llenar_Productos(true);
            }
            else
                if (e.CommandName == "Modificar")
                {
                    this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                    this.lblCambiarProducto.Text = this.gvProductos.Rows[index].Cells[2].Text;
                    this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[3].Text;
                    this.txtCambiarUnitario.Text = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginal")).Value;
                    this.lblMonedaCambiar.Text =
                        this.hdMonedaTemp.Value = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginalMoneda")).Value;

                    string strQuery = "SELECT P.minimo_compra " +
                                 " FROM productos P " +
                                 " WHERE ID = " + strID_Consecutivo[0];
                    DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                    if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
                        this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##");
                    else
                        this.hdMinimo.Value = string.Empty;

                    if (this.hdInventarios.Value.Equals("1"))
                    {
                        this.gvCambiarLote.DataSource = ObtenerLotesModificar(strID_Consecutivo[0], strID_Consecutivo[1]);
                        this.gvCambiarLote.DataBind();
                        foreach (GridViewRow gvRow in this.gvCambiarLote.Rows)
                        {
                            ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
                            ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeyup"] = "javascript:return validateCambiar(this);";
                            ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onblur"] = "javascript:return validateCambiar(this);";
                        }
                    }
                    this.mdCambiarProducto.Show();
                    string strClientScript = "setTimeout('setProductoCantidad()',100);";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                }
        }
    }

    private void Llenar_Productos(bool swGuardarNot)
    {
        this.hdCosto.Value = "0";
        this.hdCostoIVA.Value = "0";
        this.hdIVA.Value = "0";
        this.hdTotal.Value = "0";

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        CNotaDB objNota = new CNotaDB();

        objNota.Leer(int.Parse(this.hdID.Value));

        objNota.registrarMod = swGuardarNot;
        objNota.monto_subtotal = Math.Round(decimal.Parse(this.hdCosto.Value), 2);
        objNota.monto_descuento = Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2);
        objNota.monto_iva = Math.Round(decimal.Parse(this.hdIVA.Value), 2);
        objNota.monto_subtotal2 = Math.Round(decimal.Parse(this.hdSubtotal.Value), 2);
        objNota.monto_isr_ret = Math.Round(decimal.Parse(this.hdISRRet.Value), 2);
        objNota.monto_iva_ret = Math.Round(decimal.Parse(this.hdIVARet.Value), 2);
        objNota.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
        objNota.total_real = objNota.total;
        objNota.iva = decimal.Parse(this.rdIVA.SelectedValue);

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

        string strQuery = "UPDATE notas SET " +
                    "status = 9" +
                    ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        strQuery = "INSERT INTO notas_cancelacion (" +
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

        if (!this.dlEstatus.SelectedValue.Equals("8"))   //Si ya no está En Proceso
            Devolver_Inventario();

        //Recalcular_Productos_Datos();

        Mostrar_Datos();
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

            string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');

            string strMensaje = CComunDB.CCommun.Validar_Ultimo_Precio_Compra(int.Parse(strID_Consecutivo[0]), dcmPrecio, this.hdMoneda.Value, CRutinas.Obtener_TipoCambio());
            if (!string.IsNullOrEmpty(strMensaje))
            {
                this.hdVentaAccion.Value = "2";
                this.lblProdVenta.Text = strMensaje;
                this.mdProdVenta.Show();
                return;
            }
            Continuar_Cambiar_Producto();
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad/Precio no puede ser menor o igual a cero");
    }

    private void Continuar_Cambiar_Producto()
    {
        decimal dcmCantidad, dcmCantidadLotes;
        dcmCantidadLotes = 0;

        string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');

        if (this.hdInvAlmacen.Value.Equals("0") &&
            this.hdInventarios.Value.Equals("1"))
        {
            foreach (GridViewRow gvRow in this.gvCambiarLote.Rows)
            {
                decimal.TryParse(((TextBox)gvRow.FindControl("txtCantidadLote")).Text, out dcmCantidad);
            dcmCantidad = Math.Round(dcmCantidad, 2);

            if (dcmCantidad > decimal.Parse(((HiddenField)gvRow.FindControl("hdCantidadLote")).Value))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad ingresada del lote '" + 
                                                                    ((Label)gvRow.FindControl("lblLote")).Text +
                                                                     "' es mayor a la existente en inventario");
                return;
            }

                dcmCantidadLotes += dcmCantidad;
            }

            decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
            dcmCantidad = Math.Round(dcmCantidad, 2);

            if (dcmCantidadLotes != dcmCantidad)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad ingresada no corresponde a la cantidad que se va a usar de los lotes");
                return;
            }
        }
        else
        {
            decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
            dcmCantidad = Math.Round(dcmCantidad, 2);
        }

        decimal dcmPrecio = Math.Round(decimal.Parse(this.txtCambiarUnitario.Text), 2);

        decimal dcmCosto_Original = dcmPrecio;
        if (!this.hdMonedaTemp.Value.Equals(this.hdMoneda.Value))
        {
            // A pesos
            if (this.hdMoneda.Value.Equals("MXN"))
                dcmPrecio = Math.Round(CRutinas.Obtener_TipoCambio() * dcmPrecio, 2);
            else
                dcmPrecio = Math.Round(dcmPrecio / CRutinas.Obtener_TipoCambio(), 2);
        }

        string strQuery = "UPDATE notas_prod SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo_original = " + dcmCosto_Original.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
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

        if (this.hdInventarios.Value.Equals("1"))
        {
            strQuery = "DELETE FROM notas_prod_lote " +
                      " WHERE nota_ID = " + this.hdID.Value +
                      " AND producto_ID = " + strID_Consecutivo[0] +
                      " AND consecutivo = " + strID_Consecutivo[1];
            CComunDB.CCommun.Ejecutar_SP(strQuery);
            foreach (GridViewRow gvRow in this.gvCambiarLote.Rows)
            {
                if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim()) > 0)
                {
                    Agregar_Lote_Inventariar(strID_Consecutivo[1],
                                             strID_Consecutivo[0],
                                             this.gvCambiarLote.DataKeys[gvRow.RowIndex].Value.ToString(),
                                             ((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim());
                }
            }
        }
        Llenar_Productos(true);
    }

    private void Agregar_Lote_Inventariar(string strConsecutivo, string strProductoID, string strLote, string strCantidad)
    {
        if (string.IsNullOrEmpty(strLote))
            strLote = "_null";
        string[] strDatos = strLote.Split('_');

        if (!strDatos[1].Equals("null") && !strDatos[1].StartsWith("'"))
            strDatos[1] = "'" + strDatos[1] + "'";

        string strQuery = "INSERT INTO notas_prod_lote (consecutivo, nota_ID, " +
                "producto_ID, lote, fecha_caducidad, cantidad) VALUES (" +
                "'" + strConsecutivo + "'" +
                ", '" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strDatos[0] + "'" +
                ", " + strDatos[1] +
                ", '" + strCantidad + "'" +
                ")";
        CComunDB.CCommun.Ejecutar_SP(strQuery);
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

    protected void btnPrecioContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmPrecioUnitario = 0;
        if (decimal.TryParse(this.txtPrecioUnitarioCambio.Text.Trim(), out dcmPrecioUnitario))
        {
            CComunDB.CCommun.Actualizar_Precio(this.hdProductoPrecioID.Value,
                                               this.hdVentaLista.Value,
                                               dcmPrecioUnitario);
        }
    }

    protected void btnFacturarContinuar_Click(object sender, EventArgs e)
    {
        int intFacturaID = 0;
        DataSet objDataResult = new DataSet();
        string strQuery = string.Empty;
        CFacturaDB objFactura = new CFacturaDB();

        if (!string.IsNullOrEmpty(this.txtFacturaID.Text.Trim()))
        {
            int.TryParse(this.txtFacturaID.Text.Trim(), out intFacturaID);
            if (!objFactura.Leer(intFacturaID))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Factura con folio " + this.txtFacturaID.Text.Trim() + " no existe ");
                return;
            }
            else
                if (objFactura.status != 8)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Factura con folio " + this.txtFacturaID.Text.Trim() + " ya fue timbrada, está cancelada o está en cobranza");
                    return;
                }

            strQuery = "SELECT 1 FROM nota_facturas " +
                      " WHERE nota_ID = " + this.hdID.Value +
                      "   AND factura_ID = " + intFacturaID;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Factura con folio " + this.txtFacturaID.Text.Trim() + " ya fue usada para esta remisión");
                return;
            }
        }
        else
        {
            strQuery = "SELECT contado" +
                      " FROM establecimientos E" +
                      " INNER JOIN sucursales S" +
                      " ON E.ID = S.establecimiento_ID" +
                      " AND S.ID = " + this.hdSucursalID.Value;

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            DateTime dtFecha = DateTime.Now;

            objFactura.electronica = true;
            objFactura.fecha = dtFecha;
            objFactura.sucursal_ID = int.Parse(this.hdSucursalID.Value);
            objFactura.iva = Decimal.Parse(this.rdIVA.SelectedValue);
            objFactura.contado = objFactura.contado = (bool)objDataResult.Tables[0].Rows[0]["contado"];
            objFactura.status = 8;
            objFactura.lista_precios_ID = int.Parse(this.hdVentaLista.Value);
            objFactura.vendedorID = int.Parse(this.hdVentaVendedor.Value);
            objFactura.isr_ret = decimal.Parse(this.hdPorcISRRet.Value);
            objFactura.iva_ret = decimal.Parse(this.hdPorcIVARet.Value);
            objFactura.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
            objFactura.total_real = objFactura.total;
            objFactura.moneda = "MXN";
            objFactura.tipo_cambio = CRutinas.Obtener_TipoCambio();

            if (objFactura.Guardar())
            {
                intFacturaID = objFactura.ID;
            }
            else
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
                return;
            }
        }

        strQuery = "SELECT * FROM notas_prod " +
                   "WHERE nota_ID = " + this.hdID.Value;
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
            Agregar_Producto_Fact(intFacturaID, (int)objRowResult["producto_ID"],
                             (decimal)objRowResult["cantidad"],
                             (decimal)objRowResult["costo_original"],
                             (decimal)objRowResult["costo"],
                             Convert.ToBoolean(objRowResult["exento"]),
                             objFactura.moneda,
                             objRowResult["costo_original_moneda"].ToString(),
                             objFactura.tipo_cambio,
                             objRowResult["consecutivo"].ToString());
        }

        strQuery = "UPDATE notas SET " +
            "status = 4" +
            ",modificadoPorID = " + Session["SIANID"].ToString() +
            ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
            " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        strQuery = "INSERT INTO nota_facturas (nota_ID, factura_ID) VALUES(" +
                   this.hdID.Value +
                   " , " + intFacturaID +
                   ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        // Si la moneda de la nota es igual a la de la factura, sólo se trasladan los pagos a la factura
        if (this.hdMoneda.Value.Equals(objFactura.moneda))
            CComunDB.CCommun.Ejecutar_SP("INSERT INTO pago_facturas (pagoID, facturaID, monto_pago, notaID)" +
                                       " SELECT pagoID, " + intFacturaID +
                                       " , monto_pago " +
                                       " , " + this.hdID.Value +
                                       " FROM pago_notas" +
                                       " WHERE notaID = " + this.hdID.Value);
        else
        {
            // Recalcula el pago
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT P.ID" +
                                                        ",P.moneda" +
                                                        ",P.tipo_cambio" +
                                                        ",N.monto_pago" +
                                                        " FROM pago_notas N" +
                                                        " INNER JOIN pagos P" +
                                                        " ON N.pagoID = P.ID" +
                                                        " AND N.notaID = " + this.hdID.Value);

            decimal dcmMontoPago, dcmTipoCambio;
            foreach (DataRow objRow in objDataResult.Tables[0].Rows)
            {
                dcmTipoCambio = (decimal)objRow["tipo_cambio"];
                // El Pago fue en la misma moneda que la nota
                if (objRow["moneda"].ToString().Equals(this.hdMoneda.Value))
                {
                    dcmMontoPago = (decimal)objRow["monto_pago"];
                }
                else
                {
                    // Se pago en pesos, pero se aplicaron DLS, entonces convierte DLS a pesos
                    if (objRow["moneda"].ToString().Equals("MXN"))
                        dcmMontoPago = Math.Round((decimal)objRow["monto_pago"] * dcmTipoCambio, 2);
                    else
                        dcmMontoPago = Math.Round((decimal)objRow["monto_pago"] / dcmTipoCambio, 2);
                }

                if (!objRow["moneda"].ToString().Equals(objFactura.moneda))
                {
                    if (objFactura.moneda.Equals("MXN"))
                        dcmMontoPago = Math.Round(dcmTipoCambio * dcmMontoPago, 2);
                    else
                        dcmMontoPago = Math.Round(dcmMontoPago / dcmTipoCambio, 2);
                }

                CComunDB.CCommun.Ejecutar_SP("INSERT INTO pago_facturas (pagoID, facturaID, monto_pago, notaID)" +
                                            " VALUES (" +
                                            objRow["ID"] +
                                            "," + intFacturaID +
                                            "," + dcmMontoPago.ToString("0.00") +
                                            "," + this.hdID.Value +
                                            ")");
            }
        }

        CComunDB.CCommun.Ejecutar_SP("DELETE FROM pago_notas" +
                                    " WHERE notaID = " + this.hdID.Value);

        //Recalcular_Productos_Datos();

        ((master_MasterPage)Page.Master).MostrarMensajeError("Nota ha sido facturada, folio: " + objFactura.folio + objFactura.factura_suf);

        Mostrar_Datos();
    }

    private bool Validar_Inventario()
    {
        if (this.hdInventarios.Value.Equals("0"))
            return true;

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT productoID, producto, lote, SUM(cantidad) as cantidad FROM (" +
                          "SELECT C.producto_ID as productoID " +
                          ", P1.nombre as producto " +
                          ", C.lote as lote " +
                          ", C.cantidad as cantidad " +
                          " FROM notas_prod_lote C " +
                          " INNER JOIN notas_prod P " +
                          " ON C.producto_ID = P.producto_ID " +
                          " AND P.nota_ID = C.nota_ID " +
                          " AND P.nota_ID = " + this.hdID.Value +
                          " INNER JOIN productos P1" +
                          " ON P1.ID = P.producto_ID" +
                          ") AS R" +
                          " GROUP BY productoID, producto, lote ";

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
            if (decimal.Parse(objRowResult["cantidad"].ToString()) >
                CInventarios.Obtener_Existencia(objRowResult["productoID"].ToString(),
                                               CComunDB.CCommun.ObtenerAlmacenPrincipal(),
											   objRowResult["lote"].ToString()))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("No hay suficiente cantidad en el inventario para el producto " +
                                    objRowResult["producto"].ToString() +
                                    ", " + CRutinas.GetLocalizedMsg("lblLote").ToLower() + ": " + objRowResult["lote"].ToString());
                return false;
            }
        }

        return true;
    }

    private void Actualizar_Inventario()
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT producto_ID, cantidad, costo_original, costo_original_moneda" +
                          " FROM notas_prod" +
                          " WHERE nota_ID = " + this.hdID.Value;

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            CProducto_Datos objProd_Datos = new CProducto_Datos();
            objProd_Datos.intProductoID = (int)objRowResult["producto_ID"];
			objProd_Datos.intAppID = int.Parse(Session["SIANAppID"].ToString());
            objProd_Datos.Leer();
            objProd_Datos.intNotaID = int.Parse(this.hdID.Value);
            objProd_Datos.dcmNota_tipocambio = CRutinas.Obtener_TipoCambio();
            objProd_Datos.dtNota_fecha = DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX"));
            objProd_Datos.dcmNota_cantidad = (decimal)objRowResult["cantidad"];
            objProd_Datos.dcmNota_costo = (decimal)objRowResult["costo_original"];
            objProd_Datos.strNota_costo_moneda = objRowResult["costo_original_moneda"].ToString();
            objProd_Datos.dcmNota_total = Math.Round((decimal)objRowResult["costo_original"] * (decimal)objRowResult["cantidad"], 2);

            objProd_Datos.Verificar_Nota();

            objProd_Datos.Guardar();
        }

        strQuery = "SELECT L.producto_ID as productoID " +
                  ", L.lote as lote " +
                  ", SUM(L.cantidad) as cantidad " +
                  " FROM notas_prod_lote L " +
                  " INNER JOIN notas_prod C " +
                  " ON C.producto_ID = L.producto_ID " +
                  "  AND C.consecutivo = L.consecutivo" +
                  "  AND C.nota_ID = L.nota_ID " +
                  "  AND C.nota_ID = " + this.hdID.Value +
                  " GROUP BY L.producto_ID, L.lote ";

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
            CInventarios.Restar(objRowResult["productoID"].ToString(),
                                CComunDB.CCommun.ObtenerAlmacenPrincipal(),
								objRowResult["lote"].ToString(),
                                decimal.Parse(objRowResult["cantidad"].ToString()),
                                 "Remisión " + this.hdID.Value);
        }
    }

    private void Devolver_Inventario()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT L.producto_ID as productoID " +
                          ", L.lote as lote " +
                          ", L.fecha_caducidad " +
                          ", SUM(L.cantidad) as cantidad " +
                          " FROM notas_prod_lote L " +
                          " INNER JOIN notas_prod C " +
                          " ON C.producto_ID = L.producto_ID " +
                          "  AND C.consecutivo = L.consecutivo" +
                          "  AND C.nota_ID = L.nota_ID " +
                          "  AND C.nota_ID = " + this.hdID.Value +
                          " GROUP BY L.producto_ID, L.lote, L.fecha_caducidad ";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        DateTime? dtFecha;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            if (objRowResult.IsNull("fecha_caducidad"))
                dtFecha = null;
            else
                dtFecha = (DateTime)objRowResult["fecha_caducidad"];
            CInventarios.Sumar(objRowResult["productoID"].ToString(),
                                CComunDB.CCommun.ObtenerAlmacenPrincipal(),
								objRowResult["lote"].ToString(),
                                dtFecha,
                                decimal.Parse(objRowResult["cantidad"].ToString()),
                                "Remisión " + this.hdID.Value);
        }
    }

    private void Agregar_Producto_Fact(int intFacturaID,
                                       int intProductoID,
                                       decimal dcmCantidad,
                                       decimal dcmCosto_unitario,
                                       decimal dcmCosto,
                                       bool esExento,
                                       string strMonedaFact,
                                       string strMonedaProd,
                                       decimal dcmTipoCambio,
                                       string strConsecutivo)
    {
        DataSet objDataResult = new DataSet();

        decimal dcmCosto_Original = dcmCosto_unitario;

        if (!strMonedaFact.Equals(strMonedaProd))
        {
            // A pesos
            if (strMonedaFact.Equals("MXN"))
                dcmCosto_unitario = Math.Round(dcmTipoCambio * dcmCosto_unitario, 2);
            else
                dcmCosto_unitario = Math.Round(dcmCosto_unitario / dcmTipoCambio, 2);
        }

        string strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                        " FROM facturas_liq_prod " +
                        " WHERE factura_ID = " + intFacturaID;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO facturas_liq_prod (factura_ID, " +
                "producto_ID, exento, cantidad, costo_unitario" +
                ",costo_original, costo_original_moneda" +
                ",costo, consecutivo, inventariado) VALUES (" +
                "'" + intFacturaID.ToString() + "'" +
                ", '" + intProductoID + "'" +
                ", '" + (esExento ? "1" : "0") + "'" +
                ", '" + dcmCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto_Original.ToString() + "'" +
                ", '" + strMonedaProd + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", 1" +
                ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
            return;
        }

        strQuery = "INSERT INTO facturas_prod_lote (consecutivo, factura_ID, producto_ID" +
                  ",lote, fecha_caducidad, cantidad) " +
                  " SELECT " +
                  objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() +
                  ", " + intFacturaID +
                  ", " + intProductoID +
                  ", lote" +
                  ", fecha_caducidad" +
                  ", cantidad" +
                  " FROM notas_prod_lote " +
                  " WHERE nota_ID = " + this.hdID.Value +
                  "   AND producto_ID = " + intProductoID +
                  "   AND consecutivo = " + strConsecutivo;
        CComunDB.CCommun.Ejecutar_SP(strQuery);
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {
        if (System.IO.File.Exists(Server.MapPath("../facturas" + "/nota_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx")))
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                "SIANRPT",
                                                "mostrarPopUp('nota_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx?notID=" + this.hdID.Value + "')",
                                                true);
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                "SIANRPT",
                                                "mostrarPopUp('nota_impresion.aspx?notID=" + this.hdID.Value + "')",
                                                true);
    }

    private void Mostrar_Pagos(string strNotaID)
    {
        this.lblPagos.Text = "Pagos de la remisión " + strNotaID;
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

        string strQuery = "SELECT T.tipo_pago, P.referencia, P.fecha_pago, F.monto_pago, S.moneda" +
                         " FROM pago_notas F" +
                         " INNER JOIN pagos P" +
                         " ON P.ID = F.pagoID" +
                         " AND F.notaID = " + strNotaID +
                         " AND F.aplicado = 1" +
                         " INNER JOIN notas S" +
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
            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        this.hdAFacturar.Value = "0";
        if (this.gvProductos.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Venta sin productos, ingrese al menos uno");
            return;
        }

        Actualizar_Nota();

        if (!Validar_Inventario())
            return;

        string strMensaje = CComunDB.CCommun.Validar_Limite(0, int.Parse(this.hdSucursalID.Value), decimal.Parse(this.hdTotal.Value));
        if (!string.IsNullOrEmpty(strMensaje))
        {
            this.lblLimiteCR.Text = strMensaje;
            this.mdLimiteCR.Show();
            return;
        }

        Continuar_Finalizar();
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
        if (this.hdAFacturar.Value.Equals("1"))
            Finalizar_Nota(3);
        else
            Continuar_Finalizar();
    }

    private void Continuar_Finalizar()
    {
        this.txtFechaPago.Text = DateTime.Today.ToString("dd/MM/yyyy");
        this.txtReferencia.Text = string.Empty;
        this.txtPago.Text = this.hdPago.Value = this.hdTotal.Value;
        this.dlMonedaPago.ClearSelection();
        this.dlMonedaPago.Items.FindByValue(this.hdMoneda.Value).Selected = true;
        this.mdPago.Show();
    }

    private void Finalizar_Nota(byte btStatus)
    {
        string strQuery = "UPDATE notas SET " +
            "status = " + btStatus +
            ",modificadoPorID = " + Session["SIANID"].ToString() +
            ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
            " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        Actualizar_Inventario();

        if(this.hdInvAlmacen.Value.Equals("1"))
        {
            int intPedido = Generar_Pedido();
            ((master_MasterPage)Page.Master).MostrarMensajeError("Captura de la venta ha sido finalizada<br>Venta " + this.hdID.Value + "<br/>Pedido " + intPedido);
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Captura de la venta ha sido finalizada<br>Venta " + this.hdID.Value);

        Mostrar_Datos();
    }

    protected void btnPagoContinuar_Click(object sender, EventArgs e)
    {
        Finalizar_Nota(0);   //Poner como pagada

        DateTime dtFecha = DateTime.Parse(this.txtFechaPago.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1" +
                         " FROM pago_notas" +
                         " WHERE notaID = " + this.hdID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            return;

        string strCuentaBancaria = "0";

        if (this.dlCuentaBancaria.Items.Count > 0)
        {
            string[] strBancoIDs = this.dlCuentaBancaria.SelectedValue.Split('_');
            strCuentaBancaria = strBancoIDs[0];
        }

        strQuery = "INSERT INTO pagos (tipo_pago, fecha_pago, referencia, " +
                   "monto_pago, moneda, tipo_cambio, cuenta_bancariaID" +
                   ",aplicado, monto_aplicado, estatusID, nota" +
                   ", creadoPorID, creadoPorFecha, appID) VALUES(" +
                   "'" + this.dlTiposPagos.SelectedValue + "'" +
                   ",'" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ",'" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                   ",'" + this.txtPago.Text + "'" +
                   ",'" + this.hdMoneda.Value + "'" +
                   ",'" + CRutinas.Obtener_TipoCambio() + "'" +
                   "," + strCuentaBancaria +
                   ",1" +
                   ",'" + this.txtPago.Text + "'" +
                   ",8" +
                   ",1" +
                   ",'" + Session["SIANID"].ToString() + "'" +
                   ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
				   ", " + Session["SIANAppID"] +
                   ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "SELECT ID" +
                   " FROM pagos " +
                   " WHERE fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   " AND monto_pago = " + this.txtPago.Text;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        strQuery = "INSERT INTO pago_notas (pagoID, notaID, monto_pago) VALUES(" +
                   "'" + objDataResult.Tables[0].Rows[0]["ID"].ToString() + "'" +
                   ",'" + this.hdID.Value + "'" +
                   ",'" + this.hdPago.Value + "'" +
                   ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "UPDATE notas SET " +
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
        string strQuery = "UPDATE notas_prod SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND nota_id = " + this.hdID.Value + ";" +
                         "UPDATE notas_prod_lote SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND nota_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE notas_prod SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND nota_id = " + this.hdID.Value + ";" +
                  "UPDATE notas_prod_lote SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND nota_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE notas_prod SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND nota_id = " + this.hdID.Value + ";" +
                  "UPDATE notas_prod_lote SET " +
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

    private void Recalcular_Productos_Datos()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT producto_ID " +
                         " FROM notas_prod " +
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
    }

    private int Generar_Pedido()
    {
        int intPedidoID = 0;
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM orden_compra " +
                    " WHERE sucursalID = " + this.hdSucursalID.Value +
                    " AND fecha_creacion = '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            return intPedidoID;

        strQuery = "SELECT 1 " +
                  " FROM orden_compra_nota" +
                  " WHERE notaID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            return intPedidoID;

        strQuery = "INSERT INTO orden_compra (sucursalID, orden_compra, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, requerimientos," +
                   "tiempo_entrega, lugar_entrega, fecha_entrega, motivo_cancelacion, costo, " +
                   "porc_iva, iva, total," +
                   "comentarios, creadoPorID, creadoPorFecha, modificadoPorID, modificadoPorFecha, appID) VALUES (" +
                   "'" + this.hdSucursalID.Value + "'" +
                   ", ''" +
                   ", '" + this.hdVentaLista.Value + "'" +
                   ", '3'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", ''" +
                   ", ''" +
                   ", null" +
                   ", ''" +
                   ", '" + this.hdCosto.Value + "'" +
                   ", '" + this.rdIVA.SelectedValue + "'" +
                   ", '" + this.hdIVA.Value + "'" +
                   ", '" + this.hdTotal.Value + "'" +
                   ", ''" +
                   ", '" + Session["SIANID"].ToString() + "'" +
                   ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '" + Session["SIANID"].ToString() + "'" +
                   ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '" + Session["SIANAppID"].ToString() + "'" +
                   ")";
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "SELECT ID " +
                  " FROM orden_compra " +
                  " WHERE sucursalID = " + this.hdSucursalID.Value +
                  " AND fecha_creacion = '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        intPedidoID = (int)objDataResult.Tables[0].Rows[0]["ID"];

        strQuery = "INSERT INTO orden_compra_productos (orden_compraID, " +
                  "productoID, exento, cantidad, costo_unitario, costo," +
                  "costo_original, costo_original_moneda," +
                  "validado, faltante, consecutivo) " +
                  " SELECT " + intPedidoID +
                  ", producto_ID" +
                  ", exento" +
                  ", cantidad" +
                  ", costo_unitario" +
                  ", costo" +
                  ",costo_original" +
                  ", costo_original_moneda" +
                  ", 1" +
                  ", 0" +
                  ", consecutivo" +
                  " FROM notas_prod" +
                  " WHERE nota_ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "INSERT INTO orden_compra_productos_lote (orden_compraID, " +
                  " productoID, consecutivo, lote, fecha_caducidad, cantidad) " +
                  " SELECT " + intPedidoID +
                  ", producto_ID" +
                  ", consecutivo" +
                  ", lote" +
                  ", fecha_caducidad" +
                  ", cantidad" +
                  " FROM notas_prod_lote" +
                  " WHERE nota_ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "INSERT INTO orden_compra_nota (orden_compraID, notaID) " +
                  " VALUES( " +
                  intPedidoID +
                  ", " + this.hdID.Value +
                  ")" ;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        return intPedidoID;
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    private void Obtener_CreadoPor()
    {
        this.lblCreado.Text = string.Empty;
        string strQuery = "SELECT U.usuario, F.creadoPorFecha" +
                         " FROM notas F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.creadoPorID" +
                         " AND F.ID = " + this.hdID.Value + ";" +
                         "SELECT U.usuario, F.modificadoPorFecha" +
                         " FROM notas F" +
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

    private void Obtener_Moneda()
    {
        this.lblMoneda.Text =
            this.lblPrecioCambiarMoneda.Text =
            this.hdMoneda.Value = CComunDB.CCommun.ObtenerMonedaListasPrecios(this.hdVentaLista.Value);
    }

    protected void dlMonedaPago_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdMoneda.Value.Equals(this.dlMonedaPago.SelectedValue))
        {
            decimal dcmTipoCambio = CRutinas.Obtener_TipoCambio();
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

    protected void gvCambiarLote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((TextBox)e.Row.FindControl("txtCantidadLote")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
        }
    }
}
