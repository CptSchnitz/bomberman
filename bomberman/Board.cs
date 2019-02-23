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
        private Tile[,] _board;
        Random _random;
        public XYCoordinates MaxCoordinates { get; }

        public Board(Canvas canvas)
        {
            _random = new Random();
            // fix next two lines (use fields mb)
            _board = new Tile[15, 17];
            MaxCoordinates = new XYCoordinates(_board.GetLength(1) * 50, _board.GetLength(0) * 50);

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
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
                    else if (((i == 0 || i == _board.GetLength(0) - 1) && (j < 2 || j >= _board.GetLength(1) - 2)) ||
                        ((i == 1 || i == _board.GetLength(0) - 2) && (j == 0 || j == _board.GetLength(1) - 1)))
                    {
                        tile = new Tile(image);
                    }

                    _board[i, j] = tile;
                }
            }
        }

        public XYCoordinates[] SpawnLocation
        {
            get
            {
                XYCoordinates[] locations =
                    {
                    new XYCoordinates(25, 25),
                    new XYCoordinates(25,725),
                    new XYCoordinates(825,25),
                    new XYCoordinates(825,725),
                };
                return locations;
            }
        }

        public bool PlaceBomb(Player bombOwner, int bombStr, out Bomb bomb)
        {
            bomb = null;
            int column = (int)bombOwner.GetCenter().X / 50;
            int row = (int)bombOwner.GetCenter().Y / 50;
            if (!(_board[row, column] is Bomb))
            {
                _board[row, column] = bomb = new Bomb(_board[row, column].Image, bombOwner, this);
                return true;
            }
            return false;
        }

        public void OnPlayerMovement(object sender, EventArgs e)
        {
            Player player = sender as Player;
            List<Tile> corners = GetUniqueCornerTilesFromCoordinates(player.GetCenter());
            foreach (Tile cornerTile in corners)
                if (cornerTile is PowerUp powerUp)
                {
                    powerUp.PickPowerUp(player);
                    TryGettingTileLocation(powerUp, out int row, out int column);
                    _board[row, column] = new Tile(powerUp.Image);
                }
        }

        public bool IsMovePossible(XYCoordinates CenterCoordinates, Player player)
        {
            // out of bounds check
            if (CenterCoordinates.Y - 25 < 0 || CenterCoordinates.X - 25 < 0 || CenterCoordinates.Y + 25 > MaxCoordinates.Y || CenterCoordinates.X + 25 > MaxCoordinates.X)
                return false;

            List<Tile> corners = GetUniqueCornerTilesFromCoordinates(CenterCoordinates);
            foreach (Tile cornerTile in corners)
                if (!cornerTile.IsPassable(player))
                    return false;
            return true;
        }

        public bool IsPlayerInTile(Player player, Tile tile)
        {
            List<Tile> corners = GetUniqueCornerTilesFromCoordinates(player.GetCenter());
            foreach (Tile cornerTile in corners)
                if (ReferenceEquals(cornerTile, tile))
                    return true;
            return false;
        }

        public List<Tile> GetUniqueCornerTilesFromCoordinates(XYCoordinates Coordinates)
        {
            List<Tile> corners = new List<Tile>();
            Tile tile = _board[((int)Coordinates.Y - 24) / 50, ((int)Coordinates.X - 24) / 50];
            corners.Add(tile);

            tile = _board[((int)Coordinates.Y + 24) / 50, ((int)Coordinates.X - 24) / 50];
            if (!corners.Contains(tile))
                corners.Add(tile);

            tile = _board[((int)Coordinates.Y - 24) / 50, ((int)Coordinates.X + 24) / 50];
            if (!corners.Contains(tile))
                corners.Add(tile);

            tile = _board[((int)Coordinates.Y + 24) / 50, ((int)Coordinates.X + 24) / 50];
            if (!corners.Contains(tile))
                corners.Add(tile);

            return corners;
        }

        public bool TryGettingTileLocation(Tile tile, out int row, out int column)
        {
            row = 0;
            column = 0;

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    if (ReferenceEquals(_board[i, j], tile))
                    {
                        row = i;
                        column = j;
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Tile> ExplodeBomb(Bomb bomb)
        {
            TryGettingTileLocation(bomb, out int row, out int column);
            return ExplodeBomb(bomb, row, column);

        }
        private List<Tile> ExplodeBomb(Bomb bomb, int row, int column, Direction lastBombRelativeDirection = Direction.Null)
        {
            List<Tile> BombAOE = new List<Tile>();
            Tile tile = new Tile(bomb.Image);
            _board[row, column] = tile;
            BombAOE.Add(tile);
            bomb.Explode();
            if (lastBombRelativeDirection != Direction.Up)
                BombAOE.AddRange(ExplodeUp(row - 1, column, bomb.Strength));
            if (lastBombRelativeDirection != Direction.Down)
                BombAOE.AddRange(ExplodeDown(row + 1, column, bomb.Strength));
            if (lastBombRelativeDirection != Direction.Left)
                BombAOE.AddRange(ExplodeLeft(row, column - 1, bomb.Strength));
            if (lastBombRelativeDirection != Direction.Right)
                BombAOE.AddRange(ExplodeRight(row, column + 1, bomb.Strength));
            return BombAOE;

        }

        private List<Tile> ExplodeUp(int row, int column, int bombStr)
        {
            List<Tile> BombAOE = new List<Tile>();
            while (row >= 0 && !_board[row, column].IsBlocksExplosion() && bombStr > 0)
            {
                if (HandleSpecialTileCasesInExplosion(row, column, BombAOE, Direction.Down))
                {
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(_board[row,column]);
                    row--;
                    bombStr--;
                }
            }
            return BombAOE;
        }
        private List<Tile> ExplodeDown(int row, int column, int bombStr)
        {
            List<Tile> BombAOE = new List<Tile>();
            while (row < _board.GetLength(0) && !_board[row, column].IsBlocksExplosion() && bombStr > 0)
            {
                if (HandleSpecialTileCasesInExplosion(row, column, BombAOE, Direction.Up))
                {
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(_board[row, column]);
                    row++;
                    bombStr--;
                }
            }
            return BombAOE;
        }
        private List<Tile> ExplodeLeft(int row, int column, int bombStr)
        {
            List<Tile> BombAOE = new List<Tile>();
            while (column >= 0 && !_board[row, column].IsBlocksExplosion() && bombStr > 0)
            {
                if (HandleSpecialTileCasesInExplosion(row, column, BombAOE, Direction.Right))
                {
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(_board[row, column]);
                    column--;
                    bombStr--;
                }
            }
            return BombAOE;

        }
        private List<Tile> ExplodeRight(int row, int column, int bombStr)
        {
            List<Tile> BombAOE = new List<Tile>();
            while (column < _board.GetLength(1) && !_board[row, column].IsBlocksExplosion() && bombStr > 0)
            {
                if (HandleSpecialTileCasesInExplosion(row, column, BombAOE, Direction.Left))
                {
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(_board[row, column]);
                    column++;
                    bombStr--;
                }
            }
            return BombAOE;
        }

        private bool HandleSpecialTileCasesInExplosion(int row, int column, List<Tile> BombAOE, Direction bombRelativeDirection)
        {
            if (_board[row, column] is Bomb bomb)
            {
                BombAOE.AddRange(ExplodeBomb(bomb, row, column, bombRelativeDirection));
                return true;
            }

            Tile tile = new Tile(_board[row, column].Image);
            bool flag = false;
            if (_board[row, column] is Crate crate)
            {                
                switch (_random.Next() % 9)
                {
                    case 0:
                        tile = new SpeedPowerUp(_board[row, column].Image);
                        break;
                    case 1:
                        tile = new BombCountPowerUp(_board[row, column].Image);
                        break;
                    case 2:
                        tile = new BombStrPowerUp(_board[row, column].Image);
                        break;
                }
                flag =  true;
            }
            else if (_board[row, column].IsDestructable())
            {
                flag = false;
            }
            BombAOE.Add(tile);
            _board[row, column] = tile;
            return flag;
        }

    }
}
