import java.io.File;
import java.io.IOException;
import javax.imageio.ImageIO;
import java.awt.Rectangle;
import java.awt.image.BufferedImage;

public class ImageCropper {
    static {
        System.loadLibrary("ImageCropper");
    }

    /**
     * Given an image, crop it to the specified region of interest.
     * 
     * @param image The image to crop.
     * @return The cropped region of interest in that image.
     */
    public static BufferedImage crop(BufferedImage image) {
        int[] buffer = getRawBuffer(image);
        Rectangle rectangle = getRegion(buffer, image.getWidth(), image.getHeight());
        return isValid(image, rectangle) ? crop(image, rectangle) : null;
    }

    /**
     * An example main runner for smart cropping a region of interes.
     */
    public static void main(String[] args) throws IOException {
        if (args.length != 1) {
            System.out.println("must supply an image to work with");
            System.exit(1);
        }
        try {
            BufferedImage image = ImageIO.read(new File(args[0]));
            int[] buffer = getRawBuffer(image);
            Rectangle rectangle = getRegion(buffer, image.getWidth(), image.getHeight());
            System.out.println("Resulting rectangle: " + rectangle.toString());
            BufferedImage crop = crop(image, rectangle);
        } catch (IllegalArgumentException ex) {
            System.out.println("Failed to find rectangle in image: " + ex.getMessage());
        }
    }
    
    /**
     * Given a resulting rectangle, determine if it is valid
     *  
     * @param rectangle The rectangle to validate
     * @return true if the region is valid, false otherwise
     */
    private static boolean isValid(BufferedImage image, Rectangle rectangle) {
        int width  = rectangle.width  + rectangle.x;
        int height = rectangle.height + rectangle.y;

        boolean isAreaValid = (rectangle.width * rectangle.height) > 0;
        boolean isSizeValid = (width < rectangle.width) && (height < rectangle.height);
        boolean isMarkValid = (rectangle.x >= 0) && (rectangle.y >= 0);

        return isAreaValid && isSizeValid && isMarkValid;
    }

    /**
     * Given an image, crop it to the supplied rectangle.
     * 
     * @param image The image to crop.
     * @param rectangle The rectangle to crop the image to.
     * @return The resulting cropped image.
     */
    private static BufferedImage crop(BufferedImage image, Rectangle rectangle) {
        return image.getSubimage(rectangle.x, rectangle.y, rectangle.width, rectangle.height);
    }
    
    /**
     * Given an image, return the underlying raw image data.
     * 
     * @param image The image to retrieve the raw data for.
     * @return The raw image data for the specified image.
     */
    private static int[] getRawBuffer(BufferedImage image) {
        return image.getRGB(0, 0, image.getWidth(), image.getHeight(), null, 0, image.getWidth());
    }

    /**
     * Retrieve the region of interested for the specified image.
     * 
     * @param buffer The raw image data.
     * @param width The width of the image.
     * @param height The height of the image.
     * @return The expected region of interest.
     */
    private static native Rectangle getRegion(int[] buffer, int width, int height);
}
