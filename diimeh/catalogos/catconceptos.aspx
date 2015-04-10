<%@ Page Title="SIAN - Sistema de Control de Inventarios" Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catconceptos.aspx.cs" Inherits="catalogos_catconceptos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuCompras" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo Conceptos - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 569px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Nombre" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Descripción" Value="6"></asp:ListItem>
                    <asp:ListItem Text="Familia" Value="1"></asp:ListItem>
                    <asp:ListItem Text="División" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Código" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Clave" Value="5"></asp:ListItem>
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
                Concepto</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="950px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="referencia"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                    onrowcommand="grdvLista_RowCommand">
            <Columns>
                <asp:ButtonField DataTextField="clave" CommandName="Modificar" HeaderText="Clave" SortExpression="5" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" >
                    <HeaderStyle Width="320px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="precio" HeaderText="Precio" >
                    <HeaderStyle Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="familia" HeaderText="Familia" SortExpression="1" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="clase" HeaderText="División" SortExpression="2" >
                    <HeaderStyle Width="90px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="4" >
                    <HeaderStyle Width="100px" />
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
<asp:Panel ID="pnlDatos" Visible="false" runat="server" DefaultButton="btnDummy">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Datos Concepto</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Nombre:</td>
<td align="left" style="width:350px;">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="300px"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td class="Cellformat1" colspan="2" align="left" style="width:450px;" >
<asp:RadioButtonList runat="server" ID="rdExento" RepeatDirection="Horizontal">
<asp:ListItem Selected="False" Text="con IVA" Value="False"></asp:ListItem>
<asp:ListItem Text="sin IVA" Value="True"></asp:ListItem>
</asp:RadioButtonList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:100px;">Descripción:</td>
<td colspan="3" style="width:800px;" align="left">
<asp:TextBox ID="txtDescripcion" runat="server" MaxLength="500" Width="750px" Height="90px" CssClass="TextInputFormat" TextMode="MultiLine"></asp:TextBox><br />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Clave:</td>
<td style="width: 350px" align="left" colspan="3">
<asp:TextBox ID="txtClave" runat="server" MaxLength="9" Width="200px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RegularExpressionValidator id="RegularExpressionValidator6" runat="server" SkinID="regValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Clave debe ser numérica" ControlToValidate="txtClave" ValidationExpression="^\d+$" ValidationGroup="valDatos"></asp:RegularExpressionValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender16" runat="Server" TargetControlID="RegularExpressionValidator6" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:RangeValidator ID="RangeValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Clave debe ser mayor a cero" ControlToValidate="txtClave" ValidationGroup="valDatos" MinimumValue="1" MaximumValue="999999999999999999999999999999"></asp:RangeValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender17" runat="Server" TargetControlID="RangeValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Código de barras:</td>
<td align="left" style="width: 350px">
<asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" Width="290px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Familia:</td>
<td align="left" style="width: 350px">
<asp:DropDownList ID="dlFamilia" runat="server" CssClass="SelectFormat"
        DataValueField="familiaID" DataTextField="familia" AutoPostBack="True"
        onselectedindexchanged="dlFamilia_SelectedIndexChanged">
</asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">División:</td>
<td align="left" style="width: 350px">
<asp:DropDownList ID="dlClase" runat="server" CssClass="SelectFormat"
        DataValueField="claseID" DataTextField="clase">
</asp:DropDownList>
</td>
</tr>
<tr>
<td align="center" colspan="4">
<br />
<asp:ImageButton ID="btnDummy" runat="server" ImageUrl="~/imagenes/dummy.ico" Enabled="false" />
<asp:ImageButton ID="btnAgregar" runat="server" CssClass="AddFormat1"
        ToolTip="Agregar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnAgregar_Click" />
<asp:ImageButton ID="btnModificar" runat="server" CssClass="ModifyFormat1"
        ToolTip="Modificar" ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatos"
        onclick="btnModificar_Click" />
<asp:ImageButton ID="btnCancelar" runat="server" CssClass="BackFormat1" ToolTip="Regresar" OnClick="btnCancelar_Click" ImageUrl="~/imagenes/dummy.ico" />
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:Button ID="btnAgregarPrecio" runat="server" Text="Agregar precio"
        CssClass="ButtonFormat" onclick="btnAgregarPrecio_Click" />&nbsp;
<asp:Button ID="btnHistorial" runat="server" Text="Historial de precio"
        CssClass="ButtonFormat" onclick="btnHistorial_Click" />
</td>
</tr>
<tr>
<td align="center" colspan="4" class="Titleformat1">
    Lista de Precios de Venta
</td>
</tr>
<tr>
<td align="center" colspan="4">
<asp:GridView ID="gvPreciosVenta" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" Width="450px" CellPadding="0"
    AutoGenerateColumns="false" DataKeyNames="listapreciosID"
    HorizontalAlign="Center" EnableViewState="True"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvPreciosVenta_RowCommand">
<Columns>
    <asp:BoundField DataField="nombrelista" HeaderText="Lista">
        <HeaderStyle Width="330px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="precio" HeaderText="Precio">
        <HeaderStyle Width="100px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Fecha">
        <HeaderStyle Width="150px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:ButtonField Text="Modificar" CommandName="Modificar" HeaderText="Modificar" >
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
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
<asp:Panel ID="pnlPrecio" Visible="false" runat="server">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Precio del Producto</td>
</tr>
<tr>
<td align="center" style="width:800px; height: 20px;" colspan="2">
    <br />
