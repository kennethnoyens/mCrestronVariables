VariableType_getHtml["AnalogText"] = 
	function(variableName, key)
	{
		return variableName + ': <div id="atext' + key + '">Loading...</div>';
		// return variableName + " " +  val.variableValue  + "<br />";
	};
VariableType_function["AnalogText"] = 
	function (key, variable) 
	{
		$("#atext" + key).html(variable.variableValue/variable.variableOptions);
	};