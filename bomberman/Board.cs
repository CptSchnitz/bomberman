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
                    Canvas.SetZIndex(image, 1);
                    canvas.Children.Add(image);

                    Tile tile = new Crate(image);

                    if (i % 2 != 0 && j % 2 != 0)
                    {
                        tile = new Wall(image);
                    }
                    else if (((i == 0 || i == board.GetLength(0) - 1) && (j < 2 || j >= board.GetLength(1) - 2)) ||
                        ((i == 1 || i == board.GetLength(0) - 2) && (j == 0 || j == board.GetLength(1) - 1)))
                    {
                        tile = new Tile(image);
                    }

                    board[i, j] = tile;
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
            int row = 0;
            int column = 0;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (ReferenceEquals(board[i, j], bomb))
                    {
                        row = i;
                        column = j;

                    }
                }
            }
            return ExplodeBomb(bomb, row, column);

        }
        private List<XYCoordinates> ExplodeBomb(Bomb bomb, int row, int column, Direction dir = Direction.Null)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            board[row, column] = new Tile(bomb.GetImage);
            BombAOE.Add(new XYCoordinates(column * 50, row * 50));
            bomb.Explode();
            if (dir != Direction.Up)
                BombAOE.AddRange(ExplodeUp(row - 1, column, bomb.Strength));
            if (dir != Direction.Down)
                BombAOE.AddRange(ExplodeDown(row + 1, column, bomb.Strength));
            if (dir != Direction.Left)
                BombAOE.AddRange(ExplodeLeft(row, column - 1, bomb.Strength));
            if (dir != Direction.Right)
                BombAOE.AddRange(ExplodeRight(row, column + 1, bomb.Strength));
            return BombAOE;

        }

        private List<XYCoordinates> ExplodeUp(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            while (row >= 0 && !board[row, column].BlocksExplosion && bombStr > 0)
            {
                if (board[row, column] is Bomb bomb && bomb.IsArmed)
                {
                    BombAOE.AddRange(ExplodeBomb(bomb, row, column, Direction.Down));
                    return BombAOE;
                }
                else if (board[row, column].IsDestructable)
                {
                    board[row, column] = new Tile(board[row, column].GetImage);
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    row--;
                    bombStr--;
                }
            }
            return BombAOE;
        }
        private List<XYCoordinates> ExplodeDown(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            while (row < board.GetLength(0) && !board[row, column].BlocksExplosion && bombStr > 0)
            {
                if (board[row, column] is Bomb bomb && bomb.IsArmed)
                {
                    BombAOE.AddRange(ExplodeBomb(bomb, row, column, Direction.Up));
                    return BombAOE;
                }
                else if (board[row, column].IsDestructable)
                {
                    board[row, column] = new Tile(board[row, column].GetImage);
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    row++;
                    bombStr--;
                }
            }
            return BombAOE;
        }
        private List<XYCoordinates> ExplodeLeft(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            while (column >= 0 && !board[row, column].BlocksExplosion && bombStr > 0)
            {
                //Replace with method later
                if (board[row, column] is Bomb bomb && bomb.IsArmed)
                {
                    BombAOE.AddRange(ExplodeBomb(bomb, row, column, Direction.Right));
                    return BombAOE;
                }
                else if (board[row, column].IsDestructable)
                {
                    board[row, column] = new Tile(board[row, column].GetImage);
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    column--;
                    bombStr--;
                }
            }
            return BombAOE;

        }
        private List<XYCoordinates> ExplodeRight(int row, int column, int bombStr)
        {
            List<XYCoordinates> BombAOE = new List<XYCoordinates>();
            while (column < board.GetLength(1) && !board[row, column].BlocksExplosion && bombStr > 0)
            {
                //Replace with method later
                if (board[row, column] is Bomb bomb && bomb.IsArmed)
                {
                    BombAOE.AddRange(ExplodeBomb(bomb, row, column, Direction.Left));
                    return BombAOE;
                }
                else if (board[row, column].IsDestructable)
                {
                    board[row, column] = new Tile(board[row, column].GetImage);
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(new XYCoordinates(column * 50, row * 50));
                    column++;
                    bombStr--;
                }
            }
            return BombAOE;
        }

        public (Tile TopLeftTile, Tile BotLeftTile, Tile TopRightTile, Tile BotRightTile) GetTilesInCoordinatesCorner(XYCoordinates Coordinates)
        {
            Tile TopLeftTile = board[(Coordinates.Y - 24) / 50, (Coordinates.X - 24) / 50];
            Tile BotLeftTile = board[(Coordinates.Y + 24) / 50, (Coordinates.X - 24) / 50];
            Tile TopRightTile = board[(Coordinates.Y - 24) / 50, (Coordinates.X + 24) / 50];
            Tile BotRightTile = board[(Coordinates.Y + 24) / 50, (Coordinates.X + 24) / 50];
            return (TopLeftTile, BotLeftTile, TopRightTile, BotRightTile);
        }
    }
}
