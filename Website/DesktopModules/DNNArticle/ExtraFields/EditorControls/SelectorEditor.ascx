<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectorEditor.ascx.cs"
    Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.EditorControls.SelectorEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTag" runat="server" Suffix=":" ResourceKey="plTag"></dnn:Label>
    <asp:TextBox ID="txtTag" runat="server" Width="200px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="Requiredfieldvalidator6" runat="server" ControlToValidate="txtTag"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plTitle" runat="server" Suffix=":" ResourceKey="plTitle"></dnn:Label>
    <asp:TextBox ID="txtTitle" runat="server" Width="424px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="Requiredfieldvalidator7" runat="server" ControlToValidate="txtTitle"
        ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plHelpText" runat="server" Suffix=":" ResourceKey="plHelpText"></dnn:Label>
    <asp:TextBox ID="txtHelpText" runat="server" Width="424px"></asp:TextBox>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plSelectionType" runat="server" Suffix=":"></dnn:Label>
    <asp:DropDownList ID="cboType" runat="server" Width="192px" CssClass="Normal" >
        <asp:ListItem Value="DropDownList">DropDownList</asp:ListItem>
        <asp:ListItem Value="RadioButtons">RadioButtons</asp:ListItem>
       
    </asp:DropDownList>
</div>
<div class="dnnFormItem dnnClear">
    <dnn:Label ID="plSelectorValue" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtSelectorValue" runat="server" Width="424px" Height="136px" TextMode="MultiLine"></asp:TextBox>
    <asp:RequiredFieldValidator ID="Requiredfieldvalidator8" runat="server" ControlToValidate="txtSelectorValue"
        ErrorMessage="*"></asp:RequiredFieldValidator>
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
</div>
