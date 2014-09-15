using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// Worktable drawing element
    /// </summary>
    public class WorktableVisual
    {
        /// <summary>
        /// Worktable data
        /// </summary>
        private Worktable worktable;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public WorktableVisual()
        {
            this.worktable = Configurations.Instance.Worktable; 
        }

        /// <summary>
        /// Draw the worktable with pins
        /// </summary>
        /// <param name="drawingContext"></param>
        public void Draw(DrawingContext drawingContext)
        {
            for (int i = 0; i < worktable.GridCount; i++)
            {
                DrawPinsSameGrid(i, drawingContext);
            }
        }

        /// <summary>
        /// Draw three pins on specified grid on worktable 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="drawingContext"></param>
        private void DrawPinsSameGrid(int grid, DrawingContext drawingContext)
        {
            Point ptStart = worktable.FirstPinPosition;
            int offset = grid * Worktable.DistanceBetweenAdjacentPins;
            int firstPinX = (int)ptStart.X + offset;
            int firstPinY = (int)ptStart.Y;
            DrawPin(firstPinX, firstPinY,worktable.FirstRowPinSize,drawingContext);

            int secondPinX = firstPinX;
            int secondPinY = worktable.SecondPinYPosition;
            DrawPin(secondPinX, secondPinY, worktable.SecondRowPinSize, drawingContext);

            int thirdPinX = firstPinX;
            int thirdPinY = worktable.ThirdPinYPosition;
            DrawPin(thirdPinX, thirdPinY, worktable.ThirdRowPinSize, drawingContext);
        }

        /// <summary>
        /// Draw a pin on worktable
        /// </summary>
        /// <param name="pinX"></param>
        /// <param name="pinY"></param>
        /// <param name="size"></param>
        /// <param name="drawingContext"></param>
        private void DrawPin(int pinX, int pinY, Size size, DrawingContext drawingContext)
        {
            VisualCommon.DrawRect(pinX, pinY, size, drawingContext, Colors.Black);
        }
    }
}
