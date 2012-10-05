/**
 * @file bitops.h
 * @brief Portable bit operations
 */
#ifndef BIT_UTILS_H
#define BIT_UTILS_H

//---------------------------------------------------------------------------// 
// Bit Constants
//---------------------------------------------------------------------------// 

/**
 * @brief Big endian bitmaks
 */
static unsigned int bit_mask[33] = { 0x00000000,
    0x00000001, 0x00000003, 0x00000007, 0x0000000F,
    0x0000001F, 0x0000003F, 0x0000007F, 0x000000FF,
    0x000001FF, 0x000003FF, 0x000007FF, 0x00000FFF,
    0x00001FFF, 0x00003FFF, 0x00007FFF, 0x0000FFFF,
    0x0001FFFF, 0x0003FFFF, 0x0007FFFF, 0x000FFFFF,
    0x001FFFFF, 0x003FFFFF, 0x007FFFFF, 0x00FFFFFF,
    0x01FFFFFF, 0x03FFFFFF, 0x07FFFFFF, 0x0FFFFFFF,
    0x1FFFFFFF, 0x3FFFFFFF, 0x7FFFFFFF, 0xFFFFFFFF  };

/**
 * @brief Big endian bit positions
 */
static unsigned int bit_posn[32] = {
    0x00000001, 0x00000002, 0x00000004, 0x00000008,
    0x00000010, 0x00000020, 0x00000040, 0x00000080,
    0x00000100, 0x00000200, 0x00000400, 0x00000800,
    0x00001000, 0x00002000, 0x00004000, 0x00008000,
    0x00010000, 0x00020000, 0x00040000, 0x00080000,
    0x00100000, 0x00200000, 0x00400000, 0x00800000,
    0x01000000, 0x02000000, 0x04000000, 0x08000000,
    0x10000000, 0x20000000, 0x40000000, 0x80000000  };

//---------------------------------------------------------------------------// 
// Bit functions
//---------------------------------------------------------------------------// 
/**
 * Test to see if passed bit number is on 
 * @param value Value to check state on
 * @param bit Bit to test in value
 * @return true or false depending on bit state
 */
inline int is_bit_set(unsigned int value, int bit)
{  
   return((value & bit_posn[bit]) ? 1 : 0);
}

/**
 * Set bit of passed in value
 * @param value value to operate on
 * @param bit bit to set
 */
inline void set_bit(unsigned int *value, int bit)
{  
   *value |= bit_posn[bit];
}

/**
 * Clear bit of passed in value
 * @param value value to operate on
 * @param bit bit to clear
 */
inline void clear_bit(unsigned int *value, int bit)
{  
   *value &= bit_mask[32] ^ bit_posn[bit];
}

/**
 * Toggle bit of passed in value
 * @param value value to operate on
 * @param bit bit to toggle
 */
inline void toggle_bit(unsigned int *value, int bit)
{  
   *value ^= bit_posn[bit];
}

/**
 * Set bit of value with passed in val
 * This is just a wrapper for set_bit/clear_bit
 * @param value value to operate on
 * @param bit bit to set
 */
inline void set_bit_val(unsigned int *value, int bit, int val)
{  
	if (val) set_bit(value, bit);
	else	 clear_bit(value, bit);
}

/**
 * Set range of bits of passed in value
 * @param value value to operate on
 * @param start Start bit position
 * @param end End bit position
 * @param val Value to set bits
 */
inline void set_range(unsigned int *value, int start, int end, int val)
{  
	int i = start;
	if (val) for (; i<= end; set_bit(value, i++));
	else	 for (; i<= end; clear_bit(value, i++));
}

/**
 * @brief Swap the value of two bits
 * @param *value Value to switch bits on
 * @param x Bit one
 * @param y Bit two
 * @return void
 */
inline void swap_bits(unsigned int *value, int x, int y)
{  
	if (is_bit_set(*value, y))	*value &= bit_posn[x] | 1;
	else						*value |= bit_posn[x] & 0;
	if (is_bit_set(*value, x))	*value &= bit_posn[y] | 1;
	else						*value |= bit_posn[y] & 0;
}

/**
 * Shift passed value by ls_bit then '&' it with a bit mask of number of bits
 * @param value value to extract value from
 * @param ms Most signifigant bit
 * @param ls Least signifigant bit
 * @return extracted value
 */
inline unsigned int get_unsbits(unsigned int value, int ms, int ls)
{
   return(  ((ls - 1) ? value >> (ls - 1) : value)
		  & bit_mask[ms - ls + 1]);
}

/**
 * Get Raw Value, then Shift high bit to max pos and divide by num shifts
 * int t = (signed)GetUnsBits(value, ms, ls) << (31 - (ms - ls));
 * return(t / pow(2,(31 - (ms - ls))));
 * @param value value to extract value from
 * @param ms Most signifigant bit
 * @param ls Least signifigant bit
 * @return extracted value
 */
inline int get_sinbits(unsigned int value, int ms, int ls)
{
   return(   (signed)get_unsbits(value, ms, ls)
		  << (31 - ms+ls)) / pow(2,(31 - ms+ls));
}

/**
 * @brief Get a range of bits and scale it up to resolution
 * @param value value to extract value from
 * @param ms Most signifigant bit
 * @param ls Least signifigant bit
 * @return extracted value
 */
inline float get_scale(unsigned int value, int ms, int ls, float resolution)
{
   return get_sinbits(value, ms, ls) * resolution;
}

/**
 * @brief Return dec value of BCD digit up to 5 places (max 8xxxx)
 * @param bcd Binary coded decimal
 * @return extracted value
 */
inline int bcd_to_dec(unsigned int bcd)
{
   return(   (bcd        & 0xF)
		  + ((bcd >>  4) & 0xF) * 10
		  + ((bcd >>  8) & 0xF) * 100
		  + ((bcd >> 12) & 0xF) * 1000
		  + ((bcd >> 16) & 0xF) * 10000 );
}

#endif

