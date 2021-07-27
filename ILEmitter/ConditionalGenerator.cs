using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RLCodeGenerator
{
    class ConditionalGenerator
    {
        public static Label? Generate(ConditionalExpressionContext condition, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent, ExpressionGenerator expressionGenerator, Label? endOfIfChain)
        {
            Label? exitLabel = null;
            if (condition.hasCondition)
            {
                expressionGenerator.Generate(condition.Children.First.Value, table, generator, variables, parent);
            }

            Label? ifLabel = null;
            switch (condition.Statement)
            {
                case "if":
                    ifLabel = generator.DefineLabel();
                    exitLabel = generator.DefineLabel();
                    generator.Emit(OpCodes.Brfalse, ifLabel.Value);
                    break;
                case "else":
                    break;
            }

            //all children of the scope
            foreach(var child in condition.Children.First.Next.Value.Children)
            {
                parent.GenerateStatement(child, generator, table);
            }

            if(ifLabel != default)
            {
                generator.Emit(OpCodes.Br, exitLabel.Value);
                generator.MarkLabel(ifLabel.Value);
            }

            return exitLabel;
        }
    }
}