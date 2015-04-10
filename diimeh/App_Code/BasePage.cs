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
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
	public BasePage()
	{

	}

    void Page_PreInit(object sender, EventArgs e)
    {
        if (Session["SIANID"] == null)
        {
            Response.Redirect("./../default.aspx");
        }
    }
}
