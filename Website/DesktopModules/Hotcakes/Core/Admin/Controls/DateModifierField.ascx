<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.DateModifierField" Codebehind="DateModifierField.ascx.cs" %>
<asp:TextBox ID="DateTextBox" runat="server"></asp:TextBox>
<asp:CustomValidator ID="CustomValidator1" runat="server" 
    ErrorMessage="Field must be in a date format." Display="Dynamic" 
    ControlToValidate="DateTextBox" CssClass="errormessage" ForeColor=" " 
    onservervalidate="CustomValidator1_ServerValidate">*</asp:CustomValidator>
<asp:DropDownList ID="DateDropDownList" runat="server">
    <asp:ListItem>Set To</asp:ListItem>
    <asp:ListItem>Add days</asp:ListItem>
    <asp:ListItem>Subtract Days</asp:ListItem>
    <asp:ListItem>Add Months</asp:ListItem>
    <asp:ListItem>Subtract Months</asp:ListItem>
    <asp:ListItem>Add Years</asp:ListItem>
    <asp:ListItem>Subtract Years</asp:ListItem>
</asp:DropDownList>
