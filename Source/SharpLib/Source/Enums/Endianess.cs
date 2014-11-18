namespace SharpLib
{
    /// <summary>
    /// Порядок байт
    /// </summary>
    public enum Endianess
    {
        /// <summary>
        /// Порядок от младшего к старшему (0x0100 => 1) 
        /// </summary>
        Little = 0,

        /// <summary>
        /// Порядок от старшего к младшему (0x0100 => 256) 
        /// </summary>
        Big = 1
    }
}