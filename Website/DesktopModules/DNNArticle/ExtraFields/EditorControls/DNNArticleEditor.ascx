<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DNNArticleEditor.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.EditorControls.DNNArticleEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTag" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtTag" runat="server" Width="200px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTag"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTitle" runat="server" Suffix=":" ResourceKey="plTitle"></dnn:Label>
    <asp:TextBox ID="txtTitle" runat="server" Width="424px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtTitle"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plHelpText" runat="server" Suffix=":" ControlName="txtContent"></dnn:Label>
    <asp:TextBox ID="txtHelpText" runat="server" Width="424px"></asp:TextBox>
</div>

<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plSelectedModule" runat="server" Suffix=":" ControlName="txtContent"></dnn:Label>
    <asp:DropDownList ID="cboSelectedModule" runat="server">
</asp:DropDownList>
</div>


<div class="dnnFormItem  dnnClear">
    <dnn:Label ID="plViewOrder" runat="server" Suffix=":" >
    </dnn:Label>
    <asp:TextBox ID="txtViewOrder" runat="server" CssClass="NormalTextBox" Width="200px"
        MaxLength="3" Columns="20"></asp:TextBox><asp:CompareValidator ID="valViewOrder"
            resourcekey="ViewOrder.ErrorMessage" runat="server" CssClass="NormalRed" ErrorMessage="<br>View order must be an integer value."
            Display="Dynamic" ControlToValidate="txtViewOrder" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
</div>
