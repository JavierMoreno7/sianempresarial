<%@ Page Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="catproductos.aspx.cs" Inherits="catalogos_catproductos" Title="SIAN - Sistema de Control de Inventarios" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType virtualpath="~/master/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="visibility:hidden">
<cc1:AsyncFileUpload  runat="server" ID="AsyncFileUpload1"
     Width="300px" UploaderStyle="Modern" CssClass="FileUploadClass"
     UploadingBackColor="#CCFFFF" />
</div>
<asp:HiddenField ID="hdID" Value="" runat="server"/>
<asp:HiddenField ID="hdAT" Value="" runat="server"/>
<asp:HiddenField ID="hdTempID" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuCom" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuVen" Value="" runat="server"/>
<asp:HiddenField ID="hdTempImg" Value="" runat="server" />
<asp:HiddenField ID="hdPos" Value="" runat="server" />
<asp:HiddenField ID="hdTempID2" Value="" runat="server"/>
<asp:HiddenField ID="hdUsuPr" Value="" runat="server"/>
<asp:Panel ID="pnlListado" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
        <tr>
            <td class="GridFormat" colspan="3" style="height: 18px;">
                Catálogo Productos - Listado de Datos</td>
        </tr>
        <tr>
            <td style="width: 500px;  height: 20px;" align="left" class="Cellformat1">
                Buscar por:
                <asp:DropDownList ID="dlBusqueda" runat="server" CssClass="SelectFormat">
                    <asp:ListItem Selected="True" Text="Nombre/Sal/Descripción" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Familia" Value="1"></asp:ListItem>
                    <asp:ListItem Text="División" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Código" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Clave" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="Clave de gobierno" Value="8"></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:TextBox ID="txtCriterio" runat="server" Width="200px" CssClass="TextInputFormat"></asp:TextBox>
                &nbsp;<asp:ImageButton
                    ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
                    OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
            <td style="width: 150px;" align="right">
                <asp:LinkButton ID="lblMostrar" runat="server" Visible="False" CssClass="LinkFormat" OnClick="lblMostrar_Click">Todos
                los Registros</asp:LinkButton></td>
            <td style="width: 150px;" align="right">
                <asp:Label ID="lblInfo" runat="server" />
                <asp:Panel runat="server" ID="pnlInfo">
                    </asp:Panel>
                    <cc1:BalloonPopupExtender ID="PopupControlExtender1" runat="server"
                    TargetControlID="lblInfo" BehaviorID="popInfo"
                    BalloonPopupControlID="pnlInfo"
                    DynamicServiceMethod="ObtenerProdDatos"
                    DynamicContextKey="1401~1~1~Nombre"
                    DynamicServicePath="~/Services/ComboServices.asmx"
                    DynamicControlID="pnlInfo"
                    Position="BottomLeft"
                    BalloonStyle="Custom"
                    CustomClassName="custom"
                    CustomCssUrl="../css/popup2.css"
                    UseShadow="false"
                    ScrollBars="None"
                    DisplayOnMouseOver="true"
                    DisplayOnFocus="false"
                    DisplayOnClick="false" />
                <asp:LinkButton ID="lblAgregar" runat="server" CssClass="LinkFormat" OnClick="lblAgregar_Click">Agregar
                Producto</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
            <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN"
                Height="1px" SelectedIndex="0" Width="950px" AllowSorting="True"
                    CellPadding="0"  AutoGenerateColumns="false" DataKeyNames="referencia"
                HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="True"
                EnableTheming="True" UseAccessibleHeader="True" CaptionAlign="Top" GridLines="None"
                onrowcommand="grdvLista_RowCommand" onrowdatabound="grdvLista_RowDataBound">
            <Columns>
                <asp:ButtonField DataTextField="clave" CommandName="Modificar" HeaderText="Clave" SortExpression="5" >
                    <HeaderStyle Width="70px" />
                    <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
                </asp:ButtonField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" >
                    <HeaderStyle Width="300px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="sales" HeaderText="Sales" >
                    <HeaderStyle Width="160px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="precio" HeaderText="Precio" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="codigo" HeaderText="Código" SortExpression="4" >
                    <HeaderStyle Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="existencia" HeaderText="Existencia" >
                    <HeaderStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:BoundField DataField="iva" HeaderText="IVA" >
                    <HeaderStyle Width="30px" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="unidad" HeaderText="Unidad Medida" >
                    <HeaderStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:TemplateField>
                    <HeaderStyle Width="20px" />
                    <ItemTemplate>
                    <asp:Image runat="server" ID="imgP" ImageUrl="~/imagenes/info2.jpg" Height="12" Width="12" />
                    <asp:HiddenField runat="server" ID="hdP" Value='<%# DataBinder.Eval(Container.DataItem, "inforef")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
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
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr><td class="GridFormat" colspan="4" style="height: 18px;">
    Datos Producto</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Nombre:</td>
