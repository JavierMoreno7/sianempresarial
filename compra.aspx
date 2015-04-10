<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="compra.aspx.cs" Inherits="compras_compra" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:HiddenField runat="server" ID="hdMoneda" Value="" />
<asp:HiddenField runat="server" ID="hdMonedaTemp" Value="" />
<asp:HiddenField runat="server" ID="hdMinimo" Value="" />
<asp:HiddenField runat="server" ID="hdAccion" Value="" />
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
<asp:Panel ID="pnlListado" runat="server" DefaultButton="btnBuscar">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="HeaderTitulos" colspan="3" style="height: 18px;">
                &emsp;Compras - Compras - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Text="Folio" Value="0"></asp:ListItem>
                    <asp:ListItem Selected="True" Text="Proveedor" Value="1"></asp:ListItem>
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
                    onclick="lblAgregar_Click">Agregar Compra</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="800px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="compraID"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="compraID" CommandName="Modificar" HeaderText="Folio" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="proveedor" HeaderText="Proveedor" SortExpression="1" >
                    <HeaderStyle Width="335px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_creacion" HeaderText="Fecha Creación" SortExpression="2" >
                    <HeaderStyle Width="120px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_cancelacion" HeaderText="Fecha Cancelación" SortExpression="3" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="estatus" HeaderText="Estatus" SortExpression="4" >
                    <HeaderStyle Width="75px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:ButtonField DataTextField="costo" HeaderText="Monto" CommandName="Pagos">
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
    Height="1px" SelectedIndex="0" CellPadding="0" Width="470"
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
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="HeaderTitulos" colspan="4" style="height: 18px;">
    &emsp;Compra</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Compra:</td>
<td class="Cellformat1" align="left" style="width:650px;" colspan="3">
<b><asp:Label runat="server" ID="lblCompra" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Proveedor:</td>
<td align="left" style="width:650px;" colspan="3">
<asp:TextBox ID="txtProveedor" runat="server" MaxLength="200" CssClass="TextInputFormat" Width="400px" autocomplete="off"></asp:TextBox>
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
&nbsp;<asp:Image runat="server" ID="imgInfo" ImageUrl="~/imagenes/info.jpg" CssClass="ButtonFormat" Width="16" Height="16" />
<asp:Panel runat="server" ID="pnlInfo" CssClass="popNotas">
    <asp:Label ID="lblNotas" runat="server" Text="" />
</asp:Panel>
<cc1:BalloonPopupExtender ID="PopupControlExtender1" runat="server"
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
<asp:DropDownList ID="dlEstatus" runat="server" CssClass="SelectFormat" DataValueField="estatusID" DataTextField="estatus" Enabled="false">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdEstatus" Value="" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Solicitado por:</td>
<td align="left">
<asp:DropDownList ID="dlSolicitado" runat="server" CssClass="SelectFormat"
   DataValueField="personaID" DataTextField="persona"></asp:DropDownList>
</td>
<td class="Cellformat1" align="right" style="width:150px;">Compras:</td>
<td align="left">
<asp:DropDownList ID="dlCompras" runat="server" CssClass="SelectFormat"
   DataValueField="personaID" DataTextField="persona"></asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Contabilidad:</td>
<td align="left">
<asp:DropDownList ID="dlContabilidad" runat="server" CssClass="SelectFormat"
   DataValueField="personaID" DataTextField="persona"></asp:DropDownList>
</td>
<td class="Cellformat1" align="right">Gerencia:</td>
<td align="left">
<asp:DropDownList ID="dlGerencia" runat="server" CssClass="SelectFormat"
   DataValueField="personaID" DataTextField="persona"></asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Fecha Creación:</td>
