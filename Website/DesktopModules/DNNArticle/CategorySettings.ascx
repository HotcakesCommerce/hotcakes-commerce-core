<%@ Control Language="C#" AutoEventWireup="true" Codebehind="CategorySettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CategorySettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>

<div class="dnnFormItem">
    <dnn:Label ID="lbPortal" runat="server" dnnFormItem></dnn:Label>
    <asp:DropDownList ID="cboPortals" runat="server" AutoPostBack="True" Width="200"
                 OnSelectedIndexChanged="cboPortals_SelectedIndexChanged">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbModules" runat="server" Suffix=":"></dnn:Label>
    <asp:DropDownList ID="cboModules" runat="server" AutoPostBack="true" Width="300px"
                 OnSelectedIndexChanged="cboModules_SelectedIndexChanged">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lblCategory" runat="server" ControlName="lblCategory" Suffix=":" />
    <asp:DropDownList ID="cboCategory" runat="server" Width="300px" 
                 >
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbArticleList" runat="server" Suffix=":"></dnn:Label>
    <asp:DropDownList ID="cboArticleList" runat="server"  Width="300px">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbTabs" runat="server" ControlName="lbTabs" />
    <asp:DropDownList ID="cboTabs" runat="server" Width="300px" >
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbDisplayArticle" runat="server" Suffix=":" />
     <asp:CheckBox ID="chkDisplayArticle" runat="server" Width="120px" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbEnableCategoryLink" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkEnableCategoryLink" runat="server" />
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbShowCategoryImage" runat="server" Suffix=":" />
     <asp:CheckBox ID="chkShowCategoryImage" runat="server" Width="120px" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbTreeNodeImage" runat="server" Suffix=":" ControlName="lbTreeNodeImage">
            </dnn:Label>
    <Portal:URL ID="ctlTreeNodeImage" runat="server" Width="200" ShowTabs="False" ShowUrls="False"
                ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True" UrlType="F"
                ShowNewWindow="False"></Portal:URL>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbArticleImageURL" runat="server" Suffix=":" />
    <Portal:URL ID="ctlArticleImageURL" runat="server" Required="false" ShowLog="False"
                ShowNewWindow="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="True" ShowUrls="False"
                UrlType="F" Width="200" />
</div>
<%--
<div class="dnnFormItem">
    <dnn:Label ID="lbTreeViewStyle" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboTreeViewStyle" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboTreeViewStyle_SelectedIndexChanged">
                 <asp:ListItem Text="RadTreeView" Value="1"></asp:ListItem>
                <asp:ListItem Text="RadMenu" Value="2"></asp:ListItem>  
                <asp:ListItem Text="RadTreeView in DropDownList" Value="3"></asp:ListItem>  
                <asp:ListItem Text="Asp.Net TreeView" Value="0"></asp:ListItem>
                
            </asp:DropDownList>
</div>

<div class="dnnFormItem" runat="server" id="divMenuDirection">
    <dnn:Label ID="lbMenuDirection" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboMenuDirection" runat="server">
                <asp:ListItem Text="Vertical" Value="0"></asp:ListItem>
                <asp:ListItem Text="Horizontal" Value="1"></asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem" runat="server" id="divSkinType">
    <dnn:Label ID="lbSkinType" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboSkinType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboSkinType_SelectedIndexChanged">
                <asp:ListItem Text="Default" Value="Default"></asp:ListItem>
                <asp:ListItem Text="Custom" Value="Custom"></asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem" runat="server" id="divSkin">
    <dnn:Label ID="lbEditPageStyle" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboEditPageStyle" runat="server">
                <asp:ListItem Text="Default" Value="Default"></asp:ListItem>
                <asp:ListItem Text="Vista" Value="Vista"></asp:ListItem>
                <asp:ListItem Text="Forest" Value="Forest"></asp:ListItem>
                <asp:ListItem Text="Hay" Value="Hay"></asp:ListItem>
                <asp:ListItem Text="Office2007" Value="Office2007"></asp:ListItem>
                <asp:ListItem Text="Outlook" Value="Outlook"></asp:ListItem>
                <asp:ListItem Text="Simple" Value="Simple"></asp:ListItem>
                <asp:ListItem Text="Sunset" Value="Sunset"></asp:ListItem>
                <asp:ListItem Text="Windows7" Value="Windows7"></asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem" runat="server" id="divCustomSkin">
    <dnn:Label ID="lbCustomSkin" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboCustomSkin" runat="server">
            </asp:DropDownList>
