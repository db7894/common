package org.bashwork.options;

import java.util.Map;
import java.util.HashMap;
import com.beust.jcommander.JCommander;
import com.beust.jcommander.Parameter;
import com.beust.jcommander.DynamicParameter;

/**
 * An example of creating command line options using jcommander
 */
public final class Main {
    
    public static class Options {
        @Parameter(names = "--help", help = true)
        private boolean help;

        @Parameter(names = { "--host", "-h" }, description = "service host", required = true)
        public String host = "localhost";

        @Parameter(names = { "--port", "-p" }, description = "service port", required = true)
        public Integer port = 12345;

        @DynamicParameter(names = "--X", description = "extra parameters")
        private Map<String, String> extras = new HashMap<>();
    }

    public static void main(String[] args) {
        final Options options = new Options();
        final JCommander commander = new JCommander(options);

        commander.setProgramName("Main");
        commander.parse(args);

        if (options.help) {
            commander.usage();
            System.exit(0);
        }

        System.out.format("host option: %s\n", options.host);
        System.out.format("port option: %d\n", options.port);
        options.extras.forEach((key, value) ->
            System.out.format("extra option %s: %s\n", key, value));
    }
}
