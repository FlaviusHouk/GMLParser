using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class Parser
    {
        private GMLCodeGenerator _generator;
        public Parser()
        {
            _generator = new GMLCodeGenerator();
        }

        public string GenerateFileHeader(string name)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#include <stdio.h>\n");
            sb.Append($"void Build{name}(void);\n");

            foreach(string func in _generator.Functions)
            {
                sb.Append($"{func}\n");
            }

            return sb.ToString();
        }
        public string GetBaseIncludes(string fileName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#include <stdio.h>");
            sb.AppendLine("#include <gtk/gtk.h>");
            sb.AppendLine($"#include \"{fileName}.h\"");

            sb.Append("\n\n\n");

            return sb.ToString();
        }

        public string ParseGML(string code, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetBaseIncludes(fileName));

            Node root = BuildNodeTree(code);
            string generated = _generator.GenerateCode(root);

            sb.AppendFormat("{0}\n\n", _generator.Named);

            sb.Append($"void Build{fileName}");
            sb.Append("(void)\n{\n");
            
            sb.Append(generated);

            sb.Append("\n}");

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
                if(rows[i].StartsWith("</"))
                {
                    root = levels.Pop();
                    continue;
                }

                if(rows[i].StartsWith('<'))
                {   
                    if(rows[i].EndsWith('>'))
                        tag.Add(rows[i]); 
                    else
                    {
                        tag = rows.Skip(i).TakeWhile(str => !str.EndsWith(">")).ToList();  
                        tag.Add(rows[Array.IndexOf(rows, tag.Last()) + 1]);
                    }

                    Node node = new Node(tag);

                    if(levels.Count >= 1)
                    {
                        levels.Peek().Children.Add(node);
                    }

                    if(!tag.Last().EndsWith("/>"))
                        levels.Push(node);
                }

                tag.Clear();
            }

            return root;
        }
    }
}