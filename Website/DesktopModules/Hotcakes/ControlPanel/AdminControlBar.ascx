<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AdminControlBar.ascx.cs" Inherits="Hotcakes.Modules.ControlPanel.AdminControlBar" %>

<%@ Import namespace="System.Data" %>
<%@ Import Namespace="DotNetNuke.Security.Permissions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register Src="/DesktopModules/Hotcakes/Core/Admin/Controls/Languages.ascx" TagPrefix="hcc" TagName="Languages" %>

<div id="ControlBar_ControlPanel">
    <asp:Panel ID="ControlPanel" runat="server">
	    <div id="ControlBar">
		    <div class="ControlContainer">
			    <div class="ServiceIcon">
				    <asp:Image ID="conrolbar_logo" runat="server" AlternateText="HCClogo" ViewStateMode="Disabled" />
				    <asp:Image ID="updateService" runat="server" AlternateText="" ViewStateMode="Disabled" />
			    </div>
			    <% if (UserController.Instance.GetCurrentUserInfo().IsInRole(PortalSettings.AdministratorRoleName)) {%>
				    <ul id="ControlNav" >
					    <asp:Repeater ID="rpMenuTabs" runat="server" OnItemDataBound="rpMenuTabs_ItemDataBound">
						    <ItemTemplate>
							    <li class="controlBar_ArrowMenu">
								    <a style="color:white !important;" href='<%#GetUrl(Container) %>'><%#Eval("Text") %></a>
									    <asp:Repeater ID="rpMenuItems" runat="server">
										    <HeaderTemplate><div class="subNav"><dl><dd><ul></HeaderTemplate>
										    <FooterTemplate></ul></dd></dl></div></FooterTemplate>
										    <ItemTemplate>
											    <li>
												    <a style="color:white !important;" href='<%#GetUrl(Container) %>'> <%#Eval("Text") %> </a>
											    </li>
										    </ItemTemplate>
									    </asp:Repeater>
							    </li>
						    </ItemTemplate>
					    </asp:Repeater>

					    <li class="controlBar_ArrowMenu">
						    <a style="color:white !important;" href="https://hotcakes.org/community" target="_blank">Help</a>
					    </li>
				    </ul>

				    <ul id="ControlNavQuickLink">
					    <li>
						    <a id="GoToStore" href="<%=Hotcakes.Commerce.Urls.HccUrlBuilder.RouteHccUrl(Hotcakes.Commerce.Urls.HccRoute.Home) %>"> <img src="/DesktopModules/Hotcakes/ControlPanel/controlbarimages/Store-icon.png"/> Go To Store</a>
					    </li>
					    <li>
						    <hcc:Languages runat="server" ID="ucLanguages" />
					    </li>
					    <li>
						    <a runat="server" id="aHostAdmin">Superuser Admin</a>
					    </li>
					    <li>
						    <a href="<%=Hotcakes.Commerce.Urls.HccUrlBuilder.RouteHccUrl(Hotcakes.Commerce.Urls.HccRoute.Logoff) %>">Log Out</a>
					    </li>
				    </ul>
			
			    <%} %>
		    </div>
	    </div>
    </asp:Panel>
</div>
<script type="text/javascript">
	if (typeof dnn === 'undefined') dnn = {};
	dnn.controlBarSettings = {
		searchInputId: 'ControlBar_SearchModulesInput',
		makeCopyCheckboxId: 'ControlBar_Module_chkCopyModule',
    	yesText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Yes.Text", Localization.SharedResourceFile)) %>',
		noText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("No.Text", Localization.SharedResourceFile)) %>',
		titleText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Confirm.Text", Localization.SharedResourceFile)) %>',
		defaultCategoryValue: 'All',

		loadingModulesId: 'ControlBar_ModuleListWaiter_LoadingMessage'
	};

</script>
