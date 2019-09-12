#Region "Imports"
	Imports System
	Imports System.Xml
	Imports System.Data
	Imports System.Collections
	Imports System.Collections.Generic
	Imports System.Linq
	Imports Microsoft.VisualBasic
	Imports LizardLabs
	Imports LizardLabs.LogParserLizard
	Imports LizardLabs.LogParser.InputFormats
	Imports LizardLabs.InputOutput
	Imports Newtonsoft.Json
	Imports Newtonsoft.Json.Linq
#End Region
Namespace LplDotNetDataSource

Class LplSurveyGizmoCustomInputPlugin
        Inherits MultiFileReaderInputContextBase 'LPL internal helper class

#Region "Some LPL internal helpers"
        Public Sub New()
            MyBase.New()
            MyBase.IncludeFullRecord = False 'Base calss setting
        End Sub

        Protected Overrides Sub OpenInputImplementation(from As String)
            MyBase.OpenInputImplementation(from)
        End Sub

        Protected Overrides Sub InitValueExtractor()
            'Init value extractor from the class type (LPL internal helpers)
			ValueExtractor = New ValueExtractors.ObjectPropertiesValueExtractor(GetType(GizmodoSurveyFlat))
        End Sub

		'Overrides GetEnumerator
        Public Overrides Function GetEnumerator() As IEnumerator
            Return GetEntries.GetEnumerator()
        End Function
#End Region

#Region "Class from which will be crrate object to store JSON ptoperties (and flat the data for tabular view)"
	
	'TODO: for testing purposes, most data is stored in string fields. It can stored in appropriate data types (integer, datetime, double, etc...) by changes in the parsing method where needed
    
	'You can add fields or remove unecessary fields by commenting them
	Public Class GizmodoSurveyFlat
        'Root object
        Public Property result_ok As String
        Public Property total_count As Integer
        Public Property page As Integer
        Public Property total_pages As Integer
        Public Property results_per_page As Integer


        'Data object
        Public Property DataObjectId As String
        Public Property contact_id As String
        Public Property status As String
        Public Property is_test_data As String
        Public Property date_submitted As String
        Public Property session_id As String
        Public Property language As String
        Public Property date_started As String
        Public Property link_id As String
        Public Property url_variables As String
        Public Property ip_address As String
        Public Property referer As String
        Public Property user_agent As String
        Public Property response_time As String
        Public Property data_quality As String
        Public Property longitude As String
        Public Property latitude As String
        Public Property country As String
        Public Property city As String
        Public Property region As String
        Public Property postal As String
        Public Property dma As String

        ' Survey_Data()
        Public Property SurveyDataId As Integer
        Public Property type As String
        Public Property question As String
        Public Property section_id As Integer
        Public Property answer As String
        Public Property answer_id As Integer
        Public Property shown As String
        Public Property subquestions As String 'At the moment the entire JSON is stored in a string field
    End Class

#End Region

#Region "Actual data parsing method"
        Protected Iterator Function GetEntries() As System.Collections.Generic.IEnumerable(Of GizmodoSurveyFlat)
            Dim oJsonSerializer As New Newtonsoft.Json.JsonSerializer()

			'Public proerties of the type are used later to store values in the temporary object
            Dim Fields As System.Reflection.PropertyInfo() = GetType(GizmodoSurveyFlat).GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

			'Open and read  data streams in FROM 
            For Each st As System.Tuple(Of String, System.IO.Stream) In StreamFactory.GetStreams(From, Me.Recurse)
				'Set some built in fields from base class
                Me.StreamLineNumber = 0
                Me.CurrentStreamName = st.Item1
                Me.StreamRecordIndex = 0


				'Some advanced properties can be uses from base class (LPL internals)
                Using srdr As New AdvancedTextReader(st.Item2, Me.TextReaderOptions)
                    Using m_JsonReader = New Newtonsoft.Json.JsonTextReader(srdr) With {.SupportMultipleContent = True}
						
						Dim le As New GizmodoSurveyFlat 'This object will store temp data while JSON string is parsed

                        Dim path As String
                        Dim tt As JTokenType

                        While m_JsonReader.Read()
                            path = m_JsonReader.Path 'store current JSON path (used later to find the end o flattening)
                            tt = m_JsonReader.TokenType 'current token type

                            If tt = JsonToken.PropertyName Then
                                Dim pname As String = m_JsonReader.Value

                                'Some properties from JSON have different name in flatten object so we need to change pname for some of them (ex. SurveyDataId in JSON is just id - but the datata type can't have two properties with name id)
                                If System.Text.RegularExpressions.Regex.IsMatch(pname, "data\[\d+\].id") Then
                                    pname = "DataObjectId"

									'HACK: since object le stores flat data, this proeprty indicates new DataObject, so after this filed clear all values since it will contains data of a new object
                                    Dim bDeletrAllAfter As Boolean = False
                                    For i As Integer = 0 To Fields.Length - 1
                                        If Fields(i).Name = pname Then
                                            bDeletrAllAfter = True
                                        End If
                                        If bDeletrAllAfter Then Fields(i).SetValue(le, Nothing)
                                    Next
                                ElseIf path.EndsWith("survey_data.id") Then
                                    pname = "SurveyDataId"

                                    'Aimilar as previous hack: clear object values after SurveyDataId
                                    Dim bDeletrAllAfter As Boolean = False
                                    For i As Integer = 0 To Fields.Length - 1
                                        If Fields(i).Name = pname Then
                                            bDeletrAllAfter = True
                                        End If
                                        If bDeletrAllAfter Then Fields(i).SetValue(le, Nothing)
                                    Next
                                End If


                                'If JSON property name exists in the flatt type, get the string value (otherwise will be ignored)
                                Dim fld As System.Reflection.PropertyInfo = Fields.FirstOrDefault(Function(p) p.Name = pname)

                                If fld IsNot Nothing Then
                                    Dim val As Object = Nothing
                                    Try
                                        If Not m_JsonReader.Read() Then
                                            tt = JsonToken.EndObject
                                            Exit While
                                        End If

										Dim o = Nothing

                                        If m_JsonReader.TokenType = JsonToken.StartArray Then
                                            'load array as string
											val = JObject.ReadFrom(m_JsonReader)
                                        ElseIf m_JsonReader.TokenType = JsonToken.StartObject Then
                                            'load entire object as string
                                            val = JObject.Load(m_JsonReader)
                                        Else
                                            'get value as string, but in case of other data types conversion can be made to different data types
                                            val = m_JsonReader.Value
                                        End If
                                    Catch ex As JsonException
                                        'Comment this to ignore exception error messages. For now leave it in test mod
                                        val = ex.Message
                                    End Try
									
									'Convert the object to string (consider converting to appropriate data type)

									val = If(val Is Nothing, Nothing, val.ToString())
									
									val = Convert.ChangeType(val, fld.PropertyType)

                                    'Set the value to flat object
                                    fld.SetValue(le, val)
                                End If
                            End If


                            If m_JsonReader.TokenType = JsonToken.EndObject Then
                                'Test if end of survey object reached using regular expresssion against JSON path. If reached, yield object
                                If System.Text.RegularExpressions.Regex.IsMatch(path, "data\[\d+\].survey_data.\d+") Then
                                    Me.StreamLineNumber = m_JsonReader.LineNumber
                                    StreamRecordIndex += 1

                                    Yield le
                                End If
                            End If
                        End While
                    End Using
                End Using
            Next
        End Function

#End Region

End Class

End Namespace 
