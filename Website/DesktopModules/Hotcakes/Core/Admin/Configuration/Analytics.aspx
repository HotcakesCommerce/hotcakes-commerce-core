<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Analytics" Title="Untitled Page" CodeBehind="Analytics.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2><%=Localization.GetString("GoogleSection") %></h2>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkGoogleTracker" resourcekey="UseGoogleTracking" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="GoogleTrackingId" runat="server" />
                <asp:TextBox ID="GoogleTrackingIdField" runat="server" />
            </div>
            <div class="hcFormItem">
                <hr />
                <asp:CheckBox ID="chkGoogleAdwords" resourcekey="UseGoogleAdwordsConversion" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="AdwordsConversionId" runat="server" />
                <asp:TextBox ID="GoogleAdwordsConversionIdField" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="AdwordsNotificationLayout" runat="server" />
                <asp:DropDownList runat="server" ID="ddlAdwordsFormat">
                    <asp:ListItem resourcekey="SingleLine" Value="1" />
                    <asp:ListItem resourcekey="TwoLines" Value="2" />
                    <asp:ListItem resourcekey="DontAddNotification" Value="3" />
                </asp:DropDownList>
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="AdwordsLabel" runat="server" />
                <asp:TextBox ID="GoogleAdwordsLabelField" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="AdwordsBackgroundColor" runat="server" />
                <asp:TextBox ID="GoogleAdwordsBackgroundColorField" runat="server" />
            </div>
            <div class="hcFormItem">
                <hr />
                <asp:CheckBox ID="chkGoogleEcommerce" resourcekey="UseGoogleEcommerceTracking" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="GoogleStoreName" runat="server" />
                <asp:TextBox ID="GoogleEcommerceStoreNameField" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="GoogleCategoryName" runat="server" />
                <asp:TextBox ID="GoogleEcommerceCategoryNameField" runat="server" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("YahooAndOtherSection") %></h2>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkYahoo" resourcekey="UseYahooSalesTracking" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="YahooAccountId" runat="server" />
                <asp:TextBox ID="YahooAccountIdField" runat="server" />
            </div>
            <div class="hcFormItem">
                <hr />
                <asp:CheckBox ID="chkUseShopZillaSurvey" resourcekey="UseShopZillaSurvey" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="ShopZillaMID" runat="server" />
                <asp:TextBox ID="ShopZillaIdField" runat="server" />
            </div>
            <div class="hcFormItem">
                <hr />
                <asp:Label CssClass="hcLabel" resourcekey="OtherMetaTags" runat="server" />
                <asp:TextBox ID="AdditionalMetaTagsField" TextMode="MultiLine" Wrap="false" Rows="4" runat="server" CssClass="RadComboBox" />
                <small><%=Localization.GetString("MetaTagsTip") %><br />
                    &lt;meta name=&quot;google-site-verification&quot; content=&quot;FqmYcwmgR326V7S3v4oEhxQsacwmgR32jsIgFw&quot; /&gt;</small>
            </div>
            <div class="hcFormItem">
                <asp:Label CssClass="hcLabel" resourcekey="BottomPageTags" runat="server" />
                <asp:TextBox ID="BottomAnalyticsField" TextMode="MultiLine" Wrap="false" Rows="4" runat="server" CssClass="RadComboBox" />
                <small class="smalltext"><%=string.Format(Localization.GetString("PageTagsTip"), "&lt;/body&gt;") %></small>
            </div>
        </div>
    </div>
    <div class="hcForm">
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" Text="Save Changes" runat="server" OnClick="btnSave_Click" CssClass="hcPrimaryAction" />
        </li>
    </ul>

</asp:Content>