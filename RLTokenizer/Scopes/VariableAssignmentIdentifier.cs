namespace RLTokenizer.Scopes
{
    class VariableAssignmentIdentifier : IdentifierContext
    {
        public override string ToString() => "Expression value assignment to: " + Identifier;
    }
}