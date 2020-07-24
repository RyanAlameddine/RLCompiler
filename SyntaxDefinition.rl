# Rules
# 
# Strongly typed, OOP, compiled to CIL
#	int [alias for BCL type System.Int32]
#   void [alias for BCL type System.Void]
# 

# 
# Case sensitive
# No multi-line statements
# Function names, class names, and variable names must be unique
# Main function with [int] parameter is entrypoint
#

#
# Namespace: namespace Identifier
# Function: def Identifier :: input parameter(s) -> ouput parameter(s)
#   												multiple returns is passed as a tuple
# Class: class
# Arrays: \[Type\]
# Ranges: \[[0-9]+\-[0-9]+\]
# Types: (int|bool|string)
# Variables: var
# Loops: (while condition|for variable in array)
# Char Literals: '\\?.'
# String Literals: \".*?\"
# Int Literals: [0-9]+
# Identifiers: [a-zA-Z_]\w*

# List Comprehensions: [output for identifier in list if condition]

namespace Test

using System

class Program
{
	var x:int
	var coolObject:Ball
	#delegate MathFunc :: int, int -> int

	def Main :: args:[int] -> void
	{
		Add 1 2 3 -> x
		
		Add x 2 (Add 1 2 3) -> x
		
		Ball "stan but in sphere form" -> coolObject

		coolObject.PrintName (Add (x + 1) 2 3)

		#[x * 2 for x in [1..5] if x / 2 < 5] -> var y:[int]
		[num * 2 for num in [1..5] if num / 2 < x] -> var y:[int] 
		
		#coolObject.PrintName -> magicalDelegate
		#magicalDelegate () 
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

	#this might need to be added later on
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