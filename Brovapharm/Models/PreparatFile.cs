﻿using iTextSharp.text.pdf;
using System.IO;

namespace Brovapharm.Models
{
    public class PreparatFile
    {
        public bool IsSelected { get; set; }
        public FileInfo File { get; set; }

        public int CntPages {get;set;}

        public PreparatFile(string file)
        {
            File = new FileInfo(file);

            using (var reader = new PdfReader(file))
            {
                CntPages = reader.NumberOfPages;
            }
        }
    }
}