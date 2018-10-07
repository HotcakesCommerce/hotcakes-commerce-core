<%@ Control Language="C#" AutoEventWireup="false" Inherits="ZLDNN.Modules.ZLDNNFileSearch.Settings" Codebehind="Settings.ascx.cs" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ZLDNNFileSearch Settings Design Table">
    <tr>
        <td class="SubHead" nowrap="nowrap" width="237" style="height: 21px">
            <dnn:Label ID="lblFileExtension" runat="server" Suffix=":"></dnn:Label>
        </td>
        <td class="Normal" style="height: 21px; width: 305px;">
            <asp:TextBox ID="txtFileExtension" runat="server" Width="300"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="237" style="height: 21px">
            <dnn:Label ID="lblPageSize" runat="server" Suffix=":"></dnn:Label>
        </td>
        <td class="Normal" style="height: 21px; width: 305px;">
            <asp:TextBox ID="txtPageSize" runat="server" Width="300"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="237" style="height: 21px">
            <dnn:Label ID="lblMaxSize" runat="server" Suffix=":"></dnn:Label>
        </td>
        <td class="Normal" style="height: 21px; width: 305px;">
            <asp:TextBox ID="txtMaxSize" runat="server" Width="300"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="237" style="height: 21px">
            <dnn:Label ID="lblFolderName" runat="server" Suffix=":"></dnn:Label>
        </td>
        <td class="Normal" style="height: 21px; width: 305px;">
            <asp:DropDownList runat="server" ID="ddlFolder"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="237" style="height: 21px">
            <dnn:Label ID="lblOnlyUserFolder" runat="server" Suffix=":"></dnn:Label>
        </td>
        <td class="Normal" style="height: 21px; width: 305px;">
            <asp:DropDownList ID="ddlOnlyUserFolder" runat="server" Width="298px">
                <asp:ListItem resourcekey="liFalse" Value="1" Selected="True"></asp:ListItem>
                <asp:ListItem resourcekey="liTrue" Value="0"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>

