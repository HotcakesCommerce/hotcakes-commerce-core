<%@ Page Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.MailingLists_Edit" title="Untitled Page" Codebehind="MailingLists_Edit.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>
        Edit Mailing List</h1>
    
    <div style="float: right;width:450px;margin-bottom:20px;">        
        <h2>
            Members</h2>
        <div style="text-align:right; margin:3px 0px 5px 0px;">
                    <asp:ImageButton ID="btnNew" runat="server" AlternateText="Add New Mailing List"
                        ImageUrl="~/HCC/Admin/Images/Buttons/New.png" onclick="btnNew_Click" /></div>        
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BorderColor="#CCCCCC"
            CellPadding="3" DataKeyNames="Id" GridLines="None" Width="100%" 
            onrowdeleting="GridView1_RowDeleting" onrowediting="GridView1_RowEditing">
            <Columns>
                <asp:BoundField DataField="EmailAddress" HeaderText="Email Address" />
                <asp:CommandField ShowEditButton="True" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                            OnClientClick="return window.confirm('Delete this member?');" Text="Delete"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle CssClass="row" />
            <HeaderStyle CssClass="rowheader" />
            <AlternatingRowStyle CssClass="alternaterow" />
        </asp:GridView>
    </div>
    <uc1:MessageBox ID="MessageBox1" runat="server" />
    <asp:Label ID="lblError" runat="server" CssClass="errormessage"></asp:Label><asp:Panel ID="pnlMain" runat="server" DefaultButton="btnSaveChanges">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel">
                    Name:</td>
                <td class="formfield">
                    <asp:TextBox ID="NameField" runat="server" Columns="30" MaxLength="100" TabIndex="2000"
                        Width="200px"></asp:TextBox><asp:RequiredFieldValidator ID="valName" runat="server"
                            ErrorMessage="Please enter a Name" ControlToValidate="NameField">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td class="formlabel">
                    Is Private:</td>
                <td class="formfield">
                    <asp:CheckBox ID="IsPrivateField" runat="server" />
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
    <div style="margin:50px 0px 20px 0px;width:375px;">
    <h2>Import/Export</h2>    
        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="ImportField"
            ErrorMessage="All E-mail Addresses Must Be Valid." 
            ValidationGroup="Import" onservervalidate="CustomValidator1_ServerValidate"></asp:CustomValidator>&nbsp;
    <asp:TextBox ID="ImportField" runat="server" Columns="40" Rows="10" TextMode="MultiLine" Wrap="False"></asp:TextBox>
    <br />
    <asp:ImageButton ID="btnImport" runat="server" 
            ImageUrl="~/HCC/Admin/Images/Buttons/Import.png" ValidationGroup="Import" 
            onclick="btnImport_Click" />&nbsp;
        <asp:ImageButton ID="btnExport" runat="server" 
            ImageUrl="~/HCC/Admin/Images/Buttons/Export.png" CausesValidation="False" 
            onclick="btnExport_Click" />
        <asp:CheckBox ID="chkOnlyEmail" runat="server" Text="Only Export Email Addresses" /><br />
    </div>    
    <asp:HiddenField ID="BvinField" runat="server" />
</asp:Content>

