// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

// SonarIgnoreFile
namespace Aspose.Crypto
{
    public  class CrcMaker
    {
        // 'order' [1..32] is the CRC polynom order, counted without the leading '1' bit
        // 'polynom' is the CRC polynom without leading '1' bit
        // 'direct' [0,1] specifies the kind of algorithm: 1=direct, no augmented zero bits
        // 'crcinit' is the initial CRC value belonging to that algorithm
        // 'crcxor' is the final XOR value
        // 'revin' [0,1] specifies if a data byte is reflected before processing (UART) or not
        // 'revout' [0,1] specifies if the CRC will be reflected before XOR
        // Data character string
        // For CRC-CCITT : order = 16, direct=1, poly=0x1021, CRCinit = 0xFFFF, crcxor=0; revin =0, revout=0  
        // For CRC16:      order = 16, direct=1, poly=0x8005, CRCinit = 0x0, crcxor=0x0; revin =1, revout=1  
        // For CRC32:      order = 32, direct=1, poly=0x4c11db7, CRCinit = 0xFFFFFFFF, crcxor=0xFFFFFFFF; revin =1, revout=1  
        // Default : CRC32

        private int   order      = 16;
        private long polynom    = 0x1021;
        private int   direct     = 1;
        private long crcinit    = 0xFFFF;
        private long crcxor     = 0x0;
        private int   revin      = 0;
        private int   revout     = 0;
        
        private long crcmask;
        private long crchighbit;
        private long crcinit_direct;
        private long [] crctab = new long[256];

        public CrcMaker() 
        {
            SetEncoding(CrcEncoding.CRC32);
        }

        public CrcMaker(CrcEncoding encoding) 
        {
            SetEncoding(encoding);
        }

        private CrcEncoding encoding;
        public CrcEncoding Encoding 
        {
            get { return encoding; }
        }

        private void SetEncoding(CrcEncoding encoding)
        {
            this.encoding = encoding;

            switch( encoding ) 
            {
                case CrcEncoding.CRC16:
                    order = 16; direct=1; polynom = 0x8005; crcinit = 0x0; crcxor = 0x0; revin = 1; revout = 1;
                    break;
                case CrcEncoding.CRC_CCITT_Reverse:
                    order = 16; direct = 1; polynom = 0x1021; crcinit = 0x0; crcxor = 0x0; revin = 1; revout = 1;
                    break;
                case CrcEncoding.CRC_CCITT:
                    order = 16; direct = 1; polynom = 0x1021; crcinit = 0xFFFF; crcxor = 0x0; revin = 0; revout = 0;
                    break;
                case CrcEncoding.CRC32:
                default:
                    order = 32; direct=1; polynom = 0x4c11db7; crcinit = 0xFFFFFFFF; crcxor = 0xFFFFFFFF; revin = 1; revout = 1;  
                    break;
            }
            
            // Initialize all variables for seeding and builing based upon the given coding type
            // at first, compute constant bit masks for whole CRC and CRC high bit
            
            crcmask = ((((long)1<<(order-1))-1)<<1)|1;
            crchighbit = (long)1<<(order-1);

            // generate lookup table
            generate_crc_table();

            long bit, crc;
            int i;
            if ( direct == 0 ) 
            {
                crc = crcinit;
                for (i=0; i<order; i++) 
                {
                    bit = crc & crchighbit;
                    crc<<= 1;
                    if ( bit != 0 ) 
                    {
                        crc^= polynom;
                    }
                }
                crc&= crcmask;
                crcinit_direct = crc;
            }
            else 
            {
                crcinit_direct = crcinit;
                crc = crcinit;
                for (i=0; i<order; i++) 
                {
                    bit = crc & 1;
                    if (bit != 0) 
                    {
                        crc^= polynom;
                    }
                    crc >>= 1;
                }    
            }
        }

        public int MakeCRC(byte[] p) 
        {
            // fast lookup table algorithm without augmented zero bytes, e.g. used in pkzip.
            // only usable with polynom orders of 8, 16, 24 or 32.
            long crc = crcinit_direct;
            if ( revin != 0 ) 
            {
                crc = reflect(crc, order);
            }
            if ( revin == 0 ) 
            {
                for ( int i = 0; i < p.Length; i++ ) 
                {
                    crc = (crc << 8) ^ crctab[(int)(((crc >> (order - 8)) & 0xff) ^ p[i])]; 
                }
            }
            else 
            {
                for ( int i = 0; i < p.Length; i++ ) 
                {
                    crc = (crc >> 8) ^ crctab[(int)((crc & 0xff) ^ p[i])];
                }
            }
            if ( (revout^revin) != 0 ) 
            {
                crc = reflect(crc, order);
            }
            crc^= crcxor;
            crc&= crcmask;
            return (int)crc;
        }

        private static long reflect(long crc, int bitnum) 
        {

            // reflects the lower 'bitnum' bits of 'crc'

            long i, j=1, crcout = 0;

            for ( i = (long)1 <<(bitnum-1); i != 0; i>>=1) 
            {
                if ( ( crc & i ) != 0 ) 
                {
                    crcout |= j;
                }
                j<<= 1;
            }
            return (crcout);
        }

        private void generate_crc_table() 
        {

            // make CRC lookup table used by table algorithms

            int i, j;
            long bit, crc;

            for (i=0; i<256; i++) 
            {
                crc=(long)i;
                if ( revin !=0 ) 
                {
                    crc=reflect(crc, 8);
                }
                crc<<= order-8;

                for (j=0; j<8; j++) 
                {
                    bit = crc & crchighbit;
                    crc<<= 1;
                    if ( bit !=0 ) crc^= polynom;
                }            

                if (revin != 0) 
                {
                    crc = reflect(crc, order);
                }
                crc&= crcmask;
                crctab[i]= crc;
            }
        }

    }
}
