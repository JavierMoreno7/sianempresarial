using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public partial class compras_compras_correo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["SIANID"] == null ||
                Request.QueryString["c"] == null ||
                Request.QueryString["f"] == null)
            {
                this.pnlDatos.Visible = false;
                return;
            }

            bool swVer, swTot;

            if (!CComunDB.CCommun.ValidarAcceso(1101, out swVer, out swTot))    //Listas de precios de compras
            {
                this.pnlDatos.Visible = false;
                return;
            }
            Obtener_Datos();
        }
    }

    private void Obtener_Datos()
    {
        string strQuery = "SELECT emailfrom, emailsubject, mensaje " +
                        " FROM mensajes_correo";
        if (Request.QueryString["t"].Equals("o"))
            strQuery += " WHERE ID = 5";
        else
            strQuery += " WHERE ID = 4";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.hdDe.Value = objDataResult.Tables[0].Rows[0][0].ToString();

        if (Request.QueryString["t"].Equals("o"))
        {
            strQuery = "SELECT P.email" +
                      " FROM compra_orden C " +
                      " INNER JOIN proveedores P " +
                      " ON C.proveedorID = P.ID " +
                      "    AND C.ID = " + Request.QueryString["c"];

            this.txtAsunto.Text = "Envío orden de compra " + Request.QueryString["c"];
            this.txtMensaje.Text = objDataResult.Tables[0].Rows[0][2].ToString().Replace("#", "orden de compra");
            this.hdTipo.Value = "3";
        }
        else
            if (Request.QueryString["t"].Equals("c"))
            {
                strQuery = "SELECT P.email" +
                          " FROM compra C " +
                          " INNER JOIN proveedores P " +
                          " ON C.proveedorID = P.ID " +
                          "    AND C.ID = " + Request.QueryString["c"];

                this.txtAsunto.Text = "Envío compra " + Request.QueryString["c"];
                this.txtMensaje.Text = objDataResult.Tables[0].Rows[0][2].ToString().Replace("#", "compra");
                this.hdTipo.Value = "4";
            }
            else
            {
                strQuery = "SELECT P.email" +
                          " FROM compra_dev C " +
                          " INNER JOIN proveedores P " +
                          " ON C.proveedorID = P.ID " +
                          "    AND C.ID = " + Request.QueryString["c"];

                this.txtAsunto.Text = "Envío devolución " + Request.QueryString["c"];
                this.txtMensaje.Text = objDataResult.Tables[0].Rows[0][2].ToString().Replace("#", "devolución");
                this.hdTipo.Value = "5";
            }
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.txtPara.Text = objDataResult.Tables[0].Rows[0][0].ToString();

        if (!File.Exists(Server.MapPath("../xml_facturas" + "/" +
                         Request.QueryString["f"])))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Archivo no existe, contacte al administrador");
            this.btnEnviar.Enabled = false;
            return;
        }
        FileInfo flXLS = new FileInfo(Server.MapPath("../xml_facturas" + "/" +
                                      Request.QueryString["f"]));
        this.lblXLS.Text = Request.QueryString["f"] + " (" + (flXLS.Length / 1024).ToString("0.0") + "KB)";
    }

    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.txtPara.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Dirección de correo es requerida");
            return;
        }

        if (!Validar_Email(this.txtPara.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Dirección de correo no es válida");
            return;
        }

        if (!string.IsNullOrEmpty(this.txtCC.Text.Trim()) &&
            !Validar_Email(this.txtCC.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Dirección de correo (cc) no es válida");
            return;
        }

        if (string.IsNullOrEmpty(this.txtAsunto.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Asunto es requerido");
            return;
        }

        if (Enviar_Correo())
        {
            string strQuery = "SELECT 1" +
                             " FROM correo_envio" +
                             " WHERE ID = " + Request.QueryString["c"] +
                             "   AND tipo = " + this.hdTipo.Value;
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count == 0)
                strQuery = "INSERT INTO correo_envio (ID, tipo, fecha_envio, email) VALUES(" +
                           Request.QueryString["c"] +
                          ", " + this.hdTipo.Value +
                          ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          ", '" + this.txtPara.Text.Trim().Replace("'", "''") + "'" +
                          ")";
            else
                strQuery = "UPDATE correo_envio SET" +
                          " fecha_envio = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          ",email = '" + this.txtPara.Text.Trim().Replace("'", "''") + "'" +
                          " WHERE ID = " + Request.QueryString["c"] +
                          "   AND tipo = " + this.hdTipo.Value;
            CComunDB.CCommun.Ejecutar_SP(strQuery);

            mdEnviado.Show();
        }
    }

    private bool Validar_Email(string strTexto)
    {
        char[] chSeparador = new char[] { ' ' };
        if (strTexto.Contains(","))
            chSeparador[0] = ',';
        else
            if (strTexto.Contains(";"))
                chSeparador[0] = ';';

        string[] strEmails = strTexto.Split(chSeparador);

        int intValidos = 0;
        foreach (string strEmail in strEmails)
        {
            if (!string.IsNullOrEmpty(strEmail))
                if (!CRutinas.Validar_Email(strEmail))
                    return false;
                else
                    intValidos++;
        }

        if (intValidos > 0)
            return true;
        else
            return false;
    }

    private bool Enviar_Correo()
    {
        CCorreo objCorreo = new CCorreo();
        objCorreo.strAsunto = this.txtAsunto.Text.Trim();
        objCorreo.strDe = this.hdDe.Value;

        objCorreo.strMensaje = this.txtMensaje.Text.Trim();

        char[] chSeparador = new char[] { ' ' };
        if (this.txtPara.Text.Trim().Contains(","))
            chSeparador[0] = ',';
        else
            if (this.txtPara.Text.Trim().Contains(";"))
                chSeparador[0] = ';';

        string[] strEmailsPara = this.txtPara.Text.Trim().Split(chSeparador);

        foreach (string strEmail in strEmailsPara)
        {
            if (!string.IsNullOrEmpty(strEmail))
                objCorreo.strPara.Add(strEmail);
        }

        if (!string.IsNullOrEmpty(this.txtCC.Text.Trim()))
        {
            chSeparador[0] = ' ';
            if (this.txtCC.Text.Trim().Contains(","))
                chSeparador[0] = ',';
            else
                if (this.txtCC.Text.Trim().Contains(";"))
                    chSeparador[0] = ';';

            string[] strEmailsCC = this.txtCC.Text.Trim().Split(chSeparador);

            foreach (string strEmail in strEmailsCC)
            {
                if (!string.IsNullOrEmpty(strEmail))
                    objCorreo.strCC.Add(strEmail);
            }
        }

        objCorreo.strAttachments.Add(Server.MapPath("../xml_facturas" + "/" + Request.QueryString["f"]));

        string strMensaje = string.Empty;

        if (objCorreo.Enviar(out strMensaje))
            return true;
        else
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError(strMensaje);
            return false;
        }
    }

    protected void btnEnviadoContinuar_Click(object sender, EventArgs e)
    {
        string strClientScript = "cerrarPopUp();";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strCerrar", strClientScript, true);
    }
}
