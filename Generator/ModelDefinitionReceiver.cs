using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator 
{
    public class ModelDefinitionReceiver: ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassesToProcess { get; set; }

        public ModelDefinitionReceiver()
        {
            ClassesToProcess = new List<ClassDeclarationSyntax>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => (a.Name as IdentifierNameSyntax)?.Identifier.ValueText == "GenerateDto" ))
            {
                ClassesToProcess.Add(cds);
            }
        }
    }
}