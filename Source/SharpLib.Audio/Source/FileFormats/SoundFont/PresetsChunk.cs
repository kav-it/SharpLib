using System;
using System.IO;
using System.Text;

namespace NAudio.SoundFont
{
    internal class PresetsChunk
    {
        #region Поля

        private readonly GeneratorBuilder instrumentZoneGenerators = new GeneratorBuilder();

        private readonly ModulatorBuilder instrumentZoneModulators = new ModulatorBuilder();

        private readonly ZoneBuilder instrumentZones = new ZoneBuilder();

        private readonly InstrumentBuilder instruments = new InstrumentBuilder();

        private readonly PresetBuilder presetHeaders = new PresetBuilder();

        private readonly GeneratorBuilder presetZoneGenerators = new GeneratorBuilder();

        private readonly ModulatorBuilder presetZoneModulators = new ModulatorBuilder();

        private readonly ZoneBuilder presetZones = new ZoneBuilder();

        private readonly SampleHeaderBuilder sampleHeaders = new SampleHeaderBuilder();

        #endregion

        #region Свойства

        public Preset[] Presets
        {
            get { return presetHeaders.Presets; }
        }

        public Instrument[] Instruments
        {
            get { return instruments.Instruments; }
        }

        public SampleHeader[] SampleHeaders
        {
            get { return sampleHeaders.SampleHeaders; }
        }

        #endregion

        #region Конструктор

        internal PresetsChunk(RiffChunk chunk)
        {
            string header = chunk.ReadChunkID();
            if (header != "pdta")
            {
                throw new InvalidDataException(String.Format("Not a presets data chunk ({0})", header));
            }

            RiffChunk c;
            while ((c = chunk.GetNextSubChunk()) != null)
            {
                switch (c.ChunkID)
                {
                    case "PHDR":
                    case "phdr":
                        c.GetDataAsStructureArray(presetHeaders);
                        break;
                    case "PBAG":
                    case "pbag":
                        c.GetDataAsStructureArray(presetZones);
                        break;
                    case "PMOD":
                    case "pmod":
                        c.GetDataAsStructureArray(presetZoneModulators);
                        break;
                    case "PGEN":
                    case "pgen":
                        c.GetDataAsStructureArray(presetZoneGenerators);
                        break;
                    case "INST":
                    case "inst":
                        c.GetDataAsStructureArray(instruments);
                        break;
                    case "IBAG":
                    case "ibag":
                        c.GetDataAsStructureArray(instrumentZones);
                        break;
                    case "IMOD":
                    case "imod":
                        c.GetDataAsStructureArray(instrumentZoneModulators);
                        break;
                    case "IGEN":
                    case "igen":
                        c.GetDataAsStructureArray(instrumentZoneGenerators);
                        break;
                    case "SHDR":
                    case "shdr":
                        c.GetDataAsStructureArray(sampleHeaders);
                        break;
                    default:
                        throw new InvalidDataException(String.Format("Unknown chunk type {0}", c.ChunkID));
                }
            }

            instrumentZoneGenerators.Load(sampleHeaders.SampleHeaders);
            instrumentZones.Load(instrumentZoneModulators.Modulators, instrumentZoneGenerators.Generators);
            instruments.LoadZones(instrumentZones.Zones);
            presetZoneGenerators.Load(instruments.Instruments);
            presetZones.Load(presetZoneModulators.Modulators, presetZoneGenerators.Generators);
            presetHeaders.LoadZones(presetZones.Zones);
            sampleHeaders.RemoveEOS();
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Preset Headers:\r\n");
            foreach (Preset p in presetHeaders.Presets)
            {
                sb.AppendFormat("{0}\r\n", p);
            }
            sb.Append("Instruments:\r\n");
            foreach (Instrument i in instruments.Instruments)
            {
                sb.AppendFormat("{0}\r\n", i);
            }
            return sb.ToString();
        }

        #endregion
    }
}