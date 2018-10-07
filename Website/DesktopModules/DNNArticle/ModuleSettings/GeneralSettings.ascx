<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.GeneralSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/dnnarticle/TemplateSelector.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="roleselector" Src="~/desktopmodules/dnnarticle/usercontrols/RoleSelector.ascx" %>

 <div class="dnnFormItem" >
     <dnn:Label ID="lbModuleId" runat="server" ControlName="lbTabs" />
     <asp:Label ID="ArticleModuleId" runat="server" ></asp:Label>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbWLWURL" runat="server" ControlName="lbTabs" />
      <asp:Label ID="WLWURL" runat="server" ></asp:Label>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbTabs" runat="server" ControlName="lbTabs" />
      <asp:DropDownList ID="cboTabs" runat="server"  >
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbEditTab" runat="server" ControlName="lbEditTab" />
      <asp:DropDownList ID="cboEditTabs" runat="server"  >
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbTagCloudModule" runat="server"></dnn:Label>
       <asp:DropDownList ID="cboTagCloudModule" runat="server"  >
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbPageSize" runat="server"></dnn:Label>
      <asp:TextBox ID="txtPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtPageSize"
                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbEnablePage" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkEnablePage" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbDisplayCategory" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkDisplayCategory" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lblDisplayAll" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkDisplayAll" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lblDisplayAllArticles" runat="server"></dnn:Label>
        <asp:DropDownList ID="cboDisplayArticles" runat="server" >
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Active only</asp:ListItem>
                <asp:ListItem Value="2">Expired only</asp:ListItem>
                <asp:ListItem Value="3">To be published only</asp:ListItem>
                <asp:ListItem Value="4">Not Expired</asp:ListItem>
            </asp:DropDownList>
 </div>
 
  <div class="dnnFormItem" >
      <dnn:Label ID="lbCheckPermission" runat="server" />
      <asp:CheckBox ID="chkCheckPermission" runat="server" />
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lblShowMyArticles" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkShowMyArticles" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbNumber" runat="server"></dnn:Label>
       <asp:TextBox ID="txtNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="Rangevalidator11" runat="server" ControlToValidate="txtNumber"
                Type="Integer" MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lblRepeatLayout" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboRepeatLayout" runat="server" >
                 <asp:ListItem Value="1">Flow</asp:ListItem>
                <asp:ListItem Value="0">Table</asp:ListItem>
                
            </asp:DropDownList>
 </div>
 <div class="dnnFormItem" >
       <dnn:Label ID="lbRepeatColumn" runat="server"></dnn:Label>
       <asp:TextBox ID="txtRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="txtRepeatColumn"
                Type="Integer" MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbRepeatDirection" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboRepeatDirection" runat="server" Width="152px" >
                    <asp:ListItem Value="0">Horizontal</asp:ListItem>
                    <asp:ListItem Value="1">Vertical</asp:ListItem>
                </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbShowEditIcon" runat="server" />
       <asp:CheckBox ID="chkShowEditIcon" runat="server" />
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbPageControlType" runat="server"></dnn:Label>
       <asp:DropDownList ID="cboPageControlType" runat="server" >
                    <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="PageNumberListWithoutPostBack"></asp:ListItem>
                </asp:DropDownList>
 </div>
 <div class="dnnFormItem" >
      <dnn:Label ID="lbEnableRSS" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkEnableRSS" runat="server"></asp:CheckBox>
 </div>
<div class="dnnFormItem" runat="server" visible="False" id="divExtraFieldOrder">
    <dnn:Label ID="lbExtraFieldOrder" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkExtraFeildOrder" runat="server" AutoPostBack="True" OnCheckedChanged="chkExtraFeildOrder_CheckedChanged" />
</div>
<div class="dnnFormItem" runat="server" visible="False" id="divExtraFieldOrderId">
    <dnn:Label ID="lbExtraFieldOrderId" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboExtraFieldOrderId" runat="server" Width="103px" CssClass="Normal">
    </asp:DropDownList>
    <asp:DropDownList ID="cboExtraOrder" runat="server" Width="62px" CssClass="Normal">
        <asp:ListItem Value="ASC">ASC</asp:ListItem>
        <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
    </asp:DropDownList>
