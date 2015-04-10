<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catclientes.aspx.cs" Inherits="catalogos_catclientes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuVentas" Value="" runat="server"/>
<asp:HiddenField ID="hdRV" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo de Clientes - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Negocio/Nombre" Value="0"></asp:ListItem>
                    <asp:ListItem Text="RFC" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Razón Social" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Calle" Value="5"></asp:ListItem>
                    <asp:ListItem Text="Colonia" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Referencia" Value="6"></asp:ListItem>
                    <asp:ListItem Text="Contacto" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Datos no completos" Value="9"></asp:ListItem>
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
                Cliente</asp:LinkButton></td>
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
                <asp:BoundField DataField="nombre" HeaderText="Negocio" SortExpression="1" >
                    <HeaderStyle Width="200px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contacto" HeaderText="Contacto" SortExpression="2" >
                    <HeaderStyle Width="150px" />
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
                <asp:ButtonField Text="Sucursales" CommandName="Sucursales" HeaderText="Sucursales" >
                    <HeaderStyle Width="150px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
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
    Datos Cliente</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Nombre:</td>
<td align="left" class="Cellformat1" colspan="2">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="150" Width="600px" CssClass="TextInputFormat"></asp:TextBox>
<asp:Label runat="server" ID="lblClienteID"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Negocio:</td>
<td align="left" class="Cellformat1" style="width:320px;">
<asp:TextBox ID="txtNegocio" runat="server" MaxLength="150" Width="310px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator5" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el negocio" ControlToValidate="txtNegocio"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender5" runat="Server" TargetControlID="RequiredFieldValidator5" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td align="left" class="Cellformat1" style="width:450px;">
    Proveedor:
    <asp:TextBox ID="txtProveedor" runat="server" MaxLength="20" Width="70px" CssClass="TextInputFormat"></asp:TextBox>
    Teléfono:
    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Razón Social:</td>
<td align="left" class="Cellformat1">
<asp:TextBox ID="txtRazonSocial" runat="server" MaxLength="150" Width="310px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td align="left" class="Cellformat1">
RFC:
<asp:TextBox ID="txtRFC" runat="server" MaxLength="13" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Calle:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtCalle" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="150"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Núm Ext:
    <asp:TextBox ID="txtNumExt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
    Núm Int:
    <asp:TextBox ID="txtNumInt" runat="server" CssClass="TextInputFormat" Width="100px" MaxLength="100"></asp:TextBox>
    C.P.:
    <asp:TextBox ID="txtCP" runat="server" CssClass="TextInputFormat" Width="50px" MaxLength="5"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Colonia:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtColonia" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="50"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Localidad:
    <asp:TextBox ID="txtLocalidad" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="40"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Referencias:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtReferencia" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="200"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Municipio:
    <asp:TextBox ID="txtMunicipio" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Estado:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtEstado" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">País:
    <asp:TextBox ID="txtPais" runat="server" CssClass="TextInputFormat" Width="310px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Contacto Compras:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtContacto" runat="server" MaxLength="100" Width="310px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Email:
    <asp:TextBox ID="txtEmail" runat="server" CssClass="TextInputFormat" Width="180px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Contacto Finanzas:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtContacto2" runat="server" MaxLength="100" Width="310px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto2_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Email:
    <asp:TextBox ID="txtContacto2_Email" runat="server" CssClass="TextInputFormat" Width="180px" MaxLength="100"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Médico 1er Turno:</td>
<td align="left" class="Cellformat1">Dr.
    <asp:TextBox ID="txtContacto_Dr1" runat="server" MaxLength="100" Width="288px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Dr1_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Celular:
    <asp:TextBox ID="txtContacto_Dr1_Cel" runat="server" CssClass="TextInputFormat" Width="140px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">&nbsp;</td>
