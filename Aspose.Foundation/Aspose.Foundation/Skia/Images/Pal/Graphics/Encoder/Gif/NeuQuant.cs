// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Encoder
{
    internal class NeuQuant
    {

        /// <summary>
        /// Initialise network in range (0,0,0) to (255,255,255) and set parameters
        /// </summary>
        /// <param name="thepic"></param>
        /// <param name="len"></param>
        /// <param name="sample"></param>
        public NeuQuant(byte[] thepic, int len, int sample)
        {

            int i;
            int[] p;

            mThepicture = thepic;
            mLengthcount = len;
            mSamplefac = sample;

            mNetwork = new int[Netsize][];
            for (i = 0; i < Netsize; i++)
            {
                mNetwork[i] = new int[4];
                p = mNetwork[i];
                p[0] = p[1] = p[2] = (i << (Netbiasshift + 8))/Netsize;
                mFreq[i] = Intbias/Netsize; // 1/netsize
                mBias[i] = 0;
            }
        }

        public byte[] ColorMap()
        {
            byte[] map = new byte[3*Netsize];
            int[] index = new int[Netsize];
            for (int i = 0; i < Netsize; i++)
                index[mNetwork[i][3]] = i;
            int k = 0;
            for (int i = 0; i < Netsize; i++)
            {
                int j = index[i];
                map[k++] = (byte) (mNetwork[j][0]);
                map[k++] = (byte) (mNetwork[j][1]);
                map[k++] = (byte) (mNetwork[j][2]);
            }
            return map;
        }

        /// <summary>
        /// Insertion sort of network and building of netindex[0..255] (to do after unbias)
        /// </summary>
        public void Inxbuild()
        {

            int i, j, smallpos, smallval;
            int[] p;
            int[] q;
            int previouscol, startpos;

            previouscol = 0;
            startpos = 0;
            for (i = 0; i < Netsize; i++)
            {
                p = mNetwork[i];
                smallpos = i;
                smallval = p[1]; // index on g
                // find smallest in i..netsize-1
                for (j = i + 1; j < Netsize; j++)
                {
                    q = mNetwork[j];
                    if (q[1] < smallval)
                    {
                        // index on g
                        smallpos = j;
                        smallval = q[1]; // index on g
                    }
                }
                q = mNetwork[smallpos];
                // swap p (i) and q (smallpos) entries
                if (i != smallpos)
                {
                    j = q[0];
                    q[0] = p[0];
                    p[0] = j;
                    j = q[1];
                    q[1] = p[1];
                    p[1] = j;
                    j = q[2];
                    q[2] = p[2];
                    p[2] = j;
                    j = q[3];
                    q[3] = p[3];
                    p[3] = j;
                }
                // smallval entry is now in position i
                if (smallval != previouscol)
                {
                    mNetindex[previouscol] = (startpos + i) >> 1;
                    for (j = previouscol + 1; j < smallval; j++)
                        mNetindex[j] = i;
                    previouscol = smallval;
                    startpos = i;
                }
            }
            mNetindex[previouscol] = (startpos + Maxnetpos) >> 1;
            for (j = previouscol + 1; j < 256; j++)
                mNetindex[j] = Maxnetpos; // really 256
        }

        /// <summary>
        /// Main Learning Loop
        /// </summary>
        public void Learn()
        {

            int i, j, b, g, r;
            int radius, rad, alpha, step, delta, samplepixels;
            byte[] p;
            int pix, lim;

            if (mLengthcount < Minpicturebytes)
                mSamplefac = 1;
            mAlphadec = 30 + ((mSamplefac - 1)/3);
            p = mThepicture;
            pix = 0;
            lim = mLengthcount;
            samplepixels = mLengthcount/(3*mSamplefac);
            delta = samplepixels/Ncycles;
            alpha = Initalpha;
            radius = Initradius;

            rad = radius >> Radiusbiasshift;
            if (rad <= 1)
                rad = 0;
            for (i = 0; i < rad; i++)
                mRadpower[i] = alpha*(((rad*rad - i*i)*Radbias)/(rad*rad));

            // fprintf(stderr,"beginning 1D learning: initial radius=%d\n", rad);

            if (mLengthcount < Minpicturebytes)
                step = 3;
            else if ((mLengthcount%Prime1) != 0)
                step = 3*Prime1;
            else
            {
                if ((mLengthcount%Prime2) != 0)
                    step = 3*Prime2;
                else
                {
                    if ((mLengthcount%Prime3) != 0)
                        step = 3*Prime3;
                    else
                        step = 3*Prime4;
                }
            }

            i = 0;
            while (i < samplepixels)
            {
                b = (p[pix + 0] & 0xff) << Netbiasshift;
                g = (p[pix + 1] & 0xff) << Netbiasshift;
                r = (p[pix + 2] & 0xff) << Netbiasshift;
                j = Contest(b, g, r);

                Altersingle(alpha, j, b, g, r);
                if (rad != 0)
                    Alterneigh(rad, j, b, g, r); // alter neighbours

                pix += step;
                if (pix >= lim)
                    pix -= mLengthcount;

                i++;
                if (delta == 0)
                    delta = 1;
                if (i%delta == 0)
                {
                    alpha -= alpha/mAlphadec;
                    radius -= radius/Radiusdec;
                    rad = radius >> Radiusbiasshift;
                    if (rad <= 1)
                        rad = 0;
                    for (j = 0; j < rad; j++)
                        mRadpower[j] = alpha*(((rad*rad - j*j)*Radbias)/(rad*rad));
                }
            }
            // fprintf(stderr,"finished 1D learning: readonly alpha=%f
            // !\n",((float)alpha)/initalpha);
        }

        /// <summary>
        /// Search for BGR values 0..255 (after net is unbiased) and return colour index
        /// </summary>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public int Map(int b, int g, int r)
        {

            int i, j, dist, a, bestd;
            int[] p;
            int best;

            bestd = 1000; // biggest possible dist is 256*3
            best = -1;
            i = mNetindex[g]; // index on g
            j = i - 1; // start at netindex[g] and work outwards

            while ((i < Netsize) || (j >= 0))
            {
                if (i < Netsize)
                {
                    p = mNetwork[i];
                    dist = p[1] - g; // inx key
                    if (dist >= bestd)
                        i = Netsize; // stop iter
                    else
                    {
                        i++;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }
                if (j >= 0)
                {
                    p = mNetwork[j];
                    dist = g - p[1]; // inx key - reverse dif
                    if (dist >= bestd)
                        j = -1; // stop iter
                    else
                    {
                        j--;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }
            }
            return (best);
        }

        public byte[] Process()
        {
            Learn();
            Unbiasnet();
            Inxbuild();
            return ColorMap();
        }

        /// <summary>
        /// Unbias network to give byte values 0..255 and record position i to prepare for sort
        /// </summary>
        public void Unbiasnet()
        {

            int i;

            for (i = 0; i < Netsize; i++)
            {
                mNetwork[i][0] >>= Netbiasshift;
                mNetwork[i][1] >>= Netbiasshift;
                mNetwork[i][2] >>= Netbiasshift;
                mNetwork[i][3] = i; // record colour no
            }
        }

        /// <summary>
        /// Move adjacent neurons by precomputed alpha*(1-((i-j)^2/[r]^2)) in radpower[|i-j|]
        /// </summary>
        /// <param name="rad"></param>
        /// <param name="i"></param>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        private void Alterneigh(int rad, int i, int b, int g, int r)
        {

            int j, k, lo, hi, a, m;
            int[] p;

            lo = i - rad;
            if (lo < -1)
                lo = -1;
            hi = i + rad;
            if (hi > Netsize)
                hi = Netsize;

            j = i + 1;
            k = i - 1;
            m = 1;
            while ((j < hi) || (k > lo))
            {
                a = mRadpower[m++];
                if (j < hi)
                {
                    p = mNetwork[j++];
                    try
                    {
                        p[0] -= (a*(p[0] - b))/Alpharadbias;
                        p[1] -= (a*(p[1] - g))/Alpharadbias;
                        p[2] -= (a*(p[2] - r))/Alpharadbias;
                    }
                    catch
                    {
// TODO: Maybe unchecked block can be used instead of this catch.
                    } // prevents 1.3 miscompilation
                }
                if (k > lo)
                {
                    p = mNetwork[k--];
                    try
                    {
                        p[0] -= (a*(p[0] - b))/Alpharadbias;
                        p[1] -= (a*(p[1] - g))/Alpharadbias;
                        p[2] -= (a*(p[2] - r))/Alpharadbias;
                    }
                    catch
                    {
// TODO: Maybe unchecked block can be used instead of this catch.
                    }
                }
            }
        }

        /// <summary>
        /// Move neuron i towards biased (b,g,r) by factor alpha
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="i"></param>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        private void Altersingle(int alpha, int i, int b, int g, int r)
        {

            // alter hit neuron
            int[] n = mNetwork[i];
            n[0] -= (alpha*(n[0] - b))/Initalpha;
            n[1] -= (alpha*(n[1] - g))/Initalpha;
            n[2] -= (alpha*(n[2] - r))/Initalpha;
        }

        /// <summary>
        /// Search for biased BGR values
        /// </summary>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private int Contest(int b, int g, int r)
        {
            // finds closest neuron (min dist) and updates freq
            // finds best neuron (min dist-bias) and returns position
            // for frequently chosen neurons, freq[i] is high and bias[i] is negative
            // bias[i] = gamma*((1/netsize)-freq[i])

            int i, dist, a, biasdist, betafreq;
            int bestpos, bestbiaspos, bestd, bestbiasd;
            int[] n;

            bestd = ~(((int) 1) << 31);
            bestbiasd = bestd;
            bestpos = -1;
            bestbiaspos = bestpos;

            for (i = 0; i < Netsize; i++)
            {
                n = mNetwork[i];
                dist = n[0] - b;
                if (dist < 0)
                    dist = -dist;
                a = n[1] - g;
                if (a < 0)
                    a = -a;
                dist += a;
                a = n[2] - r;
                if (a < 0)
                    a = -a;
                dist += a;
                if (dist < bestd)
                {
                    bestd = dist;
                    bestpos = i;
                }
                biasdist = dist - ((mBias[i]) >> (Intbiasshift - Netbiasshift));
                if (biasdist < bestbiasd)
                {
                    bestbiasd = biasdist;
                    bestbiaspos = i;
                }
                betafreq = (mFreq[i] >> Betashift);
                mFreq[i] -= betafreq;
                mBias[i] += (betafreq << Gammashift);
            }
            mFreq[bestpos] += Beta;
            mBias[bestpos] -= Betagamma;
            return (bestbiaspos);
        }


        private const int Netsize = 256; // number of colours used

        // four primes near 500 - assume no image has a length so large
        // that it is divisible by all four primes
        private const int Prime1 = 499;

        private const int Prime2 = 491;

        private const int Prime3 = 487;

        private const int Prime4 = 503;

        private const int Minpicturebytes = (3*Prime4);

        private const int Maxnetpos = (Netsize - 1);

        private const int Netbiasshift = 4; // bias for colour values

        private const int Ncycles = 100; // no. of learning cycles

        // defs for freq and bias
        private const int Intbiasshift = 16; // bias for fractions

        private const int Intbias = (((int) 1) << Intbiasshift);

        private const int Gammashift = 10; // gamma = 1024

        private const int Gamma = (((int) 1) << Gammashift);

        private const int Betashift = 10;

        private const int Beta = (Intbias >> Betashift); // beta = 1/1024

        private const int Betagamma = (Intbias << (Gammashift - Betashift));

        // defs for decreasing radius factor

        private const int Initrad = (Netsize >> 3); // for 256 cols, radius starts


        private const int Radiusbiasshift = 6; // at 32.0 biased by 6 bits

        private const int Radiusbias = (((int) 1) << Radiusbiasshift);

        private const int Initradius = (Initrad*Radiusbias); // and decreases by a


        private const int Radiusdec = 30; // factor of 1/30 each cycle

        // defs for decreasing alpha factor
        private const int Alphabiasshift = 10; // alpha starts at 1.0

        private const int Initalpha = (((int) 1) << Alphabiasshift);

        private int mAlphadec; // biased by 10 bits

        // radbias and alpharadbias used for radpower calculation
        private const int Radbiasshift = 8;

        private const int Radbias = (((int) 1) << Radbiasshift);

        private const int Alpharadbshift = (Alphabiasshift + Radbiasshift);

        private const int Alpharadbias = (((int) 1) << Alpharadbshift);

        // Types and Global Variables
    

        private readonly byte[] mThepicture; // the input image itself

        private readonly int mLengthcount; // lengthcount = H*W*3

        private int mSamplefac; // sampling factor 1..30

        // typedef int pixel[4]; // BGRc
        private readonly int[][] mNetwork; // the network itself - [netsize][4]

        private readonly int[] mNetindex = new int[256];

        // for network lookup - really 256
        private readonly int[] mBias = new int[Netsize];

        // bias and freq arrays for learning
        private readonly int[] mFreq = new int[Netsize];

        private readonly int[] mRadpower = new int[Initrad];
    }
}

#endif
