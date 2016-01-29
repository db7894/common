package com.bashwork.commons.utility;

import static com.bashwork.commons.utility.Validate.isNonNegative;
import static com.bashwork.commons.utility.Validate.isPositive;
import static com.bashwork.commons.utility.Validate.notBlank;
import static com.bashwork.commons.utility.Validate.notEmpty;
import static com.bashwork.commons.utility.Validate.notNull;

import org.junit.Test;

public class ValidateTest {

    @Test(expected=IllegalArgumentException.class)
    public void not_null_null() {
        notNull(null);
    }

    @Test
    public void not_null_object() {
        notNull(new Object());
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_empty_null() {
        notEmpty(null);
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_empty_empty() {
        notEmpty("");
    }

    @Test
    public void not_empty_normal() {
        notEmpty("NotEmpty");
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_null() {
        notBlank(null);
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_empty() {
        notBlank("");
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_space() {
        notBlank(" ");
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_spaces() {
        notBlank("   ");
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_tab() {
        char[] tab = {'\t'};
        notBlank(new String(tab));
    }

    @Test(expected=IllegalArgumentException.class)
    public void not_blank_tabs() {
        char[] tabs = {'\t', '\t', '\t'};
        notBlank(new String(tabs));
    }

    @Test
    public void not_blank_normal() {
        notBlank("hi");
        notBlank("  there");
        notBlank("  not  ");
        notBlank("blank  ");
    }

    @Test
    public void is_positive() {
        isPositive(2);
        isPositive(2, "argument");
    }

    @Test(expected=IllegalArgumentException.class)
    public void is_not_positive() {
        isPositive(0);
    }

    @Test
    public void is_non_negative() {
        isNonNegative(0);
        isNonNegative(1, "argument");
    }

    @Test(expected=IllegalArgumentException.class)
    public void is_negative() {
        isNonNegative(-1);
    }
}
