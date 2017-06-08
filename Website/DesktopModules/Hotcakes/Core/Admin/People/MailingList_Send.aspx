<%@ Page Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.MailingList_Send" title="Untitled Page" Codebehind="MailingList_Send.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>Send Email to Mailing List</h1>
    <uc1:MessageBox ID="MessageBox1" runat="server" />
<table border="0" cellspacing="0" cellpadding="3">
<tr>
    <td class="formlabel">Mailing List:</td>
    <td class="formfield"><asp:Label ID="lblList" runat="server"></asp:Label></td>
</tr>
<tr>
    <td class="formlabel">Use Email Template:</td>
    <td class="formfield"><asp:DropDownList runat="server" ID="EmailTemplateField"></asp:DropDownList></td>
</tr>
<tr>
    <td class="formlabel"><asp:ImageButton ID="btnCancel" runat="Server" 
            ImageUrl="~/HCC/Admin/Images/BUttons/Cancel.png" onclick="btnCancel_Click" /></td>
    <td class="formfield"><asp:ImageButton ID="btnPreview" runat="Server" 
            ImageUrl="~/HCC/Admin/Images/BUttons/Preview.png" onclick="btnPreview_Click" />&nbsp;&nbsp;<asp:ImageButton 
            ID="btnSend" runat="Server" ImageUrl="~/HCC/Admin/Images/BUttons/Ok.png" 
            onclick="btnSend_Click" /></td>
</tr>
<tr>
    <td colspan="2">&nbsp;</td>
</tr>
<tr>
    <td class="formlabel">Preview Subject:</td>
    <td class="formfield"><pre><asp:Label id="PreviewSubjectField" runat="server"/></pre></td>
</tr>
<tr>
    <td class="formlabel">Preview Body:</td>
    <td class="formfield"><pre><asp:Label id="PreviewBodyField" runat="server" /></pre></td>
</tr>
</table>
<asp:HiddenField ID="BvinField" runat="server" />
</asp:Content>

