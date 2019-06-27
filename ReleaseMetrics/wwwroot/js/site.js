// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

// This is basically String.Format()
String.prototype.supplant = function (o) {
	return this.replace(/{([^{}]*)}/g,
		function (a, b) {
			var r = o[b];
			return typeof r === 'string' || typeof r === 'number' || typeof r === 'boolean' ? r : a;
		}
	);
};

// Case-insensitive version of indexOf. See https://stackoverflow.com/questions/8993773/javascript-contains-case-insensitive/38290557#38290557
String.prototype.indexOfIgnoreCase = function (needle) {
	var haystack = this; 
	var needleIndex = 0;
	var foundAt = 0;
	for (var haystackIndex = 0; haystackIndex < haystack.length; haystackIndex++) {
		var needleCode = needle.charCodeAt(needleIndex);
		if (needleCode >= 65 && needleCode <= 90) needleCode += 32; //ToLower
		var haystackCode = haystack.charCodeAt(haystackIndex);
		if (haystackCode >= 65 && haystackCode <= 90) haystackCode += 32; //ToLower

		//TODO: code to detect unicode characters and fallback to toLowerCase - when > 128?
		//if (needleCode > 128 || haystackCode > 128) return haystack.toLocaleLowerCase().indexOf(needle.toLocaleLowerCase();
		if (haystackCode !== needleCode) {
			foundAt = haystackIndex;
			needleIndex = 0; //Start again
		}
		else
			needleIndex++;

		if (needleIndex == needle.length)
			return foundAt;
	}

	return -1;
}

// https://jsfiddle.net/gabrieleromanato/bynaK/
$.fn.serializeFormJSON = function () {
	var o = {};
	var a = this.serializeArray();
	$.each(a, function () {
		if (o[this.name]) {
			if (!o[this.name].push) {
				o[this.name] = [o[this.name]];
			}
			o[this.name].push(this.value || '');
		} else {
			o[this.name] = this.value || '';
		}
	});
	return o;
};