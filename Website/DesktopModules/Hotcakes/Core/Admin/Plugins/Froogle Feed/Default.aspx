<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Plugins.FroogleFeed.Default" title="Froogle/GoogleBase Generator" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<strong><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></strong><br />
&nbsp;<br />
<table border="0" cellspacing="0" cellpadding="5">
<tr>
    <td class="formlabel">Google Base Username:</td>
    <td class="formfield"><asp:TextBox ID="gbaseUsernameField" runat="server" Width="200"></asp:TextBox></td>
</tr>
<tr>
    <td class="formlabel">Google Base Password:</td>
    <td class="formfield"><asp:TextBox ID="gbasePasswordField" runat="server" Width="200"></asp:TextBox></td>
</tr>
<tr>
    <td class="formlabel">Google Base File Name:</td>
    <td class="formfield"><asp:TextBox ID="gbaseFileNameField" runat="server" Width="200" Text="Froogle.txt"></asp:TextBox></td>
</tr>
<tr>
    <td class="formlabel">Google Base FTP:</td>
    <td class="formfield"><asp:TextBox ID="gbaseFtpField" runat="server" Width="200" Text="ftp://uploads.google.com"></asp:TextBox></td>
</tr>
<tr>
    <td colspan="2"><h2>Feed Options</h2></td>
</tr>
<tr>
    <td class="formlabel">Product Condition:</td>
    <td class="formfield"><asp:DropDownList ID="lstCondition" runat="server">
        <asp:ListItem Value="new">New</asp:ListItem>
        <asp:ListItem Value="used">Used</asp:ListItem>
        <asp:ListItem Value="refurbished">Refurbished</asp:ListItem>
    </asp:DropDownList></td>
</tr>
<tr>
    <td class="formlabel">&nbsp;<asp:Button ID="btnGo" runat="server" 
            Text="Generate Feed Only" onclick="btnGo_Click" />
    </td>
    <td class="formfield"><asp:Button ID="btnGenerateAndSend" runat="server" 
            Text="Generate Feed and FTP to Google" onclick="btnGenerateAndSend_Click" /></td>
</tr>
</table>
<asp:TextBox Wrap="false" TextMode="MultiLine" Width="900" Height="300" runat="server" ID="OutputField"></asp:TextBox>

</asp:Content>

