<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.UserPicker" Codebehind="UserPicker.ascx.cs" %>
<%@ Reference Control="MessageBox.ascx" %>
<asp:Panel ID="wrapper" runat="server" DefaultButton="btnValidateUser">
    <span class="hcLabel">E-Mail</span><br />
    <asp:TextBox ID="UserNameField" runat="server" TabIndex="3000"></asp:TextBox>
    <asp:ImageButton ID="btnValidateUser" runat="server" CausesValidation="false" AlternateText="Validate User"
        ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SmallRight.png" TabIndex="3010" 
        onclick="btnValidateUser_Click" />&nbsp;
    <asp:ImageButton ID="btnBrowseUsers" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Browse.png"
        AlternateText="Browse for User Account" CausesValidation="false" 
        onclick="btnBrowseUsers_Click" />
    <asp:ImageButton ID="btnNewUser" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/New.png"
        AlternateText="Add New User Account" CausesValidation="false" 
        onclick="btnNewUser_Click" /><br />
    <asp:Panel CssClass="controlarea2" ID="pnlUserBrowser" runat="server" Visible="false"
        DefaultButton="btnGoUserSearch">
        <span class="lightlabel">User Browser Filter</span><br />
        <asp:TextBox ID="FilterField" runat="server" Columns="15"></asp:TextBox>
        <asp:ImageButton ID="btnGoUserSearch" runat="server" AlternateText="Filter Results"
            ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SmallRight.png" CausesValidation="false" 
            onclick="btnGoUserSearch_Click" />
        <div style="width: 285px; height: 200px; overflow: auto;">
            <asp:GridView ShowHeader="false" ID="GridView1" runat="server" AutoGenerateColumns="False"
                DataKeyNames="bvin" BorderColor="#CCCCCC" CellPadding="3" GridLines="None" 
                Width="260" onrowediting="GridView1_RowEditing">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'>'></asp:Label><br />
                            <span class="smalltext">                                
                                <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>'>'></asp:Label>
                                <asp:Label ID="lblLastName" runat="server" Text='<%# Bind("LastName") %>'>'></asp:Label></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="SelectUserButton" runat="server" CausesValidation="false" CommandName="Edit"
                                ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SmallRight.png" AlternateText="Select User"></asp:ImageButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="row" />
                <HeaderStyle CssClass="rowheader" />
                <AlternatingRowStyle CssClass="alternaterow" />
            </asp:GridView>
        </div>
        <asp:ImageButton ID="btnBrowserUserCancel" CausesValidation="false" runat="server"
            ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Close.png" 
            AlternateText="Close Browser" onclick="btnNewUserCancel_Click" />
    </asp:Panel>
    <asp:Panel CssClass="controlarea2" ID="pnlNewUser" runat="server" Visible="false">
        <table border="0" cellspacing="0" cellpadding="3">
            <tr>
                <td class="formlabel">
                    E-Mail</td>
                <td class="formfield">
                    <asp:TextBox ID="NewUserEmailField" runat="server" Columns="15"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formlabel">
                    First Name</td>
                <td class="formfield">
                    <asp:TextBox ID="NewUserFirstNameField" runat="server" Columns="15"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formlabel">
                    Last Name</td>
                <td class="formfield">
                    <asp:TextBox ID="NewUserLastNameField" runat="server" Columns="15"></asp:TextBox></td>
            </tr>
                        <tr>
                <td class="formlabel">
                    Tax Exempt</td>
                <td class="formfield">
                    <asp:CheckBox id="NewUserTaxExemptField" runat="server" /></td>
            </tr>

            <tr>
                <td class="formlabel">
                    <asp:ImageButton ID="btnNewUserCancel" CausesValidation="false" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Cancel.png" /></td>
                <td class="formfield">
                    <asp:ImageButton ID="btnNewUserSave" CausesValidation="false" runat="server" 
                        ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Ok.png" onclick="btnNewUserSave_Click" /></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