</div>--%>
<div class="dnnFormItem" id="tr1" runat="server">
    <dnn:Label ID="lbDoNotAddCategoryURL" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkDoNotAddCategoryURL" runat="server" />
</div>

<div class="dnnFormItem" runat="server" id="divTree1">
    <dnn:Label ID="lbShowLines" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkShowLines" runat="server" />
</div>

<div class="dnnFormItem" runat="server" id="divTree2">
    <dnn:Label ID="lbIndent" runat="server" Suffix=":" />
    <asp:TextBox ID="txtIndent" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtIndent"
                ErrorMessage="(0-200)" MaximumValue="200" MinimumValue="0" Type="Integer"></asp:RangeValidator>
</div>
<div class="dnnFormItem" runat="server" id="divTree3">
    <dnn:Label ID="lbCollapseImageURL" runat="server" Suffix=":" />
    <Portal:URL ID="ctlCollapseImageURL" runat="server" Required="false" ShowLog="False"
                ShowNewWindow="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="True" ShowUrls="False"
                UrlType="F" Width="200" />
</div>

<div class="dnnFormItem" runat="server" id="divTree4">
    <dnn:Label ID="lbExpandImageURL" runat="server" Suffix=":" />
    <Portal:URL ID="ctlExpandImageURL" runat="server" Required="false" ShowLog="False"
                ShowNewWindow="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="True" ShowUrls="False"
                UrlType="F" Width="200" />
</div>
<div class="dnnFormItem" runat="server" id="divTree5">
    <dnn:Label ID="lbSelectedNodeStyle" runat="server" Suffix=":" />
    <asp:TextBox ID="txtSelectedNodeStyle" runat="server" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem" runat="server" id="divTree6">
    <dnn:Label ID="lbNodeStyle" runat="server" Suffix=":" />
    <asp:TextBox ID="txtNodeStyle" runat="server" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem" id="divTree7" runat="server">
    <dnn:Label ID="lbEnableIMC" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkEnableIMC" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbArticleTitleLength" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtArtitleTitleLength" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtArtitleTitleLength"
                ErrorMessage="(1-1000)" MinimumValue="1" MaximumValue="1000" Type="Integer"></asp:RangeValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblShowMeToAdminOnly" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkShowMeToAdminOnly" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbCategoryNodeTemplate" runat="server" Suffix=":" />
     <asp:TextBox ID="txtCategoryNodeTemplate" runat="server" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbShowAllCategory" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkShowAllCategory" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbCategoryViewOrder" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboCategoryViewField" runat="server" Width="103px" >
                <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="cboCategoryOrder" runat="server" Width="62px" >
                <asp:ListItem Value="ASC" Selected="True">ASC</asp:ListItem>
                <asp:ListItem Value="DESC">DESC</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbOrderField" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboOrderField" runat="server" Width="103px" >
                <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
                <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
                <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
                <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
                <asp:ListItem Value="CLICKS">View count</asp:ListItem>
                <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
                <asp:ListItem Value="RATING">Rating</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="cboOrder" runat="server" Width="62px" >
                <asp:ListItem Value="ASC">ASC</asp:ListItem>
                <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbPopularonDemand" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkPopularonDemand" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbExpandNode" runat="server" Suffix=":" />
     <asp:CheckBox ID="chkExpandNode" runat="server" Width="120px" />
</div>

