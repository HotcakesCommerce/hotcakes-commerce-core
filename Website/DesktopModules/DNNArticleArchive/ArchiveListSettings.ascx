<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleArchive.ArchiveListSettings"
            Codebehind="ArchiveListSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
    <dnn:label ID="lbModule" runat="server" CssClass="SubHead"></dnn:label>
    <asp:DropDownList ID="cboModule" runat="server" Width="200px"  CssClass="Normal">
            </asp:DropDownList>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbHideMe" runat="server"></dnn:label>
    <asp:CheckBox ID="chkHideMe" runat="server"></asp:CheckBox>
</div>
