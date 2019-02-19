using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class Board
    {
        private Tile[,] board;

        public XYCoordinates MaxCoordinates { get; }
        public Board(Canvas canvas)
        {

            board = new Tile[15, 17];
            MaxCoordinates = new XYCoordinates(board.GetLength(1) * 50, board.GetLength(0) * 50);
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Image image = new Image
                    {
                        Height = 50,
                        Width = 50,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };
                    Canvas.SetTop(image, i * 50);
                    Canvas.SetLeft(image, j * 50);
                    canvas.Children.Add(image);
                    board[i, j] = (i % 2 != 0 && j % 2 != 0) ? new Wall(image) : new Tile(image);
                }
            }
        }

        public Tile[,] GetBoard()
        {
            return board;
        }

        public bool PlaceBomb(Player bombOwner, int bombStr, out Bomb bomb)
        {
            bomb = null;
            int column = bombOwner.GetCenter().X / 50;
            int row = bombOwner.GetCenter().Y / 50;
            if (!(board[row, column] is Bomb))
            {
                board[row, column] = bomb = new Bomb(board[row, column].GetImage, bombOwner);
                return true;
            }
            return false;
        }

        public bool IsMovePossible(XYCoordinates CenterCoordinates)
        {
            // out of bounds check
            if (CenterCoordinates.Y - 25 < 0 || CenterCoordinates.X - 25 < 0 || CenterCoordinates.Y + 25 > MaxCoordinates.Y || CenterCoordinates.X + 25 > MaxCoordinates.X)
                return false;

            (Tile TopLeftTile, Tile BotLeftTile, Tile TopRightTile, Tile BotRightTile) = GetTilesInCoordinatesCorner(CenterCoordinates);
            return TopLeftTile.IsPassable() && BotLeftTile.IsPassable() && TopRightTile.IsPassable() && BotRightTile.IsPassable();
        }

        public bool IsPlayerInTile(Player player, Tile tile)
        {
            var (TopLeftTile, BotLeftTile, TopRightTile, BotRightTile) = GetTilesInCoordinatesCorner(player.GetCenter());
            return ReferenceEquals(TopLeftTile, tile) || ReferenceEquals(BotLeftTile, tile) || ReferenceEquals(TopRightTile, tile) || ReferenceEquals(BotRightTile, tile);
        }

        public List<XYCoordinates> ExplodeBomb(Bomb bomb)
        {
            int row = 0 ;
            int column = 0;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (ReferenceEquals(board[i,j], bomb))
                    {
                        row = i;
                        column = j;
                    }
                }
            }

            return ExplodeBomb(bomb,row,column);
        }
        private List<XYCoordinates> ExplodeBomb(Bomb bomb, int row, int column, Direction dir = Direction.Null)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            board[row, column] = new Tile(bomb.GetImage);
            BombAOE.Add(new XYCoordinates(column * 50, row * 50));

            if (dir != Direction.Up)
                BombAOE.AddRange(ExplodeUp(row, column, bomb.Strength));
            if (dir != Direction.Down)
                BombAOE.AddRange(ExplodeDown(row, column, bomb.Strength));
            if (dir != Direction.Left)
                BombAOE.AddRange(ExplodeLeft(row, column, bomb.Strength));
            if (dir != Direction.Right)
                BombAOE.AddRange(ExplodeRight(row, column, bomb.Strength));
            return BombAOE;

        }

        private List<XYCoordinates> ExplodeUp(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            while (row >= 0 && board[row,column].BlocksExplosion)
            {
                if (board[row,column] is Bomb bomb)
                {
                    BombAOE.AddRange(ExplodeBomb(bomb, row, column, Direction.Up));
                    return BombAOE;
                }
                else if (board[row,column])
                row--;
            }
            return BombAOE;
        }
        private List<XYCoordinates> ExplodeDown(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            return BombAOE;

        }
        private List<XYCoordinates> ExplodeLeft(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            return BombAOE;

        }
        private List<XYCoordinates> ExplodeRight(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            return BombAOE;
        }

        public (Tile TopLeftTile, Tile BotLeftTile, Tile TopRightTile, Tile BotRightTile) GetTilesInCoordinatesCorner(XYCoordinates Coordinates)
        {
            Tile TopLeftTile = board[(Coordinates.Y - 20) / 50, (Coordinates.X - 20) / 50];
            Tile BotLeftTile = board[(Coordinates.Y + 20) / 50, (Coordinates.X - 20) / 50];
            Tile TopRightTile = board[(Coordinates.Y - 20) / 50, (Coordinates.X + 20) / 50];
            Tile BotRightTile = board[(Coordinates.Y + 20) / 50, (Coordinates.X + 20) / 50];
            return (TopLeftTile, BotLeftTile, TopRightTile, BotRightTile);
        }
    }
}
