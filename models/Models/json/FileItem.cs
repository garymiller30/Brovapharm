using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models.Models.json
{
    public class FileItem
    {
        public string path { get;set;}
        public int cntPages { get;set;}
        public int number { get;set;}
        public string preparatNumber { get; set; }
        public string preparatName { get; set; }
    }
}
