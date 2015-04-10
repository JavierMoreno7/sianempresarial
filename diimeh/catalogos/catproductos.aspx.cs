using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using AjaxControlToolkit;

public partial class catalogos_catproductos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnAgregar.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnAgregar.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.btnCancelarImagen.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelarImagen.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.btnGuardarPrecio.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnGuardarPrecio.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnCancelarPrecio.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelarPrecio.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            this.hdUsuPr.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1105, out swVer, out swTot)) //Actualización precios
                this.hdUsuPr.Value = "1";
            else
            {
                this.btnAgregarPrecio.Visible = false;
            }

            this.hdUsuVen.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1100, out swVer, out swTot))    //Listas de precios de ventas
            {
                if (swTot)
                    this.hdUsuVen.Value = "2";
                else
                    this.hdUsuVen.Value = "1";
            }

            this.hdUsuCom.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1101, out swVer, out swTot))    //Listas de precios de compras
            {
                if (swTot)
                    this.hdUsuCom.Value = "2";
                else
                    this.hdUsuCom.Value = "1";
            }

            if (!CComunDB.CCommun.ValidarAcceso(1000, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");

            this.hdAT.Value = "1";
            if (!swTot)
            {
                this.lblAgregar.Enabled = false;
                this.hdAT.Value = "0";
            }

            if (this.hdUsuVen.Value.Equals("0") && this.hdUsuCom.Value.Equals("0"))
                this.btnHistorial.Visible = false;

            ViewState["SortCampo"] = "0";
            ViewState["CriterioCampo"] = "0";
            ViewState["Criterio"] = "";
            ViewState["SortOrden"] = 1;
            ViewState["PagActual"] = 1;
            Llenar_Grid();

            Llenar_Catalogos();

            this.txtClave.Enabled = false;

            this.hdID.Value = null;
        }
    }

    private void Llenar_Grid()
    {
        this.grdvLista.DataSource = ObtenerDatos();
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

    private DataTable ObtenerDatos()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("familia", typeof(string)));
        dt.Columns.Add(new DataColumn("clase", typeof(string)));
        dt.Columns.Add(new DataColumn("linea", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("clave", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("existencia", typeof(string)));
        dt.Columns.Add(new DataColumn("iva", typeof(string)));
        dt.Columns.Add(new DataColumn("unidad", typeof(string)));
        dt.Columns.Add(new DataColumn("sales", typeof(string)));
        dt.Columns.Add(new DataColumn("inforef", typeof(string)));

        string strQuery = "CALL leer_productos_consulta(" +
            ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30, 0)";
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
            dr[1] = objRowResult["nombre"].ToString();
            dr[2] = objRowResult["familia"].ToString();
            dr[3] = objRowResult["clase"].ToString();
            dr[4] = objRowResult["linea"].ToString();
            dr[5] = objRowResult["codigo"].ToString();
            dr[6] = objRowResult["clave"].ToString();
            dr[7] = string.Empty;
            if (!this.hdUsuVen.Value.Equals("0"))
            {
                if (!objRowResult.IsNull("desde"))
                    dr[7] = ((decimal)objRowResult["precio"]).ToString("c") + "   -   " +
                            ((DateTime)objRowResult["desde"]).ToString("dd/MM/yy");
                if ((bool)objRowResult["desclim"])
                    dr[7] += "(DL)";
                if ((bool)objRowResult["neto"])
                    dr[7] += "(N)";
            }
            dr[8] = ((decimal)objRowResult["existencia"]).ToString("0.##");
            if ((bool)objRowResult["exento"])
                dr[9] = "NO";
            else
                dr[9] = "SÍ";
            dr[10] = objRowResult["unimed"].ToString();
            dr[11] = objRowResult["sales"].ToString();
            dr[12] = dr[0] +
                    "~" + (this.hdUsuVen.Value.Equals("0") ? "0" : "1") +
                    "~" + (this.hdUsuCom.Value.Equals("0") ? "0" : "1") +
                    "~" + dr[1] +
                    "~" + objRowResult["imagenprincipal"].ToString() +
                    "~" + dr[5] +
                    "~0" +
                    "~" + dr[11];

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

    protected void grdvLista_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((System.Web.UI.WebControls.Image)e.Row.Cells[8].Controls[1]).Attributes["onmouseover"] =
                                             "javascript:mostrarInfo('" +
                                             ((HiddenField)e.Row.Cells[8].Controls[3]).Value +
                                             "')";
            ((System.Web.UI.WebControls.Image)e.Row.Cells[8].Controls[1]).Attributes["onmouseout"] =
                                             "javascript:esconderInfo();";
        }
    }

    protected void grdvLista_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Modificar")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            if (this.lblMostrar.Visible)
                this.hdPos.Value = index.ToString();
            else
                this.hdPos.Value = string.Empty;
            Mostrar_Datos();
            if (!this.hdAT.Value.Equals("1"))
            {
                this.btnModificar.Visible = false;
                this.btnAgregarPrecio.Visible = false;
                this.btnAgregarImagen.Visible = false;
                this.btnHistorial.Visible = false;
            }
        }
    }

    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {
        string strMensaje = string.Empty;
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()))
        {
            if (string.IsNullOrEmpty(strMensaje))
            {
                StringBuilder strCriterio = new StringBuilder();
                switch (this.dlBusqueda.SelectedValue)
                {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "6":
                    case "7":
                    case "8":
                        strCriterio.Append("%");
                        strCriterio.Append(this.txtCriterio.Text.Trim().Replace("'", "''"));
                        strCriterio.Append("%");
                        break;
                    case "4":
                    case "5":
                        strCriterio.Append(this.txtCriterio.Text.Trim().Replace("'", "''"));
                        break;
                }
                ViewState["SortCampo"] = "0";
                ViewState["SortOrden"] = 1;
                ViewState["CriterioCampo"] = this.dlBusqueda.SelectedValue;
                ViewState["Criterio"] = strCriterio.ToString();
                ViewState["PagActual"] = 1;
                Llenar_Grid();
                this.lblMostrar.Visible = true;
            }
        }
        else
            strMensaje = "Ingrese el criterio de búsqueda";

        if (!string.IsNullOrEmpty(strMensaje))
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
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
        this.txtMinimo.Text = string.Empty;
        this.txtMaximo.Text = string.Empty;
        this.txtReorden.Text = string.Empty;
        this.dlFamilia.ClearSelection();
        this.dlClase.ClearSelection();
        this.hdTempImg.Value = string.Empty;
        this.hdTempID2.Value = "0";

        this.btnNextProd.Visible = false;
        this.btnPrevProd.Visible = false;

        if (this.hdID.Value.Equals("0"))
        {
            this.txtNombre.Text = string.Empty;
            this.lblNombre.Text = string.Empty;
            this.rdExento.ClearSelection();
            this.rdExento.SelectedIndex = 0;
            this.txtSales.Text = string.Empty;
            this.txtDescripcion.Text = string.Empty;
            this.txtClave.Text = string.Empty;
            this.txtCodigo.Text = string.Empty;
            this.txtCodigo2.Text = string.Empty;
            this.txtCodigo3.Text = string.Empty;
            this.txtUbicacion.Text = string.Empty;
            //this.txtBultoOriginal.Text = string.Empty;
            //this.txtPiezasPorCaja.Text = string.Empty;
            this.txtClave_Gobierno.Text = string.Empty;
            this.txtUnidadDeMedida.Text = string.Empty;
            this.btnAgregar.Visible = true;
            this.btnModificar.Visible = false;
            if (this.dlFamilia.Items.Count > 0)
                this.dlFamilia.SelectedIndex = 0;
            if (this.dlClase.Items.Count > 0)
                this.dlClase.SelectedIndex = 0;
            this.gvPreciosVenta.DataSource = null;
            this.gvPreciosVenta.DataBind();
            this.gvPreciosCompra.DataSource = null;
            this.gvPreciosCompra.DataBind();
            this.dlImagenes.DataSource = null;
            this.dlImagenes.DataBind();
            this.btnAgregarImagen.Enabled = true;
            this.btnAgregarPrecio.Enabled = true;
            this.chkLimitado.Checked = this.chkNeto.Checked = false;
            this.chkLote.Checked = true;
            this.chkCaducidad.Checked = true;
            this.chkActivo.Checked = true;
            this.hdActivo.Value = this.chkActivo.Checked.ToString();
            ViewState["imagenes"] = string.Empty;
            int intSuma = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day +
                            DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            HttpCookie ckSIAN = Request.Cookies["userCng"];
            this.hdTempImg.Value = "tmp_" + (int.Parse(ckSIAN["ck_cliente"])).ToString("000") + "_" +
                                  (int.Parse(this.hdID.Value)).ToString("000000") + "_"
                                  + intSuma.ToString();
            Random rndValor = new Random();
            this.hdTempID2.Value = rndValor.Next(0, int.MaxValue).ToString();
        }
        else
        {
            if (!string.IsNullOrEmpty(this.hdPos.Value))
            {
                this.btnNextProd.Visible = true;
                this.btnPrevProd.Visible = true;
            }
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT nombre, exento, sales, clave, codigo, " +
                    "codigo2, codigo3, desclim, neto," +
                    "descripcion, ubicacion, clave_gobierno, " +
                    "familia_ID, clase_ID, linea_ID, " +
                    "bultooriginal, piezasporcaja, unimed, imagen, imagenprincipal " +
                    ", lote, caducidad, activo " +
                    ", minimo, maximo, reorden " +
                    " FROM productos P " +
                    " LEFT JOIN producto_limites L" +
                    " ON L.productoID = P.ID " +
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

            this.txtNombre.Text = objRowResult["nombre"].ToString();
            this.lblNombre.Text = objRowResult["nombre"].ToString();
            this.rdExento.ClearSelection();
            this.rdExento.Items.FindByValue(Convert.ToBoolean(objRowResult["exento"]).ToString()).Selected = true;
            this.txtSales.Text = objRowResult["sales"].ToString();
            this.txtDescripcion.Text = objRowResult["descripcion"].ToString();
            this.txtClave.Text = objRowResult["clave"].ToString();
            this.txtCodigo.Text = objRowResult["codigo"].ToString();
            this.txtCodigo2.Text = objRowResult["codigo2"].ToString();
            this.txtCodigo3.Text = objRowResult["codigo3"].ToString();
            this.txtUbicacion.Text = objRowResult["ubicacion"].ToString();
            //if ((double)objRowResult["bultooriginal"] != 0)
            //    this.txtBultoOriginal.Text = objRowResult["bultooriginal"].ToString();
            //else
            //    this.txtBultoOriginal.Text = string.Empty;
            //this.txtPiezasPorCaja.Text = objRowResult["piezasporcaja"].ToString();
            this.txtClave_Gobierno.Text = objRowResult["clave_gobierno"].ToString();
            this.txtUnidadDeMedida.Text = objRowResult["unimed"].ToString();
            this.chkLimitado.Checked = Convert.ToBoolean(objRowResult["desclim"]);
            this.chkNeto.Checked = Convert.ToBoolean(objRowResult["neto"]);
            this.chkLote.Checked = Convert.ToBoolean(objRowResult["lote"]);
            this.chkCaducidad.Checked = Convert.ToBoolean(objRowResult["caducidad"]);

            this.chkActivo.Checked = (bool)objRowResult["activo"];
            this.hdActivo.Value = this.chkActivo.Checked.ToString();

            ViewState["imagenes"] = objRowResult["imagen"].ToString();
            ViewState["imagenprincipal"] = objRowResult["imagenprincipal"].ToString();

            if (!objRowResult["familia_ID"].ToString().Equals("0"))
                this.dlFamilia.Items.FindByValue(objRowResult["familia_ID"].ToString()).Selected = true;

            Llenar_Clases();

            if (!objRowResult["clase_ID"].ToString().Equals("0"))
            {
                try
                {
                    this.dlClase.ClearSelection();
                    this.dlClase.Items.FindByValue(objRowResult["clase_ID"].ToString()).Selected = true;
                }
                catch
                {
                }
            }

            if (!objRowResult.IsNull("minimo"))
                this.txtMinimo.Text = ((decimal)objRowResult["minimo"]).ToString("#.##");
            if (!objRowResult.IsNull("maximo"))
                this.txtMaximo.Text = ((decimal)objRowResult["maximo"]).ToString("#.##");
            if (!objRowResult.IsNull("reorden"))
                this.txtReorden.Text = ((decimal)objRowResult["reorden"]).ToString("#.##");

            Llenar_PreciosVenta();
            Llenar_PreciosCompra();
            Llenar_Imagenes();

            this.btnAgregar.Visible = false;
            this.btnModificar.Visible = true;

            this.btnAgregarImagen.Enabled = true;
            this.btnAgregarPrecio.Enabled = true;
        }
        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
    }

    private void Llenar_Catalogos()
    {
        this.dlFamilia.DataSource = CComunDB.CCommun.ObtenerFamilias(false);
        this.dlFamilia.DataBind();

        Llenar_Clases();

        if (this.hdUsuVen.Value.Equals("2") && this.hdUsuCom.Value.Equals("2"))
        {
            this.dlListaPrecio.DataSource = CComunDB.CCommun.ObtenerListasPrecios(string.Empty);
        }
        else
            if (this.hdUsuVen.Value.Equals("2"))
            {
                this.dlListaPrecio.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
            }
            else
                if (this.hdUsuCom.Value.Equals("2"))
                {
                    this.dlListaPrecio.DataSource = CComunDB.CCommun.ObtenerListasPrecios("COMPRAS");
                }
                else
                {
                    this.dlListaPrecio.DataSource = null;
                    this.btnAgregarPrecio.Visible = false;
                }

        this.dlListaPrecio.DataBind();

        if (dlListaPrecio.Items.Count == 0)
            this.btnAgregarPrecio.Enabled = false;
        else
            if (ConfigurationManager.AppSettings["clientes"].Equals("5"))
            {
                ListItem liDiimeh = this.dlListaPrecio.Items.FindByValue("9");
                if (liDiimeh != null)
                {
                    this.dlListaPrecio.ClearSelection();
                    liDiimeh.Selected = true;
                }
            }
    }

    private void Llenar_Clases()
    {
        if (this.dlFamilia.Items.Count == 0)
        {
            this.dlClase.Items.Clear();
            this.dlClase.DataSource = null;
        }
        else
            this.dlClase.DataSource = CComunDB.CCommun.ObtenerClases(this.dlFamilia.SelectedValue, false);
        this.dlClase.DataBind();

    }

    private void Llenar_PreciosVenta()
    {
        if (!this.hdUsuVen.Value.Equals("0"))
        {
            this.gvPreciosVenta.DataSource = ObtenerDatosVenta();
            this.gvPreciosVenta.DataBind();
        }
    }

    ICollection ObtenerDatosVenta()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();
        //double dblPiezasPorCaja = 1;

        //if (!double.TryParse(this.txtPiezasPorCaja.Text.Trim(), out dblPiezasPorCaja))
        //dblPiezasPorCaja = 1;

        dt.Columns.Add(new DataColumn("listapreciosID", typeof(string)));
        dt.Columns.Add(new DataColumn("nombrelista", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("preciosugerido", typeof(string)));
        dt.Columns.Add(new DataColumn("preciounitario", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));

        string strQuery;
        if (this.hdID.Value.Equals("0"))
            strQuery = "SELECT P.lista_precios_ID as listapreciosID, " +
                      " P.precio_caja as precio, " +
                      " P.precio_unitario as preciounitario, " +
                      " P.precio_sugerido as preciosugerido, " +
                      " P.validez_desde as validez_desde, " +
                      " L.nombre_lista as nombrelista " +
                      " FROM precios_temp P " +
                      " INNER JOIN listas_precios L" +
                      " ON P.lista_precios_ID = L.ID " +
                      " WHERE L.tipo_lista='VENTAS' AND " +
                      " P.producto_ID = " + this.hdTempID2.Value +
                      " AND P.validez='2099/12/31'";
        else
            strQuery = "SELECT P.lista_precios_ID as listapreciosID, " +
                      " P.precio_caja as precio, " +
                      " P.precio_unitario as preciounitario, " +
                      " P.precio_sugerido as preciosugerido, " +
                      " P.validez_desde as validez_desde, " +
                      " L.nombre_lista as nombrelista " +
                      " FROM precios P " +
                      " INNER JOIN listas_precios L" +
                      " ON P.lista_precios_ID = L.ID " +
                      " WHERE L.tipo_lista='VENTAS' AND " +
                      " P.producto_ID = " + this.hdID.Value +
                      " AND P.validez='2099/12/31'";

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
            dr[0] = objRowResult["listapreciosID"].ToString();
            dr[1] = objRowResult["nombrelista"].ToString();
            dr[2] = ((decimal)objRowResult["precio"]).ToString("c");
            dr[3] = ((decimal)objRowResult["preciosugerido"]).ToString("c");
            dr[4] = ((decimal)objRowResult["preciounitario"]).ToString("c");
            dr[5] = ((DateTime)objRowResult["validez_desde"]).ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper().Replace(".", "");

            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        return dv;
    }

    private void Llenar_PreciosCompra()
    {
        if (!this.hdUsuCom.Value.Equals("0"))
        {
            this.gvPreciosCompra.DataSource = ObtenerDatosCompra();
            this.gvPreciosCompra.DataBind();
        }
    }

    ICollection ObtenerDatosCompra()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();
        //double dblPiezasPorCaja = 1;

        //if (!double.TryParse(this.txtPiezasPorCaja.Text.Trim(), out dblPiezasPorCaja))
        //    dblPiezasPorCaja = 1;

        dt.Columns.Add(new DataColumn("listapreciosID", typeof(string)));
        dt.Columns.Add(new DataColumn("nombrelista", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("preciosugerido", typeof(string)));
        dt.Columns.Add(new DataColumn("preciounitario", typeof(string)));
        dt.Columns.Add(new DataColumn("fecha", typeof(string)));
        dt.Columns.Add(new DataColumn("paq", typeof(string)));

        string strQuery;
        if (this.hdID.Value.Equals("0"))
            strQuery = "SELECT P.lista_precios_ID as listapreciosID, " +
                      " P.precio_caja as precio, " +
                      " P.precio_sugerido as preciosugerido, " +
                      " P.precio_unitario as preciounitario, " +
                      " P.validez_desde as validez_desde, " +
                      " L.nombre_lista as nombrelista " +
                      " FROM precios_temp P" +
                      " INNER JOIN listas_precios L " +
                      " ON P.lista_precios_ID = L.ID " +
                      " WHERE L.tipo_lista='COMPRAS' AND " +
                      " P.producto_ID = " + this.hdTempID2.Value +
                      " AND P.validez='2099/12/31'";
        else
            strQuery = "SELECT P.lista_precios_ID as listapreciosID, " +
                      " P.precio_caja as precio, " +
                      " P.precio_sugerido as preciosugerido, " +
                      " P.precio_unitario as preciounitario, " +
                      " P.validez_desde as validez_desde, " +
                      " L.nombre_lista as nombrelista " +
                      " FROM precios P" +
                      " INNER JOIN listas_precios L " +
                      " ON P.lista_precios_ID = L.ID " +
                      " WHERE L.tipo_lista='COMPRAS' AND " +
                      " P.producto_ID = " + this.hdID.Value +
                      " AND P.validez='2099/12/31'";
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
            dr[0] = objRowResult["listapreciosID"].ToString();
            dr[1] = objRowResult["nombrelista"].ToString();
            dr[2] = ((decimal)objRowResult["precio"]).ToString("c");
            dr[3] = ((decimal)objRowResult["preciosugerido"]).ToString("c");
            dr[4] = ((decimal)objRowResult["preciounitario"]).ToString("c");
            dr[5] = ((DateTime)objRowResult["validez_desde"]).ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper().Replace(".", "");
            strQuery = "SELECT DISTINCT V.cobra_paqueteria" +
                      " FROM listas_precios L" +
                      " LEFT JOIN proveedores V" +
                      " ON V.lista_precios_ID = L.ID" +
                      " WHERE L.ID = " + objRowResult["listapreciosID"].ToString() +
                      " AND cobra_paqueteria = 1";
            DataSet objDataResult2 = CComunDB.CCommun.Ejecutar_SP(strQuery);
            if (objDataResult2.Tables[0].Rows.Count > 0)
                dr[6] = "Sí";
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        return dv;
    }

    ICollection ObtenerPreciosVenta()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();
        //double dblPiezasPorCaja = 1;

        //if (!double.TryParse(this.txtPiezasPorCaja.Text.Trim(), out dblPiezasPorCaja))
        //    dblPiezasPorCaja = 1;

        dt.Columns.Add(new DataColumn("listapreciosID", typeof(string)));
        dt.Columns.Add(new DataColumn("nombrelista", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("precio_nuevo", typeof(string)));

        string strQuery;
        if (this.hdID.Value.Equals("0"))
            strQuery = "SELECT lista_precios_ID, nombre_lista, precio_caja " +
                      " FROM precios_temp P " +
                      " INNER JOIN listas_precios L " +
                      " ON P.lista_precios_ID = L.ID " +
                      " AND P.producto_ID = " + this.hdTempID2.Value +
                      " AND P.validez='2099/12/31'" +
                      " AND L.tipo_lista = 'VENTAS'";
        else
            strQuery = "SELECT lista_precios_ID, nombre_lista, precio_caja " +
                      " FROM precios P " +
                      " INNER JOIN listas_precios L " +
                      " ON P.lista_precios_ID = L.ID " +
                      " AND P.producto_ID = " + this.hdID.Value +
                      " AND P.validez='2099/12/31'" +
                      " AND L.tipo_lista = 'VENTAS'";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        decimal dcmPorcentaje = decimal.Parse(this.hdPorcentajeAumento.Value);
        decimal dcmPrecioNuevo = 0;
        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            dr = dt.NewRow();
            dr[0] = objRowResult["lista_precios_ID"].ToString();
            dr[1] = objRowResult["nombre_lista"].ToString();
            dr[2] = ((decimal)objRowResult["precio_caja"]).ToString("c");
            dcmPrecioNuevo = (decimal)objRowResult["precio_caja"];
            dcmPrecioNuevo = Math.Round(dcmPrecioNuevo * dcmPorcentaje, 2);
            dr[3] = dcmPrecioNuevo.ToString("0.00");
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        return dv;
    }

    private void Llenar_Imagenes()
    {
        if (string.IsNullOrEmpty(ViewState["imagenes"].ToString()))
            this.dlImagenes.DataSource = null;
        else
            this.dlImagenes.DataSource = ObtenerImagenes();
        this.dlImagenes.DataBind();
    }

    ICollection ObtenerImagenes()
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("imagenArchivo", typeof(string)));
        dt.Columns.Add(new DataColumn("imagenWidth", typeof(Unit)));
        dt.Columns.Add(new DataColumn("imagenHeight", typeof(Unit)));

        dt.Columns.Add(new DataColumn("imagenPrincipalTexto", typeof(string)));
        dt.Columns.Add(new DataColumn("imagenPrincipalEnabled", typeof(bool)));

        string[] strArchivos = ViewState["imagenes"].ToString().Split(';');

        const int max_pixeles = 70;
        int newWidth = 0;
        int newHeigth = 0;

        foreach (string strArchivo in strArchivos)
        {
            dr = dt.NewRow();
            dr[0] = strArchivo;
            int intWidth = int.Parse(strArchivo.Substring(strArchivo.IndexOf("-") + 1,
                                         strArchivo.IndexOf("x") - strArchivo.IndexOf("-") - 1));
            int intHeight = int.Parse(strArchivo.Substring(strArchivo.IndexOf("x") + 1,
                                         strArchivo.IndexOf(".") - strArchivo.IndexOf("x") - 1));
            if (intWidth > max_pixeles || intHeight > max_pixeles)
            {
                double relacion = 0;
                if (intWidth > intHeight)
                    relacion = max_pixeles / (double)intWidth;
                else
                    relacion = max_pixeles / (double)intHeight;
                newWidth = (int)(intWidth * relacion);
                newHeigth = (int)(intHeight * relacion);
            }
            else
            {
                newWidth = intWidth;
                newHeigth = intHeight;
            }
            dr[1] = new Unit(newWidth);
            dr[2] = new Unit(newHeigth);

            if (strArchivo.Equals(ViewState["imagenprincipal"].ToString()))
            {
                dr[3] = "Imagen Principal";
                dr[4] = false;
            }
            else
            {
                dr[3] = "Hacer Principal";
                dr[4] = true;
            }

            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        return dv;
    }

    private DataSet ObtenerHistorial()
    {
        DataTable dtListas = new DataTable("tblListas");
        DataTable dtPrecios = new DataTable("tblPrecios");
        DataRow dr;

        dtListas.Columns.Add(new DataColumn("nombrelista", typeof(string)));

        dtPrecios.Columns.Add(new DataColumn("nombrelista", typeof(string)));
        dtPrecios.Columns.Add(new DataColumn("precio", typeof(string)));
        dtPrecios.Columns.Add(new DataColumn("vigencia", typeof(string)));

        DataSet objDataResult = new DataSet();
        string strQuery = "SELECT P.precio_caja as precio, " +
                    " P.validez as validez, " +
                    " L.nombre_lista as nombrelista, U.usuario, P.creadoPorFecha " +
                    " FROM precios P " +
                    " INNER JOIN listas_precios L" +
                    " ON P.lista_precios_ID = L.ID " +
                    " LEFT JOIN usuarios U" +
                    " ON U.ID = P.creadoPorID" +
                    " WHERE P.producto_ID = " + this.hdID.Value;

        if (this.hdUsuCom.Value.Equals("0"))
            strQuery += " AND L.tipo_lista = 'VENTAS'";
        else
            if (this.hdUsuVen.Value.Equals("0"))
                strQuery += " AND L.tipo_lista = 'COMPRAS'";

        strQuery += " ORDER BY L.nombre_lista, P.validez DESC";

        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        string strNombreListaAnt = string.Empty;


        foreach (DataRow objRowResult in objDataResult.Tables[0].Rows)
        {
            if (!strNombreListaAnt.Equals(objRowResult["nombrelista"].ToString()))
            {
                dr = dtListas.NewRow();
                dr[0] = objRowResult["nombrelista"].ToString();
                dtListas.Rows.Add(dr);
                strNombreListaAnt = objRowResult["nombrelista"].ToString();
            }
            dr = dtPrecios.NewRow();
            dr[0] = objRowResult["nombrelista"].ToString();
            dr[1] = ((decimal)objRowResult["precio"]).ToString("c");
            if (((DateTime)objRowResult["validez"]).Equals(new DateTime(2099, 12, 31)))
                dr[2] = "Precio Actual";
            else
                dr[2] = ((DateTime)objRowResult["validez"]).ToString("dd/MMM/yyyy");
            if (!objRowResult.IsNull("usuario"))
                dr[2] += " (" + objRowResult["usuario"].ToString() + ")";

            dtPrecios.Rows.Add(dr);
        }

        DataSet ds = new DataSet();
        ds.Tables.Add(dtListas);
        ds.Tables.Add(dtPrecios);
        DataRelation oRel = new DataRelation("Lista_Precios", ds.Tables[0].Columns["nombrelista"],
                                                        ds.Tables[1].Columns["nombrelista"]);
        ds.Relations.Add(oRel);
        return ds;
    }

    protected void btnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        string strQuery = "SELECT MAX(clave) as clave " +
                         " FROM productos ";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        int intClave = int.Parse(objDataResult.Tables[0].Rows[0]["clave"].ToString());
        intClave++;

        this.txtClave.Text = intClave.ToString();

        if (string.IsNullOrEmpty(this.txtCodigo.Text.Trim()))
            this.txtCodigo.Text = this.txtClave.Text;

        if (!Validar_Codigo())
            return;

        if (!string.IsNullOrEmpty(this.txtCodigo3.Text.Trim()))
        {
            strQuery = "SELECT 1 " +
                " FROM productos " +
                " WHERE codigo = '" + this.txtCodigo3.Text.Trim() + "'" +
                "    OR codigo2 = '" + this.txtCodigo3.Text.Trim() + "'" +
                "    OR codigo3 = '" + this.txtCodigo3.Text.Trim() + "'";
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Código 3 ya existe para otro artículo");
                return;
            }
        }

        Guardar_Producto();
    }

    private bool Validar_Codigo()
    {
        if (!string.IsNullOrEmpty(this.txtCodigo2.Text.Trim()) &&
            (this.txtCodigo2.Text.Trim().Equals(this.txtCodigo.Text.Trim()) ||
             this.txtCodigo2.Text.Trim().Equals(this.txtCodigo3.Text.Trim())))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Códigos no pueden ser iguales");
            return false;
        }

        if (!string.IsNullOrEmpty(this.txtCodigo3.Text.Trim()) &&
            this.txtCodigo3.Text.Trim().Equals(this.txtCodigo.Text.Trim()))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Códigos no pueden ser iguales");
            return false;
        }

        string strQuery = "SELECT 1 " +
                         " FROM productos " +
                         " WHERE (codigo = '" + this.txtCodigo.Text.Trim() + "'" +
                         "        OR codigo2 = '" + this.txtCodigo.Text.Trim() + "'" +
                         "        OR codigo3 = '" + this.txtCodigo.Text.Trim() + "')" +
                         "   AND ID <> " + this.hdID.Value;
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código 1 ya existe para otro artículo");
            return false;
        }

        if (!string.IsNullOrEmpty(this.txtCodigo2.Text.Trim()))
        {
            strQuery = "SELECT 1 " +
                      " FROM productos " +
                      " WHERE (codigo = '" + this.txtCodigo2.Text.Trim() + "'" +
                      "        OR codigo2 = '" + this.txtCodigo2.Text.Trim() + "'" +
                      "        OR codigo3 = '" + this.txtCodigo2.Text.Trim() + "')" +
                      "   AND ID <> " + this.hdID.Value;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Código 2 ya existe para otro artículo");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(this.txtCodigo3.Text.Trim()))
        {
            strQuery = "SELECT 1 " +
                      " FROM productos " +
                      " WHERE (codigo = '" + this.txtCodigo3.Text.Trim() + "'" +
                      "        OR codigo2 = '" + this.txtCodigo3.Text.Trim() + "'" +
                      "        OR codigo3 = '" + this.txtCodigo3.Text.Trim() + "')" +
                      "   AND ID <> " + this.hdID.Value;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Código 3 ya existe para otro artículo");
                return false;
            }
        }

        return true;
    }

    private void Guardar_Producto()
    {
        int? intMinimo, intMaximo, intReorden;
        intMinimo = intMaximo = intReorden = null;
        int intValor;
        if (!string.IsNullOrEmpty(this.txtMinimo.Text.Trim()))
        {
            if (!int.TryParse(this.txtMinimo.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Mínimo debe ser numérico y entero");
                return;
            }
            if (intValor < 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Mínimo no puede ser menor a cero");
                return;
            }
            intMinimo = intValor;
        }

        if (!string.IsNullOrEmpty(this.txtMaximo.Text.Trim()))
        {
            if (!int.TryParse(this.txtMaximo.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Máximo debe ser numérico y entero");
                return;
            }
            if (intValor < 0 || (intMinimo.HasValue && intValor < intMinimo))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Máximo no puede ser menor a cero o menor al mínimo");
                return;
            }
            intMaximo = intValor;
        }

        if (!string.IsNullOrEmpty(this.txtReorden.Text.Trim()))
        {
            if (!int.TryParse(this.txtReorden.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Punto de reorden debe ser numérico y entero");
                return;
            }
            if (intValor < 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Punto de reorden no puede ser menor a cero");
                return;
            }
            intReorden = intValor;
        }

        string strFamiliaID = "0";
        string strClaseID = "0";

        if (this.dlFamilia.Items.Count > 0)
            strFamiliaID = this.dlFamilia.SelectedValue;

        if (this.dlClase.Items.Count > 0)
            strClaseID = this.dlClase.SelectedValue;

        string strBultoOriginal = "0";
        //if (!string.IsNullOrEmpty(this.txtBultoOriginal.Text))
        //    strBultoOriginal = this.txtBultoOriginal.Text;

        string strQuery = "INSERT INTO productos (nombre, tipo, exento, sales, descripcion, " +
                    "clave, codigo, codigo2, codigo3, ubicacion, familia_ID, clase_ID, " +
                    "bultooriginal, piezasporcaja, unimed, " +
                    "lote, caducidad, desclim, neto," +
                    "clave_gobierno, activo) VALUES (" +
                    "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ", 0" +
                    ", '" + (Convert.ToBoolean(this.rdExento.SelectedValue) ? "1" : "0") + "'" +
                    ", '" + this.txtSales.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtClave.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtCodigo.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtCodigo2.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtCodigo3.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtUbicacion.Text.Trim().Replace("'", "''") + "'" +
                    ", " + strFamiliaID +
                    ", " + strClaseID +
                    ", " + strBultoOriginal +
                    ", 1" + //this.txtPiezasPorCaja.Text.Trim() +
                    ", '" + this.txtUnidadDeMedida.Text.Trim() + "'" +
                    ", " + (this.chkLote.Checked ? "1" : "0") +
                    ", " + (this.chkCaducidad.Checked ? "1" : "0") +
                    ", " + (this.chkLimitado.Checked ? "1" : "0") +
                    ", " + (this.chkNeto.Checked ? "1" : "0") +
                    ", '" + this.txtClave_Gobierno.Text.Trim().Replace("'", "''") + "'" +
                    ", " + (this.chkActivo.Checked ? "1" : "0") +
                    ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (Exception ex)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError(strQuery + " " + ex.Message);
        }

        DataSet objDataResult = new DataSet();
        strQuery = "SELECT ID FROM productos " +
                   " WHERE clave = '" + this.txtClave.Text.Trim().Replace("'", "''") + "'";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (intMaximo.HasValue || intMinimo.HasValue || intReorden.HasValue)
        {
            strQuery = "INSERT INTO producto_limites (productoID, minimo, " +
                       "maximo, reorden) VALUES(" +
                       objDataResult.Tables[0].Rows[0]["ID"].ToString() +
                       "," + (intMinimo.HasValue ? intMinimo.Value.ToString() : "null") +
                       "," + (intMaximo.HasValue ? intMaximo.Value.ToString() : "null") +
                       "," + (intReorden.HasValue ? intReorden.Value.ToString() : "null") +
                       ")";
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }

        CProducto_Datos objProd_Datos = new CProducto_Datos();
        objProd_Datos.intProductoID = (int)objDataResult.Tables[0].Rows[0]["ID"];
        objProd_Datos.Guardar();

        if (!string.IsNullOrEmpty(this.hdTempImg.Value))
        {
            string[] flArchivos = Directory.GetFiles(Server.MapPath("../fotos") + "/", this.hdTempImg.Value + "*.jpg");
            if (flArchivos.Length > 0)
            {
                string strImagen = ViewState["imagenes"].ToString().Replace("tmp_", "prod_").Replace("00000", ((int)objDataResult.Tables[0].Rows[0]["ID"]).ToString("00000"));
                strQuery = "UPDATE productos SET " +
                           "imagen = '" + strImagen + "'" +
                           ",imagenprincipal = '" + strImagen + "'" +
                           " WHERE ID = " + objDataResult.Tables[0].Rows[0]["ID"].ToString();
                CComunDB.CCommun.Ejecutar_SP(strQuery);
                try
                {
                    File.Move(Server.MapPath("../fotos") + "/" + ViewState["imagenes"].ToString(),
                              Server.MapPath("../fotos") + "/" + strImagen);
                }
                catch
                {
                }
            }
        }

        strQuery = "INSERT INTO precios (producto_ID, lista_precios_ID, precio_caja, " +
                  " precio_unitario, validez_desde, validez, creadoPorID, creadoPorFecha)" +
                  " SELECT " + objDataResult.Tables[0].Rows[0]["ID"].ToString() +
                  " ,lista_precios_ID, precio_caja, precio_unitario, validez_desde, validez" +
                  ", " + Session["SIANID"].ToString() +
                  ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                  " FROM precios_temp" +
                  " WHERE producto_ID = " + this.hdTempID2.Value +
                  "   AND validez = '2099-12-31'";
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "DELETE FROM precios_temp" +
                  " WHERE producto_ID = " + this.hdTempID2.Value;
        CComunDB.CCommun.Ejecutar_SP(strQuery);

        ViewState["SortCampo"] = "0";
        ViewState["CriterioCampo"] = "0";
        ViewState["Criterio"] = "";
        ViewState["SortOrden"] = 1;
        ViewState["PagActual"] = 1;
        Llenar_Grid();
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        string strQuery = "SELECT 1 " +
                    " FROM productos " +
                    " WHERE clave = '" + this.txtClave.Text.Trim() + "'" +
                    " AND ID <> " + intProdID.ToString();
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        if (string.IsNullOrEmpty(this.txtCodigo.Text.Trim()))
            this.txtCodigo.Text = this.txtClave.Text;

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Clave ya existe para otro artículo");
            return;
        }

        if (!Validar_Codigo())
            return;

        if (this.chkActivo.Checked != Convert.ToBoolean(this.hdActivo.Value))
        {
            this.chkActivo.Checked = Convert.ToBoolean(this.hdActivo.Value);
            this.mdVerificacion.Show();
            return;
        }

        Modificar_Producto(intProdID);
    }

    private void Modificar_Producto(int intProdID)
    {
        int? intMinimo, intMaximo, intReorden;
        intMinimo = intMaximo = intReorden = null;
        int intValor;
        if (!string.IsNullOrEmpty(this.txtMinimo.Text.Trim()))
        {
            if (!int.TryParse(this.txtMinimo.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Mínimo debe ser numérico y entero");
                return;
            }
            if (intValor < 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Mínimo no puede ser menor a cero");
                return;
            }
            intMinimo = intValor;
        }

        if (!string.IsNullOrEmpty(this.txtMaximo.Text.Trim()))
        {
            if (!int.TryParse(this.txtMaximo.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Máximo debe ser numérico y entero");
                return;
            }
            if (intValor < 0 || (intMinimo.HasValue && intValor < intMinimo))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Máximo no puede ser menor a cero o menor al mínimo");
                return;
            }
            intMaximo = intValor;
        }

        if (!string.IsNullOrEmpty(this.txtReorden.Text.Trim()))
        {
            if (!int.TryParse(this.txtReorden.Text.Trim(), out intValor))
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Punto de reorden debe ser numérico y entero");
                return;
            }
            if (intValor < 0)
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Punto de reorden no puede ser menor a cero");
                return;
            }
            intReorden = intValor;
        }

        string strFamiliaID = "0";
        string strClaseID = "0";

        if (this.dlFamilia.Items.Count > 0)
            strFamiliaID = this.dlFamilia.SelectedValue;

        if (this.dlClase.Items.Count > 0)
            strClaseID = this.dlClase.SelectedValue;

        string strBultoOriginal = "0";
        //if (!string.IsNullOrEmpty(this.txtBultoOriginal.Text))
        //    strBultoOriginal = this.txtBultoOriginal.Text;

        string strQuery = "UPDATE productos SET " +
                    "nombre = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ",exento = '" + (Convert.ToBoolean(this.rdExento.SelectedValue) ? "1" : "0") + "'" +
                    ",sale*-s = '" + this.txtSales.Text.Trim().Replace("'", "''") + "'" +
                    ",descripcion = '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                    ",clave = '" + this.txtClave.Text.Trim().Replace("'", "''") + "'" +
                    ",codigo = '" + this.txtCodigo.Text.Trim().Replace("'", "''") + "'" +
                    ",codigo2 = '" + this.txtCodigo2.Text.Trim().Replace("'", "''") + "'" +
                    ",codigo3 = '" + this.txtCodigo3.Text.Trim().Replace("'", "''") + "'" +
                    ",ubicacion= '" + this.txtUbicacion.Text.Trim().Replace("'", "''") + "'" +
                    ",familia_ID = " + strFamiliaID +
                    ",clase_ID = " + strClaseID +
                    ",bultooriginal = " + strBultoOriginal +
                    ",piezasporcaja = 1" + //this.txtPiezasPorCaja.Text.Trim() +
                    ",unimed = '" + this.txtUnidadDeMedida.Text.Trim() + "'" +
                    ",lote = " + (this.chkLote.Checked ? "1" : "0") +
                    ",caducidad = " + (this.chkCaducidad.Checked ? "1" : "0") +
                    ",desclim = " + (this.chkLimitado.Checked ? "1" : "0") +
                    ",neto = " + (this.chkNeto.Checked ? "1" : "0") +
                    ",clave_gobierno = '" + this.txtClave_Gobierno.Text.Trim().Replace("'", "''") + "'" +
                    ",activo = " + (this.chkActivo.Checked ? "1" : "0") +
                    " WHERE ID = " + intProdID.ToString();
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        if (intMaximo.HasValue || intMinimo.HasValue || intReorden.HasValue)
        {
            DataSet objDataResult = new DataSet();
            strQuery = "SELECT 1 FROM producto_limites " +
                       " WHERE productoID = " + intProdID.ToString();
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count == 0)
                strQuery = "INSERT INTO producto_limites (productoID, minimo, " +
                           "maximo, reorden) VALUES(" +
                           intProdID.ToString() +
                           "," + (intMinimo.HasValue ? intMinimo.Value.ToString() : "null") +
                           "," + (intMaximo.HasValue ? intMaximo.Value.ToString() : "null") +
                           "," + (intReorden.HasValue ? intReorden.Value.ToString() : "null") +
                           ")";
            else
                strQuery = "UPDATE producto_limites SET " +
                           "minimo = " + (intMinimo.HasValue ? intMinimo.Value.ToString() : "null") +
                           ",maximo = " + (intMaximo.HasValue ? intMaximo.Value.ToString() : "null") +
                           ",reorden = " + (intReorden.HasValue ? intReorden.Value.ToString() : "null") +
                           " WHERE productoID = " + intProdID.ToString();

            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        else
        {
            strQuery = "DELETE FROM producto_limites " +
                       " WHERE productoID = " + intProdID.ToString();

            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Artículo ha sido modificado");

        Llenar_Grid();
        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
    }

    protected void btnAgregarImagen_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlArchivo.Visible = true;
        if (string.IsNullOrEmpty(ViewState["imagenes"].ToString()))
            this.chkPrincipal.Checked = true;
        else
            this.chkPrincipal.Checked = false;
    }

    protected void btnAgregarPrecio_Click(object sender, EventArgs e)
    {
        if (ConfigurationManager.AppSettings["clientes"].Equals("5"))
        {
            ListItem liDiimeh = this.dlListaPrecio.Items.FindByValue("9");
            if (liDiimeh != null)
            {
                this.dlListaPrecio.ClearSelection();
                liDiimeh.Selected = true;
            }
        }

        //this.txtPiezasPorCajaPrecio.Text = this.txtPiezasPorCaja.Text.Trim();
        this.txtPrecio.Text = "0.00";
        //this.txtPrecioUnitario.Text = "0.00";
        //this.txtPrecioUnitario.Attributes.Add("onfocusin", " select();");
        //this.txtPrecioUnitario.Focus();
        this.pnlPrecio.Visible = true;
        this.pnlDatos.Visible = false;
        this.dlListaPrecio.Enabled = true;
        ViewState["preciolistaID"] = "0";
    }

    protected void gvPreciosVenta_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (this.hdUsuPr.Value.Equals("0") ||
            !this.hdUsuVen.Value.Equals("2"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
            return;
        }

        int index = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Borrar")
        {
            string strQuery;
            if (this.hdID.Value.Equals("0"))
                strQuery = "UPDATE precios_temp " +
                          " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                          " WHERE lista_precios_ID = " + this.gvPreciosVenta.DataKeys[index].Value.ToString() +
                          " AND producto_ID = " + this.hdTempID2.Value +
                          " AND validez = '2099-12-31'";
            else
                strQuery = "UPDATE precios " +
                          " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                          " WHERE lista_precios_ID = " + this.gvPreciosVenta.DataKeys[index].Value.ToString() +
                          " AND producto_ID = " + this.hdID.Value +
                          " AND validez = '2099-12-31'";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }
            Llenar_PreciosVenta();
        }
        else
            if (e.CommandName == "Modificar")
            {
                //this.txtPiezasPorCajaPrecio.Text = this.txtPiezasPorCaja.Text.Trim();
                this.txtPrecio.Text = this.gvPreciosVenta.Rows[index].Cells[1].Text.ToString().Replace("$", "").Replace(",", "");
                //this.txtPrecioUnitario.Text = this.gvPreciosVenta.Rows[index].Cells[2].Text.ToString().Replace("$", "").Replace(",", "");
                this.pnlPrecio.Visible = true;
                this.pnlDatos.Visible = false;
                this.hdListaCompras.Value = "0";
                this.dlListaPrecio.ClearSelection();
                this.dlListaPrecio.Items.FindByValue(this.gvPreciosVenta.DataKeys[index].Value.ToString()).Selected = true;
                this.dlListaPrecio.Enabled = false;
                //this.txtPrecioUnitario.Focus();
                //this.txtPrecioUnitario.Attributes.Add("onfocusin", " select();");
                ViewState["preciolistaID"] = this.gvPreciosVenta.DataKeys[index].Value.ToString();
            }
    }

    protected void gvPreciosCompra_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (this.hdUsuPr.Value.Equals("0") ||
            !this.hdUsuCom.Value.Equals("2"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
            return;
        }

        int index = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Borrar")
        {
            string strQuery;
            if (this.hdID.Value.Equals("0"))
                strQuery = "UPDATE precios_temp " +
                          " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                          " WHERE lista_precios_ID = " + this.gvPreciosCompra.DataKeys[index].Value.ToString() +
                          " AND producto_ID = " + this.hdTempID2.Value +
                          " AND validez = '2099-12-31'";
            else
                strQuery = "UPDATE precios " +
                          " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                          " WHERE lista_precios_ID = " + this.gvPreciosCompra.DataKeys[index].Value.ToString() +
                          " AND producto_ID = " + this.hdID.Value +
                          " AND validez = '2099-12-31'";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }
            Llenar_PreciosCompra();
        }
        else
            if (e.CommandName == "Modificar")
            {
                //this.txtPiezasPorCajaPrecio.Text = this.txtPiezasPorCaja.Text.Trim();
                this.txtPrecio.Text = this.gvPreciosCompra.Rows[index].Cells[1].Text.ToString().Replace("$", "").Replace(",", "");
                //this.txtPrecioUnitario.Text = this.gvPreciosCompra.Rows[index].Cells[2].Text.ToString().Replace("$", "").Replace(",", "");
                this.pnlPrecio.Visible = true;
                this.pnlDatos.Visible = false;
                this.hdListaCompras.Value = "1";
                this.hdPrecioAnterior.Value = this.txtPrecio.Text;
                this.dlListaPrecio.ClearSelection();
                this.dlListaPrecio.Items.FindByValue(this.gvPreciosCompra.DataKeys[index].Value.ToString()).Selected = true;
                this.dlListaPrecio.Enabled = false;
                ViewState["preciolistaID"] = this.gvPreciosCompra.DataKeys[index].Value.ToString();
            }
    }

    protected void btnCancelarImagen_Click(object sender, EventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            FileInfo[] flArchivos = (new DirectoryInfo(Server.MapPath("../fotos"))).GetFiles(this.hdTempImg.Value + "*.jpg");
            if (flArchivos.Length > 0)
            {
                ViewState["imagenes"] = flArchivos[0].Name;
                ViewState["imagenprincipal"] = flArchivos[0].Name;
                this.btnAgregarImagen.Enabled = false;
                Llenar_Imagenes();
            }
        }
        else
            Mostrar_Datos();
        this.pnlDatos.Visible = true;
        this.pnlArchivo.Visible = false;
    }

    protected void dlImagenes_ItemCommand(object source, DataListCommandEventArgs e)
    {
        if (!this.hdAT.Value.Equals("1"))
            return;

        if (e.CommandName == "Borrar")
        {
            string strImagenes = ViewState["imagenes"].ToString();
            strImagenes = strImagenes.Replace(";" + e.CommandArgument.ToString(), "");
            strImagenes = strImagenes.Replace(e.CommandArgument.ToString() + ";", "");
            strImagenes = strImagenes.Replace(e.CommandArgument.ToString(), "");
            if (string.IsNullOrEmpty(strImagenes))
                this.btnAgregarImagen.Enabled = true;
            ViewState["imagenes"] = strImagenes;
            string strQuery = "UPDATE productos SET " +
                            "imagen = '" + strImagenes + "'" +
                            " WHERE ID = " + this.hdID.Value;
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
                File.Delete(Server.MapPath("../fotos") + "/" + e.CommandArgument.ToString());
            }
            catch
            {
            }

            if (e.CommandArgument.ToString().Equals(ViewState["imagenprincipal"].ToString()))
            {
                strQuery = "UPDATE productos SET " +
                           "imagenprincipal = ''" +
                           " WHERE ID = " + this.hdID.Value;
                try
                {
                    CComunDB.CCommun.Ejecutar_SP(strQuery);
                }
                catch
                {
                }
                ViewState["imagenprincipal"] = "";
            }

            Llenar_Imagenes();
        }
        else
            if (e.CommandName == "Mostrar")
            {
                this.imgMostrarImagen.ImageUrl = "../fotos/" + e.CommandArgument.ToString();
                this.mdImagen.Show();
            }
            else
                if (e.CommandName == "HacerPrincipal")
                {
                    string strImagenes = ViewState["imagenes"].ToString();
                    strImagenes = strImagenes.Replace(";" + e.CommandArgument.ToString(), "");
                    strImagenes = strImagenes.Replace(e.CommandArgument.ToString() + ";", "");
                    strImagenes = strImagenes.Replace(e.CommandArgument.ToString(), "");
                    strImagenes = e.CommandArgument.ToString() + ";" + strImagenes;
                    ViewState["imagenes"] = strImagenes;
                    ViewState["imagenprincipal"] = e.CommandArgument.ToString();
                    string strQuery = "UPDATE productos SET " +
                                    "imagen = '" + strImagenes + "'" +
                                    ",imagenprincipal = '" + e.CommandArgument.ToString() + "'" +
                                    " WHERE ID = " + this.hdID.Value;
                    try
                    {
                        CComunDB.CCommun.Ejecutar_SP(strQuery);
                    }
                    catch
                    {
                    }
                    Llenar_Imagenes();
                }
    }

    protected void btnGuardarPrecio_Click(object sender, EventArgs e)
    {
        string strMensaje = string.Empty;
        if (ViewState["preciolistaID"].ToString().Equals("0"))
            strMensaje = Guardar_Precio_Nuevo();
        else
        {
            Actualizar_Precio(this.dlListaPrecio.SelectedValue,
                              this.txtPrecio.Text.Trim(),
                              this.txtPrecio.Text.Trim());
            //this.txtPrecioUnitario.Text.Trim());

            if (this.hdListaCompras.Value.Equals("1") && !this.hdTempID.Value.Equals("0"))
            {
                double dblProcentaje = double.Parse(this.txtPrecio.Text.Trim()) / double.Parse(this.hdPrecioAnterior.Value);
                this.hdPorcentajeAumento.Value = dblProcentaje.ToString();
                this.gvListasPrecios.DataSource = this.ObtenerPreciosVenta();
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
        }

        if (string.IsNullOrEmpty(strMensaje))
        {
            this.Llenar_PreciosCompra();
            this.Llenar_PreciosVenta();

            this.pnlDatos.Visible = true;
            this.pnlPrecio.Visible = false;
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);

    }

    private string Guardar_Precio_Nuevo()
    {
        DataSet objDataResult = new DataSet();
        string strQuery;
        if (this.hdID.Value.Equals("0"))
            strQuery = "SELECT 1 " +
                     " FROM precios_temp " +
                     " WHERE lista_precios_ID = " + this.dlListaPrecio.SelectedValue +
                     " AND producto_ID = " + this.hdTempID2.Value +
                     " AND validez = '2099-12-31'";
        else
            strQuery = "SELECT 1 " +
                     " FROM precios " +
                     " WHERE lista_precios_ID = " + this.dlListaPrecio.SelectedValue +
                     " AND producto_ID = " + this.hdID.Value +
                     " AND validez = '2099-12-31'";
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }

        if (objDataResult.Tables[0].Rows.Count > 0)
            return "Lista de precio ya existe para el producto";

        if (this.hdID.Value.Equals("0"))
            strQuery = "INSERT INTO precios_temp (producto_ID, fecha, lista_precios_ID, precio_caja, " +
                    "precio_unitario, validez_desde, validez) VALUES (" +
                    "'" + this.hdTempID2.Value + "'" +
                    ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                    ", '" + this.dlListaPrecio.SelectedValue + "'" +
                    ", '" + this.txtPrecio.Text.Trim() + "'" +
                    ", '" + this.txtPrecio.Text.Trim() + "'" +
                //", '" + this.txtPrecioUnitario.Text.Trim() + "'" +
                    ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                    ", '2099-12-31'" +
                    ")";
        else
            strQuery = "INSERT INTO precios (producto_ID, lista_precios_ID, precio_caja, " +
                    "precio_unitario, validez_desde, validez, creadoPorID, creadoPorFecha) VALUES (" +
                    "'" + this.hdID.Value + "'" +
                    ", '" + this.dlListaPrecio.SelectedValue + "'" +
                    ", '" + this.txtPrecio.Text.Trim() + "'" +
                    ", '" + this.txtPrecio.Text.Trim() + "'" +
                //", '" + this.txtPrecioUnitario.Text.Trim() + "'" +
                    ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                    ", '2099-12-31'" +
                    ", '" + Session["SIANID"].ToString() + "'" +
                    ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        return string.Empty;
    }

    private void Actualizar_Precio(string strListaPrecioID, string strPrecio, string strPrecioUnitario)
    {
        string strQuery;
        if (this.hdID.Value.Equals("0"))
            strQuery = "UPDATE precios_temp " +
                      " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                      " WHERE lista_precios_ID = " + strListaPrecioID +
                      " AND producto_ID = " + this.hdTempID2.Value +
                      " AND validez = '2099-12-31'";
        else
            strQuery = "UPDATE precios " +
                      " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                      " WHERE lista_precios_ID = " + strListaPrecioID +
                      " AND producto_ID = " + this.hdID.Value +
                      " AND validez = '2099-12-31'";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        if (this.hdID.Value.Equals("0"))
            strQuery = "INSERT INTO precios_temp (producto_ID, fecha, lista_precios_ID, precio_caja, " +
                      " precio_unitario, validez_desde, validez) VALUES (" +
                      "'" + this.hdTempID2.Value + "'" +
                      ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                      ", '" + strListaPrecioID + "'" +
                      ", '" + strPrecio + "'" +
                      ", '" + strPrecioUnitario + "'" +
                      ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                      ", '2099-12-31'" +
                      ")";
        else
            strQuery = "INSERT INTO precios (producto_ID, lista_precios_ID, precio_caja, " +
                      " precio_unitario, validez_desde, validez, creadoPorID, creadoPorFecha) VALUES (" +
                      "'" + this.hdID.Value + "'" +
                      ", '" + strListaPrecioID + "'" +
                      ", '" + strPrecio + "'" +
                      ", '" + strPrecioUnitario + "'" +
                      ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                      ", '2099-12-31'" +
                      ", '" + Session["SIANID"].ToString() + "'" +
                      ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                      ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error: " + ex.Message);
        }
    }

    protected void btnCancelarPrecio_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = true;
        this.pnlPrecio.Visible = false;
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

    protected void dlFamilia_SelectedIndexChanged(object sender, EventArgs e)
    {
        Llenar_Clases();
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

        this.Llenar_PreciosVenta();
    }

    protected void btnHistorial_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlHistorial.Visible = true;

        this.lblProducto.Text = this.txtNombre.Text;

        this.dlHistorial.DataSource = ObtenerHistorial();
        this.dlHistorial.DataBind();
    }

    protected void flArchivo_UploadedComplete(object sender, AsyncFileUploadEventArgs e)
    {
        string strMensaje = "Archivo ha sido guardado";
        if (this.flArchivo.HasFile)
        {
            if (Regex.IsMatch(flArchivo.PostedFile.ContentType, "image/\\S+") &&
                flArchivo.PostedFile.ContentLength > 0)
            {
                HttpCookie ckSIAN = Request.Cookies["userCng"];
                const int max_pixeles = 600;
                System.Drawing.Image img = System.Drawing.Image.FromStream(flArchivo.PostedFile.InputStream);
                int newWidth = 0;
                int newHeigth = 0;
                if (img.Width > max_pixeles || img.Height > max_pixeles)
                {
                    double relacion = 0;
                    if (img.Width > img.Height)
                        relacion = max_pixeles / (double)img.Width;
                    else
                        relacion = max_pixeles / (double)img.Height;
                    newWidth = (int)(img.Width * relacion);
                    newHeigth = (int)(img.Height * relacion);
                }
                else
                {
                    newWidth = img.Width;
                    newHeigth = img.Height;
                }

                Bitmap bitmap = new Bitmap(img, newWidth, newHeigth);
                bitmap.SetResolution(200, 200);
                string strNombreArchivo = string.Empty;
                if (this.hdID.Value.Equals("0"))
                    strNombreArchivo = this.hdTempImg.Value + "-" +
                                      newWidth.ToString() + "x" +
                                      newHeigth.ToString() + ".jpg";
                else
                {
                    int intSuma = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day +
                            DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    strNombreArchivo = "prod_" +
                                      (int.Parse(ckSIAN["ck_cliente"])).ToString("000") + "_" +
                                      (int.Parse(this.hdID.Value)).ToString("000000") + "_" +
                                      intSuma.ToString() + "-" +
                                      newWidth.ToString() + "x" +
                                      newHeigth.ToString() + ".jpg";
                }
                try
                {
                    bitmap.Save(Server.MapPath("../fotos") + "/" + strNombreArchivo, System.Drawing.Imaging.ImageFormat.Jpeg);
                    if (string.IsNullOrEmpty(ViewState["imagenes"].ToString()))
                        ViewState["imagenes"] = strNombreArchivo;
                    else
                    {
                        if (this.chkPrincipal.Checked)
                            ViewState["imagenes"] = strNombreArchivo + ";" + ViewState["imagenes"];
                        else
                            ViewState["imagenes"] = ViewState["imagenes"] + ";" + strNombreArchivo;
                    }

                    string strQuery = "UPDATE productos SET " +
                            "imagen = '" + ViewState["imagenes"].ToString() + "'";

                    if (this.chkPrincipal.Checked)
                    {
                        strQuery += ",imagenprincipal = '" + strNombreArchivo + "'";
                        ViewState["imagenprincipal"] = strNombreArchivo;
                    }

                    strQuery += " WHERE ID = " + this.hdID.Value;
                    try
                    {
                        CComunDB.CCommun.Ejecutar_SP(strQuery);
                    }
                    catch (Exception ex)
                    {
                        strMensaje = "No se pudo agregar la imagen: " + ex.Message;

                    }
                }
                catch (Exception ex)
                {
                    strMensaje = "No se pudo agregar la imagen: " + ex.Message;
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
            else
                strMensaje = "Archivo no es un archivo con una imagen";
        }
        else
            strMensaje = "Seleccione un archivo";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", "archivoResult('" + strMensaje + "');", true);
    }

    protected void flArchivo_UploadedFileError(object sender, AsyncFileUploadEventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", "archivoResult('Error al guardar el archivo: " + e.StatusMessage + "');", true);
    }

    protected void btnNextProd_Click(object sender, ImageClickEventArgs e)
    {
        int intPos = int.Parse(this.hdPos.Value);
        if (intPos == (this.grdvLista.Rows.Count - 1))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Este es el último producto de la búsqueda");
            return;
        }
        intPos++;
        this.hdID.Value = this.grdvLista.DataKeys[intPos].Value.ToString();
        this.hdPos.Value = intPos.ToString();
        Mostrar_Datos();
    }

    protected void btnPrevProd_Click(object sender, ImageClickEventArgs e)
    {
        int intPos = int.Parse(this.hdPos.Value);
        if (intPos == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Este es el primer producto de la búsqueda");
            return;
        }
        intPos--;
        this.hdID.Value = this.grdvLista.DataKeys[intPos].Value.ToString();
        this.hdPos.Value = intPos.ToString();
        Mostrar_Datos();
    }

    protected void btnRegresarHist_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = true;
        this.pnlHistorial.Visible = false;
    }

    protected void txtDescripcion_TextChanged(object sender, EventArgs e)
    {
        if (this.txtDescripcion.Text.Length > 5000)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Se excedió el límite del tamaño del campo Descripción: 5000 caracteres<br/>La descripción se ha truncado a esta cantidad");
            this.txtDescripcion.Text = this.txtDescripcion.Text.Substring(0, 5000);
        }
        else
            this.txtCodigo.Focus();
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

        if (string.IsNullOrEmpty(this.txtCodigoVerificacion.Text) ||
            string.IsNullOrEmpty(strCodigo) ||
            !this.txtCodigoVerificacion.Text.Equals(strCodigo))
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

        CRutinas.Enviar_Correo("6",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Producto: " + this.txtNombre.Text + "<br />" +
                               "Precio: " + this.txtPrecio.Text + "<br />" +
                               "Razón: Activar/Inactivar producto" + "<br /><br />" +
                               strTexto.ToString().Replace("display:none;", ""));

        //Se agrego este ENVIAR CORREO PARA VER EL RESULTADO DEL CORREO.
        CRutinas.Enviar_Correo("7",
                               "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "<br />" +
                               "Código usado: " + strCodigo + "<br />" +
                               "Usuario: " + Session["SIANID"].ToString() + "<br />" +
                               "Producto: " + this.txtNombre.Text + "<br />" +
                               "Precio: " + this.txtPrecio.Text + "<br />" +
                               "Razón: Activar/Inactivar producto" + "<br /><br />" +
                               strTexto.ToString().Replace("display:none;", ""));

        this.chkActivo.Checked = !this.chkActivo.Checked;
        this.hdActivo.Value = this.chkActivo.Checked.ToString();

        Modificar_Producto(Convert.ToInt32(this.hdID.Value));
    }
}