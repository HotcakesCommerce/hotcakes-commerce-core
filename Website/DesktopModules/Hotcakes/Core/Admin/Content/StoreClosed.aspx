<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.StoreClosed" title="Untitled Page" Codebehind="StoreClosed.aspx.cs" %>

<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>Store Closed Page Content</h1>
     <asp:Panel ID="pnlMain" runat="server" >
        <table border="0" cellspacing="0" cellpadding="3">          
            <tr>                
                <td class="formfield" colspan="2">
                    <uc1:HtmlEditor ID="ContentField" runat="server" EditorHeight="300" EditorWidth="700" />
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
</asp:Content>



