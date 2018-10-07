$( document ).ready(function( $ ) {
// nivoSlider
$('#slider').nivoSlider();

							 
							 
// bxslider //
$(".bxslider").bxSlider({
	auto: true,
	pager:false,
});  


// Testimonial //
$(".Testimonial").bxSlider({
	auto: true,
	pager:true,
});  


// OurMission //
$("#OurMission").bxSlider({
	auto: true,
	pager:true,
	controls: false,
});  

// bx-example1 //
$('.bx-example1').bxSlider({
  slideWidth: 270,
    minSlides: 2,
    maxSlides: 4,
    moveSlides: 1,
    slideMargin: 10
});

// bx-thumbnail //
$('.bx-thumbnail').bxSlider({
	auto: true,
	controls:false,
	pagerCustom: '#bx-thumbnail-pager',
});

$('#works').carouFredSel({
					responsive: true,
					auto: false,
					prev: '#prev1',
					next: '#next1',
					scroll: 1,
					swipe: {
						onMouse: true,
						onTouch: true
					},
					items: {
						width:480,
						height:'auto',
					//	height: '30%',	//	optionally resize item-height
						visible: {
							min: 1,
							max: 2
						}
					}
});

	  
});