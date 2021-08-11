        Inherits EnumerableInputContext

' Sample custom plugin for reading Windows Event Logs using EvtxECmd parser (from  Eric Zimmerman) - (Beta)
' https://github.com/EricZimmerman/evtx
' By using this plugin you are accepting the EvtxECmd license and all other dll libraries included (MIT): https://github.com/EricZimmerman/evtx/blob/master/LICENSE
' The latest version can be found here https://github.com/lizardlabs/Log-Parser-Lizard-Custom-Plugins/


#Region "Constructors and destructors"
        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Public properties"

		'IMPORTANT CHANGE THIS PATH TO THE MAPS FOLDER. LATEST MAPS HERE: https://github.com/EricZimmerman/evtx/tree/master/evtx/Maps
        Public Property MapsDirectory As String = "C:\Visual Studio Projects\TestAndLearn\evtx\evtx\Maps"

#End Region

#Region "Attributes"

        Private Shared _fromSeparatorRegex As New System.Text.RegularExpressions.Regex("\s*[;,]\s*(?!(?<=(?:^|[;,])\s*""""(?:[^""""]|""""""""|\\"""")*[;,]\s*)(?:[^""""]|""""""""|\\"""")*""""\s*(?:[;,]|$))", System.Text.RegularExpressions.RegexOptions.Compiled)
        Private Shared _trimChars As Char() = {" ", vbTab, vbCr, vbLf, """", "'"}
        Private _fromEntities As String() = Nothing

#End Region

#Region "Parsing code"

        Protected Overrides Sub InitValueExtractor()
            'Init ValueExtractor that extracts property values from an object instance
            Me.ValueExtractor = New LizardLabs.InputOutput.ValueExtractors.ObjectPropertiesValueExtractor(GetType(evtx.EventRecord))
        End Sub

        Protected Overrides Sub OpenInputImplementation(from As String)
            _fromEntities = _fromSeparatorRegex.Split(from)
            MyBase.RecordIndex = 0
            MyBase.Items = Me.GetEntries().GetEnumerator() ' mybase.items is 
			
			evtx.EventLog.LoadMaps(System.IO.Path.GetFullPath(MapsDirectory))
        End Sub
		
		Protected Overrides Sub CloseInputImplementation(abort As Boolean)
            MyBase.CloseInputImplementation(abort)
            ' do we need to close something?
        End Sub

        Private Iterator Function GetEntries() As IEnumerable(Of evtx.EventRecord)
				
		
            For Each frome As String In _fromEntities
				frome = frome.Trim(_trimChars)
				Using fileS As New System.IO.FileStream(frome, System.IO.FileMode.Open, System.IO.FileAccess.Read)
  		
					dim evt = new evtx.EventLog(fileS)
                    
					For Each rec As evtx.EventRecord In evt.GetEventRecords()
						Yield rec
					Next
				
				End Using
            Next
        End Function

#End Region

#Region "Helper methods"



#End Region
