
(function($) {
    $.fn.marquee = function(Direction, Width, Height, ScrollAmount, Delay, FullScreen) {
        var newMarquee = [], 
            last = this.length; 

        function getReset(newDir, marqueeRedux, marqueeState) {
       
            return newDir == -1 ? marqueeRedux[marqueeState.widthAxis] : 0;
        }

        function animateMarquee() {
            var i = newMarquee.length, 
                marqueeRedux = null,
                $marqueeRedux = null,
                marqueeState = {},
                newMarqueeList = []; 
                
            while (i--) {
                marqueeRedux = newMarquee[i]; 
                $marqueeRedux = $(marqueeRedux); 
                marqueeState = $marqueeRedux.data('marqueeState'); 
                if ($marqueeRedux.data('paused') !== true) {
                    marqueeRedux[marqueeState.axis] += (marqueeState.scrollamount * marqueeState.dir);
                    
                    //window.status=marqueeRedux[marqueeState.axis] + ":" + marqueeState.scrollamount * marqueeState.dir ;
                    
                   // alert(marqueeRedux[marqueeState.axis]);
                   // alert(marqueeState.scrollamount * marqueeState.dir);
                    if (marqueeState.last == marqueeRedux[marqueeState.axis]) {
                    //if ( marqueeRedux[marqueeState.axis]>=100) {
                   // alert("eq");
                   
                        marqueeState.last = -1; 
                        $marqueeRedux.trigger('stop'); 
                        marqueeState.loops--;
                        if (marqueeState.loops === 0) {
                            marqueeRedux[marqueeState.axis] = getReset(marqueeState.dir * -1, marqueeRedux, marqueeState);
                            $marqueeRedux.trigger('end');
                        } else {
                            newMarqueeList.push(marqueeRedux);
                            $marqueeRedux.trigger('start');
                            marqueeRedux[marqueeState.axis] = getReset(marqueeState.dir, marqueeRedux, marqueeState);
                        }
                        //alert(marqueeRedux[marqueeState.axis]);
                    } else {
                        newMarqueeList.push(marqueeRedux); 
                    }
                    marqueeState.last = marqueeRedux[marqueeState.axis];

                    $marqueeRedux.data('marqueeState', marqueeState);
                } else {
                    newMarqueeList.push(marqueeRedux);
                }
            }
            newMarquee = newMarqueeList;
            if (newMarquee.length) {
                setTimeout(animateMarquee, Delay);
            }
        }

        this.each(function(i) {
            var $marquee = $(this), 

                width = Width.substring(Width.length - 2, Width.length) == "px" ? Number(Width.substring(0, Width.length - 2)) : (0 == $marquee.width() ? 500 : $marquee.width() * Number(Width.substring(0, Width.length - 1)) / 100), 

                height = Height.substring(Height.length - 2, Height.length) == "px" ? Number(Height.substring(0, Height.length - 2)) : (0 == $marquee.height() ? 500 : $marquee.height() * Number(Height.substring(0, Height.length - 1)) / 100), 

                tempHtml = $marquee.html(),

                $marqueeRedux = $marquee.empty().append('<div style=" width: ' + width + 'px; height: ' + height + 'px; overflow:hidden;"><div style="width:' + width + 'px;height: ' + height + 'px;white-space: normal;">' + tempHtml + '</div><div style="clear:both;"></div></div>').children(), //动态生成div,嵌入目标元素内部内容
                marqueeRedux = $marqueeRedux.get(0), 

                direction = (Direction || 'left').toLowerCase(), 
                marqueeState = {
                    dir: /down|right/.test(direction) ? -1 : 1, 
                    axis: /left|right/.test(direction) ? 'scrollLeft' : 'scrollTop', 
                    widthAxis: /left|right/.test(direction) ? 'scrollWidth' : 'scrollHeight', 
                    loops: $marquee.attr('loop') || -1,
                    last: -1, 
                    scrollamount: ScrollAmount || 2, 
                    behavior: 'scroll',
                    width: /left|right/.test(direction) ? width : height
                };
                


            if (/left|right/.test(direction)) {
//                if ($.browser.msie) {
                    $marqueeRedux.find('> div').css('padding-left',   width + 'px');
                    $marqueeRedux.find('> div').css('padding-right', width + 'px');
                    $marqueeRedux.find('> div').css('padding-bottom', '0px');
                    $marqueeRedux.find('> div').css('padding-top', '0px');
//                } else {
//                    $marqueeRedux.find('> div').css('padding-left', width + 'px');
//                    $marqueeRedux.find('> div').css('padding-right',  width + 'px');
//                    $marqueeRedux.find('> div').css('padding-bottom', '0px');
//                    $marqueeRedux.find('> div').css('padding-top', '0px');
//                }
            } else {

                $marqueeRedux.find('> div').css('padding-left', '0px');
                $marqueeRedux.find('> div').css('padding-right', '0px');
                $marqueeRedux.find('> div').css('padding-bottom',  '0px');
                $marqueeRedux.find('> div').css('padding-top',height +  'px');
            }
            $marqueeRedux.bind('stop', function() {
                $marqueeRedux.data('paused', true);
            }).bind('pause', function() {
                $marqueeRedux.data('paused', true);
            }).bind('start', function() {
                $marqueeRedux.data('paused', false);
            }).bind('unpause', function() {
                $marqueeRedux.data('paused', false);
            }).data('marqueeState', marqueeState); 

            newMarquee.push(marqueeRedux);

            marqueeRedux[marqueeState.axis] = FullScreen != true ? getReset(marqueeState.dir, marqueeRedux, marqueeState) : (/down|up/.test(direction) ? height : width);

            $marqueeRedux.trigger('start');

            if (i + 1 == last) {
                animateMarquee();
            }
        });
        return $(newMarquee);
    };
} (jQuery));




function JMarquee(IDofElement, Direction, Width, Height, ScrollAmount, Delay, FullScreen) {
   // this.Run = function() {
    $("#" + IDofElement).marquee(Direction, Width, Height, ScrollAmount, Delay, FullScreen).mouseover(function() {
            $(this).trigger('stop');
        }).mouseout(function() {
            $(this).trigger('start');
        })
   // }
}