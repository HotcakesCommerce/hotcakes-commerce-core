<%@ Control Codebehind="ImportFromCoreAnnouncements.ascx.cs" Language="C#" AutoEventWireup="true"
            Inherits="ZLDNN.Modules.DNNArticle.ImportFromCoreAnnouncements" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="560" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table" class="dnnFormItem">
    <tr>
        <td >
            <dnn:Label ID="plTemplate" runat="server" ControlName="plTemplate" Suffix=":" />
        </td>
        <td>
            <asp:DropDownList ID="cboTemplate" runat="server" CssClass="Normal" Width="300"
                              AutoPostBack="True" OnSelectedIndexChanged="cboTemplate_SelectedIndexChanged">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td  style="vertical-align: top; width: 150px">
            <dnn:Label ID="plFolder" runat="server" ControlName="cboFolders" Suffix=":"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="cboFolders" runat="server" CssClass="Normal" Width="300"
                              AutoPostBack="true"  OnSelectedIndexChanged="cboFolders_SelectedIndexChanged"/></td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plFile" runat="server" ControlName="plFile" Suffix=":"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="cboFiles" runat="server" CssClass="Normal" Width="300">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plCatalog" Suffix=":" ControlName="plCatalog" runat="server"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="cboCatalog" runat="server" Width="300" CssClass="Normal">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plApproved" runat="server" ControlName="plApproved" Suffix=":" />
        </td>
        <td>
            <asp:CheckBox ID="chkApproved" runat="server" /></td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdImport" resourcekey="cmdImport" runat="server" CssClass="CommandButton"
                    Text="Import" BorderStyle="none" OnClick="cmdImport_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="CommandButton"
                    Text="Cancel" BorderStyle="none" CausesValidation="False" OnClick="cmdCancel_Click"></asp:LinkButton>
</p>