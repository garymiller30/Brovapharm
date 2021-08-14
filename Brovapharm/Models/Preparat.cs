using System.Collections.Generic;
using System.IO;

namespace Brovapharm.Models
{
    public class Preparat
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public List<PreparatFile> FileList { get; set; } = new List<PreparatFile>();

       
    }
}