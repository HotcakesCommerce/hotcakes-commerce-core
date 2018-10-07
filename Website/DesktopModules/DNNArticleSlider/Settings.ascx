<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleSlider.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="source" Src="~/desktopmodules/DNNArticle/ctlArticleSource.ascx" %>
<div class="dnnFormItem">
    <zldnn:source runat="server" ID="ctlSource" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbNumber" runat="server" CssClass="SubHead"></dnn:Label>
     <asp:TextBox ID="txtNumber" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="Rangevalidator11" runat="server" ControlToValidate="txtNumber" Type="Integer"
                MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbRecent" runat="server" CssClass="SubHead"></dnn:Label>
    <asp:TextBox ID="txtRecent" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtRecent"
                ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbFeaturedDisplayType" runat="server" />
    <asp:DropDownList ID="cboFeaturedDisplayType" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Featured only</asp:ListItem>
                <asp:ListItem Value="2">Not Featured only</asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbTabs" runat="server" ControlName="lbTabs"  CssClass="Normal"/>
    <asp:DropDownList ID="cboTabs" runat="server" Width="200px">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbDisplayRandom" runat="server" CssClass="SubHead"></dnn:Label>
    <asp:CheckBox ID="chkDisplayRandom" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbCheckPermission" runat="server" />
    <asp:CheckBox ID="chkCheckPermission" runat="server" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lblNextButtonText" runat="server" Suffix=":" />
     <asp:TextBox ID="txtNextButtonText" runat="server" Width="170px"  CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lblautorotate_time" runat="server" Suffix=":" />
      <asp:TextBox ID="txtautorotate_time" runat="server" Width="170px"  CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtautorotate_time"
                                ErrorMessage="(1-999)" MaximumValue="999" MinimumValue="1" Type="Integer"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lblCSS" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtCSS" CssClass="NormalTextBox" Width="300" Columns="30" TextMode="MultiLine"
                         Rows="10" MaxLength="2000" runat="server" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbOrderField" runat="server"  ></dnn:Label>
    <asp:DropDownList ID="cboOrderField" runat="server" Width="103px"  CssClass="Normal">
                <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
                <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
                <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
                <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
                <asp:ListItem Value="CLICKS">View count</asp:ListItem>
                <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
                <asp:ListItem Value="RATING">Rating</asp:ListItem>
            </asp:DropDownList><asp:DropDownList ID="cboOrder" runat="server" Width="62px"  CssClass="Normal">
                                   <asp:ListItem Value="ASC">ASC</asp:ListItem>
                                   <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
                               </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:Label ID="lbShowFeaturedFirst" runat="server"></dnn:Label>
      <asp:CheckBox ID="chkShowFeaturedFirst" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":"></dnn:Label>
    <zldnn:selector runat="server" ID="ctlTemplateSelector" />
</div>
