using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace CPDF
{
    public class MyPageEvents : PdfPageEventHelper
    {
        public string _strTituloReporte;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            int max_pixeles = 120;
            Font[] fonts = new Font[3];
            fonts[0] = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, Font.NORMAL);
            fonts[1] = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.ITALIC);

            Rectangle page = document.PageSize;

            #region Header
            PdfPTable tblHeader = new PdfPTable(1);
            tblHeader.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblHeader.DefaultCell.Border = Rectangle.NO_BORDER;
            tblHeader.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            float[] ancho_columnas = new float[3];
            ancho_columnas[0] = 150;
            ancho_columnas[1] = tblHeader.TotalWidth - 300;
            ancho_columnas[2] = 150;
            PdfPTable tblHeader1 = new PdfPTable(ancho_columnas);

            HttpCookie ckSIAN = HttpContext.Current.Request.Cookies["userCng"];

            int intNombreImagen = ckSIAN["ck_logo"].LastIndexOf("/") + 1;
            Image imgLogo = Image.GetInstance(HttpContext.Current.Server.MapPath("imagenes") +
                "/" + ckSIAN["ck_logo"].Substring(intNombreImagen));

            if (imgLogo.Width > max_pixeles || imgLogo.Height > max_pixeles)
            {
                float relacion = 0;
                if (imgLogo.Width > imgLogo.Height)
                    relacion = max_pixeles / (float)imgLogo.Width;
                else
                    relacion = max_pixeles / (float)imgLogo.Height;
                imgLogo.ScalePercent(relacion * 100);
            }

            PdfPCell celdaImg = new PdfPCell(imgLogo);
            celdaImg.Border = Rectangle.NO_BORDER;
            celdaImg.VerticalAlignment = Element.ALIGN_MIDDLE;
            tblHeader1.AddCell(celdaImg);

            Paragraph texto = new Paragraph(HttpUtility.UrlDecode(ckSIAN["ck_razon"]) + "\n", fonts[0]);
            texto.Add(new Paragraph(_strTituloReporte, fonts[1]));
            PdfPCell celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.VerticalAlignment = Element.ALIGN_MIDDLE;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);

            texto = new Paragraph(DateTime.Today.ToString("dd MMMM yyyy"), fonts[1]);
            celda = new PdfPCell(texto);
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.VerticalAlignment = Element.ALIGN_TOP;
            celda.Border = Rectangle.NO_BORDER;
            tblHeader1.AddCell(celda);
            
            tblHeader.AddCell(tblHeader1);

            #endregion

            #region Footer
            PdfPTable tblFooter = new PdfPTable(1);
            tblFooter.TotalWidth = page.Width - document.LeftMargin - document.RightMargin;
            tblFooter.DefaultCell.Border = Rectangle.NO_BORDER;
            tblFooter.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tblFooter.DefaultCell.BorderWidth = 0;

            texto = new Paragraph("Página " + document.PageNumber, fonts[1]);
            celda = new PdfPCell(texto);
            celda.Padding = 1;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            celda.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            celda.Border = Rectangle.TOP_BORDER;
            tblFooter.AddCell(celda);

            #endregion

            tblHeader.WriteSelectedRows(0, -1, 0, -1, document.LeftMargin, page.Height - document.TopMargin + tblHeader.TotalHeight + 10, writer.DirectContent);

            tblFooter.WriteSelectedRows(0, -1, 0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
        }

    }
    
}
