<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="compra_orden.aspx.cs" Inherits="compras_compra_orden" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:HiddenField ID="hdCobraPaq" Value="" runat="server"/>
<asp:HiddenField ID="hdClavePaq" Value="" runat="server"/>
<asp:HiddenField ID="hdAccion" Value="" runat="server"/>
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
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Compras - Órdenes de Compra - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Text="Folio" Value="0"></asp:ListItem>
                    <asp:ListItem Selected="True" Text="Proveedor" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Fecha Creación" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Fecha Cancelación" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Fecha a Surtirse" Value="5"></asp:ListItem>
                    <asp:ListItem Text="Estatus" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Sin Fecha de Envío" Value="6"></asp:ListItem>
                    <asp:ListItem Text="Sin Número de Envío" Value="7"></asp:ListItem>
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
                    onclick="lblAgregar_Click">Agregar Orden de Compra</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="800px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="compra_ordenID"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="compra_ordenID" CommandName="Modificar" HeaderText="Folio" SortExpression="0" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="proveedor" HeaderText="Proveedor" SortExpression="1" >
                    <HeaderStyle Width="235px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_creacion" HeaderText="Fecha Creación" SortExpression="2" >
                    <HeaderStyle Width="120px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_surtirse" HeaderText="Fecha A Surtirse" SortExpression="5" >
                    <HeaderStyle Width="100px" />
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
                <asp:BoundField DataField="costo" HeaderText="Total" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Right" />
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
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Orden de compra</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Orden de compra:</td>
<td class="Cellformat1" align="left" style="width:650px;" colspan="3">
<b><asp:Label runat="server" ID="lblOrden_Compra" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Proveedor:</td>
<td align="left" style="width:650px;" colspan="3">
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
<td class="Cellformat1" align="left">Cajero:</td>
<td align="left">
<asp:DropDownList ID="dlCajero" runat="server" CssClass="SelectFormat"
   DataValueField="cajeroID" DataTextField="cajero"></asp:DropDownList>
</td>
<td class="Cellformat1" align="right" style="width:150px;">Estatus:</td>
<td align="left">
<asp:DropDownList ID="dlEstatus" runat="server" CssClass="SelectFormat" DataValueField="estatusID" DataTextField="estatus" Enabled="false">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdEstatus" Value="" />
<asp:Label runat="server" ID="lblCorreoEnvio" class="Cellformat1"></asp:Label>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Fecha Creación:</td>
<td align="left">
<asp:Textbox id="txtFechaCreacion" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFechaCreacion" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" Enabled="false" />
<cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaCreacion" TargetControlID="txtFechaCreacion">
</cc1:CalendarExtender>
</td>
<td class="Cellformat1" align="right">Fecha A Surtirse:</td>
<td align="left">
<asp:Textbox id="txtFechaSurtirse" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFechaSurtirse" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaSurtirse" TargetControlID="txtFechaSurtirse">
</cc1:CalendarExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left">Paquetería:</td>
<td align="left" class="Cellformat1">
<asp:DropDownList ID="dlPaqueteria" runat="server" CssClass="SelectFormat"
   DataValueField="paqueteriaID" DataTextField="paqueteria"></asp:DropDownList>
</td>
<td align="right" class="Cellformat1">Número de envío:</td>
<td align="left" class="Cellformat1">
<asp:TextBox ID="txtNumeroEnvio" runat="server" MaxLength="100" CssClass="TextInputFormat" Width="180px"></asp:TextBox>
</td>
</tr>
<tr>
<td align="left" class="Cellformat1">Fecha de envío:</td>
<td align="left" class="Cellformat1">
<asp:Textbox id="txtFechaEnvio" runat="server" Width="80px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFechaEnvio" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender4" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaEnvio" TargetControlID="txtFechaEnvio">
</cc1:CalendarExtender>
</td>
<td align="left" class="Cellformat1">Costo paquetería:</td>
<td align="left" class="Cellformat1">
<asp:TextBox ID="txtCostoPaq" runat="server" MaxLength="15" CssClass="TextInputFormat" Width="100px" AutoPostBack="true" ontextchanged="txtCostoPaq_TextChanged"></asp:TextBox>
<asp:HiddenField runat="server" ID="hdCostoPaq" />
<asp:HiddenField runat="server" ID="hdCostoPaqCam" />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Lista de precios:</td>
<td align="left" style="width:150px;">
<asp:DropDownList ID="dlListaPrecios" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista">
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
<td class="Cellformat1" align="left" >Tipo:</td>
<td class="Cellformat1" align="left">
<asp:RadioButtonList runat="server" ID="rdTipo" RepeatDirection="Horizontal">
<asp:ListItem Selected="True" Text="Contado" Value="True"></asp:ListItem>
<asp:ListItem Text="Crédito" Value="False"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="right" >Descuento:</td>
<td align="left">
<asp:TextBox ID="txtDescuento" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="40px" AutoPostBack="True"
        ontextchanged="txtDescuento_TextChanged" ></asp:TextBox>%
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
<asp:Button ID="btnGenerar" runat="server" Text="Generar Compra"
        CssClass="ButtonFormat" onclick="btnGenerar_Click" />
