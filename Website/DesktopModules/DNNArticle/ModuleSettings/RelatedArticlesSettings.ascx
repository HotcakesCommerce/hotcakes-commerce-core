<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedArticlesSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.RelatedArticlesSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>
<div class="dnnFormItem">
    <dnn:Label ID="lbNumber" runat="server"></dnn:Label>
    <asp:TextBox ID="txtNumber" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtNumber"
        ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbOrderField" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboOrderField" runat="server" Width="103px">
        <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
        <asp:ListItem Value="TITLE">Title</asp:ListItem>
        <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
        <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
        <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
        <asp:ListItem Value="CLICKS">View count</asp:ListItem>
        <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
        <asp:ListItem Value="RATING">Rating</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="cboOrder" runat="server" Width="62px">
        <asp:ListItem Value="ASC">ASC</asp:ListItem>
        <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
    </dnn:Label>
    <zldnn:selector runat="server" ID="ctlTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRelatedArticleListRepeatColumn" runat="server"></dnn:Label>
    <asp:TextBox ID="txtRelatedArticleListRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
    <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="txtRelatedArticleListRepeatColumn"
        Type="Integer" MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRelatedArticleListRepeatDirection" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboRelatedArticleListRepeatDirection" runat="server" Width="152px">
        <asp:ListItem Value="0">Horizontal</asp:ListItem>
        <asp:ListItem Value="1">Vertical</asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbRelatedArticleListRepeatLayout" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboRelatedArticleListRepeatLayout" runat="server">
        <asp:ListItem Value="1">Flow</asp:ListItem>
        <asp:ListItem Value="0">Table</asp:ListItem>
        
    </asp:DropDownList>
</div>