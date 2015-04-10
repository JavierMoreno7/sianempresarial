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
using System.Windows.Forms;
using System.IO;
using Operaciones;
using ConsultaString;
using ColumnaTemplate;
using System.Data.OleDb;

public partial class conge_procesos_Test : BasePage
{
    string Catalogo = string.Empty;
    string consultar = string.Empty;
    string campo = string.Empty;
    string criterio = string.Empty;

    #region eventos
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool swVer, swTot;
            ViewState["changesort"] = "YES";
            ViewState["sortDirection"] = "ASC";
            ViewState["sortExpression"] = "ID";
            ViewState["pageindex"] = Convert.ToString(0);
            string strCatalogo = Request.QueryString["Catalogo"];
            int intPantalla = 0;

            switch (strCatalogo)
            {
                case "Articulos":
                    intPantalla = 1000;
                    break;
                case "Personal":
                    intPantalla = 1300; 
                    break;
                case "Proveedores":
                    intPantalla = 1500; 
                    break;
                case "Clientes":
                    ViewState["sortExpression"] = "Nombre";
                    intPantalla = 1400; 
                    break;
                case "Comisiones":
                    intPantalla = 1200; 
                    break;
                case "Rutas":
                    intPantalla = 1600;
                    break;
                case "Precios":
                    intPantalla = 1100;
                    break;
                case "OrdenesCompra":
                case "Compras":
                case "Devoluciones":
                    intPantalla = 11000;
                    break;
            }

