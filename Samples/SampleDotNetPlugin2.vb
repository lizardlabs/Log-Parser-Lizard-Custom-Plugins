'SAMPLE VB DotNet Plugin. 
'This reads line by line from text files in a folder (you can use wildcards) or read from HTTP address

#Region "Private Enums"

		'Actual fields in this example
        Private Enum TableFields
            RecordNo = 0 
            FileName 
            TextLine 
        End Enum
		
		'Field types enum (for method GetFieldType()
		Friend Enum FieldType
			[Integer] = 1
			Real = 2
			[String] = 3
			Timestamp = 4
		End Enum

#End Region

		
#Region "Attributes"
        Private m_currentRecord As Integer = 0
		Private m_currLine As String = ""
        Private m_reader As LizardLabs.LogParser.InputFormats.Helpers.MultiFileReader 'Helper reader class for reading text files with wildcards and also recursively from subfolders 
#End Region


#Region "Properties"
#End Region


#Region "ILogParserInputContext" 

'These are methods that implements the ILogParserInputContext interface (each plugin must have all of these.)
        Public Sub OpenInput(from As String) 
            m_currentRecord = 0
			m_reader = New LizardLabs.LogParser.InputFormats.Helpers.MultiFileReader(from, 0, 0, -1, False) 'Constructor parameters: from As String, LinesToSkip As Integer, Codepage As Integer, Recurse As Integer, EntireFileIsOneRecord As Boolean
        End Sub
		
        Public Function GetFieldCount() As Integer 
            Return [Enum].GetValues(GetType(TableFields)).Length
        End Function

        Public Function GetFieldName(index As Integer) As String 
            Return CType(index, TableFields).ToString("G") 'From Enum names
        End Function

        Public Function GetFieldType(index As Integer) As Integer
            Dim lpType As FieldType = FieldType.String 'Default type is strign
					
            Select Case CType(index, TableFields)
                Case TableFields.RecordNo
                    lpType = FieldType.Integer
            End Select

            Return lpType
        End Function
		
        Public Function ReadRecord() As Boolean 
            m_currLine = m_reader.GetNextLine() 'Read next line from the current file or first line from the next file in list
            
			If m_currLine Is Nothing Then
                Return False 'This is the end of reading
            End If

            m_currentRecord += 1
            Return True
        End Function
		
        Public Function GetValue(index As Integer) As Object
             Select Case CType(index, TableFields)
                  Case TableFields.RecordNo
                        Return m_currentRecord
                    Case TableFields.FileName
                        Return m_reader.CurrentFilename
                    Case TableFields.TextLine
                        Return m_currLine 'TODO: this is just a sample. You can parse the text line here
               End Select

            Return Nothing
        End Function

        Public Sub CloseInput(abort As Boolean) 
			_reader.Close()
        End Sub

#End Region
