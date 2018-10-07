<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticle.EditDNNArticle" AutoEventWireup="true"
            Explicit="True" Codebehind="EditDNNArticle.ascx.cs" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="File" Src="ctlAttachedFile.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="ExtraField" Src="ctlExtraFieldList.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="page" Src="ctlPages.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="relatedarticles" Src="ctlRelatedArticles.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="changeAuthor" Src="ctlChangeAuthor.ascx" %>

<%@ Register TagPrefix="zldnn" TagName="version" Src="ctlVersion.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="categoryselector" Src="~/desktopmodules/dnnarticle/usercontrols/categoryselector.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="roleselector" Src="~/desktopmodules/dnnarticle/usercontrols/RoleSelector.ascx" %>

<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register TagPrefix="zldnn" TagName="AutoCS" Src="AutoCompleteBySeparator.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%--<%@Register TagPrefix="imm" Tagname="SelectImage" Src="~/DesktopModules/ZLDNNFileSearch/Controls/SelectImage.ascx" %>
--%>
<%@ Register TagPrefix="zldnn" TagName="FileSelector" Src="~/desktopmodules/dnnarticle/usercontrols/files/FileSelector.ascx" %>


<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee"
    BorderStyle="Solid" BorderWidth="1px" Transparency="50" Width="256px" Height="64px"
    RestoreOriginalRenderDelegate="false">
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle; height: 64px">
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                    runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>

<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
    <AjaxSettings>
    
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<asp:Panel ID="EditPanel" runat="server" Width="100%" CssClass="dnnForm">
  
    
    
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server"  MultiPageID="RadMultiPage1" Skin="Vista" 
                     SelectedIndex="0" Align="Left" ReorderTabsOnSelect="false" Width="95%">
    <Tabs>
        <telerik:RadTab Text="">
        </telerik:RadTab>
        <telerik:RadTab Text="">
        </telerik:RadTab>
        <telerik:RadTab Text="">
        </telerik:RadTab>
        <telerik:RadTab Text="">
        </telerik:RadTab>
        <telerik:RadTab Text="">
        </telerik:RadTab>
        <telerik:RadTab Text="">
        </telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>

<telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" CssClass="EditPageView"
    Width="95%">
    <telerik:RadPageView ID="pgBasic" runat="server">
    
    <div id="divBasicInfo" class="dnnForm" >
        <div class="dnnFormItem">
            <dnn:Label ID="lblTitle" runat="server" ControlName="lblTitle" Suffix=":" />
            <asp:TextBox ID="txtTitle" runat="server" Width="347px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle"
                ValidationGroup="ArticleEditor" EnableClientScript="true" CssClass="NormalRed"
                Display="Dynamic" ErrorMessage="*" resourcekey="Title.ErrorMessage" runat="server" />
        </div>
        <div class="dnnFormItem" runat="server" id="divArticleID">
            <dnn:Label ID="lblID" runat="server" ControlName="lblID" Suffix=":" />
            <asp:Label ID="lbID" runat="server" CssClass="Normal"></asp:Label>
        </div>
        <div class="dnnFormItem" id="divAuthorID" runat="server">
            <dnn:Label ID="lblAuthorID" runat="server" ControlName="lblAuthorID" Suffix=":" />
            <zldnn:changeAuthor runat="server" ID="myChangeAuthor" />
        </div>
        <div class="dnnFormItem" id="dCategory" runat="server" >
            <dnn:Label ID="lbCategories" runat="server" ControlName="lbCategories" Suffix=":" />
            <zldnn:categoryselector runat="server" ID="categoryselector" />
        </div>
        <div class="dnnFormItem" id="divTags" runat="server">
            <dnn:Label ID="lbEnterTags" runat="server"></dnn:Label>
            <asp:CheckBoxList ID="chkTagsList" runat="server" CssClass="Normal" RepeatColumns="5">
            </asp:CheckBoxList>
            <zldnn:AutoCS TextBoxCssClass="NormalTextBox" ID="ats" Width="350px" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plPublishDate" Suffix=":" ControlName="txtPublishDate" runat="server">
            </dnn:Label>
            <telerik:RadDateTimePicker ID="dtPublishDate" runat="server">
            </telerik:RadDateTimePicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plExpireDate" Suffix=":" ControlName="dtExpireDate" runat="server">
            </dnn:Label>
            <telerik:RadDateTimePicker ID="dtExpireDate" runat="server">
            </telerik:RadDateTimePicker>
        </div>
        <div class="dnnFormItem" runat="server" id="divContent">
            <asp:Label ID="lbContent" runat="server" ControlName="txtContent" resourcekey="lblContent" />
            <dnn:TextEditor ID="txtContent" runat="server" Height="500px" Width="100%" />
        </div>
        <div class="dnnFormItem"  runat="server" id="divDescription">
            <asp:Label ID="Label1" runat="server" ControlName="ctlSummary" resourcekey="lblSummary" />
            <dnn:TextEditor ID="ctlSummary" runat="server" Height="300" Width="100%" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lbKeyWords" runat="server" Suffix=":" />
            <asp:TextBox ID="txtKeyWords" runat="server" Width="347px" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblViewOrder" runat="server" ControlName="lblViewOrder" Suffix=":" />
            <asp:TextBox ID="txtViewOrder" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtViewOrder"
                ValidationGroup="ArticleEditor" EnableClientScript="true" ErrorMessage="Invalid Integer"
                resourcekey="InvalidInteger.ErrorMessage" Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFeatured" runat="server" ControlName="lblFeatured" Suffix=":" />
            <asp:CheckBox ID="chkFeatured" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIsApproved" runat="server" ControlName="lblIsApproved" Suffix=":" />
            <asp:CheckBox ID="chkApproved" runat="server" />
        </div>
        <div class="dnnFormItem" runat="server" id="divAllowComment">
            <dnn:Label ID="lblAllComment" runat="server" ControlName="lblAllComment" Suffix=":" />
            <asp:CheckBox ID="chkAllowComment" runat="server" />
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divImageAndURL">
            <h2 id="H1" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshURL")%></a></h2>
            <fieldset>
                <div class="dnnFormItem" runat="server" id="divImage">
                    <dnn:Label ID="lblURL" runat="server" ControlName="lblURL" Suffix=":" />
                    <asp:Label runat="server" ID="lbCopyrightnote" CssClass="Normal" resourcekey="lbCopyrightnote"
                        Visible="false"></asp:Label>
                    <div style="float: left">
                        <zldnn:FileSelector runat="server" ID="ctlImageURL"></zldnn:FileSelector>
                       <%-- <imm:selectimage runat="server" MaxSize="100" ID="mySelectImage" />--%>
                    </div>
                </div>
                <div class="dnnFormItem" runat="server" id="divURL">
                    <dnn:Label ID="lblReleatedURL" runat="server" ControlName="lblReleatedURL" Suffix=":" />
                    <div style="float: left">
                        <asp:Panel runat="server" ID="panelRelatedURL">
                            <Portal:URL ID="ctlReleatedURL" runat="server" Width="250" ShowNone="True" ShowTabs="True"
                                ShowUrls="True" ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True"
                                ShowNewWindow="False" />
                        </asp:Panel>
                    </div>
                </div>
            </fieldset>
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divSEO">
            <h2 id="H2" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshSEO")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSEOTitle" runat="server" ControlName="lblSEOTitle" Suffix=":" />
                    <asp:TextBox ID="txtSEOTitle" runat="server" Width="344px" CssClass="NormalTextBox"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSEODescription" runat="server" ControlName="lblSEODescription"
                        Suffix=":" />
                    <asp:TextBox ID="txtSEODescription" runat="server" Height="126px" TextMode="MultiLine"
                        Width="344px" CssClass="NormalTextBox"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSEOKeywords" runat="server" Suffix=":" />
                    <asp:TextBox ID="txtSEOKeywords" runat="server" Width="347px" CssClass="NormalTextBox"></asp:TextBox>
                </div>
                
                <div class="dnnFormItem">
                    <dnn:Label ID="lbURLTitle" runat="server" ControlName="lblSEOTitle" Suffix=":" />
                    <asp:TextBox ID="txtURLTitle" runat="server" Width="344px" CssClass="NormalTextBox"></asp:TextBox>
                </div>
            </fieldset>
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divSecurity">
            <h2 id="H3" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshSecurity")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lbViewRoles" runat="server" />
                    <zldnn:roleselector runat="server" ID="roleselector"></zldnn:roleselector>
                </div>
            </fieldset>
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divExtraField">
            <h2 id="H4" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshExtraFields")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <zldnn:ExtraField runat="server" ID="myExtraField" />
                </div>
            </fieldset>
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divResource">
            <h2 id="H5" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshResource")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <asp:CheckBoxList ID="chkResource" runat="server" CssClass="Normal" RepeatColumns="5">
                    </asp:CheckBoxList>
                </div>
            </fieldset>
        </div>
        
        <script type="text/javascript">
            jQuery(function ($) {
                var setupModule = function () {
                    $('#divBasicInfo').dnnPanels();
                    $('#divBasicInfo .dnnFormExpandContent a').dnnExpandAll({
                        targetArea: '#divBasicInfo'
                    });
                };
                setupModule();
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    // note that this will fire when _any_ UpdatePanel is triggered,
                    // which may or may not cause an issue
                    setupModule();
                });
            });
