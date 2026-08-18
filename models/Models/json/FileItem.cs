namespace models.Models.json
{
    public class FileItem
    {
        public string path { get;set;}
        public int cntPages { get;set;}
        /// <summary>
        /// порядковий номер препарату в таблиці
        /// </summary>
        public int number { get;set;}
        public string preparatNumber { get; set; }
        public string preparatName { get; set; }
        public int quantity { get; set; }
    }
}
