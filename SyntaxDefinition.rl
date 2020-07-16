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

namespace Test

class Program
{
	var x:int
	var coolObject:Ball
	var magicalDelegate:delegate

	def Main :: args:[int] -> void
	{
		Add 1 2 3 -> x

		Add x 2 (Add 1 2 3) -> x

		new Ball "wowowie" -> coolObject
		coolObject.PrintName ()

		#delegate magic
		coolObject.PrintName -> magicalDelegate
		magicalDelegate ()
	}

	def Add :: a:int, b:int, c:int -> int
	{
		ret a + b + c
	}
}

#(new file)

namespace Test

class Ball:Sprite
{

private:

	var n:string
	var x:int
	
internal:

	def Ball :: name:string -> Ball
	{
		if name == "stan"
		{
			throw CodeNotCleanEnoughException "wow"
		}
		name -> n
		5    -> x
	}
	
	def PrintName :: void
	{
		Console.Write (n + x:string)
	}

	def magicalFunction :: void
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
		for val:int in [0..5]
		{
			Console.WriteLine val
		}
	}
}