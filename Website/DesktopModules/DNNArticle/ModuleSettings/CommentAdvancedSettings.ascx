<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentAdvancedSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.CommentAdvancedSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>

<div style="padding-left: 30px">
    <div class="dnnFormItem  dnnClear">
        <h2 id="H2" class="dnnFormSectionHead">
            <a href="" class="">
                <%=LocalizeString("dshCommentAdvancedSettings")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentsDisplayStyle" runat="server"></dnn:label>
                <asp:DropDownList ID="cboCommentsDisplayStyle" runat="server"  AutoPostBack="True"
                    OnSelectedIndexChanged="cboCommentsDisplayStyle_SelectedIndexChanged">
                    <asp:ListItem Value="0" resourcekey="liFlat">Flat</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liThreading">Threading</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem" runat="server" id="divThreadMaxDepth">
                <dnn:label ID="lbThreadMaxDepth" runat="server"></dnn:label>
                <asp:TextBox ID="txtThreadMaxDepth" Width="100px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                <asp:RangeValidator ID="Rangevalidator11" runat="server" ControlToValidate="txtThreadMaxDepth"
                    Type="Integer" MinimumValue="1" MaximumValue="10" ErrorMessage="(1-10)"></asp:RangeValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAllowOwnerDelete" runat="server"></dnn:label>
                <asp:CheckBox ID="chkAllowOwnerDelete" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbNoCommentText" runat="server"></dnn:label>
                <asp:TextBox ID="txtNoCommentText" Width="300px" CssClass="NormalTextBox" runat="server"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbLockCommentText" runat="server"></dnn:label>
                <asp:TextBox ID="txtLockCommentText" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCannotCommentText" runat="server"></dnn:label>
                <asp:TextBox ID="txtCannotCommentText" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteFlat">
                <dnn:label ID="lbCommentTemplateFlat" runat="server"></dnn:label>
                <zldnn:selector runat="server" ID="ctlCommentTemplateSelectorFlat" />
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteFlatHeader">
                <dnn:label ID="lbCommentListHeaderTemplateFlat" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentListHeaderTemplateFlat" Width="300px" TextMode="MultiLine"
                    Rows="5" Height="200px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteFlatFooter">
                <dnn:label ID="lbCommentListFooterTemplateFlat" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentListFooterTemplateFlat" Width="300px" TextMode="MultiLine"
                    Rows="5" Height="200px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteThread">
                <dnn:label ID="lbCommentTemplateThread" runat="server"></dnn:label>
                <zldnn:selector runat="server" ID="ctlCommentTemplateSelectorThread" />
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteThreadHeader">
                <dnn:label ID="lbCommentListHeaderTemplateThread" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentListHeaderTemplateThread" Width="300px" TextMode="MultiLine"
                    Rows="5" Height="200px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem" runat="server" id="divCommentTempalteThreadFooter">
                <dnn:label ID="lbCommentListFooterTemplateThread" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentListFooterTemplateThread" Width="300px" TextMode="MultiLine"
                    Rows="5" Height="200px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAuthorNameRequired" runat="server"></dnn:label>
                <asp:DropDownList ID="cboAuthorNameRequired" runat="server" >
                    <asp:ListItem Value="0" resourcekey="liHidden">Hidden</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liRequired">Required</asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="liOptional">Optional</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAuthorEmailRequired" runat="server"></dnn:label>
                <asp:DropDownList ID="cboAuthorEmailRequired" runat="server" >
                    <asp:ListItem Value="0" resourcekey="liHidden">Hidden</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liRequired">Required</asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="liOptional">Optional</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAuthorWebsiteRequired" runat="server"></dnn:label>
                <asp:DropDownList ID="cboAuthorWebsiteRequired" runat="server" >
                    <asp:ListItem Value="0" resourcekey="liHidden">Hidden</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liRequired">Required</asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="liOptional">Optional</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentTitleRequired" runat="server"></dnn:label>
                <asp:DropDownList ID="cboCommentTitleRequired" runat="server" >
                    <asp:ListItem Value="0" resourcekey="liHidden">Hidden</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liRequired">Required</asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="liOptional">Optional</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbHideUserInfoFieldforRegisteredUsers" runat="server"></dnn:label>
                <asp:CheckBox ID="chkHideUserInfoFieldforRegisteredUsers" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAddCommentText" runat="server"></dnn:label>
                <asp:TextBox ID="txtAddCommentText" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbAddCommentCSS" runat="server"></dnn:label>
                <asp:TextBox ID="txtAddCommentCSS" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbSubmitCSS" runat="server"></dnn:label>
                <asp:TextBox ID="txtSubmitCSS" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentFormCSS" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentFormCSS" Width="300px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbDoNotDisplayCommentIfItFlagged" runat="server"></dnn:label>
                <asp:TextBox ID="txtFlaggedNumber" Width="100px" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                <asp:RangeValidator ID="Rangevalidator1" runat="server" ControlToValidate="txtFlaggedNumber"
                    Type="Integer" MinimumValue="1" MaximumValue="100" ErrorMessage="(1-100)"></asp:RangeValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbSortType" runat="server"></dnn:label>
                <asp:DropDownList ID="cboSortType" runat="server" >
                    <asp:ListItem Value="0" resourcekey="liNewestFirst">NewestFirst</asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="liOldestFirst">OldestFirst</asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="liMostPopular">MostPopular</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbShowSortTypeBoxToUser" runat="server"></dnn:label>
                <asp:CheckBox ID="chkShowSortTypeBoxToUser" runat="server" />
            </div>
            <div class="dnnFormItem" runat="server" Visible="False">
                <dnn:label ID="lbPageControlType" runat="server"></dnn:label>
                <asp:DropDownList ID="cboPageControlType" runat="server" >
                    <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="PageNumberListWithoutPostBack"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentPageSize" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator8" runat="server" ControlToValidate="txtCommentPageSize"
                    ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbCommentTextBoxWidth" runat="server"></dnn:label>
                <asp:TextBox ID="txtCommentTextBoxWidth" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtCommentTextBoxWidth"
                    ErrorMessage="(1-9999)" Type="Integer" MaximumValue="9999" MinimumValue="1"></asp:RangeValidator>
            </div>
        </fieldset>
    </div>
</div>
