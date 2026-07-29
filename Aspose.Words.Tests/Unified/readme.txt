This directory contains Unified Tests.

Unified Tests cover importers and exporters of Microsoft Word formats and most of the model.

Every test class here can have two types of unit tests:
    a. Classic unit tests for the model (that usually do not require loading a file).
    b. Import and Limited Export Tests. These are "template" unit tests that accept a LoadFormat parameter.


Import and Limited Export Tests work this way:
    1. Read from one of the Microsoft Word formats (DOC, RTF, WordML or DOCX)
    2. Save and read again.
    3. Verify model programmatically.

