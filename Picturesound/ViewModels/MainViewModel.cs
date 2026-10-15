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
        private System.TimeSpan _duration = System.TimeSpan.FromMinutes(3);
        private System.TimeSpan _elapsed = System.TimeSpan.Zero;
        private System.Windows.Threading.DispatcherTimer? _timer;

        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath == value) return;
                _imagePath = value;
                // Reset timer/progress when image changes
                Elapsed = System.TimeSpan.Zero;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsStartEnabled));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPlayingText));
                OnPropertyChanged(nameof(IsPlayingColor));
                if (_isPlaying)
                {
                    StartTimer();
                }
                else
                {
                    StopTimer();
                }
            }
        }

        public string IsPlayingText
        {
            get
            {
                return IsPlaying == true ? "Stop" : "Start";
            }
        }

        public string IsPlayingColor => IsPlaying ? "#4EE06A" : "#888888";

        public bool IsStartEnabled => !string.IsNullOrEmpty(ImagePath);

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationMinutes));
                OnPropertyChanged(nameof(DurationSeconds));
                OnPropertyChanged(nameof(TimerDisplay));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public int DurationMinutes
        {
            get => (int)Duration.TotalMinutes;
            set
            {
                var s = Duration.Seconds;
                Duration = TimeSpan.FromMinutes(Math.Max(0, value)) + TimeSpan.FromSeconds(s);
            }
        }

        public int DurationSeconds
        {
            get => Duration.Seconds;
            set
            {
                var m = (int)Duration.TotalMinutes;
                var seconds = Math.Max(0, Math.Min(59, value));
                Duration = TimeSpan.FromMinutes(m) + TimeSpan.FromSeconds(seconds);
            }
        }

        public TimeSpan Elapsed
        {
            get => _elapsed;
            private set
            {
                if (_elapsed == value) return;
                _elapsed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimerDisplay));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public string TimerDisplay
        {
            get
            {
                int em = (int)Elapsed.TotalMinutes;
                int es = Elapsed.Seconds;
                int dm = (int)Duration.TotalMinutes;
                int ds = Duration.Seconds;
                return $"{em}:{es:D2} / {dm}:{ds:D2}";
            }
        }

        public double Progress
        {
            get
            {
                var total = Duration.TotalSeconds;
                if (total <= 0) return 0.0;
                return Math.Max(0.0, Math.Min(1.0, Elapsed.TotalSeconds / total));
            }
        }

        public ICommand DropImageCommand { get; }

        public ICommand ToggleIsPlayingCommand { get; }
        public MainViewModel()
        {
            DropImageCommand = new RelayCommand(OnDropImage);
            ToggleIsPlayingCommand = new RelayCommand(OnToggleIsPlaying);
            // default duration 3:00
            _duration = TimeSpan.FromMinutes(3);
            _elapsed = TimeSpan.Zero;
        }

        private void OnToggleIsPlaying(object? parameter)
        {
            if (!IsStartEnabled)
            {
                // don't start if no image
                IsPlaying = false;
                return;
            }

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
                // Reset elapsed when a new image is dropped
                Elapsed = TimeSpan.Zero;
            }
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Render);
                _timer.Interval = TimeSpan.FromMilliseconds(50);
                _timer.Tick += Timer_Tick;
            }
            _timer.Start();
        }

        private void StopTimer()
        {
            if (_timer == null) return;
            _timer.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!IsPlaying) return;
            var next = Elapsed + _timer!.Interval;
            if (next >= Duration)
            {
                // wrap
                next = TimeSpan.Zero;
            }
            Elapsed = next;
        }

        // Called from the view when the user drags the position indicator
        public void SetProgress(double ratio)
        {
            ratio = Math.Max(0.0, Math.Min(1.0, ratio));
            Elapsed = TimeSpan.FromSeconds(Duration.TotalSeconds * ratio);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
