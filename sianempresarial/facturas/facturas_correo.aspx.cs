using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;

public partial class facturas_facturas_correo : BasePagePopUp
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool swVer, swTot;
            if (Session["SIANID"] == null ||
                Request.QueryString["ID"] == null ||
                Request.QueryString["t"] == null)
            {
                this.pnlDatos.Visible = false;
                return;
            }

            if(!CComunDB.CCommun.ValidarAcceso(6100, out swVer, out swTot) &&
               !CComunDB.CCommun.ValidarAcceso(6105, out swVer, out swTot) &&
               !CComunDB.CCommun.ValidarAcceso(6110, out swVer, out swTot))
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
                        " FROM mensajes_correo" +
                        " WHERE ID = 2";

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.hdDe.Value = objDataResult.Tables[0].Rows[0]["emailfrom"].ToString();
        this.txtDe.Text = objDataResult.Tables[0].Rows[0]["emailsubject"].ToString();
        this.txtMensaje.Text = objDataResult.Tables[0].Rows[0]["mensaje"].ToString();

        if (Request.QueryString["t"].Equals("0"))
        {
            strQuery = "SELECT F.factura as factura, F.folio, E.facturas_email as email, E.RFC " +
                      " FROM facturas_liq F " +
                      " INNER JOIN sucursales S " +
                      " ON F.sucursal_ID = S.ID " +
                      "    AND F.ID = " + Request.QueryString["ID"] +
                      " INNER JOIN establecimientos E " +
                      " ON S.establecimiento_ID = E.ID ";
        }
        else
            if (Request.QueryString["t"].Equals("1"))
            {
                strQuery = "SELECT F.nota as factura, F.nota as folio, E.facturas_email as email, E.RFC " +
                          " FROM notas_cargo F " +
                          " INNER JOIN sucursales S " +
                          " ON F.sucursal_ID = S.ID " +
                          "    AND F.ID = " + Request.QueryString["ID"] +
                          " INNER JOIN establecimientos E " +
                          " ON S.establecimiento_ID = E.ID ";
            }
            else
            {
                strQuery = "SELECT F.nota as factura, F.nota as folio, E.facturas_email as email, E.RFC " +
                          " FROM notas_credito F " +
                          " INNER JOIN sucursales S " +
                          " ON F.sucursal_ID = S.ID " +
                          "    AND F.ID = " + Request.QueryString["ID"] +
                          " INNER JOIN establecimientos E " +
                          " ON S.establecimiento_ID = E.ID ";
            }
        objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        this.hdFact.Value = objDataResult.Tables[0].Rows[0]["factura"].ToString();
        this.txtPara.Text = objDataResult.Tables[0].Rows[0]["email"].ToString();
        if (Request.QueryString["t"].Equals("0"))
        {
            this.txtAsunto.Text = "Envío CFDI " + objDataResult.Tables[0].Rows[0]["rfc"].ToString() +
                                  "_Factura_" + objDataResult.Tables[0].Rows[0]["folio"];
        }
        else
            if (Request.QueryString["t"].Equals("1"))
            {
                this.txtAsunto.Text = "Envío CFDI " + objDataResult.Tables[0].Rows[0]["rfc"].ToString() +
                                      "_Nota_Cargo_" + Request.QueryString["ID"];
            }
            else
            {
                this.txtAsunto.Text = "Envío CFDI " + objDataResult.Tables[0].Rows[0]["rfc"].ToString() +
                                      "_Nota_Credito_" + Request.QueryString["ID"];
            }

        if (!File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                         this.hdFact.Value + ".xml")) ||
            !File.Exists(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/PDF_" +
                         this.hdFact.Value + ".pdf")))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Uno de los archivos no existe, contacte al administrador");
            this.btnEnviar.Enabled = false;
            return;
        }
        FileInfo flPDF = new FileInfo(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/PDF_" +
                                      this.hdFact.Value + ".pdf"));
        FileInfo flXML = new FileInfo(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                      this.hdFact.Value + ".xml"));
        this.lblPDF.Text = "PDF_" + this.hdFact.Value + ".pdf" + " (" + (flPDF.Length / 1024).ToString("0.0") + "KB)";
        this.lblXML.Text = "XML_" + this.hdFact.Value + ".xml" + " (" + (flXML.Length / 1024).ToString("0.0") + "KB)";
    }

    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.txtPara.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Dirección de correo es requerida");
            return;
        }

        if(!Validar_Email(this.txtPara.Text.Trim()))
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("Dirección de correo no es válida");
            return;
        }

        if(!string.IsNullOrEmpty(this.txtCC.Text.Trim()) &&
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
                             " WHERE ID = " + Request.QueryString["ID"] +
                             "   AND tipo = " + Request.QueryString["t"];
            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

            if (objDataResult.Tables[0].Rows.Count == 0)
                strQuery = "INSERT INTO correo_envio (ID, tipo, fecha_envio, email) VALUES(" +
                           Request.QueryString["ID"] +
                          ", " + Request.QueryString["t"] +
                          ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          ", '" + this.txtPara.Text.Trim().Replace("'", "''") + "'" +
                          ")";
            else
                strQuery = "UPDATE correo_envio SET" +
                          " fecha_envio = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                          ",email = '" + this.txtPara.Text.Trim().Replace("'", "''") + "'" +
                          " WHERE ID = " + Request.QueryString["ID"] +
                          "   AND tipo = " + Request.QueryString["t"];
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

        objCorreo.strAttachments.Add(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/PDF_" +
                                                      this.hdFact.Value + ".pdf"));

        objCorreo.strAttachments.Add(Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                                      this.hdFact.Value + ".xml"));

        string strMensaje = string.Empty;

        if (objCorreo.Enviar(out strMensaje))
            return true;
        else
        {
            ((master_MasterPagePopUp)Page.Master).MostrarMensajeError("No se pudo enviar el correo: " + strMensaje);
            return false;
        }
    }

    protected void btnEnviadoContinuar_Click(object sender, EventArgs e)
    {
        string strClientScript = "cerrarPopUp();";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "strCerrar", strClientScript, true);
    }

    protected void txtDe_TextChanged(object sender, EventArgs e)
    {
        this.hdDe.Value = txtDe.Text;
    }
}
