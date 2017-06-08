<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.Users_Edit_Address"
    Title="Untitled Page" CodeBehind="Users_Edit_Address.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="people/customer" CurrentUrl="people/users_addressbook" runat="server" />

	 <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" runat="server" resourcekey="Close" CssClass="hcTertiaryAction" NavigateUrl="Default.aspx" />
            </div>
        </div>
	</div>

</asp:Content>

<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="Server">

    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="Message" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcFormMessage hcAddressMessage" style="display: none"></div>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("Nickname") %></label>
                <asp:TextBox runat="server" ID="txtNickName" width="100%"/>
                <asp:RequiredFieldValidator runat="server" ID="rfvNickName" resourcekey="rfvNickName" CssClass="hcFormError" Display="Dynamic" ControlToValidate="txtNickName" />
            </div>
            <div class="hcAddress">
                <hcc:AddressEditor ID="ucAddressEditor" ShowAddressLine3="false" CreateValidationInputs="True"
                    FormSelector=".hcAddress" ErrorMessageSelector=".hcAddressMessage" runat="server" />
            </div>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" runat="server" CssClass="hcPrimaryAction" resourcekey="SaveChanges" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" resourcekey="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>

    <hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" runat="server" />

    <script type="text/javascript">
        $(function () {
            var AddressValidator = createAddressValidator();
            AddressValidator.init($('.hcAddress').data('address-validation-inputs'), "#<%= btnSave.ClientID%>");
        });
    </script>
</asp:Content>
