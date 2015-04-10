<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="orden_compra.aspx.cs" Inherits="cotizacion_orden_compra" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdValidar" Value="" runat="server"/>
<asp:HiddenField runat="server" ID="hdInventarios" Value="0" />
<asp:HiddenField runat="server" ID="hdBorrar" Value="0" />
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
<asp:HiddenField runat="server" ID="hdAccion" />
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
<asp:Timer ID="tmrRefresh" runat="server" Interval="30000" ontick="tmrRefresh_Tick">
</asp:Timer>
<asp:UpdatePanel ID="updPanelListado" runat="server" >
<ContentTemplate>
<asp:Panel ID="pnlListado" runat="server" DefaultButton="btnBuscar">
    <asp:HiddenField runat="server" ID="hdAlmacen" Value="" />
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Pedidos - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Text="Folio" Value="0"></asp:ListItem>
                    <asp:ListItem Selected="True" Text="Cliente" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Fecha Creación" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Fecha Cancelación" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Estatus" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Orden Compra" Value="5"></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>
                &nbsp;<asp:ImageButton
                    ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
                    OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
            <td style="width: 108px;" align="left">
                <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos
                los Registros</asp:LinkButton></td>
            <td style="width: 109px;" align="left">
                <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat"
                    onclick="lblAgregar_Click">Agregar pedido</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="800px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="orden_compraID"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                    onrowcommand="grdvLista_RowCommand" onrowcreated="grdvLista_RowCreated"
                    onrowdatabound="grdvLista_RowDataBound">
            <Columns>
                <asp:ButtonField DataTextField="orden_compraID" CommandName="Modificar" HeaderText="Folio" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="negocio" HeaderText="Cliente" SortExpression="1" >
                    <HeaderStyle Width="180px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_creacion" HeaderText="Fecha Creación" SortExpression="2" >
                    <HeaderStyle Width="120px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_cancelacion" HeaderText="Fecha Cancelación" SortExpression="3" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="motivo_cancelacion" HeaderText="Motivo Cancelación" >
                    <HeaderStyle Width="175px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="estatus" HeaderText="Estatus" SortExpression="4" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="costo" HeaderText="Total" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:BoundField DataField="estatusID">
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
</ContentTemplate>
<Triggers>
<asp:AsyncPostBackTrigger ControlID="tmrRefresh" EventName="Tick" />
</Triggers>
</asp:UpdatePanel>
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Pedido</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Pedido:</td>
<td class="Cellformat1" align="left" style="width:650px;" colspan="3">
<b><asp:Label runat="server" ID="lblOrden_Compra" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td></td>
<td align="left" colspan="3">
<asp:Label runat="server" ID="lblCreado" class="CellInfo"></asp:Label>
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
&nbsp;<asp:Image runat="server" ID="imgInfo" ImageUrl="~/imagenes/info.jpg" Width="16" Height="16" />
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
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Orden Compra del Cliente:</td>
<td class="Cellformat1" align="left" colspan="3">
<asp:TextBox ID="txtOrdenCompra" runat="server" MaxLength="20" CssClass="TextInputFormat" Width="120px"></asp:TextBox>
&nbsp;Subtotal Orden Compra del Cliente:
<asp:TextBox ID="txtOrdenCompraTotal" runat="server" MaxLength="10" CssClass="TextInputFormat" Width="70px"></asp:TextBox>
&nbsp;&nbsp;Documento:
<asp:DropDownList ID="dlDocumento" runat="server" CssClass="SelectFormat">
<asp:ListItem Value="1" Selected="True">Factura</asp:ListItem>
<asp:ListItem Value="2">Remisión</asp:ListItem>
<asp:ListItem Value="3">Venta Mostrador</asp:ListItem>
<asp:ListItem Value="4">Venta Empleado</asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Requerimientos:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtRequerimientos" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Tiempo Entrega:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtTiempoEntrega" runat="server" MaxLength="75" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Lugar/Fecha Entrega:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtLugarEntrega" runat="server" MaxLength="150" CssClass="TextInputFormat" Width="300px"></asp:TextBox>
<asp:Textbox id="txtFechaEntrega" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFechaEntrega" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaEntrega" TargetControlID="txtFechaEntrega">
</cc1:CalendarExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Lista de precios:</td>
<td align="left" style="width:150px;">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="right" style="width:30px;">IVA:</td>
<td align="left" style="width:470px;">
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1" AutoPostBack="True" OnSelectedIndexChanged="rdIVA_SelectedIndexChanged">
</asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Estatus:</td>
<td align="left" style="width:650px;" colspan="3">
<asp:DropDownList ID="dlEstatus" runat="server" CssClass="SelectFormat" DataValueField="estatusID" DataTextField="estatus" Enabled="false">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdEstatus" Value="" />
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
<asp:Button ID="btnCotizacion" runat="server" Text="Usar Cotización"
        CssClass="ButtonFormat" onclick="btnCotizacion_Click" />
