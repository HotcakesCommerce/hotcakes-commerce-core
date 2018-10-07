<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ctlAttachedFile.ascx.cs"
            Inherits="ZLDNN.Modules.DNNArticle.ctlAttachedFile" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="roleselector" Src="~/desktopmodules/dnnarticle/usercontrols/RoleSelector.ascx" %>

        <asp:Panel runat="server" ID="panelAttachment">
            <asp:Label ID="lbSaveArticle" runat="server" CssClass="Normal" Visible="false"></asp:Label>
            <table runat="server" id="tbFiles" cellspacing="4" cellpadding="4" border="0" width="400"
                class="dnnFormItem">
                <tr>
                    <td align="left" style="width: 100%">
                        <asp:Label ID="lbFile" runat="server" CssClass="Normal"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 100%">
                        <asp:DataList ID="lstFile" runat="server" Width="100%" EnableViewState="false" ItemStyle-CssClass="FileManager_FileList"
                            OnItemCommand="lstFile_ItemCommand" OnItemDataBound="lstFile_ItemDataBound">
                            <ItemTemplate>
                                <table cellspacing="4" cellpadding="4" border="0" width="100%">
                                    <tr>
                                        <td>
                                            <asp:LinkButton CausesValidation="false" CommandName="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                                ID="cmdEdit" runat="server">
                                                <asp:Image ID="Image3" runat="server" ImageUrl="~/images/Edit.gif" AlternateText="Edit" />
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                            <asp:LinkButton CausesValidation="false" CommandName="Remove" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                                ID="cmdRemove" runat="server">
                                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/icon_recyclebin_16px.gif"
                                                    AlternateText="Remove" />
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                            <asp:LinkButton CausesValidation="false" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                                ID="cmdDelete" runat="server">
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/delete.gif" AlternateText="Delete" />
                                            </asp:LinkButton>
                                        </td>
                                        <td class="Normal" style="width: 100%; text-align: right">
                                            <asp:LinkButton CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Title").ToString()%>'
                                                CommandName="Download" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "URL").ToString()%>'
                                                ID="LinkButton2" runat="server">
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:DataList>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <table cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
                            <tr id="Tr1" runat="server">
                                <td>
                                    <dnn:Label ID="lblFileURL" runat="server" ControlName="lblFileURL" Suffix=":" />
                                </td>
                                <td>
                                    <input id="FileCtl" type="file" size="40" name="cmdBrowse" runat="server" />
                                    <Portal:URL ID="ctlFile" runat="server" Width="250" ShowTabs="False" ShowUrls="False"
                                        ShowFiles="true" ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True"
                                        UrlType="F" ShowNewWindow="False" />
                                    <asp:Label runat="server" ID="lbURL" CssClass="Normal"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dnn:Label ID="lblTitle" runat="server" ControlName="lblTitle" Suffix=":" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTitle" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" ValidationGroup="Attachment" ControlToValidate="txtTitle"
                                        runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dnn:Label ID="lbViewOrder" runat="server" ControlName="lblTitle" Suffix=":" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtViewOrder" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
                                    <asp:CompareValidator ID="CompareValidator1" ControlToValidate="txtViewOrder" runat="server"
                                        ErrorMessage="Integer" ValidationGroup="Attachment" Operator="DataTypeCheck"
                                        Type="Integer"></asp:CompareValidator>
                                </td>
                            </tr>
                            <tr id="trViewPermsiion" runat="server">
                                <td>
                                    <dnn:Label ID="lbViewFileRoles" runat="server" />
                                </td>
                                <td>
                                    <zldnn:roleselector runat="server" ID="roleselector"></zldnn:roleselector>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 155px">
                                </td>
                                <td>
                                    <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate" runat="server" CausesValidation="True"
                                        ValidationGroup="Attachment" BorderStyle="none" Text="Update" OnClick="cmdUpdate_Click"></asp:LinkButton><asp:LinkButton
                                            ID="cmdAdd" runat="server" OnClick="cmdUpdate_Click" CausesValidation="True"
                                            ValidationGroup="Attachment" BorderStyle="none" CssClass="AddLabel"></asp:LinkButton>
                                    <asp:LinkButton ID="cmdReset" runat="server" BorderStyle="none" CssClass="ResetLabel"
                                        CausesValidation="False" OnClick="cmdReset_Click"></asp:LinkButton>
                                    <asp:Label ID="lbError" runat="server" CssClass="NormalRed"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
