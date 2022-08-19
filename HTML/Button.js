VariableType_getHtml["Button"] = 
	function(variableName, key)
	{
		return '<input type="button" class="ui-button ui-widget ui-corner-all" id="button' + key + '"value="' + variableName +'">';
	};
VariableType_function["Button"] = 
	function (key, variable) 
	{
		$("#button" +  key).mousedown(function() {
	  
		  var result = $.ajax({
			method: "PUT",
			url: "cws/mv/vars/"+key,
			data: "PUSH"
			});
		});
		$("#button" +  key).mouseup(function() {
	  
		  var result = $.ajax({
			method: "PUT",
			url: "cws/mv/vars/"+key,
			data: "RELEASE"
			});
		});
	};