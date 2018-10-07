<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignToMainModule.ascx.cs" Inherits="ZLDNN.Modules.DNNUserArticles.AssignToMainModule" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
  <dnn:Label ID="lbAssignToMainModule" runat="server" Suffix=":" ></dnn:Label>
  <asp:CheckBox ID="chkAssignToMainModule" runat="server" AutoPostBack="True" 
        oncheckedchanged="chkAssignToMainModule_CheckedChanged" />
</div>


<div class="dnnFormItem" runat="server" id="divUserArticleModule">
  <dnn:Label ID="lbModules" runat="server" Suffix=":" ></dnn:Label>
  <asp:DropDownList ID="cboModules" runat="server" CssClass="Normal"></asp:DropDownList>
</div>

<div runat="server" id="divMainModuleSettings">
    <div class="dnnFormItem">
        <dnn:Label ID="lbMainModule" runat="server" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="cboMainModule" runat="server" CssClass="Normal">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbDNNArticleListModule" runat="server" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="cboDNNArticleListModule" runat="server" CssClass="Normal">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lbUserIDPassIn" runat="server" CssClass="SubHead"></dnn:Label>
        <asp:TextBox ID="txtUserIDPassIn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
    </div>
</div>
