import java.io.File;
import java.io.FileFilter;
import java.util.concurrent.BlockingQueue;

/**
 *
 */
public class FileCrawler implements Runnable {
    private final BlockingQueue<File> fileQueue;
    private final FileFilter filter;
    private final File root;

    public FileCrawler(BlockingQueue<File> queue, File root, FileFilter filter){
        this.root = root;
        this.filter = filter;
        this.fileQueue = queue;
    }

    public void run() {
        try {
            crawl(root);
        } catch (InterruptedException ex) {
            Thread.currentThread().interrupt();
        }
    }

    private void crawl(File root) throws InterruptedException {
        File[] entries = root.listFiles(filter);
        if (entries != null) {
            for (File entry : entries) {
                if (entry.isDirectory()) crawl(entry);
                else fileQueue.put(entry);
            }

        }
    }
}
