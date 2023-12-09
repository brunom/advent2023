<Query Kind="Program" />

void Main()
{
	Directory.SetCurrentDirectory(@"C:\tools\advent2023\07");
	// 253933213
	// 251617933 too low
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
	static string ordered = "AKQT98765432J";

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
			int Js = cards.Where(x => x == 'J').Count();
			var ordered = cards.Where(x => x != 'J').GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).ToList();
			if (cards.All(ch => ch == 'J'))
				return Type.Five;
			if (ordered[0] + Js == 5)
				return Type.Five;
			if (ordered[0] + Js == 4)
				return Type.Four;
			if (ordered[0] + Js == 3 && ordered[1] == 2)
				return Type.Full;
			if (ordered[0] + Js == 3)
				return Type.Three;
			if (ordered[0] == 2 && ordered[1] + Js == 2)
				return Type.TwoPair;
			if (ordered[0] + Js == 2)
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

