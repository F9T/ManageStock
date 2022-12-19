using MaterialDesignExtensions.Controls;
using System;
using System.Windows.Documents;
using System.Windows.Media;

namespace Application.Common
{
    public class CustomWindow : MaterialWindow, IDisposable
    {
        public CustomWindow()
        {
            TextElement.SetFontSize(this, 14);
            FontFamily = FindResource("MaterialDesignFont") as FontFamily;
            Foreground = Brushes.Black;
        }

        public virtual void Dispose()
        {
        }
    }
}