<asp:Button ID="btnAjustar" runat="server" Text="Ajustar Subtotal Cliente"
        CssClass="ButtonFormat" onclick="btnAjustar_Click" />
<asp:Button ID="btnLista" runat="server" Text="Usar Lista de Precios"
        CssClass="ButtonFormat" onclick="btnLista_Click" />
<asp:Button ID="btnCotejada" runat="server" Text="Poner como cotejada"
        CssClass="ButtonFormat" onclick="btnCotejada_Click" />
<asp:Button ID="btnGenerarFactura" runat="server" Text="Generar Factura"
        CssClass="ButtonFormat" OnClientClick="showModalVendedor('1');return false;" />
<asp:Button ID="btnGenerarNota" runat="server" Text="Generar Nota Remisión"
        CssClass="ButtonFormat" OnClientClick="showModalVendedor('2');return false;" />

</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1"
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnImprimir" runat="server" CssClass="ReporteFormat1"
        ToolTip="Imprimir" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnImprimir_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="CancelFormat1"
        ToolTip="Cancelar" ImageUrl="~/imagenes/dummy.ico"
        ValidationGroup="valDatos" OnClientClick="showModalCancelar();return false;" />
<asp:ImageButton ID="btnRegresar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
<asp:Panel ID="pnlLimiteCR" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnLimiteCRContinuar">
<asp:Panel ID="pnlLimiteCRHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label19" runat="server" Text="Código de verificación" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblLimiteCR" CssClass="Cellformat1"></asp:Label>
    <br /><br />
    <asp:Label runat="server" ID="Label20" CssClass="Cellformat1">Ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="Label22" CssClass="Cellformat1">Código Verificación:</asp:Label>
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
<td class="GridFormatTD" align="left" style="width:445px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:395px;">Precio</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:60px;">
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
<td class="Cellformat1" align="left" valign="top" style="width:445px;">
<asp:TextBox ID="txtProducto" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="440px" autocomplete="off"></asp:TextBox>
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
</td>
<td align="left" valign="top" style="width:395px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
<asp:CheckBox ID="chkCambiarPrecios" runat="server" Text="Actualizar precios" Checked="true" CssClass="GridFormatTD" />
<asp:RadioButtonList ID="rdNombre" CssClass="GridFormatTD" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" BorderStyle="Solid" BorderColor="Gainsboro">
<asp:ListItem Value="0" Text="Nombre" Selected="True"></asp:ListItem>
<asp:ListItem Value="1" Text="Sal"></asp:ListItem>
<asp:ListItem Value="2" Text="Cve Gob"></asp:ListItem>
</asp:RadioButtonList>
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
    <tr><td align="right" class="Cellformat1">
        <asp:Label ID="Label11" runat="server" Text="Precio Unitario:" /></td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtPrecioUnitarioCambio" runat="server" MaxLength="15" Width="60px" CssClass="TextInputFormat" ></asp:TextBox>
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
<asp:Panel ID="pnlVendedores" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVendedorContinuar">
<asp:Panel ID="pnlVendedoresHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label16" runat="server" Text="Seleccione Vendedor" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Vendedor:</td>
    <td align="left" class="Cellformat1">
        <asp:DropDownList ID="dlVendedor" runat="server" CssClass="SelectFormat"
        DataValueField="vendedorID" DataTextField="vendedor"></asp:DropDownList>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Productos:</td>
    <td align="left" class="Cellformat1">
        <asp:RadioButtonList ID="rdProductos" runat="server" CssClass="Cellformat1" RepeatDirection="Horizontal">
        <asp:ListItem Value="1" Selected="True" Text="Todos" />
        <asp:ListItem Value="2" Text="Sólo en existencia" />
        </asp:RadioButtonList>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnVendedorContinuar" runat="server" Text="Continuar" onclick="btnVendedorContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnVendedorCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyVendedor" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdCambiarVendedor" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdVendedorBehaviorID"
    TargetControlID="btnDummyVendedor"
    PopupControlID="pnlVendedores"
    CancelControlID="btnVendedorCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlNotas" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnNotaContinuar">
