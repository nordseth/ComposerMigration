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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    /// <summary>
    /// This class exists to support deserializing of the ContentFunctionData Composer class
    /// </summary>
    [XmlRoot("cf")]
    [Serializable]
    public class ComposerContentFunction : IComposerContent
    {
        private List<ComposerContentArea> _areas;
        private IEnumerable<Guid> _visitorGroups;

        [XmlAttribute("index")]
        public virtual int Index { get; set; }

        [XmlAttribute("guid")]
        public virtual Guid Guid { get; set; }

        [XmlAttribute("vgcid")]
        public virtual Guid VisitorGroupContainerID { get; set; }

        [XmlIgnore]
        public virtual string Language { get; set; }

        /// <summary>
        /// This property will be set by the ComposerContentMap when the Personalization structure is transformed
        /// </summary>
        [XmlIgnore]
        public virtual IEnumerable<Guid> VisitorGroups
        {
            get { return _visitorGroups ?? Enumerable.Empty<Guid>(); }
            set { _visitorGroups = value; }
        }

        /// <summary>
        /// This property will be set by the ComposerContentMap when the Personalization structure is transformed
        /// </summary>
        [XmlIgnore]
        public virtual string PersonalizationGroup { get; set; }

        [XmlElement("ca")]
        public virtual List<ComposerContentArea> ContentAreas
        {
            get { return _areas ?? (_areas = new List<ComposerContentArea>()); }
            set { _areas = value; }
        }
    }
}