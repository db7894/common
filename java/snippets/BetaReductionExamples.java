import java.util.function.Consumer;
import java.util.function.Function;

/**
 * A collection of examples of how to tie
 * two interfaces together in various styles
 * of programming
 */
class BetaReductionExamples {

    interface Context {
        Context DEFAULT = new Context() {
            void set(String name, String value) {}
        };

        void set(String name, String value);
    }

    interface Metadata {
        Metadata DEFAULT = new Metadata() {
            String get(String name) { return name; }
        };
        String get(String name);
    }

    /**
     * This is the standard OO way: define your interfaces
     * and simply tightly couple the implementations together.
     */
    public class set_version_1 {
        public void run(final Metadata metadata) {
            final Context context = Context.DEFAULT;
            context.set("name1", metadata.get("name1"));
            context.set("name2", metadata.get("name2"));
        }

        public void main(final Metadata metadata) {
            run(metadata);
        }
    }

    /**
     * This is a functional way that keeps one interface
     * de-coupled from any future suppliers.
     */
    public class set_version_2 {
        public void run(final Consumer<Context> consumer) {
            final Context context = Context.DEFAULT;
            consumer.accept(context);
        }

        public void main(final Metadata metadata) {
            run(context -> {
                context.set("name1", metadata.get("name1"));
                context.set("name2", metadata.get("name2"));
            });
        }
    }

    /**
     * This is a functional way that keeps both interfaces
     * de-coupled from each other.
     */
    public class set_version_3 {
        public void run(final Function<String, String> producer) {
            final Context context = Context.DEFAULT;
            context.set("name1", producer.apply("name1"));
            context.set("name2", producer.apply("name2"));
        }

        public void main(final Metadata metadata) {
            run(name -> metadata.get(name));
        }
    }

    /**
     * This is the standard imperitive way of keeping
     * interfaces decoupled by simply passing data.
     */
    public class set_version_4 {
        public void run(List<String> names) {
            final Context context = Context.DEFAULT;
            context.set("name1", names.get(0));
            context.set("name2", names.get(1));
        }

        public void main(final Metadata metadata) {
            List<String> names = Arrays.asList(
                metadata.get("name1"),
                metadata.get("name2"),
            run(names);
        }
    }
}
