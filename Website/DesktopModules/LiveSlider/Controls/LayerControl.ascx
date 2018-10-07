<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LayerControl.ascx.cs"
    Inherits="Mandeeps.DNN.Modules.LiveSlider.Controls.LayerControl" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<li id="layer" class="ls-picker-response" runat="server">
    <div class="hidden_layer">
        <asp:HiddenField ID="hiddenLayer" runat="server" />
    </div>
    <div class="layerTabs">
        <ul>
            <a data-ls="layer-name" class="ls-textcontrol layer-name"></a>
            <asp:LinkButton ID="bDeleteLayer" runat="server" OnClientClick="Save(); return confirm('Are you sure you want to delete this Layer?');"
                CssClass="ls-icon push-left float-right"><i class="fa fa-trash-o" title="Delete Layer"></i></asp:LinkButton>
            <asp:LinkButton ID="bCopyLayer" runat="server" OnClientClick="Save();" CssClass="ls-icon push-left float-right"><i class="fa fa-files-o" title="Copy Layer"></i></asp:LinkButton>
            <a style="cursor: move;" class="ls-icon push-left float-right handle"><i class="fa fa-arrows-alt"
                title="Sort Layer"></i></a>
            <li><a href="#LayerStyle"><span>Style</span></a></li>
            <li><a href="#LayerLink"><span>Link</span></a></li>
            <li><a href="#LayerOptions"><span>Options</span> </a></li>
            <li><a href="#LayerContent"><span>Basic</span></a></li>
        </ul>
        <div id="LayerContent">
            <table cellpadding="5" cellspacing="5">
                <tr>
                    <td width="115">
                        <span>Content Type</span>
                    </td>
                    <td width="158" style="text-align: left">
                        <select data-ls="layer-htmlelement" style="width: 158px;">
                            <option value="img">Image</option>
                            <option value="div">Video / HTML</option>
                            <option value="p">Paragraph</option>
                            <option value="span">Span</option>
                            <option value="h1">H1</option>
                            <option value="h2">H2</option>
                            <option value="h3">H3</option>
                            <option value="h4">H4</option>
                            <option value="h5">H5</option>
                            <option value="h6">H6</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td width="160px">
                        <span>Content Source</span>
                    </td>
                    <td style="text-align: center;">
                        <textarea rows="5" cols="50" data-ls="layer-source"></textarea>
                        <img class="layer-imgpicker" data-ls="layer-source" />
                        <div class="ls-inline-hidden-image" data-ls="slide-BGImage">
                            <asp:HiddenField ID="hfImgBackgroundImage" runat="server" />
                        </div>
                        <asp:HyperLink ID="labelLayerAltText" class="ls-textcontrol ls-picker-response" data-type="text"
                            runat="server" data-ls="layer-AltText" title=""> </asp:HyperLink>
                        <div class="ls-inline-hidden">
                            <asp:HiddenField ID="hiddenLayerAltText" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
            <input data-ls="layer-sortorder" style="display: none;" type="text" />
        </div>
        <div id="LayerOptions">
            <table width="100%" cellpadding="5" cellspacing="5">
                <tr>
                    <td rowspan="3" width="120" style="font-weight: bold">
                        <span>Transition In</span>
                    </td>
                    <td>
                        <span>OffsetX</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-offsetxin" title="The horizontal offset to align the starting position of layers. Positive and negative numbers are allowed or enter left / right to position the layer out of the frame." />
                    </td>
                    <td>
                        <span>OffsetY</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-offsetyin" title="The vertical offset to align the starting position of layers. Positive and negative numbers are allowed or enter top / bottom to position the layer out of the frame." />
                    </td>
                    <td>
                        <span>Duration</span>
                    </td>
                    <td width="89">
                        <input type="text" data-ls="layer-durationin" title="The transition duration in milliseconds when the layer entering into the slide. A second is equal to 1000 milliseconds." />
                        <span class="label-helper">ms</span>
                    </td>
                    <td>
                        <span>Delay</span>
                    </td>
                    <td width="89">
                        <input type="text" data-ls="layer-delayin" title="Delays the transition with the given amount of milliseconds before the layer entering into the slide. A second is equal to 1000 milliseconds." />
                        <span class="label-helper">ms</span>
                    </td>
                    <td rowspan="3" width="165" style="text-align: left;">
                        <span>Easing</span><br />
                        <select data-ls="layer-easingin" style="width: 150px; margin-top: 6px;" title="The timing function of the animation to manipualte the layer's movement. Click on the link next to this field to open easings.net for examples and more information.">
                            <option value="linear">linear</option>
                            <option value="swing">swing</option>
                            <option value="easeInQuad">easeInQuad</option>
                            <option value="easeOutQuad">easeOutQuad</option>
                            <option value="easeInOutQuad">easeInOutQuad</option>
                            <option value="easeInCubic">easeInCubic</option>
                            <option value="easeOutCubic">easeOutCubic</option>
                            <option value="easeInOutCubic">easeInOutCubic</option>
                            <option value="easeInQuart">easeInQuart</option>
                            <option value="easeOutQuart">easeOutQuart</option>
                            <option value="easeInOutQuart">easeInOutQuart</option>
                            <option value="easeInQuint">easeInQuint</option>
                            <option value="easeOutQuint">easeOutQuint</option>
                            <option value="easeInOutQuint">easeInOutQuint</option>
                            <option value="easeInSine">easeInSine</option>
                            <option value="easeOutSine">easeOutSine</option>
                            <option value="easeInOutSine">easeInOutSine</option>
                            <option value="easeInExpo">easeInExpo</option>
                            <option value="easeOutExpo">easeOutExpo</option>
                            <option value="easeInOutExpo">easeInOutExpo</option>
                            <option value="easeInCirc">easeInCirc</option>
                            <option value="easeOutCirc">easeOutCirc</option>
                            <option value="easeInOutCirc">easeInOutCirc</option>
                            <option value="easeInElastic">easeInElastic</option>
                            <option value="easeOutElastic">easeOutElastic</option>
                            <option value="easeInOutElastic">easeInOutElastic</option>
                            <option value="easeInBack">easeInBack</option>
                            <option value="easeOutBack">easeOutBack</option>
                            <option value="easeInOutBack">easeInOutBack</option>
                            <option value="easeInBounce">easeInBounce</option>
                            <option value="easeOutBounce">easeOutBounce</option>
                            <option value="easeInOutBounce">easeInOutBounce</option>
                        </select>
                        <span style="margin-top: 6px; display: block;">Transform Origin</span>
                        <input type="text" data-ls="layer-transformoriginin" style="width: 150px; margin-top: 6px;"
                            title="This option allows you to modify the origin for transformations of the layer according to its position. The three values represent the X, Y and Z axis in 3D space. OriginX can be left, center, right, a number or a percentage value. OriginY can be top, center, bottom, a number or a percentage value. OriginZ can be a number and corresponds the depth in 3D space." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Fade</span>
                    </td>
                    <td title="Fades in / out the layer during the transition.">
                        <input data-ls="layer-fadein" class="normalCheckBox" type="checkbox" />
                    </td>
                    <td>
                        <span>Rotate</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-rotatein" title="Rotates the layer clockwise from the given angle to zero degree. Negative values are allowed for anticlockwise rotation." />
                    </td>
                    <td>
                        <span>RotateX</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-rotatexin" title="Rotates the layer along the X (horizontal) axis from the given angle to zero degree. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>RotateY</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-rotateyin" title="Rotates the layer along the Y (vertical) axis from the given angle to zero degree. Negative values are allowed for reverse direction." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>SkewX</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-skewxin" title="Skews the layer along the X (horizontal) axis from the given angle to 0 degree. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>SkewY</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-skewyin" title="Skews the layer along the Y (vertical) axis from the given angle to 0 degree. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>ScaleX</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-scalexin" title="Scales the layer's width from the given value to its original size." />
                    </td>
                    <td>
                        <span>ScaleY</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-scaleyin" title="Scales the layer's height from the given value to its original size." />
                    </td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr class="separator">
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td rowspan="3" style="font-weight: bold">
                        <span>Transition Out</span>
                    </td>
                    <td>
                        <span>OffsetX</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-offsetxout" title="The horizontal offset to align the ending position of layers. Positive and negative numbers are allowed or write left / right to position the layer out of the frame." />
                    </td>
                    <td>
                        <span>OffsetY</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-offsetyout" title="The vertical offset to align the starting position of layers. Positive and negative numbers are allowed or write top / bottom to position the layer out of the frame." />
                    </td>
                    <td>
                        <span>Duration</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-durationout" title="The transition duration in milliseconds when the layer leaving the slide. A second is equal to 1000 milliseconds." />
                        <span class="label-helper">ms</span>
                    </td>
                    <td width="70">
                        <span>Show Until</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-showuntil" title="The layer will be visible for the time you specify here, then it will slide out. You can use this setting for layers to leave the slide before the slide itself animates out, or for example before other layers will slide in. This value in millisecs, so the value 1000 means 1 second." />
                        <span class="label-helper">ms</span>
                    </td>
                    <td rowspan="3" style="text-align: left;">
                        <span>Easing</span><br />
                        <select data-ls="layer-easingout" style="width: 150px; margin-top: 6px;" title="The timing function of the animation to manipualte the layer's movement. Click on the link next to this field to open easings.net for examples and more information.">
                            <option value="linear">linear</option>
                            <option value="swing">swing</option>
                            <option value="easeInQuad">easeInQuad</option>
                            <option value="easeOutQuad">easeOutQuad</option>
                            <option value="easeInOutQuad">easeInOutQuad</option>
                            <option value="easeInCubic">easeInCubic</option>
                            <option value="easeOutCubic">easeOutCubic</option>
                            <option value="easeInOutCubic">easeInOutCubic</option>
                            <option value="easeInQuart">easeInQuart</option>
                            <option value="easeOutQuart">easeOutQuart</option>
                            <option value="easeInOutQuart">easeInOutQuart</option>
                            <option value="easeInQuint">easeInQuint</option>
                            <option value="easeOutQuint">easeOutQuint</option>
                            <option value="easeInOutQuint">easeInOutQuint</option>
                            <option value="easeInSine">easeInSine</option>
                            <option value="easeOutSine">easeOutSine</option>
                            <option value="easeInOutSine">easeInOutSine</option>
                            <option value="easeInExpo">easeInExpo</option>
                            <option value="easeOutExpo">easeOutExpo</option>
                            <option value="easeInOutExpo">easeInOutExpo</option>
                            <option value="easeInCirc">easeInCirc</option>
                            <option value="easeOutCirc">easeOutCirc</option>
                            <option value="easeInOutCirc">easeInOutCirc</option>
                            <option value="easeInElastic">easeInElastic</option>
                            <option value="easeOutElastic">easeOutElastic</option>
                            <option value="easeInOutElastic">easeInOutElastic</option>
                            <option value="easeInBack">easeInBack</option>
                            <option value="easeOutBack">easeOutBack</option>
                            <option value="easeInOutBack">easeInOutBack</option>
                            <option value="easeInBounce">easeInBounce</option>
                            <option value="easeOutBounce">easeOutBounce</option>
                            <option value="easeInOutBounce">easeInOutBounce</option>
                        </select>
                        <span style="margin-top: 6px; display: block;">Transform Origin</span>
                        <input type="text" data-ls="layer-transformoriginout" style="width: 150px; margin-top: 6px;"
                            title="This option allows you to modify the origin for transformations of the layer according to its position. The three values represent the X, Y and Z axis in 3D space. OriginX can be left, center, right, a number or a percentage value. OriginY can be top, center, bottom, a number or a percentage value. OriginZ can be a number and corresponds the depth in 3D space." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Fade</span>
                    </td>
                    <td title="Fades in / out the layer during the transition.">
                        <input data-ls="layer-fadeout" class="normalCheckBox" type="checkbox" />
                    </td>
                    <td>
                        <span>Rotate</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-rotateout" title="Rotates the layer clockwise by the given angle from its original position. Negative values are allowed for anticlockwise rotation." />
                    </td>
                    <td>
                        <span>RotateX</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-rotatexout" title="Rotates the layer along the X (horizontal) axis by the given angle from its original state. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>RotateY</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-rotateyout" title="Rotates the layer along the Y (vertical) axis by the given angle from its orignal state. Negative values are allowed for reverse direction." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>SkewX</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-skewxout" title="Skews the layer along the X (horizontal) axis by the given angle from its orignal state. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>SkewY</span>
                    </td>
                    <td>
                        <input type="text" data-ls="layer-skewyout" title="Skews the layer along the Y (vertical) axis by the given angle from its original state. Negative values are allowed for reverse direction." />
                    </td>
                    <td>
                        <span>ScaleX</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-scalexout" title="Scales the layer's width by the given value from its original size." />
                    </td>
                    <td>
                        <span>ScaleY</span>
                    </td>
                    <td style="text-align: left;">
                        <input type="text" data-ls="layer-scaleyout" title="Scales the layer's height by the given value from its original size." />
                    </td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr class="separator">
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        <span>Other Options</span>
                    </td>
                    <td colspan="2">
                        <span>Parallax effect</span>
                    </td>
                    <td title="Applies a parallax effect on layers when you move your mouse over the slider. Higher values makes the layer more sensitive to mouse move. Negative values are allowed.">
                        <input type="text" data-ls="layer-parallaxlevel" />
                    </td>
                    <td>
                        <span>Hidden</span>
                    </td>
                    <td>
                        <input class="normalCheckBox" data-ls="layer-hidden" type="checkbox" title="If you don't want to use this layer, but you want to keep it, you can hide it with this switch." />
                    </td>
                    <td colspan="5">&nbsp;</td>
                </tr>
            </table>
        </div>
        <div id="LayerLink">
            <table width="100%" cellpadding="5">
                <tbody>
                    <tr>
                        <td width="200px">
                            <span>Link Url</span>
                        </td>
                        <td style="text-align: left;">
                            <input type="text" value="http://" data-ls="layer-Link" style="width: 500px" title="If you want to link your layer, type here the URL." />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Open in new window</span>
                        </td>
                        <td style="text-align: left;">
                            <input data-ls="layer-OpenInNewWindow" type="checkbox" class="normalCheckBox" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="LayerStyle">
            <table width="100%" cellpadding="5">
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold">Layout & Positions</span>
                    </td>
                    <td>
                        <span>Width</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Width" title="You can set the width of your layer. You can use pixels, percents, or the default value 'auto'. Examples: 100px, 50% or auto" />
                    </td>
                    <td>
                        <span>Height</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Height" title="You can set the height of your layer. You can use pixels, percents, or the default value 'auto'. Examples: 100px, 50% or auto" />
                    </td>
                    <td>
                        <span>Top</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-LayoutPositionTop" title="The layer position from the top of the slide. You can use pixels and percents. Examples: 100px or 50%. You can move your layers in the preview above with a drag n' drop, or set the exact values here." />
                    </td>
                    <td>
                        <span>Left</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-LayoutPositionLeft" title="The layer position from the left side of the slide. You can use pixels and percents. Examples: 100px or 50%. You can move your layers in the preview above with a drag n' drop, or set the exact values here." />
                    </td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr class="separator">
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold">Padding</span>
                    </td>
                    <td>
                        <span>Top</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-PaddingTop" title="Padding on the top of the layer. Example: 10px" />
                    </td>
                    <td>
                        <span>Right</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-PaddingRight" title="Padding on the right side of the layer. Example: 10px" />
                    </td>
                    <td>
                        <span>Bottom</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-PaddingBottom" title="Padding on the bottom of the layer. Example: 10px" />
                    </td>
                    <td>
                        <span>Left</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-PaddingLeft" title="Padding on the left side of the layer. Example: 10px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr class="separator">
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold">Border</span>
                    </td>
                    <td>
                        <span>Top</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-BorderTop" title="Border on the top of the layer. Example: 5px solid #000000" />
                    </td>
                    <td>
                        <span>Right</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-BorderRight" title="Border on the right side of the layer. Example: 5px solid #000000" />
                    </td>
                    <td>
                        <span>Bottom</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-BorderBottom" title="Border on the bottom of the layer. Example: 5px solid #000000" />
                    </td>
                    <td>
                        <span>Left</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-BorderLeft" title="Border on the left side of the layer. Example: 5px solid #000000" />
                    </td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr class="separator">
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold">Font</span>
                    </td>
                    <td>
                        <span>Family</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Family" title="List of your chosen fonts separated with a comma. Please use apostrophes if your font names contains white spaces. Example: Helvetica, Arial, sans-serif" />
                    </td>
                    <td>
                        <span>Size</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Size" title="The font size in pixels. Example: 16px." />
                    </td>
                    <td>
                        <span>Line-height</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-LineHeight" title="The line height of your text. The default setting is 'normal'. Example: 22px" />
                    </td>
                    <td>
                        <span>Color</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Color" />
                    </td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr class="separator">
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold">Misc</span>
                    </td>
                    <td>
                        <span>Background</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-Background" />
                    </td>
                    <td>
                        <span>Rounded corners</span>
                    </td>
                    <td class="align-left">
                        <input type="text" data-ls="layer-RoundedCorners" title="If you want rounded corners, you can set here its radius. Example: 5px" />
                    </td>
                    <td>
                        <span>Word-wrap</span>
                    </td>
                    <td class="align-left">
                        <input class="normalCheckBox" data-ls="layer-Wordwrap" type="checkbox" title="If you use custom sized layers, you have to enable this setting to wrap your text." />
                    </td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr class="separator">
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td colspan="9"></td>
                </tr>
                <tr>
                    <td class="align-center">
                        <span style="font-weight: bold;">Custom CSS</span>
                    </td>
                    <td colspan="7" class="align-left">
                        <input type="text" data-ls="layer-CustomCss" style="width: 100%;" title="If you want to set style settings other then above, you can use here any CSS codes. Please make sure to write valid markup." />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</li>
