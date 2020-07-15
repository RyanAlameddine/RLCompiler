namespace System.Test

class Program
{
	def Func 
	{
		[x * 4 + 2 for x in [1..3] if x > 2] -> var list:[int]

		#if x > 2
		#{
		#	true -> false
		#}
	}
}