<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.NavMenu" CodeBehind="NavMenu.ascx.cs" %>
<ul class="navmenu">
	<asp:Repeater ID="rpMenuItems" runat="server">
		<ItemTemplate>
			<li>
				<asp:HyperLink ID="hyperLink" runat="server" />
			</li>
		</ItemTemplate>
	</asp:Repeater>
</ul>
