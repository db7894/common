package com.bashwork.commons.producer;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import org.junit.Test;

import java.time.Duration;

public class ProducersTest {

    private static final String input = "input";
    private static final String tracker = "tracker";
    private static final Producer producer = new Producer() {
        @Override
        public <TProduct> String produce(TProduct message) {
            return tracker;
        }

        @Override
        public <TProduct> String produce(TProduct message, Duration delay) {
            return tracker;
        }
    };

    @Test
    public void specific_producer_upgrade() {
        SpecificProducer<String> specific = Producers.<String> toSpecific(producer);
        assertThat(specific.produce(input), equalTo(tracker));
    }
}
