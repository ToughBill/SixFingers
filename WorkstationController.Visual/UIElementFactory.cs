using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// factory produce UIElement from ware
    /// </summary>
    public  class UIElementFactory
    {
        /// <summary>
        /// create UIElement
        /// </summary>
        /// <param name="wareBase"></param>
        /// <returns></returns>
        public static BasewareUIElement CreateUIElement(WareBase wareBase)
        {
            BasewareUIElement newUIElement;
            if (wareBase is Labware)
            {
                newUIElement = new LabwareUIElement((Labware)wareBase);
                
            }
            else
            {
                newUIElement = new CarrierUIElement((Carrier)wareBase);
            }
            newUIElement.Selected = true;
            return newUIElement;
        }
    }
}
