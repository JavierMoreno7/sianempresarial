<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catpersonal.aspx.cs" Inherits="catalogos_catpersonal" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo del Personal - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Apellidos" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Nombre" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Rol" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Estatus" Value="3"></asp:ListItem>
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
                Persona</asp:LinkButton></td>
        </tr>
        <tr>
            <td style="text-align: center; vertical-align: top;" colspan="3">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="750px" AllowSorting="True" 
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="referencia"
                HorizontalAlign="Center" OnSorting="grdvLista_Sorting" EnableViewState="True" 
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None" 
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="referencia" CommandName="Modificar" HeaderText="Clave" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="1" >
                    <HeaderStyle Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="apellidos" HeaderText="Apellidos" SortExpression="2" >
                    <HeaderStyle Width="180px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="telefono" HeaderText="Teléfono">
                    <HeaderStyle Width="130px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="rol" HeaderText="Rol" SortExpression="3" >
                    <HeaderStyle Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="estatus" HeaderText="Estatus" SortExpression="4" >
                    <HeaderStyle Width="70px" />
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
    Datos Persona</td>
</tr>
<tr>
<td class="Cellformat1" align="left"  style="width: 150px">Nombre:</td>
<td align="left" style="width: 650px">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="50" Width="200px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator5" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender5" runat="Server" TargetControlID="RequiredFieldValidator5" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Apellidos:</td>
<td align="left">
<asp:TextBox ID="txtApellidos" runat="server" MaxLength="50" Width="200px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la razón social" ControlToValidate="txtApellidos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Rol:</td>
<td align="left">
<asp:DropDownList ID="dlRoles" runat="server" CssClass="SelectFormat" DataValueField="rolID" DataTextField="rol">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdRoles" Value="" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Teléfono:</td>
<td align="left">
<asp:TextBox ID="txtTelefono" runat="server" MaxLength="50" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Celular:</td>
<td align="left">
<asp:TextBox ID="txtCelular" runat="server" MaxLength="50" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Dirección:</td>
<td align="left">
    <asp:TextBox ID="txtCalle" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="50"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Colonia:</td>
<td align="left">
    <asp:TextBox ID="txtColonia" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="50"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Ciudad:</td>
<td align="left">
    <asp:TextBox ID="txtCiudad" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="50"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">C.P.:</td>
<td align="left">
    <asp:TextBox ID="txtCP" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="5"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Activo:</td>
<td align="left">
    <asp:CheckBox runat="server" ID="chkActivo" />
</td>
</tr>
<tr>
<td align="center" colspan="2">
<br />
<asp:ImageButton ID="btnAgregar" runat="server" CssClass="AddFormat1" 
        ToolTip="Agregar" ImageUrl="~/imagenes/dummy.ico"
        onclick="btnAgregar_Click" />
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