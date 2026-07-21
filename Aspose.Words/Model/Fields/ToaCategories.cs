// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/04/2017 by Edward Voronov

using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a table of authorities categories.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class ToaCategories
    {
        static ToaCategories()
        {
            DefaultCategories = new ToaCategories();
        }

        public ToaCategories()
        {
            mCategoryNames[1] = "Cases";
            mCategoryNames[2] = "Statutes";
            mCategoryNames[3] = "Other Authorities";
            mCategoryNames[4] = "Rules";
            mCategoryNames[5] = "Treatises";
            mCategoryNames[6] = "Regulations";
            mCategoryNames[7] = "Constitutional Provisions";
        }

        /// <summary>
        /// Gets the default table of authorities categories.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="FieldOptions.ToaCategories"/> property to specify table of authorities categories for a single document.
        /// </remarks>
        public static ToaCategories DefaultCategories { get; }

        /// <summary>
        /// Gets or sets the category heading by category number.
        /// </summary>
        public string this[int number]
        {
            get
            {
                string categoryName = mCategoryNames[number];
                return categoryName != null
                    ? categoryName
                    : number.ToString();
            }
            set { mCategoryNames[number] = value; }
        }

        private readonly IntToObjDictionary<string> mCategoryNames = new IntToObjDictionary<string>();
    }
}
