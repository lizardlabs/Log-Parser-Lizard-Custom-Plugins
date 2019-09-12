'SAMPLE VB DotNet Plugin. 
'This reads line by line from text files in a folder (you can use wildcards) or from web 

#Region "Private Enums"

		'Actual fields in this example
        Private Enum TableFields
            RecordNo = 0 
            FileName 
			LineNo
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
        Private m_reader As LizardLabs.LogParser.InputFormats.Helpers.MultiFileReader 
#End Region


#Region "Properties"
#End Region


#Region "ILogParserInputContext" 

'These are methods that implements the ILogParserInputContext interface (each plugin must have all of these.)
        Public Sub OpenInput(from As String) 
            m_currentRecord = 0

			'This is a helper reader class for reading text files with wildcards recursively from subfolders. Also reads compressed .gz archives, 
			'web content (if from starts with HTTP://), console app standard output (if starts with STDOUT://) and opens Choose Filses dialog if FROM is CHOOSE_FILES
			'Constructor parameters: from As String, LinesToSkip As Integer (skip a few lines from each file), Codepage As Integer, Recurse As Integer (level of recursion, -1 unlimited), EntireFileIsOneRecord As Boolean (if true, reads entire file else just one line)
			
			m_reader = New LizardLabs.LogParser.InputFormats.Helpers.MultiFileReader(from, 0, 0, -1, False) 
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
                    Case TableFields.LineNo
                        Return m_reader.CurrentLineNum
                    Case TableFields.TextLine
                        Return m_currLine 'TODO: this is just a sample. You can parse the text line here
               End Select

            Return Nothing
        End Function

        Public Sub CloseInput(abort As Boolean) 
			m_reader.Close()
        End Sub

#End Region
