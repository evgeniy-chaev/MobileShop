using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileShop
{
    public class MobilePhone : INotifyPropertyChanged, IComparable
    {
        protected string _productName;
        protected string _manufacturer;
        protected string _display;
        protected string _network;
        protected string _cpu;
        protected string _memory;
        protected string _camera;
        protected string _battery;
        protected int _count;
        protected decimal _price;
        protected string _imageBase64;

        public int Id { get; set; }

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged("ProductName");
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged("Manufacturer");
            }
        }

        public string Display
        {
            get => _display;
            set
            {
                _display = value;
                OnPropertyChanged("Display");
            }
        }

        public string Network
        {
            get => _network;
            set
            {
                _network = value;
                OnPropertyChanged("Network");
            }
        }

        public string CPU
        {
            get => _cpu;
            set
            {
                _cpu = value;
                OnPropertyChanged("CPU");
            }
        }
        
        public string Memory
        {
            get => _memory;
            set
            {
                _memory = value;
                OnPropertyChanged("Memory");
            }
        }

        public string Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                OnPropertyChanged("Camera");
            }
        }

        public string Battery
        {
            get => _battery;
            set
            {
                _battery = value;
                OnPropertyChanged("Battery");
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public string ImageBase64
        {
            get => _imageBase64;
            set
            {
                _imageBase64 = value;
                OnPropertyChanged("ImageBase64");
            }
        }

        public MobilePhone()
        { }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public int CompareTo(object? obj)
        {
            var otherMobilePhone = obj as MobilePhone;
            if (otherMobilePhone == null)
                throw new ArgumentException($"Object is not a {nameof(MobilePhone)}");

            return _productName.CompareTo(otherMobilePhone._productName);
        }
    }
}
