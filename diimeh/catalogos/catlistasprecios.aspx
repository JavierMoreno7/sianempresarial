<%@ Page Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catlistasprecios.aspx.cs" Inherits="catalogos_catlistasprecios" Title="SIAN - Sistema de Control de Inventarios" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdATC" Value="" runat="server"/>
<asp:HiddenField ID="hdATV" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuCompras" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuVentas" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 650px;">
<tr>
<td class="GridFormat" colspan="3" style="height: 18px;">
    Catalogo Listas de Precios - Listado de Datos</td>
</tr>
<tr>
<td style="width: 400px;  height: 20px;" align="left" class="Cellformat1">
    Buscar por:
    <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
        <asp:ListItem Selected="True" Text="Lista" Value="0"></asp:ListItem>
        <asp:ListItem Text="Porcentaje" Value="1"></asp:ListItem>
        <asp:ListItem Text="Fijo" Value="2"></asp:ListItem>
        <asp:ListItem Text="Tipo" Value="3"></asp:ListItem>
    </asp:DropDownList>&nbsp;
    <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>
    &nbsp;<asp:ImageButton
        ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
        OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
<td style="width: 100px;" align="left">
    <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos
    los Registros</asp:LinkButton></td>
<td style="width: 150px;" align="right">
    <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat" OnClick="lblAgregar_Click">Agregar
    Lista de Precios</asp:LinkButton></td>
</tr>
<tr>
<td colspan="3" style="text-align: center; vertical-align: top;">
<asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" Width="650px" AllowSorting="True"
        CellPadding="0"  AutoGenerateColumns="False" DataKeyNames="referencia"
    HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableTheming="True"
    CaptionAlign="Top" GridLines="None" onrowcommand="grdvLista_RowCommand">
<Columns>
    <asp:ButtonField DataTextField="referencia" CommandName="Modificar" HeaderText="Referencia" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:BoundField DataField="nombre" HeaderText="Lista" SortExpression="nombre" >
        <HeaderStyle Width="330px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="porcentaje" HeaderText="Porcentaje" SortExpression="porcentaje" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="fijo" HeaderText="Fijo" SortExpression="fijo" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:ImageField DataImageUrlField="imagen" HeaderText="Imagen" ControlStyle-Width="20px"  ControlStyle-Height="20px">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:ImageField>
</Columns>
</asp:GridView>
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="height: 308px; width: 800px;">
<tr><td class="GridFormat" colspan="3" style="height: 18px;">
    Listas de Precios</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Lista de precios:</td>
<td align="left" style="width:350px;">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="300px"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td rowspan="4" align="center" style="width:350px;" >
<asp:Image ID="imgListaPrecio" runat="server" ImageUrl="~/imagenes/dummy.ico" /><br />
<asp:Button ID="btnAgregarImagen" runat="server" Text="Modificar imagen"
        CssClass="ButtonFormat" onclick="btnAgregarImagen_Click" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width: 100px">Tipo de lista:</td>
<td align="left" valign="top" style="width: 350px">
<asp:RadioButton ID="rdVentas" runat="server" Text="Ventas" Checked="true" GroupName="gpTipo" CssClass="Cellformat1" /><br />
<asp:RadioButton ID="rdCompras" runat="server" Text="Compras" GroupName="gpTipo" CssClass="Cellformat1" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Lista por default:</td>
<td align="left" style="width: 350px">
    <asp:CheckBox ID="chkDefault" runat="server" CssClass="Cellformat1" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" colspan="2" style="width: 450px">