<td align="left" colspan="3">
<asp:Textbox id="txtFechaCreacion" runat="server" Width="100px" CssClass="datePicker"></asp:Textbox>
<asp:Label runat="server" ID="lblCorreoEnvio" class="Cellformat1"></asp:Label>
</td>
</tr>
<tr>
<td></td>
<td align="left" colspan="3">
<asp:Label runat="server" ID="lblCreado" class="CellInfo"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Lista de precios:</td>
<td align="left" style="width:150px;">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista" AutoPostBack="true" OnSelectedIndexChanged="dlListaPrecios_SelectedIndexChanged">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="right" >IVA:</td>
<td align="left" >
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1" AutoPostBack="True" OnSelectedIndexChanged="rdIVA_SelectedIndexChanged">
</asp:RadioButtonList>
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
<td class="Cellformat1" align="left" >Tipo:</td>
<td class="Cellformat1" align="left">
<asp:RadioButtonList runat="server" ID="rdTipo" RepeatDirection="Horizontal">
<asp:ListItem Selected="True" Text="Contado" Value="True"></asp:ListItem>
<asp:ListItem Text="Crédito" Value="False"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="right" >Descuento:</td>
<td align="left">
<asp:TextBox ID="txtDescuento" runat="server" MaxLength="6" CssClass="TextInputFormatD" Width="40px" AutoPostBack="True"
        ontextchanged="txtDescuento_TextChanged" ></asp:TextBox>%
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Núm Aduana:</td>
<td align="left">
<asp:TextBox ID="txtNum_Aduana" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="100px"></asp:TextBox>
</td>
<td class="Cellformat1" align="right" >Fecha Pedimento:</td>
<td align="left">
<asp:Textbox id="txtFechaPedimento" runat="server" Width="100px" CssClass="datePicker"></asp:Textbox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Factura:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtFactura" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="400px"></asp:TextBox>
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
<asp:Button ID="btnUsarOrden" runat="server" Text="Usar Orden Compra"
        CssClass="ButtonFormat" onclick="btnUsarOrden_Click" />
<asp:Button ID="btnFinalizar" runat="server" Text="Finalizar Captura"
        CssClass="ButtonFormat" onclick="btnFinalizar_Click" />
<asp:Button ID="btnRegistrarFactura" runat="server" Text="Registrar factura"
        CssClass="ButtonFormat" onclick="btnRegistrarFactura_Click" />
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:ImageButton ID="btnModificar" runat="server"
        ToolTip="Modificar" ImageUrl="~/imagenes/modificar.png" ValidationGroup="valDatos"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnImprimir" runat="server"
        ToolTip="Imprimir" ImageUrl="~/imagenes/reporte.png" ValidationGroup="valDatos"
        onclick="btnImprimir_Click" />
<asp:ImageButton ID="btnEmail" runat="server"
        ToolTip="Correo" ImageUrl="~/imagenes/email.png" ValidationGroup="valDatos"
        onclick="btnEmail_Click" />
<asp:ImageButton ID="btnCancelar" runat="server"
        ToolTip="Cancelar" ImageUrl="~/imagenes/cancelar.png"
        ValidationGroup="valDatos" OnClientClick="showModalCancelar();return false;" />
<asp:ImageButton ID="btnRegresar" runat="server" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/salir.png" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlDatos2" Visible="false" runat="server" DefaultButton="btnAgregarProd">
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr>
<td align="center" colspan="4" class="Titleformat1">
    Productos
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" colspan="4">
<table>
<tr>
<td class="GridFormatTD" align="left" style="width:60px;">Cantidad</td>
<td class="GridFormatTD" align="left" style="width:270px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:130px;">Cve Proveedor</td>
<td class="GridFormatTD" align="left" style="width:80px;">Precio (<asp:Label runat="server" ID="lblMoneda" />)</td>
<td class="GridFormatTD" align="left" style="width:70px;"><asp:Label ID="Label100" runat="server" Text="<%$ Resources: sian, lblLote %>" /></td>
<td class="GridFormatTD" align="left" style="width:290px;">Caducidad</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:60px;">
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="8" CssClass="TextInputFormatD" Width="60px" ></asp:TextBox>
</td>
<td class="Cellformat1" align="left" valign="top" style="width:270px;">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="265px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="acProducto"
                TargetControlID="txtProducto"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerProductosMateriaPrimaConceptos"
				UseContextKey="true"
                MinimumPrefixLength="1"
                CompletionInterval="1000"
                OnClientItemSelected="productoSeleccionado"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement"
                CompletionListItemCssClass="autocomplete_listItem"
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
</td>
<td align="left" valign="top" style="width:130px;">
    <asp:TextBox ID="txtCveProveedor" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="125px" ></asp:TextBox>
    <asp:HiddenField ID="hdCveProveedor" runat="server" />
