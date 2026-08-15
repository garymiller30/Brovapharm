using System.Collections.Generic;
using System.Threading.Tasks;
using models.Models;

namespace models.Service
{
    /// <summary>
    /// Інтерфейс сервісу для роботи з буфером обміну
    /// </summary>
    public interface IPasteService
    {
        /// <summary>
        /// Отримати необроблений текст з буфера обміну
        /// </summary>
        string GetRawText();
        
        /// <summary>
        /// Отримати список препаратів з буфера обміну
        /// </summary>
        Task<List<Preparat>> GetPreparatsFromClipboardAsync();
    }
}
