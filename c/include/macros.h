/**
 * @file macros.h
 * @brief Portable macros
 */
#ifndef MACRO_UTILS_H
#define MACRO_UTILS_H

//---------------------------------------------------------------------------// 
// Common Macros
//---------------------------------------------------------------------------// 
/**
 * @def MIN
 * @brief Macro to calculate the smallest of two values
 */
#undef MIN
#define MIN(a,b)			(((a) < (b)) ? (a) : (b))

/**
 * @def MAX
 * @brief Macro to calculate the largest of two values
 */
#undef MAX
#define MAX(a,b)			(((a) > (b)) ? (a) : (b))

/**
 * @def ABS
 * @brief Macro to calculate the absolute value of a value
 */
#undef ABS
#define ABS(a)				(((a) < (0)) ? -(a) : (a))

/**
 * @def CLAMP
 * @brief Macro to return a value in range
 */
#undef CLAMP
#define CLAMP(x, low, high)  (((x) > (high)) ? (high) : (((x) < (low)) ? (low) : (x)))

/**
 * @def ARRAY_SIZE
 * @brief Macro to calculate the size in bytes of an array
 */
#ifndef ARRAY_SIZE
#	define ARRAY_SIZE(x)	((unsigned)(sizeof(x) / sizeof((x)[0])))
#endif

/**
 * @def OFFSETOF
 * @brief Macro to calculate the offset of an element in a struct
 */
#ifndef OFFSETOF
#  define OFFSETOF(struct_type, member) \
		((long)((uint8_t *) &((struct_type*) 0)->member))
#endif

#endif
