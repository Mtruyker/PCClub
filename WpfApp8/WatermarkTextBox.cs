using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCClubAdmin.Controls
{
    public class WatermarkTextBox : TextBox
    {
        // Dependency property для текста подсказки
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox));

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        // Dependency property для цвета текста подсказки
        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(WatermarkTextBox),
                new PropertyMetadata(Brushes.Gray));
        public Brush WatermarkForeground
        {
            get => (Brush)GetValue(WatermarkForegroundProperty);
            set => SetValue(WatermarkForegroundProperty, value);
        }
    }
}