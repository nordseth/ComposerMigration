using System;
using System.Xml;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    /// <summary>
    /// This class exists to support deserializing of the TemplatePageSetting Extension class
    /// </summary>
    [XmlRoot(Namespace = "", ElementName = "templatepagesetting")]
    [Serializable]
    public class ComposerTemplatePageSetting
    {
        [XmlAttribute(Namespace = "", AttributeName = "name")]
        public virtual string Name{ get; set; }

        [XmlAttribute(Namespace = "", AttributeName = "description")]
        public virtual string Description { get; set; }

        [XmlAttribute(Namespace = "", AttributeName = "imageurl")]
        public virtual string ImageUrl { get; set; }
    }
}
