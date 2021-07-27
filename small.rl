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

		ball.AddIfLess 1 4 -> var aa:int # 5
		ball.AddIfLess 2 4 -> var ab:int # 6
		ball.AddIfLess 3 4 -> var ac:int # 3
		ball.AddIfLess 4 4 -> var ad:int # 4
		ball.AddIfLess 5 1 -> var ae:int # 5

		Console.WriteLine ""

		Console.WriteLine (aa.ToString ())
		Console.WriteLine (ab.ToString ())
		Console.WriteLine (ac.ToString ())
		Console.WriteLine (ad.ToString ())
		Console.WriteLine (ae.ToString ())

		8 / 2 * (2 + 2) -> magicNumber:int

		Console.WriteLine (magicNumber.ToString ())

	}
}

class Ball
{
public:
	def Ball :: a:string -> void
	{
		Add 123 24 -> var b:int

		Console.WriteLine ((b.ToString ()).Insert ((b.ToString ()).get_Length ()) "!")
	}

	def Add :: a:int, b:int -> int
	{
		ret a + b * 1 + b * b * a
	}

	def AddIfLess :: a:int, b:int -> int
	{
		var num:int
		if a < 3
		{
			a + b -> num
		}
		else
		{
			a -> num
		}

		ret num
	}
}