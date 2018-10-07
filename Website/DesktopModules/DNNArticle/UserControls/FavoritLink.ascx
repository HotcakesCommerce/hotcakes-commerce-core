<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FavoritLink.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.FavoritLink" %>
<asp:Literal ID="ltmark" runat="server"></asp:Literal>
<asp:LinkButton ID="cmdAdd" runat="server" CommandName="Add"  onclick="cmdAdd_Click"></asp:LinkButton>
<asp:LinkButton
    ID="cmdRemove" runat="server" CommandName="Remove" onclick="cmdAdd_Click"></asp:LinkButton>