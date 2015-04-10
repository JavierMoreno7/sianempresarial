<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catalternativos.aspx.cs" Inherits="catalogos_catalternativos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo Equivalencias - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Nombre Equivalencia" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Producto a Sustituir" Value="2"></asp:ListItem>
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
                Artículo</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="670px" AllowSorting="True" 
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="ID"
                HorizontalAlign="Center" OnSorting="grdvLista_Sorting" EnableViewState="True" 
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None" 
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="ID" CommandName="Modificar" HeaderText="Referencia" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre Equivalencia" SortExpression="1" >
                    <HeaderStyle Width="300px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="nombreProducto" HeaderText="Producto a Sustituir" SortExpression="2" >
                    <HeaderStyle Width="300px" />
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
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Datos Equivalencia</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Nombre:</td>
<td align="left" style="width:350px;">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="300px"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" colspan="2" align="left" style="width:450px;" >
    &nbsp;
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:100px;">Descripción:</td>
<td colspan="3" style="width:800px;" align="left">
<asp:TextBox ID="txtDescripcion" runat="server" MaxLength="500" Width="750px" Height="90px" CssClass="TextInputFormat" TextMode="MultiLine"></asp:TextBox><br />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Producto:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="700px" autocomplete="off"></asp:TextBox>
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
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Relacion:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtRelacion" runat="server" MaxLength="6" Width="70px" 
        CssClass="TextInputFormat" AutoPostBack="True" 
        ontextchanged="txtRelacion_TextChanged"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el código de barras" ControlToValidate="txtRelacion" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender1" runat="Server" TargetControlID="RequiredFieldValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:RegularExpressionValidator id="RegularExpressionValidator2" runat="server" SkinID="regValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Relación debe ser numérica" ControlToValidate="txtRelacion" ValidationExpression="^\d{1,2}(\.\d{1,3})?$" ValidationGroup="valDatos"></asp:RegularExpressionValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender2" runat="Server" TargetControlID="RegularExpressionValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:RangeValidator ID="RangeValidator1" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Relación debe ser mayor a cero" ControlToValidate="txtRelacion" ValidationGroup="valDatos" MinimumValue="0.001" MaximumValue="99.999"></asp:RangeValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender6" runat="Server" TargetControlID="RangeValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Sustituye a:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtProductoSus" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="700px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="AutoCompleteExtender1" 
                TargetControlID="txtProductoSus"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerProductos"
                MinimumPrefixLength="1" 
                CompletionInterval="1000"
                OnClientItemSelected="productoSeleccionadoSus"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement" 
                CompletionListItemCssClass="autocomplete_listItem" 
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdProductoSusID" Value="" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Lista de Precios:</td>
<td align="left" colspan="3">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" 
        DataValueField="listaprecioID" DataTextField="nombrelista" AutoPostBack="True" 
        onselectedindexchanged="dlListaPrecios_SelectedIndexChanged">
</asp:DropDownList> * Sólo para efectos de comparación</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">&nbsp;</td>
<td align="left" colspan="3">
    <table cellspacing="0" cellpadding="0" align="left" border="0" style="color:#333333;font-size:XX-Small;text-decoration:none;height:1px;width:240px;border-collapse:collapse;font-size: pt; direction: ltr; text-indent: 5pt; table-layout: auto; border-collapse: separate;">
		<tr class="GridFormat" align="center" valign="middle" style="color:White;border-width:123px;border-style:Double;font-size:XX-Small;font-weight:bold;width:100px;">
			<th scope="col" style="width:80px;">Precio Producto A Sustituir</th>
			<th scope="col" style="width:80px;">Precio Equivalencia</th>
			<th scope="col" style="width:80px;">Precio Relación</th>
		</tr>
		<tr align="left" style="color:Black;background-color:White;font-family:Verdana;font-size:7pt;">
			<td align="right"><asp:Label runat="server" ID="lblPrecioProducto"></asp:Label></td>
			<td align="right"><asp:Label runat="server" ID="lblPrecioOriginal"></asp:Label></td>
			<td align="right"><asp:Label runat="server" ID="lblPrecio"></asp:Label></td>
		</tr>
	</table>
</td>
</tr>
<tr>
<td align="center" colspan="4">
<br />
<asp:ImageButton ID="btnAgregar" runat="server" CssClass="AddFormat1" 
        ToolTip="Agregar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos" 
        onclick="btnAgregar_Click" />
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1" 
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos" 
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnCancelar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>   
<script type="text/javascript">
function closePopup(e) {
   var modalPopupBehavior = $find('programmaticModalPopupBehavior');
   modalPopupBehavior.hide();
} 


function isNumber(n) {
  return !isNaN(parseFloat(n)) && isFinite(n);
}

function Limite(e, campo, maximo)
{
    var tecla;
    if(window.event)
       tecla = window.event.keyCode;
    else
       tecla = e.which;
    
    if(tecla == 38 || tecla == 40 || tecla == 37 || tecla == 39 || tecla == 33 || tecla == 34 || tecla == 36 || tecla == 35 || tecla == 16 || tecla == 17 || tecla == 18 || tecla == 20 || tecla == 46 || tecla == 8 || tecla == 13 || tecla == 9 || tecla == 27)
        return true;
    else
        if(campo.value.length > maximo - 1)
            return false;
        else
            return true;
}
function prevenirPaste(campo, maximo)
{
    var restoCaracteres;
    var datos = window.clipboardData.getData("Text");
    var nvosDatos;
    
    restoCaracteres = maximo - campo.value.length;
    if(restoCaracteres <= 0)
        return false;
    else
    {
        nvosDatos = datos.substr(0, restoCaracteres);
        window.clipboardData.setData("Text", nvosDatos);
        return true;
    }
}
function productoSeleccionado(sender, e) {
    $get('<%=hdProductoID.ClientID%>').value = e.get_value();
    PageMethods.ObtenerPrecios($get('<%=dlListaPrecios.ClientID%>').value + "|" +
                            $get('<%=hdProductoSusID.ClientID%>').value + "|" +
                            e.get_value() + "|" +
                            $get('<%=txtRelacion.ClientID%>').value, Llenar_Precios);
}

function productoSeleccionadoSus(sender, e) {
    $get('<%=hdProductoSusID.ClientID%>').value = e.get_value();
    PageMethods.ObtenerPrecios($get('<%=dlListaPrecios.ClientID%>').value + "|" +
                            e.get_value() + "|" +
                            $get('<%=hdProductoID.ClientID%>').value + "|" +
                            $get('<%=txtRelacion.ClientID%>').value, Llenar_Precios);
}

function Llenar_Precios(rs) {
    var precios = rs.split("|");
    $get('<%=lblPrecioProducto.ClientID%>').innerHTML = precios[0];
    $get('<%=lblPrecioOriginal.ClientID%>').innerHTML = precios[1];
    $get('<%=lblPrecio.ClientID%>').innerHTML = precios[2];
}

function setProductoSusFoco() {
    $get('<%=txtProductoSus.ClientID%>').focus();
}
</script>
</asp:Content>