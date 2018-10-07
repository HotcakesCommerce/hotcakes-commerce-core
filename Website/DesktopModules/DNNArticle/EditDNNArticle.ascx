<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticle.EditDNNArticle" AutoEventWireup="true"
            Explicit="True" Codebehind="EditDNNArticle.ascx.cs" %>

<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>

<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<%@ Register TagPrefix="zldnn" TagName="pickdate" Src="~/desktopmodules/dnnarticle/usercontrols/ctlDateTimePicker.ascx" %>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js"  Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>


<asp:Panel ID="EditPanel" runat="server" Width="100%" CssClass="dnnForm">
    <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="ArticleEditor"   ShowSummary="True"  runat="server" />

<script type="text/javascript">
    jQuery(function () {
        var index = $.jqx.cookie.cookie("jqxTabs_EditArticle<%=ModuleId %>");
        if (undefined == index) index = 0;

        $('#tabs').jqxTabs({ width: '100%', position: 'top', selectedItem: index });

        $('#tabs').bind('selected', function (event) {
            $.jqx.cookie.cookie("jqxTabs_EditArticle<%=ModuleId %>", event.args.item);

        });


    });
</script>

    <style>
        .jqx-tabs-content-element{
            overflow: hidden;
        }
    </style>

<div style="width: 100%">
<div id="tabs">
<ul style='margin-left: 20px;'>
    <li runat="server" id="tabli0">1</li>
    <li runat="server" id="tabli1">2</li>
    <li runat="server" id="tabli2">3</li>
    <li runat="server" id="tabli3">4</li>
    <li runat="server" id="tabli4">5</li>
    <li runat="server" id="tabli5">6</li>
   
</ul>

   <div runat="server" id="divpgBasic">
    <div id="divBasicInfo" class="dnnForm" style="padding-top:20px" >
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
           <asp:PlaceHolder runat="server" ID="plauthor"></asp:PlaceHolder>
        </div>
        <div class="dnnFormItem" runat="server" id="dCategory">
           <dnn:Label ID="lbCategories" runat="server" ControlName="lbCategories" Suffix=":" />
            
            <asp:Panel runat="server" ID="plCategory"></asp:Panel>
        </div>
        <div class="dnnFormItem" id="divTags" runat="server">
            <dnn:Label ID="lbEnterTags" runat="server"></dnn:Label>
            <asp:CheckBoxList ID="chkTagsList" runat="server" CssClass="Normal" RepeatColumns="5">
            </asp:CheckBoxList>
            <asp:PlaceHolder runat="server" ID="pltags"></asp:PlaceHolder>
            
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plPublishDate" Suffix=":" ControlName="txtPublishDate" runat="server">
            </dnn:Label>
            <div style="width:30%;display:inline-block">
           <%-- <telerik:RadDateTimePicker ID="dtPublishDate" runat="server">
            </telerik:RadDateTimePicker>--%>
            
            <zldnn:pickdate runat="server" id="ctlPublishDate" ></zldnn:pickdate>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plExpireDate" Suffix=":" ControlName="dtExpireDate" runat="server">
            </dnn:Label>
            <div style="width:30%;display:inline-block">
           <%-- <telerik:RadDateTimePicker ID="dtExpireDate" runat="server">
            </telerik:RadDateTimePicker>--%>
            <zldnn:pickdate runat="server" id="ctlExpireDate" ></zldnn:pickdate></div>
            
            
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
                    <div style="float: left" runat="server" id="divimg">
                       
                        
                    </div>
                </div>
                <div class="dnnFormItem" runat="server" id="divURL">
                    <dnn:Label ID="lblReleatedURL" runat="server" ControlName="lblReleatedURL" Suffix=":" />
                    <div style="float: left">
                       <Portal:URL ID="ctlReleatedURL" runat="server" Width="250" ShowNone="True" ShowTabs="True"
                                    ShowUrls="True" ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True"
                                    ShowNewWindow="False" />
                       
                        <asp:Panel runat="server" ID="panelRelatedURL">
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
                    <asp:PlaceHolder runat="server" ID="plRoles"></asp:PlaceHolder>
                </div>
            </fieldset>
        </div>
        <div class="dnnFormItem  dnnClear" runat="server" id="divExtraField">
            <h2 id="H4" class="dnnFormSectionHead">
                <a href="" class="">
                    <%=LocalizeString("dshExtraFields")%></a></h2>
            <fieldset>
                <div class="dnnFormItem" id="divExFields" runat="server">
                    
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
         <div class="dnnFormItem" id="divPostToTwitter" runat="server">
            <dnn:Label ID="lbPostToTwitter" runat="server" ControlName="chkPostToTwitter" Suffix=":" />
            <asp:CheckBox ID="chkPostToTwitter" runat="server" />
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

            });
</script>            
    		
       
    </div>
      </div>

    
<div runat="server" id="divpgAttachment" style="height:500px;overflow:hidden">
    <asp:Literal ID="ltFiles" runat="server"></asp:Literal>    
 <%--   <iframe frameborder="0" name="ifrfiles" runat="server" id="ifrfiles" scrolling="auto" height="100%" width="100%" src="">
    </iframe>--%>
    
</div>
<div runat="server" id="divpgPages" style="height:700px;overflow:hidden">
     <asp:Literal ID="ltPages" runat="server"></asp:Literal> 
    <%--<iframe frameborder="0" name="ifrpagess" runat="server" id="ifrpages" scrolling="auto" height="100%" width="100%" src="">
    </iframe>--%>
    
</div>
<div runat="server" id="divpgRelatedArticles"  style="height: 700px;overflow:hidden">
    <asp:Literal ID="ltRelatedArticles" runat="server"></asp:Literal> 
   <%-- <iframe frameborder="0" name="ifrrelarticles" runat="server" id="ifrrelarticles" scrolling="auto" height="700px" width="100%" src="">
    </iframe>--%>
    
     </div>
<div runat="server" id="divpgVersion">
    
    <div id="divVersions" class="dnnClear" runat="server">
       
    </div>
     </div>
<div runat="server" id="divpgLightbox">
    
    <div id="divLightbox" class="dnnClear" runat="server"  style="height: 700px;overflow:hidden">
        <%--<iframe frameborder="0" name="ifrlightbox" runat="server" id="ifrlightbox" scrolling="auto" height="700px" width="100%" src="">
        </iframe>--%>
         <asp:Literal ID="ltlightbox" runat="server"></asp:Literal> 
        <asp:PlaceHolder ID="phlightbox" runat="server"></asp:PlaceHolder>
    </div>
        </div>


</div>

</div>
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
