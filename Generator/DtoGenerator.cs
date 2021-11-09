using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generator 
{
    [Generator]
    public class DtoGenerator: ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // take 2 - proper property iteration
            context.RegisterForSyntaxNotifications(() => new ModelDefinitionReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {            
            // take 1: static text just to get the point
            /*var text = new StringBuilder();
            text.AppendLine(@"namespace test_santa_api
{
    public class CreateChildDto 
    {
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string City {get;set;}
        public string Motto {get;set;}
        public bool IsNaughty {get;set;}

        public Child ToEntity() 
        {
            return new Child {
                FirstName = FirstName,
                LastName = LastName,
                City = City,
                Motto = Motto,
                IsNaughty = IsNaughty
            };
        }
    }   
}");
            context.AddSource($"ChildDto.cs", SourceText.From(text.ToString(), Encoding.UTF8) );*/

            // take 2 - proper property iteration
            var syntaxReceiver = (ModelDefinitionReceiver)context.SyntaxReceiver;
            foreach (var classToProcess in syntaxReceiver.ClassesToProcess)
            {
                var namespaceDeclaration = classToProcess.Parent as NamespaceDeclarationSyntax;
                // copy existing namespace imports
                var fileDeclaration = namespaceDeclaration.Parent as CompilationUnitSyntax;
                var usings = namespaceDeclaration.Usings.Union(fileDeclaration.Usings).Select(u => u.ToString());
                var text = usings.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s));

                // start namespace 
                text.AppendLine($"namespace {(classToProcess.Parent as NamespaceDeclarationSyntax).Name} {{");
                text.AppendLine($"\tclass Create{classToProcess.Identifier}Dto {{");

                // loop over properties and generate two fragments - entity mapping and property declaration
                var properties = classToProcess.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)
                                                                   && m.Modifiers.Any(mm => mm.ValueText == "public")).ToList();

                var propertyDeclarations = new StringBuilder();
                var entityMapping = new StringBuilder($"public {classToProcess.Identifier} ToEntity() {{");
                entityMapping.AppendLine($"return new {classToProcess.Identifier} {{");
                var needsTrimming = false;
                foreach (PropertyDeclarationSyntax p in properties)
                {
                    if (p.Identifier.ToString() == "Id") continue; //suppose we don't want a field with this name. Can make it driven by attributes, but it's a bit more involved so check out my blog for recipe

                    propertyDeclarations.AppendLine($"public {p.Type} {p.Identifier} {{get;set;}}"); // effectively copy property declaration
                    entityMapping.AppendLine($"{p.Identifier} = {p.Identifier},");
                    needsTrimming = true;
                }
                if (needsTrimming) entityMapping.Length -= 3; // trim trailing comma for last item
                entityMapping.AppendLine("};"); // close object assignment
                entityMapping.AppendLine("}"); // close function body

                // combine the parts together
                text.AppendLine(propertyDeclarations.ToString()).AppendLine(entityMapping.ToString());

                text.AppendLine("}"); //close class
                text.AppendLine("}"); //close namespace
                // and add to compilation
                context.AddSource($"Create{classToProcess.Identifier}Dto.cs", SourceText.From(text.ToString(), Encoding.UTF8));
            }
        }

        
    }
}