<%@ Page Title="" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="cotizacion.aspx.cs" Inherits="compras_cotizacion" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:Panel ID="pnlListado" runat="server" DefaultButton="btnBuscar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 1200px;">
<tr>
    <td class="GridFormat" colspan="3" style="height: 18px;">
        Compras - Cotización a Proveedores - Listado de Datos</td>
</tr>
<tr>
    <td style="width: 600px;  height: 20px;" align="left" class="Cellformat1">
        Buscar por:
        <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
            <asp:ListItem Selected="True" Text="Proveedor" Value="0"></asp:ListItem>
            <asp:ListItem Text="Cliente" Value="1"></asp:ListItem>
            <asp:ListItem Text="Producto" Value="2"></asp:ListItem>
        </asp:DropDownList>&nbsp;
        <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>
        &nbsp;<asp:ImageButton
            ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
            OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
    <td style="width: 150px;" align="left">
        <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos 
        los Registros</asp:LinkButton></td>
    <td style="width: 450px;" align="left">
        <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat" 
            onclick="lblAgregar_Click">Agregar Cotización</asp:LinkButton></td>
</tr>
<tr style="height:3px;">
<td colspan="3">&nbsp;</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 1100px;">
<tr>
    <td style="text-align: center; vertical-align: top;">
    <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
        Height="1px" SelectedIndex="0" Width="1200px" AllowSorting="True" 
            CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="cotizacionID"
        HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True" 
        EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None" 
            onrowcommand="grdvLista_RowCommand">
    <Columns>
        <asp:BoundField DataField="fecha" HeaderText="Fecha Creación" SortExpression="3" >
            <HeaderStyle Width="75px" />
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:BoundField DataField="negocio" HeaderText="Cliente" SortExpression="1" >
            <HeaderStyle Width="175px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:ButtonField DataTextField="producto" HeaderText="Producto" CommandName="Modificar" SortExpression="2" >
            <HeaderStyle Width="175px" />
            <ItemStyle HorizontalAlign="Left" ForeColor="#6CA2B7" />
        </asp:ButtonField>
        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" >
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
        <asp:BoundField DataField="contacto" HeaderText="Contacto" >
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="proveedor" HeaderText="Proveedor" SortExpression="0" >
            <HeaderStyle Width="150px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="descripcion" HeaderText="Descripción">
            <HeaderStyle Width="150px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="precio" HeaderText="Precio" >
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
        <asp:BoundField DataField="tiempo_entrega" HeaderText="Tiempo entrega" SortExpression="4" >
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="fecha_respuesta" HeaderText="Fecha respuesta" >
            <HeaderStyle Width="75px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
    </Columns>
    </asp:GridView>
    </td>
</tr>
<tr style="height:3px;">
<td>&nbsp;</td>
</tr>
<tr>
<td align="center" valign="middle">
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
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Cotización a proveedores</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Fecha:</td>
<td class="Cellformat1" align="left" style="width:700px;" colspan="3">
<asp:Textbox id="txtFecha" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFecha" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender6" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFecha" TargetControlID="txtFecha">
</cc1:CalendarExtender>&nbsp;&nbsp;
Cliente:
<asp:TextBox ID="txtNegocio" runat="server" MaxLength="200" CssClass="TextInputFormat" Width="500px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="AutoCompleteExtender4" 
TargetControlID="txtNegocio"
ServicePath="~/Services/ComboServices.asmx"
ServiceMethod="ObtenerNegocios"
MinimumPrefixLength="1" 
CompletionInterval="1000"
OnClientItemSelected="negocioSeleccionado"
CompletionSetCount="50"
CompletionListCssClass="autocomplete_completionListElement" 
CompletionListItemCssClass="autocomplete_listItem" 
CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdClienteID" Value="" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Producto:</td>
<td class="Cellformat1" align="left" style="width:700px;" colspan="3">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="500px" autocomplete="off"></asp:TextBox>
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
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />&nbsp;&nbsp;
Cantidad:
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Contacto:</td>
<td class="Cellformat1" align="left" style="width:700px;" colspan="3">
<asp:TextBox ID="txtContacto" runat="server" MaxLength="100" Width="200px" CssClass="TextInputFormat"></asp:TextBox>&nbsp;&nbsp;
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Proveedor:</td>
<td align="left" style="width:700px;" colspan="3">
<asp:TextBox ID="txtProveedor" runat="server" MaxLength="200" CssClass="TextInputFormat" Width="500px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="AutoCompleteExtender1" 
                TargetControlID="txtProveedor"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerProveedores"
                MinimumPrefixLength="1" 
                CompletionInterval="1000"
                OnClientItemSelected="proveedorSeleccionado"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement" 
                CompletionListItemCssClass="autocomplete_listItem" 
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdProveedorID" Value="" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Descripción:</td>
<td class="Cellformat1" align="left" colspan="3">
<asp:TextBox ID="txtDescripcion" runat="server" MaxLength="250" Width="500px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Precio:</td>
<td class="Cellformat1" align="left" colspan="3">
<asp:TextBox ID="txtPrecio" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Tiempo entrega:</td>
<td class="Cellformat1" align="left" colspan="3">
<asp:TextBox ID="txtTiempoEntrega" runat="server" MaxLength="100" Width="300px" CssClass="TextInputFormat"></asp:TextBox>&nbsp;&nbsp;
Fecha respuesta:
<asp:Textbox id="txtFechaRespuesta" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFechaRespuesta" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaRespuesta" TargetControlID="txtFechaRespuesta">
</cc1:CalendarExtender>
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:Label ID="lblMensaje" runat="server" CssClass="msgLabel"></asp:Label><br />
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1" 
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos" 
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnRegresar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>
<script type="text/javascript">
    function proveedorSeleccionado(sender, e) {
        $get('<%=hdProveedorID.ClientID%>').value = e.get_value();
    }

    function negocioSeleccionado(sender, e) {
        $get('<%=hdClienteID.ClientID%>').value = e.get_value();
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
    }

    function setCantidad() {
        $get('<%=txtCantidad.ClientID%>').focus();
        $get('<%=txtCantidad.ClientID%>').select();
    }
    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
</script>
</asp:Content>

