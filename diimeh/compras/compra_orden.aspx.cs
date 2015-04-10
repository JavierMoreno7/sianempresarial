using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

public partial class compras_compra_orden : BasePage
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
        this.txtFechaSurtirse.Attributes["readonly"] = "true";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            ViewState["SortCampo"] = "5";
            ViewState["CriterioCampo"] = "2";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 2;
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

            if (!CComunDB.CCommun.ValidarAcceso(10000, out swVer, out swTot))
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

            if (Request.QueryString["s"] != null)
            {
                this.dlBusqueda.ClearSelection();
                this.dlBusqueda.Items.FindByValue(Request.QueryString["s"]).Selected = true;
                this.txtCriterio.Text = string.Empty;

                if (Request.QueryString["s"].Equals("4"))
                    this.txtCriterio.Text = "Activa";

                Validar_Campos();
            }
            else
                Llenar_Grid();

            this.hdID.Value = "";

        }
    }

    private void Llenar_Catalogos()
    {
        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerCompra_OrdenEstatus(false);
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

        this.dlPaqueteria.DataSource = CComunDB.CCommun.ObtenerPaqueterias(false, true);
        this.dlPaqueteria.DataBind();
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
        this.grdvLista.DataSource = ObtenerOrdenes_Compras();
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

    private DataTable ObtenerOrdenes_Compras()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("compra_ordenID", typeof(string)));
        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_creacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_surtirse", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));

        string strQuery = "CALL leer_compras_ordenes_consulta(" +
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
            dr[0] = objRowResult["compra_ordenID"].ToString();
            dr[1] = objRowResult["proveedor"].ToString();
            dr[2] = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy HH:mm");
            dr[3] = ((DateTime)objRowResult["fecha_a_surtirse"]).ToString("dd/MM/yyyy");
            if ((DateTime)objRowResult["fecha_cancelacion"] == DateTime.Parse("1901-01-01"))
                dr[4] = string.Empty;
            else
                dr[4] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[5] = this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Text;
            dr[6] = ((decimal)objRowResult["costo"]).ToString("c");
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
        dt.Columns.Add(new DataColumn("cant_compra", typeof(string)));
        dt.Columns.Add(new DataColumn("diferencia", typeof(string)));
        dt.Columns.Add(new DataColumn("parcial", typeof(string)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", C.exento as exento " +
            ", C.cantidad_compra as cant_compra " +
            ", (C.cantidad_compra - C.cantidad) as diferencia " +
            ", LEFT(P.nombre, 80) as producto " +
            ", P.codigo as codigo " +
            " FROM compra_orden_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND compra_ordenID = " + this.hdID.Value +
            ") AS AA ORDER BY consecutivo, producto";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataSet objDataResult2;
        int intId = 0;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString();
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
            if (!this.dlEstatus.SelectedValue.Equals("1") &&
                !this.dlEstatus.SelectedValue.Equals("8"))
            {
                dr[8] = ((decimal)objRowResult["cant_compra"]).ToString("0.###");
                dr[9] = ((decimal)objRowResult["diferencia"]).ToString("0.###");
            }
            else
            {
                dr[8] = string.Empty;
                dr[9] = string.Empty;
            }
            strQuery = "SELECT DISTINCT 1" +
                      " FROM orden_compra C" +
                      " INNER JOIN orden_compra_productos P" +
                      " ON C.ID = P.orden_compraID" +
                      " AND C.estatus = 4" +
                      " AND P.validado = 0" +
                      " AND P.productoID = " + objRowResult["productoID"];

            objDataResult2 = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult2.Tables[0].Rows.Count > 0)
                dr[10] = "1";
            else
                dr[10] = "0";

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
        dt.Columns.Add(new DataColumn("caducidad", typeof(string)));
        dt.Columns.Add(new DataColumn("enabled", typeof(bool)));
        dt.Columns.Add(new DataColumn("productoChk", typeof(bool)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.consecutivo as consecutivo " +
            ", (C.cantidad - C.cantidad_compra) as diferencia " +
            ", C.exento as exento " +
            ", C.costo_unitario as costo_unitario " +
            ", LEFT(P.nombre, 70) as producto " +
            ", P.lote as lote " +
            ", P.caducidad as caducidad " +
            " FROM compra_orden_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND compra_ordenID = " + this.hdID.Value +
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
            if ((decimal)objRowResult["diferencia"] > 0)
            {
                dr = dt.NewRow();
                dr[0] = objRowResult["productoID"].ToString();
                dr[1] = objRowResult["producto"].ToString();
                dr[2] = ((decimal)objRowResult["diferencia"]).ToString("#.###");
                dr[3] = ((decimal)objRowResult["costo_unitario"]).ToString("0.00");
                dr[4] = Convert.ToBoolean(objRowResult["exento"]) ? "1" : "0";
                dr[5] = Convert.ToBoolean(objRowResult["lote"]) ? "1" : "0";
                dr[6] = Convert.ToBoolean(objRowResult["caducidad"]) ? "1" : "0";
                if (this.hdUsuPr.Value.Equals("0"))
                    dr[7] = false;
                else
                    dr[7] = true;
                dr[8] = false;
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }

    private DataTable ObtenerPreciosVenta()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();
        decimal dcmPorcentaje = decimal.Parse(this.hdPorcentajeAumento.Value);
        decimal dcmPrecioNuevo = 0;

        dt.Columns.Add(new DataColumn("listapreciosID", typeof(string)));
        dt.Columns.Add(new DataColumn("nombrelista", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("precio_nuevo", typeof(string)));

        string strQuery = "SELECT L.ID, nombre_lista, precio_caja " +
                         " FROM listas_precios L " +
                         " LEFT JOIN precios P " +
                         " ON P.lista_precios_ID = L.ID " +
                         " AND P.producto_ID = " + this.hdProductoPrecioID.Value +
                         " AND P.validez='2099/12/31'" +
                         " WHERE L.ID = (SELECT valor FROM cat_parametros WHERE ID = 6)";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows[0].IsNull("precio_caja"))
        {
            dr = dt.NewRow();
            dr[0] = objDataResult.Tables[0].Rows[0]["ID"].ToString();
            dr[1] = objDataResult.Tables[0].Rows[0]["nombre_lista"].ToString();
            dr[2] = "$0.00";
            dr[3] = Math.Round(decimal.Parse(this.hdPrecioUnitario.Value) * dcmPorcentaje, 2).ToString("0.00");
            dt.Rows.Add(dr);
        }

        strQuery = "SELECT lista_precios_ID, nombre_lista, precio_caja " +
                  " FROM precios P " +
                  " INNER JOIN listas_precios L " +
                  " ON P.lista_precios_ID = L.ID " +
                  " AND P.producto_ID = " + this.hdProductoPrecioID.Value +
                  " AND P.validez='2099/12/31'" +
                  " AND L.tipo_lista = 'VENTAS'";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["lista_precios_ID"].ToString();
            dr[1] = objRowResult["nombre_lista"].ToString();
            dr[2] = ((decimal)objRowResult["precio_caja"]).ToString("c");
            if ((decimal)objRowResult["precio_caja"] > decimal.Parse(this.hdPrecioUnitario.Value))
                dcmPrecioNuevo = Math.Round((decimal)objRowResult["precio_caja"] * dcmPorcentaje, 2);
            else
                dcmPrecioNuevo = Math.Round(decimal.Parse(this.hdPrecioUnitario.Value) * dcmPorcentaje, 2);
            dr[3] = dcmPrecioNuevo.ToString("0.00");
            dt.Rows.Add(dr);
        }

        return dt;
    }

    private DataTable ObtenerPreciosProv()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("cobra", typeof(string)));

        string strQuery = "SELECT P.precio_caja, V.proveedor, V.cobra_paqueteria" +
                         " FROM precios P" +
                         " INNER JOIN listas_precios L" +
                         " ON L.ID = P.lista_precios_ID" +
                         " AND P.producto_ID = " + this.hdProductoID.Value +
                         " AND P.validez = '2099-12-31'" +
                         " AND L.tipo_lista = 'COMPRAS'" +
                         " AND P.precio_caja < " + this.txtPrecioUnitario.Text.Trim() +
                         " INNER JOIN proveedores V" +
                         " ON V.lista_precios_ID = L.ID" +
                         " LIMIT 15";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["proveedor"].ToString();
            dr[1] = ((decimal)objRowResult["precio_caja"]).ToString("c");
            dr[2] = ((bool)objRowResult["cobra_paqueteria"] ? "Sí" : "No");
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
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()) ||
            this.dlBusqueda.SelectedValue.Equals("6") ||
            this.dlBusqueda.SelectedValue.Equals("7"))
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
                case "5":
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
                case "6":
                case "7":
                    strCriterio.Append(".");
                    this.txtCriterio.Text = string.Empty;
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
        this.hdCobraPaq.Value = "0";
        this.hdClavePaq.Value = "0";
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lblOrden_Compra.Text = string.Empty;
        this.btnGenerar.Visible = false;
        this.btnUsarLista.Visible = false;
        this.btnFinalizar.Visible = false;
        this.btnCerrar.Visible = false;
        this.lblCorreoEnvio.Text = string.Empty;
        if (this.hdID.Value.Equals("0"))
        {
            this.txtFechaCreacion.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtFechaSurtirse.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtDescuento.Text = "0";
            this.txtComentarios.Text = string.Empty;
            this.dlPaqueteria.ClearSelection();
            this.dlPaqueteria.SelectedIndex = 0;
            this.txtNumeroEnvio.Text = string.Empty;
            this.txtFechaEnvio.Text = string.Empty;
            this.txtCostoPaq.Text = this.hdCostoPaq.Value = string.Empty;
            this.dlPaqueteria.Enabled = true;
            this.txtNumeroEnvio.Enabled = true;
            this.txtFechaEnvio.Enabled = true;
            this.txtCostoPaq.Enabled = true;
            this.txtProducto.Enabled = false;
            this.txtCantidad.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = false;
            this.btnEmail.Visible = false;
            this.btnCancelar.Visible = false;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = false;
            this.txtProveedor.Text = "";
            this.hdProveedorID.Value = string.Empty;
            this.txtProveedor.Enabled = true;
            this.dlCajero.Enabled = true;
            this.rdIVA.Enabled = true;
            this.dlListaPrecios.Enabled = true;
            this.btnFechaSurtirse.Enabled = true;
            this.rdTipo.Enabled = true;
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
            Llenar_Personas(true, 0);
        }
        else
        {
            this.lblOrden_Compra.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT proveedorID, proveedor, cajeroID, " +
                    " lista_preciosID, estatus, fecha_creacion, fecha_cancelacion, " +
                    " fecha_a_surtirse, motivo_cancelacion, comentarios, " +
                    " paqueteriaID, fecha_envio, numero_envio, " +
                    " cobra_paqueteria, costo_paqueteria," +
                    " porc_iva, O.descuento, O.contado " +
                    " FROM compra_orden O" +
                    " INNER JOIN proveedores S " +
                    " ON O.proveedorID = S.ID" +
                    " AND O.ID = " + this.hdID.Value + ";" +
                    " SELECT compraID" +
                    " FROM compra_orden_compra " +
                    " WHERE compra_ordenID = " + this.hdID.Value + ";" +
                    " SELECT fecha_envio" +
                    " FROM correo_envio" +
                    " WHERE ID = " + this.hdID.Value +
                    "   AND tipo = 3;";
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
            this.txtFechaSurtirse.Text = ((DateTime)objRowResult["fecha_a_surtirse"]).ToString("dd/MM/yyyy");

            this.dlPaqueteria.ClearSelection();
            if (objRowResult.IsNull("paqueteriaID"))
            {
                this.dlPaqueteria.SelectedIndex = 0;
                this.txtNumeroEnvio.Text = string.Empty;
                this.txtFechaEnvio.Text = string.Empty;
            }
            else
            {
                this.dlPaqueteria.Items.FindByValue(objRowResult["paqueteriaID"].ToString()).Selected = true;
                this.txtNumeroEnvio.Text = objRowResult["numero_envio"].ToString();
                if (objRowResult.IsNull("fecha_envio"))
                    this.txtFechaEnvio.Text = string.Empty;
                else
                    this.txtFechaEnvio.Text = ((DateTime)objRowResult["fecha_envio"]).ToString("dd/MM/yyyy");
            }

            this.txtComentarios.Text = objRowResult["comentarios"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Selected = true;

            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_preciosID"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["porc_iva"]).ToString("0.00")).Selected = true;

            this.txtDescuento.Text = objRowResult["descuento"].ToString();

            this.rdTipo.ClearSelection();
            this.rdTipo.Items.FindByValue(Convert.ToBoolean(objRowResult["contado"]).ToString()).Selected = true;

            if (objDataResult.Tables[2].Rows.Count > 0)
                this.lblCorreoEnvio.Text = "Correo enviado: " + ((DateTime)objDataResult.Tables[2].Rows[0]["fecha_envio"]).ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("es-MX"));

            if (!objRowResult["estatus"].ToString().Equals("1") && // Activa
                !objRowResult["estatus"].ToString().Equals("8"))   // En proceso
            {
                this.hdBorrar.Value = "0";
            }
            else
            {
                this.hdBorrar.Value = "1";
            }

            if ((bool)objRowResult["cobra_paqueteria"])
            {
                this.hdCobraPaq.Value = "1";
            }

            if (!objRowResult.IsNull("costo_paqueteria"))
                this.txtCostoPaq.Text = this.hdCostoPaq.Value = ((decimal)objRowResult["costo_paqueteria"]).ToString("0.##");
            else
                this.txtCostoPaq.Text = this.hdCostoPaq.Value = string.Empty;

            Llenar_Productos();

            if (!objRowResult["estatus"].ToString().Equals("1") &&
                !objRowResult["estatus"].ToString().Equals("8"))
            {
                this.txtProveedor.Enabled = false;
                this.dlCajero.Enabled = false;
                this.btnFechaSurtirse.Enabled = false;
                this.rdTipo.Enabled = false;
                this.txtDescuento.Enabled = false;
                this.txtProducto.Enabled = false;
                this.txtCantidad.Enabled = false;
                this.txtPrecioUnitario.Enabled = false;
                this.btnModificar.Visible = false;
                this.btnImprimir.Visible = true;
                this.btnEmail.Visible = true;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.dlListaPrecios.Enabled = false;
                this.rdIVA.Enabled = false;
                this.btnGenerar.Visible = false;
                this.dlPaqueteria.Enabled = false;
                this.txtNumeroEnvio.Enabled = false;
                this.txtFechaEnvio.Enabled = false;
                this.txtCostoPaq.Enabled = false;
                if (objRowResult["estatus"].ToString().Equals("9"))
                    this.lblMensaje.Text = "Orden de compra cancelada: " + objRowResult["motivo_cancelacion"].ToString();
                else
                {
                    this.btnGenerar.Visible = true;
                    StringBuilder strTemp = new StringBuilder();
                    foreach (DataRow objRowResult2 in objDataResult.Tables[1].Rows)
                    {
                        if (strTemp.Length > 0)
                            strTemp.Append(", ");
                        strTemp.Append(objRowResult2["compraID"].ToString());
                    }
                    this.lblMensaje.Text = "Orden de compra recibida, compra(s): " + strTemp.ToString();

                    if (objRowResult["estatus"].ToString().Equals("2")) //Parcial
                        this.btnCerrar.Visible = true;
                }
            }
            else
            {
                this.txtProveedor.Enabled = true;
                this.dlCajero.Enabled = true;
                this.btnFechaSurtirse.Enabled = true;
                this.rdTipo.Enabled = true;
                this.txtDescuento.Enabled = true;
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                if (this.hdUsuPr.Value.Equals("0"))
                    this.txtPrecioUnitario.Enabled = false;
                else
                    this.txtPrecioUnitario.Enabled = true;
                this.btnModificar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnEmail.Visible = true;
                this.btnCancelar.Visible = true;
                this.btnAgregarProd.Visible = true;
                this.btnAgregarProd.Enabled = true;
                this.dlListaPrecios.Enabled = true;
                this.rdIVA.Enabled = true;
                this.dlPaqueteria.Enabled = true;
                this.txtNumeroEnvio.Enabled = true;
                this.txtFechaEnvio.Enabled = true;
                this.txtCostoPaq.Enabled = true;
                if (this.dlEstatus.SelectedValue.Equals("8"))
                {
                    this.btnUsarLista.Visible = true;
                    this.btnFinalizar.Visible = true;
                }
            }
            string[] strValores = Obtener_Notas(this.hdProveedorID.Value).Split('|');
            this.lblNotas.Text = strValores[0];
        }
        this.rdTipo.Enabled = false;

        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdProveedorID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un proveedor de la lista");
            return;
        }

        if (this.dlPaqueteria.SelectedIndex != 0)
        {
            if (string.IsNullOrEmpty(this.txtFechaEnvio.Text.Trim()))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese la fecha de envío");
                return;
            }
        }
        else
        {
            this.txtNumeroEnvio.Text = string.Empty;
            this.txtFechaEnvio.Text = string.Empty;
        }

        if (this.hdID.Value.Equals("0"))
            Agregar_Orden();
        else
            Modificar_Orden();
    }

    private bool Validar_CobraPaq(decimal dcmTotal)
    {
        if (this.hdCobraPaq.Value.Equals("0"))
            return true;

        if (!string.IsNullOrEmpty(this.txtCostoPaq.Text.Trim()))
        {
            decimal dcmCosto;
            if (!decimal.TryParse(this.txtCostoPaq.Text, out dcmCosto))
            {
                this.txtCostoPaq.Text = this.hdCostoPaq.Value;
                ((master_MasterPage)Page.Master).MostrarMensajeError("El monto del costo de la paquetería debe ser numérico");
                return false;
            }
            this.txtCostoPaq.Text = Math.Round(dcmCosto, 2).ToString("0.##");
        }
        else
        {
            if (this.hdAccion.Value.Equals("1"))   //Se está cambiando el costo de la paqueteria
            {
                this.hdCostoPaqCam.Value = this.txtCostoPaq.Text;
                this.txtCostoPaq.Text = this.hdCosto.Value;
            }
            this.lblVerificacion.Text = "Costo de la paquetería debería ser ingresado";
            this.mdVerificacion.Show();
            return false;
        }

        if (decimal.Parse(this.txtCostoPaq.Text) > (dcmTotal * 0.5M))
        {
            if (this.hdAccion.Value.Equals("1"))   //Se está cambiando el costo de la paqueteria
            {
                this.hdCostoPaqCam.Value = this.txtCostoPaq.Text;
                this.txtCostoPaq.Text = this.hdCosto.Value;
            }
            this.lblVerificacion.Text = "Costo de la paquería es mayor al 50% del total de la orden";
            this.mdVerificacion.Show();
            return false;
        }

        this.hdCostoPaq.Value = this.txtCostoPaq.Text;

        return true;
    }

    private void Agregar_Orden()
    {
        if (Crear_Compra_Orden())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("La orden de compra ha sido creada, folio: " + this.hdID.Value);
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = true;
            this.btnEmail.Visible = true;
            this.btnCancelar.Visible = true;
            this.btnAgregarProd.Enabled = true;
            this.btnUsarLista.Visible = true;
            this.btnFinalizar.Visible = true;
        }
    }

    private bool Crear_Compra_Orden()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                         " FROM compra_orden" +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una compra de orden para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra_orden (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, fecha_a_surtirse," +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios, " +
                   "paqueteriaID, fecha_envio, numero_envio, costo_paqueteria) VALUES (" +
                   "'" + this.hdProveedorID.Value + "'" +
                   ", '" + this.dlCajero.SelectedValue + "'" +
                   ", '" + this.dlListaPrecios.SelectedValue + "'" +
                   ", '" + this.dlEstatus.SelectedValue + "'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", '" + DateTime.Parse(this.txtFechaSurtirse.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'" +
                   ", ''" +
                   ", '" + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") + "'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                   ", " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : this.dlPaqueteria.SelectedValue) +
                   ", " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : "'" + DateTime.Parse(this.txtFechaEnvio.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'") +
                   ", " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : "'" + this.txtNumeroEnvio.Text.Trim() + "'") +
                   ", " + (string.IsNullOrEmpty(this.txtCostoPaq.Text) ? "null" : this.txtCostoPaq.Text) +
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
                " FROM compra_orden " +
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
            this.lblOrden_Compra.Text = this.hdID.Value;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = true;
            return true;
        }
        return false;
    }

    private bool Modificar_Orden()
    {
        string strQuery = "UPDATE compra_orden SET " +
                         "proveedorID = " + this.hdProveedorID.Value +
                         ",cajeroID = " + this.dlCajero.SelectedValue +
                         ",lista_preciosID = " + this.dlListaPrecios.SelectedValue +
                         ",fecha_a_surtirse = '" + DateTime.Parse(this.txtFechaSurtirse.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'" +
                         ",comentarios = '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                         ",paqueteriaID = " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : this.dlPaqueteria.SelectedValue) +
                         ",fecha_envio = " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : "'" + DateTime.Parse(this.txtFechaEnvio.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'") +
                         ",numero_envio = " + (this.dlPaqueteria.SelectedIndex == 0 ? "null" : "'" + this.txtNumeroEnvio.Text.Trim() + "'") +
                         ",costo_paqueteria = " + (string.IsNullOrEmpty(this.txtCostoPaq.Text) ? "null" : this.txtCostoPaq.Text) +
                         " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        ((master_MasterPage)Page.Master).MostrarMensajeError("La orden de compra ha sido modificada");

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
            this.gvPrecioProv.DataSource = ObtenerPreciosProv();
            this.gvPrecioProv.DataBind();
            if (this.gvPrecioProv.Rows.Count > 0)
                this.mdPrecioProv.Show();
            else
                Agregar_Producto();
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad y precio unitario deben ser numéricos");
        this.txtCantidad.Focus();
    }

    protected void btnPrecioProvContinuar_Click(object sender, EventArgs e)
    {
        Agregar_Producto();
    }

    private void Agregar_Producto()
    {
        int intCantidad = int.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());
        if (this.hdCobraPaq.Value.Equals("1") &&
            this.hdClavePaq.Value.Equals("0") &&
            !this.dlEstatus.SelectedValue.Equals("8"))
        {
            this.hdAccion.Value = "2";
            string strQuery = "SELECT exento " +
                             " FROM productos " +
                             " WHERE ID = " + this.hdProductoID.Value;
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            decimal dcmDctoIva;
            if (!(bool)objDataResult.Tables[0].Rows[0]["exento"])
                dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100) *
                             (1 + decimal.Parse(this.rdIVA.SelectedValue) / 100);
            else
                dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100);
            decimal dcmTotal = decimal.Parse(this.hdTotal.Value) +
                               (dcmPrecioUnitario * intCantidad * dcmDctoIva);
            if (!Validar_CobraPaq(dcmTotal))
                return;
        }
        Agregar_Producto_Continuar();
    }

    private void Agregar_Producto_Continuar()
    {
        string strMensaje = string.Empty;
        int intCantidad = int.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());

        this.lblPrecioProducto.Text = this.txtProducto.Text;
        if (this.lblPrecioProducto.Text.Length > 30)
            this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 30);
        this.hdProductoPrecioID.Value = this.hdProductoID.Value;
        if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                              intCantidad, Math.Round(dcmPrecioUnitario, 2),
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
                    this.hdPrecioAnterior.Value = dcmPrecioUnitarioOrig.ToString();
                    this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                    this.txtPrecioUnitarioCambio.Focus();
                    this.mdCambiarPrecio.Show();
                    return;
                }
                else
                {
                    this.hdPorcentajeAumento.Value = "1";
                    Mostrar_PreciosVenta();
                }
            }
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
                    strClientScript = "setTimeout('setAgrProd()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Agregar_Producto(string strProductoID, string strProducto,
                                int intCantidad, decimal dcmCosto_unitario,
                                string listaPrecios, out string strMensaje)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        string strQuery = "SELECT 1 " +
                    " FROM compra_orden_productos " +
                    " WHERE compra_ordenID = " + this.hdID.Value +
                    " AND productoID = " + strProductoID;
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
            strMensaje = "Ya existe el producto en la orden de compra: " + strProducto;
            this.hdProductoID.Value = string.Empty;
            return false;
        }

        strQuery = "SELECT exento " +
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
                    " FROM compra_orden_productos " +
                    " WHERE compra_ordenID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO compra_orden_productos (compra_ordenID, " +
                "productoID, exento, cantidad, costo_unitario, costo," +
                "consecutivo, cantidad_compra) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + intCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", 0" +
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
                e.Row.Cells[9].Controls.Clear();
            }
            else
            {
                if (e.Row.RowIndex == 0)
                    ((ImageButton)e.Row.Cells[1].Controls[1]).Enabled = false;
                if (e.Row.RowIndex.ToString().Equals(this.hdConsMaxID.Value))
                    ((ImageButton)e.Row.Cells[1].Controls[3]).Enabled = false;
            }
            if (((HiddenField)e.Row.Cells[10].FindControl("hdParcial")).Value.Equals("1"))
            {
                e.Row.Cells[2].ForeColor = e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
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

            this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString() + "_" +
                                         ((ImageButton)this.gvProductos.Rows[index].Cells[1].Controls[1]).CommandArgument;

            if (this.hdCobraPaq.Value.Equals("1") &&
               this.hdClavePaq.Value.Equals("0") &&
               !this.dlEstatus.SelectedValue.Equals("8"))
            {
                decimal dcmDctoIva;
                if (this.gvProductos.Rows[index].Cells[2].Text.EndsWith("*"))
                    dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100) *
                                 (1 + decimal.Parse(this.rdIVA.SelectedValue) / 100);
                else
                    dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100);
                this.hdAccion.Value = "4";
                decimal dcmTotal = decimal.Parse(this.hdTotal.Value) -
                                  (decimal.Parse(this.gvProductos.Rows[index].Cells[6].Text.Replace("$", "").Replace(",", "")) * dcmDctoIva);
                if (!Validar_CobraPaq(dcmTotal))
                    return;
            }

            Borrar_Producto_Continuar();
        }
        else
            if (e.CommandName == "Modificar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                this.lblCambiarProducto.Text = this.gvProductos.Rows[index].Cells[2].Text;
                this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[4].Text;
                this.txtCambiarUnitario.Text = this.hdPrecioUnitario.Value = this.gvProductos.Rows[index].Cells[5].Text.Replace("$", "").Replace(",", "");
                this.hdPrecioAnterior.Value = this.txtCambiarUnitario.Text;
                this.hdCambiarCosto.Value = this.gvProductos.Rows[index].Cells[6].Text.Replace("$", "").Replace(",", "");
                this.mdCambiarProducto.Show();
                string strClientScript = "setTimeout('setProductoCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
            }
    }

    private void Borrar_Producto_Continuar()
    {
        string[] strID = this.hdConsecutivoID.Value.Split('_');
        string strQuery = "DELETE " +
                    " FROM compra_orden_productos " +
                    " WHERE compra_ordenID = " + this.hdID.Value +
                    " AND productoID = " + strID[0];
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "UPDATE compra_orden_productos SET " +
                " consecutivo = consecutivo - 1 " +
                " WHERE compra_ordenID = " + this.hdID.Value +
                " AND consecutivo > " + strID[1];

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Llenar_Productos();
    }

    private void Llenar_Productos()
    {
        this.hdCosto.Value = "0";
        this.hdCostoIVA.Value = "0";
        this.hdIVA.Value = "0.00";
        this.hdTotal.Value = "0";

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        string strQuery = "UPDATE compra_orden SET " +
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

        if (this.gvProductos.Rows.Count > 0 &&
            (this.dlEstatus.SelectedValue.Equals("1") ||
             this.dlEstatus.SelectedValue.Equals("2")))
            this.btnGenerar.Visible = true;
        else
            this.btnGenerar.Visible = false;
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
        this.btnEmail.Visible = false;
        this.btnCancelar.Visible = false;
        this.btnAgregarProd.Enabled = false;
        this.dlCajero.Enabled = false;
        this.btnFechaSurtirse.Enabled = false;
        this.rdTipo.Enabled = false;
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
            dcmCantidad = Math.Round(dcmCantidad, 2);
            dcmPrecio = Math.Round(dcmPrecio, 2);

            if (this.hdCobraPaq.Value.Equals("1") &&
               this.hdClavePaq.Value.Equals("0") &&
               !this.dlEstatus.SelectedValue.Equals("8"))
            {
                decimal dcmDctoIva;
                if (this.lblCambiarProducto.Text.EndsWith("*"))
                    dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100) *
                                 (1 + decimal.Parse(this.rdIVA.SelectedValue) / 100);
                else
                    dcmDctoIva = (1 - decimal.Parse(this.txtDescuento.Text) / 100);
                this.hdAccion.Value = "3";
                decimal dcmTotal = decimal.Parse(this.hdTotal.Value) -
                                  (decimal.Parse(this.hdCambiarCosto.Value) * dcmDctoIva) +
                                  (dcmPrecio * dcmCantidad * dcmDctoIva);
                if (!Validar_CobraPaq(dcmTotal))
                    return;
            }

            Cambiar_Producto_Continuar();
        }
    }

    private void Cambiar_Producto_Continuar()
    {
        decimal dcmCantidad = Math.Round(decimal.Parse(this.txtCambiarCantidad.Text), 2);
        decimal dcmPrecio = Math.Round(decimal.Parse(this.txtCambiarUnitario.Text), 2);

        string strQuery = "UPDATE compra_orden_productos SET " +
                    "cantidad = " + dcmCantidad.ToString() +
                    ",costo_unitario = " + dcmPrecio.ToString() +
                    ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                    " WHERE compra_ordenID = " + this.hdID.Value +
                    " AND productoID = " + this.hdConsecutivoID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);
        Llenar_Productos();

        this.hdProductoPrecioID.Value = this.hdConsecutivoID.Value;
        this.hdPrecioUnitario.Value = dcmPrecio.ToString("0.00");
        this.hdPorcentajeAumento.Value = (dcmPrecio / decimal.Parse(this.hdPrecioAnterior.Value)).ToString();
        Mostrar_PreciosVenta();
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
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

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

    protected void btnCerrar_Click(object sender, EventArgs e)
    {
        string strQuery = "UPDATE compra_orden SET " +
                          "estatus = 3" +
                         " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Mostrar_Datos();

        ((master_MasterPage)Page.Master).MostrarMensajeError("La orden de compra ha sido cerrada");
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        Modificar_Orden();

        this.hdAccion.Value = "0";
        if(!Validar_CobraPaq(decimal.Parse(this.hdTotal.Value)))
            return;

        Finalizar_Continuar();
    }

    private void Finalizar_Continuar()
    {
        this.dlEstatus.ClearSelection();
        this.dlEstatus.Items.FindByValue("1").Selected = true;
        this.btnUsarLista.Visible = false;
        this.btnFinalizar.Visible = false;

        string strQuery = "UPDATE compra_orden SET " +
                          "estatus = 1" +
                         " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        ((master_MasterPage)Page.Master).MostrarMensajeError("La orden de compra ha sido modificada");
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarReporte('compra_excel.aspx?t=o&c=" +
                                        this.hdID.Value +
                                        "')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void btnEmail_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarMailPopUp('compra_excel.aspx?m=Y&t=o&c=" +
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

    protected void txtCostoPaq_TextChanged(object sender, EventArgs e)
    {
        this.hdAccion.Value = "1";

        if (!Validar_CobraPaq(decimal.Parse(this.hdTotal.Value)))
        {
            this.txtCostoPaq.Text = this.hdCostoPaq.Value;
            return;
        }

        if (!this.dlEstatus.SelectedValue.Equals("8"))
            Modificar_Orden();
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                "setTimeout('setLista()',50);",
                                                true);
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

        switch (this.hdAccion.Value)
        {
            case "0":       // Finalizar Compra
                Finalizar_Continuar();
                break;
            case "1":       //Se cambió el costo de la paquetería
                this.txtCostoPaq.Text = this.hdCostoPaq.Value = hdCostoPaqCam.Value;
                Modificar_Orden();
                break;
            case "2":       // Se agregó producto
                this.hdClavePaq.Value = "1";
                Agregar_Producto_Continuar();
                break;
            case "3":       // Se cambia el producto
                this.hdClavePaq.Value = "1";
                Cambiar_Producto_Continuar();
                break;
            case "4":       // Se borra un producto
                this.hdClavePaq.Value = "1";
                Borrar_Producto_Continuar();
                break;
        }

        StringBuilder strTexto = new StringBuilder();
        try
        {
            StringWriter swTexto = new StringWriter(strTexto);
            HtmlTextWriter twTexto = new HtmlTextWriter(swTexto);
            this.pnlVerificacion.RenderControl(twTexto);
        }
        catch
        {
            strTexto = new StringBuilder();
        }

        CRutinas.Enviar_Correo("6",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Orden compra: " + this.hdID.Value + "<br />" +
                               "Proveedor: " + this.txtProveedor.Text + "<br />" +
                               "Razón: Costo de la paquería es mayor al 50% del total de la orden" + "<br /><br />" +
                               strTexto.ToString().Replace("display:none;", ""));
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

    [System.Web.Services.WebMethod]
    public static string Obtener_Notas(string strParametros)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.notas, E.descuento, " +
                    " E.iva, E.lista_precios_ID, E.contado " +
                    ",E.cobra_paqueteria" +
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
               "|" + ((bool)objRowResult["contado"] ? "1" : "0") +
               "|" + ((bool)objRowResult["cobra_paqueteria"] ? "1" : "0");
    }

    protected void btnPrecioContinuar_Click(object sender, EventArgs e)
    {
        double dblProcentaje = 1;
        decimal dcmPrecioUnitario = 0;
        if (decimal.TryParse(this.txtPrecioUnitarioCambio.Text.Trim(), out dcmPrecioUnitario))
        {
            decimal dcmPrecio = CComunDB.CCommun.Actualizar_Precio(this.hdProductoPrecioID.Value,
                                                                   this.dlListaPrecios.SelectedValue,
                                                                   dcmPrecioUnitario);

            if(double.Parse(this.hdPrecioAnterior.Value) != 0)
                dblProcentaje = (double)dcmPrecio / double.Parse(this.hdPrecioAnterior.Value);
        }
        this.hdPrecioUnitario.Value = this.txtPrecioUnitarioCambio.Text;
        this.hdPorcentajeAumento.Value = dblProcentaje.ToString();
        Mostrar_PreciosVenta();
    }

    private void Mostrar_PreciosVenta()
    {
        string strQuery = "SELECT utilidad" +
                         " FROM listas_precios L" +
                         " INNER JOIN proveedores P" +
                         " ON L.ID = P.lista_precios_id" +
                         " AND L.ID = " + this.dlListaPrecios.SelectedValue +
                         " LIMIT 1";
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0 && (decimal)objDataResult.Tables[0].Rows[0]["utilidad"] > 0)
            this.hdPorcentajeAumento.Value = (1 + Math.Round((decimal)objDataResult.Tables[0].Rows[0]["utilidad"] / 100, 2)).ToString();

        this.gvListasPrecios.DataSource = ObtenerPreciosVenta();
        this.gvListasPrecios.DataBind();
        if (gvListasPrecios.Rows.Count > 0)
        {
            this.mdListasPrecio.Show();
            if (gvListasPrecios.Rows.Count > 15)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", "setTimeout('ajustarDivLista(true)',100);", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", "setTimeout('ajustarDivLista(false)',100);", true);
        }
    }

    protected void btnListasPreciosContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmPiezaPorCaja = 1;
        decimal dcmPrecio = 0;
        decimal dcmPrecioUnitario = 0;
        foreach (GridViewRow gvRow in this.gvListasPrecios.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (decimal.TryParse(((TextBox)gvRow.FindControl("txtPrecioNuevo")).Text.Trim(), out dcmPrecio))
                {
                    dcmPrecio = Math.Round(dcmPrecio, 2);
                    dcmPrecioUnitario = Math.Round(dcmPrecio / dcmPiezaPorCaja, 2);
                    Actualizar_Precio(this.gvListasPrecios.DataKeys[gvRow.RowIndex].Value.ToString(),
                                      dcmPrecio.ToString(), dcmPrecioUnitario.ToString());
                }
            }
        }
    }

    private void Actualizar_Precio(string strListaPrecioID, string strPrecio, string strPrecioUnitario)
    {
        CComunDB.CCommun.Actualizar_Precio(this.hdProductoPrecioID.Value,
                                           strListaPrecioID,
                                           decimal.Parse(strPrecioUnitario));
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
        string strQuery = "UPDATE compra_orden_productos SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND compra_ordenID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE compra_orden_productos SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND compra_ordenID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE compra_orden_productos SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND compra_ordenID = " + this.hdID.Value;

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

    protected void btnGenerar_Click(object sender, EventArgs e)
    {
        this.lblOrden_CompraDatos.Text = this.lblOrden_Compra.Text;
        this.gvProductosCompra.DataSource = ObtenerProductosCompra();
        this.gvProductosCompra.DataBind();

        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlCompraDatos.Visible = true;
    }

    protected void gvProductosCompra_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            (((TextBox)e.Row.FindControl("txtFechaDatos"))).Attributes["readonly"] = "true";
        }
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
                    if (((HiddenField)gvRow.FindControl("hdLote")).Value.Equals("1") &&
                        string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtLoteDatos")).Text.Trim()))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Lote debe ser ingresado para este producto: " + ((Label)gvRow.FindControl("lblProdDatos")).Text);
                        return;
                    }

                    if (((HiddenField)gvRow.FindControl("hdCaducidad")).Value.Equals("1"))
                    {
                        if (string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtFechaDatos")).Text))
                        {
                            ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad debe ser ingresado para este producto: " + ((Label)gvRow.FindControl("lblProdDatos")).Text);
                            return;
                        }
                        if (DateTime.Parse(((TextBox)gvRow.FindControl("txtFechaDatos")).Text, CultureInfo.CreateSpecificCulture("es-MX")) <= DateTime.Today)
                        {
                            ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad no puede ser igual o menor al día de hoy para este producto: " + ((Label)gvRow.FindControl("lblProdDatos")).Text);
                            return;
                        }
                    }
                }
                ((TextBox)gvRow.FindControl("txtCantidadDatos")).Text = intCantidad.ToString();
                ((TextBox)gvRow.FindControl("txtCostoDatos")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();
            }
        }

        if (Crear_Compra())
        {
            string strQuery = "INSERT INTO compra_orden_compra (" +
                            "compra_ordenID " +
                            ",compraID" +
                            ") VALUES(" +
                            this.hdID.Value +
                            "," + this.hdCompraID.Value +
                            ")";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {
            }

            bool swFaltante = false;
            int intConsecutivo = 1;
            decimal dcmCantidad, dcmCantidad_Compra, dcmCosto_unitario, dcmCosto;
            string strFecha;
            foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
            {
                dcmCantidad = decimal.Parse(((Label)gvRow.FindControl("lblCantDatos")).Text);
                dcmCantidad_Compra = decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text);
                strQuery = "UPDATE compra_orden_productos SET " +
                           "cantidad_compra = cantidad_compra + " + dcmCantidad_Compra.ToString() +
                           " WHERE compra_ordenID = " + this.hdID.Value +
                           " AND productoID = " + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString();
                if ((dcmCantidad_Compra - dcmCantidad) < 0)
                    swFaltante = true;
                try
                {
                    CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch
                {
                }

                if (dcmCantidad > 0)
                {
                    dcmCosto_unitario = decimal.Parse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text);
                    dcmCosto = Math.Round(dcmCosto_unitario * (decimal)dcmCantidad_Compra, 2);
                    if (string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtFechaDatos")).Text))
                        strFecha = "null";
                    else
                        strFecha = "'" + DateTime.Parse(((TextBox)gvRow.FindControl("txtFechaDatos")).Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'";
                    strQuery = "INSERT INTO compra_productos (compraID, " +
                               "productoID, exento, cantidad, costo_unitario, costo," +
                               "fecha_caducidad, lote, consecutivo) VALUES (" +
                               "'" + this.hdCompraID.Value + "'" +
                               ", '" + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString() + "'" +
                               ", '" + ((HiddenField)gvRow.FindControl("hdExento")).Value + "'" +
                               ", '" + dcmCantidad_Compra + "'" +
                               ", '" + dcmCosto_unitario + "'" +
                               ", '" + dcmCosto + "'" +
                               ", " + strFecha +
                               ", '" + ((TextBox)gvRow.FindControl("txtLoteDatos")).Text + "'" +
                               ", '" + intConsecutivo + "'" +
                               ")";
                    CComunDB.CCommun.Ejecutar_SP(strQuery);
                    if (((CheckBox)gvRow.FindControl("chkAct")).Checked)
                    {
                        CComunDB.CCommun.Actualizar_Precio(this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                           this.dlListaPrecios.SelectedValue,
                                                           dcmCosto_unitario);
                    }
                    intConsecutivo++;
                }
            }

            if (swFaltante)
                strQuery = "UPDATE compra_orden SET " +
                    "estatus = 2" +
                    " WHERE ID = " + this.hdID.Value;
            else
                strQuery = "UPDATE compra_orden SET " +
                    "estatus = 3" +
                    " WHERE ID = " + this.hdID.Value;
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }

            ((master_MasterPage)Page.Master).MostrarMensajeError("Compra ha sido creada: " + this.hdCompraID.Value);
            Mostrar_Datos();
            this.pnlCompraDatos.Visible = false;
            this.pnlDatos.Visible = true;
            this.pnlDatos2.Visible = true;
        }
    }

    private bool Crear_Compra()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM compra " +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una compra para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, " +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios" +
                   ",creadoPorID, creadoPorFecha, modificadoPorID, modificadoPorFecha" +
                   ") VALUES (" +
                   "'" + this.hdProveedorID.Value + "'" +
                   ", '" + this.dlCajero.SelectedValue + "'" +
                   ", '" + this.dlListaPrecios.SelectedValue + "'" +
                   ", '8'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", '" + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") + "'" +
                   ", '" + Math.Round(decimal.Parse(this.hdCosto.Value), 2) + "'" +
                   ", '" + this.txtDescuento.Text + "'" +
                   ", '" + Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2) + "'" +
                   ", '" + this.rdIVA.SelectedValue + "'" +
                   ", '" + Math.Round(decimal.Parse(this.hdIVA.Value), 2) + "'" +
                   ", '" + Math.Round(decimal.Parse(this.hdTotal.Value), 2) + "'" +
                   ", ''" +
                   ", '" + Session["SIANID"].ToString() + "'" +
                   ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '" + Session["SIANID"].ToString() + "'" +
                   ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
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
                " FROM compra " +
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
            this.hdCompraID.Value = objRowResult["ID"].ToString();
            return true;
        }
        ((master_MasterPage)Page.Master).MostrarMensajeError("No se generó la compra, intente de nuevo");
        return false;
    }

    protected void btnRegresarDatos_Click(object sender, EventArgs e)
    {
        this.pnlCompraDatos.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    protected void btnUsarLista_Click(object sender, EventArgs e)
    {
        string strQuery = "SELECT S.producto_ID, S.precio_caja, (L.maximo - D.existencia) as cantidad" +
                         " FROM precios S" +
                         " INNER JOIN proveedores P" +
                         " ON S.lista_precios_ID = P.lista_precios_ID" +
                         " AND P.ID = " + this.hdProveedorID.Value +
                         " AND S.validez = '2099-12-31'" +
                         " INNER JOIN producto_datos D" +
                         " ON D.productoID = S.producto_ID" +
                         " LEFT JOIN producto_limites L" +
                         " ON L.productoID = S.producto_ID" +
                         " WHERE (L.maximo - D.existencia) > 0";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        string strMensaje = string.Empty;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            Agregar_Producto(objRowResult["producto_ID"].ToString(),
                             string.Empty,
                             (int)(decimal)objRowResult["cantidad"],
                             Math.Round((decimal)objRowResult["precio_caja"]),
                             this.dlListaPrecios.SelectedValue,
                             out strMensaje);
        }
        Llenar_Productos();
        ((master_MasterPage)Page.Master).MostrarMensajeError("Productos fueron agregados");
    }
}