<asp:Panel ID="pnlNotasHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label12" runat="server" Text="Notas de Remisión" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblNotasProducto" CssClass="Cellformat1"></asp:Label>
    <br />
    <asp:GridView ID="gvNotas" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" Width="200"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="true"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN">
    <Columns>
        <asp:BoundField DataField="nota" HeaderText="Nota">
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="fecha" HeaderText="Fecha">
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
    </Columns>
    </asp:GridView>
    <br />
    <asp:Label runat="server" ID="lblCodigoVerificacion" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigo_Verificacion" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnNotaContinuar" runat="server" Text="Agregar" onclick="btnNotaContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnNotaCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyNota" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdNotasRemision" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdNotaBehaviorID"
    TargetControlID="btnDummyNota"
    PopupControlID="pnlNotas"
    CancelControlID="btnNotaCerrar"
    DropShadow="False" />
<asp:HiddenField runat="server" ID="hdPrecioUnitario" />
<asp:HiddenField runat="server" ID="hdProductoPrecioID" />
</td>
</tr>
</table>
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 950px;">
<tr>
<td align="center" colspan="4">
<asp:GridView ID="gvProductos" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" CellPadding="0" Width="950"
    AutoGenerateColumns="false" DataKeyNames="productoID"
    HorizontalAlign="Center" EnableViewState="True" ShowFooter="true"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvProductos_RowCommand"
        onrowdatabound="gvProductos_RowDataBound">
    <FooterStyle HorizontalAlign="Right"></FooterStyle>
<Columns>
    <asp:ButtonField DataTextField="id" CommandName="Modificar" >
        <HeaderStyle Width="40px" />
        <ItemStyle HorizontalAlign="Right" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:TemplateField HeaderStyle-Width="20px">
    <ItemTemplate>
    <table><tr>
        <td><asp:ImageButton runat="server" ID="btnUP" ImageUrl="~/imagenes/up.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnUP_Click" /></td>
        <td><asp:ImageButton runat="server" ID="btnDN" ImageUrl="~/imagenes/dn.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnDN_Click" /></td>
        <td><asp:ImageButton runat="server" ID="btnPos" ImageUrl="~/imagenes/updn.png" CommandName="mv" CommandArgument='<%# Eval("con") %>' OnClick="btnMV_Click" />
        <asp:HiddenField runat="server" ID="hdSal" Value='<%# Eval("sal") %>' />
        </td>
    </tr></table>
    </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="producto" HeaderText="Producto">
        <HeaderStyle Width="280px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="ubicacion" HeaderText="Ubicación">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="codigo" HeaderText="Código Barras">
        <HeaderStyle Width="90px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Últ.Venta">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="cantidad" HeaderText="Cantidad">
        <HeaderStyle Width="60px" />
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
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:BoundField DataField="validado" HeaderText="Validación">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="faltante" HeaderText="Faltante">
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
</Columns>
</asp:GridView>
<asp:HiddenField ID="hdCosto" runat="server" Value="0" />
<asp:HiddenField ID="hdCostoIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdIVA" runat="server" Value="0" />
<asp:HiddenField ID="hdTotal" runat="server" Value="0" />
<asp:HiddenField ID="hdConsecutivoID" runat="server" Value="0" />
<asp:HiddenField ID="hdSurtido" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMin" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMax" runat="server" Value="0" />
<asp:HiddenField ID="hdConsMaxID" runat="server" Value="0" />
<asp:Panel ID="pnlMV" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnMVContinuar">
<asp:Panel ID="pnlMVHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label30" runat="server" Text="Mover Producto" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
<center>
        <asp:Label ID="lblProdPos" class="Cellformat1" runat="server" />
    <br />
        <asp:Label ID="Label31" class="Cellformat1" runat="server" Text="Nueva posición: " />
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
        <asp:TextBox ID="txtCambiarCantidad" runat="server" MaxLength="5" Width="40px" CssClass="TextInputFormat" ></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">
        <asp:Label ID="Label15" runat="server" Text="Precio Unitario:" /></td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarUnitario" runat="server" MaxLength="15" Width="60px" CssClass="TextInputFormat" ></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1" valign="top">&nbsp;</td>
     <td align="left" class="Cellformat1"><asp:RadioButtonList ID="rdNombreCambiar" CssClass="GridFormatTD" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
