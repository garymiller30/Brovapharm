using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using iTextSharp.text;
using models.Models;

namespace models.Service
{
    public class PasteService
    {
        public static ObservableCollection<Preparat> GetPreparatsFromClipboard()
        {
            var rowStrings = Clipboard.GetText().Split(new[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);

            ObservableCollection<Preparat> preparats = new ObservableCollection<Preparat>();

            foreach (var rowString in rowStrings)
            {
                var row = rowString.Split('\t');

                var preparat = new Preparat()
                {
                    Id = int.Parse(row[0]),
                    Name = row[1],
                    Number = row[2]
                };

                preparats.Add(preparat);
                
            }

            return preparats;
        }
    }
}