// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;
using System.IO;

namespace Aspose.Images.Pal.Graphics.Encoder
{
    internal class LZWEncoder
    {
        internal LZWEncoder(int width, int height, byte[] pixels, int color_depth)
        {
            imgW = width;
            imgH = height;
            pixAry = pixels;
            initCodeSize = Math.Max(2, color_depth);
        }

        // characters, flush the packet to disk.
        private void Char_out(byte c, BinaryWriter outs)
        {
            accum[a_count++] = c;
            if (a_count >= 254)
                Flush_char(outs);
        }

        // table clear for block compress
        private void Cl_block(BinaryWriter outs)
        {
            Cl_hash(hsize);
            free_ent = ClearCode + 2;
            clear_flg = true;

            Output(ClearCode, outs);
        }

        // reset code table
        private void Cl_hash(int hsize)
        {
            for (int i = 0; i < hsize; ++i)
                htab[i] = -1;
        }

        private void Compress(int init_bits, BinaryWriter outs)
        {
            int fcode;
            int i /* = 0 */;
            int c;
            int ent;
            int disp;
            int hsize_reg;
            int hshift;

            // Set up the globals: g_init_bits - initial number of bits
            g_init_bits = init_bits;

            // Set up the necessary values
            clear_flg = false;
            n_bits = g_init_bits;
            maxcode = MAXCODE(n_bits);

            ClearCode = 1 << (init_bits - 1);
            EOFCode = ClearCode + 1;
            free_ent = ClearCode + 2;

            a_count = 0; // clear packet

            ent = NextPixel();

            hshift = 0;
            for (fcode = hsize; fcode < 65536; fcode *= 2)
                ++hshift;
            hshift = 8 - hshift; // set hash code range bound

            hsize_reg = hsize;
            Cl_hash(hsize_reg); // clear hash table

            Output(ClearCode, outs);


            while ((c = NextPixel()) != EOF)
            {
                fcode = (c << maxbits) + ent;
                i = (c << hshift) ^ ent; // xor hashing

                if (htab[i] == fcode)
                {
                    ent = codetab[i];
                    continue;
                }
                else if (htab[i] >= 0) // non-empty slot
                {
                    disp = hsize_reg - i; // secondary hash (after G. Knott)
                    if (i == 0)
                        disp = 1;

                    bool continueOuterLoop = false;
                    do
                    {
                        if ((i -= disp) < 0)
                            i += hsize_reg;

                        if (htab[i] == fcode)
                        {
                            ent = codetab[i];
                            continueOuterLoop = true;
                            break;
                        }
                    } while (htab[i] >= 0);

                    if (continueOuterLoop)
                        continue;
                }
                Output(ent, outs);
                ent = c;
                if (free_ent < maxmaxcode)
                {
                    codetab[i] = free_ent++; // code -> hashtable
                    htab[i] = fcode;
                }
                else
                    Cl_block(outs);
            }

            Output(ent, outs);
            Output(EOFCode, outs);
        }

        // ----------------------------------------------------------------------------
        internal void Encode(BinaryWriter os)
        {
            os.Write((byte)initCodeSize); // write "initial code size" byte

            remaining = imgW * imgH; // reset navigation variables
            curPixel = 0;

            Compress(initCodeSize + 1, os); // compress and write the pixel data

            os.Write((byte)0); // write block terminator
            os.Flush();
        }

        // Flush the packet to disk, and reset the accumulator
        private void Flush_char(BinaryWriter outs)
        {
            if (a_count > 0)
            {
                outs.Write((byte)a_count);
                outs.Write(accum, 0, a_count);
                a_count = 0;
            }
        }

        private int MAXCODE(int n_bits)
        {
            return (1 << n_bits) - 1;
        }

        /// <summary>
        /// Return the next pixel from the image
        /// </summary>
        private int NextPixel()
        {
            if (remaining == 0)
                return EOF;

            --remaining;

            byte pix = pixAry[curPixel++];

            return pix & 0xff;
        }

