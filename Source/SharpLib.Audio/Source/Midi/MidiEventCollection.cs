using System.Collections.Generic;

using NAudio.Utils;

namespace NAudio.Midi
{
    internal class MidiEventCollection : IEnumerable<IList<MidiEvent>>
    {
        #region Поля

        private readonly int deltaTicksPerQuarterNote;

        private readonly List<IList<MidiEvent>> trackEvents;

        private int midiFileType;

        #endregion

        #region Свойства

        public int Tracks
        {
            get { return trackEvents.Count; }
        }

        public long StartAbsoluteTime { get; set; }

        public int DeltaTicksPerQuarterNote
        {
            get { return deltaTicksPerQuarterNote; }
        }

        public IList<MidiEvent> this[int trackNumber]
        {
            get { return trackEvents[trackNumber]; }
        }

        public int MidiFileType
        {
            get { return midiFileType; }
            set
            {
                if (midiFileType != value)
                {
                    midiFileType = value;

                    if (value == 0)
                    {
                        FlattenToOneTrack();
                    }
                    else
                    {
                        ExplodeToManyTracks();
                    }
                }
            }
        }

        #endregion

        #region Конструктор

        public MidiEventCollection(int midiFileType, int deltaTicksPerQuarterNote)
        {
            this.midiFileType = midiFileType;
            this.deltaTicksPerQuarterNote = deltaTicksPerQuarterNote;
            StartAbsoluteTime = 0;
            trackEvents = new List<IList<MidiEvent>>();
        }

        #endregion

        #region Методы

        public IList<MidiEvent> GetTrackEvents(int trackNumber)
        {
            return trackEvents[trackNumber];
        }

        public IList<MidiEvent> AddTrack()
        {
            return AddTrack(null);
        }

        public IList<MidiEvent> AddTrack(IList<MidiEvent> initialEvents)
        {
            List<MidiEvent> events = new List<MidiEvent>();
            if (initialEvents != null)
            {
                events.AddRange(initialEvents);
            }
            trackEvents.Add(events);
            return events;
        }

        public void RemoveTrack(int track)
        {
            trackEvents.RemoveAt(track);
        }

        public void Clear()
        {
            trackEvents.Clear();
        }

        public void AddEvent(MidiEvent midiEvent, int originalTrack)
        {
            if (midiFileType == 0)
            {
                EnsureTracks(1);
                trackEvents[0].Add(midiEvent);
            }
            else
            {
                if (originalTrack == 0)
                {
                    switch (midiEvent.CommandCode)
                    {
                        case MidiCommandCode.NoteOff:
                        case MidiCommandCode.NoteOn:
                        case MidiCommandCode.KeyAfterTouch:
                        case MidiCommandCode.ControlChange:
                        case MidiCommandCode.PatchChange:
                        case MidiCommandCode.ChannelAfterTouch:
                        case MidiCommandCode.PitchWheelChange:
                            EnsureTracks(midiEvent.Channel + 1);
                            trackEvents[midiEvent.Channel].Add(midiEvent);
                            break;
                        default:
                            EnsureTracks(1);
                            trackEvents[0].Add(midiEvent);
                            break;
                    }
                }
                else
                {
                    EnsureTracks(originalTrack + 1);
                    trackEvents[originalTrack].Add(midiEvent);
                }
            }
        }

        private void EnsureTracks(int count)
        {
            for (int n = trackEvents.Count; n < count; n++)
            {
                trackEvents.Add(new List<MidiEvent>());
            }
        }

        private void ExplodeToManyTracks()
        {
            IList<MidiEvent> originalList = trackEvents[0];
            Clear();
            foreach (MidiEvent midiEvent in originalList)
            {
                AddEvent(midiEvent, 0);
            }
            PrepareForExport();
        }

        private void FlattenToOneTrack()
        {
            bool eventsAdded = false;
            for (int track = 1; track < trackEvents.Count; track++)
            {
                foreach (MidiEvent midiEvent in trackEvents[track])
                {
                    if (!MidiEvent.IsEndTrack(midiEvent))
                    {
                        trackEvents[0].Add(midiEvent);
                        eventsAdded = true;
                    }
                }
            }
            for (int track = trackEvents.Count - 1; track > 0; track--)
            {
                RemoveTrack(track);
            }
            if (eventsAdded)
            {
                PrepareForExport();
            }
        }

        public void PrepareForExport()
        {
            var comparer = new MidiEventComparer();

            foreach (List<MidiEvent> list in trackEvents)
            {
                MergeSort.Sort(list, comparer);

                int index = 0;
                while (index < list.Count - 1)
                {
                    if (MidiEvent.IsEndTrack(list[index]))
                    {
                        list.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            int track = 0;

            while (track < trackEvents.Count)
            {
                IList<MidiEvent> list = trackEvents[track];
                if (list.Count == 0)
                {
                    RemoveTrack(track);
                }
                else
                {
                    if (list.Count == 1 && MidiEvent.IsEndTrack(list[0]))
                    {
                        RemoveTrack(track);
                    }
                    else
                    {
                        if (!MidiEvent.IsEndTrack(list[list.Count - 1]))
                        {
                            list.Add(new MetaEvent(MetaEventType.EndTrack, 0, list[list.Count - 1].AbsoluteTime));
                        }
                        track++;
                    }
                }
            }
        }

        public IEnumerator<IList<MidiEvent>> GetEnumerator()
        {
            return trackEvents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return trackEvents.GetEnumerator();
        }

        #endregion
    }
}