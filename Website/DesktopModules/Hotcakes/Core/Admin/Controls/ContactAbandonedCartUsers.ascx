<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactAbandonedCartUsers.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.ContactAbandonedCartUsers" %>
<%@ Register Src="../Controls/HtmlEditor.ascx" TagName="HtmlEditor" TagPrefix="hcc" %>

<script type="text/javascript">
    var hcContactAbandonedCartUsers = function () {
        $("#hcContactAbandonedCartUsers").hcDialog({
            title: "<%= Localization.GetString("ContactAbandonedCartUsers") %>",
            width: 750,
            height: 500,
            maxHeight: 800,
            parentElement: '#<%=pnlContactAbandonedCartUsers.ClientID%>',
            close: function () {
                <%= Page.ClientScript.GetPostBackEventReference(btnCloseDialog, "") %>
            }
        });
    };
</script>

<asp:Panel ID="pnlContactAbandonedCartUsers" runat="server" Visible="false">
    <div id="hcContactAbandonedCartUsers" class="dnnClear">
        <asp:MultiView runat="server" ID="mvScreens" ActiveViewIndex="0">
            <asp:View runat="server" ID="vCustomMessage">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:Label runat="server" resourcekey="CustomMessage" CssClass="hcLabel" />
                        <hcc:HtmlEditor ID="txtCustomMessage" runat="server" EditorHeight="120" EditorWidth="685"
                            EditorWrap="true" />
                    </div>
                </div>
                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="btnSendEmails" resourcekey="btnSendEmails" CssClass="hcPrimaryAction" runat="server" OnClick="btnSendEmails_Click" />
                    </li>
                    <li>
                        <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CssClass="hcSecondaryAction" runat="server" CausesValidation="False" OnClick="btnCancelClose_Click" />
                    </li>
                </ul>
            </asp:View>
            <asp:View runat="server" ID="vResults">
                <div class="hcForm">
                    <div class="hcFormItem">
                        <asp:Repeater ID="rpEmailedCustomers" runat="server">
                            <HeaderTemplate>
                                <table class="hcGrid">
                                    <tr class="hcGridHeader">
                                        <th><%=Localization.GetString("FirstName") %></th>
                                        <th><%=Localization.GetString("LastName") %></th>
                                        <th><%=Localization.GetString("Email") %></th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="hcGridRow">
                                    <td><%#Eval("FirstName") %></td>
                                    <td><%#Eval("LastName") %></td>
                                    <td><%#Eval("Email") %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <ul class="hcActions">
                    <li>
                        <asp:LinkButton ID="btnDownloadContacts" resourcekey="btnDownloadContacts" CssClass="hcPrimaryAction" runat="server" OnClick="btnDownloadContacts_Click" />
                    </li>
                    <li>
                        <asp:LinkButton ID="btnClose" resourcekey="btnClose" CssClass="hcSecondaryAction" runat="server" CausesValidation="False" OnClick="btnCancelClose_Click" />
                    </li>
                </ul>
            </asp:View>
        </asp:MultiView>
        <asp:LinkButton ID="btnCloseDialog" runat="server" CssClass="Hidden" OnClick="btnCancelClose_Click" />
    </div>
</asp:Panel>
