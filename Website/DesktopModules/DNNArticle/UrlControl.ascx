<%@ Control Language="C#" CodeBehind="URLControl.ascx.cs" AutoEventWireup="true"
    Explicit="True" Inherits="ZLDNN.Modules.DNNArticle.UrlControl" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr id="TypeRow" runat="server">
        <td style="white-space: nowrap" colspan="2">
            <br>
            <asp:Label ID="lblURLType" runat="server" EnableViewState="False" resourcekey="Type"
                CssClass="NormalBold"></asp:Label><br />
            <asp:RadioButtonList ID="optType" CssClass="NormalBold" AutoPostBack="True" RepeatDirection="Horizontal"
                runat="server" OnSelectedIndexChanged="optType_SelectedIndexChanged">
            </asp:RadioButtonList>
            <br />
        </td>
    </tr>
    <tr id="URLRow" runat="server">
        <td style="white-space: nowrap">
            <asp:Label ID="lblURL" runat="server" EnableViewState="False" resourcekey="URL" CssClass="NormalBold"></asp:Label><asp:DropDownList
                ID="cboUrls" runat="server" DataTextField="Url" DataValueField="Url" CssClass="NormalTextBox"
                 Width="300">
            </asp:DropDownList>
            <asp:TextBox ID="txtUrl" runat="server" CssClass="NormalTextBox" Width="300"></asp:TextBox><br />
            <asp:LinkButton ID="cmdSelect" resourcekey="Select" CssClass="CommandButton" runat="server"
                CausesValidation="False" OnClick="cmdSelect_Click">Select</asp:LinkButton><asp:LinkButton
                    ID="cmdDelete" resourcekey="Delete" CssClass="CommandButton" runat="server" CausesValidation="False">Delete</asp:LinkButton><asp:LinkButton
                        ID="cmdAdd" resourcekey="Add" CssClass="CommandButton" runat="server" CausesValidation="False"
                        OnClick="cmdAdd_Click">Add</asp:LinkButton>
        </td>
    </tr>
    <tr id="FileRow" runat="server">
        <td style="white-space: nowrap">
            <asp:Label ID="lblFolder" runat="server" EnableViewState="False" resourcekey="Folder"
                CssClass="NormalBold"></asp:Label><asp:DropDownList ID="cboFolders" runat="server" OnSelectedIndexChanged="cboFolders_SelectedIndexChanged"
                    AutoPostBack="True" CssClass="NormalTextBox" Width="300">
                </asp:DropDownList>
            <asp:Image ID="imgStorageLocationType" runat="server" Visible="False"></asp:Image><br>
            <asp:Label ID="lblFile" runat="server" EnableViewState="False" resourcekey="File"
                CssClass="NormalBold"></asp:Label>
            <asp:ListBox ID="cboFiles" runat="server" DataTextField="Text" DataValueField="Value"
                CssClass="NormalTextBox" Width="300" Height="200"></asp:ListBox>
            <input id="txtFile" type="file" size="30" name="txtFile" runat="server" style="width: 300px" />
            <br />
            <asp:LinkButton ID="cmdUpload" resourcekey="Upload" CssClass="CommandButton" runat="server"
                CausesValidation="False" OnClick="cmdUpload_Click">Upload</asp:LinkButton><asp:LinkButton
                    ID="cmdSave" resourcekey="Save" OnClick="cmdSave_Click" CssClass="CommandButton"
                    runat="server" CausesValidation="False">Save</asp:LinkButton><asp:LinkButton ID="cmdCancel"
                        resourcekey="Cancel" CssClass="CommandButton" runat="server" CausesValidation="False"
                        OnClick="cmdCancel_Click">Cancel</asp:LinkButton>
        </td>
        <td valign="top" align="center" rowspan="3">
            <asp:Image runat="server" ID="img" BorderWidth="0" Width="150px" />
        </td>
    </tr>
    <tr id="ErrorRow" runat="server">
        <td>
            <asp:Label ID="lblMessage" runat="server" EnableViewState="False" CssClass="NormalRed"></asp:Label><br>
        </td>
    </tr>
</table>
<asp:Literal ID="JS" runat="server" />