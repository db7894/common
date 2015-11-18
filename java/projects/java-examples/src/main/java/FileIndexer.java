import java.io.File;
import java.util.concurrent.BlockingQueue;

/**
 *
 */
public class FileIndexer implements Runnable {
    private final BlockingQueue<File> fileQueue;

    public FileIndexer(BlockingQueue<File> queue) {
        this.fileQueue = queue;
    }

    public void run() {
        try {
            while (true)
                index(fileQueue.take());
        } catch (InterruptedException ex) {
            Thread.currentThread().interrupt();
        }
    }

    private void index(File file) {
        System.err.println(file);
    }
}
