#region Copyright (C) 2013 EPiServer AB
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
#endregion
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace EPiServer.ComposerMigration
{
    /// <summary>
    /// This class mirrors the Extension methods for deserializing objects.
    /// </summary>
    public class ComposerSerializer
    {
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ComposerSerializer));

        /// <summary>
        ///     Deserialize an object from Xml value
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public virtual T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Warn("The Xml value is null or empty");
                return default(T);
            }
            Logger.Info(xml);
            var sr = new XmlSerializer(typeof(T));
            var xmlReader = new XmlTextReader(new StringReader(xml));
            try
            {
                return (T)sr.Deserialize(xmlReader);
            }
            catch (XmlException xe)
            {
                Logger.Error("Error when deserializing object", xe);
                Logger.Error(string.Format("XML to be deserialized: {0}", xml));
                return default(T);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error("Error when deserializing object", ex);
                Logger.Error(string.Format("XML to be deserialized: {0}", xml));
                return default(T);
            }
            finally
            {
                xmlReader.Close();
            }
        }

        /// <summary>
        ///     Serialize an object and return the Xml value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Serialize<T>(T obj)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer sr = new XmlSerializer(typeof(T));
            string output = string.Empty;

            var writer = new StringWriterUTF8(new StringBuilder());
            try
            {
                sr.Serialize(writer, obj, ns);
                output = writer.ToString();
            }
            catch (XmlException xe)
            {
                Logger.Error("while deserializing FromXML", xe);
            }
            catch (InvalidOperationException ioe)
            {
                Logger.Error("while deserializing FromXML", ioe);
            }
            catch (Exception ex)
            {
                Logger.Error("while deserializing FromXML", ex);
                throw;
            }
            finally
            {
                writer.Close();
            }
            return output;
        }

        private class StringWriterUTF8 : System.IO.StringWriter
        {
            #region Ctor
            /// <summary>
            /// Default constructor. Initializes an instance of <see cref="StringWriterUTF8"/>.
            /// </summary>
            public StringWriterUTF8()
                : base()
            {
            }

            /// <summary>
            /// Initializes an instance of <see cref="StringWriterUTF8"/> with the specified <see cref="StringBuilder"/>.
            /// </summary>
            /// <param name="sb">Specifies an instance of the <see cref="StringBuilder"/> class.</param>
            public StringWriterUTF8(StringBuilder sb)
                : base(sb)
            {
            }

            /// <summary>
            /// Initializes an instance of <see cref="StringWriterUTF8"/> with the specified <see cref="IFormatProvider"/>.
            /// </summary>
            /// <param name="formatProvider">Specifies an instance of the <see cref="IFormatProvider"/> class.</param>
            public StringWriterUTF8(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            /// <summary>
            /// Initializes an instance of <see cref="StringWriterUTF8"/> with the specified <see cref="StringBuilder"/> and <see cref="IFormatProvider"/>.
            /// </summary>
            /// <param name="sb">Specifies an instance of the <see cref="StringBuilder"/> class.</param>
            /// <param name="formatProvider">Specifies an instance of the <see cref="IFormatProvider"/> class.</param>
            public StringWriterUTF8(StringBuilder sb, IFormatProvider formatProvider)
                : base(sb, formatProvider)
            {
            }
            #endregion

            #region Core
            /// <summary>
            /// Overrides the base Encoding property to return UTF-8 encoding.
            /// </summary>
            public override Encoding Encoding
            {
                get
                {
                    return System.Text.Encoding.UTF8;
                }
            }
            #endregion
        }
    }
}
