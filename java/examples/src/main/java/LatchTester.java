import java.util.concurrent.CountDownLatch;

/**
 *
 */
public final class LatchTester {
    public static void main(String[] args) throws InterruptedException {
        final int threads = 6;
        final CountDownLatch startLatch = new CountDownLatch(1);
        final CountDownLatch finishLatch = new CountDownLatch(threads);

        for (int i = 0; i < threads; ++i) {
            new Thread() {
                @Override
                public void run() {
                    try {
                        System.err.println("Starting thread " + Thread.currentThread().getId());
                        startLatch.await();
                        Thread.sleep((long)(10000 * Math.random()));
                        finishLatch.countDown();
                    } catch (InterruptedException ex) {}
                }
            }.start();
        }

        long start = System.nanoTime();
        startLatch.countDown();
        finishLatch.await();
        long finish = System.nanoTime();
        System.out.println("Total Time: " + (finish - start));
    }
}
