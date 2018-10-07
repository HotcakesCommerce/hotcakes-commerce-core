<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentEmailSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.CommentEmailSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<div style="padding-left: 30px">
    <div class="dnnFormItem  dnnClear">
        <h2 id="H2" class="dnnFormSectionHead">
            <a href="" class="">
                <%=LocalizeString("dshCommentEmailSettings.Text")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableCommentEmailAlert" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnalbeCommentEmailAlert" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentEmailAddress" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentEmailAddress" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                <asp:RegularExpressionValidator ID="valCommentEmailAddress" ControlToValidate="txtCommentEmailAddress"
                    runat="server" ErrorMessage="Name@Domain" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentEmailSubject" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentEmailSubject" runat="server" Width="300px" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentEmailTemplate" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentEmailTemplate" runat="server" TextMode="MultiLine" Width="300px"
                    CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableEmailToCommentModerator" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnableEmailToCommentModerator" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentEmailSubjectToModerator" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentEmailSubjectToModerator" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentEmailTemplateToModerator" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentEmailTemplateToModerator" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableEmailToArticleAuthor" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnableEmailToArticleAuthor" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectTemplateToArticleAuthor" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectTemplateToArticleAuthor" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateToArticleAuthor" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateToArticleAuthor" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableEmailToCommentAuthor" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnableEmailToCommentAuthor" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectTempalteToCommentAuthorApprove" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectTemplateToCommentAuthorApprove" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateToCommentAuthorApprove" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateToCommentAuthorApprove" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectTemplateToCommentAuthorReject" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectTemplateToCommentAuthorReject" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateToCommentAuthorReject" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateToCommentAuthorReject" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailToCommentAuthorDelete" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEmailToCommentAuthorDelete" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectToCommentAuthorDelete" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectToCommentAuthorDelete" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateToCommentAuthorDelete" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateToCommentAuthorDelete" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableEmailToParentCommentAuthor" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnableEmailToParentCommentAuthor" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectToParentCommentAuthor" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectToParentCommentAuthor" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateToParentCommentAuthor" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateToParentCommentAuthor" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEnableEmailWhenCommentFlagged" runat="server"></dnn:label>
                <asp:CheckBox ID="chkEnableEmailWhenCommentFlagged" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailSubjectWhenCommentFlagged" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailSubjectWhenCommentFlagged" runat="server" CssClass="NormalTextBox"
                    Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbEmailTemplateWhenCommentFlagged" runat="server"></dnn:label>
                <asp:TextBox ID="txtEmailTemplateWhenCommentFlagged" runat="server" TextMode="MultiLine"
                    Width="300px" CssClass="NormalTextBox" Height="134px"></asp:TextBox>
            </div>
        </fieldset>
    </div>
</div>
