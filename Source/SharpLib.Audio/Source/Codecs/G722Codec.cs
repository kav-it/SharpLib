﻿using System;

namespace SharpLib.Audio.Codecs
{
    internal class G722Codec
    {
        #region Поля

        private static readonly int[] _ihn = { 0, 1, 0 };

        private static readonly int[] _ihp = { 0, 3, 2 };

        private static readonly int[] _ilb =
        {
            2048, 2093, 2139, 2186, 2233, 2282, 2332, 2383, 2435, 2489, 2543, 2599, 2656, 2714, 2774, 2834, 2896, 2960, 3025, 3091, 3158, 3228, 3298, 3371, 3444, 3520,
            3597, 3676, 3756, 3838, 3922, 4008
        };

        private static readonly int[] _iln = { 0, 63, 62, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 0 };

        private static readonly int[] ilp = { 0, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 0 };

        private static readonly int[] q6 =
        {
            0, 35, 72, 110, 150, 190, 233, 276, 323, 370, 422, 473, 530, 587, 650, 714, 786, 858, 940, 1023, 1121, 1219, 1339, 1458, 1612, 1765, 1980, 2195, 2557, 2919,
            0, 0
        };

        private static readonly int[] qm2 = { -7408, -1616, 7408, 1616 };

        private static readonly int[] qm4 = { 0, -20456, -12896, -8968, -6288, -4240, -2584, -1200, 20456, 12896, 8968, 6288, 4240, 2584, 1200, 0 };

        private static readonly int[] qm5 =
        {
            -280, -280, -23352, -17560, -14120, -11664, -9752, -8184, -6864, -5712, -4696, -3784, -2960, -2208, -1520, -880, 23352, 17560, 14120, 11664, 9752, 8184,
            6864, 5712, 4696, 3784, 2960, 2208, 1520, 880, 280, -280
        };

        private static readonly int[] qm6 =
        {
            -136, -136, -136, -136, -24808, -21904, -19008, -16704, -14984, -13512, -12280, -11192, -10232, -9360, -8576, -7856, -7192, -6576, -6000, -5456, -4944,
            -4464, -4008, -3576, -3168, -2776, -2400, -2032, -1688, -1360, -1040, -728, 24808, 21904, 19008, 16704, 14984, 13512, 12280, 11192, 10232, 9360, 8576, 7856, 7192, 6576, 6000, 5456, 4944,
            4464, 4008, 3576, 3168, 2776, 2400, 2032, 1688, 1360, 1040, 728, 432, 136, -432, -136
        };

        private static readonly int[] qmf_coeffs = { 3, -11, 12, 32, -210, 951, 3876, -805, 362, -156, 53, -11 };

        private static readonly int[] rh2 = { 2, 1, 2, 1 };

        private static readonly int[] rl42 = { 0, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0 };

        private static readonly int[] wh = { 0, -214, 798 };

        private static readonly int[] wl = { -60, -30, 58, 172, 334, 538, 1198, 3042 };

        #endregion

        #region Методы

        private static short Saturate(int amp)
        {
            short amp16 = (short)amp;
            if (amp == amp16)
            {
                return amp16;
            }
            if (amp > Int16.MaxValue)
            {
                return Int16.MaxValue;
            }
            return Int16.MinValue;
        }

