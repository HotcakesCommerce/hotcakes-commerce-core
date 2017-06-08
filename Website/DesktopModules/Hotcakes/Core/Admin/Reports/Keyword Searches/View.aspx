<%@ Page Language="C#" MasterPageFile="~/HCC/Admin/Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Keyword_Searches.View" title="Keyword Searches" Codebehind="View.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>
        Keyword Searches</h1>
        
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td class="layout4column1">&nbsp;</td>
        <td class="gutter">&nbsp;</td>
        <td class="layout4column2">
        <asp:GridView Width="100%" ID="GridView1" runat="server" 
                AutoGenerateColumns="False" CellPadding="3" GridLines="none" 
                onrowdatabound="GridView1_RowDataBound">
            <Columns>
                <asp:BoundField DataField="QueryPhrase" HeaderText="Search Phrase" />
                <asp:BoundField DataField="Count" HeaderText="Hits" />
                <asp:BoundField DataField="Percentage" DataFormatString="{0}%" HeaderText="Percentage" >
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ID="imgBar" runat="server" Width="200" Height="9" ImageUrl="~/HCC/Admin/Images/HorizontalBar.png" AlternateText="" />
                </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle CssClass="row" />
            <HeaderStyle CssClass="rowheader" />
            <AlternatingRowStyle CssClass="alternaterow" />
        </asp:GridView>&nbsp;<br />
            <asp:ImageButton ID="btnReset" runat="server" AlternateText="Reset and Clear All Searches"
                ImageUrl="~/HCC/Admin/Images/Buttons/Reset.png" 
                OnClientClick="return window.confirm('Delete all search history?');" 
                onclick="btnReset_Click" /></td>
        <td class="gutter">&nbsp;</td>
        <td class="layout4column3">&nbsp;</td>       
    </tr>
    </table>      
</asp:Content>

