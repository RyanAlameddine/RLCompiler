using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEmitter
{
    class MethodEvaluator : MethodEvaluatorBase
    {
        public MethodBuilder MethodBuilder { get; }

        public MethodEvaluator(MethodBuilder methodBuilder, FunctionHeaderContext header, ClassEvaluator parent) :base(header, parent)
        {
            MethodBuilder = methodBuilder;
        }

        protected override ILGenerator GetGenerator() => MethodBuilder.GetILGenerator();

    }
}
