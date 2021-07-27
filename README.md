# RLCompiler
Tokenizer, Parser, and Compiler for rLang (.rl extension), a strongly typed OO language compiled to CIL created by me.
This repository also includes a vscode extension and a Language Server (see below)

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
* Conditionals: (if condition|elif condition|else)
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

```python
namespace Good.Old.Times

#wow such good code
class TestClass
{
    #function called Func
    #takes two ints as parameters and returns a list of lists of int
    def Func :: a:int, b:int -> [[int]]
    {
        var list:[int]
        [x * 4 + 2 for x in [1..4] if GreaterThanTwo x] -> list
        #this evaluates to [14, 18]
        
        [list, [1, 2, 3 a], [b]] -> var megaList:[[int]]
        
        ret megaList
    }
    
    def GreaterThanTwo :: a:int -> bool
    {
        ret a > 2
    }
}
```

```python
namespace Test

using System

class Program
{
	var x:int
	var coolObject:Ball
	#NOTE: DELEGATES NOT YET IMPLEMENTED
	delegate MathFunc :: int, int -> int

	def Main :: args:[int] -> void
	{
		Add 1 2 3 -> x
		
		Add x 2 (Add 1 2 3) -> x
		
		Ball "stan but in sphere form" -> coolObject

		coolObject.PrintName (Add (x + 1) 2 3)

		#calling function on base class
		coolObject.Draw ()

		#[x * 2 for x in [1..5] if x / 2 < 5] -> var y:[int]
		[num * 2 for num in [1..5] if num / 2 < x] -> var y:[int] 
		
		coolObject.PrintName -> magicalDelegate
		magicalDelegate () 
	}

	def Add :: a:int, b:int, c:int -> int
	{
		ret a + b + c
	}
}

class Ball : Sprite
{

private:

	var n:string
	var x:int
	
internal:

	def PrintName :: a:int -> void
	{
		Console.Write (n + x:string)
	}

public:

	def magicalFunction :: void#hi there
	{
		var array:[int]
		[1, 3, 4] -> array
		[0..5] -> array

		var i:int 
		
		while i < list.Length
		{
			Console.WriteLine (list.get i)
			i + 1 -> i
		}
	}

	def evenMoreMagicalFunction
	{
		for val:int in [x * 4 + n for x in [1..3] if x > 2]
		{
			Console.WriteLine val
		}
	}

	def Ball :: name:string -> Ball
	{
		name -> n
	}
}

class Sprite
{
public:
	
	def Draw 
	{
		#some cool stuff in here
	}
}
```
