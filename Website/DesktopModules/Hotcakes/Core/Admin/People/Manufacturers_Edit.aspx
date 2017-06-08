<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Manufacturers_Edit" Title="Untitled Page" CodeBehind="Manufacturers_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/UserPicker.ascx" TagName="UserPicker" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="people/" />

	 <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" resourcekey="Close" CssClass="hcTertiaryAction" NavigateUrl="manufacturers.aspx" />
            </div>
        </div>
	</div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="MessageBox1" runat="server" />
        
    <div class="hcFormMessage hcAddressMessage"></div>
    
    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Name") %></label>
                <asp:TextBox ID="DisplayNameField" runat="server" MaxLength="100" TabIndex="2000" Width="100%"/>
                <asp:RequiredFieldValidator ID="rfvDisplayNameField" resourcekey="rfvDisplayNameField" runat="server" Display="Dynamic" CssClass="hcFormError" ControlToValidate="DisplayNameField"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Email") %></label>
                <asp:TextBox ID="EmailField" runat="server" MaxLength="100" TabIndex="2001"  Width="100%"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("EmailTemplate") %></label>
                <asp:DropDownList ID="EmailTemplateDropDownList" runat="server"/>
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%; padding-left: 0.25em;">
        <div class="hcForm">
            <h2><%=Localization.GetString("Address") %></h2>
            <hcc:AddressEditor ID="AddressEditor1" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcAddress" ErrorMessageSelector=".hcAddressMessage" runat="server" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" CssClass="hcPrimaryAction" TabIndex="2500" runat="server" OnClick="btnSaveChanges_Click"/>
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" TabIndex="2501" runat="server" CausesValidation="False" OnClick="btnCancel_Click"/>
        </li>
    </ul>
    
    <asp:HiddenField ID="BvinField" runat="server" />

    <hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" runat="server" />

    <script type="text/javascript">
        $(function () {
            var AddressValidator = createAddressValidator();
            AddressValidator.init($('.hcAddress').data('address-validation-inputs'), "#<%= btnSaveChanges.ClientID%>");
        });
    </script>
</asp:Content>