<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="punto_venta.aspx.cs" Inherits="facturas_punto_venta" %>
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
<asp:HiddenField runat="server" ID="hdVentaLista" Value="0" />
<asp:HiddenField runat="server" ID="hdVentaListaNombre" Value="0" />
<asp:HiddenField runat="server" ID="hdVentaVendedor" Value="0" />
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
<asp:Panel ID="pnlListado" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Punto de Venta - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Folio" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Nota" Value="5"></asp:ListItem>
                    <asp:ListItem Text="Fecha Creación" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Estatus" Value="4"></asp:ListItem>
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
                    onclick="lblAgregar_Click">Generar Venta</asp:LinkButton></td>
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
<asp:Panel ID="pnlPagos" runat="server" CssClass="modalPopup" style="display:none;width:410px;padding:10px" HorizontalAlign="Center">
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
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnModificar">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Venta</td>
</tr>
<tr style="height:10px">
<td colspan="4"></td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:150px;">Nota:</td>
<td class="Cellformat1" align="left" colspan="2">
<asp:TextBox ID="txtNota" runat="server" MaxLength="10" CssClass="TextInputFormat" Width="100px"></asp:TextBox>
-&nbsp;<asp:TextBox ID="txtNota_Suf" runat="server" MaxLength="10" CssClass="TextInputFormat" Width="100px"></asp:TextBox>
</td>
<td class="Cellformat1" align="right">Pagos:
<asp:LinkButton ID="lnkPagos" runat="server" CssClass="LinkFormat" 
onclick="lnkPagos_Click" />
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
<asp:Textbox id="txtFecha" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
<asp:ImageButton ID="btnFecha" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
<cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFecha" TargetControlID="txtFecha">
</cc1:CalendarExtender>
</td>
<td class="Cellformat1" align="left" style="width:50px;">Estatus:</td>
<td align="left" style="width:400px;">
<asp:DropDownList ID="dlEstatus" runat="server" CssClass="SelectFormat" DataValueField="estatusID" DataTextField="estatus" Enabled="false">
</asp:DropDownList>
<asp:HiddenField runat="server" ID="hdEstatus" Value="" />
<asp:RadioButtonList ID="rdIVA" runat="server" AppendDataBoundItems="True" RepeatDirection="Horizontal"
    RepeatLayout="Flow" CssClass="Cellformat1" AutoPostBack="True" OnSelectedIndexChanged="rdIVA_SelectedIndexChanged">
</asp:RadioButtonList>
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
<asp:Button ID="btnFinalizar" runat="server" Text="Finalizar Captura" 
        CssClass="ButtonFormat" onclick="btnFinalizar_Click" />
<asp:Button ID="btnFacturar" runat="server" Text="Facturar" 
        CssClass="ButtonFormat" OnClientClick="showModalFacturar();return false;" />
</td>
</tr>
<tr>
<td align="center" colspan="4">
<br />
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1" 
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos" 
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnImprimir" runat="server" CssClass="ReporteFormat1" 
        ToolTip="Imprimir" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos" 
        onclick="btnImprimir_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="CancelFormat1" 
        ToolTip="Cancelar" ImageUrl="~/imagenes/dummy.ico" 
        ValidationGroup="valDatos" OnClientClick="showModalCancelar();return false;" />
<asp:ImageButton ID="btnRegresar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnRegresar_Click" ImageUrl="~/imagenes/dummy.ico" CausesValidation="false" />
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
<td class="GridFormatTD" align="left" style="width:450px;">Producto</td>
<td class="GridFormatTD" align="left" style="width:60px;">Cantidad</td>
<td class="GridFormatTD" align="left" style="width:390px;">Precio</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:450px;">
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
<asp:HiddenField runat="server" ID="hdProductoID" Value="" />
</td>
<td class="Cellformat1" align="left" valign="top" style="width:60px;">
<asp:TextBox ID="txtCantidad" runat="server" MaxLength="4" CssClass="TextInputFormat" Width="50px" ></asp:TextBox>
</td>
<td align="left" valign="top" style="width:390px;">
<asp:TextBox ID="txtPrecioUnitario" runat="server" MaxLength="8" CssClass="TextInputFormat" Width="75px" ></asp:TextBox>
&nbsp;<asp:ImageButton ID="btnAgregarProd" runat="server" ToolTip="Agregar" ImageUrl="~/imagenes/agregaritem.jpg" 
         onclick="btnAgregarProd_Click" />
