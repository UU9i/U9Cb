using U9Cb.Core;

namespace U9Cb.MVVM.ViewModel
{
    class MainViewModel:ObservableObject
    {
        public RelayCommand FileViewCommand { get; set; }
        public RelayCommand DirectoryViewCommand { get; set; }
        public RelayCommand CopyBViewCommand { get; set; }
        public RelayCommand AboutViewCommand { get; set; }
        public FileViewModel FileVm {get; set;}
        public DirectoryViewModel DirectoryVm { get; set; }
        public CopyBViewModel CopyBVm { get; set; }
        public AboutViewModel AboutVm { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            { 
                _currentView = value; 
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            FileVm=new FileViewModel();
            DirectoryVm = new DirectoryViewModel();
            CopyBVm = new CopyBViewModel();
            AboutVm = new AboutViewModel();
            CurrentView = FileVm;
            FileViewCommand = new RelayCommand(o => 
            { 
                CurrentView= FileVm;
            });
            DirectoryViewCommand = new RelayCommand(o =>
            {
                CurrentView = DirectoryVm;
            });
            CopyBViewCommand = new RelayCommand(o =>
            {
                CurrentView = CopyBVm;
            });
            AboutViewCommand = new RelayCommand(o =>
            {
                CurrentView = AboutVm;
            });
        }
    }
}
