<%@ Page ValidateRequest="false" Language="C#"
MasterPageFile="../AdminNav.master" AutoEventWireup="True"
Inherits="Hotcakes.Modules.Core.Admin.Catalog.Products_Edit"
CodeBehind="Products_Edit.aspx.cs" %> <%@ Register Assembly="Hotcakes.Modules"
Namespace="Hotcakes.Modules.Core.Admin.AppCode" TagPrefix="hcc" %> <%@ Register
Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %> <%@
Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc"
%> <%@ Register Src="../Controls/UrlsAssociated.ascx" TagName="UrlsAssociated"
TagPrefix="hcc" %> <%@ Register Src="../Controls/ProductEditMenu.ascx"
TagName="ProductEditMenu" TagPrefix="hcc" %> <%@ Register
Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
  <hcc:ProductEditMenu
    ID="ProductNavigator"
    SelectedMenuItem="General"
    runat="server"
  />

  <div class="hcBlock hcBlockNotTopPadding">
    <div class="hcForm">
      <div class="hcFormItem">
        <asp:HyperLink
          ID="lnkViewInStore"
          resourcekey="lnkViewInStore"
          runat="server"
          CssClass="hcTertiaryAction"
          Target="_blank"
        />
      </div>
      <div class="hcFormItem">
        <asp:HyperLink
          ID="lnkClone"
          resourcekey="lnkClone"
          runat="server"
          CssClass="hcTertiaryAction"
        />
      </div>
      <div class="hcFormItem">
        <asp:LinkButton
          ID="btnDelete"
          resourcekey="btnDelete"
          runat="server"
          CssClass="hcTertiaryAction hcDeleteProduct"
          CausesValidation="false"
          OnClick="btnDelete_Click"
        />
      </div>
      <div class="hcFormItem">
        <asp:HyperLink
          ID="lnkBacktoAbandonedCartsReport"
          resourcekey="lnkBacktoAbandonedCartsReport"
          CssClass="hcTertiaryAction"
          Target="_self"
          runat="server"
        />
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
        var updateElementState = function ($checkbox, $control) {
            var checked = $checkbox.is(':checked');
            if (checked) {
                $control.removeAttr("hidden");
                $control.removeClass("hidden");
            }
            else {
                $control.attr("hidden", "hidden");
                $control.addClass("hidden");
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

        var $chbAllowUpcharge = $('.hcAllowUpcharge input');
        var $hcAllowUpchargeSection = $('.hcAllowUpchargeSection');

        var updateAllowUpchargeSectionVisibility = function () { updateElementState($chbAllowUpcharge, $hcAllowUpchargeSection); }

        $chbAllowUpcharge.change(updateAllowUpchargeSectionVisibility);

        updateAllowUpchargeSectionVisibility();


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

        $(".hcDeleteProduct").click(function (e) {
            e.preventDefault();
            return hcConfirm(e, "<%=Localization.GetJsEncodedString("Confirm")%>");
        });
    });
  </script>

  <h1><%=PageTitle %></h1>
  <hcc:MessageBox ID="ucMessageBox" runat="server" />

  <div class="hcColumnLeft" style="width: 50%">
    <div class="hcForm">
      <h2><%=Localization.GetString("MainHeader") %></h2>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblProductStyle") %></label
        >
        <asp:RadioButtonList
          ID="rbBehaviour"
          AutoPostBack="true"
          CausesValidation="false"
          RepeatLayout="Flow"
          RepeatDirection="Horizontal"
          runat="server"
          CssClass="hcProductBehaviour"
        >
          <asp:ListItem
            resourcekey="rbBehaviour_Product"
            Value=""
            Selected="True"
          />
          <asp:ListItem resourcekey="rbBehaviour_Bundle" Value="B" />
          <asp:ListItem resourcekey="rbBehaviour_GiftCard" Value="GC" />
        </asp:RadioButtonList>
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblProductName") %><i
            class="hcLocalizable"
          ></i
        ></label>
        <asp:TextBox
          ID="txtProductName"
          ClientIDMode="static"
          runat="server"
          Width="50%"
        />
        <asp:RequiredFieldValidator
          ID="valNameRequired"
          resourcekey="valNameRequired"
          runat="server"
          Display="Dynamic"
          ControlToValidate="txtProductName"
          CssClass="hcFormError"
        />
        <hcc:TextBoxLengthValidator
          runat="server"
          ID="tlvNameLength"
          MaxLength="512"
          ControlToValidate="txtProductName"
          Display="Dynamic"
          Text='<%=Localization.GetString("tlvNameLength") %>'
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("lblSku") %></label>
        <asp:TextBox ID="SkuField" runat="server" Width="100%" />
        <asp:RequiredFieldValidator
          ID="valSkuRequired"
          resourcekey="valSkuRequired"
          runat="server"
          ControlToValidate="SkuField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <hcc:TextBoxLengthValidator
          runat="server"
          ID="tlvSkuLength"
          MaxLength="50"
          ControlToValidate="SkuField"
          Display="Dynamic"
          Text='<%=Localization.GetString("tlvSkuLength") %>'
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem50p hcAlignRight">
        <label class="hcLabel"
          ><%=Localization.GetString("lblProductType") %></label
        >
        <asp:DropDownList
          ID="lstProductType"
          runat="server"
          AutoPostBack="True"
          CausesValidation="false"
          Width="95%"
        />
        <asp:CustomValidator
          ID="ProductTypeCustomValidator"
          runat="server"
          Display="Dynamic"
          OnServerValidate="ProductTypeCustomValidator_ServerValidate"
        />
      </div>
      <div>
        <asp:Literal ID="litProductTypeProperties" runat="server" />
      </div>
    </div>
    <asp:Panel ID="pnlPricing" runat="server" class="hcForm">
      <h2 class="hcClear"><%=Localization.GetString("lblPricing") %></h2>
      <div class="hcFormItem hcFormItem50p hcAlignLeft">
        <asp:CheckBox
          CssClass="hcUserPrice"
          ID="chkUserPrice"
          resourcekey="chkUserPrice"
          runat="server"
        />
      </div>
      <div class="hcFormItem hcFormItem50p hcAlignRight">
         <asp:CheckBox
           CssClass="hcAllowUpcharge"
           ID="chkAllowUpcharge"
           resourcekey="chkAllowUpcharge"
           runat="server"
        />
      </div>
        <div runat="server" class="hcColumnLeft hcForm hcAllowUpchargeSection" style="width: 100%">
        <div class="hcFormItemLabel">
          <label class="hcLabel"
            ><%=Localization.GetString("AdjustPriceBy") %></label
          >
        </div>
        <div class="hcFormItem hcFormItem50p">
          <asp:TextBox
            ID="UpchargeAmountField"
            runat="server"
            Columns="10"
          ></asp:TextBox>
        </div>
        <div class="hcFormItem hcFormItem50p">
          <asp:DropDownList ID="lstUpchargeUnitType" runat="server" />
        </div>
      </div>
      <div class="hcFormItem">
        <asp:CheckBox ID="chkHideQty" resourcekey="chkHideQty" runat="server" />
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblUserPriceLabel") %><i
            class="hcLocalizable"
          ></i
        ></label>
        <asp:TextBox
          ID="txtUserPriceLabel"
          runat="server"
          CssClass="hcUserPriceLabel"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("lblMSRP") %></label>
        <asp:TextBox
          ID="ListPriceField"
          runat="server"
          CssClass="FormInput hcPriceInput"
          style="width: 200px"
        />
        <asp:RequiredFieldValidator
          ID="rfvListPrice"
          resourcekey="rfvListPrice"
          runat="server"
          CssClass="hcFormError"
          Display="Dynamic"
          ControlToValidate="ListPriceField"
        />
        <asp:CustomValidator
          ID="cvListPrice"
          resourcekey="cvListPrice"
          runat="server"
          ControlToValidate="ListPriceField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("lblCost") %></label>
        <asp:TextBox
          ID="CostField"
          runat="server"
          CssClass="hcPriceInput"
          style="width: 200px"
        />
        <asp:RequiredFieldValidator
          ID="rfvCostField"
          resourcekey="rfvCostField"
          runat="server"
          ControlToValidate="CostField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CustomValidator
          ID="cvCostField"
          resourcekey="cvCostField"
          runat="server"
          ControlToValidate="CostField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("lblPrice") %></label>
        <asp:TextBox
          ID="SitePriceField"
          runat="server"
          Columns="10"
          CssClass="FormInput hcPriceInput"
          style="width: 200px"
        />
        <asp:RequiredFieldValidator
          ID="rfvSitePrice"
          resourcekey="rfvSitePrice"
          runat="server"
          CssClass="hcFormError"
          Display="Dynamic"
          ControlToValidate="SitePriceField"
        />
        <asp:CustomValidator
          ID="cvSitePrice"
          resourcekey="cvSitePrice"
          runat="server"
          ControlToValidate="SitePriceField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"
          ><%=Localization.GetString("lblText") %><i class="hcLocalizable"></i
        ></label>
        <asp:TextBox
          ID="PriceOverrideTextBox"
          runat="server"
          Columns="10"
          CssClass="hcPriceInput"
          style="width: 200px"
        />
      </div>
    </asp:Panel>
    <div class="hcForm">
      <h2 class="hcClear">Properties</h2>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"
          ><%=Localization.GetString("lblManufacturer") %></label
        >
        <asp:DropDownList ID="lstManufacturers" runat="server" />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <label class="hcLabel"><%=Localization.GetString("lblVendor") %></label>
        <asp:DropDownList ID="lstVendors" runat="server" />
      </div>
    </div>
  </div>
  <div class="hcColumnRight hcLeftBorder" style="width: 49%">
    <div class="hcForm">
      <h2><%=Localization.GetString("DisplayHeader") %></h2>
      <div class="hcFormItem hcFormItem50p">
        <asp:CheckBox
          Checked="true"
          ID="chkActive"
          resourcekey="chkActive"
          runat="server"
        />
      </div>
      <div class="hcFormItem hcFormItem50p">
        <asp:CheckBox
          Checked="true"
          ID="chkSearchable"
          resourcekey="chkSearchable"
          runat="server"
        />
      </div>
     
      <div class="hcFormItem hcFormItem50p">
        <asp:CheckBox
          ID="chkFeatured"
          resourcekey="chkFeatured"
          runat="server"
        />
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblTemplate") %></label
        >
        <asp:DropDownList
          ID="ddlTemplateList"
          runat="server"
          AutoPostBack="False"
        />
      </div>
      <div class="hcFormItem hcFormItem33p hcClear">
        <label class="hcLabel"
          ><%=Localization.GetString("lblLargeImage") %></label
        >
        <hcc:ImageUploader runat="server" ID="ucImageUploadLarge" />
      </div>
      <div class="hcFormItem hcFormItem66p">
        <label class="hcLabel"
          ><%=Localization.GetString("lblImageDescription") %><i
            class="hcLocalizable"
          ></i
        ></label>
        <asp:TextBox
          ID="MediumImageAlternateTextField"
          TextMode="MultiLine"
          Height="150px"
          runat="server"
        />
      </div>
      <div class="hcFormItem hcFormItem33p hcClear">
        <label class="hcLabel"
          ><%=Localization.GetString("lblSmallImage") %></label
        >
        <asp:Image
          ID="imgPreviewSmall"
          runat="server"
          ImageUrl="../images/NoImageAvailable.gif"
          Height="150px"
        />
      </div>
      <div class="hcFormItem hcFormItem66p">
        <label class="hcLabel"
          ><%=Localization.GetString("lblImageDescription") %><i
            class="hcLocalizable"
          ></i
        ></label>
        <asp:TextBox
          ID="SmallImageAlternateTextField"
          TextMode="MultiLine"
          Height="150px"
          runat="server"
        />
      </div>
    </div>
  </div>

  <div class="hcForm hcClear">
    <h2>
      <%=Localization.GetString("DescriptionHeader") %><i
        class="hcLocalizable"
      ></i>
    </h2>
    <div class="hcFormItem">
      <hcc:HtmlEditor
        ID="LongDescriptionField"
        runat="server"
        EditorHeight="300"
        EditorWidth="1340"
        EditorWrap="true"
      />
    </div>
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"
        ><%=Localization.GetString("lblAlternateSearchKeywords") %><i
          class="hcLocalizable"
        ></i
      ></label>
      <asp:TextBox ID="txtKeywords" runat="server" />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel"
        ><%=Localization.GetString("lblMetaTitle") %><i
          class="hcLocalizable"
        ></i
      ></label>
      <asp:TextBox ID="txtMetaTitle" runat="server" />
      <hcc:TextBoxLengthValidator
        runat="server"
        ID="tlvMetaTitle"
        MaxLength="512"
        ControlToValidate="txtMetaTitle"
        Display="Dynamic"
        Text='<%=Localization.GetString("tlvMetaTitle") %>'
        CssClass="hcFormError"
      />
    </div>
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"
        ><%=Localization.GetString("lblMetaKeywords") %><i
          class="hcLocalizable"
        ></i
      ></label>
      <asp:TextBox
        ID="txtMetaKeywords"
        runat="server"
        TextMode="multiLine"
        Rows="3"
        Width="100%"
      />
      <hcc:TextBoxLengthValidator
        runat="server"
        ID="tlvMetaKeywords"
        MaxLength="255"
        ControlToValidate="txtMetaKeywords"
        Display="Dynamic"
        Text="Meta Keywords can not be more than 255 characters long"
        CssClass="hcFormError"
      />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel"
        ><%=Localization.GetString("lblMetaDescription") %><i
          class="hcLocalizable"
        ></i
      ></label>
      <asp:TextBox
        ID="txtMetaDescription"
        runat="server"
        TextMode="multiLine"
        Rows="3"
        Width="100%"
      />
      <hcc:TextBoxLengthValidator
        runat="server"
        ID="tlvMetaDescription"
        MaxLength="255"
        ControlToValidate="txtMetaDescription"
        Display="Dynamic"
        Text='<%=Localization.GetString("tlvMetaDescription") %>'
        CssClass="hcFormError"
      />
    </div>
    <div class="hcFormItem" runat="server" id="divTaxonomyBlock">
      <label class="hcLabel"
        ><%=Localization.GetString("lblTaxonomyTags") %></label
      >
      <asp:TextBox
        ID="txtTaxonomyTags"
        runat="server"
        TextMode="multiLine"
        Rows="2"
        Width="100%"
      />
    </div>
  </div>

  <div class="hcForm hcClear">
    <h2><%=Localization.GetString("TaxHeader") %></h2>
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"
        ><%=Localization.GetString("lblTaxSchedule") %></label
      >
      <asp:DropDownList ID="lstTaxClasses" runat="server" />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel">&nbsp;</label>
      <asp:CheckBox
        Checked="false"
        ID="TaxExemptField"
        resourcekey="TaxExemptField"
        Text="Tax Exempt?"
        runat="server"
      />
    </div>
  </div>

  <h2 class="hcClear">Shipping</h2>

  <div class="hcColumnLeft" style="width: 50%">
    <div class="hcForm">
      <div class="hcFormItem hcFormItem33p">
        <label class="hcLabel"><%=Localization.GetString("lblWeight") %></label>
        <asp:TextBox
          ID="txtWeight"
          runat="server"
          CssClass="RadComboBox"
        /><span class="hcInset"
          ><%=Localization.GetString("WeightType") %></span
        >
        <asp:RequiredFieldValidator
          ID="rfvWeight"
          resourcekey="rfvWeight"
          runat="server"
          ControlToValidate="txtWeight"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CustomValidator
          ID="cstvWeight"
          resourcekey="cstvWeight"
          runat="server"
          ControlToValidate="txtWeight"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CompareValidator
          ID="cvWeight"
          resourcekey="cvWeight"
          ControlToValidate="txtWeight"
          Type="Double"
          Display="Dynamic"
          Operator="DataTypeCheck"
          runat="server"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblExtraShipFee") %></label
        >
        <asp:TextBox ID="ExtraShipFeeField" runat="server" />
        <asp:RequiredFieldValidator
          ID="rfvExtraShipFee"
          resourcekey="rfvExtraShipFee"
          runat="server"
          ControlToValidate="ExtraShipFeeField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CustomValidator
          ID="cvExtraShipFee"
          resourcekey="cvExtraShipFee"
          runat="server"
          ControlToValidate="ExtraShipFeeField"
          Display="Dynamic"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem">
        <asp:CheckBox
          ID="chkNonShipping"
          resourcekey="chkNonShipping"
          runat="server"
        />
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblShippingCharges") %>
          <i class="hcIconInfo">
            <span class="hcFormInfo" style="display: none"
              ><%=Localization.GetString("lblShippingCharges.Help") %></span
            >
          </i>
        </label>
        <asp:DropDownList
          ID="lstShippingCharge"
          runat="server"
          CssClass="hcShippingChargeInput"
        >
          <asp:ListItem resourcekey="ShippingCharge_NoCharge" Value="0" />
          <asp:ListItem resourcekey="ShippingCharge_ChargeAll" Value="1" />
          <asp:ListItem resourcekey="ShippingCharge_ChargeShipping" Value="2" />
          <asp:ListItem resourcekey="ShippingCharge_ChargeHandling" Value="3" />
        </asp:DropDownList>
      </div>
    </div>
  </div>
  <div class="hcColumnRight" style="width: 49%">
    <div class="hcForm">
      <div class="hcFormItemLabel">
        <label class="hcLabel"
          ><%=Localization.GetString("lblDimensions") %></label
        >
      </div>
      <div class="hcFormItem hcFormItem33p">
        <asp:TextBox
          ID="txtLength"
          runat="server"
          CssClass="RadComboBox"
        /><span class="hcInset"
          ><%=Localization.GetString("DimensionInset_Length") %></span
        >
        <asp:RequiredFieldValidator
          ID="rfvLength"
          resourcekey="rfvLength"
          runat="server"
          ControlToValidate="txtLength"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CompareValidator
          ID="cvLength"
          resourcekey="cvLength"
          ControlToValidate="txtLength"
          Type="Double"
          Display="Dynamic"
          Operator="DataTypeCheck"
          runat="server"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem33p">
        <asp:TextBox ID="txtWidth" runat="server" CssClass="RadComboBox" /><span
          class="hcInset"
          ><%=Localization.GetString("DimensionInset_Width") %></span
        >
        <asp:RequiredFieldValidator
          ID="rfvWidth"
          resourcekey="rfvWidth"
          runat="server"
          ControlToValidate="txtWidth"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CustomValidator
          ID="cstvWidth"
          resourcekey="cstvWidth"
          runat="server"
          ControlToValidate="txtWidth"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CompareValidator
          ID="cvWidth"
          resourcekey="cvWidth"
          ControlToValidate="txtWidth"
          Type="Double"
          Display="Dynamic"
          Operator="DataTypeCheck"
          runat="server"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem hcFormItem33p">
        <asp:TextBox
          ID="txtHeight"
          runat="server"
          CssClass="RadComboBox"
        /><span class="hcInset"
          ><%=Localization.GetString("DimensionInset_Height") %></span
        >
        <asp:RequiredFieldValidator
          ID="rfvHeight"
          resourcekey="rfvHeight"
          runat="server"
          ControlToValidate="txtHeight"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CustomValidator
          ID="cstvHeight"
          resourcekey="cstvHeight"
          runat="server"
          ControlToValidate="txtHeight"
          Display="Dynamic"
          CssClass="hcFormError"
        />
        <asp:CompareValidator
          ID="cvHeight"
          resourcekey="cvHeight"
          ControlToValidate="txtHeight"
          Type="Double"
          Display="Dynamic"
          Operator="DataTypeCheck"
          runat="server"
          CssClass="hcFormError"
        />
      </div>
      <div class="hcFormItem">
        <label class="hcLabel"
          ><%=Localization.GetString("lblShipMode") %></label
        >
        <asp:DropDownList ID="ddlShipType" runat="server">
          <asp:ListItem resourcekey="ShipMode_FromStore" Value="1" />
          <asp:ListItem resourcekey="ShipMode_FromManufacturer" Value="3" />
          <asp:ListItem resourcekey="ShipMode_FromVendor" Value="2" />
        </asp:DropDownList>
      </div>
      <div class="hcFormItem">
        <asp:CheckBox
          ID="chkShipSeparately"
          resourcekey="chkShipSeparately"
          runat="server"
        />
      </div>
    </div>
  </div>

  <h2 class="hcClear">Advanced</h2>

  <div class="hcForm">
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"
        ><%=Localization.GetString("lblMinimumQuantity") %></label
      >
      <asp:TextBox
        ID="txtMinimumQty"
        runat="server"
        Columns="5"
        Rows="1"
        TabIndex="6050"
        Text="0"
      />
      <asp:RequiredFieldValidator
        ID="rfvMinimumQty"
        resourcekey="rfvMinimumQty"
        runat="server"
        ControlToValidate="txtMinimumQty"
        Display="Dynamic"
        CssClass="hcFormError"
      />
      <asp:RegularExpressionValidator
        ID="revMinimumQty"
        resourcekey="revMinimumQty"
        runat="server"
        ControlToValidate="txtMinimumQty"
        Display="Dynamic"
        ValidationExpression="[0-9]{1,6}"
        CssClass="hcFormError"
      />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel"
        ><%=Localization.GetString("lblAllowReviews") %></label
      >
      <asp:RadioButtonList
        runat="server"
        ID="rblAllowReviews"
        RepeatLayout="Flow"
        RepeatDirection="Horizontal"
      >
        <asp:ListItem
          resourcekey="AllowReviews_Inherit"
          Value=""
          Selected="True"
        />
        <asp:ListItem resourcekey="AllowReviews_Yes" Value="True" />
        <asp:ListItem resourcekey="AllowReviews_No" Value="False" />
      </asp:RadioButtonList>
    </div>
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"><%=Localization.GetString("lblPageURL") %></label>
      <asp:TextBox ID="txtRewriteUrl" ClientIDMode="Static" runat="server" />
      <hcc:UrlsAssociated ID="UrlsAssociated1" runat="server" />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel"
        ><%=Localization.GetString("lblSwatchFolderName") %></label
      >
      <asp:TextBox ID="swatchpathfield" runat="server" />
    </div>
    <div class="hcFormItem hcFormItemLeft">
      <label class="hcLabel"
        ><%=Localization.GetString("lblHeaderContentColumn") %></label
      >
      <asp:DropDownList ID="ddlPreContentColumn" runat="server" />
    </div>
    <div class="hcFormItem hcFormItemRight">
      <label class="hcLabel"
        ><%=Localization.GetString("lblFooterContentColumn") %></label
      >
      <asp:DropDownList ID="ddlPostContentColumn" runat="server" />
    </div>
  </div>
  <ul class="hcActions">
    <li>
      <asp:LinkButton
        ID="btnUpdate"
        resourcekey="btnUpdate"
        runat="server"
        CssClass="hcPrimaryAction"
        OnClick="btnUpdate_Click"
      />
    </li>
    <li>
      <asp:LinkButton
        ID="btnSave"
        resourcekey="btnSave"
        runat="server"
        CssClass="hcSecondaryAction"
        OnClick="btnSave_Click"
      />
    </li>
    <li>
      <asp:LinkButton
        ID="btnCancel"
        resourcekey="btnCancel"
        runat="server"
        CssClass="hcSecondaryAction"
        CausesValidation="False"
        OnClick="btnCancel_Click"
      />
    </li>
  </ul>
</asp:Content>

