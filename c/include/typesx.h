/**
 * @file typesx.h
 * @brief Portable types
 */
#ifndef TYPE_UTILS_H
#define TYPE_UTILS_H

//---------------------------------------------------------------------------// 
// Test for types
//---------------------------------------------------------------------------// 
// Check for stdint.h
//---------------------------------------------------------------------------// 
#ifndef __int8_t_defined
typedef signed char			int8_t		/**< 1-byte -127 .. 127 */
typedef signed short		int16_t		/**< 2-bytes -32767 .. 32767 */
typedef int					int32_t		/**< 4-bytes -2147483647 to 2147483647 */
typedef signed long long	int64_t		/**< 64-bit signed */
#	define __int8_t_defined
#endif

#ifndef __uint32_t_defined
typedef unsigned char		uint8_t		/**< 1-byte 0 .. 255 */
typedef unsigned short		uint16_t	/**< 2-bytes 0 .. 65535 */
typedef unsigned			uint32_t	/**< 4-bytes 0 to 16777215 */
typedef unsigned long long	uint64_t	/**< 64-bit unsigned */
#	define __uint32_t_defined
#endif

//---------------------------------------------------------------------------// 
// Size Constants
//---------------------------------------------------------------------------// 
// Check for limits.h
//---------------------------------------------------------------------------// 
#if 0
#define MIN_INT8   ((int8_t) 0x80)
#define MAX_INT8   ((int8_t) 0x7f)
#define MAX_UINT8  ((int8_t) 0xff)

#define MIN_INT16  ((int16_t)  0x8000)
#define MAX_INT16  ((int16_t)  0x7fff)
#define MAX_UINT16 ((uint16_t) 0xffff)

#define MIN_INT32  ((int32_t)  0x80000000)
#define MAX_INT32  ((int32_t)  0x7fffffff)
#define MAX_UINT32 ((uint32_t) 0xffffffff)

#define MIN_INT64  ((int64_t)  0x8000000000000000LL)
#define MAX_INT64  ((int64_t)  0x7fffffffffffffffLL)
#define MAX_UINT64 ((uint64_t) 0xffffffffffffffffULL)
#endif

//---------------------------------------------------------------------------// 
// Math Constants
//---------------------------------------------------------------------------// 
// No type can handle this full range, but maybe someday?
//---------------------------------------------------------------------------// 
#define S_E     2.7182818284590452353602874713526624977572470937000 
#define S_LN2   0.69314718055994530941723212145817656807550013436026 
#define S_LN10  2.3025850929940456840179914546843642076011014886288 
#define S_PI    3.1415926535897932384626433832795028841971693993751 
#define S_PI_2  1.5707963267948966192313216916397514420985846996876 
#define S_PI_4  0.78539816339744830961566084581987572104929234984378 
#define S_SQRT2 1.4142135623730950488016887242096980785696718753769 

#endif
