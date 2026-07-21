// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.Collections;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Utility functions for handling TrueType instructions.
    /// </summary>
    internal static class TTInstructionsUtil
    {
        /// <summary>
        /// Reads push data from binary reader.
        /// </summary>
        /// <returns>List of push values.</returns>
        /// <remarks>
        /// TrueType instructions in most cases starts with several push instructions.
        /// "Push data" refers to data which is pushed to the stack by these instructions.
        /// Same values may be pushed using different set of push instructions.
        /// </remarks>
        internal static IntList ReadPushData(BigEndianBinaryReader reader)
        {
            IntList pushValues = IntList.CreateAllocated();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Read instruction OpCode.
                short opCode = reader.ReadByte();

                // Check OpCode. 
                // If OpCode refers to push instruction - collect value format and count.
                // If not a push instruction - push block is ended.
                bool isPushWordCommand;
                int pushValuesCount;
                if (opCode == OpcodeNPushB)
                {
                    isPushWordCommand = false;
                    pushValuesCount = reader.ReadByte();
                }
                else if (opCode == OpcodeNPushW)
                {
                    isPushWordCommand = true;
                    pushValuesCount = reader.ReadByte();
                }
                else if (opCode >= OpcodePushB0 && opCode <= OpcodePushB7)
                {
                    isPushWordCommand = false;
                    pushValuesCount = opCode - OpcodePushB0 + 1;
                }
                else if (opCode >= OpcodePushW0 && opCode <= OpcodePushW7)
                {
                    isPushWordCommand = true;
                    pushValuesCount = opCode - OpcodePushW0 + 1;
                }
                else
                {
                    // Not a push instruction. Push data ends here.
                    reader.BaseStream.Position--;
                    break;
                }

                // Read push values.
                for (int i = 0; i < pushValuesCount; i++)
                {
                    pushValues.Add(isPushWordCommand ? reader.ReadInt16() : reader.ReadByte());
                }
            }

            return pushValues;
        }

        /// <summary>
        /// Writes push data to binary writer.
        /// </summary>
        public static void WritePushData(BigEndianBinaryWriter writer, short[] pushValues)
        {
            // Note: He is simple writing of all push values as words using NPushW command.
            // Output size can be decreased if we utilize other push commands.
            int position = 0;
            while (position < pushValues.Length)
            {
                int valuesToWrite = Math.Min(255, pushValues.Length - position);

                WriteNPushWCommand(writer, valuesToWrite);
                for (int i = 0; i < valuesToWrite; i++)
                {
                    writer.WriteUInt16(pushValues[position++]);
                }
            }
        }

        private static void WriteNPushWCommand(BigEndianBinaryWriter writer, int n)
        {
            writer.WriteByte((byte)OpcodeNPushW);
            writer.WriteByte((byte)n);
        }

        private const short OpcodeNPushB = 0x40;
        private const short OpcodeNPushW = 0x41;
        private const short OpcodePushB0 = 0xB0;
        private const short OpcodePushB7 = 0xB7;
        private const short OpcodePushW0 = 0xB8;
        private const short OpcodePushW7 = 0xBF;
    }
}
