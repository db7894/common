#include <jni.h>
#include "ImageCropper.h"
#include "NativeImageCropper.h"

/**
 * @summary Creates a new instance of the requested java type
 * @param env The current java environment
 * @param klass The java class to create an instance of
 * @return An instance of the requested java type
 */
jobject new_jni_object(JNIEnv* env, jclass klass) {
    jmethodID init_id = env->GetMethodID(klass, "<init>", "()V");
    if (init_id == 0) {
        env->ExceptionClear();
        return NULL;
    }
    
    jobject object = env->NewObject(klass, init_id);
    if(object == NULL) {
        env->ExceptionClear();
        return NULL;
    }
    
    return object;
}

/**
 * @summary Sets a specified int field of a java object
 * @param env The current java environment
 * @param klass The java class to set the field on
 * @param object The java class instance to set the field on
 * @param field_name The name of the field to set
 * @param value The value to set the field to
 */
bool set_jni_field_int(JNIEnv* env, jclass klass, jobject object,
    const char* field_name, int value) {

    jfieldID field = env->GetFieldID(klass, field_name, "I");
    if (field) {
        env->SetIntField(object, field, value);
        return true;
    }
    env->ExceptionClear();
    return false;
}

/**
 * @summary Throws an exception with the given message
 * @param env The current java environment
 * @param message The message to include in the exception
 * @returns The result of the throw operation
 */
jint throw_exception(JNIEnv *env, const std::string& message)
{
    jclass klass = env->FindClass("java/lang/IllegalArgumentException");

    return env->ThrowNew(klass, message.c_str());
}

/**
 * @summary Converts an opencv Rect to a java Rectangle
 * @param env The current java environment
 * @param rectangle The opencv rectangle to convert
 * @return The converted java Rectangle or NULL if any errors
 */
jobject new_jni_rectangle(JNIEnv* env, const cv::Rect& rectangle) {
    jclass klass = env->FindClass("java/awt/Rectangle");
    if (!klass) return NULL;

    jobject object = new_jni_object(env, klass);
    if (!object) return NULL;

    if (   set_jni_field_int(env, klass, object, "x", rectangle.x)
        && set_jni_field_int(env, klass, object, "y", rectangle.y)
        && set_jni_field_int(env, klass, object, "width", rectangle.width)
        && set_jni_field_int(env, klass, object, "height", rectangle.height)) {
        return object;
    }

    env->DeleteLocalRef(object);
    return NULL;
}

/**
 * @summary JNI binding to the native image cropper
 * @param env The current java environment
 * @param object The current object pointer
 * @param buffer The image buffer to crop
 * @param width The width of the supplied image
 * @param height The height of the supplied image
 * @return A discovered rectangle if one exists
 */
JNIEXPORT jobject JNICALL Java_ImageCropper_getRegion(
    JNIEnv *env, jclass object, jintArray buffer, jint width, jint height) {

    jsize length = env->GetArrayLength(buffer);
    jint* raw_data = env->GetIntArrayElements(buffer, 0);

    if ((width  <= 0) || (height <= 0) || (length <= 0)) {
        throw_exception(env, "The supplied image is invalid");
        return NULL;
    }

    cv::Mat image(height, width, CV_8UC4, raw_data);
    cv::Mat image_mask = bashwork::vision::get_region_mask(image);
    cv::Rect rectangle = bashwork::vision::get_largest_rectangle(image_mask);
    return new_jni_rectangle(env, rectangle);
}
