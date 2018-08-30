using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GMLParser.Model
{
    public class GMLCodeGenerator
    {
        private GMLRules _rules;
        public GMLCodeGenerator()
        {
            _rules = GMLRules.GetRules();
        }


        public string GenerateCode(Node root)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(AnalyzeNode(root));

            foreach(Node node in root.Children)
            {
                sb.Append(AnalyzeNode(node));
            }

            return sb.ToString();
        }

        private string AnalyzeNode(Node node)
        {
            StringBuilder sb = new StringBuilder();

            

            return sb.ToString();
        }
    }
}