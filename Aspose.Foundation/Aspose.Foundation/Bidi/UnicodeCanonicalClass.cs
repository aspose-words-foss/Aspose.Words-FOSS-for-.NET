// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// The different canonical classes of Unicode characters.
    /// </summary>
    public enum UnicodeCanonicalClass
    {
        ///<summary>Not Reordered</summary>
        ///<remarks>Spacing, split, enclosing, reordrant, and Tibetan subjoined.</remarks>
        NR = 0,
        
        ///<summary>Overlays and interior</summary>
        OV = 1,
        
        ///<summary>Nuktas</summary>
        NK = 7,
        
        ///<summary>Hiragana/Katakana voicing marks</summary>
        KV = 8,

        ///<summary>Viramas</summary>
        VR = 9,
        
        #region Fixed Position Classes
        /// <summary>
        /// General class level 10.
        /// </summary>
        CLASS_10 = 10,
        /// <summary>
        /// General class level 11.
        /// </summary>
        CLASS_11 = 11,
        /// <summary>
        /// General class level 12.
        /// </summary>
        CLASS_12 = 12,
        /// <summary>
        /// General class level 13.
        /// </summary>
        CLASS_13 = 13,
        /// <summary>
        /// General class level 14.
        /// </summary>
        CLASS_14 = 14,
        /// <summary>
        /// General class level 15.
        /// </summary>
        CLASS_15 = 15,
        /// <summary>
        /// General class level 16.
        /// </summary>
        CLASS_16 = 16,
        /// <summary>
        /// General class level 17.
        /// </summary>
        CLASS_17 = 17,
        /// <summary>
        /// General class level 18.
        /// </summary>
        CLASS_18 = 18,
        /// <summary>
        /// General class level 19.
        /// </summary>
        CLASS_19 = 19,
        /// <summary>
        /// General class level 20.
        /// </summary>
        CLASS_20 = 20,
        /// <summary>
        /// General class level 21.
        /// </summary>
        CLASS_21 = 21,
        /// <summary>
        /// General class level 22.
        /// </summary>
        CLASS_22 = 22,
        /// <summary>
        /// General class level 23.
        /// </summary>
        CLASS_23 = 23,
        /// <summary>
        /// General class level 24.
        /// </summary>
        CLASS_24 = 24,
        /// <summary>
        /// General class level 25.
        /// </summary>
        CLASS_25 = 25,
        /// <summary>
        /// General class level 26.
        /// </summary>
        CLASS_26 = 26,
        /// <summary>
        /// General class level 27.
        /// </summary>
        CLASS_27 = 27,
        /// <summary>
        /// General class level 28.
        /// </summary>
        CLASS_28 = 28,
        /// <summary>
        /// General class level 29.
        /// </summary>
        CLASS_29 = 29,
        /// <summary>
        /// General class level 30.
        /// </summary>
        CLASS_30 = 30,
        /// <summary>
        /// General class level 31.
        /// </summary>
        CLASS_31 = 31,
        /// <summary>
        /// General class level 32.
        /// </summary>
        CLASS_32 = 32,
        /// <summary>
        /// General class level 33.
        /// </summary>
        CLASS_33 = 33,
        /// <summary>
        /// General class level 34.
        /// </summary>
        CLASS_34 = 34,
        /// <summary>
        /// General class level 35.
        /// </summary>
        CLASS_35 = 35,
        /// <summary>
        /// General class level 36.
        /// </summary>
        CLASS_36 = 36,
        /// <summary>
        /// General class level 37.
        /// </summary>
        CLASS_37 = 37,
        /// <summary>
        /// General class level 38.
        /// </summary>
        CLASS_38 = 38,
        /// <summary>
        /// General class level 39.
        /// </summary>
        CLASS_39 = 39,
        /// <summary>
        /// General class level 40.
        /// </summary>
        CLASS_40 = 40,
        /// <summary>
        /// General class level 41.
        /// </summary>
        CLASS_41 = 41,
        /// <summary>
        /// General class level 42.
        /// </summary>
        CLASS_42 = 42,
        /// <summary>
        /// General class level 43.
        /// </summary>
        CLASS_43 = 43,
        /// <summary>
        /// General class level 44.
        /// </summary>
        CLASS_44 = 44,
        /// <summary>
        /// General class level 45.
        /// </summary>
        CLASS_45 = 45,
        /// <summary>
        /// General class level 46.
        /// </summary>
        CLASS_46 = 46,
        /// <summary>
        /// General class level 47.
        /// </summary>
        CLASS_47 = 47,
        /// <summary>
        /// General class level 48.
        /// </summary>
        CLASS_48 = 48,
        /// <summary>
        /// General class level 49.
        /// </summary>
        CLASS_49 = 49,
        /// <summary>
        /// General class level 50.
        /// </summary>
        CLASS_50 = 50,
        /// <summary>
        /// General class level 51.
        /// </summary>
        CLASS_51 = 51,
        /// <summary>
        /// General class level 52.
        /// </summary>
        CLASS_52 = 52,
        /// <summary>
        /// General class level 53.
        /// </summary>
        CLASS_53 = 53,
        /// <summary>
        /// General class level 54.
        /// </summary>
        CLASS_54 = 54,
        /// <summary>
        /// General class level 55.
        /// </summary>
        CLASS_55 = 55,
        /// <summary>
        /// General class level 56.
        /// </summary>
        CLASS_56 = 56,
        /// <summary>
        /// General class level 57.
        /// </summary>
        CLASS_57 = 57,
        /// <summary>
        /// General class level 58.
        /// </summary>
        CLASS_58 = 58,
        /// <summary>
        /// General class level 59.
        /// </summary>
        CLASS_59 = 59,
        /// <summary>
        /// General class level 60.
        /// </summary>
        CLASS_60 = 60,
        /// <summary>
        /// General class level 61.
        /// </summary>
        CLASS_61 = 61,
        /// <summary>
        /// General class level 62.
        /// </summary>
        CLASS_62 = 62,
        /// <summary>
        /// General class level 63.
        /// </summary>
        CLASS_63 = 63,
        /// <summary>
        /// General class level 64.
        /// </summary>
        CLASS_64 = 64,
        /// <summary>
        /// General class level 65.
        /// </summary>
        CLASS_65 = 65,
        /// <summary>
        /// General class level 66.
        /// </summary>
        CLASS_66 = 66,
        /// <summary>
        /// General class level 67.
        /// </summary>
        CLASS_67 = 67,
        /// <summary>
        /// General class level 68.
        /// </summary>
        CLASS_68 = 68,
        /// <summary>
        /// General class level 69.
        /// </summary>
        CLASS_69 = 69,
        /// <summary>
        /// General class level 70.
        /// </summary>
        CLASS_70 = 70,
        /// <summary>
        /// General class level 71.
        /// </summary>
        CLASS_71 = 71,
        /// <summary>
        /// General class level 72.
        /// </summary>
        CLASS_72 = 72,
        /// <summary>
        /// General class level 73.
        /// </summary>
        CLASS_73 = 73,
        /// <summary>
        /// General class level 74.
        /// </summary>
        CLASS_74 = 74,
        /// <summary>
        /// General class level 75.
        /// </summary>
        CLASS_75 = 75,
        /// <summary>
        /// General class level 76.
        /// </summary>
        CLASS_76 = 76,
        /// <summary>
        /// General class level 77.
        /// </summary>
        CLASS_77 = 77,
        /// <summary>
        /// General class level 78.
        /// </summary>
        CLASS_78 = 78,
        /// <summary>
        /// General class level 79.
        /// </summary>
        CLASS_79 = 79,
        /// <summary>
        /// General class level 80.
        /// </summary>
        CLASS_80 = 80,
        /// <summary>
        /// General class level 81.
        /// </summary>
        CLASS_81 = 81,
        /// <summary>
        /// General class level 82.
        /// </summary>
        CLASS_82 = 82,
        /// <summary>
        /// General class level 83.
        /// </summary>
        CLASS_83 = 83,
        /// <summary>
        /// General class level 84.
        /// </summary>
        CLASS_84 = 84,
        /// <summary>
        /// General class level 85.
        /// </summary>
        CLASS_85 = 85,
        /// <summary>
        /// General class level 86.
        /// </summary>
        CLASS_86 = 86,
        /// <summary>
        /// General class level 87.
        /// </summary>
        CLASS_87 = 87,
        /// <summary>
        /// General class level 88.
        /// </summary>
        CLASS_88 = 88,
        /// <summary>
        /// General class level 89.
        /// </summary>
        CLASS_89 = 89,
        /// <summary>
        /// General class level 90.
        /// </summary>
        CLASS_90 = 90,
        /// <summary>
        /// General class level 91.
        /// </summary>
        CLASS_91 = 91,
        /// <summary>
        /// General class level 92.
        /// </summary>
        CLASS_92 = 92,
        /// <summary>
        /// General class level 93.
        /// </summary>
        CLASS_93 = 93,
        /// <summary>
        /// General class level 94.
        /// </summary>
        CLASS_94 = 94,
        /// <summary>
        /// General class level 95.
        /// </summary>
        CLASS_95 = 95,
        /// <summary>
        /// General class level 96.
        /// </summary>
        CLASS_96 = 96,
        /// <summary>
        /// General class level 97.
        /// </summary>
        CLASS_97 = 97,
        /// <summary>
        /// General class level 98.
        /// </summary>
        CLASS_98 = 98,
        /// <summary>
        /// General class level 99.
        /// </summary>
        CLASS_99 = 99,
        /// <summary>
        /// General class level 100.
        /// </summary>
        CLASS_100 = 100,
        /// <summary>
        /// General class level 101.
        /// </summary>
        CLASS_101 = 101,
        /// <summary>
        /// General class level 102.
        /// </summary>
        CLASS_102 = 102,
        /// <summary>
        /// General class level 103.
        /// </summary>
        CLASS_103 = 103,
        /// <summary>
        /// General class level 104.
        /// </summary>
        CLASS_104 = 104,
        /// <summary>
        /// General class level 105.
        /// </summary>
        CLASS_105 = 105,
        /// <summary>
        /// General class level 106.
        /// </summary>
        CLASS_106 = 106,
        /// <summary>
        /// General class level 107.
        /// </summary>
        CLASS_107 = 107,
        /// <summary>
        /// General class level 108.
        /// </summary>
        CLASS_108 = 108,
        /// <summary>
        /// General class level 109.
        /// </summary>
        CLASS_109 = 109,
        /// <summary>
        /// General class level 110.
        /// </summary>
        CLASS_110 = 110,
        /// <summary>
        /// General class level 111.
        /// </summary>
        CLASS_111 = 111,
        /// <summary>
        /// General class level 112.
        /// </summary>
        CLASS_112 = 112,
        /// <summary>
        /// General class level 113.
        /// </summary>
        CLASS_113 = 113,
        /// <summary>
        /// General class level 114.
        /// </summary>
        CLASS_114 = 114,
        /// <summary>
        /// General class level 115.
        /// </summary>
        CLASS_115 = 115,
        /// <summary>
        /// General class level 116.
        /// </summary>
        CLASS_116 = 116,
        /// <summary>
        /// General class level 117.
        /// </summary>
        CLASS_117 = 117,
        /// <summary>
        /// General class level 118.
        /// </summary>
        CLASS_118 = 118,
        /// <summary>
        /// General class level 119.
        /// </summary>
        CLASS_119 = 119,
        /// <summary>
        /// General class level 120.
        /// </summary>
        CLASS_120 = 120,
        /// <summary>
        /// General class level 121.
        /// </summary>
        CLASS_121 = 121,
        /// <summary>
        /// General class level 122.
        /// </summary>
        CLASS_122 = 122,
        /// <summary>
        /// General class level 123.
        /// </summary>
        CLASS_123 = 123,
        /// <summary>
        /// General class level 124.
        /// </summary>
        CLASS_124 = 124,
        /// <summary>
        /// General class level 125.
        /// </summary>
        CLASS_125 = 125,
        /// <summary>
        /// General class level 126.
        /// </summary>
        CLASS_126 = 126,
        /// <summary>
        /// General class level 127.
        /// </summary>
        CLASS_127 = 127,
        /// <summary>
        /// General class level 128.
        /// </summary>
        CLASS_128 = 128,
        /// <summary>
        /// General class level 129.
        /// </summary>
        CLASS_129 = 129,
        /// <summary>
        /// General class level 130.
        /// </summary>
        CLASS_130 = 130,
        /// <summary>
        /// General class level 131.
        /// </summary>
        CLASS_131 = 131,
        /// <summary>
        /// General class level 132.
        /// </summary>
        CLASS_132 = 132,
        /// <summary>
        /// General class level 133.
        /// </summary>
        CLASS_133 = 133,
        /// <summary>
        /// General class level 134.
        /// </summary>
        CLASS_134 = 134,
        /// <summary>
        /// General class level 135.
        /// </summary>
        CLASS_135 = 135,
        /// <summary>
        /// General class level 136.
        /// </summary>
        CLASS_136 = 136,
        /// <summary>
        /// General class level 137.
        /// </summary>
        CLASS_137 = 137,
        /// <summary>
        /// General class level 138.
        /// </summary>
        CLASS_138 = 138,
        /// <summary>
        /// General class level 139.
        /// </summary>
        CLASS_139 = 139,
        /// <summary>
        /// General class level 140.
        /// </summary>
        CLASS_140 = 140,
        /// <summary>
        /// General class level 141.
        /// </summary>
        CLASS_141 = 141,
        /// <summary>
        /// General class level 142.
        /// </summary>
        CLASS_142 = 142,
        /// <summary>
        /// General class level 143.
        /// </summary>
        CLASS_143 = 143,
        /// <summary>
        /// General class level 144.
        /// </summary>
        CLASS_144 = 144,
        /// <summary>
        /// General class level 145.
        /// </summary>
        CLASS_145 = 145,
        /// <summary>
        /// General class level 146.
        /// </summary>
        CLASS_146 = 146,
        /// <summary>
        /// General class level 147.
        /// </summary>
        CLASS_147 = 147,
        /// <summary>
        /// General class level 148.
        /// </summary>
        CLASS_148 = 148,
        /// <summary>
        /// General class level 149.
        /// </summary>
        CLASS_149 = 149,
        /// <summary>
        /// General class level 150.
        /// </summary>
        CLASS_150 = 150,
        /// <summary>
        /// General class level 151.
        /// </summary>
        CLASS_151 = 151,
        /// <summary>
        /// General class level 152.
        /// </summary>
        CLASS_152 = 152,
        /// <summary>
        /// General class level 153.
        /// </summary>
        CLASS_153 = 153,
        /// <summary>
        /// General class level 154.
        /// </summary>
        CLASS_154 = 154,
        /// <summary>
        /// General class level 155.
        /// </summary>
        CLASS_155 = 155,
        /// <summary>
        /// General class level 156.
        /// </summary>
        CLASS_156 = 156,
        /// <summary>
        /// General class level 157.
        /// </summary>
        CLASS_157 = 157,
        /// <summary>
        /// General class level 158.
        /// </summary>
        CLASS_158 = 158,
        /// <summary>
        /// General class level 159.
        /// </summary>
        CLASS_159 = 159,
        /// <summary>
        /// General class level 160.
        /// </summary>
        CLASS_160 = 160,
        /// <summary>
        /// General class level 161.
        /// </summary>
        CLASS_161 = 161,
        /// <summary>
        /// General class level 162.
        /// </summary>
        CLASS_162 = 162,
        /// <summary>
        /// General class level 163.
        /// </summary>
        CLASS_163 = 163,
        /// <summary>
        /// General class level 164.
        /// </summary>
        CLASS_164 = 164,
        /// <summary>
        /// General class level 165.
        /// </summary>
        CLASS_165 = 165,
        /// <summary>
        /// General class level 166.
        /// </summary>
        CLASS_166 = 166,
        /// <summary>
        /// General class level 167.
        /// </summary>
        CLASS_167 = 167,
        /// <summary>
        /// General class level 168.
        /// </summary>
        CLASS_168 = 168,
        /// <summary>
        /// General class level 169.
        /// </summary>
        CLASS_169 = 169,
        /// <summary>
        /// General class level 170.
        /// </summary>
        CLASS_170 = 170,
        /// <summary>
        /// General class level 171.
        /// </summary>
        CLASS_171 = 171,
        /// <summary>
        /// General class level 172.
        /// </summary>
        CLASS_172 = 172,
        /// <summary>
        /// General class level 173.
        /// </summary>
        CLASS_173 = 173,
        /// <summary>
        /// General class level 174.
        /// </summary>
        CLASS_174 = 174,
        /// <summary>
        /// General class level 175.
        /// </summary>
        CLASS_175 = 175,
        /// <summary>
        /// General class level 176.
        /// </summary>
        CLASS_176 = 176,
        /// <summary>
        /// General class level 177.
        /// </summary>
        CLASS_177 = 177,
        /// <summary>
        /// General class level 178.
        /// </summary>
        CLASS_178 = 178,
        /// <summary>
        /// General class level 179.
        /// </summary>
        CLASS_179 = 179,
        /// <summary>
        /// General class level 180.
        /// </summary>
        CLASS_180 = 180,
        /// <summary>
        /// General class level 181.
        /// </summary>
        CLASS_181 = 181,
        /// <summary>
        /// General class level 182.
        /// </summary>
        CLASS_182 = 182,
        /// <summary>
        /// General class level 183.
        /// </summary>
        CLASS_183 = 183,
        /// <summary>
        /// General class level 184.
        /// </summary>
        CLASS_184 = 184,
        /// <summary>
        /// General class level 185.
        /// </summary>
        CLASS_185 = 185,
        /// <summary>
        /// General class level 186.
        /// </summary>
        CLASS_186 = 186,
        /// <summary>
        /// General class level 187.
        /// </summary>
        CLASS_187 = 187,
        /// <summary>
        /// General class level 188.
        /// </summary>
        CLASS_188 = 188,
        /// <summary>
        /// General class level 189.
        /// </summary>
        CLASS_189 = 189,
        /// <summary>
        /// General class level 190.
        /// </summary>
        CLASS_190 = 190,
        /// <summary>
        /// General class level 191.
        /// </summary>
        CLASS_191 = 191,
        /// <summary>
        /// General class level 192.
        /// </summary>
        CLASS_192 = 192,
        /// <summary>
        /// General class level 193.
        /// </summary>
        CLASS_193 = 193,
        /// <summary>
        /// General class level 194.
        /// </summary>
        CLASS_194 = 194,
        /// <summary>
        /// General class level 195.
        /// </summary>
        CLASS_195 = 195,
        /// <summary>
        /// General class level 196.
        /// </summary>
        CLASS_196 = 196,
        /// <summary>
        /// General class level 197.
        /// </summary>
        CLASS_197 = 197,
        /// <summary>
        /// General class level 198.
        /// </summary>
        CLASS_198 = 198,
        /// <summary>
        /// General class level 199.
        /// </summary>
        CLASS_199 = 199,
        #endregion
        
        ///<summary>Attached Below Left</summary>
        ATBL = 200,

        ///<summary>Attached Below</summary>
        ATB = 202,
        
        ///<summary>Attached Below Right</summary>
        ATBR = 204,
        
        ///<summary>Attached Left</summary>
        ///<remarks>Reordrant around single base character.</remarks>
        ATL = 208,
        
        ///<summary>Attached Right</summary>
        ATR = 210,
        
        ///<summary>Attached Above Left</summary>
        ATAL = 212,
        
        ///<summary>Attached Above</summary>
        ATA = 214,
        
        ///<summary>Attached Above Right</summary>
        ATAR = 216,
        
        ///<summary>Below Left</summary>
        BL = 218,
        
        ///<summary>Below</summary>
        B = 220,
        
        ///<summary>Below Right</summary>
        BR = 222,
        
        ///<summary>Left</summary>
        ///<remarks>Reordrant around single base character.</remarks>
        L = 224,
        
        ///<summary>Right</summary>
        R = 226,
        
        ///<summary>Above Left</summary>
        AL = 228,
        
        ///<summary>Above</summary>
        A = 230,
        
        ///<summary>Above Right</summary>
        AR = 232,
        
        ///<summary>Double Below</summary>
        DB = 233,
        
        ///<summary>Double Above</summary>
        DA = 234,
        
        ///<summary>Iota Subscript</summary>
        IS = 240
    }
}
