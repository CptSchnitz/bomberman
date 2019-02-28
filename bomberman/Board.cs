using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class Board
    {
        private Tile[,] _board;
        Random _random;
        public Point MaxCoordinates { get; }
        private int _numOfCrates;
        private int _numOfRows = 15;
        private int _numOfColumns = 17;

        public Board(Canvas canvas)
        {
            _random = new Random();
            _numOfCrates = 0;
            _board = new Tile[_numOfRows, _numOfColumns];
            MaxCoordinates = new Point(_board.GetLength(1) * Game.TileSize, _board.GetLength(0) * Game.TileSize);

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    Image image = new Image
                    {
                        Height = Game.TileSize,
                        Width = Game.TileSize,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };
                    Canvas.SetTop(image, i * Game.TileSize);
                    Canvas.SetLeft(image, j * Game.TileSize);
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
                    else
                    {
                        _numOfCrates++;
                    }
                    _board[i, j] = tile;
                }
            }
        }

        public Point[] SpawnLocation
        {
            get
            {
                Point[] locations =
                    {
                    new Point(25, 25),
                    new Point(25,725),
                    new Point(825,25),
                    new Point(825,725)
                };
                return locations;
            }
        }

        public int NumOfCrates
        {
            get
            {
                return _numOfCrates;
            }
        }

        public bool PlaceBomb(Player bombOwner, int bombStr, Game game)
        {
            int column = (int)bombOwner.Center.X / Game.TileSize;
            int row = (int)bombOwner.Center.Y / Game.TileSize;

            foreach (Player player in game.Players)
            {
                if (!ReferenceEquals(bombOwner, player) && player.Alive && IsPlayerInTile(player, _board[row, column]))
                    return false;
            }

            if (!(_board[row, column] is Bomb))
            {
                Bomb bomb = new Bomb(_board[row, column].Image, bombOwner, game);
                _board[row, column] = bomb;
                bomb.BombExplosion += game.OnBombExplosion;
                return true;
            }
            return false;
        }

        public void OnPlayerMovement(object sender, EventArgs e)
        {
            Player player = sender as Player;
            List<Tile> corners = GetUniqueCornerTilesFromCenterPoint(player.Center);
            foreach (Tile cornerTile in corners)
                if (cornerTile is PowerUp powerUp)
                {
                    powerUp.PickPowerUp(player);
                    TryGettingTileLocation(powerUp, out int row, out int column);
                    _board[row, column] = new Tile(powerUp.Image);
                }
        }

        public bool IsMovePossible(Point CenterPoint, Player player)
        {
            // out of bounds check
            if (CenterPoint.Y - Game.PlayerSize / 2 < 0 || CenterPoint.X - Game.PlayerSize / 2 < 0 ||
                CenterPoint.Y + Game.PlayerSize / 2 > MaxCoordinates.Y || CenterPoint.X + Game.PlayerSize / 2 > MaxCoordinates.X)
                return false;

            List<Tile> corners = GetUniqueCornerTilesFromCenterPoint(CenterPoint);
            foreach (Tile cornerTile in corners)
                if (!cornerTile.IsPassable(player))
                    return false;
            return true;
        }

        public bool IsPlayerInTile(Player player, Tile tile)
        {
            List<Tile> corners = GetUniqueCornerTilesFromCenterPoint(player.Center);
            foreach (Tile cornerTile in corners)
                if (ReferenceEquals(cornerTile, tile))
                    return true;
            return false;
        }

        public List<Tile> GetUniqueCornerTilesFromCenterPoint(Point point)
        {
            int distanceToPlayerEdge = Game.PlayerSize / 2 - 1;
            List<Tile> corners = new List<Tile>();
            Tile tile = _board[((int)point.Y - distanceToPlayerEdge) / Game.TileSize, ((int)point.X - distanceToPlayerEdge) / Game.TileSize];
            corners.Add(tile);

            tile = _board[((int)point.Y + distanceToPlayerEdge) / Game.TileSize, ((int)point.X - distanceToPlayerEdge) / Game.TileSize];
            if (!corners.Contains(tile))
                corners.Add(tile);

            tile = _board[((int)point.Y - distanceToPlayerEdge) / Game.TileSize, ((int)point.X + distanceToPlayerEdge) / Game.TileSize];
            if (!corners.Contains(tile))
                corners.Add(tile);

            tile = _board[((int)point.Y + distanceToPlayerEdge) / Game.TileSize, ((int)point.X + distanceToPlayerEdge) / Game.TileSize];
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
            while (row >= 0 && !_board[row, column].BlocksExplosion && bombStr > 0)
            {
                if (HandleSpecialTileCasesInExplosion(row, column, BombAOE, Direction.Down))
                {
                    return BombAOE;
                }
                else
                {
                    BombAOE.Add(_board[row, column]);
                    row--;
                    bombStr--;
                }
            }
            return BombAOE;
        }
        private List<Tile> ExplodeDown(int row, int column, int bombStr)
        {
            List<Tile> BombAOE = new List<Tile>();
            while (row < _board.GetLength(0) && !_board[row, column].BlocksExplosion && bombStr > 0)
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
            while (column >= 0 && !_board[row, column].BlocksExplosion && bombStr > 0)
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
            while (column < _board.GetLength(1) && !_board[row, column].BlocksExplosion && bombStr > 0)
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
                _numOfCrates--;
                switch (_random.Next() % 9)
                {
                    case 1:
                        tile = new SpeedPowerUp(_board[row, column].Image);
                        break;
                    case 0:
                        tile = new BombCountPowerUp(_board[row, column].Image);
                        break;
                    case 2:
                        tile = new BombStrPowerUp(_board[row, column].Image);
                        break;
                }
                flag = true;
            }
            else if (_board[row, column].Destructable)
            {
                flag = false;
            }
            BombAOE.Add(tile);
            _board[row, column] = tile;
            return flag;
        }

        public bool IsLocationSafe(Point point)
        {
            List<Tile> tileList = GetUniqueCornerTilesFromCenterPoint(point);
            foreach (Tile tile in tileList)
            {
                if (!IsTileSafe(tile))
                    return false;
            }
            return true;

        }

        private bool IsTileSafe(Tile tile)
        {
            if (tile is Bomb)
                return false;

            TryGettingTileLocation(tile, out int row, out int col);
            bool UpChecked = false, DownChecked = false, LeftChecked = false, RightChecked = false;
            for (int i = 1; i <= 5; i++)
            {
                bool isTileSafe = true;
                if (!UpChecked && (row - i < 0 || IsTileSafeHelper(row - i, col, i, out isTileSafe)))
                {
                    if (!isTileSafe)
                        return false;
                    UpChecked = true;
                }
                isTileSafe = true;
                if (!DownChecked && (row + i == _board.GetLength(0) || IsTileSafeHelper(row + i, col, i, out isTileSafe)))
                {
                    if (!isTileSafe)
                        return false;
                    DownChecked = true;
                }
                isTileSafe = true;
                if (!LeftChecked && (col - i < 0 || IsTileSafeHelper(row, col - i, i, out isTileSafe)))
                {
                    if (!isTileSafe)
                        return false;
                    LeftChecked = true;
                }
                isTileSafe = true;
                if (!RightChecked && (col + i == _board.GetLength(1) || IsTileSafeHelper(row, col + i, i, out isTileSafe)))
                {
                    if (!isTileSafe)
                        return false;
                    RightChecked = true;
                }
            }
            return true;
        }

        public bool IsNextToCrate(Player player)
        {
            int curRow = (int)player.Center.Y / Game.TileSize;
            int curCol = (int)player.Center.X / Game.TileSize;
            if (curRow - 1 > 0 && _board[curRow - 1, curCol] is Crate)
                return true;
            if (curRow + 1 < _board.GetLength(0) && _board[curRow + 1, curCol] is Crate)
                return true;
            if (curCol - 1 > 0 && _board[curRow, curCol - 1] is Crate)
                return true;
            if (curCol + 1 < _board.GetLength(1) && _board[curRow, curCol + 1] is Crate)
                return true;
            return false;
        }
        // the return value decides if theres a result about the saftey of the tile.
        private bool IsTileSafeHelper(int row, int col, int distanceFromTile, out bool isTileSafe)
        {
            isTileSafe = true;
            if (_board[row, col] is Crate || _board[row, col].BlocksExplosion)
            {
                isTileSafe = true;
                return true;
            }
            if (_board[row, col] is Bomb bomb)
            {
                isTileSafe = bomb.Strength < distanceFromTile;
                return true;
            }
            return false;
        }
    }
}
