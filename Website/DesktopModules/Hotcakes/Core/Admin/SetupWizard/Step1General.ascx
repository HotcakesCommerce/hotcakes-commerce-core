<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Step1General.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.Step1General" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>
<%@ Register Src="../Controls/AddressNormalizationDialog.ascx" TagName="AddressNormalizationDialog" TagPrefix="hcc" %>

<div class="hcWizGeneral">
    <div class="hcForm">
        <div class="hcColumn">
            <div class="hcPicture left">
                <hcc:ImageUploader runat="server" ID="ucStoreLogo" />
            </div>
        </div>
        <div class="hcColumn">
            <div class="hcForm">
                <div class="hcFormItem hcStoreName">
                    <asp:Label resourcekey="StoreName" runat="server" AssociatedControlID="txtStoreName" CssClass="hcLabel" />
                    <asp:TextBox ID="txtStoreName" Columns="50" runat="server" />
                    <i class="hcIconInfo">
                        <span class="hcFormInfo hcSetupWizardStoreNameHelp"><%=Localization.GetString("StoreNameHelp") %></span>
                    </i>
                </div>
                <div class="hcFormItem">
                    <asp:CheckBox ID="chkUseSSL" resourcekey="chkUseSSL" CssClass="hcCheckbox" runat="server" />
                </div>
            </div>
        </div>
    </div>
    <div class="hcColumnLeft dnnClear" style="width: 33.2%">
        <div class="hcForm hcAddress">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <h2><%=Localization.GetString("StoreAddress") %></h2>
                    <div class="hcFormMessage hcAddressMessage"></div>
                    <div class="hcFormItem">
                        <asp:Label ID="lblCountry" resourcekey="lblCountry" AssociatedControlID="ddlCountries" runat="server" CssClass="hcLabel" />
                        <asp:DropDownList ID="ddlCountries" AutoPostBack="true" runat="server" CausesValidation="false" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:TextBox ID="txtFirstName" CssClass="hcInput50p" runat="server" MaxLength="255" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:TextBox asp:TextBox ID="txtLastName" CssClass="hcInput50p" runat="server" MaxLength="255" />
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtCompany" runat="server" MaxLength="255" />
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtAddressLine1" runat="server" MaxLength="255" />
                        <asp:RequiredFieldValidator id="rfvAddress1" ControlToValidate="txtAddressLine1" Display="Dynamic" CssClass="hcFormError" runat="server" />
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtAddressLine2" runat="server" MaxLength="255" />
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtCity" CssClass="hcInput33p" runat="server" MaxLength="255" />
                        <asp:RequiredFieldValidator ID="rfvCity" ControlToValidate="txtCity" Display="Dynamic" CssClass="hcFormError" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:DropDownList ID="ddlRegions" CssClass="hcInput33p" runat="server" />
                        <asp:TextBox ID="txtRegion" CssClass="hcInput33p" Visible="false" runat="server" MaxLength="255" />
                        <asp:CompareValidator ID="valRegion" ControlToValidate="ddlRegions" Operator="NotEqual" Display="Dynamic" CssClass="hcFormError" runat="server" />
                    </div>
                    <div class="hcFormItem hcFormItem50p">
                        <asp:TextBox ID="txtZip" CssClass="hcInput33p" runat="server" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvZip" ControlToValidate="txtZip" Display="Dynamic" CssClass="hcFormError" runat="server" />
                    </div>
                    <div class="hcFormItem">
                        <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="hcColumn hcLeftBorder" style="width: 33.2%">
        <div class="hcForm">
            <h2><%=Localization.GetString("GeoLocation") %></h2>
            <hcc:MessageBox ID="ucMessageBox" runat="server" AddValidationSummaries="false" />
            <div class="hcFormItem">
                <asp:Label resourcekey="TimeZone" AssociatedControlID="ddlTimeZone" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlTimeZone" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:Label resourcekey="CurrencyCulture" AssociatedControlID="ddlCurrencyCulture" runat="server" CssClass="hcLabel" />
                <asp:DropDownList ID="ddlCurrencyCulture" runat="server" AutoPostBack="true" CausesValidation="false"
                    OnSelectedIndexChanged="ddlCurrencyCulture_SelectedIndexChanged" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 33.2%">
        <div class="hcForm">
            <h2><%=Localization.GetString("PageConfiguration") %></h2>
            <div class="hcFormItem">
                <asp:Label AssociatedControlID="txtCategoryUrl" runat="server" CssClass="hcLabel">
                <%=Localization.GetString("Category") %> <i class="hcIconInfo"><span class="hcFormInfo Hidden"><%=Localization.GetString("CategoryHelp") %></span></i>
                </asp:Label>
                <asp:DropDownList ID="ddlCategoryTab" runat="server" />
            </div>
            <div runat="server" id="divCategoryUrl" class="hcFormItem">
                <asp:TextBox ID="txtCategoryUrl" runat="server" MaxLength="255" />
                <asp:RequiredFieldValidator runat="server" ID="rfvCategoryUrl" ControlToValidate="txtCategoryUrl" Display="Dynamic" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:Label ID="lblProductsUrl" AssociatedControlID="txtProductsUrl" runat="server" CssClass="hcLabel">
                   <%=Localization.GetString("Products") %> <i class="hcIconInfo"><span class="hcFormInfo Hidden"><%=Localization.GetString("ProductsHelp") %></span></i>
                </asp:Label>
                <asp:DropDownList ID="ddlProductsTab" runat="server" />
            </div>
            <div runat="server" id="divProductsUrl" class="hcFormItem">
                <asp:TextBox ID="txtProductsUrl" runat="server" MaxLength="255" />
                <asp:RequiredFieldValidator runat="server" ID="rfvProductsUrl" ControlToValidate="txtProductsUrl" Display="Dynamic" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:Label ID="lblCheckoutUrl" AssociatedControlID="txtCheckoutUrl" runat="server" CssClass="hcLabel">
                    <%=Localization.GetString("Checkout") %> <i class="hcIconInfo"><span class="hcFormInfo Hidden"><%=Localization.GetString("CheckoutHelp") %></span></i>
                </asp:Label>
                <asp:DropDownList ID="ddlCheckoutTab" runat="server" />
            </div>
            <div runat="server" id="divCheckoutUrl" class="hcFormItem">
                <asp:TextBox ID="txtCheckoutUrl" runat="server" MaxLength="255" />
                <asp:RequiredFieldValidator runat="server" ID="rfvCheckoutUrl" ControlToValidate="txtCheckoutUrl" Display="Dynamic" CssClass="hcFormError" />
            </div>
        </div>
    </div>

    <div class="hcActionsRight">
        <ul class="hcActions">
            <li>
                <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnLater" resourcekey="btnLater" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" OnClick="btnLater_Click" />
            </li>
            <li>
                <asp:LinkButton ID="btnExit" resourcekey="btnExit" CssClass="hcSecondaryAction" CausesValidation="false" runat="server" OnClick="btnExit_Click" />
            </li>
        </ul>
    </div>
