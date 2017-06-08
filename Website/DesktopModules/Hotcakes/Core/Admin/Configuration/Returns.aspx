<%@ Page Language="C#" MasterPageFile="../AdminNav_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Returns" title="Untitled Page" Codebehind="Returns.aspx.cs" %>
<%@ Register src="NavMenu.ascx" tagname="NavMenu" tagprefix="uc2" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <uc2:NavMenu ID="NavMenu1" Path="Item[@Text='Settings']" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>
        Returns</h1>        
        <uc1:MessageBox ID="MessageBox1" runat="server" />
        <asp:Label id="lblError" runat="server" CssClass="errormessage"></asp:Label>
        <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSave">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel">Automatically Approve RMA:</td>
                <td class="formfield">
                    <asp:DropDownList ID="AutomaticallyIssueRMACheckBoxList" runat="server">
                        <asp:ListItem Text="Yes" Value="1" Selected="False"></asp:ListItem>
                        <asp:ListItem Text="No" Value="0" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                 <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                    <td class="formlabel">
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="../images/buttons/Cancel.png"
                            CausesValidation="False" onclick="btnCancel_Click"></asp:ImageButton></td>
                    <td class="formfield"><asp:ImageButton ID="btnSave" CausesValidation="true"
                                runat="server" ImageUrl="../images/buttons/SaveChanges.png" 
                            onclick="btnSave_Click"></asp:ImageButton></td>
             </tr>
             <tr>
                 <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        </asp:Panel>
</asp:Content>

