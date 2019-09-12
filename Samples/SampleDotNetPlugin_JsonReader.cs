using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using LizardLabs.InputOutput;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Xml;
using LizardLabs;
using LizardLabs.InputOutput;
using LizardLabs.InputOutput.ValueExtractors;
using LizardLabs.LogParser.InputFormats;

// C# 5 features are supported
namespace LplDotNetDataSource
{
    public class JsonInputReaderSample : MultiFileReaderInputContextBase
    {
		private string _JsonPropertyToRead = "";
        public string JsonPropertyToRead 
		{ 
			get
			{
				return _JsonPropertyToRead;
			}
			set
			{
				_JsonPropertyToRead = value;
			}
		}

        public JsonInputReaderSample() : base()
        {
        }

        public class JsonLogEntry : Dictionary<string, object>
        {
        }

        protected override void OpenInputImplementation(string from)
        {
            base.OpenInputImplementation(from);
        }

        protected override void InitValueExtractor()
        {
            // Init value extractor from first record

            ValueExtractor = new DictionaryExtractor(new string[] { }); // init empty first if can't init later

            Newtonsoft.Json.JsonSerializer oJsonSerializer = new Newtonsoft.Json.JsonSerializer();
            foreach (var st in StreamFactory.GetStreams(this.From, this.Recurse)) // returns touplse (stream name as string, Stream object implementaion)
            {
                using (AdvancedTextReader srdr = new AdvancedTextReader(st.Item2, this.TextReaderOptions))
                {
                    using (var m_JsonReader = new Newtonsoft.Json.JsonTextReader(srdr) { SupportMultipleContent = true })
                    {
                        bool bStartRead = string.IsNullOrEmpty(JsonPropertyToRead);
                        while (m_JsonReader.Read())
                        {
                            if (!bStartRead && !(m_JsonReader.TokenType == JsonToken.PropertyName && m_JsonReader.Value == JsonPropertyToRead))
                                continue;
                            else
                                bStartRead = true;

                            if (m_JsonReader.TokenType == JsonToken.StartObject)
                            {
                                // Deserialize 
                                JsonLogEntry logentry = oJsonSerializer.Deserialize<JsonLogEntry>(m_JsonReader);

                                // Set the value extractor from the first JSON record and exit
                                ValueExtractor = new DictionaryExtractor(logentry);

                                break;
                            }
                        }
                    }
                }
            }
        }

        protected System.Collections.Generic.IEnumerable<JsonLogEntry> GetEntries()
        {
            Newtonsoft.Json.JsonSerializer oJsonSerializer = new Newtonsoft.Json.JsonSerializer();
            foreach (System.Tuple<string, System.IO.Stream> st in StreamFactory.GetStreams(From, this.Recurse))
            {
                this.StreamLineNumber = 0;
                this.CurrentStreamName = st.Item1;
                this.RecordIndex = 0;

                using (AdvancedTextReader srdr = new AdvancedTextReader(st.Item2, this.TextReaderOptions))
                {
                    using (var m_JsonReader = new Newtonsoft.Json.JsonTextReader(srdr) { SupportMultipleContent = true })
                    {
                        bool bStartRead = string.IsNullOrEmpty(JsonPropertyToRead);
                        while (m_JsonReader.Read())
                        {
                            if (!bStartRead && !(m_JsonReader.TokenType == JsonToken.PropertyName && m_JsonReader.Value == JsonPropertyToRead))
                                continue;
                            else
                                bStartRead = true;

                            if (m_JsonReader.TokenType == JsonToken.StartObject)
                            {
                                // Additional fields
                                this.StreamLineNumber = m_JsonReader.LineNumber;
                                this.RecordIndex += 1;

                                // Deserialize entries and yield
                                JsonLogEntry le = oJsonSerializer.Deserialize<JsonLogEntry>(m_JsonReader);
                                yield return le;
                            }
                        }
                    }
                }
            }
        }
		
        public override System.Collections.IEnumerator GetEnumerator()
        {
            return GetEntries().GetEnumerator();
        }
    }
}