<asp:CheckBox ID="chkCambiarPrecios" runat="server" Text="Actualizar precios" CssClass="GridFormatTD" />
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
    <tr><td align="right" class="Cellformat1">Precio Unitario:</td>
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
        <asp:HiddenField runat="server" ID="hdSal" Value='<%# Eval("sal") %>' />
        </td>
    </tr></table>
    </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="producto" HeaderText="Producto">
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
<asp:Panel ID="pnlMV" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnMVContinuar">
<asp:Panel ID="pnlMVHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label6" runat="server" Text="Mover Producto" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
<center>
        <asp:Label ID="lblProdPos" class="Cellformat1" runat="server" />
    <br />
        <asp:Label ID="Label3" class="Cellformat1" runat="server" Text="Nueva posición: " />
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
    <asp:Label ID="Label5" runat="server" Text="Actualizar Producto" CssClass="msgErrorHeader" />
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
    </td></tr>
    <tr><td align="right" class="Cellformat1" valign="top">Lote:</td>
        <td align="left" class="Cellformat1">
        <asp:GridView ID="gvCambiarLote" runat="server" AutoGenerateColumns="false" 
            CaptionAlign="Top" CellPadding="0" DataKeyNames="loteID" 
            EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false" 
            HorizontalAlign="Left" SelectedIndex="0">
            <Columns>
            <asp:TemplateField>
            <ItemTemplate>
                <asp:TextBox ID="txtCantidadLote" runat="server" CssClass="TextInputFormat" 
                           Text='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' MaxLength="5" Width="50px"></asp:TextBox>
                <asp:Label ID="lblLote" runat="server" CssClass="Cellformat1" 
                           Text='<%# DataBinder.Eval(Container.DataItem, "lote") %>'></asp:Label>
                <asp:HiddenField runat="server" ID="hdCantidadLote" Value='<%# DataBinder.Eval(Container.DataItem, "cantidad_inv") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
         </asp:GridView>
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
 <asp:Panel ID="pnlLotes" runat="server" CssClass="modalPopup" style="display:none;width:250px;padding:10px" HorizontalAlign="Center" DefaultButton="btnLoteContinuar">
<asp:Panel ID="pnlLotesHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label8" runat="server" Text="Seleccione Lote" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1" valign="top">Lote:</td>
        <td align="left" class="Cellformat1">
        <asp:GridView ID="gvLote" runat="server" AutoGenerateColumns="false" 
            CaptionAlign="Top" CellPadding="0" DataKeyNames="loteID" 
            EnableViewState="True" GridLines="None" Height="1px" ShowHeader="false" 
            HorizontalAlign="Left" SelectedIndex="0">
            <Columns>
            <asp:TemplateField>
            <ItemTemplate>
                <asp:TextBox ID="txtCantidadLote" runat="server" CssClass="TextInputFormat" 
                           Text="0" MaxLength="5" Width="50px"></asp:TextBox>
                <asp:Label ID="lblLote" runat="server" CssClass="Cellformat1" 
                           Text='<%# DataBinder.Eval(Container.DataItem, "lote") %>'></asp:Label>
                <asp:HiddenField runat="server" ID="hdCantidadLote" Value='<%# DataBinder.Eval(Container.DataItem, "cantidad") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
         </asp:GridView>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnLoteContinuar" runat="server" Text="Continuar" onclick="btnLoteContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnLoteCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />   
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyLote" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdSeleccionarLote" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdLoteBehaviorID"
    TargetControlID="btnDummyLote"
    PopupControlID="pnlLotes"
    CancelControlID="btnLoteCerrar"
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
<asp:Panel ID="pnlMessageFacturar" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnFacturarContinuar">
<asp:Panel ID="pnlMessageFacturarHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label4" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="lblMessageFacturar" runat="server" Text="Ingrese el folio de la factura o deje en blanco para generar una nueva" CssClass="msgError1" /><br />
    <asp:TextBox ID="txtFacturaID" runat="server" MaxLength="10" CssClass="TextInputFormat" Width="100px" ></asp:TextBox><br />
    <asp:Button ID="btnFacturarContinuar" runat="server" Text="Continuar" onclick="btnFacturarContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnFacturarCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyFacturar" Text="" style="visibility:hidden" />
<cc1:ModalPopupExtender ID="mdFacturar" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdFacturarBehaviorID"
    TargetControlID="btnDummyFacturar"
    PopupControlID="pnlMessageFacturar"
    CancelControlID="btnFacturarCerrar"
    DropShadow="False" />
