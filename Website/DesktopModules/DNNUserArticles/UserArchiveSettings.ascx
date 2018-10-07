<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNUserArticles.UserArchiveSettings" Codebehind="UserArchiveSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
  <dnn:Label ID="lbModules" runat="server" Suffix=":" ></dnn:Label>
  <asp:DropDownList ID="cboModules" runat="server" AutoPostBack="true"  CssClass="Normal">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
  <dnn:Label ID="lblTemplate" runat="server" ControlName="lblTemplate" Suffix=":"></dnn:Label>
  <asp:TextBox ID="txtTemplate" runat="server" CssClass="NormalTextBox" Width="300px" TextMode="MultiLine" Rows="5" ></asp:TextBox>
</div>

