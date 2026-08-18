using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using models.Models;

namespace models.Service
{
    public class PasteService
    {
        public static ObservableCollection<Preparat> GetPreparatsFromClipboard()
        {
            var preparats = new ObservableCollection<Preparat>();
            var rawText = Clipboard.GetText();

            foreach (var preparat in PreparatParser.Parse(rawText))
            {
                preparats.Add(preparat);
            }

            return preparats;
        }
    }
}
