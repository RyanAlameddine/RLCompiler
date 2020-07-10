using System.Linq;
using System.Text;

namespace RLTokenizer.Scopes
{
    class FunctionHeaderContext : Context
    {
        private bool namePresent  = false;
        private bool colonPresent = false;
        private bool arrowPresent = false;

        public AccessModifiers AccessModifier { get; private set; }

        public string Name { get => ((IdentifierContext)Children.First.Value     ).Identifier; }

        public (string Name, string Type)[] InputVariables
        {
            get => 
                Children.Skip(1).SkipLast(2).Cast<VariableDefinitionContext>()
                .Select((x) => (x.Name, x.Type)).ToArray();
        }

        public string ReturnType { get => ((IdentifierContext)Children.Last.Previous.Value).Identifier; }

        public FunctionHeaderContext(AccessModifiers accessModifier)
        {
            AccessModifier = accessModifier;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "{") return (true, new FunctionBodyContext());

            if (arrowPresent)
            {
                if (token.IsLineOrWhitespace()) return (true, this);
                //add support for multiple returns here
            }
            
            if (colonPresent)
            {
                if (token == "-") 
                    return (false, this);
                if(token == "->")
                {
                    arrowPresent = true;
                    return (true, new IdentifierContext());
                }
                if(previous == ',')
                {
                    return (true, new VariableDefinitionContext(AccessModifiers.Function, ","));
                }
                if (token.IsWhitespace()) return (true, this);
                throw new TokenizationException("Comma not found after function parameter declaration");
            }

            if(Children.Count == 0)
            {
                if (!namePresent)
                {
                    namePresent = true;
                    return (true, new IdentifierContext());
                }
                if (!token.IsLineOrWhitespace()) 
                    throw new TokenizationException("No whitespace found after function declaration");
            }

            if(Children.Count == 1)
            {
                if (token.IsWhitespace()) return (true, this);
                if (token == ":") return (false, this);
                if (token == "::")
                {
                    colonPresent = true;
                    return (true, new VariableDefinitionContext(AccessModifiers.Function, ","));
                }

                throw new TokenizationException("No :: found after function declaration");
            }

            throw new TokenizationException("Failed to complete Function declaration");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Function Header (Name:");
            sb.Append(Name);
            sb.Append(", Return Type: ");
            sb.Append(ReturnType);
            sb.Append(")");

            return sb.ToString();
        }
    }
}