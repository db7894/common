/**
 * jQuery Scroll to Top
 *
 * This adds a scroll to top button on the page
 *
 * @name jquery-scroller.js
 * @author Galen Collins
 * @version 0.1
 * @date January 10, 2009
 * @category jQuery plugin
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */
(function($){
	$.fn.scroller = function(options) {
		var opts = $.extend({}, $.fn.scroller.defaults, options);
		return this.each(function() {
			$('<a href="#" id="scroll-top">'+opts.title+'</a>').appendTo(this);
			$('#scroll-top').css(opts.css)
			.unbind('click').click(function(){
				$('html,body').animate({scrollTop:0}, opts.speed);
				return false; /* stop link from following */
			});
		});
	
	};

	$.fn.scroller.defaults = {
		css: {
			position:'absolute',
			display:'scroll',
			position:'fixed',
			bottom:'1%',
			right:'1%',
		},
		speed:'slow',
		title:'Top'
	};
})(jQuery);

// -------------------------------------------------------------------------- //
// Example Run
// -------------------------------------------------------------------------- //
//$(document).ready(function() {
//	/* default floating scroller */
//	$('#container').scroller();
//
//	/* non-floating scroller */
//	opts = { css: { position:'absolute',bottom:'1%',right:'1%'}};
//	$('#container').scroller(opts);
//});
