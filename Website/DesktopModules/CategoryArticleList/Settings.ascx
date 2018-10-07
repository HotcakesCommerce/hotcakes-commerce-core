<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.CategoryArticleList.Settings"
            Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CategorySelection" Src="~/desktopmodules/DNNArticle/ModuleSettings/CategorySelection.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="RelatedArticlesSettings" Src="~/desktopmodules/DNNArticle/ModuleSettings/RelatedArticlesSettings.ascx" %>


<zldnn:CategorySelection runat="server" ID="CategorySelection" />
<div class="dnnFormItem">
    <dnn:Label ID="lbRecent" runat="server"></dnn:Label>
    <asp:TextBox ID="txtRecent" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtRecent"
        ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRepeatColumn" runat="server"></dnn:Label>
    <asp:TextBox ID="txtRepeatColumn" runat="server"></asp:TextBox><asp:RangeValidator
        ID="RangeValidator4" runat="server" ControlToValidate="txtRepeatColumn" Type="Integer"
        MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblRepeatLayout" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboRepeatLayout" runat="server">
        <asp:ListItem Value="1">Flow</asp:ListItem>
        <asp:ListItem Value="0">Table</asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbCategoryTemplate" runat="server" Suffix=":"></dnn:Label>
    <zldnn:selector runat="server" ID="ctlCategoryTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbHeaderTemplate" runat="server" />
    <zldnn:selector runat="server" ID="ctlHeaderTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbFooterTemplate" runat="server" />
    <zldnn:selector runat="server" ID="ctlFooterTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblCSS" runat="server" Suffix=":"></dnn:Label>
    <asp:DropDownList ID="ddlCSS" runat="server">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
            <dnn:label ID="lbShowMessageIfNoRecord" runat="server" />
            <asp:CheckBox ID="chkShowMessageIfNoRecord" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbMessageofNoRecord" runat="server" />
            <asp:TextBox ID="txtMessageofNoRecord" runat="server"></asp:TextBox>
        </div>
<zldnn:RelatedArticlesSettings runat="server" ID="RelatedArticlesSettings" />

<div class="dnnFormItem">
            <dnn:label ID="lbPageSize" runat="server"></dnn:label>
            <asp:TextBox ID="txtPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtPageSize"
                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbEnablePage" runat="server"></dnn:label>
            <asp:CheckBox ID="chkEnablePage" runat="server"></asp:CheckBox>
        </div>
         <div class="dnnFormItem">
            <dnn:label ID="lbPageControlType" runat="server"></dnn:label>
            <asp:DropDownList ID="cboPageControlType" runat="server" Width="152px" CssClass="Normal">
                <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
                <asp:ListItem Value="2" resourcekey="PageNumberListWithoutPostBack"></asp:ListItem>
            </asp:DropDownList>
        </div>
