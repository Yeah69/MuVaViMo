using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MuVaViMo.WPFDemo
{
    public class MainWindowViewModel : ObservableObject
    {
        private int _i, _a, _b;            

        private readonly ObservableCollection<Model> _models = new ObservableCollection<Model>();

        public TransformingObservableReadOnlyList<Model, ViewModel> ViewModels { get; }

        public ObservableCollection<ModelA> ModelsA { get; }

        public ObservableCollection<ModelB> ModelsB { get; }

        public ConcatenatingObservableReadOnlyList<ViewModel> ViewModelsAB { get; }

        public ICommand IncreaseCommand => new AlwaysExecutableRelayCommand(obj => _models.Add(new Model(_i++)));
        public ICommand DecreaseCommand => new AlwaysExecutableRelayCommand(obj =>
        {
            if ((int)obj > -1)
                _models.RemoveAt((int)obj);
        });
        public ICommand ClearCommand => new AlwaysExecutableRelayCommand(obj =>
        {
            _models.Clear();
            _i = 0;
        });

        public ICommand IncreaseACommand => new AlwaysExecutableRelayCommand(obj => ModelsA.Add(new ModelA(_a++)));
        public ICommand DecreaseACommand => new AlwaysExecutableRelayCommand(obj =>
        {
            if ((int)obj > -1)
                ModelsA.RemoveAt((int)obj);
        });
        public ICommand ClearACommand => new AlwaysExecutableRelayCommand(obj =>
        {
            ModelsA.Clear();
            _a = 0;
        });

        public ICommand IncreaseBCommand => new AlwaysExecutableRelayCommand(obj => ModelsB.Add(new ModelB(_b++)));
        public ICommand DecreaseBCommand => new AlwaysExecutableRelayCommand(obj =>
        {
            if ((int)obj > -1)
                ModelsB.RemoveAt((int)obj);
        });
        public ICommand ClearBCommand => new AlwaysExecutableRelayCommand(obj =>
        {
            ModelsB.Clear();
            _b = 0;
        });

        public MainWindowViewModel()
        {
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));
            ViewModels = new TransformingObservableReadOnlyList<Model, ViewModel>(_models, model => new ViewModel(model));
            _models.Add(new Model(_i++));
            _models.Add(new Model(_i++));

            ModelsA = new ObservableCollection<ModelA>();
            ModelsB = new ObservableCollection<ModelB>();
            ModelsA.Add(new ModelA(_a++));
            ModelsA.Add(new ModelA(_a++));
            ModelsA.Add(new ModelA(_a++));
            ModelsA.Add(new ModelA(_a++));
            ModelsB.Add(new ModelB(_b++));
            ModelsB.Add(new ModelB(_b++));
            WrappingObservableReadOnlyList<ModelA> wrapA = new WrappingObservableReadOnlyList<ModelA>(ModelsA);
            WrappingObservableReadOnlyList<ModelB> wrapB = new WrappingObservableReadOnlyList<ModelB>(ModelsB);
            ModelsA.Add(new ModelA(_a++));
            ModelsB.Add(new ModelB(_b++));
            TransformingObservableReadOnlyList<ModelA, ViewModelA> transA =
                new TransformingObservableReadOnlyList<ModelA, ViewModelA>(wrapA, a => new ViewModelA(a));
            TransformingObservableReadOnlyList<ModelB, ViewModelB> transB =
                new TransformingObservableReadOnlyList<ModelB, ViewModelB>(wrapB, b => new ViewModelB(b));
            ModelsA.Add(new ModelA(_a++));
            ModelsB.Add(new ModelB(_b++));
            ViewModelsAB = new ConcatenatingObservableReadOnlyList<ViewModel>(transA, transB);
            ModelsA.Add(new ModelA(_a++));
            ModelsB.Add(new ModelB(_b++));
        }
    }
}