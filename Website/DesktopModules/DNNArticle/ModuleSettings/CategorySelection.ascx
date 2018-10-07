<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategorySelection.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.CategorySelection" %>
<%@ Register TagPrefix="zldnn" TagName="cbocategoryselector" Src="~/desktopmodules/dnnarticle/usercontrols/cbocategoryselector.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>


<div class="dnnFormItem">
    <dnn:Label ID="lbPortal" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboPortals" runat="server" AutoPostBack="True" Width="200"
        OnSelectedIndexChanged="cboPortals_SelectedIndexChanged">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbModule" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboModule" runat="server" Width="300px" AutoPostBack="True"
        OnSelectedIndexChanged="cboModule_SelectedIndexChanged">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbTabs" runat="server" ControlName="lbTabs" />
    <asp:DropDownList ID="cboTabs" runat="server" Width="300px">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbArticleList" runat="server" Suffix=":"></dnn:Label>
    <asp:DropDownList ID="cboArticleList" runat="server" AutoPostBack="true" Width="300px">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbSelectCategoryToshow" runat="server" ControlName="lbTabs" />
    <asp:CheckBox ID="chkShowExistingCategories" AutoPostBack="true" runat="server" OnCheckedChanged="chkShowExistingCategories_CheckedChanged" />
</div>
<div class="dnnFormItem" runat="server" id="divShowCategory">
    <dnn:Label ID="lbSelectCategories" runat="server" ControlName="lbTabs" />
       <zldnn:cbocategoryselector runat="server" ID="showCategories" />
   
</div>
<div class="dnnFormItem" runat="server" id="divShowHasArticleOnly">
    <dnn:Label ID="lbShowHasArticleOnly" runat="server" ControlName="chkShowHasArticleOnly" />
    <asp:CheckBox ID="chkShowHasArticleOnly" runat="server" />
</div>
<div class="dnnFormItem" runat="server" id="divShowTopLevelOnly">
    <dnn:Label ID="lbShowTopLevelOnly" runat="server" ControlName="chkShowTopLevelOnly" />
    <asp:CheckBox ID="chkShowTopLevelOnly" runat="server" />
</div>
<div class="dnnFormItem" runat="server" id="divCategoryViewOrder">
    <dnn:Label ID="lbCategoryViewOrder" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboCategoryViewField" runat="server" Width="103px">
        <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
        <asp:ListItem Value="TITLE">Title</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="cboCategoryOrder" runat="server" Width="62px">
        <asp:ListItem Value="ASC" Selected="True">ASC</asp:ListItem>
        <asp:ListItem Value="DESC">DESC</asp:ListItem>
    </asp:DropDownList>
</div>
