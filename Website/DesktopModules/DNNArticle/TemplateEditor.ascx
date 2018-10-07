<%@ Control CodeBehind="TemplateEditor.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticle.TemplateEditor" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<div ID="pnEdit" runat="server">
    <table id="tbEdit" cellspacing="1" cellpadding="1" width="900" border="0" runat="server"  class="dnnFormItem" >
        <tr>
            <td >
                <asp:Label ID="lbTemplateType" runat="server" resourcekey="lbTemplateType"></asp:Label>
            </td>
            <td>
                <asp:DropDownList runat="server" ID="cboTemplateType" AutoPostBack="true" Width="300px" OnSelectedIndexChanged="cboTemplateType_SelectedIndexChanged">
                    <asp:ListItem Text="Article Template" Value=""></asp:ListItem>
                    <asp:ListItem Text="CategroyArticle List Template" Value="CategoryArticleListTemplate/"></asp:ListItem>
                    <asp:ListItem Text="List Header Template" Value="ArticleListHeaderTemplate/"></asp:ListItem>
                    <asp:ListItem Text="List Footer Template" Value="ArticleListFooterTemplate/"></asp:ListItem>
                    <asp:ListItem Text="Comment Template" Value="CommentTemplate/"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td rowspan="3">
                <span >Tokens:</span><br/>
                <div style="width:500px;height:400px;overflow:scroll;padding:4px;border:solid 1px #cccccc;background:#eeeeee ">
                    <asp:Literal ID="lthelp" runat="server"></asp:Literal>
                </div>
            </td>
        </tr>
        <tr>
            <td valign="top" >
                <asp:Label ID="lbTemplateList" resourcekey="lbTemplateList" runat="server"></asp:Label>
            </td>
            <td  valign="top" >
                 <asp:DropDownList runat="server" ID="cboPathType" AutoPostBack="true" 
                     onselectedindexchanged="cboPathType_SelectedIndexChanged" >
                    <asp:ListItem Text="Host" Value="host"></asp:ListItem>
                    <asp:ListItem Text="Portal" Value="portal/"></asp:ListItem>
                   
                </asp:DropDownList>
                <asp:ListBox ID="ListBox1" Height="200px" Width="300px" runat="server" CssClass="Normal"
                    AutoPostBack="True" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
                <br>
                <asp:LinkButton ID="cmdEdit" runat="server" CssClass="EditLabel" resourcekey="cmdEdit"
                    OnClick="cmdEdit_Click">Edit</asp:LinkButton>
                <asp:LinkButton ID="cmdAddNew" runat="server" CssClass="AddLabel" resourcekey="cmdAddNew"
                    OnClick="cmdAddNew_Click">AddNew</asp:LinkButton>
                <asp:LinkButton ID="cmdClone" runat="server" CssClass="CloneLabel" resourcekey="cmdClone"
                    OnClick="cmdClone_Click">Clone</asp:LinkButton>
                <asp:LinkButton ID="cmdDelete" runat="server" CssClass="DeleteLabel" resourcekey="cmdDelete"
                    OnClick="cmdDelete_Click">Delete</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td >
                <asp:Label ID="lbFileName" runat="server" resourcekey="lbFileName">FileName</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" Width="300px" CssClass="NormalTextBox"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td  valign="top">
                <asp:Label ID="lbTemplate" runat="server" resourcekey="lbTemplate">Template</asp:Label>
            </td>
            <td colspan="2">
                <dnn:TextEditor ID="txtTemp" runat="server" Height="500px" Width="800px" />
                 <asp:TextBox ID="txtboxTemp" runat="server" Height="500px" Width="800px" TextMode="MultiLine" CssClass="NormalTextBox"></asp:TextBox>
                <br/>
                <asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" OnClick="cmdUpdate_Click"
                    CssClass="UpdateLabel">Update</asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:LinkButton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CancelLabel"
        OnClick="cmdCancel_Click">Cancel</asp:LinkButton></div>