<%@ Page Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.MailingLists" title="Untitled Page" Codebehind="MailingLists.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>
    Mailing Lists</h1>
    
            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td align="left" valign="middle"><h2>
                        <asp:Label ID="lblResults" runat="server">No Results</asp:Label></h2></td>                   
                    <td align="right" valign="middle">
                        <asp:ImageButton ID="btnNew" runat="server" 
                            AlternateText="Add New Mailing List" 
                            ImageUrl="~/HCC/Admin/Images/Buttons/New.png" onclick="btnNew_Click" /></td>
                </tr>
            </table>
        &nbsp;
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
        BorderColor="#CCCCCC" CellPadding="3" GridLines="None" Width="100%" 
        onrowdeleting="GridView1_RowDeleting" onrowediting="GridView1_RowEditing">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="List Name" />
            <asp:CheckBoxField DataField="IsPrivate" HeaderText="Private List" />
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="MailingList_Send.aspx?id={0}"
                Text="Send Email" Visible="false" />
            <asp:CommandField ShowEditButton="True" />
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton OnClientClick="return window.confirm('Delete this mailing list?');" ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                        Text="Delete"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <RowStyle CssClass="row" />
        <HeaderStyle CssClass="rowheader" />
        <AlternatingRowStyle CssClass="alternaterow" />
    </asp:GridView>
</asp:Content>

