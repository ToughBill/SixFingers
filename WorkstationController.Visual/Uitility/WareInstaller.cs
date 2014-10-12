using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement.Uitility
{
    class WareInstaller
    {
        public static void MountThis(BasewareUIElement baseUIElement, Point position, Grid container)
        {
            int grid = VisualCommon.FindCorrespondingGrid(position.X);
            int totalGrid = Configurations.Instance.Worktable.GridCount;

            //1 moves out of worktable
            if(grid < 1 || grid > totalGrid)
            {
                container.Children.Remove(baseUIElement);
            }

            //2 if is Carrier, see whether there are enough grids for it
            if( baseUIElement is CarrierUIElement)
            {
                CarrierUIElement carrierUIElement = (CarrierUIElement)baseUIElement;
                bool outOfRange = IsOutofRange(grid,carrierUIElement);
                carrierUIElement.Grid = grid;
            }

            //3 if is labware, see whether there are suitable carrier for mounting onto
            if( baseUIElement is LabwareUIElement)
            {
                //bool hasSuitableSite2Mount = HasSuitableSite(baseUIElement.Ware.TypeName)
            }

            baseUIElement.Selected = false;
            //baseUIElement.Grid = grid;
        }

        private static bool IsOutofRange(int grid, CarrierUIElement carrierUIElement)
        {


            return true;
        }
    }

    class OverlapChecker
    {
        public static bool IsOverlapped(Grid grid, CarrierUIElement newCarrierUI,int newUICurGrid)
        {
            bool bOverlapped = false;
            int newCarrierXStart = newCarrierUI.GetBoundingRectXStart();
            int newCarrierXEnd = newCarrierXStart +  VisualCommon.GetCarrierLength(newCarrierUI);

            foreach(UIElement uiElement in grid.Children)
            {
                if (!(uiElement is CarrierUIElement))
                    continue;
                CarrierUIElement carrierUIElement = uiElement as CarrierUIElement;
                int oldCarrierXStart = carrierUIElement.GetBoundingRectXStart();
                int oldCarrierXEnd = oldCarrierXStart + VisualCommon.GetCarrierLength(carrierUIElement);
                bOverlapped = IsOverlapped(newCarrierXStart, newCarrierXEnd, oldCarrierXStart, oldCarrierXEnd);
                if (bOverlapped)
                    break;
            }
            return bOverlapped;
        }

        private static bool IsOverlapped(int newCarrierXStart, int newCarrierXEnd, int oldCarrierXStart, int oldCarrierXEnd)
        {
            if (newCarrierXStart == oldCarrierXStart)
                return true;
            if (newCarrierXEnd == oldCarrierXEnd)
                return true;

            bool newStartBetweenExisting = IsBetween(newCarrierXStart, oldCarrierXStart, oldCarrierXEnd);
            bool newEndBetweenExisting =  IsBetween(newCarrierXEnd, oldCarrierXStart, oldCarrierXEnd);
            bool existingStartBetweenNew = IsBetween(oldCarrierXStart, newCarrierXStart, newCarrierXEnd);
            bool existingEndBetweenNew = IsBetween(oldCarrierXEnd, newCarrierXStart, newCarrierXEnd);
            return newStartBetweenExisting ||
                   newEndBetweenExisting ||
                   existingStartBetweenNew ||
                   existingEndBetweenNew;

        }

        private static bool IsBetween(int test, int start, int end)
        {
            return test > start && test < end;
        }



      

   

  
    }
}
