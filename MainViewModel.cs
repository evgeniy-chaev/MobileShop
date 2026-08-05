using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Threading;

namespace MobileShop
{
    public class MainViewModel
    {
        public ObservableCollection<MobilePhone> MobilePhones { get; private set; }

        public MobilePhone? SelectedPhone { get; set; }

        public string? NameFilter
        {
            get => _nameFilter;
            set
            {
                _nameFilter = value;
                RestartNameFiltering(_nameFilter);
            }
        }

        private readonly IRepository<MobilePhone> _repository;
        private RelayCommand? _createCommand;
        private RelayCommand? _updateCommand;
        private RelayCommand? _deleteCommand;
        private string? _nameFilter;
        private Task? _filteringTask;
        private CancellationTokenSource _filteringCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Для обновления интерфейса окна из параллельных потоков
        /// </summary>
        private readonly Dispatcher _currentDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// .ctor
        /// </summary>
        public MainViewModel()
        {
            _repository = App.AppHost.Services.GetRequiredService<IRepository<MobilePhone>>();

            var mobilePhonesList = _repository.GetAll().ToList();
            mobilePhonesList.Sort();
            MobilePhones = new ObservableCollection<MobilePhone>(mobilePhonesList);
        }

        public RelayCommand CreateCommand
        {
            get
            {
                return _createCommand ??
                    (_createCommand = new RelayCommand((o) =>
                    {
                        var updateWindow = new MobilePhoneUpdateWindow(new MobilePhoneUpdateModel());
                        if (updateWindow.ShowDialog() == true)
                        {

                            var newPhone = new MobilePhone()
                            {
                                ProductName = updateWindow.MobilePhone.ProductName,
                                Manufacturer = updateWindow.MobilePhone.Manufacturer,
                                Display = updateWindow.MobilePhone.Display,
                                Network = updateWindow.MobilePhone.Network,
                                CPU = updateWindow.MobilePhone.CPU,
                                Memory = updateWindow.MobilePhone.Memory,
                                Camera = updateWindow.MobilePhone.Camera,
                                Battery = updateWindow.MobilePhone.Battery,
                                Count = updateWindow.MobilePhone.Count,
                                Price = updateWindow.MobilePhone.Price
                            };

                            if (!string.IsNullOrWhiteSpace(updateWindow.MobilePhone.ImagePath))
                            {
                                using (Image image = Image.FromFile(updateWindow.MobilePhone.ImagePath))
                                {
                                    newPhone.ImageBase64 = ImageHelper.GetEncodedImage(image);
                                }
                            }

                            _repository.Create(newPhone);
                            RestartNameFiltering(_nameFilter);
                        }
                    }));
            }
        }

        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand ??
                    (_updateCommand = new RelayCommand((selectedItem) =>
                    {
                        // получаем выделенный объект
                        MobilePhone? mobilePhone = selectedItem as MobilePhone;
                        if (mobilePhone == null) return;

                        var updateModel = new MobilePhoneUpdateModel(mobilePhone);
                        var updateWindow = new MobilePhoneUpdateWindow(updateModel);


                        if (updateWindow.ShowDialog() == true)
                        {
                            mobilePhone.ProductName = updateWindow.MobilePhone.ProductName;
                            mobilePhone.Manufacturer = updateWindow.MobilePhone.Manufacturer;
                            mobilePhone.Display = updateWindow.MobilePhone.Display;
                            mobilePhone.Network = updateWindow.MobilePhone.Network;
                            mobilePhone.CPU = updateWindow.MobilePhone.CPU;
                            mobilePhone.Memory = updateWindow.MobilePhone.Memory;
                            mobilePhone.Camera = updateWindow.MobilePhone.Camera;
                            mobilePhone.Battery = updateWindow.MobilePhone.Battery;
                            mobilePhone.Count = updateWindow.MobilePhone.Count;
                            mobilePhone.Price = updateWindow.MobilePhone.Price;

                            if (!string.IsNullOrWhiteSpace(updateWindow.MobilePhone.ImagePath))
                            {
                                using (Image image = Image.FromFile(updateWindow.MobilePhone.ImagePath))
                                {
                                    mobilePhone.ImageBase64 = ImageHelper.GetEncodedImage(image);
                                }
                            }

                            _repository.Update(mobilePhone);
                        }
                    }));
            }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                    (_deleteCommand = new RelayCommand((selectedItem) =>
                    {
                        MobilePhone? mobilePhone = selectedItem as MobilePhone;
                        if (mobilePhone == null) return;

                        _repository.Delete(mobilePhone.Id);
                        MobilePhones.Remove(mobilePhone);
                    }));
            }
        }

        /// <summary>
        /// Перезапуск фильтрации по названию
        /// </summary>
        /// <param name="filter">фильтр</param>
        private void RestartNameFiltering(string? filter)
        {
            // если предыдущая фильтрация завершилась / отменена / упала с ошибкой / ещё не было
            if (_filteringTask == null
                || (_filteringTask.Status != TaskStatus.Running
                    && _filteringTask.Status != TaskStatus.WaitingToRun
                    && _filteringTask.Status != TaskStatus.WaitingForActivation))
            {
                _filteringCancellationTokenSource = new CancellationTokenSource();
                _filteringTask = NameFilteringAsync(filter, _filteringCancellationTokenSource);
            }
            // если предыдущая фильтрация в процессе и не отменялась
            else if (!_filteringCancellationTokenSource.IsCancellationRequested)
            {
                var newCancellationTokenSource = new CancellationTokenSource();
                _filteringTask.ContinueWith(
                    t =>
                    {
                        _filteringTask = NameFilteringAsync(filter, newCancellationTokenSource);
                    });
                _filteringCancellationTokenSource.Cancel();
                _filteringCancellationTokenSource = newCancellationTokenSource;
            }
        }

        private async Task NameFilteringAsync(string? filter, CancellationTokenSource cancellationTokenSource)
        {
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                List<MobilePhone> filteredList;

                if (string.IsNullOrEmpty(filter))
                {
                    filteredList = (await _repository.GetAllAsync(cancellationToken))
                        .ToList();
                }
                else
                {
                    filteredList = await _repository.GetNameFilteredAsync(
                            filter, cancellationToken)
                        .ToListAsync(cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();

                filteredList.Sort();

                cancellationToken.ThrowIfCancellationRequested();

                await _currentDispatcher.InvokeAsync(() =>
                {
                    MobilePhones.Clear();
                    foreach (var mobilePhone in filteredList)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        MobilePhones.Add(mobilePhone);
                    }
                });

                // отметка о том, что фильтрация завершена
                _filteringTask = null;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
