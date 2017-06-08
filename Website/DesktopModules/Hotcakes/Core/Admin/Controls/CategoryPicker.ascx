<%@ Control Language="C#" AutoEventWireup="True"
    Inherits="Hotcakes.Modules.Core.Admin.Controls.CategoryPicker" CodeBehind="CategoryPicker.ascx.cs" %>

<div class="hcForm">
    <asp:Panel CssClass="hcTableIframe" runat="server">
        <asp:GridView ID="CategoriesGridView" runat="server" AutoGenerateColumns="False"
            DataKeyNames="bvin" CssClass="hcGrid">
            <RowStyle CssClass="hcGridRow" />
            <HeaderStyle CssClass="hcGridHeader" />
            <Columns>
                <asp:TemplateField>
                    <ItemStyle Width="22px" />
                    <ItemTemplate>
                        <div class="hcCheckboxOuter">
                            <asp:CheckBox ID="chkSelected" Enabled='<%#IsEnabled(Container) %>' runat="server" />
                            <span></span>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Text" HeaderText="All Categories" />
                <asp:TemplateField>
                    <ItemStyle Width="22px" />
                    <ItemTemplate>
                        <asp:LinkButton Text="Add" CommandName="Add" CommandArgument='<%#Container.DataItemIndex %>' CssClass="hcIconRight"
                            Visible='<%#IsEnabled(Container) %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <div class="hcFormItem">
        <asp:LinkButton ID="lnkAddSelected" Text="Add Selected >>" CssClass="hcButton" runat="server" />
    </div>
</div>
