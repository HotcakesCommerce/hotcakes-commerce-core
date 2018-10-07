<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailNotificationSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.EmailNotificationSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/dnnarticle/TemplateSelector.ascx" %>
<div class="dnnFormItem">
    <dnn:Label ID="lbEmailAlert" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkEmailAlert" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbCommentFromEmailAddress" runat="server"></dnn:Label>
    <asp:TextBox ID="txtCommentFromEmailAddress" runat="server" CssClass="NormalTextBox"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtCommentFromEmailAddress"
        runat="server" ErrorMessage="Name@Domain" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbEmail" runat="server"></dnn:Label>
    <asp:TextBox ID="txtEmail" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalidate Email"
        Display="Dynamic" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbEmailSubject" runat="server" ControlName="lbEmailSubject" />
    <asp:TextBox ID="txtEmailSubject" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbEmailTemplate" runat="server" ControlName="lblEmailTemplate" Suffix=":">
    </dnn:Label>
    <zldnn:selector runat="server" ID="ctlEmailTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbNewArticleEmailSubjectToAuthor" runat="server" ControlName="lbEmailSubject" />
    <asp:TextBox ID="txtNewArticleEmailSubjectToAuthor" runat="server" Width="192px"
        CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbNewArticleEmailTemplateToAuthor" runat="server" ControlName="lblEmailTemplate"
        Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtNewArticleEmailTemplateToAuthor" runat="server" TextMode="MultiLine"
        CssClass="NormalTextBox" Width="330px" Height="200px"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbApprovedArticleEmailSubjectToAuthor" runat="server" ControlName="lbEmailSubject" />
    <asp:TextBox ID="txtApprovedArticleEmailSubjectToAuthor" runat="server" Width="192px"
        CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbApprovedArticleEmailTemplateToAuthor" runat="server" ControlName="lblEmailTemplate"
        Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtApprovedArticleEmailTemplateToAuthor" runat="server" TextMode="MultiLine"
        CssClass="NormalTextBox" Width="330px" Height="200px"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lbSendToViewRoles" runat="server"></dnn:Label>
    <asp:CheckBox ID="chkSendToViewRoles" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem" runat="server" visible="false">
    <dnn:Label ID="lbSubScribeRole" runat="server" ControlName="lblEmailTemplate" Suffix=":">
    </dnn:Label>
    <asp:DropDownList ID="cboSubScribeRole" runat="server" >
    </asp:DropDownList>
</div>
<div class="dnnFormItem" runat="server" visible="false">
    <dnn:Label ID="lbScribeEmailSubject" runat="server" ControlName="lbEmailSubject" />
    <asp:TextBox ID="txtScribeEmailSubject" runat="server" Width="192px" CssClass="NormalTextBox"></asp:TextBox>
</div>
<div class="dnnFormItem" runat="server" visible="false">
    <dnn:Label ID="lbScribeEmailTemplate" runat="server" ControlName="lblEmailTemplate"
        Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtScribeEmailTemplate" runat="server" TextMode="MultiLine" CssClass="NormalTextBox"
        Width="330px" Height="200px"></asp:TextBox>
</div>