<asp:ListItem Value="0" Text="Nombre" Selected="True"></asp:ListItem>
<asp:ListItem Value="1" Text="Sal"></asp:ListItem>
<asp:ListItem Value="2" Text="Cve Gob"></asp:ListItem>
</asp:RadioButtonList></td>
     </tr>
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
    <asp:Label ID="Label29" runat="server" Text="Los productos ya surtidos serán devueltos al inventario" CssClass="msgError1" /><br />
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
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlAlmacen" Visible="false" runat="server" DefaultButton="btnGuardarAlmacen">
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Pedido</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Pedido:</td>
<td class="Cellformat1" align="left" style="width:650px;" colspan="2">
<b><asp:Label runat="server" ID="lblOrden_CompraAlmacen" class="Cellformat1"></asp:Label></b>
</td>
<td class="Cellformat1" align="left" style="width:100px;"><asp:LinkButton ID="lnkValidar" runat="server" CssClass="LinkFormat" onclick="lnkValidar_Click">Código validación</asp:LinkButton></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Cliente:</td>
<td align="left" style="width:750px;" colspan="3" class="Cellformat1">
<asp:Label runat="server" ID="lblNegocio"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Comentarios:</td>
<td align="left" style="width:750px;" colspan="3" class="Cellformat1">
<asp:TextBox ID="txtComentariosAlmacen" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Requerimientos:</td>
<td align="left" style="width:750px;" colspan="3" class="Cellformat1">
<asp:TextBox ID="txtRequerimientosAlmacen" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px" Enabled="false"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Tiempo Entrega:</td>
<td align="left" style="width:750px;" colspan="3" class="Cellformat1">
<asp:TextBox ID="txtTiempoEntregaAlmacen" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px" Enabled="false"></asp:TextBox>
</td>
</tr>
<tr>
<td align="left" colspan="4">
    <asp:Panel runat="server" ID="Panel2" Width="900">
        <table style="border-collapse:collapse; table-layout:fixed; width:900px;">
            <tr class="GridFormat">
                <td style="width:350px;color:white;font-size:XX-Small;"><asp:Label ID="Label4" runat="server">Producto</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label27" runat="server">Ubicación</asp:Label>
                </td>
                <td style="width:130px;color:white;font-size:XX-Small;"><asp:Label ID="Label5" runat="server">Código</asp:Label>
                </td>
                <td style="width:150px;color:white;font-size:XX-Small;"><asp:Label ID="Label21" runat="server">Lote</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label17" runat="server">Cantidad</asp:Label>
                </td>
                <td style="width:70px;color:white;font-size:XX-Small;"><asp:Label ID="Label8" runat="server">En Existencia</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label9" runat="server">Faltante</asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:GridView ID="gvProductosAlmacen" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="productoID" Width="900"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
        HorizontalAlign="Left" SelectedIndex="0" SkinID="grdSIAN"
        onrowdatabound="gvProductosAlmacen_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="900">
                        <table style="border-collapse:collapse; table-layout:fixed; width:900px;">
                            <tr>
                                <td style="width:350px;overflow:hidden; max-width:330px;" align="left" valign="top"><asp:Label ID="lblProducto" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "producto") %>'></asp:Label>
                                </td>
                                <td style="width:100px;overflow:hidden; max-width:100px;" align="left" valign="top"><asp:Label ID="Label28" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "ubicacion") %>'></asp:Label>
                                </td>
                                <td style="width:130px" align="left" valign="top"><asp:Label ID="lblCodigo" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "codigo") %>'></asp:Label><br />
                                        <asp:TextBox ID="txtCodigoProd" runat="server" CssClass="TextInputFormat" AutoPostBack="true"
                                                   Text='<%# DataBinder.Eval(Container.DataItem, "codigotemp") %>' MaxLength="50" Width="130px" OnTextChanged="txtCodigoProd_TextChanged" ></asp:TextBox>
                                        <asp:HiddenField runat="server" ID="hdCodigos" Value='<%# DataBinder.Eval(Container.DataItem, "codigos") %>' />
                                        <asp:HiddenField runat="server" ID="hdValidado" Value="0" />
                                        <asp:HiddenField runat="server" ID="hdTemporal" Value='<%# DataBinder.Eval(Container.DataItem, "temporal") %>' />
                                </td>
                                <td style="width:150px" valign="top"><asp:ImageButton ID="btnRecargar" runat="server" CssClass="ButtonFormat" Height="12px" Width="12px" ImageUrl="~/imagenes/recargar.gif" OnCommand="btnRecargar_Click" ToolTip="Refrescar" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "btnID") %>' />
                                <asp:Label ID="lblMensaje" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "mensaje") %>'></asp:Label><br />
                                <asp:GridView ID="gvProductosAlmacenLote" runat="server" AutoGenerateColumns="false"
                                    CaptionAlign="Top" CellPadding="0" DataKeyNames="loteID" Width="200"
                                    EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
                                    HorizontalAlign="Left" SelectedIndex="0">
                                    <Columns>
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtCantidadLote" runat="server" CssClass="TextInputFormat"
                                                   Text='<%# DataBinder.Eval(Container.DataItem, "canttemp") %>' MaxLength="5" Width="50px"></asp:TextBox>
                                        <asp:Label ID="lblLote" runat="server" CssClass="Cellformat1"
                                                   Text='<%# DataBinder.Eval(Container.DataItem, "lote") %>'></asp:Label>
                                        <asp:HiddenField runat="server" ID="hdCantidadLote" Value='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                </td>
                                <td style="width:50px" align="right" valign="top"><asp:Label ID="lblCantidad" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>'></asp:Label>
                                        <asp:HiddenField runat="server" ID="hdCantidad" Value='<%# DataBinder.Eval(Container.DataItem, "cantidadasurtir") %>' />
                                </td>
                                <td style="width:70px" align="center" valign="top"><asp:CheckBox ID="chkExistencia" runat="server" SkinID="chkXSmall"
                                        Checked = '<%# DataBinder.Eval(Container.DataItem, "validado") %>' />
                                </td>
                                <td style="width:50px" align="center" valign="top"><asp:TextBox ID="txtFaltante" runat="server" CssClass="TextInputFormat"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "faltante") %>' MaxLength="5" Width="50px"></asp:TextBox>
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
<td align="center" colspan="4">
<asp:Button ID="btnPonerProceso" runat="server" Text="Regresar a Revisión"
        CssClass="ButtonFormat" onclick="btnPonerProceso_Click" />
