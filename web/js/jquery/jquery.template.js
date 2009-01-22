/**
 * Jquery Plugin Stub
 *
 * Base for a jquery plugin
 *
 * @name jquery-template.js
 * @author Galen Collins
 * @version 0.1
 * @date January 10, 2009
 * @category jQuery plugin
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */

(function($) {

	/**
	 * Initializer
	 */
	$.fn.stub = function(options) {
		debug(this);
		var opts = $.extend({}, $.fn.stub.defaults, options);
	
		return this.each(function() {
		});
	};

	/**
	 * Debugging function
	 */
	function debug($obj) {
		if (window.console && window.console.log)
			window.console.log('hilight selection count: ' + $obj.size());
	};

 	/**
	 * Defaults handler
	 */
	$.fn.stub.defaults = { 
		defaults:'value',
	};
})(jQuery);

/**
 * public functions
 */
jQuery.stub = ({
	example: function(dir) {
	};
});

/**
 * Test Driver
 */
$(document).ready(function() {
	$('#container').stub();
});
