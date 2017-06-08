<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ProductReviews" Title="Untitled Page" CodeBehind="ProductReviews.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcForm">
        <div class="hcFormItemHor">
            <label class="hcLabel hcBlockContent20p"><%=Localization.GetString("ModerateProductReviews") %></label>
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkProductReviewModerate" runat="server"/>
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel hcBlockContent20p"><%=Localization.GetString("AllowProductRating") %></label>
            <div class="hcCheckboxOuter">
                <asp:CheckBox ID="chkAllowProductReviews" runat="server"/>
                <span></span>
            </div>
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel hcBlockContent20p"><%=Localization.GetString("ShowHowManyReviews") %></label>
            <asp:TextBox ID="txtProductReviewCount" runat="server" CssClass="FormInput" Text="3"/>
            <asp:CompareValidator id="cvProductReviewCount" 
                ControlToValidate="txtProductReviewCount"
                Operator="GreaterThan"
                ValueToCompare="0"
                Type="Integer"
                CssClass="hcFormError"
                ValidationGroup="ProductReview"
                runat="server" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" ValidationGroup="ProductReview"
                OnClick="btnSave_Click"
                CssClass="hcPrimaryAction"
                runat="server" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="False"
                OnClick="btnCancel_Click"
                CssClass="hcSecondaryAction"
                runat="server" />
        </li>
    </ul>
</asp:Content>

