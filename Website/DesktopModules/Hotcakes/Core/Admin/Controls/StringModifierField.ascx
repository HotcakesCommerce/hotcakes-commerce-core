<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.StringModifierField" Codebehind="StringModifierField.ascx.cs" %>
<asp:TextBox ID="StringTextBox" runat="server"></asp:TextBox>
<asp:DropDownList ID="StringDropDownList" runat="server">
    <asp:ListItem Selected="True">Set To</asp:ListItem>
    <asp:ListItem>Append To</asp:ListItem>
    <asp:ListItem>Remove From End</asp:ListItem>
</asp:DropDownList>
