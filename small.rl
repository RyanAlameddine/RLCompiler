namespace Small.Test

class Program
{
	var privateBool:bool

public:

	def Main
	{
		Second 3
	}

	def Second :: coolInt:int -> void
	{
		var x:int
		coolInt -> x

		true -> privateBool

		var scopedBool:bool
		privateBool -> scopedBool

		var ball:Ball
		Ball "hello" -> ball

		"hi" -> var str:string

		Console.WriteLine str
	}
}

class Ball
{
public:
	def Ball :: a:string -> void
	{
		Add 1 2 -> var b:int
		Console.WriteLine (b.ToString ())
	}

	def Add :: a:int, b:int -> int
	{
		ret a + b * 1 + b * b * a
	}
}