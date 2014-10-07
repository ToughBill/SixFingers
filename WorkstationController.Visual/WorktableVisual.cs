using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            if (Configurations.Instance.Worktable == null)
                throw new Exception("Worktable cannot be null");
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
            DrawBorder(worktable.Size,drawingContext);
        }

        private void DrawBorder(Size size, DrawingContext drawingContext)
        {
            VisualCommon.DrawRect(0, 0, size, drawingContext, Colors.Black);
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
            VisualCommon.DrawGridNumber(grid,firstPinX,drawingContext);
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
            VisualCommon.DrawSolidRect(pinX, pinY, size, drawingContext, Colors.Black);
        }

    }
}
