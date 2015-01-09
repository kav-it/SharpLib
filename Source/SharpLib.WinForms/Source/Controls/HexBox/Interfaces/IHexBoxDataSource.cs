using System;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Класс доступа в массиву байт в компоненте HexBox
    /// </summary>
    public interface IHexBoxDataSource
    {
        #region Свойства

        /// <summary>
        /// Общий размер данных
        /// </summary>
        long Length { get; }

        /// <summary>
        /// true: данные изменены
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// true: Разрешена запись байт
        /// </summary>
        bool IsCanWriteByte { get; }

        /// <summary>
        /// true: Разрешена вставка байт
        /// </summary>
        bool IsCanInsertBytes { get; }

        /// <summary>
        /// true: Разрешено удаление байт
        /// </summary>
        bool IsCanDeleteBytes { get; }

        #endregion

        #region События

        /// <summary>
        /// Событие "Данные изменены"
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Событие "Размер данных изменен"
        /// </summary>
        event EventHandler LengthChanged;

        #endregion

        #region Методы

        /// <summary>
        /// Чтение байта
        /// </summary>
        byte ReadByte(long index);

        /// <summary>
        /// Запись байта
        /// </summary>
        void WriteByte(long index, byte value);

        /// <summary>
        /// Вставка байта (увеличение длины, если индекс больше размера)
        /// </summary>
        void InsertBytes(long index, byte[] values);

        /// <summary>
        /// Удаление байт
        /// </summary>
        void DeleteBytes(long index, long length);

        /// <summary>
        /// Примерение изменений
        /// </summary>
        void ApplyChanges();

        #endregion
    }
}