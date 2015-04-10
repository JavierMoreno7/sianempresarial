<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="notas_cargo.aspx.cs" Inherits="notas_notas_cargo" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField runat="server" ID="hdInventarios" Value="0" />
<asp:HiddenField runat="server" ID="hdInvAlmacen" Value="0" />
<asp:HiddenField runat="server" ID="hdHonorarios" Value="0" />
<asp:HiddenField runat="server" ID="hdPorcIVARet" Value="0" />
<asp:HiddenField runat="server" ID="hdPorcISRRet" Value="0" />
<asp:HiddenField runat="server" ID="hdMoneda" Value="" />
<asp:HiddenField runat="server" ID="hdMonedaTemp" Value="" />
<asp:HiddenField runat="server" ID="hdMinimo" Value="" />
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:Label ID="lblDummy" runat="server" Text="" />
<asp:Panel runat="server" ID="pnlDummy">
</asp:Panel>
<cc1:BalloonPopupExtender ID="bpeDummy" runat="server"
TargetControlID="lblDummy" BehaviorID="popDummy"
BalloonPopupControlID="pnlDummy"
Position="BottomLeft"
BalloonStyle="Custom"
CustomClassName="custom"
CustomCssUrl="../css/popnotas.css"
UseShadow="false"
ScrollBars="None"
DisplayOnMouseOver="true"
DisplayOnFocus="false"
DisplayOnClick="false" />
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="HeaderTitulos" colspan="3" style="height: 18px;">
                &emsp;Notas de Cargo - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Text="Folio" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Nota" Value="5"></asp:ListItem>
                    <asp:ListItem Selected="True" Text="Cliente" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Fecha Creación" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Fecha Cancelación" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Estatus" Value="4"></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>
                &nbsp;<asp:ImageButton
                    ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/buscar.png"
                    OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
            <td style="width: 108px;" align="left">
                <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos
                los Registros</asp:LinkButton></td>
            <td style="width: 109px;" align="left">
                <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat"
                    onclick="lblAgregar_Click">Generar Nota de Cargo</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="800px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="notaID"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="notaID" CommandName="Modificar" HeaderText="Folio" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nota" HeaderText="Nota" SortExpression="5" >
                    <HeaderStyle Width="165px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="negocio" HeaderText="Cliente" SortExpression="1" >
                    <HeaderStyle Width="180px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="2" >
                    <HeaderStyle Width="120px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_cancelacion" HeaderText="Fecha Cancelación" SortExpression="3" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="estatus" HeaderText="Estatus" SortExpression="4" >
                    <HeaderStyle Width="75px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:ButtonField DataTextField="monto" HeaderText="Monto" CommandName="Pagos">
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Right" ForeColor="#6CA2B7" />
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
<asp:Panel ID="pnlPagos" runat="server" CssClass="modalPopup" style="display:none;width:480px;padding:10px" HorizontalAlign="Center">
<asp:Panel ID="pnlPagosHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="lblPagos" runat="server" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:GridView ID="gvPagos" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" CellPadding="0" Width="400"
    AutoGenerateColumns="false" HorizontalAlign="Center" EnableViewState="True"
    CaptionAlign="Top" GridLines="None">
    <FooterStyle HorizontalAlign="Right"></FooterStyle>
<Columns>
    <asp:BoundField DataField="tipo" HeaderText="Pago">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="referencia" HeaderText="Referencia">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Fecha Pago">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="monto" HeaderText="Monto Pago">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
</Columns>
</asp:GridView>
    <asp:Button ID="btnPagosCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyPagos" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCambiarPagos" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdPagosBehaviorID"
    TargetControlID="btnDummyPagos"
    PopupControlID="pnlPagos"
    CancelControlID="btnPagosCerrar"
    DropShadow="False" />
<asp:HiddenField ID="hdFactID" runat="server" />
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="HeaderTitulos" colspan="4" style="height: 18px;">
    &emsp;Nota de Cargo</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left">Asesor de ventas:</td>
<td class="Cellformat1" align="left" colspan="2">
<asp:DropDownList ID="dlVendedor" runat="server" CssClass="SelectFormat"
        DataValueField="vendedorID" DataTextField="vendedor">
