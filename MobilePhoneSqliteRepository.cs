using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace MobileShop
{
    public class MobilePhoneSqliteRepository : IRepository<MobilePhone>
    {
        private bool _disposedValue;
        private readonly MobilePhoneSqliteContext _db;

        public MobilePhoneSqliteRepository()
        {
            _db = new MobilePhoneSqliteContext();
            _db.Database.EnsureCreated();
        }

        public IEnumerable<MobilePhone> GetAll()
        {
            _db.MobilePhones.Load();
            return _db.MobilePhones.Local;
        }

        public async Task<IEnumerable<MobilePhone>> GetAllAsync(CancellationToken cancellationToken)
        {
            await _db.MobilePhones.LoadAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return _db.MobilePhones.Local;
        }

        public async IAsyncEnumerable<MobilePhone> GetNameFilteredAsync(string filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (filter.Contains('\'') || filter.Contains('"'))
                throw new ArgumentException(null, nameof(filter));

            var command = new SqliteCommand("", _db.Connection);

            command.CommandText = @"
                SELECT Id FROM MobilePhones
                WHERE ProductName LIKE '%" + filter + @"%'; ";

            var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var id = reader.GetInt32(0);
                yield return _db.MobilePhones.Find(id);
            }
        }

        public void Create(MobilePhone item)
        {
            _db.MobilePhones.Add(item);
            _db.SaveChanges();
        }

        public void Update(MobilePhone item)
        {
            var mobilePhone = _db.MobilePhones.Find(item.Id);
            if (mobilePhone != null)
            {
                mobilePhone.ProductName = item.ProductName;
                mobilePhone.Manufacturer = item.Manufacturer;
                mobilePhone.Display = item.Display;
                mobilePhone.Network = item.Network;
                mobilePhone.CPU = item.CPU;
                mobilePhone.Memory = item.Memory;
                mobilePhone.Camera = item.Camera;
                mobilePhone.Battery = item.Battery;
                mobilePhone.Count = item.Count;
                mobilePhone.Price = item.Price;
                mobilePhone.ImageBase64 = item.ImageBase64;
                _db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var mobilePhone = _db.MobilePhones.Find(id);
            if (mobilePhone is null) return;
            _db.MobilePhones.Remove(mobilePhone);
            _db.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _db.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
