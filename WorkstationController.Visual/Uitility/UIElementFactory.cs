using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.VisualElement.Uitility
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
                //Labware labware = ((Labware)wareBase).Clone();
                newUIElement = new LabwareUIElement((Labware)wareBase);
            }
            else
            {
                Carrier carrier = ((Carrier)wareBase).Clone() as Carrier;
                newUIElement = new CarrierUIElement((Carrier)carrier);
            }
            newUIElement.Selected = true;
            return newUIElement;
        }
    }
}
