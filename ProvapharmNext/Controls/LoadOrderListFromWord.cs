using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ProvapharmNext.Controls;

public class LoadOrderListFromWord
{
    public static IEnumerable<Preparat> Load(string wordFile)
    {
        List<Preparat> list = new List<Preparat>();
        try
        {
            using var doc = WordprocessingDocument.Open(wordFile, false);

            if (doc?.MainDocumentPart?.Document?.Body == null)
            {
                MessageBox.Show("Не вдалося відкрити документ Word.");
                return list;
            }

            Table table = doc.MainDocumentPart.Document.Body.Elements<Table>().FirstOrDefault();
            if (table == null)
            {
                MessageBox.Show("У документі не знайдено таблицю з даними.");
                return list;
            }

            List<TableRow> rows = table.Elements<TableRow>().ToList();

            for (int i = rows.Count - 1; i > 0; i--)
            {
                var cells = rows[i].Descendants<TableCell>().ToList();

                if (cells.Count < 9)
                    continue;

                var idText = cells[0].InnerText?.Trim();
                var nameText = cells[1].InnerText?.Trim();
                var numberText = cells[2].InnerText?.Trim();
                var quantityText = cells[8].InnerText?.Trim();

                if (string.IsNullOrEmpty(idText) || string.IsNullOrEmpty(nameText) || string.IsNullOrEmpty(numberText))
                    continue;

                if (!int.TryParse(idText, out var id))
                    continue;

                var preparat = new Preparat()
                {
                    Id = id,
                    Name = nameText,
                    Number = numberText,
                };

                if (!string.IsNullOrEmpty(quantityText) &&
                    decimal.TryParse(quantityText, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out var quantityDecimal))
                {
                    preparat.Quantity = (int)(quantityDecimal * 1000);
                }

                list.Insert(0, preparat);

                if (preparat.Id == 1)
                    break;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }

        return list;
    }
}
