<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynamicCommentList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.DynamicCommentList.DynamicCommentList" %>


<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/desktopmodules/DNNArticle/UserControls/DynamicCommentList/lib/pagination.css" Priority="120" ></dnn:DnnCssInclude>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server" FilePath="~/desktopmodules/DNNArticle/UserControls/DynamicCommentList/lib/jquery.pagination.js" Priority="120"  />
<dnn:DnnJsInclude ID="DnnJsInclude2" runat="server" FilePath="~/desktopmodules/DNNArticle/UserControls/DynamicCommentList/lib/dynamicalist.js" Priority="120" />
<div id="commentlistouter">
<div id="commentlist">
<asp:Panel runat="server" ID="panelSortType" CssClass="panelSortType">
    <asp:Label ID="lbSortType" runat="server" CssClass="Normal" resourckey="lbSortType"></asp:Label>
    <asp:DropDownList ID="cboSortType" runat="server" CssClass="Normal">
        <asp:ListItem Value="0" resourcekey="liNewestFirst" Selected="True">NewestFirst</asp:ListItem>
        <asp:ListItem Value="1" resourcekey="liOldestFirst">OldestFirst</asp:ListItem>
        <asp:ListItem Value="2" resourcekey="liMostPopular">MostPopular</asp:ListItem>
        <asp:ListItem Value="3" resourcekey="liMostUnlike">Most unliked</asp:ListItem>
    </asp:DropDownList>
</asp:Panel>

<div id="list<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>" class="dylist">
     <asp:PlaceHolder ID="plBegin" runat="server"></asp:PlaceHolder>
    <div id="dynamic">
    </div>
    <div id="search-background">
        <label>
            <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                runat="server" />
        </label>
    </div>
      <asp:PlaceHolder ID="plEnd" runat="server"></asp:PlaceHolder>
    <div id="Pagination" class="mypagination">
    </div>
    <a id="btnLoadDynamic" style="display:none" href="javascript:void(0)" class="dnnPrimaryAction"><%=Localization.GetString("Load.Text",LocalResourceFile) %></a>
</div>
</div>
<asp:Literal ID="lterror" runat="server"></asp:Literal>
</div>

<asp:Literal ID="ltJS" runat="server"></asp:Literal>
        
<script type="text/javascript">

