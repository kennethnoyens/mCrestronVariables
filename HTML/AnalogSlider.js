getHtmlForVariableType["AnalogSlider"] = 
	function (key, variable) 
	{ 
		setTimeout(//function(){ alert("Hello"); }, 3000);
		//$( document ).on( "pagecreate", 
		function() {
	    $("<input type='number' data-type='range' id='slider" + key + "input' min='0' max='100' step='1' value='" + variable.variableValue + "'>" )
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
			//alert("response from "+ slider_value +": " + result);
		});
		}, 1000);
		//);
		//content = '<form><label for="slider'+variableName+'">Input slider:</label>';
		//content += '<input type="range" name="slider'+variableName+'" id="slider'+variableName+'" value="60" min="0" max="100">';
		//return(content + "<form/><br />");
	};