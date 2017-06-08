<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.StoreInfo" Title="Untitled Page" CodeBehind="StoreInfo.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressEditor.ascx" TagName="AddressEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessage" runat="server" />
    <div class="hcFormMessage hcAddressMessage"></div>
    <div class="hcColumnLeft hcAddress" style="width: 50%">
        <hcc:AddressEditor ID="ucAddressEditor" ShowAddressLine3="false" CreateValidationInputs="True" FormSelector=".hcAddress" ErrorMessageSelector=".hcAddressMessage" runat="server" />
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
            </li>
        </ul>
    </div>
    <hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" runat="server" />
    <script type="text/javascript">
        $(function () {
            var AddressValidator = createAddressValidator();
            AddressValidator.init($('.hcAddress').data('address-validation-inputs'), "#<%= btnSave.ClientID%>");
        });
    </script>
</asp:Content>

