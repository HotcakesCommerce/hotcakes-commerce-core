<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Affiliates_Edit" Title="Untitled Page" CodeBehind="Affiliates_Edit.aspx.cs" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" BaseUrl="reports/" ID="NavMenu" />

    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkDnnUserProfile" Visible="false" CssClass="hcTertiaryAction" resourcekey="ViewUserProfile" Target="_blank" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="text/javascript">
        hcAttachUpdatePanelLoader();
    </script>
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft hcRightBorder" style="width: 50%">
        <div class="hcForm">
            <div class="hcClear"></div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkApproved" resourcekey="Approved" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkEnabled" resourcekey="Enabled" runat="server" />
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("Username") %></label>
                <asp:TextBox ID="txtUsername" runat="server" />
                <asp:RequiredFieldValidator runat="server" resourcekey="rfvUsername" ControlToValidate="txtUsername" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("Email") %></label>
                <asp:TextBox ID="txtEmail" runat="server" />
                <asp:RequiredFieldValidator runat="server" resourcekey="rfvEmail" ControlToValidate="txtEmail" CssClass="hcFormError" />
                <asp:RegularExpressionValidator resourcekey="revEmail" ControlToValidate="txtEmail" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="hcFormError" />
            </div>
            <div class="hcClear"></div>
            <div id="divPassword" runat="server" class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("Password") %></label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" resourcekey="rfvPassword" ControlToValidate="txtPassword" CssClass="hcFormError" />
            </div>
            <div id="divPassword2" runat="server" class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("Confirmation") %></label>
                <asp:TextBox ID="txtPasswordConfirmation" runat="server" TextMode="Password" />
                <asp:CompareValidator runat="server" resourcekey="cvPasswordConfirmation" ControlToValidate="txtPasswordConfirmation" ControlToCompare="txtPassword" CssClass="hcFormError" />
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("AffiliateID") %></label>
                <asp:TextBox ID="txtAffiliateID" runat="server" />
                <asp:RequiredFieldValidator resourcekey="rfvAffiliateID" ControlToValidate="txtAffiliateID" runat="server" CssClass="hcFormError" />
                <asp:RegularExpressionValidator ID="valAffiliateID" resourcekey="valAffiliateID" ControlToValidate="txtAffiliateID" runat="server" ValidationExpression="\w*" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("ReferralAffiliateID") %></label>
                <asp:TextBox ID="txtReferralId" runat="server" />
                <asp:RegularExpressionValidator ID="valReferralID" resoucekey="valReferralID" ControlToValidate="txtReferralId" runat="server" ValidationExpression="\w*" CssClass="hcFormError" />
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItemLabel">
                <label class="hcLabel"><%=Localization.GetString("Commission") %></label>
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:TextBox ID="CommissionAmountField" runat="server" CssClass="RadComboBox" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:DropDownList ID="lstCommissionType" runat="server" CssClass="RadComboBox">
                    <asp:ListItem Value="1" resourcekey="PercentageOfSale" />
                    <asp:ListItem Value="2" resourcekey="FlatRateCommission" />
                </asp:DropDownList>
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItem hcFormItem50p">
                <asp:Label runat="server" AssociatedControlID="txtReferralDays" CssClass="hcLabel">
                    <%=Localization.GetString("DefaultReferralDays") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("DefaultReferralDaysHelp") %></span>
                    </i>
                </asp:Label>
                <asp:TextBox ID="txtReferralDays" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("TaxID") %></label>
                <asp:TextBox ID="TaxIdField" runat="server" />
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("DriversLicense") %></label>
                <asp:TextBox ID="DriversLicenseField" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel"><%=Localization.GetString("WebsiteUrl") %></label>
                <asp:TextBox ID="WebsiteUrlField" runat="server" />
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Notes") %></label>
                <asp:TextBox ID="NotesTextBox" runat="server" Rows="4" TextMode="MultiLine" CssClass="RadComboBox" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("SampleURL") %></label>
                <asp:Label ID="SampleUrlLabel" runat="server"/>
            </div>
        </div>
    </div>

    <div class="hcColumnRight" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Address") %></h2>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <uc:AddressEditor ID="ucAddress" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" runat="server" resourcekey="SaveChanges" CssClass="hcPrimaryAction" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" resourcekey="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>

</asp:Content>