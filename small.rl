namespace System.Test#heyo

class Program
{
	def Add #:: a:int, b:int, c:int -> int
	{
		Add (1*2) 2 3 -> var x:int
		ret a + b * c 
	}

	def Main
	{
		var a:int
		var list:[[int]]#cool comments
	
		Console.WriteLine a
	
		Console.Readline () -> a
	
		[[1..3], [1..5]] -> list
		[[1, a, 3], [1 + 5]] -> var l:[[int]]
		
	}
	
	#yeet
	def Add :: a:int, b:int, c:int -> int
	{
		#ret (a + (b + ((b * c) * c)))
		
		ret a + b / c * d != a + b - b
		
		#ret ((a + (((b / c) * d)) != a + (b - b)))
		#ret (a + ((b * c) * d) + (b + b))
	}
}