<td align="left" class="Cellformat1">Enf.
    <asp:TextBox ID="txtContacto_Enf1" runat="server" MaxLength="100" Width="282px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Enf1_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Celular:
    <asp:TextBox ID="txtContacto_Enf1_Cel" runat="server" CssClass="TextInputFormat" Width="140px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Médico 2o Turno:</td>
<td align="left" class="Cellformat1">Dr.
    <asp:TextBox ID="txtContacto_Dr2" runat="server" MaxLength="100" Width="288px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Dr2_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Celular:
    <asp:TextBox ID="txtContacto_Dr2_Cel" runat="server" CssClass="TextInputFormat" Width="140px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">&nbsp;</td>
<td align="left" class="Cellformat1">Enf.
    <asp:TextBox ID="txtContacto_Enf2" runat="server" MaxLength="100" Width="282px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left">Teléfono:
    <asp:TextBox ID="txtContacto_Enf2_Tel" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
    Celular:
    <asp:TextBox ID="txtContacto_Enf2_Cel" runat="server" CssClass="TextInputFormat" Width="140px" MaxLength="150"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Email Facturas:</td>
<td align="left" colspan="3">
    <asp:TextBox ID="txtFacturas_Email" runat="server" CssClass="TextInputFormat" Width="650px" MaxLength="500"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Domicilio Entrega 1:</td>
<td align="left" colspan="3">
    <asp:TextBox ID="txtDirEntrega1" runat="server" CssClass="TextInputFormat" Width="650px" MaxLength="500"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Domicilio Entrega 2:</td>
<td align="left" colspan="3">
    <asp:TextBox ID="txtDirEntrega2" runat="server" CssClass="TextInputFormat" Width="650px" MaxLength="500"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Domicilio Entrega 3:</td>
<td align="left" colspan="3">
    <asp:TextBox ID="txtDirEntrega3" runat="server" CssClass="TextInputFormat" Width="650px" MaxLength="500"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">Notas:</td>
<td align="left" colspan="3">
    <asp:TextBox ID="txtNotas" runat="server" MaxLength="500" Width="650px" Height="65px" CssClass="TextInputFormat" TextMode="MultiLine"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Método de pago:</td>
<td align="left" class="Cellformat1">
<asp:DropDownList ID="dlMetodoPago" runat="server" CssClass="SelectFormat" DataValueField="metodo_pagoID" DataTextField="metodo_pago">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left" valign="top">Cuenta (4 dígitos):
    <asp:TextBox ID="txtCuenta_Bancaria" runat="server" MaxLength="4" Width="100px" CssClass="TextInputFormat"></asp:TextBox>
    Banco:
    <asp:TextBox ID="txtBanco" runat="server" MaxLength="50" Width="100px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Lista de precios:</td>
<td align="left" class="Cellformat1">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left">IVA:
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1"></asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Tipo:</td>
<td align="left" class="Cellformat1">
<asp:RadioButtonList runat="server" ID="rdTipo" RepeatDirection="Horizontal">
<asp:ListItem Selected="True" Text="Contado" Value="True"></asp:ListItem>
<asp:ListItem Text="Crédito" Value="False"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="left">Desc1/Desc2:
    <asp:TextBox ID="txtDescuento1" runat="server" CssClass="TextInputFormat" Width="75px" MaxLength="150"></asp:TextBox>%
    <asp:TextBox ID="txtDescuento2" runat="server" CssClass="TextInputFormat" Width="75px" MaxLength="150"></asp:TextBox>%
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Límite crédito:</td>
<td align="left" class="Cellformat1"><asp:TextBox ID="txtLimiteCR" runat="server" MaxLength="50" Width="100px" CssClass="TextInputFormat"></asp:TextBox>
<asp:HiddenField runat="server" ID="hdLimiteCR" />
</td>
<td class="Cellformat1" align="left">Días crédito:
<asp:TextBox ID="txtDiasCR" runat="server" MaxLength="3" Width="50px" CssClass="TextInputFormat"></asp:TextBox>
<asp:HiddenField runat="server" ID="hdDiasCR" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">&nbsp;</td>
<td align="left" class="Cellformat1">
<asp:CheckBox runat="server" ID="chkCompletos" Text="Datos completos" CssClass="Cellformat1" />
</td>
<td class="Cellformat1" align="left">Fecha último cambio:
    <asp:Label runat="server" ID="lblFechaModificacion" CssClass="Cellformat1" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">&nbsp;</td>
