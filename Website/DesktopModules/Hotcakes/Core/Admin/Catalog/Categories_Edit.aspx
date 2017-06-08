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
                <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcTertiaryAction" NavigateUrl="Categories.aspx" />
            </div>
        </div>
    </div>
    <div class="hcBlock hcBlockNotTopPadding">
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:HyperLink ID="lnkViewInStore" runat="server" CssClass="hcTertiaryAction" Target="_blank">View in Store</asp:HyperLink>
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
            <h2>Main</h2>
            <div class="hcFormItem">
                <label class="hcLabel">Name<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="NameField" runat="server" ClientIDMode="Static"/>
                <asp:RequiredFieldValidator ID="valName" runat="server" CssClass="hcFormError" Display="Dynamic"
                    ErrorMessage="Please enter a name" ControlToValidate="NameField" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Description<i class="hcLocalizable"></i></label>
                <hcc:HtmlEditor ID="DescriptionField" runat="server" EditorHeight="175" EditorWidth="630"
                    EditorWrap="true" TabIndex="2001" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Page Title<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaTitleField" runat="server" Columns="30" MaxLength="512" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Meta Description<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaDescriptionField" runat="server" MaxLength="255" Width="630px" Height="75px" TextMode="MultiLine"/>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Meta Keywords<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaKeywordsField" runat="server" Columns="30" MaxLength="255" TabIndex="2004"
                    Width="630px"></asp:TextBox>
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Search Keywords<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="keywords" runat="server" Columns="30" MaxLength="512" TabIndex="2005"
                    Width="630px"></asp:TextBox>
            </div>
            <div class="hcFormItem" runat="server" id="TaxonomyBlock">
                <label class="hcLabel">Taxonomy Tags</label>
                <asp:TextBox ID="txtTaxonomyTags" runat="server" TextMode="multiLine" Columns="40" Rows="3" Width="630px" TabIndex="3400"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="hcColumnRight hcLeftBorder" style="width: 49%">
        <div class="hcForm">
            <h2>Display</h2>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkHidden" runat="server" Text="Hide Category" TabIndex="2009" />
            </div>
            <div class="hcFormItem hcFormItem50p">
                <asp:CheckBox ID="chkShowTitle" runat="server" Text="Category Name is Visible" Checked="True" TabIndex="2011" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Template</label>
                <asp:DropDownList ID="TemplateList" runat="server" AutoPostBack="False" TabIndex="2011" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Icon</label>
                <hcc:ImageUploader runat="server" ShowRemoveAction="true" ID="ucIconImage" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Banner</label>
                <hcc:ImageUploader runat="server" ShowRemoveAction="true" ID="ucBannerImage" />
            </div>
           
        </div>
    </div>
    <div class="hcForm hcClear">
        <h2>Advanced</h2>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Page Name /</label>
            <asp:TextBox ID="RewriteUrlField" ClientIDMode="Static" runat="server" /><br />
            <hcc:UrlsAssociated ID="UrlsAssociated1" runat="server" />
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Header Content Column</label>
            <asp:DropDownList ID="PreContentColumnIdField" runat="server">
                <asp:ListItem Value=""> - None -</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Parent Category</label>
            <asp:DropDownList runat="server" ID="ParentCategoryDropDownList"/>
        </div>
        <div class="hcFormItem hcFormItemRight">
            <label class="hcLabel">Footer Content Column</label>
            <asp:DropDownList ID="PostContentColumnIdField" runat="server">
                <asp:ListItem Value=""> - None -</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="hcFormItem hcFormItemLeft">
            <label class="hcLabel">Sort Order</label>
            <asp:DropDownList ID="SortOrderDropDownList" runat="server">
                <asp:ListItem Value="1">Manual Order</asp:ListItem>
                <asp:ListItem Value="2">Product Name (a-z)</asp:ListItem>
                <asp:ListItem Value="6">Product Name (z-a)</asp:ListItem>
                <asp:ListItem Value="3">Product Price Ascending</asp:ListItem>
                <asp:ListItem Value="4">Product Price Descending</asp:ListItem>
                <asp:ListItem Value="7">Product SKU (a-z)</asp:ListItem>
                <asp:ListItem Value="8">Product SKU (z-a)</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="UpdateButton" runat="server" Text="Save" CssClass="hcPrimaryAction" OnClick="UpdateButton_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnSaveChanges" runat="server" Text="Save &amp; Close" CssClass="hcSecondaryAction" OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="False" 
                OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>
