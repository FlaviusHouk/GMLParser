using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class GMLCodeGenerator
    {
        private GMLRules _rules;
        private Stack<string> _elementsToShow;
        private Dictionary<string, int> _counters;
        public GMLCodeGenerator()
        {
            _rules = GMLRules.GetRules();
            _counters = new Dictionary<string, int>();
        }


        private bool _firstIn = true;
        public string GenerateCode(Node root)
        {
            _firstIn = false;

            StringBuilder sb = new StringBuilder();
            
            sb.Append(AnalyzeNode(root));

            foreach(Node node in root.Children)
            {
                sb.Append(GenerateCode(node));
            }

            if(_firstIn)
            {
                _counters.Clear();
                ShowVisualElements();
            }

            _firstIn = true;

            return sb.ToString();
        }

        private string AnalyzeNode(Node node)
        {
            StringBuilder sb = new StringBuilder();

            GTKObjectRepresentation obj = _rules.ObjectType
                                                .First(o=>string.Compare(node.NodeName, o.TypeName) == 0);

            string objName = string.Empty;
            if(!_counters.ContainsKey(obj.TypeName))
            {
                objName = $"{obj.TypeName.ToLower()}{0}";
                _counters.Add(obj.TypeName, 1);
            }
            else
            {
                objName = $"{obj.TypeName.ToLower()}{_counters[obj.TypeName]}";
                _counters[obj.TypeName]++;
            }

            _elementsToShow.Push(objName);

            sb.AppendLine(string.Format(obj.CreationString, objName));

            foreach(var attr in node.ObjectProperties)
            {
                Property prop = obj.ObjectProperties.First(p=>string.Compare(p.Name, attr.Key) == 0);

                sb.AppendLine($"{objName}->{string.Format(prop.SetterRepresentation, attr.Key)}");
            }

            return sb.ToString();
        }

        private string ShowVisualElements()
        {
            StringBuilder sb = new StringBuilder();

            while(_elementsToShow.Count > 0)
            {
                sb.AppendLine($"{_elementsToShow.Pop()}->Show();");
            }

            return sb.ToString();
        }
    }
}