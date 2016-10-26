using System.Windows.Media;

namespace MuVaViMo.WPFDemo
{
    public class ViewModel : ObservableObject
    {
        private readonly Model _model;

        public int Number
        {
            get { return _model.Number; }
            set
            {
                if(_model.Number == value) return;
                _model.Number = value;
                OnPropertyChanged();
            }
        }

        public Brush Color => _model.Number % 2 == 0 ? Brushes.Purple : Brushes.Blue;

        public ViewModel(Model model)
        {
            _model = model;
        }
    }
}