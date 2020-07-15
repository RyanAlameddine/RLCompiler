# RLCompiler
Tokenizer, Parser, and Compiler for RLang, a strongly typed OO language compiled to CIL created by me

# Rules
 
 Strongly typed, OOP, compiled to CIL
* int [alias for BCL type System.Int32]
* void [alias for BCL type System.Void]
 #
 
* Case sensitive
* No multi-line statements
* Function names, class names, and variable names must be unique
* Main function with [int] parameter is entrypoint

#
* Namespace: namespace Identifier
* Function: def Identifier :: input parameter(s) -> ouput parameter(s)
   												multiple returns is passed as a tuple
                          

#
* Class: class
* Arrays: \[Type\]
* Ranges: \[[0-9]+\-[0-9]+\]
* Types: (int|bool|string)
* Variables: var
* Loops: (while condition|for variable in array)
* Char Literals: '\\?.'
* String Literals: \".*?\"
* Int Literals: [0-9]+
* Identifiers: [a-zA-Z_]\w*
