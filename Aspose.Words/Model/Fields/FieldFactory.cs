// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2010 by Dmitry Vorobyev

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Creates instances of field classes.
    /// </summary>
    internal static class FieldFactory
    {
        /// <summary>
        /// Responsible for creating concrete instances of fields classes for the specified field type enumeration.
        /// </summary>
        internal static Field CreateField(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            Field result = CreateField(start.FieldType);
            result.Initialize(start, separator, end);
            return result;
        }

        /// <summary>
        /// Responsible for creating concrete instances of fields classes for the specified field type enumeration.
        /// </summary>
        internal static Field CreateField(FieldBundle fieldBundle)
        {
            return CreateField(fieldBundle.Start, fieldBundle.Separator, fieldBundle.End);
        }

        private static Field CreateField(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldNone:
                    return new FieldUnknown();
                case FieldType.FieldCannotParse:
                    throw new NotSupportedException("Should not reach this line.");
                case FieldType.FieldAddin:
                    return new FieldAddIn();
                case FieldType.FieldAddressBlock:
                    return new FieldAddressBlock();
                case FieldType.FieldAdvance:
                    return new FieldAdvance();
                case FieldType.FieldAsk:
                    return new FieldAsk();
                case FieldType.FieldAuthor:
                    return new FieldAuthor();
                case FieldType.FieldAutoNum:
                    return new FieldAutoNum();
                case FieldType.FieldAutoNumLegal:
                    return new FieldAutoNumLgl();
                case FieldType.FieldAutoNumOutline:
                    return new FieldAutoNumOut();
                case FieldType.FieldAutoText:
                    return new FieldAutoText();
                case FieldType.FieldAutoTextList:
                    return new FieldAutoTextList();
                case FieldType.FieldBarcode:
                    return new FieldBarcode();
                case FieldType.FieldBibliography:
                    return new FieldBibliography();
                case FieldType.FieldBidiOutline:
                    return new FieldBidiOutline();
                case FieldType.FieldCitation:
                    return new FieldCitation();
                case FieldType.FieldComments:
                    return new FieldComments();
                case FieldType.FieldCompare:
                    return new FieldCompare();
                case FieldType.FieldCreateDate:
                    return new FieldCreateDate();
                case FieldType.FieldData:
                    return new FieldData();
                case FieldType.FieldDatabase:
                    return new FieldDatabase();
                case FieldType.FieldDate:
                    return new FieldDate();
                case FieldType.FieldDDE:
                    return new FieldDde();
                case FieldType.FieldDisplayBarcode:
                    return new FieldDisplayBarcode();
                case FieldType.FieldMergeBarcode:
                    return new FieldMergeBarcode();
                case FieldType.FieldDDEAuto:
                    return new FieldDdeAuto();
                case FieldType.FieldDocProperty:
                    return new FieldDocProperty();
                case FieldType.FieldDocVariable:
                    return new FieldDocVariable();
                case FieldType.FieldEditTime:
                    return new FieldEditTime();
                case FieldType.FieldEmbed:
                    return new FieldEmbed();
                case FieldType.FieldEquation:
                    return new FieldEQ();
                case FieldType.FieldFileName:
                    return new FieldFileName();
                case FieldType.FieldFileSize:
                    return new FieldFileSize();
                case FieldType.FieldFillIn:
                    return new FieldFillIn();
                case FieldType.FieldFootnoteRef:
                    return new FieldFootnoteRef();
                case FieldType.FieldFormCheckBox:
                    return new FieldFormCheckBox();
                case FieldType.FieldFormDropDown:
                    return new FieldFormDropDown();
                case FieldType.FieldFormTextInput:
                    return new FieldFormText();
                case FieldType.FieldFormula:
                    return new FieldFormula();
                case FieldType.FieldGreetingLine:
                    return new FieldGreetingLine();
                case FieldType.FieldGlossary:
                    return new FieldGlossary();
                case FieldType.FieldGoToButton:
                    return new FieldGoToButton();
                case FieldType.FieldHtmlActiveX:
                    return new FieldHtmlActiveX();
                case FieldType.FieldHyperlink:
                    return new FieldHyperlink();
                case FieldType.FieldIf:
                    return new FieldIf();
                case FieldType.FieldInclude:
                    return new FieldInclude();
                case FieldType.FieldIncludePicture:
                    return new FieldIncludePicture();
                case FieldType.FieldIncludeText:
                    return new FieldIncludeText();
                case FieldType.FieldIndex:
                    return new FieldIndex();
                case FieldType.FieldIndexEntry:
                    return new FieldXE();
                case FieldType.FieldInfo:
                    return new FieldInfo();
                case FieldType.FieldImport:
                    return new FieldImport();
                case FieldType.FieldKeyword:
                    return new FieldKeywords();
                case FieldType.FieldLastSavedBy:
                    return new FieldLastSavedBy();
                case FieldType.FieldLink:
                    return new FieldLink();
                case FieldType.FieldListNum:
                    return new FieldListNum();
                case FieldType.FieldMacroButton:
                    return new FieldMacroButton();
                case FieldType.FieldMergeField:
                    return new FieldMergeField();
                case FieldType.FieldMergeRec:
                    return new FieldMergeRec();
                case FieldType.FieldMergeSeq:
                    return new FieldMergeSeq();
                case FieldType.FieldNext:
                    return new FieldNext();
                case FieldType.FieldNextIf:
                    return new FieldNextIf();
                case FieldType.FieldNoteRef:
                    return new FieldNoteRef();
                case FieldType.FieldNumChars:
                    return new FieldNumChars();
                case FieldType.FieldNumPages:
                    return new FieldNumPages();
                case FieldType.FieldNumWords:
                    return new FieldNumWords();
                case FieldType.FieldOcx:
                    return new FieldOcx();
                case FieldType.FieldPage:
                    return new FieldPage();
                case FieldType.FieldPageRef:
                    return new FieldPageRef();
                case FieldType.FieldPrint:
                    return new FieldPrint();
                case FieldType.FieldPrintDate:
                    return new FieldPrintDate();
                case FieldType.FieldPrivate:
                    return new FieldPrivate();
                case FieldType.FieldQuote:
                    return new FieldQuote();
                case FieldType.FieldRef:
                    return new FieldRef();
                case FieldType.FieldRefNoKeyword:
                    return new FieldUnknown();
                case FieldType.FieldRefDoc:
                    return new FieldRD();
                case FieldType.FieldRevisionNum:
                    return new FieldRevNum();
                case FieldType.FieldSaveDate:
                    return new FieldSaveDate();
                case FieldType.FieldSection:
                    return new FieldSection();
                case FieldType.FieldSectionPages:
                    return new FieldSectionPages();
                case FieldType.FieldSequence:
                    return new FieldSeq();
                case FieldType.FieldSet:
                    return new FieldSet();
                case FieldType.FieldShape:
                    return new FieldShape();
                case FieldType.FieldSkipIf:
                    return new FieldSkipIf();
                case FieldType.FieldStyleRef:
                    return new FieldStyleRef();
                case FieldType.FieldSubject:
                    return new FieldSubject();
                case FieldType.FieldSymbol:
                    return new FieldSymbol();
                case FieldType.FieldTemplate:
                    return new FieldTemplate();
                case FieldType.FieldTime:
                    return new FieldTime();
                case FieldType.FieldTitle:
                    return new FieldTitle();
                case FieldType.FieldTOA:
                    return new FieldToa();
                case FieldType.FieldTOAEntry:
                    return new FieldTA();
                case FieldType.FieldTOC:
                    return new FieldToc();
                case FieldType.FieldTOCEntry:
                    return new FieldTC();
                case FieldType.FieldUserAddress:
                    return new FieldUserAddress();
                case FieldType.FieldUserInitials:
                    return new FieldUserInitials();
                case FieldType.FieldUserName:
                    return new FieldUserName();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