<asp:Panel ID="pnlPago" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnPagoContinuar">
<asp:Panel ID="pnlPagoHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label9" runat="server" Text="Ingrese Pago" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <table>
    <tr><td align="right" class="Cellformat1">Fecha pago:</td>
        <td align="left" class="Cellformat1">
        <asp:Textbox id="txtFechaPago" runat="server" Width="100px" CssClass="TextInputFormat"></asp:Textbox>
        <asp:ImageButton ID="btnFechaPago" runat="server" CausesValidation="False" ImageUrl="../imagenes/calendario.png" Height="20px" Width="20px" />
        <cc1:CalendarExtender ID="CalendarExtender3" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnFechaPago" TargetControlID="txtFechaPago"></cc1:CalendarExtender>
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
    <tr><td align="right" class="Cellformat1">Monto del pago:</td>
        <td align="left" class="Cellformat1">
        <asp:TextBox ID="txtPago" runat="server" CssClass="TextReadOnly" Width="90px" MaxLength="10"></asp:TextBox>
    </td></tr>
    </table>
    <br />
    <asp:Button ID="btnPagoContinuar" runat="server" Text="Continuar" onclick="btnPagoContinuar_Click" CssClass="ButtonFormat"  />
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
</td>
</tr>
</table>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdAFacturar" />
<asp:Panel ID="pnlVerificacion" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVerificacionContinuar">
<asp:Panel ID="pnlVerificacionHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label12" runat="server" Text="Verificación de Factura" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblVerificacionProducto" CssClass="Cellformat1">Está creando una remisión sin usar un pedido, ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="lblCodigoVerificacion" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigo_Verificacion" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnVerificacionContinuar" runat="server" Text="Agregar" onclick="btnVerificacionContinuar_Click" CssClass="ButtonFormat"  />
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
<asp:Panel ID="pnlLimiteCR" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnLimiteCRContinuar">
<asp:Panel ID="pnlLimiteCRHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label192" runat="server" Text="Verificación de Límite de Crédito" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblLimiteCR" CssClass="Cellformat1"></asp:Label>
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
<asp:Panel ID="pnlProdVenta" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnProdVentaContinuar">
<asp:Panel ID="pnlProdVentaHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label11" runat="server" Text="Verificación de Precio de Venta" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblProdVenta" CssClass="Cellformat1"></asp:Label>
    <br /><br />
    <asp:Label runat="server" ID="Label13" CssClass="Cellformat1">Ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="Label14" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigoProdVenta" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
    <br />
    <asp:Button ID="btnProdVentaContinuar" runat="server" Text="Continuar" onclick="btnProdVentaContinuar_Click" CssClass="ButtonFormat"  />
    <asp:Button ID="btnProdVentaCerrar" runat="server" Text="Cerrar" CssClass="ButtonFormat" />   
    <br />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyProdVenta" Text="" style="visibility:hidden" />
<asp:HiddenField runat="server" ID="hdVentaAccion" />
<cc1:ModalPopupExtender ID="mdProdVenta" runat="server"
    BackgroundCssClass="modalBackground" BehaviorID="mdProdVentaBehaviorID"
    TargetControlID="btnDummyProdVenta"
    PopupControlID="pnlProdVenta"
    CancelControlID="btnProdVentaCerrar"
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

    function productoSeleccionado(sender, e) {
        $get('<%=hdProductoID.ClientID%>').value = e.get_value();
        $get('<%=txtPrecioUnitario.ClientID%>').value = PageMethods.ObtenerPrecio(e.get_value() + '|' + $get('<%=hdVentaLista.ClientID%>').value, colocarPrecio);
        $get('<%=btnAgregarProd.ClientID%>').disabled = false;
    }

    function colocarPrecio(precio) {
        $get('<%=txtPrecioUnitario.ClientID%>').value = precio;
        $get('<%=hdPrecioUnitario.ClientID%>').value = precio;
        $get('<%=txtCantidad.ClientID%>').focus();
    }

    function showModalCancelar(ev) {
        var modalPopupBehavior = $find('mdCancelarBehaviorID');
        modalPopupBehavior.show();
    }

    function showModalFacturar(ev) {
        var modalPopupBehavior = $find('mdFacturarBehaviorID');
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

    function setCantidad() {
        $get('<%=txtCantidad.ClientID%>').focus();
    }

    function mostrarPopUp(strURL) {
        window.open(strURL, "SIANRPT", "location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=900, height=500,left=40,top=50");
    }
    function setPos() {
        $get('<%=txtPosicion.ClientID%>').focus();
    }
    function esconder() {
        var ballonPop = $find('popNegocio');
        ballonPop.hidePopup();
    }
    function esconder2() {
        var ballonPop = $find('popNegocio2');
        ballonPop.hidePopup();
    }

    function isNumber(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode != 8 && charCode > 31
            && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

    function validateLote(n) {
        if (n.value.length == 0)
            n.value = '0';
        if (isNaN(n.value)) {
            alert('Monto no válido');
            n.focus();
            n.select();
            return false;
        }

        var s = parseInt("0");

        var gv = $get('<%=gvLote.ClientID%>');
        var inputList = gv.getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i].type == "text")
                s += parseInt(inputList[i].value);
        }

        if (s == parseInt($get('<%=txtCantidad.ClientID%>').value))
            $get('<%=btnLoteContinuar.ClientID%>').disabled = false;
        else
            $get('<%=btnLoteContinuar.ClientID%>').disabled = true;

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

        var gv = $get('<%=gvCambiarLote.ClientID%>');
        if (gv != null) {
            var s = parseInt("0");
            var inputList = gv.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                if (inputList[i].type == "text")
                    s += parseInt(inputList[i].value);
            }

            if (s == parseInt($get('<%=txtCambiarCantidad.ClientID%>').value))
                $get('<%=btnCambiarContinuar.ClientID%>').disabled = false;
            else
                $get('<%=btnCambiarContinuar.ClientID%>').disabled = true;
        }
    }
    function limpiarProdID() {
        $get('<%=txtProducto.ClientID%>').value = '';
        $get('<%=hdProductoID.ClientID%>').value = '';
    }
</script>
</asp:Content>