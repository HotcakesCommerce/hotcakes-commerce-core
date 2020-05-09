<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Promotions.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Promotions" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="Promotions_List.ascx" TagPrefix="hcc" TagName="Promotions_List" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" />

    <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnGo" CssClass="hcBlock hcBlockLight hcPaddingBottom">
        <div class="hcForm">
            <div class="hcFormItem hcGo">
                <label class="hcLabel"><%=Localization.GetString("SearchByKeywords") %></label>
                <div class="hcFieldOuter">
                    <asp:TextBox ID="txtKeywords" runat="server" />
                    <asp:LinkButton ID="btnGo" runat="server" AlternateText="Filter Results" CssClass="hcIconRight" />
                </div>
            </div>
            <div class="hcFormItem hcSmall">
                <asp:CheckBox ID="chkShowDisabled" AutoPostBack="true" runat="server" resourcekey="chkShowDisabled" />
            </div>
        </div>
    </asp:Panel>

    <div class="hcBlock hcBlockLight hcPaddingBottom">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("NewPromotion") %></label>
                <asp:DropDownList ID="lstNewType" runat="server" />
            </div>
        </div>
    </div>

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:LinkButton ID="btnNew" runat="server" resourcekey="btnNew" CssClass="hcTertiaryAction" OnClick="btnNew_Click" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<asp:UpdatePanel ID="uplPromotionsList" runat="server" UpdateMode="Always">
		<ContentTemplate>
			<h1>Promotions</h1>
			<asp:LinkButton ID="lnkMigrate" Text="Migrate" runat="server" Visible="false" CssClass="hcSecondaryAction" />
			<hcc:MessageBox ID="ucMessageBox" runat="server" />
			<hcc:Promotions_List runat="server" ID="ucSalesList" Mode="Sale" />
            <hcc:Promotions_List runat="server" ID="ucOffersFreeItem" Mode="OfferForFreeItems" />
			<hcc:Promotions_List runat="server" ID="ucOffersOrderItems" Mode="OfferForLineItems" />
			<hcc:Promotions_List runat="server" ID="ucOffersOrderSubTotal" Mode="OfferForOrder" />
			<hcc:Promotions_List runat="server" ID="ucOffersOrderShipping" Mode="OfferForShipping" />
			<hcc:Promotions_List runat="server" ID="ucAffiliatePromotions" Mode="Affiliate" />         
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