</asp:DropDownList>
&nbsp;&nbsp;Folio:
<b><asp:Label runat="server" ID="lblNotaID" class="Cellformat1"></asp:Label></b>
</td>
<td class="Cellformat1" align="right">Pagos:
<asp:LinkButton ID="lnkPagos" runat="server" CssClass="LinkFormat"
onclick="lnkPagos_Click" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Nota de Cargo:</td>
<td class="Cellformat1" align="left" colspan="3">
<b><asp:Label runat="server" ID="lblNota" class="Cellformat1"></asp:Label></b>
&nbsp;&nbsp;Fecha Timbrado:
<b><asp:Label runat="server" ID="lblFecha_Timbrado" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td></td>
<td align="left" colspan="3">
<asp:Label runat="server" ID="lblCreado" class="CellInfo"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Fecha:</td>
<td align="left" style="width:200px;">
<asp:Textbox id="txtFecha" runat="server" Width="100px" CssClass="datePicker"></asp:Textbox>
</td>
<td class="Cellformat1" align="left" style="width:50px;">Estatus:</td>
<td align="left" style="width:400px;">
<asp:DropDownList ID="dlEstatus" runat="server" CssClass="SelectFormat" DataValueField="estatusID" DataTextField="estatus" Enabled="false">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdEstatus" Value="" />
<asp:Label runat="server" ID="lblCorreoEnvio" class="Cellformat1"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Cliente:</td>
<td align="left" style="width:650px;" colspan="3">
<asp:TextBox ID="txtSucursal" runat="server" MaxLength="200" CssClass="TextInputFormat" Width="500px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="AutoCompleteExtender1"
                TargetControlID="txtSucursal"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerSucursales"
                MinimumPrefixLength="1"
                CompletionInterval="1000"
                OnClientItemSelected="sucursalSeleccionada"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement"
                CompletionListItemCssClass="autocomplete_listItem"
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdSucursalID" Value="" />
&nbsp;<asp:Image runat="server" ID="imgInfo" ImageUrl="~/imagenes/info.jpg" CssClass="ButtonFormat" Width="16" Height="16" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Lista de precios:</td>
<td align="left" style="width:200px;">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista" AutoPostBack="true" OnSelectedIndexChanged="dlListaPrecios_SelectedIndexChanged">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left" style="width:50px;">IVA:</td>
<td align="left" style="width:400px;">
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1" AutoPostBack="True" OnSelectedIndexChanged="rdIVA_SelectedIndexChanged">
</asp:RadioButtonList>
<asp:CheckBox ID="chkIVARet" runat="server" Text="Retener IVA" AutoPostBack="true"
        CssClass="GridFormatTD" oncheckedchanged="chkIVARet_CheckedChanged" />
<asp:CheckBox ID="chkISRRet" runat="server" Text="Retener ISR" AutoPostBack="true"
        CssClass="GridFormatTD" oncheckedchanged="chkISRRet_CheckedChanged" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Moneda:</td>
<td align="left">
<asp:DropDownList ID="dlMoneda" runat="server" CssClass="SelectFormat" DataValueField="monedaID" DataTextField="moneda"
    AutoPostBack="true" OnSelectedIndexChanged="dlMoneda_SelectedIndexChanged">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left" colspan="2">Tipo de cambio:
<asp:TextBox ID="txtTipoCambio" runat="server" MaxLength="6" Enabled="false"
        CssClass="TextReadOnlyD" Width="60px" Text="1.00"></asp:TextBox>
<asp:ImageButton ID="btnRefrescarTipoCambio" runat="server" CssClass="ButtonFormat" Height="12px" Width="12px" ImageUrl="~/imagenes/recargar.gif"
     OnCommand="btnRefrescarTipoCambio_Command" ToolTip="Refrescar" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Tipo</td>
<td class="Cellformat1" align="left" style="width:200px;">
<asp:RadioButtonList runat="server" ID="rdTipo" RepeatDirection="Horizontal">
<asp:ListItem Selected="True" Text="Contado" Value="True"></asp:ListItem>
<asp:ListItem Text="Crédito" Value="False"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="left" colspan="2">
Descuento1:<asp:TextBox ID="txtDescuento1" runat="server" MaxLength="6"
        CssClass="TextInputFormatD" Width="40px" AutoPostBack="True"
        ontextchanged="txtDescuento1_TextChanged" ></asp:TextBox>%
