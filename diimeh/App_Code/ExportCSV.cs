using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

namespace ExportExcel
{
    /// <summary>
    /// Summary description for ExportCSV
    /// </summary>
    public class ExportCSV
    {
        public void WriteToCSV(DataTable Info, DataTable Articulos, string IDProceso)
        {
            string attachment = string.Format("attachment; filename={0}.csv",string.Format("{0}{1}{2}{3}{4}{5}",IDProceso,DateTime.Now.Millisecond.ToString(),DateTime.Now.Second.ToString(),DateTime.Now.Minute.ToString(),DateTime.Now.Hour.ToString(),DateTime.Now.ToShortDateString()));
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("Pragma", "public");

            WriteColumnName();

            foreach (DataRow row in Info.Rows)
            {
                WriteUserInfo(row);
            }

            WriteColumnNameArt();


            foreach (DataRow rows in Articulos.Rows)
            {
                WriteUserInfoArt(rows);
            }

            HttpContext.Current.Response.End();
        }

        private void WriteUserInfo(DataRow item)
        {
            StringBuilder strb = new StringBuilder();

            AddComma(item["REFERENCIA"].ToString(), strb);
            AddComma(item["FECGEN"].ToString(), strb);
            AddComma(item["FECSUR"].ToString(), strb);
            AddComma(item["TIPOCAMBIO"].ToString(), strb);
            AddComma(item["DOC"].ToString(), strb);
            AddComma(item["COMM"].ToString(), strb);
            AddComma(item["MDESC"].ToString(), strb);
            AddComma(item["MDESCP"].ToString(), strb);
            AddComma(item["MSUBT"].ToString(), strb);
            AddComma(item["MNETO"].ToString(), strb);
            AddComma(item["MIVA"].ToString(), strb);
            AddComma(item["MIVAP"].ToString(), strb);
            AddComma(item["MTOTAL"].ToString(), strb);
            AddComma(item["MSALDO"].ToString(), strb);
            AddComma(item["proveedor"].ToString(), strb);
            AddComma(item["Estatus"].ToString(), strb);
            AddComma(item["TIPO"].ToString(), strb);
            AddComma(item["PAGO"].ToString(), strb);
            AddComma(item["CAJERO"].ToString(), strb);
            AddComma(item["IDIOMAS"].ToString(), strb);

            HttpContext.Current.Response.Write(strb.ToString());
            HttpContext.Current.Response.Write(Environment.NewLine);
        }

        private void WriteUserInfoArt(DataRow item)
        {
            StringBuilder strb = new StringBuilder();

            AddComma(item["ID_ARTICULO"].ToString(), strb);
            AddComma(item["DESCRIPCION"].ToString(), strb);
            AddComma(item["CANTIDAD"].ToString(), strb);
            AddComma(item["PRECIO"].ToString(), strb);
            AddComma(item["SUBTOTAL"].ToString(), strb);
            AddComma(item["CLAVE"].ToString(), strb);

            HttpContext.Current.Response.Write(strb.ToString());
            HttpContext.Current.Response.Write(Environment.NewLine);
        }

        private void AddComma(string item, StringBuilder strb)
        {
            strb.Append(item.Replace(',', ' '));
            strb.Append(" ,");
        }

        private void WriteColumnName()
        {
            string str = "Referencia, FechaGeneracion, FechaSurtirse, TipoCambio, Documento, Comentario, MontoDescuento, MontoDescuentoPorcentaje, MontoSubtotal, MontoNeto, MontoIva, MontoIvaPorcentaje, MontoTotal, MontoSaldo, Proveedor, Estatus, TipoDeMoneda, TipoDePago, Cajero, TipoDeIdioma";
            HttpContext.Current.Response.Write(str);
            HttpContext.Current.Response.Write(Environment.NewLine);
        }

        private void WriteColumnNameArt()
        {
            string str = "ID_ARTICULO, Descripcion, Cantidad, Precio, SubTotal, Clave";
            HttpContext.Current.Response.Write(str);
            HttpContext.Current.Response.Write(Environment.NewLine);
        }
    }
}
