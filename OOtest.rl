#using System is automatically included

namespace System.Test

class Animal
{

public:
	var food:int

	#takes in the amount of food eaten
	def Eat :: foodCount:int -> void
	{
		food + foodCount -> food
	}

	def Damage :: foodCount:int -> void
	{
		food - foodCount -> food
	}
}

class Cat : Animal
{
	var catName:string

	def Cat :: name:string -> cat
	{
		name -> catName
	}

	def Moo { }
}

class Giraffe : Animal
{
	def AttackWithNeck :: other:Giraffe, power:int -> void
	{
		other.Damage power
	}

	def Mutate :: Cat 
	{
		var cat:Cat

		Cat () -> cat
		
		ret cat
	}
}