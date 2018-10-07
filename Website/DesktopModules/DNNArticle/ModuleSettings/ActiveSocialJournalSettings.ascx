<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActiveSocialJournalSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.ActiveSocialJournalSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
      <dnn:Label ID="lbIntegrateWithActiveSocialJournal" runat="server" Suffix=":"></dnn:Label>
      <asp:CheckBox ID="chkIntegrateWithActiveSocialJournal" runat="server" />
</div>
<div class="dnnFormItem">
      <dnn:Label ID="lbGUIDCreateArticle" Suffix=":" runat="server"></dnn:Label>
      <asp:TextBox ID="txtGUIDCreateArticle" runat="server" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
       <dnn:Label ID="lbSecuritySettings" Suffix=":" runat="server"></dnn:Label>
       <asp:DropDownList ID="cboSecuritySettings" runat="server">
                <asp:ListItem Text="Inherit/Default" Value="0"></asp:ListItem>
                <asp:ListItem Text="Everyone" Value="1"></asp:ListItem>
                <asp:ListItem Text="Community" Value="2"></asp:ListItem>
                <asp:ListItem Text="FriendsOnly" Value="3"></asp:ListItem>
                <asp:ListItem Text="GroupMembers" Value="4"></asp:ListItem>
                <asp:ListItem Text="Private" Value="5"></asp:ListItem>
            </asp:DropDownList>
</div>
