using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for BasePageUsuario
/// </summary>
public class BasePageUsuario : System.Web.UI.Page
{
    public BasePageUsuario()
	{

	}

    void Page_PreInit(object sender, EventArgs e)
    {
        if (Session["SIANID"] == null)
            Response.Redirect("./../default.aspx");

        HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
        if (ckSIAN == null)
            Response.Redirect("./../default.aspx");
        else
        {
            if(((int.Parse(ckSIAN["ck_app"])) % 10) != 0)
                Response.Redirect("./../inicio/errorUsuarios.aspx");
        }
    }
}
