<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPagePopUp.master" AutoEventWireup="true" CodeFile="compras_correo.aspx.cs" Inherits="compras_compras_correo" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPagePopUp.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField runat="server" ID="hdDe" />
<asp:HiddenField runat="server" ID="hdTipo" />
<asp:Panel ID="pnlDatos" runat="server" DefaultButton="btnDummy">
<table border="0" cellspacing="0" cellpadding="0" style="width: 610px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Envío compras</td>
</tr>
<tr style="height:10px">
<td colspan="2"></td>
</tr>
<tr>
<td align="left" colspan="2">
<asp:Button ID="btnEnviar" runat="server" Text="Enviar"
        CssClass="ButtonFormat" onclick="btnEnviar_Click" />
<asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
        CssClass="ButtonFormat" OnClientClick="cerrarPopUp();return false;" />
<asp:Button ID="btnDummy" runat="server" CssClass="ButtonFormat" Text="" />
</td>
</tr>
<tr style="height:10px">
<td colspan="2"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:60px;">Para:</td>
<td class="Cellformat1" align="left" style="width:550px;">
<asp:Textbox id="txtPara" runat="server" Width="540px" MaxLength="200" CssClass="TextInputFormat"></asp:Textbox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">CC:</td>
<td class="Cellformat1" align="left">
<asp:Textbox id="txtCC" runat="server" Width="540px" MaxLength="200" CssClass="TextInputFormat"></asp:Textbox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Asunto:</td>
<td class="Cellformat1" align="left">
<asp:Textbox id="txtAsunto" runat="server" Width="540px" MaxLength="200" CssClass="TextInputFormat"></asp:Textbox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">&nbsp;</td>
<td class="Cellformat1" align="left">
<asp:Label runat="server" ID="lblXLS" CssClass="LinkFormat"></asp:Label>
</td>
</tr>
<tr>
<td align="left" colspan="2">
<asp:TextBox ID="txtMensaje" runat="server" TextMode="MultiLine" CssClass="TextInputFormat" Width="600px" Height="200px"></asp:TextBox>
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlEnviado" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnEnviadoContinuar">
<asp:Panel ID="pnlEnviadoHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label3" runat="server" Text="Correo enviado" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="lblEnviar" runat="server" Text="El documento ha sido enviado" />
    <br /><br />
    <asp:Button ID="btnEnviadoContinuar" runat="server" Text="OK" onclick="btnEnviadoContinuar_Click" CssClass="ButtonFormat"  />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyEnviado" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdEnviado" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdEnviadoBehaviorID"
    TargetControlID="btnDummyEnviado"
    PopupControlID="pnlEnviado"
    DropShadow="False" />
<script type="text/javascript">
    function cerrarPopUp() {
        window.close();
    }
</script>
</asp:Content>

