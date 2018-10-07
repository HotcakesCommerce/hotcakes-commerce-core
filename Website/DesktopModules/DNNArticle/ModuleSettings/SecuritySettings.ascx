<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecuritySettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.SecuritySettings" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
    <dnn:Label ID="lbAutoApproved" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkAutoApproved" runat="server"></asp:CheckBox>
</div>


<div class="dnnFormItem">
    <dnn:Label ID="lbAuthorCanEdit" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkAuthorCanEdit" runat="server"></asp:CheckBox>
</div>


<div class="dnnFormItem">
    <dnn:Label ID="lbAuthorCanEditArticleButNotComment" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkAuthorCanEditArticleButNotComment" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbLockApproved" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkLockApproved" runat="server"></asp:CheckBox>
</div>
