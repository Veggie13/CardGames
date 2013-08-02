namespace CardGames
{

    public enum Colour
    {
        Red, Black
    }

    public enum Suit
    {
        None, Hearts, Diamonds, Clubs, Spades
    }

    public enum Face
    {
        Joker,
        Ace = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        _10 = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public enum CardVisibility
    {
        FaceDown, FaceUp, PlayerHand
    }

    public enum DeckOrientation
    {
        FaceDown, FaceUp, TopUp
    }

}