<asp:Button ID="btnTemporal" runat="server" Text="Guardar captura temporal"
        CssClass="ButtonFormat" onclick="btnTemporal_Click" />
<br />
<asp:ImageButton ID="btnGuardarAlmacen" runat="server" CssClass="ModifyFormat1"
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnGuardarAlmacen_Click" />
<asp:ImageButton ID="btnImprimirAlmacen" runat="server" CssClass="ReporteFormat1"
        ToolTip="Imprimir" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnImprimir_Click" />
<asp:ImageButton ID="btnRegresarAlmacen" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlVerificacion" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVerificacionContinuar">
<asp:Panel ID="pnlVerificacionHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="lblVerificacionH" runat="server" Text="Verificación de Lotes" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblVerificacion" CssClass="Cellformat1">Ingrese el código de verificación para permitir el uso de lotes más recientes</asp:Label>
    <br />
    <asp:Label runat="server" ID="Label194" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtLote_Verificacion" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnVerificacionContinuar" runat="server" Text="Continuar" onclick="btnVerificacionContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnVerificacionCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyVerificacion" Text="" style="visibility:hidden" />
<asp:HiddenField runat="server" ID="hdAccionVerif" />
<cc1:ModalPopupExtender ID="mdVerificacion" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdVerificacionBehaviorID"
    TargetControlID="btnDummyVerificacion"
    PopupControlID="pnlVerificacion"
    CancelControlID="btnVerificacionCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlInventario" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnInventarioContinuar">
<asp:Panel ID="pnlInventarioHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label18" runat="server" Text="Inventario" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="center" class="Cellformat1">
    <asp:Label runat="server" ID="lblInventario"></asp:Label>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnInventarioContinuar" runat="server" Text="Continuar" onclick="btnInventarioContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnInventarioCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyInventario" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdInventario" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdInventarioBehaviorID"
    TargetControlID="btnDummyInventario"
    PopupControlID="pnlInventario"
    CancelControlID="btnInventarioCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlCotizacion" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" style="height: 18px;">
    Cotización</td>
</tr>
<tr style="height:10px">
<td></td>
</tr>
<tr>
<td class="Cellformat1" align="center">Folio de la Cotización:&nbsp;
<asp:TextBox ID="txtCotizacionID" runat="server" MaxLength="10" CssClass="TextInputFormat" Width="50px"></asp:TextBox>
</td>
</tr>
<tr>
<td align="center">
<asp:Label ID="lblMensajeCotizacion" runat="server" CssClass="msgLabel"></asp:Label><br />
<asp:Button ID="btnObtenerCotizacion" runat="server" Text="Obtener Datos Cotización"
        CssClass="ButtonFormat" onclick="btnObtenerCotizacion_Click" />
<asp:Button ID="btnRegresarCotizacion" runat="server" Text="Regresar"
        CssClass="ButtonFormat" onclick="btnRegresarCotizacion_Click" />
