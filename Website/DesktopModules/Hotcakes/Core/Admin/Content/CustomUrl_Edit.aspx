<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.CustomUrl_Edit" Title="Untitled Page" Codebehind="CustomUrl_Edit.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        Edit Custom Url</h1>
    <uc1:MessageBox ID="MessageBox1" runat="server" />
    <asp:Label ID="lblError" runat="server" CssClass="errormessage"></asp:Label>
    <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSaveChanges">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel">
                    Requested Url:</td>
                <td class="formfield">
                    <asp:TextBox ID="RequestedUrlField" runat="server" Columns="80" TabIndex="2000" Width="500px"></asp:TextBox><asp:RequiredFieldValidator
                        ID="RequiredVal1" runat="server" ErrorMessage="Please enter a Requested Url"
                        ControlToValidate="RequestedUrlField">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td class="formlabel">
                    Redirect To Url:</td>
                <td class="formfield">
                    <asp:TextBox ID="RedirectToUrlField" runat="server" Columns="80" TabIndex="2010" Width="500px"></asp:TextBox><asp:RequiredFieldValidator
                        ID="RequiredVal2" runat="server" ErrorMessage="Please enter a Redirect To Url"
                        ControlToValidate="RedirectToUrlField">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td class="formlabel">&nbsp;</td>
                <td class="formfield"><asp:CheckBox ID="chkPermanent" runat="server" Text="Use a permanent 301 redirect" /></td>
            </tr>
            <tr>
                <td class="formlabel">
                    <asp:ImageButton ID="btnCancel" TabIndex="2502" runat="server" ImageUrl="../images/buttons/Cancel.png"
                        CausesValidation="False" onclick="btnCancel_Click"></asp:ImageButton></td>
                <td class="formfield">
                    <asp:ImageButton ID="btnUpdate" TabIndex ="2501" runat="server" ImageUrl="../images/buttons/Update.png"
                    CausesValidation="true" OnClick="btnUpdate_Click" />&nbsp;<asp:ImageButton ID="btnSaveChanges" TabIndex="2500" runat="server" 
                        ImageUrl="../images/buttons/SaveChanges.png" onclick="btnSaveChanges_Click">
                    </asp:ImageButton></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="BvinField" runat="server" />
    <h2>Preview of Currently Saved Url</h2>
    From: <asp:Literal ID="litPreviewFrom" runat="server"></asp:Literal><br />
    &nbsp;<br />
    To: <asp:Literal ID="litPreviewTo" runat="server"></asp:Literal>
</asp:Content>
