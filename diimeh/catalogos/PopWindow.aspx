<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopWindow.aspx.cs" Inherits="conge_procesos_Test" Title="SIAN - Sistema de Control de Inventarios" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>SIAN - Sistema de Control de Inventarios</title>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <meta name="Keywords" content="recursos, SERVICIOS, diseño, web, NEXT, Mobile, Solutions, CONSULTORÍA, APLICACIÓN, SOLUCIONES, TECNOLÓGICAS, SIAN" />
    <meta name="Description" content="SIAN - SERVICIOS DE CONSULTORÍA Y APLICACIÓN DE SOLUCIONES TECNOLÓGICAS" />
    <meta name="Author" content="NEXT Mobile Solutions" />
    <meta name="Subject" content="SIAN - Sistema de Control de Inventarios" />
    <meta name="Language" content="es" />
    <meta name="GENERATOR" content="MSHTML 6.00.2716.2200" />
    <meta http-equiv="page-enter" content="blendTrans(Duration=1.0)" />
	<link rel="stylesheet" href="css/congelados.css" type="text/css" />
	<base target="_self" />
    <link href="../css/congelados.css" rel="stylesheet" type="text/css" />
</head>
<body id="top" style="background-image: url(../imagenes/back-contenido.jpg); background-repeat:no-repeat; background-position:center;">
	<form id="form1" runat="server">
    <asp:Panel ID="Panel1" runat="server">
    <table border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td colspan="4" rowspan="1" class="GridFormat">
                <asp:Label ID="lblCatalogo" runat="server" Width="60px" ForeColor="White" Font-Size="X-Small"/></td>
        </tr>
        <tr>
            <td colspan="3" style="font-weight: bold; color: white; height: 24px">
                <asp:LinkButton ID="lbtnRegresar" runat="server" Font-Size="XX-Small" ForeColor="#0000C0" EnableTheming="True" >Regresar</asp:LinkButton></td>
            <td colspan="1" style="font-weight: bold; color: white; height: 24px; text-align: right; width: 302px;">
                <asp:LinkButton ID="lnkAgregarNuevo" runat="server" EnableTheming="True" Font-Size="XX-Small"
                    ForeColor="#0000C0">Agregar Sucursal</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Error" Visible="False" Font-Size="XX-Small"/></td>
        </tr>
        <tr>
            <td colspan="4" style="vertical-align: top; text-align: center">
                <asp:GridView ID="grdvLista" runat="server" AllowPaging="True" Height="30px" SelectedIndex="0" Width="480px" AllowSorting="True" CellPadding="0" ForeColor="#333333" OnPageIndexChanging="grdvLista_PageIndexChanging" HorizontalAlign="Center" OnSorting="grdvLista_Sorting" EnableViewState="False" Font-Overline="False" Font-Size="XX-Small" GridLines="None" ShowFooter="True" EnableTheming="True" PageSize="30" UseAccessibleHeader="False">
<PagerSettings FirstPageImageUrl="../imagenes/backIcon.gif" LastPageImageUrl="../imagenes/forwardIcon.gif" NextPageImageUrl="../imagenes/nextPageIcon.gif" PreviousPageImageUrl="../imagenes/previousPageIcon.gif" Mode="NextPreviousFirstLast"></PagerSettings>

<RowStyle Wrap="False" Font-Strikeout="False" CssClass="gridRow"></RowStyle>

<FooterStyle Font-Bold="True" ForeColor="White" CssClass="GridFormat"></FooterStyle>

<SelectedRowStyle Font-Bold="False" CssClass="gridRow"></SelectedRowStyle>

<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="GridFormat"></HeaderStyle>

<EditRowStyle BackColor="#999999"></EditRowStyle>

<AlternatingRowStyle Wrap="False" Font-Overline="False" CssClass="gridAltRow"></AlternatingRowStyle>
                    <EmptyDataRowStyle Font-Bold="False" />
                    <PagerStyle ForeColor="White" HorizontalAlign="Center" BorderStyle="Dotted" />

</asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: center">
                <asp:label id="lblPagina" runat="server"/><asp:label id="lblPaginatot" runat="server"/>
            </td>
        </tr>
    </table>
    </asp:Panel>
</form>
</body>
</html>