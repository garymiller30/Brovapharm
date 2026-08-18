using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProvapharmNext.Commons
{
    /// <summary>
    /// Базовий клас для ViewModel з реалізацією INotifyPropertyChanged
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isProcessing;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Прапорець, що вказує на обробку даних (для блокування UI)
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            protected set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    OnPropertyChanged();
                    // Сповістити про зміну CanExecute для команд
                    OnCanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Подія для оновлення стану CanExecute
        /// </summary>
        public event EventHandler? OnCanExecuteChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Викликати подію зміни CanExecute для всіх команд
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
