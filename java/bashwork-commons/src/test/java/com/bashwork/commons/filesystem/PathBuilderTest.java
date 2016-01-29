package com.bashwork.commons.filesystem;

import static org.junit.Assert.assertEquals;

import java.util.StringJoiner;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.junit.Test;

public class PathBuilderTest {

    private static final String WAREHOUSE = "TST6";
    private static final String CAMERA = "PathBuilderTestCam1";

    private static final int YEAR = 2015;
    private static final String YEAR_STRING = String.format("%04d", YEAR);
    private static final int MONTH = 1;
    private static final String MONTH_STRING = String.format("%02d", MONTH);
    private static final int DAY = 7;
    private static final String DAY_STRING = String.format("%02d", DAY);
    private static final int HOUR = 19;
    private static final int MINUTE = 11;
    private static final int SECOND = 33;
    private static final int MILLIS = 2;
    private static final DateTime DATE_TIME = new DateTime(
        YEAR, MONTH, DAY, HOUR, MINUTE, SECOND, MILLIS, DateTimeZone.UTC);
    private static final long TIMESTAMP = DATE_TIME.getMillis();
    private static final long TIMESTAMP_2 = DATE_TIME.plusDays(1).getMillis();
    private static final CharSequence IMAGE_EXTENSION = "." + PathBuilder.Type.IMAGE.getFileType().getExtension();
    private static final CharSequence VIDEO_EXTENSION = "." + PathBuilder.Type.VIDEO.getFileType().getExtension();

    @Test
    public void image() {
        String expected = new StringJoiner("/")
        .add(PathBuilder.Type.IMAGE.getPathKey())
        .add(WAREHOUSE)
        .add(CAMERA)
        .add(YEAR_STRING)
        .add(MONTH_STRING)
        .add(DAY_STRING)
        .add(make19Chars(TIMESTAMP) + IMAGE_EXTENSION)
        .toString();

        String result = PathBuilder.forImage(WAREHOUSE, CAMERA, TIMESTAMP);

        assertEquals(expected, result);
    }

    @Test
    public void video() {
        String expected = new StringJoiner("/")
        .add(PathBuilder.Type.VIDEO.getPathKey())
        .add(WAREHOUSE)
        .add(CAMERA)
        .add(YEAR_STRING)
        .add(MONTH_STRING)
        .add(DAY_STRING)
        .add(make19Chars(TIMESTAMP) + "-" + make19Chars(TIMESTAMP_2) + VIDEO_EXTENSION)
        .toString();

        String result = PathBuilder.forVideo(WAREHOUSE, CAMERA, TIMESTAMP, TIMESTAMP_2);

        assertEquals(expected, result);
    }

    private String make19Chars(long num) {
        String result = String.valueOf(num);
        while (result.length() < 19) {
            result = "0" + result;
        }
        return result;
    }
}
