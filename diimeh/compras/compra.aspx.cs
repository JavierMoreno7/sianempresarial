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

public partial class compras_compra : BasePage
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
        this.txtCambiarFecha.Attributes["readonly"] = "true";
        this.txtFechaCaducidad.Attributes["readonly"] = "true";
        this.txtPago.Attributes["readonly"] = "true";
        this.txtFechaPago.Attributes["readonly"] = "true";
        this.txtFechaCambio.Attributes["readonly"] = "true";

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

            if (!CComunDB.CCommun.ValidarAcceso(10010, out swVer, out swTot))
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

        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerCompraEstatus(false);
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
        DataTable dtResultado = ObtenerCompras();

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

    private DataTable ObtenerCompras()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("compraID", typeof(string)));
        dt.Columns.Add(new DataColumn("proveedor", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_creacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));

        string strQuery = "CALL leer_compras_consulta(" +
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
            dr[0] = objRowResult["compraID"].ToString();
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
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("reqfecha", typeof(string)));
        dt.Columns.Add(new DataColumn("reqlote", typeof(string)));
        dt.Columns.Add(new DataColumn("inv", typeof(string)));

        string strQuery = "SELECT * FROM (" +
                         "SELECT C.productoID as productoID " +
                         ", C.cantidad as cantidad " +
                         ", C.consecutivo as consecutivo " +
                         ", C.costo_unitario as costo_unitario " +
                         ", C.costo as costo " +
                         ", C.exento as exento " +
                         ", C.fecha_caducidad as fecha " +
                         ", C.lote as lote " +
                         ", C.inventariado" +
                         ", LEFT(P.nombre, 70) as producto " +
                         ", P.codigo as codigo " +
                         ", P.lote as reqlote " +
                         ", P.caducidad as reqfecha " +
                         " FROM compra_productos C " +
                         " INNER JOIN productos P " +
                         " ON C.productoID = P.ID " +
                         " AND compraID = " + this.hdID.Value +
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
            if (!objRowResult.IsNull("fecha"))
                dr[8] = ((DateTime)objRowResult["fecha"]).ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("es-MX"));
            else
                dr[8] = string.Empty;
            dr[9] = objRowResult["lote"].ToString();
            dr[10] = Convert.ToBoolean(objRowResult["reqlote"]) ? "1" : "0";
            dr[11] = Convert.ToBoolean(objRowResult["reqfecha"]) ? "1" : "0";
            dr[12] = Convert.ToBoolean(objRowResult["inventariado"]) ? "1" : "0";
            dt.Rows.Add(dr);
        }

        dcmCosto = Math.Round(dcmCosto, 2);
        dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

        this.hdCosto.Value = dcmCosto.ToString();
        this.hdCostoIVA.Value = dcmCostoIVA.ToString();

        return dt;
    }

    private DataTable ObtenerProductosOrden(List<COrden> lstOrden)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dt.Columns.Add(new DataColumn("exento", typeof(string)));
        dt.Columns.Add(new DataColumn("loteR", typeof(string)));
        dt.Columns.Add(new DataColumn("caducidadR", typeof(string)));
        dt.Columns.Add(new DataColumn("enabled", typeof(bool)));
        dt.Columns.Add(new DataColumn("productoChk", typeof(bool)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("caducidad", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad2", typeof(string)));
        dt.Columns.Add(new DataColumn("repetir", typeof(string)));
        dt.Columns.Add(new DataColumn("enabled2", typeof(bool)));

        if (lstOrden.Count == 0)
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT * FROM (" +
                "SELECT C.productoID as productoID " +
                ", C.consecutivo as consecutivo " +
                ", C.cantidad as cantidad " +
                ", C.exento as exento " +
                ", C.costo_unitario as costo_unitario " +
                ", LEFT(P.nombre, 70) as producto " +
                ", P.lote as lote " +
                ", P.caducidad as caducidad " +
                " FROM compra_orden_productos C " +
                " INNER JOIN productos P " +
                " ON C.productoID = P.ID " +
                " AND compra_ordenID = " + this.txtOrden_CompraID.Text +
                ") AS AA ORDER BY consecutivo, producto";

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
            {
                COrden objOrden = new COrden();
                objOrden.intProductoID = (int)objRowResult["productoID"];
                objOrden.strProducto = objRowResult["producto"].ToString();
                objOrden.dcmCantidad = (decimal)objRowResult["cantidad"];
                objOrden.intRecibida = (int)(decimal)objRowResult["cantidad"];
                objOrden.dcmPrecio = (decimal)objRowResult["costo_unitario"];
                objOrden.swExento = (bool)objRowResult["exento"];
                objOrden.swLote = (bool)objRowResult["lote"];
                objOrden.swCaducidad = (bool)objRowResult["caducidad"];
                if (this.hdUsuPr.Value.Equals("0"))
                    objOrden.swPrecioEnabled = objOrden.swPrecioChkEnabled = false;
                else
                    objOrden.swPrecioEnabled = objOrden.swPrecioChkEnabled = true;
                objOrden.swPrecioAct = false;

                objOrden.strLote = objOrden.strCaducidad  = string.Empty;
                objOrden.swRepetir = true;
                lstOrden.Add(objOrden);
            }
        }

        int intProdAnt = 0;
        foreach (COrden objOrden in lstOrden)
        {
            dr = dt.NewRow();
            dr[0] = objOrden.intProductoID.ToString();
            dr[1] = objOrden.strProducto;
            dr[2] = objOrden.dcmCantidad.ToString("#.##");
            dr[3] = objOrden.dcmPrecio.ToString("0.00");
            dr[7] = objOrden.swPrecioEnabled;
            if (intProdAnt != objOrden.intProductoID)
            {
                dr[13] = objOrden.swPrecioChkEnabled;
                dr[8] = objOrden.swPrecioAct;
                intProdAnt = objOrden.intProductoID;
            }
            else
            {
                dr[13] = false;
                dr[8] = false;
            }

            dr[4] = (objOrden.swExento ? "1" : "0");
            dr[5] = (objOrden.swLote ? "1" : "0");
            dr[6] = (objOrden.swCaducidad ? "1" : "0");
            dr[9] = objOrden.strLote;
            dr[10] = objOrden.strCaducidad;
            dr[11] = objOrden.intRecibida.ToString("#");
            dr[12] = (objOrden.swRepetir ? "1" : "0");
            dt.Rows.Add(dr);
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
        else
            if (e.CommandName == "Pagos")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                Mostrar_Pagos(this.grdvLista.DataKeys[index].Value.ToString());
            }
    }

    private void Mostrar_Pagos(string strCompraID)
    {
        this.lblPagos.Text = "Pagos de la compra " + strCompraID;
        this.gvPagos.DataSource = ObtenerPagos(strCompraID);
        this.gvPagos.DataBind();
        this.mdCambiarPagos.Show();
    }

    private DataTable ObtenerPagos(string strCompraID)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("tipo", typeof(string)));
        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));

        string strQuery = "SELECT T.tipo_pago, P.referencia, P.fecha_pago, F.monto_pago" +
                         " FROM pago_compras F" +
                         " INNER JOIN pagos P" +
                         " ON P.ID = F.pagoID" +
                         " AND F.compraID = " + strCompraID +
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
            dr[2] = ((DateTime)objRowResult["fecha_pago"]).ToString("dd/MMM/yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
            dr[3] = ((decimal)objRowResult["monto_pago"]).ToString("c");
            dt.Rows.Add(dr);
        }

        return dt;
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
        this.lblCreado.Text = string.Empty;
        this.lblCompra.Text = string.Empty;
        this.txtFechaCaducidad.Text = string.Empty;
        this.lblCorreoEnvio.Text = string.Empty;
        if (this.hdID.Value.Equals("0"))
        {
            this.txtFechaCreacion.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtDescuento.Text = "0";
            this.txtComentarios.Text = string.Empty;
            this.txtComentarios.Enabled = true;
            this.txtFactura.Text = string.Empty;
            this.txtFactura.Enabled = true;
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
            this.btnUsarOrden.Visible = true;
            this.btnFinalizar.Visible = false;
            this.txtLote.Enabled = false;
            this.btnFechaCaducidad.Enabled = false;
            Llenar_Personas(true, 0);
        }
        else
        {
            this.lblCompra.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT proveedorID, proveedor, cajeroID, " +
                    " lista_preciosID, estatus, fecha_creacion, fecha_cancelacion, " +
                    " motivo_cancelacion, comentarios, factura, " +
                    " porc_iva, O.descuento, O.contado " +
                    " FROM compra O" +
                    " INNER JOIN proveedores S " +
                    " ON O.proveedorID = S.ID " +
                    " WHERE O.ID = " + this.hdID.Value + ";" +
                    " SELECT fecha_envio" +
                    " FROM correo_envio" +
                    " WHERE ID = " + this.hdID.Value +
                    "   AND tipo = 4;";
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
            this.txtFactura.Text = objRowResult["factura"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Selected = true;

            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_preciosID"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["porc_iva"]).ToString("0.00")).Selected = true;

            this.txtDescuento.Text = objRowResult["descuento"].ToString();

            this.rdTipo.ClearSelection();
            this.rdTipo.Items.FindByValue(Convert.ToBoolean(objRowResult["contado"]).ToString()).Selected = true;

            if (objDataResult.Tables[1].Rows.Count > 0)
                this.lblCorreoEnvio.Text = "Correo enviado: " + ((DateTime)objDataResult.Tables[1].Rows[0]["fecha_envio"]).ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("es-MX"));

            if (!objRowResult["estatus"].ToString().Equals("8"))
                this.hdBorrar.Value = "0";
            else
                this.hdBorrar.Value = "1";

            Llenar_Productos(false);
            Obtener_CreadoPor();

            if (!objRowResult["estatus"].ToString().Equals("8"))
            {
                this.txtProveedor.Enabled = false;
                this.dlCajero.Enabled = false;
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
                this.btnUsarOrden.Visible = false;
                this.txtLote.Enabled = false;
                this.btnFechaCaducidad.Enabled = false;
                this.btnFinalizar.Visible = false;
                this.txtFactura.Enabled = false;
                this.txtComentarios.Enabled = false;
                if (objRowResult["estatus"].ToString().Equals("9"))
                    this.lblMensaje.Text = "Orden de compra cancelada: " + objRowResult["motivo_cancelacion"].ToString();
            }
            else
            {
                this.txtFactura.Enabled = true;
                this.txtComentarios.Enabled = true;
                this.txtProveedor.Enabled = true;
                this.dlCajero.Enabled = true;
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
                this.btnUsarOrden.Visible = true;
                this.txtLote.Enabled = true;
                this.btnFechaCaducidad.Enabled = true;
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
        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Compra();
        }
        else
        {
            Modificar_Compra();
            ((master_MasterPage)Page.Master).MostrarMensajeError("La compra ha sido modificada");
        }
    }

    private void Agregar_Compra()
    {
        if (string.IsNullOrEmpty(this.hdProveedorID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un proveedor de la lista");
            return;
        }

        if (Crear_Compra())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("La compra ha sido creada, folio: " + this.hdID.Value);
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
            this.txtLote.Enabled = true;
            this.btnFechaCaducidad.Enabled = true;
            Obtener_CreadoPor();
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una compra de orden para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion," +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios" +
                   ",factura" +
                   ",creadoPorID, creadoPorFecha, modificadoPorID, modificadoPorFecha" +
                   ") VALUES (" +
                   "'" + this.hdProveedorID.Value + "'" +
                   ", '" + this.dlCajero.SelectedValue + "'" +
                   ", '" + this.dlListaPrecios.SelectedValue + "'" +
                   ", '" + this.dlEstatus.SelectedValue + "'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", '" + (Convert.ToBoolean(this.rdTipo.SelectedValue) ? "1" : "0") + "'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '0'" +
                   ", '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                   ", '" + this.txtFactura.Text.Trim() + "'" +
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
            this.hdID.Value = objRowResult["ID"].ToString();
            this.lblCompra.Text = this.hdID.Value;
            return true;
        }
        return false;
    }

    private bool Modificar_Compra()
    {
        string strQuery = "UPDATE compra SET " +
                    "proveedorID = " + this.hdProveedorID.Value +
                    ",cajeroID = " + this.dlCajero.SelectedValue +
                    ",lista_preciosID = " + this.dlListaPrecios.SelectedValue +
                    ",comentarios = '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                    ",factura = '" + this.txtFactura.Text.Trim() + "'" +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();
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

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT lote, caducidad " +
                         " FROM productos " +
                         " WHERE ID = " + this.hdProductoID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if(Convert.ToBoolean(objDataResult.Tables[0].Rows[0]["lote"]) &&
           string.IsNullOrEmpty(this.txtLote.Text.Trim()))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Lote debe ser ingresado para este producto");
            return;
        }

        if (Convert.ToBoolean(objDataResult.Tables[0].Rows[0]["caducidad"]) &&
           string.IsNullOrEmpty(this.txtFechaCaducidad.Text.Trim()))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad debe ser ingresada para este producto");
            return;
        }

        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
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
        string strFecha;
        if (string.IsNullOrEmpty(this.txtFechaCaducidad.Text.Trim()))
            strFecha = "null";
        else
        {
            if (DateTime.Parse(this.txtFechaCaducidad.Text, CultureInfo.CreateSpecificCulture("es-MX")) <= DateTime.Today)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad no puede ser menor o igual al día actual");
                return;
            }
            strFecha = "'" + DateTime.Parse(this.txtFechaCaducidad.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'";
        }

        int intCantidad = int.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());
        string strMensaje = string.Empty;
        this.lblPrecioProducto.Text = this.txtProducto.Text;
        if (this.lblPrecioProducto.Text.Length > 30)
            this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 30);
        this.hdProductoPrecioID.Value = this.hdProductoID.Value;
        if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                intCantidad, Math.Round(dcmPrecioUnitario, 2),
                                strFecha, this.txtLote.Text.Trim(),
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
                    strClientScript = "setTimeout('setLote()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Agregar_Producto(string strProductoID, string strProducto,
                                int intCantidad, decimal dcmCosto_unitario,
                                string strFechaCaducidad, string strLote,
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
                    " FROM compra_productos " +
                    " WHERE compraID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO compra_productos (compraID, " +
                "productoID, exento, cantidad, costo_unitario, costo," +
                "fecha_caducidad, lote, " +
                "consecutivo) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + intCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", " + strFechaCaducidad +
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
        this.txtFechaCaducidad.Text = string.Empty;
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
                e.Row.Cells[9].Controls.Clear();
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

        int index = Convert.ToInt32(e.CommandArgument);
        string[] strID_Consecutivo = this.gvProductos.DataKeys[index].Value.ToString().Split('_');

        if (((HiddenField)this.gvProductos.Rows[index].Cells[1].Controls[11]).Value.Equals("1"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Producto no puede modificarse porque ya fue inventariado");
            return;
        }

        if (e.CommandName == "Borrar")
        {
            string strQuery = "DELETE " +
                    " FROM compra_productos " +
                    " WHERE compraID = " + this.hdID.Value +
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

            strQuery = "UPDATE compra_productos SET " +
                    " consecutivo = consecutivo - 1 " +
                    " WHERE compraID = " + this.hdID.Value +
                    " AND consecutivo > " + strID_Consecutivo[1];

            CComunDB.CCommun.Ejecutar_SP(strQuery);

            Llenar_Productos(true);
        }
        else
            if (e.CommandName == "Modificar")
            {
                this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                this.lblCambiarProducto.Text = this.gvProductos.Rows[index].Cells[2].Text;
                this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[4].Text;
                this.txtCambiarUnitario.Text = this.hdPrecioUnitario.Value = this.gvProductos.Rows[index].Cells[5].Text.Replace("$", "").Replace(",", "");
                this.hdPrecioAnterior.Value = this.txtCambiarUnitario.Text;
                this.txtCambiarLote.Text = this.gvProductos.Rows[index].Cells[7].Text.Replace("&nbsp;", "");
                if (this.gvProductos.Rows[index].Cells[8].Text.Equals("&nbsp;"))
                    this.txtCambiarFecha.Text = string.Empty;
                else
                    this.txtCambiarFecha.Text = DateTime.Parse(this.gvProductos.Rows[index].Cells[8].Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("dd/MM/yyyy");
                this.hdModLote.Value = ((HiddenField)this.gvProductos.Rows[index].Cells[1].Controls[7]).Value;
                this.hdModCaducidad.Value = ((HiddenField)this.gvProductos.Rows[index].Cells[1].Controls[9]).Value;
                this.mdCambiarProducto.Show();
                string strClientScript = "setTimeout('setProductoCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
            }
    }

    private void Llenar_Productos(bool swGuardarCompra)
    {
        this.hdCosto.Value = "0";
        this.hdCostoIVA.Value = "0";
        this.hdIVA.Value = "0.00";
        this.hdTotal.Value = "0";

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        string strQuery = "UPDATE compra SET " +
                        "porc_iva = " + this.rdIVA.SelectedValue +
                        ",descuento = " + this.txtDescuento.Text +
                        ",monto_subtotal = " + Math.Round(decimal.Parse(this.hdCosto.Value), 2) +
                        ",descuento = " + this.txtDescuento.Text +
                        ",monto_descuento = " + Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2) +
                        ",porc_iva = " + this.rdIVA.SelectedValue +
                        ",monto_iva = " + Math.Round(decimal.Parse(this.hdIVA.Value), 2) +
                        ",total = " + Math.Round(decimal.Parse(this.hdTotal.Value), 2);

        if (swGuardarCompra)
            strQuery += ",modificadoPorID = " + Session["SIANID"].ToString() +
                        ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        strQuery += " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        if (this.gvProductos.Rows.Count > 0)
            this.btnFinalizar.Visible = true;
        else
            this.btnFinalizar.Visible = false;
    }

    protected void btnCancelarContinuar_Click(object sender, EventArgs e)
    {
        string strQuery = "UPDATE compra SET " +
                    "estatus = 9" +
                    ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    ",motivo_cancelacion = '" + this.txtMotivoCancelacion.Text.Trim().Replace("'", "''") + "'" +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        DataSet objDataResult = new DataSet();

        strQuery = "SELECT C.productoID as productoID " +
                  " FROM compra_productos C " +
                  " WHERE compraID = " + this.hdID.Value;
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
            objProd_Datos.intProductoID = (int)objRowResult["productoID"];
            objProd_Datos.Leer();
            objProd_Datos.Recalcular_Compras();
            objProd_Datos.Guardar();
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
        this.rdTipo.Enabled = false;
        this.rdIVA.Enabled = false;
        this.dlListaPrecios.Enabled = false;
        this.txtDescuento.Enabled = false;
        this.txtComentarios.Enabled = false;
        this.txtFactura.Enabled = false;
    }

    protected void btnCambiarContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad, dcmPrecio;

        string strFecha;

        if (string.IsNullOrEmpty(this.txtCambiarFecha.Text))
            strFecha = "null";
        else
        {
            if (DateTime.Parse(this.txtCambiarFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")) <= DateTime.Today)
                return;
            strFecha = "'" + DateTime.Parse(this.txtCambiarFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'";
        }

        if (this.hdModLote.Value.Equals("1") &&
            string.IsNullOrEmpty(this.txtCambiarLote.Text.Trim()))
            return;

        decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
        decimal.TryParse(this.txtCambiarUnitario.Text, out dcmPrecio);

        if (dcmCantidad > 0 && dcmPrecio > 0)
        {
            string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');
            dcmCantidad = Math.Round(dcmCantidad, 2);
            dcmPrecio = Math.Round(dcmPrecio, 2);
            string strQuery = "UPDATE compra_productos SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                        ",lote = '" + this.txtCambiarLote.Text.Trim().Replace("'", "''") + "'" +
                        ",fecha_caducidad = " + strFecha +
                        " WHERE compraID = " + this.hdID.Value +
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
            Llenar_Productos(true);

            this.hdProductoPrecioID.Value = strID_Consecutivo[0];
            this.hdPrecioUnitario.Value = dcmPrecio.ToString("0.00");
            this.hdPorcentajeAumento.Value = (dcmPrecio / decimal.Parse(this.hdPrecioAnterior.Value)).ToString();
            Mostrar_PreciosVenta();
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
        string strPopUP = "mostrarReporte('compra_excel.aspx?t=c&c=" +
                                        this.hdID.Value +
                                        "')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void btnEmail_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarMailPopUp('compra_excel.aspx?m=Y&t=c&c=" +
                                        this.hdID.Value +
                                        "')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    protected void txtDescuento_TextChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
        {
            decimal dcmDescuento;
            decimal.TryParse(this.txtDescuento.Text.Trim(), out dcmDescuento);
            dcmDescuento = Math.Round(dcmDescuento, 2);
            this.txtDescuento.Text = dcmDescuento.ToString();
            Llenar_Productos(true);
        }
        this.txtFactura.Focus();
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
        double dblProcentaje = 1;
        if (decimal.TryParse(this.txtPrecioUnitarioCambio.Text.Trim(), out dcmPrecioUnitario))
        {
            decimal dcmPrecio = CComunDB.CCommun.Actualizar_Precio(this.hdProductoPrecioID.Value,
                                                                   this.dlListaPrecios.SelectedValue,
                                                                   dcmPrecioUnitario);

            if (double.Parse(this.hdPrecioAnterior.Value) != 0)
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
        decimal dcmPiezaPorCaja = 1; // decimal.Parse(this.txtPiezasPorCajaPrecio.Text.Trim());
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
        string strQuery = "UPDATE compra_productos SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND compraID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE compra_productos SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND compraID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE compra_productos SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND compraID = " + this.hdID.Value;

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

    protected void btnUsarOrden_Click(object sender, EventArgs e)
    {
        this.txtOrden_CompraID.Text = string.Empty;
        this.txtOrden_CompraID.Enabled = true;
        this.btnObtenerOrden_Compra.Visible = true;
        this.btnObtenerOrden_Compra.Enabled = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlCompra_Orden.Visible = true;
        this.pnlCompraDatos.Visible = false;
    }

    protected void btnRegresarOrden_Compra_Click(object sender, EventArgs e)
    {
        Mostrar_Datos();
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlCompra_Orden.Visible = false;
    }

    protected void btnObtenerOrden_Compra_Click(object sender, EventArgs e)
    {
        long lngOrden_CompraID = 0;
        if (!long.TryParse(this.txtOrden_CompraID.Text.Trim(), out lngOrden_CompraID))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Folio debe ser numérico");
            return;
        }

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT O.proveedorID, cajeroID, " +
                    " lista_preciosID, estatus, " +
                    " porc_iva, O.descuento, O.contado " +
                    " FROM compra_orden O" +
                    " INNER JOIN proveedores S " +
                    " ON O.proveedorID = S.ID " +
                    " WHERE O.ID = " + this.txtOrden_CompraID.Text.Trim();
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Orden de compra no existe");
            return;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];
        if (objRowResult["estatus"].ToString().Equals("9"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Orden de compra ha sido cancelada, seleccione otra");
            return;
        }

        if (!objRowResult["estatus"].ToString().Equals("1"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Orden de compra ya fue usada para generar una compra o está En Proceso, seleccione otra");
            return;
        }

        this.hdProveedorID2.Value = objRowResult["proveedorID"].ToString();
        this.hdCajeroID.Value = objRowResult["cajeroID"].ToString();
        this.hdLista_preciosID.Value = objRowResult["lista_preciosID"].ToString();
        this.hdPorc_Iva.Value = objRowResult["porc_iva"].ToString();
        this.hdDescuento.Value = objRowResult["descuento"].ToString();
        this.hdContado.Value = Convert.ToBoolean(objRowResult["contado"]) ? "1" : "0";

        List<COrden> lstOrden = new List<COrden>();
        this.gvProductosCompra.DataSource = ObtenerProductosOrden(lstOrden);
        this.gvProductosCompra.DataBind();

        if (this.gvProductosCompra.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Orden de compra no tiene productos");
            return;
        }

        this.btnObtenerOrden_Compra.Visible = false;
        this.txtOrden_CompraID.Enabled = false;
        this.btnProcesar.Visible = true;
        this.pnlCompraDatos.Visible = true;
    }

    protected void gvProductosCompra_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((HiddenField)e.Row.FindControl("hdRep")).Value.Equals("0"))
                e.Row.Cells[0].Controls[0].Visible = false;
            (((TextBox)e.Row.FindControl("txtFechaDatos"))).Attributes["readonly"] = "true";
        }
    }

    protected void gvProductosCompra_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        string strProductoID = this.gvProductosCompra.DataKeys[index].Value.ToString();

        decimal dcmTemp;
        if (e.CommandName == "Repetir")
        {
            List<COrden> lstOrden = new List<COrden>();
            bool swAgregado = false;
            foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
            {
                if (gvRow.RowType == DataControlRowType.DataRow)
                {
                    COrden objOrden = new COrden();
                    objOrden.intProductoID = int.Parse(this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString());
                    objOrden.strProducto = ((Label)gvRow.FindControl("lblProdDatos")).Text;
                    if (!string.IsNullOrEmpty(((Label)gvRow.FindControl("lblCantDatos")).Text))
                        objOrden.dcmCantidad = decimal.Parse(((Label)gvRow.FindControl("lblCantDatos")).Text);
                    else
                        objOrden.dcmCantidad = 0;
                    decimal.TryParse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text.Trim(), out dcmTemp);
                    objOrden.intRecibida = (int)dcmTemp;
                    decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text.Trim(), out dcmTemp);
                    objOrden.dcmPrecio = dcmTemp;
                    objOrden.swExento = (((HiddenField)gvRow.FindControl("hdExento")).Value.Equals("1") ? true : false);
                    objOrden.swLote = (((HiddenField)gvRow.FindControl("hdLote")).Value.Equals("1") ? true : false);
                    objOrden.swCaducidad = (((HiddenField)gvRow.FindControl("hdCaducidad")).Value.Equals("1") ? true : false);
                    objOrden.swPrecioEnabled = ((TextBox)gvRow.FindControl("txtCostoDatos")).Enabled;
                    objOrden.swPrecioChkEnabled = ((CheckBox)gvRow.FindControl("chkAct")).Enabled;
                    objOrden.swPrecioAct = ((CheckBox)gvRow.FindControl("chkAct")).Checked;

                    objOrden.strLote = ((TextBox)gvRow.FindControl("txtLoteDatos")).Text.Trim();
                    objOrden.strCaducidad = ((TextBox)gvRow.FindControl("txtFechaDatos")).Text.Trim();
                    objOrden.swRepetir = (((HiddenField)gvRow.FindControl("hdRep")).Value.Equals("1") ? true : false);
                    lstOrden.Add(objOrden);

                    if (!swAgregado && strProductoID.Equals(this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString()))
                    {
                        COrden newObject = new COrden(objOrden);
                        newObject.strProducto = string.Empty;
                        newObject.dcmCantidad = 0;
                        newObject.intRecibida = 0;
                        newObject.swPrecioChkEnabled = false;
                        newObject.swPrecioAct = false;
                        newObject.strLote = newObject.strCaducidad = string.Empty;
                        newObject.swRepetir = false;
                        lstOrden.Add(newObject);
                        swAgregado = true;
                    }
                }
            }

            this.gvProductosCompra.DataSource = ObtenerProductosOrden(lstOrden);
            this.gvProductosCompra.DataBind();
        }
    }

    protected void btnProcesar_Click(object sender, EventArgs e)
    {
        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        string strProducto = string.Empty;
        foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (!string.IsNullOrEmpty(((Label)gvRow.FindControl("lblProdDatos")).Text))
                    strProducto = ((Label)gvRow.FindControl("lblProdDatos")).Text;

                if (!int.TryParse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text.Trim(), out intCantidad) ||
                    !decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text.Trim(), out dcmPrecioUnitario))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser numérica: " + strProducto);
                    return;
                }
                if (intCantidad > 0)
                {
                    if (((HiddenField)gvRow.FindControl("hdLote")).Value.Equals("1") &&
                        string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtLoteDatos")).Text.Trim()))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Lote debe ser ingresado para este producto: " + strProducto);
                        return;
                    }

                    if (((HiddenField)gvRow.FindControl("hdCaducidad")).Value.Equals("1"))
                    {
                        if (string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtFechaDatos")).Text))
                        {
                            ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad debe ser ingresado para este producto: " + strProducto);
                            return;
                        }
                        if (DateTime.Parse(((TextBox)gvRow.FindControl("txtFechaDatos")).Text, CultureInfo.CreateSpecificCulture("es-MX")) <= DateTime.Today)
                        {
                            ((master_MasterPage)Page.Master).MostrarMensajeError("Fecha de caducidad no puede ser igual o menor al día de hoy para este producto: " + strProducto);
                            return;
                        }
                    }
                }
                ((TextBox)gvRow.FindControl("txtCantidadDatos")).Text = intCantidad.ToString();
                ((TextBox)gvRow.FindControl("txtCostoDatos")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();
            }
        }

        if (this.hdID.Value.Equals("0"))
        {
            if (!Crear_Compra_En_Orden())
                return;
        }

        string strQuery = "INSERT INTO compra_orden_compra (" +
                        "compra_ordenID " +
                        ",compraID" +
                        ") VALUES(" +
                        this.txtOrden_CompraID.Text +
                        "," + this.hdID.Value +
                        ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        decimal dcmCantidad, dcmCantidad_Compra, dcmCosto_unitario, dcmCosto;
        string strFecha;
        DataSet objDataResult = new DataSet();
        foreach (GridViewRow gvRow in this.gvProductosCompra.Rows)
        {
            dcmCantidad_Compra = decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text);

            if (dcmCantidad_Compra > 0)
            {
                strQuery = "SELECT cantidad, cantidad_compra" +
                           " FROM compra_orden_productos" +
                           " WHERE compra_ordenID = " + this.txtOrden_CompraID.Text +
                           " AND productoID = " + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString();

                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

                dcmCantidad = (decimal)objDataResult.Tables[0].Rows[0]["cantidad"];
                dcmCantidad_Compra += (decimal)objDataResult.Tables[0].Rows[0]["cantidad_compra"];

                strQuery = "UPDATE compra_orden_productos SET " +
                           "cantidad_compra = " + dcmCantidad_Compra.ToString() +
                           " WHERE compra_ordenID = " + this.txtOrden_CompraID.Text +
                           " AND productoID = " + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString();

                CComunDB.CCommun.Ejecutar_SP(strQuery);

                dcmCosto_unitario = decimal.Parse(((TextBox)gvRow.FindControl("txtCostoDatos")).Text);
                dcmCosto = Math.Round(dcmCosto_unitario * (decimal)dcmCantidad_Compra, 2);

                strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                    " FROM compra_productos " +
                    " WHERE compraID = " + this.hdID.Value;
                try
                {
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch (ApplicationException ex)
                {
                    throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
                }

                dcmCantidad_Compra = decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadDatos")).Text);
                if (string.IsNullOrEmpty(((TextBox)gvRow.FindControl("txtFechaDatos")).Text))
                    strFecha = "null";
                else
                    strFecha = "'" + DateTime.Parse(((TextBox)gvRow.FindControl("txtFechaDatos")).Text, CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd") + "'";
                strQuery = "INSERT INTO compra_productos (compraID, " +
                           "productoID, exento, cantidad, costo_unitario, costo," +
                           "fecha_caducidad, lote, consecutivo) VALUES (" +
                           "'" + this.hdID.Value + "'" +
                           ", '" + this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString() + "'" +
                           ", '" + ((HiddenField)gvRow.FindControl("hdExento")).Value + "'" +
                           ", '" + dcmCantidad_Compra + "'" +
                           ", '" + dcmCosto_unitario + "'" +
                           ", '" + dcmCosto + "'" +
                           ", " + strFecha +
                           ", '" + ((TextBox)gvRow.FindControl("txtLoteDatos")).Text + "'" +
                           ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                           ")";
                CComunDB.CCommun.Ejecutar_SP(strQuery);
                if (((CheckBox)gvRow.FindControl("chkAct")).Checked)
                {
                    CComunDB.CCommun.Actualizar_Precio(this.gvProductosCompra.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                       this.hdLista_preciosID.Value,
                                                       dcmCosto_unitario);
                }
            }

        }

        strQuery = "SELECT 1" +
                  " FROM compra_orden_productos" +
                  " WHERE compra_ordenID = " + this.txtOrden_CompraID.Text +
                  " AND (cantidad - cantidad_compra) > 0";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            strQuery = "UPDATE compra_orden SET " +
                "estatus = 2" +
                " WHERE ID = " + this.txtOrden_CompraID.Text;
        else
            strQuery = "UPDATE compra_orden SET " +
                "estatus = 3" +
                " WHERE ID = " + this.txtOrden_CompraID.Text;
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Compra ha sido creada/actualizada");
        Mostrar_Datos();
        this.pnlCompra_Orden.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    private bool Crear_Compra_En_Orden()
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM compra " +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una compra para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO compra (proveedorID, cajeroID, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, " +
                   "motivo_cancelacion, contado, monto_subtotal, descuento, " +
                   "monto_descuento, porc_iva, monto_iva, total, comentarios" +
                   ",creadoPorID, creadoPorFecha, modificadoPorID, modificadoPorFecha" +
                   ") VALUES (" +
                   "'" + this.hdProveedorID2.Value + "'" +
                   ", '" + this.hdCajeroID.Value + "'" +
                   ", '" + this.hdLista_preciosID.Value + "'" +
                   ", '8'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", ''" +
                   ", '" + this.hdContado.Value + "'" +
                   ", '0'" +
                   ", '" + this.hdDescuento.Value + "'" +
                   ", '0'" +
                   ", '" + this.hdPorc_Iva.Value + "'" +
                   ", '0'" +
                   ", '0'" +
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
        Modificar_Compra();

        DateTime? dtFecha = null;

        if (!Convert.ToBoolean(this.rdTipo.SelectedValue))
        {
            dtFecha = CComunDB.CCommun.Obtener_FechaPago(
                           1,
                           int.Parse(this.hdID.Value));
            if (!dtFecha.HasValue)
                dtFecha = DateTime.Today;
            this.txtFechaCambio.Text = dtFecha.Value.ToString("dd/MM/yyyy");
            this.rdCambiar.ClearSelection();
            this.rdCambiar.SelectedIndex = 0;
            this.mdCambiarFecha.Show();
        }
        else
        {
            this.txtFechaPago.Text = DateTime.Today.ToString("dd/MM/yyyy");
            this.txtReferencia.Text = string.Empty;
            this.txtPago.Text = this.hdTotal.Value;
            this.mdPago.Show();
            return;
        }
    }

    protected void btnPagoContinuar_Click(object sender, EventArgs e)
    {
        DateTime? dtFechaNull = null;
        Finalizar_Compra(dtFechaNull, "0");

        DateTime dtFecha = DateTime.Parse(this.txtFechaPago.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        string strQuery = "DELETE FROM pago_compras" +
                   " WHERE compraID = " + this.hdID.Value + ";" +
                   "INSERT INTO pagos (tipo_pago, fecha_pago, referencia, " +
                   "monto_pago, creadoPorID, creadoPorFecha) VALUES(" +
                   "'" + this.dlTiposPagos.SelectedValue + "'" +
                   ",'" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ",'" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                   ",'" + this.txtPago.Text + "'" +
                   ",'" + Session["SIANID"].ToString() + "'" +
                   ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        DataSet objDataResult = new DataSet();
        strQuery = "SELECT ID" +
                   " FROM pagos " +
                   " WHERE fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   " AND monto_pago = " + this.txtPago.Text;

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "INSERT INTO pago_compras (pagoID, compraID, monto_pago) VALUES(" +
                   "'" + objDataResult.Tables[0].Rows[0]["ID"].ToString() + "'" +
                   ",'" + this.hdID.Value + "'" +
                   ",'" + this.txtPago.Text + "'" +
                   ")";

        CComunDB.CCommun.Ejecutar_SP(strQuery);
    }

    protected void btnFechaContinuar_Click(object sender, EventArgs e)
    {
        DateTime dtFecha = DateTime.Parse(this.txtFechaCambio.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        Finalizar_Compra(dtFecha, "1");
    }

    private void Finalizar_Compra(DateTime? dtFecha, string strTipo)
    {
        string strQuery = "UPDATE compra SET ";

        if (dtFecha.HasValue)
        {
            if(this.rdCambiar.SelectedIndex == 0)
                strQuery += "estatus = " + strTipo +
                         ", fecha_contrarecibo = '" + dtFecha.Value.ToString("yyyy-MM-dd") + "'";
            else
                strQuery += "estatus = " + strTipo +
                         ", fecha_pago = '" + dtFecha.Value.ToString("yyyy-MM-dd") + "'";
        }
        else
            strQuery += "estatus = " + strTipo;

        strQuery += ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        DataSet objDataResult = new DataSet();

        Obtener_CreadoPor();

        strQuery = "SELECT C.productoID as productoID " +
                  ", C.cantidad as cantidad " +
                  ", C.costo_unitario as costo_unitario " +
                  ", C.costo as costo " +
                  ", C.fecha_caducidad as fecha " +
                  ", C.lote as lote " +
                  " FROM compra_productos C " +
                  " WHERE compraID = " + this.hdID.Value +
                  "   AND inventariado = 0;" +
                  " UPDATE compra_productos" +
                  " SET inventariado = 1" +
                  " WHERE compraID = " + this.hdID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dtFecha = null;
            CInventarios.Sumar(objRowResult["productoID"].ToString(),
                               "100",
                               objRowResult["lote"].ToString(),
                               (objRowResult.IsNull("fecha") ? dtFecha : (DateTime)objRowResult["fecha"]),
                               (decimal)objRowResult["cantidad"],
                                "Compra " + this.hdID.Value);

            CProducto_Datos objProd_Datos = new CProducto_Datos();
            objProd_Datos.intProductoID = (int)objRowResult["productoID"];
            objProd_Datos.Leer();
            objProd_Datos.intCompraID = int.Parse(this.hdID.Value);
            objProd_Datos.dtCompra_fecha = DateTime.Parse(this.txtFechaCreacion.Text, CultureInfo.CreateSpecificCulture("es-MX"));
            objProd_Datos.dcmCompra_cantidad = (decimal)objRowResult["cantidad"];
            objProd_Datos.dcmCompra_costo = (decimal)objRowResult["costo_unitario"];
            objProd_Datos.dcmCompra_total = (decimal)objRowResult["costo"];

            objProd_Datos.Verificar_Compra();

            objProd_Datos.Guardar();

            Verificar_Pedidos_Parciales(objRowResult["productoID"].ToString());
        }

        Mostrar_Datos();
    }

    private void Verificar_Pedidos_Parciales(string strProdID)
    {
        string strQuery = "SELECT DISTINCT C.ID" +
                         " FROM orden_compra C" +
                         " INNER JOIN orden_compra_productos P" +
                         " ON C.ID = P.orden_compraID" +
                         " AND C.estatus = 4" +
                         " AND P.validado = 0" +
                         " AND P.productoID = " + strProdID;

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            // El pedido se pone en 'Parcial listo'
            strQuery = "UPDATE orden_compra" +
                      " SET estatus = 5" +
                      " WHERE ID = " + objRowResult["ID"];

            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
    }

    private void Obtener_CreadoPor()
    {
        this.lblCreado.Text = string.Empty;
        string strQuery = "SELECT U.usuario, F.creadoPorFecha" +
                         " FROM compra F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.creadoPorID" +
                         " AND F.ID = " + this.hdID.Value + ";" +
                         "SELECT U.usuario, F.modificadoPorFecha" +
                         " FROM compra F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.modificadoPorID" +
                         " AND F.ID = " + this.hdID.Value;
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            this.lblCreado.Text = "Creación: " + objDataResult.Tables[0].Rows[0]["usuario"].ToString() +
                                  "(" + ((DateTime)objDataResult.Tables[0].Rows[0]["creadoPorFecha"]).ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() +
                                  ")";
        }

        if (objDataResult.Tables[1].Rows.Count > 0)
        {
            this.lblCreado.Text += "Última modificación: " + objDataResult.Tables[1].Rows[0]["usuario"].ToString() +
                                  "(" + ((DateTime)objDataResult.Tables[1].Rows[0]["modificadoPorFecha"]).ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() +
                                  ")";
        }
    }
}

public class COrden
{
    public string strProducto;
    public int intProductoID;
    public decimal dcmCantidad;
    public int intRecibida;
    public decimal dcmPrecio;
    public bool swPrecioEnabled;
    public bool swPrecioChkEnabled;
    public bool swPrecioAct;
    public string strLote;
    public bool swLote;
    public string strCaducidad;
    public bool swCaducidad;
    public bool swExento;
    public bool swRepetir;

    public COrden()
    {

    }

    public COrden(COrden objOrden)
    {
        strProducto = objOrden.strProducto;
        intProductoID = objOrden.intProductoID;
        dcmCantidad = objOrden.dcmCantidad;
        intRecibida = objOrden.intRecibida;
        dcmPrecio = objOrden.dcmPrecio;
        swPrecioEnabled = objOrden.swPrecioEnabled;
        swPrecioAct = objOrden.swPrecioAct;
        strLote = objOrden.strLote;
        swLote = objOrden.swLote;
        strCaducidad = objOrden.strCaducidad;
        swCaducidad = objOrden.swCaducidad;
        swExento = objOrden.swExento;
        swRepetir = objOrden.swRepetir;
    }
}