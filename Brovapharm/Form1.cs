using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brovapharm.Models;

namespace Brovapharm
{
    public partial class Form1 : Form
    {
        private GlobalSettings _globalSettings = new GlobalSettings();
        private List<Preparat> _preparats = new List<Preparat>();
        public Form1()
        {
            InitializeComponent();

            treeListView1.CanExpandGetter = x => x is Preparat preparat ?  preparat.FileList.Any() : false;
            treeListView1.ChildrenGetter = x => (x as Preparat).FileList;


            olvColumnId.AspectGetter = x => x is Preparat preparat ? preparat.Id.ToString() : string.Empty;
            olvColumnName.AspectGetter = x => x is Preparat preparat ? preparat.Name : ((PreparatFile)x).File.Name;
            olvColumnNumber.AspectGetter = x => x is Preparat preparat ? preparat.Number : String.Empty;
            olvColumnCntPages.AspectGetter =x=> x is PreparatFile file ? file.CntPages.ToString() : string.Empty;
            olvColumnDate.AspectGetter = x =>
                x is PreparatFile file ? file.File.LastWriteTime.Date.ToShortDateString() : String.Empty;

        }

        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
            var rowStrings = Clipboard.GetText().Split(new[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);

            foreach (var rowString in rowStrings)
            {
                var row = rowString.Split('\t');

                var preparat = new Preparat()
                {
                    Id = Int32.Parse(row[0]),
                    Name = row[1],
                    Number = row[2]
                };

                _preparats.Add(preparat);
                
            }

            treeListView1.AddObjects(_preparats);

        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            foreach (var preparat in _preparats)
            {
                
                var files = Directory.GetFiles(_globalSettings.ProductsRepository, $"*{preparat.Number}*.pdf");
                if (files.Any())
                {
                    preparat.FileList.AddRange(files.ToList().Select(x=>new PreparatFile(x)));
                    treeListView1.RefreshObject(preparat);
                }
            }


            treeListView1.ExpandAll();
        }

        private void treeListView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeListView1.SelectedObject is PreparatFile file)
            {
                Process.Start(file.File.FullName);
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeListView1.SelectedObject is PreparatFile file)
            {
                file.IsSelected = !file.IsSelected;
            }
        }

        private void treeListView1_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            if (e.Model is PreparatFile file && file.IsSelected)
            {
                e.Item.BackColor = Color.CadetBlue;
                e.Item.ForeColor = Color.White;
            }
            else
            {
                e.Item.BackColor = default;
                e.Item.ForeColor = default;
                
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeListView1.SelectedObject is PreparatFile file)
            {
                var filePath = new StringCollection {file.File.FullName};
                try
                {
                    Clipboard.SetFileDropList(filePath);
                }
                catch { }

            }

           
        }

        private void toolStripButtonExpandAll_Click(object sender, EventArgs e)
        {
            treeListView1.ExpandAll();
        }
    }
}
