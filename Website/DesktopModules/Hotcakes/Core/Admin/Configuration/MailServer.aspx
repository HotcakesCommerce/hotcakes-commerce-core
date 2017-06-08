<%@ Page Language="C#" MasterPageFile="../AdminNav_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.MailServer" title="Untitled Page" Codebehind="MailServer.aspx.cs" %>
<%@ Register src="NavMenu.ascx" tagname="NavMenu" tagprefix="uc2" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc2:NavMenu ID="NavMenu1" Path="Item[@Text='Settings']" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>Mail Server</h1>        
   <uc1:MessageBox ID="msg" runat="server" />    
    <asp:Label id="lblError" runat="server" CssClass="errormessage"></asp:Label>
    
<asp:RadioButtonList ID="lstMailServerChoice" runat="server" AutoPostBack="true" 
        onselectedindexchanged="lstMailServerChoice_SelectedIndexChanged">
    <asp:ListItem Value="0">Use the built-in mail server to send messages</asp:ListItem>
    <asp:ListItem Value="1">Use my own mail server settings</asp:ListItem>
</asp:RadioButtonList>
<div class="editorcontrols">

                    <asp:ImageButton ID="btnCancel" runat="server" CausesValidation="False" 
                        ImageUrl="../images/buttons/Cancel.png" onclick="btnCancel_Click" />
                    <asp:ImageButton ID="btnSave" runat="server" CausesValidation="true" 
                        ImageUrl="../images/buttons/SaveChanges.png" onclick="btnSave_Click" />
</div>
&nbsp;<br />
<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSave" Visible="false" CssClass="controlarea2">
    <table border="0" cellspacing="0" cellpadding="3">
        <tr>
            <td class="formlabel" valign="top">
                Mail Server:</td>
            <td class="formfield">
                <asp:TextBox ID="MailServerField" runat="server" Columns="40"></asp:TextBox></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td class="formfield"><asp:CheckBox ID="chkMailServerAuthentication" runat="server"
                    Text="Use Basic SMTP Authentication"></asp:CheckBox></td>
        </tr>
        <tr>
            <td class="formlabel" valign="top">
                Username:</td>
            <td class="formfield" valign="top">
                <asp:TextBox ID="UsernameField"  runat="server" Columns="40"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="formlabel" valign="top">
                Password:</td>
            <td class="formfield" valign="top">
                <asp:TextBox ID="PasswordField" runat="server" Columns="40"></asp:TextBox></td>
        </tr>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td class="formfield">
                <asp:CheckBox ID="chkSSL" runat="server" Text="Use SSL for SMTP" />
            </td>
            <tr>
                <td class="formlabel" valign="top">
                    SMTP Port:</td>
                <td class="formfield" valign="top">
                    <asp:TextBox ID="SmtpPortField" runat="server" Columns="4"></asp:TextBox>
                    (leave blank for default port)</td>
            </tr>                        
            
        </tr>
    </table>    
</asp:Panel>
<div style="width:500px;margin:50px 0 0 30px;">
    <h4>
        Send Test Message</h4>
    <table border="0" cellspacing="0" cellpadding="5" width="400">
        <tr>
            <td class="formlabel">
                To:</td>
            <td class="formfield">
                <asp:TextBox ID="TestToField" runat="server" Columns="40" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TestToField"
                    ErrorMessage="E-mail address is required." ValidationGroup="TestEmail">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TestToField"
                    ErrorMessage="E-mail address is not in proper format." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    ValidationGroup="TestEmail">*</asp:RegularExpressionValidator></td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                <asp:ImageButton ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/ok.png" ID="btnSendTest" 
                    runat="server" ValidationGroup="TestEmail" onclick="btnSendTest_Click" /></td>
        </tr>
    </table>
    </div>
</asp:Content>


<asp:Content ID="Content2" runat="server" contentplaceholderid="headcontent">
    <style type="text/css">
        .style1
        {
            height: 25px;
        }
    </style>
</asp:Content>



