<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNUserArticles.UserList" AutoEventWireup="true" Explicit="True" Codebehind="UserList.ascx.cs" %>
<asp:DataList ID="lst" runat="server" EnableViewState="false" OnItemDataBound="lst_ItemDataBound">
    <ItemTemplate>
        <asp:Label ID="lblContent" runat="server" CssClass="Normal" />
    </ItemTemplate>
</asp:DataList>