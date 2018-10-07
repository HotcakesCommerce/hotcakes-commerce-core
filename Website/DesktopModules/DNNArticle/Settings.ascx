<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticle.Settings"
            Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="TwitterSettings" Src="~/desktopmodules/dnnarticle/modulesettings/TwitterSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="FacebookSettings" Src="~/desktopmodules/dnnarticle/modulesettings/FacebookSettings.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="DNNJournalSettings" Src="~/desktopmodules/dnnarticle/modulesettings/DNNJournalSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="GeneralSettings" Src="~/desktopmodules/dnnarticle/modulesettings/generalsettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="ArticleViewSettings" Src="~/desktopmodules/dnnarticle/modulesettings/ArticleViewSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="EmailNotificationSettings" Src="~/desktopmodules/dnnarticle/modulesettings/EmailNotificationSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="EditSettings" Src="~/desktopmodules/dnnarticle/modulesettings/EditSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CommentSettings" Src="~/desktopmodules/dnnarticle/modulesettings/CommentSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="SecuritySettings" Src="~/desktopmodules/dnnarticle/modulesettings/SecuritySettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="MultiLanguageSettings" Src="~/desktopmodules/dnnarticle/modulesettings/MultiLanguageSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="ActiveSocialJournalSettings" Src="~/desktopmodules/dnnarticle/modulesettings/ActiveSocialJournalSettings.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="RSSSettings" Src="~/desktopmodules/dnnarticle/modulesettings/RSSSettings.ascx" %>

<asp:ValidationSummary ID="ValidationSummary1"   ShowSummary="True"  runat="server" />

<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshGeneral")%></a></h2>
    <fieldset>
        <zldnn:GeneralSettings runat="server" ID="GeneralSettings" />
    </fieldset>
</div>

<div class="dnnFormItem  dnnClear">
    <h2 id="H1" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshDetailView")%></a></h2>
    <fieldset>
        <zldnn:ArticleViewSettings runat="server" ID="ArticleViewSettings" />
        </fieldset>
</div>
<div class="dnnFormItem  dnnClear">
    <h2 id="H3" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshEmailNotification")%></a></h2>
    <fieldset>
        <zldnn:EmailNotificationSettings runat="server" ID="EmailNotificationSettings" />
        </fieldset>
</div>
<div class="dnnFormItem  dnnClear">
    <h2 id="H4" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshSecurity")%></a></h2>
    <fieldset>
        <zldnn:SecuritySettings runat="server" ID="SecuritySettings" />
        </fieldset>
</div>
<div class="dnnFormItem  dnnClear">
    <h2 id="H5" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshEdit")%></a></h2>
    <fieldset>
         <zldnn:EditSettings runat="server" ID="EditSettings" />
        </fieldset>
</div>


<zldnn:RSSSettings runat="server" ID="RSSSettings" />

<div class="dnnFormItem  dnnClear">
    <h2 id="H9" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshComment")%></a></h2>
    <fieldset>
        <zldnn:CommentSettings runat="server" ID="CommentSettings" />
        </fieldset>
</div>
<div class="dnnFormItem  dnnClear" runat="server" id="divMultiLanguage">
    <h2 id="H10" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshMultiLanguage")%></a></h2>
    <fieldset>
        <zldnn:MultiLanguageSettings runat="server" ID="MultiLanguageSettings" />
        </fieldset>
</div>

<zldnn:TwitterSettings runat="server" ID="TwitterSettings" />

<zldnn:FacebookSettings runat="server" ID="FacebookSettings" />

<zldnn:DNNJournalSettings runat="server" ID="DNNJournalSettings" />

<zldnn:ActiveSocialJournalSettings runat="server" ID="ActiveSocialJournalSettings" />

<div class="dnnFormItem  dnnClear" runat="server" id="divlightboxContent">
    <h2 id="H7" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshlightboxContent")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lbEnableLightBoxContent" runat="server" Suffix=":"></dnn:Label>
             <asp:CheckBox ID="chkEnableLightboxContent" runat="server" />
         </div>
          <div class="dnnFormItem">
               <asp:PlaceHolder ID="phLightbox" runat="server"></asp:PlaceHolder>
           </div>
     </fieldset>
</div>