function getOptions<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>() {
        var opt = {
            portalid: "<%=DNNArticleView.PortalId.ToString(CultureInfo.InvariantCulture) %>",
            moduleid: "<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>",
            articleid: "<%=DNNArticleView.ItemID.ToString(CultureInfo.InvariantCulture) %>",
            tabid: "<%=DNNArticleView.TabId.ToString(CultureInfo.InvariantCulture) %>",
            SortType: $('#<%=cboSortType.ClientID%>').val()
        };
    if (!$('#<%=cboSortType.ClientID%>').val())
        opt.SortType = 0;
      
        return opt;
    }

    var opt = getOptions<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>();
       
    var dl<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %> = new DynamicalList(opt,'<%=DotNetNuke.Common.Globals.ResolveUrl("~/desktopmodules/DNNArticle/UserControls/DynamicCommentList/getlist.ashx?portalid=" + DNNArticleView.PortalId)%>',false,false);
  
    var already = false;


    function pageLoad() {
        if (!already)
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxResponseEnd);
        already = true;
    }

    function AjaxResponseEnd(sender, args) {

         dl<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>.init(Init);
    }

    function Init() {
         
        $('a[id^="approve-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('approve-', '');

            ajaxPostWithlink($(this), 'APPROVE', jid, HideApproveLink);

        });

        $('a[id^="delete-"]').click(function (e) {

            if (confirm('<%=Localization.GetString("DeleteItem") %>')) {
                e.preventDefault();
                var jid = $(this).attr('id').replace('delete-', '');

                ajaxPostWithlink($(this), 'DELETE', jid, refreshList);
            }

        });

        $('a[id^="unapprove-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('unapprove-', '');

            //            var data = { };
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'UNAPPROVE', jid, HideUnApproveLink);
        });

        $('a[id^="reply-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('reply-', '');
            if (jid) {
                dnnModal.show(makecommenturl + '?popUp=true&commentid=' + jid, true, 300, 550, false);
            } else {
            dnnModal.show(makecommenturl + '?popUp=true', true, 300, 550, false);
            }
        });

        function HideApproveLink(result) {
            var id = result;
            // $('#approve-' + id).style.color = "red";
            $('#approve-' + id).hide();

            $('#unapprove-' + id).show();
        }

        function HideUnApproveLink(result) {
            var id = result;

            $('#approve-' + id).show();
            $('#unapprove-' + id).hide();
        }

        $('a[id^="like-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('like-', '');

            ajaxPostWithlink($(this), 'LIKE', jid, HideLikeLink);
        });

        $('a[id^="dislike-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('dislike-', '');
            //            var data = {};
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'DISLIKE', jid, HideUnLikeLink);
        });

        $('a[id^="flag-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('flag-', '');
            //            var data = {};
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'FLAG', jid, HideFlagLink);
        });

        $('a[id^="unflag-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('unflag-', '');
            //            var data = {};
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'UNFLAG', jid, HideUnFlagLink);
        });

        function HideLikeLink(result) {
            var ss = result.split(";");
            var id = ss[0];

            $('#like-' + id).hide();
            $('#dislike-' + id).show();


            $('#likenumber-' + id).text(ss[1]);
            $('#dislikenumber-' + id).text(ss[2]);
        }

        function HideUnLikeLink(result) {

            var ss = result.split(";");
            var id = ss[0];

            $('#like-' + id).show();
            $('#dislike-' + id).hide();

            $('#likenumber-' + id).text(ss[1]);
            $('#dislikenumber-' + id).text(ss[2]);
        }


        $('a[id^="markspam-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('markspam-', '');

            ajaxPostWithlink($(this), 'MARKSPAM', jid, HideSpamLink);
        });

        $('a[id^="marknospam-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('marknospam-', '');
            //            var data = {};
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'MARKNOSPAM', jid, HideUnSpamLink);
        });

        function HideSpamLink(result) {
            var id = result;

            $('#markspam-' + id).hide();
            $('#marknospam-' + id).show();
        }

        function HideUnSpamLink(result) {
            var id = result;

            $('#markspam-' + id).show();
            $('#marknospam-' + id).hide();
        }

        function HideFlagLink(result) {
            var ss = result.split(";");
            var id = ss[0];

            $('#flag-' + id).hide();

            $('#reporttimes-' + id).text(ss[1]);
            // $('#unflag-' + id).show();
        }

        function HideUnFlagLink(result) {
            var ss = result.split(";");
            var id = ss[0];

            $('#unflag-' + id).hide();

            $('#reporttimes-' + id).text(ss[1]);
            // $('#flag-' + id).show();
        }
    }


    function ajaxPostWithlink(link, method, data, callback) {

        //        var className = link.attr('class');
        //        link.removeClass(className);
        link.addClass("linkdisabled");

        $.ajax({
            type: "POST",
            url: BaseURL + data + "&action=" + method,

            success: function (res) {
                link.removeClass("linkdisabled");
                //                link.addClass(className);
                if (typeof (callback) != "undefined") {

                    callback(res);


                }
            },
            error: function (xhr, status, error) {
                link.removeClass("linkdisabled");
                //                link.addClass(className);
                alert(error);
            }
        });
    };

    function ajaxPost(method, data, callback) {


        $.ajax({
            type: "POST",
            url: BaseURL + data + "&action=" + method,

            success: function (res) {
                if (typeof (callback) != "undefined") {

                    callback(res);


                }
            },
            error: function (xhr, status, error) {
                alert(error);
            }
        });
    };

    function refreshList() {
        dl<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>.refreshOpts(getOptions<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>(),Init);

    }

    function HideObj() {
        
    }

//    jQuery(document).ready(function ($) {

//        Init();
//    });
</script>

<script type="text/javascript" language="javascript">
   
    $(document).ready(function () {
      
    $('#<%=cboSortType.ClientID%>').change(function() {
        refreshList();
    });


        dl<%=DNNArticleView.ModuleId.ToString(CultureInfo.InvariantCulture) %>.init(Init);

    
    });

  
    
</script>
