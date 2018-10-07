(function ($) {
    $.facebox = function (data, klass) {
        if (data.refrashWindow != null) refrashWindow = data.refrashWindow;
        jfacebox.loading(data);
        if (data.ajax) fillFaceboxFromIframe(data.ajax, klass);
        else if (data.div != null) fillFaceboxFromDiv(data.div, klass);
        else $.facebox.reveal(data, klass);
    };

    jfacebox = {};
    var objParent = top;
    var refrashWindow = null;

    /*
    * 将Settings中的值和各方法都集成到jfacebox
    */
    $.extend(jfacebox, {
        settings: {
            opacity: .6,
            overlay: true,
            userAnimate: false,
            speedAnimate: 500,
            popWidth: "800px",
            popHeight: "600px",
            cssContainer: "",
            closeText: "",
            submitText: "",
            changesizeText: "",
            closeVisable: true,
            submitVisable: true,
            changesizeVisable: true,
            faceboxHtml: '\
                        <div id="facebox" style="display:none;"> \
                          <div class="popup"> \
                            <table width="100%" height="100%"> \
                              <tbody> \
                                <tr> \
                                  <td class="tl"/><td class="b"/><td class="tr"/> \
                                </tr> \
                                <tr> \
                                  <td class="b"/> \
                                  <td class="tdcontent"> \
                                    <div class="body"> \
                                        <div class="content"> \
                                        </div> \
                                        <div class="action"> \
                                          <a href="#" class="submit"> \
                                            <span class=submitspan></span> \
                                          </a> \
                                          <a href="#" class="changesize"> \
                                            <span class=changesizespan></span> \
                                          </a> \
                                          <a href="#" class="close"> \
                                            <span class=closespan></span> \
                                          </a> \
                                        </div> \
                                    </div> \
                                  </td> \
                                  <td class="b"/> \
                                </tr> \
                                <tr> \
                                  <td class="bl"/><td class="b"/><td class="br"/> \
                                </tr> \
                              </tbody> \
                            </table> \
                          </div> \
                        </div>'
        },

        Onloading: function () {
        },
        BeforeReveal: function () {
        },
        Revealed: function () {
        },
        Oninit: function () {
        },
        loading: function (data) {
            init(data);
            with (objParent) {
                if ($('#facebox .loading').length == 1) return true;
                showOverlay();
                $('#facebox .content').empty();
                $('#facebox .body').children().hide().end().append('<div class="loading"></div>');
                $('#facebox').css({
                    top: getPageScroll()[1] + (getPageHeight() / 10),
                    left: 385.5
                }).show();
            }

            $(document).bind('keydown.facebox', function (e) {
                if (e.keyCode == 27) jfacebox.close();
                return true;
            });
            jfacebox.Onloading();
            return true;
        },

        reveal: function (data, klass) {
            jfacebox.BeforeReveal();
            with (objParent) {
                var wi = jfacebox.settings.popWidth;
                var winWidth = $(window).width() - 40;
                var popheight = jfacebox.settings.popHeight;
                var winheight = $(window).height() - 40;
                var ti = getPageScroll()[1] + ($(window).height() / 2 - popheight / 2);
                var li = $(window).width() / 2 - wi / 2;
                if (wi > winWidth) {
                    wi = winWidth;
                    li = 0;
                }
                if (popheight > winheight) {
                    popheight = winheight;
                    ti = getPageScroll()[1];
                }
                if (klass) $('#facebox .content').addClass(klass);
                $('#facebox .content').append(data);
                $('#facebox .loading').remove();
                if (jfacebox.settings.userAnimate) {
                    $('#facebox .body').children().show("fast");
                    $('#facebox .body').css("width", 20).css("height", 20);
                    $('#facebox').css('left', $(window).width() / 2).css("top", $(window).height() / 2);
                    $('#facebox .body').animate({ width: wi, height: popheight }, jfacebox.settings.speedAnimate * 2, function () {
                        SetIframeHeight();
                    });
                    $('#facebox').animate({ left: li, top: ti }, jfacebox.settings.speedAnimate * 2);
                }
                else {
                    $('#facebox .body').css("width", wi).css("height", popheight);
                    SetIframeHeight();
                    $('#facebox .body').children().fadeIn('normal');
                    $('#facebox').css('left', li).css('top', ti);
                }
            }
            jfacebox.Revealed();
        },
        close: function () {
            $(document).trigger('close.facebox');
            $(document).trigger('AfterCloseFacebox');
            return true;
        },
        submit: function () {
            $(document).trigger('SubmitFacebox');
            return true;
        },
        changesize: function () {
            $(document).trigger('ChangeFaceboxWindowSize');
            return false;
        }
    });

    /*
    * 私有方法
    */
    //第一步：初始化；
    function init(settings) {
        //保证初始化动作只执行一次；
//        if (jfacebox.settings.inited) return true;
//        else jfacebox.settings.inited = true;
        //触发init.facebox事件；
        //合并用户设置
        if (settings) $.extend(jfacebox.settings, settings);
        if (jfacebox.settings.cssContainer != "")
            jfacebox.settings.faceboxHtml = "<div class='" + jfacebox.settings.cssContainer + "'>" + jfacebox.settings.faceboxHtml + "</div>";
        jfacebox.Oninit();
        //加载facebox的html
        with (objParent) {
//            alert("");
//            console.log($('#facebox'));
            $('#facebox').remove();
            $('body').append(jfacebox.settings.faceboxHtml);
            $("#facebox .body").resizable();
            $("#facebox .body").bind("resize", SetIframeHeight);
            //设置按钮
            if (!jfacebox.settings.closeVisable) $('#facebox .close').css("display", "none");
            if (!jfacebox.settings.submitVisable) $('#facebox .submit').css("display", "none");
            if (!jfacebox.settings.changesizeVisable) $('#facebox .changesize').css("display", "none");
            if (jfacebox.settings.closeText != "") $('#facebox .closespan').html(jfacebox.settings.closeText);
            if (jfacebox.settings.submitText != "") $('#facebox .submitspan').html(jfacebox.settings.submitText);
            if (jfacebox.settings.changesizeText != "") $('#facebox .changesizespan').html(jfacebox.settings.changesizeText);
            //注册确认按钮的click事件
            $('#facebox .submit').bind('click', jfacebox.submit);
            //注册关闭按钮的click事件
            $('#facebox .close').bind('click', jfacebox.close);
            //注册改变尺寸按钮的click事件
            $('#facebox .changesize').bind('click', jfacebox.changesize);

            return true;
        }
    }


    //获取滚动条位置
    function getPageScroll() {
        with (objParent) {
            var xScroll, yScroll;
            if (self.pageYOffset) {
                yScroll = self.pageYOffset;
                xScroll = self.pageXOffset;
            } else if (document.documentElement && document.documentElement.scrollTop) { // Explorer 6 Strict
                yScroll = document.documentElement.scrollTop;
                xScroll = document.documentElement.scrollLeft;
            } else if (document.body) { // all other Explorers
                yScroll = document.body.scrollTop;
                xScroll = document.body.scrollLeft;
            }
            return new Array(xScroll, yScroll);
        }
    }

    // 获取页面高度
    function getPageHeight() {
        with (objParent) {
            var windowHeight;
            if (self.innerHeight) {	// all except Explorer
                windowHeight = self.innerHeight;
            } else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
                windowHeight = document.documentElement.clientHeight;
            } else if (document.body) { // other Explorers
                windowHeight = document.body.clientHeight;
            }
            return windowHeight;
        }
    }

    //计算iframe的高度
    function SetIframeHeight() {
        var bodyWidth = $('#facebox .body').width();
        var bodyHeight = $('#facebox .body').height();
        $('#facebox').css('left', $(window).width() / 2 - bodyWidth / 2);
        $('#facebox').css('top', getPageScroll()[1] + $(window).height() / 2 - bodyHeight / 2);
        var actionHeight = $('#facebox .action').height();
        $('#facebox .content').css("height", bodyHeight - actionHeight);
        return false;
    }


    ;
    // 判断显示方式，并且显示；
    // 格式为 :
    // div: #id
    // image: 本地或者远程路径；
    // ajax: 1、iframe
    //       2、ajax
    //Iframe填充页面
    function fillFaceboxFromIframe(href, klass) {
        with (objParent) {
            var iframe = '<iframe class="iframe" src=' + href + '></iframe>';
            jfacebox.reveal(iframe, klass);
        }

    }

    function fillFaceboxFromDiv(div, klass) {
        var divData = $("<div></div>").append(div.clone()).html();
        with (objParent) {
            jfacebox.reveal(divData, klass);
        }
    }

    //跳过图层效果
    function skipOverlay() {
        with (objParent) {
            return jfacebox.settings.overlay == false || jfacebox.settings.opacity === null;
        }
    }
    //图层效果
    function showOverlay() {
        with (objParent) {
            if (skipOverlay()) return false;
            if ($('facebox_overlay').length == 0)
                $("body").append('<div id="facebox_overlay" class="facebox_hide"></div>');
            $('#facebox_overlay').hide()//隐藏图层
            .addClass("facebox_overlayBG")//添加图层样式
            .css('opacity', jfacebox.settings.opacity)//设置透明
            .click(function () { $(document).trigger('close.facebox'); })//注册点击事件
            .fadeIn(200); //淡入效果
            return false;
        }
    }
    //删除图层效果
    function hideOverlay() {
        with (objParent) {
            if (skipOverlay()) return false;
            $('#facebox_overlay').fadeOut(200, function () { //淡出效果
                $("#facebox_overlay").removeClass("facebox_overlayBG"); //移除图层样式
                $("#facebox_overlay").addClass("facebox_hide"); //添加隐藏样式
                $("#facebox_overlay").remove(); //删除图层元素
            });
            return false;
        }
    }
    /*
    * 绑定关闭事件；
    */
    $(document).bind('close.facebox', function () {
        //1、关闭facebox事件
        $(document).unbind('keydown.facebox');
        //2、注册淡出事件
        $('#facebox').fadeOut(function () {
            $('#facebox .content').removeClass().addClass('content');
            hideOverlay();
            $('#facebox .loading').remove();
        });
    });
    $(document).bind('RefrashFaceboxParentWindow', function () {
        alert("RefrashFaceboxParentWindow");
        if (refrashWindow != null) {
            refrashWindow.location.reload(true);
            //refrashWindow.location = "http://www.baidu.com";
        }
    });
    $(document).bind('ChangeFaceboxWindowSize', function () {
        var wi = $(window).width() - 40;
        if (jfacebox.settings.userAnimate == false) {
            if (wi <= $('#facebox .body').width()) {
                $('#facebox .body').css("width", jfacebox.settings.popWidth);
                $('#facebox .body').css("height", jfacebox.settings.popHeight);
                $('#facebox').css('left', $(window).width() / 2 - ($('#facebox .body').width() / 2));
                $('#facebox').css('top', getPageScroll()[1] + (getPageHeight() / 10));
            }
            else {
                $('#facebox').css('left', 0).css('top', getPageScroll()[1]);
                $('#facebox .body').css("width", wi);
                $('#facebox .body').css("height", $(window).height() - 40);
            }
            SetIframeHeight();
        }
        else {
            if (wi <= $('#facebox .body').width()) {
                $('#facebox .body').animate({ width: jfacebox.settings.popWidth, height: jfacebox.settings.popHeight }, jfacebox.settings.speedAnimate);
                $('#facebox .content').animate({ height: jfacebox.settings.popHeight - $('#facebox .action').height() }, jfacebox.settings.speedAnimate);
                $('#facebox').animate({ left: $(window).width() / 2 - (jfacebox.settings.popWidth / 2), top: getPageScroll()[1] + ($(window).height() / 2 - jfacebox.settings.popHeight / 2) }, jfacebox.settings.speedAnimate);
            }
            else {
                $('#facebox .body').animate({ width: wi, height: $(window).height() - 40 }, jfacebox.settings.speedAnimate);
                $('#facebox .content').animate({ height: $(window).height() - 40 - $('#facebox .action').height() }, jfacebox.settings.speedAnimate);
                $('#facebox').animate({ left: 0, top: getPageScroll()[1] }, jfacebox.settings.speedAnimate);

            }
        }
    });
})(jQuery);



