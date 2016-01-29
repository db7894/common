package com.bashwork.commons.testing.hamcrest;

import org.hamcrest.Description;
import org.hamcrest.TypeSafeMatcher;

/**
 * A hamcrest matcher that will match regex strings.
 */
public final class RegexMatcher extends TypeSafeMatcher<String> {

    private final String regex;

    /**
     * Creates a new instance of the RegexMatcher
     *
     * @param regex The regex to match against.
     */
    public RegexMatcher(final String regex) {
        this.regex = regex;
    }

    @Override
    public void describeTo(final Description description) {
        description.appendText("matches regex=`" + regex + "`");
    }

    @Override
    public boolean matchesSafely(final String string) {
        return string.matches(regex);
    }
}
