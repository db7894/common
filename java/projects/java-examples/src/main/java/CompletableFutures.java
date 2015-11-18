import java.util.concurrent.Executor;
import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

/**
 * http://www.nurkiewicz.com/2013/05/java-8-definitive-guide-to.html
 * http://download.java.net/lambda/b88/docs/api/java/util/concurrent/CompletableFuture.html
 */
public final class CompletableFutures {
    private final Executor executor = ForkJoinPool.commonPool();

    /**
     *
     */
    public void wait_on_complete_result() throws InterruptedException {
        final CompletableFuture<String> future = new CompletableFuture<>();
        executor.execute((ThrowingRunnable)() -> {
            System.out.println("[1] waiting for future");
            System.out.println(future.get());
            System.out.println("[5] finished getting future");
        });
        System.out.println("[2] performing long operation");
        Thread.sleep(1000);
        future.complete("[4] result of future");
        System.out.println("[3] finished long operation");
    }

    /**
     *
     */
    public static void main(String args[]) throws Exception {
        final CompletableFutures futures = new CompletableFutures();
        futures.wait_on_complete_result();
    }

    /**
     *
     */
    @FunctionalInterface
    public interface ThrowingRunnable extends Runnable {
    
        @Override
        default void run() {
            try {
                runThrows();
            } catch (final Exception ex) {
                throw new RuntimeException(ex);
            }
        }
    
        void runThrows() throws Exception;
    }

    /**
     *
     */
    public static void ignoreExceptions(ThrowingRunnable runnable) {
        try {
            runnable.run();
        } catch (Exception ex) {}
    }
}