Descuento2:<asp:TextBox ID="txtDescuento2" runat="server" MaxLength="6"
        CssClass="TextInputFormatD" Width="40px" AutoPostBack="True"
        ontextchanged="txtDescuento2_TextChanged" ></asp:TextBox>%
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Comentarios:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtComentarios" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:Label ID="lblMensaje" runat="server" CssClass="msgLabel"></asp:Label><br />
<asp:HyperLink ID="lnkSAT" runat="server" NavigateUrl="https://portalcfdi.notaelectronica.sat.gob.mx/" Text="Cancelación en el sitio del SAT" Target="_blank" ></asp:HyperLink>
</td>
</tr>
<tr>
<td align="center" colspan="4">
<br />
<asp:ImageButton ID="btnModificar" runat="server"
        ToolTip="Modificar" ImageUrl="~/imagenes/modificar.png" ValidationGroup="valDatos"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnFE" runat="server"
        ToolTip="Timbrar" ImageUrl="~/imagenes/sat.png"
        ValidationGroup="valDatos" onclick="btnFE_Click" />
<asp:ImageButton ID="btnXML" runat="server"
        ToolTip="Generar XML" ImageUrl="~/imagenes/excel.png"
        ValidationGroup="valDatos" onclick="btnXML_Click" />
<asp:ImageButton ID="btnImprimir" runat="server"
        ToolTip="Imprimir" ImageUrl="~/imagenes/reporte.png" ValidationGroup="valDatos"
        onclick="btnImprimir_Click" />
<asp:ImageButton ID="btnEmail" runat="server"
        ToolTip="Correo" ImageUrl="~/imagenes/email.png" ValidationGroup="valDatos"
        onclick="btnEmail_Click" />
<asp:ImageButton ID="btnCancelar" runat="server"
        ToolTip="Cancelar" ImageUrl="~/imagenes/cancelar.png"
        ValidationGroup="valDatos" OnClientClick="showModalCancelar();return false;" />
<asp:ImageButton ID="btnRegresar" runat="server"  ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/salir.png" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlDatos2" Visible="false" runat="server" DefaultButton="btnAgregarProd">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" colspan="4" class="Titleformat1">
    Conceptos
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" colspan="4">
<table>
<tr>
<td class="GridFormatTD" align="left" style="width:450px;">Concepto</td>
<td class="GridFormatTD" align="left" style="width:60px;">Cantidad</td>
<td class="GridFormatTD" align="left" style="width:290px;">Precio (<asp:Label runat="server" ID="lblMoneda" />)</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:450px;">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="445px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="acProducto"
                TargetControlID="txtProducto"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerConceptos"
                MinimumPrefixLength="1"
                CompletionInterval="1000"
                OnClientItemSelected="productoSeleccionado"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement"
                CompletionListItemCssClass="autocomplete_listItem"
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
</td>
<td class="Cellformat1" align="left" valign="top" style="width:60px;">
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="8" CssClass="TextInputFormatD" Width="60px" ></asp:TextBox>
</td>
<td align="left" valign="top" style="width:290px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="15" CssClass="TextInputFormatD" Width="75px" ></asp:TextBox>
<asp:Button ID="btnDetalle" runat="server" Text="Detalle" CssClass="ButtonFormat" OnClientClick="showModalDetalle();return false;" />
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
<asp:CheckBox ID="chkCambiarPrecios" runat="server" Text="Actualizar precios" CssClass="GridFormatTD" />
<asp:HiddenField runat="server" ID="hdPrecioUnitario" />
<asp:HiddenField runat="server" ID="hdProductoPrecioID" />
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
    Height="1px" SelectedIndex="0" CellPadding="0" Width="800"
    AutoGenerateColumns="false" DataKeyNames="productoID"
    HorizontalAlign="Center" EnableViewState="True" ShowFooter="true"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvProductos_RowCommand"
        onrowdatabound="gvProductos_RowDataBound">
    <FooterStyle HorizontalAlign="Right"></FooterStyle>
<Columns>
    <asp:ButtonField DataTextField="id" CommandName="Modificar" >
        <HeaderStyle Width="20px" />
        <ItemStyle HorizontalAlign="Right" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:TemplateField HeaderStyle-Width="25px">
    <ItemTemplate>
    <table><tr>
        <td><asp:ImageButton runat="server" ID="btnUP" ImageUrl="~/imagenes/up.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnUP_Click" /></td>
        <td><asp:ImageButton runat="server" ID="btnDN" ImageUrl="~/imagenes/dn.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnDN_Click" /></td>
        <td><asp:ImageButton runat="server" ID="btnPos" ImageUrl="~/imagenes/updn.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnMV_Click" />
        </td>
    </tr></table>
        <asp:HiddenField runat="server" ID="hdCostoOriginal" Value='<%# DataBinder.Eval(Container.DataItem, "costo_original") %>' />
        <asp:HiddenField runat="server" ID="hdCostoOriginalMoneda" Value='<%# DataBinder.Eval(Container.DataItem, "costo_original_moneda") %>' />
    </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="producto" HeaderText="Producto" HtmlEncode="false">
        <HeaderStyle Width="465px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="cantidad" HeaderText="Cantidad">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="costo_unitario" HeaderText="Precio Unitario">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="costo" HeaderText="Subtotal">
        <HeaderStyle Width="80px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
