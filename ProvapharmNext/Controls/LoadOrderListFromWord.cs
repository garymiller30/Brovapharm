using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProvapharmNext.Controls
{
    public class LoadOrderListFromWord
    {
        public static IEnumerable<Preparat> Load(string wordFile)
        {
            List<Preparat> list = new List<Preparat>();
            try
            {
                using (var doc = WordprocessingDocument.Open(wordFile, false))
                {
                    DataTable dt = new DataTable();

                    Table table = doc.MainDocumentPart.Document.Body.Elements<Table>().First();

                    // To get all rows from table  
                    List<TableRow> rows = table.Elements<TableRow>().ToList();

                    for (int i = rows.Count() - 1; i > 0; i--)
                    {
                        var cells = rows[i].Descendants<TableCell>().ToList();

                        var preparat = new Preparat()
                        {
                            Id = int.Parse(cells[0].InnerText),
                            Name = cells[1].InnerText,
                            Number = cells[2].InnerText,
                        };

                        list.Insert(0, preparat);

                        if (preparat.Id == 1) break;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                
            }


            return list;
        }
    }
}
