package org.action;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;

public class FileCopier {

    public static void main(String args[]) throws Exception {

        File inbox = new File("data/inbox");
        File outbox = new File("data/outbox");

        if (!outbox.mkdir()) {
			throw new IllegalArgumentException("Invalid output directory");
		}

        File[] files = inbox.listFiles();
        for (File source : files) {
            if (source.isFile()) {
                String filename  = outbox.getPath() + File.separator + source.getName();
                File destination = new File(filename);
                copyFile(source, destination);
            }
        }
    }

    public static void copyFile(File source, File destination)
        throws IOException {

        OutputStream out = new FileOutputStream(destination);
        FileInputStream in = new FileInputStream(source);
        byte[] buffer = new byte[(int)source.length()];
        in.read(buffer);

        try {
            out.write(buffer);
        } finally {
            out.close();
            in.close();
        }
    }
}