</td>
</tr>
</table>
<asp:Panel runat="server" ID="pnlCotizacionDatos" Visible="false" >
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Cliente:</td>
<td align="left" style="width:650px;" colspan="3">
<asp:TextBox ID="txtSucursalCotizacion" runat="server" MaxLength="200" CssClass="TextInputFormat" Width="500px" Enabled="false"></asp:TextBox>
<asp:HiddenField runat="server" ID="hdSucursalCotizacionID" Value="" />
<asp:Image runat="server" ID="imgInfo2" ImageUrl="~/imagenes/info.jpg" Width="16" Height="16" />
<asp:Panel runat="server" ID="pnlInfo2" CssClass="popNotas">
    <asp:Label ID="lblNotas2" runat="server" Text="" />
</asp:Panel>
<cc1:BalloonPopupExtender ID="BalloonPopupExtender1" runat="server"
    TargetControlID="imgInfo2" BehaviorID="popNegocio2"
    BalloonPopupControlID="pnlInfo2"
    Position="BottomLeft"
    BalloonStyle="Custom"
    CustomClassName="custom"
    CustomCssUrl="../css/popnotas.css"
    UseShadow="false"
    ScrollBars="None"
    DisplayOnMouseOver="true"
    DisplayOnFocus="false"
    DisplayOnClick="false" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Requerimientos:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtRequerimientosCotizacion" runat="server" MaxLength="500" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Tiempo Entrega:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtTiempoEntregaCotizacion" runat="server" MaxLength="75" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Lugar/Fecha Entrega:</td>
<td align="left" colspan="3">
<asp:TextBox ID="txtLugarEntregaCotizacion" runat="server" MaxLength="150" CssClass="TextInputFormat" Width="650px"></asp:TextBox>
<asp:HiddenField runat="server" ID="hdListaPreciosCotizacion" />
<asp:HiddenField runat="server" ID="hdIVACotizacion" />
</td>
</tr>
<tr>
<td align="center" class="Titleformat1" colspan="4">
    Productos
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" colspan="4">
<asp:Button ID="btnSeleccionar" runat="server" Text="Seleccionar/Deseleccionar todo" CssClass="ButtonFormat"  />
<asp:HiddenField runat="server" ID="hdSeleccionar" />
</td>
</tr>
<tr>
<td align="center" colspan="4">
    <asp:Panel runat="server" ID="Panel1" Width="800">
        <table>
            <tr class="GridFormat">
                <td style="width:500px;color:white;font-size:XX-Small;"><asp:Label ID="Label2" runat="server">Producto</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label3" runat="server">Cantidad</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label6" runat="server">Precio Unitario</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label7" runat="server">Subtotal</asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:GridView ID="gvProductosCotizacion" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="productoID" Width="800"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="800">
                        <table>
                            <tr>
                                <td style="width:500px"><asp:CheckBox ID="chkProducto" runat="server" SkinID="chkXSmall" Text='<%# DataBinder.Eval(Container.DataItem, "producto") %>' Checked="true" />
                                <asp:HiddenField runat="server" ID="hdValidado" Value="0" />
                                <asp:HiddenField runat="server" ID="hdSal" Value='<%# DataBinder.Eval(Container.DataItem, "sal") %>' />
                                </td>
                                <td style="width:100px"><asp:TextBox ID="txtCantidadCotizacion" runat="server" CssClass="TextInputFormat"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' MaxLength="10" Width="50px"></asp:TextBox>
                                </td>
                                <td style="width:100px"><asp:TextBox ID="txtCostoUnitarioCotizacion" runat="server" CssClass="TextInputFormat" Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'
                                        Text='<%# DataBinder.Eval(Container.DataItem, "costo_unitario") %>' MaxLength="10" Width="75px"></asp:TextBox>
                                </td>
                                <td style="width:100px" align="right"><asp:Label ID="lblSubTotal" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "costo") %>' MaxLength="10" Width="50px"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:HiddenField runat="server" ID="hdIndex" />
    <asp:Button ID="btnProcesar" runat="server" Text="Procesar"
        CssClass="ButtonFormat" onclick="btnProcesar_Click"  />
   <asp:Panel ID="pnlNotaOrden" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnNotaOrdenContinuar">
    <asp:Panel ID="pnlNotaOrdenHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
        <asp:Label ID="Label13" runat="server" Text="Notas de Remisión" CssClass="msgErrorHeader" />
    </asp:Panel>
    <div>
        <br />
        <asp:Label runat="server" ID="lblNotaOrdenProducto" CssClass="Cellformat1"></asp:Label>
        <br />
        <asp:GridView ID="gvNotaOrden" runat="server" AutoGenerateColumns="false"
            CaptionAlign="Top" CellPadding="0" Width="200"
            EnableViewState="True" GridLines="None" Height="1px" ShowHeader="true"
            HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN">
        <Columns>
            <asp:BoundField DataField="nota" HeaderText="Nota">
                <HeaderStyle Width="100px" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="fecha" HeaderText="Fecha">
                <HeaderStyle Width="100px" />
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>
        </Columns>
        </asp:GridView>
        <br />
        <asp:Label runat="server" ID="lblCodigoVerificacionOrden" CssClass="Cellformat1">Código Verificación:</asp:Label>
        <asp:TextBox ID="txtCodigo_VerificacionOrden" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="btnNotaOrdenContinuar" runat="server" Text="Agregar" onclick="btnNotaOrdenContinuar_Click" CssClass="ButtonFormat"  />
        <asp:Button ID="btnNotaOrdenCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
        <br />
    </div>
    </asp:Panel>
    <asp:Button runat="server" ID="btnDummyNotaOrden" Text="" style="visibility:hidden" />
    <cc1:ModalPopupExtender ID="mdNotaOrdenRemision" runat="server"
        BackgroundCssClass="modalBackground" BehaviorID="mdNotaOrdenBehaviorID"
        TargetControlID="btnDummyNotaOrden"
        PopupControlID="pnlNotaOrden"
        CancelControlID="btnNotaOrdenCerrar"
        DropShadow="False" />
