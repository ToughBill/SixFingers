using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for ColorPickerCombo.xaml
    /// </summary>
    public partial class ColorPickerCombo
    {
        /// <summary>
        /// The selected color property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerCombo"/> class.
        /// </summary>
        public ColorPickerCombo()
        {
            this.InitializeComponent();
        }

        static ColorPickerCombo()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(Colors.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
            SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerCombo), metadata);
        }

        /// <summary>
        /// Gets or sets the color of the selected.
        /// </summary>
        /// <value>
        /// The color of the selected.
        /// </value>
        public Color SelectedColor
        {
            get 
            { 
                return (Color)this.GetValue(SelectedColorProperty); 
            }

            set 
            {
                this.SetValue(SelectedColorProperty, value);

                if(this.superCombo.SelectedIndex == -1)
                {
                    int index = GetKnownColorIndex(value);
                    if(index > 0)
                    {
                        this.superCombo.SelectedIndex = index;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the known color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The name of the color</returns>
        private static int GetKnownColorIndex(Color color)
        {
            Color knownColor;
            int count = 0;

            // Use reflection to get all known colors 
            Type colorType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] arrPiColors = colorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            // Iterate over all known colors, convert each to a <Color> and then compare 
            // that color to the passed color. 
            foreach (PropertyInfo pi in arrPiColors)
            {
                knownColor = (Color)pi.GetValue(null, null);
                if (knownColor == color)
                { 
                    return count; 
                }

                count++;
            }

            return -1;
        }
    }
}
