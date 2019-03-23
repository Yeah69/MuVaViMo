namespace MuVaViMo.WPFDemo
{
    public class Model : ObservableObject
    {
        private int _number;

        public int Number
        {
            get => _number;
            set
            {
                if(_number == value) return;

                _number = value;
                OnPropertyChanged();
            }
        }

        public Model(int number)
        {
            _number = number;
        }
    }

    public class ModelA : Model
    {
        public ModelA(int number) : base(number) { }
    }

    public class ModelB : Model
    {
        public ModelB(int number) : base(number) { }
    }
}