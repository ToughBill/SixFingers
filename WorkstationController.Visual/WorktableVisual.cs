using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.Uitility;

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
        private Layout layout;
        /// <summary>
        /// Default constructor
        /// </summary>
        public WorktableVisual(Layout layout)
        {
            this.layout = layout;
            if (Configurations.Instance.Worktable == null)
                throw new Exception("Worktable cannot be null");
            this.worktable = Configurations.Instance.Worktable; 
        }

        /// <summary>
        /// Draw the worktable with pins, and the current DitiBox
        /// </summary>
        /// <param name="drawingContext"></param>
        public void Draw(DrawingContext drawingContext)
        {
            for (int i = 0; i < worktable.GridCount; i++)
            {
                DrawPinsSameGrid(i, drawingContext);
            }
            DrawBorder(worktable.Size,drawingContext);

            //if(layout.DitiInfo.DitiInfoItems.Count !=)
            string currentDitiLabel = layout.DitiInfo.CurrentDitiLabware;
            if(currentDitiLabel != "")
            {
                Labware labware = layout.FindLabware(currentDitiLabel);
                if(labware != null)
                {
                    labware.CalculatePositionInLayout();
                    var position = labware.GetAbsPosition(96);

                    VisualCommon.DrawCircle(position, 26, drawingContext, Colors.Red, true);
                }
            }
            
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
            Point ptStart = worktable.TopLeftPinPosition;
            int offset = grid * Worktable.DistanceBetweenAdjacentPins;
            int topPinX = (int)ptStart.X + offset;
            int topPinY = (int)ptStart.Y;
            DrawPin(topPinX, topPinY,worktable.PinSize,drawingContext);

            int bottomPinX = topPinX;
            int bottomPinY = worktable.BottomLeftPinYPosition;
            DrawPin(bottomPinX, bottomPinY, worktable.PinSize, drawingContext);
            VisualCommon.DrawGridNumber(grid,topPinX,drawingContext);
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
