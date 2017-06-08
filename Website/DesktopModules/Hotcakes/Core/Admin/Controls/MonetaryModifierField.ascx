<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.MonetaryModifierField" Codebehind="MonetaryModifierField.ascx.cs" %>
<asp:TextBox ID="MonetaryTextBox" runat="server"></asp:TextBox>
<asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="MonetaryTextBox"
    Display="Dynamic" ErrorMessage="Must be a monetary value." CssClass="errormessage" ForeColor=" ">*</asp:CustomValidator>
<asp:DropDownList ID="MonetaryDropDownList" runat="server">
    <asp:ListItem Selected="True">Set To</asp:ListItem>
    <asp:ListItem>Increase By Amount</asp:ListItem>
    <asp:ListItem>Decrease By Amount</asp:ListItem>
    <asp:ListItem>Increase By Percent</asp:ListItem>
    <asp:ListItem>Decrease By Percent</asp:ListItem>
</asp:DropDownList>
