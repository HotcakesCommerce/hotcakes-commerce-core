<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextHtmlEditor.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.EditorControls.TextHtmlEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTag" runat="server" ResourceKey="plTag" Suffix=":" />
    <asp:TextBox ID="txtTag" runat="server" Width="200px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtTag"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTitle" runat="server" Suffix=":" ResourceKey="plTitle"></dnn:Label>
    <asp:TextBox ID="txtTitle" runat="server" Width="200px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="Requiredfieldvalidator9" runat="server" ControlToValidate="txtTitle"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plHelpText" runat="server" Suffix=":" ResourceKey="plHelpText"></dnn:Label>
    <asp:TextBox ID="txtHelpText" runat="server" Width="424px"></asp:TextBox>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="lbTextBoxLength" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtTextBoxLength" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="Comparevalidator1" runat="server" ControlToValidate="txtTextBoxLength"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="lbTextBoxHeight" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtTextBoxHeight" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="Comparevalidator2" runat="server" ControlToValidate="txtTextBoxHeight"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="lbIsMultiLine" runat="server" Suffix=":"></dnn:Label>
    <asp:CheckBox ID="chkIsMultiLine" runat="server" />
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plViewOrder" runat="server" Suffix=":" ControlName="AnswerField">
    </dnn:Label>
    <asp:TextBox ID="txtViewOrder" runat="server" CssClass="NormalTextBox" Width="200px"
        MaxLength="3" Columns="20"></asp:TextBox><asp:CompareValidator ID="valViewOrder"
            resourcekey="ViewOrder.ErrorMessage" runat="server" CssClass="NormalRed" ErrorMessage="<br>View order must be an integer value."
            Display="Dynamic" ControlToValidate="txtViewOrder" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="lbIsRequire" runat="server" Suffix=":"></dnn:Label>
    <asp:CheckBox ID="chkIsRequire" runat="server" />
</div>
 