</td>
<td align="left" valign="top" style="width:80px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="15" CssClass="TextInputFormatD" Width="75px" ></asp:TextBox>
</td>
<td align="left" valign="top" style="width:70px;">
<asp:TextBox ID="txtLote" runat="server" MaxLength="20" Width="65px" CssClass="TextInputFormat" ></asp:TextBox>
</td>
<td align="left" valign="top" style="width:290px;">
<asp:Textbox id="txtFechaCaducidad" runat="server" Width="85px" CssClass="datePicker"></asp:Textbox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
<asp:CheckBox ID="chkCambiarPrecios" runat="server" Text="Actualizar Precio" Checked="true" CssClass="GridFormatTD" />
<asp:HiddenField runat="server" ID="hdPrecioAnterior" />
<asp:HiddenField runat="server" ID="hdPorcentajeAumento" />
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
    Height="1px" SelectedIndex="0" CellPadding="0" Width="900"
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
        <asp:HiddenField runat="server" ID="hdReqLote" Value='<%# Eval("reqlote") %>' />
        <asp:HiddenField runat="server" ID="hdReqFecha" Value='<%# Eval("reqfecha") %>' />
        <asp:HiddenField runat="server" ID="hdInv" Value='<%# Eval("inv") %>' />
        </td>
    </tr></table>
        <asp:HiddenField runat="server" ID="hdCostoOriginal" Value='<%# DataBinder.Eval(Container.DataItem, "costo_original") %>' />
        <asp:HiddenField runat="server" ID="hdCostoOriginalMoneda" Value='<%# DataBinder.Eval(Container.DataItem, "costo_original_moneda") %>' />
    </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="producto" HeaderText="Producto">
        <HeaderStyle Width="245px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="codigo" HeaderText="Código">
        <HeaderStyle Width="95px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="clave" HeaderText="Clave<br/>Proveedor" HtmlEncode="false">
        <HeaderStyle Width="95px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="cantidad" HeaderText="Cantidad">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="costo_unitario" HeaderText="Precio Unitario">
        <HeaderStyle Width="80px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="costo" HeaderText="Subtotal">
        <HeaderStyle Width="80px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="lote" HeaderText="<%$ Resources: sian, lblLote %>">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Caducidad">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
</Columns>
</asp:GridView>
<asp:HiddenField ID="hdBorrar" runat="server" Value="0" />
<asp:HiddenField ID="hdCosto" runat="server" Value="0" />
<asp:HiddenField ID="hdCostoIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdCostoDescuento" runat="server" Value="0" />
<asp:HiddenField ID="hdIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdTotal" runat="server" Value="0" />
<asp:HiddenField ID="hdConsecutivoID" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMin" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMax" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMaxID" runat="server" Value="0" />
<asp:HiddenField runat="server" ID="hdModLote" Value="" />
<asp:HiddenField runat="server" ID="hdModCaducidad" Value="" />
</td>
</tr>
</table>
</asp:Panel>
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
<asp:Panel ID="pnlCambiar" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnCambiarContinuar">
<asp:Panel ID="pnlCambiarHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label14" runat="server" Text="Actualizar Producto" CssClass="msgErrorHeader" />
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
    <tr><td align="right" class="Cellformat1"><asp:Label ID="Label101" runat="server" Text="<%$ Resources: sian, lblLote %>" />:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarLote" runat="server" MaxLength="20" Width="60px" CssClass="TextInputFormat" ></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Caducidad:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox id="txtCambiarFecha" runat="server" Width="100px" CssClass="datePicker"></asp:TextBox>
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
    <asp:Button ID="btnCancelarContinuar" runat="server" Text="Continuar" onclick="btnCancelarContinuar_Click" CssClass="ButtonFormat"  />
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
<asp:Panel ID="pnlListasPrecios" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center">
<asp:Panel ID="pnlListasPreciosHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label92" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="Label3" runat="server" Text="Actualizar Listas de Ventas" CssClass="msgError1" /><br />
    <center>
    <table>
            <tr class="GridFormat">
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label112" runat="server">Lista de Precios</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label5" runat="server">Precio Actual</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label2" runat="server">Precio Nuevo</asp:Label>
                </td>
            </tr>
    </table>
    </center>
    <asp:GridView ID="gvListasPrecios" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="listapreciosID" Width="250"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false" OnRowDataBound="gvListasPrecios_RowDataBound"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="250">
                        <table>
                            <tr>
                                <td style="width:100px" align="left"><asp:Label ID="lblLista" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "nombrelista") %>'></asp:Label>
                                </td>
                                <td style="width:50px" align="right"><asp:Label ID="lblPrecio" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "precio") %>'></asp:Label>
                                </td>
                                <td style="width:100px" align="center"><asp:TextBox ID="txtPrecioNuevo" runat="server" CssClass="TextInputFormatD"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "precio_nuevo") %>' MaxLength="15" Width="70px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btnListasPreciosContinuar" runat="server" Text="Continuar" onclick="btnListasPreciosContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnListasPreciosCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyListasPrecios" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdListasPrecio" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdListasPreciosBehaviorID"
    TargetControlID="btnDummyListasPrecios"
    PopupControlID="pnlListasPrecios"
    CancelControlID="btnListasPreciosCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlAlmacen" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnAlmacenContinuar">