</Columns>
</asp:GridView>
<asp:HiddenField ID="hdCosto" runat="server" Value="0" />
<asp:HiddenField ID="hdBorrar" runat="server" Value="0" />
<asp:HiddenField ID="hdCostoDescuento" runat="server" Value="0" />
<asp:HiddenField ID="hdCostoIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdSubtotal" runat="server" Value="0" />
<asp:HiddenField ID="hdISRRet" runat="server" Value="0" />
<asp:HiddenField ID="hdIVARet" runat="server" Value="0" />
<asp:HiddenField ID="hdTotal" runat="server" Value="0" />
<asp:HiddenField ID="hdConsecutivoID" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMin" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMax" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMaxID" runat="server" Value="0" />
<asp:HiddenField ID="hdTAX" runat="server" Value="0" />
</td>
</tr>
<tr>
<td align="right" colspan="4">
    <asp:Button ID="btnTAX" runat="server" Text="Ajustar Impuestos Manualmente" onclick="btnTAX_Click" CssClass="ButtonFormat" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlPrecios" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnPrecioContinuar">
<asp:Panel ID="pnlPreciosHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label10" runat="server" Text="Actualizar Precio" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Lista de Precios:</td>
        <td align="left" class="Cellformat1">
        <asp:Label ID="lblPrecioLista" runat="server" />
    </td></tr>
    <tr><td align="right" class="Cellformat1">Producto:</td>
        <td align="left" class="Cellformat1">
        <asp:Label ID="lblPrecioProducto" runat="server" />
    </td></tr>
    <tr><td align="right" class="Cellformat1">Precio Anterior:</td>
        <td align="left" class="Cellformat1">
        <asp:Label ID="lblPrecioAnterior" runat="server" />
    </td></tr>
    <tr><td align="right" class="Cellformat1">Precio Nuevo:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtPrecioUnitarioCambio" runat="server" MaxLength="15" Width="60px" CssClass="TextInputFormatD" ></asp:TextBox>
        <asp:Label ID="lblPrecioCambiarMoneda" runat="server" />
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnPrecioContinuar" runat="server" Text="Actualizar" onclick="btnPrecioContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnPrecioCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyPrecio" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCambiarPrecio" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdPrecioBehaviorID"
    TargetControlID="btnDummyPrecio"
    PopupControlID="pnlPrecios"
    CancelControlID="btnPrecioCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlDetalle" runat="server" CssClass="modalPopup" style="display:none;width:600px;padding:10px" HorizontalAlign="Center" DefaultButton="btnDetalleCerrar">
<asp:Panel ID="pnlDetalleHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label18" runat="server" Text="Detalle Producto" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Producto:</td>
        <td align="left" class="Cellformat1">
        <asp:Label ID="lblDetalleProducto" runat="server" />
    </td></tr>
    <tr><td align="right" valign="top" class="Cellformat1">Detalle:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtDetalle" runat="server" TextMode="MultiLine" Width="500px" Height="150px" CssClass="TextInputFormat" ></asp:TextBox>
        <br /><asp:CheckBox ID="chkImpDetalle" runat="server" Text="Imprimir detalle" />
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnDetalleCerrar" runat="server" Text="Continuar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyDetalle" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdDetalleProducto" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdDetalleBehaviorID"
    TargetControlID="btnDummyDetalle"
    PopupControlID="pnlDetalle"
    CancelControlID="btnDetalleCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlMV" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnMVContinuar">
