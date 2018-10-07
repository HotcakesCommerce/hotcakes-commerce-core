<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleViewSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.ArticleViewSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/dnnarticle/TemplateSelector.ascx" %>

<div class="dnnFormItem">
      <dnn:Label ID="lblViewTemplate" runat="server" Suffix=":"></dnn:Label>  
       <zldnn:selector runat="server" ID="ctlViewTemplateSelector" />   
</div>
<div class="dnnFormItem">
        <dnn:Label ID="lbRelatedArticleTemplate" runat="server" Suffix=":"></dnn:Label>
        <zldnn:selector runat="server" ID="ctlRelatedArticlesTemplate" />    
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbPageTemplate" runat="server" Suffix=":"></dnn:Label>
     <asp:DropDownList ID="cboPageTemplate" runat="server" Width="330px" >
            </asp:DropDownList>      
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbShowContentToPages" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkShowContentToPages" runat="server"></asp:CheckBox>    
</div>

<div class="dnnFormItem" runat="server" visible="False">
     <dnn:Label ID="lblEnablePrint" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkEnablePrint" runat="server"></asp:CheckBox>      
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbRelatedArticleListRepeatLayout" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboRelatedArticleListRepeatLayout" runat="server" >
                <asp:ListItem Value="0">Table</asp:ListItem>
                <asp:ListItem Value="1">Flow</asp:ListItem>
            </asp:DropDownList>     
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRelatedArticleLabelText" runat="server" />
    <asp:TextBox ID="txtRelatedArticleLabelText" runat="server" CssClass="NormalTextBox"></asp:TextBox>       
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbRelatedArticleLabelCSS" runat="server" />
      <asp:TextBox ID="txtRelatedArticleLabelCSS" runat="server" CssClass="NormalTextBox"></asp:TextBox>     
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRelatedArticleListCSS" runat="server" />       
     <asp:TextBox ID="txtRelatedArticleListCSS" runat="server" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbRelatedArticleListRepeatColumn" runat="server"></dnn:Label>
      <asp:TextBox ID="txtRelatedArticleListRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="txtRelatedArticleListRepeatColumn"
                Type="Integer" MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>    
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbRelatedArticleListRepeatDirection" runat="server"></dnn:Label>
       <asp:DropDownList ID="cboRelatedArticleListRepeatDirection" runat="server" Width="152px"
                    >
                    <asp:ListItem Value="0">Horizontal</asp:ListItem>
                    <asp:ListItem Value="1">Vertical</asp:ListItem>
                </asp:DropDownList>     
</div>
<div class="dnnFormItem">
       <dnn:Label ID="lbAttachmentTemplate" runat="server" />
        <asp:TextBox ID="txtAttachmentTemplate" runat="server" Width="330px" TextMode="MultiLine"
                CssClass="NormalTextBox" Height="50px"></asp:TextBox>    
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lblHideAttachedFileURL" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkHideAttachedFileURL" runat="server"></asp:CheckBox>     
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAttachmentLabelText" runat="server" />
      <asp:TextBox ID="txtAttachmentLabelText" runat="server" CssClass="NormalTextBox"></asp:TextBox>     
</div>
<div class="dnnFormItem">
       <dnn:Label ID="lbAttachmentLabelCSS" runat="server" />
       <asp:TextBox ID="txtAttachmentLabelCSS" runat="server" CssClass="NormalTextBox"></asp:TextBox>     
</div>
<div class="dnnFormItem">
       <dnn:Label ID="lbAttachmentListCSS" runat="server" />
        <asp:TextBox ID="txtAttachmentListCSS" runat="server" CssClass="NormalTextBox"></asp:TextBox>   
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbAttachmentListRepeatColumn" runat="server"></dnn:Label>
      <asp:TextBox ID="txtAttachementListRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtAttachementListRepeatColumn"
                Type="Integer" MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>    
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbAttachmentListRepeatDirection" runat="server"></dnn:Label>
        <asp:DropDownList ID="cboAttachmentListRepeatDirection" runat="server" Width="152px"
                    >
                    <asp:ListItem Value="0">Horizontal</asp:ListItem>
                    <asp:ListItem Value="1">Vertical</asp:ListItem>
                </asp:DropDownList>    
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbUseSEOTemplate" runat="server"></dnn:Label>     
     <asp:CheckBox ID="chkUseSEOTemplate" runat="server"></asp:CheckBox> 
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbSEOViewTitleTemplate" runat="server"></dnn:Label>
     <asp:TextBox ID="txtSEOViewTitleTemplate" runat="server" Width="300px" TextMode="MultiLine"
                Rows="5"></asp:TextBox>      
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbSEOViewDescriptionTemplate" runat="server"></dnn:Label>
     <asp:TextBox ID="txtSEOViewDescriptionTemplate" runat="server" Width="300px" TextMode="MultiLine"
                Rows="5"></asp:TextBox>      
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbSEOViewKeywordsTemplate" runat="server"></dnn:Label>
     <asp:TextBox ID="txtSEOViewKeywordsTemplate" runat="server" Width="300px" TextMode="MultiLine"
                Rows="5"></asp:TextBox>       
</div>
