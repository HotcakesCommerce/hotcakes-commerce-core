<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="GeoLocation.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.GeoLocation" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<asp:Content ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=PageTitle %></h1>
    <hcc:MessageBox ID="ucMessageBox" runat="server" AddValidationSummaries="false" />
    <div class="hcForm">
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="TimeZone" CssClass="hcLabel" />
            <asp:DropDownList ID="lstTimeZone" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <asp:Label runat="server" resourcekey="CurrencyCulture" CssClass="hcLabel" />
            <asp:DropDownList ID="lstCulture" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCulture_SelectedIndexChanged" CausesValidation="false" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton runat="server" ID="btnSaveChanges" resourcekey="btnSaveChanges" OnClick="btnSaveChanges_Click" CssClass="hcPrimaryAction" />
        </li>
    </ul>
</asp:Content>