using System;

namespace SharpLib.Audio.Dsp
{
    internal class EnvelopeGenerator
    {
        #region Перечисления

        public enum EnvelopeState
        {
            Idle = 0,

            Attack,

            Decay,

            Sustain,

            Release
        };

        #endregion

        #region Поля

        private float attackBase;

        private float attackCoef;

        private float attackRate;

        private float decayBase;

        private float decayCoef;

        private float decayRate;

        private float output;

        private float releaseBase;

        private float releaseCoef;

        private float releaseRate;

        private EnvelopeState state;

        private float sustainLevel;

        private float targetRatioAttack;

        private float targetRatioDecayRelease;

        #endregion

        #region Свойства

        public float AttackRate
        {
            get { return attackRate; }
            set
            {
                attackRate = value;
                attackCoef = CalcCoef(value, targetRatioAttack);
                attackBase = (1.0f + targetRatioAttack) * (1.0f - attackCoef);
            }
        }

        public float DecayRate
        {
            get { return decayRate; }
            set
            {
                decayRate = value;
                decayCoef = CalcCoef(value, targetRatioDecayRelease);
                decayBase = (sustainLevel - targetRatioDecayRelease) * (1.0f - decayCoef);
            }
        }

        public float ReleaseRate
        {
            get { return releaseRate; }
            set
            {
                releaseRate = value;
                releaseCoef = CalcCoef(value, targetRatioDecayRelease);
                releaseBase = -targetRatioDecayRelease * (1.0f - releaseCoef);
            }
        }

        public float SustainLevel
        {
            get { return sustainLevel; }
            set
            {
                sustainLevel = value;
                decayBase = (sustainLevel - targetRatioDecayRelease) * (1.0f - decayCoef);
            }
        }

        public EnvelopeState State
        {
            get { return state; }
        }

        #endregion

        #region Конструктор

        public EnvelopeGenerator()
        {
            Reset();
            AttackRate = 0;
            DecayRate = 0;
            ReleaseRate = 0;
            SustainLevel = 1.0f;
            SetTargetRatioAttack(0.3f);
            SetTargetRatioDecayRelease(0.0001f);
        }

        #endregion

        #region Методы

        private static float CalcCoef(float rate, float targetRatio)
        {
            return (float)Math.Exp(-Math.Log((1.0f + targetRatio) / targetRatio) / rate);
        }

        private void SetTargetRatioAttack(float targetRatio)
        {
            if (targetRatio < 0.000000001f)
            {
                targetRatio = 0.000000001f;
            }
            targetRatioAttack = targetRatio;
            attackBase = (1.0f + targetRatioAttack) * (1.0f - attackCoef);
        }

        private void SetTargetRatioDecayRelease(float targetRatio)
        {
            if (targetRatio < 0.000000001f)
            {
                targetRatio = 0.000000001f;
            }
            targetRatioDecayRelease = targetRatio;
            decayBase = (sustainLevel - targetRatioDecayRelease) * (1.0f - decayCoef);
            releaseBase = -targetRatioDecayRelease * (1.0f - releaseCoef);
        }

        public float Process()
        {
            switch (state)
            {
                case EnvelopeState.Idle:
                    break;
                case EnvelopeState.Attack:
                    output = attackBase + output * attackCoef;
                    if (output >= 1.0f)
                    {
                        output = 1.0f;
                        state = EnvelopeState.Decay;
                    }
                    break;
                case EnvelopeState.Decay:
                    output = decayBase + output * decayCoef;
                    if (output <= sustainLevel)
                    {
                        output = sustainLevel;
                        state = EnvelopeState.Sustain;
                    }
                    break;
                case EnvelopeState.Sustain:
                    break;
                case EnvelopeState.Release:
                    output = releaseBase + output * releaseCoef;
                    if (output <= 0.0)
                    {
                        output = 0.0f;
                        state = EnvelopeState.Idle;
                    }
                    break;
            }
            return output;
        }

        public void Gate(bool gate)
        {
            if (gate)
            {
                state = EnvelopeState.Attack;
            }
            else if (state != EnvelopeState.Idle)
            {
                state = EnvelopeState.Release;
            }
        }

        public void Reset()
        {
            state = EnvelopeState.Idle;
            output = 0.0f;
        }

        public float GetOutput()
        {
            return output;
        }

        #endregion
    }
}