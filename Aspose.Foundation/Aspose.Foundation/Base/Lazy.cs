// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2021 by Edward Voronov

namespace Aspose
{
    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    public abstract class Lazy<T>
        where T : class
    {
        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="Lazy{T}"/> instance.
        /// </summary>
        public T Value
        {
            get { return mValue ?? (mValue = InitValue()); }
        }

        /// <summary>
        /// The method that is invoked to produce the lazily initialized value when it is needed.
        /// </summary>
        protected abstract T InitValue();

        private T mValue;
    }
}
