<%@ Page Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.MailingLists_EditMember" title="Untitled Page" Codebehind="MailingLists_EditMember.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>Edit Mailing List Member</h1>
    <uc1:MessageBox ID="MessageBox1" runat="server" />
    <asp:Label ID="lblError" runat="server" CssClass="errormessage"></asp:Label><asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSaveChanges">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel">
                    Email Address:</td>
                <td class="formfield">
                    <asp:TextBox ID="EmailAddressField" runat="server" Columns="30" TabIndex="2000"
                        Width="200px"></asp:TextBox><asp:RequiredFieldValidator ID="valEmail" runat="server"
                            ErrorMessage="Please enter an email address" ControlToValidate="EmailAddressField">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td class="formlabel">
                    First Name:</td>
                <td class="formfield">
                    <asp:TextBox ID="FirstNameField" runat="server" Columns="30" TabIndex="2010"
                        Width="200px" />
                </td>
            </tr>
             <tr>
                <td class="formlabel">
                    Last Name:</td>
                <td class="formfield">
                   <asp:TextBox ID="LastNameField" runat="server" Columns="30" TabIndex="2020"
                        Width="200px" />
                </td>
            </tr>
            <tr>
                <td class="formlabel">
                    <asp:ImageButton ID="btnCancel" TabIndex="2501" runat="server" ImageUrl="../images/buttons/Cancel.png"
                        CausesValidation="False" onclick="btnCancel_Click"></asp:ImageButton></td>
                <td class="formfield">
                    <asp:ImageButton ID="btnSaveChanges" TabIndex="2500" runat="server" 
                        ImageUrl="../images/buttons/SaveChanges.png" onclick="btnSaveChanges_Click">
                    </asp:ImageButton></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="BvinField" runat="server" />
    <asp:HiddenField ID="ListIDField" runat="server" />
</asp:Content>