<asp:Button ID="btnUsarLista" runat="server" Text="Usar lista de precios"
        CssClass="ButtonFormat" onclick="btnUsarLista_Click" />
<asp:Button ID="btnFinalizar" runat="server" Text="Finalizar Orden"
        CssClass="ButtonFormat" onclick="btnFinalizar_Click" />
<asp:Button ID="btnCerrar" runat="server" Text="Cerrar Orden"
        CssClass="ButtonFormat" onclick="btnCerrar_Click" />
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
<asp:ImageButton ID="btnEmail" runat="server" CssClass="EmailFormat1"
        ToolTip="Correo" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnEmail_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="CancelFormat1"
        ToolTip="Cancelar" ImageUrl="~/imagenes/dummy.ico"
        ValidationGroup="valDatos" OnClientClick="showModalCancelar();return false;" />
<asp:ImageButton ID="btnRegresar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlDatos2" Visible="false" runat="server" DefaultButton="btnAgregarProd">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
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
<td class="GridFormatTD" align="left" style="width:510px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:230px;">Precio</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:60px;">
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
<td class="Cellformat1" align="left" valign="top" style="width:510px;">
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
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
</td>
<td align="left" valign="top" style="width:230px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg"
         onclick="btnAgregarProd_Click" />
<asp:CheckBox ID="chkCambiarPrecios" runat="server" Text="Actualizar precios" Checked="false" Visible="false" CssClass="GridFormatTD" />
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
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label8" runat="server">Precio Nuevo</asp:Label>
                </td>
            </tr>
    </table>
    </center>
	<div id="divLista">
    <asp:GridView ID="gvListasPrecios" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="listapreciosID" Width="250"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
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
                                <td style="width:100px" align="center"><asp:TextBox ID="txtPrecioNuevo" runat="server" CssClass="TextInputFormat"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "precio_nuevo") %>' MaxLength="15" Width="70px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
	</div>
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
    </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="producto" HeaderText="Producto">
        <HeaderStyle Width="245px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="codigo" HeaderText="Código Barras">
        <HeaderStyle Width="90px" />
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
    <asp:BoundField DataField="cant_compra" HeaderText="Recibido">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="diferencia" HeaderText="Diferencia">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="50px" />
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
        <asp:TextBox ID="txtCambiarCantidad" runat="server" MaxLength="5" Width="40px" CssClass="TextInputFormat" ></asp:TextBox>
    </td></tr>
    <tr><td align="right" class="Cellformat1">Precio Unitario:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtCambiarUnitario" runat="server" MaxLength="15" Width="60px" CssClass="TextInputFormat" ></asp:TextBox>
        <asp:HiddenField ID="hdCambiarCosto" runat="server" />
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
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCompraDatos" Visible="false" >
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Orden de compra:</td>
<td align="left" style="width:650px;" colspan="3">
<b><asp:Label runat="server" ID="lblOrden_CompraDatos" class="Cellformat1"></asp:Label></b>
</td>
</tr>
<tr>
<td align="center" class="Titleformat1" colspan="4">
    Productos
