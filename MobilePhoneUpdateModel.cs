namespace MobileShop
{
    public class MobilePhoneUpdateModel : MobilePhone
    {
        private string? _imagePath;

        public string? ImagePath 
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        public MobilePhoneUpdateModel()
        { }

        public MobilePhoneUpdateModel(MobilePhone mobilePhone)
        {
            Id = mobilePhone.Id;
            _productName = mobilePhone.ProductName;
            _manufacturer = mobilePhone.Manufacturer;
            _display = mobilePhone.Display;
            _network = mobilePhone.Network;
            _cpu = mobilePhone.CPU;
            _memory = mobilePhone.Memory;
            _camera = mobilePhone.Camera;
            _battery = mobilePhone.Battery;
            _count = mobilePhone.Count;
            _price = mobilePhone.Price;
            _imageBase64 = mobilePhone.ImageBase64;
        }
    }
}