        private void Output(int code, BinaryWriter outs)
        {
            cur_accum &= masks[cur_bits];

            if (cur_bits > 0)
                cur_accum |= (code << cur_bits);
            else
                cur_accum = code;

            cur_bits += n_bits;

            while (cur_bits >= 8)
            {
                Char_out((byte)(cur_accum & 0xff), outs);
                cur_accum >>= 8;
                cur_bits -= 8;
            }

            // If the next entry is going to be too big for the code size,
            // then increase it, if possible.
            if (free_ent > maxcode || clear_flg)
            {
                if (clear_flg)
                {
                    n_bits = g_init_bits;
                    maxcode = MAXCODE(n_bits);
                    clear_flg = false;
                }
                else
                {
                    ++n_bits;
                    if (n_bits == maxbits)
                        maxcode = maxmaxcode;
                    else
                        maxcode = MAXCODE(n_bits);
                }
            }

            if (code == EOFCode)
            {
                // At EOF, write the rest of the buffer.
                while (cur_bits > 0)
                {
                    Char_out((byte)(cur_accum & 0xff), outs);
                    cur_accum >>= 8;
                    cur_bits -= 8;
                }

                Flush_char(outs);
            }
        }



        private static readonly int EOF = -1;

        private readonly int imgW;
        private readonly int imgH;

        private readonly byte[] pixAry;

        private readonly int initCodeSize;

        private int remaining;

        private int curPixel;

        // GIFCOMPR.C - GIF Image compression routines
        //
        // Lempel-Ziv compression based on 'compress'. GIF modifications by
        // David Rowley (mgardi@watdcsu.waterloo.edu)

        // General DEFINEs

        private static readonly int BITS = 12;
        private static readonly int HSIZE = 5003; // 80% occupancy

        // GIF Image compression - modified 'compress'
        //
        // Based on: compress.c - File compression ala IEEE Computer, June 1984.
        //
        // By Authors: Spencer W. Thomas (decvax!harpo!utah-cs!utah-gr!thomas)
        // Jim McKie (decvax!mcvax!jim)
        // Steve Davies (decvax!vax135!petsd!peora!srd)
        // Ken Turkowski (decvax!decwrl!turtlevax!ken)
        // James A. Woods (decvax!ihnp4!ames!jaw)
        // Joe Orost (decvax!vax135!petsd!joe)

        private int n_bits; // number of bits/code

        private readonly int maxbits = BITS; // user settable max # bits/code

        private int maxcode; // maximum code, given n_bits

        private readonly int maxmaxcode = 1 << BITS; // should NEVER generate this code

        private readonly int[] htab = new int[HSIZE];
        private readonly int[] codetab = new int[HSIZE];
        private readonly int hsize = HSIZE; // for dynamic table sizing

        private int free_ent = 0; // first unused entry

        // block compression parameters -- after all codes are used up,
        // and compression rate changes, start over.
        private bool clear_flg = false;

        // Algorithm: use open addressing double hashing (no chaining) on the
        // prefix code / next character combination. We do a variant of Knuth's
        // algorithm D (vol. 3, sec. 6.4) along with G. Knott's relatively-prime
        // secondary probe. Here, the modular division first probe is gives way
        // to a faster exclusive-or manipulation. Also do block compression with
        // an adaptive reset, whereby the code table is cleared when the compression
        // ratio decreases, but after the table fills. The variable-length output
        // codes are re-sized at this point, and a special CLEAR code is generated
        // for the decompressor. Late addition: construct the table according to
        // file size for noticeable speed improvement on small files. Please direct
        // questions about this implementation to ames!jaw.

        private int g_init_bits;
        private int ClearCode;
        private int EOFCode;

        // output
        //
        // Output the given code.
        // Inputs:
        // code: A n_bits-bit integer. If == -1, then EOF. This assumes
        // that n_bits =< wordsize - 1.
        // Outputs:
        // Outputs code to the file.
        // Assumptions:
        // Chars are 8 bits long.
        // Algorithm:
        // Maintain a BITS character long buffer (so that 8 codes will
        // fit in it exactly). Use the VAX insv instruction to insert each
        // code in turn. When the buffer fills up empty it and start over.

        private int cur_accum = 0;
        private int cur_bits = 0;
        private readonly int[] masks = { 0x0000, 0x0001, 0x0003, 0x0007, 0x000F, 0x001F, 0x003F, 0x007F, 0x00FF, 0x01FF, 0x03FF, 0x07FF, 0x0FFF, 0x1FFF, 0x3FFF, 0x7FFF, 0xFFFF };

        // Number of characters so far in this 'packet'
        private int a_count;

        // Define the storage for the packet accumulator
        private readonly byte[] accum = new byte[256];
    }
}
#endif
