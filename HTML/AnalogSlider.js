VariableType_getHtml["AnalogSlider"] = 
	function(variableName, key)
	{
		return variableName + ": <form id='slider" + key + "'></form>";
	};
VariableType_function["AnalogSlider"] = 
	function (key, variable) 
	{
		var minMax = variable.variableOptions;
		if(minMax.includes("-"))
			{
				var keySplit = minMax.split("-");
				min = keySplit[0];
				max = keySplit[1];
			}
		else
			{
				min = 0;
				max = 100;
			}

	    $("<input type='number' data-type='range' id='slider" + key + "input' min='" + min +"' max='"+ max +"' step='1' value='" + variable.variableValue + "'>" )
	        .appendTo( "#slider" + key )
	        .slider()
	        .textinput();
		$("#slider" + key + "input").on("slidestop", function() {
		  var theObject = $("#slider" + key + "input");
		  var slider_value = theObject.val();
		  var result = $.ajax({
			method: "PUT",
			url: "cws/mv/vars/"+key,
			data: slider_value
			});
		});
	};