<asp:Panel ID="pnlMVHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label6" runat="server" Text="Mover Producto" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
<center>
        <asp:Label ID="lblProdPos" class="Cellformat1" runat="server" />
    <br />
        <asp:Label ID="Label9" class="Cellformat1" runat="server" Text="Nueva posición: " />
        <asp:HiddenField runat="server" ID="hdPos" />
        <asp:TextBox ID="txtPosicion" runat="server" MaxLength="3" Width="40px" CssClass="TextInputFormat" ></asp:TextBox>
    <br />
    <asp:Button ID="btnMVContinuar" runat="server" Text="Mover" onclick="btnMVContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnMVCerrar" runat="server" Text="Cancelar" CssClass="ButtonFormat" />
    <br />
</center>
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyMV" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdMV" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdMVBehaviorID"
    TargetControlID="btnDummyMV"
    PopupControlID="pnlMV"
    CancelControlID="btnMVCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlCambiar" runat="server" CssClass="modalPopup" style="display:none;width:600px;padding:10px" HorizontalAlign="Center" DefaultButton="btnCambiarContinuar">
<asp:Panel ID="pnlCambiarHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label4" runat="server" Text="Actualizar Producto" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Producto:</td>
        <td align="left" class="Cellformat1">
        <asp:Label ID="lblCambiarProducto" runat="server" />
    </td></tr>
    <tr><td align="right" class="Cellformat1">Cantidad:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarCantidad" runat="server" MaxLength="8" Width="60px" CssClass="TextInputFormatD" ></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Precio (<asp:Label runat="server" ID="lblMonedaCambiar" />):</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarUnitario" runat="server" MaxLength="15" Width="60px" CssClass="TextInputFormatD" ></asp:TextBox>
    </td></tr>
	<tr><td align="right" valign="top" class="Cellformat1">Detalle:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarDetalle" runat="server" TextMode="MultiLine" Width="500px" Height="150px" CssClass="TextInputFormat" ></asp:TextBox>
        <br /><asp:CheckBox ID="chkCambiarImpDet" runat="server" Text="Imprimir detalle" />
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnCambiarContinuar" runat="server" Text="Actualizar" onclick="btnCambiarContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnCambiarCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyCambiar" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCambiarProducto" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdCambiarBehaviorID"
    TargetControlID="btnDummyCambiar"
    PopupControlID="pnlCambiar"
    CancelControlID="btnCambiarCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlMessageCancelar" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnCancelarContinuar">
<asp:Panel ID="pnlMessageCancelarHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label1" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="lblMessageCancelar" runat="server" Text="Motivo de la cancelación" CssClass="msgError1" /><br />
    <asp:TextBox ID="txtMotivoCancelacion" runat="server" MaxLength="150" CssClass="TextInputFormat" Width="300px" ></asp:TextBox><br />
    <br />
    <asp:Label runat="server" ID="Label2" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigo_Ver_Canc" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br /><br />
    <asp:Button ID="btnCancelarContinuar" runat="server" Text="Continuar" OnClientClick="hideModalCancelar();return true;" onclick="btnCancelarContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnCancelarCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyCancelar" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCancelar" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdCancelarBehaviorID"
    TargetControlID="btnDummyCancelar"
    PopupControlID="pnlMessageCancelar"
    CancelControlID="btnCancelarCerrar"
    DropShadow="False" />
<asp:Panel runat="server" ID="pnlInfo" CssClass="popNotas">
    <asp:Label ID="lblNotas" runat="server" Text="" />
</asp:Panel>
<cc1:BalloonPopupExtender ID="PopupControlExtender2" runat="server"
TargetControlID="imgInfo" BehaviorID="popNegocio"
BalloonPopupControlID="pnlInfo"
Position="BottomLeft"
BalloonStyle="Custom"
CustomClassName="custom"
CustomCssUrl="../css/popnotas.css"
UseShadow="false"
ScrollBars="None"
DisplayOnMouseOver="true"
DisplayOnFocus="false"
DisplayOnClick="false" />
<asp:Panel ID="pnlTAX" runat="server" CssClass="modalPopup" style="display:none;width:250px;padding:10px" HorizontalAlign="Center" DefaultButton="btnTAXCalcular">
<asp:Panel ID="pnlTAXHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label20" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Subtotal:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtSubtotal" runat="server" CssClass="TextInputFmtDer" Width="90px" MaxLength="10"></asp:TextBox>
    </td></tr>
    <tr><td align="right" valign="top" class="Cellformat1">IVA:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtIVA" runat="server" CssClass="TextInputFmtDer" Width="90px" MaxLength="10"></asp:TextBox>
    </td></tr>
    <tr><td align="right" valign="top" class="Cellformat1">Retención IVA:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtIVARet" runat="server" CssClass="TextInputFmtDer" Width="90px" MaxLength="10" Enabled="false"></asp:TextBox>
    </td></tr>
    <tr><td align="right" valign="top" class="Cellformat1">Retención ISR:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtISRRet" runat="server" CssClass="TextInputFmtDer" Width="90px" MaxLength="10" Enabled="false"></asp:TextBox>
    </td></tr>
    <tr><td align="right" valign="top" class="Cellformat1">TOTAL:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtTotal" runat="server" CssClass="TextInputFmtDer" Width="90px" MaxLength="10"></asp:TextBox>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnTAXCalcular" runat="server" Text="Calcular" CssClass="ButtonFormat" />
    <asp:Button ID="btnTAXContinuar" runat="server" Text="Registrar cambio" CssClass="ButtonFormat" onclick="btnTAXContinuar_Click" />
    <asp:Button ID="btnTAXCancelar" runat="server" Text="Cancelar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyTAX" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdTAX" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdTAXBehaviorID"
    TargetControlID="btnDummyTAX"
    PopupControlID="pnlTAX"
    CancelControlID="btnTAXCancelar"
    DropShadow="False" />
