<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.EditSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="cbocategoryselector" Src="~/desktopmodules/dnnarticle/usercontrols/cbocategoryselector.ascx" %>



<div class="dnnFormItem">
     <dnn:Label ID="lbAllowPage" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowPage" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>

<div class="dnnFormItem">
      <dnn:Label ID="lbAllowSetCategory" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowSetCategory" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>

<div class="dnnFormItem" runat="server" id="divOrCategory">
     <dnn:Label ID="lbDefaultCategories" runat="server" Suffix=":" />
     
                
                <zldnn:cbocategoryselector runat="server" ID="defaultCategories" />
          
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbSelectAtLeastOneCategory" runat="server"></dnn:Label>
 <asp:CheckBox ID="chkSelectAtLeastOneCategory" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbUseTreeViewCategory" runat="server"></dnn:Label>
 <asp:CheckBox ID="chkUseTreeViewCategory" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowSetTags" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowSetTags" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbTagEnter" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboTatEnter" runat="server" >
               <asp:ListItem Value="1" resourcekey="ItFromText"></asp:ListItem>
                <asp:ListItem Value="0" resourcekey="ItFromCheckList"></asp:ListItem>
       </asp:DropDownList>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbTagNumber" runat="server"></dnn:Label>
     <asp:TextBox ID="txtTagNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator6" runat="server" ControlToValidate="txtTagNumber"
                ErrorMessage="(1-99)" Type="Integer" MaximumValue="99" MinimumValue="1"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbTagSpliter" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboTagSpliter" runat="server" Visible="false" >
                <asp:ListItem Value="," resourcekey="Itcomma"></asp:ListItem>
                <asp:ListItem Value=";" resourcekey="Itsemicolon"></asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="txtTagSpliter" runat="server"></asp:TextBox>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbDefaultTags" runat="server"></dnn:Label>
     <asp:TextBox ID="txtDefaultTags" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbSelectAtLeastOneTag" runat="server"></dnn:Label>
 <asp:CheckBox ID="chkSelectAtLeastOneTag" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbSetImageIfNoImage" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkSetTagImageIfNoImage" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowDescription" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowDescription" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowContent" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowContent" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowRelatedArticle" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowRelatedArticle" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbRelatedArticleType" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboRelatedArticleType" runat="server" >
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Active only</asp:ListItem>
                <asp:ListItem Value="2">Expired only</asp:ListItem>
                <asp:ListItem Value="3">To be published only</asp:ListItem>
                <asp:ListItem Value="4">Not Expired</asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbRelatedArticleModules" runat="server"></dnn:Label>
      <asp:CheckBoxList ID="chkRelatedArticleModules" runat="server" >
            </asp:CheckBoxList>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbAllowSetPermission" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowSetPermission" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowAttachment" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowAttachment" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbAllowSetAttachmentViewPermission" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowSetAttachmentViewPermission" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbAllUserSelecteFile" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkAllUserSelecteFile" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowSEO" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowSEO" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAutoFillSEOField" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkAutoFillSEOField" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAutoUpdateSEOField" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAutoUpdateSEOField" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbSEOTitleTemplate" runat="server" ControlName="lbEmailSubject" />
     <asp:TextBox ID="txtSEOTitleTemplate" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbSEODescriptionTemplate" runat="server" ControlName="lbEmailSubject" />
      <asp:TextBox ID="txtSEODescriptionTemplate" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowImage" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowImage" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbShowUserFolderOnlyInImage" runat="server"></dnn:Label>
 <asp:CheckBox ID="chkShowUserFolderOnlyInImage" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowUploadImageOnly" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowUploadImageOnly" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowRelatedURL" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboAllowRelatedURL" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowExtraField" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboAllowExtraField" runat="server" >
        <asp:ListItem Value="0" resourcekey="ItNone"></asp:ListItem>
        <asp:ListItem Value="1" resourcekey="ItApprovers"></asp:ListItem>
        <asp:ListItem Value="2" resourcekey="ItBoth"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbDisableTheBrowserFileInRelatedURL" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkDisableTheBrowserFileInRelatedURL" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbTemplateArticleID" runat="server"></dnn:Label>
     <asp:TextBox ID="txtTemplateArticleID" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtTemplateArticleID"
                MaximumValue="999999" MinimumValue="1" Type="Integer" ErrorMessage="Integer"></asp:RangeValidator>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbDefaultContentTemplate" runat="server"></dnn:Label>
     <asp:TextBox ID="txtDefaultContentTemplate" runat="server" TextMode="MultiLine" Width="330px"
                CssClass="NormalTextBox" Height="200px"></asp:TextBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowVersion" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkAllowVersion" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbMaxVersionNumber" runat="server"></dnn:Label>
      <asp:TextBox ID="txtMaxVersionNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                Type="Integer" MaximumValue="1000" MinimumValue="1" ControlToValidate="txtMaxVersionNumber"
                ID="RangeValidator5" runat="server" ErrorMessage="(1-1000)"></asp:RangeValidator>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbUsingAdminSkin" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkUsingAdminSkin" runat="server"></asp:CheckBox>

</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowAddResource" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowAddResource" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbAllowAuthorEnableDisableComment" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowAuthorEnableDisalbeComment" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbRTLStyle" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkRTLStyle" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbUseHTMLEditorAsTemplateEditor" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkUseHTMLEditorAsTemplateEditor" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbAllowUserControlPostToTwitter" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowUserControlPostToTwitter" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbAllowAddCategoryWhenEdit" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowAddCategoryWhenEdit" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbDebug" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkDebug" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbEnableS3" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkEnableS3"  runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbAccessKey" runat="server"></dnn:Label>
    <asp:TextBox ID="txtAccessKey" runat="server" Width="300px"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbSecurityKey" runat="server"></dnn:Label>
    <asp:TextBox ID="txtSecurityKey" runat="server" Width="300px"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbBulk" runat="server"></dnn:Label>
    <asp:TextBox ID="txtBulk" runat="server" Width="126px"></asp:TextBox>
</div>