        private static void Block4(G722CodecState s, int band, int d)
        {
            int wd1;
            int wd2;
            int wd3;
            int i;

            s.Band[band].d[0] = d;
            s.Band[band].r[0] = Saturate(s.Band[band].s + d);

            s.Band[band].p[0] = Saturate(s.Band[band].sz + d);

            for (i = 0; i < 3; i++)
            {
                s.Band[band].sg[i] = s.Band[band].p[i] >> 15;
            }
            wd1 = Saturate(s.Band[band].a[1] << 2);

            wd2 = (s.Band[band].sg[0] == s.Band[band].sg[1]) ? -wd1 : wd1;
            if (wd2 > 32767)
            {
                wd2 = 32767;
            }
            wd3 = (s.Band[band].sg[0] == s.Band[band].sg[2]) ? 128 : -128;
            wd3 += (wd2 >> 7);
            wd3 += (s.Band[band].a[2] * 32512) >> 15;
            if (wd3 > 12288)
            {
                wd3 = 12288;
            }
            else if (wd3 < -12288)
            {
                wd3 = -12288;
            }
            s.Band[band].ap[2] = wd3;

            s.Band[band].sg[0] = s.Band[band].p[0] >> 15;
            s.Band[band].sg[1] = s.Band[band].p[1] >> 15;
            wd1 = (s.Band[band].sg[0] == s.Band[band].sg[1]) ? 192 : -192;
            wd2 = (s.Band[band].a[1] * 32640) >> 15;

            s.Band[band].ap[1] = Saturate(wd1 + wd2);
            wd3 = Saturate(15360 - s.Band[band].ap[2]);
            if (s.Band[band].ap[1] > wd3)
            {
                s.Band[band].ap[1] = wd3;
            }
            else if (s.Band[band].ap[1] < -wd3)
            {
                s.Band[band].ap[1] = -wd3;
            }

            wd1 = (d == 0) ? 0 : 128;
            s.Band[band].sg[0] = d >> 15;
            for (i = 1; i < 7; i++)
            {
                s.Band[band].sg[i] = s.Band[band].d[i] >> 15;
                wd2 = (s.Band[band].sg[i] == s.Band[band].sg[0]) ? wd1 : -wd1;
                wd3 = (s.Band[band].b[i] * 32640) >> 15;
                s.Band[band].bp[i] = Saturate(wd2 + wd3);
            }

            for (i = 6; i > 0; i--)
            {
                s.Band[band].d[i] = s.Band[band].d[i - 1];
                s.Band[band].b[i] = s.Band[band].bp[i];
            }

            for (i = 2; i > 0; i--)
            {
                s.Band[band].r[i] = s.Band[band].r[i - 1];
                s.Band[band].p[i] = s.Band[band].p[i - 1];
                s.Band[band].a[i] = s.Band[band].ap[i];
            }

            wd1 = Saturate(s.Band[band].r[1] + s.Band[band].r[1]);
            wd1 = (s.Band[band].a[1] * wd1) >> 15;
            wd2 = Saturate(s.Band[band].r[2] + s.Band[band].r[2]);
            wd2 = (s.Band[band].a[2] * wd2) >> 15;
            s.Band[band].sp = Saturate(wd1 + wd2);

            s.Band[band].sz = 0;
            for (i = 6; i > 0; i--)
            {
                wd1 = Saturate(s.Band[band].d[i] + s.Band[band].d[i]);
                s.Band[band].sz += (s.Band[band].b[i] * wd1) >> 15;
            }
            s.Band[band].sz = Saturate(s.Band[band].sz);

            s.Band[band].s = Saturate(s.Band[band].sp + s.Band[band].sz);
        }

