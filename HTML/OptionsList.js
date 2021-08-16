VariableType_getHtml["OptionsList"] = 
	function(variableName, key)
	{
		//return variableName + ": <form><div class='ui-field-contain'><select id='options" + key + "'></select></div></form>";
		return variableName + ": <form><select id='options" + key + "'></select></form>";
	};
VariableType_function["OptionsList"] = 
	function (key, variable) 
	{
		//1:HD-WP,2:Signage, 3:ESG
		//var options = variable.variableOptions;
		if(variable.variableOptions.includes(","))
		{
			var options = variable.variableOptions.split(",");		
			options.forEach(function (item, index) 
			{
				if(item.includes(":"))
					items = item.split(":");
				
				$("<option value='" + items[0] + "'>" + items[1] + "</option>")
	        	.appendTo( "#options" + key );
			});
		}
	    
		$("#options" + key).selectmenu();//'refresh', true);
		$("#options" + key).val(variable.variableValue);
		$("#options" + key).selectmenu("refresh");
		
		$("#options" + key).on("change", function() {
		  var theObject = $("#options" + key);
		  var value = theObject.val();
		  var value = value.replace(":", "");
		  var result = $.ajax({
			method: "PUT",
			url: "cws/mv/vars/"+key,
			data: value
			});
		});
	};