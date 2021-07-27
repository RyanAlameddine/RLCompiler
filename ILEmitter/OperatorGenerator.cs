using RLParser;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RLCodeGenerator
{
    class OperatorGenerator
    {
        public static void GenerateOperator(string opID, ILGenerator generator)
        {
            OpCode opCode;
            switch (opID)
            {
                case "+":
                    opCode = OpCodes.Add;
                    break;
                case "-":
                    opCode = OpCodes.Sub;
                    break;
                case "*":
                    opCode = OpCodes.Mul;
                    break;
                case "/":
                    opCode = OpCodes.Div;
                    break;
                case ">":
                    opCode = OpCodes.Cgt;
                    break;
                case "<":
                    opCode = OpCodes.Clt;
                    break;
                case "<=":
                    generator.Emit(OpCodes.Cgt);
                    generator.Emit(OpCodes.Ldc_I4_0);
                    opCode = OpCodes.Ceq;
                    break;
                case ">=":
                    generator.Emit(OpCodes.Clt);
                    generator.Emit(OpCodes.Ldc_I4_0);
                    opCode = OpCodes.Ceq;
                    break;
                case "==":
                    opCode = OpCodes.Ceq;
                    break;
                case "!=":
                    generator.Emit(OpCodes.Ceq);
                    generator.Emit(OpCodes.Ldc_I4_0);
                    opCode = OpCodes.Ceq;
                    break;
                default:
                    throw new CompileException("Unable to find operator " + opID);
            }
            generator.Emit(opCode);
        }
    }
}