<asp:Panel ID="pnlAlmacenHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label19" runat="server" Text="Seleccionar almacén" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="center" class="Cellformat1">Seleccione el almacén:&nbsp;
    <asp:DropDownList ID="dlAlmacen" runat="server" CssClass="SelectFormat" DataValueField="subalmacenID" DataTextField="nombre_subalmacen"></asp:DropDownList>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnAlmacenContinuar" runat="server" Text="Continuar" onclick="btnAlmacenContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnAlmacenCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyAlmacen" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdAlmacen" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdAlmacenBehaviorID"
    TargetControlID="btnDummyAlmacen"
    PopupControlID="pnlAlmacen"
    CancelControlID="btnAlmacenCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlOrden_CompraListado" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" style="height: 18px;">
    Pedido</td>
</tr>
<tr style="height:10px">
<td></td>
</tr>
<tr>
    <td class="Cellformat1" align="center">Ingrese el número de la orden de compra:
        <asp:TextBox ID="txtOrden_CompraID" runat="server" MaxLength="6" CssClass="TextInputFormatD" Width="60px" ></asp:TextBox>
        <asp:Button ID="btnBuscarOrden_Compra" runat="server" Text="Continuar" CssClass="ButtonFormat" onclick="btnBuscarOrden_Compra_Click" />
    </td>
</tr>
<tr style="height:10px">
<td></td>
</tr>
<tr runat="server" id="trPedido">
    <td class="Cellformat1" align="center">Seleccione la orden de compra:&nbsp;</td>
</tr>
<tr>
    <td class="Cellformat1" align="center"><br />
        <asp:GridView ID="gvListadoOrden_Compra" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="orden_compraID" Width="480"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN" >
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="480">
                        <table>
                            <tr>
                                <td style="width:160px"><asp:RadioButton ID="chkSeleccion" runat="server" SkinID="chkXSmall" OnCheckedChanged="chkSeleccion_CheckedChanged" AutoPostBack="true" Text='<%# DataBinder.Eval(Container.DataItem, "orden_compra") %>' />
                                </td>
                                <td style="width:100px" align="left"><asp:Label ID="Label12" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "estatus") %>' />
                                </td>
                                <td style="width:100px" align="center"><asp:Label ID="lblMoneda" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "fecha") %>' />
                                </td>
                                <td style="width:120px" align="right"><asp:Label ID="Label27" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "monto") %>' />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </td>
</tr>
<tr>
    <td class="Cellformat1" align="center">
        <asp:Button ID="btnRegresarListaOrden_Compra" runat="server" Text="Regresar" CssClass="ButtonFormat" onclick="btnRegresarOrden_Compra_Click" />
    </td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlOrden_Compra" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr><td class="GridFormat" style="height: 18px;" colspan="2">
    Orden de Compra</td>
</tr>
<tr style="height:10px">
<td colspan="2"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px">Orden de compra:</td>
<td align="left" class="Cellformat1" style="width:750px">
<asp:Label ID="lblOrden_Compra" runat="server"></asp:Label>
</td>
</tr>
<tr>
<td align="center" colspan="2">
<asp:Label ID="lblMensajeOrden_Compra" runat="server" CssClass="msgLabel"></asp:Label><br />
<asp:HiddenField runat="server" ID="hdProveedorID2" />
<asp:HiddenField runat="server" ID="hdCajeroID" />
<asp:HiddenField runat="server" ID="hdLista_preciosID" />
<asp:HiddenField runat="server" ID="hdPorc_Iva" />
<asp:HiddenField runat="server" ID="hdDescuento" />
<asp:HiddenField runat="server" ID="hdContado" />
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td align="center" class="Titleformat1" colspan="4">
    Productos
