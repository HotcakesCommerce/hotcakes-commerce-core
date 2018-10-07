<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNUserArticles.UserListSettings" Codebehind="UserListSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
  <dnn:Label ID="lbModules" runat="server" Suffix=":"></dnn:Label>
  <asp:DropDownList ID="cboModules" CssClass="Normal" runat="server" AutoPostBack="true" >
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
  <dnn:Label ID="lblTemplate" runat="server" ControlName="lblTemplate" Suffix=":"></dnn:Label>
  <asp:TextBox ID="txtTemplate" runat="server" width="300px"  CssClass="NormalTextBox"></asp:TextBox>
</div>

