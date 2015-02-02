using System;

namespace SharpLib.Audio.Dsp
{
    internal class ImpulseResponseConvolution
    {
        #region Ìועמהû

        public float[] Convolve(float[] input, float[] impulseResponse)
        {
            var output = new float[input.Length + impulseResponse.Length];
            for (int t = 0; t < output.Length; t++)
            {
                for (int n = 0; n < impulseResponse.Length; n++)
                {
                    if ((t >= n) && (t - n < input.Length))
                    {
                        output[t] += impulseResponse[n] * input[t - n];
                    }
                }
            }
            Normalize(output);
            return output;
        }

        public void Normalize(float[] data)
        {
            float max = 0;
            for (int n = 0; n < data.Length; n++)
            {
                max = Math.Max(max, Math.Abs(data[n]));
            }
            if (max > 1.0)
            {
                for (int n = 0; n < data.Length; n++)
                {
                    data[n] /= max;
                }
            }
        }

        #endregion
    }
}