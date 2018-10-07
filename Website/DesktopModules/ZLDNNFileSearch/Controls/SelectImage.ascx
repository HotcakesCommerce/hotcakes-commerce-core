<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectImage.ascx.cs" Inherits="ZLDNN.Modules.ZLDNNFileSearch.Controls.SelectImage" %>
 
<script type="text/javascript" >
    $(function () {
        $("#<%=lbRemoveImage.ClientID %>").click(function () {
            $("#<%=lbReplaceImage.ClientID %>").attr("style", "display:none;");
            $("#<%=lbRemoveImage.ClientID %>").attr("style", "display:none;");
            $("#<%=imgShow.ClientID %>").attr("style", "display:none;");
            $("#<%=txtSelectImagePath.ClientID %>").attr("value", "");
            $("#<%=txtImageRelativePath.ClientID %>").attr("value", "");
            $("#<%=txtSelectImageStyle.ClientID %>").attr("value", "");
            $("#<%=txtSelectFileInfo.ClientID %>").attr("value", "");
            $("#<%=txtSelectFileInfo.ClientID %>").attr("value", "");
            $("#<%=lblFileName.ClientID %>").html("");
            $("#<%=lbSetImage.ClientID %>").attr("style", "display:inline;");
            return false;
        });
        $("#<%=lbSetImage.ClientID %>").click(function () {
            var aurl = $("#<%=lbSetImage.ClientID %>").attr("rel");
            //aurl = aurl + "&imageurl=" + $("#<%=txtImageRelativePath.ClientID %>").attr("value");
            $.facebox({ ajax: aurl,
                opacity: .4   
            },
                { width: $(window).width() - 200, height: $(window).height() - 180 }
            );
            $('#facebox .closespan').html("<%=ColseText %>");
            $('#facebox .submitspan').html("<%=SetText %>");
            $(document).bind('closespan.facebox', function () {
                var txtValue = $("#<%=txtSelectImagePath.ClientID %>").attr("value");
                var srcValue = $("#<%=imgShow.ClientID %>").attr("src");
                if (srcValue != txtValue) {
                    $("#<%=txtSelectImagePath.ClientID %>").attr("value", srcValue);
                }
            });
            $(document).bind('saveset.facebox', function () {
                var checkUrl = $("#facebox .ifurlcheckbox").attr("checked");
                var srcValue = $("#<%=txtSelectImagePath.ClientID %>").attr("value");
                if (checkUrl == "checked") {
                    if (srcValue == "") {
                        return false;
                    }
                } else {
                    if (srcValue == "") {
                        $("#<%=lbReplaceImage.ClientID %>").attr("style", "display:none;");
                        $("#<%=lbSetImage.ClientID %>").attr("style", "display:inline;");
                        $("#<%=lbRemoveImage.ClientID %>").attr("style", "display:none;");
                        return false;
                    } else {
                        $("#<%=lbReplaceImage.ClientID %>").attr("style", "display:inline;");
                        $("#<%=lbRemoveImage.ClientID %>").attr("style", "display:inline;");
                        $("#<%=lbSetImage.ClientID %>").attr("style", "display:none;");
                        var styleValue = $("#<%=txtSelectImageStyle.ClientID %>").attr("value") + "border:0;display:inline;";
                        $("#<%=imgShow.ClientID %>").attr("style", styleValue);
                        $("#<%=imgShow.ClientID %>").attr("src", srcValue);
                        var fileinfo = $("#<%=txtSelectFileInfo.ClientID %>").attr("value");
                        if (fileinfo != null && fileinfo != "") {
                            var str = fileinfo.split("|");
                            if (str.length > 1 && str[1] != null && str[1] != "") {
                                $("#<%=lblFileName.ClientID %>").html(str[1]);
                                var strfilter = ".psd|.jpg|.gif|.bmp|.jpeg|.png|.pic|.ico|.tiff|.iff|.lbm|.mag|.mac|.mpt|.opt";
                                var strEx = str[1].split(".");
                                var fileEx = strEx[strEx.length - 1];
                                if (strfilter.indexOf("." + fileEx) >= 0) $("#<%=lblFileName.ClientID %>").html("");
                                else $("#<%=lblFileName.ClientID %>").html(str[1]);
                            }
                        }
                    }
                }
                return true;
            });
            return false;
        });
        $("#<%=lbReplaceImage.ClientID %>").click(function () {
            $("#<%=lbSetImage.ClientID %>").trigger('click');
            return false;
        });
    });
</script> 

 <div runat="server" id="divInfo" class="divHeader">
        <asp:Image runat="server" ID="imgShow"/><asp:Label runat="server" ID="lblFileName"></asp:Label>
        <asp:TextBox runat="server" CssClass="recordImagePathTextBox" ID="txtSelectImagePath" style="display:none;"></asp:TextBox>
        <asp:TextBox runat="server" CssClass="recordRelativePathTextBox" ID="txtImageRelativePath" style="display:none;"></asp:TextBox>
        <asp:TextBox runat="server" CssClass="recordImageStyleTextBox" ID="txtSelectImageStyle" style="display:none;"></asp:TextBox>
        <asp:TextBox runat="server" CssClass="recordFileInfoTextBox" ID="txtSelectFileInfo" style="display:none;"></asp:TextBox>
    </div>
    <asp:LinkButton runat="server" ID="lbReplaceImage"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbSetImage"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbRemoveImage"></asp:LinkButton>
     