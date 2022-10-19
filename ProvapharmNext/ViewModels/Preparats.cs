using models.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvapharmNext.ViewModels
{
    public static class Preparats
    {
        public static ObservableCollection<Preparat> PreparatList {get;} = new ObservableCollection<Preparat>();
    }
}
