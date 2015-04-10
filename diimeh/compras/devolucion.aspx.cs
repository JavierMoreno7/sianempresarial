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

public partial class compras_devolucion : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='CancelFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='CancelFormat2'";
        this.btnRegresar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.btnImprimir.Attributes["onmouseout"] = "javascript:this.className='ReporteFormat1'";
        this.btnImprimir.Attributes["onmouseover"] = "javascript:this.className='ReporteFormat2'";
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

            this.hdUsuPr.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1105, out swVer, out swTot))
                this.hdUsuPr.Value = "1";
            else
            {
                this.chkCambiarPrecios.Checked = false;
                this.chkCambiarPrecios.Enabled = false;
                this.txtCambiarUnitario.Enabled = false;
            }

            if (!CComunDB.CCommun.ValidarAcceso(10020, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Visible = false;
                this.hdAT.Value = "0";
            }

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

            Llenar_Catalogos();

            Llenar_Grid();

            this.hdID.Value = "";
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerCompra_DevEstatus(false);
        this.dlEstatus.DataBind();

        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("COMPRAS");
        this.dlListaPrecios.DataBind();

        Llenar_Personas(true, 0);

        if (this.dlCajero.Items.Count == 0 || this.dlListaPrecios.Items.Count == 0)
            this.lblAgregar.Visible = false;
        else
            this.lblAgregar.Visible = true;

        StringBuilder strEstatus = new StringBuilder();
        for (int i = 0; i < this.dlEstatus.Items.Count; i++)
        {
            if (strEstatus.Length > 0)
                strEstatus.Append(", ");
            strEstatus.Append(this.dlEstatus.Items[i].Text);
        }
        this.hdEstatus.Value = strEstatus.ToString();
    }

    private void Llenar_Personas(bool swLlenarActivos, int intPersonaID)
    {
        if (swLlenarActivos)
        {
            this.dlCajero.DataSource = CComunDB.CCommun.ObtenerCajeros(false, true, 0);
            this.dlCajero.DataBind();
        }
        else
        {
            if (this.dlCajero.Items.FindByValue(intPersonaID.ToString()) == null)
            {
                this.dlCajero.DataSource = CComunDB.CCommun.ObtenerCajeros(false, true, intPersonaID);
                this.dlCajero.DataBind();
            }
        }
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerDevoluciones();
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

    private DataTable ObtenerDevoluciones()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("compra_devID", typeof(string)));
        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_creacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));

        string strQuery = "CALL leer_compras_dev_consulta(" +
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
            dr[0] = objRowResult["compra_devID"].ToString();
            dr[1] = objRowResult["proveedor"].ToString();
            dr[2] = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy HH:mm");
            if ((DateTime)objRowResult["fecha_cancelacion"] == DateTime.Parse("1901-01-01"))
                dr[3] = string.Empty;
            else
                dr[3] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[4] = this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Text;
            dr[5] = ((decimal)objRowResult["costo"]).ToString("c");
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
        dt.Columns.Add(new DataColumn("id", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("con", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", C.exento as exento " +
            ", C.lote as lote " +
            ", LEFT(P.nombre, 70) as producto " +
            ", P.codigo as codigo " +
            " FROM compra_dev_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND compra_devID = " + this.hdID.Value +
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
            dr[3] = ((decimal)objRowResult["costo_unitario"]).ToString("c");
            decimal dblCosto = (decimal)objRowResult["costo"];
            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("#.###");
            dr[4] = dblCosto.ToString("c");
            dcmCosto += dblCosto;
            if (!Convert.ToBoolean(objRowResult["exento"]))
            {
                dcmCostoIVA += dblCosto;
                dr[1] = dr[1] + "*";
            }

            if (intId == 0)
                this.hdConsMin.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMax.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMaxID.Value = intId.ToString();
            intId++;
            dr[5] = intId.ToString();
            dr[6] = objRowResult["codigo"].ToString();
            dr[7] = objRowResult["consecutivo"].ToString();
            dr[8] = objRowResult["lote"].ToString();
            dt.Rows.Add(dr);
        }

        dcmCosto = Math.Round(dcmCosto, 2);
        dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

        this.hdCosto.Value = dcmCosto.ToString();
        this.hdCostoIVA.Value = dcmCostoIVA.ToString();

        return dt;
    }

    private DataTable ObtenerProductosCompra()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dt.Columns.Add(new DataColumn("exento", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("enabled", typeof(bool)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.consecutivo as consecutivo " +
            ", C.cantidad as cantidad " +
            ", C.exento as exento " +
            ", C.costo_unitario as costo_unitario " +
            ", C.lote as lote " +
            ", LEFT(P.nombre, 70) as producto " +
            " FROM compra_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND compraID = " + this.txtCompraID.Text +
            ") AS AA ORDER BY consecutivo, producto";
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
            dr[0] = objRowResult["productoID"].ToString();
            dr[1] = objRowResult["producto"].ToString();
            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("#.###");
            dr[3] = ((decimal)objRowResult["costo_unitario"]).ToString("0.00");
            dr[4] = Convert.ToBoolean(objRowResult["exento"]) ? "1" : "0";
            dr[5] = objRowResult["lote"].ToString();
            if (this.hdUsuPr.Value.Equals("0"))
                dr[6] = false;
            else
                dr[6] = true;
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
                    strCriterio.Append("%");
                    strCriterio.Append(this.txtCriterio.Text.Trim());
                    strCriterio.Append("%");
                    break;
                case "2":
                case "3":
                case "5":
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
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lblCompra_Dev.Text = string.Empty;
        this.hdCompraID.Value = "0";
        this.lblCorreoEnvio.Text = string.Empty;
        if (this.hdID.Value.Equals("0"))
        {
            this.txtFechaCreacion.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtDescuento.Text = "0";
            this.txtComentarios.Text = string.Empty;
            this.txtProducto.Enabled = false;
            this.txtCantidad.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = false;
            this.btnCancelar.Visible = false;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = false;
            this.txtProveedor.Text = "";
            this.hdProveedorID.Value = string.Empty;
            this.txtProveedor.Enabled = true;
            this.dlCajero.Enabled = true;
            this.rdIVA.Enabled = true;
            this.dlListaPrecios.Enabled = true;
            this.txtDescuento.Enabled = true;
            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue("8").Selected = true;
            this.dlEstatus.Enabled = false;
            this.hdCosto.Value = "0";
            this.hdCostoIVA.Value = "0";
            this.hdIVA.Value = "0.00";
            this.hdTotal.Value = "0";
            this.gvProductos.DataSource = null;
            this.gvProductos.DataBind();
            this.lblNotas.Text = string.Empty;
            this.hdBorrar.Value = "1";
            this.btnUsarCompra.Visible = true;
            this.btnFinalizar.Visible = false;
            this.txtLote.Enabled = false;
            Llenar_Personas(true, 0);
        }
        else
        {
            this.lblCompra_Dev.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT proveedorID, proveedor, cajeroID, " +
                    " lista_preciosID, estatus, fecha_creacion, fecha_cancelacion, " +
                    " motivo_cancelacion, comentarios, " +
                    " porc_iva, O.descuento, O.contado " +
                    " FROM compra_dev O" +
                    " INNER JOIN proveedores S " +
                    " ON O.proveedorID = S.ID " +
                    " WHERE O.ID = " + this.hdID.Value + ";" +
                    " SELECT compraID" +
                    " FROM compra_dev_compras" +
                    " WHERE compra_devID = " + this.hdID.Value + ";" +
                    " SELECT fecha_envio" +
                    " FROM correo_envio" +
                    " WHERE ID = " + this.hdID.Value +
                    "   AND tipo = 5;";
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

            Llenar_Personas(false, (int)objRowResult["cajeroID"]);
            this.dlCajero.ClearSelection();
            this.dlCajero.Items.FindByValue(objRowResult["cajeroID"].ToString()).Selected = true;

            this.txtFechaCreacion.Text = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy");

            this.txtComentarios.Text = objRowResult["comentarios"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Selected = true;

            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_preciosID"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["porc_iva"]).ToString("0.00")).Selected = true;

            this.txtDescuento.Text = objRowResult["descuento"].ToString();

            if (objDataResult.Tables[2].Rows.Count > 0)
                this.lblCorreoEnvio.Text = "Correo enviado: " + ((DateTime)objDataResult.Tables[2].Rows[0]["fecha_envio"]).ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("es-MX"));

            if (!objRowResult["estatus"].ToString().Equals("8"))
                this.hdBorrar.Value = "0";
            else
                this.hdBorrar.Value = "1";

            if (objDataResult.Tables[1].Rows.Count > 0)
            {
                this.hdCompraID.Value = objDataResult.Tables[1].Rows[0][0].ToString();
                this.lblMensaje.Text = "Compra " + this.hdCompraID.Value;
            }

            Llenar_Productos();

            if (!objRowResult["estatus"].ToString().Equals("8"))
            {
                this.txtProveedor.Enabled = false;
                this.dlCajero.Enabled = false;
                this.txtDescuento.Enabled = false;
                this.txtProducto.Enabled = false;
                this.txtCantidad.Enabled = false;
                this.txtPrecioUnitario.Enabled = false;
                this.btnModificar.Visible = false;
                this.btnImprimir.Visible = true;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.dlListaPrecios.Enabled = false;
                this.rdIVA.Enabled = false;
                this.btnUsarCompra.Visible = false;
                this.txtLote.Enabled = false;
                this.btnFinalizar.Visible = false;
                if (objRowResult["estatus"].ToString().Equals("9"))
                    this.lblMensaje.Text = "Devolución cancelada: " + objRowResult["motivo_cancelacion"].ToString();
            }
            else
            {
                this.txtProveedor.Enabled = true;
                this.dlCajero.Enabled = true;
                this.txtDescuento.Enabled = true;
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                if (this.hdUsuPr.Value.Equals("0"))
                    this.txtPrecioUnitario.Enabled = false;
                else
                    this.txtPrecioUnitario.Enabled = true;
                this.btnModificar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnCancelar.Visible = true;
                this.btnAgregarProd.Visible = true;
                this.btnAgregarProd.Enabled = true;
                this.dlListaPrecios.Enabled = true;
                this.rdIVA.Enabled = true;
                if(this.hdCompraID.Value.Equals("0"))
                    this.btnUsarCompra.Visible = true;
                else
                    this.btnUsarCompra.Visible = false;
                this.txtLote.Enabled = true;
            }
            string[] strValores = Obtener_Notas(this.hdProveedorID.Value).Split('|');
            this.lblNotas.Text = strValores[0];
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Devolucion();
        }
        else
            Modificar_Devolucion();
    }

    private void Agregar_Devolucion()
    {
        if (string.IsNullOrEmpty(this.hdProveedorID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un proveedor de la lista");
            return;
        }

        if (Crear_Devolucion())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("La devolución ha sido creada, folio: " + this.hdID.Value);
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = true;
            this.btnCancelar.Visible = true;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = true;
            this.txtLote.Enabled = true;
        }
    }

    private bool Crear_Devolucion()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM compra_dev" +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe unacompra_devde orden para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra_dev (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion," +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios) VALUES (" +
                   "'" + this.hdProveedorID.Value + "'" +
                   ", '" + this.dlCajero.SelectedValue + "'" +
                   ", '" + this.dlListaPrecios.SelectedValue + "'" +
                   ", '" + this.dlEstatus.SelectedValue + "'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", '1'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
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
                " FROM compra_dev" +
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
            this.lblCompra_Dev.Text = this.hdID.Value;
            return true;
        }
        return false;
    }

    private bool Modificar_Devolucion()
    {
        string strQuery = "UPDATE compra_dev SET " +
                    "proveedorID = " + this.hdProveedorID.Value +
                    ",cajeroID = " + this.dlCajero.SelectedValue +
                    ",lista_preciosID = " + this.dlListaPrecios.SelectedValue +
                    ",comentarios = '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }
        ((master_MasterPage)Page.Master).MostrarMensajeError("Lacompra_devha sido modificada");

        return true;
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

        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        string strMensaje = string.Empty;
        if (int.TryParse(this.txtCantidad.Text.Trim(), out intCantidad) &&
            decimal.TryParse(this.txtPrecioUnitario.Text.Trim(), out dcmPrecioUnitario))
        {
            if (intCantidad > CInventarios.Obtener_Existencia(this.hdProductoID.Value,
                                                           "100",
                                                           this.txtLote.Text.Trim()))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("No hay suficiente inventario para este producto/lote");
                return;
            }

            this.lblPrecioProducto.Text = this.txtProducto.Text;
            if (this.lblPrecioProducto.Text.Length > 30)
                this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 30);
            this.hdProductoPrecioID.Value = this.hdProductoID.Value;
            if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                  intCantidad, Math.Round(dcmPrecioUnitario, 2),
                                  this.txtLote.Text.Trim(),
                                  this.dlListaPrecios.SelectedValue,
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

        this.txtCantidad.Focus();
    }

    private bool Buscar_Producto()
    {
        if (!string.IsNullOrEmpty(this.txtProducto.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, CONCAT(nombre, ' - ', sales) as nombre," +
                             "           V.precio_caja as precio " +
                             "    FROM productos P " +
                             "    LEFT JOIN precios V " +
                             "    ON P.ID = V.producto_ID" +
                             "    AND lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                             "    AND validez = '2099-12-31'" +
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
                if (objDataResult.Tables[0].Rows[0].IsNull(2))
                    this.txtPrecioUnitario.Text = "0.00";
                else
                    this.txtPrecioUnitario.Text = objDataResult.Tables[0].Rows[0][2].ToString();
                this.hdPrecioUnitario.Value = this.txtPrecioUnitario.Text;
                string strClientScript;
                if (this.txtPrecioUnitario.Enabled)
                    strClientScript = "setTimeout('setProductoPrecio()',100);";
                else
                    strClientScript = "setTimeout('setLote()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Agregar_Producto(string strProductoID, string strProducto,
                                int intCantidad, decimal dcmCosto_unitario,
                                string strLote,
                                string listaPrecios, out string strMensaje)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

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
        dcmCosto = Math.Round(dcmCosto_unitario * (decimal)intCantidad, 2);

        strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                    " FROM compra_dev_productos " +
                    " WHERE compra_devID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO compra_dev_productos (compra_devID, " +
                "productoID, exento, cantidad, costo_unitario, costo," +
                "lote, " +
                "consecutivo) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + intCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + strLote + "'" +
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

        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.txtLote.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        Llenar_Productos();
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
                e.Row.Cells[8].Controls.Clear();
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
                decimal dcmPorcDescuento;
                decimal dcmCosto, dcmCostoDescuento, dcmCostoIVA;
                decimal dcmIVA, dcmTotal;
                decimal.TryParse(this.txtDescuento.Text, out dcmPorcDescuento);
                decimal.TryParse(this.hdCosto.Value, out dcmCosto);
                decimal.TryParse(this.hdCostoIVA.Value, out dcmCostoIVA);

                //Subtotal antes de descuento, es la suma de todos los productos
                dcmCosto = Math.Round(dcmCosto, 2);
                strLeyenda.Append("Subtotal:");
                strValores.Append(dcmCosto.ToString("c"));

                dcmCostoDescuento = dcmCosto;
                dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

                // Si hay descuento, se calcula el subtotal con descuento
                if (dcmPorcDescuento != 0)
                {
                    dcmCostoDescuento = Math.Round(dcmCostoDescuento * (1 - (dcmPorcDescuento / 100)), 2);
                    this.hdCostoDescuento.Value = dcmCostoDescuento.ToString();
                    dcmCostoIVA = Math.Round(dcmCostoIVA * (1 - (dcmPorcDescuento / 100)), 2);
                    strLeyenda.Append("<br />Subtotal con descuento:");
                    strValores.Append("<br />" + dcmCostoDescuento.ToString("c"));
                }
                else
                    this.hdCostoDescuento.Value = "0";

                // IVA
                dcmIVA = Math.Round(dcmCostoIVA * decimal.Parse(this.rdIVA.SelectedValue) / 100, 2);
                this.hdIVA.Value = dcmIVA.ToString();
                strLeyenda.Append("<br />IVA " + this.rdIVA.SelectedValue + "%:");
                strValores.Append("<br />" + dcmIVA.ToString("c"));

                dcmTotal = Math.Round(dcmCostoDescuento + dcmIVA, 2);

                this.hdTotal.Value = dcmTotal.ToString();
                strLeyenda.Append("<br />TOTAL:");
                strValores.Append("<br />" + dcmTotal.ToString("c"));

                e.Row.Cells[0].ColumnSpan = 6;
                e.Row.Cells[0].Text = strLeyenda.ToString();
                e.Row.Cells[1].Text = strValores.ToString();
                e.Row.Cells[2].Text = string.Empty;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[7].Visible = false;
            }
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("mv"))
            return;

        if (e.CommandName == "Borrar")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            string[] strID_Consecutivo = this.gvProductos.DataKeys[index].Value.ToString().Split('_');

            string strQuery = "DELETE " +
                    " FROM compra_dev_productos " +
                    " WHERE compra_devID = " + this.hdID.Value +
                    " AND productoID = " + strID_Consecutivo[0] +
                    " AND consecutivo = " + strID_Consecutivo[1];
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            strQuery = "UPDATE compra_dev_productos SET " +
                    " consecutivo = consecutivo - 1 " +
                    " WHERE compra_devID = " + this.hdID.Value +
                    " AND consecutivo > " + strID_Consecutivo[1];

            CComunDB.CCommun.Ejecutar_SP(strQuery);

            Llenar_Productos();
        }
        else
            if (e.CommandName == "Modificar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                this.lblCambiarProducto.Text = this.gvProductos.Rows[index].Cells[2].Text;
                this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[4].Text;
                this.txtCambiarUnitario.Text = this.gvProductos.Rows[index].Cells[5].Text.Replace("$", "").Replace(",", "");
                this.txtCambiarLote.Text = this.gvProductos.Rows[index].Cells[7].Text.Replace("&nbsp;", "");
                this.mdCambiarProducto.Show();
                string strClientScript = "setTimeout('setProductoCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
            }
    }

    private void Llenar_Productos()
    {
        this.hdCosto.Value = "0";
        this.hdCostoIVA.Value = "0";
        this.hdIVA.Value = "0.00";
        this.hdTotal.Value = "0";

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        string strQuery = "UPDATE compra_dev SET " +
                    "porc_iva = " + this.rdIVA.SelectedValue +
                    ",descuento = " + this.txtDescuento.Text +
                    ",monto_subtotal = " + Math.Round(decimal.Parse(this.hdCosto.Value), 2) +
                    ",descuento = " + this.txtDescuento.Text +
                    ",monto_descuento = " + Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2) +
                    ",porc_iva = " + this.rdIVA.SelectedValue +
                    ",monto_iva = " + Math.Round(decimal.Parse(this.hdIVA.Value), 2) +
                    ",total = " + Math.Round(decimal.Parse(this.hdTotal.Value), 2) +
                    " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        if (this.gvProductos.Rows.Count > 0)
            this.btnFinalizar.Visible = true;
        else
            this.btnFinalizar.Visible = false;
    }

    protected void btnCancelarContinuar_Click(object sender, EventArgs e)
    {
        string strQuery = "UPDATE compra_orden SET " +
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

        this.txtProveedor.Enabled = false;
        this.txtProducto.Enabled = false;
        this.txtCantidad.Enabled = false;
        this.txtPrecioUnitario.Enabled = false;
        this.btnModificar.Visible = false;
        this.btnImprimir.Visible = false;
        this.btnCancelar.Visible = false;
        this.btnAgregarProd.Enabled = false;
        this.dlCajero.Enabled = false;
        this.rdIVA.Enabled = false;
        this.dlListaPrecios.Enabled = false;
        this.txtDescuento.Enabled = false;
        this.txtComentarios.Enabled = false;
    }

    protected void btnCambiarContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad, dcmPrecio;

        decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
        decimal.TryParse(this.txtCambiarUnitario.Text, out dcmPrecio);

        if (dcmCantidad > 0 && dcmPrecio > 0)
        {
            string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');
            dcmCantidad = Math.Round(dcmCantidad, 2);
            dcmPrecio = Math.Round(dcmPrecio, 2);
            string strQuery = "UPDATE compra_dev_productos SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                        ",lote = '" + this.txtCambiarLote.Text.Trim().Replace("'", "''") + "'" +
                        " WHERE compra_devID = " + this.hdID.Value +
                        " AND productoID = " + strID_Consecutivo[0] +
                        " AND consecutivo = " + strID_Consecutivo[1];
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }
            Llenar_Productos();
        }
    }

    [System.Web.Services.WebMethod]
    public static string ObtenerPrecio(string strParametros)
    {
        string[] strParametro = strParametros.Split('|');

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT precio_caja as precio FROM precios " +
                    "WHERE producto_ID = " + strParametro[0] +
                    " AND lista_precios_ID = " + strParametro[1] +
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
            decimal dcmCosto_unitario;
            DataRow objRowResult = objDataResult.Tables[0].Rows[0];
            dcmCosto_unitario = Math.Round((decimal)objRowResult["precio"], 2);
            return dcmCosto_unitario.ToString("0.00");
        }
        else
            return "0.00";
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarReporte('compra_excel.aspx?t=d&c=" +
                                        this.hdID.Value +
                                        "')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos();
    }

    protected void txtDescuento_TextChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
        {
            decimal dcmDescuento;
            decimal.TryParse(this.txtDescuento.Text.Trim(), out dcmDescuento);
            dcmDescuento = Math.Round(dcmDescuento, 2);
            this.txtDescuento.Text = dcmDescuento.ToString();
            Llenar_Productos();
        }
        this.txtComentarios.Focus();
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
        string strQuery = "UPDATE compra_dev_productos SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND compra_devID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE compra_dev_productos SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND compra_devID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE compra_dev_productos SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND compra_devID = " + this.hdID.Value;

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

    protected void btnUsarCompra_Click(object sender, EventArgs e)
    {
        this.txtCompraID.Text = string.Empty;
        this.txtCompraID.Enabled = true;
        this.btnObtenerCompra.Visible = true;
        this.btnObtenerCompra.Enabled = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlCompra.Visible = true;
        this.pnlCompraDatos.Visible = false;
    }

    protected void btnRegresarCompra_Click(object sender, EventArgs e)
    {
        Mostrar_Datos();
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlCompra.Visible = false;
    }

    protected void btnObtenerCompra_Click(object sender, EventArgs e)
    {
        long lngCompraID = 0;
        if (!long.TryParse(this.txtCompraID.Text.Trim(), out lngCompraID))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Folio debe ser numérico");
            return;
        }

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1" +
                         " FROM compra_dev_compras" +
                         " WHERE compraID = " + this.txtCompraID.Text.Trim();

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra ya tiene una devolución");
            return;
        }

        strQuery = "SELECT O.proveedorID, cajeroID, " +
                  " lista_preciosID, estatus, " +
                  " porc_iva, O.descuento, O.contado " +
                  " FROM compra O" +
                  " INNER JOIN proveedores S " +
                  " ON O.proveedorID = S.ID " +
                  " WHERE O.ID = " + this.txtCompraID.Text.Trim();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }

        catch (ApplicationException ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Error: " + ex.Message + strQuery);
            return;
        }

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra no existe");
            return;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];
        if (objRowResult["estatus"].ToString().Equals("9"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra ha sido cancelada, seleccione otra");
            return;
        }

        if (objRowResult["estatus"].ToString().Equals("8") ||
            objRowResult["estatus"].ToString().Equals("0"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra se encuentra En Proceso o ya fue pagada, seleccione otra");
            return;
        }

        this.hdProveedorID2.Value = objRowResult["proveedorID"].ToString();
        this.hdCajeroID.Value = objRowResult["cajeroID"].ToString();
        this.hdLista_preciosID.Value = objRowResult["lista_preciosID"].ToString();
        this.hdPorc_Iva.Value = objRowResult["porc_iva"].ToString();
        this.hdDescuento.Value = objRowResult["descuento"].ToString();
        this.hdContado.Value = objRowResult["contado"].ToString();

        this.gvProductosCompra.DataSource = ObtenerProductosCompra();
        this.gvProductosCompra.DataBind();

        if (this.gvProductosCompra.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra no tiene productos");
            return;
        }

        this.hdCompraID.Value = this.txtCompraID.Text.Trim();
        this.btnObtenerCompra.Visible = false;
        this.txtCompraID.Enabled = false;
        this.btnProcesar.Visible = true;
        this.pnlCompraDatos.Visible = true;
    }

    protected void btnProcesar_Click(object sender, EventArgs e)
    {
        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (!int.TryParse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text.Trim(), out intCantidad) ||
                    !decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text.Trim(), out dcmPrecioUnitario))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser numérica: " + ((Label)gvRow.FindControl("lblProdDatos")).Text);
                    return;
                }
                if (intCantidad > 0)
                {
                    if (intCantidad > CInventarios.Obtener_Existencia(this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                                      "100",
                                                                      ((TextBox)gvRow.FindControl("txtLoteDatos")).Text.Trim()))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("No hay suficiente inventario para este producto/lote: " + ((Label)gvRow.FindControl("lblProdDatos")).Text + ", Lote: " + ((TextBox)gvRow.FindControl("txtLoteDatos")).Text.Trim());
                        return;
                    }
                }
                ((TextBox)gvRow.FindControl("txtCantidadDatos")).Text = intCantidad.ToString();
                ((TextBox)gvRow.FindControl("txtCostoDatos")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();
            }
        }

        if (this.hdID.Value.Equals("0"))
        {
            if (!Crear_Devolucion_En_Orden())
                return;
        }

        string strQuery;

        decimal dcmCantidad, dcmCantidad_Compra, dcmCosto_unitario, dcmCosto;
        DataSet objDataResult = new DataSet();
        foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
        {
            dcmCantidad = decimal.Parse(((Label)gvRow.FindControl("lblCantDatos")).Text);
            dcmCantidad_Compra = decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text);

            if (dcmCantidad_Compra > 0)
            {
                dcmCosto_unitario = decimal.Parse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text);
                dcmCosto = Math.Round(dcmCosto_unitario * (decimal)dcmCantidad_Compra, 2);

                strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                    " FROM compra_dev_productos " +
                    " WHERE compra_devID = " + this.hdID.Value;
                try
                {
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch (ApplicationException ex)
                {
                    throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
                }

                strQuery = "INSERT INTO compra_dev_productos (compra_devID, " +
                           "productoID, exento, cantidad, costo_unitario, costo," +
                           "lote, consecutivo) VALUES (" +
                           "'" + this.hdID.Value + "'" +
                           ", '" + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString() + "'" +
                           ", '" + ((HiddenField)gvRow.FindControl("hdExento")).Value + "'" +
                           ", '" + dcmCantidad_Compra + "'" +
                           ", '" + dcmCosto_unitario + "'" +
                           ", '" + dcmCosto + "'" +
                           ", '" + ((TextBox)gvRow.FindControl("txtLoteDatos")).Text + "'" +
                           ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                           ")";
                try
                {
                    CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch
                {
                }
            }
        }

        strQuery = "INSERT INTO compra_dev_compras (compra_devID, compraID)" +
                  " VALUES (" +
                  "'" + this.hdID.Value + "'" +
                  ",'" + this.hdCompraID.Value + "'" +
                  ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        ((master_MasterPage)Page.Master).MostrarMensajeError("Devolución ha sido creada/actualizada");
        Mostrar_Datos();
        this.pnlCompra.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    private bool Crear_Devolucion_En_Orden()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM compra_dev" +
                    " WHERE proveedorID = " + this.hdProveedorID2.Value +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe unacompra_devpara este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra_dev (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, " +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios) VALUES (" +
                   "'" + this.hdProveedorID2.Value + "'" +
                   ", '" + this.hdCajeroID.Value + "'" +
                   ", '" + this.hdLista_preciosID.Value + "'" +
                   ", '8'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", '" + this.hdPorc_Iva.Value + "'" +
                   ", '0'" +
                   ", '" + this.hdDescuento.Value + "'" +
                   ", '0'" +
                   ", '" + this.hdPorc_Iva.Value + "'" +
                   ", '0'" +
                   ", '0'" +
                   ", ''" +
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
                " FROM compra_dev" +
                " WHERE proveedorID = " + this.hdProveedorID2.Value +
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
            return true;
        }
        ((master_MasterPage)Page.Master).MostrarMensajeError("No se generó la compra, intente de nuevo");
        return false;
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT R.*, LEFT(P.nombre, 70) as producto " +
            "FROM ( " +
            "     SELECT C.productoID as productoID " +
            "     , C.lote as lote " +
            "     , SUM(C.cantidad) as cantidad " +
            "     FROM compra_dev_productos C " +
            "     WHERE compra_devID = " + this.hdID.Value +
            "    GROUP BY C.productoID, C.lote" +
            " ) AS R"+
            " INNER JOIN productos P"+
            " ON P.ID = R.productoID";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        decimal dcmCantidad = 0;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dcmCantidad = decimal.Parse(objRowResult["cantidad"].ToString());

            if (dcmCantidad > CInventarios.Obtener_Existencia(objRowResult["productoID"].ToString(),
                                                              "100",
                                                              objRowResult["lote"].ToString()))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("No hay suficiente inventario para este producto/lote: " + objRowResult["producto"].ToString() + ", Lote: " + objRowResult["lote"].ToString());
                return;
            }
        }


        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            CInventarios.Restar(objRowResult["productoID"].ToString(),
                               "100",
                               objRowResult["lote"].ToString(),
                               decimal.Parse(objRowResult["cantidad"].ToString()),
                                "Devolución " + this.hdID.Value);

        }

        strQuery = "UPDATE compra_dev SET " +
                  " estatus = 0" +
                  " WHERE ID = " + this.hdID.Value;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        if (!this.hdCompraID.Value.Equals("0"))
        {
            DateTime dtFecha = DateTime.Now;
            strQuery = "INSERT INTO pagos (tipo_pago, fecha_pago, referencia, " +
                      "monto_pago, creadoPorID, creadoPorFecha) VALUES(" +
                      "'50'" +
                      ",'" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                      ",'Devolución " + this.hdID.Value + "'" +
                      ",'" + this.hdTotal.Value + "'" +
                      ",'" + Session["SIANID"].ToString() + "'" +
                      ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                      ")";
            CComunDB.CCommun.Ejecutar_SP(strQuery);

            strQuery = "SELECT ID" +
                   " FROM pagos " +
                   " WHERE fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   " AND monto_pago = " + this.hdTotal.Value;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            strQuery = "INSERT INTO pago_compras (pagoID, compraID, monto_pago)" +
                      " VALUES(" +
                      "'" + objDataResult.Tables[0].Rows[0]["ID"].ToString() + "'" +
                      ",'" + this.hdCompraID.Value + "'" +
                      ",'" + this.hdTotal.Value + "'" +
                      ")";

            CComunDB.CCommun.Ejecutar_SP(strQuery);

            strQuery = "SELECT total" +
                      " FROM compra " +
                      " WHERE ID = " + this.hdCompraID.Value + ";" +
                      " SELECT SUM(monto_pago) as monto_pago" +
                      " FROM pago_compras " +
                      " WHERE compraID = " + this.hdCompraID.Value;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if ((decimal)objDataResult.Tables[1].Rows[0][0] >=
                (decimal)objDataResult.Tables[0].Rows[0][0])
            {
                strQuery = "UPDATE compra" +
                          " SET estatus = 0" +
                          " WHERE ID = " + this.hdCompraID.Value;
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
        }

        Mostrar_Datos();
    }
}