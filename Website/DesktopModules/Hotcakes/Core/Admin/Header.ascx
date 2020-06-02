<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Header" CodeBehind="Header.ascx.cs" %>
<%@ Register Src="Controls/Menu.ascx" TagName="Menu" TagPrefix="hcc" %>
<%@ Register Src="Controls/Languages.ascx" TagPrefix="hcc" TagName="Languages" %>
<%@ Import Namespace="Hotcakes.Commerce.Urls" %>

<div id="branding">
	<div class="hcQuickLinks">
		<hcc:Languages runat="server" ID="ucLanguages" />
		<a runat="server" id="aHostAdmin"><%=Localization.GetString("SuperuserAdmin") %></a>
		<a href="https://hotcakescommerce.zendesk.com/hc" target="_blank"><%=Localization.GetString("Support") %></a>
		<a href="<%=HccUrlBuilder.RouteHccUrl(HccRoute.Logoff) %>"><%=Localization.GetString("LogOut.Text") %></a>
		<a href="<%=HccUrlBuilder.RouteHccUrl(HccRoute.Home) %>"><%=Localization.GetString("BackToStore") %></a>
	</div>
</div>

<div id="mainmenu">
    <hcc:Menu ID="ucMenu" runat="server" />
</div>