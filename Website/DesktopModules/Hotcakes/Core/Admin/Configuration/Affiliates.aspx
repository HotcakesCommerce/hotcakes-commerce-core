<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Affiliates" Title="Untitled Page" CodeBehind="Affiliates.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft hcRightBorder" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItem hcFormItemLabel">
                <asp:Label runat="server" resourcekey="DefaultCommission" AssociatedControlID="lstCommissionType" CssClass="hcLabel" />
            </div>
            <div class="hcFormItem hcFormItem66p">
                <asp:DropDownList ID="lstCommissionType" runat="server">
                    <asp:ListItem Value="1" resourcekey="PercentageOfSale"/>
                    <asp:ListItem Value="2" resourcekey="FlatRateCommission"/>
                </asp:DropDownList>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="txtCommissionAmount" runat="server" MaxLength="10" />
                <asp:RequiredFieldValidator resourcekey="rfvCommissionAmount" ControlToValidate="txtCommissionAmount" Display="Dynamic" CssClass="hcFormError" runat="server" />
				<asp:CustomValidator id="cvCommissionAmount" resourcekey="cvCommissionAmount" runat="server"  OnServerValidate="ValidateCommission"  ControlToValidate="txtCommissionAmount" CssClass="hcFormError" ></asp:CustomValidator>
            </div>
            <div class="hcFormItem hcFormItemLabel">
                <asp:Label runat="server" AssociatedControlID="txtReferralDays" CssClass="hcLabel">
                    <%=Localization.GetString("DefaultReferralDays") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("DefaultReferralDaysHelp") %></span>
                    </i>
                </asp:Label>
                <asp:TextBox ID="txtReferralDays" runat="server" MaxLength="4" />
                <asp:RequiredFieldValidator resourcekey="rfvReferralDays" ControlToValidate="txtReferralDays" Display="Dynamic" CssClass="hcFormError" runat="server" />
                <asp:CompareValidator resourcekey="cvReferralDays" ControlToValidate="txtReferralDays" Display="Dynamic" CssClass="hcFormError"
                    Type="Integer" ValueToCompare="0" Operator="GreaterThanEqual" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label runat="server" resourcekey="ConflictResolutionMode" AssociatedControlID="lstConflictMode" CssClass="hcLabel" />
                <asp:DropDownList ID="lstConflictMode" runat="server">
                    <asp:ListItem Value="1" resourcekey="FavorOlderAffiliate"/>
                    <asp:ListItem Value="2" resourcekey="FavorNewerAffiliate"/>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="hcColumnRight" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkRequireApproval" resourcekey="chkRequireApproval" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkDisplayChildren" resourcekey="chkDisplayChildren" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkShowIDOnCheckout" resourcekey="chkShowIDOnCheckout" runat="server" />
            </div>
			<div class="hcFormItem">
                <asp:CheckBox ID="chkAffiliateNotify" resourcekey="chkAffiliateNotify" runat="server" />
            </div>
        </div>
    </div>
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Label runat="server" resourcekey="AffiliateAgreement" AssociatedControlID="txtAgreementText" CssClass="hcLabel" />
            <hcc:HtmlEditor ID="txtAgreementText" runat="server" EditorHeight="200" EditorWidth="1340" EditorWrap="true" />
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CausesValidation="true" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
        </li>
    </ul>

</asp:Content>