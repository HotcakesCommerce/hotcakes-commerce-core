<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReceiveFreeProductEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Actions.ReceiveFreeProduct" %>
<%@ Register Src="../../Controls/ProductPicker.ascx" TagName="ProductPicker" TagPrefix="hcc" %>
<h1><%=Localization.GetString("ReceiveTheseProducts") %></h1>
<table class="hc-popup-table" hc-popup-width="700" border="0">
    <tr>
        <td style="vertical-align: top; width: 45%">
            <hcc:ProductPicker id="ProductPickerOrderProducts" runat="server" DisplayGiftCards="false"  DisplayProductWithChoice="false"/>
        </td>
        <td>
            <asp:LinkButton ID="btnAddProduct" runat="server"
                Text=">>" CssClass="hcSecondaryAction" OnClientClick="setPopupHeightForAction();" OnClick="btnAddProduct_Click" />
        </td>
        <td style="vertical-align: top;">
            <%=Localization.GetString("TheFollowingProducts") %>
            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False"
                DataKeyNames="Bvin" CssClass="hcGrid" OnRowDeleting="gvProducts_RowDeleting"
                ShowHeader="False" AlternatingRowStyle-Wrap="true"  RowStyle-Wrap="true">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundField DataField="DisplayName" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:TextBox runat="server" ID="txtQuantity" CssClass="hcTxtQuantity" width="50px" ></asp:TextBox>
                            <asp:CompareValidator runat="server" ID="cvCompare" Operator="GreaterThan" Text="*" ValueToCompare="0" ControlToValidate="txtQuantity" Type="Integer"></asp:CompareValidator>
							<asp:RequiredFieldValidator runat="server" ID="rvQuantity" ControlToValidate="txtQuantity" Text="*" CssClass="hcError"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteProduct" runat="server" CausesValidation="False"
                                CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteProduct_OnPreRender" />
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
