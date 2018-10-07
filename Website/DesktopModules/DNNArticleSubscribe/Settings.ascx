<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleSubscribe.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<div class="dnnFormItem">
    <dnn:Label ID="lbModule" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboModule" runat="server" Width="200px"  CssClass="Normal">
            </asp:DropDownList><br />
            <asp:Label ID="lbWarning" runat="server" CssClass="NormalRed" resourcekey="lbWarning"></asp:Label>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbEmailSubject" runat="server" ControlName="lbEmailSubject" />
    <asp:TextBox ID="txtEmailSubject" runat="server" Width="192px"  CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbEmailTemplate" runat="server" ControlName="lblEmailTemplate" Suffix=":">
            </dnn:Label>
<asp:TextBox ID="txtEmailTemplate" runat="server" TextMode="MultiLine" Width="330px"  CssClass="NormalTextBox"
                         Height="200px"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbExpandNode" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkExpandNode" runat="server" Width="120px" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbShowLines" runat="server" Suffix=":" />
    <asp:CheckBox ID="chkShowLines" runat="server" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbIndent" runat="server" Suffix=":" />
    <asp:TextBox ID="txtIndent" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtIndent"
                                ErrorMessage="(0-200)" MaximumValue="200" MinimumValue="0" Type="Integer"></asp:RangeValidator>
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbTreeNodeImage" runat="server" Suffix=":" ControlName="lbTreeNodeImage">
            </dnn:Label>
 <Portal:URL ID="ctlTreeNodeImage" runat="server" Width="200" ShowTabs="False" ShowUrls="False"
                        ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True" UrlType="F"
                        ShowNewWindow="False"></Portal:URL>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbCollapseImageURL" runat="server" Suffix=":" />
    
            <Portal:URL ID="ctlCollapseImageURL" runat="server" Required="false" ShowLog="False"
                        ShowNewWindow="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="True" ShowUrls="False"
                        UrlType="F" Width="200" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbExpandImageURL" runat="server" Suffix=":" />
    <Portal:URL ID="ctlExpandImageURL" runat="server" Required="false" ShowLog="False"
                        ShowNewWindow="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="True" ShowUrls="False"
                        UrlType="F" Width="200" />
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbSelectedNodeStyle" runat="server" Suffix=":" />
     <asp:TextBox ID="txtSelectedNodeStyle" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem">
    <dnn:Label ID="lbNodeStyle" runat="server" Suffix=":" />
     <asp:TextBox ID="txtNodeStyle" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
</div>


      