<td align="left" style="width:350px;">
<asp:TextBox ID="txtNombre" runat="server" MaxLength="50" CssClass="TextInputFormat" Width="345px"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese el nombre" ControlToValidate="txtNombre" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender3" runat="Server" TargetControlID="RequiredFieldValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
<td align="left" style="width:450px;" colspan="2">
<table border="0" cellspacing="0" cellpadding="0" style="width: 450px;">
<tr>
<td class="Cellformat1" align="left" style="width:250px;" >
<asp:RadioButtonList runat="server" ID="rdExento" RepeatDirection="Horizontal">
<asp:ListItem Selected="False" Text="con IVA" Value="False"></asp:ListItem>
<asp:ListItem Text="sin IVA" Value="True"></asp:ListItem>
</asp:RadioButtonList>
</td>
<td class="Cellformat1" align="right" style="width:200px;" >
<asp:ImageButton runat="server" ID="btnPrevProd" Height="10px" Width="10px"
     ImageUrl="~/imagenes/previousPageIcon.gif" onclick="btnPrevProd_Click" />&nbsp;
<asp:ImageButton runat="server" ID="btnNextProd" Height="10px" Width="10px"
     ImageUrl="~/imagenes/nextPageIcon.gif" onclick="btnNextProd_Click" />&nbsp;
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Sales:</td>
<td align="left" style="width:350px;">
<asp:TextBox ID="txtSales" runat="server" MaxLength="250" CssClass="TextInputFormat" Width="345px"></asp:TextBox>
</td>
<td colspan="2" align="left" style="width:450px;" rowspan="3" >
<table border="0" cellspacing="0" cellpadding="0" style="width: 450px;">
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:20px;" >
<asp:ImageButton ID="btnAgregarImagen" runat="server" Text="Agregar imagen"
 ImageUrl="~/imagenes/foto.jpg" Height="20" Width="20" onclick="btnAgregarImagen_Click" />
</td>
<td align="left" style="width:430px;">
<div style="Height:130px;Width:430px;Overflow:Auto">
<asp:DataList ID="dlImagenes" runat="server" ItemStyle-HorizontalAlign="center"
    BorderStyle="None" DataKeyField="imagenArchivo"
    BorderWidth="0px" CellPadding="3" GridLines="Both"
    RepeatDirection="Horizontal" Width="430px" OnItemCommand="dlImagenes_ItemCommand">
        <ItemTemplate>
            <asp:ImageButton ID="imgProducto" runat="server" ImageUrl='<%# "~/fotos/" + DataBinder.Eval(Container.DataItem, "imagenArchivo") %>' Width='<%# DataBinder.Eval(Container.DataItem, "imagenWidth") %>' Height='<%# DataBinder.Eval(Container.DataItem, "imagenHeight") %>' CommandName="Mostrar" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "imagenArchivo") %>' />
            <br /><asp:ImageButton ID="btnBorrar" runat="server" ImageUrl="~/imagenes/eliminar.png" AlternateText="Borrar" CommandName="Borrar" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "imagenArchivo") %>' Width="10" Height="10" />
            <asp:Button ID="btnHacerPrincipal" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "imagenPrincipalTexto") %>' Enabled='<%# DataBinder.Eval(Container.DataItem, "imagenPrincipalEnabled") %>'
                        CssClass="ButtonFormat" CommandName="HacerPrincipal" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "imagenArchivo") %>'/>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Center" VerticalAlign="Bottom" BackColor="Transparent" />
    </asp:DataList>
