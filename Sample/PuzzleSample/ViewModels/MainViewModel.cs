using System;
using System.Collections.ObjectModel;
using System.Net;

namespace PuzzleSample.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        ObservableCollection<object> _imageCollection;
        public ObservableCollection<object> ImageCollection
        {
            get { return _imageCollection; }
            set
            {
                _imageCollection = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
        }
    }
}
