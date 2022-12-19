using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Application.Common.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly  DependencyProperty AutoSelectAllProperty = DependencyProperty.RegisterAttached("AutoSelectAll", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AutoSelectAllPropertyChangedCallback));

        public static bool IsAutoSelectAll(TextBox _TextBox)
        {
            return (bool) _TextBox.GetValue(AutoSelectAllProperty);
        }

        public static void SetAutoSelectAll(TextBox _TextBox, bool _Value)
        {
            _TextBox.SetValue(AutoSelectAllProperty, _Value);
        }

        public static readonly  DependencyProperty EnterToConfirmProperty = DependencyProperty.RegisterAttached("EnterToConfirm", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EnterToConfirmChangedCallback));
        
        public static bool IsEnterToConfirm(TextBox _TextBox)
        {
            return (bool) _TextBox.GetValue(EnterToConfirmProperty);
        }

        public static void SetEnterToConfirm(TextBox _TextBox, bool _Value)
        {
            _TextBox.SetValue(EnterToConfirmProperty, _Value);
        }
        
        public static readonly  DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(TextBoxHelper), new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public static ICommand GetCommand(TextBox _TextBox)
        {
            return (ICommand) _TextBox.GetValue(CommandProperty);
        }

        public static void SetCommand(TextBox _TextBox, ICommand _Value)
        {
            _TextBox.SetValue(CommandProperty, _Value);
        }


        public static readonly  DependencyProperty ParameterCommandProperty = DependencyProperty.RegisterAttached("ParameterCommand", typeof(object), typeof(TextBoxHelper), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public static object GetParameterCommand(TextBox _TextBox)
        {
            return (object) _TextBox.GetValue(ParameterCommandProperty);
        }

        public static void SetParameterCommand(TextBox _TextBox, object _Value)
        {
            _TextBox.SetValue(ParameterCommandProperty, _Value);
        }
        

        private static void AutoSelectAllPropertyChangedCallback(DependencyObject _D, DependencyPropertyChangedEventArgs _E)
        {
            TextBox textBox = _D as TextBox;
            if (textBox == null)
                return;

            if (_E.NewValue is bool value)
            {
                if (value)
                {
                    textBox.GotFocus += GotFocus;
                    textBox.PreviewMouseDown += OnPreviewMouseDown;
                }
                else
                {
                    textBox.GotFocus -= GotFocus;
                    textBox.PreviewMouseDown -= OnPreviewMouseDown;
                }
            }

        }

        private static void EnterToConfirmChangedCallback(DependencyObject _D, DependencyPropertyChangedEventArgs _E)
        {
            TextBox textBox = _D as TextBox;
            if (textBox == null)
                return;

            if (_E.NewValue is bool value)
            {
                if (value)
                {
                    textBox.PreviewKeyUp += OnPreviewKeyUp;
                }
                else
                {
                    textBox.PreviewKeyUp -= OnPreviewKeyUp;
                }
            }
        }

        private static void OnPreviewKeyUp(object _Sender, KeyEventArgs _E)
        {
            if (_E.Key == Key.Enter)
            {
                TextBox textBox = _Sender as TextBox;
                if (textBox != null)
                {
                    object parameter = GetParameterCommand(textBox);
                    ICommand command = GetCommand(textBox);
                    command?.Execute(parameter);
                }
            }
        }

        private static void OnPreviewMouseDown(object _Sender, MouseButtonEventArgs _E)
        {
            TextBox textBox = _Sender as TextBox;
            if (textBox != null && !textBox.IsKeyboardFocusWithin)
            {
                if (_E.OriginalSource.GetType().Name == "TextBoxView")
                {
                    _E.Handled = true;
                    textBox.Focus();
                }
            }
        }

        private static void GotFocus(object _Sender, RoutedEventArgs _E)
        {
            TextBox textBox = _Sender as TextBox;
            if(textBox != null)
            {
                textBox.SelectAll();
                _E.Handled = true;
            }
        }
    }
}
