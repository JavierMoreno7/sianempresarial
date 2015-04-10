<%@ Page Language="C#" MasterPageFile="~/master/MasterPage.master" AutoEventWireup="true" CodeFile="ProcesoCatalogo.aspx.cs" Inherits="conge_procesos_Test" Title="SIAN - Sistema de Control de Inventarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br />
    <asp:Panel ID="Panel1" defaultbutton="btnBuscar" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" style="width: 808px;">
        <tr>
            <td class="GridFormat">
                Catalogo
                <asp:Label ID="lblCatalogo" runat="server" ForeColor="White"></asp:Label>
                - Listado de Datos</td>
            <td class="GridFormat">
            </td>
            <td class="GridFormat">
            </td>
        </tr>
        <tr>
            <td style="width: 569px; height: 18px;">
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Error" Visible="False" Font-Size="X-Small"></asp:Label></td>
            <td style="width: 108px; height: 18px;">
            </td>
            <td style="width: 109px; height: 18px;">
            </td>
        </tr>
        <tr>
            <td style="width: 569px; height: 36px; font-size: 10pt;">
                Buscar por:
                <asp:DropDownList ID="lblCampo" runat="server" CssClass="SelectFormat">
                </asp:DropDownList>&nbsp;
                <asp:TextBox ID="txtCriterio" runat="server" Width="146px" CssClass="TextInputFormat"></asp:TextBox>&nbsp;<asp:ImageButton
                    ID="btnBuscar" runat="server" CssClass="ButtonFormat" Height="17px" ImageUrl="~/imagenes/dn.gif"
                    OnClick="btnBuscar_Click" ToolTip="Buscar" Width="19px" /></td>
            <td style="width: 108px; height: 36px">
                <asp:LinkButton ID="lbtnMostrarTodos" runat="server" visible="False" OnClick="lbtnMostrarTodos_Click" CssClass="tb" Width="139px">Todos los Registros</asp:LinkButton></td>
            <td style="width: 109px; height: 36px; text-align: left;">
                <asp:LinkButton ID="lbtnAgregar" runat="server" OnClick="lbtnAgregar_Click" CssClass="tb" Width="102px"></asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center; vertical-align: top;">
                <asp:GridView ID="grdvLista" runat="server" SkinID="grdSIAN" AllowPaging="True" Height="1px" SelectedIndex="0" Width="799px" AllowSorting="True" CellPadding="0" OnSelectedIndexChanged="grdvLista_SelectedIndexChanged" OnPageIndexChanging="grdvLista_PageIndexChanging" HorizontalAlign="Left" OnSorting="grdvLista_Sorting" EnableViewState="False" EnableTheming="True" PageSize="30" UseAccessibleHeader="False" CaptionAlign="Top" GridLines="None">

</asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <asp:label id="lblPagina" runat="server" CssClass="tb"/><asp:label id="lblPaginatot" runat="server" CssClass="tb" />
            </td>
        </tr>
    </table>
    </asp:Panel>
</asp:Content>