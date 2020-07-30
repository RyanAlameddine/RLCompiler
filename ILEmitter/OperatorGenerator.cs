using RLParser;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ILEmitter
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
                default:
                    throw new CompileException("Unable to find operator " + opID);
            }
            generator.Emit(opCode);
        }
    }
}