</td>
</tr>
</table>
</asp:Panel>
</asp:Panel>
<asp:Panel ID="pnlListaPrecios" Visible="false" runat="server" DefaultButton="btnAgrProdLista">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" style="height: 18px;">
    Lista de Precios</td>
</tr>
<tr style="height:10px">
<td></td>
</tr>
<tr>
<td class="Cellformat1" align="center">Lista de Precios
<asp:Label runat="server" ID="lblLista" class="Cellformat1"></asp:Label>
</td>
</tr>
<tr>
<td align="center">
<asp:TextBox ID="txtProdLista" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="440px" autocomplete="off"></asp:TextBox>
<cc1:AutoCompleteExtender runat="server" ID="AutoCompleteExtender2"
                TargetControlID="txtProdLista"
                ServicePath="~/Services/ComboServices.asmx"
                ServiceMethod="ObtenerProductos"
                MinimumPrefixLength="1"
                CompletionInterval="1000"
                OnClientItemSelected="productoSelLista"
                CompletionSetCount="50"
                CompletionListCssClass="autocomplete_completionListElement"
                CompletionListItemCssClass="autocomplete_listItem"
                CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" />
<asp:TextBox ID="txtCantLista" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
<asp:TextBox ID="txtPrecioLista" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgrProdLista" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgrProdLista_Click" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlListaPrecios2" Visible="false" runat="server" DefaultButton="btnProcesarLista">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr>
<td align="center" class="Titleformat1">
    Productos
</td>
</tr>
<tr>
<td align="center">
<asp:Panel runat="server" ID="Panel3" Width="900">
<table>
<tr class="GridFormat">
<td style="width:460px;color:white;font-size:XX-Small;"><asp:Label ID="Label23" runat="server">Producto</asp:Label></td>
<td style="width:165px;color:white;font-size:XX-Small;"><asp:Label ID="Label26" runat="server">Código</asp:Label></td>
<td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label32" runat="server">Última Venta</asp:Label></td>
<td style="width:55px;color:white;font-size:XX-Small;"><asp:Label ID="Label24" runat="server">Cantidad</asp:Label></td>
<td style="width:120px;color:white;font-size:XX-Small;"><asp:Label ID="Label25" runat="server">Precio Unitario (Actualizar)</asp:Label></td>
</tr>
</table>
</asp:Panel>
<asp:GridView ID="gvLista" runat="server" AutoGenerateColumns="false"
CaptionAlign="Top" CellPadding="0" DataKeyNames="productoID" Width="900"
EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN">
<Columns>
<asp:TemplateField>
<ItemTemplate>
<asp:Panel runat="server" ID="pnlDatos" Width="900">
<table>
<tr>
<td style="width:460px"><asp:Label ID="lblProdList" runat="server" CssClass="Cellformat1"
        Text='<%# DataBinder.Eval(Container.DataItem, "producto") %>'></asp:Label>
