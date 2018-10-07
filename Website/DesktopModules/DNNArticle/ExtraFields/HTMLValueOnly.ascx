<%@ Control CodeBehind="HTMLValueOnly.ascx.cs" Language="C#" AutoEventWireup="true" Inherits=" ZLDNN.Modules.DNNArticle.ExtraFields.HTMLValueOnly" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<dnn:TextEditor ID="txtContent" runat="server" DefaultMode="RICH" Height="400" />
<asp:RequiredFieldValidator ID="rq" runat="server" ControlToValidate="txtContent"
    ErrorMessage="*" ValidationGroup="ArticleEditor" Display="Dynamic"></asp:RequiredFieldValidator>
