<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserIdEditor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Qualifications.UserIdEditor" %>
<%@ Register Src="../../Controls/UserPicker.ascx" TagName="UserPicker" TagPrefix="hcc" %>
<%@ Register Src="../../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<hcc:MessageBox ID="ucMessageBox" runat="server" />
<h1><%=Localization.GetString("WhenUserIs") %></h1>
<table border="0">
    <tr>
        <td style="vertical-align: top; width: 45%;">
            <hcc:UserPicker ID="UserPicker1" runat="server" />
        </td>
        <td style="vertical-align: top;">
            <asp:GridView ID="gvUserIs" runat="server" AutoGenerateColumns="False"
                DataKeyNames="Bvin" OnRowDeleting="gvUserIs_RowDeleting" CssClass="hcGrid"
                ShowHeader="False">
                <RowStyle CssClass="hcGridRow" />
                <Columns>
                    <asp:BoundField DataField="DisplayName" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteUserIs" runat="server" CausesValidation="False"
                                CommandName="Delete" Text="Delete" CssClass="hcIconDelete" OnPreRender="btnDeleteUserIs_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>

