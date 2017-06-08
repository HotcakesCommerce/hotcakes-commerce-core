<%@ Page MasterPageFile="~/HCC/Admin/Admin_old.master" Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Customer_List.View"
title="Custom List Report" Codebehind="View.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h2>Customer List</h2>
<p>This report generates a tab-delimited list of customers and the amounts of each specified product they purchased. You can save the text to a *.txt file and load it into Excel or another spreadsheet for mailing lists.</p>
<table border="0" cellspacing="0" cellpadding="3">
<tr>
    <td align="right">Report Sales of These SKUs<br />
    (e.x. ABC123,ABC003,DEF123 )</td>
    <td><asp:TextBox ID="PurchasedSkuField" runat="server" Text="" 
            Width="400px"></asp:TextBox></td>
    <td><asp:ImageButton ID="btnGo" runat="server" 
            ImageUrl="~/HCC/Admin/Images/Buttons/Submit.png" onclick="btnGo_Click" /></td>
</tr>
</table>
<asp:Label ID="lblResult" runat="server"></asp:Label>
<asp:TextBox ID="txtResults" runat="server" TextMode="MultiLine" Rows="20" 
        Columns="80" Wrap="False"></asp:TextBox>
</asp:Content>


