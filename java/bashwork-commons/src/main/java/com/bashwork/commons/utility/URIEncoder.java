package com.bashwork.commons.utility;

import java.net.URI;
import java.util.BitSet;
import java.util.stream.IntStream;

/**
 * A simple URIEncoder because java and all the libraries get it
 * a little wrong for our use case:
 * <ul>
 * <li>URI.create will throw on any invalid characters</li>
 * <li>URLEncoder encodes ://, which it doesn't need to</li>
 * <li>The apache commons utils are deprecated</li>
 * <li>Other suggestions involve chaining N libraries...each a little wrong</li>
 * </ul>
 */
public final class URIEncoder {
    private URIEncoder() { }

    private static final int BASE_10 = 10;
    private static final int BASE_16 = 16;
    private static final BitSet SKIP_ENCODING;
    static {
        SKIP_ENCODING = new BitSet();
        IntStream.rangeClosed('a', 'z').forEach(SKIP_ENCODING::set);
        IntStream.rangeClosed('A', 'Z').forEach(SKIP_ENCODING::set);
        IntStream.rangeClosed('0', '9').forEach(SKIP_ENCODING::set);
        SKIP_ENCODING.set('/'); // http:(//)www.foo.com/bar?baz=123
        SKIP_ENCODING.set(':'); // http(:)//www.foo.com/bar?baz=123
        SKIP_ENCODING.set('-'); // valid character
        SKIP_ENCODING.set('_'); // valid character
        SKIP_ENCODING.set('.'); // http://www(.)foo(.)com/bar?baz=123
        SKIP_ENCODING.set('*'); // valid character
        SKIP_ENCODING.set('?'); // http://www.foo.com/bar(?)baz=123
    }

    /**
     * Given a string URI, convert it to a java.net.URI instance.
     *
     * @param uri The string URI to convert.
     * @return The converted URI instance.
     */
    public static URI encode(String uri) {
        StringBuilder builder = new StringBuilder(uri.length());
        for (char c : uri.toCharArray()) {
            if (SKIP_ENCODING.get(c)) {
                builder.append(c);
            } else {
                builder.append('%');
                builder.append(toHex(c / BASE_16));
                builder.append(toHex(c % BASE_16));
            }
        }

        return URI.create(builder.toString());
    }

    /**
     * Helper method to convert the supplied character to hex.
     * @param c The character to convert to its hex value.
     * @return The hex value of the supplied character.
     */
    private static char toHex(int c) {
        return (char) ((c < BASE_10) ? ('0' + c) : ('A' + c - BASE_10));
    }
}
