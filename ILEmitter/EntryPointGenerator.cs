using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RLCodeGenerator
{
    class EntryPointGenerator
    {
        public static MethodInfo Generate(ModuleBuilder moduleBuilder, ConstructorInfo constructor, MethodInfo entryPoint)
        {
            var typeBuilder = moduleBuilder.DefineType("<RLEntryProgram>");
            var methodBuilder = typeBuilder.DefineMethod("EntryPointMain", MethodAttributes.Static);
            var ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Newobj, constructor);
            ilGenerator.Emit(OpCodes.Call, entryPoint);
            ilGenerator.Emit(OpCodes.Ret);

            typeBuilder.CreateType();
            return methodBuilder;
        }
    }
}
