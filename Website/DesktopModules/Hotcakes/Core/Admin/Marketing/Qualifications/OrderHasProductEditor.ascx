<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderHasProductEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.OrderHasProductEditor" %>
<%@ Register Src="../../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>

<asp:Panel runat="server" ID="pnlHasHeader">
    <h1><%=Localization.GetString("WhenOrderHasProducts") %></h1>
</asp:Panel>
<asp:Panel runat="server" ID="pnlHasNotHeader">
    <h1><%=Localization.GetString("WhenOrderHasNotProducts") %></h1>
</asp:Panel>
<table border="0">
    <tr>
        <td style="vertical-align: top; width: 45%">
            <hcc:ProductPicker ID="ProductPickerOrderProducts" runat="server" />
        </td>
        <td>
            <asp:LinkButton ID="btnAddOrderProduct" runat="server"
                Text=">>" CssClass="hcSecondaryAction" OnClick="btnAddOrderProduct_Click" />
        </td>
        <td style="vertical-align: top;">
            <asp:Panel runat="server" ID="pnlHas">
                <%=Localization.GetString("WhenOrderHasAtLeast") %>
                <asp:TextBox ID="OrderProductQuantityField" runat="server" Columns="10" />
                <asp:RangeValidator ID="rvOrderProductQuantityField" runat="server" Type="Integer" MinimumValue="1" MaximumValue="9999" ControlToValidate="OrderProductQuantityField" CssClass="hcFormError" EnableClientScript="true" resourcekey="ErrorOrderProductQuantityFieldRange"></asp:RangeValidator>
                <asp:RequiredFieldValidator ID="rfvOrderProductQuantityField" runat="server" ControlToValidate="OrderProductQuantityField" Display="Dynamic" CssClass="hcFormError" EnableClientScript="true" resourcekey="ErrorOrderProductQuantityField" />
                <%=Localization.GetString("Of") %>
                <asp:DropDownList ID="lstOrderProductSetMode" runat="server" />
                <%=Localization.GetString("OfTheseProducts") %>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlHasNot">
                <%=Localization.GetString("WhenOrderHasNot") %>
            </asp:Panel>

            <asp:GridView ID="gvOrderProducts" runat="server" AutoGenerateColumns="False"
                DataKeyNames="Bvin" CssClass="hcGrid" OnRowDeleting="gvOrderProducts_RowDeleting"
                ShowHeader="False">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundField DataField="DisplayName" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteOrderProduct" runat="server" CausesValidation="False"
                                CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteOrderProduct_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <%=Localization.GetString("NoProductsAdded.Text") %>
                </EmptyDataTemplate>
            </asp:GridView>
        </td>
    </tr>
</table>
