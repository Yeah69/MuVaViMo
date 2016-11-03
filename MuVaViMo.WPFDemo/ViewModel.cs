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

        public virtual Brush Color => _model.Number % 2 == 0 ? Brushes.Purple : Brushes.Blue;

        public ViewModel(Model model)
        {
            _model = model;
        }
    }

    public class ViewModelA : ViewModel
    {
        public ViewModelA(ModelA model) : base(model) { }

        public override Brush Color => Brushes.Chocolate;
    }

    public class ViewModelB : ViewModel
    {
        public ViewModelB(ModelB model) : base(model) { }

        public override Brush Color => Brushes.Lime;
    }
}