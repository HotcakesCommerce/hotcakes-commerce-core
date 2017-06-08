<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Caching.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Caching" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register src="../Controls/NavMenu.ascx" tagname="NavMenu" tagprefix="hcc" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("Caching") %></h1>
    <hcc:MessageBox ID="msg" runat="server" />
    
    <div class="hcForm" style="width:50%">
        <div class="hcFormItem">
            <%=Localization.GetString("CacheInfo") %>
        </div>
        <div class="hcFormItem">
            <asp:LinkButton ID="btnClearStoreCache" resourcekey="btnClearStoreCache" runat="server" OnClick="btnClearStoreCache_Click" CssClass="hcPrimaryAction" />
        </div>
    </div>
</asp:Content>
