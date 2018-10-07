<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RSSSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.RSSSettings" %>


<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshRssSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lbShowArticleNumber" runat="server"></dnn:label>
            <asp:TextBox ID="txtShowArticleNumber" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator1" ControlToValidate="txtShowArticleNumber"
                MaximumValue="1000" MinimumValue="1" Type="Integer" runat="server" ErrorMessage="(1-1000)"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbChannelTitle" runat="server"></dnn:label>
            <asp:TextBox ID="txtChannelTitle" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbChannelDescription" runat="server"></dnn:label>
            <asp:TextBox ID="txtChannelDescription" TextMode="MultiLine" Rows="5" Width="300px"
                CssClass="NormalTextBox" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbChannelLink" runat="server"></dnn:label>
            <asp:TextBox ID="txtChannelLink" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbChannelCopyright" runat="server"></dnn:label>
            <asp:TextBox ID="txtChannelCopyright" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbChannelWebMaster" runat="server"></dnn:label>
            <asp:TextBox ID="txtlbChannelWebMaster" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbItemTitle" runat="server"></dnn:label>
            <asp:TextBox ID="txtItemTitle" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbItemDescription" runat="server"></dnn:label>
            <asp:TextBox ID="txtItemDescription" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbItemCreator" runat="server"></dnn:label>
            <asp:TextBox ID="txtItemCreator" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblPublishDate" runat="server"></dnn:label>
            <asp:TextBox ID="txtPublishDate" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbCacheTime" runat="server"></dnn:label>
            <asp:TextBox ID="txtCacheTime" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" ControlToValidate="txtCacheTime"
                MaximumValue="9999" MinimumValue="0" Type="Integer" runat="server" ErrorMessage="(0-9999)"></asp:RangeValidator>
        </div>
    </fieldset>
</div>
