<Query Kind="Program" />

void Main()
{
	var lines = File.ReadLines("input.txt");
	lines
		.Select(x => x.Split())
		.Select(arr => new { hand = new Hand { cards = arr[0] }, bid = long.Parse(arr[1]), })
		.OrderByDescending(x => x.hand)
		.Zip(Enumerable.Range(1, int.MaxValue), (x, i) => x.bid * i)
		.Sum()
		.Dump();
}

enum Type
{
	Five,
	Four,
	Full,
	Three,
	TwoPair,
	OnePair,
	HighCard,
}

struct Card : IComparable<Card>
{
	public char ch;
	static string ordered = "AKQJT98765432";

	(int, char) Key
	{
		get => (ordered.IndexOf(this.ch), this.ch);
	}

	public int CompareTo(Card other)
	{
		return Key.CompareTo(other.Key);
	}
}

struct Hand : IComparable<Hand>
{
	public string cards;
	public Type Type
	{
		get
		{
			var ordered = cards.GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).ToList();
			if (ordered[0] == 5)
				return Type.Five;
			if (ordered[0] == 4)
				return Type.Four;
			if (ordered[0] == 3 && ordered[1] == 2)
				return Type.Full;
			if (ordered[0] == 3)
				return Type.Three;
			if (ordered[0] == 2 && ordered[1] == 2)
				return Type.TwoPair;
			if (ordered[0] == 2)
				return Type.OnePair;
			return Type.HighCard;
		}
	}
	(Type, Card, Card, Card, Card, Card) Key
	{
		get => (Type,
		new Card { ch = cards[0] },
		new Card { ch = cards[1] },
		new Card { ch = cards[2] },
		new Card { ch = cards[3] },
		new Card { ch = cards[4] });
	}
	public int CompareTo(Hand other)
	{
		return Key.CompareTo(other.Key);
	}
}

