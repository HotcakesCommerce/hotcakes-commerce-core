<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit" CodeBehind="Products_Edit.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Hotcakes.Modules" Namespace="Hotcakes.Modules.Core.Admin.AppCode" TagPrefix="hcc" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/UrlsAssociated.ascx" TagName="UrlsAssociated" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:ProductEditMenu ID="ProductNavigator" SelectedMenuItem="General" runat="server" />

    <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank">View in Store</asp:HyperLink>
            </div>
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkClone" runat="server" CssClass="hcTertiaryAction">Clone Product</asp:HyperLink>
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnDelete" runat="server" Text="Delete Product" CssClass="hcTertiaryAction" CausesValidation="false"
                    OnClick="btnDelete_Click" OnClientClick="return hcConfirm(event,'Are you sure you want to delete this product?');" />
            </div>
			<div class="hcFormItem">
				<asp:HyperLink ID="lnkBacktoAbandonedCartsReport" Text="Back to Abandoned Carts Report" CssClass="hcTertiaryAction" Target="_self" runat="server" />
			</div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        jQuery(function ($) {

            var updateState = function ($checkbox, $control) {
                var checked = $checkbox.is(':checked');
                if (checked) {
                    $control.attr("disabled", "disabled");
                    $control.addClass("aspNetDisabled");
                }
                else {
                    $control.removeAttr("disabled");
                    $control.removeClass("aspNetDisabled");
                }
            };
            var showControlParent = function ($checkbox, $targetCheckbox) {
                var checked = $checkbox.is(':checked');
                if (checked)
                    $targetCheckbox.parent().show();
                else {
                    var trgChecked = $targetCheckbox.is(':checked');
                    if (trgChecked) $targetCheckbox.click();
                    $targetCheckbox.parent().hide();
                }
            };
            var hideControlParent = function ($checkbox, $targetCheckbox) {
                var checked = $checkbox.is(':checked');
                if (checked)
                    $targetCheckbox.parent().hide();
                else {
                    var trgChecked = $targetCheckbox.is(':checked');
                    if (trgChecked) $targetCheckbox.click();
                    $targetCheckbox.parent().show();
                }
            };

            var $chbUserPrice = $('.hcUserPrice input');
            var $hcUserPriceLabel = $('.hcUserPriceLabel');
            var $txtPriceInputs = $('.hcPriceInput');
            var $chbHideQty = $('#<%=chkHideQty.ClientID%>');
            var $txtMinQty = $('#<%=txtMinimumQty.ClientID%>');
            var $cbShippingCharge = $('#<%=chkNonShipping.ClientID%>');
            var $hcShippingChargeInput = $('.hcShippingChargeInput');

            var $txtProductName = $('#txtProductName');
            var $txtRewriteUrl = $('#txtRewriteUrl');

            var updatePriceVisibility = function () { updateState($chbUserPrice, $txtPriceInputs); }
            var updateQtyVisibility = function () { updateState($chbHideQty, $txtMinQty); }
            var updateQtyFormItem = function () { showControlParent($chbUserPrice, $chbHideQty); }
            var updatePriceLabelFormItem = function () { showControlParent($chbUserPrice, $hcUserPriceLabel); }
            var updateShippingChargeVisibility = function () { hideControlParent($cbShippingCharge, $hcShippingChargeInput); }

            $chbUserPrice.change(updatePriceVisibility);
            $chbUserPrice.change(updateQtyFormItem);
            $chbUserPrice.change(updatePriceLabelFormItem);
            $chbHideQty.change(updateQtyVisibility);
            $cbShippingCharge.change(updateShippingChargeVisibility);

            updatePriceVisibility();
            updateQtyVisibility();
            updateQtyFormItem();
            updatePriceLabelFormItem();
            updateShippingChargeVisibility();

            $txtProductName.change(function () {
                var rawName = $(this).val();
                var cleanName = $txtRewriteUrl.val();
                $.post('../Controllers/Slugify.aspx', { "input": rawName },
					function (data) {
					    if (cleanName === '') {
					        $txtRewriteUrl.val(data);
					    }
					});
            });
        });

    </script>

    <h1>Edit Product</h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2>Main</h2>
            <div class="hcFormItem">
                <label class="hcLabel">Product Style</label>
                <asp:RadioButtonList ID="rbBehaviour" AutoPostBack="true" CausesValidation="false"
                    RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server" CssClass="hcProductBehaviour">
                    <asp:ListItem Text="Product" Value="" Selected="True" />
                    <asp:ListItem Text="Bundle" Value="B" />
                    <asp:ListItem Text="Gift Card" Value="GC" />
                </asp:RadioButtonList>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Name<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="txtProductName" ClientIDMode="static" runat="server" Width="50%" />
                <asp:RequiredFieldValidator ID="valNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="txtProductName" Text="Name is Required" CssClass="hcFormError" />
                <hcc:TextBoxLengthValidator runat="server" ID="tlvNameLength" MaxLength="512" ControlToValidate="txtProductName" Display="Dynamic"
                    Text="Name can not be longer than 255 characters" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">SKU</label>
                <asp:TextBox ID="SkuField" runat="server" Width="100%" />
                <asp:RequiredFieldValidator ID="valSkuRequired" runat="server" ControlToValidate="SkuField" Display="Dynamic"
                    Text="Sku is Required" CssClass="hcFormError" />
                <hcc:TextBoxLengthValidator runat="server" ID="tlvSkuLength" MaxLength="50" ControlToValidate="SkuField" Display="Dynamic"
                    Text="Sku can not be more than 50 characters long" CssClass="hcFormError" />
            </div>
            <div class="hcFormItem hcFormItem50p hcAlignRight">
                <label class="hcLabel">Product Type</label>
                <telerik:RadComboBox ID="lstProductType" runat="server" AutoPostBack="True" CausesValidation="false" Width="95%" />
                <asp:CustomValidator ID="ProductTypeCustomValidator" runat="server" Display="Dynamic" ErrorMessage="Test"
                    OnServerValidate="ProductTypeCustomValidator_ServerValidate" />
            </div>
            <div>
                <asp:Literal ID="litProductTypeProperties" runat="server" />
            </div>
        </div>
        <asp:Panel ID="pnlPricing" runat="server" class="hcForm">
            <h2 class="hcClear">Pricing</h2>
            <div class="hcFormItem">
                <asp:CheckBox CssClass="hcUserPrice" ID="chkUserPrice" Text="Allow User Supplied Price" runat="server" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkHideQty" Text="Hide Quantity" runat="server" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">User Price Label<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="txtUserPriceLabel" runat="server" CssClass="hcUserPriceLabel" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">MSRP</label>
                <asp:TextBox ID="ListPriceField" runat="server" CssClass="FormInput hcPriceInput" style="width:200px"/>
                <asp:RequiredFieldValidator ID="rfvListPrice" runat="server" CssClass="hcFormError" Display="Dynamic"
                    ControlToValidate="ListPriceField" Text="List Price is required" />
                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="ListPriceField" Display="Dynamic"
                    CssClass="hcFormError" Text="Msrp must be a currency value" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">Cost</label>
                <asp:TextBox ID="CostField" runat="server" CssClass="hcPriceInput" style="width:200px" />
                <asp:RequiredFieldValidator ID="rfvCostField" runat="server" ControlToValidate="CostField" Display="Dynamic"
                    CssClass="hcFormError" Text="Cost is required" />
                <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="CostField" Display="Dynamic"
                    CssClass="hcFormError" Text="Cost must be a currency value" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">Price</label>
                <asp:TextBox ID="SitePriceField" runat="server" Columns="10" CssClass="FormInput hcPriceInput" style="width:200px" />
                <asp:RequiredFieldValidator ID="rfvSitePrice" runat="server" CssClass="hcFormError" Display="Dynamic"
                    ControlToValidate="SitePriceField" Text="Site Price is required" />
                <asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="SitePriceField" Display="Dynamic"
                    CssClass="hcFormError" Text="Price must be a currency value" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">Text<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="PriceOverrideTextBox" runat="server" Columns="10" CssClass="hcPriceInput" style="width:200px"/>
            </div>
        </asp:Panel>
        <div class="hcForm">
            <h2 class="hcClear">Properties</h2>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">Manufacturer</label>
                <telerik:RadComboBox ID="lstManufacturers" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <label class="hcLabel">Vendor</label>
                <telerik:RadComboBox ID="lstVendors" runat="server" />
            </div>

        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Display</h2>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox Checked="true" ID="chkActive" Text="Active" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox Checked="true" ID="chkSearchable" Text="Searchable" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkFeatured" Text="Featured" runat="server" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Template</label>
                <telerik:RadComboBox ID="ddlTemplateList" runat="server" AutoPostBack="False" />
            </div>
            <div class="hcFormItem hcFormItem33p hcClear">
                <label class="hcLabel">Large Image</label>
                <hcc:ImageUploader runat="server" ID="ucImageUploadLarge" />
            </div>
            <div class="hcFormItem hcFormItem66p">
                <label class="hcLabel">Image Description<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MediumImageAlternateTextField" TextMode="MultiLine" Height="150px" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem33p hcClear">
                <label class="hcLabel">Small Image</label>
                <asp:Image ID="imgPreviewSmall" runat="server" ImageUrl="../images/NoImageAvailable.gif" Height="150px" />
            </div>
            <div class="hcFormItem hcFormItem66p">
                <label class="hcLabel">Image Description<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="SmallImageAlternateTextField" TextMode="MultiLine" Height="150px" runat="server" />
            </div>
        </div>
    </div>

    <div class="hcForm hcClear">
        <h2>Description<i class="hcLocalizable"></i></h2>
        <div class="hcFormItem">
            <hcc:HtmlEditor ID="LongDescriptionField" runat="server" EditorHeight="300" EditorWidth="1340"
                EditorWrap="true" />
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Alternate Search Keywords<i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtKeywords" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Meta Title<i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtMetaTitle" runat="server" />
            <hcc:TextBoxLengthValidator runat="server" ID="tlvMetaTitle" MaxLength="512" ControlToValidate="txtMetaTitle" Display="Dynamic"
                Text="Meta Title can not be more than 512 characters long" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Meta Keywords<i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtMetaKeywords" runat="server" TextMode="multiLine" Rows="3" Width="100%" />
            <hcc:TextBoxLengthValidator runat="server" ID="tlvMetaKeywords" MaxLength="255" ControlToValidate="txtMetaKeywords" Display="Dynamic"
                Text="Meta Keywords can not be more than 255 characters long" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Meta Description<i class="hcLocalizable"></i></label>
            <asp:TextBox ID="txtMetaDescription" runat="server" TextMode="multiLine" Rows="3" Width="100%" />
            <hcc:TextBoxLengthValidator runat="server" ID="tlvMetaDescription" MaxLength="255" ControlToValidate="txtMetaDescription" Display="Dynamic"
                Text="Meta Description can not be more than 255 characters long" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem" runat="server" id="divTaxonomyBlock">
            <label class="hcLabel">Taxonomy Tags</label>
            <asp:TextBox ID="txtTaxonomyTags" runat="server" TextMode="multiLine" Rows="2" Width="100%" />
        </div>
    </div>

    <div class="hcForm hcClear">
        <h2>Tax</h2>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Tax Schedule</label>
            <asp:DropDownList ID="lstTaxClasses" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">&nbsp;</label>
            <asp:CheckBox Checked="false" ID="TaxExemptField" Text="Tax Exempt?" runat="server" />
        </div>
    </div>

    <h2 class="hcClear">Shipping</h2>

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem hcFormItem33p">
                <label class="hcLabel">Weight</label>
                <asp:TextBox ID="txtWeight" runat="server" CssClass="RadComboBox" /><span class="hcInset">lb</span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtWeight"
                    Display="Dynamic" Text="Weight is required." CssClass="hcFormError" />
                <asp:CustomValidator ID="CustomValidator4" runat="server" ControlToValidate="txtWeight"
                    Display="Dynamic" Text="Weight must be a numeric value." CssClass="hcFormError" />
				<asp:CompareValidator ID="cvWeight" ControlToValidate="txtWeight"
					Type="Double" Display="Dynamic" Operator="DataTypeCheck"
					ErrorMessage="Not a valid number." runat="server" CssClass="hcFormError"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Extra Ship Fee</label>
                <asp:TextBox ID="ExtraShipFeeField" runat="server" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ExtraShipFeeField"
                    Display="Dynamic" Text="Extra Ship Fee is required." CssClass="hcFormError" />
                <asp:CustomValidator ID="CustomValidator8" runat="server" ControlToValidate="ExtraShipFeeField"
                    Display="Dynamic" Text="Extra Ship Fee must be a numeric value." CssClass="hcFormError" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkNonShipping" runat="server" Text="Non-Shipping Product"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Shipping Charges
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none">This should not be used as a promotion, but to define products that always or never charge the specified fees.</span>
                    </i>
                </label>
                <asp:DropDownList ID="lstShippingCharge" runat="server" CssClass="hcShippingChargeInput">
                    <asp:ListItem Text="No Charge" Value="0" />
                    <asp:ListItem Text="Charge Shipping &amp; Handling" Value="1" />
                    <asp:ListItem Text="Charge Shipping Only" Value="2" />
                    <asp:ListItem Text="Charge Handling Only" Value="3" />
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="hcColumnRight" style="width: 49%">
        <div class="hcForm">
            <div class="hcFormItemLabel">
                <label class="hcLabel">Dimensions</label>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="txtLength" runat="server" CssClass="RadComboBox" /><span class="hcInset">L</span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtLength"
                    Display="Dynamic" Text="Length is required." CssClass="hcFormError" />
				<asp:CompareValidator ID="cvLength" ControlToValidate="txtLength"
					Type="Double" Display="Dynamic" Operator="DataTypeCheck"
					ErrorMessage="Not a valid number." runat="server" CssClass="hcFormError"/>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="txtWidth" runat="server" CssClass="RadComboBox"/><span class="hcInset">W</span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtWidth"
                    Display="Dynamic" Text="Width is required." CssClass="hcFormError" />
                <asp:CustomValidator ID="CustomValidator6" runat="server" ControlToValidate="txtWidth"
                    Display="Dynamic" Text="Width must be a numeric value." CssClass="hcFormError" />
				<asp:CompareValidator ID="cvWidth" ControlToValidate="txtWidth"
					Type="Double" Display="Dynamic" Operator="DataTypeCheck"
					ErrorMessage="Not a valid number." runat="server" CssClass="hcFormError"/>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="txtHeight" runat="server" CssClass="RadComboBox" /><span class="hcInset">H</span>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtHeight"
                    Display="Dynamic" Text="Height is required." CssClass="hcFormError" />
                <asp:CustomValidator ID="CustomValidator7" runat="server" ControlToValidate="txtHeight"
                    Display="Dynamic" Text="Height must be a numeric value." CssClass="hcFormError" />
				<asp:CompareValidator ID="cvHeight" ControlToValidate="txtHeight"
					Type="Double" Display="Dynamic" Operator="DataTypeCheck"
					ErrorMessage="Not a valid number." runat="server" CssClass="hcFormError"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Ship Mode</label>
                <asp:DropDownList ID="ddlShipType" runat="server">
                    <asp:ListItem Text="Ship from Store" Value="1" />
                    <asp:ListItem Text="Drop Ship from Manufacturer" Value="3" />
                    <asp:ListItem Text="Drop Ship from Vendor" Value="2" />
                </asp:DropDownList>
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkShipSeparately" runat="server" Text="Ships in a Separate Box" />
            </div>
        </div>
    </div>

    <h2 class="hcClear">Advanced</h2>

    <div class="hcForm">
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Minimum Quantity</label>
            <asp:TextBox ID="txtMinimumQty" runat="server" Columns="5" Rows="1" TabIndex="6050" Text="0" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtMinimumQty"
                Display="Dynamic" Text="Minimum Quantity is required." CssClass="hcFormError" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtMinimumQty"
                Display="Dynamic" Text="Minimum Quantity must be numeric." ValidationExpression="[0-9]{1,6}" CssClass="hcFormError" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Allow Reviews</label>
            <asp:RadioButtonList runat="server" ID="rblAllowReviews" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Text="Inherit" Value="" Selected="True" />
                <asp:ListItem Text="Yes" Value="True" />
                <asp:ListItem Text="No" Value="False" />
            </asp:RadioButtonList>
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Page URL</label>
            <asp:TextBox ID="txtRewriteUrl" ClientIDMode="Static" runat="server" />
            <hcc:UrlsAssociated ID="UrlsAssociated1" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Swatch Folder Name</label>
            <asp:TextBox ID="swatchpathfield" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Header Content Column</label>
            <asp:DropDownList ID="ddlPreContentColumn" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Footer Content Column</label>
            <asp:DropDownList ID="ddlPostContentColumn" runat="server" />
        </div>
    </div>
    <%--
    <tr style="display: none;">
            <td colspan="2">
                <h2>Gift Wrap</h2>
            </td>
        </tr>
        <tr style="display: none;">
            <td class="formlabel">Gift Wrap:</td>
            <td class="formfield">
                <asp:CheckBox ID="chkGiftWrapAllowed" runat="server" /></td>
        </tr>
        <tr style="display: none;">
            <td class="formlabel">Gift Wrap Charge:</td>
            <td class="formfield">
                <asp:TextBox ID="txtGiftWrapCharge" runat="server" Columns="5" Rows="1" TabIndex="6050" Text="0.00"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGiftWrapCharge"
                    Display="Dynamic" Text="Gift Wrap Charge is required." />
                <asp:CustomValidator runat="server" ControlToValidate="txtGiftWrapCharge"
                    CssClass="errormessage" Text="Gift Wrap Charge must be a currency value" /></td>
        </tr>--%>
    <ul class="hcActions">
        <li>
            <asp:LinkButton runat="server" Text="Save" CssClass="hcPrimaryAction" OnClick="btnUpdate_Click" />
        </li>
        <li>
            <asp:LinkButton runat="server" Text="Save and Close" CssClass="hcSecondaryAction" OnClick="btnSave_Click" />
        </li>
        <li>
            <asp:LinkButton runat="server" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
