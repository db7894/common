============================================================
Documentation Style Guide
============================================================
:Date: |today|

.. highlightlang:: rest

The Bashwork documentation should follow the `Apple Publications Style Guide`_
wherever possible. This particular style guide was selected mostly because it
seems reasonable and is easy to get online.

Topics which are not covered in the Apple's style guide will be discussed in
this document.

All reST files use an indentation of 3 spaces.  The maximum line length is 80
characters for normal text, but tables, deeply indented code samples and long
links may extend beyond that.

Make generous use of blank lines where applicable; they help grouping things
together.

A sentence-ending period may be followed by one or two spaces; while reST
ignores the second space, it is customarily put in by some users, for example
to aid Emacs' auto-fill mode.

Footnotes are generally discouraged, though they may be used when they are the
best way to present specific information. When a footnote reference is added at
the end of the sentence, it should follow the sentence-ending punctuation. The
reST markup should appear something like this::

    This sentence has a footnote reference. [#]_ This is the next sentence.

Footnotes should be gathered at the end of a file, or if the file is very long,
at the end of a section. The docutils will automatically create backlinks to
the footnote reference.

Footnotes may appear in the middle of sentences where appropriate.

.. _Apple Publications Style Guide: http://developer.apple.com/documentation/UserExperience/Conceptual/APStyleGuide/APSG_2008.pdf

