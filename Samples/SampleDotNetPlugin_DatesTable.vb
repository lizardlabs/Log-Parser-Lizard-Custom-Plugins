'SAMPLE VB DotNet Plugin. 
'This is a implementation similar to "Dates Table Input Format"

#Region "Private Enums"

		'Actual fields in this example
        Private Enum UltimateDatesTableFields
            'Basic
            DatesTableRowNo = 0 'Int32
            [Date] ' 'DateTime
            DateKey
            DateInt 'Int32
            YearKey 'Int32
            QuarterOfYear 'Int32
            MonthOfYear 'Int32
            DayOfMonth 'Int32
            MonthName 'String
            MonthInCalendar 'String
            QuarterInCalendar 'String
            DayOfWeekName 'String
            DayInWeek       'Int32
            LongDate 'String
            ShortDate 'String
            IsoDate 'String
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
        Private m_currentDate As Date
        Private m_TheCulture As System.Globalization.CultureInfo
#End Region


#Region "Properties"
        Public Property DateFrom As Date = DateTime.Today.AddYears(-1)
        Public Property DateTo As Date = DateTime.Today
        <System.ComponentModel.DefaultValue("current")> _
        Public Property Culture As String = "current"
#End Region


#Region "ILogParserInputContext" 

'These are methods that implements the ILogParserInputContext interface (each plugin must have all of these.)

        Public Sub CloseInput(abort As Boolean) 
        End Sub

        Public Function GetFieldCount() As Integer 
            Return [Enum].GetValues(GetType(UltimateDatesTableFields)).Length
        End Function

        Public Function GetFieldName(index As Integer) As String 
            Return CType(index, UltimateDatesTableFields).ToString("G")
        End Function

        Public Function GetFieldType(index As Integer) As Integer
            Dim lpType As FieldType = FieldType.String
            Select Case CType(index, UltimateDatesTableFields)
                Case UltimateDatesTableFields.DatesTableRowNo
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.Date
                    lpType = FieldType.Timestamp
                Case UltimateDatesTableFields.DateKey
                    lpType = FieldType.String
                Case UltimateDatesTableFields.DateInt
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.YearKey
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.QuarterOfYear
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.MonthOfYear
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.DayOfMonth
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.MonthName 
					lpType = FieldType.String
                Case UltimateDatesTableFields.MonthInCalendar
					lpType = FieldType.String
                Case UltimateDatesTableFields.QuarterInCalendar
					lpType = FieldType.String
                Case UltimateDatesTableFields.DayOfWeekName
					lpType = FieldType.String
                Case UltimateDatesTableFields.DayInWeek
                    lpType = FieldType.Integer
                Case UltimateDatesTableFields.LongDate
                    lpType = FieldType.String
                Case UltimateDatesTableFields.ShortDate
                    lpType = FieldType.String
                Case UltimateDatesTableFields.IsoDate
                    lpType = FieldType.String
            End Select

            Return lpType
        End Function

        Public Function GetValue(index As Integer) As Object
			With m_currentDate
                Select Case CType(index, UltimateDatesTableFields)
                    Case UltimateDatesTableFields.DatesTableRowNo
                        Return m_currentRecord
                    Case UltimateDatesTableFields.Date
                        Return m_currentDate
                    Case UltimateDatesTableFields.DateKey
                        Return m_currentDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                    Case UltimateDatesTableFields.DateInt
                        Return .Year * 10000 + .Month * 100 + .Day
                    Case UltimateDatesTableFields.YearKey
                        Return .Year()
                    Case UltimateDatesTableFields.QuarterOfYear
					    Return CType(((.Month - 1) / 3) + 1, Integer)
                    Case UltimateDatesTableFields.MonthOfYear
                        Return .Month
                    Case UltimateDatesTableFields.DayOfMonth
                        Return .Day
                    Case UltimateDatesTableFields.MonthName 'String
                        Return .ToString("MMMM", m_TheCulture)
                    Case UltimateDatesTableFields.MonthInCalendar 'String
                        Return .ToString("yyyy MM", System.Globalization.CultureInfo.InvariantCulture)
                    Case UltimateDatesTableFields.QuarterInCalendar 'String
                        Return .ToString("yyyy Q") & CType(((.Month - 1) / 3) + 1, Integer).ToString()
                    Case UltimateDatesTableFields.DayOfWeekName 'String
                        Return .ToString("dddd", m_TheCulture)
                    Case UltimateDatesTableFields.DayInWeek
                        Return .DayOfWeek + 1
                    Case UltimateDatesTableFields.LongDate
                        Return .ToString("D", m_TheCulture)
                    Case UltimateDatesTableFields.ShortDate
                        Return .ToString("d", m_TheCulture)
                    Case UltimateDatesTableFields.IsoDate
                        Return .ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
               End Select
            End With

            Return Nothing
        End Function

        Public Sub OpenInput(from As String) 
            If from.ToUpper <> "DEFAULT" Then
                Dim splits() As String = from.Split(",")
                If splits.Count >= 1 Then DateFrom = DateTime.Parse(splits(0).Trim(New Char() {" ", "'"}))
                If splits.Count >= 2 Then DateTo = DateTime.Parse(splits(1).Trim(New Char() {" ", "'"}))
            End If

            If Culture.ToUpper.Trim = "CURRENT" Then
                Me.m_TheCulture = System.Globalization.CultureInfo.CurrentCulture
            Else
                Me.m_TheCulture = New System.Globalization.CultureInfo(Culture.Trim)
            End If

            m_currentDate = DateFrom.AddDays(-1) 'Take one day at start since on is always added in ReadRecord
            m_currentRecord = 0
        End Sub

        Public Function ReadRecord() As Boolean 
            If m_currentDate >= DateTo Then
                Return False 'Finish reading
            End If

            'Go to next row from 
            m_currentRecord += 1
            m_currentDate = m_currentDate.AddDays(1)
            Return True

        End Function

#End Region
