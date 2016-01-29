package com.bashwork.commons.testing.hamcrest;

/**
 * A collection of hamcrest matchers that are not included
 * in the base library.
 */
public final class MoreMatchers {
    private MoreMatchers() { }

    /**
     * Check if the resulting string matches the given regex.
     *
     * @param regex The regext to match.
     * @return A new matcher.
     */
    public static org.hamcrest.Matcher<String> matchesRegex(final String regex) {
        return new RegexMatcher(regex);
    }
}
