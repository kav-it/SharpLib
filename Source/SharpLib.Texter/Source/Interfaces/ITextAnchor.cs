﻿using System;

namespace SharpLib.Texter.Document
{
    public interface ITextAnchor
    {
        #region Свойства

        TextLocation Location { get; }

        int Offset { get; }

        AnchorMovementType MovementType { get; set; }

        bool SurviveDeletion { get; set; }

        bool IsDeleted { get; }

        int Line { get; }

        int Column { get; }

        #endregion

        #region События

        event EventHandler Deleted;

        #endregion
    }
}