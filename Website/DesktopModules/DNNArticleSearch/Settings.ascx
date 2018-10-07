<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Settings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleSearch.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>
<div class="dnnFormItem">
    <dnn:Label ID="lbShowInDNNArticleList" runat="server" ControlName="lbShowInDNNArticleList" />
    <asp:CheckBox ID="chkShowInDNNArticleList" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowInDNNArticleList_CheckedChanged" />
</div>
<div class="dnnFormItem" runat="server" id="divDNNArticleList">
    <dnn:Label ID="lbDNNArticleList" runat="server" ControlName="lbDNNArticleList" />
    <asp:DropDownList ID="cboDNNArticleList" runat="server" Width="390px" CssClass="Normal">
    </asp:DropDownList>
</div>
<div runat="server" id="divList">
    <div class="dnnFormItem">
        <dnn:Label ID="lbModule" runat="server" dnnFormItem></dnn:Label>
        <asp:CheckBoxList ID="chkmodules" runat="server" CssClass="Normal">
        </asp:CheckBoxList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbTabs" runat="server" ControlName="lbTabs" />
        <asp:DropDownList ID="cboTabs" runat="server" Width="390px" CssClass="Normal">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbDisplayListInAnotherPage" runat="server" ControlName="lbDisplayListInAnotherPage" />
        <asp:CheckBox ID="chkDisplayListInAnotherPage" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbSearchListPage" runat="server" ControlName="lbTabs" />
        <asp:DropDownList ID="cboSearchListPage" runat="server" Width="390px" CssClass="Normal">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbPageSize" runat="server"></dnn:Label>
        <asp:TextBox ID="txtPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtPageSize"
            ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbEnablePage" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkEnablePage" runat="server"></asp:CheckBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbCheckPermission" runat="server" />
        <asp:CheckBox ID="chkCheckPermission" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCSS" runat="server" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="ddlCSS" runat="server">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbListTitle" runat="server"></dnn:Label>
        <zldnn:selector runat="server" ID="ctlTemplateSelector" />
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
        <dnn:Label ID="lblRepeatLayout" runat="server"></dnn:Label>
        <asp:DropDownList ID="cboRepeatLayout" runat="server" CssClass="Normal">
            <asp:ListItem Value="0">Table</asp:ListItem>
            <asp:ListItem Value="1">Flow</asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbPageControlType" runat="server" dnnFormItem></dnn:Label>
        <asp:DropDownList ID="cboPageControlType" runat="server" Width="152px" CssClass="Normal">
            <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
            <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbOrderField" runat="server" dnnFormItem></dnn:Label>
        <asp:DropDownList ID="cboOrderField" runat="server" Width="103px" CssClass="Normal">
            <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
            <asp:ListItem Value="TITLE">Title</asp:ListItem>
            <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
            <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
            <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
            <asp:ListItem Value="CLICKS">View count</asp:ListItem>
            <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
            <asp:ListItem Value="RATING">Rating</asp:ListItem>
            <asp:ListItem Value="ITEMID">Articel Id</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="cboOrder" runat="server" Width="62px" CssClass="Normal">
            <asp:ListItem Value="ASC">ASC</asp:ListItem>
            <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem" runat="server" visible="false">
        <dnn:Label ID="lbBasedOnCoreSearchEngin" runat="server"></dnn:Label>
        <asp:CheckBox ID="chkBasedOnCoreSearchEngin" runat="server"></asp:CheckBox>
    </div>
    <div class="dnnFormItem" runat="server" visible="False">
        <asp:LinkButton ID="cmdClearAndReindex" CssClass="CommandButton" resourcekey="cmdClearAndReindex"
            runat="server" OnClick="cmdClearAndReindex_Click"></asp:LinkButton>
        <asp:LinkButton ID="cmdReindex" runat="server" CssClass="CommandButton" resourcekey="cmdReindex"
            OnClick="cmdReindex_Click"></asp:LinkButton>
    </div>
</div>
