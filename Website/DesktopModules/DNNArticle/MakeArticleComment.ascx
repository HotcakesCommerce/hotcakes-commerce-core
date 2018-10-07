<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MakeArticleComment.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.MakeArticleComment" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>


<asp:Literal ID="ltFinish" runat="server"></asp:Literal>
<asp:Panel runat="server" ID="panel1">
    <asp:Literal ID="lt" runat="server"></asp:Literal>
    <asp:Panel runat="server" ID="panelCommentSubmit">
        <asp:Panel ID="panelMakeComment" runat="server" CssClass="panelMakeComment">
            <div class="dnnFormItem" runat="server" id="divName"> 
                <dnn:Label ID="plYourName" runat="server" CssClass="NormalBold" resourcekey="lbYourName"></dnn:Label>
                <asp:TextBox runat="server" ID="txtName" CssClass="cmtName">
                </asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvName" ControlToValidate="txtName" ValidationGroup="NewArticleComment"
                    EnableClientScript="true" CssClass="CommentFormError" Display="Dynamic" ErrorMessage="*"
                    runat="server" />
            </div>
            <div class="dnnFormItem"  runat="server" id="divEmail">
                <dnn:Label ID="lbEmail" runat="server" CssClass="NormalBold" resourcekey="lbEmail"></dnn:Label>
                <asp:TextBox runat="server" ID="txtEmail" CssClass="cmtEmail">
                </asp:TextBox>
                <asp:RegularExpressionValidator ID="rfvEmailFormat" runat="server" ControlToValidate="txtEmail"
                    ValidationGroup="NewArticleComment" EnableClientScript="true" CssClass="CommentFormError"
                    Display="Dynamic" ErrorMessage="username@domainanme" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                
                <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" ValidationGroup="NewArticleComment"
                    EnableClientScript="true" CssClass="CommentFormError" Display="Dynamic" ErrorMessage="*"
                    runat="server" />
            </div>
            <div class="dnnFormItem"   runat="server" id="divWebSite">
                <dnn:Label ID="lbWebSite" runat="server" CssClass="NormalBold" resourcekey="lbWebsite"></dnn:Label>
                <asp:TextBox runat="server" ID="txtWebSite" CssClass="cmtWebsite">
                </asp:TextBox>
               
                <asp:RequiredFieldValidator ID="rfvWebSite" ControlToValidate="txtWebSite" ValidationGroup="NewArticleComment"
                    EnableClientScript="true" CssClass="CommentFormError" Display="Dynamic" ErrorMessage="*"
                    runat="server" />
            </div>
            <div class="dnnFormItem"  runat="server" id="divTitle">
                <dnn:Label ID="plCommentTitle" runat="server" CssClass="NormalBold" resourcekey="lbCommentTitle"></dnn:Label>
                <asp:TextBox runat="server" ID="txtTitle" CssClass="cmtTitle">
                </asp:TextBox>
               
                <asp:RequiredFieldValidator ID="rfvTitle" ControlToValidate="txtTitle" ValidationGroup="NewArticleComment"
                    EnableClientScript="true" CssClass=" CommentFormError" Display="Dynamic" ErrorMessage="*"
                    runat="server" />
            </div>
            <div class="dnnFormItem"  runat="server" id="divComment">
                <dnn:Label ID="plComment" runat="server" CssClass="NormalBold" resourcekey="lbComment"></dnn:Label>
                <asp:TextBox runat="server" ID="txtComment" CssClass="cmtComment" TextMode="MultiLine">
                </asp:TextBox>
              
                <asp:RequiredFieldValidator ID="rfvComment" ControlToValidate="txtComment" ValidationGroup="NewArticleComment"
                    EnableClientScript="true" CssClass=" CommentFormError" Display="Dynamic" ErrorMessage="*"
                    runat="server" />
            </div>
            <div class="dnnFormItem">
                <asp:CheckBox ID="chkNotification" CssClass="chkNotification" runat="server" />
                </div>
            <div class="dnnFormItem">
            <asp:Panel runat="server" ID="panelCap" CssClass="panelCap">
                <cc1:CaptchaControl ID="CaptchaControl1" runat="server" CaptchaHeight="50px" CaptchaWidth="150px"
                    CssClass="CommentCaptcha" />
            </asp:Panel></div>

        </asp:Panel>
        <div style="clear: both">
        </div>

        <asp:Label ID="ltNote" runat="server"></asp:Label>
        <div class="dnnFormItem">
        <asp:LinkButton ID="cmdSubmit" runat="server" CssClass="dnnPrimaryAction" ValidationGroup="NewArticleComment"
            OnClick="cmdSubmit_Click">Submit</asp:LinkButton>
        </div>
    </asp:Panel>
</asp:Panel>
