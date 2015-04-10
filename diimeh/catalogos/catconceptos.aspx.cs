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

public partial class catalogos_catconceptos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtDescripcion.Attributes["onKeyPress"] = "javascript:return Limite(event, this, 500)";
        this.txtDescripcion.Attributes["onpaste"] = "javascript:return prevenirPaste(this, 500)";
        //this.txtPrecioUnitario.Attributes["onblur"] = "javascript:return calcularPrecio(this, 'uni')";
        this.btnAgregar.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnAgregar.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.btnGuardarPrecio.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnGuardarPrecio.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnCancelarPrecio.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelarPrecio.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        if (!IsPostBack)
        {
            bool swVer, swTot;

            this.hdUsuCompras.Value = "1";
            if (!CComunDB.CCommun.ValidarAcceso(1101, out swVer, out swTot))    //Listas de precios de compras
                this.hdUsuCompras.Value = "0";

            this.hdUsuPr.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1105, out swVer, out swTot))
                this.hdUsuPr.Value = "1";
            else
            {
                this.btnAgregarPrecio.Visible = false;
            }

            if (!CComunDB.CCommun.ValidarAcceso(1050, out swVer, out swTot))
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
            Llenar_Grid();

            Llenar_Catalogos();

            this.txtClave.Enabled = false;

            this.hdID.Value = null;
        }
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
        dt.Columns.Add(new DataColumn("familia", typeof(string)));
        dt.Columns.Add(new DataColumn("clase", typeof(string)));
        dt.Columns.Add(new DataColumn("linea", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("clave", typeof(string)));
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("existencia", typeof(string)));

        string strQuery = "CALL leer_productos_consulta(" +
            ViewState["SortCampo"].ToString() +
            ", " + ViewState["SortOrden"].ToString() +
            ", " + ViewState["CriterioCampo"].ToString() +
            ", '" + ViewState["Criterio"].ToString().Replace("'","''''") + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30, 2)";
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
            if (objRowResult.IsNull("desde"))
                dr[7] = string.Empty;
            else
                dr[7] = ((decimal)objRowResult["precio"]).ToString("c") + "   -   " +
                        ((DateTime)objRowResult["desde"]).ToString("dd/MM/yy");
            if ((bool)objRowResult["desclim"])
                dr[7] += "(DL)";
            if ((bool)objRowResult["neto"])
                dr[7] += "(N)";
            dr[8] = ((decimal)objRowResult["existencia"]).ToString("0.##");

            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32(decimal.Truncate((decimal)objRowResult2["paginas"]));

        return dt;
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

        if (this.hdUsuCompras.Value.Equals("0"))
            strQuery += " AND L.tipo_lista = 'VENTAS'";
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
                this.btnAgregarPrecio.Visible = false;
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
        this.dlFamilia.ClearSelection();
        this.dlClase.ClearSelection();
        if (this.hdID.Value.Equals("0"))
        {
            this.txtNombre.Text = string.Empty;
            this.lblNombre.Text = string.Empty;
            this.rdExento.ClearSelection();
            this.rdExento.SelectedIndex = 0;
            this.txtDescripcion.Text = string.Empty;
            this.txtClave.Text = string.Empty;
            this.txtCodigo.Text = string.Empty;
            //this.txtBultoOriginal.Text = string.Empty;
            //this.txtPiezasPorCaja.Text = string.Empty;
            this.btnAgregar.Visible = true;
            this.btnModificar.Visible = false;
            if (this.dlFamilia.Items.Count > 0)
                this.dlFamilia.SelectedIndex = 0;
            if (this.dlClase.Items.Count > 0)
                this.dlClase.SelectedIndex = 0;
            this.gvPreciosVenta.DataSource = null;
            this.gvPreciosVenta.DataBind();
            this.btnAgregarPrecio.Enabled = false;
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT nombre, exento, sales, clave, codigo, " +
                    "codigo2, codigo3, desclim," +
                    "descripcion, ubicacion, clave_gobierno, " +
                    "familia_ID, clase_ID, linea_ID, " +
                    "bultooriginal, piezasporcaja, unimed, imagen, imagenprincipal " +
                    ", lote, caducidad " +
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
            this.txtDescripcion.Text = objRowResult["descripcion"].ToString();
            this.txtClave.Text = objRowResult["clave"].ToString();
            this.txtCodigo.Text = objRowResult["codigo"].ToString();
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

            Llenar_PreciosVenta();

            this.btnAgregar.Visible = false;
            this.btnModificar.Visible = true;

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

        this.dlListaPrecio.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
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
        this.gvPreciosVenta.DataSource = ObtenerDatosVenta();
        this.gvPreciosVenta.DataBind();
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

        string strQuery = "SELECT P.lista_precios_ID as listapreciosID, " +
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
            dr[5] = ((DateTime)objRowResult["validez_desde"]).ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("es-MX"));

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

        string strQuery = "SELECT lista_precios_ID, nombre_lista, precio_caja " +
                    "FROM precios P " +
                    "INNER JOIN listas_precios L " +
                    "ON P.lista_precios_ID = L.ID " +
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

    protected void btnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        DataSet objDataResult = new DataSet();
        int intProdID = Convert.ToInt32(this.hdID.Value);

        if (string.IsNullOrEmpty(this.txtClave.Text.Trim()))
        {
        }

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

        Guardar_Producto();
    }

    private bool Validar_Codigo()
    {
        string strQuery = "SELECT 1 " +
                         " FROM productos " +
                         " WHERE codigo = '" + this.txtCodigo.Text.Trim() + "'" +
                         "   AND ID <> " + this.hdID.Value;
        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Código ya existe para otro concepto");
            return false;
        }

        return true;
    }

    private void Guardar_Producto()
    {
        string strFamiliaID = "0";
        string strClaseID = "0";

        if (this.dlFamilia.Items.Count > 0)
            strFamiliaID = this.dlFamilia.SelectedValue;

        if (this.dlClase.Items.Count > 0)
            strClaseID = this.dlClase.SelectedValue;

        string strBultoOriginal = "0";

        string strQuery = "INSERT INTO productos (nombre, tipo, exento, sales, descripcion, " +
                    "clave, codigo, codigo2, codigo3, ubicacion, familia_ID, clase_ID, " +
                    "bultooriginal, piezasporcaja, unimed," +
                    "lote, caducidad, desclim, neto, " +
                    "clave_gobierno) VALUES (" +
                    "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ", 2" +
                    ", '" + (Convert.ToBoolean(this.rdExento.SelectedValue) ? "1" : "0") + "'" +
                    ", ''" +
                    ", '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtClave.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + this.txtCodigo.Text.Trim().Replace("'", "''") + "'" +
                    ", ''" +
                    ", ''" +
                    ", ''" +
                    ", " + strFamiliaID +
                    ", " + strClaseID +
                    ", " + strBultoOriginal +
                    ", 1" + //this.txtPiezasPorCaja.Text.Trim() +
                    ", 'No aplica'" +
                    ", 0" +
                    ", 0" +
                    ", 0" +
                    ", 0" +
                    ", ''" +
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

        CProducto_Datos objProd_Datos = new CProducto_Datos();
        objProd_Datos.intProductoID = (int)objDataResult.Tables[0].Rows[0]["ID"];
        objProd_Datos.Guardar();

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
            ((master_MasterPage)Page.Master).MostrarMensajeError("Clave ya existe para otro concepto");
            return;
        }

        if (!Validar_Codigo())
            return;

        Modificar_Producto(intProdID);
    }

    private void Modificar_Producto(int intProdID)
    {
        string strFamiliaID = "0";
        string strClaseID = "0";

        if (this.dlFamilia.Items.Count > 0)
            strFamiliaID = this.dlFamilia.SelectedValue;

        if (this.dlClase.Items.Count > 0)
            strClaseID = this.dlClase.SelectedValue;

        string strBultoOriginal = "0";

        string strQuery = "UPDATE productos SET " +
                    "nombre = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ",exento = '" + (Convert.ToBoolean(this.rdExento.SelectedValue) ? "1" : "0") + "'" +
                    ",descripcion = '" + this.txtDescripcion.Text.Trim().Replace("'", "''") + "'" +
                    ",clave = '" + this.txtClave.Text.Trim().Replace("'", "''") + "'" +
                    ",codigo = '" + this.txtCodigo.Text.Trim().Replace("'", "''") + "'" +
                    ",familia_ID = " + strFamiliaID +
                    ",clase_ID = " + strClaseID +
                    ",bultooriginal = " + strBultoOriginal +
                    ",piezasporcaja = 1" + //this.txtPiezasPorCaja.Text.Trim() +
                    " WHERE ID = " + intProdID.ToString();
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Concepto ha sido modificado");

        Llenar_Grid();
        this.pnlDatos.Visible = false;
        this.pnlListado.Visible = true;
    }

    protected void btnAgregarPrecio_Click(object sender, EventArgs e)
    {
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
        if (this.hdUsuPr.Value.Equals("0"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
            return;
        }

        int index = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Borrar")
        {
            string strQuery = "UPDATE precios " +
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

    protected void btnGuardarPrecio_Click(object sender, EventArgs e)
    {
        string strMensaje = string.Empty;
        if (ViewState["preciolistaID"].ToString().Equals("0"))
            strMensaje = Guardar_Precio_Nuevo();
        else
        {

            Actualizar_Precio(this.dlListaPrecio.SelectedValue,
                              this.txtPrecio.Text.Trim());
            //this.txtPrecioUnitario.Text.Trim());

            if (this.hdListaCompras.Value.Equals("1"))
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
        string strQuery = "SELECT 1 " +
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

        CComunDB.CCommun.Actualizar_Precio(this.hdID.Value,
                                           this.dlListaPrecio.SelectedValue,
                                           decimal.Parse(this.txtPrecio.Text.Trim()));

        return string.Empty;
    }

    private void Actualizar_Precio(string strListaPrecioID, string strPrecioUnitario)
    {
        CComunDB.CCommun.Actualizar_Precio(this.hdID.Value,
                                           strListaPrecioID,
                                           decimal.Parse(strPrecioUnitario));
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
                if(intPagina > int.Parse(ViewState["PagTotal"].ToString()))
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
                                      dcmPrecioUnitario.ToString());
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

    protected void btnRegresarHist_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = true;
        this.pnlHistorial.Visible = false;
    }
}