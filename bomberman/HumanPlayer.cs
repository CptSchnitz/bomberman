using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class HumanPlayer : Player
    {
        public HumanPlayer(int x, int y, Image image) : base(x, y, image, "Assets/redcatface.png")
        {

        }

        public void MoveLeft(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(centerCoordinate.X - MoveSpeed, centerCoordinate.Y);
            if (board.IsMovePossible(newCenter))
            {
                centerCoordinate = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) - MoveSpeed);
            }
        }
        public void MoveUp(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(centerCoordinate.X, centerCoordinate.Y - MoveSpeed);
            if (board.IsMovePossible(newCenter))
            {
                centerCoordinate = newCenter;
                Canvas.SetTop(_image, Canvas.GetTop(_image) - MoveSpeed);
            }
        }
        public void MoveRight(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(centerCoordinate.X + MoveSpeed, centerCoordinate.Y);
            if (board.IsMovePossible(newCenter))
            {
                centerCoordinate = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) + MoveSpeed);
            }
        }
        public void MoveDown(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(centerCoordinate.X, centerCoordinate.Y + MoveSpeed);
            if (board.IsMovePossible(newCenter))
            {
                centerCoordinate = newCenter;
                Canvas.SetTop(_image, Canvas.GetTop(_image) + MoveSpeed);
            }
        }
    }
}
