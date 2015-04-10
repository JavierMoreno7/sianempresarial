<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="admin.aspx.cs" Inherits="admin_admin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr style="height:5px;"><td colspan="2">&nbsp;</td></tr>
<tr>
<td class="Cellformat1" align="left">&nbsp;</td>
<td class="Cellformat1" align="left">
<asp:RadioButtonList ID="rdFactNot" runat="server" RepeatDirection="Horizontal" 
        RepeatLayout="Flow" CssClass="Cellformat1" >
<asp:ListItem Selected="True" Value="1" Text="Factura"></asp:ListItem>
<asp:ListItem Value="2" Text="Remisión"></asp:ListItem>
<asp:ListItem Value="3" Text="Compra"></asp:ListItem>
<asp:ListItem Value="4" Text="Pedido"></asp:ListItem>
</asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="right">Factura/Remisión/Compra/Pedido:</td>
<td class="Cellformat1" align="left">
<asp:TextBox ID="txtFactura" runat="server" CssClass="TextInputFormat" Width="90px" 
        MaxLength="10"></asp:TextBox>
</td>
</tr>
<tr><td colspan="2" align="center"><b>
<asp:Label runat="server" ID="lblM" CssClass="msgError">Este proceso NO REVIERTE los cambios hechos en el inventario, así que debe hacerlos de manera manual en el módulo de inventarios</asp:Label></b>
</td></tr>
<tr>
<td align="center" colspan="2">
    <br />
   <asp:Button ID="btnCobranza" runat="server" Text="Cambiar estatus a cobranza" 
        CssClass="ButtonFormat" onclick="btnCobranza_Click" />
   <asp:Button ID="btnEnProceso" runat="server" Text="Cambiar estatus a En Proceso" 
        CssClass="ButtonFormat" onclick="btnEnProceso_Click" />
</td>
</tr>
</table>
</asp:Content>
