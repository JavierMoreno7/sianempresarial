<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catproveedores.aspx.cs" Inherits="catalogos_catproveedores" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo de Proveedores - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Nombre" Value="0"></asp:ListItem>
                    <asp:ListItem Text="RFC" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Especialidad" Value="2"></asp:ListItem>
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
                Proveedor</asp:LinkButton></td>
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
                <asp:BoundField DataField="rfc" HeaderText="RFC" SortExpression="3" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
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
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr><td class="GridFormat" colspan="3" style="height: 18px;">
    Datos Proveedor</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Proveedor:</td>
<td class="Cellformat1" align="left" colspan="2">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="150" Width="460px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator5" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender5" runat="Server" TargetControlID="RequiredFieldValidator5" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
&nbsp;&nbsp;Número de proveedor:
<asp:TextBox ID="txtNumero_Proveedor" runat="server" MaxLength="100" Width="100px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Razón Social:</td>
<td align="left" class="Cellformat1">
<asp:TextBox ID="txtRazonSocial" runat="server" MaxLength="150" Width="310px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la razón social" ControlToValidate="txtRazonSocial"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td align="left" class="Cellformat1">
RFC:
<asp:TextBox ID="txtRFC" runat="server" MaxLength="13" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator13" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el RFC" ControlToValidate="txtRFC"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender14" runat="Server" TargetControlID="RequiredFieldValidator13" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
Teléfono:
<asp:TextBox ID="txtTelefono" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator3" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el teléfono" ControlToValidate="txtTelefono"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender4" runat="Server" TargetControlID="RequiredFieldValidator3" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>

</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Calle:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtCalle" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="150"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la calle" ControlToValidate="txtCalle"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender1" runat="Server" TargetControlID="RequiredFieldValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" align="left">Núm Ext:
    <asp:TextBox ID="txtNumExt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator4" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el número exterior" ControlToValidate="txtNumExt"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender2" runat="Server" TargetControlID="RequiredFieldValidator4" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
    Núm Int:
    <asp:TextBox ID="txtNumInt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
    C.P.:
    <asp:TextBox ID="txtCP" runat="server" CssClass="TextInputFormat" Width="50px" MaxLength="5"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator12" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el código postal" ControlToValidate="txtCP"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender13" runat="Server" TargetControlID="RequiredFieldValidator12" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Colonia:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtColonia" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="50"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator6" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la colonia" ControlToValidate="txtColonia"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender7" runat="Server" TargetControlID="RequiredFieldValidator6" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" align="left">Localidad:
    <asp:TextBox ID="txtLocalidad" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="40"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator7" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la localidad" ControlToValidate="txtLocalidad"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender8" runat="Server" TargetControlID="RequiredFieldValidator7" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Municipio:</td>
<td align="left" class="Cellformat1" colspan="2">
    <asp:TextBox ID="txtMunicipio" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator9" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el municipio" ControlToValidate="txtMunicipio"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender10" runat="Server" TargetControlID="RequiredFieldValidator9" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Estado:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtEstado" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator10" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el estado" ControlToValidate="txtEstado"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender11" runat="Server" TargetControlID="RequiredFieldValidator10" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" align="left">País:
    <asp:TextBox ID="txtPais" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator11" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el país" ControlToValidate="txtPais"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender12" runat="Server" TargetControlID="RequiredFieldValidator11" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Email:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtEmail" runat="server" CssClass="TextInputFormat" Width="300px" MaxLength="150"></asp:TextBox>
    <asp:RequiredFieldValidator id="RequiredFieldValidator8" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el email" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender6" runat="Server" TargetControlID="RequiredFieldValidator8" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo no válido</b><br/>El email no es correcto" ValidationExpression="^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender9" runat="Server" TargetControlID="RegularExpressionValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" align="left">Fax:
    <asp:TextBox ID="txtFax" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Contacto Vtas:</td>
<td align="left">
    <asp:TextBox ID="txtContacto" runat="server" MaxLength="100" Width="300px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Email:
    <asp:TextBox ID="txtContacto_Email" runat="server" CssClass="TextInputFormat" Width="180px" MaxLength="100"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo no válido</b><br/>El email no es correcto" ValidationExpression="^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ControlToValidate="txtContacto_Email"></asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender16" runat="Server" TargetControlID="RegularExpressionValidator3" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Contacto CR/COB:</td>
<td align="left">
    <asp:TextBox ID="txtContacto2" runat="server" MaxLength="100" Width="300px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto2_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Email:
    <asp:TextBox ID="txtContacto2_Email" runat="server" CssClass="TextInputFormat" Width="180px" MaxLength="100"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo no válido</b><br/>El email no es correcto" ValidationExpression="^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ControlToValidate="txtContacto2_Email"></asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender15" runat="Server" TargetControlID="RegularExpressionValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Especialidad:</td>
<td align="left" colspan="2">
    <asp:TextBox ID="txtEspecialidad" runat="server" MaxLength="150" Width="300px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Notas:</td>
<td align="left" colspan="2">
    <asp:TextBox ID="txtNotas" runat="server" MaxLength="500" Width="650px" Height="65px" CssClass="TextInputFormat" TextMode="MultiLine"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Datos Bancarios:</td>
<td class="Cellformat1" align="left" colspan="3">
    <asp:TextBox ID="txtDatos_Bancarios" runat="server" MaxLength="200" Width="755px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Utilidad:</td>
<td class="Cellformat1" align="left" colspan="3">
    <asp:TextBox ID="txtUtilidad" runat="server" CssClass="TextInputFormat" Width="75px" MaxLength="5"></asp:TextBox>%
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Lista de precios:</td>
<td align="left">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left">IVA:
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1">
</asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Tipo:</td>
<td class="Cellformat1" align="left">
<asp:RadioButtonList runat="server" ID="rdTipo" RepeatDirection="Horizontal">
<asp:ListItem Selected="True" Text="Contado" Value="True"></asp:ListItem>
<asp:ListItem Text="Crédito" Value="False"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="left">Descuento:
    <asp:TextBox ID="txtDescuento" runat="server" CssClass="TextInputFormat" Width="75px" MaxLength="5"></asp:TextBox>%
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Días entrega:</td>
<td class="Cellformat1" align="left">
<asp:TextBox ID="txtDiasEntrega" runat="server" CssClass="TextInputFormat" Width="75px" MaxLength="3"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">
<table><tr>
<td class="Cellformat1" align="left">Cobra paquetería:
<td class="Cellformat1" align="left">
<asp:RadioButtonList runat="server" ID="rdCobraPaq" RepeatDirection="Horizontal">
<asp:ListItem Text="No" Value="False"></asp:ListItem>
<asp:ListItem Text="Sí" Value="True"></asp:ListItem>
</asp:RadioButtonList>
</td></tr></table>
</td>
</tr>
<tr>
<td align="center" colspan="3">
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