</div>
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" valign="top" style="width:100px;">Descripción:</td>
<td style="width:350px;" align="left">
<asp:TextBox ID="txtDescripcion" runat="server" MaxLength="5000" Width="345px"
        Height="90px" CssClass="TextInputFormat" TextMode="MultiLine"
        AutoPostBack="True" ontextchanged="txtDescripcion_TextChanged"></asp:TextBox><br />
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width:100px;">Clave:</td>
<td style="width: 350px" align="left">
<asp:TextBox ID="txtClave" runat="server" MaxLength="9" Width="200px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RegularExpressionValidator id="RegularExpressionValidator6" runat="server" SkinID="regValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Clave debe ser numérica" ControlToValidate="txtClave" ValidationExpression="^\d+$" ValidationGroup="valDatos"></asp:RegularExpressionValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender16" runat="Server" TargetControlID="RegularExpressionValidator6" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
<asp:RangeValidator ID="RangeValidator2" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Dato Inválido</b><br/>Clave debe ser mayor a cero" ControlToValidate="txtClave" ValidationGroup="valDatos" MinimumValue="1" MaximumValue="999999999999999999999999999999"></asp:RangeValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender17" runat="Server" TargetControlID="RangeValidator2" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
</td>
</tr>
</table>
<table border="0" cellspacing="0" cellpadding="0" style="width: 900px;">
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Código 1:</td>
<td align="left" style="width: 200px">
<asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" Width="195px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left" style="width: 100px">Código 2:</td>
<td align="left" style="width: 200px">
<asp:TextBox ID="txtCodigo2" runat="server" MaxLength="50" Width="195px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left" style="width: 100px">Código 3:</td>
<td align="left" style="width: 200px">
<asp:TextBox ID="txtCodigo3" runat="server" MaxLength="50" Width="195px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Ubicación:</td>
<td align="left" style="width: 200px">
<asp:TextBox ID="txtUbicacion" runat="server" MaxLength="200" Width="195px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td class="Cellformat1" align="left" style="width: 100px">Familia:</td>
<td align="left" style="width: 200px">
<asp:DropDownList ID="dlFamilia" runat="server" CssClass="SelectFormat"
        DataValueField="familiaID" DataTextField="familia" AutoPostBack="True"
        onselectedindexchanged="dlFamilia_SelectedIndexChanged">
</asp:DropDownList>
</td>
<td class="Cellformat1" align="left" style="width: 100px">División:</td>
<td align="left" style="width: 200px">
<asp:DropDownList ID="dlClase" runat="server" CssClass="SelectFormat"
        DataValueField="claseID" DataTextField="clase">
</asp:DropDownList>
</td>
</tr>
<tr>
<td class="Cellformat1" align="left" style="width: 100px">Clave gobierno:</td>
<td align="left" style="width: 200px">
<asp:TextBox ID="txtClave_Gobierno" runat="server" MaxLength="50" Width="195px" CssClass="TextInputFormat"></asp:TextBox>
</td>
<td align="left" class="Cellformat1" style="width:100px;">Unidad Medida:</td>
<td align="left" class="Cellformat1" colspan="3">
<asp:TextBox ID="txtUnidadDeMedida" runat="server" MaxLength="10" Width="100px" CssClass="TextInputFormat"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator8" runat="server" SkinID="reqValid" SetFocusOnError="true" Display="None" ErrorMessage="<b>Campo Requerido</b><br/>Ingrese la unidad de medidad" ControlToValidate="txtUnidadDeMedida" ValidationGroup="valDatos"></asp:RequiredFieldValidator>
<cc1:ValidatorCalloutExtender id="ValidatorCalloutExtender18" runat="Server" TargetControlID="RequiredFieldValidator8" HighlightCssClass="validatorCalloutHighlight"></cc1:ValidatorCalloutExtender>
Mínimo:
<asp:TextBox ID="txtMinimo" runat="server" MaxLength="5" Width="50px" CssClass="TextInputFormat"></asp:TextBox>
Máximo:
<asp:TextBox ID="txtMaximo" runat="server" MaxLength="5" Width="50px" CssClass="TextInputFormat"></asp:TextBox>
Reorden:
<asp:TextBox ID="txtReorden" runat="server" MaxLength="5" Width="50px" CssClass="TextInputFormat"></asp:TextBox>
</td>
</tr>
<tr>
<td align="left" class="Cellformat1" style="width:100px;">&nbsp;</td>
<td align="left" class="Cellformat1" colspan="4">
<asp:CheckBox runat="server" ID="chkLimitado" Text="Descuento limitado" />
<asp:CheckBox runat="server" ID="chkNeto" Text="Neto" />
<asp:CheckBox runat="server" ID="chkLote" Text="Lote requerido" />
<asp:CheckBox runat="server" ID="chkCaducidad" Text="Fecha de Caducidad requerida" />
<asp:CheckBox runat="server" ID="chkActivo" Text="Activo" />
<asp:HiddenField runat="server" ID="hdActivo" />
</td>
</tr>
<tr>
<td align="center" colspan="6">
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
<td align="center" colspan="6">
<asp:Button ID="btnAgregarPrecio" runat="server" Text="Agregar precio"
        CssClass="ButtonFormat" onclick="btnAgregarPrecio_Click" />&nbsp;
<asp:Button ID="btnHistorial" runat="server" Text="Historial de precio"
        CssClass="ButtonFormat" onclick="btnHistorial_Click" />
</td>
</tr>
<tr>
<td align="center" colspan="3" class="Titleformat1">
    Lista de Precios de Venta
</td>
<td align="center" colspan="3" class="Titleformat1">
    Lista de Precios de Compra
</td>
</tr>
<tr>
<td align="center" colspan="3" valign="top">
<asp:GridView ID="gvPreciosVenta" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" Width="430px" CellPadding="0"
    AutoGenerateColumns="false" DataKeyNames="listapreciosID"
    HorizontalAlign="Center" EnableViewState="True"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvPreciosVenta_RowCommand">
