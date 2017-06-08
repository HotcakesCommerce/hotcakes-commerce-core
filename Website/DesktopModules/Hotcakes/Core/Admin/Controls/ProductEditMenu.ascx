<%@ Control Language="C#" AutoEventWireup="True"
    Inherits="Hotcakes.Modules.Core.Admin.Controls.ProductEditMenu" CodeBehind="ProductEditMenu.ascx.cs" %>

<ul class="hcNavMenu">
	<li <%=GetCurrentCssClass(MenuItemType.Performance) %>>
        <asp:HyperLink ID="hypPerformance" runat="server" Text="Product Performance" NavigateUrl="../Catalog/Products_Performance.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.General) %>>
        <asp:HyperLink ID="hypGeneral" runat="server" Text="Name, Description, Price" NavigateUrl="../Catalog/Products_Edit.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.AdditionalImages) %>>
        <asp:HyperLink ID="hypAdditionalImages" runat="server" Text="Additional Images" NavigateUrl="../Catalog/Products_Edit_Images.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.Categories) %>>
        <asp:HyperLink ID="hypCategories" runat="server" Text="Categories" NavigateUrl="../Catalog/Products_Edit_Categories.aspx" />
    </li>
    <asp:PlaceHolder runat="server" ID="liCustomerChoices">
        <li <%=GetCurrentCssClass(MenuItemType.CustomerChoices) %>>
            <asp:HyperLink ID="hypCustomerChoices" runat="server" Text="Choices - Edit" NavigateUrl="../Catalog/ProductChoices.aspx" />
        </li>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="liVariants">
        <li <%=GetCurrentCssClass(MenuItemType.Variants) %>>
            <asp:HyperLink ID="hypVariants" runat="server" Text="Choices - Variants" NavigateUrl="../Catalog/ProductChoices_Variants.aspx" />
        </li>
    </asp:PlaceHolder>
    <li <%=GetCurrentCssClass(MenuItemType.Files) %>>
        <asp:HyperLink ID="hypFiles" runat="server" Text="File Downloads" NavigateUrl="../Catalog/FileDownloads.aspx" />
    </li>
    <asp:PlaceHolder runat="server" ID="liInventory">
        <li <%=GetCurrentCssClass(MenuItemType.Inventory) %>>
            <asp:HyperLink ID="hypInventory" runat="server" Text="Inventory" NavigateUrl="../Catalog/Products_Edit_Inventory.aspx" />
        </li>
    </asp:PlaceHolder>
    <li <%=GetCurrentCssClass(MenuItemType.UpSellCrossSell) %>>
        <asp:HyperLink ID="hypUpSellCrossSell" runat="server" Text="Related Items" NavigateUrl="../Catalog/ProductUpSellCrossSell.aspx" />
    </li>
    <asp:PlaceHolder runat="server" ID="liBundledProducts">
        <li <%=GetCurrentCssClass(MenuItemType.BundledProducts) %>>
            <asp:HyperLink ID="hypBundledProducts" runat="server" Text="Bundled Products" NavigateUrl="../Catalog/BundledProducts.aspx" />
        </li>
    </asp:PlaceHolder>
    <li <%=GetCurrentCssClass(MenuItemType.InfoTabs) %>>
        <asp:HyperLink ID="hypInfoTabs" runat="server" Text="Info Tabs" NavigateUrl="../Catalog/ProductsEdit_Tabs.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.ProductReviews) %>>
        <asp:HyperLink ID="hypProductReviews" runat="server" Text="Reviews" NavigateUrl="../Catalog/Products_Edit_Reviews.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.VolumeDiscounts) %>>
        <asp:HyperLink ID="hypVolumeDiscounts" runat="server" Text="Volume Discounts" NavigateUrl="../Catalog/ProductVolumeDiscounts.aspx" />
    </li>
    <li <%=GetCurrentCssClass(MenuItemType.Roles) %>>
        <asp:HyperLink ID="hypRoles" runat="server" Text="Roles" NavigateUrl="../Catalog/Products_Edit_Roles.aspx" />
    </li>

</ul>

<div class="hcBlock hcBlockNotBottomPadding">
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="../Catalog/Default.aspx" />
        </div>
    </div>
</div>


