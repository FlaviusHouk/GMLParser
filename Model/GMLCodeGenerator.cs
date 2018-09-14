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

        public string GenerateCode(Node root)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateCodePrivate(root));

            _counters.Clear();

            sb.Append(ShowVisualElements());

            return sb.ToString();
        }

        public string GenerateCodePrivate(Node root, Node parent = null)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(AnalyzeNode(root, parent));
            sb.AppendLine();

            foreach(Node node in root.Children)
            {
                sb.Append(GenerateCodePrivate(node, root));
            }

            return sb.ToString();
        }

        private string AnalyzeNode(Node node, Node parent = null)
        {
            StringBuilder sb = new StringBuilder();

            GTKObjectRepresentation obj = GetTypeRepresentation(node.NodeName);

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

            node.CodeName = objName;
            _elementsToShow.Push(objName);

            sb.AppendLine(CreateElement(node));

            foreach(var attr in node.ObjectProperties)
            {
                string setter = FindProperty(obj, attr.Key).SetterRepresentation;

                sb.AppendLine($"{string.Format(setter, objName, attr.Value)}");
            }

            if(parent != null)
            {
                if(IsContainer(parent))
                {
                    Property add = FindProperty(parent, "_Add");

                    if(!node.AttachedProperties.All(p => string.Equals(p.Key.Split(':').First(), parent.NodeName)))
                    {
                        throw new Exception("Wrong parent type");
                    }

                    var packArgs = add.GetterRepresentation
                                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(par => par.Trim())
                                       .Skip(2);

                    List<string> values = new List<string>(packArgs.Count());

                    foreach(string prop in packArgs)
                    {
                        values.Add(node.AttachedProperties[$"{parent.NodeName}:{prop}"]);
                    }

                    if(packArgs.Count() > 0)
                    {
                        sb.AppendFormat(add.SetterRepresentation, parent.CodeName, node.CodeName, values[0], values[1], values[2]);
                    }
                    else
                    {
                        sb.AppendFormat(add.SetterRepresentation, parent.CodeName, node.CodeName);
                    }
                    sb.AppendLine();
                }
                else
                {
                    throw new Exception("Attempt to put children in non container element");
                }
            }

            return sb.ToString();
        }

        private Property FindProperty(Node node, string propName)
        {
            GTKObjectRepresentation obj = GetTypeRepresentation(node.NodeName);

            return FindProperty(obj, propName);
        }
        private Property FindProperty(GTKObjectRepresentation obj, string propName)
        {
            Property toRet = null;

            GTKObjectRepresentation curr = obj;
            while(curr != null)
            {
                if(curr.ObjectProperties.Any(p=>string.Compare(p.Name, propName) == 0))
                {
                    toRet = curr.ObjectProperties.First(p=>string.Compare(p.Name, propName) == 0);
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

        private bool IsContainer(Node node)
        {
            bool toRet = false;
            
            string type = node.NodeName;
            while(!string.IsNullOrEmpty(type))
            {
                GTKObjectRepresentation obj = GetTypeRepresentation(type);

                if(obj.ObjectProperties.Any(p => string.Equals(p.Name, "_Add")))
                    return true;
                else
                    type = obj.BaseType;
            }

            return toRet;
        }        

        private GTKObjectRepresentation GetTypeRepresentation(string typeName)
        {
            return _rules.ObjectType
                         .First(o=>string.Compare(typeName, o.TypeName) == 0);  
        }

        private string ShowVisualElements()
        {
            StringBuilder sb = new StringBuilder();

            while(_elementsToShow.Count > 0)
            {
                sb.AppendLine($"gtk_widget_show({_elementsToShow.Pop()});");
            }

            return sb.ToString();
        }

        private string CreateElement(Node node)
        {
            GTKObjectRepresentation obj = GetTypeRepresentation(node.NodeName);
            Property creationProp = FindProperty(obj, "_Creation");

            var packArgs = creationProp.GetterRepresentation
                                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(par => par.Trim());

            List<string> values = new List<string>(packArgs.Count());
            values.Add(node.CodeName);
            foreach (string prop in packArgs)
            {
                values.Add(node.ObjectProperties[prop]);
                node.ObjectProperties.Remove(prop);
            }

            return string.Format(obj.CreationString, values.ToArray());
        }
    }
}