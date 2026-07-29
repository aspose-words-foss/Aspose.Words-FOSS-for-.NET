// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using Aspose.Common;

namespace Aspose.Words.Fields
{
    internal class RichStringBehaviour : IStringBehaviour
    {
        private RichStringBehaviour()
        {
        }

        internal static readonly RichStringBehaviour Instance = new RichStringBehaviour();

        private static IStringBehaviour Behaviour
        {
            get { return Instance; }
        }

        IString IStringBehaviour.EmptyString
        {
            get { return RichString.Empty; }
        }

        bool IStringBehaviour.IsNullOrEmpty(IString value)
        {
            return value == null || value.Length == 0;
        }

        IString IStringBehaviour.CreateString(string value)
        {
            return RichString.CreateFromString(value);
        }

        IStringBuilder IStringBehaviour.CreateBuilder()
        {
            return new RichStringBuilder();
        }

        public IStringBuilder CreateBuilder(IString value)
        {
            return new RichStringBuilder((RichString)value);
        }

        IStringBuilder IStringBehaviour.CreateBuilder(int capacity)
        {
            return new RichStringBuilder(capacity);
        }

        internal static bool IsNullOrEmptyInternal(RichString value)
        {
            return Behaviour.IsNullOrEmpty(value);
        }
    }
}