<asp:Panel runat="server" ID="pnlCopiar">
<table border="0" cellspacing="0" cellpadding="0" style="width:100%;">
<tr>
<td class="Cellformat1" align="left" valign="top" style="width: 100px">Copiar precios de otra lista:</td>
<td align="left" valign="top" style="width: 350px">
    <asp:CheckBox ID="chkCopiar" runat="server" CssClass="Cellformat1" /><br />
    <asp:DropDownList ID="dlListasPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista" Enabled="false">
    </asp:DropDownList><br />
    <asp:RadioButton ID="rdFijo" runat="server" Text="Mismo precio" Checked="true" GroupName="gpCopiar" CssClass="Cellformat1" /><br />
    <asp:RadioButton ID="rdPorcentaje" runat="server" Text="Porcentaje precio " GroupName="gpCopiar" CssClass="Cellformat1"  /><br />
    &nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtPorcentaje" runat="server" MaxLength="2" CssClass="TextInputFormat" Width="30px" Enabled="false"></asp:TextBox>%
    <asp:RegularExpressionValidator id="RegularExpressionValidator2" runat="server" SkinID="regValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Porcentaje debe ser numérico" ControlToValidate="txtPorcentaje" ValidationExpression="^\d+$" ValidationGroup="valDatos"></asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender2" runat="Server" TargetControlID="RegularExpressionValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
    <asp:RangeValidator ID="RangeValidator1" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Porcentaje debe ser mayor a 0 y menor a 100" ControlToValidate="txtPorcentaje" ValidationGroup="valDatos" MinimumValue="1" MaximumValue="99"></asp:RangeValidator>
    <cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender6" runat="Server" TargetControlID="RangeValidator1" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
    <asp:CheckBox ID="chkDesc" runat="server" CssClass="Cellformat1" Text="Descuento sobre precios"  />
</td>
</tr>
</table>
</asp:Panel>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">&nbsp;</td>
<td align="left" colspan="2" style="width: 350px">
   <asp:Button ID="btnAgregarPrecio" runat="server" Text="Agregar precios"
        CssClass="ButtonFormat" onclick="btnAgregarPrecio_Click" />
</td>
</tr>
<tr>
<td align="center" colspan="3">
<br />
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1"
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnCancelar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlDatos2" Visible="false" runat="server" DefaultButton="btnAgregarProd">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" colspan="2" class="Titleformat1">
    Productos
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" colspan="2">
<table>
<tr>
<td class="GridFormatTD" align="left" style="width:550px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:250px;">Precio</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:550px;">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="540px" autocomplete="off"></asp:TextBox>
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
<td align="left" valign="top" style="width:250px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td align="center" colspan="2">
<asp:GridView ID="gvProductos" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" CellPadding="0" Width="900"
    AutoGenerateColumns="false" DataKeyNames="productoID"
    HorizontalAlign="Center" EnableViewState="True" ShowFooter="true"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvProductos_RowCommand"
    onrowdatabound="gvProductos_RowDataBound">
    <FooterStyle HorizontalAlign="Right"></FooterStyle>
<Columns>
    <asp:BoundField DataField="producto" HeaderText="Producto">
        <HeaderStyle Width="350px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="codigo" HeaderText="Código Barras">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="precio" HeaderText="Precio">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="usuario" HeaderText="Usuario">
        <HeaderStyle Width="130px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="minimo" HeaderText="Mínimo">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="maximo" HeaderText="Máximo">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="existencia" HeaderText="Existencia">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:TemplateField HeaderText="" >
        <HeaderStyle Width="0" />
        <ItemTemplate>
            <asp:HiddenField ID="hdParcial" runat="server" Value='<%# Eval("parcial") %>' />
        </ItemTemplate>
    </asp:TemplateField>
</Columns>
</asp:GridView>
</td>
</tr>
<tr style="height:3px;">
<td colspan="2">&nbsp;</td>
</tr>
<tr>
<td colspan="2" align="center" valign="middle">
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
<asp:Panel ID="pnlArchivo" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Imagen de la lista</td>
</tr>
<tr>
<td align="center" style="width:900px; height: 20px;" colspan="2">
    <br />
&nbsp; </td>
</tr>
<tr>
<td align="right" style="width:400px;" class="Cellformat1" >Imagen:
</td>
    <td align="left" style="width:500px;">
        <asp:FileUpload ID="flArchivoImagen" runat="server"
            CssClass="TextInputFormat" />
    </td>
</tr>
    <tr>
        <td align="center" colspan="2" style="width:800px;">
            <br />
            <asp:ImageButton ID="btnSubirImagen" runat="server" CssClass="UpLoadFormat1"
            ImageUrl="~/imagenes/dummy.ico" onclick="btnSubirImagen_Click" Text="Agregar Imagen" />
            <asp:ImageButton ID="btnCancelarImagen" runat="server" CssClass="BackFormat1"
            ImageUrl="~/imagenes/dummy.ico" onclick="btnCancelarImagen_Click" Text="Regresar" />
        </td>
    </tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlPrecio" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" style="height: 18px;">
    Listas de Precios - Agregar Precios</td>
</tr>
<tr>
<td align="center" style="width:800px; height: 20px;">
    <br />
