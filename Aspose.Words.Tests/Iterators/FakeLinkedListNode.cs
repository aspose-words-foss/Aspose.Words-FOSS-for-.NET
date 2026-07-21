// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.RW.Markdown;

namespace Aspose.Words.Tests
{
    internal class FakeLinkedListNode : ILinkedListNode
    {
        public FakeLinkedListNode(string text)
        {
            mText = text;
            Index = Int32.MinValue;
        }

        public override string ToString()
        {
            return mText;
        }
        public ILinkedListNode NextNode
        {
            get { return mNextNode; }
            set { mNextNode = value; }
        }

        public ILinkedListNode PrevNode
        {
            get { return mPrevNode; }
            set { mPrevNode = value; }
        }

        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        public long SecondaryIndex
        {
            get { return mSecondaryIndex; }
            set { mSecondaryIndex = value; }
        }

        public bool IsNotIncluded
        {
            get { return Index == Int32.MinValue; }
        }

        public void MarkAsRemoved()
        {
            Index = Int32.MinValue;
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return obj.ToString() == this.ToString();
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return mText.GetHashCode();
        }

        private readonly string mText;
        private ILinkedListNode mNextNode;
        private ILinkedListNode mPrevNode;
        private int mIndex;
        private long mSecondaryIndex;
    }
}
