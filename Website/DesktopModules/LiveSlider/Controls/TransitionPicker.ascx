<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransitionPicker.ascx.cs"
    Inherits="Mandeeps.DNN.Modules.LiveSlider.Controls.TransitionPicker" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<!--[if lt IE 9]>
		<script src="assets/js/html5.js"></script>
	<![endif]-->
<style>
    a.mbutton
    {
        font-family: Helvetica,Arial,sans-serif;
        padding: 5px 10px;
        background: #818181;
        filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=#818181, endColorstr=#656565);
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr=#818181, endColorstr=#656565)";
        background: -webkit-gradient(linear, left top, left bottom, from(#818181), to(#656565));
        background: -moz-linear-gradient(center top , #818181 0%, #656565 100%) repeat scroll 0 0 transparent;
        border-color: #FFFFFF;
        border-radius: 3px;
        color: #FFFFFF;
        font-weight: bold;
        text-decoration: none;
        color: #fff;
        text-shadow: 0 1px 1px #000000;
        display: inline-block;
    }
    
    a.mbutton:hover
    {
        background: #4E4E4E;
        color: #ffffff;
        text-decoration: none;
    }
</style>
<%--<script>
    (function ($) {
        $.QueryString = (function (a) {
            if (a == "") return {};
            var b = {};
            for (var i = 0; i < a.length; ++i) {
                var p = a[i].split('=');
                if (p.length != 2) continue;
                b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
            }
            return b;
        })(window.location.search.substr(1).split('&'))
    })(jQuery);

    $(function () {
        $('.transitions a').click(function () {
            window.parent.ipSelectFile($.QueryString['controlid'], $(this).attr('rel').substr(2));
        });
    });
</script>--%>
<div>
    <table class="transitions">
        <tr id="Transition2D" runat="server">
            <td>
                <div class="table ls-transition-list" id="slide-transitions-2d">
                    <table>
                        <thead>
                            <tr>
                                <th class="c">
                                    ID
                                </th>
                                <th>
                                    Transition name
                                </th>
                                <th class="c">
                                    ID
                                </th>
                                <th>
                                    Transition name
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </td>
        </tr>
        <tr id="Transition3D" runat="server">
            <td>
                <a href="#" rel="trNone" class="mbutton" style="float: right;">No Transition</a>
                <div style="clear: both;">
                </div>
                <div class="table ls-transition-list" id="slide-transitions-3d">
                    <table>
                        <thead>
                            <tr>
                                <th class="c">
                                    ID
                                </th>
                                <th>
                                    Transition name
                                </th>
                                <th class="c">
                                    ID
                                </th>
                                <th>
                                    Transition name
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</div>
