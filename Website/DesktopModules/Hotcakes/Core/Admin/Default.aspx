<%@ Page Language="C#" MasterPageFile="Admin.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Default" Title="Dashboard" CodeBehind="Default.aspx.cs" Async="true" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="SetupWizard/Step0Dashboard.ascx" TagName="Step0Dashboard" TagPrefix="hcc" %>
<%@ Register Src="Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="Dashboard/Dashboard.ascx" TagPrefix="hcc" TagName="Dashboard" %>


<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="hcDashboard">
        <hcc:MessageBox ID="ucMessageBox" runat="server" EnableViewState="false" />
        <hcc:Step0Dashboard runat="server" id="ucStep0Dashboard" />
        <hcc:Dashboard runat="server" />
    </div>
</asp:Content>

