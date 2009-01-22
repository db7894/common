/**
 * jQuery Swapper
 *
 * @name jquery-swapper.js
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

	/**
	 * Public selector point
	 */
    $.fn.swapper = function(options) {
		debug(this);
		settings = $.extend($.fn.swapper.defaults, options);
		initialize();
		return this.each(function(){
			$(this).unbind("click").bind("click", function(){
				start(this);
				return false;
			});
		});
	};

	/**
	 * Public default settings
	 */
	$.fn.swapper.defaults = {
		speed:		"normal",
		storage:	"body",
		start:		function(){},
		loaded:		function(){},
		hide:		{opacity:'0'},
		show:		{opacity:'1'}
	};

	/**
	 * Private Initializer
	 */
	function initialize() {
		$("<div id='slide-loader'/>")
			.css({display:"none"})
			.appendTo(document.body);	
		settings.storage = $(settings.storage);
	};

	/**
	 * Private debugger
	 */
	function debug($obj) {
		if (window.console && window.console.log) {
			window.console.log('Selection Count: ' + $obj.size());
			$obj.each(function(){
				window.console.log('['+$obj.html()+'] ' + $obj.attr("href"));
			});
		}
	};
		
	/**
	 * Private swapper
	 */
	function start(el) {
		settings.start();
		settings.storage.animate(settings.hide,
			settings.speed, function(){
				$.ajax({type: "GET",
					url: $(el).attr("href"),
					success: function(data) {
						$("<div style='display:none' id='slide-loader'/>")
							.appendTo(document.body);	
						$("#slide-loader").html(data).queue(function() {
							settings.loaded();
							settings.storage.html($("#slide-loader").html());
							settings.storage.animate(settings.show,
			   					settings.speed);
							$("#slide-loader").remove();
							$(this).dequeue();
						});
					}});
			});
	};
})(jQuery);

//---------------------------------------------------------------------------// 
// Example Driver
//---------------------------------------------------------------------------// 
//$(document).ready(function(){
//	$('a.swapper').swapper({
//		storage:	".post-archived",
//		speed:		"slow"
//	});
//});
