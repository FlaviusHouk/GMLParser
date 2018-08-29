using System;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class Node
    {
        public string NodeName { get; private set; }
        public Dictionary<string, string> ObjectProperties { get; } 
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
        }
    }
}