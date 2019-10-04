# Log Parser Lizard Custom Plugins

Collection of custom plugins for Log Parser Lizard created by [Lizard Labs Software](http://www.lizard-labs.com)

Log Parser Lizard allows you to query data using SQL-like syntax from various input formats. Find out more on our web site:

http://lizard-labs.com/log_parser_lizard.aspx

In case that out-of-the box parsers can’t read the complex data files, custom .NET plugins in Visual Basic .Net or C# can be created. 

This is the repository with some samples. If you are interested, you can build and add your own samples here.

## Content:

- SurveyGizmoResponseReader.vb - read SurveyGizmo JSON v5 API Response into flat table to query with SQL
- Various Samples:
  - DatesTable.vb - This sample is similar to "Dates Table Input Format" to fill table of dates and its properties
  - JsonReader.cs - parse JSON formatted log file to demonstrate the use of MultiFileReaderInputContextBase. Can parse large files since Newtonsoft.Json.JsonTextReader is used to read the records.
  - SampleDotNetPlugin_XmlReader.vb - parse log4net and log4j XML files. Demonstrate use of XmlTextReader to parse huge XML formatted logs
  - SampleDotNetPlugin_ReadTextFiles.vb and SampleDotNetPlugin2.vb - simple demo to read text files line by line

