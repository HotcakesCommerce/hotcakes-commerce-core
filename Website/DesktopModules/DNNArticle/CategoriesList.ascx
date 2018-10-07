<%@ Control CodeBehind="CategoriesList.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticle.CategoriesList" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Tabs Design Table"
       border="0">
    <tr>
        <td style="width: 560px">
            <asp:Panel ID="pnlTabs" runat="server" CssClass="WorkPanel" Visible="True">
                <table cellspacing="0" cellpadding="0" summary="Tabs Design Table" border="0">
                    <tr valign="top">
                        <td style="width: 400px">
                            <asp:ListBox ID="lstCategories" CssClass="NormalTextBox" runat="server" Width="400px"
                                         DataValueField="ItemId" DataTextField="Title" Rows="22"></asp:ListBox></td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <table summary="Tabs Design Table">
                                <tr>
                                    <td class="SubHead" valign="top" colspan="2">
                                        <asp:Label ID="lblMovePage" runat="server" resourcekey="MovePage">Move Page</asp:Label>
                                        <hr style="height: 2px; border-width: 0; color: gray; background-color: gray" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: top; width: 10%">
                                        <asp:ImageButton ID="cmdUp" runat="server" resourcekey="cmdUp.Help" ImageUrl="~/images/up.gif"
                                                         CommandName="up" AlternateText="Move Up In Current Level" OnClick="UpDown_Click"></asp:ImageButton></td>
                                    <td style="vertical-align: top; width: 90%">
                                        <dnn:HelpButton ID="hbtnUpHelp" runat="server" ResourceKey="cmdUp"></dnn:HelpButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td  style="vertical-align: top; width: 10%">
                                        <asp:ImageButton ID="cmdDown" runat="server" resourcekey="cmdDown.Help" ImageUrl="~/images/dn.gif"
                                                         CommandName="down" AlternateText="Move Down In Current Level" OnClick="UpDown_Click"></asp:ImageButton></td>
                                    <td style="vertical-align: top; width: 90%">
                                        <dnn:HelpButton ID="hbtnDownHelp" runat="server" ResourceKey="cmdDown"></dnn:HelpButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td   style="vertical-align: top; width: 10%">
                                        <asp:ImageButton ID="cmdLeft" runat="server" resourcekey="cmdLeft.Help" ImageUrl="~/images/lt.gif"
                                                         CommandName="left" AlternateText="Move Up One Hierarchical Level" OnClick="RightLeft_Click"></asp:ImageButton></td>
                                    <td style="vertical-align: top; width: 90%">
                                        <dnn:HelpButton ID="hbtnLeftHelp" runat="server" ResourceKey="cmdLeft"></dnn:HelpButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td  style="vertical-align: top; width: 10%">
                                        <asp:ImageButton ID="cmdRight" runat="server" resourcekey="cmdRight.Help" ImageUrl="~/images/rt.gif"
                                                         CommandName="right" AlternateText="Move Down One Hierarchical Level" OnClick="RightLeft_Click"></asp:ImageButton></td>
                                    <td style="vertical-align: top; width: 90%">
                                        <dnn:HelpButton ID="hbtnRightHelp" runat="server" ResourceKey="cmdRight"></dnn:HelpButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="height: 25px">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" colspan="2">
                                        <asp:Label ID="lblActions" runat="server" resourcekey="Actions">Actions</asp:Label>
                                        <hr style="height: 2px; border-width: 0; color: gray; background-color: gray" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: top; width: 10%">
                                        <asp:ImageButton ID="cmdEdit" runat="server" resourcekey="cmdEdit.Help" ImageUrl="~/images/edit.gif"
                                                         AlternateText="Edit" OnClick="cmdEdit_Click"></asp:ImageButton></td>
                                    <td style="vertical-align: top; width: 90%">
                                        <dnn:HelpButton ID="hbtnViewHelp" runat="server" ResourceKey="cmdEdit"></dnn:HelpButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td style="width: 10px">
            &nbsp;</td>
    </tr>
</table>
<asp:LinkButton ID="cmdAdd" runat="server" CssClass="CommandButton" resourcekey="cmdAdd" OnClick="cmdAdd_Click">Add Catalog</asp:LinkButton>&nbsp;
<asp:LinkButton ID="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" OnClick="cmdCancel_Click">Cancel</asp:LinkButton>