</td>
<td style="width:165px"><asp:Label ID="lblCodigo" runat="server" CssClass="Cellformat1"
        Text='<%# DataBinder.Eval(Container.DataItem, "codigo") %>'></asp:Label>
</td>
<td style="width:100px" align="center"><asp:Label ID="Label16" runat="server" CssClass="Cellformat1"
        Text='<%# DataBinder.Eval(Container.DataItem, "fecha") %>'></asp:Label>
</td>
<td style="width:55px"><asp:TextBox ID="txtCantidadLista" runat="server" CssClass="TextInputFormat"
        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' MaxLength="10" Width="50px"></asp:TextBox>
</td>
<td style="width:120px"><asp:TextBox ID="txtCostoLista" runat="server" CssClass="TextInputFormat" Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'
        Text='<%# DataBinder.Eval(Container.DataItem, "costo_unitario") %>' MaxLength="10" Width="75px"></asp:TextBox>
    <asp:CheckBox runat="server" ID="chkAct" SkinID="chkXSmall" Checked='<%# DataBinder.Eval(Container.DataItem, "productoChk")%>' Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'/>
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
<td align="center">
<asp:Panel runat="server" ID="pnlListaButones" BorderWidth="1" BorderStyle="solid" BorderColor="DarkBlue">
    <asp:Button ID="btnProcesarLista" runat="server" Text="Procesar"
        CssClass="ButtonFormat" onclick="btnProcesarLista_Click"  />
    <asp:Button ID="btnRegresarLista" runat="server" Text="Regresar"
        CssClass="ButtonFormat" onclick="btnRegresarLista_Click"  />
</asp:Panel>
<cc1:AlwaysVisibleControlExtender ID="avcLista" runat="server"
    TargetControlID="pnlListaButones"
    VerticalSide="Top"
    VerticalOffset="200"
    HorizontalSide="Right"
    HorizontalOffset="30"
    ScrollEffectDuration=".1" />
</td>
</tr>
</table>
</asp:Panel>
<script type="text/javascript">
    function isNumber(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }

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
        PageMethods.Obtener_Notas(e.get_value(), colocarNotas);
    }

    function colocarNotas(rs) {
        var valores = rs.split("|");
        $get('<%=lblNotas.ClientID%>').innerHTML = valores[0];
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
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtPrecioUnitario.ClientID%>').value = PageMethods.ObtenerPrecio(e.get_value() + '|' + $get('<%=dlListaPrecios.ClientID%>').value, colocarPrecio);
        $get('<%=btnAgregarProd.ClientID%>').disabled = false;
    }

    function productoSelLista(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtCantLista.ClientID%>').value = '';
        $get('<%=txtPrecioLista.ClientID%>').value = '';
        setTimeout('setProductoCantLista()', 50);
    }

    function colocarPrecio(precio) {
        $get('<%=txtPrecioUnitario.ClientID%>').value = precio;
        $get('<%=hdPrecioUnitario.ClientID%>').value = precio;
        if (!$get('<%=txtPrecioUnitario.ClientID%>').disabled) {
            $get('<%=txtPrecioUnitario.ClientID%>').focus();
            $get('<%=txtPrecioUnitario.ClientID%>').select();
        }
        else
            $get('<%=btnAgregarProd.ClientID%>').focus();
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
    function setAgrProd() {
        $get('<%=btnAgregarProd.ClientID%>').focus();
    }
    function setProductoCantLista() {
        $get('<%=txtCantLista.ClientID%>').focus();
        $get('<%=txtCantLista.ClientID%>').select();
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
    function esconder() {
        var ballonPop = $find('popNegocio');
        ballonPop.hidePopup();
    }
    function esconder2() {
        var ballonPop = $find('popNegocio2');
        ballonPop.hidePopup();
    }

    function showModalVendedor(accion) {
        $get('<%=hdAccion.ClientID%>').value = accion;
        var modalPopupBehavior = $find('mdVendedorBehaviorID');
        modalPopupBehavior.show();
    }

    function seleccionar_todo() {
        if ($get('<%=hdSeleccionar.ClientID%>').value == "1") {
            seleccionar(false);
            $get('<%=hdSeleccionar.ClientID%>').value = "0";
        }
        else {
            seleccionar(true);
            $get('<%=hdSeleccionar.ClientID%>').value = "1";
        }
    }

    function seleccionar(valor) {
        var gv = $get('<%=gvProductosCotizacion.ClientID%>');
        var inputList = gv.getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i].type == "checkbox")
                inputList[i].checked = valor;
        }
    }
    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
    function limpiarProdListaID() {
        $get('<%=txtProdLista.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
</script>
</asp:Content>