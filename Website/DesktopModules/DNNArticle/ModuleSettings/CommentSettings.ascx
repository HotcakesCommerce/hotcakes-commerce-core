<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.CommentSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/dnnarticle/TemplateSelector.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CommentAdvancedSettings" Src="~/desktopmodules/dnnarticle/modulesettings/CommentAdvancedSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CommentEmailSettings" Src="~/desktopmodules/dnnarticle/modulesettings/CommentEmailSettings.ascx" %>

<div class="dnnFormItem">
        <dnn:Label ID="lbEnableRating" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkEnableRating" runat="server"></asp:CheckBox>          
</div>
<div class="dnnFormItem">
       <dnn:Label ID="lbEnableComment" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkEnableComment" runat="server"></asp:CheckBox>          
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAutoapprovecomment" runat="server" />
     <asp:CheckBox ID="chkAutoapprovecomment" runat="server" />            
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbExpandComment" runat="server" />
     <asp:CheckBox ID="chkExpandComment" runat="server" />            
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbMultiRating" runat="server"></dnn:Label>    
     <asp:CheckBox ID="chkMultiRating" runat="server"></asp:CheckBox>         
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbMultiComment" runat="server"></dnn:Label>   
      <asp:CheckBox ID="chkMultiComment" runat="server"></asp:CheckBox>         
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbEnableCaptcha" runat="server"></dnn:Label>  
     <asp:CheckBox ID="chkEnableCaptcha" runat="server"></asp:CheckBox>          
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbDisableCaptchaForLoginUser" runat="server"></dnn:Label> 
    <asp:CheckBox ID="chkDisableCaptchaForLoginUser" runat="server"></asp:CheckBox>            
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbCommentTemplate" runat="server"></dnn:Label>
    <zldnn:selector runat="server" ID="ctlCommentTemplateSelector" />             
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbCommentPageSize" runat="server"></dnn:Label>
    <asp:TextBox ID="txtCommentPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator8" runat="server" ControlToValidate="txtCommentPageSize"
                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>             
</div>

<zldnn:CommentAdvancedSettings runat="server" ID="CommentAdvancedSettings" />

<zldnn:CommentEmailSettings runat="server" ID="CommentEmailSettings" />

