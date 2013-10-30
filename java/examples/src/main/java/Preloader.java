import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 *
 */
public class Preloader {
    private final FutureTask<String> future = new FutureTask<String>(new Callable<String>() {
        @Override
        public String call() throws Exception {
            Thread.currentThread().interrupt();
            Thread.interrupted();
            Thread.sleep(5000);
            return "finished with the loading";
        }
    });

    public void run() throws ExecutionException, InterruptedException {
        try {
            new Thread(future).start();
            String result = future.get();
            System.err.println(result);
        } catch (Exception ex) {
            ex.printStackTrace();
        }

    }

    public <T> T orDefault(T value) {
        return value;

    }

    public static void main(String[] args) {
        try {
            new Preloader().run();
        } catch (Exception ex) {}
    }
}
