// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2011 by Dmitry Matveenko

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents options to control field handling in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public sealed class FieldOptions
    {
        /// <summary>
        /// The default constructor is internal.
        /// There is no point in creating FieldOptions instance for a public API user: Document.FieldOptions is read-only.
        /// </summary>
        internal FieldOptions(Document document)
        {
            mDocument = document;
        }

        /// <summary>
        /// Specifies what culture to use to format the field result.
        /// </summary>
        /// <remarks>
        /// <para>By default, the culture of the current thread is used.</para>
        /// <para>The setting affects only date/time fields with \\@ format switch.</para>
        /// </remarks>
        public FieldUpdateCultureSource FieldUpdateCultureSource { get; set; }

        /// <summary>
        /// Gets or sets a provider that returns a culture object specific for each particular field.
        /// </summary>
        /// <remarks>
        /// <para>The provider is requested when the value of <see cref="FieldUpdateCultureSource"/> is <see cref="FieldUpdateCultureSource.FieldCode"/>.</para>
        /// <para>If the provider is present, then the culture object it returns is used for the field update. Otherwise, a system culture is used.</para>
        /// </remarks>
        public IFieldUpdateCultureProvider FieldUpdateCultureProvider { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether bidirectional text is fully supported during field update or not.
        /// </summary>
        /// <remarks>
        /// <para>When this property is set to <c>true</c>, additional steps are performed to produce Right-To-Left language
        /// (i.e. Arabic or Hebrew) compatible field result during its update.</para>
        /// <para>When this property is set to <c>false</c> and Right-To-Left language is used, correctness of field result
        /// after its update is not guaranteed.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </remarks>
        public bool IsBidiTextSupportedOnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the respondent to user prompts during field update.
        /// </summary>
        /// <remarks>
        /// <para>If the value of this property is set to <c>null</c>, the fields that require user response on prompting
        /// (such as <see cref="FieldAsk"/> or <see cref="FieldFillIn"/>) are not updated.</para>
        /// <para>The default value is <c>null</c>.</para>
        /// </remarks>
        public IFieldUserPromptRespondent UserPromptRespondent { get; set; }

        /// <summary>
        /// Gets or sets the field comparison expressions evaluator.
        /// </summary>
        /// <seealso cref="IComparisonExpressionEvaluator"/>
        public IComparisonExpressionEvaluator ComparisonExpressionEvaluator { get; set; }

        /// <summary>
        /// Gets or sets default document author's name. If author's name is already specified in built-in document properties,
        /// this option is not considered.
        /// </summary>
        public string DefaultDocumentAuthor { get; set; }

        /// <summary>
        /// Gets or sets custom style separator for the \t switch in <see cref="FieldToc"/> field.
        /// </summary>
        /// <remarks>
        /// By default, custom styles defined by the \t switch in the <see cref="FieldToc"/> field are separated by a delimiter taken from the current culture.
        /// This property overrides that behaviour by specifying a user defined delimiter.
        /// </remarks>
        public string CustomTocStyleSeparator { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether legacy (early than AW 13.10) number format for fields is enabled or not.
        /// </summary>
        /// <remarks>
        /// <para>When this property is set to <c>true</c>, template symbol "#" worked as in .net:
        /// Replaces the pound sign with the corresponding digit if one is present; otherwise, no symbols appears in the result string.</para>
        /// <para>When this property is set to <c>false</c>, template symbol "#" works as MS Word:
        /// This format item specifies the requisite numeric places to display in the result.
        /// If the result does not include a digit in that place, MS Word displays a space. For example, { = 9 + 6 \# $### } displays $ 15.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </remarks>
        public bool LegacyNumberFormat { get; set; }

        /// <summary>
        ///  Gets or sets the value indicating that number format is parsed using invariant culture or not
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this property is set to <c>true</c>, number format is taken from an invariant culture.
        /// </para>
        /// <para>
        /// When this property is set to <c>false</c>, number format is taken from the current thread's culture.
        /// </para>
        /// <para>The default value is <c>false</c>.</para>
        /// </remarks>
        public bool UseInvariantCultureNumberFormat { get; set; }

        internal NumberFormattingOptions GetNumberFormattingOptions()
        {
            NumberFormattingOptions options = NumberFormattingOptions.Default;
            options = NumberFormattingOptionsUtil.WithLegacyNumberFormat(options, LegacyNumberFormat);
            options = NumberFormattingOptionsUtil.WithFormatIsInInvariantCulture(options, UseInvariantCultureNumberFormat);
            return options;
        }

        /// <summary>
        /// Gets or set custom barcode generator.
        /// </summary>
        /// <remarks>
        /// Custom barcode generator should implement public interface <see cref="IBarcodeGenerator"/>.
        /// </remarks>
        public IBarcodeGenerator BarcodeGenerator { get; set; }

        /// <summary>
        /// Gets or sets a provider that returns a query result for the <see cref="FieldDatabase"/> field.
        /// </summary>
        public IFieldDatabaseProvider FieldDatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets a provider that returns a bibliography style for
        /// the <see cref="FieldBibliography"/> and <see cref="FieldCitation"/> fields.
        /// </summary>
        public IBibliographyStylesProvider BibliographyStylesProvider { get; set; }

        /// <summary>
        /// Gets or sets the culture to preprocess field values.
        /// </summary>
        /// <remarks>
        /// <p>Currently this property only affects value of the <see cref="FieldDocProperty"/> field.</p>
        /// <p>The default value is <c>null</c>. When this property is set to <c>null</c>, the <see cref="FieldDocProperty"/> field's value is preprocessed
        /// with the culture controlled by the <see cref="FieldUpdateCultureSource"/> property.</p>
        /// </remarks>
        public CultureInfo PreProcessCulture { get; set; }

        /// <summary>
        /// Gets or sets the current user information.
        /// </summary>
        public UserInformation CurrentUser { get; set; }

        internal UserInformation EffectiveUser
        {
            get { return CurrentUser != null ? CurrentUser : UserInformation.DefaultUser; }
        }

        /// <summary>
        /// Gets or sets the table of authorities categories.
        /// </summary>
        public ToaCategories ToaCategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="FieldIndexFormat"/> that represents
        /// the formatting for the <see cref="FieldIndex"/> fields in the document.
        /// </summary>
        public FieldIndexFormat FieldIndexFormat
        {
            get { return FieldIndexFormatApplier.Identify(mDocument); }
            set { FieldIndexFormatApplier.Apply(mDocument, value); }
        }

        internal ToaCategories EffectiveToaCategories
        {
            get { return ToaCategories != null ? ToaCategories : ToaCategories.DefaultCategories; }
        }

        /// <summary>
        /// Gets or sets the file name of the document.
        /// </summary>
        /// <remarks>
        /// <p>This property is used by the <see cref="FieldFileName"/> field with higher priority than the <see cref="Document.OriginalFileName"/> property.</p>
        /// </remarks>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file name of the template used by the document.
        /// </summary>
        /// <remarks>
        /// <p>This property is used by the <see cref="FieldTemplate"/> field if the <see cref="Document.AttachedTemplate"/> property is empty.</p>
        /// <p>If this property is empty, the default template file name <c>Normal.dotm</c> is used.</p>
        /// </remarks>
        public string TemplateName { get; set; }

        /// <summary>
        /// Allows to control how the field result is formatted.
        /// </summary>
        public IFieldResultFormatter ResultFormatter { get; set; }

        /// <summary>
        /// Gets or sets paths of MS Word built-in templates.
        /// </summary>
        /// <remarks>
        /// <p>This property is used by the <see cref="FieldAutoText"/> and <see cref="FieldGlossary"/> fields, if referenced auto text entry is not found in the <see cref="Document.AttachedTemplate"/> template.</p>
        /// <p>By default MS Word stores built-in templates in c:\Users\&lt;username&gt;\AppData\Roaming\Microsoft\Document Building Blocks\1033\16\Built-In Building Blocks.dotx and
        /// C:\Users\&lt;username&gt;\AppData\Roaming\Microsoft\Templates\Normal.dotm files.</p>
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public string[] BuiltInTemplatesPaths
        {
            get { return mBuiltInTemplatesPaths; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mBuiltInTemplatesPaths = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="IFieldUpdatingCallback"/> implementation
        /// </summary>
        public IFieldUpdatingCallback FieldUpdatingCallback { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IFieldUpdatingProgressCallback"/> implementation.
        /// </summary>
        public IFieldUpdatingProgressCallback FieldUpdatingProgressCallback { get; set; }

        private string[] mBuiltInTemplatesPaths = new string[0];
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;
    }
}
