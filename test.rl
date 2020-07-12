namespace System.Test

class Program
{
public:
	var wow:[int]
	var coolInt:int
	var coolObject:Ball

	def Magic :: a:int -> bool
	{
		Add a a (Add 2 3 a) -> coolInt
		ret (coolInt + 5) == 3
	}

private:
	def Add :: a:int, b:int, c:int -> int
	{
		wow - 1 -> wow

		ret a + b / c
	}

	def Test :: int:a, int:b -> void
	{
		else
		{
			a + b -> a
		}
	}

}