var hcc = hcc || {};

(function ($) {
    hcc.vars = null;
    hcc.config = hcc.config || {};

    hcc.getVars = function () {
        if (hcc.vars == null) {
            var jsonValue = $('#__hccVariables').val();

            if (jsonValue)
                hcc.vars = jQuery.parseJSON(jsonValue);
            else
                hcc.vars = [];
        }
        return hcc.vars;
    },

    hcc.getVar = function (key, def) {
        if (hcc.getVars()[key] != null) {
            return hcc.getVars()[key];
        }
        return def;
    },

    hcc.getSiteRoot = function () {
        if (dnn && dnn.getVar)
            return dnn.getVar("hc_siteRoot", "/");
        else
            return hcc.getVar("hc_siteRoot", "/");
    };
    hcc.getResourceUrl = function (path) {
        var resourceUrl = hcc.getSiteRoot();
        resourceUrl += "DesktopModules/Hotcakes/Core/" + path;
        return resourceUrl;
    };
    hcc.getServiceUrl = function (path) {
        var serviceUrl = hcc.getSiteRoot();
        serviceUrl += "DesktopModules/Hotcakes/API/mvc/" + path;
        return serviceUrl;
    };

    hcc.formIsValid = function ($form) {
        var validator = $.data($form[0], 'validator');
        if (validator && !validator.form()) {
            return false;
        }
        return true;
    };

    hcc.getUrlVar = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };

    //fix record image height
    hcc.autoHeight = function (className) {
        $(window).resize(function () {
            onresize(className);
        });

        var onresize = function () {
            $(className).each(function (i, e) {
                var $el = $(e);
                $el.height($el.width() * 0.7);
                $el.find("img").height($el.width() * 0.7);
                //$el.css("display", "table-cell");
            });
        }
        setTimeout(onresize, 10);
    }

    $.xhrPool = [];
    $.xhrPool.abortAll = function () {
        $(this).each(function (i, jqXHR) {   //  cycle through list of recorded connection
            jqXHR.abort();  //  aborts connection
            $.xhrPool.splice(i, 1); //  removes from list by index
        });
    }
    $.ajaxSetup({
        beforeSend: function (jqXHR) { $.xhrPool.push(jqXHR); }, //  annd connection to list
        complete: function (jqXHR) {
            var i = $.xhrPool.indexOf(jqXHR);   //  get index for current connection completed
            if (i > -1) $.xhrPool.splice(i, 1); //  removes from list by index
        }
    });

    $.fn.ajaxLoader = function (type, options) {
        // Becomes this.options
        var defaults = {
            bgColor: '#fff',
            duration: 300,
            opacity: 0.5
        }
        this.container = this;

        this.init = function (element, options) {
            var container = this.container;
            // Delete any other loaders
            this.remove();
            // Create the overlay 
            var overlay = $('<div></div>').css({
                'background-color': this.options.bgColor,
                'opacity': this.options.opacity,
                'width': container.width(),
                'height': container.height(),
                'position': 'absolute',
                'top': '0px',
                'left': '0px',
                'z-index': 99999
            }).addClass('ajax_overlay');
            // add an overiding class name to set new loader style 
            if (this.options.classOveride) {
                overlay.addClass(this.options.classOveride);
            }
            // insert overlay and loader into DOM 
            if (container.css("position") != "absolute") {
                container.css("position", "relative");
            }

            container.append(
                overlay.append(
                    $('<div></div>').addClass('hcAjaxLoader')
                ).fadeIn(this.options.duration)
            );
        };

        this.remove = function () {
            var overlay = this.container.children(".ajax_overlay");
            if (overlay.length) {
                //overlay.fadeOut(this.options., function () {
                overlay.remove();
                //});
            }
        }

        if (type != 'stop') {
            this.options = jQuery.extend(defaults, options);
            this.init(this, options);
        }
        else {
            this.remove();
        }

    };

    $.fn.hcDialog = function (options) {
        if (options == 'close' || options == 'open') {
            $(this).dialog(options);
        } else {
            var defaults = {
                width: $(this).data('width'),
                height: $(this).data('height'),
                minWidth: $(this).data('minWidth'),
                minHeight: $(this).data('minHeight'),
                maxWidth: $(this).data('maxWidth'),
                maxHeight: $(this).data('maxHeight'),
                title: $(this).data('title'),
                autoOpen: true,
                dialogClass: hcc.config.dialogClass
            };
            var fakeForm = $(this).closest('div[data-type="form"]');
            defaults.parentElement = fakeForm.length ? fakeForm : $('form');
            options = $.extend(defaults, options);

            $(this).dialog({
                width: options.width,
                height: options.height,
                minWidth: options.minWidth,
                minHeight: options.minHeight,
                maxWidth: options.maxWidth,
                maxHeight: options.maxHeight,
                title: options.title,
                modal: false,
                autoOpen: options.autoOpen,
                closeOnEscape: true,
                position: "center",
                dialogClass: options.dialogClass,
                open: function (event, ui) {
                    $(".ui-widget-overlay").remove();
                    var $overlay = $("<div/>").addClass("ui-widget-overlay");
                    $overlay.css("position", "fixed");
                    $overlay.css("z-index", "1000");
                    $overlay.appendTo(options.parentElement);
                    $(this).parent().appendTo(options.parentElement);

                    var $dialog = $(this).parent();
                    $dialog.css("position", "fixed");
                    $dialog.css("z-index", "1001");
                    var top = ($(window).height() - $dialog.height()) / 2;
                    if (top < 10) {
                        top = 10;
                        $dialog.find(".ui-dialog-content")
                            .height($(window).height() - 150)
                            .css("overflow", "auto");
                    }
                    $dialog.css("top", top);

                    if (options.maxHeight)
                        $dialog.find(".ui-dialog-content").css("max-height", options.maxHeight);

                    if (options.open) options.open();
                },
                close: function () {
                    if (options.close) options.close();
                    $(".ui-widget-overlay").remove();
                }
            });
        }
    };

    $.fn.scrollTo = function (target, options) {
        var settings = $.extend({
            scrollTarget: target,
            offsetTop: 50,
            duration: 500
        }, options);
        return this.each(function () {
            $(this).animate({ scrollTop: $(target).offset().top - settings.offsetTop }, parseInt(settings.duration));
        });
    };

    $.fn.hcCardInput = function (iconsSelector, cleanNumberCallback) {
        var $icons = $(iconsSelector);

        function CardInput(el) {
            this.$el = $(el);
            this.init();
        }

        CardInput.prototype = {
            init: function () {
                var $el = this.$el;
                var self = this;
                $el.change(function () { cleanNumberCallback($el); });
                $el.keyup(function () { self.changeIcons() });
            },
            changeIcons: function () {
                var cc = this.getCardType(this.$el.val());
                if (cc != "") {
                    $icons.find("span").addClass("cc-disabled");
                    $icons.find(".cc-" + cc).removeClass("cc-disabled");
                }
                else {
                    $icons.find("span").removeClass("cc-disabled");
                }
            },
            getCardType: function (number) {
                var re = new RegExp("^4");
                if (number.match(re) != null)
                    return "visa";

                re = new RegExp("^(34|37)");
                if (number.match(re) != null)
                    return "amex";

                re = new RegExp("^5[1-5]");
                if (number.match(re) != null)
                    return "mastercard";

                re = new RegExp("^(6011|644|645|646|647|648|649|65)");
                if (number.match(re) != null)
                    return "discover";

                var firstSix = number.substring(0, 6) * 1;
                if (firstSix >= 622126 && firstSix <= 622925) {
                    return "discover";
                }

                re = new RegExp("^(30|38)");
                if (number.match(re) != null)
                    return "diners";

                re = new RegExp("^35");
                if (number.match(re) != null)
                    return "jcb";

                return "";
            }
        };

        return this.each(function () {
            if (!$.data(this, "plugin_hcCardInput")) {
                $.data(this, "plugin_hcCardInput", new CardInput(this));
            }
        });
    };

    $.fn.selectionRibbon = function () {
    	return this.each(function () {
    		var $sel = $(this);
    		if (!$sel.data("selribbon")) {
    			var $options = $sel.find("option");
    			var $ul = $("<ul></ul>");
    			$ul.addClass("hcSelectionRibbon");

    			$options.each(function (i, el) {
    				var $li = $("<li/>")
    				$li.attr("data-value", $(el).val());
    				$li.text($(el).text());
    				$ul.append($li);
    				$li.click(function () {
    					$sel.val($(el).val());
    					$sel.change();
    					$ul.find("li").removeClass("selected");
    					$li.addClass("selected");
    				});
    				if ($(el).val() == $sel.val())
    					$li.addClass("selected");
    			});

    			$sel.after($ul);
    			$sel.hide();
    			$sel.data("selribbon", true);
    		}
    	});
    };

    $.fn.selectionList = function () {
    	return this.each(function () {
    		var self = $(this);

    		if (!self.data("selectionList")) {
    			var $options = self.find("option");
    			var $selectedOption = self.find("option:selected");

    			var $wrapperSpan = $("<span></span>");
    			var $span = $("<span></span>");
    			var $ul = $("<ul></ul>");

    			$wrapperSpan.addClass(self.attr("class"));
    			$span.text($selectedOption.text());

    			$wrapperSpan.on("mouseover", function () {
    				$ul.show();
    			});
    			$wrapperSpan.on("mouseout", function () {
    				$ul.hide();
    			});

    			$options.each(function (i, el) {
    				var $li = $("<li/>")
    				$li.attr("data-value", $(el).val());
    				$li.text($(el).text());
    				$ul.append($li);
    				$li.on("click", function () {
    					self.val($(el).val());
    					$span.text($(el).text());

    					$ul.hide();

    					self.change();
    					$ul.find("li").removeClass("selected");
    					$li.addClass("selected");
    				});
    				if ($(el).val() == self.val())
    					$li.addClass("selected");
    			});

    			self.after($wrapperSpan);
    			$wrapperSpan.append($span);
    			$wrapperSpan.append($ul);

    			self.hide();
    			self.data("selectionList", true);
    		}
    	});
    };
}(jQuery));

jQuery(function ($) {
	$("select.hcSelectionRibbon").selectionRibbon();
	$("select.hcSelectionList").selectionList();
});
