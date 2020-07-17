# RLCompiler
Tokenizer, Parser, and Compiler for rLang (.rl extension), a strongly typed OO language compiled to CIL created by me

# Rules
 
* Case sensitive
* No multi-line statements
* Function names, class names, and variable names must be unique
* Function calls do not require parenthesis (unless the function has no parameters)
* Main function with \[int] parameter is entrypoint

#
* Namespace: namespace Identifier
* Function: def Identifier :: input parameter, input parameter2, etc -> ouput parameter(s)

#
* Class: class
* Arrays: \[Type\]
* Ranges: \[\[0-9]+\-\0-9]+\]
* Types: (int|bool|string)
* Variables: var
* Loops: (while condition|for variable in array)
* Char Literals: '\\?.'
* String Literals: \".*?\"
* Int Literals: \[0-9]+
* Identifiers: \[a-zA-Z_]\w*

* List Comprehensions: \[{output} for {identifier} in {list} if {condition}]

# VSCode

Also contains the rLang language server protocol implemented in C# a vscode extension (found in the /rlang directory) implemented in typescript which adds the following features:

 * Comments
 * Auto-closing pairs: (), [], {}, "", ''
 * Syntax Highlighting
 * Basic Intellisense
 * Snippets for common code blocks
     * e.g. lc -> \[{output} for {identifier} in {list} if {condition}]
 * Abstract Syntax Tree Visualizer for parsed rLang in the vscode explorer that updates in realtime
     * includes jumpTo button to navigate through code from the perspective of the syntax tree
 * Diagnostics (error reports) which shows the little red squiggles we all love to see under our code


# Example Code

Some example files can be found in this repository (see anything with the .rl extension)

```scala
namespace Good.Old.Times

#wow such good code
class TestClass
{
    def Func :: b:int -> [int]
    {
        [x * 4 + 2 for x in [1..4] if GreaterThanTwo x] -> var list:[int]
        
        # returns [14, 18]
        ret list
    }
    
    def GreaterThanTwo :: a:int -> bool
    {
        ret a > 2
    }
}
```
