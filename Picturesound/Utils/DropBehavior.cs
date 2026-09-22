using System.Windows;
using System.Windows.Input;

namespace Picturesound.Utils
{
    public static class DropBehavior
    {
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropCommand",
                typeof(ICommand),
                typeof(DropBehavior),
                new PropertyMetadata(null, OnDropCommandChanged));

        public static void SetDropCommand(
            DependencyObject element,
            ICommand value)
        {
            element.SetValue(DropCommandProperty, value);
        }

        public static ICommand? GetDropCommand(
            DependencyObject element)
        {
            return element.GetValue(DropCommandProperty) as ICommand;
        }

        private static void OnDropCommandChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not UIElement element)
                return;

            if (e.OldValue is ICommand)
            {
                element.Drop -= OnDrop;
                element.DragOver -= OnDragOver;
            }

            if (e.NewValue is ICommand)
            {
                element.Drop += OnDrop;
                element.DragOver += OnDragOver;
            }
        }

        private static void OnDragOver(
            object sender,
            DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private static void OnDrop(
            object sender,
            DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
                return;
            }

            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files is null || files.Length == 0)
            {
                e.Handled = true;
                return;
            }

            if (sender is DependencyObject dependencyObject)
            {
                var command = GetDropCommand(dependencyObject);

                if (command?.CanExecute(files) == true)
                {
                    command.Execute(files);
                }
            }

            e.Handled = true;
        }
    }
}