        public int Decode(G722CodecState state, short[] outputBuffer, byte[] inputG722Data, int inputLength)
        {
            int dlowt;
            int rlow;
            int ihigh;
            int dhigh;
            int rhigh;
            int xout1;
            int xout2;
            int wd1;
            int wd2;
            int wd3;
            int code;
            int outlen;
            int i;
            int j;

            outlen = 0;
            rhigh = 0;
            for (j = 0; j < inputLength;)
            {
                if (state.Packed)
                {
                    if (state.InBits < state.BitsPerSample)
                    {
                        state.InBuffer |= (uint)(inputG722Data[j++] << state.InBits);
                        state.InBits += 8;
                    }
                    code = (int)state.InBuffer & ((1 << state.BitsPerSample) - 1);
                    state.InBuffer >>= state.BitsPerSample;
                    state.InBits -= state.BitsPerSample;
                }
                else
                {
                    code = inputG722Data[j++];
                }

                switch (state.BitsPerSample)
                {
                    default:
                    case 8:
                        wd1 = code & 0x3F;
                        ihigh = (code >> 6) & 0x03;
                        wd2 = qm6[wd1];
                        wd1 >>= 2;
                        break;
                    case 7:
                        wd1 = code & 0x1F;
                        ihigh = (code >> 5) & 0x03;
                        wd2 = qm5[wd1];
                        wd1 >>= 1;
                        break;
                    case 6:
                        wd1 = code & 0x0F;
                        ihigh = (code >> 4) & 0x03;
                        wd2 = qm4[wd1];
                        break;
                }

                wd2 = (state.Band[0].det * wd2) >> 15;

                rlow = state.Band[0].s + wd2;

                if (rlow > 16383)
                {
                    rlow = 16383;
                }
                else if (rlow < -16384)
                {
                    rlow = -16384;
                }

                wd2 = qm4[wd1];
                dlowt = (state.Band[0].det * wd2) >> 15;

                wd2 = rl42[wd1];
                wd1 = (state.Band[0].nb * 127) >> 7;
                wd1 += wl[wd2];
                if (wd1 < 0)
                {
                    wd1 = 0;
                }
                else if (wd1 > 18432)
                {
                    wd1 = 18432;
                }
                state.Band[0].nb = wd1;

                wd1 = (state.Band[0].nb >> 6) & 31;
                wd2 = 8 - (state.Band[0].nb >> 11);
                wd3 = (wd2 < 0) ? (_ilb[wd1] << -wd2) : (_ilb[wd1] >> wd2);
                state.Band[0].det = wd3 << 2;

                Block4(state, 0, dlowt);

                if (!state.EncodeFrom8000Hz)
                {
                    wd2 = qm2[ihigh];
                    dhigh = (state.Band[1].det * wd2) >> 15;

                    rhigh = dhigh + state.Band[1].s;

                    if (rhigh > 16383)
                    {
                        rhigh = 16383;
                    }
                    else if (rhigh < -16384)
                    {
                        rhigh = -16384;
                    }

                    wd2 = rh2[ihigh];
                    wd1 = (state.Band[1].nb * 127) >> 7;
                    wd1 += wh[wd2];
                    if (wd1 < 0)
                    {
                        wd1 = 0;
                    }
                    else if (wd1 > 22528)
                    {
                        wd1 = 22528;
                    }
                    state.Band[1].nb = wd1;

                    wd1 = (state.Band[1].nb >> 6) & 31;
                    wd2 = 10 - (state.Band[1].nb >> 11);
                    wd3 = (wd2 < 0) ? (_ilb[wd1] << -wd2) : (_ilb[wd1] >> wd2);
                    state.Band[1].det = wd3 << 2;

                    Block4(state, 1, dhigh);
                }

                if (state.ItuTestMode)
                {
                    outputBuffer[outlen++] = (short)(rlow << 1);
                    outputBuffer[outlen++] = (short)(rhigh << 1);
                }
                else
                {
                    if (state.EncodeFrom8000Hz)
                    {
                        outputBuffer[outlen++] = (short)(rlow << 1);
                    }
                    else
                    {
                        for (i = 0; i < 22; i++)
                        {
                            state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
                        }
                        state.QmfSignalHistory[22] = rlow + rhigh;
                        state.QmfSignalHistory[23] = rlow - rhigh;

                        xout1 = 0;
                        xout2 = 0;
                        for (i = 0; i < 12; i++)
                        {
                            xout2 += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
                            xout1 += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
                        }
                        outputBuffer[outlen++] = (short)(xout1 >> 11);
                        outputBuffer[outlen++] = (short)(xout2 >> 11);
                    }
                }
            }
            return outlen;
        }

