// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System.Xml.Serialization;

namespace SharpLib.Log
{
    [XmlInclude(typeof(LoggerConfigTargetFile))]
    public class LoggerConfigTarget
    {
    }
}