using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Провайде данных для небольших блоков данных (менее 10 Мегабайт)
    /// </summary>
    public class HexBoxBufferProvider : IByteProvider
    {
        #region Поля

        /// <summary>
        /// Коллекция байт
        /// </summary>
        private readonly List<byte> _bytes;

        #endregion

        #region Свойства

        /// <summary>
        /// Коллекция байт
        /// </summary>
        public ReadOnlyCollection<Byte> Bytes
        {
            get { return _bytes.AsReadOnly(); }
        }

        /// <summary>
        /// Размер данных
        /// </summary>
        public long Length
        {
            get { return _bytes.Count; }
        }

        /// <summary>
        /// true: данные изменены
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// true: Разрешена запись байт
        /// </summary>
        public bool IsCanWriteByte
        {
            get { return true; }
        }

        /// <summary>
        /// true: Разрешена вставка байт
        /// </summary>
        public bool IsCanInsertBytes
        {
            get { return true; }
        }

        /// <summary>
        /// true: Разрешено удаление байт
        /// </summary>
        public bool IsCanDeleteBytes
        {
            get { return true; }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Изменены данные"
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Событие "Изменены размер данных"
        /// </summary>
        public event EventHandler LengthChanged;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public HexBoxBufferProvider(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToList();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Генерация события "Changed"
        /// </summary>
        private void RaiseChanged(EventArgs e)
        {
            IsChanged = true;

            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// Генерация события "LengthChanged"
        /// </summary>
        private void RaiseLengthChanged(EventArgs e)
        {
            if (LengthChanged != null)
            {
                LengthChanged(this, e);
            }
        }

        /// <summary>
        /// Примерение изменений (сброс флага Modify)
        /// </summary>
        public void ApplyChanges()
        {
            IsChanged = false;
        }

        /// <summary>
        /// Чтение байта по указанному смещению
        /// </summary>
        public byte ReadByte(long index)
        {
            return _bytes[(int)index];
        }

        /// <summary>
        /// Запись байта по указанном смещению
        /// </summary>
        public void WriteByte(long index, byte value)
        {
            _bytes[(int)index] = value;
            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Удаление байт из коллекции
        /// </summary>
        public void DeleteBytes(long index, long length)
        {
            int internalIndex = (int)Math.Max(0, index);
            int internalLength = (int)Math.Min((int)Length, length);
            _bytes.RemoveRange(internalIndex, internalLength);

            RaiseLengthChanged(EventArgs.Empty);
            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Вставка байт в коллекцию
        /// </summary>
        public void InsertBytes(long index, byte[] bs)
        {
            _bytes.InsertRange((int)index, bs);

            RaiseLengthChanged(EventArgs.Empty);
            RaiseChanged(EventArgs.Empty);
        }

        #endregion
    }
}