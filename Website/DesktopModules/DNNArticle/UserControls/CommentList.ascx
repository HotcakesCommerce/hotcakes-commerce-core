<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.CommentList" %>
<%@ Register TagPrefix="zldnn" TagName="makcomment" Src="~/desktopmodules/DNNArticle/UserControls/MakeComment.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="PageNav" Src="~/desktopmodules/DNNArticle/PageNav.ascx" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
    <AjaxSettings>
       
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

 <asp:Panel ID="panelMakeComment" runat="server" style="display:none;" CssClass="CommentReplyPanel">
     <div  class="CommentReplyPanelDiv">
          <zldnn:makcomment id="ctlMakeComment" runat="server" IsReplyMode="true" />
     </div>
 </asp:Panel>

<asp:Panel ID="panelCommentList" runat="server"  >


    <asp:Literal ID="ltNoComment" runat="server"></asp:Literal>
    <asp:Panel ID="panelComments" runat="server"  >
    <asp:Panel runat="server" ID="panelSortType" CssClass="panelSortType">
        <asp:Label ID="lbSortType" runat="server" CssClass="Normal" resourckey="lbSortType"></asp:Label>
        <asp:DropDownList ID="cboSortType" runat="server" CssClass="Normal" 
            AutoPostBack="true" 
            onselectedindexchanged="cboSortType_SelectedIndexChanged" >
                                        <asp:ListItem Value="0" resourcekey="liNewestFirst">NewestFirst</asp:ListItem>
                                        <asp:ListItem Value="1" resourcekey="liOldestFirst">OldestFirst</asp:ListItem>
                                        <asp:ListItem Value="2" resourcekey="liMostPopular">MostPopular</asp:ListItem>
                                       <asp:ListItem Value="3" resourcekey="liMostUnlike">Most unliked</asp:ListItem>
                                    </asp:DropDownList>
    </asp:Panel>
      <asp:PlaceHolder ID="plBegin" runat="server"></asp:PlaceHolder>
     <asp:Repeater ID="rptContent" runat="server" OnItemDataBound="rptContent_ItemDataBound" OnItemCommand="rptContent_ItemCommand">
        <ItemTemplate>
          <asp:Panel ID="panelItem" runat="server"  >
        <asp:PlaceHolder ID="pl" runat="server"></asp:PlaceHolder>
        
        </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>
    
  
    <asp:PlaceHolder ID="plEnd" runat="server"></asp:PlaceHolder>
        <zldnn:PageNav runat="server" ID="MyPageNav" ></zldnn:PageNav>
        <asp:Literal ID="ltJS" runat="server"></asp:Literal>
        
        

</asp:Panel>
</asp:Panel>

<script type="text/javascript">

    var already = false;
    function pageLoad() {
        if (!already)
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxResponseEnd);
        already = true;
    }
    
    function AjaxResponseEnd(sender, args) {
        Init();
    }

    function Init() {
       // alert("init");
        $('a[id^="approve-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('approve-', '');

            ajaxPostWithlink($(this), 'APPROVE', jid, HideApproveLink);

        });

        $('a[id^="unapprove-"]').click(function (e) {
            e.preventDefault();
            var jid = $(this).attr('id').replace('unapprove-', '');
            
            //            var data = { };
            //            data.AlbumId = jid;

            ajaxPostWithlink($(this), 'UNAPPROVE', jid, HideUnApproveLink);
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

            ajaxPostWithlink($(this),'LIKE', jid, HideLikeLink);
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

    jQuery(document).ready(function ($) {
        
        Init();
    });
</script>