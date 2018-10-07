<%@ Control CodeBehind="TextBoxValueOnly.ascx.cs" Language="C#" AutoEventWireup="true" Inherits=" ZLDNN.Modules.DNNArticle.ExtraFields.TextBoxValueOnly" %>
<asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ID="rq" runat="server" ControlToValidate="txtValue" ErrorMessage="*"
    ValidationGroup="ArticleEditor" Display="Dynamic"></asp:RequiredFieldValidator>
        