</script>            
    		
       
    </div>
      </telerik:RadPageView>
     <telerik:RadPageView ID="pgAttachment" runat="server">
    <div id="divFiles" class="dnnClear" runat="server">
        <zldnn:File runat="server" ID="myFiles" />
    </div>
     </telerik:RadPageView>
    <telerik:RadPageView ID="pgPages" runat="server">
    <div id="divPages" class="dnnClear" runat="server">
        <zldnn:page runat="server" ID="myPage" />
    </div>
     </telerik:RadPageView>
    <telerik:RadPageView ID="pgRelatedArticles" runat="server">
    <div id="divRelatedArticles" class="dnnClear" runat="server">
        <zldnn:relatedarticles runat="server" ID="myRelatedArticles" />
    </div>
     </telerik:RadPageView>
    <telerik:RadPageView ID="pgVersion" runat="server">
    <div id="divVersions" class="dnnClear" runat="server">
        <zldnn:version runat="server" ID="myVersion" />
    </div>
     </telerik:RadPageView>
    <telerik:RadPageView ID="pgLightbox" runat="server">
    <div id="divLightbox" class="dnnClear" runat="server">
        <asp:PlaceHolder ID="phlightbox" runat="server"></asp:PlaceHolder>
    </div>
        </telerik:RadPageView>
</telerik:RadMultiPage>
    <table>
        <tr>
            <td>
                <asp:LinkButton CssClass="UpdateLabel" ID="cmdSave" resourcekey="cmdSave" runat="server"
                    ValidationGroup="ArticleEditor" BorderStyle="none" CausesValidation="true" OnClick="cmdSave_Click"></asp:LinkButton>&nbsp;
                <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate" resourcekey="cmdUpdate" runat="server"
                    CausesValidation="true" ValidationGroup="ArticleEditor" BorderStyle="none" OnClick="cmdUpdate_Click"></asp:LinkButton>&nbsp;
                <asp:LinkButton CssClass="CancelLabel" ID="cmdCancel" resourcekey="cmdReturn" runat="server"
                    ValidationGroup="ArticleEditor" BorderStyle="none" CausesValidation="false" OnClick="cmdCancel_Click"></asp:LinkButton>&nbsp;
                <asp:LinkButton CssClass="DeleteLabel" ID="cmdDelete" resourcekey="cmdDelete" runat="server"
                    ValidationGroup="ArticleEditor" BorderStyle="none" CausesValidation="false" OnClick="cmdDelete_Click"></asp:LinkButton>
                &nbsp;
                <asp:LinkButton CssClass="GeoLabel" ID="cmdMultiLanguage" resourcekey="cmdMultiLanguage"
                    runat="server" BorderStyle="none" CausesValidation="false" OnClick="cmdMultiLanguage_Click"></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>
<%--<script type="text/javascript">
    jQuery(function ($) {
        $('#<% =EditPanel.ClientID%>').dnnTabs();

        var setupModule = function () {
            $('#<% =EditPanel.ClientID%>').dnnPanels();

        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });

</script>
--%>