<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Promotions_Edit_Actions.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Promotions_Edit_Actions" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="Actions/AdjustProductPriceEditor.ascx" TagPrefix="hcc" TagName="AdjustProductPrice" %>
<%@ Register Src="Actions/AdjustRewardPointsEditor.ascx" TagPrefix="hcc" TagName="AdjustRewardPoints" %>
<%@ Register Src="Actions/LineItemFreeShippingEditor.ascx" TagPrefix="hcc" TagName="LineItemFreeShipping" %>
<%@ Register Src="Actions/AdjustOrderTotalEditor.ascx" TagPrefix="hcc" TagName="AdjustOrderTotalEditor" %>
<%@ Register Src="Actions/OrderShippingAdjustmentEditor.ascx" TagPrefix="hcc" TagName="OrderShippingAdjustmentEditor" %>
<%@ Register Src="Actions/AdjustLineItemEditor.ascx" TagPrefix="hcc" TagName="AdjustLineItemEditor" %>
<%@ Register Src="Actions/ReceiveFreeProductEditor.ascx" TagPrefix="hcc" TagName="ReceiveFreeProductEditor" %>
<%@ Register Src="Actions/CategoryDiscountEditor.ascx" TagPrefix="hcc" TagName="CategoryDiscountEditor" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" />

<asp:MultiView ID="mvActions" runat="server">
    <asp:View ID="viewLineItemFreeShipping" runat="server">
        <hcc:LineItemFreeShipping runat="server" ID="ucActionLineItemFreeShipping" />
    </asp:View>
    <asp:View ID="viewAdjustProductPrice" runat="server">
        <hcc:AdjustProductPrice runat="server" ID="ucActionAdjustProductPrice" />
    </asp:View>
    <asp:View ID="viewAdjustOrderTotal" runat="server">
        <hcc:AdjustOrderTotalEditor runat="server" ID="ucActionAdjustOrderTotal" />
    </asp:View>
    <asp:View ID="viewAdjustLineItem" runat="server">
        <hcc:AdjustLineItemEditor runat="server" ID="ucActionAdjustLineItem" />
    </asp:View>
    <asp:View ID="viewOrderShippingAdjustment" runat="server">
        <hcc:OrderShippingAdjustmentEditor runat="server" ID="ucActionOrderShippingAdjustment" />
    </asp:View>
    <asp:View ID="viewAdjustRewardPoints" runat="server">
        <hcc:AdjustRewardPoints runat="server" ID="ucActionAdjustRewardPoints" />
    </asp:View>
    <asp:View ID="viewReceiveFreeProduct" runat="server">
        <hcc:ReceiveFreeProductEditor runat="server" ID="ucReceiveFreeProductEditor" />
    </asp:View>
    <asp:View ID="viewCategoryDiscount" runat="server">
        <hcc:CategoryDiscountEditor runat="server" ID="ucCategoryDiscountEditor" />
    </asp:View>
</asp:MultiView>
