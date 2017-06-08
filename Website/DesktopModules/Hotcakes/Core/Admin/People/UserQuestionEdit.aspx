<%@ Page Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.People.UserQuestionEdit" title="Untitled Page" Codebehind="UserQuestionEdit.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>Question Options Edit</h1>
    <asp:RadioButtonList ID="QuestionTypeRadioButtonList" runat="server" 
        AutoPostBack="True" 
        onselectedindexchanged="QuestionTypeRadioButtonList_SelectedIndexChanged">
        <asp:ListItem Selected="True">Free Response</asp:ListItem>
        <asp:ListItem>Multiple Choice</asp:ListItem>
    </asp:RadioButtonList>
    <h4>Question Name:</h4>
            <asp:TextBox ID="NameTextBox" runat="server"></asp:TextBox><br /><br />
    <h4>Question:</h4>
    <asp:TextBox id="QuestionTextBox" runat="server" Columns="60" TextMode="multiLine" Rows="2"></asp:TextBox><br /><br />
    <asp:Panel ID="MultipleChoicePanel" runat="server">
    <h4>Multiple Choice Options:</h4>
    <table>
    <tr>
    <td>
                <asp:ImageButton ID="NewOptionImageButton" runat="server" 
                    ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/New.png" 
                    onclick="NewOptionImageButton_Click" />
    </td>
    <td>
            <asp:GridView ID="ValuesGridView" runat="server" AutoGenerateColumns="False" 
                GridLines="none" CellPadding="2" onrowdeleting="ValuesGridView_RowDeleting">
        <Columns>
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <asp:TextBox ID="ValueTextBox" runat="server"></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>           
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="DeleteImageButton" AlternateText="Delete" CommandName="Delete" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Delete.png" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <RowStyle CssClass="row" />
        <HeaderStyle CssClass="rowheader" />
        <AlternatingRowStyle CssClass="alternaterow" />
    </asp:GridView>
    </td>
    </tr>
    </table>

    </asp:Panel>
    <br />
    <div><asp:ImageButton id="CancelImageButton" runat="server" AlternateText="Cancel" 
            ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Cancel.png" 
            onclick="CancelImageButton_Click">
        </asp:ImageButton><asp:ImageButton id="SaveImageButton" runat="server" 
            AlternateText="Save" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/SaveChanges.png" 
            onclick="SaveImageButton_Click">
        </asp:ImageButton></div>
</asp:Content>

