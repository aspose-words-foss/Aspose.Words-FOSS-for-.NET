// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using SkiaSharp;
using System.IO;

namespace Aspose.Images.Pal.Graphics.Encoder
{
    internal class GifEncoder
    {
        /// <summary>
        /// Sets the delay time between each frame, or changes it for subsequent frames (applies to last frame added).
        /// </summary>
        /// <param name="ms">int delay time in milliseconds</param>
        public void SetDelay(int ms)
        {
            mDelay = ms/10;
        }

        /// <summary>
        /// Sets the GIF frame disposal code for the last added frame and any
        /// subsequent frames. Default is 0 if no transparent color has been set,
        /// otherwise 2.
        /// </summary>
        /// <param name="code">int disposal code.</param>
        public void SetDispose(int code)
        {
            if (code >= 0)
            {
                mDispose = code;
            }
        }

        /// <summary>
        /// Sets the number of times the set of GIF frames should be played. Default is
        /// 1; 0 means play indefinitely. Must be invoked before the first image is
        /// added.
        /// </summary>
        /// <param name="iter">int number of iterations.</param>
        public void SetRepeat(int iter)
        {
            if (iter >= 0)
            {
                mRepeat = iter;
            }
        }

        /// <summary>
        /// Sets the transparent color for the last added frame and any subsequent
        /// frames. Since all colors are subject to modification in the quantization
        /// process, the color in the final palette for each frame closest to the given
        /// color becomes the transparent color for that frame. May be set to null to
        /// indicate no transparent color.
        /// </summary>
        /// <param name="c">Color to be treated as transparent on display.</param>
        public void SetTransparent(int c)
        {
            mTransparent = c;
        }

