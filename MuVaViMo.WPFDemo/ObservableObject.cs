using System.ComponentModel;
using System.Runtime.CompilerServices;
using MuVaViMo.WPFDemo.Properties;

namespace MuVaViMo.WPFDemo
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}