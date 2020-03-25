        Inherits EnumerableInputContext

#Region "Constructors and destructors"
        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Public properties"

        Public Property ReverseDirection As Boolean = True

        Public Property ResolveSIDs As Boolean = True

        Public Property StringsSep As String = "|"

        Public Property FormatMsg As Boolean = True

        Public Property FullTextMsg As Boolean = True
		
		Public Property IgnoreErrors As Boolean = True
		
        'sample advanced query if needed = "<QueryList>
        '                  <Query Id=""0"" Path=""Application"">
        '                    <Select Path=""Application"">*</Select>
        '                  </Query>
        '                </QueryList>"
		Public Property AdvancedEventLogQuery as string = Nothing
#End Region

#Region "Attributes"

        Private Shared _fromSeparatorRegex As New System.Text.RegularExpressions.Regex("\s*[;,]\s*(?!(?<=(?:^|[;,])\s*""""(?:[^""""]|""""""""|\\"""")*[;,]\s*)(?:[^""""]|""""""""|\\"""")*""""\s*(?:[;,]|$))", System.Text.RegularExpressions.RegexOptions.Compiled)
        Private Shared _trimChars As Char() = {" ", vbTab, vbCr, vbLf, """", "'"}
        Private _fromEntities As String() = Nothing

#End Region

#Region "Parsing code"

		'Class to store readed record
        Public Class EventRecordEx
            ' name of the event log where this event is logged.
            Public Property LogName As String

            ' event record identifier of the event in the log.
            Public Property RecordId As Long?

            ' time, in System.DateTime format, that the event was created.
            Public Property TimeCreated As Date?


            ' identifier for this event. All events with this identifier value represent the same type of event.
            Public Property EventId As Integer

            ' display name of the level for this event.
            Public Property EventTypeName As String

            ' full text message
            Public Property Message As String

            ' display name of the task for the event.
            Public Property TaskDisplayName As String

            ' name of the event provider that published this event.
            Public Property ProviderName As String

            ' globally unique identifier (GUID) of the event provider that published this event.
            Public Property ProviderId As Guid?

            ' display names of the keywords used in the keyword mask for this event.
            Public Property Keywords As String

            ' display name of the opcode for this event.
            Public Property OpcodeName As String

            ' security descriptor of the user whose context is used to publish the event.
            Public Property UserId As String

            ' name of the computer on which this event was logged.
            Public Property MachineName As String

            ' user-supplied properties of the event.
            Public Property Strings As String

            ' process identifier for the event provider that logged this event.
            Public Property ProcessId As Integer?

            ' thread identifier for the thread that the event provider is running in.
            Public Property ThreadId As Integer?

            ' level of the event. The level signifies the severity of the event. For
            '     the name of the level, get the value of the EventTypeName
            '     property.
            Public Property EventType As Byte?

            ' keyword mask of the event. Get the value of the System.Diagnostics.Eventing.Reader.EventRecord.KeywordsDisplayNames property to get the name of the keywords used in this mask.
            Public Property KeywordsFlag As Long?

            ' opcode of the event. The opcode defines a numeric value that identifies
            '     the activity or a point within an activity that the application was performing
            '     when it raised the event. For the name of the opcode, get the value of the OpcodeDisplayName
            '     property.
            Public Property Opcode As Short?

            ' a task identifier for a portion of an application or a component that publishes
            '     an event. A task is a 16-bit value with 16 top values reserved. This type allows
            '     any value between 0x0000 and 0xffef to be used. To obtain the task name, get
            '     the value of the System.Diagnostics.Eventing.Reader.EventRecord.TaskDisplayName
            '     property.
            Public Property Task As Integer?

            ' version number for the event.
            Public Property Version As Byte?

            ' qualifier numbers that are used for event identification.
            Public Property Qualifiers As Integer?

            ' a globally unique identifier (GUID) for a related activity in a process
            Public Property RelatedActivityId As Guid?

            ' globally unique identifier (GUID) for the activity in process for which the event is involved. This allows consumers to group related activities.
            Public Property ActivityId As Guid?

            Public Sub Clear()
                With Me
                    .ActivityId = Nothing
                    .EventId = 0
                    .KeywordsFlag = Nothing
                    .Keywords = Nothing
                    .EventType = Nothing
                    .EventTypeName = Nothing
                    .LogName = Nothing
                    .MachineName = Nothing
                    .Message = Nothing
                    .Opcode = Nothing
                    .OpcodeName = Nothing
                    .ProcessId = Nothing
                    .Strings = Nothing
                    .ProviderName = Nothing
                    .Qualifiers = Nothing
                    .RecordId = Nothing
                    .RelatedActivityId = Nothing
                    .Task = Nothing
                    .TaskDisplayName = Nothing
                    .ThreadId = Nothing
                    .TimeCreated = Nothing
                    .UserId = Nothing
                    .Version = Nothing
                End With
            End Sub
        End Class

        Protected Overrides Sub InitValueExtractor()
            'Init ValueExtractor that extracts property values from an object instance
            Me.ValueExtractor = New LizardLabs.InputOutput.ValueExtractors.ObjectPropertiesValueExtractor(GetType(EventRecordEx))
        End Sub

        Protected Overrides Sub OpenInputImplementation(from As String)
            _fromEntities = _fromSeparatorRegex.Split(from)
            MyBase.RecordIndex = 0
            MyBase.Items = Me.GetEntries().GetEnumerator() ' mybase.items is 
        End Sub
		
		Protected Overrides Sub CloseInputImplementation(abort As Boolean)
            MyBase.CloseInputImplementation(abort)
            ' do we need to close something?
        End Sub

        Private Iterator Function GetEntries() As IEnumerable(Of EventRecordEx)
            Dim rec As New EventRecordEx() 'Store values in this object

            For Each frome As String In _fromEntities
				frome = frome.Trim(_trimChars)
                
				Dim elq As New System.Diagnostics.Eventing.Reader.EventLogQuery(frome, System.Diagnostics.Eventing.Reader.PathType.LogName, Me.AdvancedEventLogQuery) With {
                        .ReverseDirection = Me.ReverseDirection, .TolerateQueryErrors = True
                    }

                Using elr As New System.Diagnostics.Eventing.Reader.EventLogReader(elq)
                    Dim eventInstance As System.Diagnostics.Eventing.Reader.EventRecord = elr.ReadEvent()

                    While eventInstance IsNot Nothing
                        MyBase.RecordIndex += 1
						
						Try

                        'Clear record and map field values
                        rec.Clear()

                        With rec
                            .ActivityId = eventInstance.ActivityId
                            .EventId = eventInstance.Id
                            .Keywords = eventInstance.Keywords
                            .EventType = eventInstance.Level
                            .EventTypeName = eventInstance.LevelDisplayName
                            .LogName = eventInstance.LogName
                            .MachineName = eventInstance.MachineName
                            .Opcode = eventInstance.Opcode
                            .OpcodeName = eventInstance.OpcodeDisplayName
                            .ProcessId = eventInstance.ProcessId
                            .ProviderName = eventInstance.ProviderName
                            .Qualifiers = eventInstance.Qualifiers
                            .RecordId = eventInstance.RecordId
                            .RelatedActivityId = eventInstance.RelatedActivityId
                            .Task = eventInstance.Task
                            .TaskDisplayName = eventInstance.TaskDisplayName
                            .ThreadId = eventInstance.ThreadId
                            .TimeCreated = eventInstance.TimeCreated
                            .Version = eventInstance.Version

                            ' transformed fields
							
							'User  name
                            .UserId = GetSIDString(eventInstance.UserId)
							
							'Message
                            Dim msg As String = Nothing
							Try
								msg = eventInstance.FormatDescription()
							Catch exc As Exception
								msg = "Reading Error: " & exc.Message
							End try
							
                            If Me.FormatMsg Then
                                msg = System.Text.RegularExpressions.Regex.Replace(msg, "\s+", " ") 'replace multiple space chars with single space for better readability
                            End If
                            .Message = msg
							
							'Keywords
                            If eventInstance.KeywordsDisplayNames IsNot Nothing Then
                                Dim sb As New System.Text.StringBuilder()
                                Dim bFirst As Boolean = True
                                For Each o As String In eventInstance.KeywordsDisplayNames
                                    If Not bFirst Then
                                        sb.Append(Me.StringsSep)
                                    End If
									If o IsNot Nothing Then
										sb.Append(o.Replace(Me.StringsSep, "_STRINGS_SEP_")) 'if StringsSep is found in the property it will be replaced to avoid field parse errors
									End If
                                    bFirst = False
                                Next
                                If sb.Length > 0 Then .Keywords = sb.ToString()
                            End If
							
							'Strings
                            If eventInstance.Properties IsNot Nothing Then
                                Dim sb As New System.Text.StringBuilder()
                                Dim bFirst As Boolean = True
                                For Each o As System.Diagnostics.Eventing.Reader.EventProperty In eventInstance.Properties
                                    If Not bFirst Then
                                        sb.Append(Me.StringsSep)
                                    End If
                                    If o.Value IsNot Nothing Then
                                        sb.Append(o.Value.ToString().Replace(Me.StringsSep, "_STRINGS_SEP_")) 'If StringsSep is found In the Property it will be replaced To avoid field parse errors
                                    End If
                                    bFirst = False
                                Next
                                If sb.Length > 0 Then .Strings = sb.ToString()
                            End If
                        End With
						
						'end reading record



                        ' return record to the reader
                        Yield rec
						
						Catch exc as Exception
							If Not Me.IgnoreErrors
								Throw
							End If
						End Try

                        eventInstance = elr.ReadEvent() 'read next


                    End While
                End Using
            Next
        End Function

#End Region

#Region "Helper methods"

        Private Function GetSIDString(sid As System.Security.Principal.SecurityIdentifier) As String
            If sid Is Nothing Then
                Return Nothing
            End If
            Try
                If Me.ResolveSIDs Then
                    Return sid.Translate(GetType(System.Security.Principal.NTAccount)).ToString()
                Else
                    Return sid.Value
                End If
            Catch ex As Exception
                Return ex.Message
            End Try
            Return Nothing
        End Function

#End Region

