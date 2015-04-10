using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

public partial class admin_admin : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool swVer, swTot;
            if (!CComunDB.CCommun.ValidarAcceso(90000, out swVer, out swTot))
                Response.Redirect("../inicio/error.aspx");
            if (int.Parse(Session["SIANID"].ToString()) != 1)
            {
                this.btnCobranza.Visible = false;
            }
        }
    }

    protected void btnCobranza_Click(object sender, EventArgs e)
    {
        int intFacturaID;

        if (!int.TryParse(this.txtFactura.Text.Trim(), out intFacturaID))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Número de factura/remisión/compra no válido");
            return;
        }

        DataSet objDataResult = new DataSet();
        string strQuery;
        if (this.rdFactNot.SelectedIndex == 0)
            strQuery = "SELECT 1 " +
                       " FROM facturas_liq F" +
                       " WHERE F.ID = " + intFacturaID +
                       " AND F.status = 0";
        else
            if (this.rdFactNot.SelectedIndex == 1)
                strQuery = "SELECT 1" +
                           " FROM notas F" +
                           " WHERE F.ID = " + intFacturaID +
                           " AND F.status = 0";
            else
            {
                ((master_MasterPage)Page.Master).MostrarMensajeError("Sólo se puede hacer el cambio en una factura o remisión");
                return;
            }
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Factura/Remisión no existe, está en proceso o ya está cancelada");
            return;
        }

        if (this.rdFactNot.SelectedIndex == 0)
            strQuery = "UPDATE facturas_liq";
        else
            strQuery = "UPDATE notas";

        strQuery += " SET status=1, contado=0 " +
                   ", fecha_contrarecibo='" + DateTime.Today.ToString("yyyy-MM-dd") + "'" +
                   " WHERE ID = " + intFacturaID;

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        ((master_MasterPage)Page.Master).MostrarMensajeError("Factura/Remisión " + intFacturaID + " ha sido cambiada");

        this.txtFactura.Text = string.Empty;
    }

    protected void btnEnProceso_Click(object sender, EventArgs e)
    {
        int intFacturaID;

        if (!int.TryParse(this.txtFactura.Text.Trim(), out intFacturaID))
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Número no válido");
            return;
        }

        DataSet objDataResult = new DataSet();
        string strQuery = string.Empty;
        switch (this.rdFactNot.SelectedValue)
        {
            case "1":
                strQuery = "SELECT 1 " +
                          " FROM facturas_liq F" +
                          " WHERE F.ID = " + intFacturaID +
                          " AND F.status <> 9";
                break;
            case "2":
                strQuery = "SELECT 1" +
                          " FROM notas F" +
                          " INNER JOIN sucursales S" +
                          " WHERE F.ID = " + intFacturaID +
                          " AND F.status <> 9" +
                          " AND S.ID = F.sucursal_ID " +
                          " AND S.establecimiento_ID <> 0";
                break;
            case "3":
                strQuery = "SELECT 1" +
                          " FROM compra F" +
                          " WHERE F.ID = " + intFacturaID +
                          " AND F.estatus <> 9";
                break;
            case "4":
                strQuery = "SELECT 1" +
                          " FROM orden_compra F" +
                          " WHERE F.ID = " + intFacturaID +
                          " AND F.estatus <> 3" +
                          " AND F.estatus <> 9";
                break;
        }
        try
        {
            objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        if (objDataResult.Tables[0].Rows.Count == 0)
        {
            ((master_MasterPage)Page.Master).MostrarMensajeError("Factura/Remisión/Compra/Pedido no existe o ya está cancelada o surtido");
            return;
        }

        switch (this.rdFactNot.SelectedValue)
        {
            case "1":
                strQuery = "UPDATE facturas_liq" +
                          " SET status = 8 " +
                          " WHERE ID = " + intFacturaID;
                break;
            case "2":
                strQuery = "UPDATE notas" +
                          " SET status = 8 " +
                          " WHERE ID = " + intFacturaID;
                break;
            case "3":
                strQuery = "UPDATE compra" +
                          " SET estatus = 8 " +
                          " WHERE ID = " + intFacturaID;
                break;
            case "4":
                strQuery = "UPDATE orden_compra" +
                          " SET estatus = 1 " +
                          " WHERE ID = " + intFacturaID;
                break;
        }

        try
        {
            CComunDB.CCommun.Ejecutar_SP(strQuery);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Error: " + ex.Message + strQuery);
        }

        switch (this.rdFactNot.SelectedValue)
        {
            case "1":
                ((master_MasterPage)Page.Master).MostrarMensajeError("Factura " + intFacturaID + " ha sido cambiada a En Proceso");
                break;
            case "2":
                ((master_MasterPage)Page.Master).MostrarMensajeError("Remisión " + intFacturaID + " ha sido cambiada a En Proceso");
                break;
            case "3":
                ((master_MasterPage)Page.Master).MostrarMensajeError("Compra " + intFacturaID + " ha sido cambiada a En Proceso");
                break;
            case "4":
                ((master_MasterPage)Page.Master).MostrarMensajeError("Pedido " + intFacturaID + " ha sido cambiado a En Proceso");
                break;
        }

        this.txtFactura.Text = string.Empty;
    }
}