</td>
</tr>
<tr>
<td align="center" colspan="4">
    <asp:Panel runat="server" ID="Panel2" Width="900">
        <table>
            <tr class="GridFormat">
                <td style="width:310px;color:white;font-size:XX-Small;"><asp:Label ID="lblProdList" runat="server">Producto</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label4" runat="server">Cantidad</asp:Label>
                </td>
                <td style="width:70px;color:white;font-size:XX-Small;"><asp:Label ID="Label115" runat="server">Cantidad<br/>Recibida</asp:Label>
                </td>
		        <td style="width:90px;color:white;font-size:XX-Small;"><asp:Label ID="Label16" runat="server">Precio</asp:Label>
                </td>
                <td style="width:70px;color:white;font-size:XX-Small;"><asp:Label ID="Label17" runat="server" Text="<%$ Resources: sian, lblLote %>"></asp:Label>
                </td>
                <td style="width:120px;color:white;font-size:XX-Small;"><asp:Label ID="Label18" runat="server">Caducidad</asp:Label>
                </td>
                <td style="width:190px;color:white;font-size:XX-Small;"><asp:Label ID="Label21" runat="server">Requisición</asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:GridView ID="gvProductosCompra" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="productoID" Width="900"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN"
        onrowdatabound="gvProductosCompra_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="900">
                        <table>
                            <tr>
                                <td style="width:310px"><asp:Label ID="lblProdDatos" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "producto") %>'></asp:Label>
                                </td>
                                <td style="width:50px" align="right"><asp:Label ID="lblCantDatos" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>'></asp:Label>
                                        <asp:HiddenField runat="server" ID="hdExento" Value='<%# DataBinder.Eval(Container.DataItem, "exento") %>' />
                                        <asp:HiddenField runat="server" ID="hdLote" Value='<%# DataBinder.Eval(Container.DataItem, "lote") %>' />
                                        <asp:HiddenField runat="server" ID="hdCaducidad" Value='<%# DataBinder.Eval(Container.DataItem, "caducidad") %>' />
                                        <asp:HiddenField runat="server" ID="hdMinimo" Value='<%# DataBinder.Eval(Container.DataItem, "minimo") %>' />
                                        <asp:HiddenField runat="server" ID="hdConsecutivo" Value='<%# DataBinder.Eval(Container.DataItem, "consecutivo") %>' />
                                </td>
                                <td style="width:70px" align="right"><asp:TextBox ID="txtCantidadDatos" runat="server" CssClass="TextInputFormatD"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' MaxLength="8" Width="60px"></asp:TextBox>
                                </td>
                                <td style="width:100px" align="right"><asp:TextBox ID="txtCostoDatos" runat="server" CssClass="TextInputFormatD" Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'
                                        Text='<%# DataBinder.Eval(Container.DataItem, "costo_unitario") %>' MaxLength="10" Width="60px"></asp:TextBox><asp:Label ID="lblMoneda" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "moneda") %>'></asp:Label>
                                </td>
                                <td style="width:70px" align="right"><asp:TextBox ID="txtLoteDatos" runat="server" CssClass="TextInputFormat"
                                        Text="" MaxLength="20" Width="60px"></asp:TextBox>
                                </td>
                                <td style="width:110px" align="right"><asp:Textbox id="txtFechaDatos" runat="server" Width="90px" CssClass="datePicker" Text=""></asp:Textbox>
                                </td>
                                <td style="width:190px"><asp:HiddenField ID="hdConsReqID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "consReqID") %>' />
                                    <asp:HiddenField ID="hdReqCantidad" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "reqCantidad") %>' />
                                    <asp:CheckBox ID="chkAplicarReq" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "requisicion") %>'
                                        Visible='<%# DataBinder.Eval(Container.DataItem, "reqVisible") %>' Checked="true" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Button ID="btnProcesar" runat="server" Text="Procesar"
        CssClass="ButtonFormat" onclick="btnProcesar_Click" />
    <asp:Button ID="btnRegresarOrden_Compra" runat="server" Text="Regresar"
        CssClass="ButtonFormat" onclick="btnRegresarOrden_Compra_Click" />
    </td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlVerificacion" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVerificacionContinuar">