<asp:Panel ID="pnlPago" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnPagoContinuar">
<asp:Panel ID="pnlPagoHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label8" runat="server" Text="Ingrese Pago" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Fecha pago:</td>
        <td align="left" class="Cellformat1">
        <asp:Textbox id="txtFechaPago" runat="server" Width="100px" CssClass="datePicker"></asp:Textbox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Tipo de pago:</td>
        <td align="left" class="Cellformat1">
        <asp:DropDownList ID="dlTiposPagos" runat="server" CssClass="SelectFormat"
        DataValueField="tipo_pagoID" DataTextField="tipo_pago"></asp:DropDownList>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Referencia:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtReferencia" runat="server" CssClass="TextInputFormat" Width="200px" MaxLength="50"></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Moneda:</td>
        <td align="left" class="Cellformat1">
        <asp:DropDownList ID="dlMonedaPago" runat="server" CssClass="SelectFormat" DataValueField="monedaID" DataTextField="moneda" AutoPostBack="true" OnSelectedIndexChanged="dlMonedaPago_SelectedIndexChanged">
        </asp:DropDownList>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Monto del pago:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtPago" runat="server" CssClass="TextReadOnlyD" Width="90px" MaxLength="10"></asp:TextBox>
        <asp:HiddenField ID="hdPago" runat="server" />
    </td></tr>
    <tr><td align="right" class="Cellformat1">Cuenta Bancaria:</td>
        <td align="left" class="Cellformat1">
        <asp:DropDownList ID="dlCuentaBancaria" runat="server" CssClass="SelectFormat" DataValueField="cuentaID" DataTextField="cuenta">
        </asp:DropDownList>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnPagoContinuar" runat="server" Text="Registrar Pago" OnClientClick="hideModalPago();return true;" onclick="btnPagoContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnPosponer" runat="server" Text="Pago posterior" OnClientClick="hideModalPago();return true;" onclick="btnPosponer_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnPagoCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyPago" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdPago" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdPagoBehaviorID"
    TargetControlID="btnDummyPago"
    PopupControlID="pnlPago"
    CancelControlID="btnPagoCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlCorreo" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnCorreoContinuar">
<asp:Panel ID="pnlCorreoHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label3" runat="server" Text="Enviar Correo" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="center" class="Cellformat1">
    <asp:Label ID="lblCorreo" runat="server" />
    </td></tr>
    <tr><td align="center" class="Cellformat1">
    <asp:Label ID="lblEnviar" runat="server" Text="Enviar nota electrónica por correo?" />
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnCorreoContinuar" runat="server" Text="Enviar Correo" OnClientClick="hideModalCorreo();return true;" onclick="btnCorreoContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnCorreoCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyCorreo" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCorreo" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdCorreoBehaviorID"
    TargetControlID="btnDummyCorreo"
    PopupControlID="pnlCorreo"
    CancelControlID="btnCorreoCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlMessageCancelarWS" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnCancelarWSContinuar">
