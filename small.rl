using System

namespace rLang.Test
#heyo

class Program
{
	var a:int

	def Add :: bool #:: a:int, b:int, c:int -> int
	{
		5:string -> var str:string
		Math (1*2) 2 3 -> var hmm:bool

		str:int -> var x:int

		if x < 3
		{
			ret false
		}

		ret a * x == 1
	}
	
	def Main
	{
		"hi" -> var b:string
		var list:[[int]]#cool comments
	
		Console.WriteLine b

		Console.ReadLine () -> b
	
		[[1..3], [1..5]] -> list
		[[1, a, 3], [1 + 5]] -> var l:[[int]]
		
	}
	
	#yeet
	def Math :: b:int, c:int, d:int -> bool
	{
		#ret (a + (b + ((b * c) * c)))
		
		ret a + b / c * d != a + b - b
		
		#ret ((a + (((b / c) * d)) != a + (b - b)))
		#ret (a + ((b * c) * d) + (b + b))
	}
}
