using System.ComponentModel;
using System.Diagnostics;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// Static helper class for notifying object property change
    /// </summary>
    public static class PropertyChangedNotifyHelper
    {
        public static void NotifyPropertyChanged<T>(ref T oldValue, T setValue, object sender, string propertyName, PropertyChangedEventHandler handler)
        {
            if (oldValue.Equals(setValue))
                return;

            oldValue = setValue;

#if DEBUG
            // Output debug message for notifying value changed
            string debug_message = string.Format("[{0}] changed - Old Value: {1}; New Value: {2}",
                propertyName, oldValue, setValue);
            Trace.WriteLine(debug_message);
#endif
            PropertyChangedEventHandler handler_ref = handler;
            if(handler_ref != null)
                handler_ref(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
