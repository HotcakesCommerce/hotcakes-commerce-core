
(function ($) {
    $.facebox = function (data, location, klass) {
        jfacebox.loading();
        if (data.ajax) fillFaceboxFromIframe(data.ajax, location);
        else if (data.image) fillFaceboxFromImage(data.image, location);
        else if (data.div) fillFaceboxFromHref(data.div, location);
        else if ($.isFunction(data)) data.call(jQuery);
        else $.facebox.reveal(data, klass);

    };

    jfacebox = {};
    /*
    * Public, jfacebox methods
    */
    $.extend(jfacebox, {
        settings: {
            opacity: .6,
            overlay: true,
            imageTypes: ['png', 'jpg', 'jpeg', 'gif'],
            iframeWidth: "500px",
            iframeHeight: "500px",
            faceboxHtml: '\
                        <div id="facebox" style="display:none;"> \
                          <div class="popup"> \
                            <table> \
                              <tbody> \
                                <tr> \
                                  <td class="tl"/><td class="b"/><td class="tr"/> \
                                </tr> \
                                <tr> \
                                  <td class="b"/> \
                                  <td class="body"> \
                                    <div class="content"> \
                                    </div> \
                                    <div class="footer"> \
                                      <a href="#" class="saveset dnnPrimaryAction"> \
                                        <span class=submitspan></span> \
                                      </a> \
                                      <a href="#" class="close dnnPrimaryAction"> \
                                        <span class=closespan></span> \
                                      </a> \
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
        loading: function () {
            init();
            if ($('#facebox .loading').length == 1) return true;
            showOverlay();

            $('#facebox .content').empty();
            $('#facebox .body').children().hide().end().
                append('<div class="loading"></div>');

            $('#facebox').css({
                top: getPageScroll()[1] + (getPageHeight() / 10),
                left: 385.5
            }).show();

            $(document).bind('keydown.facebox', function (e) {
                if (e.keyCode == 27) jfacebox.close();
                return true;
            });
            //第一处更改；
            //j(document).trigger('loading.facebox')
            jfacebox.Onloading();
            return true;
        },

        reveal: function (data, klass) {
            //第二处更改；
            //j(document).trigger('beforeReveal.facebox');
            jfacebox.BeforeReveal();
            if (klass) $('#facebox .content').addClass(klass);

            $('#facebox .content').append(data);

            $('#facebox .loading').remove();
            $('#facebox .body').children().fadeIn('normal');
            $('#facebox').css('left', $(window).width() / 2 - ($('#facebox table').width() / 2));
            //第三处更改
            //j(document).trigger('reveal.facebox').trigger('afterReveal.facebox');
            jfacebox.Revealed();
        },
        close: function () {
            $(document).trigger('close.facebox');
            $(document).trigger('closespan.facebox');
            return true;
        },
        saveset: function () {
            $(document).trigger('saveset.facebox');
            $(document).trigger('close.facebox');
            return false;
        }
    });

    /*
    * 公共的方法, 插件的facebox方法；
    */
    $.fn.facebox = function (settings) {
        //初始化；
        init(settings);
        //注册目标元素的点击事件

        function clickHandler() {
            jfacebox.loading(true);
            //支持 rel="facebox.inline_popup" 声明,用来为显示的部分添加样式
            var klass = this.rel.match(/facebox\[?\.(\w+)\]?/);
            var myself = this;
            if (klass) klass = klass[1];

            var width = myself.rel.match(/width=\[?(\w+)\]?/);
            var height = myself.rel.match(/height=\[?(\w+)\]?/);

            fillFaceboxFromHref(this.href, { width: width[1], height: height[1] }, klass);
            return false;
        }

        //调用clickHandler
        return this.click(clickHandler);
    };
    /*
    * 私有方法
    */
    //第一步：初始化；
    function init(settings) {
        //有多个facebox时出错
        //保证初始化动作只执行一次；
        //  if (jfacebox.settings.inited) return true;
        //   else jfacebox.settings.inited = true;



        //触发init.facebox事件；
        //合并用户设置
        if (settings) $.extend(jfacebox, jfacebox.settings, settings);
        //第四处更改；
        //j(document).trigger('init.facebox');
        jfacebox.Oninit();
        //此控件的向后兼容处理；
        makeCompatible();
        //把数组放入字符串；
        var imageTypes = jfacebox.settings.imageTypes.join('|');
        //'i'不区分大小写，'g'执行全局匹配（查找所有匹配而非在找到第一个匹配后停止）,'m'执行多行匹配。
        jfacebox.settings.imageTypesRegexp = new RegExp('\.' + imageTypes + '$', 'i');
        //加载facebox的html
       
        $('#facebox').remove();

        $('body').append(jfacebox.settings.faceboxHtml);
        //图片的预加载
        var preload = [new Image(), new Image()];
        preload[0].src = jfacebox.settings.closeImage;
        preload[1].src = jfacebox.settings.loadingImage;
        $('#facebox').find('.b:first, .bl, .br, .tl, .tr').each(function (i) {
            preload.push(new Image());
            preload.slice(-1).src = $(this).css('background-image').replace(/url\((.+)\)/, '$1');
        });
        //注册确认按钮的click事件
        $('#facebox .saveset').bind('click', jfacebox.saveset);
        //注册关闭按钮的click事件
        $('#facebox .close').bind('click', jfacebox.close);
        //给关闭按钮的src赋值
        $('#facebox .close_image').attr('src', jfacebox.settings.closeImage);
        return true;
    }


    // getPageScroll() by quirksmode.com
    function getPageScroll() {
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

    // Adapted from getPageSize() by quirksmode.com
    function getPageHeight() {
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


    // Backwards compatibility
    //向后兼容的处理
    function makeCompatible() {
        var js = jfacebox.settings;
        js.loadingImage = js.loading_image || js.loadingImage;
        js.closeImage = js.close_image || js.closeImage;
        js.imageTypes = js.image_types || js.imageTypes;
        js.faceboxHtml = js.facebox_html || js.faceboxHtml;
    }

    ;
    // 判断显示方式，并且显示；
    // 格式为 :
    // div: #id
    // image: 本地或者远程路径；
    // ajax: 1、iframe
    //       2、ajax
    function fillFaceboxFromHref(href, location, klass) {
        //利用'#'判断div片段;
        if (href.match(/#/)) {

            var url = window.location.href.split('#')[0];
            var target = href.replace(url, '');
            //div填充
            if (location.height != null && location.width != null)
                jfacebox.reveal($(target).clone().show().css({ "height": location.height, "width": location.width }), klass);
            else {
                jfacebox.reveal($(target).clone().show(), klass);
            }
        } else if (href.match(jfacebox.settings.imageTypesRegexp)) {
            //imageTypesRegexp:/.png|jpg|jpeg|gif$/i
            fillFaceboxFromImage(href, location, klass)
        } else {
            //if (jfacebox.settings.iframe)
            fillFaceboxFromIframe(href, location, klass);
            //else
            //    fillFaceboxFromAjax(href, klass);
        }
    }

    ;
    //Image填充页面
    function fillFaceboxFromImage(href, location, klass) {

        var image = new Image();
        image.onload = function () {
            if (location.width != null && location.height != null) {
                jfacebox.reveal('<div style="width:' + location.width + ';height:' + location.height + '" class="image"><img src="' + href + '" /></div>', klass)
            } else {
                jfacebox.reveal('<div class="image"><img src="' + href + '" /></div>', klass);
            }
        };
        image.src = href;
    }

    //ajax填充页面
    //    function fillFaceboxFromAjax(href, klass) {
    //        j.get(href, function(data) { jfacebox.reveal(data, klass) })
    //    }

    //Iframe填充页面
    function fillFaceboxFromIframe(href, location, klass) {
        var width = location.width;
        var height = location.height;
        width = width == null ? jfacebox.settings.iframeWidth : width;
        height = height == null ? jfacebox.settings.iframeHeight : height;

        var iframe = '<iframe class="iframe" src=' + href + ' width=' + width + ' height=' + height + ' border=0 marginheight=0 frameborder=no  marginwidth=0></iframe>';

        jfacebox.reveal(iframe, klass);

    }

    //跳过图层效果
    function skipOverlay() {
        return jfacebox.settings.overlay == false || jfacebox.settings.opacity === null;
    }
    //图层效果
    function showOverlay() {
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
    //删除图层效果
    function hideOverlay() {
        if (skipOverlay()) return false;
        $('#facebox_overlay').fadeOut(200, function () { //淡出效果
            $("#facebox_overlay").removeClass("facebox_overlayBG"); //移除图层样式
            $("#facebox_overlay").addClass("facebox_hide"); //添加隐藏样式
            $("#facebox_overlay").remove(); //删除图层元素
        });
        return false;
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
})(jQuery);
