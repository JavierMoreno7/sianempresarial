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

public partial class conge_procesos_Test : System.Web.UI.Page
{
    string VID = string.Empty;
    string consultar = string.Empty;
    string VNombre = string.Empty;

    #region eventos
    protected void Page_Load(object sender, EventArgs e)
    {
        VID = Request.QueryString["ID"];
        VNombre = Request.QueryString["Nombre"].Replace("@apos", "'");

        if (string.IsNullOrEmpty(VID))
        {
            Deshabilitar();
            lblError.Text = "No se determino el Catalogo";
        }
        else
        {

            Habilitar();
            lblCatalogo.Text = "Sucursales de Cliente: " + VNombre;
            Page.Title = "Vista de Sucursales";

            if (!IsPostBack)
            {
                LlenarCampos();
                lblPagina.Text = "Página 1";
                grdvLista.PageIndex = 0;
                ViewState["changesort"] = "YES";
                ViewState["sortDirection"] = "ASC";
                ViewState["sortExpression"] = "ID";
                ViewState["pageindex"] = Convert.ToString(0);
                cargarGrid();
                lbtnRegresar.Attributes.Add("onclick", "window.close();");
                lnkAgregarNuevo.PostBackUrl = string.Format("../estab/sucursal_proc.asp?accion=1&sucID=0&estID={0}&lastid={0}&Nombre={1}", VID, VNombre);
            }
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

    #endregion eventos

    #region basicas

    private void Deshabilitar()
    {
        grdvLista.Enabled = false;
        lblError.Visible = true;
        lnkAgregarNuevo.Visible = false;

    }

    private void Habilitar()
    {
        grdvLista.Enabled = true;
        lblError.Visible = false;
    }

    private void LlenarCampos()
    {
        string vlquery = "SELECT * FROM CONFIG_CATALOGOS WHERE Mostrar=1 AND Catalogo='Sucursal' ORDER BY ID";
        Operaciones.Seleccionar Query = new Operaciones.Seleccionar();
        DataTable vldata = Query.Obtener(vlquery);
        Session["datacols"] = vldata;
    }

    private void cargarGrid()
    {
        consultar = "SELECT '" + VNombre.Replace("'", "@apos") + "' ENOM,ESTABLECIMIENTO_ID EID,SUCURSAL,ID ID_Sucursal,TELEFONO,DIRECCION FROM SUCURSALES WHERE ESTABLECIMIENTO_ID=" + VID + " ORDER BY SUCURSAL";
        Operaciones.Seleccionar Proceso = new Operaciones.Seleccionar();
        DataTable vldata = Proceso.Obtener(consultar);
        if (vldata.Rows.Count == 0)
        {
            lblPagina.Text = "No se encontraron registros";
            //esto lo utilizo para ver el query que regresa
            //lblPagina.Text = consultar;
            lblPaginatot.Text = "";
            //lnkAgregarNuevo.Visible = false;
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
            if (i > 2)
            {
                BoundField vlColumn = new BoundField();
                vlColumn.DataField = row["Campo"].ToString();
                vlColumn.HeaderText = row["Campo"].ToString();
                vlColumn.SortExpression = row["Campo"].ToString();
                grdvLista.Columns.Add(vlColumn);
            }
            else
                if (i == 2)
                {
                    TemplateField vltempf = new TemplateField();
                    vltempf.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, row["Campo"].ToString(), "Sucursal");
                    vltempf.HeaderTemplate = new GridViewTemplate(DataControlRowType.Header, row["Campo"].ToString(), "Sucursal");
                    vltempf.SortExpression = row["Campo"].ToString();
                    grdvLista.Columns.Add(vltempf);
                }
            i++;
        }
    }

    #endregion basicas

}