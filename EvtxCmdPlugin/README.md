# Windows Event Logs reader using EvtxECmd parser (from  Eric Zimmerman) - (Beta)

EvtxECmd repository on GitHub: https://github.com/EricZimmerman/evtx
 
By using this plugin you are accepting the EvtxECmd license and all other dll libraries included (MIT): https://github.com/EricZimmerman/evtx/blob/master/LICENSE

Log Parser Lizard allows you to query data using SQL-like syntax from varius input formats and can be downloaded here: http://lizard-labs.com/log_parser_lizard.aspx

This is a Log Parser Lizard Plugin for reading Windows Event Logs (using EvtxECmd parser) to a tabular data. 


## How to use the plugin with Log Parser Lizard:

1. Download and copy this folder and its files (the plugin source code and DLLs) to MyDocuments\LogParserLizard\Plugins folder. You can review the code for more information.
2. IMPORTANT: Change the property value of **MapsDirectory** in  the VB source file to the EvtxECmd MAPS folder path. Latest MAPS can be found here (you can use GIT to sync): https://github.com/EricZimmerman/evtx/tree/master/evtx/Maps
3. Create new query of type: "Custom Microsoft .Net Plugin"
4. Set query properties to: 
	- ***SourceCodeFile: EvtxCmdPlugin\DotNetPlugin_EvtxCmdReader.vb*** - path of the plugin source code. Path can be changed as you need.
	- ***DebugMode: OFF*** - turn on if you develop your own plugin to not cache the code and recompile on each start.
	- ***SimpleMode: ON*** - since the source code does not contains full class and namespaces, simple mode is ON
5. Write simple query to see if it works (replace the values with your own or download the data to a local file): 

```
SELECT TOP 100 * FROM 'c:\temp\Application.evtx'
```

