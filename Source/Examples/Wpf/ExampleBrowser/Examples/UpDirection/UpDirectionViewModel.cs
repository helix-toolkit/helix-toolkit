using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace UpDirection;

public sealed class UpDirectionViewModel : ObservableObject
{
    private Vector3D _upModel = new(0, 1, 0);

    public Vector3D UpModel
    {
        get
        {
            return _upModel;
        }

        set
        {
            _upModel = value;
            OnPropertyChanged(nameof(UpModel));
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
        }
    }

    public double X
    {
        get
        {
            return _upModel.X;
        }

        set
        {
            _upModel.X = value;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(UpModel));
        }
    }
    public double Y
    {

        get
        {
            return _upModel.Y;
        }

        set
        {
            _upModel.Y = value;
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(UpModel));

        }
    }
    public double Z
    {
        get
        {
            return _upModel.Z;
        }

        set
        {
            _upModel.Z = value;
            OnPropertyChanged(nameof(Z));
            OnPropertyChanged(nameof(UpModel));
        }
    }
}
