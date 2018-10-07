<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HyperLinkValueOnly.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.HyperLinkValueOnly" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<div style="float:left">
<Portal:URL ID="ctlReleatedURL" runat="server" Width="250" ShowNone="True" ShowTabs="True"
    ShowUrls="True" ShowTrack="False" ShowLog="False" Required="false" ShowUpLoad="True"
    ShowNewWindow="False" />
    </div>
    <div style="clear:both"></div>
