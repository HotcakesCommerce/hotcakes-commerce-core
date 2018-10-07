<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.RssList" %>
<asp:DataList ID="lst" DataKeyField="ItemID" runat="server" CellPadding="4" Width="95%">
    <ItemTemplate>
        <asp:HyperLink ID="HyperLink1" NavigateUrl='<%#BuildURL(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "ItemID")))%>'
                       Target="_blank" runat="server">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/rss.gif" AlternateText="xml" /></asp:HyperLink>
        <asp:Label ID="lblContent" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Title")%>'
                   CssClass="Normal" />
    </ItemTemplate>
</asp:DataList>