<asp:Panel ID="pnlMessageCancelarWSHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label15" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="lblMessageCancelarWS" runat="server" Text="Motivo de la cancelación" CssClass="msgError1" /><br /><br />
    <asp:Label ID="Label16" runat="server" CssClass="msgError1">La nota de cargo no pudo ser cancelada automáticamente, pero puede cancelarse manualmente a través del sitio del SAT</asp:Label><br /><br />
    <asp:Label ID="Label17" runat="server" CssClass="msgError1">Presione el botón Continuar para hacer la cancelación en el sistema y abrir una nueva ventana con la página del SAT para hacer la cancelación manual</asp:Label><br />
    <asp:Button ID="btnCancelarWSContinuar" runat="server" Text="Continuar" OnClientClick="AbrirSAT();return true;" onclick="btnCancelarWSContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnCancelarWSCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyCancelarWS" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCancelarWS" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdCancelarWSBehaviorID"
    TargetControlID="btnDummyCancelarWS"
    PopupControlID="pnlMessageCancelarWS"
    CancelControlID="btnCancelarWSCerrar"
    DropShadow="False" />
<script type="text/javascript">
    function Limite(e, campo, maximo) {
        var tecla;
        if (window.event)
            tecla = window.event.keyCode;
        else
            tecla = e.which;

        if (tecla == 38 || tecla == 40 || tecla == 37 || tecla == 39 || tecla == 33 || tecla == 34 || tecla == 36 || tecla == 35 || tecla == 16 || tecla == 17 || tecla == 18 || tecla == 20 || tecla == 46 || tecla == 8 || tecla == 13 || tecla == 9 || tecla == 27)
            return true;
        else
            if (campo.value.length > maximo - 1)
                return false;
            else
                return true;
    }
    function prevenirPaste(campo, maximo) {
        var restoCaracteres;
        var datos = window.clipboardData.getData("Text");
        var nvosDatos;

        restoCaracteres = maximo - campo.value.length;
        if (restoCaracteres <= 0)
            return false;
        else {
            nvosDatos = datos.substr(0, restoCaracteres);
            window.clipboardData.setData("Text", nvosDatos);
            return true;
        }
    }

    function sucursalSeleccionada(sender, e) {
        $get('<%=hdSucursalID.ClientID%>').value = e.get_value();
        PageMethods.Obtener_NotasWeb(e.get_value(), colocarNotas);
    }

    function colocarNotas(rs) {
        // 0 - Notas
        // 1 - Descuento
        // 2 - Descuento2
        // 3 - IVA
        // 4 - Lista precios
        // 5 - Contado/Credito
        // 6 - Moneda
        // 7 - Vendedor
        var valores = rs.split("|");
        $get('<%=lblNotas.ClientID%>').innerHTML = valores[0];
        $get('<%=txtDescuento1.ClientID%>').value = valores[1];
        $get('<%=txtDescuento2.ClientID%>').value = valores[2];
        var rdIVA = document.getElementsByName('<%=rdIVA.UniqueID%>');
        for (i = 0; i < rdIVA.length; i++) {
            if (rdIVA[i].value == valores[3])
                rdIVA[i].checked = true;
            else
                rdIVA[i].checked = false;
        }
        var dlLista = $get('<%=dlListaPrecios.ClientID%>');
        for (i = 0; i < dlLista.length; i++) {
            if (dlLista[i].value == valores[4])
                dlLista[i].selected = true;
            else
                dlLista[i].selected = false;
        }
        var rdTipo = document.getElementsByName('<%=rdTipo.UniqueID%>');
        if (valores[5] == "1") {
            rdTipo[0].checked = true;
            rdTipo[1].checked = false;
        }
        else {
            rdTipo[0].checked = false;
            rdTipo[1].checked = true;
        }
        var dlMoneda = $get('<%=dlMoneda.ClientID%>');
		if (valores[6] == "") {
            valores[6] = "MX";
            $get('<%=dlMoneda.ClientID%>').disabled = false;
        }
        else {
            $get('<%=dlMoneda.ClientID%>').disabled = true;
        }
        for (i = 0; i < dlMoneda.length; i++) {
            if (dlMoneda[i].value == valores[6])
                dlMoneda[i].selected = true;
            else
                dlMoneda[i].selected = false;
        }

        if (valores[7] != "0") {
            var dlVendedor = $get('<%=dlVendedor.ClientID%>');
            for (i = 0; i < dlVendedor.length; i++) {
                if (dlVendedor[i].value == valores[7])
                    dlVendedor[i].selected = true;
                else
                    dlVendedor[i].selected = false;
            }
        }
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtPrecioUnitario.ClientID%>').value = PageMethods.ObtenerPrecio($get('<%=hdSucursalID.ClientID%>').value +
                                                                                '|' + e.get_value() +
                                                                                '|' + $get('<%=dlListaPrecios.ClientID%>').value +
                                                                                '|' + $get('<%=hdMoneda.ClientID%>').value,
                                                                                colocarPrecio);
        $get('<%=btnAgregarProd.ClientID%>').disabled = false;
    }

    function colocarPrecio(precio) {
        var valores = precio.split("|");
        $get('<%=txtPrecioUnitario.ClientID%>').value = valores[0];
        $get('<%=hdPrecioUnitario.ClientID%>').value = valores[0];
        $get('<%=txtCantidad.ClientID%>').focus();

        $get('<%=hdMinimo.ClientID%>').value = valores[3];
    }

    function showModalCancelar(ev) {
        var modalPopupBehavior = $find('mdCancelarBehaviorID');
        modalPopupBehavior.show();
    }

    function hideModalCancelar(ev) {
        var modalPopupBehavior = $find('mdCancelarBehaviorID');
        modalPopupBehavior.hide();
    }

    function showModalDetalle(ev) {
        $get('<%=lblDetalleProducto.ClientID%>').innerHTML = $get('<%=txtProducto.ClientID%>').value
        var modalPopupBehavior = $find('mdDetalleBehaviorID');
        modalPopupBehavior.show();
        setFoco('<%=txtDetalle.ClientID%>');
    }

    function hideModalPago(ev) {
        var modalPopupBehavior = $find('mdPagoBehaviorID');
        modalPopupBehavior.hide();
    }

    function hideModalCorreo(ev) {
        var modalPopupBehavior = $find('mdCorreoBehaviorID');
        modalPopupBehavior.hide();
    }

    function setProductoFoco() {
        $get('<%=txtProducto.ClientID%>').focus();
    }

    function setProductoCantidad() {
        $get('<%=txtCambiarCantidad.ClientID%>').focus();
        $get('<%=txtCambiarCantidad.ClientID%>').select();
    }

    function setAgrProd() {
        $get('<%=btnAgregarProd.ClientID%>').focus();
    }

    function setProductoPrecio() {
        $get('<%=txtPrecioUnitario.ClientID%>').focus();
        $get('<%=txtPrecioUnitario.ClientID%>').select();
    }

    function setCantidad() {
        $get('<%=txtCantidad.ClientID%>').focus();
    }
    function setPos() {
        $get('<%=txtPosicion.ClientID%>').focus();
    }

    function mostrarPopUp(strURL) {
        window.open(strURL, "SIANRPT", "location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=900, height=500,left=40,top=50");
    }

    function mostrarMailPopUp(strURL) {
        window.open(strURL, "SIANRPT", "location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=630, height=500,left=40,top=50");
    }
    function esconder() {
        var ballonPop = $find('popNegocio');
        ballonPop.hidePopup();
    }
    function esconder2() {
        var ballonPop = $find('popNegocio2');
        ballonPop.hidePopup();
    }

    function validateCambiar(n) {
        if (n.value.length == 0)
            n.value = '0';
        if (isNaN(n.value)) {
            alert('Monto no válido');
            n.focus();
            n.select();
            return false;
        }
    }
    function AbrirSAT() {
        window.open("https://portalcfdi.facturaelectronica.sat.gob.mx/");
    }
    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
        get('<%=txtDetalle.ClientID%>').value = '';
    }

    function sumarPago(txtSubtotal, txtIVA, txtIVARet, txtISRRet, txtTotal) {
        var iva = parseFloat('0');
        var ivaret = parseFloat('0');
        var isrret = parseFloat('0');

        if ($get(txtIVA).value != '' && $get(txtIVA).value != '.')
            iva = parseFloat($get(txtIVA).value);
        else
            $get(txtIVA).value = '0';

        if ($get(txtIVARet).value != '' && $get(txtIVARet).value != '.')
            ivaret = parseFloat($get(txtIVARet).value);
        else
            $get(txtIVARet).value = '0';

        if ($get(txtISRRet).value != '' && $get(txtISRRet).value != '.')
            isrret = parseFloat($get(txtISRRet).value);
        else
            $get(txtISRRet).value = '0';

        var total = parseFloat($get(txtSubtotal).value) + iva - ivaret -isrret;

        $get(txtTotal).value = total.toFixed(2);
    }
</script>
</asp:Content>