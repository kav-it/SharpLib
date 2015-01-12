namespace SharpLib
{
    public class BufferRange
    {
        /// <summary>
        /// Индекс начала блока
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// Индекс окончания блока
        /// </summary>
        public int End
        {
            get { return Start + Data.Length; }
        }

        /// <summary>
        /// Размер блока
        /// </summary>
        public int Size
        {
            get { return Data.Length; }

        }

        /// <summary>
        /// Данные блока
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferRange(int start, byte[] data)
        {
            Start = start;
            Data = data;
        }
    }
}