</div>
  <div class="dnnFormItem" runat="server" id="divOrderField">
      <dnn:Label ID="lbOrderField" runat="server"></dnn:Label>
       <asp:DropDownList ID="cboOrderField" runat="server" Width="103px" >
                <asp:ListItem Value="PUBLISHDATE" Selected="True">Publish Date</asp:ListItem>
                <asp:ListItem Value="VIEWORDER" >ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
                <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
                <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
                <asp:ListItem Value="CLICKS">View count</asp:ListItem>
                <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
                <asp:ListItem Value="RATING">Rating</asp:ListItem>
                <asp:ListItem Value="ITEMID">Articel Id</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="cboOrder" runat="server" Width="62px" >
                <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
                <asp:ListItem Value="ASC" >ASC</asp:ListItem>
                
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbShowFeaturedFirst" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkShowFeaturedFirst" runat="server"></asp:CheckBox>
 </div>
 <div class="dnnFormItem" >
     <dnn:Label ID="lbSEOFriendly" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkSEOFriendly" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lblURL" runat="server" ControlName="lblURL" Suffix=":" />
        <asp:DropDownList ID="ddlDirectory" runat="server"  Width="304px">
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbTrackLinkClicks" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkTrackLinkClicks" runat="server"></asp:CheckBox>

 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbRefreshTrackURL" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkRefreshTrackURL" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbShowMeToRoles" runat="server"></dnn:Label>
      <zldnn:roleselector runat="server" ID="roleselector"></zldnn:roleselector>
      
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbFilterjavascript" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkFilterjavascript" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
            </dnn:Label>
 <zldnn:selector runat="server" ID="ctlTemplateSelector" />
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbHeaderTemplate" runat="server" />
       <zldnn:selector runat="server" ID="ctlHeaderTemplateSelector" />
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbFooterTemplate" runat="server" />
       <zldnn:selector runat="server" ID="ctlFooterTemplateSelector" />
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lblCSS" runat="server" Suffix=":"></dnn:Label>
       <asp:DropDownList ID="ddlCSS" runat="server">
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbReplaceEnvironmentTokens" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkEnvironmentTokens" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbTokenUser" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkTokenUser" runat="server"></asp:CheckBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbTextofNewContent" runat="server"></dnn:Label>
      <asp:TextBox ID="txtTextofNewContent" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div> 
 <div class="dnnFormItem" >
      <dnn:Label ID="lbReplaceArticleWith" runat="server"></dnn:Label>
      <asp:TextBox ID="txtReplaceArticleWith" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbReplaceArticlesWith" runat="server"></dnn:Label>
       <asp:TextBox ID="txtReplaceArticlesWith" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>
  <div class="dnnFormItem" >
       <dnn:Label ID="lbReplaceCategoryWith" runat="server"></dnn:Label>
       <asp:TextBox ID="txtReplaceCategoryWith" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>
  <div class="dnnFormItem" >
      <dnn:Label ID="lbReplaceCategoriesWith" runat="server"></dnn:Label>
      <asp:TextBox ID="txtReplaceCategoriesWith" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>
 <div class="dnnFormItem" >
     <dnn:Label ID="lbSearchIndexNumber" runat="server"></dnn:Label>
     <asp:TextBox ID="txtSearchIndexNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="Rangevalidator7" runat="server" ControlToValidate="txtSearchIndexNumber"
                Type="Integer" MinimumValue="1" MaximumValue="20000" ErrorMessage="(1-20000)"></asp:RangeValidator>
 </div>
 
  <div class="dnnFormItem" >
     <dnn:Label ID="lbSiteMapPriority" runat="server"></dnn:Label>
     <asp:TextBox ID="txtSiteMapPriority" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            
 </div>
 
   <div class="dnnFormItem" >
     <dnn:Label ID="lbSiteMapField" runat="server"></dnn:Label>
     <asp:DropDownList ID="cboSiteMapField" runat="server"  >
               
            </asp:DropDownList>
 </div>

  <div class="dnnFormItem" runat="server" visible="False">
      <dnn:Label ID="lbCacheType" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboCacheType" runat="server" Width="103px" >
                <asp:ListItem Value="0">None</asp:ListItem>
                <asp:ListItem Value="1">Moderate</asp:ListItem>
                <%--  <asp:ListItem Value="2">Heavy</asp:ListItem>--%>
            </asp:DropDownList>
 </div>
  <div class="dnnFormItem" runat="server" visible="False">
      <dnn:Label ID="lbEnableMultiCategories" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkEnableMultiCategories" runat="server"></asp:CheckBox>
 </div>
 <div class="dnnFormItem" runat="server" >
     <dnn:Label ID="lbUseCategoryAddLogic" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkCategoryAddLogic" runat="server"></asp:CheckBox>
 </div>
 
  <div id="divAllowSelectArticlesFromOtherPortal" class="dnnFormItem" runat="server" visible="False">
     <dnn:Label ID="lbAllowSelectArticlesFromOtherPortal" runat="server"></dnn:Label>
     <asp:CheckBox ID="chkAllowSelectArticlesFromOtherPortal" runat="server"></asp:CheckBox>
                    
 </div>
 <div class="dnnFormItem" >
       <dnn:Label ID="lbHideArticleIDInViewURL" runat="server"></dnn:Label>
       <asp:CheckBox ID="chkHideArticleIdInViewURL" runat="server"></asp:CheckBox>
 </div>
 
  <div class="dnnFormItem" >
       <dnn:Label ID="lbHideArticleIDPath" runat="server"></dnn:Label>
       <asp:TextBox ID="txtHideArticleIDPathTemplate" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>
 
 <div class="dnnFormItem">
    <dnn:Label ID="lbDefaultAlias" runat="server"></dnn:Label>
    <asp:TextBox ID="txtDefaultAlias" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
    </div>
    
 
<div class="dnnFormItem">
        <dnn:Label ID="lbGoogleMapApiKey" runat="server" ></dnn:Label>
   
        <asp:TextBox ID="txtGoogleMapApiKey" runat="server" CssClass="NormalTextBox"></asp:TextBox>
 </div>