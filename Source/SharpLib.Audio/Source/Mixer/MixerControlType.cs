using System;

namespace NAudio.Mixer
{
    [Flags]
    internal enum MixerControlClass
    {
        Custom = 0x00000000,

        Meter = 0x10000000,

        Switch = 0x20000000,

        Number = 0x30000000,

        Slider = 0x40000000,

        Fader = 0x50000000,

        Time = 0x60000000,

        List = 0x70000000,

        Mask = Custom | Meter | Switch | Number | Slider | Fader | Time | List
    }

    [Flags]
    internal enum MixerControlSubclass
    {
        SwitchBoolean = 0x00000000,

        SwitchButton = 0x01000000,

        MeterPolled = 0x00000000,

        TimeMicrosecs = 0x00000000,

        TimeMillisecs = 0x01000000,

        ListSingle = 0x00000000,

        ListMultiple = 0x01000000,

        Mask = 0x0F000000
    }

    [Flags]
    internal enum MixerControlUnits
    {
        Custom = 0x00000000,

        Boolean = 0x00010000,

        Signed = 0x00020000,

        Unsigned = 0x00030000,

        Decibels = 0x00040000,

        Percent = 0x00050000,

        Mask = 0x00FF0000
    }

    internal enum MixerControlType
    {
        Custom = (MixerControlClass.Custom | MixerControlUnits.Custom),

        BooleanMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Boolean),

        SignedMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Signed),

        PeakMeter = (SignedMeter + 1),

        UnsignedMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Unsigned),

        Boolean = (MixerControlClass.Switch | MixerControlSubclass.SwitchBoolean | MixerControlUnits.Boolean),

        OnOff = (Boolean + 1),

        Mute = (Boolean + 2),

        Mono = (Boolean + 3),

        Loudness = (Boolean + 4),

        StereoEnhance = (Boolean + 5),

        Button = (MixerControlClass.Switch | MixerControlSubclass.SwitchButton | MixerControlUnits.Boolean),

        Decibels = (MixerControlClass.Number | MixerControlUnits.Decibels),

        Signed = (MixerControlClass.Number | MixerControlUnits.Signed),

        Unsigned = (MixerControlClass.Number | MixerControlUnits.Unsigned),

        Percent = (MixerControlClass.Number | MixerControlUnits.Percent),

        Slider = (MixerControlClass.Slider | MixerControlUnits.Signed),

        Pan = (Slider + 1),

        QSoundPan = (Slider + 2),

        Fader = (MixerControlClass.Fader | MixerControlUnits.Unsigned),

        Volume = (Fader + 1),

        Bass = (Fader + 2),

        Treble = (Fader + 3),

        Equalizer = (Fader + 4),

        SingleSelect = (MixerControlClass.List | MixerControlSubclass.ListSingle | MixerControlUnits.Boolean),

        Mux = (SingleSelect + 1),

        MultipleSelect = (MixerControlClass.List | MixerControlSubclass.ListMultiple | MixerControlUnits.Boolean),

        Mixer = (MultipleSelect + 1),

        MicroTime = (MixerControlClass.Time | MixerControlSubclass.TimeMicrosecs | MixerControlUnits.Unsigned),

        MilliTime = (MixerControlClass.Time | MixerControlSubclass.TimeMillisecs | MixerControlUnits.Unsigned),
    }
}