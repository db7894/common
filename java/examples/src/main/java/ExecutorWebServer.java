import java.io.DataOutputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

/**
 *
 */
public class ExecutorWebServer {
    private static final int NUM_THREADS = 100;
    private static final ExecutorService exec = Executors.newFixedThreadPool(NUM_THREADS);

    public static void main(String[] args) throws IOException {
        System.err.println("Starting Socket Server");
        ServerSocket socket = new ServerSocket(8080);
        while (!exec.isShutdown()) {
            final Socket client = socket.accept();
            Runnable handler = new Runnable() {
                @Override
                public void run() {
                    handleRequest(client);
                }
            };
            exec.execute(handler);
        }
    }

    public void stop() {
        exec.shutdown();
    }

    private static void handleRequest(Socket client) {
        try {
            System.err.println("Handling client: " + client.toString());
            DataOutputStream output = new DataOutputStream(client.getOutputStream());
            output.writeBytes("<h1>Hello World</h1>");
            output.flush();
            output.close();
            client.close();
        } catch (IOException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        }
    }
}
