<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateSelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.TemplateSelector" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="width:300px;float:left">
<asp:DropDownList ID="cboType" runat="server" Width="330px" AutoPostBack="True"    OnSelectedIndexChanged="cboType_SelectedIndexChanged">
    <asp:ListItem Value="1">Select a template file</asp:ListItem>
    <asp:ListItem Value="2">Edit template</asp:ListItem>
</asp:DropDownList><br />
<asp:DropDownList ID="cboFiles" runat="server" Width="330px"   >
</asp:DropDownList>
<div runat="server" id="myDIV" class="Normal">
    <dnn:Label ID="lbTokens" runat="server" Visible="false" />
   
    <asp:TextBox ID="txtTemplate" Width="330px" Height="80px" runat="server" TextMode="MultiLine"   CssClass="NormalTextBox"></asp:TextBox>
</div></div>