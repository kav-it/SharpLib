using System;

namespace NAudio.SoundFont
{
    internal enum TransformEnum
    {
        Linear = 0
    }

    internal class Modulator
    {
        #region Поля

        private short amount;

        private GeneratorEnum destinationGenerator;

        private ModulatorType sourceModulationAmount;

        private ModulatorType sourceModulationData;

        private TransformEnum sourceTransform;

        #endregion

        #region Свойства

        public ModulatorType SourceModulationData
        {
            get { return sourceModulationData; }
            set { sourceModulationData = value; }
        }

        public GeneratorEnum DestinationGenerator
        {
            get { return destinationGenerator; }
            set { destinationGenerator = value; }
        }

        public short Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public ModulatorType SourceModulationAmount
        {
            get { return sourceModulationAmount; }
            set { sourceModulationAmount = value; }
        }

        public TransformEnum SourceTransform
        {
            get { return sourceTransform; }
            set { sourceTransform = value; }
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("Modulator {0} {1} {2} {3} {4}",
                sourceModulationData, destinationGenerator,
                amount, sourceModulationAmount, sourceTransform);
        }

        #endregion
    }
}