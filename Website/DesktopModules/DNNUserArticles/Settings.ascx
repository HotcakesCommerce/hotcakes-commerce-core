<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNUserArticles.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="RSSSettings" Src="~/desktopmodules/dnnarticle/modulesettings/RSSSettings.ascx" %>

<div class="dnnFormItem">
    <dnn:Label ID="lbModules" runat="server" Suffix=":" ></dnn:Label>
    <asp:DropDownList ID="cboModules" runat="server" AutoPostBack="true" CssClass="Normal">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbPageSize" runat="server"></dnn:Label>
    <asp:TextBox ID="txtPageSize" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtPageSize"
                                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbEnablePage" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkEnablePage" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbOrderField" runat="server"></dnn:Label>
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
    <dnn:label ID="lblCSS" runat="server" Suffix=":"></dnn:label>
    <asp:DropDownList ID="ddlCSS" runat="server">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":"></dnn:Label>
    <zldnn:selector runat="server" ID="ctlTemplateSelector" />
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbHeaderTemplate" runat="server" />
    <zldnn:selector runat="server" ID="ctlHeaderTemplateSelector" />
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbFooterTemplate" runat="server" />
     <zldnn:selector runat="server" ID="ctlFooterTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblRepeatLayout" runat="server"></dnn:Label>
    <asp:DropDownList ID="cboRepeatLayout" runat="server"  CssClass="Normal">
                <asp:ListItem Value="0">Table</asp:ListItem>
                <asp:ListItem Value="1">Flow</asp:ListItem>
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbPageControlType" runat="server" CssClass="SubHead"></dnn:Label>
    <asp:DropDownList ID="cboPageControlType" runat="server" Width="152px" CssClass="Normal">
                    <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
                </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbUserIDPassIn" runat="server" CssClass="SubHead"></dnn:Label>
    <asp:TextBox ID="txtUserIDPassIn" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbEnableRSS" runat="server" CssClass="SubHead"></dnn:Label>
    <asp:CheckBox ID="chkEnableRSS" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    
</div>



<zldnn:RSSSettings runat="server" ID="RSSSettings" />