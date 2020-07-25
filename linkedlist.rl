using System

class LinkedList
{
    var head:Node
    def LinkedList :: val:int -> LinkedList
    {
        Node val -> head
    }

    def AddLast :: val:int -> void
    {
        head -> var current:Node
        if head == ()
        {
            Node val -> head
            ret ()
        }

        while current.next != ()
        {
            current.next -> current
        }

        Node val -> current.next
    }

    def AddFirst :: val:int -> void
    {
        Node val -> var newHead:Node
        head -> newHead.next

        newHead -> head
    }

    def Contains :: val:int -> bool
    {
        head -> var current:Node
        while current != ()
        {
            if current.val == val
            {
                ret true
            }
            current.next -> current
        }
        ret false
    }
}

class Node
{
public:
    var val:int
    var next:Node

    def Node :: value:int -> Node
    {
        value -> val
        () -> next
    }
}