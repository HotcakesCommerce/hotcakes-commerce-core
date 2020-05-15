<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.UserPicker" Codebehind="UserPicker.ascx.cs" %>
<%@ Reference Control="MessageBox.ascx" %>
<asp:Panel ID="wrapper" runat="server" DefaultButton="btnValidateUser">
    <span class="hcLabel"><%=Localization.GetString("lblEmail") %></span><br />
    <asp:TextBox ID="UserNameField" runat="server" />
    <asp:LinkButton ID="btnValidateUser" runat="server" CausesValidation="false" resourcekey="btnValidateUser" CssClass="hcTertiaryAction hcSmall" onclick="btnValidateUser_Click" />&nbsp;
    <asp:LinkButton ID="btnBrowseUsers" runat="server" resourcekey="btnBrowseUsers" CausesValidation="false" CssClass="hcTertiaryAction hcSmall" onclick="btnBrowseUsers_Click" />
    <asp:LinkButton ID="btnNewUser" runat="server" resourcekey="btnNewUser" CausesValidation="false" CssClass="hcTertiaryAction hcSmall" onclick="btnNewUser_Click" /><br />
    <asp:Panel CssClass="controlarea2" ID="pnlUserBrowser" runat="server" Visible="false" DefaultButton="btnGoUserSearch">
        <span class="lightlabel"><%=Localization.GetString("lblCustomerBrowseFilter") %></span><br />
        <asp:TextBox ID="FilterField" runat="server" Columns="15"/>
        <asp:LinkButton ID="btnGoUserSearch" runat="server" resourcekey="btnFilterResults" CausesValidation="false" onclick="btnGoUserSearch_Click" />
        <div style="width: 285px; height: 200px; overflow: auto;">
            <asp:GridView ShowHeader="false" ID="GridView1" runat="server" AutoGenerateColumns="False"
                DataKeyNames="bvin" GridLines="None" Width="260" onrowediting="GridView1_RowEditing" CssClass="hcGrid">
                <RowStyle CssClass="hcGridRow" />
                <HeaderStyle CssClass="hcGridHeader" />
                <AlternatingRowStyle CssClass="hcGridAltRow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'/><br />
                            <span class="smalltext">                                
                                <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>'/>
                                <asp:Label ID="lblLastName" runat="server" Text='<%# Bind("LastName") %>'/>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="SelectUserButton" runat="server" CausesValidation="false" CommandName="Edit" resourcekey="btnSelectUser"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <asp:LinkButton ID="btnBrowserUserCancel" CausesValidation="false" runat="server" resourcekey="btnCloseBrowser" onclick="btnNewUserCancel_Click" />
    </asp:Panel>
    <asp:Panel CssClass="controlarea2" ID="pnlNewUser" runat="server" Visible="false">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel"><%=Localization.GetString("lblEmail") %></td>
                <td class="formfield"><asp:TextBox ID="NewUserEmailField" runat="server" Columns="15"/></td>
            </tr>
            <tr>
                <td class="formlabel"><%=Localization.GetString("lblFirstName") %></td>
                <td class="formfield"><asp:TextBox ID="NewUserFirstNameField" runat="server" Columns="15"/></td>
            </tr>
            <tr>
                <td class="formlabel"><%=Localization.GetString("lblLastName") %></td>
                <td class="formfield"><asp:TextBox ID="NewUserLastNameField" runat="server" Columns="15"/></td>
            </tr>
            <tr>
                <td class="formlabel"><%=Localization.GetString("lblTaxExempt") %></td>
                <td class="formfield"><asp:CheckBox id="NewUserTaxExemptField" runat="server" /></td>
            </tr>

            <tr>
                <td class="formlabel">
                    <asp:LinkButton ID="btnNewUserCancel" resourcekey="btnNewUserCancel" CausesValidation="false" runat="server" /></td>
                <td class="formfield">
                    <asp:LinkButton ID="btnNewUserSave" resourcekey="btnNewUserSave" CausesValidation="false" runat="server" onclick="btnNewUserSave_Click" /></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
