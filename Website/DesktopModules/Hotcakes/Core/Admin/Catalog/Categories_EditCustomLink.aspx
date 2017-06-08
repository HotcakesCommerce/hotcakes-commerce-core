<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Categories_EditCustomLink.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories_EditCustomLink" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/CategoryBreadCrumbTrail.ascx" TagName="CategoryBreadCrumbTrail" TagPrefix="hcc" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>
<%@ Register Src="../Controls/UrlsAssociated.ascx" TagName="UrlsAssociated" TagPrefix="hcc" %>
<%@ Register Src="../Controls/ImageUploader.ascx" TagName="ImageUploader" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="ucNavMenu" Level="2" BaseUrl="catalog/category" runat="server" />
    <div class="hcBlockNoBorder">
        <asp:HyperLink ID="hypClose" runat="server" Text="Close" CssClass="hcSecondaryAction" NavigateUrl="Categories.aspx" />
    </div>
</asp:Content>

<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" />
    <hcc:CategoryBreadCrumbTrail ID="CategoryBreadCrumbTrail1" runat="server" />

    <div class="hcColumnLeft" style="width: 60%">
        <div class="hcForm">
            <h2>Main</h2>
            <div class="hcFormItem">
                <label class="hcLabel">Name<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="NameField" runat="server" ClientIDMode="Static" />
                <asp:RequiredFieldValidator ID="valName" runat="server" CssClass="hcFormError" Display="Dynamic"
                    ErrorMessage="Please enter a name" ControlToValidate="NameField" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Link To<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="LinkToField" runat="server" Columns="30" MaxLength="1024" />
            </div>
            <div class="hcFormItem">
                <label class="hcLabel">Page Title<i class="hcLocalizable"></i></label>
                <asp:TextBox ID="MetaTitleField" runat="server" Columns="30" MaxLength="512" />
            </div>
            <div class="hcFormItem">
                <asp:CheckBox ID="chkHidden" runat="server" Text="Hide Category" />
            </div>
        </div>
    </div>

    <div class="hcColumnRight hcLeftBorder" style="width: 39%">
        <div class="hcForm">
            <h2>Display</h2>
            <div class="hcFormItem">
                <label class="hcLabel">Icon</label>
                <hcc:ImageUploader runat="server" ShowRemoveAction="true" ID="ucIconImage" />
            </div>
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="UpdateButton" runat="server" Text="Save" CssClass="hcPrimaryAction" OnClick="UpdateButton_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnSaveChanges" runat="server" Text="Save Changes" CssClass="hcSecondaryAction"
                OnClick="btnSaveChanges_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="hcSecondaryAction" CausesValidation="False"
                OnClick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>