        public int Encode(G722CodecState state, byte[] outputBuffer, short[] inputBuffer, int inputBufferCount)
        {
            int dlow;
            int dhigh;
            int el;
            int wd;
            int wd1;
            int ril;
            int wd2;
            int il4;
            int ih2;
            int wd3;
            int eh;
            int mih;
            int i;
            int j;

            int xlow;
            int xhigh;
            int g722_bytes;

            int sumeven;
            int sumodd;
            int ihigh;
            int ilow;
            int code;

            g722_bytes = 0;
            xhigh = 0;
            for (j = 0; j < inputBufferCount;)
            {
                if (state.ItuTestMode)
                {
                    xlow =
                        xhigh = inputBuffer[j++] >> 1;
                }
                else
                {
                    if (state.EncodeFrom8000Hz)
                    {
                        xlow = inputBuffer[j++] >> 1;
                    }
                    else
                    {
                        for (i = 0; i < 22; i++)
                        {
                            state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
                        }
                        state.QmfSignalHistory[22] = inputBuffer[j++];
                        state.QmfSignalHistory[23] = inputBuffer[j++];

                        sumeven = 0;
                        sumodd = 0;
                        for (i = 0; i < 12; i++)
                        {
                            sumodd += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
                            sumeven += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
                        }
                        xlow = (sumeven + sumodd) >> 14;
                        xhigh = (sumeven - sumodd) >> 14;
                    }
                }

                el = Saturate(xlow - state.Band[0].s);

                wd = (el >= 0) ? el : -(el + 1);

                for (i = 1; i < 30; i++)
                {
                    wd1 = (q6[i] * state.Band[0].det) >> 12;
                    if (wd < wd1)
                    {
                        break;
                    }
                }
                ilow = (el < 0) ? _iln[i] : ilp[i];

                ril = ilow >> 2;
                wd2 = qm4[ril];
                dlow = (state.Band[0].det * wd2) >> 15;

                il4 = rl42[ril];
                wd = (state.Band[0].nb * 127) >> 7;
                state.Band[0].nb = wd + wl[il4];
                if (state.Band[0].nb < 0)
                {
                    state.Band[0].nb = 0;
                }
                else if (state.Band[0].nb > 18432)
                {
                    state.Band[0].nb = 18432;
                }

                wd1 = (state.Band[0].nb >> 6) & 31;
                wd2 = 8 - (state.Band[0].nb >> 11);
                wd3 = (wd2 < 0) ? (_ilb[wd1] << -wd2) : (_ilb[wd1] >> wd2);
                state.Band[0].det = wd3 << 2;

                Block4(state, 0, dlow);

                if (state.EncodeFrom8000Hz)
                {
                    code = (0xC0 | ilow) >> (8 - state.BitsPerSample);
                }
                else
                {
                    eh = Saturate(xhigh - state.Band[1].s);

                    wd = (eh >= 0) ? eh : -(eh + 1);
                    wd1 = (564 * state.Band[1].det) >> 12;
                    mih = (wd >= wd1) ? 2 : 1;
                    ihigh = (eh < 0) ? _ihn[mih] : _ihp[mih];

                    wd2 = qm2[ihigh];
                    dhigh = (state.Band[1].det * wd2) >> 15;

                    ih2 = rh2[ihigh];
                    wd = (state.Band[1].nb * 127) >> 7;
                    state.Band[1].nb = wd + wh[ih2];
                    if (state.Band[1].nb < 0)
                    {
                        state.Band[1].nb = 0;
                    }
                    else if (state.Band[1].nb > 22528)
                    {
                        state.Band[1].nb = 22528;
                    }

                    wd1 = (state.Band[1].nb >> 6) & 31;
                    wd2 = 10 - (state.Band[1].nb >> 11);
                    wd3 = (wd2 < 0) ? (_ilb[wd1] << -wd2) : (_ilb[wd1] >> wd2);
                    state.Band[1].det = wd3 << 2;

                    Block4(state, 1, dhigh);
                    code = ((ihigh << 6) | ilow) >> (8 - state.BitsPerSample);
                }

                if (state.Packed)
                {
                    state.OutBuffer |= (uint)(code << state.OutBits);
                    state.OutBits += state.BitsPerSample;
                    if (state.OutBits >= 8)
                    {
                        outputBuffer[g722_bytes++] = (byte)(state.OutBuffer & 0xFF);
                        state.OutBits -= 8;
                        state.OutBuffer >>= 8;
                    }
                }
                else
                {
                    outputBuffer[g722_bytes++] = (byte)code;
                }
            }
            return g722_bytes;
        }

        #endregion
    }

    internal class G722CodecState
    {
        #region Свойства

        public bool ItuTestMode { get; set; }

        public bool Packed { get; private set; }

        public bool EncodeFrom8000Hz { get; private set; }

        public int BitsPerSample { get; private set; }

        public int[] QmfSignalHistory { get; private set; }

        public Band[] Band { get; private set; }

        public uint InBuffer { get; internal set; }

        public int InBits { get; internal set; }

        public uint OutBuffer { get; internal set; }

        public int OutBits { get; internal set; }

        #endregion

        #region Конструктор

        public G722CodecState(int rate, G722Flags options)
        {
            Band = new Band[2] { new Band(), new Band() };
            QmfSignalHistory = new int[24];
            ItuTestMode = false;

            if (rate == 48000)
            {
                BitsPerSample = 6;
            }
            else if (rate == 56000)
            {
                BitsPerSample = 7;
            }
            else if (rate == 64000)
            {
                BitsPerSample = 8;
            }
            else
            {
                throw new ArgumentException("Invalid rate, should be 48000, 56000 or 64000");
            }
            if ((options & G722Flags.SampleRate8000) == G722Flags.SampleRate8000)
            {
                EncodeFrom8000Hz = true;
            }
            if (((options & G722Flags.Packed) == G722Flags.Packed) && BitsPerSample != 8)
            {
                Packed = true;
            }
            else
            {
                Packed = false;
            }
            Band[0].det = 32;
            Band[1].det = 8;
        }

        #endregion
    }

    internal class Band
    {
        #region Поля

        public int[] a = new int[3];

        public int[] ap = new int[3];

        public int[] b = new int[7];

        public int[] bp = new int[7];

        public int[] d = new int[7];

        public int det;

        public int nb;

        public int[] p = new int[3];

        public int[] r = new int[3];

        public int s;

        public int[] sg = new int[7];

        public int sp;

        public int sz;

        #endregion
    }

    [Flags]
    internal enum G722Flags
    {
        None = 0,

        SampleRate8000 = 0x0001,

        Packed = 0x0002
    }
}