package com.bashwork.commons.utility;

import static com.bashwork.commons.utility.Validate.notNull;
import static com.bashwork.commons.utility.Validate.check;
import java.time.Clock;
import java.time.Duration;
import java.util.concurrent.TimeUnit;

/**
 * A simple stopwatch using the java.time API.
 */
public final class Stopwatch {
    private final Clock clock;
    private volatile boolean running = false;
    private volatile long startTimeMs = 0L;
    private volatile long elapsedTimeMs = 0L;

    private Stopwatch(Clock clock) {
        this.clock = notNull(clock, "Clock");
    }

    public static Stopwatch create(Clock clock) {
        return new Stopwatch(clock);
    }

    public static Stopwatch create() {
        return create(Clock.systemUTC());
    }

    public long elapsedMs() {
        return running ? (clock.millis() - startTimeMs + elapsedTimeMs) : elapsedTimeMs;
    }

    public long elapsed(TimeUnit unit) {
        return unit.convert(elapsedMs(), TimeUnit.MILLISECONDS);
    }

    public Duration elapsedDuration() {
        return Duration.ofMillis(elapsedMs());
    }

    public boolean isRunning() {
        return running;
    }

    public Stopwatch stop() {
        check(running == true, "The stopwatch must be running to stop");
        running = false;
        elapsedTimeMs += (clock.millis() - startTimeMs);
        return this;
    }

    public Stopwatch reset() {
        running = false;
        startTimeMs = 0L;
        elapsedTimeMs = 0L;
        return this;
    }

    public Stopwatch start() {
        check(running == false, "The stopwatch must be stopped to start");
        running = true;
        startTimeMs = clock.millis();
        return this;
    }
}
