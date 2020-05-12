<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_Edit"
    Title="Edit Category" CodeBehind="Categories_Edit.aspx.cs" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CategoryBreadCrumbTrail.ascx" TagName="CategoryBreadCrumbTrail" TagPrefix="hcc" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/UrlsAssociated.ascx" TagName="UrlsAssociated" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
    <div class="hcBlock">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="hypClose" resourcekey="hypClose" runat="server" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx" />
            </div>
        </div>
    </div>
    <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" resourcekey="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank"/>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        jQuery(function ($) {
            $("#NameField").change(function () {

                var rawName = $(this).val();
                var $urlField = $("#RewriteUrlField");

                if ($urlField.val() == "") {
                    $.post('CatalogHandler.ashx',
                        {
                            "method": "Slugify",
                            "name": rawName
                        },
                        function (data) {
                            console.log(data);
                            $urlField.val(data);
                        });
                }
            });
        });
    </script>
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <hcc:CategoryBreadCrumbTrail ID="CategoryBreadCrumbTrail1" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Main.Text") %></h2>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblName") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="NameField" runat="server" ClientIDMode="Static"/>
                <asp:RequiredFieldValidator ID="valName" resourcekey="valName" runat="server" CssClass="hcFormError" Display="Dynamic" ControlToValidate="NameField" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblDescription") %><i class="hcLocalizable"></i></label>
                <hcc:HtmlEditor ID="DescriptionField" runat="server" EditorHeight="175" EditorWidth="630" EditorWrap="true" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblPageTitle") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaTitleField" runat="server" Columns="30" MaxLength="512" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblMetaDescription") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaDescriptionField" runat="server" MaxLength="255" Width="630px" Height="75px" TextMode="MultiLine"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblMetaKeywords") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaKeywordsField" runat="server" Columns="30" MaxLength="255" Width="630px"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblSearchKeywords") %><i class="hcLocalizable"></i></label>
                <asp:TextBox ID="keywords" runat="server" Columns="30" MaxLength="512" Width="630px"/>
            </div>
            <div class="hcFormItem" runat="server" id="TaxonomyBlock">
                <label class="hcLabel"><%=Localization.GetString("lblTaxonomyTags") %></label>
                <asp:TextBox ID="txtTaxonomyTags" runat="server" TextMode="multiLine" Columns="40" Rows="3" Width="630px" />
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2><%=Localization.GetString("Display") %></h2>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkHidden" resourcekey="chkHidden" runat="server" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkShowTitle" resourcekey="chkShowTitle" runat="server" Checked="True" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblTemplate") %></label>
                <asp:DropDownList ID="TemplateList" runat="server" AutoPostBack="False" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblIcon") %></label>
                <hcc:ImageUploader runat="server" ShowRemoveAction="true" ID="ucIconImage" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("lblBanner") %></label>
                <hcc:ImageUploader runat="server" ShowRemoveAction="true" ID="ucBannerImage" />
            </div>
           
        </div>
    </div>
    <div class="hcForm hcClear">
        <h2><%=Localization.GetString("lblAdvanced") %></h2>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel"><%=Localization.GetString("lblPageSlug") %></label>
            <asp:TextBox ID="RewriteUrlField" ClientIDMode="Static" runat="server" /><br />
            <hcc:UrlsAssociated ID="UrlsAssociated1" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel"><%=Localization.GetString("lblHeaderContentColumn") %></label>
            <asp:DropDownList ID="PreContentColumnIdField" runat="server">
                <asp:ListItem Value="" resourcekey="None"/>
            </asp:DropDownList>
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel"><%=Localization.GetString("lblParentCategory") %></label>
            <asp:DropDownList runat="server" ID="ParentCategoryDropDownList"/>
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel"><%=Localization.GetString("lblFooterContentColumn") %></label>
            <asp:DropDownList ID="PostContentColumnIdField" runat="server">
                <asp:ListItem Value="" resourcekey="None"/>
            </asp:DropDownList>
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Sort Order</label>
            <asp:DropDownList ID="SortOrderDropDownList" runat="server">
                <asp:ListItem Value="1" resourcekey="Sort_Manual"/>
                <asp:ListItem Value="2" resourcekey="Sort_NameAsc"/>
                <asp:ListItem Value="6" resourcekey="Sort_NameDesc"/>
                <asp:ListItem Value="3" resourcekey="Sort_PriceAsc"/>
                <asp:ListItem Value="4" resourcekey="Sort_PriceDesc"/>
                <asp:ListItem Value="7" resourcekey="Sort_SkuAsc"/>
                <asp:ListItem Value="8" resourcekey="Sort_SkuDesc"/>
            </asp:DropDownList>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="UpdateButton" resourcekey="UpdateButton" runat="server" CssClass="hcPrimaryAction" OnClick="UpdateButton_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnSaveChanges" resourcekey="btnSaveChanges" runat="server" CssClass="hcSecondaryAction" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" runat="server" CssClass="hcSecondaryAction" CausesValidation="False" OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
