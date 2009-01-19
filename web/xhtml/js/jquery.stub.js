/**
 * Jquery Plugin Stub
 */

(function($) {

	/**
	 * Initializer
	 */
	$.fn.stub = function(options) {
		//debug(this);
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
		defaults:
	};
})(jQuery);

/**
 * public functions
 */
jQuery.stub = ({
	example: function(dir) {
	};
});
