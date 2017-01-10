//(Javascript modified from https://www.kirupa.com/html5/get_element_position_using_javascript.htm)
function ScrollPosition(el) {
	if (el.tagName == 'BODY') {
		// deal with browser quirks with body/window/document and page scroll
		return {
			left: el.scrollLeft || document.documentElement.scrollLeft,
			top: el.scrollTop || document.documentElement.scrollTop,
			right: (el.scrollLeft + el.scrollWidth) || (document.documentElement.scrollLeft + document.documentElement.scrollWidth),
			bottom: (el.scrollTop + el.scrollHeight) || (document.documentElement.scrollTop + document.documentElement.scrollHeight)
		};
	}
	return {
		left: el.scrollLeft, 
		top: el.scrollTop, 
		right: el.scrollLeft + el.scrollWidth, 
		bottom: el.scrollTop + el.scrollHeight
	};
}

// Helper function to get an element's exact position
function AbsolutePosition(elem) { // crossbrowser version
    var box = elem.getBoundingClientRect();

    var body = document.body;
    var docEl = document.documentElement;

    var scrollTop = window.pageYOffset || docEl.scrollTop || body.scrollTop;
    var scrollLeft = window.pageXOffset || docEl.scrollLeft || body.scrollLeft;

    var clientTop = docEl.clientTop || body.clientTop || 0;
    var clientLeft = docEl.clientLeft || body.clientLeft || 0;

    var boxWidth = box.right - box.left;
    var top  = box.top +  scrollTop - clientTop;
    var left = box.left + scrollLeft - clientLeft;

    return { top: Math.round(top), left: Math.round(left), midX: Math.round(left + (boxWidth / 2)) };
}
