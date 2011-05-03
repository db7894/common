/**
 * @file RegistryFixture.cpp
 * @brief Fixture to test functionality of the GenericActiveObject class
 * @todo Use sleep abstraction when c++0x adds threading
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#if 0
#include "Registry.hpp"

//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//
static const char* test_key		= "Testing";
static const char* test_name	= "Test";

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class RegistryFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(RegistryFixture);
		CPPUNIT_TEST(testReadDefault);
		CPPUNIT_TEST(testWriteRead);
		CPPUNIT_TEST(testWriteClear);
	CPPUNIT_TEST_SUITE_END();

public:
	//-----------------------------------------------------------------------//
	/* define the test interface */
	//-----------------------------------------------------------------------//
	void setUp(void);
	void tearDown(void);

	//-----------------------------------------------------------------------//
	/* define out tests */
	//-----------------------------------------------------------------------//
	void testReadDefault(void);
	void testWriteRead(void);
	void testWriteClear(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(RegistryFixture);


//---------------------------------------------------------------------------//
// Test Suite Inteface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void RegistryFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void RegistryFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief Test if the default result value was used
 * @return void
 */
void RegistryFixture::testReadDefault(void)
{
	std::string result = "";
	std::string input = "Testing";
	std::string output = Registry::Read(test_key, test_name, result);
	CPPUNIT_ASSERT(result == output);
}

/**
 * @brief Test a write and a subsequent read
 * @return void
 */
void RegistryFixture::testWriteRead(void)
{
	std::string input = "Testing";
	Registry::Write(test_key, test_name, input);
	std::string output = Registry::Read(test_key, test_name, string(""));
	CPPUNIT_ASSERT(input == output);
}

/**
 * @brief Test a write and a subsequent clear
 * @return void
 */
void RegistryFixture::testWriteClear(void)
{
	std::string result = "";
	std::string input = "Testing";
	Registry::Write(test_key, test_name, input);
	std::string output = Registry::Read(test_key, test_name, result);
	CPPUNIT_ASSERT(input == output);

	Registry::Delete(test_key, test_name);
	output = Registry::Read(test_key, test_name, result);
	CPPUNIT_ASSERT(output == result);
}
#endif