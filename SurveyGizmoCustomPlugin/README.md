# Read SurveyGizmo JSON v5 API Response into flat table

Log Parser Lizard allows you to query data using SQL-like syntax from varius input formats and can be downloaded here: http://lizard-labs.com/log_parser_lizard.aspx


Log Parser Lizard Plugin for transforming SurveyGizmo V5 API Response JSON format to a tabular data. 

SurveyGizmo Response API documentation can be found here: https://apihelp.SurveyGizmo.com/help/surveyresponse-sub-object-v5


## How to use the plugin with Log Parser Lizard:

1. Doanlwas and copy ***SurveyGizmoResponseReader.vb*** (the plugin source code) to MyDocuments\LogParserLizard\Plugins folder. You can review the code for more information.
2. Create new query of type: "Custom Microsoft .Net Plugin"
3. Set query properties to: 
	- ***SourceCodeFile: SurveyGizmoResponseReader.vb*** - path of the plugin source code. Path can be changed as you need.
	- ***DebugMode: OFF*** - turn on if you develop yor own plugin
	- ***SimpleMode: OFF*** - since the source code conatins full class and namespaces, simple mode is OFF
	- ***ClassName: LplSurveyGizmoCustomInputPlugin*** - the main class name
4. Write simple query to see if it works (replace the values with your own or download the data to a local file): 

```
SELECT TOP 1000 * 
FROM 
'https://restapi.SurveyGizmo.com/v5/survey/XXXXX/surveyresponse?api_token=YYYYYYY&api_token_secret=ZZZZZZ
```

