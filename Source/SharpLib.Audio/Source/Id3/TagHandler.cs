using System;
using System.Drawing;

using Id3Lib.Frames;

namespace Id3Lib
{
    internal class TagHandler
    {
        #region Поля

        private readonly string _language = "eng";

        private readonly TextCode _textCode = TextCode.Ascii;

        private TagModel _frameModel;

        #endregion

        #region Свойства

        public TagModel FrameModel
        {
            get { return _frameModel; }
            set { _frameModel = value; }
        }

        public string Song
        {
            get { return Title; }
            set { Title = value; }
        }

        public string Title
        {
            get { return GetTextFrame("TIT2"); }
            set { SetTextFrame("TIT2", value); }
        }

        public string Artist
        {
            get { return GetTextFrame("TPE1"); }
            set { SetTextFrame("TPE1", value); }
        }

        public string Album
        {
            get { return GetTextFrame("TALB"); }
            set { SetTextFrame("TALB", value); }
        }

        public string Year
        {
            get { return GetTextFrame("TYER"); }
            set { SetTextFrame("TYER", value); }
        }

        public string Composer
        {
            get { return GetTextFrame("TCOM"); }
            set { SetTextFrame("TCOM", value); }
        }

        public string Genre
        {
            get { return GetTextFrame("TCON"); }
            set { SetTextFrame("TCON", value); }
        }

        public string Track
        {
            get { return GetTextFrame("TRCK"); }
            set { SetTextFrame("TRCK", value); }
        }

        public string Disc
        {
            get { return GetTextFrame("TPOS"); }
            set { SetTextFrame("TPOS", value); }
        }

        public TimeSpan? Length
        {
            get
            {
                string strlen = GetTextFrame("TLEN");
                if (String.IsNullOrEmpty(strlen))
                {
                    return null;
                }

                int len;
                if (int.TryParse(strlen, out len))
                {
                    return new TimeSpan(0, 0, 0, 0, len);
                }
                return null;
            }
        }

        public uint PaddingSize
        {
            get { return _frameModel.Header.PaddingSize; }
        }

        public string Lyrics
        {
            get { return GetFullTextFrame("USLT"); }
            set { SetFullTextFrame("USLT", value); }
        }

        public string Comment
        {
            get { return GetFullTextFrame("COMM"); }
            set { SetFullTextFrame("COMM", value); }
        }

        public Image Picture
        {
            get
            {
                var frame = FindFrame("APIC") as FramePicture;
                return frame != null ? frame.Picture : null;
            }
            set
            {
                var frame = FindFrame("APIC") as FramePicture;
                if (frame != null)
                {
                    if (value != null)
                    {
                        frame.Picture = value;
                    }
                    else
                    {
                        _frameModel.Remove(frame);
                    }
                }
                else
                {
                    if (value != null)
                    {
                        var framePic = FrameFactory.Build("APIC") as FramePicture;
                        framePic.Picture = value;
                        _frameModel.Add(framePic);
                    }
                }
            }
        }

        #endregion

        #region Конструктор

        public TagHandler(TagModel frameModel)
        {
            _frameModel = frameModel;
        }

        #endregion

        #region Методы

        private void SetTextFrame(string frameId, string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    ((FrameText)frame).Text = message;
                }
                else
                {
                    _frameModel.Remove(frame);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(message))
                {
                    FrameText frameText = (FrameText)FrameFactory.Build(frameId);
                    frameText.Text = message;
                    frameText.TextCode = _textCode;
                    _frameModel.Add(frameText);
                }
            }
        }

        private string GetTextFrame(string frameId)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                return ((FrameText)frame).Text;
            }
            return string.Empty;
        }

        private void SetFullTextFrame(string frameId, string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    FrameFullText framefulltext = (FrameFullText)frame;
                    framefulltext.Text = message;
                    framefulltext.TextCode = _textCode;
                    framefulltext.Description = string.Empty;
                    framefulltext.Language = _language;
                }
                else
                {
                    _frameModel.Remove(frame);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(message))
                {
                    FrameFullText frameLCText = (FrameFullText)FrameFactory.Build(frameId);
                    frameLCText.TextCode = _textCode;
                    frameLCText.Language = "eng";
                    frameLCText.Description = string.Empty;
                    frameLCText.Text = message;
                    _frameModel.Add(frameLCText);
                }
            }
        }

        private string GetFullTextFrame(string frameId)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                return ((FrameFullText)frame).Text;
            }
            return string.Empty;
        }

        private FrameBase FindFrame(string frameId)
        {
            foreach (var frame in _frameModel)
            {
                if (frame.FrameId == frameId)
                {
                    return frame;
                }
            }
            return null;
        }

        #endregion
    }
}