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

public partial class catalogos_catlistasprecios : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtProducto.Attributes["onfocus"] = "javascript:limpiarProdID();";
        this.btnSubirImagen.Attributes["onmouseout"] = "javascript:this.className='UpLoadFormat1'";
        this.btnSubirImagen.Attributes["onmouseover"] = "javascript:this.className='UpLoadFormat2'";
        this.btnCancelarImagen.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelarImagen.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.btnModificar.Attributes["onmouseout"] = "javascript:this.className='ModifyFormat1'";
        this.btnModificar.Attributes["onmouseover"] = "javascript:this.className='ModifyFormat2'";
        this.btnCancelar.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelar.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.btnGuardarPrecio.Attributes["onmouseout"] = "javascript:this.className='AddFormat1'";
        this.btnGuardarPrecio.Attributes["onmouseover"] = "javascript:this.className='AddFormat2'";
        this.btnValidarPrecio.Attributes["onmouseout"] = "javascript:this.className='SalidaFormat1'";
        this.btnValidarPrecio.Attributes["onmouseover"] = "javascript:this.className='SalidaFormat2'";
        this.btnCancelarPrecio.Attributes["onmouseout"] = "javascript:this.className='BackFormat1'";
        this.btnCancelarPrecio.Attributes["onmouseover"] = "javascript:this.className='BackFormat2'";

        this.rdFijo.Attributes["onclick"] = "javascript:activar_fijo_desc();";
        this.rdPorcentaje.Attributes["onclick"] = "javascript:activar_fijo_desc();";
        this.chkCopiar.Attributes["onclick"] = "javascript:activar_copia();";

        if (!IsPostBack)
        {
            bool swVer, swTot;
            this.hdUsuPr.Value = "0";
            if (CComunDB.CCommun.ValidarAcceso(1105, out swVer, out swTot))
                this.hdUsuPr.Value = "1";
            else
            {
                this.txtProducto.Enabled = false;
                this.txtPrecioUnitario.Enabled = false;
                this.btnAgregarProd.Visible = false;
                this.btnAgregarPrecio.Visible = false;
            }

            this.hdUsuVentas.Value = "1";
            this.hdUsuCompras.Value = "1";

            if (!CComunDB.CCommun.ValidarAcceso(1100, out swVer, out swTot))
                this.hdUsuVentas.Value = "0";

            this.hdATV.Value = "1";
            if (!swTot)
                this.hdATV.Value = "0";

            if (!CComunDB.CCommun.ValidarAcceso(1101, out swVer, out swTot))
                this.hdUsuCompras.Value = "0";

            this.hdATC.Value = "1";
            if (!swTot)
                this.hdATC.Value = "0";

            if(this.hdUsuVentas.Value.Equals("0") && this.hdUsuCompras.Value.Equals("0"))
                Response.Redirect("../inicio/error.aspx");

            if (this.hdATV.Value.Equals("0") && this.hdATC.Value.Equals("0"))
                this.lblAgregar.Visible = false;

            ViewState["dvSort"] = "tipo";
            Llenar_Grid(string.Empty);

            Llenar_Catalogos();

            this.hdID.Value = "";

        }
    }

    private void Llenar_Catalogos()
    {
        if (this.hdUsuVentas.Value.Equals("1") && this.hdUsuCompras.Value.Equals("1"))
        {
            this.dlListasPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios(string.Empty);
        }
        else
            if (this.hdUsuVentas.Value.Equals("1"))
            {
                this.dlListasPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("VENTAS");
                this.rdVentas.Checked = true;
                this.rdCompras.Checked = false;
                this.rdCompras.Enabled = false;
            }
            else
            {
                this.dlListasPrecios.DataSource = CComunDB.CCommun.ObtenerListasPrecios("COMPRAS");
                this.rdVentas.Checked = false;
                this.rdCompras.Checked = true;
                this.rdCompras.Enabled = true;
            }

        this.dlListasPrecios.DataBind();

        if (dlListasPrecios.Items.Count == 0)
            this.chkCopiar.Enabled = false;
        else
            this.chkCopiar.Enabled = true;
    }

    private void Llenar_Grid(string strCriterio)
    {
        DataTable dtResultado = ObtenerDatos(strCriterio);

        if (dtResultado.Rows.Count != 0)
        {
            ViewState["dtResultado"] = dtResultado;
            this.grdvLista.DataSource = dtResultado;
            this.grdvLista.DataBind();
        }
        ViewState["dvSortOrder"] = "ASC";
    }

    private DataTable ObtenerDatos(string strCriterio)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        DataSet objDataResult = new DataSet();

        dt.Columns.Add(new DataColumn("referencia", typeof(string)));
        dt.Columns.Add(new DataColumn("nombre", typeof(string)));
        dt.Columns.Add(new DataColumn("porcentaje", typeof(string)));
        dt.Columns.Add(new DataColumn("fijo", typeof(string)));
        dt.Columns.Add(new DataColumn("tipo", typeof(string)));
        dt.Columns.Add(new DataColumn("imagen", typeof(string)));

        if (this.hdUsuVentas.Value.Equals("1") && this.hdUsuCompras.Value.Equals("1"))
        {
        }
        else
        {
            if (this.hdUsuVentas.Value.Equals("1"))
            {
                if (string.IsNullOrEmpty(strCriterio))
                    strCriterio = " WHERE tipo_lista = 'VENTAS'";
                else
                    strCriterio += " AND tipo_lista = 'VENTAS'";
            }
            else
            {
                if (string.IsNullOrEmpty(strCriterio))
                    strCriterio = " WHERE tipo_lista = 'COMPRAS'";
                else
                    strCriterio += " AND tipo_lista = 'COMPRAS'";
            }
        }

        string strQuery = "SELECT ID as referencia " +
            ",nombre_lista as nombre" +
            ",porcentaje " +
            ",fijo" +
            ",tipo_lista as tipo " +
            ",imagen " +
            " FROM listas_precios " +
            strCriterio +
            " ORDER BY " + ViewState["dvSort"].ToString();

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
            dr[0] = objRowResult["referencia"].ToString();
            dr[1] = objRowResult["nombre"].ToString();
            if (((int)objRowResult["porcentaje"]) == 1)
                dr[2] = "Si";
            else
                dr[2] = "No";
            if (((int)objRowResult["fijo"]) == 1)
                dr[3] = "Si";
            else
                dr[3] = "No";
            dr[4] = objRowResult["tipo"].ToString();
            if (string.IsNullOrEmpty(objRowResult["imagen"].ToString()))
                dr[5] = "~/imagenes/dummy.ico";
            else
                dr[5] = "~/fotos/" + objRowResult["imagen"].ToString();

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
        dt.Columns.Add(new DataColumn("precio", typeof(string)));
        dt.Columns.Add(new DataColumn("codigo", typeof(string)));
        dt.Columns.Add(new DataColumn("minimo", typeof(string)));
        dt.Columns.Add(new DataColumn("maximo", typeof(string)));
        dt.Columns.Add(new DataColumn("existencia", typeof(string)));
        dt.Columns.Add(new DataColumn("usuario", typeof(string)));
        dt.Columns.Add(new DataColumn("parcial", typeof(string)));

        string strQuery = "CALL leer_productos_lista(" +
            "'" + this.hdID.Value + "'" +
            ", " + ViewState["PagActual"].ToString() +
            ", 30)";

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
            dr[0] = objRowResult["producto_ID"].ToString();
            dr[1] = objRowResult["nombre"].ToString();
            dr[2] = ((decimal)objRowResult["precio"]).ToString("c");
            dr[3] = objRowResult["codigo"].ToString();
            if(!objRowResult.IsNull("minimo"))
                dr[4] = ((decimal)objRowResult["minimo"]).ToString("#,##0.##");
            if (!objRowResult.IsNull("maximo"))
                dr[5] = ((decimal)objRowResult["maximo"]).ToString("#,##0.##");
            if (!objRowResult.IsNull("existencia"))
                dr[6] = ((decimal)objRowResult["existencia"]).ToString("#,##0.##");
            else
                dr[6] = "0";
            if(!objRowResult.IsNull("usuario"))
                dr[7] = objRowResult["usuario"].ToString() + " " +
                        ((DateTime)objRowResult["creadoPorFecha"]).ToString("dd/MMM/yyyy");

            strQuery = "SELECT DISTINCT 1" +
                      " FROM orden_compra C" +
                      " INNER JOIN orden_compra_productos P" +
                      " ON C.ID = P.orden_compraID" +
                      " AND C.estatus = 4" +
                      " AND P.validado = 0" +
                      " AND P.productoID = " + objRowResult["producto_ID"];

            objDataResult2 = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult2.Tables[0].Rows.Count > 0)
                dr[8] = "1";
            else
                dr[8] = "0";

            dt.Rows.Add(dr);
        }

        DataRow objRowResult2 = objDataResult.Tables[1].Rows[0];
        ViewState["PagTotal"] = Convert.ToInt32(decimal.Truncate((decimal)objRowResult2["paginas"]));

        return dt;
    }

    protected void grdvLista_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression.Equals(ViewState["dvSort"].ToString()))
        {
            if (ViewState["dvSortOrder"].ToString().Equals("ASC"))
                ViewState["dvSortOrder"] = "DESC";
            else
                ViewState["dvSortOrder"] = "ASC";
        }
        else
            ViewState["dvSortOrder"] = "ASC";

        ViewState["dvSort"] = e.SortExpression;

        DataTable dtResultado = ViewState["dtResultado"] as DataTable;

        dtResultado.DefaultView.Sort = ViewState["dvSort"].ToString() + " " + ViewState["dvSortOrder"].ToString();
        grdvLista.DataSource = dtResultado;
        grdvLista.DataBind();
    }

    protected void grdvLista_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Modificar")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            this.hdID.Value = this.grdvLista.DataKeys[index].Value.ToString();
            Mostrar_Datos_Lista();
            if ((this.rdVentas.Checked && this.hdATV.Value.Equals("0")) ||
                (this.rdCompras.Checked && this.hdATC.Value.Equals("0")))
            {
                this.btnAgregarImagen.Visible = false;
                this.btnModificar.Visible = false;
                this.btnAgregarPrecio.Visible = false;
                this.btnAgregarProd.Visible = false;
                this.gvProductos.Enabled = false;
            }
            else
            {
                this.btnAgregarImagen.Visible = true;
                this.btnModificar.Visible = true;
                this.btnAgregarPrecio.Visible = true;
                this.btnAgregarProd.Visible = true;
                this.gvProductos.Enabled = true;
            }
        }
    }

    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {
        string strMensaje = string.Empty;
        if (!string.IsNullOrEmpty(this.txtCriterio.Text.Trim()))
        {
            if (this.dlBusqueda.SelectedValue == "1" ||
                this.dlBusqueda.SelectedValue == "2")
            {
                if (!this.txtCriterio.Text.Trim().ToLower().Equals("si") &&
                    !this.txtCriterio.Text.Trim().ToLower().Equals("no"))
                    strMensaje = "Criterio de búsqueda debe ser 'SI' o 'NO'";

            }
            if (string.IsNullOrEmpty(strMensaje))
            {
                StringBuilder strCriterio = new StringBuilder("WHERE ");
                switch (this.dlBusqueda.SelectedValue)
                {
                    case "0":
                        strCriterio.Append("nombre_lista like '%");
                        strCriterio.Append(this.txtCriterio.Text.Trim());
                        strCriterio.Append("%'");
                        break;
                    case "1":
                        strCriterio.Append("porcentaje = ");
                        if (this.txtCriterio.Text.Trim().ToLower().Equals("si"))
                            strCriterio.Append("1");
                        else
                            strCriterio.Append("0");
                        break;
                    case "2":
                        strCriterio.Append("fijo = ");
                        if (this.txtCriterio.Text.Trim().ToLower().Equals("si"))
                            strCriterio.Append("1");
                        else
                            strCriterio.Append("0");
                        break;
                    case "3":
                        strCriterio.Append("tipo_lista like '%");
                        strCriterio.Append(this.txtCriterio.Text.Trim());
                        strCriterio.Append("%'");
                        break;
                }

                Llenar_Grid(strCriterio.ToString());
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

        ViewState["dvSort"] = "nombre";
        Llenar_Grid(string.Empty);
    }

    protected void lblAgregar_Click(object sender, EventArgs e)
    {
        this.hdID.Value = "0";
        Mostrar_Datos_Lista();
    }

    private void Mostrar_Datos_Lista()
    {
        this.hdProductoID.Value = string.Empty;
        if (this.hdID.Value.Equals("0"))
        {
            this.txtNombre.Text = string.Empty;
            this.rdVentas.Checked = true;
            this.rdCompras.Checked = false;
            this.chkDefault.Checked = false;
            this.chkCopiar.Checked = false;
            this.rdFijo.Checked = true;
            this.rdPorcentaje.Checked = false;
            this.txtPorcentaje.Text = string.Empty;
            this.chkDesc.Checked = false;
            this.btnModificar.Visible = true;
            this.btnAgregarImagen.Enabled = false;
            this.pnlCopiar.Visible = true;
            this.btnAgregarPrecio.Enabled = false;
            this.gvProductos.DataSource = null;
            this.gvProductos.DataBind();

            this.btnFirstPage.Visible = false;
            this.btnLastPage.Visible = false;
            this.btnNextPage.Visible = false;
            this.btnPrevPage.Visible = false;
            this.lblPagina.Text = "";
            this.lblPaginatot.Text = "";
            this.txtPag.Visible = false;

            this.txtProducto.Enabled = false;
            this.txtPrecioUnitario.Enabled = false;
            this.btnAgregarProd.Enabled = false;

            if (this.hdATC.Value.Equals("0"))
            {
                this.rdVentas.Enabled = true;
                this.rdVentas.Checked = true;
                this.rdCompras.Enabled = false;
            }
            else
                if (hdATV.Value.Equals("0"))
                {
                    this.rdVentas.Enabled = false;
                    this.rdCompras.Enabled = true;
                    this.rdCompras.Checked = true;
                }
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT nombre_lista as nombre" +
                    ",tipo_lista as tipo " +
                    ",default_lista " +
                    ",imagen " +
                    " FROM listas_precios " +
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
            if (objRowResult["tipo"].ToString().Equals("VENTAS"))
            {
                this.rdVentas.Checked = true;
                this.rdCompras.Checked = false;
            }
            else
            {
                this.rdVentas.Checked = false;
                this.rdCompras.Checked = true;
            }

            if (objRowResult["default_lista"].ToString().Equals("1"))
                this.chkDefault.Checked = true;
            else
                this.chkDefault.Checked = false;

            if (!string.IsNullOrEmpty(objRowResult["imagen"].ToString()))
            {
                const int max_pixeles = 200;
                int newWidth = 0;
                int newHeigth = 0;
                string strArchivo = objRowResult["imagen"].ToString();

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

                this.imgListaPrecio.ImageUrl = "~/fotos/" + objRowResult["imagen"].ToString();
                this.imgListaPrecio.Width = newWidth;
                this.imgListaPrecio.Height = newHeigth;
            }
            else
                this.imgListaPrecio.ImageUrl = "../imagenes/dummy.ico/";

            this.btnModificar.Visible = true;
            this.btnAgregarImagen.Enabled = true;
            this.btnAgregarPrecio.Enabled = true;
            this.pnlCopiar.Visible = false;

            this.txtProducto.Enabled = true;
            this.txtPrecioUnitario.Enabled = true;
            this.btnAgregarProd.Enabled = true;

            ViewState["PagActual"] = 1;
            Llenar_Productos();
        }

        this.pnlListado.Visible = false;
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
    }

    protected void btnCancelar_Click(object sender, ImageClickEventArgs e)
    {
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
    }

    protected void btnModificar_Click(object sender, ImageClickEventArgs e)
    {
        if (this.hdID.Value.Equals("0"))
        {
            DataSet objDataResult = new DataSet();
            string strQuery = "SELECT 1 " +
                        " FROM listas_precios " +
                        " WHERE nombre_lista = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            if (objDataResult.Tables[0].Rows.Count > 0)
                ((master_MasterPage)Page.Master).MostrarMensajeError("Nombre de lista ya existe");
            else
            {
                if (this.rdPorcentaje.Checked &&
                    string.IsNullOrEmpty(this.txtPorcentaje.Text.Trim()))
                    ((master_MasterPage)Page.Master).MostrarMensajeError("Ingrese el porcentaje");
                else
                    Guardar_Lista();
            }
        }
        else
        {
            DataSet objDataResult = new DataSet();

            string strQuery = "SELECT 1 " +
                        " FROM listas_precios " +
                        " WHERE nombre_lista = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                        " AND ID <> " + this.hdID.Value;
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count > 0)
                ((master_MasterPage)Page.Master).MostrarMensajeError("Nombre de lista ya existe");
            else
                Modificar_Lista(Convert.ToInt32(this.hdID.Value));
        }
    }

    private void Guardar_Lista()
    {
        string strQuery = string.Empty;
        string strDefault = "0";
        string strTipoLista = "VENTAS";
        string strPorcentaje = "0";
        string strFijo = "0";

        DataSet objDataResult = new DataSet();
        if (this.chkDefault.Checked)
        {
            strQuery = "UPDATE listas_precios SET default_lista = '0'";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }
            strDefault = "1";
        }

        if (this.rdCompras.Checked)
            strTipoLista = "COMPRAS";

        if (this.rdFijo.Checked)
            strFijo = "1";

        if (this.rdPorcentaje.Checked)
            strPorcentaje = "1";

        strQuery = "INSERT INTO listas_precios (nombre_lista, default_lista, " +
                    "tipo_lista, porcentaje, fijo) VALUES (" +
                    "'" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ", '" + strDefault + "'" +
                    ", '" + strTipoLista + "'" +
                    ", '" + strPorcentaje + "'" +
                    ", '" + strFijo + "'" +
                    ")";
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        if (this.chkCopiar.Checked)
        {
            string strListaPrecioID = "0";

            strQuery = "SELECT ID " +
                    " FROM listas_precios " +
                    " WHERE nombre_lista = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'";
            try
            {
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }

            DataRow objRowResult = objDataResult.Tables[0].Rows[0];

            strListaPrecioID = objRowResult["ID"].ToString();
            double dbPorcentaje = 1;

            if (this.rdPorcentaje.Checked)
            {
                if (this.chkDesc.Checked)
                    dbPorcentaje -= (double.Parse(this.txtPorcentaje.Text.Trim()) / 100);
                else
                    dbPorcentaje += (double.Parse(this.txtPorcentaje.Text.Trim()) / 100);
            }

            strQuery = "INSERT INTO precios (producto_id, lista_precios_id, " +
                    "precio_caja, " +
                    "precio_unitario, " +
                    "validez_desde, validez, clave, creadoPorID, creadoPorFecha) " +
                    "(SELECT producto_id, " + strListaPrecioID +
                    ",precio_caja * " + dbPorcentaje.ToString() + " as precio_caja" +
                    ",precio_unitario * " + dbPorcentaje.ToString() + " as precio_unitario" +
                    ",'" + DateTime.Today.ToString("yyyy-MM-dd") + "' as validez_desde" +
                    ",validez, clave " +
                    ", " + Session["SIANID"].ToString() +
                    ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " FROM precios " +
                    " WHERE lista_precios_id = " + this.dlListasPrecios.SelectedValue +
                    "   AND validez ='2099-12-31')";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }
        }

        Llenar_Catalogos();
        ViewState["dvSort"] = "nombre";
        Llenar_Grid(string.Empty);
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
    }

    private void Modificar_Lista(int intListaPrecioID)
    {
        string strQuery = string.Empty;
        string strDefault = "0";
        string strTipoLista = "VENTAS";

        if (this.chkDefault.Checked)
        {
            strQuery = "UPDATE listas_precios SET default_lista = '0'";
            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch
            {

            }
            strDefault = "1";
        }

        if (this.rdCompras.Checked)
            strTipoLista = "COMPRAS";

        strQuery = "UPDATE listas_precios SET " +
                    "nombre_lista = '" + this.txtNombre.Text.Trim().Replace("'", "''") + "'" +
                    ",default_lista = '" + strDefault + "'" +
                    ",tipo_lista = '" + strTipoLista + "'" +
                    " WHERE ID = " + intListaPrecioID.ToString();
        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch
        {

        }

        Llenar_Catalogos();
        ViewState["dvSort"] = "nombre";
        Llenar_Grid(string.Empty);
        this.pnlListado.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
    }

    protected void btnAgregarImagen_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
        this.pnlArchivo.Visible = true;
    }

    protected void btnSubirImagen_Click(object sender, EventArgs e)
    {
        string strMensaje = string.Empty;
        if (this.flArchivoImagen.HasFile)
        {
            if (Regex.IsMatch(flArchivoImagen.PostedFile.ContentType, "image/\\S+") &&
                flArchivoImagen.PostedFile.ContentLength > 0)
            {
                HttpCookie ckSIAN = Request.Cookies["userCng"];
                int max_pixeles = 600;
                System.Drawing.Image img = System.Drawing.Image.FromStream(flArchivoImagen.PostedFile.InputStream);
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
                string strNombreArchivo = "lst_" +
                                    (int.Parse(ckSIAN["ck_cliente"])).ToString("000") + "_" +
                                    (int.Parse(this.hdID.Value)).ToString("000000") + "-" +
                                    newWidth.ToString() + "x" +
                                    newHeigth.ToString() + ".jpg";

                try
                {
                    if (File.Exists(Server.MapPath("../fotos") + "/" + this.imgListaPrecio.ImageUrl.Replace("~/fotos/", "")))
                        File.Delete(Server.MapPath("../fotos") + "/" + this.imgListaPrecio.ImageUrl.Replace("~/fotos/", ""));
                    bitmap.Save(Server.MapPath("../fotos") + "/" + strNombreArchivo, System.Drawing.Imaging.ImageFormat.Jpeg);

                    string strQuery = "UPDATE listas_precios SET " +
                            "imagen = '" + strNombreArchivo + "'" +
                            " WHERE ID = " + this.hdID.Value;
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
                if (string.IsNullOrEmpty(strMensaje))
                {
                    max_pixeles = 200;
                    if (newWidth > max_pixeles || newHeigth > max_pixeles)
                    {
                        double relacion = 0;
                        if (newWidth > newHeigth)
                            relacion = max_pixeles / (double)newWidth;
                        else
                            relacion = max_pixeles / (double)newHeigth;
                        newWidth = (int)(newWidth * relacion);
                        newHeigth = (int)(newHeigth * relacion);
                    }
                    this.imgListaPrecio.ImageUrl = "../fotos/" + strNombreArchivo;
                    this.imgListaPrecio.Width = newWidth;
                    this.imgListaPrecio.Height = newHeigth;
                    this.pnlDatos.Visible = true;
                    this.pnlDatos2.Visible = true;
                    this.pnlArchivo.Visible = false;
                    Llenar_Grid(string.Empty);
                }
            }
            else
                strMensaje = "Archivo no es un archivo con una imagen";
        }
        else
            strMensaje = "Seleccione un archivo";

        if (!string.IsNullOrEmpty(strMensaje))
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);
    }

    protected void btnCancelarImagen_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlArchivo.Visible = false;
    }

    protected void btnAgregarPrecio_Click(object sender, EventArgs e)
    {
        this.lblNombre.Text = this.txtNombre.Text.Trim();
        this.txtPrecios.Text = string.Empty;
        this.txtPrecios.Enabled = true;
        this.btnValidarPrecio.Visible = true;
        this.btnGuardarPrecio.Visible = false;
        this.pnlPrecio.Visible = true;
        this.pnlDatos.Visible = false;
        this.pnlDatos2.Visible = false;
    }

    protected void btnValidarPrecio_Click(object sender, EventArgs e)
    {
        string strMensaje = string.Empty;

        if (!string.IsNullOrEmpty(this.txtPrecios.Text.Trim()))
        {
            string[] strLineas = this.txtPrecios.Text.Trim().Split('\n');

            int i = 1;
            foreach (string strLinea in strLineas)
            {
                if(!string.IsNullOrEmpty(strLinea))
                    strMensaje += Validar_Linea(strLinea, i);
                i++;
            }

        }
        else
            strMensaje = "Ingrese los precios";

        if (string.IsNullOrEmpty(strMensaje))
        {
            this.txtPrecios.Enabled = false;
            this.btnValidarPrecio.Visible = false;
            this.btnGuardarPrecio.Visible = true;
            ((master_MasterPage)Page.Master).MostrarMensajeError("Datos validados, presione el botón Guardar para continuar");
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError(strMensaje);

    }

    public string Validar_Linea(string strLinea, int i)
    {
        string strMensaje = string.Empty;

        char[] chSeparador = new char[] {'\t'};
        if (this.rdSeparador.SelectedIndex == 1)
            chSeparador[0] = ';';
        else
            if (this.rdSeparador.SelectedIndex == 2)
                chSeparador[0] = '|';

        if (!strLinea.Contains(chSeparador[0].ToString()))
            strMensaje = "Linea " + i.ToString() + @" no está en el formato correcto<br />";
        else
        {
            string[] strDatos = strLinea.Split(chSeparador);
            if (strDatos.Length != 2)
                strMensaje = "Linea " + i.ToString() + @" está incompleta<br />";
            else
            {
                decimal dbPrecio = 0;
                DataSet objDataResult = new DataSet();
                string strQuery = "SELECT 1 " +
                                " FROM productos " +
                                " WHERE (codigo = '" + strDatos[0].Trim() + "'" +
                                "    OR codigo2 = '" + strDatos[0].Trim() + "'" +
                                "    OR codigo3 = '" + strDatos[0].Trim() + "'" +
								"    )" +
							    "    AND activo = 1";
                objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

                if (objDataResult.Tables[0].Rows.Count == 0)
                    strMensaje += "Linea " + i.ToString() + @" Clave no es existe en el sistema<br />";

                if (!decimal.TryParse(strDatos[1].Trim().Replace("$", "").Replace(",", ""), out dbPrecio))
                    strMensaje += "Linea " + i.ToString() + @" Precio no es numérico<br />";
            }

        }

        return strMensaje;
    }

    protected void btnGuardarPrecio_Click(object sender, EventArgs e)
    {
        string strMensaje = string.Empty;

        if (!string.IsNullOrEmpty(this.txtPrecios.Text.Trim()))
        {
            string[] strLineas = this.txtPrecios.Text.Trim().Split('\n');

            foreach (string strLinea in strLineas)
            {
                if (!string.IsNullOrEmpty(strLinea))
                    Guardar_Precio_Nuevo(strLinea);
            }

        }
        Llenar_Productos();
        ((master_MasterPage)Page.Master).MostrarMensajeError("Precios fueron guardados");

        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlPrecio.Visible = false;
    }

    private void Guardar_Precio_Nuevo(string strLinea)
    {
        DataSet objDataResult = new DataSet();
        char[] chSeparador = new char[] { '\t' };
        if (this.rdSeparador.SelectedIndex == 1)
            chSeparador[0] = ';';
        else
            if (this.rdSeparador.SelectedIndex == 2)
                chSeparador[0] = '|';

        string[] strDatos = strLinea.Split(chSeparador);

        string strQuery = "SELECT ID " +
                         " FROM productos " +
                         " WHERE (codigo = '" + strDatos[0].Trim() + "'" +
                         "    OR codigo2 = '" + strDatos[0].Trim() + "'" +
                         "    OR codigo3 = '" + strDatos[0].Trim() + "'" +
						 "    )" +
					     "    AND activo = 1";
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        DataRow objRowResult = objDataResult.Tables[0].Rows[0];

        strQuery = "UPDATE precios " +
                " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                " WHERE lista_precios_ID = " + this.hdID.Value +
                " AND producto_ID = " + objRowResult["ID"].ToString() +
                " AND validez = '2099-12-31'";

        CComunDB.CCommun.Ejecutar_SP(strQuery);

        strQuery = "INSERT INTO precios (producto_ID, lista_precios_ID, precio_caja, " +
                "precio_unitario, validez_desde, validez, creadoPorID, creadoPorFecha) VALUES (" +
                "'" + objRowResult["ID"].ToString() + "'" +
                ", '" + this.hdID.Value + "'" +
                ", '" + strDatos[1].Trim().Replace("$", "").Replace(",", "").Replace("\n", "") + "'" +
                ", '" + strDatos[1].Trim().Replace("$", "").Replace(",", "").Replace("\n", "") + "'" +
                ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                ", '2099-12-31'" +
                ", '" + Session["SIANID"].ToString() + "'" +
                ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                ")";
        CComunDB.CCommun.Ejecutar_SP(strQuery);
    }

    protected void btnCancelarPrecio_Click(object sender, EventArgs e)
    {
        this.pnlDatos.Visible = true;
        this.pnlDatos2.Visible = true;
        this.pnlPrecio.Visible = false;
    }

    protected void btnAgregarProd_Click(object sender, ImageClickEventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdProductoID.Value))
        {
            if (!Buscar_Producto())
                ((master_MasterPage)Page.Master).MostrarMensajeError("Seleccione un producto de la lista");
            return;
        }

        decimal dcmPrecio = 0;
        if (decimal.TryParse(this.txtPrecioUnitario.Text.Trim(), out dcmPrecio))
        {
            this.txtPrecioUnitario.Text = dcmPrecio.ToString();
            Actualizar_Precio();
        }
        else
            ((master_MasterPage)Page.Master).MostrarMensajeError("Precio unitario debe ser numérico");
    }

    private bool Buscar_Producto()
    {
        if (!string.IsNullOrEmpty(this.txtProducto.Text.Trim()))
        {
            string strQuery = "SELECT R.*, IFNULL(D.existencia, 0) as existencia " +
                             " FROM (" +
                             "    SELECT P.ID, nombre" +
                             "    FROM productos P " +
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
                string strClientScript = "setTimeout('setProductoPrecio()',100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "strFoco", strClientScript, true);
                return true;
            }
        }
        return false;
    }

    private void Actualizar_Precio()
    {
        CComunDB.CCommun.Actualizar_Precio(this.hdProductoID.Value,
                                           this.hdID.Value,
                                           decimal.Parse(this.txtPrecioUnitario.Text));

        ((master_MasterPage)Page.Master).MostrarMensajeError("Producto ha sido agregado");

        this.txtPrecioUnitario.Text = string.Empty;
        this.txtProducto.Text = string.Empty;
        this.hdProductoID.Value = string.Empty;

        Llenar_Productos();
    }

    protected void gvProductos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((HiddenField)e.Row.Cells[8].FindControl("hdParcial")).Value.Equals("1"))
            {
                e.Row.Cells[0].ForeColor = e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (this.hdUsuPr.Value.Equals("0"))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("No tiene permisos para ejecutar esta operación");
            return;
        }

        if (e.CommandName == "Borrar")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            string strQuery = "UPDATE precios " +
                " SET validez = '" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'" +
                " WHERE lista_precios_ID = " + this.hdID.Value +
                " AND producto_ID = " + this.gvProductos.DataKeys[index].Value.ToString() +
                " AND validez = '2099-12-31'";

            try
            {
                CComunDB.CCommun.Ejecutar_SP(strQuery);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error: " + ex.Message);
            }
            ((master_MasterPage)Page.Master).MostrarMensajeError("Producto ha sido eliminado");
            Llenar_Productos();
        }
    }

    private void Llenar_Productos()
    {
        this.gvProductos.DataSource = ObtenerProductos();
        this.gvProductos.DataBind();

        this.btnFirstPage.Visible = false;
        this.btnLastPage.Visible = false;
        this.btnNextPage.Visible = false;
        this.btnPrevPage.Visible = false;
        if (this.gvProductos.Rows.Count == 0)
        {
            this.lblPagina.Text = "";
            this.lblPaginatot.Text = "";
            this.txtPag.Visible = false;
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

    protected void btnFirstPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = 1;
        Llenar_Productos();
    }

    protected void btnNextPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = int.Parse(ViewState["PagActual"].ToString()) + 1;
        Llenar_Productos();
    }

    protected void btnPrevPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = int.Parse(ViewState["PagActual"].ToString()) - 1; ;
        Llenar_Productos();
    }

    protected void btnLastPage_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["PagActual"] = ViewState["PagTotal"];
        Llenar_Productos();
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
                Llenar_Productos();
            }
        }
        this.txtPag.Focus();
    }
}
