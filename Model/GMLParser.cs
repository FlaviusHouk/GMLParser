using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class Parser
    {
        public Parser()
        {}

        public string ParseGML(string code)
        {
            StringBuilder sb = new StringBuilder();

            Node root = BuildNodeTree(code);

            return sb.ToString();
        }

        private Node BuildNodeTree(string code)
        {
            string[] rows = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            Stack<Node> levels = new Stack<Node>();
            Node root = null;
            List<string> tag = new List<string>();

            for(int i = 0; i<rows.Length; i++)
            {
                if(rows[i].StartsWith('<'))
                {
                    if(rows[i].StartsWith("</"))
                    {
                        root = levels.Pop();
                        continue;
                    }
                    
                    if(!rows[i].EndsWith('>'))
                        tag = rows.TakeWhile(str => !str.EndsWith(">")).ToList();
                    else
                        tag.Add(rows[i]);
                    
                    levels.Push(new Node(tag));
                    
                    if(levels.Count > 1)
                    {
                        levels.Peek().Children.Add(new Node(tag));
                    }
                }

                tag.Clear();
            }

            return root;
        }
    }
}