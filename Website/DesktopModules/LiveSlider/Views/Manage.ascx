<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Manage.ascx.cs" EnableViewState="false"
    Inherits="Mandeeps.DNN.Modules.LiveSlider.Views.Manage" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UrlControl" Src="~/controls/UrlControl.ascx" %>
<div class="ls-admin">
<div class="preview-control" title="You can preview your slider in real-time. Use the icons to bring the preview in view, start and stop the preview, and dock it at top or bottom of the window.">
    <div class="sl-slider-preview">
        <i class="ls-position fa fa-toggle-up" onclick="position($(this)); return false;" title="Dock Top / Bottom" ></i>
        <i class="fa fa-film"></i>
        <h4>
            <asp:Label ID="lPreview" runat="server" ResourceKey="lPreview" Text="Preview" />
            <div class="ls-slider" style="display: none;">
            </div>&nbsp;&nbsp;
            <span class="ls-slider-val" style="display: none;">100%</span>
        </h4>
        <i class="ls-display fa fa-plus-square" onclick="display($(this)); return false;" title="Show / Hide" ></i>
        <i class="ls-preview fa fa-play-circle-o" style="display: none;" onclick="preview($(this)); return false;" title="Start / Stop"></i>
    </div>
    <div class="PreviewContainer" style="display: none;">
    </div>
        </div>
    <div id="mainTabs">
        <ul>
            <li><a href="#tabSlides"><i class="fa fa-files-o"></i>&nbsp;&nbsp;<asp:Label runat="server"
                resourcekey="lSlides" Text="Slides"></asp:Label></a></li>
            <li><a href="#tabSettings"><i class="fa fa-cogs"></i>&nbsp;&nbsp;<asp:Label runat="server"
                resourcekey="lSliderSettings" Text="Slider Settings"></asp:Label></a></li>
        </ul>
        <div id="tabSlides">
            <asp:PlaceHolder ID="phSlides" runat="server">
                <asp:LinkButton ID="bAddSlide" runat="server" CssClass="mbutton-add" Text="Add Slide"
                    OnClientClick="Save();" OnClick="bAddSlide_Click"></asp:LinkButton>
                <div style="clear: both;">
                </div>
                <asp:HiddenField ID="hiddenSlideTab" runat="server" Value="0" />
                <asp:PlaceHolder ID="phSortSlides" runat="server"></asp:PlaceHolder>
            </asp:PlaceHolder>
        </div>
        <div id="tabSettings">
            <ul>
                <li><a href="#layout"><i class="fa fa-arrows-alt"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lLayout" Text="Layout"></asp:Label></a></li>
                <li><a href="#slideshow"><i class="fa fa-film"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lSlideshow" Text="Slideshow"></asp:Label></a></li>
                <li><a href="#appearance"><i class="fa fa-pencil"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lAppearance" Text="Appearance"></asp:Label></a></li>
                <li><a href="#navigationarea"><i class="fa fa-caret-left"></i>|<i class="fa fa-caret-right"></i>&nbsp;&nbsp;<asp:Label
                    runat="server" resourcekey="lNavigationArea" Text="Navigation Area"></asp:Label></a></li>
                <li><a href="#thumbnailnav"><i class="fa fa-th"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lThumbnailNavigation" Text="Thumbnail Navigation"></asp:Label></a></li>
                <li><a href="#ssvideos"><i class="fa fa-youtube-play"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lVideos" Text="Videos"></asp:Label></a></li>
                <li><a href="#imagepreload"><i class="fa fa-th-large"></i>&nbsp;&nbsp;<asp:Label
                    runat="server" resourcekey="lImagePreload" Text="ImagePreload"></asp:Label></a></li>
                <li><a href="#yourlogo"><i class="fa fa-picture-o"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lYourLogo" Text="Your Logo"></asp:Label></a></li>
                <li><a href="#events"><i class="fa fa-calendar"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="levents" Text="Events"></asp:Label></a></li>
                <li><a href="#other"><i class="fa fa-cog"></i>&nbsp;&nbsp;<asp:Label runat="server"
                    resourcekey="lOther" Text="Other"></asp:Label></a></li>
            </ul>
            <div id="layout">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ID="lWidth" ControlName="tbWidth" ResourceKey="lWidth"
                                    Text="Width" />
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbWidth" Width="75" data-ls="slider-Width" title="The width of the slider in pixels." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lHeight" ControlName="tbHeight" ResourceKey="lHeight"
                                    Text="Height" />
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbHeight" Width="75" data-ls="slider-Height" title="The height of the slider in pixels." />
                            </td>
                        </tr>
                         <tr>
                            <td>
                                <asp:Label runat="server" ID="lFullwidth" ControlName="cbFullwidth" ResourceKey="lFullWidth" />
                            </td>
                            <td>
                                <asp:CheckBox data-ls="slider-fullwidth" class="normalCheckBox ClsFullWidth" runat="server"
                                    ID="cbFullwidth" title="Enable this option to force the slider to become full-width, even if your theme does not support such layout." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lResponsive" ControlName="cbResponsive" ResourceKey="lResponsive"
                                    Text="Responsive" />
                            </td>
                            <td>
                                <asp:CheckBox data-ls="slider-responsive" CssClass="nomjscss normalCheckBox enableResponsive" runat="server"
                                    ID="cbResponsive" title="Responsive mode provides optimal viewing experience across a wide range of devices (from desktop to mobile) by adapting and scaling your sliders for the viewing environment." />
                            </td>
                        </tr>                       
                        <tr class="trHide">
                            <td>
                                <asp:Label runat="server" ID="lResponsiveUnder" ControlName="tbResponsiveUnder" ResourceKey="lResponsiveUnder"
                                    Text="Responsive Under" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-responsiveUnder" Width="60" runat="server" ID="tbResponsiveUnder"
                                    title="Turns on responsive mode in a full-width slider under the specified value in pixels. Can only be used with full-width mode." />
                            </td>
                        </tr>
                        <tr class="trHide">
                            <td>
                                <asp:Label runat="server" ControlName="tbLayersContainer" ID="lLayersContainer" ResourceKey="lLayersContainer"
                                    Text="Layers Container" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-layersContainer" Width="60" runat="server" ID="tbLayersContainer"
                                    title="Creates an invisible inner container with the given dimension in pixels to hold and center your layers." />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="slideshow">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="cbAutoStart" ResourceKey="AutoStart" Text="Start slideshow" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbAutoStart" data-ls="slider-autoStart"
                                    title="Slideshow will automatically start after pages have loaded." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbPauseOnHover" ResourceKey="PauseOnHover"
                                    Text="Pause On Hover" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbPauseOnHover" data-ls="slider-pauseOnHover"
                                    title="Slideshow will temporally pause when someone moves the mouse cursor over the slider." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbAnimateFirstSlide" ResourceKey="AnimateFirstSlide"
                                    Text="Animate First Slide" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbAnimateFirstSlide" data-ls="slider-animateFirstSlide"
                                    title="The slider will start with the specified slide. You can use the value 'random'." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbKeyboardNav" ResourceKey="KeyboardNav" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbKeyboardNav" data-ls="slider-keybNav"
                                    title="You can navigate through slides with the left and right arrow keys." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbTouchNav" ResourceKey="TouchNav" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbTouchNav" data-ls="slider-touchNav"
                                    title="Gesture-based navigation with swiping on touch-enabled devices." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbLoops" ResourceKey="Loops" Text="Loops" />
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbLoops" Width="60" data-ls="slider-loops" title="Number of loops if automatically start slideshow is enabled (0 means infinite!)" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbTwoWaySlideshow" ResourceKey="TwoWaySlideshow"
                                    Text="Two Way Slideshow" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbTwoWaySlideshow" data-ls="slider-towWaySlideshow"
                                    title="Slideshow can go backwards if someone switch to a previous slide." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbRandomSlideshow" ResourceKey="RandomSlideshow"
                                    Text="Random Slideshow" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbRandomSlideshow" data-ls="slider-randomSlideshow"
                                    title="Slideshow will proceed in random order. This feature does not work with looping." />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="appearance">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="ddlTheme" ResourceKey="Theme" Text="Theme" />
                            </td>
                            <td>
                                <asp:DropDownList data-ls="slider-skin" ID="ddlThemes" Style="width: 175px;" runat="server"
                                    title="You can change the skin of the slider. The 'noskin' skin is a border- and buttonless skin. Your custom skins will appear in the list when you create their folders as well.">
                                </asp:DropDownList>
                                <div data-ls="slider-skinsPath">
                                    <asp:HiddenField ID="hfskinsPath" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbGlobalBackgroundColor" ResourceKey="GlobalBackgroundColor"
                                    Text="Global Background Color" />
                            </td>
                            <td>
                                <input id="cpGlobalBackgroundColor" runat="server" type="text" class="m-color-picker"
                                    data-ls="slider-globalBGColor" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbGlobalBackgroundImage" ResourceKey="GlobalBackgroundImage"
                                    Text="Global Background Image" />
                            </td>
                            <td>
                                <asp:Image runat="server" ID="imgGlobalBackgroundImage" CssClass="ls-imgpicker ls-picker-response" BorderWidth="1" />
                                <div class="ls-inline-hidden-image" data-ls="slider-globalBGImage">
                                    <asp:HiddenField ID="hfGlobalBackgroundImage" runat="server" />
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="navigationarea">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td colspan="2">
                                <strong>Show navigation buttons</strong>
                            </td>
                        </tr>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="cbNavPrevNext" ResourceKey="NavPrevNext" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbNavPrevNext" data-ls="slider-navPrevNext"
                                    title="Disabling this option will hide the Prev and Next buttons." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbNavStartStop" ResourceKey="NavStartStop" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbNavStartStop" data-ls="slider-navStartStop"
                                    title="Disabling this option will hide the Start & Stop buttons." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbNavButtons" ResourceKey="NavButtons" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbNavButtons" data-ls="slider-navButtons"
                                    title="Disabling this option will hide slide navigation buttons or thumbnails." />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <strong>Navigation buttons on hover</strong>
                            </td>
                        </tr>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="cbHoverPrevNext" ResourceKey="HoverPrevNext" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbHoverPrevNext" data-ls="slider-hoverPrevNext"
                                    title="Show the buttons only when someone moves the mouse cursor over the slider. This option depends on the previous setting." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbHoverBottomNav" ResourceKey="HoverBottomNav" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbHoverBottomNav" data-ls="slider-hoverBottomNav"
                                    title="Slide navigation buttons (including thumbnails) will be shown on mouse hover only." />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <strong>Slideshow timers</strong>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbBarTimer" ResourceKey="BarTimer" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbBarTimer" data-ls="slider-showBarTimer"
                                    title="Show the bar timer to indicate slideshow progression." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbCircleTimer" ResourceKey="CircleTimer" Text="Circle Timer" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" CssClass="normalCheckBox" ID="cbCircleTimer" data-ls="slider-showCircleTimer"
                                    title="Use circle timer to indicate slideshow progression." />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="thumbnailnav">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="ddlThumbnailNavigation" ResourceKey="ThumbnailNavigation"
                                    Text="Thumbnail Navigation" />
                            </td>
                            <td>
                                <asp:DropDownList data-ls="slider-thumbnailNavigation" ID="ddlThumbnailNavigation"
                                    runat="server" Style="width: 175px;">
                                    <asp:ListItem Value="'disabled'">Disabled</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="'hover'">Show On Hover Only</asp:ListItem>
                                    <asp:ListItem Value="'always'">Show Always</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbThumbnailContainerWidth" ResourceKey="ThumbnailContainerWidth"
                                    Text="Thumbnail Container Width" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-tnContainerWidth" Width="60" runat="server" ID="tbThumbnailContainerWidth"
                                    title="The width of the thumbnail area in pixels." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbThumbnailWidth" ResourceKey="ThumbnailWidth"
                                    Text="Thumbnail Width" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-tnWidth" Width="60" runat="server" ID="tbThumbnailWidth"
                                    title="The width of thumbnails in the navigation area in pixels." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbThumbnailHeight" ResourceKey="ThumbnailHeight"
                                    Text="Thumbnail Height" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-tnHeight" Width="60" runat="server" ID="tbThumbnailHeight"
                                    title="The height of thumbnails in the navigation area in pixels." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbThumbnailActiveOpacity" ResourceKey="ThumbnailActiveOpacity"
                                    Text="Thumbnail Active Opacity" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-tnActiveOpacity" Width="60" runat="server" ID="tbThumbnailActiveOpacity"
                                    title="Opacity in percents of the active slide's tumbnail in pixels." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbThumbnailInactiveOpacity" ResourceKey="ThumbnailInactiveOpacity"
                                    Text="Thumbnail Inactive Opacity" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-tnInactiveOpacity" Width="60" runat="server" ID="tbThumbnailInactiveOpacity"
                                    title="Opacity in percents of inactive slide thumbnails in pixels." />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="ssvideos">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="cbAutoPlayVideos" ResourceKey="AutoPlayVideos"
                                    Text="Auto Play Videos" />
                            </td>
                            <td>
                                <asp:CheckBox CssClass="normalCheckBox" data-ls="slider-autoPlayVideos" runat="server"
                                    ID="cbAutoPlayVideos" title="Videos will be automatically started on the active slide." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="ddlAutoPauseSlideshow" ResourceKey="AutoPauseSlideshow"
                                    Text="Automatically Pause Slideshow" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlAutoPauseSlideshow" Style="width: 175px;" runat="server"
                                    data-ls="slider-autoPauseSlideshow" title="The slideshow can temporally paused while videos are plaing. You can choose to permanently stop the pause until manual restarting.">
                                    <asp:ListItem Value="'auto'">Pause While Video is Playing</asp:ListItem>
                                    <asp:ListItem Value="true">Stop Slideshow if Video is Playing</asp:ListItem>
                                    <asp:ListItem Value="false">Require Manual Restart</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="imagepreload">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="cbImagePreload" ResourceKey="ImagePreload"
                                    Text="Preload Images" />
                            </td>
                            <td>
                                <asp:CheckBox CssClass="normalCheckBox" data-ls="slider-imgPreload" runat="server"
                                    ID="cbImagePreload" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbLazyLoad" ResourceKey="LazyLoad" Text="Lazy Load" />
                            </td>
                            <td>
                                <asp:CheckBox CssClass="normalCheckBox" runat="server" ID="cbLazyLoad" data-ls="slider-lazyLoad" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="yourlogo">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ControlName="tbYourLogo" ResourceKey="lYourLogo" Text="Your Logo" />
                            </td>
                            <td>
                                <asp:Image ID="imgYourLogo" runat="server" CssClass="ls-imgpicker ls-picker-response" BorderWidth="1" />
                                <div class="ls-inline-hidden-image" data-ls="slider-yourLogo">
                                    <asp:HiddenField ID="hfYourLogo" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbYourLogoStyle" ResourceKey="YourLogoStyle"
                                    Text="CSS Properties" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoStyle" Width="400" runat="server" ID="tbYourLogoStyle"
                                    Rows="3" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbYourLogoLink" ResourceKey="YourLogoLink"
                                    Text="Link" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoLink" Width="400" runat="server" ID="tbYourLogoLink" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="cbYourLogoLinkNewWindow" ResourceKey="YourLogoLinkNewWindow"
                                    Text="Open In New Window" />
                            </td>
                            <td>
                                <asp:CheckBox CssClass="normalCheckBox" data-ls="slider-yourLogoTarget" runat="server"
                                    ID="cbYourLogoLinkNewWindow" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
             <div id="events">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbInit" ResourceKey="CbInit"
                                    Text="CbInit" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoStyle" Width="400" runat="server" ID="tbCbInit"
                                    Rows="3" TextMode="MultiLine" title="Fires when LayerSlider has loaded." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbStart" ResourceKey="CbStart"
                                    Text="CbStart" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoLink" Width="400" runat="server" ID="tbCbStart"
                                    Rows="3" TextMode="MultiLine" title="Calling when the slideshow has started."  />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbStop" ResourceKey="CbStop"
                                    Text="CbStop" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoStyle" Width="400" runat="server" ID="tbCbStop"
                                    Rows="3" TextMode="MultiLine" title="Calling when the slideshow is stopped by the user."/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbPause" ResourceKey="CbPause"
                                    Text="CbPause" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoLink" Width="400" runat="server" ID="tbCbPause" 
                                     Rows="3" TextMode="MultiLine" title="Firing when the slideshow is temporary on hold (e.g.: 'Pause on hover' feature)"/>
                            </td>
                        </tr>
                      <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbAnimStart" ResourceKey="CbAnimStart"
                                    Text="CbAnimStart" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoStyle" Width="400" runat="server" ID="tbCbAnimStart"
                                    Rows="3" TextMode="MultiLine" title="Calling when the slider commencing slide change (animation start)." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbAnimStop" ResourceKey="CbAnimStop"
                                    Text="CbAnimStop" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoLink" Width="400" runat="server" ID="tbCbAnimStop" 
                                     Rows="3" TextMode="MultiLine" title="Firing when the slider finished a slide change (animation end)."/>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbPrev" ResourceKey="CbPrev"
                                    Text="CbPrev" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoStyle" Width="400" runat="server" ID="tbCbPrev"
                                    Rows="3" TextMode="MultiLine" title="Calling when the slider will change to the previous slide by the user."/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ControlName="tbCbNext" ResourceKey="CbNext"
                                    Text="CbNext" />
                            </td>
                            <td>
                                <asp:TextBox data-ls="slider-yourLogoLink" Width="400" runat="server" ID="tbCbNext" 
                                     Rows="3" TextMode="MultiLine" title="Calling when the slider will change to the next slide by the user."/>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="other">
                <table width="100%" cellpadding="5" cellspacing="5">
                    <tbody>
                        <tr>
                            <td width="250">
                                <asp:Label runat="server" ID="lDnnSearchable" ResourceKey="lDnnSearchable" />
                            </td>
                            <td>
                                <asp:CheckBox CssClass="normalCheckBox" runat="server" ID="cbDnnSearchable" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <br />
    <asp:LinkButton ID="bUpdate" OnClientClick="Save();" runat="server" CssClass="mbutton"
        ResourceKey="Save" Text="Update" OnClick="bUpdate_Click" />
    <asp:LinkButton ID="bCancel" runat="server" CssClass="mbutton2" ResourceKey="Cancel"
        Text="Cancel" OnClick="bCancel_Click" />
</div>
