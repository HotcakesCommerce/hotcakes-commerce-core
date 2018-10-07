<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleArchive.Settings"
            Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>


<div class="dnnFormItem">
    <dnn:label ID="lbPortal" runat="server"></dnn:label>
    <asp:DropDownList ID="cboPortals" runat="server" AutoPostBack="True" Width="200"
        CssClass="Normal" OnSelectedIndexChanged="cboPortals_SelectedIndexChanged">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbModule" runat="server"></dnn:label>
    <asp:DropDownList ID="cboModule" runat="server" Width="200px" AutoPostBack="True"
        CssClass="Normal" OnSelectedIndexChanged="cboModules_SelectedIndexChanged">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblCategory" runat="server" ControlName="lblCategory" />
    <asp:DropDownList ID="cboCategory" runat="server" Width="200px" CssClass="Normal">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbTabs" runat="server" ControlName="lbTabs" />
    <asp:DropDownList ID="cboTabs" runat="server" Width="200px" CssClass="Normal">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbListTab" runat="server" ControlName="lbTabs" />
    <asp:DropDownList ID="cboListTab" runat="server" Width="200px" CssClass="Normal">
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbDisplayListInAnotherPage" runat="server" ControlName="lbDisplayListInAnotherPage" />
    <asp:CheckBox ID="chkDisplayListInAnotherPage" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbDisplayCategory" runat="server"></dnn:label>
    <asp:CheckBox ID="chkDisplayCategory" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbDisplyShowYears" runat="server"></dnn:label>
    <asp:CheckBox ID="chkDisplayShowYears" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbType" runat="server" />
    <asp:DropDownList ID="cboType" runat="server" Width="103px" CssClass="Normal">
        <asp:ListItem Selected="True" Value="year">Year</asp:ListItem>
        <asp:ListItem Value="month">Month</asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbShowAllYear" runat="server" ControlName="chkShowAllYear" />
    <asp:CheckBox ID="chkShowAllYear" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowAllYear_CheckedChanged" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbDefaultYear" runat="server" />
    <asp:DropDownList ID="cboYear" runat="server" Width="103px" CssClass="Normal">
    </asp:DropDownList>
</div>
<div class="dnnFormItem  dnnClear">
    <h2 id="H3" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshGeneral")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lbArchiveListPageSize" runat="server"></dnn:label>
            <asp:TextBox ID="txtArchiveListPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="txtArchiveListPageSize"
                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbArchiveListOrder" runat="server"></dnn:label>
            <asp:DropDownList ID="cboArchiveListOrder" runat="server" Width="62px" CssClass="Normal">
                <asp:ListItem Value="ASC">ASC</asp:ListItem>
                <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbArchiveRepeatColumn" runat="server"></dnn:label>
            <asp:TextBox ID="txtArchiveRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator1" runat="server" ControlToValidate="txtRepeatColumn" Type="Integer"
                MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbArchiveListTemplate" runat="server"></dnn:label>
            <asp:TextBox ID="txtArchiveListTemplate" TextMode="MultiLine" runat="server" Width="330"
                CssClass="NormalTextBox"></asp:TextBox>
        </div>
    </fieldset>
</div>
<div class="dnnFormItem  dnnClear">
    <h2 id="H1" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshArticleList")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lbExpiredOnly" runat="server" />
            <asp:DropDownList ID="cboDisplayArticles" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Active only</asp:ListItem>
                <asp:ListItem Value="2">Expired only</asp:ListItem>
                <asp:ListItem Value="4">Not Expired</asp:ListItem>
            </asp:DropDownList>
        </div>
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
            <dnn:label ID="lbRepeatDirection" runat="server"></dnn:label>
            <asp:DropDownList ID="cboRepeatDirection" runat="server" Width="152px" CssClass="NormalTextBox">
                <asp:ListItem Value="0">Horizontal</asp:ListItem>
                <asp:ListItem Value="1">Vertical</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbPageControlType" runat="server"></dnn:label>
            <asp:DropDownList ID="cboPageControlType" runat="server" Width="152px" CssClass="Normal">
                <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbRepeatColumn" runat="server"></dnn:label>
            <asp:TextBox ID="txtRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator4" runat="server" ControlToValidate="txtRepeatColumn" Type="Integer"
                MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblRepeatLayout" runat="server"></dnn:label>
            <asp:DropDownList ID="cboRepeatLayout" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">Table</asp:ListItem>
                <asp:ListItem Value="1">Flow</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbOrderField" runat="server"></dnn:label>
            <asp:DropDownList ID="cboOrderField" runat="server" Width="103px" CssClass="Normal">
                <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
                <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
                <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
                <asp:ListItem Value="CLICKS">View count</asp:ListItem>
                <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
                <asp:ListItem Value="RATING">Rating</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="cboOrder" runat="server" Width="62px" CssClass="Normal">
                <asp:ListItem Value="ASC">ASC</asp:ListItem>
                <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbShowFeaturedFirst" runat="server"></dnn:label>
            <asp:CheckBox ID="chkShowFeaturedFirst" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblCSS" runat="server" Suffix=":"></dnn:label>
            <asp:DropDownList ID="ddlCSS" runat="server">
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
            </dnn:label>
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
    </fieldset>
</div>