<Columns>
    <asp:BoundField DataField="nombrelista" HeaderText="Lista">
        <HeaderStyle Width="170px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="precio" HeaderText="Precio">
        <HeaderStyle Width="90px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Fecha">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:ButtonField Text="Modificar" CommandName="Modificar" HeaderText="Modificar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
</Columns>
</asp:GridView>
</td>
<td align="center" colspan="3" valign="top">
<asp:GridView ID="gvPreciosCompra" runat="server" SkinID="grdSIAN"
    Height="1px" SelectedIndex="0" Width="430px" CellPadding="0"
    AutoGenerateColumns="false" DataKeyNames="listapreciosID"
    HorizontalAlign="Center" EnableViewState="True"
    CaptionAlign="Top" GridLines="None" onrowcommand="gvPreciosCompra_RowCommand">
<Columns>
    <asp:BoundField DataField="nombrelista" HeaderText="Lista">
        <HeaderStyle Width="150px" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:BoundField>
    <asp:BoundField DataField="precio" HeaderText="Precio">
        <HeaderStyle Width="90px" />
        <ItemStyle HorizontalAlign="Right" />
    </asp:BoundField>
    <asp:BoundField DataField="fecha" HeaderText="Fecha">
        <HeaderStyle Width="70px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:BoundField DataField="Paq" HeaderText="Paq">
        <HeaderStyle Width="30px" />
        <ItemStyle HorizontalAlign="Center" />
    </asp:BoundField>
    <asp:ButtonField Text="Modificar" CommandName="Modificar" HeaderText="Modificar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
    <asp:ButtonField Text="Borrar" CommandName="Borrar" HeaderText="Borrar" >
        <HeaderStyle Width="50px" />
        <ItemStyle HorizontalAlign="Center" ForeColor="#6CA2B7" />
    </asp:ButtonField>
</Columns>
</asp:GridView>
</td>
</tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlArchivo" runat="server" Visible="false">
<table border="0" cellspacing="0" cellpadding="0" style="width: 800px;">
<tr><td class="GridFormat" colspan="2" style="height: 18px;">
    Imagen del Producto</td>
</tr>
<tr>
<td align="center" style="width:800px; height: 20px;" colspan="2">
    <br />
&nbsp; </td>
</tr>
<tr>
<td align="right" valign="top" style="width:400px;" class="Cellformat1" >Imagen:
</td>
    <td align="left" style="width:500px;">
    <cc1:AsyncFileUpload  runat="server" ID="flArchivo"
      OnUploadedComplete="flArchivo_UploadedComplete"
      OnUploadedFileError="flArchivo_UploadedFileError"
     Width="300px" UploaderStyle="Modern" CssClass="FileUploadClass"
     UploadingBackColor="#CCFFFF" /><br />
        <asp:CheckBox ID="chkPrincipal" runat="server" CssClass="Cellformat1" Text="Principal" Enabled="false"  />
    </td>
</tr>
    <tr>
        <td align="center" colspan="2" style="width:800px;">
            <br />
            <asp:ImageButton ID="btnCancelarImagen" runat="server" CssClass="BackFormat1"
            ImageUrl="~/imagenes/dummy.ico" onclick="btnCancelarImagen_Click" Text="Regresar" />
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
    <center>
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
<asp:Panel ID="pnlVerificacion" runat="server" CssClass="modalPopup" style="display:none;width:350px;padding:10px" HorizontalAlign="Center" DefaultButton="btnVerificacionContinuar">
<asp:Panel ID="pnlVerificacionHeader" runat="server" style="background-color:#DDDDDD;border:solid 1px Gray;color:Black">
    <asp:Label ID="Label192" runat="server" Text="Verificación" CssClass="msgErrorHeader" />
</asp:Panel>
<div>
    <br />
    <asp:Label runat="server" ID="lblVerificacion" CssClass="Cellformat1">Está modificando el estatus del producto (Activo / Inactivo)</asp:Label>
    <br /><br />
    <asp:Label runat="server" ID="Label193" CssClass="Cellformat1">Ingrese el código de verificación para continuar</asp:Label>
    <br />
    <asp:Label runat="server" ID="Label194" CssClass="Cellformat1">Código Verificación:</asp:Label>
    <asp:TextBox ID="txtCodigoVerificacion" runat="server" MaxLength="6" CssClass="TextInputFormat" Width="75px" TextMode="Password" ></asp:TextBox>
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
    function mostrarInfo(llave) {
        var ballonPop = $find('popInfo');
        ballonPop._DynamicContextKey = llave;
        ballonPop.showPopup();
    }
    function esconderInfo() {
        var ballonPop = $find('popInfo');
        ballonPop.hidePopup();
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