&nbsp; </td>
</tr>
<tr>
<td align="center" class="Cellformat1" colspan="2"><b><asp:Label runat="server" ID="lblNombre"></asp:Label></b><br />&nbsp;
</td>
</tr>
<tr>
<td align="right" style="width:400px;" class="Cellformat1" >Lista de precios:&nbsp;
</td>
<td align="left" style="width:500px;">
<asp:DropDownList ID="dlListaPrecio" runat="server" CssClass="SelectFormat" DataValueField="listaprecioID" DataTextField="nombrelista">
</asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="right" style="width: 400px">Precio:&nbsp;</td>
<td align="left" style="width: 500px">
<asp:TextBox ID="txtPrecio" runat="server" MaxLength="15" Width="100px" CssClass="TextInputFormat" ></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator6" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el precio por cajas" ControlToValidate="txtPrecio" ValidationGroup="valDatosPre"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender11" runat="Server" TargetControlID="RequiredFieldValidator6" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:RegularExpressionValidator id="RegularExpressionValidator4" runat="server" SkinID="regValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Precio por cajas debe ser numérico" ControlToValidate="txtPrecio" ValidationExpression="^\d+(\.\d\d)?$" ValidationGroup="valDatosPre"></asp:RegularExpressionValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender12" runat="Server" TargetControlID="RegularExpressionValidator4" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
<tr>
    <td align="center" colspan="2" style="width:800px;">
        <br />
        <asp:ImageButton ID="btnGuardarPrecio" runat="server" CssClass="AddFormat1"
            ImageUrl="~/imagenes/dummy.ico" ValidationGroup="valDatosPre"
            onclick="btnGuardarPrecio_Click" Text="Guardar" />
        <asp:ImageButton ID="btnCancelarPrecio" runat="server" CssClass="BackFormat1"
            ImageUrl="~/imagenes/dummy.ico" onclick="btnCancelarPrecio_Click" Text="Regresar" />
    </td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlMostrarImagen" runat="server" CssClass="modalPopup" style="display:none;width:650px;padding:10px" HorizontalAlign="Center">
<div onclick="javascript:closePopup()";>
    <asp:Image ID="imgMostrarImagen" runat="server" />
    <asp:Button ID="btnMostrarImagen" runat="server" CssClass="BackFormat1" ToolTip="Regresar" />
</div>
</asp:Panel>
<asp:Button runat="server" ID="btnDummyMostrarImagen" Text="" style="visibility:hidden"    />
<cc1:ModalPopupExtender ID="mdImagen" runat="server"
    BehaviorID="programmaticModalPopupBehavior"
    BackgroundCssClass="modalBackground"
    TargetControlID="btnDummyMostrarImagen"
    PopupControlID="pnlMostrarImagen"
    DropShadow="False" />
<asp:Panel ID="pnlListasPrecios" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center">
<asp:Panel ID="pnlListasPreciosHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label2" runat="server" Text="Mensaje" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label ID="Label3" runat="server" Text="Actualizar Listas de Ventas" CssClass="msgError1" /><br />
    <table>
            <tr class="GridFormat">
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label4" runat="server">Lista de Precios</asp:Label>
                </td>
                <td style="width:50px;color:white;font-size:XX-Small;"><asp:Label ID="Label5" runat="server">Precio Actual</asp:Label>
                </td>
                <td style="width:100px;color:white;font-size:XX-Small;"><asp:Label ID="Label8" runat="server">Precio Nuevo</asp:Label>
                </td>
            </tr>
    </table>
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
<asp:HiddenField runat="server" ID="hdListaCompras" Value="" />
<asp:HiddenField runat="server" ID="hdPrecioAnterior" Value="" />
<asp:HiddenField runat="server" ID="hdPorcentajeAumento" Value="" />
<asp:Panel ID="pnlHistorial" runat="server" Visible="false" DefaultButton="btnRegresarHist">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" style="height: 18px;">
    Historial de Precios</td>
</tr>
<tr>
<td class="tb" align="center">
<br />
Producto:&nbsp;<asp:Label ID="lblProducto" runat="server" Text="" class="tb"></asp:Label><br />
</td>
</tr>
<tr>
<td align="center">
<asp:DataList ID="dlHistorial" runat="server" ItemStyle-HorizontalAlign="center"
    BackColor="White" BorderStyle="None" GridLines="Both"
    RepeatDirection="Vertical" Width="450px">
        <ItemTemplate>
            <asp:Panel ID="pnlHeader" runat="server">
                <asp:Label ID="lblNombreLista" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "nombrelista") %>'></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlPrecios" runat="server">
                <asp:DataList ID="dlHistorial" runat="server" ItemStyle-HorizontalAlign="center"
                    BackColor="White" BorderStyle="None"
                    DataSource='<%# ((System.Data.DataRowView)Container.DataItem).CreateChildView("Lista_Precios") %>'
                    GridLines="Both" RepeatDirection="Vertical" Width="450px">
                <ItemTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                <td align="center" style="width:50%">
                    <asp:Label ID="lblPrecio" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "precio") %>'></asp:Label>
                </td>
                <td align="center" style="width:50%">
                    <asp:Label ID="lblFechaVigencia" runat="server" CssClass="Cellformat1" Text='<%# DataBinder.Eval(Container.DataItem, "vigencia") %>'></asp:Label>
                </td>
                </tr>
                </table>
                </ItemTemplate>
                </asp:DataList>
            </asp:Panel>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Left" BackColor="#D5EAEA" />
    </asp:DataList>
</td>
</tr>
<tr>
<td align="center">
<asp:ImageButton ID="btnRegresarHist" runat="server" CssClass="BackFormat1"
            ImageUrl="~/imagenes/dummy.ico" onclick="btnRegresarHist_Click" Text="Regresar" />
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
    function archivoResult(rs) {
        alert(rs);
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