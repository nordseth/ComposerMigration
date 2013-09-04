using System;
using System.Xml;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    /// <summary>
    /// This class exists to support deserializing of the ContentFunctionAnchor Extension class
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "a")]
    public class ComposerContentBlockAnchor
    {
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }

        [XmlText]
        public string Name { get; set; }
    }
}
