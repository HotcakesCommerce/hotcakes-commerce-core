
function DynamicalList(opts, url, append, scrollappend) {
    this.opts = opts;
    this.url = url;
    this.append = append;
    this.scrollappend = scrollappend;
    var me = this;
  
    var moduleid = opts["moduleid"];
    var listid = "#list" + moduleid;
    var searchbackground = listid + " #search-background";
    var panginationid = listid + " #Pagination";
    var btnloadid = listid + " #btnLoadDynamic";
    var contentid = listid + " #dynamic";
    var wholepanel = "#commentlist";

    var totalpagecount = 0;
    function showLoader(){
	  $(searchbackground ).fadeIn(200);
    }
	
    function hideLoader(){
        $(searchbackground).fadeOut(200);
    };
    var curpage = 0;
   
    this.init = function (callback) {
        me.callback = callback;

        loadDynamic();
        $(btnloadid).click(loadDynamic);
    };

    function initPagination() {
        
        var num_entries = totalpagecount;
        
        if (totalpagecount > 1 && me.append && me.scrollappend) {
            $(btnloadid).hide();
            $(window).scroll(function () {
                if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                    loadDynamic();
                }
            });
        }

        if (totalpagecount == 1 || me.append) {
            $(panginationid).hide();
        } else {
            $(panginationid).pagination(num_entries, {
                num_edge_entries: 1,
                num_display_entries: 4,
                callback: pageselectCallback,
                items_per_page: 1
            });
        }
        if (totalpagecount == 1 || !me.append) {
            $(btnloadid).hide();
        }
        if (totalpagecount > 1 && me.append && !me.scrollappend) {
            $(btnloadid).show();
        }
    }

    function pageselectCallback(page_index, jq){

        curpage = page_index;
        loadDynamic();
		return false;
}

    this.refreshOpts = function(opts1, callback) {
        this.opts = opts1;

        initPagination();

        curpage = 0;
        this.init(callback);
    };


	function loadDynamic() {
	   
	    if (curpage > totalpagecount) return;
        showLoader();
        $.ajax({
            type: "POST",
            url: me.url,
            data: getOptions(),
            success: function (xml) {

                var xmlbd = $(xml).find('articleresult');
                var html = xmlbd.find('content').text();

                if (!html) {
                    $(wholepanel).hide();

                } else {
                    $(wholepanel).show();
                }
                var totalpage = xmlbd.find('total').text();
                if (totalpagecount == 0) {
                    totalpagecount = parseInt(totalpage);
                    initPagination();
                }
                if (totalpagecount == 0) {
                    $(listid).hide();
                    return;
                }
                if (parseInt(totalpage) >= curpage) {
                    if (parseInt(totalpage) == curpage) $(btnloadid).hide();
                    
                    if (me.append) $(contentid).append(html);
                    else $(contentid).html(html);
                    
                } else {
                    $(btnloadid).hide();
                }
                hideLoader();

              
                if (me.callback != null) {
                
                    me.callback();
                }
            },
            dataType: "xml"
        });


        curpage += 1;
    }  
    
   
    
    function getOptions() {
        var opt = me.opts;// { portalid: "<%=PortalId.ToString(CultureInfo.InvariantCulture) %>", moduleid: "<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>" };

        opt["currentpage"] = curpage;
       
        
        return opt;
    }
}