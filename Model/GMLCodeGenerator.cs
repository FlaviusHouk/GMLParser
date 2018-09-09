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
            _elementsToShow = new Stack<string>();
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
                string setter = FindSetter(obj, attr.Key);

                sb.AppendLine($"{objName}->{string.Format(setter, attr.Value)}");
            }

            return sb.ToString();
        }

        private string FindSetter(GTKObjectRepresentation obj, string propName)
        {
            string toRet = null;

            GTKObjectRepresentation curr = obj;
            while(curr != null)
            {
                if(curr.ObjectProperties.Any(p=>string.Compare(p.Name, propName) == 0))
                {
                    toRet = curr.ObjectProperties.First(p=>string.Compare(p.Name, propName) == 0).SetterRepresentation;
                    break;
                }
                else
                {
                    if(string.IsNullOrEmpty(curr.BaseType))
                    {
                        throw new Exception("There is no such type in collection.");
                    }

                    curr = _rules.ObjectType.First(o=>string.Compare(o.TypeName, curr.BaseType) == 0);
                }
            }

            return toRet;
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