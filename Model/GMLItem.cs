using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace GMLParser.Model
{
    public class Node
    {
        public string NodeName { get; private set; }
        public string CodeName { get; set; }
        public Dictionary<string, string> ObjectProperties { get; } 
                                        = new Dictionary<string, string>();
        
        public Dictionary<string, string> AttachedProperties { get; }
                                        = new Dictionary<string, string>();

        public List<Node> Children { get; } = new List<Node>();
        public Node(IEnumerable<string> tag)
        {
            ParseTag(tag);
        }

        private void ParseTag(IEnumerable<string> tag)
        {
            var chars = tag.First().SkipWhile(c => c == ' ' || c == '<').TakeWhile(c => char.IsLetter(c));
            NodeName = new string(chars.ToArray());

            Regex attributes = new Regex("[ ,\t]{1}[a-z,A-Z,0-9]{1,}[ ]{0,}[=]{1}[ ]{0,}[\"]{1}[a-z,A-Z,0-9,_ ]{0,}[\"]{1}");

            System.Text.StringBuilder tagStr = new System.Text.StringBuilder();

            foreach(string str in tag)
            {
                tagStr.Append($" {str.Trim('\n')} ");
            } 

            IEnumerable<string> attr = attributes.Matches(tagStr.ToString()).Select(m=>m.Value);

            foreach(string attribute in attr)
            {
                string[] parts = attribute.Split('=');
                ObjectProperties.Add(parts[0].Trim(), parts[1].Trim().Trim('"'));
            }

            attributes = new Regex("[a-z,A-Z,0-9]{1,}[:]{1}[a-z,A-Z,0-9]{1,}[ ]{0,}[=]{1}[ ]{0,}[\"]{1}[a-z,A-Z,0-9]{0,}[\"]{1}");

            attr = attributes.Matches(tagStr.ToString()).Select(m=>m.Value);

            foreach(string attribute in attr)
            {
                string[] parts = attribute.Split('=');
                AttachedProperties.Add(parts[0], parts[1].Trim('"'));
            }
        }
    }
}