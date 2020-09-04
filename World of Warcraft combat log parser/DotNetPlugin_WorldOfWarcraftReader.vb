        Inherits MultiFileReaderInputContextBase

        ' Description: 
        '   World of Warcraft (WoW) combat log parser, reader, analyzer.
        '
        '   Inspired by this reddit https://www.reddit.com/r/CompetitiveWoW/comments/48z0gl/quick_access_to_any_data_from_your_combat_log/
        '
        '   Pluggin For Log Parser Lizard (log parsing and data analysis tool): http://www.lizard-labs.com/log_parser_lizard.aspx
        '
        ' Sample queries:
        '
        '   Select Case TOP 100 * FROM 'E:\World of Warcraft\Logs\CombatLogParser\ExampleCombatLog.txt'
        ' 
        '   Select Case top 1000
        '   destGUID, 
        '   destName, 
        '   MIN(timestamp) as firstSeen, 
        '   MAX(timestamp) as lastSeen, 
        '   DIV( ROUND(MUL( TO_REAL(SUB( MAX(timeStamp), MIN(timeStamp))) ,10)) ,10) as SecondsAlive 
        '   FROM 'E:\World of Warcraft\Logs\CombatLogParser\ExampleCombatLog.txt' 
        '   -- WHERE destName = 'Dungeoneer's Training Dummy' 
        '   GROUP BY destGUID, destName 
        '   ORDER BY firstSeen ASC
        '
        Public Class WoWLogEntry
            Inherits Dictionary(Of String, Object)
        End Class

        Dim arrFieldNames(77) As String
        Dim arrCurrentRecord(77) As String


        Public Sub New()
            MyBase.New()
            MyBase.IncludeFullRecord = False

            arrFieldNames(0) = "timeStamp"
            arrFieldNames(1) = "event"
            arrFieldNames(2) = "sourceGUID"
            arrFieldNames(3) = "sourceName"
            arrFieldNames(4) = "sourceFlags"
            arrFieldNames(5) = "sourceRaidFlags"
            arrFieldNames(6) = "destGUID"
            arrFieldNames(7) = "destName"
            arrFieldNames(8) = "destFlags"
            arrFieldNames(9) = "destRaidFlags"
            arrFieldNames(10) = "spellId"
            arrFieldNames(11) = "spellName"
            arrFieldNames(12) = "spellSchool"
            arrFieldNames(13) = "resourceActor"
            arrFieldNames(14) = "resourceActorOwner"
            arrFieldNames(15) = "hitPoints"
            arrFieldNames(16) = "maxHitPoints"
            arrFieldNames(17) = "attackPower"
            arrFieldNames(18) = "spellPower"
            arrFieldNames(19) = "resolve"
            arrFieldNames(20) = "resourceType"
            arrFieldNames(21) = "resourceAmount"
            arrFieldNames(22) = "maxResourceAmount"
            arrFieldNames(23) = "xPosition"
            arrFieldNames(24) = "yPosition"
            arrFieldNames(25) = "itemLevel"
            arrFieldNames(26) = "amount"
            arrFieldNames(27) = "overkill"
            arrFieldNames(28) = "school"
            arrFieldNames(29) = "resisted"
            arrFieldNames(30) = "blocked"
            arrFieldNames(31) = "absorbed"
            arrFieldNames(32) = "critical"
            arrFieldNames(33) = "glancing"
            arrFieldNames(34) = "crushing"
            arrFieldNames(35) = "multistrike"
            arrFieldNames(36) = "environmentalType"
            arrFieldNames(37) = "overhealing"
            arrFieldNames(38) = "powerType"
            arrFieldNames(39) = "extraAmount"
            arrFieldNames(40) = "absorbGUID"
            arrFieldNames(41) = "absorbName"
            arrFieldNames(42) = "absorbFlags"
            arrFieldNames(43) = "absorbRaidFlags"
            arrFieldNames(44) = "absorbSpellId"
            arrFieldNames(45) = "absorbSpellName"
            arrFieldNames(46) = "absorbSpellSchool"
            arrFieldNames(47) = "missType"
            arrFieldNames(48) = "isOffhand"
            arrFieldNames(49) = "amountMissed"
            arrFieldNames(50) = "extraSpellId"
            arrFieldNames(51) = "extraSpellName"
            arrFieldNames(52) = "extraSchool"
            arrFieldNames(53) = "auraType"
            arrFieldNames(54) = "failedType"
            arrFieldNames(55) = "effect1"
            arrFieldNames(56) = "effect2"
            arrFieldNames(57) = "effect3"
            arrFieldNames(58) = "effect4"
            arrFieldNames(59) = "effect5"
            arrFieldNames(60) = "effect6"
            arrFieldNames(61) = "effect7"
            arrFieldNames(62) = "effect8"
            arrFieldNames(63) = "effect9"
            arrFieldNames(64) = "effect10"
            arrFieldNames(65) = "effect11"
            arrFieldNames(66) = "effect12"
            arrFieldNames(67) = "effect13"
            arrFieldNames(68) = "encounterID"
            arrFieldNames(69) = "encounterName"
            arrFieldNames(70) = "difficultyID"
            arrFieldNames(71) = "raidSize"
            arrFieldNames(72) = "endStatus"
            arrFieldNames(73) = "itemId"
            arrFieldNames(74) = "itemName"
            arrFieldNames(75) = "importStatus"
            arrFieldNames(76) = "importFailureReason"
            arrFieldNames(77) = "importFailureInput"
        End Sub

        Protected Overrides Sub OpenInputImplementation(from As String)
            MyBase.OpenInputImplementation(from)
        End Sub

        Protected Overrides Sub InitValueExtractor()
            Dim de As New LizardLabs.InputOutput.ValueExtractors.DictionaryExtractor(arrFieldNames)
            de.m_valueTypes(0) = GetType(DateTime)
            ValueExtractor = de
        End Sub

        Private Sub RecognizeEvent(strEvent, fieldCount, ByRef intResult, ByRef strFailReason)
            ' check the event against our list of recognized event
            ' and the number of arugments
            ' returns 
            '    intResult -> 0 = event recognized & field count correct
            '                 1 = event not recognized
            '                 2 = event recognized, but field count incorrect
            '
            '    strFailReason  = if intResult non-zero, reason for failure
            '
            ' initially, assume field count incorrect
            intResult = 2
            strFailReason = "Wrong number of fields for this event"

            Select Case strEvent
        'if we recognize and event & the field count is correct,
        'we'll set the correct return values
                Case "RANGE_DAMAGE", "DAMAGE_SPLIT", "SPELL_DAMAGE", "SPELL_PERIODIC_DAMAGE"
                    If fieldCount = 34 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "ENVIRONMENTAL_DAMAGE"
                    If fieldCount = 32 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SWING_DAMAGE", "SWING_DAMAGE_LANDED"
                    If fieldCount = 31 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_PERIODIC_HEAL", "SPELL_HEAL"
                    If fieldCount = 29 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_DRAIN"
                    If fieldCount = 27 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_PERIODIC_ENERGIZE", "SPELL_ENERGIZE"
                    If fieldCount = 26 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_CAST_SUCCESS"
                    If fieldCount = 24 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_ABSORBED"
                    If (fieldCount = 16) Or (fieldCount = 19) Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_MISSED", "SPELL_PERIODIC_MISSED", "RANGE_MISSED"
                    If (fieldCount = 14) Or (fieldCount = 15) Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SWING_MISSED"
                    If (fieldCount = 11) Or (fieldCount = 12) Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_DISPEL", "SPELL_STOLEN", "SPELL_AURA_BROKEN_SPELL"
                    If fieldCount = 15 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_INTERRUPT"
                    If fieldCount = 14 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_AURA_REMOVED_DOSE", "SPELL_AURA_APPLIED_DOSE", "SPELL_AURA_APPLIED", "SPELL_AURA_REFRESH", "SPELL_AURA_REMOVED", "SPELL_AURA_BROKEN"
                    'these records have to have at least 13 fields (an upperbound of 12)
                    'but can have an optional number of "effects" following (up to 13 optional effect parameters)

                    If (fieldCount >= 12) And (fieldCount <= 25) Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_CAST_FAILED"
                    If fieldCount = 12 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "SPELL_CAST_START", "SPELL_SUMMON", "SPELL_CREATE", "SPELL_INSTAKILL", "SPELL_RESURRECT"
                    If fieldCount = 11 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "ENCHANT_APPLIED", "ENCHANT_REMOVED"
                    If fieldCount = 11 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "UNIT_DIED", "PARTY_KILL", "UNIT_DESTROYED"
                    If fieldCount = 8 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "ENCOUNTER_END"
                    If fieldCount = 5 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case "ENCOUNTER_START"
                    If fieldCount = 4 Then
                        intResult = 0
                        strFailReason = ""
                    End If

                Case Else
                    'we didn't recognize the event
                    intResult = 1
                    strFailReason = "Event not recognized"
            End Select
        End Sub

        Private Sub MapRecordFields(strEvent As String, ByRef arrRawInput As String())
            'maps record fields into the correct index in arrCurrent Record
            'based on the strEvent
            '
            'no need to checks on strEvent or bounds checking on the arrRawInput array
            'those checks happen prior to this subroutine being called

            Select Case strEvent
                Case "RANGE_DAMAGE", "DAMAGE_SPLIT", "SPELL_DAMAGE", "SPELL_PERIODIC_DAMAGE"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapResourceInfo(arrRawInput, 12)
                    MapDamage(arrRawInput, 25)

                Case "ENVIRONMENTAL_DAMAGE"

                    MapSourceDest(arrRawInput, 1)
                    MapResourceInfo(arrRawInput, 9)
                    MapDamage(arrRawInput, 23)

                Case "SWING_DAMAGE", "SWING_DAMAGE_LANDED"

                    MapSourceDest(arrRawInput, 1)
                    MapResourceInfo(arrRawInput, 9)
                    MapDamage(arrRawInput, 22)

                Case "SPELL_PERIODIC_HEAL", "SPELL_HEAL"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapResourceInfo(arrRawInput, 12)

                    ' map remaining fields
                    ' amount	overhealing	absorbed	critical	multistrike
                    arrCurrentRecord(26) = arrRawInput(25)
                    arrCurrentRecord(37) = arrRawInput(26)
                    arrCurrentRecord(31) = arrRawInput(27)
                    arrCurrentRecord(32) = arrRawInput(28)
                    arrCurrentRecord(35) = arrRawInput(29)


                Case "SPELL_DRAIN"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapResourceInfo(arrRawInput, 12)

                    ' map remaining fields
                    ' amount	powerType	extraAmount
                    arrCurrentRecord(26) = arrRawInput(25)
                    arrCurrentRecord(38) = arrRawInput(26)
                    arrCurrentRecord(39) = arrRawInput(27)

                Case "SPELL_PERIODIC_ENERGIZE", "SPELL_ENERGIZE"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapResourceInfo(arrRawInput, 12)

                    ' map remaining fields
                    ' amount	powerType
                    arrCurrentRecord(26) = arrRawInput(25)
                    arrCurrentRecord(38) = arrRawInput(26)

                Case "SPELL_CAST_SUCCESS"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapResourceInfo(arrRawInput, 12)


                Case "SPELL_ABSORBED"

                    If UBound(arrRawInput) = 19 Then
                        MapSourceDest(arrRawInput, 1)
                        MapSpellIdNameSchool(arrRawInput, 9)
                        MapAbsorb(arrRawInput, 12)

                    ElseIf UBound(arrRawInput) = 16 Then
                        'melee swings don't include attacker ability info
                        MapSourceDest(arrRawInput, 1)
                        MapAbsorb(arrRawInput, 9)

                    End If

                Case "SPELL_MISSED", "SPELL_PERIODIC_MISSED", "RANGE_MISSED"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)

                    ' map remaining fields
                    ' missType	isOffhand	multistrike	amountMissed
                    arrCurrentRecord(47) = arrRawInput(12)
                    arrCurrentRecord(48) = arrRawInput(13)
                    arrCurrentRecord(35) = arrRawInput(14)

                    If (UBound(arrRawInput) = 15) Then
                        arrCurrentRecord(49) = arrRawInput(15)

                    End If

                Case "SWING_MISSED"

                    MapSourceDest(arrRawInput, 1)
                    ' map remaining fields
                    ' missType	isOffhand	multistrike amount
                    arrCurrentRecord(47) = arrRawInput(9)
                    arrCurrentRecord(48) = arrRawInput(10)
                    arrCurrentRecord(35) = arrRawInput(11)

                    If (UBound(arrRawInput) = 12) Then
                        arrCurrentRecord(49) = arrRawInput(12)

                    End If

                Case "SPELL_DISPEL", "SPELL_STOLEN", "SPELL_AURA_BROKEN_SPELL"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapExtraSpell(arrRawInput, 12)

                    ' map remaining fields
                    ' auraType
                    arrCurrentRecord(53) = arrRawInput(15)

                Case "SPELL_INTERRUPT"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    MapExtraSpell(arrRawInput, 12)

                Case "SPELL_AURA_REMOVED_DOSE", "SPELL_AURA_APPLIED_DOSE", "SPELL_AURA_APPLIED", "SPELL_AURA_REFRESH", "SPELL_AURA_REMOVED", "SPELL_AURA_BROKEN"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)
                    ' map remaining field
                    ' auraType	
                    arrCurrentRecord(53) = arrRawInput(12)

                    ' check for optional info
                    MapOptionalEffectInfo(arrRawInput, 13)

                Case "SPELL_CAST_FAILED"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)

                    ' map remaining fields
                    ' failedType
                    arrCurrentRecord(54) = arrRawInput(12)

                Case "SPELL_CAST_START", "SPELL_SUMMON", "SPELL_CREATE", "SPELL_INSTAKILL", "SPELL_RESURRECT"

                    MapSourceDest(arrRawInput, 1)
                    MapSpellIdNameSchool(arrRawInput, 9)

                Case "ENCHANT_APPLIED", "ENCHANT_REMOVED"

                    MapSourceDest(arrRawInput, 1)

                    ' map remaining fields
                    ' spellName	itemId	itemName
                    arrCurrentRecord(11) = arrRawInput(9)
                    arrCurrentRecord(73) = arrRawInput(10)
                    arrCurrentRecord(74) = arrRawInput(11)

                Case "UNIT_DIED", "PARTY_KILL", "UNIT_DESTROYED"

                    MapSourceDest(arrRawInput, 1)

                Case "ENCOUNTER_END"
                    MapEncounterInfo(arrRawInput, 1)
                    ' map remaining fields
                    ' endStatus
                    arrCurrentRecord(72) = arrRawInput(5)

                Case "ENCOUNTER_START"
                    MapEncounterInfo(arrRawInput, 1)

            End Select
        End Sub

        Private Sub MapSourceDest(ByRef arrRawInput As String(), intStartIndex As Integer)
            ' maps the Source & Dest Fields into arrCurrent Record
            Dim intIndex

            For intIndex = 0 To 7
                arrCurrentRecord(2 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub

        Private Sub MapSpellIdNameSchool(arrRawInput, intStartIndex)
            ' maps the SpellId, SpellName, and SpellSchool into arrCurrent Record
            Dim intIndex

            For intIndex = 0 To 2
                arrCurrentRecord(10 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub

        Private Sub MapResourceInfo(arrRawInput, intStartIndex)
            ' maps the Resource Actor information to arrCurrentRecord
            Dim intIndex

            For intIndex = 0 To 12
                arrCurrentRecord(13 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub

        Private Sub MapDamage(arrRawInput, intStartIndex)
            ' maps Damage information to arrCurrentRecord
            Dim intIndex

            For intIndex = 0 To 9
                arrCurrentRecord(26 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub

        Private Sub MapAbsorb(arrRawInput, intStartIndex)
            'map Absorb information to arrCurrentRecord
            Dim intIndex

            For intIndex = 0 To 6
                arrCurrentRecord(40 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
            arrCurrentRecord(26) = arrRawInput(intStartIndex + 7)
        End Sub

        Private Sub MapOptionalEffectInfo(arrRawInput, intStartIndex)
            'check to see if we have any optional effect info
            'could be zero, could be up to 13 additional fields
            Dim intEffectCount
            Dim intIndex

            intEffectCount = UBound(arrRawInput) - intStartIndex
            If (intEffectCount >= 0) Then
                For intIndex = 0 To intEffectCount
                    arrCurrentRecord(55 + intIndex) = arrRawInput(intStartIndex + intIndex)
                Next
            End If
        End Sub

        Private Sub MapExtraSpell(arrRawInput, intStartIndex)
            'map Extra Spell information to arrCurrentRecord
            Dim intIndex

            For intIndex = 0 To 2
                arrCurrentRecord(50 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub

        Private Sub MapEncounterInfo(arrRawInput, intStartIndex)
            'map Encounter information to arrCurrentRecord
            Dim intIndex
            For intIndex = 0 To 3
                arrCurrentRecord(68 + intIndex) = arrRawInput(intStartIndex + intIndex)
            Next
        End Sub


        Private Function ParseEventParameters(ByVal unsplitParameters As String) As String()
            Dim dataList As List(Of String) = New List(Of String)()
            Dim index As Integer = 0
            Dim inquote As Boolean = False
            Dim startIndex As Integer = 0

            While index <= unsplitParameters.Length

                If index = unsplitParameters.Length Then
                    dataList.Add(unsplitParameters.Substring(startIndex, index - startIndex))
                    Exit While
                End If

                If unsplitParameters(index) = """"c Then
                    inquote = Not inquote
                ElseIf unsplitParameters(index) = ","c Then

                    If Not inquote Then
                        Dim s As String = unsplitParameters.Substring(startIndex, index - startIndex)
                        If s(0) = """"c AndAlso s(s.Length - 1) = """"c Then s = s.Substring(1, s.Length - 2)
                        dataList.Add(s)
                        startIndex = index + 1
                    End If
                End If

                index += 1
            End While

            Return dataList.ToArray()
        End Function
        Protected Iterator Function GetEntries() As System.Collections.Generic.IEnumerable(Of WoWLogEntry)
            For Each st As System.Tuple(Of String, System.IO.Stream) In LizardLabs.InputOutput.StreamFactory.GetStreams(From, Me.Recurse)
                Me.StreamLineNumber = 0
                Me.CurrentStreamName = st.Item1
                Me.StreamRecordIndex = 0

                Using srdr As New System.IO.StreamReader(st.Item2)
                    While Not srdr.EndOfStream
                        Dim strRawInput As String = srdr.ReadLine()

                        If strRawInput Is Nothing Then Exit While

                        For i As Integer = 0 To arrCurrentRecord.Length - 1
                            arrCurrentRecord(i) = Nothing
                        Next

                        Dim arrFieldsToProcess() As String = ParseEventParameters(strRawInput)

                        Dim timeStamp As DateTime = DateTime.Now.Date

                        Try
                            Dim timeStampStr = arrFieldsToProcess(0).Substring(0, 18).Trim()
                            Dim strEvent As String = arrFieldsToProcess(0).Substring(18).Trim()

                            timeStamp = DateTime.ParseExact(timeStampStr, "M/d HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)


                            'check the event -> do we recognize the event & do we have the right number of fields for this event?
                            Dim intEventResult As Integer
                            Dim strFailReason As String
                            RecognizeEvent(strEvent, arrFieldsToProcess.Length - 1, intEventResult, strFailReason)

                            If (intEventResult = 0) Then
                                'everything checks out, map the import fields to the output and save results
                                MapRecordFields(strEvent, arrFieldsToProcess)
                                arrCurrentRecord(1) = strEvent
                                arrCurrentRecord(75) = "OK"
                            Else
                                'we had an event check failure -> save the reason why
                                arrCurrentRecord(75) = "ImportError"
                                arrCurrentRecord(76) = strFailReason
                                arrCurrentRecord(77) = strRawInput
                            End If
                        Catch ex As Exception
                            'we had an event check failure -> save the reason why
                            arrCurrentRecord(75) = "ImportError"
                            arrCurrentRecord(76) = ex.Message
                            arrCurrentRecord(77) = strRawInput
                        End Try

                        'save row number and file name for each row we process -> whether successful or not
                        Me.StreamLineNumber += 1
                        Me.StreamRecordIndex += 1

                        ' Flag the log file as having more data to process.

                        Dim wowE As New WoWLogEntry

                        wowE("timeStamp") = timeStamp

                        For i As Integer = 1 To arrFieldNames.Length - 1
                            wowE(arrFieldNames(i)) = arrCurrentRecord(i)
                        Next

                        Yield wowE

                    End While

                End Using
            Next
        End Function


        Public Overrides Function GetEnumerator() As IEnumerator
            Return GetEntries.GetEnumerator()
        End Function
