<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catpaq.aspx.cs" Inherits="catalogos_catpaq" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo de Paqueterías - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Nombre" Value="0"></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>
                &nbsp;<asp:ImageButton
                    ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
                    OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
            <td style="width: 108px;" align="left">
                <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos 
                los Registros</asp:LinkButton></td>
            <td style="width: 109px;" align="left">
                <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat" OnClick="lblAgregar_Click">Agregar 
                Paquetería</asp:LinkButton></td>
        </tr>
        <tr>
            <td style="text-align: center; vertical-align: top;" colspan="3">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="800px" AllowSorting="True" 
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="referencia"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True" 
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None" 
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="referencia" CommandName="Modificar" HeaderText="Clave" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="1" >
                    <HeaderStyle Width="300px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contacto" HeaderText="Contacto" SortExpression="2" >
                    <HeaderStyle Width="200px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="telefono" HeaderText="Teléfono" >
                    <HeaderStyle Width="130px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
            </asp:GridView>
            </td>
        </tr>
        <tr style="height:3px;">
<td colspan="3">&nbsp;</td>
</tr>
<tr>
<td colspan="3" align="center" valign="middle">
<asp:ImageButton runat="server" ID="btnFirstPage" Height="12" Width="12"
        ImageUrl="~/imagenes/backIcon.gif" onclick="btnFirstPage_Click" />&nbsp;
<asp:ImageButton runat="server" ID="btnPrevPage" Height="12" Width="12"
        ImageUrl="~/imagenes/previousPageIcon.gif" onclick="btnPrevPage_Click" />&nbsp;
    <asp:label id="lblPagina" runat="server" CssClass="tb"/>
    <asp:TextBox runat="server" ID="txtPag" CssClass="TextPag" MaxLength="5" AutoPostBack="true" ontextchanged="txtPag_TextChanged" />
    <asp:label id="lblPaginatot" runat="server" CssClass="tb" />
<asp:ImageButton runat="server" ID="btnNextPage" Height="12" Width="12" 
        ImageUrl="~/imagenes/nextPageIcon.gif" onclick="btnNextPage_Click" />&nbsp;
<asp:ImageButton runat="server" ID="btnLastPage" Height="12" Width="12"
        ImageUrl="~/imagenes/forwardIcon.gif" onclick="btnLastPage_Click" />
</td>
</tr>
    </table>
    </asp:Panel>
<asp:Panel ID="pnlDatos" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Datos Paquetería</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Paquetería:</td>
<td align="left">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="150" Width="600px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator5" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender5" runat="Server" TargetControlID="RequiredFieldValidator5" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Teléfono:</td>
<td style="width: 700px" align="left">
<asp:TextBox ID="txtTelefono" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Calle:</td>
<td align="left">
    <asp:TextBox ID="txtCalle" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Número Exterior:</td>
<td align="left">
    <asp:TextBox ID="txtNumExt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Número Interior:</td>
<td align="left">
    <asp:TextBox ID="txtNumInt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Colonia:</td>
<td align="left">
    <asp:TextBox ID="txtColonia" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="50"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Localidad:</td>
<td align="left">
    <asp:TextBox ID="txtLocalidad" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="40"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Municipio:</td>
<td align="left">
    <asp:TextBox ID="txtMunicipio" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="100"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator9" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el municipio" ControlToValidate="txtMunicipio"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender10" runat="Server" TargetControlID="RequiredFieldValidator9" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Estado:</td>
<td align="left">
    <asp:TextBox ID="txtEstado" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="100"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator10" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el estado" ControlToValidate="txtEstado"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender11" runat="Server" TargetControlID="RequiredFieldValidator10" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">País:</td>
<td align="left">
    <asp:TextBox ID="txtPais" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="100"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator11" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el país" ControlToValidate="txtPais"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender12" runat="Server" TargetControlID="RequiredFieldValidator11" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">C.P.:</td>
<td align="left">
    <asp:TextBox ID="txtCP" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="5"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator12" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el código postal" ControlToValidate="txtCP"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender13" runat="Server" TargetControlID="RequiredFieldValidator12" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Email:</td>
<td align="left">
    <asp:TextBox ID="txtEmail" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="150"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator8" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el email" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender6" runat="Server" TargetControlID="RequiredFieldValidator8" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo no válido</b><br/>El email no es correcto" ValidationExpression="^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender9" runat="Server" TargetControlID="RegularExpressionValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Fax:</td>
<td align="left">
    <asp:TextBox ID="txtFax" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Contacto:</td>
<td align="left">
    <asp:TextBox ID="txtContacto" runat="server" MaxLength="500" Width="300px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Chofer:</td>
<td align="left">
    <asp:TextBox ID="txtChofer" runat="server" MaxLength="500" Width="300px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Notas:</td>
<td align="left">
    <asp:TextBox ID="txtNotas" runat="server" MaxLength="500" Width="650px" Height="90px" CssClass="TextInputFormat" TextMode="MultiLine"></asp:TextBox>
</td>
</tr>
<tr>
<td align="center" colspan="2">
<br />
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1" 
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="BackFormat1" CausesValidation="false" 
    ToolTip="Regresar" OnClick="btnCancelar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>
</asp:Content>