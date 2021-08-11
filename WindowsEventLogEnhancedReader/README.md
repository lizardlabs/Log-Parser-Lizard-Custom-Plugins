# Enhanced Windows Event Logs reader 
## For better results than built-in EVT format
## Read events from "Applications and Services Logs" (for instnace Operational logs)

Log Parser Lizard allows you to query data using SQL-like syntax from various input formats and can be downloaded here: http://lizard-labs.com/log_parser_lizard.aspx

Log Parser Lizard Plugin for reading Windows Event Logs to a tabular data. 

Microsoft Logparser engine have some troubles reading Windows Event Log input. For intance collecting logs from "Applications and Services Logs". A workaround can be foud here if you want to use built in EVT input format:

https://forums.iis.net/t/1170786.aspx?Collecting+logs+from+Applications+and+Services+Logs+on+Windows+R2+Windows+7

But this custom plugin may help with this and other issues with Windows Event Logs. Also adds some additional fields and information in the results so the plugin can be used as a replacement of built-in EVT input format.

## How to use the plugin with Log Parser Lizard:

1. Download and copy ***DotNetPlugin_WindowsEventLogExReader.vb*** (the plugin source code) to MyDocuments\LogParserLizard\Plugins folder. You can review the code for more information.
2. Create new query of type: "Custom Microsoft .Net Plugin"
3. Set query properties to: 
	- ***SourceCodeFile: DotNetPlugin_WindowsEventLogExReader.vb*** - path of the plugin source code. Path can be changed as you need.
	- ***DebugMode: OFF*** - turn on if you develop your own plugin to not cache the code and recompile on each start.
	- ***SimpleMode: ON*** - since the source code does not contains full class and namespaces, simple mode is ON
4. Write simple query to see if it works (replace the values with your own or download the data to a local file): 

```
SELECT TOP 1000 * 
FROM Microsoft-Windows-TerminalServices-LocalSessionManager/Operational
```

Note: full event source name can be found in Properties dialog box in Windows Event Viewer (right click on the event collection and click on Properties to open the dialog box).