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
        public XmlSchema GetSchema()
        {
            return null;
        }

        public Property()
        {
        }

        public void ReadXml(XmlReader reader)
        {
            /*reader.Read();

            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Setter"));
            SetterRepresentation = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Getter"));
            GetterRepresentation = serializer.Deserialize(reader) as string;

            serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("PropName"));
            Name = serializer.Deserialize(reader) as string;*/
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

                string[] paramTypes = creation.GetterRepresentation.Split(';');
                
                if(paramTypes.All(param => ObjectProperties.Any(prop=>string.Compare(param, prop.Name) == 0)))
                {}
                else
                {
                    baseStr = string.Format(baseStr, "GTK_WINDOW_TOPLEVEL");
                }

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

            serializer = new XmlSerializer(typeof(List<Property>), new XmlRootAttribute("ObjectProperties"));
            //reader.ReadToDescendant("ObjectProperties");
            var props = serializer.Deserialize(reader) as List<Property>;
            ObjectProperties.AddRange(props);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            XmlSerializer serializer = new XmlSerializer(typeof(string), new XmlRootAttribute("TypeName"));
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