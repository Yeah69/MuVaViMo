namespace MuVaViMo.WPFDemo
{
    public class Model : ObservableObject
    {
        private int _number;

        public int Number
        {
            get { return _number; }
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
}