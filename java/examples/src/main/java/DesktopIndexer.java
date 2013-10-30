import java.io.File;
import java.io.FileFilter;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

/**
 *
 */
public final class DesktopIndexer {
    public static void main(String[] args) {
        BlockingQueue<File> queue = new LinkedBlockingQueue<File>(100);
        FileFilter filter = new FileFilter() {
            @Override
            public boolean accept(File pathname) {
                return true;
            }
        };
        File root = new File("/home/devel");

        new Thread(new FileCrawler(queue, root, filter)).start();
        new Thread(new FileIndexer(queue)).start();
        try {
        while (true) Thread.sleep(10000);
        } catch (InterruptedException ex) {}

    }
}
