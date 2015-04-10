using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class facturas_factura_xml : BasePagePopUp
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strArchivoXML = Server.MapPath("../xml_facturas" + HttpContext.Current.Request.ApplicationPath + "/CFDI_" +
                                                Request.QueryString["fact"]);
        if (!File.Exists(strArchivoXML))
        {
            this.lblMessage.Text = "Factura Electrónica no existe";
            return;
        }

        Response.Clear();
        Response.ContentType = "text/xml";
        Response.AppendHeader("Content-Disposition",
                String.Format("attachment;filename={0}", Request.QueryString["fact"]));
        Response.TransmitFile(strArchivoXML);
        Response.End();
    }
}
