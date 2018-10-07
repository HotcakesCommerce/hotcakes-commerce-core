<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleTagCloud.Settings"
            Codebehind="Settings.ascx.cs" %>
    
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
            <dnn:Label ID="lbPortal" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboPortals" runat="server" AutoPostBack="True" Width="200"
                               OnSelectedIndexChanged="cboPortals_SelectedIndexChanged">
            </asp:DropDownList></div>
<div class="dnnFormItem">
    <dnn:label id="lbModule" runat="server"></dnn:label>
    <asp:DropDownList ID="cboModule" runat="server" Width="300px"  AutoPostBack="true"  OnSelectedIndexChanged="cboModule_SelectedIndexChanged">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbCategory" runat="server"></dnn:label>
    <asp:DropDownList ID="cboCategory" runat="server" Width="300px"  >
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbTabs" runat="server" controlname="lbTabs" />
    <asp:DropDownList ID="cboTabs" runat="server" Width="300px" >
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbSpaceBetweenTags" runat="server" css></dnn:label>
    <asp:TextBox ID="txtSpacceBetweenTags" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator1" runat="server" ControlToValidate="txtSpacceBetweenTags"
                Type="Integer" MinimumValue="0" MaximumValue="40" ErrorMessage="(0-40)"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbTagTemplate" runat="server" css></dnn:label>
    <asp:TextBox ID="txtTagTemplate" runat="server"   CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbAutoGenerateStyle" runat="server" css></dnn:label>
    <asp:CheckBox ID="chkAutoGenerateStyle" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:label id="lbRefnumber" runat="server" css></dnn:label>
     <asp:TextBox ID="txtRefnumber" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator4" runat="server" ControlToValidate="txtRefnumber" Type="Integer"
                MinimumValue="0" MaximumValue="10000" ErrorMessage="Integer"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbFontSizeOfMaxNumber" runat="server" css></dnn:label>
    <asp:TextBox ID="txtFontSizeOfMaxNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator2" runat="server" ControlToValidate="txtFontSizeOfMaxNumber"
                Type="Integer" MinimumValue="1" MaximumValue="100" ErrorMessage="(1-100)"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbFontSizeOfMinNumber" runat="server" css></dnn:label>
    <asp:TextBox ID="txtFontSizeOfMinNumber" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator3" runat="server" ControlToValidate="txtFontSizeOfMinNumber"
                Type="Integer" MinimumValue="1" MaximumValue="100" ErrorMessage="(1-100)"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbColors" runat="server" css></dnn:label>
    <asp:TextBox ID="txtColors" runat="server" TextMode="MultiLine" Rows="5" Width="200"
                CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbOtherStyle" runat="server" css></dnn:label>
    <asp:TextBox ID="txtOtherStyle" runat="server" TextMode="MultiLine" Rows="5" Width="200"
                CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:label id="lbAltTextTemplate" runat="server" css></dnn:label>
    <asp:TextBox ID="txtAltTextTemplate" runat="server" Width="200"   CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:label id="lbAltTextTemplateOneArticle" runat="server"   ></dnn:label>
    <asp:TextBox ID="txtAltTextTemplateOneArticle" runat="server" Width="200" CssClass="NormalTextBox"></asp:TextBox>
</div>

