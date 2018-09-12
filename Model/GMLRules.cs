using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    [XmlRoot("Property")]
    public class Property// : IXmlSerializable
    {
        [XmlElement("PropName")]
        public string Name { get; set; }
        
        [XmlElement("Setter")]
        public string SetterRepresentation { get; set; }

        [XmlElement("Getter")]
        public string GetterRepresentation { get; set; }

        public Property()
        {
        }
    }

    public class GTKObjectRepresentation : IXmlSerializable
    {
        public string TypeName { get; private set; }

        public string BaseType { get; private set; }

        [XmlArray("ObjectProperties"), XmlArrayItem("Property",typeof(Property))]
        public List<Property> ObjectProperties { get; }
            = new List<Property>();

        public string CreationString
        {
            get
            {
                Property creation = ObjectProperties
                                 .First(prop=>string.Compare("_Creation", prop.Name) == 0);

                string baseStr = creation.SetterRepresentation;

                StringBuilder sb = new StringBuilder();

                sb.Append($"{TypeName}*");
                sb.Append(" {0}");
                sb.Append(baseStr);

                return sb.ToString();
            }
        }

        public GTKObjectRepresentation()
        {
            
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("TypeName"));
            TypeName = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("BaseType"));
            BaseType = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(List<Property>), new XmlRootAttribute("ObjectProperties"));
            var props = serializer.Deserialize(reader) as List<Property>;
            ObjectProperties.AddRange(props);

            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("TypeName"));
            serializer.Serialize(writer, TypeName);

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("BaseType"));
            serializer.Serialize(writer, TypeName);

            serializer = new XmlSerializer(typeof(List<Property>), new XmlRootAttribute("ObjectProperties"));
            serializer.Serialize(writer, ObjectProperties, ns);
        }
    }

    public class GMLRules : IXmlSerializable
    {
        public static GMLRules GetRules()
        {
            GMLRules toRet = null;
            
            using (System.IO.FileStream fs = System.IO.File.Open("GMLRules.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GMLRules));
                toRet = serializer.Deserialize(fs) as GMLRules;
            }

            return toRet;
        }

        [XmlArray("Objects"), XmlArrayItem(typeof(GTKObjectRepresentation))]
        public List<GTKObjectRepresentation> ObjectType { get; }
            = new List<GTKObjectRepresentation>();

        public GMLRules()
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            XmlSerializer serializer = new XmlSerializer(typeof(List<GTKObjectRepresentation>), new XmlRootAttribute("Objects"));
            ObjectType.AddRange(serializer.Deserialize(reader) as List<GTKObjectRepresentation>);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            XmlSerializer serializer = new XmlSerializer(typeof(List<GTKObjectRepresentation>), new XmlRootAttribute("Objects"));
            serializer.Serialize(writer, ObjectType, ns);
        }
    }

}