using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class Property : IXmlSerializable
    {
        public string Name { get; private set; }
        public string SetterRepresentation { get; private set; }
        public string GetterRepresentation { get; private set; }
        public XmlSchema GetSchema()
        {
            return null;
        }

        public Property()
        {
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Setter"));
            SetterRepresentation = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Getter"));
            GetterRepresentation = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("PropName"));
            Name = serializer.Deserialize(reader) as string;
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Setter"));
            serializer.Serialize(writer, SetterRepresentation, ns);

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Getter"));
            serializer.Serialize(writer, GetterRepresentation, ns);

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("PropName"));
            serializer.Serialize(writer, Name, ns);
        }
    }

    public class GTKObjectRepresentation : IXmlSerializable
    {
        public string TypeName { get; private set; }

        [XmlArray("ObjectProperties"), XmlArrayItem(typeof(Property))]
        public List<Property> ObjectProperties { get; }
            = new List<Property>();

        public GTKObjectRepresentation()
        {
            
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Property>), new XmlRootAttribute("ObjectProperties"));
            ObjectProperties.AddRange(serializer.Deserialize(reader) as List<Property>);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Property>), new XmlRootAttribute("ObjectProperties"));
            serializer.Serialize(writer, ObjectProperties, ns);
        }
    }

    public class GMLRules : IXmlSerializable
    {
        public static GMLRules GetRules()
        {
            GMLRules toRet = null;
            
            using (System.IO.FileStream fs = System.IO.File.Open("GMLRules.xml", System.IO.FileMode.Open))
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