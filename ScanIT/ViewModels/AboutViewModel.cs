using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScanIT.ViewModels
{

    public class AboutViewModel : BaseViewModel
    {

        private string _author;
        public string author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _dateOfCreation;
        public string dateOfCreation
        {
            get { return _dateOfCreation; }
            set
            {
                if (_dateOfCreation != value)
                {
                    _dateOfCreation = value;
                    OnPropertyChanged();
                }
            }
        }

        public AboutViewModel()
        {
            
            author = "Created by Kamil Kaczmarek";
            dateOfCreation = "Date of Creation: January 2024";

        }

    }

}