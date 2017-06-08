<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.Menu" %>
<ul>
    <asp:Repeater ID="rpMenuTabs" runat="server">
        <ItemTemplate>
            <li>
                <asp:HyperLink NavigateUrl='<%#GetUrl(Container) %>' ToolTip="" runat="server" >
                    <i class='hcMenuIcon<%#Container.ItemIndex %>'></i><%#Eval("Text") %>
                </asp:HyperLink>
                <asp:Repeater ID="rpMenuItems" DataSource='<%#Eval("ChildItems") %>' runat="server">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:HyperLink NavigateUrl='<%#GetUrl(Container) %>' Text='<%#Eval("Text") %>' runat="server" />
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>