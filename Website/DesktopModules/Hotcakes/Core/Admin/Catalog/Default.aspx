<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Default" Title="Hotcakes Admin" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/SimpleProductFilter.ascx" TagName="SimpleProductFilter" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/Pager.ascx" TagName="Pager" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">

    <hcc:NavMenu ID="ucNavMenu" runat="server" />

    <div class="hcBlock hcBlockLight">
        <hcc:SimpleProductFilter ID="ucSimpleProductFilter" runat="server" />
    </div>

    <div class="hcBlock hcNavContentBottom">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink NavigateUrl="Products_Edit.aspx" Text="+ Add New Product" CssClass="hcTertiaryAction" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkImport" CssClass="hcTertiaryAction" NavigateUrl="Products_Import.aspx" Text="Import from Excel" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="lnkExport" CssClass="hcTertiaryAction" Text="Export to Excel" Visible="true" runat="server" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <script>
        $(function (jQuery) {
            $(".hcExports a").on("click", function () {
                $(this).closest("li").remove();
                if (!$(".hcExports ul").children().length)
                    $(".hcExports").hide();
            })
        });
    </script>
    <h1><%=PageTitle %></h1>

    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <asp:Panel ID="pnlSamples" runat="server" Visible="false" CssClass="hcFormMessage hcFormInfo">
        You don't have any products in your store right now. Either
        <asp:HyperLink NavigateUrl="Products_Edit.aspx" CssClass="hcButton hcSmall" Text="+ Add New Product" runat="server" />
        or
        <asp:LinkButton ID="lnkAddSamples" runat="server" CssClass="hcButton hcSmall" Text="Add Sample Products and Categories to the Store" OnClick="lnkAddSamples_Click" />
        <br />
        <br />
        Please Note: Adding sample products/categories will also add sample orders, customers, and analytics to help you visualize store management in a populated store. <br />
        This data will be removed when you remove the sample products/categories later.
    </asp:Panel>
    <asp:Panel ID="pnlExports" runat="server" Visible="false" CssClass="hcExports hcFormMessage hcFormInfo">
        <asp:Label runat="server" resourcekey="ExportedFilesExists" />
        <asp:Repeater runat="server" ID="rptExportedFiles">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <asp:LinkButton runat="server" ID="btnDownloadExportedFile" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FullName") %>' OnCommand="btnDownloadExportedFile_Command" />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <asp:Label ID="lblNoRecordsMessage" CssClass="hcInfoLabelLeft" Visible="false" runat="server">
        No Products Where Found
    </asp:Label>

    <asp:Panel ID="pnlMain" runat="server">
        <hcc:Pager ID="ucPager" PageSize="50" LinkedPagerID="ucPager2" runat="server" />
        <div class="hcInfoLabel"><%=RowCount %> Products Found</div>

        <asp:GridView ID="gvProducts" DataKeyNames="Bvin" CssClass="hcGrid" AutoGenerateColumns="false" runat="server">
            <HeaderStyle CssClass="hcGridHeader" />
            <RowStyle CssClass="hcGridRow" />
            <Columns>
                <asp:TemplateField ItemStyle-CssClass="hcNoPadding">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkImage" runat="server">
                            <asp:Image runat="server" Width="80px" />
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ProductName" HeaderText="Name" />
                <asp:BoundField DataField="Sku" HeaderText="SKU" />
                <asp:TemplateField HeaderText="Price">
                    <ItemTemplate>
                        <%#GetProductPrice(Container) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle Width="80px" />
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="lnkEdit" CssClass="hcIconEdit" Text="Edit" />

                        <asp:LinkButton OnClientClick="return hcConfirm(event, 'Delete this product?');"
                            runat="server" CssClass="hcIconDelete"
                            CausesValidation="False" CommandName="Delete" Text="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <hcc:Pager ID="ucPager2" runat="server" />
        <ul class="hcActions">
            <li>
                <asp:LinkButton runat="server" ID="btnRemoveSamples" CssClass="hcButton"
                    Text="Remove Sample Products and Categories" OnClick="btnRemoveSamples_Click" />
            </li>
        </ul>
    </asp:Panel>
</asp:Content>
