<%@ Page Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Content.CustomUrl" Title="Untitled Page" Codebehind="CustomUrl.aspx.cs" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        URL Mapper</h1>
    <uc1:MessageBox ID="msg" runat="server" />
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tr>
            <td align="left" valign="middle">
                <h2>
                    <asp:Label ID="lblResults" runat="server">No Results</asp:Label></h2>
            </td>
            <td class="formlabel" align="right" valign="middle">
                <asp:ImageButton ID="btnNew" runat="server" AlternateText="Add New Custom Url" 
                    ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/New.png" onclick="btnNew_Click" /></td>
        </tr>
    </table>
    &nbsp;
    <asp:Literal id="litPager1" runat="server" EnableViewState="false"></asp:Literal>            
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="bvin"
        BorderColor="#CCCCCC" CellPadding="3" GridLines="None" Width="100%" 
        AllowPaging="False" onrowdeleting="GridView1_RowDeleting" onrowediting="GridView1_RowEditing">
        <Columns>
            <asp:TemplateField HeaderText="Url">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("RequestedUrl") %>'></asp:Label><br />
                    &nbsp;&nbsp;&raquo; <asp:Label ID="Label2" runat="server" Text='<%# Bind("RedirectToUrl") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" />
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton OnClientClick="return window.confirm('Delete this custom url?');"
                        ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                        Text="Delete"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <RowStyle CssClass="row" />
        <HeaderStyle CssClass="rowheader" />
        <AlternatingRowStyle CssClass="alternaterow" />
        <PagerSettings Position="TopAndBottom" />
    </asp:GridView>
    <asp:Literal id="litPager2" runat="server" EnableViewState="false"></asp:Literal>

</asp:Content>
