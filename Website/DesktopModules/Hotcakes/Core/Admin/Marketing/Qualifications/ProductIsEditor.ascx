<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductIsEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.ProductIsEditor" %>
<%@ Register Src="../../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>

<h1><%=Title %></h1>
<table border="0">
    <tr>
        <td style="vertical-align: top; width: 45%">
            <hcc:ProductPicker id="ucProductPicker" runat="server" />
        </td>
        <td>
            <asp:LinkButton ID="btnAddProduct" runat="server" CssClass="hcSecondaryAction" Text=">>" />
        </td>
        <td style="vertical-align: top;">
            <asp:GridView ID="gvProductBvins" runat="server" AutoGenerateColumns="False" DataKeyNames="Bvin" CssClass="hcGrid" ShowHeader="False">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:TemplateField >
                        <ItemTemplate>
                            [<%#Eval("Sku") %>] <%#Eval("ProductName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CausesValidation="False" CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoProductsAdded") %>
                </EmptyDataTemplate>
            </asp:GridView>
        </td>
    </tr>
</table>
