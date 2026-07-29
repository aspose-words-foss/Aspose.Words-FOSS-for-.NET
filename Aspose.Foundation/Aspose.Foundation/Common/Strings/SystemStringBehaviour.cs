// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System.Text;

namespace Aspose.Common
{
    public class SystemStringBehaviour : IStringBehaviour
    {
        private SystemStringBehaviour()
        {
        }

        public static readonly SystemStringBehaviour Instance = new SystemStringBehaviour();

        bool IStringBehaviour.IsNullOrEmpty(IString value)
        {
            if (value == null)
                return true;

            return string.IsNullOrEmpty(((SystemStringAdapter)value).Value);
        }

        IString IStringBehaviour.CreateString(string value)
        {
            return SystemStringAdapter.Create(value);
        }

        IStringBuilder IStringBehaviour.CreateBuilder()
        {
            return new SystemStringBuilderAdapter();
        }

        IStringBuilder IStringBehaviour.CreateBuilder(IString value)
        {
            return new SystemStringBuilderAdapter(new StringBuilder(((SystemStringAdapter)value).Value));
        }

        IStringBuilder IStringBehaviour.CreateBuilder(int capacity)
        {
            return new SystemStringBuilderAdapter(capacity);
        }

        IString IStringBehaviour.EmptyString
        {
            get { return SystemStringAdapter.Empty; }
        }
    }
}