&nbsp; </td>
</tr>
<tr>
<td align="center" style="width:800px;" class="Cellformat1" >
Lista de Precios:&nbsp;<asp:Label ID="lblNombre" runat="server" Text="" class="Cellformat1"></asp:Label><br />
</td>
</tr>
<tr>
<td class="Cellformat1" align="center" style="width: 900px">
Ingrese los productos en el formato:
<br />
<br />
<i>Código&lt;separador&gt;Precio</i><br />
<asp:TextBox ID="txtEjemplo" CssClass="TextInputFormat" runat="server" Enabled="false" Width="210" Height="59" TextMode="MultiLine">8565656;$214.74 8565658;$220.78 12017505;$134.00
</asp:TextBox><br />
<asp:RadioButtonList ID="rdSeparador" runat="server" RepeatDirection="Horizontal">
<asp:ListItem Value="0" Text="Tabulador" Selected="True" />
<asp:ListItem Value="1" Text="; (Punto y coma)" />
<asp:ListItem Value="2" Text="| (Barra vertical)" />
</asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="center" style="width: 800px">
Ingrese los datos a agregar:
<br />
<asp:TextBox ID="txtPrecios" CssClass="TextInputFormat" runat="server" Width="250" Height="240" TextMode="MultiLine">
</asp:TextBox>
</td>
</tr>
<tr>
<td align="center" colspan="2" style="width:800px;">
    <br />
    <asp:ImageButton ID="btnValidarPrecio" runat="server" CssClass="SalidaFormat1"
        ImageUrl="~/imagenes/dummy.ico"
        onclick="btnValidarPrecio_Click" Text="Validar" />
    <asp:ImageButton ID="btnGuardarPrecio" runat="server" CssClass="AddFormat1"
        ImageUrl="~/imagenes/dummy.ico"
        onclick="btnGuardarPrecio_Click" Text="Guardar" />
    <asp:ImageButton ID="btnCancelarPrecio" runat="server" CssClass="BackFormat1"
        ImageUrl="~/imagenes/dummy.ico" onclick="btnCancelarPrecio_Click" Text="Regresar" />
</td>
</tr>
</table>
</asp:Panel>
<script type="text/javascript" language="javascript">
if(document.getElementById("<%=chkDesc.ClientID%>") != null)
{
    document.getElementById("<%=chkDesc.ClientID%>").disabled = true;
    document.getElementById("<%=rdFijo.ClientID%>").disabled = true;
    document.getElementById("<%=rdPorcentaje.ClientID%>").disabled = true;
}
function activar_copia()
{
    if(document.getElementById("<%=chkCopiar.ClientID%>").checked)
    {
        document.getElementById("<%=dlListasPrecios.ClientID%>").disabled = false;
        document.getElementById("<%=rdFijo.ClientID%>").disabled = false;
        document.getElementById("<%=rdPorcentaje.ClientID%>").disabled = false;
        activar_fijo_desc();
    }
    else
    {
        document.getElementById("<%=dlListasPrecios.ClientID%>").disabled = true;
        document.getElementById("<%=rdFijo.ClientID%>").disabled = true;
        document.getElementById("<%=rdPorcentaje.ClientID%>").disabled = true;
        document.getElementById("<%=dlListasPrecios.ClientID%>").disabled = true;
        document.getElementById("<%=txtPorcentaje.ClientID%>").disabled = true;
        document.getElementById("<%=chkDesc.ClientID%>").disabled = true;
    }
}

function activar_fijo_desc()
{
    if(document.getElementById("<%=rdFijo.ClientID%>").checked)
    {
        document.getElementById("<%=txtPorcentaje.ClientID%>").disabled = true;
        document.getElementById("<%=chkDesc.ClientID%>").disabled = true;
    }
    else
    {
        document.getElementById("<%=txtPorcentaje.ClientID%>").disabled = false;
        document.getElementById("<%=chkDesc.ClientID%>").disabled = false;
    }
}
function productoSeleccionado(sender, e) {
    $get('<%=hdProductoID.ClientID%>').value = e.get_value();
    setTimeout('setProductoPrecio()', 50);
}
function setProductoPrecio() {
    $get('<%=txtPrecioUnitario.ClientID%>').focus();
    $get('<%=txtPrecioUnitario.ClientID%>').select();
}
function limpiarProdID() {
    $get('<%=txtProducto.ClientID%>').value = '';
    $get('<%=hdProductoID.ClientID%>').value = '';
}
</script>
</asp:Content>