<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NumericEditor.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.EditorControls.NumericEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormItem dnnClear" >
    <dnn:Label ID="plTag" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtTag" runat="server" Width="200px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTag"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTitle" runat="server" Suffix=":" ResourceKey="plTitle" ControlName="plTitle"></dnn:Label>
    <asp:TextBox ID="txtTitle" runat="server" Width="424px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtTitle"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plHelpText" runat="server" Suffix=":" ControlName="txtContent"></dnn:Label>
    <asp:TextBox ID="txtHelpText" runat="server" Width="424px"></asp:TextBox>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plDefaultValue" runat="server" Suffix=":" ControlName="txtContent">
    </dnn:Label>
    <asp:TextBox ID="txtDefaultValue" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="cpIntegerDefault" runat="server" ControlToValidate="txtDefaultValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
    <asp:CompareValidator ID="cpDecimalDefault" runat="server" ControlToValidate="txtDefaultValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Double" Display="Dynamic"></asp:CompareValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plMaxValue" runat="server" Suffix=":" ControlName="txtContent"></dnn:Label>
    <asp:TextBox ID="txtMaxValue" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="cpIntegermax" runat="server" ControlToValidate="txtMaxValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
    <asp:CompareValidator ID="cpDecimalMax" runat="server" ControlToValidate="txtMaxValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Double" Display="Dynamic"></asp:CompareValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plMinValue" runat="server" Suffix=":" ControlName="txtContent"></dnn:Label>
    <asp:TextBox ID="txtMinValue" runat="server" Width="200px"></asp:TextBox>
    <asp:CompareValidator ID="cpIntegerMin" runat="server" ControlToValidate="txtMinValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
    <asp:CompareValidator ID="cpDecimalMin" runat="server" ControlToValidate="txtMinValue"
        ErrorMessage="Invalidated" Operator="DataTypeCheck" Type="Double" Display="Dynamic"></asp:CompareValidator>
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
