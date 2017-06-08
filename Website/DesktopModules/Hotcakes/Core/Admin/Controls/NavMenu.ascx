<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.NavMenu" CodeBehind="NavMenu.ascx.cs" %>
<ul class="hcNavMenu">
	<asp:Repeater ID="rpMenuItems" runat="server">
		<ItemTemplate>
			<li<%#GetCurrentClass(Container) %>>
				<asp:HyperLink NavigateUrl='<%#GetUrl(Container) %>' Text='<%#Eval("Text") %>' runat="server" />
			</li>
		</ItemTemplate>
	</asp:Repeater>
</ul>
