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
using CFE;
using System.Configuration;

public partial class cotizacion_orden_compra : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.txtProdLista.Attributes["onfocus"] = "javascript:limpiarProdListaID();";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnGuardarAlmacen.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnGuardarAlmacen.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='CancelFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='CancelFormat2'";
        this.btnRegresar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.btnRegresarAlmacen.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnRegresarAlmacen.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";
        this.btnImprimir.Attributes["onmouseout"] = "javascript:this.className='ReporteFormat1'";
        this.btnImprimir.Attributes["onmouseover"] = "javascript:this.className='ReporteFormat2'";
        this.imgInfo.Attributes["onmouseout"] = "javascript:esconder();";
        this.imgInfo2.Attributes["onmouseout"] = "javascript:esconder2();";
        this.btnSeleccionar.Attributes["onclick"] = "javascript:seleccionar_todo();return false;";
        this.txtFechaEntrega.Attributes["readonly"] = "true";
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
                this.txtProdLista.Enabled = false;
                this.txtCantLista.Enabled = false;
                this.txtPrecioLista.Enabled = false;
                this.btnAgrProdLista.Visible = false;
            }

            this.tmrRefresh.Enabled = false;
            this.hdAlmacen.Value = "0";
            if (!CComunDB.CCommun.ValidarAcceso(2506, out swVer, out swTot))
            {
                if (!CComunDB.CCommun.ValidarAcceso(2508, out swVer, out swTot))
                    Response.Redirect("../inicio/error.aspx");
                else
                {
                    this.lblAgregar.Visible = false;
                    this.tmrRefresh.Enabled = true;
                    this.hdAlmacen.Value = "1";
                }
            }

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

            if (Request.QueryString["s"] != null &&
                Request.QueryString["s"].Equals("4"))
            {
                this.dlBusqueda.ClearSelection();
                this.dlBusqueda.Items.FindByValue("4").Selected = true;
                this.txtCriterio.Text = "Surtido parcial";
                Validar_Campos();
            }

            Llenar_Grid();

            this.hdID.Value = "";
            this.hdInventarios.Value = ConfigurationManager.AppSettings["inventario"].ToString();
        }
    }

    private void Llenar_Catalogos()
    {
        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerOrden_CompraEstatus(false, this.hdAlmacen.Value);
        this.dlEstatus.DataBind();

        this.dlListaPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
        this.dlListaPrecios.DataBind();

        this.dlVendedor.DataSource = CComunDB.CCommun.ObtenerVendedores(false, true, 0);
        this.dlVendedor.DataBind();

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

            if (this.hdAlmacen.Value.Equals("1"))
                this.grdvLista.Columns[6].Visible = false;
            else
                this.grdvLista.Columns[6].Visible = true;
        }

        if (this.hdAlmacen.Value.Equals("1"))
            this.tmrRefresh.Enabled = true;
        else
            this.tmrRefresh.Enabled = false;
    }

    private DataTable ObtenerOrdenes_Compras()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("orden_compraID", typeof(string)));
        dt.Columns.Add(new DataColumn("negocio", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_creacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("motivo_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));
        dt.Columns.Add(new DataColumn("estatusID", typeof(string)));

        string strQuery = "CALL leer_ordenes_compras_consulta(" +
            ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30" +
            ", '" + this.hdAlmacen.Value + "'" +
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
            dr[0] = objRowResult["orden_compraID"].ToString();
            dr[1] = objRowResult["negocio"].ToString();
            if ((DateTime)objRowResult["fecha_creacion"] == DateTime.Parse("1901-01-01"))
                dr[2] = string.Empty;
            else
                dr[2] = ((DateTime)objRowResult["fecha_creacion"]).ToString("dd/MM/yyyy HH:mm");
            if ((DateTime)objRowResult["fecha_cancelacion"] == DateTime.Parse("1901-01-01"))
                dr[3] = string.Empty;
            else
                dr[3] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[4] = this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Text;
            dr[5] = objRowResult["motivo_cancelacion"].ToString();
            dr[6] = ((decimal)objRowResult["costo"]).ToString("c");
            dr[7] = objRowResult["estatus"].ToString();
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
        dt.Columns.Add(new DataColumn("validado", typeof(string)));
        dt.Columns.Add(new DataColumn("faltante", typeof(string)));
        dt.Columns.Add(new DataColumn("id", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("con", typeof(string)));
        dt.Columns.Add(new DataColumn("sal", typeof(string)));
        dt.Columns.Add(new DataColumn("ubicacion", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.cantidad_devuelto " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", C.usar_sal as usar_sal " +
            ", C.usar_cve_gob " +
            ", LEFT(P.nombre, 80) as producto " +
            ", LEFT(P.sales, 80) as sales " +
            ", LEFT(P.clave_gobierno, 80) as clave_gobierno " +
            ", P.codigo as codigo " +
            ", P.ubicacion " +
            ", C.exento as exento " +
            ", C.validado as validado " +
            ", C.faltante as faltante " +
            " FROM orden_compra_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND orden_compraID = " + this.hdID.Value +
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
        int intLetra1 = 97;
        int intLetra2 = 32;
        DataSet objDataResult2;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString() + "_" +
                    objRowResult["consecutivo"].ToString();

            if ((bool)objRowResult["usar_sal"])
            {
                dr[11] = "1";
                dr[1] = objRowResult["sales"].ToString();
            }
            else
                if ((bool)objRowResult["usar_cve_gob"])
                {
                    dr[11] = "2";
                    dr[1] = objRowResult["clave_gobierno"].ToString();
                }
                else
                {
                    dr[11] = "0";
                    dr[1] = objRowResult["producto"].ToString();
                }

            dr[3] = ((decimal)objRowResult["costo_unitario"]).ToString("c");
            decimal dblCosto = (decimal)objRowResult["costo"];
            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("#.###");
            dr[4] = dblCosto.ToString("c");
            dcmCosto += dblCosto;
            if (Convert.ToBoolean(objRowResult["exento"]))
                dr[5] = "1";
            else
            {
                dcmCostoIVA += dblCosto;
                dr[1] = dr[1] + "*";
                dr[5] = "0";
            }

            if (!objRowResult.IsNull("cantidad_devuelto"))
            {
                if ((decimal)objRowResult["cantidad_devuelto"] == (decimal)objRowResult["cantidad"])
                    dr[6] = "Devuelto";
                else
                    dr[6] = "Dev.Parc";
            }
            else
            {
                if (Convert.ToBoolean(objRowResult["validado"]))
                    dr[6] = "Surtido";
                else
                    dr[6] = string.Empty;
                if (objRowResult["faltante"].ToString().Equals("0"))
                    dr[7] = string.Empty;
                else
                    dr[7] = ((decimal)objRowResult["faltante"]).ToString("0.###");
            }

            if (intId == 0)
                this.hdConsMin.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMax.Value = objRowResult["consecutivo"].ToString();
            this.hdConsMaxID.Value = intId.ToString();
            intId++;
            if (intLetra1 == 123)
            {
                intLetra1 = 97;
                if (intLetra2 == 32)
                    intLetra2 = 97;
                else
                    intLetra2++;
            }
            dr[8] = intId.ToString() + "-" + ((char)intLetra2).ToString().Trim() + ((char)intLetra1).ToString();
            dr[9] = objRowResult["codigo"].ToString();
            dr[10] = objRowResult["consecutivo"].ToString();
            dr[12] = objRowResult["ubicacion"].ToString();

            strQuery = "SELECT MAX(F.fecha) as fecha" +
                      " FROM facturas_liq F" +
                      " INNER JOIN facturas_liq_prod P" +
                      " ON F.ID = P.factura_ID" +
                      " AND F.lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                      " AND P.producto_ID = " + objRowResult["productoID"].ToString();

            objDataResult2 = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (!objDataResult2.Tables[0].Rows[0].IsNull("fecha"))
                dr[13] = ((DateTime)objDataResult2.Tables[0].Rows[0]["fecha"]).ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("es-MX"));

            dt.Rows.Add(dr);
            intLetra1++;
        }

        dcmCosto = Math.Round(dcmCosto, 2);
        dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

        this.hdCosto.Value = dcmCosto.ToString();
        this.hdCostoIVA.Value = dcmCostoIVA.ToString();

        return dt;
    }

    private DataTable ObtenerProductosAlmacen(string strEstatus)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("validado", typeof(bool)));
        dt.Columns.Add(new DataColumn("faltante", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("mensaje", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidadasurtir", typeof(string)));
        dt.Columns.Add(new DataColumn("codigos", typeof(string)));
        dt.Columns.Add(new DataColumn("ubicacion", typeof(string)));
        dt.Columns.Add(new DataColumn("btnID", typeof(string)));
        dt.Columns.Add(new DataColumn("temporal", typeof(string)));
        dt.Columns.Add(new DataColumn("codigotemp", typeof(string)));

        string strQuery = "SELECT * FROM (" +
            "SELECT C.productoID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", LEFT(P.nombre, 80) as producto " +
            ", P.codigo as codigo " +
            ", P.ubicacion " +
            ", CONCAT(P.codigo, '.', P.codigo2, '.', P.codigo3) as codigos " +
            ", P.unimed" +
            ", C.exento as exento " +
            ", C.validado as validado " +
            ", C.temporal as temporal " +
            ", C.faltante as faltante " +
            " FROM orden_compra_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND orden_compraID = " + this.hdID.Value +
            ") AS AA ORDER BY consecutivo, producto";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        int i = 0;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString() + "_" +
                    objRowResult["consecutivo"].ToString(); ;
            dr[1] = objRowResult["producto"].ToString() + " (" +
                    objRowResult["unimed"].ToString() + ")";
            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("#.###");
            dr[3] = Convert.ToBoolean(objRowResult["validado"]);
            dr[4] = ((decimal)objRowResult["faltante"]).ToString("0.###");
            if (objRowResult["codigo"].ToString().Length < 4 || Session["SIANID"].ToString().Equals("1"))
                dr[5] = objRowResult["codigo"].ToString();
            else
                dr[5] = ".." + objRowResult["codigo"].ToString().Substring(objRowResult["codigo"].ToString().Length - 4);

            if (Convert.ToBoolean(objRowResult["validado"]))
            {
                dr[6] = "Producto ya surtido";
                dr[7] = "0";
            }
            else
                if ((decimal)objRowResult["faltante"] != 0)
                {
                    dr[6] = "Surtido: " + (((decimal)objRowResult["cantidad"]) - ((decimal)objRowResult["faltante"])).ToString("0.###") +
                            " de " + ((decimal)objRowResult["cantidad"]).ToString("0.###");
                    dr[7] = ((decimal)objRowResult["faltante"]).ToString("#.###");
                }
                else
                    dr[7] = ((decimal)objRowResult["cantidad"]).ToString("#.###");

            dr[8] = objRowResult["codigos"].ToString();
            dr[9] = objRowResult["ubicacion"].ToString();
            dr[10] = i;
            dr[11] = (bool)objRowResult["temporal"] ? "1" : "0";
            if((bool)objRowResult["temporal"])
                dr[12] = objRowResult["codigo"].ToString();
            dt.Rows.Add(dr);
            i++;
        }

        return dt;
    }

    private DataTable ObtenerNotasRemision(string strProductoID,
                                           string strSucursalID)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("nota", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));

        // Busca si el producto está en alguna nota que este en estatus 2-Contrarecibo
        // 3-A cobrar, 4-A facturar
        string strQuery = "SELECT N.nota, N.fecha " +
            " FROM notas N " +
            " INNER JOIN notas_prod P " +
            " ON N.ID = P.nota_ID " +
            " AND P.producto_ID = " + strProductoID +
            " AND N.status IN (1, 2, 3) " +
            " AND N.fecha > '" + DateTime.Today.AddMonths(-2).ToString("yyyy-MM-dd") + "'" +
            " INNER JOIN sucursales S " +
            " ON N.sucursal_ID = S.ID " +
            " AND S.establecimiento_ID = " +
            " (SELECT establecimiento_ID " +
            "  FROM sucursales " +
            "  WHERE ID = " + strSucursalID + ")";
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
            dr[0] = objRowResult["nota"].ToString();
            dr[1] = ((DateTime)objRowResult["fecha"]).ToString("dd-MMM-yyy", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX"));
            dt.Rows.Add(dr);
        }

        return dt;
    }

    private DataTable ObtenerLotes(string strID)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("loteID", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("canttemp", typeof(string)));

        string strQuery = "SELECT existencia, lote" +
                          ", ifnull(fecha_caducidad, makedate(9999, 365)) as fecha_caducidad" +
                          " FROM inventario " +
                          " WHERE producto_ID = " + strID +
                          "   AND subalmacen_ID = 100 " +
                          " ORDER BY fecha_caducidad, existencia asc";
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
            string strLote = "S/N";
            if (!string.IsNullOrEmpty(objRowResult["lote"].ToString()))
                strLote = objRowResult["lote"].ToString();

            dr = dt.NewRow();
            if ((DateTime)objRowResult["fecha_caducidad"] == DateTime.Parse("9999-12-31"))
            {
                dr[0] = objRowResult["lote"].ToString() + "_null";
                dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0") + ")";
            }
            else
            {
                dr[0] = objRowResult["lote"].ToString() + "_'" +
                        ((DateTime)objRowResult["fecha_caducidad"]).ToString("yyyy-MM-dd") +
                        "'";
                dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0") + " - " + ((DateTime)objRowResult["fecha_caducidad"]).ToString("dd/MM/yyyy") + ")";
            }
            dr[2] = ((decimal)objRowResult["existencia"]).ToString("#0");
            dr[3] = "0";
            dt.Rows.Add(dr);
        }
        return dt;
    }

    private DataTable ObtenerLotesAlmacen(string strID,
                                          string strTemporal)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("loteID", typeof(string)));
        dt.Columns.Add(new DataColumn("lote", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("canttemp", typeof(string)));

        string strQuery;
        if(strTemporal.Equals("0"))
            strQuery = "SELECT existencia, lote" +
                      ", ifnull(fecha_caducidad, makedate(9999, 365)) as fecha_caducidad" +
                      ", null as cantidad_temporal" +
                      " FROM inventario " +
                      " WHERE producto_ID = " + strID +
                      "   AND subalmacen_ID = 100 " +
                      " ORDER BY fecha_caducidad, existencia asc";
        else
            strQuery = "SELECT A.producto_ID, A.existencia, A.lote, A.fecha_caducidad, L.cantidad_temporal" +
                      " FROM (" +
                      "    SELECT producto_ID, existencia, lote" +
                      "    , ifnull(fecha_caducidad, makedate(9999, 365)) as fecha_caducidad" +
                      "    FROM inventario" +
                      "    WHERE producto_ID = " + strID +
                      "      AND subalmacen_ID = 100 " +
                      " ) AS A" +
                      " LEFT JOIN orden_compra_productos_lote L" +
                      " ON L.productoID = A.producto_ID" +
                      "   AND L.lote = A.lote" +
                      "   AND L.orden_compraID = " + this.hdID.Value +
                      " ORDER BY A.fecha_caducidad, existencia asc";
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
            string strLote = "S/N";
            if (!string.IsNullOrEmpty(objRowResult["lote"].ToString()))
                strLote = objRowResult["lote"].ToString();

            dr = dt.NewRow();
            if ((DateTime)objRowResult["fecha_caducidad"] == DateTime.Parse("9999-12-31"))
            {
                dr[0] = objRowResult["lote"].ToString() + "_null";
                dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0") + ")";
            }
            else
            {
                dr[0] = objRowResult["lote"].ToString() + "_'" +
                        ((DateTime)objRowResult["fecha_caducidad"]).ToString("yyyy-MM-dd") +
                        "'";
                dr[1] = strLote + " (" + ((decimal)objRowResult["existencia"]).ToString("#,##0") + " - " + ((DateTime)objRowResult["fecha_caducidad"]).ToString("dd/MM/yyyy") + ")";
            }
            dr[2] = ((decimal)objRowResult["existencia"]).ToString("#0");
            if (objRowResult.IsNull("cantidad_temporal"))
                dr[3] = "0";
            else
                dr[3] = ((decimal)objRowResult["cantidad_temporal"]).ToString("#0");
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

    protected void grdvLista_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[5].Text.Equals("Cotejada"))
            {
                ((LinkButton)e.Row.Cells[0].Controls[0]).ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }
            else
                if (e.Row.Cells[5].Text.Equals("Parcial listo"))
                {
                    ((LinkButton)e.Row.Cells[0].Controls[0]).ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Green;
                }
        }
    }

    protected void grdvLista_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Modificar")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos(this.grdvLista.Rows[index].Cells[7].Text);
            if (!this.hdAT.Value.Equals("1"))
            {
                this.btnModificar.Visible = false;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.gvProductos.Enabled = false;
                this.btnGenerarNota.Visible = false;
                this.btnGenerarFactura.Visible = false;
                this.btnCotejada.Visible = false;
            }
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
        Mostrar_Datos(string.Empty);
    }

    private void Mostrar_Datos(string strEstatusID)
    {
        if (this.hdAlmacen.Value.Equals("1") ||
            (strEstatusID.Equals("2") && Session["SIANID"].ToString().Equals("1")))
            Mostrar_Datos_Almacen();
        else
            Mostrar_Datos_General();
    }

    private void Mostrar_Datos_General()
    {
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lblCreado.Text = string.Empty;
        this.lblOrden_Compra.Text = string.Empty;
        this.rdNombre.ClearSelection();
        this.rdNombre.SelectedIndex = 0;
        this.btnAjustar.Visible = false;
        if (this.hdID.Value.Equals("0"))
        {
            this.hdBorrar.Value = "1";
            this.txtOrdenCompra.Text = string.Empty;
            this.txtRequerimientos.Text = string.Empty;
            this.txtTiempoEntrega.Text = string.Empty;
            this.txtLugarEntrega.Text = string.Empty;
            this.txtFechaEntrega.Text = string.Empty;
            this.txtComentarios.Text = string.Empty;
            this.txtProducto.Enabled = false;
            this.txtCantidad.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = false;
            this.btnCancelar.Visible = false;
            this.btnAgregarProd.Visible = true;
            this.btnAgregarProd.Enabled = false;
            this.dlEstatus.ClearSelection();
            this.dlEstatus.SelectedIndex = 0;
            this.btnCotizacion.Visible = true;
            this.btnLista.Visible = false;
            this.hdCosto.Value = "0";
            this.hdCostoIVA.Value = "0";
            this.hdIVA.Value = "0.00";
            this.hdTotal.Value = "0";
            this.gvProductos.DataSource = null;
            this.gvProductos.DataBind();
            this.lblNotas.Text = string.Empty;
            this.txtSucursal.Text = string.Empty;
            this.txtSucursal.Enabled = true;
            this.hdSucursalID.Value = "0";
            this.btnGenerarFactura.Visible = false;
            this.btnGenerarNota.Visible = false;
            this.dlDocumento.ClearSelection();
            this.dlDocumento.SelectedIndex = 0;
        }
        else
        {
            this.lblOrden_Compra.Text = this.hdID.Value;
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT sucursalID, concat(negocio, ' - ', sucursal) as nombre, " +   //Table 0
                    " lista_preciosID, estatus, requerimientos, " +
                    " tiempo_entrega, lugar_entrega, fecha_entrega, " +
                    " motivo_cancelacion, comentarios, " +
                    " porc_iva, orden_compra, orden_compra_total, O.documento " +
                    " FROM orden_compra O" +
                    " INNER JOIN sucursales S " +
                    " ON O.sucursalID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " WHERE O.ID = " + this.hdID.Value + ";" +
                    " SELECT facturaID" +                                                           //Table 1 - Facturas
                    " FROM orden_compra_factura" +
                    " WHERE orden_compraID = " + this.hdID.Value +
                    " ORDER BY facturaID;" +
                    " SELECT notaID" +                                                              //Table 2 - Remisiones
                    " FROM orden_compra_nota" +
                    " WHERE orden_compraID = " + this.hdID.Value +
                    " ORDER BY notaID" + ";" +
                    " SELECT entrada_ID" +                                                          //Table 3 - Entradas
                    " FROM entrada_devolucion" +
                    " WHERE tipo = 3" +
                    " AND tipo_ID = " + this.hdID.Value + ";" +
                    " SELECT cotizacionID" +                                                        //Table 4 - Cotizaciones
                    " FROM cotizacion_orden_compra" +
                    " WHERE orden_compraID = " + this.hdID.Value +
                    " ORDER BY cotizacionID";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            this.txtOrdenCompra.Text = objRowResult["orden_compra"].ToString();
            if (string.IsNullOrEmpty(this.txtOrdenCompra.Text))
                this.txtOrdenCompraTotal.Text = string.Empty;
            else
                this.txtOrdenCompraTotal.Text = ((decimal)objRowResult["orden_compra_total"]).ToString("0.00");
            this.txtRequerimientos.Text = objRowResult["requerimientos"].ToString();
            this.txtTiempoEntrega.Text = objRowResult["tiempo_entrega"].ToString();
            this.txtLugarEntrega.Text = objRowResult["lugar_entrega"].ToString();
            if (objRowResult.IsNull("fecha_entrega"))
                this.txtFechaEntrega.Text = string.Empty;
            else
                this.txtFechaEntrega.Text = ((DateTime)objRowResult["fecha_entrega"]).ToString("dd-MM-yyyy");
            this.txtComentarios.Text = objRowResult["comentarios"].ToString();
            this.txtSucursal.Text = objRowResult["nombre"].ToString();
            this.hdSucursalID.Value = objRowResult["sucursalID"].ToString();

            this.dlEstatus.ClearSelection();
            this.dlEstatus.Items.FindByValue(objRowResult["estatus"].ToString()).Selected = true;

            this.dlListaPrecios.ClearSelection();
            this.dlListaPrecios.Items.FindByValue(objRowResult["lista_preciosID"].ToString()).Selected = true;

            this.rdIVA.ClearSelection();
            this.rdIVA.Items.FindByValue(((decimal)objRowResult["porc_iva"]).ToString("0.00")).Selected = true;

            this.dlDocumento.ClearSelection();
            this.dlDocumento.Items.FindByValue(objRowResult["documento"].ToString()).Selected = true;

            this.btnCotizacion.Visible = false;
            this.btnLista.Visible = true;
            this.gvProductos.Enabled = true;

            if (!objRowResult["estatus"].ToString().Equals("1"))
            {
                this.hdBorrar.Value = "0";
                this.gvProductos.Enabled = false;
                this.btnGenerarFactura.Visible = false;
                this.btnGenerarNota.Visible = false;
                this.txtSucursal.Enabled = false;
                this.txtProducto.Enabled = false;
                this.txtCantidad.Enabled = false;
                this.txtPrecioUnitario.Enabled = false;
                this.btnModificar.Visible = false;
                this.btnImprimir.Visible = true;
                this.btnCancelar.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.dlListaPrecios.Enabled = false;
                this.btnCotejada.Visible = false;
                this.rdIVA.Enabled = false;
                this.btnGenerarFactura.Visible = true;
                this.btnGenerarNota.Visible = true;
                this.btnLista.Visible = false;

                if (this.dlEstatus.SelectedValue.Equals("3") || // Surtido
                    this.dlEstatus.SelectedValue.Equals("4") || // Surtido parcial
                    this.dlEstatus.SelectedValue.Equals("5"))   // Parcial Listo
                    this.btnCancelar.Visible = true;
                else
                    this.btnCancelar.Visible = false;

                if (objRowResult["estatus"].ToString().Equals("9"))
                {
                    this.lblMensaje.Text = "Pedido cerrado: " + objRowResult["motivo_cancelacion"].ToString();
                    this.btnGenerarFactura.Visible = false;
                    this.btnGenerarNota.Visible = false;
                    this.btnLista.Visible = false;
                }
            }
            else
            {
                this.hdBorrar.Value = "1";
                this.btnGenerarFactura.Visible = true;
                this.btnGenerarNota.Visible = true;
                this.txtSucursal.Enabled = true;
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
                this.btnCotejada.Visible = true;
                this.btnGenerarFactura.Visible = true;
                this.btnGenerarNota.Visible = true;
            }
            if (objDataResult.Tables[1].Rows.Count > 0)
            {
                StringBuilder strFacturas = new StringBuilder("Factura(s): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[1].Rows)
                {
                    if (strFacturas.Length != 12)
                        strFacturas.Append(", ");
                    strFacturas.Append(objRowResult2[0].ToString());
                }
                this.lblMensaje.Text = strFacturas.ToString();
                this.btnGenerarNota.Visible = false;
                this.btnGenerarFactura.Visible = false;
            }

            if (objDataResult.Tables[2].Rows.Count > 0)
            {
                StringBuilder strFacturas = new StringBuilder("Remisión(es): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[2].Rows)
                {
                    if (strFacturas.Length != 14)
                        strFacturas.Append(", ");
                    strFacturas.Append(objRowResult2[0].ToString());
                }
                this.lblMensaje.Text += " " + strFacturas.ToString();
            }

            if (objDataResult.Tables[3].Rows.Count > 0)
            {
                StringBuilder strEntradas = new StringBuilder("Productos devueltos en la(s) entrada(s): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[3].Rows)
                {
                    if (strEntradas.Length != 41)
                        strEntradas.Append(", ");
                    strEntradas.Append(objRowResult2[0].ToString());
                }
                if(this.lblMensaje.Text.Length != 0)
                    this.lblMensaje.Text += ". " + strEntradas.ToString();
                else
                    this.lblMensaje.Text = strEntradas.ToString();
            }

            if (objDataResult.Tables[4].Rows.Count > 0)
            {
                StringBuilder strFacturas = new StringBuilder("Cotización(es): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[4].Rows)
                {
                    if (strFacturas.Length != 16)
                        strFacturas.Append(", ");
                    strFacturas.Append(objRowResult2[0].ToString());
                }
                this.lblMensaje.Text += " " + strFacturas.ToString();
            }

            string[] strValores = Obtener_Notas(this.hdSucursalID.Value).Split('|');
            this.lblNotas.Text = strValores[0];
            Llenar_Productos(false);
            Obtener_CreadoPor();
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    private void Mostrar_Datos_Almacen()
    {
        //Guarda los datos cada 15 minutos
        //En milisegundos 15 min = 15 * 60 * 1000 = 900000
        this.tmrRefresh.Interval = 900000;
        this.hdValidar.Value = "1";
        this.lblOrden_CompraAlmacen.Text = this.hdID.Value;

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT negocio, O.comentarios, O.estatus, O.requerimientos, O.tiempo_entrega " +
                " FROM orden_compra O " +
                " INNER JOIN sucursales S " +
                " ON O.sucursalID = S.ID " +
                " INNER JOIN establecimientos E " +
                " ON S.establecimiento_ID = E.ID " +
                " WHERE O.ID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];
        this.lblNegocio.Text = objRowResult["negocio"].ToString();
        this.txtComentariosAlmacen.Text = objRowResult["comentarios"].ToString();
        this.txtRequerimientosAlmacen.Text = objRowResult["requerimientos"].ToString();
        this.txtTiempoEntregaAlmacen.Text = objRowResult["tiempo_entrega"].ToString();

        this.gvProductosAlmacen.DataSource = ObtenerProductosAlmacen(objRowResult["estatus"].ToString());
        this.gvProductosAlmacen.DataBind();

        if (objRowResult["estatus"].ToString().Equals("2"))
            this.btnPonerProceso.Visible = true;
        else
            this.btnPonerProceso.Visible = false;

        this.btnGuardarAlmacen.Visible = true;
        this.pnlListado.Visible = false;
        this.pnlAlmacen.Visible = true;
    }

    private void Agregar_Orden()
    {
        if (this.hdSucursalID.Value.Equals("0"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return;
        }

        decimal dcmOrdenCompraTotal = 0;

        if (!string.IsNullOrEmpty(this.txtOrdenCompra.Text))
        {
            if (!decimal.TryParse(this.txtOrdenCompraTotal.Text, out dcmOrdenCompraTotal))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese el subtotal de la orden de compra del cliente");
                return;
            }
        }

        DateTime? dtFechaEntrega = null;

        if (!string.IsNullOrEmpty(this.txtFechaEntrega.Text))
            dtFechaEntrega = DateTime.Parse(this.txtFechaEntrega.Text, CultureInfo.CreateSpecificCulture("es-MX"));

        if (Crear_Orden_Compra(this.hdSucursalID.Value, this.txtOrdenCompra.Text.Trim(),
                                dcmOrdenCompraTotal, this.dlListaPrecios.SelectedValue,
                                this.txtRequerimientos.Text.Trim(), this.txtTiempoEntrega.Text.Trim(),
                                this.txtLugarEntrega.Text.Trim(), dtFechaEntrega,
                                this.txtComentarios.Text.Trim()))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("El pedido ha sido creado, folio: " + this.hdID.Value);
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            if (this.hdUsuPr.Value.Equals("0"))
                this.txtPrecioUnitario.Enabled = false;
            else
                this.txtPrecioUnitario.Enabled = true;
            this.btnModificar.Visible = true;
            this.btnImprimir.Visible = true;
            this.btnCancelar.Visible = true;
            this.btnAgregarProd.Enabled = true;
            this.btnLista.Visible = true;
            Obtener_CreadoPor();
        }
    }

    private bool Crear_Orden_Compra(string strSucursalID, string strOrden_Compra, decimal dcmOrden_Compra_Total,
                                string strListaPrecios, string strRequerimientos,
                                string strTiempoEntrega, string strLugarEntrega,
                                DateTime? dtFechaEntrega,
                                string strComentarios)
    {
        DataSet objDataResult = new DataSet();
        DateTime dtAhora = DateTime.Now;

        string strQuery = "SELECT 1 " +
                    " FROM orden_compra " +
                    " WHERE sucursalID = " + strSucursalID +
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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ya existe una orden_compra para este cliente en este día");
            return false;
        }

        strQuery = "INSERT INTO orden_compra (sucursalID, orden_compra, lista_preciosID, " +
                   "estatus, fecha_creacion, fecha_cancelacion, requerimientos," +
                   "tiempo_entrega, lugar_entrega, fecha_entrega, motivo_cancelacion, costo, " +
                   "porc_iva, iva, total, orden_compra_total, documento," +
                   "comentarios, " +
                   "creadoPorID, creadoPorFecha, modificadoPorID, modificadoPorFecha) VALUES (" +
                   "'" + strSucursalID + "'" +
                   ", '" + strOrden_Compra + "'" +
                   ", '" + strListaPrecios + "'" +
                   ", '" + this.dlEstatus.SelectedValue + "'" +
                   ", '" + dtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                   ", '1901-01-01'" +
                   ", '" + strRequerimientos + "'" +
                   ", '" + strTiempoEntrega + "'" +
                   ", '" + strLugarEntrega + "'" +
                   ", " + (dtFechaEntrega.HasValue ? "'" + dtFechaEntrega.Value.ToString("yyyy-MM-dd") + "'" : "null") +
                   ", ''" +
                   ", '" + this.hdCosto.Value + "'" +
                   ", '" + this.rdIVA.SelectedValue + "'" +
                   ", '" + this.hdIVA.Value + "'" +
                   ", '" + this.hdTotal.Value + "'" +
                   ", '" + dcmOrden_Compra_Total + "'" +
                   ", " + this.dlDocumento.SelectedValue +
                   ", '" + strComentarios + "'" +
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
                " FROM orden_compra " +
                " WHERE sucursalID = " + strSucursalID +
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
            this.btnCotizacion.Visible = false;
            this.btnGenerarFactura.Visible = true;
            this.btnGenerarNota.Visible = true;
            return true;
        }
        return false;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            Agregar_Orden();
        }
        else
            Modificar_Orden();
    }

    private bool Modificar_Orden()
    {
        decimal dcmOrdenCompraTotal = 0;
        if (!string.IsNullOrEmpty(this.txtOrdenCompra.Text))
        {
            if (!decimal.TryParse(this.txtOrdenCompraTotal.Text, out dcmOrdenCompraTotal))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese el subtotal de la orden de compra del cliente");
                return false;
            }
            dcmOrdenCompraTotal = Math.Round(dcmOrdenCompraTotal, 2);
            this.txtOrdenCompraTotal.Text = dcmOrdenCompraTotal.ToString();

            decimal dcmDiferencia = decimal.Parse(this.hdCosto.Value) - decimal.Parse(this.txtOrdenCompraTotal.Text);
            if (dcmDiferencia > 0.5m || dcmDiferencia < -0.5m)
            {
                this.lblMensaje.Text = "Subtotal de la orden de compra del cliente no coincide con el subtotal, diferencia: " + dcmDiferencia;
            }
        }

        DateTime? dtFechaEntrega = null;

        if (!string.IsNullOrEmpty(this.txtFechaEntrega.Text))
            dtFechaEntrega = DateTime.Parse(this.txtFechaEntrega.Text, CultureInfo.CreateSpecificCulture("es-MX"));

        string strQuery = "UPDATE orden_compra SET " +
                    "sucursalID = " + this.hdSucursalID.Value +
                    ",orden_compra = '" + this.txtOrdenCompra.Text.Trim() + "'" +
                    ",orden_compra_total = " + dcmOrdenCompraTotal.ToString() +
                    ",requerimientos = '" + this.txtRequerimientos.Text.Trim().Replace("'", "''") + "'" +
                    ",tiempo_entrega = '" + this.txtTiempoEntrega.Text.Trim().Replace("'", "''") + "'" +
                    ",lugar_entrega = '" + this.txtLugarEntrega.Text.Trim().Replace("'", "''") + "'" +
                    ",fecha_entrega = " + (dtFechaEntrega.HasValue ? "'" + dtFechaEntrega.Value.ToString("yyyy-MM-dd") + "'" : "null") +
                    ",documento = " + this.dlDocumento.SelectedValue +
                    ",comentarios = '" + this.txtComentarios.Text.Trim().Replace("'", "''") + "'" +
                    ",estatus = " + this.dlEstatus.SelectedValue +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        ((master_MasterPage)Page.Master).MostrarMensajeError("El pedido ha sido modificado");

        return true;
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        //Refresca los datos cada 30 segundos
        //En milisegundos 30 seg = 30 * 1000 = 30000
        this.tmrRefresh.Interval = 30000;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlAlmacen.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    private bool Validar_Consistencia()
    {
        string strQuery = string.Empty;
        DataSet objDataResult = new DataSet();
        foreach (GridViewRow gvRow in this.gvProductosAlmacen.Rows)
        {
            string[] strID = this.gvProductosAlmacen.DataKeys[gvRow.RowIndex].Value.ToString().Split('_');
            strQuery = "SELECT 1 FROM orden_compra_productos " +
                      " WHERE orden_compraID = " + this.hdID.Value +
                      "  AND productoID = " + strID[0] +
                      "  AND consecutivo = " + strID[1];
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count == 0)
                return false;
        }
        return true;
    }

    protected void btnGuardarAlmacen_Click(object sender, ImageClickEventArgs e)
    {
        if (!Validar_Consistencia())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("El pedido fue modificado por otra persona así que hay que volver a abrirlo");
            return;
        }

        int intLoteSuma = 0;
        int intCantIngr = 0;
        int intFaltante = 0;
        int intCantidad = 0;
        int intCantidadLote = 0;
        bool swParcial = false;
        bool swNoMas = false;
        foreach (GridViewRow gvRow in this.gvProductosAlmacen.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (((HiddenField)gvRow.FindControl("hdValidado")).Value.Equals("0"))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Código no corresponde al producto " + ((Label)gvRow.FindControl("lblProducto")).Text);
                    return;
                }
                if (((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows.Count > 0)
                {
                    intLoteSuma = 0;
                    intCantidad = int.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value);
                    swNoMas = false;
                    foreach (GridViewRow gvRow2 in ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows)
                    {
                        intCantidadLote = int.Parse(((HiddenField)gvRow2.FindControl("hdCantidadLote")).Value);
                        if (!int.TryParse(((TextBox)gvRow2.FindControl("txtCantidadLote")).Text.Trim(), out intCantIngr))
                        {
                            ((master_MasterPage)Page.Master).MostrarMensajeError("La cantidad ingresada del lote " + ((Label)gvRow2.FindControl("lblLote")).Text + " debe ser numérica, producto: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                            return;
                        }
                        if (intCantIngr > 0)
                        {
                            if (intCantIngr > intCantidadLote)
                            {
                                ((master_MasterPage)Page.Master).MostrarMensajeError("La cantidad ingresada del lote " + ((Label)gvRow2.FindControl("lblLote")).Text + " es mayor a la existente, producto: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                                return;
                            }

                            if (this.hdValidar.Value.Equals("1"))
                            {
                                if (swNoMas)
                                {
                                    ((master_MasterPage)Page.Master).MostrarMensajeError("Debe utilizar los productos de los lotes más antiguos: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                                    return;
                                }

                                if (intCantIngr < intCantidadLote)
                                    swNoMas = true;
                            }

                            intLoteSuma += intCantIngr;
                            intCantidad -= intCantIngr;
                        }
                        else
                            if (this.hdValidar.Value.Equals("1"))
                                swNoMas = true;
                    }
                    if (intLoteSuma == 0)
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese la cantidad de al menos un lote del producto: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                        return;
                    }

                    intCantidad = int.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value);
                    if (intLoteSuma > intCantidad)
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("La cantidad ingresada es mayor a la solicitada o faltante, producto: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                        return;
                    }

                    intFaltante = intCantidad - intLoteSuma;

                    if (intFaltante == 0)
                    {
                        ((TextBox)gvRow.FindControl("txtFaltante")).Text = "0";
                        ((CheckBox)gvRow.FindControl("chkExistencia")).Checked = true;
                    }
                    else
                        ((TextBox)gvRow.FindControl("txtFaltante")).Text = intFaltante.ToString();
                }
                else
                    if(this.hdInventarios.Value.Equals("1"))
                        ((TextBox)gvRow.FindControl("txtFaltante")).Text = ((HiddenField)gvRow.FindControl("hdCantidad")).Value;

                if (!int.TryParse(((TextBox)gvRow.FindControl("txtFaltante")).Text.Trim(), out intFaltante))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Faltante del producto debe ser numérica: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                    return;
                }
                if (!((CheckBox)gvRow.FindControl("chkExistencia")).Checked)
                {
                    swParcial = true;
                    if (intFaltante <= 0)
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Favor de indicar la cantidad faltante: " + ((Label)gvRow.FindControl("lblProducto")).Text);
                        return;
                    }
                }
                else
                    ((TextBox)gvRow.FindControl("txtFaltante")).Text = "0";
            }
        }

        string strTemp = string.Empty;
        string strQuery = string.Empty;
        StringBuilder strError = new StringBuilder("<br/>");
        foreach (GridViewRow gvRow in this.gvProductosAlmacen.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows.Count > 0)
                {
                    foreach (GridViewRow gvRow2 in ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows)
                    {
                        if (int.Parse(((TextBox)gvRow2.FindControl("txtCantidadLote")).Text.Trim()) > 0)
                        {
                            decimal dcmCantidadNoInv;
                            Agregar_Lote_Inventariar(this.gvProductosAlmacen.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                     ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).DataKeys[gvRow2.RowIndex].Value.ToString(),
                                                     decimal.Parse(((TextBox)gvRow2.FindControl("txtCantidadLote")).Text.Trim()),
                                                     out dcmCantidadNoInv);
                            if (dcmCantidadNoInv != 0)   //Esta cantidad no se descontó del inventario
                            {
                                swParcial = true;
                                ((CheckBox)gvRow.FindControl("chkExistencia")).Checked = false;
                                ((TextBox)gvRow.FindControl("txtFaltante")).Text = (dcmCantidadNoInv +
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtFaltante")).Text)).ToString("0.##");
                                string strLote = ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).DataKeys[gvRow2.RowIndex].Value.ToString();
                                if (string.IsNullOrEmpty(strLote))
                                    strLote = "S/N_null";
                                string[] strDatos = strLote.Split('_');
                                strError.Append(((Label)gvRow.FindControl("lblProducto")).Text + " lote " + strDatos[0] + ", no se surtió la cantidad: " + dcmCantidadNoInv + "<br/>");
                            }
                        }
                    }
                }
                string[] strID = this.gvProductosAlmacen.DataKeys[gvRow.RowIndex].Value.ToString().Split('_');

                if (((CheckBox)gvRow.FindControl("chkExistencia")).Checked)
                    strTemp = "1";
                else
                    strTemp = "0";
                strQuery = "UPDATE orden_compra_productos SET " +
                    "validado = '" + strTemp + "'" +
                    ",faltante = " + ((TextBox)gvRow.FindControl("txtFaltante")).Text.Trim() +
                    " WHERE orden_compraID = " + this.hdID.Value +
                    " AND productoID = " + strID[0] +
                    " AND consecutivo = " + strID[1];
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
        }

        bool swFechaParcial = false;
        strQuery = "SELECT fecha_parcial" +
                  " FROM orden_compra" +
                  " WHERE ID = " + this.hdID.Value;
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (!objDataResult.Tables[0].Rows[0].IsNull("fecha_parcial"))
            swFechaParcial = true;

        if (!swParcial)
            strTemp = "3";  //Surtido
        else
            strTemp = "4";  //Surtido Parcial

        strQuery = "UPDATE orden_compra SET " +
                   "estatus = '" + strTemp + "'" +
                   ",comentarios = '" + this.txtComentariosAlmacen.Text.Trim().Replace("'", "''") + "'" +
                   ",modificadoPorID = " + Session["SIANID"].ToString() +
                   ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        if (strTemp.Equals("4") && !swFechaParcial)
            strQuery += ",fecha_parcial = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        strQuery += " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        ((master_MasterPage)Page.Master).MostrarMensajeError("El pedido fue actualizado" + strError.ToString());

        this.btnPonerProceso.Visible = false;
        this.btnGuardarAlmacen.Visible = false;
    }

    private void Agregar_Lote_Inventariar(string strConsecutivoID, string strLote,
                                          decimal dcmCantidad, out decimal dcmCantidadNoInv)
    {
        dcmCantidadNoInv = 0;
        string[] strID = strConsecutivoID.Split('_');
        DataSet objDataResult = new DataSet();

        if (string.IsNullOrEmpty(strLote))
            strLote = "_null";
        string[] strDatos = strLote.Split('_');

        decimal dcmCantidadInventario = CInventarios.Obtener_Existencia(strID[0], "100", strDatos[0]);

        if (dcmCantidadInventario < dcmCantidad)
        {
            if (dcmCantidadInventario == 0)
            {
                dcmCantidadNoInv = dcmCantidad;
                return;
            }

            dcmCantidadNoInv = dcmCantidad - dcmCantidadInventario;    //Es la cantidad que no se va a poder descontar del inventario
            dcmCantidad = dcmCantidadInventario;
        }

        string strQuery = "SELECT cantidad FROM orden_compra_productos_lote " +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         "  AND productoID = " + strID[0] +
                         "  AND consecutivo = " + strID[1] +
                         "  AND lote = '" + strDatos[0] + "'";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            strQuery = "INSERT INTO orden_compra_productos_lote (orden_compraID, " +
                      " productoID, consecutivo, lote, fecha_caducidad, cantidad) VALUES (" +
                      " '" + this.hdID.Value + "'" +
                      ", '" + strID[0] + "'" +
                      ", '" + strID[1] + "'" +
                      ", '" + strDatos[0] + "'" +
                      ", " + strDatos[1] +
                      ", '" + dcmCantidad + "'" +
                      ")";
        }
        else
        {
            strQuery = "UPDATE orden_compra_productos_lote " +
                      " SET cantidad = " + ((decimal)objDataResult.Tables[0].Rows[0][0] + dcmCantidad) +
                      " WHERE orden_compraID = " + this.hdID.Value +
                      "  AND productoID = " + strID[0] +
                      "  AND consecutivo = " + strID[1] +
                      "  AND lote = '" + strDatos[0] + "'";
        }
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        CInventarios.Restar(strID[0], "100",
                            strDatos[0],
                            dcmCantidad,
                            "Pedido " + this.hdID.Value);
    }

    private void Agregar_Lote_Temporal(string strConsecutivoID, string strLote, decimal dcmCantidad)
    {
        string[] strID = strConsecutivoID.Split('_');
        DataSet objDataResult = new DataSet();

        if (string.IsNullOrEmpty(strLote))
            strLote = "_null";
        string[] strDatos = strLote.Split('_');

        string strQuery = "SELECT 1 FROM orden_compra_productos_lote " +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         "  AND productoID = " + strID[0] +
                         "  AND consecutivo = " + strID[1] +
                         "  AND lote = '" + strDatos[0] + "'";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            strQuery = "INSERT INTO orden_compra_productos_lote (orden_compraID, " +
                      " productoID, consecutivo, lote, fecha_caducidad, cantidad, cantidad_temporal) VALUES (" +
                      " '" + this.hdID.Value + "'" +
                      ", '" + strID[0] + "'" +
                      ", '" + strID[1] + "'" +
                      ", '" + strDatos[0] + "'" +
                      ", " + strDatos[1] +
                      ", '0'" +
                      ", '" + dcmCantidad + "'" +
                      ")";
        }
        else
        {
            strQuery = "UPDATE orden_compra_productos_lote " +
                      " SET cantidad_temporal = " + dcmCantidad +
                      " WHERE orden_compraID = " + this.hdID.Value +
                      "  AND productoID = " + strID[0] +
                      "  AND consecutivo = " + strID[1] +
                      "  AND lote = '" + strDatos[0] + "'";
        }
        CComunDB.CCommun.Ejecutar_SP(strQuery);
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
            if (intCantidad <= 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad no puede ser menor o igual a cero");
                return;
            }

            if (dcmPrecioUnitario <= 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Precio no puede ser menor o igual a cero");
                return;
            }
            this.lblPrecioProducto.Text = this.txtProducto.Text;
            if (this.lblPrecioProducto.Text.Length > 30)
                this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 30);
            this.hdProductoPrecioID.Value = this.hdProductoID.Value;
            this.hdAccion.Value = "1";
            if (Validar_Producto_Remision(this.hdProductoID.Value,
                                          this.hdSucursalID.Value,
                                          this.lblPrecioProducto.Text,
                                          this.txtCantidad.Text))
            {
                if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                                      intCantidad, Math.Round(dcmPrecioUnitario, 2),
                                      this.dlListaPrecios.SelectedValue,
                                      this.rdNombre.SelectedValue, true,
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
                    strClientScript = "setTimeout('setAgrProd()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Validar_Producto_Remision(string strProductoID,
                                           string strSucursalID,
                                           string strProducto,
                                           string strCantidad)
    {
        this.txtCodigo_Verificacion.Text = string.Empty;
        if (strProducto.Length > 30)
            strProducto = strProducto.Substring(0, 30);
        this.lblNotasProducto.Text = strProducto;
        this.gvNotas.DataSource = ObtenerNotasRemision(strProductoID, strSucursalID);
        this.gvNotas.DataBind();

        if (this.gvNotas.Rows.Count > 0)
        {
            this.mdNotasRemision.Show();
            return false;
        }

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT IFNULL(SUM(existencia), 0) as existencia" +
                         " FROM inventario" +
                         " WHERE producto_ID = " + strProductoID +
                         "   AND subalmacen_ID = 100";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (decimal.Parse(objDataResult.Tables[0].Rows[0][0].ToString()) < decimal.Parse(strCantidad))
        {
            this.lblInventario.Text = "No hay suficiente cantidad en el inventario para este producto, existencia: " + decimal.Parse(objDataResult.Tables[0].Rows[0][0].ToString()).ToString("0.##");
            this.mdInventario.Show();
            return false;
        }

        return true;
    }

    protected void btnNotaContinuar_Click(object sender, EventArgs e)
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

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT IFNULL(SUM(existencia), 0) as existencia" +
                         " FROM inventario" +
                         " WHERE producto_ID = " + this.hdProductoID.Value +
                         "   AND subalmacen_ID = 100";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (decimal.Parse(objDataResult.Tables[0].Rows[0][0].ToString()) < decimal.Parse(this.txtCantidad.Text))
        {
            this.lblInventario.Text = "No hay suficiente cantidad en el inventario para este producto, existencia: " + decimal.Parse(objDataResult.Tables[0].Rows[0][0].ToString()).ToString("0.##");
            this.mdInventario.Show();
            return;
        }

        switch (this.hdAccion.Value)
        {
            case "1":
                Confirmacion_Agregar_Producto();
                break;
            case "3":
                Continuar_Agregar_ProdLista();
                break;
        }

        StringBuilder strTexto = new StringBuilder();
        try
        {
            StringWriter swTexto = new StringWriter(strTexto);
            HtmlTextWriter twTexto = new HtmlTextWriter(swTexto);
            this.pnlNotas.RenderControl(twTexto);
        }
        catch
        {
            strTexto = new StringBuilder();
        }

        CRutinas.Enviar_Correo("6",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Pedido: " + this.hdID.Value + "<br />" +
                               "Cliente: " + this.txtSucursal.Text + "<br />" +
                               "Razón: Cliente con notas de remisión sin pagar" + "<br /><br />" +
                               strTexto.ToString().Replace("display:none;", ""));
    }


    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

    protected void btnInventarioContinuar_Click(object sender, EventArgs e)
    {
        switch (this.hdAccion.Value)
        {
            case "1":
                Confirmacion_Agregar_Producto();
                break;
            case "3":
                Continuar_Agregar_ProdLista();
                break;
        }
    }

    private void Confirmacion_Agregar_Producto()
    {
        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        int.TryParse(this.txtCantidad.Text.Trim(), out intCantidad);
        decimal.TryParse(this.txtPrecioUnitario.Text.Trim(), out dcmPrecioUnitario);
        string strMensaje = string.Empty;
        if (!Agregar_Producto(this.hdProductoID.Value, this.txtProducto.Text,
                              intCantidad, Math.Round(dcmPrecioUnitario, 2),
                              this.dlListaPrecios.SelectedValue,
                              this.rdNombre.SelectedValue,
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
                    this.lblPrecioLista.Text = this.dlListaPrecios.SelectedItem.Text;
                    this.lblPrecioAnterior.Text = dcmPrecioUnitarioOrig.ToString("c");
                    this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                    this.txtPrecioUnitarioCambio.Focus();
                    this.mdCambiarPrecio.Show();
                    return;
                }
            }
    }

    private bool Agregar_Producto(string strProductoID, string strProducto,
                                int intCantidad, decimal dcmCosto_unitario,
                                string listaPrecios, string strUsarOtro, bool llenarProds,
                                out string strMensaje)
    {
        DataSet objDataResult = new DataSet();

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
                    " FROM orden_compra_productos " +
                    " WHERE orden_compraID = " + this.hdID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO orden_compra_productos (orden_compraID, " +
                "productoID, exento, cantidad, costo_unitario, costo," +
                "validado, faltante, consecutivo, usar_sal, usar_cve_gob) VALUES (" +
                "'" + this.hdID.Value + "'" +
                ", '" + strProductoID + "'" +
                ", '" + strExento + "'" +
                ", '" + intCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '0'" +
                ", '0'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", '" + (strUsarOtro.Equals("1") ? "1" : "0") + "'" +
                ", '" + (strUsarOtro.Equals("2") ? "1" : "0") + "'" +
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
                decimal dcmCosto, dcmCostoIVA, dcmIVA, dcmTotal;
                decimal.TryParse(this.hdCosto.Value, out dcmCosto);
                decimal.TryParse(this.hdCostoIVA.Value, out dcmCostoIVA);
                dcmCosto = Math.Round(dcmCosto, 2);
                dcmCostoIVA = Math.Round(dcmCostoIVA, 2);
                dcmIVA = Math.Round(dcmCostoIVA * decimal.Parse(this.rdIVA.SelectedValue) / 100, 2);
                this.hdIVA.Value = dcmIVA.ToString("0.00");
                dcmTotal = Math.Round(dcmCosto + dcmIVA, 2);
                this.hdTotal.Value = dcmTotal.ToString();

                e.Row.Cells[0].ColumnSpan = 8;
                e.Row.Cells[0].Text = "Subtotal:<br />IVA " + this.rdIVA.SelectedValue + "%:<br />TOTAL:";
                e.Row.Cells[1].Text = dcmCosto.ToString("c") + "<br />" +
                                      dcmIVA.ToString("c") + "<br />" +
                                      dcmTotal.ToString("c");
                e.Row.Cells[2].Text = string.Empty;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[7].Visible = false;
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
            }
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("mv"))
            return;

        int index = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Borrar")
        {
            if (this.gvProductos.Rows[index].Cells[10].Text.Equals("Surtido") ||
                this.gvProductos.Rows[index].Cells[10].Text.StartsWith("Dev") ||
                !this.gvProductos.Rows[index].Cells[11].Text.Equals("0"))
            {
                this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                this.hdAccionVerif.Value = "2";
                this.lblVerificacionH.Text = "Producto ya surtido";
                this.lblVerificacion.Text = "El producto que se quiere borrar ya había sido surtido total o parcialmente<br/>Ingrese el código de verificación para hacer el borrado y regresar el producto surtido al inventario";
                this.mdVerificacion.Show();
                return;
            }

            Borrar_Producto(this.gvProductos.DataKeys[index].Value.ToString());
        }
        else
            if (e.CommandName == "Modificar")
            {
                string[] strID_Consecutivo = this.gvProductos.DataKeys[index].Value.ToString().Split('_');
                if (this.gvProductos.Rows[index].Cells[10].Text.Equals("Surtido") ||
                    this.gvProductos.Rows[index].Cells[10].Text.StartsWith("Dev"))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Producto ya fue surtido/devuelto y no puede modificarse");
                    return;
                }

                this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                this.lblCambiarProducto.Text = this.gvProductos.Rows[index].Cells[2].Text;
                this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[6].Text;
                this.txtCambiarUnitario.Text = this.gvProductos.Rows[index].Cells[7].Text.Replace("$", "").Replace(",", "");
                this.rdNombreCambiar.ClearSelection();
                this.rdNombreCambiar.Items.FindByValue(((HiddenField)this.gvProductos.Rows[index].Cells[1].Controls[7]).Value).Selected = true;
                if (int.Parse(this.gvProductos.Rows[index].Cells[11].Text) > 0)
                    this.hdSurtido.Value = (int.Parse(this.gvProductos.Rows[index].Cells[6].Text) -
                                            int.Parse(this.gvProductos.Rows[index].Cells[11].Text)).ToString();
                else
                    this.hdSurtido.Value = "0";
                this.mdCambiarProducto.Show();
                string strClientScript = "setTimeout('setProductoCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
            }
    }

    private void Borrar_Producto(string strID)
    {
        string[] strID_Consecutivo = strID.Split('_');

        // Regresa el inventario
        string strQuery = "SELECT lote, fecha_caducidad, cantidad " +
                         " FROM orden_compra_productos_lote " +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         " AND productoID = " + strID_Consecutivo[0] +
                         " AND consecutivo = " + strID_Consecutivo[1];

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        DateTime? dtFecha;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dtFecha = null;
            if (!objRowResult.IsNull("fecha_caducidad"))
                dtFecha = (DateTime)objRowResult["fecha_caducidad"];
            CInventarios.Sumar(strID_Consecutivo[0], "100",
                               objRowResult["lote"].ToString(),
                               dtFecha,
                               (decimal)objRowResult["cantidad"],
                               "Pedido " + this.hdID.Value);
        }

        // Borra el producto
        strQuery = "DELETE " +
                  " FROM orden_compra_productos " +
                  " WHERE orden_compraID = " + this.hdID.Value +
                  " AND productoID = " + strID_Consecutivo[0] +
                  " AND consecutivo = " + strID_Consecutivo[1] + ";" +
                  " DELETE " +
                  " FROM orden_compra_productos_lote " +
                  " WHERE orden_compraID = " + this.hdID.Value +
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

        strQuery = "UPDATE orden_compra_productos SET " +
                  " consecutivo = consecutivo - 1 " +
                  " WHERE orden_compraID = " + this.hdID.Value +
                  " AND consecutivo > " + strID_Consecutivo[1] + ";" +
                  "UPDATE orden_compra_productos_lote SET " +
                  " consecutivo = consecutivo - 1 " +
                  " WHERE orden_compraID = " + this.hdID.Value +
                  " AND consecutivo > " + strID_Consecutivo[1];

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Llenar_Productos(true);
    }

    private void Llenar_Productos(bool swGuardarCompra)
    {
        this.hdCosto.Value = "0";
        this.hdCostoIVA.Value = "0";
        this.hdIVA.Value = "0.00";
        this.hdTotal.Value = "0";

        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        decimal dcmCosto, dcmCostoIVA, dcmIVA, dcmTotal;
        decimal.TryParse(this.hdCosto.Value, out dcmCosto);
        dcmCosto = Math.Round(dcmCosto, 2);

        decimal.TryParse(this.hdCostoIVA.Value, out dcmCostoIVA);
        dcmCostoIVA = Math.Round(dcmCostoIVA, 2);

        decimal.TryParse(this.hdIVA.Value, out dcmIVA);
        dcmIVA = Math.Round(dcmIVA, 2);

        decimal.TryParse(this.hdTotal.Value, out dcmTotal);
        dcmTotal = Math.Round(dcmTotal, 2);

        string strQuery = "UPDATE orden_compra SET " +
                    "costo = " + dcmCosto.ToString() +
                    ",porc_iva = " + this.rdIVA.SelectedValue +
                    ",iva = " + dcmIVA.ToString() +
                    ",total = " + dcmTotal.ToString();

        if (swGuardarCompra)
            strQuery += ",modificadoPorID = " + Session["SIANID"].ToString() +
                        ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        strQuery += " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        if (!string.IsNullOrEmpty(this.txtOrdenCompra.Text))
        {
            decimal dcmDiferencia = decimal.Parse(this.hdCosto.Value) - decimal.Parse(this.txtOrdenCompraTotal.Text);
            if (dcmDiferencia > 0.5m || dcmDiferencia < -0.5m)
            {
                this.lblMensaje.Text = "Subtotal de la orden de compra del cliente no coincide con el subtotal, diferencia: " + dcmDiferencia;
                if (!this.dlEstatus.SelectedValue.Equals("1"))
                    this.btnAjustar.Visible = true;
            }
        }
    }

    protected void btnCancelarContinuar_Click(object sender, EventArgs e)
    {
        if (this.dlEstatus.SelectedValue.Equals("3") || // Surtido
            this.dlEstatus.SelectedValue.Equals("4") || // Surtido parcial
            this.dlEstatus.SelectedValue.Equals("5"))   // Parcial Listo
        {
            this.hdAccion.Value = "3";
            this.lblLimiteCR.Text = string.Empty;
            this.mdLimiteCR.Show();
            return;
        }

        Cancelar_Pedido();
    }

    private void Cancelar_Pedido()
    {
        string strQuery = "UPDATE orden_compra SET " +
                    "estatus = 9" +
                    ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    ",motivo_cancelacion = '" + this.txtMotivoCancelacion.Text.Trim().Replace("'", "''") + "'" +
                    ",modificadoPorID = " + Session["SIANID"].ToString() +
                    ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        //Regresar Inventario
        strQuery = "SELECT productoID, lote, fecha_caducidad, cantidad " +
                  " FROM orden_compra_productos_lote " +
                  " WHERE orden_compraID = " + this.hdID.Value;
                  //" AND cantidad_temporal is null";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        DateTime? dtFecha;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dtFecha = null;
            if (!objRowResult.IsNull("fecha_caducidad"))
                dtFecha = (DateTime)objRowResult["fecha_caducidad"];
            CInventarios.Sumar(objRowResult["productoID"].ToString(), "100",
                               objRowResult["lote"].ToString(),
                               dtFecha,
                               (decimal)objRowResult["cantidad"],
                               "Pedido " + this.hdID.Value);
        }

        Mostrar_Datos_General();
    }

    protected void btnCambiarContinuar_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad, dcmPrecio, dcmSurtido;

        decimal.TryParse(this.txtCambiarCantidad.Text, out dcmCantidad);
        decimal.TryParse(this.txtCambiarUnitario.Text, out dcmPrecio);
        decimal.TryParse(this.hdSurtido.Value, out dcmSurtido);

        if (dcmCantidad > 0 && dcmPrecio > 0)
        {
            string strValidado = "0";
            decimal dcmFaltante = 0;
            if (dcmSurtido > 0)
            {
                if (dcmCantidad < dcmSurtido)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("La cantidad no puede ser menor a la ya surtida: " + this.hdSurtido.Value);
                    return;
                }
                dcmFaltante = Math.Round(dcmCantidad - dcmSurtido, 2);
                if (dcmFaltante == 0)
                    strValidado = "1";
            }

            string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');

            dcmCantidad = Math.Round(dcmCantidad, 2);
            dcmPrecio = Math.Round(dcmPrecio, 2);

            string strQuery = "UPDATE orden_compra_productos SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                        ",usar_sal = " + (this.rdNombreCambiar.SelectedValue.Equals("1") ? "1" : "0") +
                        ",usar_cve_gob = " + (this.rdNombreCambiar.SelectedValue.Equals("2") ? "1" : "0") +
                        ",faltante = " + dcmFaltante +
                        ",validado = " + strValidado +
                        " WHERE orden_compraID = " + this.hdID.Value +
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
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad/Precio no puede ser menor o igual a cero");
    }

    private DataTable ObtenerProductosCotizacion()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dt.Columns.Add(new DataColumn("costo", typeof(string)));
        dt.Columns.Add(new DataColumn("sal", typeof(string)));
        dt.Columns.Add(new DataColumn("enabled", typeof(bool)));

        string strQuery = "SELECT * FROM ( " +
            "SELECT C.productoID as productoID " +
            ", C.cantidad as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", C.costo_unitario as costo_unitario " +
            ", C.costo as costo " +
            ", C.usar_sal as usar_sal " +
            ", C.usar_cve_gob " +
            ", P.nombre as productoSORT " +
            ", LEFT(P.nombre, 80) as producto " +
            ", LEFT(P.sales, 80) as sales " +
            ", LEFT(P.clave_gobierno, 80) as clave_gobierno " +
            ", C.exento as exento " +
            ", C.productoAltID as productoAltID " +
            " FROM cotizacion_productos C " +
            " INNER JOIN productos P " +
            " ON C.productoID = P.ID " +
            " AND cotizacionID = " + this.txtCotizacionID.Text.Trim() +
            " AND productoAltID = 0 " +
            " UNION ALL " +
            "SELECT P2.ID as productoID " +
            ", CEILING(C.cantidad * P.relacion) as cantidad " +
            ", C.consecutivo as consecutivo " +
            ", truncate(costo / CEILING(C.cantidad * P.relacion), 2) as costo_unitario " +
            ", CEILING(C.cantidad * P.relacion) * truncate(costo / CEILING(C.cantidad * P.relacion), 2) as costo " +
            ", C.usar_sal as usar_sal " +
            ", C.usar_cve_gob " +
            ", P3.nombre as productoSORT " +
            ", LEFT(P2.nombre, 80) as producto " +
            ", LEFT(P2.sales, 80) as sales " +
            ", LEFT(P2.clave_gobierno, 80) as clave_gobierno " +
            ", C.exento as exento " +
            ", C.productoAltID as productoAltID " +
            " FROM cotizacion_productos C " +
            " INNER JOIN alternativos P " +
            " ON C.productoID = P.ID " +
            " AND cotizacionID = " + this.txtCotizacionID.Text.Trim() +
            " AND productoAltID <> 0 " +
            " INNER JOIN productos P2 " +
            " ON P.productoID = P2.ID " +
            " INNER JOIN productos P3 " +
            " ON C.productoAltID = P3.ID " +
            ") AS A ORDER BY consecutivo, productoSORT, productoAltID, producto";
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
            if ((int)objRowResult["productoAltID"] == 0)
                if (!objRowResult["usar_sal"].ToString().Equals("0"))
                {
                    dr[5] = "1";
                    dr[1] = objRowResult["sales"].ToString();

                }
                else
                    if (!objRowResult["usar_cve_gob"].ToString().Equals("0"))
                    {
                        dr[5] = "2";
                        dr[1] = objRowResult["clave_gobierno"].ToString();

                    }
                    else
                    {
                        dr[5] = "0";
                        dr[1] = objRowResult["producto"].ToString();
                    }
            else
                if (!objRowResult["usar_sal"].ToString().Equals("0"))
                {
                    dr[5] = "1";
                    dr[1] = "(Sugerencia) " + objRowResult["sales"].ToString();

                }
                else
                    if (!objRowResult["usar_cve_gob"].ToString().Equals("0"))
                    {
                        dr[5] = "2";
                        dr[1] = "(Sugerencia) " + objRowResult["clave_gobierno"].ToString();

                    }
                    else
                    {
                        dr[5] = "0";
                        dr[1] = "(Sugerencia) " + objRowResult["producto"].ToString();
                    }

            dr[2] = ((decimal)objRowResult["cantidad"]).ToString("#.###");
            dr[3] = objRowResult["costo_unitario"].ToString();
            dr[4] = ((decimal)objRowResult["costo"]).ToString("c");
            if (this.hdUsuPr.Value.Equals("0"))
                dr[6] = false;
            else
                dr[6] = true;
            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected void btnCotizacion_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlCotizacion.Visible = true;
        this.pnlCotizacionDatos.Visible = false;
        this.txtCotizacionID.Text = string.Empty;
        this.btnProcesar.Visible = false;
        this.txtCotizacionID.Enabled = true;
        this.btnObtenerCotizacion.Enabled = true;
        this.gvProductosCotizacion.DataSource = null;
        this.gvProductosCotizacion.DataBind();
    }

    protected void btnCotejada_Click(object sender, EventArgs e)
    {
        if (this.gvProductos.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese al menos un producto");
            return;
        }

        string strMensaje = CComunDB.CCommun.Validar_Limite(0, int.Parse(this.hdSucursalID.Value), decimal.Parse(this.hdTotal.Value));
        if(!string.IsNullOrEmpty(strMensaje))
        {
            this.hdAccion.Value = "1";
            this.lblLimiteCR.Text = strMensaje;
            this.mdLimiteCR.Show();
            return;
        }

        Poner_Como_Cotejada();
    }

    protected void btnAjustar_Click(object sender, EventArgs e)
    {
        this.hdAccion.Value = "2";
        this.lblLimiteCR.Text = "El subtotal de la orden de compra del cliente se igualará al subtotal de los productos capturados";
        this.mdLimiteCR.Show();
        return;
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

        switch (this.hdAccion.Value)
        {
            case "1":
                Poner_Como_Cotejada();
                CRutinas.Enviar_Correo("6",
                                       "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                                       "Código usado: " + strCodigo + "<br />" +
                                       "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                                       "Pedido: " + this.hdID.Value + "<br />" +
                                       "Cliente: " + this.txtSucursal.Text + "<br />" +
                                       "Razón: Cliente ha alcanzado su límite de crédito" + "<br /><br />" +
                                       strTexto.ToString().Replace("display:none;", ""));
                break;
            case "2":
                Ajustar_Subtotal();
                CRutinas.Enviar_Correo("6",
                                       "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                                       "Código usado: " + strCodigo + "<br />" +
                                       "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                                       "Pedido: " + this.hdID.Value + "<br />" +
                                       "Cliente: " + this.txtSucursal.Text + "<br />" +
                                       "Razón: Subtotal de la orden de compra ha sido igualado al subtotal de los productos capturados" + "<br /><br />" +
                                       strTexto.ToString().Replace("display:none;", ""));
                break;
            case "3":
                Cancelar_Pedido();
                CRutinas.Enviar_Correo("6",
                                       "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                                       "Código usado: " + strCodigo + "<br />" +
                                       "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                                       "Pedido: " + this.hdID.Value + "<br />" +
                                       "Cliente: " + this.txtSucursal.Text + "<br />" +
                                       "Razón: Pedido en estatus Surtido/Surtido Parcial/Parcial Listo ha sido cancelado" + "<br /><br />" +
                                       strTexto.ToString().Replace("display:none;", ""));
                break;
        }
    }

    private void Poner_Como_Cotejada()
    {
        bool swSurtido = true;
        string strEstatus = "2";
        foreach (GridViewRow objRow in this.gvProductos.Rows)
        {
            if (!objRow.Cells[10].Text.Equals("Surtido") &&
                !objRow.Cells[10].Text.StartsWith("Dev"))
            {
                swSurtido = false;
                break;
            }
        }

        if (swSurtido)
        {
            strEstatus = "3";
            ((master_MasterPage)Page.Master).MostrarMensajeError("Todos los productos del pedido ya están surtidos así que se pone como Surtido");
        }

        string strQuery = "UPDATE orden_compra SET " +
                          "estatus = " + strEstatus +
                          ",modificadoPorID = " + Session["SIANID"].ToString() +
                          ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlAlmacen.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    private void Ajustar_Subtotal()
    {
        this.txtOrdenCompraTotal.Text = this.hdCosto.Value;
        string strQuery = "UPDATE orden_compra SET" +
                         " orden_compra_total = " + this.hdCosto.Value +
                         ",modificadoPorID = " + Session["SIANID"].ToString() +
                         ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        this.lblMensaje.Text = string.Empty;
        this.btnAjustar.Visible = false;
    }

    protected void btnRegresarCotizacion_Click(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Mostrar_Datos(string.Empty);
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlCotizacion.Visible = false;
    }

    protected void btnObtenerCotizacion_Click(object sender, EventArgs e)
    {
        long lngCotizacionID = 0;
        if (!long.TryParse(this.txtCotizacionID.Text.Trim(), out lngCotizacionID))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Folio debe ser numérico");
            return;
        }

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT sucursalID, concat(negocio, ' - ', sucursal) as nombre, " +
                    " estatus, requerimientos, " +
                    " tiempo_entrega, lugar_fecha_entrega, estatus, " +
                    " lista_preciosID, porc_iva " +
                    " FROM cotizacion C" +
                    " INNER JOIN sucursales S " +
                    " ON C.sucursalID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " WHERE C.ID = " + this.txtCotizacionID.Text.Trim();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cotización no existe");
            return;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];
        if (objRowResult["estatus"].ToString().Equals("9"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cotización ha sido cancelada, seleccione otra");
            return;
        }

        this.txtRequerimientosCotizacion.Text = objRowResult["requerimientos"].ToString();
        this.txtTiempoEntregaCotizacion.Text = objRowResult["tiempo_entrega"].ToString();
        this.txtLugarEntregaCotizacion.Text = objRowResult["lugar_fecha_entrega"].ToString();
        this.txtSucursalCotizacion.Text = objRowResult["nombre"].ToString();
        this.hdSucursalCotizacionID.Value = objRowResult["sucursalID"].ToString();

        this.hdListaPreciosCotizacion.Value = objRowResult["lista_preciosID"].ToString();
        this.hdIVACotizacion.Value = ((decimal)objRowResult["porc_iva"]).ToString("0.00");

        this.gvProductosCotizacion.DataSource = ObtenerProductosCotizacion();
        this.gvProductosCotizacion.DataBind();

        if (this.gvProductosCotizacion.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cotización no tiene productos");
            return;
        }

        this.btnObtenerCotizacion.Enabled = false;
        this.txtCotizacionID.Enabled = false;
        this.btnProcesar.Visible = true;
        this.hdSeleccionar.Value = "1";

        Obtener_Notas2();

        this.pnlCotizacionDatos.Visible = true;
    }

    private bool Validar_Producto_RemisionNota(string strProductoID,
                                           string strSucursalID,
                                           string strProducto)
    {
        this.txtCodigo_VerificacionOrden.Text = string.Empty;
        if (strProducto.Length > 30)
            strProducto = strProducto.Substring(0, 30);
        this.lblNotaOrdenProducto.Text = strProducto;
        this.gvNotaOrden.DataSource = ObtenerNotasRemision(strProductoID, strSucursalID);
        this.gvNotaOrden.DataBind();

        if (this.gvNotaOrden.Rows.Count > 0)
        {
            this.mdNotaOrdenRemision.Show();
            return false;
        }
        return true;
    }

    protected void btnNotaOrdenContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if (DateTime.Parse(strValores[1]) >= DateTime.Now)
                strCodigo = strValores[0];
        }

        if (string.IsNullOrEmpty(this.txtCodigo_VerificacionOrden.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigo_VerificacionOrden.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
        }
        ((HiddenField)this.gvProductosCotizacion.Rows[int.Parse(this.hdIndex.Value)].FindControl("hdValidado")).Value = "1";

        StringBuilder strTexto = new StringBuilder();
        try
        {
            StringWriter swTexto = new StringWriter(strTexto);
            HtmlTextWriter twTexto = new HtmlTextWriter(swTexto);
            this.pnlNotaOrden.RenderControl(twTexto);
        }
        catch
        {
            strTexto = new StringBuilder();
        }

        CRutinas.Enviar_Correo("6",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Pedido: " + this.hdID.Value + "<br />" +
                               "Cliente: " + this.txtSucursal.Text + "<br />" +
                               "Razón: Cliente con notas de remisión sin pagar" + "<br /><br />" +
                               strTexto.ToString().Replace("display:none;", ""));
    }

    protected void btnProcesar_Click(object sender, EventArgs e)
    {
        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        bool swSeleccionados = false;
        foreach (GridViewRow gvRow in this.gvProductosCotizacion.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (!int.TryParse(((TextBox)gvRow.FindControl("txtCantidadCotizacion")).Text.Trim(), out intCantidad) ||
                    !decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoUnitarioCotizacion")).Text.Trim(), out dcmPrecioUnitario))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser numérica: " + ((CheckBox)gvRow.FindControl("chkProducto")).Text);
                    return;
                }
                ((TextBox)gvRow.FindControl("txtCantidadCotizacion")).Text = intCantidad.ToString();
                ((TextBox)gvRow.FindControl("txtCostoUnitarioCotizacion")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();
                if (!swSeleccionados && ((CheckBox)gvRow.FindControl("chkProducto")).Checked)
                    swSeleccionados = true;
                if (((CheckBox)gvRow.FindControl("chkProducto")).Checked &&
                    !((TextBox)gvRow.FindControl("txtCantidadCotizacion")).Text.Equals("0") &&
                    ((HiddenField)gvRow.FindControl("hdValidado")).Value.Equals("0"))
                {
                    if (!Validar_Producto_RemisionNota(this.gvProductosCotizacion.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                 this.hdSucursalCotizacionID.Value,
                                                 gvRow.Cells[0].Text))
                    {
                        this.hdIndex.Value = gvRow.RowIndex.ToString();
                        return;
                    }
                    else
                        ((HiddenField)gvRow.FindControl("hdValidado")).Value = "1";
                }

            }
        }

        if (!swSeleccionados)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione al menos un producto");
            return;
        }

        DateTime? dtFechaEntrega = null;

        string strMensajeorden_compra = string.Empty;
        if (this.hdID.Value.Equals("0"))
            if (Crear_Orden_Compra(this.hdSucursalCotizacionID.Value, string.Empty, 0,
                                this.hdListaPreciosCotizacion.Value,
                                this.txtRequerimientosCotizacion.Text.Trim(), this.txtTiempoEntregaCotizacion.Text.Trim(),
                                this.txtLugarEntregaCotizacion.Text.Trim(),
                                dtFechaEntrega,
                                string.Empty))
            {
                strMensajeorden_compra = "Pedido creado: " + this.hdID.Value + "<br>";
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                if (this.hdUsuPr.Value.Equals("0"))
                    this.txtPrecioUnitario.Enabled = false;
                else
                    this.txtPrecioUnitario.Enabled = true;
                this.btnModificar.Visible = true;
                this.btnImprimir.Visible = true;
                this.btnCancelar.Visible = true;
                this.btnAgregarProd.Enabled = true;
            }

        this.rdIVA.ClearSelection();
        this.rdIVA.Items.FindByValue(this.hdIVACotizacion.Value).Selected = true;

        StringBuilder strMensajeFinal = new StringBuilder();
        bool swAgregados = false;
        foreach (GridViewRow gvRow in this.gvProductosCotizacion.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (((CheckBox)gvRow.FindControl("chkProducto")).Checked && !((TextBox)gvRow.FindControl("txtCantidadCotizacion")).Text.Equals("0"))
                {
                    string strMensaje = string.Empty;
                    if (Agregar_Producto(this.gvProductosCotizacion.DataKeys[gvRow.RowIndex].Value.ToString(),
                                    ((CheckBox)gvRow.FindControl("chkProducto")).Text,
                                    int.Parse(((TextBox)gvRow.FindControl("txtCantidadCotizacion")).Text.Trim()),
                                    decimal.Parse(((TextBox)gvRow.FindControl("txtCostoUnitarioCotizacion")).Text.Trim()),
                                    this.hdIVACotizacion.Value,
                                    ((HiddenField)gvRow.FindControl("hdSal")).Value,
                                    false,
                                    out strMensaje))
                        swAgregados = true;
                    else
                    {
                        if (strMensajeFinal.Length > 0)
                            strMensajeFinal.Append("<br>");
                        strMensajeFinal.Append(strMensaje);
                    }
                }
            }
        }

        if (swAgregados)
        {
            string strQuery = "INSERT INTO cotizacion_orden_compra (cotizacionID, orden_compraID)  " +
                    "VALUES('" + this.txtCotizacionID.Text.Trim() + "'" +
                    ", '" + this.hdID.Value + "'" +
                    ")";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {
            }
        }

        if (strMensajeFinal.Length == 0)
            if (swAgregados)
            {
                Llenar_Productos(true);
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensajeorden_compra + "Los productos fueron agregados");
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensajeorden_compra + "Ningún producto fue agregado a la cotización");
        else
            if (swAgregados)
            {
                Llenar_Productos(true);
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensajeorden_compra + "Se agregaron productos, pero los siguientes no: <br>" + strMensajeFinal.ToString());
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError(strMensajeorden_compra + "Los productos no se agregaron: <br>" + strMensajeFinal.ToString());

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

    protected void dlListaPreciosCotizacion_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.gvProductosCotizacion.DataSource = ObtenerProductosCotizacion();
        this.gvProductosCotizacion.DataBind();
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarReporte('orden_compra_imprimir.aspx?c=" +
                                        this.hdID.Value +
                                        "&t=" + this.dlDocumento.SelectedValue +
                                        "')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }
    protected void grdvLista_RowCreated(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[7].Visible = false;
    }

    protected void tmrRefresh_Tick(object sender, EventArgs e)
    {
        if (this.pnlListado.Visible)
            Llenar_Grid();
        else
            Generar_Temporal();
    }

    protected void btnTemporal_Click(object sender, EventArgs e)
    {
        Generar_Temporal();
    }

    [System.Web.Services.WebMethod]
    public static string Obtener_Notas(string strParametros)
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.notas, E.descuento, E.descuento2, " +
                    " E.iva, E.lista_precios_ID, E.contado " +
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
               "|" + (Convert.ToBoolean(objRowResult["contado"]) ? "1" : "0");
    }

    private void Obtener_Notas2()
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT E.notas " +
                    " FROM establecimientos E " +
                    " INNER JOIN sucursales S " +
                    " ON S.establecimiento_ID = E.ID " +
                    " AND S.ID = " + this.hdSucursalCotizacionID.Value;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        this.lblNotas2.Text = objRowResult["notas"].ToString().Replace("\r\n", "<br />").Replace("\n", "<br />");

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

    protected void btnVendedorContinuar_Click(object sender, EventArgs e)
    {
        if(!Modificar_Orden())
            return;

        if (this.rdProductos.SelectedIndex == 1)
        {
            string strQuery = "SELECT COUNT(*) as cantidad" +
                             " FROM orden_compra_productos" +
                             " WHERE validado = 1" +
                             "   AND orden_compraID = " + this.hdID.Value;
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (int.Parse(objDataResult.Tables[0].Rows[0]["cantidad"].ToString()) == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido no tiene ningún producto en existencia");
                return;
            }
        }

        if (!string.IsNullOrEmpty(this.txtOrdenCompra.Text))
        {
            decimal dcmDiferencia = decimal.Parse(this.hdCosto.Value) - decimal.Parse(this.txtOrdenCompraTotal.Text);
            if (dcmDiferencia > 0.5m || dcmDiferencia < -0.5m)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Subtotal de la orden de compra del cliente no coincide con el subtotal, diferencia: " + dcmDiferencia);
                return;
            }
        }

        if (this.hdAccion.Value.Equals("1"))
            Generar_Factura();
        else
            Generar_Nota();
    }

    private void Generar_Factura()
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT contado, separar_facturas, E.ID" +        // Table 0 - contado, separa facturas
                         " FROM establecimientos E" +
                         " INNER JOIN sucursales S" +
                         " ON E.ID = S.establecimiento_ID" +
                         " AND S.ID = " + this.hdSucursalID.Value + ";" +
                         "SELECT DISTINCT 1" +                              // Table 1 - Tiene productos exentos
                         " FROM orden_compra_productos" +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         "   AND exento = 1;" +
                         "SELECT DISTINCT 1" +                              // Table 2 - Tiene productos no exentos
                         " FROM orden_compra_productos" +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         "   AND exento = 0;" +
                         "SELECT factura_ivaID, factura_sin_ivaID" +        // Table 3 - ID de las facturas 
                         " FROM facturas_iva F" +
                         " INNER JOIN sucursales S" +
                         " ON F.establecimientoID = S.establecimiento_ID" +
                         " AND S.ID = " + this.hdSucursalID.Value +
                         " LIMIT 1";

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        bool swSeparar = (bool)objDataResult.Tables[0].Rows[0]["separar_facturas"];
        int estabID = (int)objDataResult.Tables[0].Rows[0]["ID"];

        int intFacturaConIVAID = 0, intFacturaSinIVAID = 0;

        //Si tiene productos exentos y no exentos y hay que separar las facturas
        if (swSeparar &&
            objDataResult.Tables[1].Rows.Count > 0 &&
            objDataResult.Tables[2].Rows.Count > 0
            )
        {
            if(objDataResult.Tables[3].Rows.Count > 0)
            {
                if(!objDataResult.Tables[3].Rows[0].IsNull("factura_ivaID"))
                    intFacturaConIVAID = (int)objDataResult.Tables[3].Rows[0]["factura_ivaID"];

                if(!objDataResult.Tables[3].Rows[0].IsNull("factura_sin_ivaID"))
                    intFacturaSinIVAID = (int)objDataResult.Tables[3].Rows[0]["factura_sin_ivaID"];
            }
        }
        else
            swSeparar = false;

        CFacturaDB objFactura = new CFacturaDB();

        objFactura.electronica = true;
        objFactura.fecha = DateTime.Now;
        objFactura.sucursal_ID = int.Parse(this.hdSucursalID.Value);
        objFactura.iva = Decimal.Parse(this.rdIVA.SelectedValue);
        objFactura.contado = (bool)objDataResult.Tables[0].Rows[0]["contado"];
        objFactura.descuento = 0;
        objFactura.descuento2 = 0;
        objFactura.comentarios = this.txtComentarios.Text.Trim();
        objFactura.status = 8;
        objFactura.lista_precios_ID = int.Parse(this.dlListaPrecios.SelectedValue);

        objFactura.vendedorID = int.Parse(this.dlVendedor.SelectedValue);
        objFactura.isr_ret = 0;
        objFactura.iva_ret = 0;
        objFactura.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
        objFactura.total_real = objFactura.total;
        objFactura.orden_compra = this.txtOrdenCompra.Text.Trim();
        objFactura.comentarios = this.txtComentarios.Text.Trim();

        // Si es igual a ceros la tiene que crear
        if (intFacturaConIVAID == 0)
        {
            if (objFactura.Guardar())
            {
                intFacturaConIVAID = objFactura.ID;
            }
            else
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
                return;
            }
        }

        // Si hay que separar y aún no existe hay que crear una segunda factura
        if (swSeparar && intFacturaSinIVAID == 0)
        {
            objFactura.ID = 0;
            objFactura.fecha = DateTime.Now;
            objFactura.factura = string.Empty;

            if (objFactura.Guardar())
            {
                intFacturaSinIVAID = objFactura.ID;
            }
            else
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
                return;
            }
        }

        if (swSeparar)
        {
            // Ahora guarda las facturas
            if (objDataResult.Tables[3].Rows.Count > 0)
            {
                strQuery = "UPDATE facturas_iva SET" +
                          " factura_ivaID = " + intFacturaConIVAID +
                          ",factura_sin_ivaID = " + intFacturaSinIVAID +
                          " WHERE establecimientoID = " + estabID;
            }
            else
            {
                strQuery = "INSERT INTO facturas_iva " +
                          " (establecimientoID, factura_ivaID, factura_sin_ivaID)" +
                          " VALUES(" +
                                estabID +
                          "," + intFacturaConIVAID +
                          "," + intFacturaSinIVAID +
                          ")";
            }
            CComunDB.CCommun.Ejecutar_SP(strQuery);

            Generar_Factura_Continuar(intFacturaSinIVAID, 1);        // Guarda la factura con los productos exentos que NO generan IVA

            Generar_Factura_Continuar(intFacturaConIVAID, 2);        // Guarda la factura con los productos no exentos que generan IVA

            strQuery = "SELECT 1" +
                     " FROM facturas_iva_log" +
                     " WHERE factura_ivaID = " + intFacturaConIVAID +
                     "    OR factura_sin_ivaID = " + intFacturaSinIVAID;

            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count == 0)
            {
                strQuery = "INSERT INTO facturas_iva_log " +
                          " (factura_ivaID, factura_sin_ivaID)" +
                          " VALUES(" +
                                intFacturaConIVAID +
                          "," + intFacturaSinIVAID +
                          ")";

                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }

            ((master_MasterPage)Page.Master).MostrarMensajeError("Factura CON IVA, folio: " + intFacturaConIVAID + "<br/>" +
                                                                 "Factura SIN IVA, folio: " + intFacturaSinIVAID);
        }
        else
        {
            Generar_Factura_Continuar(intFacturaConIVAID, 0);       //Guarda la factura con los datos tal cual

            ((master_MasterPage)Page.Master).MostrarMensajeError("Factura ha sido creada, folio: " + intFacturaConIVAID);
        }

        // Cerrar remisiones asociadas
        strQuery = "UPDATE notas" +
                  " SET status = 4" +
                  " WHERE ID IN (" +
                  "    SELECT notaID" +
                  "    FROM orden_compra_nota" +
                  "    WHERE orden_compraID = " + this.hdID.Value +
                  " )";
        CComunDB.CCommun.Ejecutar_SP(strQuery);
    }

    private void Generar_Factura_Continuar(int intFacturaID, byte btTipo)
    {
        string strQuery = "SELECT 1 FROM orden_compra_productos" +
                         " WHERE orden_compraID = " + this.hdID.Value +
                         "   AND usar_cve_gob = 1";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
            strQuery = "SELECT P.*, L.lote, L.cantidad as cantlote, L.fecha_caducidad" +
                      " FROM orden_compra_productos P" +
                      " LEFT JOIN orden_compra_productos_lote L" +
                      " ON L.consecutivo = P.consecutivo" +
                      " AND L.orden_compraID = P.orden_compraID" +
                      " WHERE P.orden_compraID = " + this.hdID.Value;
        else
            strQuery = "SELECT P.*, '' as lote, P.cantidad as cantlote, null as fecha_caducidad" +
                      " FROM orden_compra_productos P" +
                      " WHERE P.orden_compraID = " + this.hdID.Value;

        if (this.rdProductos.SelectedIndex == 1)
            strQuery += " AND P.validado = 1";

        if (btTipo == 1)        //Producto exentos
            strQuery += " AND P.exento = 1";
        else if (btTipo == 2)   // Producto no exento
            strQuery += " AND P.exento = 0";


        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        int intIndex = 0;
        decimal dcmCantidad;
        decimal dcmCosto;
        short intConsecutivoAnt = 0;
        StringBuilder strDetalle = new StringBuilder();
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            if (intConsecutivoAnt != (short)objRowResult["consecutivo"])
            {
                if (intConsecutivoAnt != 0)
                {
                    dcmCantidad = (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["cantidad"];
                    dcmCosto = (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo"];
                    if (!objDataResult.Tables[0].Rows[intIndex - 1].IsNull("cantidad_devuelto"))
                    {
                        dcmCantidad -= (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["cantidad_devuelto"];
                        dcmCosto = dcmCantidad * (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo_unitario"];
                    }
                    if(dcmCantidad > 0)
                        Agregar_Producto_Factura(intFacturaID,
                                                 (int)objDataResult.Tables[0].Rows[intIndex - 1]["productoID"],
                                                 dcmCantidad,
                                                 (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo_unitario"],
                                                 dcmCosto,
                                                 (bool)objDataResult.Tables[0].Rows[intIndex - 1]["exento"],
                                                 (bool)objDataResult.Tables[0].Rows[intIndex - 1]["usar_sal"],
                                                 (bool)objDataResult.Tables[0].Rows[intIndex - 1]["usar_cve_gob"],
                                                 strDetalle.ToString());
                }
                intConsecutivoAnt = (short)objRowResult["consecutivo"];
                strDetalle.Clear();
            }

            if(!string.IsNullOrEmpty(objRowResult["lote"].ToString()) ||
               !objRowResult.IsNull("fecha_caducidad"))
            {
                if (strDetalle.Length != 0)
                    strDetalle.Append(", ");
                if(string.IsNullOrEmpty(objRowResult["lote"].ToString()))
                    strDetalle.Append("Lt. S/N");
                else
                    strDetalle.Append("Lt. " + objRowResult["lote"].ToString());
                strDetalle.Append(" Cant. " + ((decimal)objRowResult["cantlote"]).ToString("0.###"));
                if(!objRowResult.IsNull("fecha_caducidad"))
                    strDetalle.Append(" Cad. " + ((DateTime)objRowResult["fecha_caducidad"]).ToString("dd/MM/yyyy"));
            }

            intIndex++;
        }

        if (intConsecutivoAnt != 0)
        {
            dcmCantidad = (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["cantidad"];
            dcmCosto = (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo"];
            if (!objDataResult.Tables[0].Rows[intIndex - 1].IsNull("cantidad_devuelto"))
            {
                dcmCantidad -= (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["cantidad_devuelto"];
                dcmCosto = dcmCantidad * (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo_unitario"];
            }
            if (dcmCantidad > 0)
                Agregar_Producto_Factura(intFacturaID,
                                         (int)objDataResult.Tables[0].Rows[intIndex - 1]["productoID"],
                                         (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["cantidad"],
                                         (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo_unitario"],
                                         (decimal)objDataResult.Tables[0].Rows[intIndex - 1]["costo"],
                                         (bool)objDataResult.Tables[0].Rows[intIndex - 1]["exento"],
                                         (bool)objDataResult.Tables[0].Rows[intIndex - 1]["usar_sal"],
                                         (bool)objDataResult.Tables[0].Rows[intIndex - 1]["usar_cve_gob"],
                                         strDetalle.ToString());
        }

        strQuery = "INSERT INTO orden_compra_factura (orden_compraID, facturaID)  " +
                   "VALUES('" + this.hdID.Value + "'" +
                   ", '" + intFacturaID + "'" +
                   ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }        
    }

    private void Agregar_Producto_Factura(int intFacturaID, int intProductoID,
                                decimal dcmCantidad, decimal dcmCosto_unitario,
                                decimal dcmCosto, bool esExento, bool swSal,
                                bool swCveGob, string strDetalle)
    {
        DataSet objDataResult = new DataSet();

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
                "producto_ID, exento, cantidad, costo_unitario, " +
                "costo, consecutivo, usar_sal, usar_cve_gob, detalle) VALUES (" +
                "'" + intFacturaID.ToString() + "'" +
                ", '" + intProductoID + "'" +
                ", '" + (esExento ? "1" : "0") + "'" +
                ", '" + dcmCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", '" + (swSal ? "1" : "0") + "'" +
                ", '" + (swCveGob ? "1" : "0") + "'" +
                ", " + (string.IsNullOrEmpty(strDetalle) ? "null" : "'" + strDetalle + "'") +
                ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
            return;
        }
    }

    private void Generar_Nota()
    {
        string strQuery = "SELECT contado" +
                         " FROM establecimientos E" +
                         " INNER JOIN sucursales S" +
                         " ON E.ID = S.establecimiento_ID" +
                         " AND S.ID = " + this.hdSucursalID.Value;

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        int intNotaID = 0;

        CNotaDB objNota = new CNotaDB();
        DateTime dtFecha = DateTime.Now;

        objNota.fecha = dtFecha;
        objNota.sucursal_ID = int.Parse(this.hdSucursalID.Value);
        objNota.iva = Decimal.Parse(this.rdIVA.SelectedValue);
        objNota.contado = (bool)objDataResult.Tables[0].Rows[0]["contado"];
        objNota.descuento = 0;
        objNota.descuento2 = 0;
        objNota.comentarios = this.txtComentarios.Text.Trim();
        objNota.status = 8;
        objNota.lista_precios_ID = int.Parse(this.dlListaPrecios.SelectedValue);

        objNota.vendedorID = int.Parse(this.dlVendedor.SelectedValue);
        objNota.isr_ret = 0;
        objNota.iva_ret = 0;
        objNota.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
        objNota.total_real = objNota.total;
        objNota.orden_compra = this.txtOrdenCompra.Text.Trim();
        objNota.comentarios = this.txtComentarios.Text.Trim();

        if (objNota.Guardar())
        {
            intNotaID = objNota.ID;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objNota.Mensaje);
            return;
        }

        strQuery = "SELECT * FROM orden_compra_productos " +
                  " WHERE orden_compraID = " + this.hdID.Value;

        if (this.rdProductos.SelectedIndex == 1)
            strQuery += " AND validado = 1";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        decimal dcmCantidad;
        decimal dcmCosto;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dcmCantidad = (decimal)objRowResult["cantidad"];
            dcmCosto = (decimal)objRowResult["costo"];
            if (!objRowResult.IsNull("cantidad_devuelto"))
            {
                dcmCantidad -= (decimal)objRowResult["cantidad_devuelto"];
                dcmCosto = dcmCantidad * (decimal)objRowResult["costo_unitario"];
            }
            if (dcmCantidad > 0)
                Agregar_Producto_Nota(intNotaID, (int)objRowResult["productoID"],
                                 dcmCantidad,
                                 (decimal)objRowResult["costo_unitario"],
                                 dcmCosto,
                                 (bool)objRowResult["exento"],
                                 (bool)objRowResult["usar_sal"],
                                 (bool)objRowResult["usar_cve_gob"]);
        }

        strQuery = "INSERT INTO orden_compra_nota (orden_compraID, notaID)  " +
                   "VALUES('" + this.hdID.Value + "'" +
                   ", '" + intNotaID + "'" +
                   ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Nota ha sido creada, folio: " + intNotaID);
    }

    private void Agregar_Producto_Nota(int intNotaID, int intProductoID,
                                decimal dcmCantidad, decimal dcmCosto_unitario,
                                decimal dcmCosto, bool esExento, bool swSal, bool swCveGob)
    {
        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT IFNULL(MAX(consecutivo) + 1, 1) as consecutivo " +
                    " FROM notas_prod " +
                    " WHERE nota_ID = " + intNotaID;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + " " + strQuery);
        }

        strQuery = "INSERT INTO notas_prod (nota_ID, " +
                "producto_ID, exento, cantidad," +
                "costo_unitario, costo, consecutivo, usar_sal, usar_cve_gob) VALUES (" +
                "'" + intNotaID.ToString() + "'" +
                ", '" + intProductoID + "'" +
                ", '" + (esExento ? "1" : "0") + "'" +
                ", '" + dcmCantidad.ToString() + "'" +
                ", '" + dcmCosto_unitario.ToString() + "'" +
                ", '" + dcmCosto.ToString() + "'" +
                ", '" + objDataResult.Tables[0].Rows[0]["consecutivo"].ToString() + "'" +
                ", '" + (swSal ? "1" : "0") + "'" +
                ", '" + (swCveGob ? "1" : "0") + "'" +
                ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {
            return;
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
        string strQuery = "UPDATE orden_compra_productos SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND orden_compraID = " + this.hdID.Value + ";" +
                         "UPDATE orden_compra_productos_lote SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND orden_compraID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE orden_compra_productos SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND orden_compraID = " + this.hdID.Value + ";" +
                  "UPDATE orden_compra_productos_lote SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND orden_compraID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE orden_compra_productos SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND orden_compraID = " + this.hdID.Value + ";" +
                  "UPDATE orden_compra_productos_lote SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND orden_compraID = " + this.hdID.Value;

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

    protected void gvProductosAlmacen_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (this.hdInventarios.Value.Equals("1"))
            {
                if (!((CheckBox)e.Row.FindControl("chkExistencia")).Checked)
                {
                    string[] strID = this.gvProductosAlmacen.DataKeys[e.Row.RowIndex].Value.ToString().Split('_');
                    ((GridView)e.Row.FindControl("gvProductosAlmacenLote")).DataSource = ObtenerLotesAlmacen(strID[0],
                                                                                         ((HiddenField)e.Row.FindControl("hdTemporal")).Value);
                    ((GridView)e.Row.FindControl("gvProductosAlmacenLote")).DataBind();
                    if (((GridView)e.Row.FindControl("gvProductosAlmacenLote")).Rows.Count == 0)
                    {
                        ((TextBox)e.Row.FindControl("txtCodigoProd")).Visible = false;
                        ((HiddenField)e.Row.FindControl("hdValidado")).Value = "1";
                        if (string.IsNullOrEmpty(((Label)e.Row.FindControl("lblMensaje")).Text))
                            ((Label)e.Row.FindControl("lblMensaje")).Text = "NO hay inventario";
                        else
                            ((Label)e.Row.FindControl("lblMensaje")).Text += "; NO hay inventario";
                    }

                    // Verifica los productos temporales
                    if (((HiddenField)e.Row.FindControl("hdTemporal")).Value.Equals("1"))
                    {
                        if (((GridView)e.Row.FindControl("gvProductosAlmacenLote")).Rows.Count > 0)
                            ((HiddenField)e.Row.FindControl("hdValidado")).Value = "1";
                        string strQuery = "UPDATE orden_compra_productos" +
                                         " SET temporal = 0 " +
                                         " WHERE orden_compraID = " + this.hdID.Value +
                                         "   AND productoID = " + strID[0] +
                                         "   AND consecutivo = " + strID[1];
                        CComunDB.CCommun.Ejecutar_SP(strQuery);
                    }
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("btnRecargar")).Visible = false;
                    ((TextBox)e.Row.FindControl("txtCodigoProd")).Visible = false;
                    ((HiddenField)e.Row.FindControl("hdValidado")).Value = "1";
                }

                ((CheckBox)e.Row.FindControl("chkExistencia")).Enabled = false;
                ((TextBox)e.Row.FindControl("txtFaltante")).Enabled = false;
            }
        }
    }

    protected void btnRecargar_Click(object sender, CommandEventArgs e)
    {
        if (this.hdInventarios.Value.Equals("1"))
        {
            GridViewRow eRow = gvProductosAlmacen.Rows[int.Parse(e.CommandArgument.ToString())];
            if (!((CheckBox)eRow.FindControl("chkExistencia")).Checked)
            {
                bool swNoHay = false;
                if (((GridView)eRow.FindControl("gvProductosAlmacenLote")).Rows.Count == 0)
                {
                    swNoHay = true;
                    ((Label)eRow.FindControl("lblMensaje")).Text = ((Label)eRow.FindControl("lblMensaje")).Text.Replace("NO hay inventario", "").Replace("; ", "");
                }
                string[] strID = this.gvProductosAlmacen.DataKeys[eRow.RowIndex].Value.ToString().Split('_');
                ((GridView)eRow.FindControl("gvProductosAlmacenLote")).DataSource = ObtenerLotes(strID[0]);
                ((GridView)eRow.FindControl("gvProductosAlmacenLote")).DataBind();
                if (((GridView)eRow.FindControl("gvProductosAlmacenLote")).Rows.Count == 0)
                {
                    ((TextBox)eRow.FindControl("txtCodigoProd")).Visible = false;
                    ((HiddenField)eRow.FindControl("hdValidado")).Value = "1";
                    if (string.IsNullOrEmpty(((Label)eRow.FindControl("lblMensaje")).Text))
                        ((Label)eRow.FindControl("lblMensaje")).Text = "NO hay inventario";
                    else
                        ((Label)eRow.FindControl("lblMensaje")).Text += "; NO hay inventario";
                }
                else
                {
                    if (swNoHay)
                    {
                        ((TextBox)eRow.FindControl("txtCodigoProd")).Visible = true;
                        ((HiddenField)eRow.FindControl("hdValidado")).Value = "0";
                        ((TextBox)eRow.FindControl("txtCodigoProd")).Focus();
                    }
                    else
                    {
                        ((TextBox)((GridView)eRow.FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Focus();
                        ((TextBox)((GridView)eRow.FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Attributes["onfocus"] = "this.select();";
                    }
                }
            }
        }
    }

    protected void txtCodigoProd_TextChanged(object sender, EventArgs e)
    {
        TextBox txtProducto = (TextBox)sender;
        int intPos1 = txtProducto.ClientID.IndexOf("ctl", 6);
        int intPos2 = txtProducto.ClientID.IndexOf("_txt");

        int intInd = int.Parse(txtProducto.ClientID.Substring(intPos1 + 3, intPos2 - intPos1 - 3)) - 2;

        if (string.IsNullOrEmpty(txtProducto.Text))
        {
            ((HiddenField)gvProductosAlmacen.Rows[intInd].FindControl("hdValidado")).Value = "0";
            ((TextBox)((GridView)gvProductosAlmacen.Rows[intInd].FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Focus();
            ((TextBox)((GridView)gvProductosAlmacen.Rows[intInd].FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Attributes["onfocus"] = "this.select();";
            return;
        }

        string[] strCodigos = ((HiddenField)gvProductosAlmacen.Rows[intInd].FindControl("hdCodigos")).Value.Split('.');

        if (strCodigos.Length != 3)
        {
            ((HiddenField)gvProductosAlmacen.Rows[intInd].FindControl("hdValidado")).Value = "0";
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código no corresponde al producto");
            return;
        }

        if(!strCodigos[0].Equals(txtProducto.Text) &&
           !strCodigos[1].Equals(txtProducto.Text) &&
           !strCodigos[2].Equals(txtProducto.Text))
        {
            ((HiddenField)gvProductosAlmacen.Rows[intInd].FindControl("hdValidado")).Value = "0";
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código no corresponde al producto");
            return;
        }

        ((HiddenField)gvProductosAlmacen.Rows[intInd].FindControl("hdValidado")).Value = "1";
        ((TextBox)((GridView)gvProductosAlmacen.Rows[intInd].FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Focus();
        ((TextBox)((GridView)gvProductosAlmacen.Rows[intInd].FindControl("gvProductosAlmacenLote")).Rows[0].FindControl("txtCantidadLote")).Attributes["onfocus"] = "this.select();";
    }

    protected void lnkValidar_Click(object sender, EventArgs e)
    {
        this.hdAccionVerif.Value = "1";
        this.lblVerificacionH.Text = "Verificación de Lotes";
        this.lblVerificacion.Text = "Ingrese el código de verificación para permitir el uso de lotes más recientes";
        this.mdVerificacion.Show();
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

        if (string.IsNullOrEmpty(this.txtLote_Verificacion.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtLote_Verificacion.Text.Equals(strCodigo))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código de validación no válido");
            return;
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

        switch (this.hdAccionVerif.Value)
        {
            case "1":
                this.hdValidar.Value = "0";
                CRutinas.Enviar_Correo("6",
                                       "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                                       "Código usado: " + strCodigo + "<br />" +
                                       "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                                       "Pedido: " + this.hdID.Value + "<br />" +
                                       "Cliente: " + this.txtSucursal.Text + "<br />" +
                                       "Razón: Código ingresado para permitir el uso de lotes más recientes" + "<br /><br />" +
                                       strTexto.ToString().Replace("display:none;", ""));
                break;
            case "2":
                Borrar_Producto(this.hdConsecutivoID.Value);
                CRutinas.Enviar_Correo("6",
                                       "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                                       "Código usado: " + strCodigo + "<br />" +
                                       "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                                       "Pedido: " + this.hdID.Value + "<br />" +
                                       "Cliente: " + this.txtSucursal.Text + "<br />" +
                                       "Razón: Se borró producto ya surtido total o parcialmente" + "<br /><br />" +
                                       strTexto.ToString().Replace("display:none;", ""));
                break;
        }
    }

    protected void btnPonerProceso_Click(object sender, EventArgs e)
    {
        if (!Validar_Consistencia())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("El pedido fue modificado por otra persona así que hay que volver a abrirlo");
            return;
        }

        Generar_Temporal();

        string strQuery = "UPDATE orden_compra SET" +
                         " estatus = 1" +
                         ",modificadoPorID = " + Session["SIANID"].ToString() +
                         ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlAlmacen.Visible = false;
        this.pnlListado.Visible = true;
        Llenar_Grid();
    }

    private void Generar_Temporal()
    {
        int intCantidad = 0;
        int intCantIngr = 0;
        bool swAgregado = false;

        string strQuery = "UPDATE orden_compra_productos" +
                         " SET temporal = 0 " +
                         " WHERE orden_compraID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (GridViewRow gvRow in this.gvProductosAlmacen.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                // Productos que ya se validaron y con valor, se marcan como temporal para guardar
                // lo que se ha capturado hasta ahora
                if (((HiddenField)gvRow.FindControl("hdValidado")).Value.Equals("1") &&
                    ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows.Count > 0)
                {
                    swAgregado = false;
                    intCantidad = int.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value);
                    foreach (GridViewRow gvRow2 in ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).Rows)
                    {
                        if (int.TryParse(((TextBox)gvRow2.FindControl("txtCantidadLote")).Text.Trim(), out intCantIngr))
                        {
                            if (intCantIngr > 0)
                            {
                                Agregar_Lote_Temporal(this.gvProductosAlmacen.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                      ((GridView)gvRow.FindControl("gvProductosAlmacenLote")).DataKeys[gvRow2.RowIndex].Value.ToString(),
                                                      intCantIngr);
                                swAgregado = true;
                            }
                        }
                    }
                    if (swAgregado)
                    {
                        string[] strID = this.gvProductosAlmacen.DataKeys[gvRow.RowIndex].Value.ToString().Split('_');
                        strQuery = "UPDATE orden_compra_productos" +
                                  " SET temporal = 1 " +
                                  " WHERE orden_compraID = " + this.hdID.Value +
                                  "   AND productoID = " + strID[0] +
                                  "   AND consecutivo = " + strID[1];
                        CComunDB.CCommun.Ejecutar_SP(strQuery);
                    }
                }
            }
        }
    }

    private DataTable ObtenerProductosLista()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("productoID", typeof(string)));
        dt.Columns.Add(new DataColumn("producto", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dt.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dt.Columns.Add(new DataColumn("productoChk", typeof(bool)));
        dt.Columns.Add(new DataColumn("enabled", typeof(bool)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));

        string strQuery = "SELECT P.ID as productoID " +
                         " ,P.codigo" +
                         " ,LEFT(P.nombre, 80) as producto " +
                         " ,S.precio_caja as costo " +
                         " ,IFNULL(D.existencia, 0) as existencia" +
                         " FROM precios S " +
                         " INNER JOIN productos P " +
                         " ON P.ID = S.producto_ID " +
                         " AND S.lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                         " AND S.validez = '2099-12-31' " +
                         " LEFT JOIN producto_datos D" +
                         " ON D.productoID = P.ID" +
                         " ORDER BY P.nombre ";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        DataSet objDataResult2;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString();
            dr[1] = objRowResult["producto"].ToString() +
                    "(" + ((decimal)objRowResult["existencia"]).ToString("0.##") + ")";
            dr[2] = objRowResult["codigo"].ToString();
            dr[3] = "0";
            dr[4] = ((decimal)objRowResult["costo"]).ToString("0.00");
            dr[5] = false;
            if (this.hdUsuPr.Value.Equals("0"))
                dr[6] = false;
            else
                dr[6] = true;

            strQuery = "SELECT MAX(F.fecha) as fecha" +
                      " FROM facturas_liq F" +
                      " INNER JOIN facturas_liq_prod P" +
                      " ON F.ID = P.factura_ID" +
                      " AND F.lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                      " AND P.producto_ID = " + objRowResult["productoID"].ToString();

            objDataResult2 = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (!objDataResult2.Tables[0].Rows[0].IsNull("fecha"))
                dr[7] = ((DateTime)objDataResult2.Tables[0].Rows[0]["fecha"]).ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("es-MX"));

            dt.Rows.Add(dr);
        }

        ViewState["listaTmp"] = dt;
        return dt;
    }

    protected void btnLista_Click(object sender, EventArgs e)
    {
        int intMaximo = 0;

        int.TryParse(CComunDB.CCommun.Obtener_Valor_CatParametros(5), out intMaximo);

        string strQuery = "SELECT COUNT(*) as cantidad " +
                         " FROM precios " +
                         " WHERE validez = '2099-12-31'" +
                         "   AND lista_precios_ID = " + this.dlListaPrecios.SelectedValue;

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if ((long)objDataResult.Tables[0].Rows[0]["cantidad"] > intMaximo)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Esta funcionalidad es únicamente para listas de precios con hasta " + intMaximo + " productos");
            return;
        }

        if ((long)objDataResult.Tables[0].Rows[0]["cantidad"] > 10)
        {
            this.pnlListaButones.BorderColor = System.Drawing.Color.DarkBlue;
            this.avcLista.VerticalSide = AjaxControlToolkit.VerticalSide.Top;
            this.avcLista.HorizontalSide = AjaxControlToolkit.HorizontalSide.Right;
            this.avcLista.Enabled = true;
        }
        else
        {
            this.pnlListaButones.BorderColor = System.Drawing.Color.White;
            this.avcLista.Enabled = false;
        }

        this.lblLista.Text = this.dlListaPrecios.SelectedItem.Text;

        this.gvLista.DataSource = ObtenerProductosLista();
        this.gvLista.DataBind();

        this.pnlListaPrecios.Visible = true;
        this.pnlListaPrecios2.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
    }

    protected void btnAgrProdLista_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdProductoID.Value))
        {
            if (!Buscar_Producto_Lista())
                ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un producto de la lista");
            return;
        }

        bool swExiste = false;
        foreach (GridViewRow gvRow in this.gvLista.Rows)
        {
            if (this.hdProductoID.Value.Equals(this.gvLista.DataKeys[gvRow.RowIndex].Value.ToString()))
            {
                swExiste = true;
                break;
            }
        }

        if (swExiste)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Producto ya existe en la lista");
            return;
        }

        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        if (int.TryParse(this.txtCantLista.Text.Trim(), out intCantidad) &&
            decimal.TryParse(this.txtPrecioLista.Text.Trim(), out dcmPrecioUnitario))
        {
            this.lblPrecioProducto.Text = this.txtProdLista.Text;
            if (this.lblPrecioProducto.Text.Length > 30)
                this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 30);
            this.hdAccion.Value = "3";
            if (Validar_Producto_Remision(this.hdProductoID.Value,
                                          this.hdSucursalID.Value,
                                          this.lblPrecioProducto.Text,
                                          this.txtCantLista.Text))
            {
                Continuar_Agregar_ProdLista();
            }
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad y precio unitario deben ser numéricos");

        this.txtCantLista.Focus();
    }

    private bool Buscar_Producto_Lista()
    {
        if (!string.IsNullOrEmpty(this.txtProdLista.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, CONCAT(nombre, ' - ', sales) as nombre" +
                             "    FROM productos P " +
                             "    WHERE (codigo = '" + this.txtProdLista.Text.Trim() + "'" +
                             "       OR codigo2 = '" + this.txtProdLista.Text.Trim() + "'" +
                             "       OR codigo3 = '" + this.txtProdLista.Text.Trim() + "'" +
							 "       )" +
							 "       AND activo = 1" +
                             " ) R" +
                             " LEFT JOIN producto_datos D" +
                             " ON D.productoID = R.ID";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult.Tables[0].Rows.Count == 1)
            {
                this.hdProductoID.Value = objDataResult.Tables[0].Rows[0][0].ToString();
                this.txtProdLista.Text = objDataResult.Tables[0].Rows[0][1].ToString() +
                                        "(" + ((decimal)objDataResult.Tables[0].Rows[0]["existencia"]).ToString("0.##") + ")";
                this.txtCantLista.Text = this.txtPrecioLista.Text = string.Empty;
                string strClientScript = "setTimeout('setProductoCantLista()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private void Continuar_Agregar_ProdLista()
    {
        string strMensaje = string.Empty;
        decimal dcmPrecioUnitario = Math.Round(decimal.Parse(this.txtPrecioLista.Text.Trim()), 2);

        DataTable dt = (DataTable)ViewState["listaTmp"];

        int i = 0;
        foreach (GridViewRow gvRow in this.gvLista.Rows)
        {
            dt.Rows[i][3] = ((TextBox)gvRow.FindControl("txtCantidadLista")).Text;
            dt.Rows[i][4] = ((TextBox)gvRow.FindControl("txtCostoLista")).Text;
            dt.Rows[i][5] = ((CheckBox)gvRow.FindControl("chkAct")).Checked;
            i++;
        }

        DataRow dr = dt.NewRow();
        dr[0] = this.hdProductoID.Value;
        dr[1] = this.txtProdLista.Text;
        dr[3] = this.txtCantLista.Text;
        dr[4] = this.txtPrecioLista.Text;
        dr[5] = true;
        if (this.hdUsuPr.Value.Equals("0"))
            dr[6] = false;
        else
            dr[6] = true;
        dt.Rows.Add(dr);

        this.gvLista.DataSource = dt;
        this.gvLista.DataBind();

        this.txtProdLista.Text = string.Empty;
        this.txtCantLista.Text = string.Empty;
        this.txtPrecioLista.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;
    }

    protected void btnProcesarLista_Click(object sender, EventArgs e)
    {
        int intCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        foreach (GridViewRow gvRow in this.gvLista.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (!int.TryParse(((TextBox)gvRow.FindControl("txtCantidadLista")).Text.Trim(), out intCantidad) ||
                    !decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim(), out dcmPrecioUnitario))
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser numérica: " + gvRow.Cells[0].Text);
                    return;
                }
                ((TextBox)gvRow.FindControl("txtCantidadLista")).Text = intCantidad.ToString();
                ((TextBox)gvRow.FindControl("txtCostoLista")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();
            }
        }

        StringBuilder strMensajeFinal = new StringBuilder();
        foreach (GridViewRow gvRow in this.gvLista.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (!((TextBox)gvRow.FindControl("txtCantidadLista")).Text.Equals("0"))
                {
                    string strMensaje = string.Empty;
                    Agregar_Producto(this.gvLista.DataKeys[gvRow.RowIndex].Value.ToString(),
                                    gvRow.Cells[0].Text,
                                    int.Parse(((TextBox)gvRow.FindControl("txtCantidadLista")).Text.Trim()),
                                    decimal.Parse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim()),
                                    this.dlListaPrecios.SelectedValue, "0", false,
                                    out strMensaje);
                    if (((CheckBox)gvRow.FindControl("chkAct")).Checked)
                    {
                        CComunDB.CCommun.Actualizar_Precio(this.gvLista.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                           this.dlListaPrecios.SelectedValue,
                                                           decimal.Parse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim()));
                    }
                }
                ((TextBox)gvRow.FindControl("txtCantidadLista")).Text = "0";
            }
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Productos fueron agregados");
    }

    protected void btnRegresarLista_Click(object sender, EventArgs e)
    {
        Llenar_Productos(true);

        this.txtProdLista.Text = string.Empty;
        this.txtCantLista.Text = string.Empty;
        this.txtPrecioLista.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        this.pnlListaPrecios.Visible = false;
        this.pnlListaPrecios2.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    private void Obtener_CreadoPor()
    {
        this.lblCreado.Text = string.Empty;
        string strQuery = "SELECT U.usuario, F.creadoPorFecha" +
                         " FROM orden_compra F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.creadoPorID" +
                         " AND F.ID = " + this.hdID.Value + ";" +
                         "SELECT U.usuario, F.modificadoPorFecha" +
                         " FROM orden_compra F" +
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