</div>
<hcc:AddressNormalizationDialog ID="AddressNormalizationDialog" runat="server" />
<script type="text/javascript">
    function pageLoad() {
        /* Create and init address validation inputs */
        var AddressValidationInputs = createAddressValidationInputs();
        AddressValidationInputs.init(".hcAddress", ".hcAddressMessage");

        AddressValidationInputs.getCountry = function () {
            return $("#<%= ddlCountries.ClientID %>").val();
        };
        AddressValidationInputs.setCountry = function (value) {
            $find("#<%= ddlCountries.ClientID %>").val(value);
        };
        AddressValidationInputs.getFirstName = function () {
            return $("#<%= txtFirstName.ClientID %>").val();
        };
        AddressValidationInputs.setFirstName = function (value) {
            $("#<%= txtFirstName.ClientID %>").val(value);
        };
        AddressValidationInputs.getLastName = function () {
            return $("#<%= txtLastName.ClientID %>").val();
        };
        AddressValidationInputs.setLastName = function (value) {
            $("#<%= txtLastName.ClientID %>").val(value);
        };
        AddressValidationInputs.getCompany = function () {
            return $("#<%= txtCompany.ClientID %>").val();
        };
        AddressValidationInputs.setCompany = function (value) {
            $("#<%= txtCompany.ClientID %>").val(value);
        };
        AddressValidationInputs.getAddressLine1 = function () {
            return $("#<%= txtAddressLine1.ClientID %>").val();
        };
        AddressValidationInputs.setAddressLine1 = function (value) {
            $("#<%= txtAddressLine1.ClientID %>").val(value);
        };
        AddressValidationInputs.getAddressLine2 = function () {
            return $("#<%= txtAddressLine2.ClientID %>").val();
        };
        AddressValidationInputs.setAddressLine2 = function (value) {
            $("#<%= txtAddressLine2.ClientID %>").val(value);
        };
        AddressValidationInputs.getCity = function () {
            return $("#<%= txtCity.ClientID %>").val();
        };
        AddressValidationInputs.setCity = function (value) {
            $("#<%= txtCity.ClientID %>").val(value);
        };
        AddressValidationInputs.getRegion = function () {
            return $("#<%= ddlRegions.ClientID %>").val();
        };
        AddressValidationInputs.setRegion = function (value) {
            $("#<%= ddlRegions.ClientID %>").val(value);
        };
        AddressValidationInputs.getPostalCode = function () {
            return $("#<%= txtZip.ClientID %>").val();
        };
        AddressValidationInputs.setPostalCode = function (value) {
            $("#<%= txtZip.ClientID %>").val(value);
        };
        AddressValidationInputs.getPhone = function () {
            return $("#<%= txtPhone.ClientID %>").val();
        };
        AddressValidationInputs.setPhone = function (value) {
            $("#<%= txtPhone.ClientID %>").val(value);
        };

        /* Create and init address validator */
        var AddressValidator = createAddressValidator();
        AddressValidator.init(AddressValidationInputs, "#<%= btnSave.ClientID%>");
    }
</script>
