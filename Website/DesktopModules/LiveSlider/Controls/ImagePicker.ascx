<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImagePicker.ascx.cs"
    Inherits="Mandeeps.DNN.Modules.LiveSlider.Controls.ImagePicker" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/UrlControl.ascx" %>
<style>
    .urlControlFileRow
    {
        margin: 15px 20px 20px;
    }
    .urlControlFileRow span.dnnFormLabel
    {
        width: 100px;
    }
    .urlControlFileRow .dnnFormItem
    {
        margin-bottom: 15px;
    }
</style>
<dnn:UrlControl id="urlImagePicker" runat="server" ShowLog="false" ShowTrack="false"
    ShowNewWindow="false" UrlType="F" ShowUrls="false" ShowTabs="false" FileFilter="jpg,jpeg,png,gif">
</dnn:UrlControl>
<div id="ImagePicker">
</div>

<a class="mbutton" style="margin-left: 20px;" onclick="window.parent.ipSelectFile($.QueryStringPath('controlid'),$('.thumbnail.selected img').attr('src')); ">
    Select File</a>. <a class="mbutton2" onclick="window.parent.ipCloseDialog(); ">Cancel</a>.
<a href="#" class="mbutton" style="position: absolute; right: 15px; top: 0;" id="bNoImage"
    runat="server">No Image</a>