using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MuVaViMo.WPFDemo
{
    public class MainWindowViewModel : ObservableObject
    {
        private int _i;

        private readonly ObservableCollection<Model> _models = new ObservableCollection<Model>();

        public TransformingObservableCollectionWrapper<Model, ViewModel> ViewModels { get; }

        public ICommand IncreaseCommand => new AlwaysExecutableRelayCommand(obj => _models.Add(new Model(_i++)));
        public ICommand DecreaseCommand => new AlwaysExecutableRelayCommand(obj =>
                                                                            {
                                                                                if((int)obj > -1)
                                                                                    _models.RemoveAt((int) obj);
                                                                            });

        public MainWindowViewModel()
        {
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            ViewModels = new TransformingObservableCollectionWrapper<Model, ViewModel>(_models, model => new ViewModel(model));
        }
    }
}