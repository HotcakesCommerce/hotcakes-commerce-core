<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewArticleSettings.ascx.cs"
            Inherits="ZLDNN.Modules.DNNArticle.ViewArticleSettings" %>

<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="TemplateSelector.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="selectarticlesettings" Src="~/desktopmodules/DNNArticle/ModuleSettings/SelectArticleSettings.ascx" %>

<zldnn:selectarticlesettings id="SelectArticleSettings" runat="server"></zldnn:selectarticlesettings>

<div class="dnnFormItem">
     <dnn:label ID="lbRedirectPage" runat="server" ></dnn:label>
     <asp:DropDownList ID="cboRedirectPage" runat="server" Width="200px" CssClass="Normal">
            </asp:DropDownList>  
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbDisableSEO" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkDisableSEO" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
   <dnn:label ID="lbDisableRating" runat="server" ></dnn:label>
   <asp:CheckBox ID="chkDisableRating" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbDisableComment" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkDisableComment" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lblCSS" runat="server" Suffix=":"></dnn:label>
     <asp:DropDownList ID="ddlCSS" runat="server">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbUseMainModuleTemplate" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkUseMainModuleTemplate" AutoPostBack="true" runat="server" OnCheckedChanged="chkUseMainModuleTemplate_CheckedChanged"></asp:CheckBox>
       
</div>

<div class="dnnFormItem" id="divViewTemplate" runat="server">
    <dnn:label ID="lblViewTemplate" runat="server" Suffix=":"></dnn:label>
    <zldnn:selector runat="server" ID="ctlViewTemplateSelector" />
</div>

<div class="dnnFormItem" id="div1" runat="server">
    <dnn:label ID="lblPrintTemplate" runat="server" Suffix=":"></dnn:label>
    <zldnn:selector runat="server" ID="ctlPrintTemplateSelector" />
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbShowTitle" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkShowArticleTitle" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbShowExpiredArticle" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkShowExpiredArticle" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbCheckPermission" runat="server" />
    <asp:CheckBox ID="chkCheckPermission" runat="server" />
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbEnableViewCount" runat="server" />
    <asp:CheckBox ID="chkEnableViewCount" runat="server" />
</div>

