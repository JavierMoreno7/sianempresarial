using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Data;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Summary description for CRutinas
/// </summary>
public class CRutinas
{
    public static DateTime Hora_Actual()
    {
        string strTimeZone;
        if (!CacheManager.ObtenerValor("TZ", out strTimeZone))
        {
            string strQuery = "SELECT valor " +
                             " FROM cat_parametros " +
                             " WHERE ID = 999";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            strTimeZone = objDataResult.Tables[0].Rows[0]["valor"].ToString();

            CacheManager.ColocarValor("TZ", strTimeZone);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                               TimeZoneInfo.FindSystemTimeZoneById(strTimeZone));
    }

    public static DateTime Dia_Actual()
    {
        string strTimeZone;
        if (!CacheManager.ObtenerValor("TZ", out strTimeZone))
        {
            string strQuery = "SELECT valor " +
                             " FROM cat_parametros " +
                             " WHERE ID = 999";

            DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);
            strTimeZone = objDataResult.Tables[0].Rows[0]["valor"].ToString();

            CacheManager.ColocarValor("TZ", strTimeZone);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                               TimeZoneInfo.FindSystemTimeZoneById(strTimeZone)).Date;
    }

    public static string ObtenerImporteLetras(decimal amtMonto)
    {
        string[] strUnidades = new string[9] {
                                    "un", "dos", "tres", "cuatro", "cinco",
                                    "seis", "siete", "ocho", "nueve"
                                       };
        string[] strDiez = new string[9] {
                                    "once", "doce", "trece", "catorce", "quince",
                                    "dieciseis", "diecisiete", "dieciocho", "diecinueve"
                                       };

        string[] strDecenas = new string[9] {
                                    "diez", "veinti", "treinta", "cuarenta", "cincuenta",
                                    "sesenta", "setenta", "ochenta", "noventa"
                                       };

        string[] strCentenas = new string[9] {
                                    "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos",
                                    "seiscientos", "setecientos", "ochocientos", "novecientos"
                                       };

        StringBuilder strCadena = new StringBuilder();
        int intMontoEntero = 0;
        int intDigito = 0;

        if (amtMonto >= 1000)
        {
            intMontoEntero = (int)(amtMonto / 1000);
            if (intMontoEntero >= 100)
            {
                intDigito = (int)(intMontoEntero / 100);
                if (intMontoEntero == 100)
                    strCadena.Append("cien");
                else
                    strCadena.Append(strCentenas[intDigito - 1]);
                strCadena.Append(" ");
                intMontoEntero -= (intDigito * 100);
            }
            if (intMontoEntero >= 30)
            {
                intDigito = (int)(intMontoEntero / 10);
                strCadena.Append(strDecenas[intDigito - 1]);
                strCadena.Append(" ");
                intMontoEntero -= (intDigito * 10);
                if (intMontoEntero > 0)
                {
                    strCadena.Append("y ");
                    strCadena.Append(strUnidades[intMontoEntero - 1]);
                    strCadena.Append(" ");
                }
            }
            else
            {
                if (intMontoEntero >= 20)
                {
                    if (intMontoEntero == 20)
                        strCadena.Append("veinte");
                    else
                    {
                        intMontoEntero -= 20;
                        strCadena.Append(strDecenas[1]);
                        strCadena.Append(strUnidades[intMontoEntero - 1]);
                    }
                    strCadena.Append(" ");
                }
                else
                {
                    if (intMontoEntero >= 10)
                    {
                        if (intMontoEntero == 10)
                            strCadena.Append(strDecenas[0]);
                        else
                        {
                            intMontoEntero -= 10;
                            strCadena.Append(strDiez[intMontoEntero - 1]);
                        }
                        strCadena.Append(" ");
                    }
                    else
                    {
                        if (intMontoEntero > 0)
                        {
                            strCadena.Append(strUnidades[intMontoEntero - 1]);
                            strCadena.Append(" ");
                        }
                    }
                }
            }
            strCadena.Append("mil ");
            intMontoEntero = (int)(amtMonto - ((int)(amtMonto / 1000) * 1000));
        }
        else
            intMontoEntero = (int)amtMonto;


        if (intMontoEntero >= 100)
        {
            intDigito = (int)(intMontoEntero / 100);
            if (intMontoEntero == 100)
                strCadena.Append("cien");
            else
                strCadena.Append(strCentenas[intDigito - 1]);
            strCadena.Append(" ");
            intMontoEntero -= (intDigito * 100);
        }

        if (intMontoEntero >= 30)
        {
            intDigito = (int)(intMontoEntero / 10);
            strCadena.Append(strDecenas[intDigito - 1]);
            strCadena.Append(" ");
            intMontoEntero -= (intDigito * 10);
            if (intMontoEntero > 0)
            {
                strCadena.Append("y ");
                strCadena.Append(strUnidades[intMontoEntero - 1]);
                strCadena.Append(" ");
            }
        }
        else
        {
            if (intMontoEntero >= 20)
            {
                if (intMontoEntero == 20)
                    strCadena.Append("veinte");
                else
                {
                    intMontoEntero -= 20;
                    strCadena.Append(strDecenas[1]);
                    strCadena.Append(strUnidades[intMontoEntero - 1]);
                }
                strCadena.Append(" ");
            }
            else
            {
                if (intMontoEntero >= 10)
                {
                    if (intMontoEntero == 10)
                        strCadena.Append(strDecenas[0]);
                    else
                    {
                        intMontoEntero -= 10;
                        strCadena.Append(strDiez[intMontoEntero - 1]);
                    }
                    strCadena.Append(" ");
                }
                else
                {
                    if (intMontoEntero > 0)
                    {
                        strCadena.Append(strUnidades[intMontoEntero - 1]);
                        strCadena.Append(" ");
                    }
                }
            }
        }

        if (amtMonto < 2)
            if (amtMonto < 1)
                strCadena.Append("cero pesos ");
            else
                strCadena.Append("peso ");
        else
            strCadena.Append("pesos ");

        intMontoEntero = (int)((amtMonto - (int)amtMonto) * 100);

        strCadena.Append(intMontoEntero.ToString("00"));
        strCadena.Append("/100 M.N.");

        return strCadena.ToString();
    }

    public static string PrimeraLetraMayuscula(string strCadena)
    {
        if (string.IsNullOrEmpty(strCadena))
            return string.Empty;

        strCadena = strCadena.ToLower();

        char[] chLetras = strCadena.ToCharArray();

        chLetras[0] = char.ToUpper(chLetras[0]);

        return new string(chLetras);
    }

    public static bool FechaValida(string strFecha, out DateTime dtFecha)
    {
        dtFecha = DateTime.Today;

        int valor;
        string[] strValores = strFecha.Split('/');
        if (strValores.Length != 3)
        {
            string[] strValores2 = strFecha.Split('-');
            if (strValores2.Length != 3)
                return false;
            if (!int.TryParse(strValores2[0], out valor) ||
                !int.TryParse(strValores2[1], out valor) ||
                !int.TryParse(strValores2[2], out valor))
                return false;
            if (!DateTime.TryParse(strValores2[2] + "-" + strValores2[1] + "-" + strValores2[0], out dtFecha))
                return false;
        }
        else
        {
            if (!int.TryParse(strValores[0], out valor) ||
                !int.TryParse(strValores[1], out valor) ||
                !int.TryParse(strValores[2], out valor))
                return false;
            if (!DateTime.TryParse(strValores[2] + "-" + strValores[1] + "-" + strValores[0], out dtFecha))
                return false;
        }

        return true;
    }

    public static bool Validar_RFC(string strRFC)
    {
        strRFC = strRFC.ToUpper();

        if (!Regex.IsMatch(strRFC, @"^[A-ZÑ&]{4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z0-9]{3}$") &&
            !Regex.IsMatch(strRFC, @"^[A-ZÑ&]{3}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z0-9]{3}$"))
            return false;
        else
            return true;
    }

    public static bool Validar_Email(string strEmail)
    {
        if (!Regex.IsMatch(strEmail, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            return false;
        else
            return true;
    }

    public static void Enviar_Correo(string strID, string strMensaje)
    {
        string strQuery = "SELECT emailfrom, emailto, emailsubject, mensaje " +
                         " FROM mensajes_correo" +
                         " WHERE ID = " + strID;

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        CCorreo objCorreo = new CCorreo();
        objCorreo.strAsunto = objDataResult.Tables[0].Rows[0]["emailsubject"].ToString();
        objCorreo.strDe = objDataResult.Tables[0].Rows[0]["emailfrom"].ToString();

        string[] strEmailsPara = objDataResult.Tables[0].Rows[0]["emailto"].ToString().Split(';');

        foreach (string strEmail in strEmailsPara)
        {
            if (!string.IsNullOrEmpty(strEmail))
                objCorreo.strPara.Add(strEmail);
        }

        objCorreo.strMensaje = objDataResult.Tables[0].Rows[0]["mensaje"].ToString() + strMensaje;

        string strError = string.Empty;
        objCorreo.Enviar(out strError);
    }

    public static float Calcular_Relacion(int intMaxWidth, int intMaxHeight, int intLogoWidth, int intLogoHeight)
    {
        float flRelacion = 1f;

        //Si alguno es más grande que los máximos
        if (intLogoWidth > intMaxWidth || intLogoHeight > intMaxHeight)
        {
            // Si ambos son más grandes, se toma el maximo menor para hacer el cálculo
            if (intLogoWidth > intMaxWidth && intLogoHeight > intMaxHeight)
            {
                if(intMaxHeight > intMaxWidth)  // El ancho es menor, así que se usa el ancho
                    flRelacion = intMaxWidth / (float)intLogoWidth;
                else
                    flRelacion = intMaxHeight / (float)intLogoHeight;
            }
            else
                if (intLogoWidth > intMaxWidth)
                    flRelacion = intMaxWidth / (float)intLogoWidth;
                else
                    flRelacion = intMaxHeight / (float)intLogoHeight;
        }

        return flRelacion;
    }

    private static byte[] Encrypt(byte[] clearText, byte[] Key, byte[] IV)
    {
        MemoryStream ms = new MemoryStream();
        Rijndael alg = Rijndael.Create();
        alg.Key = Key;
        alg.IV = IV;
        CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(clearText, 0, clearText.Length);
        cs.Close();
        byte[] encryptedData = ms.ToArray();
        return encryptedData;
    }

    public static string Encrypt(string clearText, string Password)
    {
        byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
        return HttpServerUtility.UrlTokenEncode(encryptedData);
    }

    private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
    {
        MemoryStream ms = new MemoryStream();
        Rijndael alg = Rijndael.Create();
        alg.Key = Key;
        alg.IV = IV;
        CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherData, 0, cipherData.Length);
        cs.Close();
        byte[] decryptedData = ms.ToArray();
        return decryptedData;
    }

    public static string Decrypt(string cipherText, string Password)
    {
        try
        {
            byte[] cipherBytes = HttpServerUtility.UrlTokenDecode(cipherText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }
        catch
        {
            return string.Empty;
        }
    }
}

public class CCorreo
{
    public string strAsunto { get;  set; }
    public string strMensaje { get;  set; }
    public string strDe { get;  set; }
    public List<string> strPara { get;  set; }
    public List<string> strCC { get; set; }
    public List<string> strAttachments { get; set; }

    public bool Enviar(out string strError)
    {
        strError = string.Empty;

        if (string.IsNullOrEmpty(strAsunto))
        {
            strError = "Asunto es requerido";
            return false;
        }

        if (string.IsNullOrEmpty(strDe))
        {
            strError = "Dirección de correo DE es requerida";
            return false;
        }

        if (!CRutinas.Validar_Email(strDe))
        {
            strError = "Dirección de correo DE no es válida";
            return false;
        }

        if (strPara.Count == 0)
        {
            strError = "Dirección de correo PARA es requerida";
            return false;
        }

        foreach (string strEmail in strPara)
        {
            if (!CRutinas.Validar_Email(strEmail))
            {
                strError = "Dirección de correo PARA no es válida";
                return false;
            }
        }

        if (strPara.Count != 0)
            foreach (string strEmail in strCC)
            {
                if (!CRutinas.Validar_Email(strEmail))
                {
                    strError = "Dirección de correo CC no es válida";
                    return false;
                }
            }

        HttpCookie ckSIAN = System.Web.HttpContext.Current.Request.Cookies["userCng"];
        string img_path = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath) + System.Web.HttpContext.Current.Request.ApplicationPath + ckSIAN["ck_logo"];

        string[] strSize = ckSIAN["ck_logo_size"].Split('x');
        int intLogoWidth = int.Parse(strSize[0]);
        int intLogoHeight = int.Parse(strSize[1]);
        int max_pixeles = 200;

        if (intLogoWidth > max_pixeles || intLogoHeight > max_pixeles)
        {
            double relacion = 0;
            if (intLogoWidth > intLogoHeight)
                relacion = max_pixeles / (double)intLogoWidth;
            else
                relacion = max_pixeles / (double)intLogoHeight;

            intLogoWidth = (int)(intLogoWidth * relacion);
            intLogoHeight = (int)(intLogoHeight * relacion);
        }

        string strImgSize = " height=\"" + intLogoHeight.ToString() +
                         "\" width=\"" + intLogoWidth.ToString() + "\"";

        MailMessage objMailMensaje = new MailMessage();
        objMailMensaje.From = new MailAddress(this.strDe);

        foreach (string strEmail in strPara)
        {
            objMailMensaje.To.Add(new MailAddress(strEmail));
        }

        foreach (string strEmail in strCC)
        {
            objMailMensaje.CC.Add(new MailAddress(strEmail));
        }

        objMailMensaje.Subject = strAsunto.Trim();

        LinkedResource logo = new LinkedResource(img_path);
        logo.ContentId = "companylogo";
        AlternateView av1 = AlternateView.CreateAlternateViewFromString(
                        "<html><body><table><tr><td style='font-size: 11px; font-family: Verdana;'>" +
                       strMensaje.Trim().Replace("\r\n", "<br />") +
                        "</td></tr><tr>" +
                        "<td align='left'><img src=cid:companylogo" + strImgSize + "/></td>" +
                        "</tr></table></body></html>", 
                        null, System.Net.Mime.MediaTypeNames.Text.Html);
        av1.LinkedResources.Add(logo);

        objMailMensaje.AlternateViews.Add(av1);
        objMailMensaje.IsBodyHtml = true;

        foreach (string strAtt in strAttachments)
        {
            try
            {
                if (File.Exists(strAtt))
                    objMailMensaje.Attachments.Add(new Attachment(strAtt));
            }
            catch
            { }
        }

        SmtpClient objSmtp = new SmtpClient();
        try
        {
            objSmtp.Send(objMailMensaje);
            return true;
        }
        catch (Exception ex)
        {
            strError = "No se pudo enviar el correo: " + ex.Message;
        }

        return false;
    }

    public CCorreo()
    {
        strAsunto = string.Empty;
        strMensaje = string.Empty;
        strDe = string.Empty;
        strPara = new List<string>();
        strCC = new List<string>();
        strAttachments = new List<string>();
    }
}