using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Провайдер данных для больших файлов (более 100 МБ)
    /// </summary>
    public class HexBoxFileDataSource : IHexBoxDataSource, IDisposable
    {
        #region Поля

        /// <summary>
        /// true: Только чтение
        /// </summary>
        private readonly bool _readOnly;

        /// <summary>
        /// Коллекция всех изменений
        /// </summary>
        private readonly Dictionary<long, byte> _writes;

        /// <summary>
        /// Поток данных файла
        /// </summary>
        private FileStream _fileStream;

        #endregion

        #region Свойства

        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Размер данных
        /// </summary>
        public long Length
        {
            get { return _fileStream.Length; }
        }

        /// <summary>
        /// true: данные изменены
        /// </summary>
        public bool IsChanged
        {
            get { return (_writes.Count > 0); }
        }

        /// <summary>
        /// true: Разрешена запись байт
        /// </summary>
        public bool IsCanWriteByte
        {
            get { return !_readOnly; }
        }

        /// <summary>
        /// true: Разрешена вставка байт
        /// </summary>
        public bool IsCanInsertBytes
        {
            get { return false; }
        }

        /// <summary>
        /// true: Разрешено удаление байт
        /// </summary>
        public bool IsCanDeleteBytes
        {
            get { return false; }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Изменены данные"
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public HexBoxFileDataSource(string fileName)
        {
            _writes = new Dictionary<long, byte>();

            FileName = fileName;

            try
            {
                // Попытка открыть файл в режиме "Запись"
                _fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }
            catch
            {
                // Открытие файла в режиме "Read" и "Shared"
                _fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _readOnly = true;
            }
        }

        /// <summary>
        /// Паттер Dispose (не используется unmanaged)
        /// </summary>
        ~HexBoxFileDataSource()
        {
            Dispose();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Генерация события "Changed"
        /// </summary>
        private void RaiseChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public void ApplyChanges()
        {
            if (_readOnly)
            {
                throw new Exception("File is in read-only mode.");
            }

            if (IsChanged == false)
            {
                return;
            }

            // Выполнение записи в файл
            foreach (var entry in _writes)
            {
                var index = entry.Key;
                var value = entry.Value;

                _fileStream.Position = index;
                _fileStream.Write(new[] { value }, 0, 1);
            }

            _writes.Clear();
        }

        /// <summary>
        /// Отмена изменений
        /// </summary>
        public void RejectChanges()
        {
            _writes.Clear();
        }

        /// <summary>
        /// Чтение байта из файла
        /// </summary>
        public byte ReadByte(long index)
        {
            if (_writes.ContainsKey(index))
            {
                return _writes[index];
            }

            _fileStream.Position = index;

            byte res = (byte)_fileStream.ReadByte();

            return res;
        }

        /// <summary>
        /// Запись байта
        /// </summary>
        public void WriteByte(long index, byte value)
        {
            if (_writes.ContainsKey(index))
            {
                _writes[index] = value;
            }
            else
            {
                _writes.Add(index, value);
            }

            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Удаление байта (не поддерживается)
        /// </summary>
        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException("HexBoxFileDataSource.DeleteBytes");
        }

        /// <summary>
        /// Вставка байта (не поддерживается)
        /// </summary>
        public void InsertBytes(long index, byte[] bs)
        {
            throw new NotSupportedException("HexBoxFileDataSource.InsertBytes");
        }

        /// <summary>
        /// Releases the file handle used by the HexBoxFileDataSource.
        /// </summary>
        public void Dispose()
        {
            if (_fileStream != null)
            {
                FileName = null;

                _fileStream.Close();
                _fileStream = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}