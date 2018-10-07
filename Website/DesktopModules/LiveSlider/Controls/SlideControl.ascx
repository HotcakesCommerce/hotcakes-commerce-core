<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SlideControl.ascx.cs"
    Inherits="Mandeeps.DNN.Modules.LiveSlider.Controls.SlideControl" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/UrlControl.ascx" %>
<%@ Register Assembly="Mandeeps.DNN.Modules.LiveSlider" Namespace="Mandeeps.DNN.Modules.LiveSlider.Controls"
    TagPrefix="Mandeeps" %>
<asp:HiddenField ID="hiddenLayerTab" runat="server" />
<div style="clear: both;">
</div>
<h4 class="mhead">
    <i class="fa fa-edit"></i>&nbsp;&nbsp;<asp:Label runat="server" ID="lAppearance"
        Text="Appearance"></asp:Label>
    <asp:LinkButton ID="bDeleteSlide" Style="margin-right: 0;" OnClientClick="return confirm('Are you sure you want to delete this slider?');"
        runat="server" CssClass="ls-icon push-right"><i class="fa fa-trash-o" title="Delete Slide"></i></asp:LinkButton>
    <asp:LinkButton ID="bCopySlide" runat="server" CssClass="ls-icon push-right"><i class="fa fa-files-o" title="Copy Slide"></i></asp:LinkButton>
</h4>
<table class="appearance-demo" width="100%" cellpadding="5" cellspacing="5">
    <tr>
        <td width="155" style="text-align: center; vertical-align: top;">
            <asp:Image ID="ImgBackgroundImage" runat="server" data-ls="slide-BGImage" CssClass="ls-imgpicker ls-picker-response"
                BorderWidth="1" Style="margin: 0 auto;" title="Slide transitions will work only if a slide background image is added. Click on the image to choose or upload an image." />
            <div class="ls-inline-hidden-image" data-ls="slide-BGImage">
                <asp:HiddenField ID="hfImgBackgroundImage" runat="server" />
            </div>
            <asp:HyperLink ID="labelBackgroundImageAltText" class="ls-textcontrol ls-picker-response" data-type="text"
                runat="server" data-ls="slide-BGImageAltText" title="Click on the text to add slide background image alt text."> </asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hiddenBackgroundImageAltText" runat="server" />
            </div>
        </td>
        <td width="155" style="text-align: center; vertical-align: top;">
            <asp:Image ID="imgThumbnail" runat="server" data-ls="slide-Thumbnail" CssClass="ls-imgpicker ls-picker-response"
                BorderWidth="1" Style="margin: 0 auto;" title="The thumbnail image of this slide. Click on the image to choose or upload an image. If you leave this field empty, the slide image will be used." />
            <div class="ls-inline-hidden-image" data-ls="slide-Thumbnail">
                <asp:HiddenField ID="hfimgThumbnail" runat="server" />
            </div>
            <asp:HyperLink ID="hlThumbnailAltText" class="ls-textcontrol ls-picker-response" data-type="text" runat="server"
                data-ls="slide-ThumbnailAltText" title="Click on the text to add slide thumbnail image alt text."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfThumbnailAltText" runat="server" />
            </div>
        </td>
        <td width="250" style="vertical-align: top;">
            <asp:Label runat="server" ID="lbLink" Font-Bold="true" resourcekey="SlideLink"></asp:Label>
            <asp:HyperLink ID="hlLink" class="hllink ls-textcontrol ls-picker-response" data-type="text" runat="server"
                data-ls="slide-link" title="If you want to link the whole slide, enter the URL of your link here."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfLink" runat="server" />
            </div>
            <br />
            <asp:Label runat="server" ID="lbOpenInNewWindow" Font-Bold="true" resourcekey="SlideLinkNewWindow"></asp:Label>
            <asp:CheckBox runat="server" ID="cbOpenInNewWindow" data-ls="slide-3dtransition"
                CssClass="normalCheckBox" title="If you want to open link in new window, you can check this option." />
            <div class="ls-inline-hidden">
            </div>
            <br />
            <asp:Label runat="server" ID="lHiddenSlide" Font-Bold="true" resourcekey="lHiddenSlide"></asp:Label>
            <asp:CheckBox runat="server" ID="cbHiddenSlide" data-ls="slide-hidden"
                CssClass="normalCheckBox" title="visibility setting of the slide" />
        </td>
        <td width="180" style="vertical-align: top;">
            <asp:Label runat="server" ID="lbTransition2D" Font-Bold="true" resourcekey="Slide2DTransition"></asp:Label>
            <asp:HyperLink ID="hlTransition2D" class="ls-transpicker ls-picker-response" data-ls="slide-transition"
                data-type="text" runat="server" title="You can select your desired slide 2D transitions by clicking on this button."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfTransition2D" runat="server" />
            </div>
            <br />
            <asp:Label runat="server" ID="lbTransition3D" Font-Bold="true" resourcekey="Slide3DTransition"></asp:Label>
            <asp:HyperLink ID="hlTransition3D" class="ls-transpicker ls-picker-response" data-ls="slide-3dtransition"
                data-type="text" runat="server" title="You can select your desired slide 3D transitions by clicking on this button."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfTransition3D" runat="server" />
            </div>
            <br />
            <asp:Label runat="server" ID="lbVisiblity" Font-Bold="true" ResourceKey="lVisiblity"/>
            <asp:DropDownList ID="ddlShowOn" RepeatDirection="Horizontal" runat="server"
                                    data-ls="slider-Visibilty" title="Slide is visible in Mobile/Desktop or both">
                                    <asp:ListItem Value="mobile">Mobile Only</asp:ListItem>
                                    <asp:ListItem Value="desktop">Desktop Only</asp:ListItem>
                                    <asp:ListItem Value="both">All Devices</asp:ListItem>
                
                                </asp:DropDownList>
        </td>
        <td style="vertical-align: top; width: 150px;">
            <asp:Label runat="server" ID="tbDelay" Font-Bold="true" resourcekey="SlideDelay"></asp:Label>
            <asp:HyperLink ID="hlDelay" class="ls-textcontrol ls-picker-response" data-ls="slide-delay" data-type="text"
                runat="server" title="Here you can set the time interval between slide changes, this slide will stay visible for the time specified here. This value is in millisecs, so the value 1000 means 1 second. Please don't use 0 or very low values."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfDelay" runat="server" />
            </div>
            <br />
            <asp:Label runat="server" ID="tbTimeShift" Font-Bold="true" ResourceKey="SlideTimeShift"></asp:Label>
            <asp:HyperLink ID="hlTimeShift" class="ls-textcontrol ls-picker-response" data-ls="slide-timeshift" data-type="text"
                runat="server" title="You can control here the timing of the layer animations when the slider changes to this slide with a 3D/2D transition. Zero means that the layers of this slide will animate in when the slide transition ends. You can time-shift the starting time of the layer animations with positive or negative values."></asp:HyperLink>
            <div class="ls-inline-hidden">
                <asp:HiddenField ID="hfTimeShift" runat="server" />
            </div>
            <div class="slideSortOrder">
                <asp:HiddenField ID="hiddenSortOrder" runat="server" />
            </div>
        </td>
    </tr>
