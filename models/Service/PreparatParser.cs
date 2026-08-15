using System;
using System.Collections.Generic;
using System.Diagnostics;
using models.Models;

namespace models.Service
{
    /// <summary>
    /// Парсер даних препаратів з тексту (формат: ID\tНазва\tНомер)
    /// </summary>
    public static class PreparatParser
    {
        private const int MinRequiredColumns = 3;
        
        /// <summary>
        /// Розпарсити список препаратів з необробленого тексту
        /// </summary>
        /// <param name="rawText">Текст у форматі: ID\tНазва\tНомер (кожен запис в окремому рядку)</param>
        /// <returns>Список розпізнаних препаратів</returns>
        public static List<Preparat> Parse(string rawText)
        {
            var preparats = new List<Preparat>();
            
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return preparats;
            }

            var rows = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i].Trim();
                
                if (string.IsNullOrEmpty(row))
                    continue;

                try
                {
                    var columns = row.Split('\t');
                    
                    if (columns.Length < MinRequiredColumns)
                    {
                        Debug.WriteLine($"Рядок {i + 1}: Недостатньо колонок (очікується {MinRequiredColumns}, отримано: {columns.Length})");
                        continue;
                    }

                    // Парсинг ID
                    if (!int.TryParse(columns[0].Trim(), out int id))
                    {
                        Debug.WriteLine($"Рядок {i + 1}: Неправильний формат ID '{columns[0]}'. Очікується ціле число.");
                        continue;
                    }

                    // Вилучення номера (може містити символи)
                    var number = columns.Length > 2 ? string.Join("\t", columns, 2, columns.Length - 2) : string.Empty;

                    var preparat = new Preparat
                    {
                        Id = id,
                        Name = columns[1].Trim(),
                        Number = number.Trim()
                    };

                    // Валідація обов'язкових полів
                    if (string.IsNullOrEmpty(preparat.Name))
                    {
                        Debug.WriteLine($"Рядок {i + 1}: Назва препарату не може бути порожньою");
                        continue;
                    }

                    preparats.Add(preparat);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Помилка парсингу рядка {i + 1}: {ex.Message}");
                    // Продовжити обробку інших рядків
                    continue;
                }
            }

            return preparats;
        }
    }
}