<td align="left" class="Cellformat1">
<asp:CheckBox runat="server" ID="chkSeparar_Facturas" Text="Separar Facturas CON IVA y SIN IVA" CssClass="Cellformat1" />
</td>
<td class="Cellformat1" align="left">&nbsp;
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
<asp:Panel ID="pnlLimiteCR" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnLimiteCRContinuar">
<asp:Panel ID="pnlLimiteCRHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label192" runat="server" Text="Verificación de Límite de Crédito" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblLimiteCR" CssClass="Cellformat1">Está modificando el límite/días de crédito del cliente</asp:Label>
    <br /><br />
    <asp:Label runat="server" ID="Label193" CssClass="Cellformat1">Ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="Label194" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigoLimiteCR" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnLimiteCRContinuar" runat="server" Text="Continuar" onclick="btnLimiteCRContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnLimiteCRCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyLimiteCR" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdLimiteCR" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdLimiteCRBehaviorID"
    TargetControlID="btnDummyLimiteCR"
    PopupControlID="pnlLimiteCR"
    CancelControlID="btnLimiteCRCerrar"
    DropShadow="False" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlVentas" Visible="false">
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr>
<td class="Cellformat1" align="left" style="width:120px;">Última Venta:</td>
<td align="left" class="Cellformat1"><asp:Label runat="server" ID="lblUltimaVenta" CssClass="Cellformat1" /></td>
</tr>
<tr>
<td class="Cellformat1" align="left">Ventas Anuales:</td>
<td align="left" class="Cellformat1"><asp:Label runat="server" ID="lblVentas1" CssClass="Cellformat1" /></td>
</tr>
<tr>
<td class="Cellformat1" align="left">Saldo Actual:</td>
<td align="left" class="Cellformat1"><asp:Label runat="server" ID="lblSaldo" CssClass="Cellformat1" /></td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlSucursales" runat="server" Visible="false">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Sucursales del Cliente</td>
</tr>
<tr>
<td class="Cellformat1" align="right" style="width:300px;">Negocio:&nbsp;</td>
<td align="left" style="width:500px;">
<b><asp:Label runat="server" ID="lblNegocio" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td align="center" colspan="2">
<asp:Button ID="btnAgregarSucursal" runat="server" Text="Agregar Sucursal"
        CssClass="ButtonFormat" onclick="btnAgregarSucursal_Click" />
<br />
<asp:ImageButton ID="btnRegresar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" class="Titleformat1">
    Sucursales
</td>
</tr>
<tr>
<td align="center">
<asp:GridView ID="gvSucursales" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" CellPadding="0" Width="600"
    AutoGenerateColumns="false" DataKeyNames="sucursalID"
    HorizontalAlign="Center" EnableViewState="True" ShowFooter="true"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvSucursales_RowCommand" >
<Columns>
    <asp:ButtonField DataTextField="sucursalID" CommandName="Modificar" HeaderText="Clave" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:BoundField DataField="sucursal" HeaderText="Sucursal">
        <HeaderStyle Width="200px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="telefono" HeaderText="Teléfono" >
        <HeaderStyle Width="130px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="direccion" HeaderText="Dirección" >
        <HeaderStyle Width="200px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
