<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.FloatModifierField" Codebehind="FloatModifierField.ascx.cs" %>
<asp:TextBox ID="FloatTextBox" runat="server"></asp:TextBox>
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FloatTextBox"
    Display="Dynamic" ErrorMessage="Must be in the format ###.###" ValidationExpression="\d{1,7}.\d{1,10}" CssClass="errormessage" ForeColor=" ">*</asp:RegularExpressionValidator>
<asp:DropDownList ID="FloatDropDownList" runat="server">
    <asp:ListItem Selected="True">Set To</asp:ListItem>
    <asp:ListItem>Add To</asp:ListItem>
    <asp:ListItem>Subtract From</asp:ListItem>
</asp:DropDownList>
