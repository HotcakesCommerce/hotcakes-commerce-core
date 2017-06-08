<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Header" CodeBehind="Header.ascx.cs" %>
<%@ Register Src="Controls/Menu.ascx" TagName="Menu" TagPrefix="hcc" %>
<%@ Register Src="Controls/Languages.ascx" TagPrefix="hcc" TagName="Languages" %>
<%@ Register Src="/DesktopModules/Hotcakes/ControlPanel/AdminControlBar.ascx" TagPrefix="hcc" TagName="AdminControlBar" %>

<%@ Import Namespace="Hotcakes.Commerce.Urls" %>

<% if (true) { %>
    <hcc:AdminControlBar ID="AdminControlBar" runat="server" />
<%} else { %>
<div id="branding">
	<div class="hcQuickLinks">
		<hcc:Languages runat="server" ID="ucLanguages" />
		<a runat="server" id="aHostAdmin">Superuser Admin</a>
		<a href="https://hotcakes.org/Community" target="_blank">Help</a>
		<a href="<%=HccUrlBuilder.RouteHccUrl(HccRoute.Logoff) %>">Log Out</a>
		<a href="<%=HccUrlBuilder.RouteHccUrl(HccRoute.Home) %>">Go To Store</a>
	</div>
</div>

<div id="mainmenu">
    <hcc:Menu ID="ucMenu" runat="server" />
</div>
<%} %>