        /// <summary>
        /// Adds next GIF frame. The frame is not written immediately, but is actually
        /// deferred until the next frame is received so that timing data can be
        /// inserted. Invoking <see cref="Finish"/> flushes all frames. If
        /// <see cref="SetSize"/> was not invoked, the size of the first image is used
        /// for all subsequent frames.
        /// </summary>
        /// <param name="im">BufferedImage containing frame to write.</param>
        /// <returns>true if successful.</returns>
        public bool AddFrame(SKBitmap im)
        {
            if ((im == null) || !mStarted)
            {
                return false;
            }
            bool ok = true;
            try
            {
                if (!mSizeSet)
                {
                    // use first frame's size
                    SetSize(im.Width, im.Height);
                }
                mImage = im;
                GetImagePixels(); // convert to correct format if necessary
                AnalyzePixels(); // build color table & map pixels
                if (mFirstFrame)
                {
                    WriteLsd(); // logical screen descriptor
                    WritePalette(); // global color table
                    if (mRepeat >= 0)
                    {
                        // use NS app extension to indicate reps
                        WriteNetscapeExt();
                    }
                }
                WriteGraphicCtrlExt(); // write graphic control extension
                WriteImageDesc(); // image descriptor
                if (!mFirstFrame)
                {
                    WritePalette(); // local color table
                }
                WritePixels(); // encode and write pixel data
                mFirstFrame = false;
            }
            catch (IOException)
            {
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Flushes any pending data and closes output file. If writing to an
        /// OutputStream, the stream is not closed.
        /// </summary>
        public bool Finish()
        {
            if (!mStarted)
                return false;
            bool ok = true;
            mStarted = false;
            try
            {
                mOutput.Write((byte) 0x3b); // gif trailer
                mOutput.Flush();
                if (mCloseStream)
                {
                    mOutput.Close();
                }
            }
            catch (IOException)
            {
                ok = false;
            }

            // reset for subsequent use
            mTransIndex = 0;
            mOutput = null;
            mImage = null;
            mPixels = null;
            mIndexedPixels = null;
            mColorTab = null;
            mCloseStream = false;
            mFirstFrame = true;

            return ok;
        }

        /// <summary>
        /// Sets frame rate in frames per second. Equivalent to SetDelay(1000/fps).
        /// </summary>
        /// <param name="fps">float frame rate (frames per second)</param>
        public void SetFrameRate(float fps)
        {
            if (!MathUtil.IsZero(fps))
                mDelay = (int) (100/fps);
        }

        /// <summary>
        /// Sets quality of color quantization (conversion of images to the maximum 256
        /// colors allowed by the GIF specification). Lower values (minimum = 1)
        /// produce better colors, but slow processing significantly. 10 is the
        /// default, and produces good color mapping at reasonable speeds. Values
        /// greater than 20 do not yield significant improvements in speed.
        /// </summary>
        /// <param name="quality">int greater than 0.</param>
        public void SetQuality(int quality)
        {
            if (quality < 1)
                quality = 1;
            mSample = quality;
        }

        /// <summary>
        /// Sets the GIF frame size. The default size is the size of the first frame
        /// added if this method is not invoked.
        /// </summary>
        /// <param name="w">int frame width.</param>
        /// <param name="h">int frame width.</param>
        public void SetSize(int w, int h)
        {
            mWidth = w;
            mHeight = h;
            if (mWidth < 1)
                mWidth = 320;
            if (mHeight < 1)
                mHeight = 240;
            mSizeSet = true;
        }

        /// <summary>
        /// Sets the GIF frame position. The position is 0,0 by default.
        /// Useful for only updating a section of the image
        /// </summary>
        /// <param name="x">int frame width.</param>
        /// <param name="y">int frame width.</param>
        public void SetPosition(int x, int y)
        {
            mX = x;
            mY = y;
        }

        /// <summary>
        /// Initiates GIF file creation on the given stream. The stream is not closed
        /// automatically.
        /// </summary>
        /// <param name="os">OutputStream on which GIF images are written.</param>
        /// <returns>false if initial write failed.</returns>
        public bool Start(Stream os)
        {
            if (os == null)
                return false;
            bool ok = true;
            mCloseStream = false;
            mOutput = new BinaryWriter(os);
            try
            {
                WriteString("GIF89a"); // header
            }
            catch (IOException)
            {
                ok = false;
            }
            return mStarted = ok;
        }

        /// <summary>
        /// Analyzes image colors and creates color map.
        /// </summary>
        private void AnalyzePixels()
        {
            int len = mPixels.Length;
            int nPix = len / 3;
            mIndexedPixels = new byte[nPix];
            NeuQuant nq = new NeuQuant(mPixels, len, mSample);
            // initialize quantizer
            mColorTab = nq.Process(); // create reduced palette
            // convert map from BGR to RGB
            for (int i = 0; i < mColorTab.Length; i += 3)
            {
                byte temp = mColorTab[i];
                mColorTab[i] = mColorTab[i + 2];
                mColorTab[i + 2] = temp;
                mUsedEntry[i / 3] = false;
            }
            // map image pixels to new palette
            int k = 0;
            for (int i = 0; i < nPix; i++)
            {
                int index = nq.Map(mPixels[k++] & 0xff, mPixels[k++] & 0xff, mPixels[k++] & 0xff);
                mUsedEntry[index] = true;
                mIndexedPixels[i] = (byte) index;
            }
            mPixels = null;
            mColorDepth = 8;
            mPalSize = 7;
            // get closest match to transparent color if specified
            if (mTransparent != -1)
            {
                mTransIndex = FindClosest(mTransparent);
            }
        }

        /// <summary>
        /// Returns index of palette color closest to c
        /// </summary>
        private int FindClosest(int c)
        {
            if (mColorTab == null)
                return -1;
            int r = (c >> 16) & 0xff;
            int g = (c >> 8) & 0xff;
            int b = (c >> 0) & 0xff;
            int minpos = 0;
            int dmin = 256 * 256 * 256;
            int len = mColorTab.Length;
            int i = 0;
            while (i < len)
            {
                int dr = r - (mColorTab[i++] & 0xff);
                int dg = g - (mColorTab[i++] & 0xff);
                int db = b - (mColorTab[i] & 0xff);
                int d = dr * dr + dg * dg + db * db;
                int index = i / 3;
                if (mUsedEntry[index] && (d < dmin))
                {
                    dmin = d;
                    minpos = index;
                }
                i++;
            }
            return minpos;
        }

        /// <summary>
        /// Extracts image pixels into byte array "pixels"
        /// </summary>
        private void GetImagePixels()
        {
            int w = mImage.Width;
            int h = mImage.Height;
            if ((w != mWidth) || (h != mHeight))
            {
                // create new image with right size/format
                SKBitmap temp = new SKBitmap(mWidth, mHeight, SKColorType.Rgb565, SKAlphaType.Premul);
                using (SKCanvas g = new SKCanvas(temp))
                using (SKPaint p = new SKPaint())
                {
                    GraphicsQualityOptions.ApplyDefault(p);
                    g.DrawBitmap(mImage, 0, 0, p);
                }
                mImage = temp;
            }
            SKColor[] data = mImage.Pixels;
            mPixels = new byte[data.Length * 3];
            for (int i = 0; i < data.Length; i++)
            {
                SKColor td = data[i];
                int tind = i * 3;
                mPixels[tind++] = td.Blue;
                mPixels[tind++] = td.Green;
                mPixels[tind] = td.Red;
            }
        }

        /// <summary>
        /// Writes Graphic Control Extension
        /// </summary>
        private void WriteGraphicCtrlExt()
        {
            mOutput.Write((byte) 0x21); // extension introducer
            mOutput.Write((byte) 0xf9); // GCE label
            mOutput.Write((byte) 4); // data block size
            int transp, disp;
            if (mTransparent == -1)
            {
                transp = 0;
                disp = 0; // dispose = no action
            }
            else
            {
                transp = 1;
                disp = 2; // force clear if using transparent color
            }
            if (mDispose >= 0)
            {
                disp = mDispose & 7; // user override
            }
            disp <<= 2;

            // packed fields
            mOutput.Write((byte) (0 | // 1:3 reserved
                                  disp | // 4:6 disposal
                                  0 | // 7 user input - 0 = none
                                  transp)); // 8 transparency flag

            WriteShort(mDelay); // delay x 1/100 sec
            mOutput.Write((byte) mTransIndex); // transparent color index
            mOutput.Write((byte) 0); // block terminator
        }

        /// <summary>
        /// Writes Image Descriptor
        /// </summary>
        private void WriteImageDesc()
        {
            mOutput.Write((byte) 0x2c); // image separator
            WriteShort(mX); // image position x,y = 0,0
            WriteShort(mY);
            WriteShort(mWidth); // image size
            WriteShort(mHeight);
            // packed fields
            if (mFirstFrame)
            {
                // no LCT - GCT is used for first (or only) frame
                mOutput.Write((byte) 0);
            }
            else
            {
                // specify normal LCT
                mOutput.Write((byte) (0x80 | // 1 local color table 1=yes
                                      0 | // 2 interlace - 0=no
                                      0 | // 3 sorted - 0=no
                                      0 | // 4-5 reserved
                                      mPalSize)); // 6-8 size of color table
            }
        }

        /// <summary>
        /// Writes Logical Screen Descriptor
        /// </summary>
        private void WriteLsd()
        {
            // logical screen size
            WriteShort(mWidth);
            WriteShort(mHeight);
            // packed fields
            mOutput.Write((byte) (0x80 | // 1 : global color table flag = 1 (gct used)
                                  0x70 | // 2-4 : color resolution = 7
                                  0x00 | // 5 : gct sort flag = 0
                                  mPalSize)); // 6-8 : gct size

            mOutput.Write((byte) 0); // background color index
            mOutput.Write((byte) 0); // pixel aspect ratio - assume 1:1
        }

        /// <summary>
        /// Writes Netscape application extension to define repeat count.
        /// </summary>
        private void WriteNetscapeExt()
        {
            mOutput.Write((byte) 0x21); // extension introducer
            mOutput.Write((byte) 0xff); // app extension label
            mOutput.Write((byte) 11); // block size
            WriteString("NETSCAPE" + "2.0"); // app id + auth code
            mOutput.Write((byte) 3); // sub-block size
            mOutput.Write((byte) 1); // loop sub-block id
            WriteShort(mRepeat); // loop count (extra iterations, 0=repeat forever)
            mOutput.Write((byte) 0); // block terminator
        }

        /// <summary>
        /// Writes color table
        /// </summary>
        private void WritePalette()
        {
            mOutput.Write(mColorTab, 0, mColorTab.Length);
            int n = (3 * 256) - mColorTab.Length;
            for (int i = 0; i < n; i++)
            {
                mOutput.Write((byte) 0);
            }
        }

        /// <summary>
        /// Encodes and writes pixel data
        /// </summary>
        private void WritePixels()
        {
            LZWEncoder encoder = new LZWEncoder(mWidth, mHeight, mIndexedPixels, mColorDepth);
            encoder.Encode(mOutput);
        }

        /// <summary>
        /// Write 16-bit value to output stream, LSB first
        /// </summary>
        private void WriteShort(int value)
        {
            mOutput.Write((byte) (value & 0xff));
            mOutput.Write((byte) ((value >> 8) & 0xff));
        }

        /// <summary>
        /// Writes string to output stream
        /// </summary>
        private void WriteString(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                mOutput.Write((byte) s[i]);
            }
        }

        private int mWidth; // image size
        private int mHeight;
        private int mX;
        private int mY;
        private int mTransparent = -1; // transparent color if given
        private int mTransIndex; // transparent index in color table
        private int mRepeat = -1; // no repeat
        private int mDelay; // frame delay (hundredths)
        private bool mStarted; // ready to output frames
        private BinaryWriter mOutput;
        private SKBitmap mImage; // current frame
        private byte[] mPixels; // BGR byte array from frame
        private byte[] mIndexedPixels; // converted frame indexed to palette
        private int mColorDepth; // number of bit planes
        private byte[] mColorTab; // RGB palette
        private readonly bool[] mUsedEntry = new bool[256]; // active palette entries
        private int mPalSize = 7; // color table size (bits-1)
        private int mDispose = -1; // disposal code (-1 = use default)
        private bool mCloseStream; // close stream when finished
        private bool mFirstFrame = true;
        private bool mSizeSet; // if false, get size from first frame
        private int mSample = 10; // default sample interval for quantizer
    }
}
#endif
