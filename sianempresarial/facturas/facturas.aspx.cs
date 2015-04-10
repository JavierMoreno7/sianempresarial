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

public partial class facturas_facturas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.txtProdLista.Attributes["onfocus"] = "javascript:limpiarProdListaID();";
        this.imgInfo.Attributes["onmouseout"] = "javascript:esconder();";
        this.imgInfo2.Attributes["onmouseout"] = "javascript:esconder2();";
        this.txtFecha.Attributes["readonly"] = "true";
        this.txtFechaOrden.Attributes["readonly"] = "true";
        this.txtPago.Attributes["readonly"] = "true";
        this.txtFechaPago.Attributes["readonly"] = "true";

        this.txtPag.Attributes["onkeypress"] =
            this.txtOrden_CompraID.Attributes["onkeypress"] = "javascript:return isNumber(event, this);";

        this.txtCambiarCantidad.Attributes["onkeyup"] = "javascript:return validateCambiar(this);";
        this.txtCambiarCantidad.Attributes["onblur"] = "javascript:return validateCambiar(this);";

        this.txtDescuento1.Attributes["onkeypress"] =
            this.txtDescuento2.Attributes["onkeypress"] =
            this.txtCantidad.Attributes["onkeypress"] =
            this.txtCambiarCantidad.Attributes["onkeypress"] =
            this.txtPrecioUnitarioCambio.Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
        
        this.txtPrecioUnitario.Attributes["onkeypress"] =
            this.txtCambiarUnitario.Attributes["onkeypress"] = "javascript:return isNumberDecNeg(event, this, 2);";

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
		
        this.AutoCompleteExtender2.ContextKey =
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
                this.txtProdLista.Enabled = false;
                this.txtCantLista.Enabled = false;
                this.txtPrecioLista.Enabled = false;
                this.btnAgrProdLista.Visible = false;
            }

            if (!CComunDB.CCommun.ValidarAcceso(6100, out swVer, out swTot))
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
            this.txtFechaOrden.Text = this.txtFecha.Text;

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

        this.dlEstatus.DataSource = CComunDB.CCommun.ObtenerFacturaEstatus(false);
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
        {
            this.btnOrden_Compra.Enabled = false;
            this.btnModificar.Enabled = false;
        }
        else
        {
            this.btnOrden_Compra.Enabled = true;
            this.btnModificar.Enabled = true;
        }

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
        DataTable dtResultado = ObtenerFacturas();

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

    private DataTable ObtenerFacturas()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("facturaID", typeof(string)));
        dt.Columns.Add(new DataColumn("negocio", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha_cancelacion", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("estatusID", typeof(string)));
        dt.Columns.Add(new DataColumn("factura", typeof(string)));
        dt.Columns.Add(new DataColumn("folio", typeof(string)));

        string strQuery = "CALL leer_facturas_consulta(" +
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
            dr[0] = objRowResult["facturaID"].ToString();
            dr[1] = objRowResult["negocio"].ToString();
            if ((DateTime)objRowResult["fecha"] == DateTime.Parse("1901-01-01"))
                dr[2] = string.Empty;
            else
                dr[2] = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy HH:mm");
            if(objRowResult.IsNull("fecha_cancelacion"))
                dr[3] = string.Empty;
            else
                dr[3] = ((DateTime)objRowResult["fecha_cancelacion"]).ToString("dd/MM/yyyy");

            dr[4] = objRowResult["estatus"].ToString();
            dr[5] = ((decimal)objRowResult["monto"]).ToString("c") + " " + objRowResult["moneda"].ToString();
            dr[6] = objRowResult["estatusID"].ToString();
            dr[7] = objRowResult["factura"].ToString();
            dr[8] = objRowResult["folio"].ToString();
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
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("clave", typeof(string)));
        dt.Columns.Add(new DataColumn("grupoID", typeof(string)));
        dt.Columns.Add(new DataColumn("grupo_consecutivo", typeof(string)));

        List<string> lstProductosFE = new List<string>();

        if (!string.IsNullOrEmpty(this.lblFactura.Text))
        {
            if (File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                          this.lblFactura.Text + ".xml")))
            {
                XmlDocument xmlFactura = new XmlDocument();
                xmlFactura.Load(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                               this.lblFactura.Text + ".xml"));
                XmlNodeList xmlProductos = xmlFactura.GetElementsByTagName("cfdi:Concepto");
                for (int i = 0; i < xmlProductos.Count; i++)
                {
                    lstProductosFE.Add(xmlProductos[i].Attributes["descripcion"].Value);
                }
            }
        }

        objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT detalle_incluido" +
                                                    " FROM facturas_liq" +
                                                    " WHERE ID =" + this.hdID.Value);

        bool swDetalleIncluido = (bool)objDataResult.Tables[0].Rows[0]["detalle_incluido"];

        string strQuery = "SELECT * FROM (" +
                        "SELECT C.producto_ID as productoID " +
                        ", C.cantidad as cantidad " +
                        ", C.consecutivo as consecutivo " +
                        ", C.costo_unitario as costo_unitario " +
                        ", C.costo as costo " +
                        ", C.costo_original " +
                        ", C.costo_original_moneda " +
                        ", C.usar_unimed2" +
                        ", CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
                        ", C.exento as exento " +
                        ", C.detalle as detalle " +
                        ", C.grupoID " +
                        ", C.grupo_cantidad " +
                        ", C.grupo_consecutivo" +
                        ", P.usar_detalle" +
                        ", P.unimed" +
                        ", P.unimed2" +
                        ", P.relacion1" +
                        ", P.relacion2" +
                        ", P.unimed_original" +
                        ", P.codigo as codigo " +
                        ", S.clave" +
                        ", CONCAT('(', G.codigo, ') ', LEFT(G.nombre, 80)) as producto_grupo " +
                        " FROM facturas_liq_prod C " +
                        " INNER JOIN productos P " +
                        " ON C.producto_ID = P.ID " +
                        " AND factura_ID = " + this.hdID.Value +
                        " LEFT JOIN productos G" +
                        " ON G.ID = C.grupoID" +
                        " LEFT JOIN establecimiento_producto S" +
                        " ON S.productoID = C.producto_ID" +
                        " AND S.establecimientoID = (" +
                        "               SELECT establecimiento_ID" +
                        "               FROM sucursales" +
                        "               WHERE ID = " + this.hdSucursalID.Value +
                        "              )" +
                        " WHERE factura_ID = " + this.hdID.Value +
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
        decimal dcmCantidadAlterno;
        decimal dcmCantidad;
        this.hdContieneKits.Value = "0";
        string strGrupoConsecutivo = string.Empty;

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
            {
                // Se agrega un registro en blanco con los datos del Kit
                if (!strGrupoConsecutivo.Equals(objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString()))
                {
                    this.hdContieneKits.Value = "1";
                    dr = dt.NewRow();
                    dr[0] = objRowResult["productoID"].ToString() + "_" +
                            objRowResult["consecutivo"].ToString();
                    dr[1] = objRowResult["producto_grupo"].ToString() + ", Cant.: " + ((decimal)objRowResult["grupo_cantidad"]).ToString("0.##");
                    dr[12] = objRowResult["grupoID"].ToString();
                    dr[13] = objRowResult["grupo_consecutivo"].ToString();
                    dt.Rows.Add(dr);
                    strGrupoConsecutivo = objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString();
                }
            }

            dr = dt.NewRow();
            dr[0] = objRowResult["productoID"].ToString() + "_" +
                    objRowResult["consecutivo"].ToString();

            if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
            {
                dr[1] = "&nbsp;&nbsp;&nbsp;";
            }

            if ((bool)objRowResult["usar_detalle"])
            {
                if (objRowResult["detalle"].ToString().Length < 50)
                    dr[1] += objRowResult["detalle"].ToString();
                else
                    dr[1] += objRowResult["detalle"].ToString().Substring(0, 50);
            }
            else
            {
                dcmCantidad = (decimal)objRowResult["cantidad"];
                if (lstProductosFE.Count > intId)
                    dr[1] = lstProductosFE[intId];
                else
                    dr[1] = objRowResult["producto"].ToString();

                if (string.IsNullOrEmpty(objRowResult["unimed2"].ToString()))
                    dr[1] += " (" + objRowResult["unimed"].ToString() + ")";
                else
                {
                    if ((bool)objRowResult["unimed_original"])
                    {
                        if ((decimal)objRowResult["relacion1"] == 1)
                            dcmCantidadAlterno = Math.Round(dcmCantidad / (decimal)objRowResult["relacion2"], 2);
                        else
                            dcmCantidadAlterno = Math.Round(dcmCantidad * (decimal)objRowResult["relacion1"], 2);

                        dr[1] += " (" +
                            objRowResult["unimed"].ToString() + ")" +
                            "<br/>" +
                            "(Equivalente a " +
                            dcmCantidadAlterno.ToString("0.##") + " " +
                            objRowResult["unimed2"].ToString() + ")";
                    }
                    else
                    {
                        if ((bool)objRowResult["usar_unimed2"])
                        {
                            if ((decimal)objRowResult["relacion1"] == 1)
                                dcmCantidadAlterno = Math.Round(dcmCantidad * (decimal)objRowResult["relacion2"], 2);
                            else
                                dcmCantidadAlterno = Math.Round(dcmCantidad / (decimal)objRowResult["relacion1"], 2);

                            dr[1] += " (" +
                                objRowResult["unimed2"].ToString() + ")" +
                                "<br/>" +
                                "(Equivalente a " +
                                dcmCantidadAlterno.ToString("0.##") + " " +
                                objRowResult["unimed"].ToString() + ")";
                        }
                        else
                        {
                            if ((decimal)objRowResult["relacion1"] == 1)
                                dcmCantidadAlterno = Math.Round(dcmCantidad / (decimal)objRowResult["relacion2"], 2);
                            else
                                dcmCantidadAlterno = Math.Round(dcmCantidad * (decimal)objRowResult["relacion1"], 2);

                            dr[1] += " (" +
                                objRowResult["unimed"].ToString() + ")" +
                                "<br/>" +
                                "(Equivalente a " +
                                dcmCantidadAlterno.ToString("0.##") + " " +
                                objRowResult["unimed2"].ToString() + ")";
                        }
                    }
                }
            }

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
            if (!(bool)objRowResult["usar_detalle"] && objRowResult["detalle"].ToString().Length > 0)
            {
                if (string.IsNullOrEmpty(this.lblFactura.Text) || !swDetalleIncluido)
                {
                    if (!objRowResult.IsNull("detalle") && objRowResult["detalle"].ToString().Length > 0)
                    {
                        if (objRowResult["detalle"].ToString().Length < 50)
                            dr[1] += "<br/>" + objRowResult["detalle"].ToString();
                        else
                            dr[1] += "<br/>" + objRowResult["detalle"].ToString().Substring(0, 50);
                    }
                }

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
            dr[10] = objRowResult["codigo"].ToString();
            dr[11] = objRowResult["clave"].ToString();
            dr[12] = objRowResult["grupoID"].ToString();
            dr[13] = objRowResult["grupo_consecutivo"].ToString();
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
                this.btnFE.Visible = false;
                this.btnSustituir.Visible = false;
                this.btnOrden_Compra.Visible = false;
                this.btnRemision.Visible = false;
            }
        }
        else
            if (e.CommandName == "Pagos")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                Mostrar_Pagos(this.grdvLista.DataKeys[index].Value.ToString(), ((LinkButton)this.grdvLista.Rows[index].Cells[0].Controls[0]).Text);
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

        DataSet objDataResult = new DataSet();

        if (!this.hdID.Value.Equals("0"))
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT count(*) AS productos" +
                                                        " FROM" +
                                                        " (" +
                                                        "    SELECT DISTINCT(producto_ID) as productoID" +
                                                        "    FROM facturas_liq_prod" +
                                                        "    WHERE factura_ID = " + this.hdID.Value +
                                                        " ) AS A" +
                                                        " LEFT JOIN productos P" +
                                                        " ON P.ID = A.productoID" +
                                                        " WHERE P.ID is null");

            if (int.Parse(objDataResult.Tables[0].Rows[0]["productos"].ToString()) > 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Hay un error con la factura ya que tiene productos que no están en el catálogo. Contacte al administrador");
                return;
            }
        }

        this.hdCosto.Value = this.hdCostoIVA.Value = this.hdCostoDescuento.Value = this.hdIVA.Value =
            this.hdSubtotal.Value = this.hdISRRet.Value = this.hdIVARet.Value = this.hdTotal.Value = "0";

        this.txtSerie.Text = string.Empty;
        this.txtDetalle.Text = string.Empty;
        this.lnkSAT.Visible = false;
        this.lblMensaje.Text = string.Empty;
        this.lblCreado.Text = string.Empty;
        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtCveCliente.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.lblMensaje.Text = string.Empty;
        this.lblFacturaID.Text = string.Empty;
        this.lblCorreoEnvio.Text = string.Empty;
        this.lnkPagos.Text = "$0.00";
        this.btnSustituir.Visible = false;
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
            this.txtFechaOrden.Text = this.txtFecha.Text;
            this.txtOrdenCompra.Text = string.Empty;
            this.txtSucursal.Text = string.Empty;
            this.txtComentarios.Text = string.Empty;
            this.txtRefCliente.Text = string.Empty;
            this.txtDescuento1.Text = "0";
            this.txtDescuento2.Text = "0";
            this.txtProducto.Enabled = false;
            this.txtCantidad.Enabled = false;
            this.txtCveCliente.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.lblFactura.Text = string.Empty;
            this.lblFecha_Timbrado.Text = string.Empty;
            this.btnOrden_Compra.Visible = false;
            this.btnRemision.Visible = false;
            this.btnLista.Visible = false;
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
            string strQuery = "SELECT S.establecimiento_ID, sucursal_ID, concat(negocio, ' - ', sucursal) as nombre, " +
                    " folio, factura_suf, factura, fecha_timbrado, fecha, F.status, F.contado, comentarios, " +
                    " F.descuento, F.descuento2, F.iva, F.lista_precios_ID, F.vendedorID, " +
                    " F.monto_iva_ret, F.monto_iva, F.tax_manual, F.monto_isr_ret, F.isr_ret, F.iva_ret," +
                    " C.razon as motivo_cancelacion, F.orden_compra, F.referencia " +
                    ",F.moneda, F.tipo_cambio" +
                    " FROM facturas_liq F " +
                    " INNER JOIN sucursales S " +
                    " ON F.sucursal_ID = S.ID " +
                    " INNER JOIN establecimientos E " +
                    " ON S.establecimiento_ID = E.ID " +
                    " LEFT JOIN facturas_cancelacion C " +
                    " ON F.ID = C.factura_ID " +
                    " WHERE F.ID = " + this.hdID.Value +
                    ";" +
                    " SELECT orden_compraID" +
                    " FROM orden_compra_factura" +
                    " WHERE facturaID = " + this.hdID.Value +
                    " ORDER BY orden_compraID" +
                    ";" +
                    " SELECT fecha_envio" +
                    " FROM correo_envio" +
                    " WHERE ID = " + this.hdID.Value +
                    "   AND tipo = 0" +
                    ";" +                                                       // Tabla 3
                    "SELECT nota_ID" +
                    " FROM nota_facturas" +
                    " WHERE factura_ID = " + this.hdID.Value +
                    " ORDER BY nota_ID" +
                    ";" +                                                       // Tabla 4
                    "SELECT F.folio" +
                    " FROM facturas_reemplazo R" +
                    " INNER JOIN facturas_liq F" +
                    " ON F.ID = R.factura_IDAnt" +
                    " AND R.factura_ID = " + this.hdID.Value;
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
                ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al leer la factura: " + this.hdID.Value);
                return;
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            if (!objRowResult["status"].ToString().Equals("8"))
                Estado_Campos(false);
            else
                Estado_Campos(true);

            this.txtOrdenCompra.Text = objRowResult["orden_compra"].ToString();
            this.lblFacturaID.Text = objRowResult["folio"].ToString();
            this.txtSerie.Text = objRowResult["factura_suf"].ToString();
            this.lblFactura.Text = objRowResult["factura"].ToString();
            if (!objRowResult.IsNull("fecha_timbrado"))
                this.lblFecha_Timbrado.Text = CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objRowResult["fecha_timbrado"]);
            else
                this.lblFecha_Timbrado.Text = string.Empty;

            this.txtFecha.Text = ((DateTime)objRowResult["fecha"]).ToString("dd/MM/yyyy");

            if ((int)objRowResult["establecimiento_ID"] != 0)
            {
                this.txtSucursal.Text = objRowResult["nombre"].ToString();
                this.hdSucursalID.Value = objRowResult["sucursal_ID"].ToString();
            }
            else
            {
                this.txtSucursal.Text = string.Empty;
                this.hdSucursalID.Value = string.Empty;
            }

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

            this.txtRefCliente.Text = objRowResult["referencia"].ToString();

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

            StringBuilder strMensaje = new StringBuilder();

            if (!objRowResult["status"].ToString().Equals("8"))
            {
                if (objRowResult["status"].ToString().Equals("9"))
                {
                    this.lnkSAT.Visible = true;
                    this.hdBorrar.Value = "0";
                    this.btnOrden_Compra.Visible = false;
                    this.btnRemision.Visible = false;
                    this.btnLista.Visible = false;
                    this.txtProducto.Enabled = false;
                    this.txtCantidad.Enabled = false;
                    this.txtCveCliente.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnFE.Visible = false;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = false;
                    this.btnAgregarProd.Visible = false;
                    this.btnEmail.Visible = false;
                    strMensaje.Append("FACTURA CANCELADA: " + objRowResult["motivo_cancelacion"].ToString());
                }
                else
                {
                    this.hdBorrar.Value = "0";
                    this.btnOrden_Compra.Visible = false;
                    this.btnRemision.Visible = false;
                    this.btnLista.Visible = false;
                    this.txtProducto.Enabled = false;
                    this.txtCantidad.Enabled = false;
                    this.txtCveCliente.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnFE.Visible = false;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = true;
                    this.btnAgregarProd.Visible = false;
                    this.btnSustituir.Visible = true;
                    this.btnEmail.Visible = true;
                }
            }
            else
            {
                this.hdBorrar.Value = "1";
                if (this.hdInvAlmacen.Value.Equals("1"))
                    this.btnOrden_Compra.Visible = true;
                else
                    this.btnOrden_Compra.Visible = false;

                this.btnRemision.Visible = true;
                this.btnLista.Visible = true;
                this.txtProducto.Enabled = true;
                this.txtCantidad.Enabled = true;
                this.txtCveCliente.Enabled = true;
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
                    this.btnOrden_Compra.Visible = false;
                    this.btnRemision.Visible = false;
                    this.btnLista.Visible = false;
                    this.txtProducto.Enabled = false;
                    this.btnRefrescarTipoCambio.Visible = false;
                    this.txtCantidad.Enabled = false;
                    this.txtCveCliente.Enabled = false;
                    this.txtPrecioUnitario.Enabled = false;
                    this.btnModificar.Visible = false;
                    this.btnXML.Visible = true;
                    this.btnImprimir.Visible = true;
                    this.btnCancelar.Visible = true;
                    this.btnAgregarProd.Visible = false;
                    this.btnSustituir.Visible = true;
                }
            }

            if (objDataResult.Tables[2].Rows.Count > 0)
                this.lblCorreoEnvio.Text = "Correo enviado: " + CRutinas.Fecha_DD_MMM_YYYY_Hora((DateTime)objDataResult.Tables[2].Rows[0]["fecha_envio"]);

            if (objDataResult.Tables[1].Rows.Count > 0)
            {
                StringBuilder strOrdenes = new StringBuilder("Pedido(s): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[1].Rows)
                {
                    if (strOrdenes.Length != 11)
                        strOrdenes.Append(", ");
                    strOrdenes.Append(objRowResult2[0].ToString());
                }
                if (strMensaje.Length > 0)
                    strMensaje.Append(", ");
                strMensaje.Append(strOrdenes.ToString());
            }

            if (objDataResult.Tables[3].Rows.Count > 0)
            {
                StringBuilder strOrdenes = new StringBuilder("Remisiones(s): ");
                foreach (DataRow objRowResult2 in objDataResult.Tables[3].Rows)
                {
                    if (strOrdenes.Length != 15)
                        strOrdenes.Append(", ");
                    strOrdenes.Append(objRowResult2[0].ToString());
                }
                if (strMensaje.Length > 0)
                    strMensaje.Append(", ");
                strMensaje.Append(strOrdenes.ToString());
            }

            if (objDataResult.Tables[4].Rows.Count > 0)
            {
                if (strMensaje.Length > 0)
                    strMensaje.Append(", ");
                strMensaje.Append("FACTURA SUSTITUYE A LA FACTURA: " + objDataResult.Tables[4].Rows[0]["folio"].ToString());
            }

            this.lblMensaje.Text = strMensaje.ToString();

            strQuery = "SELECT SUM(F.monto_pago) AS monto_pago" +
                       " FROM pago_facturas F" +
                       " WHERE F.facturaID = " + this.hdID.Value +
                       " AND F.aplicado = 1";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if(!objDataResult.Tables[0].Rows[0].IsNull("monto_pago"))
                this.lnkPagos.Text = ((decimal)objDataResult.Tables[0].Rows[0]["monto_pago"]).ToString("c");

            Llenar_Productos(false);
            Obtener_Notas();
        }

        Obtener_Moneda();
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
        this.txtOrdenCompra.Enabled = sw_estado;
        this.chkIVARet.Enabled = sw_estado;
        this.chkISRRet.Enabled = sw_estado;
        this.txtRefCliente.Enabled = sw_estado;
        this.dlMoneda.Enabled = sw_estado;
        this.txtSerie.Enabled = sw_estado;
    }

    private void Agregar_Factura()
    {
        if (string.IsNullOrEmpty(this.hdSucursalID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return;
        }

        if (Crear_Factura(int.Parse(this.hdSucursalID.Value),
                          DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")),
                          decimal.Parse(this.rdIVA.SelectedValue),
                          Convert.ToBoolean(this.rdTipo.SelectedValue),
                          decimal.Parse(this.txtDescuento1.Text),
                          decimal.Parse(this.txtDescuento2.Text),
                          this.txtComentarios.Text.Trim(),
                          int.Parse(this.dlListaPrecios.SelectedValue),
                          int.Parse(this.dlVendedor.SelectedValue),
                          this.txtOrdenCompra.Text.Trim(),
                          this.txtRefCliente.Text.Trim(),
                          this.dlMoneda.SelectedValue,
                          decimal.Parse(this.txtTipoCambio.Text)))
        {
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT nota, nota_suf " +
                            " FROM notas " +
                            " where sucursal_ID in (" +
                            " select ID from sucursales where establecimiento_ID =" +
                            " (" +
                            " select ID from establecimientos" +
                            " where ID = " +
                            " (select establecimiento_ID from sucursales" +
                            " where ID = " + this.hdSucursalID.Value + ")" +
                            " )" +
                            " )" +
                            " and status = 0" +
                            " AND appID = " + Session["SIANAppID"] +
                            " ORDER BY nota, nota_suf";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {
            }

            StringBuilder strNotas = new StringBuilder();
            foreach (DataRow objRowResult2 in objDataResult.Tables[0].Rows)
            {
                if (strNotas.Length > 0)
                    strNotas.Append(", ");
                strNotas.Append(objRowResult2["nota"].ToString());
                if (!string.IsNullOrEmpty(objRowResult2["nota_suf"].ToString()))
                    strNotas.Append("-" + objRowResult2["nota_suf"].ToString());
            }

            if(strNotas.Length == 0)
                ((master_MasterPage)Page.Master).MostrarMensajeError("La factura ha sido creada, folio interno: " + this.lblFacturaID.Text + this.txtSerie.Text);
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("La factura ha sido creada, folio interno: " + this.lblFacturaID.Text + this.txtSerie.Text +
                                            "<br/><br/><b>El cliente tiene las siguientes remisiones sin facturar: <br/>" + strNotas.ToString() + "</b>");
            this.txtProducto.Enabled = true;
            this.txtCantidad.Enabled = true;
            this.txtCveCliente.Enabled = true;
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
            this.btnLista.Visible = true;
            this.dlMoneda.Enabled = true;
            this.btnRefrescarTipoCambio.Visible = true;
            if (this.hdInvAlmacen.Value.Equals("1"))
                this.btnOrden_Compra.Visible = true;
            else
                this.btnOrden_Compra.Visible = false;
            this.btnRemision.Visible = true;
            this.btnTAX.Visible = false;
            Obtener_CreadoPor();

            this.pnlDatos2.Visible = true;
            this.btnModificar.ImageUrl = "~/imagenes/modificar.png";
        }
    }

    private bool Crear_Factura(int intSucursalID, DateTime dtFecha,
                               decimal dcmIVA, bool esContado,
                               decimal dcmDescuento1, decimal dcmDescuento2,
                               string strComentarios, int intListaPreciosID,
                               int intVendedorID, string strOrden_Compra, string strReferencia,
                               string strMoneda, decimal dcmTipo_Cambio)
    {
        DataSet objDataResult = new DataSet();
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        CFacturaDB objFactura = new CFacturaDB();

        objFactura.electronica = true;
        objFactura.fecha = dtFecha;
        objFactura.sucursal_ID = intSucursalID;
        objFactura.iva = dcmIVA;
        objFactura.contado = esContado;
        objFactura.descuento = dcmDescuento1;
        objFactura.descuento2 = dcmDescuento2;
        objFactura.comentarios = strComentarios;
        objFactura.status = 8;
        objFactura.lista_precios_ID = intListaPreciosID;
        objFactura.vendedorID = intVendedorID;
        objFactura.orden_compra = strOrden_Compra;
        objFactura.referencia = strReferencia;
        objFactura.isr_ret = (this.chkISRRet.Checked ? decimal.Parse(this.hdPorcISRRet.Value) : 0);
        objFactura.iva_ret = (this.chkIVARet.Checked ? decimal.Parse(this.hdPorcIVARet.Value) : 0);
        objFactura.tax_manual = (this.hdTAX.Value.Equals("1") ? true : false);
        objFactura.moneda = strMoneda;
        objFactura.tipo_cambio = dcmTipo_Cambio;
        objFactura.factura_suf = this.txtSerie.Text;

        objFactura.fecha_contrarecibo = null;
        objFactura.fecha_cobranza = null;
        if (!objFactura.contado)
        {
            // Busca fecha de contrarecibo
            DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza_SucursalID(1, intSucursalID);
            if (dtFechaCobranza.HasValue)
                objFactura.fecha_contrarecibo = dtFechaCobranza.Value;
            else
            {
                // Busca fecha de cobro
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza_SucursalID(2, intSucursalID);
                if (dtFechaCobranza.HasValue)
                    objFactura.fecha_cobranza = dtFechaCobranza.Value;
                else
                    objFactura.fecha_cobranza = DateTime.Today;
            }
        }

        if (objFactura.Guardar())
        {
            this.hdID.Value = objFactura.ID.ToString();
            this.lblFacturaID.Text = objFactura.folio.ToString();
            this.txtSerie.Text = objFactura.factura_suf;
            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
            return false;
        }
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            //if (this.hdInvAlmacen.Value.Equals("1"))
            //{
            //    this.hdAccion.Value = "2";
            //    this.lblVerificacion.Text = "Está creando una factura sin usar un pedido, ingrese el código de verificación para continuar";
            //    this.txtCodigo_Verificacion.Text = string.Empty;
            //    this.mdVerificacion.Show();
            //}
            //else
                Agregar_Factura();
        }
        else
            if (Actualizar_Factura(8))
                ((master_MasterPage)Page.Master).MostrarMensajeError("La factura ha sido modificada");
    }

    protected void btnVerificacionContinuar_Click(object sender, EventArgs e)
    {
        string strEtiqueta;
        string strCodigo = string.Empty;
        if (CacheManager.ObtenerValor("CV", out strEtiqueta))
        {
            string[] strValores = strEtiqueta.Split('_');
            if(DateTime.Parse(strValores[1]) >= DateTime.Now)
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
            case "1": 
                Agregar_Factura();
                break;
            case "2":
                Mostrar_Orden_Compra();
                break;
        }
    }

    private bool Actualizar_Factura(byte btStatus)
    {
        if (string.IsNullOrEmpty(this.hdSucursalID.Value))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un cliente de la lista");
            return false;
        }

        DateTime dtFecha = DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX"));
        TimeSpan hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        dtFecha = dtFecha.Add(hora);

        decimal dcmDescuento1, dcmDescuento2;
        decimal.TryParse(this.txtDescuento1.Text.Trim(), out dcmDescuento1);
        decimal.TryParse(this.txtDescuento2.Text.Trim(), out dcmDescuento2);

        CFacturaDB objFactura = new CFacturaDB();

        objFactura.Leer(int.Parse(this.hdID.Value));

        this.txtDescuento1.Text = dcmDescuento1.ToString("0.00");
        this.txtDescuento2.Text = dcmDescuento2.ToString("0.00");

        objFactura.fecha = dtFecha;
        objFactura.sucursal_ID = int.Parse(this.hdSucursalID.Value);
        objFactura.iva = decimal.Parse(this.rdIVA.SelectedValue);
        objFactura.contado = Convert.ToBoolean(this.rdTipo.SelectedValue);
        objFactura.descuento = dcmDescuento1;
        objFactura.descuento2 = dcmDescuento2;
        objFactura.comentarios = this.txtComentarios.Text.Trim();
        objFactura.status = btStatus;
        objFactura.lista_precios_ID = int.Parse(this.dlListaPrecios.SelectedValue);
        objFactura.vendedorID = int.Parse(this.dlVendedor.SelectedValue);
        objFactura.orden_compra = this.txtOrdenCompra.Text.Trim();
        objFactura.referencia = this.txtRefCliente.Text.Trim();
        objFactura.tax_manual = (this.hdTAX.Value.Equals("1") ? true : false);
        objFactura.moneda = this.dlMoneda.SelectedValue;
        objFactura.tipo_cambio = decimal.Parse(this.txtTipoCambio.Text);
        objFactura.factura_suf = this.txtSerie.Text;

        if (!objFactura.contado)
        {
            // Busca fecha de contrarecibo
            DateTime? dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(1, objFactura.ID);
            if (dtFechaCobranza.HasValue)
                objFactura.fecha_contrarecibo = dtFechaCobranza.Value;
            else
            {
                // Busca fecha de cobro
                dtFechaCobranza = CComunDB.CCommun.Obtener_FechaCobranza(2, objFactura.ID);
                if (dtFechaCobranza.HasValue)
                    objFactura.fecha_cobranza = dtFechaCobranza.Value;
                else
                    objFactura.fecha_cobranza = DateTime.Today;
            }
        }
        else
        {
            objFactura.fecha_contrarecibo = null;
            objFactura.fecha_cobranza = null;
        }

        if (objFactura.Guardar())
        {
            Obtener_CreadoPor();
            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
            return false;
        }
    }

    protected void btnRegresar_Click(object sender, ImageClickEventArgs e)
    {
        Regresar_Continuar();
    }

    private void Regresar_Continuar()
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
            dcmPrecioUnitario = Math.Round(dcmPrecioUnitario, 2);
			this.txtCantidad.Text = dcmCantidad.ToString();
            if (dcmCantidad <= 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad no puede ser cero");
                return;
            }

            if (dcmPrecioUnitario == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Precio no puede ser menor");
                return;
            }

            if (dcmPrecioUnitario < 0 && !this.hdProductoTipo.Value.Equals("9"))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Precio sólo puede ser negativo para conceptos, no para productos");
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

            string strMensaje = CComunDB.CCommun.Validar_Ultimo_Precio_Compra(int.Parse(this.hdProductoID.Value), dcmPrecioUnitario, this.dlMoneda.SelectedValue, decimal.Parse(this.txtTipoCambio.Text));
            if (!string.IsNullOrEmpty(strMensaje))
            {
                this.hdAccion.Value = "1";
                this.lblProdVenta.Text = strMensaje;
                this.mdProdVenta.Show();
                return;
            }

            if(Convert.ToBoolean(this.hdUsarDetalle.Value) &&
                string.IsNullOrEmpty(this.txtDetalle.Text.Trim()))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("El detalle es requerido para este producto");
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

        switch (this.hdAccion.Value)
        {
            case "1":
                Continuar_Agregar_Producto();
                break;
            case "2":
                Continuar_Cambiar_Producto();
                break;
            case "3":
                Continuar_Agregar_ProdLista();
                break;
            case "4":
                Mostrar_Orden_Compra();
                break;
        }

        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private void Continuar_Agregar_Producto()
    {
        this.hdTAX.Value = "0";
        decimal dcmCantidad = decimal.Parse(this.txtCantidad.Text.Trim());
        decimal dcmPrecioUnitario = decimal.Parse(this.txtPrecioUnitario.Text.Trim());
        string strMensaje = string.Empty;

        this.lblPrecioProducto.Text = this.txtProducto.Text;
        if (this.lblPrecioProducto.Text.Length > 25)
            this.lblPrecioProducto.Text = this.lblPrecioProducto.Text.Substring(0, 25);
        this.hdProductoPrecioID.Value = this.hdProductoID.Value;

        if (!Validar_Lote())
            return;

        bool swAgregado = false;

        if (this.hdProductoTipo.Value.Equals("2"))      // Es un kit, así que agrega todos los productos que lo componen
        {
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT COUNT(*) as cantidad" +
                                                                " FROM producto_materiaprima" +
                                                                " WHERE productoID = " + this.hdProductoID.Value +
                                                                ";" +

                                                                " SELECT P.producto_ID, M.cantidad, P.precio_caja as precio" +
                                                                " FROM precios P" +
                                                                " INNER JOIN  producto_materiaprima M" +
                                                                " ON M.materiaprimaID = producto_ID" +
                                                                " AND M.productoID = " + this.hdProductoID.Value +
                                                                " AND P.validez = '2099-12-31'" +
                                                                " AND P.lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                                                                ";" +

                                                                "SELECT IFNULL(MAX(grupo_consecutivo) + 1, 1) as consecutivo " +
                                                                " FROM facturas_liq_prod " +
                                                                " WHERE factura_ID = " + this.hdID.Value);

            if (int.Parse(objDataResult.Tables[0].Rows[0]["cantidad"].ToString()) == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Este es un Kit que no tiene productos en su Ficha Técnica, por lo que no se puede agregar");
                return;
            }

            if (int.Parse(objDataResult.Tables[0].Rows[0]["cantidad"].ToString()) != objDataResult.Tables[1].Rows.Count)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Hay productos del Kit que no tienen precio en la lista de precios seleccionada, por lo que no se puede agregar");
                return;
            }

            byte btConsecutivo = byte.Parse(objDataResult.Tables[2].Rows[0]["consecutivo"].ToString());
            foreach (DataRow objRow in objDataResult.Tables[1].Rows)
            {
                Agregar_Producto(objRow["producto_ID"].ToString(),
                                 string.Empty,
                                 Math.Round(dcmCantidad * (decimal)objRow["cantidad"], 2),
                                 (decimal)objRow["precio"],
                                 true,
                                 this.txtDetalle.Text.Trim(),
                                 this.chkImpDetalle.Checked,
                                 string.Empty,
                                 "0",
                                 this.hdProductoID.Value,
                                 btConsecutivo,
                                 dcmCantidad,
                                 (decimal)objRow["cantidad"],
                                 out strMensaje);
            }
            swAgregado = true;
        }
        else
        {
            swAgregado = Agregar_Producto(this.hdProductoID.Value,
                                          this.txtProducto.Text,
                                          dcmCantidad,
                                          Math.Round(dcmPrecioUnitario, 2),
                                          true, 
                                          this.txtDetalle.Text.Trim(),
                                          this.chkImpDetalle.Checked,
                                          string.Empty,
                                          "0",
                                          string.Empty,
                                          0,
                                          0,
                                          0,
                                          out strMensaje);
        }

        if (!swAgregado)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
            return;
        }

        if (!this.txtCveCliente.Text.Trim().Equals(this.hdCveCliente.Value))
        {
            CComunDB.CCommun.Guardar_Clave_Producto_Sucursal(this.hdSucursalID.Value,
                                                             this.hdProductoID.Value,
                                                             this.txtCveCliente.Text.Trim());
            Llenar_Productos(false);
        }
        this.txtCveCliente.Text = string.Empty;

        if (this.chkCambiarPrecios.Checked && dcmPrecioUnitario > 0 && !this.hdProductoTipo.Value.Equals("2"))
        {
            decimal dcmPrecioUnitarioOrig = 0;
            decimal.TryParse(this.hdPrecioUnitario.Value, out dcmPrecioUnitarioOrig);
            if (dcmPrecioUnitario != dcmPrecioUnitarioOrig)
            {
                this.lblPrecioLista.Text = this.dlListaPrecios.SelectedItem.Text;
                this.lblPrecioAnterior.Text = dcmPrecioUnitarioOrig.ToString("c") + " " + this.lblMoneda.Text;
                this.txtPrecioUnitarioCambio.Text = dcmPrecioUnitario.ToString();
                this.txtPrecioUnitarioCambio.Focus();
                this.mdCambiarPrecio.Show();
                return;
            }
        }

        this.hdProductoID.Value = string.Empty;

        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private bool Buscar_Producto()
    {
        if (!string.IsNullOrEmpty(this.txtProducto.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, P.tipo, nombre," +
                             "           V.precio_caja as precio " +
                             "          ,P.usar_detalle" +
                             "          ,P.minimo_compra" +
                             "    FROM productos P " +
                             "    LEFT JOIN precios V " +
                             "    ON P.ID = V.producto_ID" +
                             "    AND lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                             "    AND validez = '2099-12-31'" +
                             "    WHERE (codigo = '" + this.txtProducto.Text.Trim() + "' OR" +
                             "           codigo2 = '" + this.txtProducto.Text.Trim() + "' OR" +
                             "           codigo3 = '" + this.txtProducto.Text.Trim() + "'" +
                             "          )" +
                             "    AND tipo <> 9" +
                             " ) R" +
                             " LEFT JOIN producto_datos D" +
                             " ON D.productoID = R.ID" +
							 " AND D.appID = " + Session["SIANAppID"];

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult.Tables[0].Rows.Count == 1)
            {
                this.hdProductoID.Value = objDataResult.Tables[0].Rows[0]["ID"].ToString();
                this.hdProductoTipo.Value = objDataResult.Tables[0].Rows[0]["tipo"].ToString();
                this.txtProducto.Text = objDataResult.Tables[0].Rows[0]["nombre"].ToString() +
                                        "(" + ((decimal)objDataResult.Tables[0].Rows[0]["existencia"]).ToString("0.##") + ")";
                if (objDataResult.Tables[0].Rows[0].IsNull("precio"))
                    this.txtPrecioUnitario.Text = "0.00";
                else
                    this.txtPrecioUnitario.Text = objDataResult.Tables[0].Rows[0]["precio"].ToString();
                this.hdPrecioUnitario.Value = this.txtPrecioUnitario.Text;
                if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
                    this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##");
                else
                    this.hdMinimo.Value = string.Empty;
                this.hdUsarDetalle.Value = ((bool)objDataResult.Tables[0].Rows[0]["usar_detalle"]).ToString();
                string strClientScript = "setTimeout('setCantidad()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private bool Validar_Lote()
    {
        if (this.hdInvAlmacen.Value.Equals("1"))
            return true;

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
                                      true, 
                                      this.txtDetalle.Text.Trim(),
                                      this.chkImpDetalle.Checked,
                                      string.Empty,
                                      "0",
                                      string.Empty,
                                      0,
                                      0,
                                      0,
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

        string strClientScript = "setTimeout('setProductoFoco()',100);";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
    }

    private bool Agregar_Producto(string strProductoID,
                                  string strProducto,
                                  decimal dcmCantidad,
                                  decimal dcmCosto_unitario,
                                  bool llenarProds,
                                  string strDetalle,
                                  bool swImprimirDetalle,
                                  string strLoteFecha,
                                  string strUsarUnimed2,
                                  string strGrupoID,
                                  byte btGrupoConsecutivo,
                                  decimal dcmGrupoCantidad,
                                  decimal dcmGrupoRelacion,
                                  out string strMensaje)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        decimal dcmCosto_Original= dcmCosto_unitario;

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
                    " FROM facturas_liq_prod " +
                    " WHERE factura_ID = " + this.hdID.Value;
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

        strQuery = "INSERT INTO facturas_liq_prod (factura_ID " +
                ",producto_ID, exento, cantidad, costo_unitario" +
                ",costo_original, costo_original_moneda" +
                ",costo, consecutivo, detalle, imprimir_detalle" +
                ",usar_unimed2" +
                ",grupoID, grupo_consecutivo, grupo_cantidad, grupo_relacion" +
                ") VALUES (" +
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
                ", " + strUsarUnimed2 +
                ", " + (string.IsNullOrEmpty(strGrupoID) ? "null" : strGrupoID) +
                ", " + (string.IsNullOrEmpty(strGrupoID) ? "null" : btGrupoConsecutivo.ToString()) +
                ", " + (string.IsNullOrEmpty(strGrupoID) ? "null" : dcmGrupoCantidad.ToString()) +
                ", " + (string.IsNullOrEmpty(strGrupoID) ? "null" : dcmGrupoRelacion.ToString()) +
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

        //if (this.hdInvAlmacen.Value.Equals("0") &&
        //    this.hdInventarios.Value.Equals("1"))
        //{
        //    foreach (GridViewRow gvRow in this.gvLote.Rows)
        //    {
        //        if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim()) > 0)
        //        {
        //            Agregar_Lote_Inventariar(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString(),
        //                                     strProductoID,
        //                                     this.gvLote.DataKeys[gvRow.RowIndex].Value.ToString(),
        //                                     ((TextBox)gvRow.FindControl("txtCantidadLote")).Text.Trim());
        //        }
        //    }
        //}

        if (!string.IsNullOrEmpty(strLoteFecha))
        {
            string[] strLotesFechas = strLoteFecha.Split('^');

            foreach (string strValor in strLotesFechas)
            {
                Agregar_Lote_Inventariar(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString(),
                                        strProductoID,
                                        strValor,
                                        dcmCantidad.ToString());
            }
        }

        this.txtProducto.Text = string.Empty;
        this.txtCantidad.Text = string.Empty;
        this.txtPrecioUnitario.Text = string.Empty;
        this.txtDetalle.Text = string.Empty;

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
                ((ImageButton)e.Row.FindControl("btnUP")).Attributes["style"] =
                    ((ImageButton)e.Row.FindControl("btnDN")).Attributes["style"] =
                    ((ImageButton)e.Row.FindControl("btnPos")).Attributes["style"] = 
                    ((LinkButton)e.Row.Cells[8].Controls[0]).Attributes["style"] = "display:none;";
            }
            else
            {
                if (e.Row.Cells[5].Text.Equals("&nbsp;"))   // Cantidad en blanco, es un kit
                {
                    e.Row.Cells[0].Enabled = false;
                    ((ImageButton)e.Row.FindControl("btnUP")).Attributes["style"] =
                        ((ImageButton)e.Row.FindControl("btnDN")).Attributes["style"] =
                        ((ImageButton)e.Row.FindControl("btnPos")).Attributes["style"] = "display:none;";
                }
                else
                {
                    if (!string.IsNullOrEmpty(((HiddenField)e.Row.FindControl("hdGrupoID")).Value))
                    {
                        ((LinkButton)e.Row.Cells[8].Controls[0]).Attributes["style"] = "display:none;";
                    }

                    if (((LinkButton)e.Row.Cells[0].Controls[0]).Text.Equals(this.hdConsMin.Value))
                        ((ImageButton)e.Row.FindControl("btnUP")).Attributes["style"] = "display:none;";
                    else if (((LinkButton)e.Row.Cells[0].Controls[0]).Text.Equals(this.hdConsMax.Value))
                        ((ImageButton)e.Row.FindControl("btnDN")).Attributes["style"] = "display:none;";
                }
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
                    dcmIVA = Math.Round(dcmCostoIVA * decimal.Parse(this.rdIVA.SelectedValue) / 100, 2);
                else
                    dcmIVA = decimal.Parse(this.hdIVA.Value);
                this.hdIVA.Value = dcmIVA.ToString();
                strLeyenda.Append("<br />IVA " + decimal.Parse(this.rdIVA.SelectedValue).ToString("0.##") + "%:");
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

                e.Row.Cells[0].ColumnSpan = 5;
                e.Row.Cells[0].Text = strLeyenda.ToString();
                e.Row.Cells[1].ColumnSpan = 2;
                e.Row.Cells[1].Text = strValores.ToString();
                e.Row.Cells[2].Text = string.Empty;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
            }
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("mv"))
            return;

        if (this.hdBorrar.Value.Equals("0"))
            return;

        if (Factura_Timbrada())
            return;

        if (this.dlEstatus.SelectedValue.Equals("8"))
        {
            int index = Convert.ToInt32(e.CommandArgument);
            string[] strID_Consecutivo = this.gvProductos.DataKeys[index].Value.ToString().Split('_');
            string strQuery = "SELECT inventariado " +
                             " FROM facturas_liq_prod " +
                             " WHERE factura_ID = " + this.hdID.Value +
                             " AND producto_ID = " + strID_Consecutivo[0] +
                             " AND consecutivo = " + strID_Consecutivo[1];

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (this.hdInvAlmacen.Value.Equals("0") &&
                Convert.ToBoolean(objDataResult.Tables[0].Rows[0]["inventariado"]))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("La operación no puede ejecutarse porque el producto ya fue inventariado en una remisión");
                return;
            }

            if (e.CommandName == "Borrar")
            {
                int intConsecutivo = int.Parse(strID_Consecutivo[1]);
                int intConsecutivoUltimo = intConsecutivo;

                objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT grupoID, grupo_consecutivo" +
                                                            " FROM facturas_liq_prod" +
                                                            " WHERE factura_ID = " + this.hdID.Value +
                                                            " AND consecutivo = " + intConsecutivo);

                if (!objDataResult.Tables[0].Rows[0].IsNull("grupoID"))
                {
                    objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT MIN(consecutivo) as consecutivo, MAX(consecutivo) as consecutivoUltimo" +
                                                                " FROM facturas_liq_prod" +
                                                                " WHERE factura_ID = " + this.hdID.Value +
                                                                " AND grupoID = " + objDataResult.Tables[0].Rows[0]["grupoID"] +
                                                                " AND grupo_consecutivo = " + objDataResult.Tables[0].Rows[0]["grupo_consecutivo"]);

                    intConsecutivo = int.Parse(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString());
                    intConsecutivoUltimo = int.Parse(objDataResult.Tables[0].Rows[0]["consecutivoUltimo"].ToString());
                } 
                
                CComunDB.CCommun.Ejecutar_SP("DELETE " +
                                            " FROM facturas_liq_prod " +
                                            " WHERE factura_ID = " + this.hdID.Value +
                                            " AND consecutivo >= " + intConsecutivo +
                                            " AND consecutivo <= " + intConsecutivoUltimo +
                                            ";" +

                                            "DELETE " +
                                            " FROM facturas_prod_lote " +
                                            " WHERE factura_ID = " + this.hdID.Value +
                                            " AND consecutivo >= " + intConsecutivo +
                                            " AND consecutivo <= " + intConsecutivoUltimo);

                intConsecutivoUltimo = intConsecutivoUltimo - intConsecutivo + 1;
                CComunDB.CCommun.Ejecutar_SP("UPDATE facturas_liq_prod SET " +
                                           " consecutivo = consecutivo - " + intConsecutivoUltimo +
                                           " WHERE factura_ID = " + this.hdID.Value +
                                           " AND consecutivo > " + intConsecutivo +
                                           ";" +

                                           "UPDATE facturas_prod_lote SET " +
                                           " consecutivo = consecutivo - " + intConsecutivoUltimo +
                                           " WHERE factura_ID = " + this.hdID.Value +
                                           " AND consecutivo > " + intConsecutivo +
                                           ";" +

                                           "UPDATE facturas_liq SET " +
                                           " modificadoPorID = " + Session["SIANID"].ToString() +
                                           ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                                           " WHERE ID = " + this.hdID.Value);

                Llenar_Productos(true);
            }
            else
                if (e.CommandName == "Modificar")
                {
                    if (!string.IsNullOrEmpty(((HiddenField)this.gvProductos.Rows[index].FindControl("hdGrupoID")).Value))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Producto no puede modificarse porque es parte de un kit");
                        return;
                    }

                    this.hdConsecutivoID.Value = this.gvProductos.DataKeys[index].Value.ToString();
                    this.txtCambiarCantidad.Text = this.gvProductos.Rows[index].Cells[5].Text;
                    this.txtCambiarUnitario.Text = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginal")).Value;
                    this.lblMonedaCambiar.Text =
                        this.hdMonedaTemp.Value = ((HiddenField)this.gvProductos.Rows[index].FindControl("hdCostoOriginalMoneda")).Value;
                    strQuery = "SELECT P.nombre, C.detalle, C.imprimir_detalle, P.minimo_compra, P.tipo " +
                              " FROM facturas_liq_prod C" +
                              " INNER JOIN productos P " +
                              " ON C.producto_ID = P.ID " +
                              " AND C.factura_ID = " + this.hdID.Value +
                              " AND C.producto_ID = " + strID_Consecutivo[0] +
                              " AND C.consecutivo = " + strID_Consecutivo[1] +
                              ";" +
                              " SELECT lote, fecha_caducidad" +
                              " FROM facturas_prod_lote" +
                              " WHERE factura_ID = " + this.hdID.Value +
                              " AND producto_ID = " + strID_Consecutivo[0] +
                              " AND consecutivo = " + strID_Consecutivo[1];
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

                    if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
                        this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##");
                    else
                        this.hdMinimo.Value = string.Empty;
                    this.hdProductoTipo.Value = objDataResult.Tables[0].Rows[0]["tipo"].ToString();
                    this.lblCambiarProducto.Text = objDataResult.Tables[0].Rows[0]["nombre"].ToString();
					this.txtCambiarDetalle.Text = objDataResult.Tables[0].Rows[0]["detalle"].ToString();
                    this.chkCambiarImpDet.Checked = (bool)objDataResult.Tables[0].Rows[0]["imprimir_detalle"];
                    //if (this.hdInvAlmacen.Value.Equals("0") &&
                    //    this.hdInventarios.Value.Equals("1"))
                    //{
                    //    this.gvCambiarLote.DataSource = ObtenerLotesModificar(strID_Consecutivo[0], strID_Consecutivo[1]);
                    //    this.gvCambiarLote.DataBind();
                    //    foreach (GridViewRow gvRow in this.gvCambiarLote.Rows)
                    //    {
                    //        ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
                    //        ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onkeyup"] = "javascript:return validateCambiar(this);";
                    //        ((TextBox)gvRow.FindControl("txtCantidadLote")).Attributes["onblur"] = "javascript:return validateCambiar(this);";
                    //    }
                    //}
                    StringBuilder strLotes = new StringBuilder();
                    foreach (DataRow objRowResult in objDataResult.Tables[1].Rows)
                    {
                        if (strLotes.Length > 0)
                            strLotes.Append(", ");
                        strLotes.Append(objRowResult["lote"].ToString());
                        if (!objRowResult.IsNull("fecha_caducidad"))
                            strLotes.Append("_" + ((DateTime)objRowResult["fecha_caducidad"]).ToString("yyyy-MM-dd"));
                    }

                    this.lblLote_Fecha.Text = strLotes.ToString();

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

        CFacturaDB objFactura = new CFacturaDB();

        objFactura.Leer(int.Parse(this.hdID.Value));

        objFactura.registrarMod = swGuardarMod;
        objFactura.isr_ret = (this.chkISRRet.Checked ? decimal.Parse(this.hdPorcISRRet.Value) : 0);
        objFactura.iva_ret = (this.chkIVARet.Checked ? decimal.Parse(this.hdPorcIVARet.Value) : 0);
        objFactura.monto_subtotal = Math.Round(decimal.Parse(this.hdCosto.Value), 2);
        objFactura.monto_descuento = Math.Round(decimal.Parse(this.hdCostoDescuento.Value), 2);
        objFactura.monto_iva = Math.Round(decimal.Parse(this.hdIVA.Value), 2);
        objFactura.monto_subtotal2 = Math.Round(decimal.Parse(this.hdSubtotal.Value), 2);
        objFactura.monto_isr_ret = Math.Round(decimal.Parse(this.hdISRRet.Value), 2);
        objFactura.monto_iva_ret = Math.Round(decimal.Parse(this.hdIVARet.Value), 2);
        objFactura.total = Math.Round(decimal.Parse(this.hdTotal.Value), 2);
        objFactura.total_real = objFactura.total;
        objFactura.descuento = decimal.Parse(this.txtDescuento1.Text);
        objFactura.descuento2 = decimal.Parse(this.txtDescuento2.Text);
        objFactura.iva =  decimal.Parse(this.rdIVA.SelectedValue);
        objFactura.tax_manual = (this.hdTAX.Value.Equals("1") ? true : false);
        objFactura.moneda = this.dlMoneda.SelectedValue;
        objFactura.tipo_cambio = decimal.Parse(this.txtTipoCambio.Text);
        objFactura.factura_suf = this.txtSerie.Text;

        if (!objFactura.Guardar())
            ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);

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

        if (this.hdAccion.Value.Equals("1"))
            Cancelar_Factura();
        else
            Sustituir_Factura();
    }

    protected void btnCancelarWSContinuar_Click(object sender, EventArgs e)
    {
        if (this.hdAccion.Value.Equals("1"))
            Cancelar_Factura_Validada();
        else
        {
            Cancelar_Factura_Validada();
            Sustituir_Factura_Validada();
        }
    }

    private bool Cancelar_Factura()
    {
        if (!string.IsNullOrEmpty(this.lblFecha_Timbrado.Text))  // Factura timbrada
        {
            if (!Cancelar_Factura_WS())
                return false;
        }

        Cancelar_Factura_Validada();
        return true;
    }

    private void Cancelar_Factura_Validada()
    {
        string strQuery = "UPDATE facturas_liq SET " +
                         " status = 9" +
                         ",fecha_cancelacion = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         ",modificadoPorID = " + Session["SIANID"].ToString() +
                         ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                         " WHERE ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        strQuery = "INSERT INTO facturas_cancelacion (" +
                       "factura_ID" +
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

        //DataSet objDataResult = new DataSet();

        //strQuery = "SELECT producto_ID " +
        //          " FROM facturas_liq_prod " +
        //          " WHERE factura_ID = " + this.hdID.Value;
        //try
        //{
        //    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        //}
        //catch (ApplicationException ex)
        //{
        //    throw new ApplicationException("Error: " + ex.Message);
        //}

        //foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        //{
        //    CProducto_Datos objProd_Datos = new CProducto_Datos();
        //    objProd_Datos.intProductoID = (int)objRowResult[0];
        //    objProd_Datos.intAppID = int.Parse(Session["SIANAppID"].ToString());
        //    objProd_Datos.Leer();
        //    objProd_Datos.Recalcular_Facturas();
        //    objProd_Datos.Guardar();
        //}

        Mostrar_Datos();
    }

    private bool Cancelar_Factura_WS()
    {
        string strMensaje;

        if (!CFDI.Cancelar_Factura(int.Parse(this.hdID.Value), out strMensaje))
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

        if (dcmCantidad <= 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad no puede ser menor o igual a cero");
            return;
        }

        if (dcmPrecio == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Precio no puede ser igual a cero");
            return;
        }

        if (dcmPrecio < 0  && !this.hdProductoTipo.Value.Equals("2"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Precio sólo puede ser negativo para conceptos, no para productos");
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

        this.hdTAX.Value = "0";
        string[] strID_Consecutivo = this.hdConsecutivoID.Value.Split('_');
        string strMensaje = CComunDB.CCommun.Validar_Ultimo_Precio_Compra(int.Parse(strID_Consecutivo[0]), dcmPrecio, this.dlMoneda.SelectedValue, decimal.Parse(this.txtTipoCambio.Text));
        if (!string.IsNullOrEmpty(strMensaje))
        {
            this.hdAccion.Value = "2";
            this.lblProdVenta.Text = strMensaje;
            this.mdProdVenta.Show();
            return;
        }
        Continuar_Cambiar_Producto();
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
			
        string strQuery = "UPDATE facturas_liq_prod SET " +
                        "cantidad = " + dcmCantidad.ToString() +
                        ",costo_unitario = " + dcmPrecio.ToString() +
                        ",costo_original = " + dcmCosto_Original.ToString() +
                        ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                        ",detalle = " + strDetalle +
                        ",imprimir_detalle = " + strImprimirDetalle +
                        " WHERE factura_ID = " + this.hdID.Value +
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

        if (this.hdInvAlmacen.Value.Equals("0") &&
            this.hdInventarios.Value.Equals("1"))
        {
            strQuery = "DELETE FROM facturas_prod_lote " +
                      " WHERE factura_ID = " + this.hdID.Value +
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

        string strQuery = "INSERT INTO facturas_prod_lote (consecutivo, factura_ID, " +
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

    private DataTable ObtenerOrdenes_Compra()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("orden_compraID", typeof(string)));
        dt.Columns.Add(new DataColumn("orden_compra", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));
        dt.Columns.Add(new DataColumn("estatus", typeof(string)));

        string strQuery = "SELECT C.ID" +
                         ",C.fecha_creacion" +
                         ",C.total" +
                         ",C.moneda" +
                         ",E.estatus" +
                         " FROM orden_compra C" +
                         " INNER JOIN sucursales S" +
                         " ON S.ID = C.sucursalID" +
                         " AND C.estatus IN (2)" + // 2 - Cotejada
                         " AND C.appID = " + Session["SIANAppID"] +
                         " AND S.establecimiento_ID = (" +
                         "      SELECT establecimiento_ID" +
                         "      FROM sucursales" +
                         "      WHERE ID = " + this.hdSucursalID.Value +
                         " )" +
                         " INNER JOIN orden_compra_estatus E" +
                         " ON E.ID = C.estatus" +
                         " LEFT JOIN orden_compra_nota N" +
                         " ON N.orden_compraID = C.ID" +
                         " LEFT JOIN orden_compra_factura F" +
                         " ON F.orden_compraID = C.ID" +
                         " WHERE N.orden_compraID IS NULL" +
                         "   AND F.orden_compraID IS NULL" +

                         " UNION ALL" +
                         " SELECT C.ID" +
                         ",C.fecha_creacion" +
                         ",C.total" +
                         ",C.moneda" +
                         ",E.estatus" +
                         " FROM orden_compra C" +
                         " INNER JOIN sucursales S" +
                         " ON S.ID = C.sucursalID" +
                         " AND C.estatus IN (3)" + // 3 - Surtido y no facturado y no remisionada
                         " AND C.facturada = 0" +
                         " AND C.appID = " + Session["SIANAppID"] +
                         " AND S.establecimiento_ID = (" +
                         "      SELECT establecimiento_ID" +
                         "      FROM sucursales" +
                         "      WHERE ID = " + this.hdSucursalID.Value +
                         " )" +
                         " INNER JOIN orden_compra_estatus E" +
                         " ON E.ID = C.estatus" +
                         " LEFT JOIN orden_compra_nota N" +
                         " ON N.orden_compraID = C.ID" +
                         " WHERE N.orden_compraID IS NULL" +

                         " UNION ALL" +
                         " SELECT C.ID" +
                         ",C.fecha_creacion" +
                         ",C.total" +
                         ",C.moneda" +
                         ",E.estatus" +
                         " FROM orden_compra C" +
                         " INNER JOIN sucursales S" +
                         " ON S.ID = C.sucursalID" +
                         " AND C.estatus IN (4, 5)" + // 4 - Surtido Parcial, 5 -Parcial listo y no remisionada
                         " AND C.remisionada = 0" +
                         " AND C.appID = " + Session["SIANAppID"] +
                         " AND S.establecimiento_ID = (" +
                         "      SELECT establecimiento_ID" +
                         "      FROM sucursales" +
                         "      WHERE ID = " + this.hdSucursalID.Value +
                         " )" +
                         " INNER JOIN orden_compra_estatus E" +
                         " ON E.ID = C.estatus" +
                         " ORDER BY ID";
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
            dr[0] = objRowResult["ID"].ToString();
            dr[1] = "Pedido " + objRowResult["ID"].ToString();
            dr[2] = CRutinas.Fecha_DD_MMM_YYYY((DateTime)objRowResult["fecha_creacion"]);
            dr[3] = ((decimal)objRowResult["total"]).ToString("c") + " " + objRowResult["moneda"].ToString();
            dr[4] = objRowResult["estatus"].ToString();
            dt.Rows.Add(dr);
        }
        return dt;
    }

    private DataTable ObtenerProductosOrden_Compra()
    {
        DataTable dtProd = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dtProd.Columns.Add(new DataColumn("productoID", typeof(string)));
        dtProd.Columns.Add(new DataColumn("producto", typeof(string)));
        dtProd.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dtProd.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dtProd.Columns.Add(new DataColumn("costo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("enabled", typeof(bool)));
        dtProd.Columns.Add(new DataColumn("moneda", typeof(string)));
        dtProd.Columns.Add(new DataColumn("detalle", typeof(string)));
        dtProd.Columns.Add(new DataColumn("impDet", typeof(string)));
        dtProd.Columns.Add(new DataColumn("lote_fecha", typeof(string)));
        dtProd.Columns.Add(new DataColumn("cant_surtida", typeof(string)));
        dtProd.Columns.Add(new DataColumn("cant_facturada", typeof(string)));
        dtProd.Columns.Add(new DataColumn("cant_orig", typeof(string)));
        dtProd.Columns.Add(new DataColumn("consecutivo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("minimo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("usar_unimed2", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupoID", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_consecutivo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_cantidad", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_relacion", typeof(string)));

        string strQuery = "SELECT * FROM ( " +
                         "SELECT C.productoID as productoID " +
                         ", C.cantidad_a_facturar as cantidad " +
                         ", C.cantidad_surtido_a_facturar as cantidad_surtido" +
                         ", C.cantidad_devuelto " +
                         ", C.consecutivo as consecutivo " +
                         ", C.costo_unitario as costo_unitario " +
                         ", C.costo_original" +
                         ", C.costo_original_moneda" +
                         ", C.costo as costo " +
                         ", C.grupoID " +
                         ", C.grupo_cantidad " +
                         ", C.grupo_consecutivo" +
                         ", C.grupo_relacion" +
                         ", C.detalle" +
                         ", C.imprimir_detalle" +
                         ", C.validado" +
                         ", C.cantidad_facturado" +
                         ", CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
                         ", C.exento as exento " +
                         ", C.faltante " +
                         ", C.usar_unimed2 " +
                         ", P.minimo_compra" +
                         ", P.unimed" +
                         ", P.unimed2" +
                         ", P.relacion1" +
                         ", P.relacion2" +
                         ", P.unimed_original" +
                         ", CONCAT('(', G.codigo, ') ', LEFT(G.nombre, 80)) as producto_grupo " +
                         " FROM orden_compra_productos C " +
                         " INNER JOIN productos P " +
                         " ON C.productoID = P.ID " +
                         " AND orden_compraID = " + this.lblOrden_Compra.Text +
                         " LEFT JOIN productos G" +
                         " ON G.ID = C.grupoID" +
                         ") AS A ORDER BY consecutivo, producto" +
                         ";" +
                         "SELECT consecutivo, productoID, lote, fecha_caducidad" +
                         " FROM orden_compra_productos_lote" +
                         " WHERE orden_compraID = " + this.lblOrden_Compra.Text;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        bool swDevuelto;
        decimal dcmCantidadPedida;
        decimal dcmCantidadAFacturar;
        decimal dcmCantidadSurtida;
        decimal dcmCantidadFacturado;
        decimal dcmCosto;
        decimal dcmPrecio;
        StringBuilder strLotes = new StringBuilder();
        string strGrupoConsecutivo = string.Empty;

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            swDevuelto = false;
            dcmCantidadPedida = (decimal)objRowResult["cantidad"];
            dcmPrecio = (decimal)objRowResult["costo_original"];

            if (!objRowResult.IsNull("cantidad_devuelto"))
            {
                if ((decimal)objRowResult["cantidad_devuelto"] == (decimal)objRowResult["cantidad"])
                    swDevuelto = true;
                else
                {
                    dcmCantidadPedida -= (decimal)objRowResult["cantidad_devuelto"];
                }
            }

            dcmCantidadSurtida = (decimal)objRowResult["cantidad_surtido"];

            if (!swDevuelto)
            {
                if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
                {
                    // Se agrega un registro en blanco con los datos del Kit
                    if (!strGrupoConsecutivo.Equals(objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString()))
                    {
                        dr = dtProd.NewRow();
                        dr[0] = objRowResult["productoID"].ToString();
                        dr[1] = objRowResult["producto_grupo"].ToString() + ", Cant.: " + ((decimal)objRowResult["grupo_cantidad"]).ToString("0.##");
                        dr[5] = false;
                        dr[16] = objRowResult["grupoID"].ToString();
                        dr[17] = objRowResult["grupo_consecutivo"].ToString();
                        dtProd.Rows.Add(dr);
                        strGrupoConsecutivo = objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString();
                    }
                }

                dr = dtProd.NewRow();
                dr[0] = objRowResult["productoID"].ToString();

                if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
                {
                    dr[1] = "&nbsp;&nbsp;&nbsp;";
                }

                if (string.IsNullOrEmpty(objRowResult["unimed2"].ToString()))
                    dr[1] += objRowResult["producto"].ToString();
                else
                {
                    if ((bool)objRowResult["usar_unimed2"])
                    {
                        // Siempre se factura en la unidad de medida original
                        if ((bool)objRowResult["unimed_original"])
                        {
                            dr[1] += objRowResult["producto"].ToString() + " (" +
                                    objRowResult["unimed"].ToString() + ")";

                            if ((decimal)objRowResult["relacion1"] == 1)
                            {
                                dcmPrecio = Math.Round(dcmPrecio / (decimal)objRowResult["relacion2"], 2);
                            }
                            else
                            {
                                dcmPrecio = Math.Round(dcmPrecio * (decimal)objRowResult["relacion1"], 2);
                            }
                        }
                        else
                        {
                            dr[1] += objRowResult["producto"].ToString() + " (" +
                                    objRowResult["unimed2"].ToString() + ")";

                        }
                    }
                    else
                        dr[1] += objRowResult["producto"].ToString() + " (" +
                                objRowResult["unimed"].ToString() + ")";
                }

                dr[7] = string.Empty;
                if (!objRowResult.IsNull("detalle") && objRowResult["detalle"].ToString().Length > 0)
                {
                    dr[7] = objRowResult["detalle"].ToString();
                    if (objRowResult["detalle"].ToString().Length < 50)
                        dr[1] += "<br/>" + objRowResult["detalle"].ToString();
                    else
                        dr[1] += "<br/>" + objRowResult["detalle"].ToString().Substring(0, 50);
                }

                dr[10] = dcmCantidadSurtida.ToString("0.##");
                dr[12] = dcmCantidadPedida.ToString("0.##");
                dr[3] = dcmPrecio.ToString("0.00");
                if (this.hdUsuPr.Value.Equals("0"))
                    dr[5] = false;
                else
                    dr[5] = true;
                dr[6] = objRowResult["costo_original_moneda"].ToString();

                dr[8] = (Convert.ToBoolean(objRowResult["imprimir_detalle"])).ToString();

                dcmCantidadFacturado = (decimal)objRowResult["cantidad_facturado"];

                dr[11] = dcmCantidadFacturado.ToString("0.##");

                // Se se ha surtido algo, se pone lo surtido y se quita lo facturado
                dcmCantidadAFacturar = dcmCantidadSurtida - dcmCantidadFacturado;

                if (dcmCantidadAFacturar < 0)
                    dcmCantidadAFacturar = 0;

                dr[2] = dcmCantidadAFacturar.ToString("0.##");

                dcmCosto = dcmCantidadAFacturar * dcmPrecio;
                dr[4] = dcmCosto.ToString("c");

                DataRow[] dtLotes = objDataResult.Tables[1].Select(
                                                   "consecutivo = " + objRowResult["consecutivo"].ToString());
                strLotes.Clear();

                foreach (DataRow dtLote in dtLotes)
                {
                    if (strLotes.Length > 0)
                        strLotes.Append("^");
                    strLotes.Append(dtLote["lote"].ToString() + "_");
                    if (dtLote.IsNull("fecha_caducidad"))
                        strLotes.Append("null");
                    else
                        strLotes.Append(((DateTime)dtLote["fecha_caducidad"]).ToString("yyyy-MM-dd"));
                }

                dr[9] = strLotes.ToString();

                dr[13] = objRowResult["consecutivo"].ToString();

                if (objRowResult.IsNull("minimo_compra"))
                    dr[14] = string.Empty;
                else
                    dr[14] = ((decimal)objRowResult["minimo_compra"]).ToString("0.##");

                dr[15] = ((bool)objRowResult["usar_unimed2"] ? "1" : "0");
                dr[16] = objRowResult["grupoID"].ToString();
                dr[17] = objRowResult["grupo_consecutivo"].ToString();
                dr[18] = objRowResult["grupo_cantidad"].ToString();
                dr[19] = objRowResult["grupo_relacion"].ToString();

                dtProd.Rows.Add(dr);
            }
        }

        return dtProd;
    }

    protected void btnOrden_Compra_Click(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

        this.gvListadoOrden_Compra.DataSource = ObtenerOrdenes_Compra();
        this.gvListadoOrden_Compra.DataBind();

        if (gvListadoOrden_Compra.Rows.Count == 0)
        {
            this.trPedido.Visible = false;
            //((master_MasterPage)Page.Master).MostrarMensajeError("No hay pedidos que puedan usarse para este cliente");
            //return;
        }
        else
            this.trPedido.Visible = true;

        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlOrden_CompraListado.Visible = true;
        this.pnlOrden_Compra.Visible = false;
        this.txtOrden_CompraID.Text = string.Empty;
        this.txtOrden_CompraID.Focus();
    }

    protected void btnBuscarPedido_Click(object sender, EventArgs e)
    {
        int intPedidoID;

        int.TryParse(this.txtOrden_CompraID.Text, out intPedidoID);

        if (intPedidoID == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese el número del pedido");
            return;
        }

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT C.sucursalID, C.estatus, S.establecimiento_ID " +
                                                            " FROM orden_compra C" +
                                                            " INNER JOIN sucursales S " +
                                                            " ON C.sucursalID = S.ID " +
                                                            " AND C.ID = " + intPedidoID +
                                                            " AND C.appID = " + Session["SIANAppID"] +
                                                            ";" +
                                                            "SELECT establecimiento_ID" +
                                                            " FROM sucursales" +
                                                            " WHERE ID = " + this.hdSucursalID.Value +
                                                            ";" + // Pedido con remisión
                                                            " SELECT 1" +
                                                            " FROM orden_compra_nota O" +
                                                            " INNER JOIN notas F" +
                                                            " ON O.notaID = F.ID" +
                                                            " AND F.status <> 9" +
                                                            " AND O.orden_compraID = " + intPedidoID);

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido no existe");
            return;
        }

        if ((int)objDataResult.Tables[0].Rows[0]["establecimiento_ID"] != (int)objDataResult.Tables[1].Rows[0]["establecimiento_ID"])
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido no corresponde al cliente");
            return;
        }

        if ((byte)objDataResult.Tables[0].Rows[0]["estatus"] >= 8)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido se encuentra En Captura o está Cancelado");
            return;
        }

        if ((byte)objDataResult.Tables[2].Rows.Count > 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido ya tiene asociada una remisión");
            return;
        }

        Obtener_OC(intPedidoID.ToString());
        Continuar_Pedido(intPedidoID.ToString(), null);
    }

    protected void btnRegresarOrden_Compra_Click(object sender, EventArgs e)
    {
        Mostrar_Datos();
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlOrden_CompraListado.Visible = false;
        this.pnlOrden_Compra.Visible = false;
    }

    protected void chkSeleccion_CheckedChanged(object sender, EventArgs e)
    {
        Continuar_Pedido(((RadioButton)sender).Text.Substring(((RadioButton)sender).Text.LastIndexOf(" ") + 1), (RadioButton)sender);
        Obtener_OC(((RadioButton)sender).Text.Substring(((RadioButton)sender).Text.LastIndexOf(" ") + 1));
    }

    private void Obtener_OC(string strOrdenID)
    {
        string strCotizacionID = string.Empty;
        string strQuery = "SELECT orden_compra FROM orden_compra WHERE ID=" + strOrdenID;
        DataSet objDataResult = new DataSet();
        DataRow objRowResult;

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        if (objDataResult.Tables[0].Rows.Count != 0)
        {
            objRowResult = objDataResult.Tables[0].Rows[0];

            if (objRowResult["orden_compra"].ToString() != null)
            {
                txtOrdenCompra.Text = objRowResult["orden_compra"].ToString();
            }
        }
        
        strQuery = "SELECT cotizacionID FROM cotizacion_orden_compra WHERE orden_compraID=" + strOrdenID;
        
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }
        
        if (objDataResult.Tables[0].Rows.Count != 0)
        {
            objRowResult = objDataResult.Tables[0].Rows[0];

            if (objRowResult["cotizacionID"].ToString() != null)
            {
                strCotizacionID = objRowResult["cotizacionID"].ToString();

                strQuery = "SELECT atencion FROM cotizacion WHERE ID=" + strCotizacionID;

                try
                {
                    objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch (ApplicationException ex)
                {
                    throw new ApplicationException("Error: " + ex.Message);
                }

                if (objDataResult.Tables[0].Rows.Count != 0)
                {
                    objRowResult = objDataResult.Tables[0].Rows[0];

                    if (objRowResult["atencion"].ToString() != null)
                    {
                        txtRefCliente.Text = objRowResult["atencion"].ToString();
                    }
                }
            }
        }
      
    }

    private void Continuar_Pedido(string strOrden_CompraID, RadioButton rdSeleccionado)
    {
        this.lblOrden_Compra.Text = strOrden_CompraID;

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1" +
                         " FROM orden_compra_factura" +
                         " WHERE orden_compraID = " + this.lblOrden_Compra.Text +
                         "   AND facturaID = " + this.hdID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            if (rdSeleccionado != null)
                rdSeleccionado.Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido ya fue agregado a la factura");
            return;
        }

        strQuery = "SELECT sucursalID, concat(negocio, ' - ', sucursal) as nombre, " +
                  "estatus, O.vendedorID, O.comentarios, O.moneda" +
                  ",lista_preciosID, O.iva, orden_compra, E.contado" +
                  ",modificadoPorFecha" +
                  " FROM orden_compra O" +
                  " INNER JOIN sucursales S " +
                  " ON O.sucursalID = S.ID " +
                  " INNER JOIN establecimientos E " +
                  " ON S.establecimiento_ID = E.ID " +
                  " WHERE O.ID = " + this.lblOrden_Compra.Text;

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            if (rdSeleccionado != null)
                rdSeleccionado.Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido no existe");
            return;
        }

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];
        if (objRowResult["estatus"].ToString().Equals("9"))
        {
            if (rdSeleccionado != null)
                rdSeleccionado.Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido se encuentra cancelado");
            return;
        }

        this.hdVendedorID.Value = objRowResult["vendedorID"].ToString();
        this.hdComentariosPedido.Value = objRowResult["comentarios"].ToString();
        this.txtSucursalOrden.Text = objRowResult["nombre"].ToString();
        this.hdSucursalOrdenID.Value = objRowResult["sucursalID"].ToString();
        this.dlMoneda.SelectedValue = objRowResult["moneda"].ToString();

        this.hdListaPreciosOrden_Compra.Value = objRowResult["lista_preciosID"].ToString();
        this.hdIVAOrden_Compra.Value = objRowResult["iva"].ToString();
        this.hdOrden_Compra.Value = objRowResult["orden_compra"].ToString();
        this.hdTipoOrden_Compra.Value = objRowResult["contado"].ToString();

        this.hdFechaTemp.Value = ((DateTime)objRowResult["modificadoPorFecha"]).ToString("yyyy-MM-dd HH:mm:ss");

        try
        {
            this.gvProductosOrden_Compra.DataSource = ObtenerProductosOrden_Compra();
            this.gvProductosOrden_Compra.DataBind();
        }
        catch (ApplicationException ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(ex.Message);
            return;
        }

        if (this.gvProductosOrden_Compra.Rows.Count == 0)
        {
            if (rdSeleccionado != null)
                rdSeleccionado.Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido no tiene productos");
            return;
        }

        foreach (GridViewRow gvRow in this.gvProductosOrden_Compra.Rows)
        {
            if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                continue;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                    "setTimeout('setFoco(\"" +
                                                    ((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).ClientID +
                                                    "\")',50);",
                                                    true);
            break;
        }

        if (rdSeleccionado == null)
        {
            this.hdAccion.Value = "2";
            this.lblVerificacion.Text = "Pedido ya usado anteriormente en otra factura";
            this.mdVerificacion.Show();
            return;
        }
        else if (objRowResult["estatus"].ToString().Equals("2")) // Pedido cotejado
        {
            if (rdSeleccionado != null)
                rdSeleccionado.Checked = false;
            this.hdAccion.Value = "2";
            this.lblVerificacion.Text = "Pedido cotejado";
            this.mdVerificacion.Show();
            return;
        }
        Mostrar_Orden_Compra();
    }

    private void Mostrar_Orden_Compra()
    {
        this.pnlOrden_Compra.Visible = true;
        this.pnlOrden_CompraListado.Visible = false;

        Obtener_Notas2();
    }

    protected void gvProductosOrden_Compra_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!string.IsNullOrEmpty(((HiddenField)e.Row.FindControl("hdGrupoID")).Value) &&
                string.IsNullOrEmpty(((HiddenField)e.Row.FindControl("hdCantidad")).Value))
            {
                ((TextBox)e.Row.FindControl("txtCostoUnitarioOrden_Compra")).Attributes["style"] = "display:none;";
                ((TextBox)e.Row.FindControl("txtCantidadOrden_Compra")).Attributes["style"] = "display:none;";
            }
            else
            {
                ((TextBox)e.Row.FindControl("txtCostoUnitarioOrden_Compra")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
                ((TextBox)e.Row.FindControl("txtCantidadOrden_Compra")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
            }
        }
    }

    protected void btnProcesar_Click(object sender, EventArgs e)
    {
        if (Pedido_Modificado())
            return;

        decimal dcmCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        bool swSeleccionados = false;
        List<CKitVal> lstKits = new List<CKitVal>();

        foreach (GridViewRow gvRow in this.gvProductosOrden_Compra.Rows)
        {
            if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
            {
                // Si no esta lo agrega
                if (lstKits.Find(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                      x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value)) == null)
                {
                    lstKits.Add(new CKitVal(((HiddenField)gvRow.FindControl("hdGrupoID")).Value,
                                            ((HiddenField)gvRow.FindControl("hdGrupoCons")).Value,
                                            ((Label)gvRow.FindControl("lblProd")).Text)
                               );
                }

                // Si tiene cantidad la suma
                if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                {
                    List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                  x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                    lstUnKit[0].dcmSuma += decimal.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value);
                }
            }
        }

        foreach (GridViewRow gvRow in this.gvProductosOrden_Compra.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    continue;

                decimal.TryParse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text.Trim(), out dcmCantidad);
                decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoUnitarioOrden_Compra")).Text.Trim(), out dcmPrecioUnitario);

                ((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text = Math.Round(dcmCantidad, 2).ToString();
                ((TextBox)gvRow.FindControl("txtCostoUnitarioOrden_Compra")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();

                if (dcmCantidad > 0)
                    swSeleccionados = true;

                if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
                {
                    if (dcmCantidad > 0 &&
                        dcmCantidad != decimal.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto " + ((Label)gvRow.FindControl("lblProd")).Text +
                                                                             " no puede modificarse porque es parte de un kit");
                        return;
                    }

                    List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                    x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                    lstUnKit[0].dcmSumaIngresada += dcmCantidad;
                }

                //if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdMinimo")).Value) && dcmCantidad > 0)
                //{
                //    if (dcmCantidad % decimal.Parse(((HiddenField)gvRow.FindControl("hdMinimo")).Value) != 0)
                //    {
                //        ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto " + ((Label)gvRow.FindControl("lblProd")).Text +
                //                                                             " debe ser un múltiplo de " + ((HiddenField)gvRow.FindControl("hdMinimo")).Value);
                //        return;
                //    }
                //}
            }
        }

        if (!swSeleccionados)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese la cantidad de al menos un producto");
            return;
        }

        foreach (CKitVal kit in lstKits)
        {
            if (kit.dcmSumaIngresada > 0 &&
                kit.dcmSuma != kit.dcmSumaIngresada)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Tiene que ingresar la cantidad de todos los productos del kit " +
                                                                      kit.strNombre);
                return;
            }
        }

        StringBuilder strMensajeFinal = new StringBuilder();
        bool swAgregados = false;
        foreach (GridViewRow gvRow in this.gvProductosOrden_Compra.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    continue;

                if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text) > 0)
                {
                    string strMensaje = string.Empty;
                    this.hdMoneda.Value = ((Label)gvRow.FindControl("lblMoneda")).Text;

                    if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
                    {
                        List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                    x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                        if (lstUnKit[0].btNuevoGrupoConsecutivo == 0)
                        {
                            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT IFNULL(MAX(grupo_consecutivo) + 1, 1) as consecutivo " +
                                                                                " FROM facturas_liq_prod " +
                                                                                " WHERE factura_ID = " + this.hdID.Value);
                            lstUnKit[0].btNuevoGrupoConsecutivo = byte.Parse(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString());
                        }

                        if (Agregar_Producto(this.gvProductosOrden_Compra.DataKeys[gvRow.RowIndex].Value.ToString(),
                                            ((Label)gvRow.FindControl("lblProd")).Text,
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text.Trim()),
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCostoUnitarioOrden_Compra")).Text.Trim()),
                                            false,
                                            ((HiddenField)gvRow.FindControl("hdDetalle")).Value,
                                            Convert.ToBoolean(((HiddenField)gvRow.FindControl("hdImpDetalle")).Value),
                                            ((HiddenField)gvRow.FindControl("hdLoteFecha")).Value,
                                            ((HiddenField)gvRow.FindControl("hdUsar_Unimed2")).Value,
                                            lstUnKit[0].strGrupoID,
                                            lstUnKit[0].btNuevoGrupoConsecutivo,
                                            decimal.Parse(((HiddenField)gvRow.FindControl("hdGrupoCantidad")).Value),
                                            decimal.Parse(((HiddenField)gvRow.FindControl("hdGrupoRelacion")).Value),
                                            out strMensaje))
                        {
                            swAgregados = true;

                            CComunDB.CCommun.Ejecutar_SP("UPDATE orden_compra_productos" +
                                                        " SET facturado = " +
                                                        "     CASE WHEN cantidad_a_facturar > (cantidad_facturado + " + decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text) + ")" +
                                                        "          THEN 0" +
                                                        "          ELSE 1" +
                                                        "     END" +
                                                        ", cantidad_facturado = cantidad_facturado + " + decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text) +
                                                        " WHERE orden_compraID = " + this.lblOrden_Compra.Text +
                                                        "   AND productoID = " + this.gvProductosOrden_Compra.DataKeys[gvRow.RowIndex].Value.ToString() +
                                                        "   AND consecutivo = " + ((HiddenField)gvRow.FindControl("hdConsecutivo")).Value);

                        }
                        else
                        {
                            if (strMensajeFinal.Length > 0)
                                strMensajeFinal.Append("<br>");
                            strMensajeFinal.Append(strMensaje);
                        }
                    }
                    else
                    {

                        if (Agregar_Producto(this.gvProductosOrden_Compra.DataKeys[gvRow.RowIndex].Value.ToString(),
                                            ((Label)gvRow.FindControl("lblProd")).Text,
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text.Trim()),
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCostoUnitarioOrden_Compra")).Text.Trim()),
                                            false,
                                            ((HiddenField)gvRow.FindControl("hdDetalle")).Value,
                                            Convert.ToBoolean(((HiddenField)gvRow.FindControl("hdImpDetalle")).Value),
                                            ((HiddenField)gvRow.FindControl("hdLoteFecha")).Value,
                                            ((HiddenField)gvRow.FindControl("hdUsar_Unimed2")).Value,
                                            string.Empty,
                                            0,
                                            0,
                                            0,
                                            out strMensaje))
                        {
                            swAgregados = true;

                            CComunDB.CCommun.Ejecutar_SP("UPDATE orden_compra_productos" +
                                                        " SET facturado = " +
                                                        "     CASE WHEN cantidad_a_facturar > (cantidad_facturado + " + decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text) + ")" +
                                                        "          THEN 0" +
                                                        "          ELSE 1" +
                                                        "     END" +
                                                        ", cantidad_facturado = cantidad_facturado + " + decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadOrden_Compra")).Text) +
                                                        " WHERE orden_compraID = " + this.lblOrden_Compra.Text +
                                                        "   AND productoID = " + this.gvProductosOrden_Compra.DataKeys[gvRow.RowIndex].Value.ToString() +
                                                        "   AND consecutivo = " + ((HiddenField)gvRow.FindControl("hdConsecutivo")).Value);

                        }
                        else
                        {
                            if (strMensajeFinal.Length > 0)
                                strMensajeFinal.Append("<br>");
                            strMensajeFinal.Append(strMensaje);
                        }
                    }
                }
            }
        }

        this.hdMoneda.Value = CComunDB.CCommun.ObtenerMonedaListasPrecios(this.dlListaPrecios.SelectedValue);

        string strQuery;
        if (swAgregados)
        {
            CComunDB.CCommun.Ejecutar_SP("UPDATE orden_compra" +
                                        " SET facturada = " +
                                        " (" +
                                        "    SELECT DISTINCT facturado" +
                                        "    FROM orden_compra_productos" +
                                        "    WHERE orden_compraid = " + this.lblOrden_Compra.Text +
                                        "    ORDER BY facturado" +
                                        "    LIMIT 1" +
                                        " )" +
                                        ",modificadoPorID = " + Session["SIANID"].ToString() +
                                        ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                                        " WHERE ID = " + this.lblOrden_Compra.Text);

            strQuery = "INSERT INTO orden_compra_factura (orden_compraID, facturaID)  " +
                      " VALUES('" + this.lblOrden_Compra.Text + "'" +
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
                ((master_MasterPage)Page.Master).MostrarMensajeError("Los productos fueron agregados");
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("Ningún producto fue agregado a la factura");
        else
            if (swAgregados)
            {
                Llenar_Productos(true);
                ((master_MasterPage)Page.Master).MostrarMensajeError("Se agregaron productos, pero los siguientes no: <br>" + strMensajeFinal.ToString());
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("Los productos no se agregaron: <br>" + strMensajeFinal.ToString());

        strQuery = "UPDATE facturas_liq SET" +
                  " comentarios = '" + this.hdComentariosPedido.Value + "'";
        this.txtComentarios.Text = this.hdComentariosPedido.Value;

        if (!this.hdVendedorID.Value.Equals("0"))
        {
            this.dlVendedor.ClearSelection();
            this.dlVendedor.Items.FindByValue(this.hdVendedorID.Value).Selected = true;
            strQuery += ",vendedorID = " + this.hdVendedorID.Value;
        }

        strQuery += " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlOrden_Compra.Visible = false;
        this.pnlOrden_CompraListado.Visible = false;
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

    protected void dlListaPreciosOrden_Compra_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.gvProductosOrden_Compra.DataSource = ObtenerProductosOrden_Compra();
        this.gvProductosOrden_Compra.DataBind();
    }

    protected void btnImprimir_Click(object sender, ImageClickEventArgs e)
    {

        if (System.IO.File.Exists(Server.MapPath("../facturas" + "/factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx")))
        {
            if (string.IsNullOrEmpty(this.lblFactura.Text))
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + "_pre.aspx?factID=" + this.hdID.Value + "')",
                                                    true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx?factID=" + this.hdID.Value + "')",
                                                    true);
        }
        else
        {
            if (string.IsNullOrEmpty(this.lblFactura.Text))
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_preimpresion.aspx?factID=" + this.hdID.Value + "')",
                                                    true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                    "SIANRPT",
                                                    "mostrarPopUp('factura_impresion.aspx?factID=" + this.hdID.Value + "')",
                                                    true);
        }
    }

    protected void rdIVA_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    protected void dlNegocios_SelectedIndexChanged(object sender, EventArgs e)
    {
        Obtener_Notas();
    }

    private void Obtener_Notas()
    {
        if (string.IsNullOrEmpty(this.hdSucursalID.Value))
        {
            this.lblNotas.Text = string.Empty;
            return;
        }

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

    private void Obtener_Notas2()
    {
        string[] strValores = Obtener_NotasWeb(this.hdSucursalOrdenID.Value).Split('|');
        this.lblNotas2.Text = strValores[0];
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
        if (Factura_Timbrada())
            return;

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
        if (Factura_Timbrada())
            return;

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
        // Contabilidad
        if (CComunDB.CCommun.Obtener_Valor_CatParametros(9).Equals("1"))
        {
            if(DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX")) < DateTime.Today)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Debido a cuestiones de contablidad, la fecha debe ser la del día de hoy");
                return;
            }
        }

        if (!Actualizar_Factura(8))
            return;

        if (!Validar_Inventario())
            return;

        string strMensaje = CComunDB.CCommun.Validar_Limite(0, int.Parse(this.hdSucursalID.Value), decimal.Parse(this.hdTotal.Value));
        if (!string.IsNullOrEmpty(strMensaje))
        {
            this.lblLimiteCR.Text = strMensaje;
            this.mdLimiteCR.Show();
            return;
        }

        Continuar_FE();
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
        Continuar_FE();
    }

    private void Continuar_FE()
    {
        decimal dcmResto = Math.Round(decimal.Parse(this.hdTotal.Value) -
                                      decimal.Parse(this.lnkPagos.Text.Remove(0, 1)),
                                      2);

        if (dcmResto < 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Monto total de la factura menor a los pagos ya registrados");
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
                Timbrar_Factura();
        }
        else
        {
            if (dcmResto == 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Factura ya se encuentra pagada por lo que no se puede poner mandar a cobranza, cambie el tipo a Contado");
                return;
            }
            Timbrar_Factura();
        }
    }

    private bool Validar_Inventario()
    {
        if (this.hdInvAlmacen.Value.Equals("1"))
            return true;

        if (this.hdInventarios.Value.Equals("0"))
            return true;

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT productoID, producto, lote, SUM(cantidad) as cantidad FROM (" +
                          "SELECT C.producto_ID as productoID " +
                          ", P1.nombre as producto " +
                          ", C.lote as lote " +
                          ", C.cantidad as cantidad " +
                          " FROM facturas_prod_lote C " +
                          " INNER JOIN facturas_liq_prod P " +
                          " ON C.producto_ID = P.producto_ID " +
                          " AND P.factura_ID = C.factura_ID " +
                          " AND P.factura_ID = " + this.hdID.Value +
                          " AND P.inventariado = 0" +
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

    private bool Timbrar_Factura()
    {
        if (!CComunDB.CCommun.Validar_Folios_Electronicos())
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Se ha alcanzado el máximo número de folios electrónicos permitidos para timbrar, favor de contactar al administrador");
            return false;
        }

        string strMensaje, strUUID;
        DateTime dtFechaTimbrado;
        if (CFE.CFDI.Generar(int.Parse(this.hdID.Value),
                            out strMensaje,
                            out strUUID,
                            out dtFechaTimbrado))
        {
            Mostrar_Datos();
            if (strMensaje.Contains("UUID:") ||
                strMensaje.Contains("creada"))
            {
                Actualizar_Inventario();
                // Contabilidad
                if (CComunDB.CCommun.Obtener_Valor_CatParametros(9).Equals("1"))
                {
                    Generar_Movimiento_Contable_Diario();
                }
            }

            this.lblCorreo.Text = strMensaje;
            this.mdCorreo.Show();

            return true;
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Hubo un error al generar la factura eléctrónica:" + strMensaje);
            return false;
        }
    }

    private void Actualizar_Inventario()
    {
        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT producto_ID, cantidad, costo_original, costo_original_moneda" +
                          " FROM facturas_liq_prod" +
                          " WHERE factura_ID = " + this.hdID.Value;

        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            CProducto_Datos objProd_Datos = new CProducto_Datos();
            objProd_Datos.intProductoID = (int)objRowResult["producto_ID"];
			objProd_Datos.intAppID = int.Parse(Session["SIANAppID"].ToString());
            objProd_Datos.Leer();
            objProd_Datos.intFacturaID = int.Parse(this.hdID.Value);
            objProd_Datos.dcmFactura_tipocambio = decimal.Parse(this.txtTipoCambio.Text);
            objProd_Datos.dtFactura_fecha = DateTime.Parse(this.txtFecha.Text, CultureInfo.CreateSpecificCulture("es-MX"));
            objProd_Datos.dcmFactura_cantidad = (decimal)objRowResult["cantidad"];
            objProd_Datos.dcmFactura_costo = (decimal)objRowResult["costo_original"];
            objProd_Datos.strFactura_costo_moneda = objRowResult["costo_original_moneda"].ToString();
            objProd_Datos.dcmFactura_total = Math.Round((decimal)objRowResult["costo_original"] * (decimal)objRowResult["cantidad"], 2);

            objProd_Datos.Verificar_Factura();

            objProd_Datos.Guardar();
        }

        if (this.hdInvAlmacen.Value.Equals("1"))
            return;

        strQuery = "SELECT L.producto_ID as productoID " +
                  ", L.lote as lote " +
                  ", SUM(L.cantidad) as cantidad " +
                  " FROM facturas_prod_lote L " +
                  " INNER JOIN facturas_liq_prod C " +
                  " ON C.producto_ID = L.producto_ID " +
                  "  AND C.consecutivo = L.consecutivo" +
                  "  AND C.factura_ID = L.factura_ID " +
                  "  AND C.factura_ID = " + this.hdID.Value +
                  "  AND C.inventariado = 0" +
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
                                "Factura " + this.hdID.Value);
        }

        strQuery = "UPDATE facturas_liq_prod " +
                   " SET inventariado = 1 " +
                   " WHERE factura_ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);
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
                                                "mostrarMailPopUp('factura_impresion_" + Request.ApplicationPath.Replace("/", "") + ".aspx?m=Y&fact=" + this.lblFactura.Text + ".xml')",
                                                true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                                                "SIANRPT",
                                                "mostrarMailPopUp('factura_impresion.aspx?m=Y&fact=" + this.lblFactura.Text + ".xml')",
                                                true);
        }
    }

    protected void btnXML_Click(object sender, ImageClickEventArgs e)
    {
        string strPopUP = "mostrarPopUp('factura_xml.aspx?fact=" +
                                   this.lblFactura.Text + ".xml')";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "SIANRPT", strPopUP, true);
    }

    protected void chkIVARet_CheckedChanged(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

        this.hdTAX.Value = "0";

        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);
    }

    protected void chkISRRet_CheckedChanged(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

        this.hdTAX.Value = "0";

        if (!this.hdID.Value.Equals("0"))
            Llenar_Productos(true);

    }

    private void Mostrar_Pagos(string strFacturaID, string strFactura)
    {
        this.lblPagos.Text = "Pagos de la factura " + strFactura;
        this.gvPagos.DataSource = ObtenerPagos(strFacturaID);
        this.gvPagos.DataBind();
        this.mdCambiarPagos.Show();
    }

    private DataTable ObtenerPagos(string strFacturaID)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("no_pago", typeof(string)));
        dt.Columns.Add(new DataColumn("tipo", typeof(string)));
        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("banco", typeof(string)));
        dt.Columns.Add(new DataColumn("cuenta", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));

        string strQuery = "SELECT P.ID as no_pago, T.tipo_pago, P.referencia, P.fecha_pago, F.monto_pago" +
                         ",F.notaID, S.moneda, BA.banco, B.nombre as cuenta_bancaria" +
                         " FROM pago_facturas F" +
                         " INNER JOIN pagos P" +
                         " ON P.ID = F.pagoID" +
                         " AND F.facturaID = " + strFacturaID +
                         " AND F.aplicado = 1" +
                         " INNER JOIN facturas_liq S" +
                         " ON S.ID = F.facturaID" +
                         " INNER JOIN tipos_pagos T" +
                         " ON T.ID = P.tipo_pago" +
                         " LEFT JOIN cuentas_bancarias B" +
                         " ON B.ID = P.cuenta_bancariaID" +
                         " INNER JOIN bancos as BA ON BA.ID = B.bancoID" +
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
            dr[0] = objRowResult["no_pago"].ToString();
            dr[1] = objRowResult["tipo_pago"].ToString();
            dr[2] = objRowResult["referencia"].ToString();
            dr[3] = objRowResult["banco"].ToString();
            dr[4] = objRowResult["cuenta_bancaria"].ToString();
            dr[5] = CRutinas.Fecha_DD_MMM_YYYY((DateTime)objRowResult["fecha_pago"]);
            dr[6] = ((decimal)objRowResult["monto_pago"]).ToString("c") + " " + objRowResult["moneda"].ToString();
            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected void btnPagoContinuar_Click(object sender, EventArgs e)
    {
        if (!Timbrar_Factura())
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
                          ",aplicado, monto_aplicado, estatusID" +
                          ",creadoPorID, creadoPorFecha, appID) VALUES(" +
                          "'" + this.dlTiposPagos.SelectedValue + "'" +
                          ",'" + dtFecha.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          ",'" + this.txtReferencia.Text.Trim().Replace("'", "''") + "'" +
                          ",'" + this.txtPago.Text + "'" +
                          ",'" + this.dlMoneda.SelectedValue + "'" +
                          ",'" + this.txtTipoCambio.Text + "'" +
                          "," + strCuentaBancaria +
                          ",1" +
                          ",'" + this.txtPago.Text + "'" +
                          ",8" +
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

        string strPagoID = objDataResult.Tables[0].Rows[0]["ID"].ToString();

        strQuery = "INSERT INTO pago_facturas (pagoID, facturaID, monto_pago) VALUES(" +
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

        strQuery = "UPDATE facturas_liq SET " +
                  " fecha_pago = '" + dtFecha.ToString("yyyy-MM-dd") + "'" +
                  ",modificadoPorID = " + Session["SIANID"].ToString() +
                  ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                  " WHERE ID = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        Obtener_CreadoPor();

        strQuery = "SELECT establecimiento_ID as estabID, fecha_ultimo_pago, E.cid, E.razonsocial " +
                  " FROM sucursales S" +
                  " INNER JOIN establecimientos E" +
                  " ON S.establecimiento_ID = E.ID" +
                  "    AND S.ID = " + this.hdSucursalID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (CComunDB.CCommun.Obtener_Valor_CatParametros(9).Equals("1"))
        {
            Generar_Movimiento_Contable_Ingreso(strPagoID,
                                                objDataResult.Tables[0].Rows[0]["cid"].ToString(),
                                                objDataResult.Tables[0].Rows[0]["razonsocial"].ToString());
        }

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
        if (!Timbrar_Factura())
            return;

        // Se pone como No pagada
        string strQuery = "UPDATE facturas_liq SET " +
                    "status = 7" +
                    ",fecha_cobranza = '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
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
        Mostrar_Pagos(this.hdID.Value, this.lblFacturaID.Text + this.txtSerie.Text);
    }

    private void Sustituir_Factura()
    {
        CFacturaDB objFactura = new CFacturaDB();

        objFactura.Leer(int.Parse(this.hdID.Value));

        objFactura.ID = 0;
        objFactura.factura = string.Empty;
        objFactura.factura_suf = string.Empty;
        objFactura.fecha = DateTime.Now;
        objFactura.fecha_timbrado = null;
        objFactura.status = 8;

        if (objFactura.Guardar())
        {
            this.hdFactID.Value = objFactura.ID.ToString();
            this.hdFolio.Value = objFactura.folio.ToString();
            this.txtMotivoCancelacion.Text += " Fact. " + objFactura.folio;
            if (!Cancelar_Factura())
                return;

            Sustituir_Factura_Validada();
        }
        else
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(objFactura.Mensaje);
            return;
        }

        Mostrar_Datos();
    }

    private void Sustituir_Factura_Validada()
    {
        string strQuery = "INSERT INTO facturas_liq_prod (factura_ID, " +
                         "producto_ID, exento, cantidad, costo_unitario, " +
                         "costo_original, costo_original_moneda," +
                         "costo, consecutivo, inventariado, appID, detalle, imprimir_detalle)" +
                         " SELECT " + this.hdFactID.Value +
                         " ,producto_ID, exento, cantidad, costo_unitario " +
                         " ,costo_original, costo_original_moneda" +
                         " ,costo, consecutivo, inventariado, appID, detalle, imprimir_detalle" +
                         " FROM facturas_liq_prod " +
                         " WHERE factura_ID = " + this.hdID.Value;

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        strQuery = "INSERT INTO facturas_prod_lote (factura_ID, " +
                  "producto_ID, consecutivo, lote, fecha_caducidad, cantidad)" +
                  " SELECT " + this.hdFactID.Value +
                  " ,producto_ID, consecutivo, lote, fecha_caducidad, cantidad" +
                  " FROM facturas_prod_lote " +
                  " WHERE factura_ID = " + this.hdID.Value;

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        strQuery = "INSERT INTO facturas_reemplazo (factura_ID, " +
                  "factura_IDAnt) VALUES (" +
                  this.hdFactID.Value +
                  "," + this.hdID.Value +
                  ")";

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        strQuery = "UPDATE nota_facturas" +
                  " SET factura_ID = " + this.hdFactID.Value +
                  " WHERE factura_ID = " + this.hdID.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        ((master_MasterPage)Page.Master).MostrarMensajeError("Se ha generado la factura: " + this.hdFolio.Value);
    }

    protected void btnUP_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((ImageButton)sender).CommandArgument))
            return;

        if (this.hdBorrar.Value.Equals("0"))
            return;

        if (this.hdContieneKits.Value.Equals("1"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Acción no puede ejecutarse porque existen kits en la lista de productos");
            return;
        }

        if (!((ImageButton)sender).CommandArgument.Equals(this.hdConsMin.Value))
        {
            Mover(((ImageButton)sender).CommandArgument, true);
        }
    }

    protected void btnMV_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((ImageButton)sender).CommandArgument))
            return;

        if (this.hdBorrar.Value.Equals("0"))
            return;

        if (this.hdContieneKits.Value.Equals("1"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Acción no puede ejecutarse porque existen kits en la lista de productos");
            return;
        }

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
        string strQuery = "UPDATE facturas_liq_prod SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND factura_id = " + this.hdID.Value + ";" +
                         "UPDATE facturas_prod_lote SET " +
                         " consecutivo = 0" +
                         " WHERE consecutivo = " + btnUPID +
                         "   AND factura_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Mueve el producto arriba a su nueva posicion
        strQuery = "UPDATE facturas_liq_prod SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND factura_id = " + this.hdID.Value + ";" +
                  "UPDATE facturas_prod_lote SET " +
                  " consecutivo = " + btnUPID +
                  " WHERE consecutivo = " + intAntInicio +
                  "   AND factura_id = " + this.hdID.Value;

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        //Ahora mueve el producto
        strQuery = "UPDATE facturas_liq_prod SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND factura_id = " + this.hdID.Value + ";" +
                  "UPDATE facturas_prod_lote SET " +
                  " consecutivo = " + intAntInicio +
                  " WHERE consecutivo = 0" +
                  "   AND factura_id = " + this.hdID.Value;

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

        if (this.hdBorrar.Value.Equals("0"))
            return;

        if (this.hdContieneKits.Value.Equals("1"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Acción no puede ejecutarse porque existen kits en la lista de productos");
            return;
        }

        if (!btnDN.CommandArgument.Equals(this.hdConsMax.Value))
            Mover((int.Parse(btnDN.CommandArgument) + 1).ToString(), true);
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
        dt.Columns.Add(new DataColumn("minimo", typeof(string)));

        string strQuery = "SELECT P.ID as productoID " +
                         " ,P.codigo" +
                         " ,P.minimo_compra" +
                         " ,CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
                         " ,S.precio_caja as costo " +
                         " ,IFNULL(D.existencia, 0) as existencia" +
                         " FROM precios S " +
                         " INNER JOIN productos P " +
                         " ON P.ID = S.producto_ID " +
                         " AND S.lista_precios_ID = " + this.dlListaPrecios.SelectedValue +
                         " AND S.validez = '2099-12-31' " +
                         " LEFT JOIN producto_datos D" +
                         " ON D.productoID = P.ID" +
					     " AND D.appID = " + Session["SIANAppID"] +
                         " ORDER BY P.nombre ";
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
            if (objRowResult.IsNull("minimo_compra"))
                dr[7] = string.Empty;
            else
                dr[7] = ((decimal)objRowResult["minimo_compra"]).ToString("0.##");
            dt.Rows.Add(dr);
        }

        ViewState["listaTmp"] = dt;
        return dt;
    }

    protected void btnLista_Click(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

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
        this.lblMonedaListaPrecios.Text = CComunDB.CCommun.ObtenerMonedaListasPrecios(this.dlListaPrecios.SelectedValue);

        this.gvListaPreciosProductos.DataSource = ObtenerProductosLista();
        this.gvListaPreciosProductos.DataBind();

        foreach (GridViewRow gvRow in this.gvListaPreciosProductos.Rows)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                    "setTimeout('setFoco(\"" +
                                                    ((TextBox)gvRow.FindControl("txtCantidadLista")).ClientID +
                                                    "\")',50);",
                                                    true);
            break;
        }

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
        foreach (GridViewRow gvRow in this.gvListaPreciosProductos.Rows)
        {
            if (this.hdProductoID.Value.Equals(this.gvListaPreciosProductos.DataKeys[gvRow.RowIndex].Value.ToString()))
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

        decimal dcmCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        decimal.TryParse(this.txtCantLista.Text.Trim(), out dcmCantidad);
        decimal.TryParse(this.txtPrecioLista.Text.Trim(), out dcmPrecioUnitario);
        dcmCantidad = Math.Round(dcmCantidad, 2);
        dcmPrecioUnitario = Math.Round(dcmPrecioUnitario, 2);

        this.txtCantLista.Text = dcmCantidad.ToString();
        this.txtPrecioLista.Text = dcmPrecioUnitario.ToString();

        if (dcmCantidad > 0 &&
            dcmPrecioUnitario > 0)
        {
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT minimo_compra" +
                                                                " FROM productos" +
                                                                " WHERE ID = " + this.hdProductoID.Value);

            this.hdMinimo.Value = string.Empty;
            if (!objDataResult.Tables[0].Rows[0].IsNull("minimo_compra"))
            {
                if (dcmCantidad % (decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"] != 0)
                {
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto debe ser un múltiplo de " + ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString("0.##"));
                    return;
                }

                this.hdMinimo.Value = ((decimal)objDataResult.Tables[0].Rows[0]["minimo_compra"]).ToString();
            }

            string strMensaje = CComunDB.CCommun.Validar_Ultimo_Precio_Compra(int.Parse(this.hdProductoID.Value), dcmPrecioUnitario, this.dlMoneda.SelectedValue, decimal.Parse(this.txtTipoCambio.Text));
            if (!string.IsNullOrEmpty(strMensaje))
            {
                this.hdAccion.Value = "3";
                this.lblProdVenta.Text = strMensaje;
                this.mdProdVenta.Show();
                return;
            }
            Continuar_Agregar_ProdLista();
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad y precio unitario deben ser mayores a cero");

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
                             "    WHERE (codigo = '" + this.txtProdLista.Text.Trim() + "' OR" +
                             "           codigo2 = '" + this.txtProdLista.Text.Trim() + "' OR" +
                             "           codigo3 = '" + this.txtProdLista.Text.Trim() + "'" +
                             "          )" +
                             "    AND tipo < 2" +
                             " ) R" +
                             " LEFT JOIN producto_datos D" +
                             " ON D.productoID = R.ID" +
                             " AND D.appID = " + Session["SIANAppID"];

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
        foreach (GridViewRow gvRow in this.gvListaPreciosProductos.Rows)
        {
            dt.Rows[i][3] = ((TextBox)gvRow.FindControl("txtCantidadLista")).Text;
            dt.Rows[i][4] = ((TextBox)gvRow.FindControl("txtCostoLista")).Text;
            dt.Rows[i][5] = ((CheckBox)gvRow.FindControl("chkAct")).Checked;
            i++;
        }

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT codigo" +
                                                            " FROM productos" +
                                                            " WHERE ID = " + this.hdProductoID.Value);

        DataRow dr = dt.NewRow();
        dr[0] = this.hdProductoID.Value;
        dr[1] = this.txtProdLista.Text;
        dr[2] = objDataResult.Tables[0].Rows[0]["codigo"].ToString();
        dr[3] = this.txtCantLista.Text;
        dr[4] = this.txtPrecioLista.Text;
        dr[5] = true;
        if (this.hdUsuPr.Value.Equals("0"))
            dr[6] = false;
        else
            dr[6] = true;
        dr[7] = this.hdMinimo.Value;
        dt.Rows.Add(dr);

        this.gvListaPreciosProductos.DataSource = dt;
        this.gvListaPreciosProductos.DataBind();

        this.txtProdLista.Text = string.Empty;
        this.txtCantLista.Text = string.Empty;
        this.txtPrecioLista.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;
        this.hdMinimo.Value = string.Empty;
    }

    protected void btnProcesarLista_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        foreach (GridViewRow gvRow in this.gvListaPreciosProductos.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                decimal.TryParse(((TextBox)gvRow.FindControl("txtCantidadLista")).Text.Trim(), out dcmCantidad);
                decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim(), out dcmPrecioUnitario);
                ((TextBox)gvRow.FindControl("txtCantidadLista")).Text = Math.Round(dcmCantidad, 2).ToString();
                ((TextBox)gvRow.FindControl("txtCostoLista")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();

                if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdMinimo")).Value) && dcmCantidad > 0)
                {
                    if (dcmCantidad % decimal.Parse(((HiddenField)gvRow.FindControl("hdMinimo")).Value) != 0)
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto " + ((Label)gvRow.FindControl("lblProdList")).Text +
                                                                             " debe ser un múltiplo de " + ((HiddenField)gvRow.FindControl("hdMinimo")).Value);
                        return;
                    }
                }
            }
        }

        StringBuilder strMensajeFinal = new StringBuilder();
        foreach (GridViewRow gvRow in this.gvListaPreciosProductos.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadLista")).Text) > 0)
                {
                    string strMensaje = string.Empty;
                    Agregar_Producto(this.gvListaPreciosProductos.DataKeys[gvRow.RowIndex].Value.ToString(),
                                    gvRow.Cells[0].Text,
                                    decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadLista")).Text.Trim()),
                                    decimal.Parse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim()),
                                    false,
                                    string.Empty, 
                                    false,
                                    string.Empty,
                                    "0",
                                    string.Empty,
                                    0,
                                    0,
                                    0,
                                    out strMensaje);
                    if (((CheckBox)gvRow.FindControl("chkAct")).Checked)
                    {
                        CComunDB.CCommun.Actualizar_Precio(this.gvListaPreciosProductos.DataKeys[gvRow.RowIndex].Value.ToString(),
                                                           this.dlListaPrecios.SelectedValue,
                                                           decimal.Parse(((TextBox)gvRow.FindControl("txtCostoLista")).Text.Trim()));
                    }
                }
                ((TextBox)gvRow.FindControl("txtCantidadLista")).Text = "0";
            }
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Productos fueron agregados");
		
		this.pnlListaPrecios.Visible = false;
        this.pnlListaPrecios2.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
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
                         " FROM facturas_liq F" +
                         " INNER JOIN usuarios U" +
                         " ON U.ID = F.creadoPorID" +
                         " AND F.ID = " + this.hdID.Value + ";" +
                         "SELECT U.usuario, F.modificadoPorFecha" +
                         " FROM facturas_liq F" +
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

    private bool Factura_Timbrada()
    {
        string strQuery = "SELECT 1" +
                         " FROM facturas_liq" +
                         " WHERE ID = " + this.hdID.Value +
                         "   AND fecha_timbrado IS NOT null";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            Regresar_Continuar();
            ((master_MasterPage)Page.Master).MostrarMensajeError("La operación no puede ejecutarse porque la factura ha cambiado, vuelva a ingresar a ella");
            return true;
        }

        return false;
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
                                                            " FROM facturas_liq_prod" +
                                                            " WHERE factura_ID = " + this.hdID.Value);

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

                CComunDB.CCommun.Ejecutar_SP("UPDATE facturas_liq_prod SET" +
                                            " costo_unitario = " + dcmPrecio +
                                            ",costo = " + Math.Round(dcmCantidad * dcmPrecio, 2) +
                                            " WHERE factura_ID = " + this.hdID.Value +
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

    protected void gvListaPreciosProductos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((TextBox)e.Row.FindControl("txtCostoLista")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
            ((TextBox)e.Row.FindControl("txtCantidadLista")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
        }
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

    private DataTable ObtenerRemisiones()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("remisionID", typeof(string)));
        dt.Columns.Add(new DataColumn("remision", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("monto", typeof(string)));

        string strQuery = "SELECT C.ID" +
                         ",C.fecha" +
                         ",C.total" +
                         ",C.moneda" +
                         " FROM notas C" +
                         " INNER JOIN sucursales S" +
                         " ON S.ID = C.sucursal_ID" +
                         " AND C.status < 4" +
                         " AND S.establecimiento_ID = (" +
                         "      SELECT establecimiento_ID" +
                         "      FROM sucursales" +
                         "      WHERE ID = " + this.hdSucursalID.Value +
                         " )" +
                         " LEFT JOIN nota_facturas O" +
                         " ON O.nota_ID = C.ID" +
                         " WHERE O.nota_ID IS NULL" +
                         " ORDER BY ID";
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
            dr[0] = objRowResult["ID"].ToString();
            dr[1] = "Remisión " + objRowResult["ID"].ToString();
            dr[2] = CRutinas.Fecha_DD_MMM_YYYY((DateTime)objRowResult["fecha"]);
            dr[3] = ((decimal)objRowResult["total"]).ToString("c") + " " + objRowResult["moneda"].ToString();
            dt.Rows.Add(dr);
        }
        return dt;
    }

    private DataTable ObtenerProductosRemision()
    {
        DataTable dtProd = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dtProd.Columns.Add(new DataColumn("productoID", typeof(string)));
        dtProd.Columns.Add(new DataColumn("producto", typeof(string)));
        dtProd.Columns.Add(new DataColumn("cantidad", typeof(string)));
        dtProd.Columns.Add(new DataColumn("costo_unitario", typeof(string)));
        dtProd.Columns.Add(new DataColumn("costo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("enabled", typeof(bool)));
        dtProd.Columns.Add(new DataColumn("moneda", typeof(string)));
        dtProd.Columns.Add(new DataColumn("detalle", typeof(string)));
        dtProd.Columns.Add(new DataColumn("impDet", typeof(string)));
        dtProd.Columns.Add(new DataColumn("lote_fecha", typeof(string)));
        dtProd.Columns.Add(new DataColumn("minimo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("usar_unimed2", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupoID", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_consecutivo", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_cantidad", typeof(string)));
        dtProd.Columns.Add(new DataColumn("grupo_relacion", typeof(string)));

        string strQuery = "SELECT * FROM ( " +
                        "SELECT C.producto_ID as productoID " +
                        ", C.cantidad as cantidad " +
                        ", C.consecutivo as consecutivo " +
                        ", C.costo_unitario as costo_unitario " +
                        ", C.costo_original" +
                        ", C.costo_original_moneda" +
                        ", C.costo as costo " +
                        ", C.grupoID " +
                        ", C.grupo_cantidad " +
                        ", C.grupo_consecutivo" +
                        ", C.grupo_relacion" +
                        ", C.detalle" +
                        ", C.imprimir_detalle" +
                        ", C.usar_unimed2" +
                        ", CONCAT('(', P.codigo, ') ', LEFT(P.nombre, 80)) as producto " +
                        ", C.exento as exento " +
                        ", P.minimo_compra" +
                        ", CONCAT('(', G.codigo, ') ', LEFT(G.nombre, 80)) as producto_grupo " +
                        " FROM notas_prod C " +
                        " INNER JOIN productos P " +
                        " ON C.producto_ID = P.ID " +
                        " AND nota_ID = " + this.lblRemision.Text +
                        " LEFT JOIN productos G" +
                        " ON G.ID = C.grupoID" +
                        ") AS A ORDER BY consecutivo, producto" +
                        ";" +
                        "SELECT consecutivo, producto_ID, lote, fecha_caducidad" +
                        " FROM notas_prod_lote" +
                        " WHERE nota_ID = " + this.lblRemision.Text;
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        decimal dcmCantidad;
        decimal dcmCosto;
        decimal dcmPrecio;
        StringBuilder strLotes = new StringBuilder();
        string strGrupoConsecutivo = string.Empty;

        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
            {
                // Se agrega un registro en blanco con los datos del Kit
                if (!strGrupoConsecutivo.Equals(objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString()))
                {
                    dr = dtProd.NewRow();
                    dr[0] = objRowResult["productoID"].ToString();
                    dr[1] = objRowResult["producto_grupo"].ToString() + ", Cant.: " + ((decimal)objRowResult["grupo_cantidad"]).ToString("0.##");
                    dr[5] = false;
                    dr[12] = objRowResult["grupoID"].ToString();
                    dr[13] = objRowResult["grupo_consecutivo"].ToString();
                    dtProd.Rows.Add(dr);
                    strGrupoConsecutivo = objRowResult["grupoID"].ToString() + "_" + objRowResult["grupo_consecutivo"].ToString();
                }
            }

            dcmCantidad = (decimal)objRowResult["cantidad"];
            dcmPrecio = (decimal)objRowResult["costo_original"];

            dcmCosto = dcmCantidad * dcmPrecio;

            dr = dtProd.NewRow();
            dr[0] = objRowResult["productoID"].ToString();

            if (!string.IsNullOrEmpty(objRowResult["grupoID"].ToString()))
            {
                dr[1] = "&nbsp;&nbsp;&nbsp;";
            }

            dr[1] += objRowResult["producto"].ToString();

            dr[7] = string.Empty;
            if (!objRowResult.IsNull("detalle") && objRowResult["detalle"].ToString().Length > 0)
            {
                dr[7] = objRowResult["detalle"].ToString();
                if (objRowResult["detalle"].ToString().Length < 50)
                    dr[1] += "<br/>" + objRowResult["detalle"].ToString();
                else
                    dr[1] += "<br/>" + objRowResult["detalle"].ToString().Substring(0, 50);
            }

            dr[2] = dcmCantidad.ToString("0.##");
            dr[3] = objRowResult["costo_original"].ToString();
            dr[4] = dcmCosto.ToString("c");
            if (this.hdUsuPr.Value.Equals("0"))
                dr[5] = false;
            else
                dr[5] = true;
            dr[6] = objRowResult["costo_original_moneda"].ToString();

            dr[8] = (Convert.ToBoolean(objRowResult["imprimir_detalle"])).ToString();
            DataRow[] dtLotes = objDataResult.Tables[1].Select(
                                                   "consecutivo = " + objRowResult["consecutivo"].ToString());
            strLotes.Clear();
            foreach (DataRow dtLote in dtLotes)
            {
                if (strLotes.Length > 0)
                    strLotes.Append("^");
                strLotes.Append(dtLote["lote"].ToString() + "_");
                if (dtLote.IsNull("fecha_caducidad"))
                    strLotes.Append("null");
                else
                    strLotes.Append(((DateTime)dtLote["fecha_caducidad"]).ToString("yyyy-MM-dd"));
            }
            dr[9] = strLotes.ToString();

            if (objRowResult.IsNull("minimo_compra"))
                dr[10] = string.Empty;
            else
                dr[10] = ((decimal)objRowResult["minimo_compra"]).ToString("0.##");

            dr[11] = ((bool)objRowResult["usar_unimed2"] ? "1" : "0");
            dr[12] = objRowResult["grupoID"].ToString();
            dr[13] = objRowResult["grupo_consecutivo"].ToString();
            dr[14] = objRowResult["grupo_cantidad"].ToString();
            dr[15] = objRowResult["grupo_relacion"].ToString();

            dtProd.Rows.Add(dr);
        }

        return dtProd;
    }

    protected void btnRemision_Click(object sender, EventArgs e)
    {
        if (Factura_Timbrada())
            return;

        this.gvListadoRemision.DataSource = ObtenerRemisiones();
        this.gvListadoRemision.DataBind();

        if (gvListadoRemision.Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No hay remisiones que puedan usarse para este cliente");
            return;
        }

        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlRemisionListado.Visible = true;
        this.pnlRemision.Visible = false;
    }

    protected void btnRegresarRemision_Click(object sender, EventArgs e)
    {
        Mostrar_Datos();
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlRemision.Visible = false;
        this.pnlRemisionListado.Visible = false;
    }

    protected void gvProductosRemision_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!string.IsNullOrEmpty(((HiddenField)e.Row.FindControl("hdGrupoID")).Value) &&
                string.IsNullOrEmpty(((HiddenField)e.Row.FindControl("hdCantidad")).Value))
            {
                ((TextBox)e.Row.FindControl("txtCostoUnitarioRemision")).Attributes["style"] = "display:none;";
                ((TextBox)e.Row.FindControl("txtCantidadRemision")).Attributes["style"] = "display:none;";
            }
            else
            {
                ((TextBox)e.Row.FindControl("txtCostoUnitarioRemision")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
                ((TextBox)e.Row.FindControl("txtCantidadRemision")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
            }
        }
    }

    protected void chkSeleccionRemision_CheckedChanged(object sender, EventArgs e)
    {
        string strRemisionID = ((RadioButton)sender).Text.Substring(((RadioButton)sender).Text.LastIndexOf(" ") + 1);
        this.lblRemision.Text = strRemisionID;

        DataSet objDataResult = new DataSet();

        string strQuery = "SELECT 1" +
                         " FROM nota_facturas" +
                         " WHERE nota_ID = " + this.lblRemision.Text +
                         "   AND factura_ID = " + this.hdID.Value;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            ((RadioButton)sender).Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Remisión ya fue agregada a la factura");
            return;
        }

        strQuery = "SELECT moneda FROM notas WHERE ID = " + strRemisionID;
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        this.dlMoneda.SelectedValue = objDataResult.Tables[0].Rows[0]["moneda"].ToString();

        this.gvProductosRemision.DataSource = ObtenerProductosRemision();
        this.gvProductosRemision.DataBind();

        if (this.gvProductosRemision.Rows.Count == 0)
        {
            ((RadioButton)sender).Checked = false;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Remision no tiene productos");
            return;
        }

        foreach (GridViewRow gvRow in this.gvProductosRemision.Rows)
        {
            if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                continue;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco",
                                                    "setTimeout('setFoco(\"" +
                                                    ((TextBox)gvRow.FindControl("txtCantidadRemision")).ClientID +
                                                    "\")',50);",
                                                    true);
            break;
        }

        this.pnlRemision.Visible = true;
        this.pnlRemisionListado.Visible = false;
    }

    protected void btnProcesarRemision_Click(object sender, EventArgs e)
    {
        decimal dcmCantidad = 0;
        decimal dcmPrecioUnitario = 0;
        bool swSeleccionados = false;
        List<CKitVal> lstKits = new List<CKitVal>();

        foreach (GridViewRow gvRow in this.gvProductosRemision.Rows)
        {
            if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
            {
                // Si no esta lo agrega
                if (lstKits.Find(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                      x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value)) == null)
                {
                    lstKits.Add(new CKitVal(((HiddenField)gvRow.FindControl("hdGrupoID")).Value,
                                            ((HiddenField)gvRow.FindControl("hdGrupoCons")).Value,
                                            ((Label)gvRow.FindControl("lblProd")).Text)
                               );
                }

                // Si tiene cantidad la suma
                if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                {
                    List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                  x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                    lstUnKit[0].dcmSuma += decimal.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value);
                }
            }
        }

        foreach (GridViewRow gvRow in this.gvProductosRemision.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    continue;

                decimal.TryParse(((TextBox)gvRow.FindControl("txtCantidadRemision")).Text.Trim(), out dcmCantidad);
                decimal.TryParse(((TextBox)gvRow.FindControl("txtCostoUnitarioRemision")).Text.Trim(), out dcmPrecioUnitario);

                ((TextBox)gvRow.FindControl("txtCantidadRemision")).Text = Math.Round(dcmCantidad, 2).ToString();
                ((TextBox)gvRow.FindControl("txtCostoUnitarioRemision")).Text = Math.Round(dcmPrecioUnitario, 2).ToString();

                if (dcmCantidad > 0)
                    swSeleccionados = true;

                if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
                {
                    if (dcmCantidad > 0 &&
                        dcmCantidad != decimal.Parse(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    {
                        ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto " + ((Label)gvRow.FindControl("lblProd")).Text +
                                                                             " no puede modificarse porque es parte de un kit");
                        return;
                    }

                    List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                    x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                    lstUnKit[0].dcmSumaIngresada += dcmCantidad;
                }

                //if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdMinimo")).Value) && dcmCantidad > 0)
                //{
                //    if (dcmCantidad % decimal.Parse(((HiddenField)gvRow.FindControl("hdMinimo")).Value) != 0)
                //    {
                //        ((master_MasterPage)Page.Master).MostrarMensajeError("Cantidad del producto " + ((Label)gvRow.FindControl("lblProd")).Text +
                //                                                             " debe ser un múltiplo de " + ((HiddenField)gvRow.FindControl("hdMinimo")).Value);
                //        return;
                //    }
                //}
            }
        }

        if (!swSeleccionados)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese la cantidad de al menos un producto");
            return;
        }

        foreach (CKitVal kit in lstKits)
        {
            if (kit.dcmSumaIngresada > 0 &&
                kit.dcmSuma != kit.dcmSumaIngresada)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Tiene que ingresar la cantidad de todos los productos del kit " +
                                                                      kit.strNombre);
                return;
            }
        }

        StringBuilder strMensajeFinal = new StringBuilder();
        bool swAgregados = false;
        foreach (GridViewRow gvRow in this.gvProductosRemision.Rows)
        {
            if (gvRow.RowType == DataControlRowType.DataRow)
            {
                if (string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdCantidad")).Value))
                    continue;

                if (decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadRemision")).Text) > 0)
                {
                    string strMensaje = string.Empty;
                    this.hdMoneda.Value = ((Label)gvRow.FindControl("lblMoneda")).Text;

                    if (!string.IsNullOrEmpty(((HiddenField)gvRow.FindControl("hdGrupoID")).Value))
                    {
                        List<CKitVal> lstUnKit = lstKits.FindAll(x => x.strGrupoID.Equals(((HiddenField)gvRow.FindControl("hdGrupoID")).Value) &&
                                                                    x.strGrupoConsecutivo.Equals(((HiddenField)gvRow.FindControl("hdGrupoCons")).Value));

                        if (lstUnKit[0].btNuevoGrupoConsecutivo == 0)
                        {
                            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT IFNULL(MAX(grupo_consecutivo) + 1, 1) as consecutivo " +
                                                                                " FROM facturas_liq_prod " +
                                                                                " WHERE factura_ID = " + this.hdID.Value);
                            lstUnKit[0].btNuevoGrupoConsecutivo = byte.Parse(objDataResult.Tables[0].Rows[0]["consecutivo"].ToString());
                        }

                        if (Agregar_Producto(this.gvProductosRemision.DataKeys[gvRow.RowIndex].Value.ToString(),
                                            ((Label)gvRow.FindControl("lblProd")).Text,
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadRemision")).Text.Trim()),
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCostoUnitarioRemision")).Text.Trim()),
                                            false,
                                            ((HiddenField)gvRow.FindControl("hdDetalle")).Value,
                                            Convert.ToBoolean(((HiddenField)gvRow.FindControl("hdImpDetalle")).Value),
                                            ((HiddenField)gvRow.FindControl("hdLoteFecha")).Value,
                                            ((HiddenField)gvRow.FindControl("hdUsar_Unimed2")).Value,
                                            lstUnKit[0].strGrupoID,
                                            lstUnKit[0].btNuevoGrupoConsecutivo,
                                            decimal.Parse(((HiddenField)gvRow.FindControl("hdGrupoCantidad")).Value),
                                            decimal.Parse(((HiddenField)gvRow.FindControl("hdGrupoRelacion")).Value),
                                            out strMensaje))
                        {
                            swAgregados = true;
                        }
                        else
                        {
                            if (strMensajeFinal.Length > 0)
                                strMensajeFinal.Append("<br>");
                            strMensajeFinal.Append(strMensaje);
                        }
                    }
                    else
                    {
                        if (Agregar_Producto(this.gvProductosRemision.DataKeys[gvRow.RowIndex].Value.ToString(),
                                            ((Label)gvRow.FindControl("lblProd")).Text,
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCantidadRemision")).Text.Trim()),
                                            decimal.Parse(((TextBox)gvRow.FindControl("txtCostoUnitarioRemision")).Text.Trim()),
                                            false,
                                            ((HiddenField)gvRow.FindControl("hdDetalle")).Value,
                                            Convert.ToBoolean(((HiddenField)gvRow.FindControl("hdImpDetalle")).Value),
                                            ((HiddenField)gvRow.FindControl("hdLoteFecha")).Value,
                                            ((HiddenField)gvRow.FindControl("hdUsar_Unimed2")).Value,
                                            string.Empty,
                                            0,
                                            0,
                                            0,
                                            out strMensaje))
                        {
                            swAgregados = true;
                        }
                        else
                        {
                            if (strMensajeFinal.Length > 0)
                                strMensajeFinal.Append("<br>");
                            strMensajeFinal.Append(strMensaje);
                        }
                    }
                }
            }
        }

        this.hdMoneda.Value = CComunDB.CCommun.ObtenerMonedaListasPrecios(this.dlListaPrecios.SelectedValue);

        if (swAgregados)
        {
            string strQuery = "INSERT INTO nota_facturas (nota_ID, factura_ID)  " +
                             " VALUES('" + this.lblRemision.Text + "'" +
                             " , '" + this.hdID.Value + "'" +
                             " )" +
                             ";" +
                             "UPDATE notas SET" +
                             " status = 4" +
                             ",modificadoPorID = " + Session["SIANID"].ToString() +
                             ",modificadoPorFecha = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                             " WHERE ID = " + this.lblRemision.Text;
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
                ((master_MasterPage)Page.Master).MostrarMensajeError("Los productos fueron agregados");
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("Ningún producto fue agregado a la factura");
        else
            if (swAgregados)
            {
                Llenar_Productos(true);
                ((master_MasterPage)Page.Master).MostrarMensajeError("Se agregaron productos, pero los siguientes no: <br>" + strMensajeFinal.ToString());
            }
            else
                ((master_MasterPage)Page.Master).MostrarMensajeError("Los productos no se agregaron: <br>" + strMensajeFinal.ToString());

        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlRemisionListado.Visible = false;
        this.pnlRemision.Visible = false;
    }

    private void Generar_Movimiento_Contable_Diario()
    {
        decimal dcmTotal = 0;
        decimal dcmSubtotal = Math.Round(decimal.Parse(this.hdCosto.Value), 2);
        decimal dcmIVA = Math.Round(decimal.Parse(this.hdIVA.Value), 2);

        if (decimal.Parse(this.hdSubtotal.Value) > 0)
            dcmTotal = Math.Round(decimal.Parse(this.hdSubtotal.Value), 2);
        else
            dcmTotal = Math.Round(decimal.Parse(this.hdCosto.Value), 2);

        // Convierte a pesos
        if (!this.dlMoneda.SelectedValue.Equals("MXN"))
        {
            dcmSubtotal = Math.Round(dcmSubtotal * decimal.Parse(this.txtTipoCambio.Text), 2);
            dcmIVA = Math.Round(dcmIVA * decimal.Parse(this.txtTipoCambio.Text), 2);
            dcmTotal = Math.Round(dcmTotal * decimal.Parse(this.txtTipoCambio.Text), 2);
        }


        Contabilidad.Genera_Cuenta_Contable(Contabilidad.Cuenta.Ventas_por_Servicios,
                                            Contabilidad.Poliza.Poliza_Diario,
                                            DateTime.Today,
                                            Contabilidad.Documento.Factura,
                                            int.Parse(this.hdID.Value),
                                            "Ventas por servicio, factura " + this.lblFacturaID.Text + this.txtSerie.Text,
                                            0,
                                            dcmTotal);

        Contabilidad.Genera_Cuenta_Contable(Contabilidad.Cuenta.IVA_Por_Pagar,
                                            Contabilidad.Poliza.Poliza_Diario,
                                            DateTime.Today,
                                            Contabilidad.Documento.Factura,
                                            int.Parse(this.hdID.Value),
                                            "IVA Trasladado 16%, factura " + this.lblFacturaID.Text + this.txtSerie.Text,
                                            0,
                                            dcmIVA);

    }

    private void Generar_Movimiento_Contable_Ingreso(string strPagoID,
                                                     string strCID,
                                                     string strCliente)
    {
        decimal dcmPago = Math.Round(decimal.Parse(this.txtPago.Text), 2);

        // Convierte a pesos
        if (!this.dlMonedaPago.SelectedValue.Equals("MXN"))
        {
            dcmPago = Math.Round(dcmPago * decimal.Parse(this.txtTipoCambio.Text), 2);
        }

        string strCuentaBancaria = "0";

        if (this.dlCuentaBancaria.Items.Count > 0)
        {
            string[] strBancoIDs = this.dlCuentaBancaria.SelectedValue.Split('_');
            strCuentaBancaria = strBancoIDs[0];
        }

        // Cuenta bancaria
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT cuenta" +
                                                            " FROM cuentas_bancarias" +
                                                            " WHERE ID = " + strCuentaBancaria +
                                                            ";" +
                                                            "SELECT persona" +
                                                            " FROM usuarios" +
                                                            " WHERE ID = " + Session["SIANID"] +
                                                            ";" +
                                                            "SELECT factura" +
                                                            " FROM facturas_liq" +
                                                            " WHERE ID = " + this.hdID.Value);

        if (objDataResult.Tables[0].Rows.Count > 0)
            strCuentaBancaria = objDataResult.Tables[0].Rows[0]["cuenta"].ToString();
        else
            strCuentaBancaria = string.Empty;

        string strPersona = string.Empty;
        if (objDataResult.Tables[1].Rows.Count > 0)
            strPersona = new string(objDataResult.Tables[1].Rows[0]["persona"].ToString().Take(20).ToArray());

        string strUUID = objDataResult.Tables[2].Rows[0]["factura"].ToString();

        string strRegistro_IN = string.Format(Contabilidad.Registro_Ingresos_IN,
                                              "IN",                                     //0 - ingresos
                                              "04020",                                  //1 - IdDocumentoDe
                                              "42",                                     //2 - TipoDocumento
                                              strPagoID,                                //3 - Folio
                                              DateTime.Today,                           //4 - Fecha
                                              DateTime.Today.Year,                      //5 - Ejercicio
                                              DateTime.Today.Month.ToString("00"),      //6 - Periodo
                                              DateTime.Today,                           //7 - FechaAplicacion
                                              DateTime.Today.Year,                      //8 - EjercicioAp
                                              DateTime.Today.Month.ToString("00"),      //9 - PeriodoAp
                                              Session["SIANID"].ToString(),             //10 - CodigoPersona
                                              new string(strCliente.Take(200).ToArray()),   //11 - BeneficiarioPagador
                                              strCuentaBancaria,                        //12 - IdCuentaCheques
                                              "2",                                      //13 - NatBancaria
                                              "0",                                      //14 - Naturaleza
                                              "1",                                      //15 - CodigoMoneda
                                              "2",                                      //16 - CodigoMonedaTipoCambio
                                              "0.0",                                      //17 - TipoCambio
                                              dcmPago,                                  //18 - Total
                                              "F/" + this.lblFacturaID.Text + this.txtSerie.Text,                   //19 - Referencia
                                              "F/" + this.lblFacturaID.Text + this.txtSerie.Text,                   //20 - Concepto
                                              "0",                                      //21 - EsConciliado
                                              "1",                                      //22 - EsAsociado
                                              "0",                                      //23 - UsuAutorizaPresupuesto
                                              "0",                                      //24 - IdMovEdoCta
                                              "0",                                      //25 - IdEgresoOrigen
                                              DateTime.Today.Year,                      //26 - EjercicioPol
                                              DateTime.Today.Month.ToString("00"),      //27 - PeriodoPol
                                              "1",                                      //28 - TipoPol
                                              "0",                                      //29 - NumPol
                                              "0",                                      //30 - PosibilidadPago
                                              "0",                                      //31 - EsProyectado
                                              "0",                                      //32 - IdPoliza
                                              "201",                                    //33 - Origen
                                              "0",                                      //34 - IdDocumento
                                              "0",                                      //35 - EsAnticipo
                                              "0",                                      //36 - EsTraspasado
                                              "0",                                      //37 - PolizaAgrupada
                                              strPersona,                               //38 - UsuarioCrea
                                              strPersona,                               //39 - UsuarioModifica
                                              "0",                                      //40 - tieneCFD
                                              strUUID                                   //41 - Guid
                                              );

        Contabilidad.Genera_Cuenta_Contable(Contabilidad.Cuenta.Cuenta_Bancos,
                                            Contabilidad.Poliza.Poliza_Ingresos,
                                            DateTime.Today,
                                            Contabilidad.Documento.Factura,
                                            int.Parse(this.hdID.Value),
                                            strRegistro_IN,
                                            0,
                                            dcmPago);

        string strRegistro_CA = string.Format(Contabilidad.Registro_Ingresos_CA,
                                              "CA",                                     //0 - AsocCargosAbonos
                                              "4020",                                   //1 - IdDocumentoDe
                                              "42",                                     //2 - TipoDocumento
                                              strCuentaBancaria,                        //3 - IdCuentaCheques
                                              strPagoID,                                //4 - Folio
                                              "10",                                     //5 - CodigoConceptoPago
                                              string.Empty,                             //6 - SeriePago
                                              strPagoID,                                //7 - FolioPago
                                              dcmPago,                                  //8 - ImportePago
                                              DateTime.Today,                           //9 - FechaPago
                                              "Pago del Cliente",                       //10 - ConceptoPago
                                              "C" + strCID,                             //11 - CodigoClienteProveedor
                                              new string(strCliente.Take(100).ToArray()),    //12 - ClienteProveedor
                                              "0",                                      //13 - EsCFD
                                              "0.0",                                      //14 - SaldoCargoAbono
                                              "0",                                      //15 - EsDisponibleDoctoBancario
                                              DateTime.Today                            //16 - Fecha    
                                              );

        Contabilidad.Genera_Cuenta_Contable(Contabilidad.Cuenta.Cuenta_Bancos,
                                            Contabilidad.Poliza.Poliza_Ingresos,
                                            DateTime.Today,
                                            Contabilidad.Documento.Factura,
                                            int.Parse(this.hdID.Value),
                                            strRegistro_CA,
                                            0,
                                            dcmPago);
    }

    protected void gvCambiarLote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((TextBox)e.Row.FindControl("txtCantidadLote")).Attributes["onkeypress"] = "javascript:return isNumberDec(event, this, 2);";
        }
    }

    private bool Documento_Modificado()
    {
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT modificadoPorFecha" +
                                                            " FROM notas" +
                                                            " WHERE ID = " + this.hdID.Value);

        if (!objDataResult.Tables[0].Rows[0].IsNull("modificadoPorFecha"))
        {
            if ((DateTime)objDataResult.Tables[0].Rows[0]["modificadoPorFecha"] > Master.FechaAcceso)
            {
                Master.MostrarMensajeError("El documento ha sido modificado por alguna otra persona, ciérrelo y vuelva a intentar");
                return true;
            }
        }

        return false;
    }

    private bool Pedido_Modificado()
    {
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP("SELECT modificadoPorFecha" +
                                                            " FROM orden_compra" +
                                                            " WHERE ID = " + this.lblOrden_Compra.Text);

        if (!objDataResult.Tables[0].Rows[0].IsNull("modificadoPorFecha"))
        {
            if ((DateTime)objDataResult.Tables[0].Rows[0]["modificadoPorFecha"] > DateTime.Parse(this.hdFechaTemp.Value))
            {
                Master.MostrarMensajeError("El pedido ha sido modificado por alguna otra persona, cierre el pedido y vuelva a intentar");
                return true;
            }
        }

        return false;
    }
}