<asp:Panel ID="pnlVerificacionHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label12" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblVerificacion" CssClass="Cellformat1">Está creando una remisión sin usar un pedido, ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="lblCodigoVerificacion" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigo_Verificacion" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnVerificacionContinuar" runat="server" Text="Continuar" onclick="btnVerificacionContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnVerificacionCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyVerificacion" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdVerificacion" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdVerificacionBehaviorID"
    TargetControlID="btnDummyVerificacion"
    PopupControlID="pnlVerificacion"
    CancelControlID="btnVerificacionCerrar"
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

    function proveedorSeleccionado(sender, e) {
        $get('<%=hdProveedorID.ClientID%>').value = e.get_value();
        PageMethods.Obtener_Notas(e.get_value(), colocarNotas);
    }

    function colocarNotas(rs) {
        // 0 - Notas
        // 1 - Descuento
        // 2 - IVA
        // 3 - Lista precios
        // 4 - Contado/Credito
        // 5 - Moneda
        var valores = rs.split("|");
        $get('<%=lblNotas.ClientID%>').innerHTML = valores[0];
        $get('<%=txtDescuento.ClientID%>').value = valores[1];
        var rdIVA = document.getElementsByName('<%=rdIVA.UniqueID%>');
        for (i = 0; i < rdIVA.length; i++) {
            if (rdIVA[i].value == valores[2])
                rdIVA[i].checked = true;
            else
                rdIVA[i].checked = false;
        }
        var dlLista = $get('<%=dlListaPrecios.ClientID%>');
        for (i = 0; i < dlLista.length; i++) {
            if (dlLista[i].value == valores[3])
                dlLista[i].selected = true;
            else
                dlLista[i].selected = false;
        }
        var rdTipo = document.getElementsByName('<%=rdTipo.UniqueID%>');
        if (valores[4] == "1") {
            rdTipo[0].checked = true;
            rdTipo[1].checked = false;
        }
        else {
            rdTipo[0].checked = false;
            rdTipo[1].checked = true;
        }
		if (valores[5] == "") {
            valores[5] = "MX";
            $get('<%=dlMoneda.ClientID%>').disabled = false;
        }
        else {
            $get('<%=dlMoneda.ClientID%>').disabled = true;
        }
        var dlMoneda = $get('<%=dlMoneda.ClientID%>');
        for (i = 0; i < dlMoneda.length; i++) {
            if (dlMoneda[i].value == valores[5])
                dlMoneda[i].selected = true;
            else
                dlMoneda[i].selected = false;
        }
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtPrecioUnitario.ClientID%>').value = PageMethods.ObtenerPrecio($get('<%=hdProveedorID.ClientID%>').value +
                                                                                '|' + e.get_value() +
                                                                                '|' + $get('<%=dlListaPrecios.ClientID%>').value +
                                                                                '|' + $get('<%=hdMoneda.ClientID%>').value,
                                                                                colocarPrecio);
        $get('<%=btnAgregarProd.ClientID%>').disabled = false;
    }

    function colocarPrecio(precio) {
        var valores = precio.split("|");
        $get('<%=txtPrecioUnitario.ClientID%>').value =
            $get('<%=hdPrecioUnitario.ClientID%>').value = valores[0];

        $get('<%=txtCveProveedor.ClientID%>').value =
            $get('<%=hdCveProveedor.ClientID%>').value = valores[1];

        $get('<%=txtLote.ClientID%>').value = '';
        $get('<%=txtFechaCaducidad.ClientID%>').value = '';

        if (valores[1] == '') {
            $get('<%=txtCveProveedor.ClientID%>').focus();
        }
        else {
            if (!$get('<%=txtPrecioUnitario.ClientID%>').disabled) {
                $get('<%=txtPrecioUnitario.ClientID%>').focus();
                $get('<%=txtPrecioUnitario.ClientID%>').select();
            }
            else
                $get('<%=txtLote.ClientID%>').focus();
        }

        $get('<%=hdMinimo.ClientID%>').value = valores[2];
    }

    function showModalCancelar(ev) {
        var modalPopupBehavior = $find('mdCancelarBehaviorID');
        modalPopupBehavior.show();
    }

    function setProductoFoco() {
        $get('<%=txtProducto.ClientID%>').focus();
    }
    function setProductoCantidad() {
        $get('<%=txtCambiarCantidad.ClientID%>').focus();
        $get('<%=txtCambiarCantidad.ClientID%>').select();
    }
    function setLote() {
        $get('<%=txtLote.ClientID%>').focus();
    }
    function setProductoPrecio() {
        $get('<%=txtPrecioUnitario.ClientID%>').focus();
        $get('<%=txtPrecioUnitario.ClientID%>').select();
    }
    function setPos() {
        $get('<%=txtPosicion.ClientID%>').focus();
    }
    function mostrarReporte(strURL) {
        window.open(strURL, "SIANRPT", "location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=900, height=500,left=40,top=50");
    }
    function mostrarMailPopUp(strURL) {
        window.open(strURL, "SIANRPT", "location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=630, height=500,left=40,top=50");
    }
    function esconder() {
        var ballonPop = $find('popNegocio');
        ballonPop.hidePopup();
    }

    function hideModalPago(ev) {
        var modalPopupBehavior = $find('mdPagoBehaviorID');
        modalPopupBehavior.hide();
    }

    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
</script>
</asp:Content>