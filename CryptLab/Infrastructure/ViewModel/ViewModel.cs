using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptLab.Infrastructure.ViewModel;

public abstract class ViewModel : INotifyPropertyChanged, IDisposable
{
    private bool _disposed;

    public event PropertyChangedEventHandler? PropertyChanged;
        
    public void Dispose()
    {
        Dispose(true);
    }
        
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _disposed) return;

        _disposed = true;
        //Освобождение управляемых ресурсов
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        if (Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(PropertyName);

        return true;
    }
}