using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Picturesound.Utils;
using System.IO;

namespace Picturesound.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string? _imagePath;
        private bool _isPlaying;

        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath == value) return;
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public string IsPlayingText
        {
            get
            {
                return IsPlaying == true ? "Stop" : "Start";
            }
        }

        public ICommand DropImageCommand { get; }

        public ICommand ToggleIsPlayingCommand { get; }
        public MainViewModel()
        {
            DropImageCommand = new RelayCommand(OnDropImage);
            ToggleIsPlayingCommand = new RelayCommand(OnToggleIsPlaying);
        }

        private void OnToggleIsPlaying(object? parameter)
        {
            IsPlaying = !IsPlaying;
        }

        private void OnDropImage(object? parameter)
        {
            if (parameter is not string[] files) return;

            if (files.Length == 0) return;

            string file = files[0];

            string extension = Path.GetExtension(file).ToLowerInvariant();

            if(extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif")
            {
                ImagePath = file;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