            if (intPantalla != 0)
            {
                if (!CComunDB.CCommun.ValidarAcceso(intPantalla, out swVer, out swTot))
                {
                    Response.Redirect("../inicio/error.aspx");
                }
            }
            else
                Response.Redirect("../inicio/error.aspx");
        }

        Catalogo = Request.QueryString["Catalogo"];
        if (string.IsNullOrEmpty(Catalogo))
        {
            Deshabilitar();
            lblError.Text = "No se determino el Catalogo";
        }
        else
        {

            Habilitar();
            lblCatalogo.Text = Catalogo;

            if (!IsPostBack)
            {
                LlenarCampos(Catalogo);
                campo = lblCampo.SelectedValue;
                criterio = txtCriterio.Text.Trim();
                lblPagina.Text = "Página 1";
                grdvLista.PageIndex = 0;
                cargarGrid(Catalogo, campo, criterio);

                string vlnewwindow = string.Empty;
                vlnewwindow = "window.open('{0}','{1}','width={2}, height={3}, scrollbars={4}, menubar=no, resizable=no'); return false;";

                if (Catalogo == "Articulos")
                {
                    lbtnAgregar.Text = "Agregar Articulo";
                }
                else 
                {
                    lbtnAgregar.Text = "Agregar Registro";
                }

                switch (Catalogo)
                {
                    case "Rutas":
                        vlnewwindow = string.Format(vlnewwindow, "../rutas/rutas_estab.asp?accion=1&rutID=0", "Agrega_Precios", "750", "300", "yes");
                        break;
                    case "Precios":
                        vlnewwindow = string.Format(vlnewwindow, "../conge/preciosLista_crear.asp?accion=1&listaID=0", "Agrega_Precios", "300", "330", "no");
                        break;
                    case "OrdenesCompra":
                        break;
                    case "Compras":
                        break;
                    case "Devoluciones":
                        break;
                    default:
                        vlnewwindow = "void(0);";
                        break;
                }
                lbtnAgregar.Attributes.Add("onclick", vlnewwindow);
            }
        }
    }

    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {
        campo = lblCampo.SelectedValue;
        criterio = txtCriterio.Text.Trim();
        if (criterio == "")
            lbtnMostrarTodos.Visible = false;
        else
        {
            lbtnMostrarTodos.Visible = true;
            ViewState["dropindex"] = lblCampo.SelectedIndex.ToString();
        }
        grdvLista.PageIndex = 0;
        ViewState["changesort"] = "YES";
        ViewState["sortDirection"] = "ASC";
        lblPagina.Text = "Página 1";
        ViewState["pageindex"] = Convert.ToString(0);
        cargarGrid(Catalogo, campo, criterio);
    }

    protected void lbtnMostrarTodos_Click(object sender, EventArgs e)
    {
        lbtnMostrarTodos.Visible = false;
        lblCampo.SelectedIndex = Convert.ToInt32(ViewState["dropindex"]);
        txtCriterio.Text = string.Empty;
        switch (Catalogo)
        {
            case "Articulos":
                campo = "P.ID";
                break;
            case "Personal":
                campo = "A.ID";
                break;
            case "Rutas":
                campo = "R.ID";
                break;
            case "Precios":
                campo = "L.ID";
                break;
            case "OrdenesCompra":
                campo = "A.POC_REFERENCIA";
                break;
            case "Compras":
                campo = "A.COM_REFERENCIA";
                break;
            case "Devoluciones":
                campo = "A.DEV_REFERENCIA";
                break;
            default:
                campo = "ID";
                break;
        }
        criterio = string.Empty;
        grdvLista.PageIndex = 0;
        ViewState["changesort"] = "YES";
        ViewState["sortDirection"] = "ASC";
        lblPagina.Text = "Página 1";
        ViewState["pageindex"] = Convert.ToString(0);
        cargarGrid(Catalogo, campo, criterio);
    }

    protected void lbtnAgregar_Click(object sender, EventArgs e)
    {
        switch (Catalogo)
        {
            case "Articulos":
                Response.Redirect("../conge/productos.asp?parentpage=catalogos");
                break;
            case "Personal":
                Response.Redirect("../conge/vendedores.asp?parentpage=catalogos&");
                break;
            case "OrdenesCompra":
                Response.Redirect("../compras/ProcesoCompras.aspx?Proceso=OrdenDeCompra&Accion=NEW");
                break;
            case "Compras":
                Response.Redirect("../compras/ProcesoCompras.aspx?Proceso=Compras&Accion=NEW");
                break;
            case "Devoluciones":
                Response.Redirect("../compras/ProcesoCompras.aspx?Proceso=Devoluciones&Accion=NEW");
                break;
            case "Proveedores":
                Response.Redirect("../prov/proveedor_proc.asp?accion=1&provID=0");
                break;
            case "Clientes":
                Response.Redirect("../estab/establecimiento_proc.asp?accion=1&estID=0");
                break;
            case "Comisiones":
                Response.Redirect("../conge/comisiones_crear.asp?accion=1&comisionID=0");
                break;
        }
    }

    protected void grdvLista_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int pagei = e.NewPageIndex + 1;
        ViewState["pageindex"] = e.NewPageIndex.ToString();
        ViewState["changesort"] = "NO";
        lblPagina.Text = "Página " + pagei.ToString();
        DataTable dt = Session["SortInfo"] as DataTable;

        if (dt != null)
        {
            creacols();
            grdvLista.PageIndex = e.NewPageIndex;
            dt.DefaultView.Sort = ViewState["sortExpression"].ToString() + " " + GetSortDirection(ViewState["sortExpression"].ToString());
            grdvLista.DataSource = Session["SortInfo"];
            grdvLista.DataBind();
        }
    }

    protected void grdvLista_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    #endregion eventos

    #region basicas

    private void Deshabilitar()
    {
        lblCampo.Enabled = false;
        txtCriterio.Enabled = false;
        btnBuscar.Enabled = false;
        lbtnAgregar.Enabled = false;
        lbtnMostrarTodos.Enabled = false;
        grdvLista.Enabled = false;
        lblError.Visible = true;
    }

    private void Habilitar()
    {
        lblCampo.Enabled = true;
        txtCriterio.Enabled = true;
        btnBuscar.Enabled = true;
        lbtnAgregar.Enabled = true;
        lbtnMostrarTodos.Enabled = true;
        grdvLista.Enabled = true;
        lblError.Visible = false;
    }

    private void LlenarCampos(string catalog)
    {
        int dropindex = 0;
        string vlquery = string.Format("SELECT * FROM CONFIG_CATALOGOS WHERE Mostrar=1 AND Catalogo='{0}' ORDER BY ID", catalog);
        string vlcampo, vlvalor;
        Operaciones.Seleccionar Query = new Operaciones.Seleccionar();
        DataTable vldata = Query.Obtener(vlquery);
        Session["datacols"] = vldata;
        int datacount = vldata.Rows.Count;
        int i = 0;
        ListItemCollection itemfields = new ListItemCollection();
        foreach (DataRow row in vldata.Rows)
        {
            if (row["Campo"].ToString() != "ID")
            {
                vlcampo = row["Campo"].ToString();
                vlvalor = row["Valor"].ToString();
                itemfields.Add(new ListItem(vlcampo, vlvalor));
                switch (catalog)
                {
                    case "Articulos":
                        if (vlcampo == "Nombre")
                            dropindex = i;
                        ViewState["sortExpression"] = "Nombre";
                        break;
                    case "Proveedores":
                        if (vlcampo == "Proveedor")
                            dropindex = i;
                        break;
                    case "Clientes":
                        if (vlcampo == "Nombre")
                            dropindex = i;
                        //ViewState["sortExpression"] = "Nombre";
                        break;
                    case "Personal":
                        if (vlcampo == "Apellidos")
                            dropindex = i;
                        break;
                    case "Comisiones":
                        if (vlcampo == "Concepto")
                            dropindex = i;
                        break;
                    case "Rutas":
                        if (vlcampo == "Ruta")
                            dropindex = i;
                        break;
                    case "Precios":
                        if (vlcampo == "Lista")
                            dropindex = i;
                        break;
                    default:
                        dropindex = i;
                        break;
                }
                i++;
            }
        }
        Session["QFields"]=itemfields;
        foreach (ListItem item1 in itemfields)
           if (item1.Text!="Imagen")
              lblCampo.Items.Add(item1);

        ViewState["dropindex"] = dropindex.ToString();
        lblCampo.SelectedIndex = dropindex;
    }

    private void cargarGrid(string strcatalogo, string strcampo, string strCriterio)
    {
    	ListItemCollection itemfields = Session["QFields"] as ListItemCollection;
        ConsultaString.Consultas Query = new ConsultaString.Consultas();
        consultar = Query.GenerarQuery(strcatalogo, strcampo, strCriterio, itemfields);
        Operaciones.Seleccionar Proceso = new Operaciones.Seleccionar();
        DataTable vldata = Proceso.Obtener(consultar);
        if (vldata.Rows.Count == 0)
        {
            lblPagina.Text = "No se encontraron registros";
            //esto lo utilizo para ver el query que regresa
            //lblPagina.Text = Query.GenerarQuery(strcatalogo, strcampo, strCriterio, itemfields);
            lblPaginatot.Text = "";
        }
        else
        {
            double datacount = (vldata.Rows.Count / 30) + 1;
            lblPaginatot.Text = " de " + datacount.ToString();
            creacols();
            Session["SortInfo"] = vldata;
            grdvLista.DataSource = Session["SortInfo"];
            grdvLista.DataBind();
        }
    }

    protected void grdvLista_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dt = Session["SortInfo"] as DataTable;
        int vlpage = Convert.ToInt32(ViewState["pageindex"].ToString());
        ViewState["changesort"] = "YES";

        if (dt != null)
        {
            grdvLista.PageIndex = vlpage;
            creacols();
            dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
            grdvLista.DataSource = Session["SortInfo"];
            grdvLista.DataBind();
        }
    }

    private string GetSortDirection(string column)
    {
        string sortDirection = ViewState["sortDirection"] as string;
        string sortExpression = ViewState["sortExpression"] as string;
        string sortCols = ViewState["changesort"] as string;
        if (sortCols == "YES")
        {
            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["sortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                        sortDirection = "DESC";
                    else
                        sortDirection = "ASC";
                }
                else
                    sortDirection = "ASC";
            }

            ViewState["sortDirection"] = sortDirection;
            ViewState["sortExpression"] = column;
        }

        return sortDirection;
    }

    private void creacols()
    {
        int i = 0;
        DataTable gcolsinfo = Session["datacols"] as DataTable;
        grdvLista.AutoGenerateColumns = false;

        foreach (DataRow row in gcolsinfo.Rows)
        {
            if (i > 1)
            {
                if (((Catalogo == "Clientes") && (row["Campo"].ToString() == "Sucursal")) ||
                    ((Catalogo == "Precios") && (row["Campo"].ToString() == "Imagen")))
                {
                    TemplateField vltempf = new TemplateField();
                    vltempf.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, row["Campo"].ToString(), Catalogo);
                    vltempf.HeaderTemplate = new GridViewTemplate(DataControlRowType.Header, row["Campo"].ToString(), Catalogo);
                    vltempf.SortExpression = row["Campo"].ToString();
                    grdvLista.Columns.Add(vltempf);
                }
                else
                {
                    BoundField vlColumn = new BoundField();
                    vlColumn.DataField = row["Campo"].ToString();
                    vlColumn.HeaderText = row["Campo"].ToString();
                    vlColumn.SortExpression = row["Campo"].ToString();
                    grdvLista.Columns.Add(vlColumn);
                }
            }
            else
                if (i > 0)
                {
                    TemplateField vltempf = new TemplateField();
                    vltempf.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, row["Campo"].ToString(), Catalogo);
                    vltempf.HeaderTemplate = new GridViewTemplate(DataControlRowType.Header, row["Campo"].ToString(), Catalogo);
                    vltempf.SortExpression = row["Campo"].ToString();
                    grdvLista.Columns.Add(vltempf);
                }
            i++;
        }
    }

    #endregion basicas
}