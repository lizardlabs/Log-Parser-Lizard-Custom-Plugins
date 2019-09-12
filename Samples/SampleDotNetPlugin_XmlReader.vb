        Inherits MultiFileReaderInputContextBase

'When compiled in simple mode adds code around the auto generated class. This class is inherited from builtin type MultiFileReaderInputContextBase

#Region "Log entry type"

	'Type that contains log entries

        Public Class Log4jLogItemSample
            Public Property LocalTimeStamp As DateTime
            Public Property Logger As String
            Public Property Thread As String
            Public Property LevelIndex As Integer

            Public Property Level As String
                Get
                    Return _level
                End Get
                Set(ByVal value As String)

                    If value <> _level Then
                        _level = value
                        assignLevelIndex(_level)
                    End If
                End Set
            End Property
            Public Property Message As String
            Public Property MachineName As String
            Public Property UserName As String
            Public Property HostName As String
            Public Property App As String
            Public Property Throwable As String
            Public Property [Class] As String
            Public Property Method As String
            Public Property File As String
            Public Property Line As String
            Public Property Uncategorized As String
            Public Property TimeStampUTC As DateTime
            Public Property Delta As Double?
            Public Property LocalDate As DateTime
            Public Property LocalTime As DateTime

            Private _level As String

            Private Sub assignLevelIndex(ByVal level As String)
                Dim ul As String = If(Not String.IsNullOrWhiteSpace(level), level.Trim().ToUpper(), String.Empty)

                Select Case ul
                    Case "DEBUG"
                        LevelIndex = 1
                    Case "INFO"
                        LevelIndex = 2
                    Case "WARN"
                        LevelIndex = 3
                    Case "ERROR"
                        LevelIndex = 4
                    Case "FATAL"
                        LevelIndex = 5
                    Case Else
                        LevelIndex = 0
                End Select
            End Sub
        End Class

#End Region

#Region "Constructors and destructors"
        Public Sub New()
            MyBase.New()
			MyBase.IncludeFullRecord = False
        End Sub
#End Region


#Region "Overrides"
        Public Overrides Function GetEnumerator() As IEnumerator
            Return GetEntries().GetEnumerator
        End Function
#End Region

#Region "Methods"

		'Code to open and read from strams after FROM and parse the content in the entry object than YIELD the entry. LPL will do the rest

        Protected Iterator Function GetEntries() As System.Collections.Generic.IEnumerable(Of Log4jLogItemSample)
            Dim nt = New NameTable()
            Dim mgr = New XmlNamespaceManager(nt)

            mgr.AddNamespace("log4j", "http://jakarta.apache.org/log4j")
            Dim dateMinUx = New DateTime(1970, 1, 1, 0, 0, 0, 0)

            Dim settings = New XmlReaderSettings With {.ConformanceLevel = ConformanceLevel.Fragment}
            Dim pc = New XmlParserContext(nt, mgr, String.Empty, XmlSpace.[Default])

            For Each st as System.Tuple(Of String, System.IO.Stream) In LizardLabs.InputOutput.StreamFactory.GetStreams(From, Me.Recurse)
				'Parent class properties
				Me.FullRecord = Nothing 'No full record in this plugin
                Me.StreamLineNumber = 0
                Me.CurrentStreamName = st.Item1
                Me.StreamRecordIndex = 0

                Using srdr As New LizardLabs.InputOutput.AdvancedTextReader(st.Item2, Me.TextReaderOptions)
                    Using xmlRdr = XmlTextReader.Create(srdr, settings, pc)
                        Dim prevTimeStamp As DateTime? = Nothing

                        Dim li As Xml.IXmlLineInfo = TryCast(xmlRdr, IXmlLineInfo)

                        While xmlRdr.Read()
                            If (xmlRdr.NodeType <> XmlNodeType.Element) OrElse (xmlRdr.Name <> "log4j:event") Then Continue While
                            Dim entry As New Log4jLogItemSample

                            If (li IsNot Nothing) Then Me.StreamLineNumber = li.LineNumber

                            entry.Logger = xmlRdr.GetAttribute("logger")
                            entry.TimeStampUTC = dateMinUx.AddMilliseconds(Convert.ToDouble(xmlRdr.GetAttribute("timestamp")))
                            entry.LocalTimeStamp = entry.TimeStampUTC.ToLocalTime()
                            entry.LocalDate = entry.LocalTimeStamp.Date
                            entry.LocalTime = New DateTime(1900, 1, 1).Add(entry.LocalTimeStamp.TimeOfDay)
                            If prevTimeStamp.HasValue Then entry.Delta = (entry.TimeStampUTC - prevTimeStamp.Value).TotalSeconds
                            prevTimeStamp = entry.TimeStampUTC
                            entry.Level = xmlRdr.GetAttribute("level")
                            entry.Thread = xmlRdr.GetAttribute("thread")

                            While xmlRdr.Read()
                                Dim breakLoop = False

                                Select Case xmlRdr.Name
                                    Case "log4j:event"
                                        breakLoop = True
                                    Case Else

                                        Select Case xmlRdr.Name
                                            Case ("log4j:message")
                                                entry.Message = xmlRdr.ReadString()
                                            Case ("log4j:data")

                                                Select Case xmlRdr.GetAttribute("name")
                                                    Case ("log4net:UserName")
                                                        entry.UserName = xmlRdr.GetAttribute("value")
                                                    Case ("log4japp")
                                                        entry.App = xmlRdr.GetAttribute("value")
                                                    Case ("log4jmachinename")
                                                        entry.MachineName = xmlRdr.GetAttribute("value")
                                                    Case ("log4net:HostName")
                                                        entry.HostName = xmlRdr.GetAttribute("value")
                                                End Select

                                            Case ("log4j:throwable")
                                                entry.Throwable = xmlRdr.ReadString()
                                            Case ("log4j:locationInfo")
                                                entry.[Class] = xmlRdr.GetAttribute("class")
                                                entry.Method = xmlRdr.GetAttribute("method")
                                                entry.File = xmlRdr.GetAttribute("file")
                                                entry.Line = xmlRdr.GetAttribute("line")
                                        End Select
                                End Select

                                If breakLoop Then Exit While
                            End While

                            StreamRecordIndex += 1
                            Yield entry
                        End While
                    End Using
                End Using
            Next
        End Function

        Protected Overrides Sub InitValueExtractor()
            'Init ValueExtractor that extracts property values from an object instance
            Me.ValueExtractor = New LizardLabs.InputOutput.ValueExtractors.ObjectPropertiesValueExtractor(GetType(Log4jLogItemSample))
        End Sub

#End Region

