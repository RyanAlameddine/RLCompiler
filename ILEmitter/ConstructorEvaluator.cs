using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RLCodeGenerator
{
    class ConstructorEvaluator : MethodEvaluatorBase
    {
        public ConstructorBuilder ConstructorBuilder { get; }

        public ConstructorEvaluator(ConstructorBuilder constructorBuilder, FunctionHeaderContext header, ClassEvaluator parent) : base(header, parent)
        {
            ConstructorBuilder = constructorBuilder;
        }

        protected override ILGenerator GetGenerator()
        {
            var generator = ConstructorBuilder.GetILGenerator();

            //TODO call base constructor
            generator.Emit(OpCodes.Ldarg_0);    
            generator.Emit(OpCodes.Call, typeof(object).GetConstructors().Single());

            return generator;
        }

        internal static ConstructorEvaluator GenerateConstructor(ConstructorBuilder constructorBuilder, ClassEvaluator parent, string name)
        {
            var header = new FunctionHeaderContext(AccessModifiers.Public);
            header.Children.AddLast(new IdentifierContext() { Identifier = name });
            return new ConstructorEvaluator(constructorBuilder, header, parent);
        }
    }
}
