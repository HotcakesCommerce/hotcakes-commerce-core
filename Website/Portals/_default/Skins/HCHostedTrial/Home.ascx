<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGO" Src="~/Admin/Skins/Logo.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SEARCH" Src="~/Admin/Skins/Search.ascx" %>
<%@ Register TagPrefix="dnn" TagName="BREADCRUMB" Src="~/Admin/Skins/BreadCrumb.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USER" Src="~/Admin/Skins/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGIN" Src="~/Admin/Skins/Login.ascx" %>
<%@ Register TagPrefix="dnn" TagName="PRIVACY" Src="~/Admin/Skins/Privacy.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TERMS" Src="~/Admin/Skins/Terms.ascx" %>
<%@ Register TagPrefix="dnn" TagName="COPYRIGHT" Src="~/Admin/Skins/Copyright.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LINKTOMOBILE" Src="~/Admin/Skins/LinkToMobileSite.ascx" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="MENU" Src="~/DesktopModules/DDRMenu/Menu.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

<dnn:DnnJsInclude ID="bootstrapJS" runat="server" FilePath="bootstrap/js/bootstrap.min.js" PathNameAlias="SkinPath" AddTag="false" />
<dnn:DnnJsInclude ID="customJS" runat="server" FilePath="js/scripts.js" PathNameAlias="SkinPath" AddTag="false" />

<div id="siteWrapper">
    <div id="userControls" class="container">
        <div class="row-fluid">
            <div class="span2 language pull-left">
                <dnn:LANGUAGE runat="server" ID="LANGUAGE1" ShowMenu="False" ShowLinks="True" />
            </div>
            <div id="search" class="span3 pull-right">
                <dnn:SEARCH ID="dnnSearch" runat="server" ShowSite="false" ShowWeb="false" EnableTheming="true" Submit="Search" CssClass="SearchButton" />
            </div>
            <div id="login" class="span5 pull-right">
                <dnn:LOGIN ID="dnnLogin" CssClass="LoginLink" runat="server" LegacyMode="false" />
                <dnn:USER ID="dnnUser" runat="server" LegacyMode="false" />
            </div>
        </div>
    </div>
    <div id="siteHeadouter">
        <div id="siteHeadinner" class="container">
            <div class="navbar">
                <div class="navbar-inner">
                    <span class="brand visible-desktop">
                        <dnn:LOGO runat="server" ID="dnnLOGO" />
                    </span>
                    <span class="brand hidden-desktop">
                        <dnn:LOGO runat="server" ID="dnnLOGOmobi" />
                    </span>
                    <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">Menu</a>
                    <div id="navdttg" class="nav-collapse collapse pull-right">
                        <dnn:MENU ID="bootstrapNav" MenuStyle="bootstrapNav" runat="server"></dnn:MENU>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="contentWrapper">
        <div class="container">
            <div class="row-fluid">
		        <div id="ContentPane" class="span12 contentPane" runat="server"></div>
            </div>
        </div>
        <div class="row-fluid">
            <div id="LeftPane" class="span6 leftPane" runat="server"></div>
            <div id="RightPane" class="span6 rightPane" runat="server"></div>
        </div>
        <div id="FullPane" class="row-fluid fullPane" runat="server"></div>
            <div class="container">
                <div id="BottomPane" class="span12 bottomPane" runat="server"></div>
              </div>  
        <div class="footerWrapper">
            <div class="container">
                    <div class="row-fluid">
                        <div class="span3">
                            <ul class="socialLinks">
                                <li class="facebook"><a target="_blank" href="https://www.facebook.com/HotcakesCommerce">Like us on Facebook</a></li>
                                <li class="twitter"><a target="_blank" href="https://twitter.com/mmmCommerce">Follow us on Twitter</a></li>
                                <li class="youtube"><a target="_blank" href="https://www.youtube.com/user/HotcakesCommerce">Subscribe to us on Youtube</a></li>
                                <li class="linkedin"><a target="_blank" href="http://www.linkedin.com/company/hotcakes-commerce">Find us on LinkedIn</a></li>
                            </ul>
                        </div>
				        <div class="span5 pull-right copyrightLinks">
				            <dnn:COPYRIGHT ID="dnnCopyright" runat="server" CssClass="pull-left" />
                            <a href="http://hotcakes.org/" target="_blank">About Us</a>
					        <dnn:TERMS ID="dnnTerms" runat="server" />
					        <dnn:PRIVACY ID="dnnPrivacy" runat="server" />
				        </div>
                    </div>
                </div>
            </div>
        </div>
</div>
<img src="//hotcakes.org/Portals/0/images/spacer.gif" alt="" />
</div>
<dnn:DnnJsInclude ID="dttg" runat="server" FilePath="js/doubletaptogo.min.js" PathNameAlias="SkinPath" AddTag="false" />
<script type="text/javascript">
    $(function () {
        $('#navdttg li:has(ul)').doubleTapToGo();
    });
</script>

