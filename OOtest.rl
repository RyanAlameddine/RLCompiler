#using System is automatically included
using System

namespace Animal.Classifications

class Program
{
	def Main :: param:int -> int
	{
		Cat "hi" -> var coolCat:Cat
		ret 0
	}
}

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
	
public:
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
		Cat "cat" -> var cat:Cat
		
		ret cat
	}
}