</Columns>
</asp:GridView>
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlSucursal" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Datos Sucursal</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 150px">Sucursal:</td>
<td align="left" style="width: 650px">
<asp:TextBox ID="txtSucursal" runat="server" MaxLength="150" Width="400px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator14" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtSucursal"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender15" runat="Server" TargetControlID="RequiredFieldValidator14" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:HiddenField runat="server" ID="hdSucursalID" />
</td>
</tr>
<tr>
<td>&nbsp;</td>
<td align="left" class="Cellformat1">
<asp:Button ID="btnCopiar" runat="server" Text="Copiar Datos del Negocio"
        CssClass="ButtonFormat" onclick="btnCopiar_Click" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Teléfono:</td>
<td align="left" class="Cellformat1">
<asp:TextBox ID="txtSucursal_Telefono" runat="server" MaxLength="100" Width="150px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Dirección:</td>
<td align="left" class="Cellformat1">
    <asp:TextBox ID="txtSucursal_Dirección" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="200"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Frecuencia Visita:</td>
<td align="left" class="Cellformat1">
<asp:DropDownList ID="dlFrecuencia" runat="server" CssClass="SelectFormat">
<asp:ListItem Selected="True" Value="0">Indefinido</asp:ListItem>
<asp:ListItem Value="1">Semanal</asp:ListItem>
<asp:ListItem Value="2">Quincenal</asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr>
<td align="center" colspan="2">
<br />
<asp:ImageButton ID="btnGuardarSucursal" runat="server" CssClass="AddFormat1"
        ToolTip="Guardar" ImageUrl="~/imagenes/dummy.ico"
        onclick="btnGuardarSucursal_Click" />
<asp:ImageButton ID="btnRegresarSucursal" runat="server" CssClass="BackFormat1" CausesValidation="false"
    ToolTip="Regresar" OnClick="btnRegresarSucursal_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" class="Titleformat1">
    Productos
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top">
<table>
<tr>
<td class="GridFormatTD" align="left" style="width:500px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:100px;">Minimo</td>
<td class="GridFormatTD" align="left" style="width:100px;">Maximo</td>
<td class="GridFormatTD" align="left" style="width:100px;">Punto Reorden</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:500px;">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="490px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="acProducto"
                TargetControlID="txtProducto"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerProductos"
                MinimumPrefixLength="1"
                CompletionInterval="1000"
                OnClientItemSelected="productoSeleccionado"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement"
                CompletionListItemCssClass="autocomplete_listItem"
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
</td>
<td class="Cellformat1" align="left" style="width:100px;">
<asp:TextBox ID="txtMinimo" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
<td class="Cellformat1" align="left" style="width:100px;">
<asp:TextBox ID="txtMaximo" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
<td class="Cellformat1" align="left" style="width:100px;">
<asp:TextBox ID="txtPunto_Reorden" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
</td>
</tr>
</table>
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" colspan="4">
<asp:GridView ID="gvProductos" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" CellPadding="0" Width="628"
    AutoGenerateColumns="false" DataKeyNames="productoID"
    HorizontalAlign="Center" EnableViewState="True" ShowFooter="true"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvProductos_RowCommand" >
    <FooterStyle HorizontalAlign="Right"></FooterStyle>
<Columns>
    <asp:BoundField DataField="producto" HeaderText="Producto">
        <HeaderStyle Width="400px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="minimo" HeaderText="Minimo">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="maximo" HeaderText="Precio Unitario">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="punto_reorden" HeaderText="Subtotal">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
</Columns>
</asp:GridView>
</td>
</tr>
</table>
</asp:Panel>
<script type="text/javascript">
    function setProductoFoco() {
        $get('<%=txtProducto.ClientID%>').focus();
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        setTimeout('setMinimoFoco()', 50);
    }

    function setMinimoFoco() {
        $get('<%=txtMinimo.ClientID%>').focus();
    }

    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
    function iva11()
    {
        if (document.getElementsByName('<%=rdIVA.UniqueID %>').length > 0) {
            var rdIVA = document.getElementsByName('<%=rdIVA.UniqueID %>');
            rdIVA[1].disabled = true;
        }
    }
</script>
</asp:Content>