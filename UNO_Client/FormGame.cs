using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Client
{
    public partial class FormGame : Form
    {
        private string _roomName;
        private List<string> _players;
        public FormGame(string roomName, List<string> players)
        {
            InitializeComponent();
            _roomName = roomName;
            _players = players;
        }


        private UnoRoom _room;
        private UnoGameLogic _logic;
        private void FormGame_Load(object sender, EventArgs e)
        {
            _room = new UnoRoom(_roomName);
            _logic = new UnoGameLogic();

            foreach (var name in _players)
                _room.Players.Add(new Player(name));

            _room.StartGame();

            //Sau khi tạo giao diện thì chuyển cmt này về thành code

            //lblTopCard.Text = $"Top Card: {_room.TopCard}";

            //Label này dùng để hiển thị lá bài đang nằm trên bàn (lá bài trên cùng của chồng bài đánh ra).
        }
    }

    public enum UnoColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Wild
    }

    public enum UnoValue
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip, Reverse, DrawTwo,
        Wild, WildDrawFour
    }

    //Class lá bài
    public class UnoCard
    {
        public UnoColor Color { get; set; }
        public UnoValue Value { get; set; }

        public UnoCard(UnoColor color, UnoValue value)
        {
            Color = color;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Color} {Value}";
        }
    }

    //Class cho bộ bài
    public class UnoDeck
    {
        private Stack<UnoCard> _cards;

        public UnoDeck()
        {
            _cards = new Stack<UnoCard>(GenerateDeck());
            Shuffle();
        }

        private List<UnoCard> GenerateDeck()
        {
            var deck = new List<UnoCard>();
            var colors = new[] { UnoColor.Red, UnoColor.Blue, UnoColor.Green, UnoColor.Yellow };

            foreach (var color in colors)
            {
                foreach (UnoValue value in Enum.GetValues(typeof(UnoValue)))
                {
                    if (value == UnoValue.Wild || value == UnoValue.WildDrawFour) continue;
                    deck.Add(new UnoCard(color, value));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                deck.Add(new UnoCard(UnoColor.Wild, UnoValue.Wild));
                deck.Add(new UnoCard(UnoColor.Wild, UnoValue.WildDrawFour));
            }

            return deck;
        }

        private void Shuffle()
        {
            var rnd = new Random();
            _cards = new Stack<UnoCard>(_cards.OrderBy(c => rnd.Next()));
        }

        public UnoCard DrawCard()
        {
            return _cards.Count > 0 ? _cards.Pop() : null;
        }
    }

    //Class dựng cho người chơi
    public class Player
    {
        public string Name { get; set; }
        public List<UnoCard> Hand { get; set; }

        public Player(string name)
        {
            Name = name;
            Hand = new List<UnoCard>();
        }

        public void DrawCard(UnoDeck deck)
        {
            var card = deck.DrawCard();
            if (card != null)
                Hand.Add(card);
        }

        public void PlayCard(UnoCard card)
        {
            Hand.Remove(card);
        }
    }



    //Class gamelogic(luật chơi)
    public class UnoGameLogic
    {
        public bool CanPlay(UnoCard card, UnoCard topCard)
        {
            return card.Color == topCard.Color ||
                   card.Value == topCard.Value ||
                   card.Color == UnoColor.Wild;
        }

        public void ApplySpecialEffect(UnoCard card, List<Player> players, ref int currentIndex, ref bool isClockwise)
        {
            switch (card.Value)
            {
                case UnoValue.Skip:
                    currentIndex = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    break;
                case UnoValue.Reverse:
                    isClockwise = !isClockwise;
                    break;
                case UnoValue.DrawTwo:
                    int nextIndex = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    players[nextIndex].DrawCard(new UnoDeck());
                    players[nextIndex].DrawCard(new UnoDeck());
                    break;
                case UnoValue.WildDrawFour:
                    int next = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    for (int i = 0; i < 4; i++)
                        players[next].DrawCard(new UnoDeck());
                    break;
            }
        }

        private int GetNextPlayerIndex(int totalPlayers, int currentIndex, bool isClockwise)
        {
            return isClockwise
                ? (currentIndex + 1) % totalPlayers
                : (currentIndex - 1 + totalPlayers) % totalPlayers;
        }
    }


    public enum RoomState
    {
        Waiting,
        Playing,
        Finished
    }

    //Class phòng chơi
    public class UnoRoom
    {
        public string RoomName { get; set; }
        public List<Player> Players { get; set; }
        public UnoDeck Deck { get; set; }
        public UnoCard TopCard { get; set; }
        public RoomState State { get; set; }

        public UnoRoom(string roomName)
        {
            RoomName = roomName;
            Players = new List<Player>();
            Deck = new UnoDeck();
            State = RoomState.Waiting;
        }

        public void StartGame() //Hàm chia bài
        {
            State = RoomState.Playing;
            TopCard = Deck.DrawCard();

            foreach (var player in Players)
            {
                for (int i = 0; i < 7; i++)
                    player.DrawCard(Deck);
            }
        }

        public void EndGame()
        {
            State = RoomState.Finished;
        }
    }




}