</table>
<div style="clear: both;">
</div>
<h4 class="mhead layer">
    <i class="fa fa-sliders"></i>&nbsp;&nbsp;<asp:Label runat="server" ID="lLayers" Text="Layers"></asp:Label>
    <asp:LinkButton ID="bAddLayer" OnClientClick="Save();" runat="server" CssClass="mbutton-add" Text="Add Layer"
        OnClick="bAddLayer_Click"></asp:LinkButton>
</h4>
<ul class="sortLayers">
    <asp:PlaceHolder ID="phSlideLayers" runat="server"></asp:PlaceHolder>
</ul>
<h4 class="mhead layer">
    <i class="fa fa-users"></i>&nbsp;&nbsp;<asp:Label runat="server" ID="lPermissions" Text="Permissions"></asp:Label>
</h4>
<br />
<table cellpadding="5" cellspacing="5">
    <td width="250" style="text-align: right; font-weight: bold; vertical-align: top;" >
        <asp:Label runat="server" ID="lEnablePermissions" ResourceKey="EnablePermissions"></asp:Label>
    </td>
    <td>
        <asp:CheckBox ID="cbEnablePermissions" CssClass="cb-enable-permissions" runat="server" style="margin-left: 9px;" />
        <br />
        <br />
        <asp:CheckBoxList ID="cbPermissions" CssClass="cb-permissions" runat="server">
        </asp:CheckBoxList>
    </td>
</table>
<div class="hidden-field-permissions">
    <asp:HiddenField ID="hfPermissions" runat="server" />
</div>