</td>
</tr>
<tr>
<td align="center" colspan="4">
    <asp:Panel runat="server" ID="Panel2" Width="800">
        <table>
            <tr class="GridFormat">
                <td style="width:360px;color:white;font-size:XX-Small;"><asp:Label ID="lblProdList" runat="server">Producto</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label4" runat="server">Cantidad</asp:Label>
                </td>
                <td style="width:70px;color:white;font-size:XX-Small;"><asp:Label ID="Label115" runat="server">Cantidad<br/>Recibida</asp:Label>
                </td>
		        <td style="width:130px;color:white;font-size:XX-Small;"><asp:Label ID="Label16" runat="server">Precio Unitario (Actualizar)</asp:Label>
                </td>
                <td style="width:70px;color:white;font-size:XX-Small;"><asp:Label ID="Label17" runat="server">Lote</asp:Label>
                </td>
                <td style="width:120px;color:white;font-size:XX-Small;"><asp:Label ID="Label18" runat="server">Caducidad</asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:GridView ID="gvProductosCompra" runat="server" AutoGenerateColumns="false"
        CaptionAlign="Top" CellPadding="0" DataKeyNames="productoID" Width="800"
        EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false"
        HorizontalAlign="Center" SelectedIndex="0" SkinID="grdSIAN"
        onrowdatabound="gvProductosCompra_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Panel runat="server" ID="pnlDatos" Width="800">
                        <table>
                            <tr>
                                <td style="width:360px"><asp:Label ID="lblProdDatos" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "producto") %>'></asp:Label>
                                </td>
                                <td style="width:50px" align="right"><asp:Label ID="lblCantDatos" runat="server" CssClass="Cellformat1"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>'></asp:Label>
                                        <asp:HiddenField runat="server" ID="hdExento" Value='<%# DataBinder.Eval(Container.DataItem, "exento") %>' />
                                        <asp:HiddenField runat="server" ID="hdLote" Value='<%# DataBinder.Eval(Container.DataItem, "lote") %>' />
                                        <asp:HiddenField runat="server" ID="hdCaducidad" Value='<%# DataBinder.Eval(Container.DataItem, "caducidad") %>' />
                                </td>
                                <td style="width:70px" align="right"><asp:TextBox ID="txtCantidadDatos" runat="server" CssClass="TextInputFormat"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' MaxLength="10" Width="50px"></asp:TextBox>
                                </td>
                                <td style="width:130px" align="right"><asp:TextBox ID="txtCostoDatos" runat="server" CssClass="TextInputFormat" Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'
                                        Text='<%# DataBinder.Eval(Container.DataItem, "costo_unitario") %>' MaxLength="10" Width="90px"></asp:TextBox>
                                        <asp:CheckBox runat="server" ID="chkAct" SkinID="chkXSmall" Checked='<%# DataBinder.Eval(Container.DataItem, "productoChk")%>' Enabled='<%# DataBinder.Eval(Container.DataItem, "enabled") %>'/>
                                </td>
                                <td style="width:70px" align="right"><asp:TextBox ID="txtLoteDatos" runat="server" CssClass="TextInputFormat"
                                        Text="" MaxLength="20" Width="60px"></asp:TextBox>
                                </td>
                                <td style="width:120px" align="right"><asp:Textbox id="txtFechaDatos" runat="server" Width="90px" CssClass="TextInputFormat" Text=""></asp:Textbox>
                                        <asp:ImageButton ID="btnFechaDatos" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
                                        <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaDatos" TargetControlID="txtFechaDatos"></cc1:CalendarExtender>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Button ID="btnProcesar" runat="server" Text="Procesar"
        CssClass="ButtonFormat" onclick="btnProcesar_Click"  />
    <asp:Button ID="btnRegresarDatos" runat="server" Text="Regresar"
        CssClass="ButtonFormat" onclick="btnRegresarDatos_Click"  />
    <asp:HiddenField runat="server" ID="hdCompraID" />
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlVerificacion" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVerificacionContinuar">
<asp:Panel ID="pnlVerificacionHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label12" runat="server" Text="Verificación datos" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblVerificacion" CssClass="Cellformat1"></asp:Label>
    <br /><br />
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
<asp:Panel ID="pnlPrecioProv" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center">
<asp:Panel ID="pnlPrecioProvHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label2" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="Label7" runat="server" Text="Proveedores con menor precio" CssClass="msgError1" /><br />
    <asp:GridView ID="gvPrecioProv" runat="server" SkinID="grdSIAN"
        Height="1px" SelectedIndex="0" Width="350px"
        CellPadding="0"  HorizontalAlign="Center" AutoGenerateColumns="false"
        EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None">
    <Columns>
        <asp:BoundField DataField="proveedor" HeaderText="Proveedor">
            <HeaderStyle Width="200px" />
            <ItemStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="precio" HeaderText="Precio">
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
        <asp:BoundField DataField="cobra" HeaderText="Paquetería">
            <HeaderStyle Width="50px" />
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
    </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btnPrecioProvContinuar" runat="server" Text="Continuar" onclick="btnPrecioProvContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnPrecioProvCancelar" runat="server" Text="Cancelar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyPrecioProv" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdPrecioProv" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdPrecioProvBehaviorID"
    TargetControlID="btnDummyPrecioProv"
    PopupControlID="pnlPrecioProv"
    CancelControlID="btnPrecioProvCancelar"
    DropShadow="False" />
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

    function proveedorSeleccionado(sender, e) {
        $get('<%=hdProveedorID.ClientID%>').value = e.get_value();
        PageMethods.Obtener_Notas(e.get_value(), colocarNotas);
    }

    function colocarNotas(rs) {
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
        $get('<%=hdCobraPaq.ClientID%>').value = valores[5];
    }

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtPrecioUnitario.ClientID%>').value = PageMethods.ObtenerPrecio(e.get_value() + '|' + $get('<%=dlListaPrecios.ClientID%>').value, colocarPrecio);
        $get('<%=btnAgregarProd.ClientID%>').disabled = false;
    }

    function colocarPrecio(precio) {
        $get('<%=txtPrecioUnitario.ClientID%>').value = precio;
        $get('<%=hdPrecioUnitario.ClientID%>').value = precio;
        if (!$get('<%=txtPrecioUnitario.ClientID%>').disabled)
        {
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
    function setProductoPrecio() {
        $get('<%=txtPrecioUnitario.ClientID%>').focus();
        $get('<%=txtPrecioUnitario.ClientID%>').select();
    }
    function setLista() {
        $get('<%=dlListaPrecios.ClientID%>').focus();
    }
    function setAgrProd() {
        $get('<%=btnAgregarProd.ClientID%>').focus();
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
    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
	function ajustarDivLista(hacerscroll)
    {
        if (hacerscroll) {
            $('#divLista').css('height', '350px');
            $('#divLista').css('overflow', 'scroll');
        }
        else
            $('#divLista').removeAttr('style');
    }
</script>
</asp:Content>