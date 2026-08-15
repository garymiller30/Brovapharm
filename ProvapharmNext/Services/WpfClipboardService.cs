using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using models.Service;
using models.Models;

namespace ProvapharmNext.Services
{
    /// <summary>
    /// Реалізація IPasteService для WPF додатку
    /// </summary>
    public class WpfClipboardService : IPasteService, IDisposable
    {
        private bool _disposed = false;

        public string GetRawText()
        {
            try
            {
                return Clipboard.GetText();
            }
            catch (Exception ex)
            {
                // Log the exception - in production you'd use a logger
                System.Diagnostics.Debug.WriteLine($"Clipboard error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Preparat>> GetPreparatsFromClipboardAsync()
        {
            var rawText = GetRawText();
            
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return new List<Preparat>();
            }

            // Парсинг відбувається у PasteService або іншому сервісі
            // Цей клас лише отримує дані з буфера обміну
            var preparats = PreparatParser.Parse(rawText);
            
            return await Task.FromResult(preparats);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        ~WpfClipboardService()
        {
            Dispose(false);
        }
    }
}