<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Promotions_Edit_Qualification.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Promotions_Edit_Qualification" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="Qualifications/AffiliateApprovedEditor.ascx" TagPrefix="hcc" TagName="AffiliateApprovedEditor" %>
<%@ Register Src="Qualifications/ProductIsEditor.ascx" TagPrefix="hcc" TagName="ProductIsEditor" %>
<%@ Register Src="Qualifications/LineItemCategoryEditor.ascx" TagPrefix="hcc" TagName="LineItemCategoryEditor" %>
<%@ Register Src="Qualifications/OrderHasProductEditor.ascx" TagPrefix="hcc" TagName="OrderHasProductEditor" %>
<%@ Register Src="Qualifications/OrderSubTotalIsEditor.ascx" TagPrefix="hcc" TagName="OrderSubTotalIsEditor" %>
<%@ Register Src="Qualifications/ProductTypeEditor.ascx" TagPrefix="hcc" TagName="ProductTypeEditor" %>
<%@ Register Src="Qualifications/ProductCategoryEditor.ascx" TagPrefix="hcc" TagName="ProductCategoryEditor" %>
<%@ Register Src="Qualifications/OrderHasCouponEditor.ascx" TagPrefix="hcc" TagName="OrderHasCouponEditor" %>
<%@ Register Src="Qualifications/UserIdEditor.ascx" TagPrefix="hcc" TagName="UserIdEditor" %>
<%@ Register Src="Qualifications/UserIsInGroupEditor.ascx" TagPrefix="hcc" TagName="UserIsInGroupEditor" %>
<%@ Register Src="Qualifications/UserIsInRoleEditor.ascx" TagPrefix="hcc" TagName="UserIsInRoleEditor" %>
<%@ Register Src="Qualifications/ShippingMethodIsEditor.ascx" TagPrefix="hcc" TagName="ShippingMethodIsEditor" %>
<%@ Register Src="Qualifications/ProductsSumCountEditor.ascx" TagPrefix="hcc" TagName="ProductsSumCountEditor" %>
<%@ Register Src="Qualifications/VendorOrManufacturerEditor.ascx" TagPrefix="hcc" TagName="VendorOrManufacturerEditor" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" />
<asp:MultiView ID="mvQualifications" runat="server">
    <asp:View ID="viewLineItemCategory" runat="server">
        <hcc:LineItemCategoryEditor runat="server" ID="ucLineItemCategoryEditor" />
    </asp:View>
    <asp:View ID="viewProductBvin" runat="server">
        <hcc:ProductIsEditor runat="server" ID="ucProductBvinEditor" />
    </asp:View>
    <asp:View ID="viewOrderHasProduct" runat="server">
        <hcc:OrderHasProductEditor runat="server" ID="ucOrderHasProductEditor" />
    </asp:View>
    <asp:View ID="viewOrderSubTotalIs" runat="server">
        <hcc:OrderSubTotalIsEditor runat="server" ID="ucOrderSubTotalIsEditor" />
    </asp:View>
    <asp:View ID="viewProductType" runat="server">
        <hcc:ProductTypeEditor runat="server" ID="ucProductTypeEditor" IsNotMode="true"/>
    </asp:View>
    <asp:View ID="viewProductCategory" runat="server">
        <hcc:ProductCategoryEditor runat="server" ID="ucProductCategoryEditor" />
    </asp:View>
    <asp:View ID="viewOrderHasCoupon" runat="server">
        <hcc:OrderHasCouponEditor runat="server" ID="ucOrderHasCouponEditor" />
    </asp:View>
    <asp:View ID="viewUserId" runat="server">
        <hcc:UserIdEditor runat="server" ID="ucUserIdEditor" />
    </asp:View>
    <asp:View ID="viewUserIsInGroup" runat="server">
        <hcc:UserIsInGroupEditor runat="server" ID="ucUserIsInGroupEditor" />
    </asp:View>
    <asp:View ID="viewUserIsInRole" runat="server">
        <hcc:UserIsInRoleEditor runat="server" ID="ucUserIsInRoleEditor" />
    </asp:View>
    <asp:View ID="viewShippingMethodIs" runat="server">
        <hcc:ShippingMethodIsEditor runat="server" ID="ucShippingMethodIsEditor" />
    </asp:View>
    <asp:View ID="viewAffiliateApproved" runat="server">
        <hcc:AffiliateApprovedEditor runat="server" ID="ucAffiliateApproved" />
    </asp:View>
    <asp:View ID="viewLineItemIs" runat="server">
        <hcc:ProductIsEditor runat="server" ID="ucProductIsEditor" Title="WhenLineItemIs" />
    </asp:View>
    <asp:View ID="viewProductsSumCount" runat="server">
        <hcc:ProductsSumCountEditor runat="server" ID="ucProductsSumCountEditor" />
    </asp:View>
    <asp:View ID="viewProductTypeIsNot" runat="server">
        <hcc:ProductTypeEditor runat="server" ID="ucProductTypeIsNot" IsNotMode="true" />
    </asp:View>
    <asp:View ID="viewProductIsNot" runat="server">
        <hcc:OrderHasProductEditor runat="server" ID="ucProductIsNot" IsNotMode="true" />
    </asp:View>
    <asp:View ID="viewVendorManufacturer" runat="server">
        <hcc:VendorOrManufacturerEditor runat="server" ID="ucVendorManufacturer" IsNotMode="true" />
    </asp:View>
</asp:MultiView>