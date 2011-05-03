/**
 * @file GenericActiveObjectFixture.cpp
 * @brief Fixture to test functionality of the GenericActiveObject class
 * @todo Use sleep abstraction when c++0x adds threading
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#include <boost/bind.hpp>
#include "ActiveObject.hpp"

//---------------------------------------------------------------------------//
// Helper Functions
//---------------------------------------------------------------------------//
void count_one(int& count)
{
	++count;
}

void count_many(int& count, int number)
{
	count += number;
}

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class GenericActiveObjectFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(GenericActiveObjectFixture);
		CPPUNIT_TEST(testRunCount);
		CPPUNIT_TEST(testMultipleFunctions);
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
	void testRunCount(void);
	void testMultipleFunctions(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(GenericActiveObjectFixture);


//---------------------------------------------------------------------------//
// Test Suite Inteface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void GenericActiveObjectFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void GenericActiveObjectFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief This tests that the timed call runs at least number of times
 * @return void
 */
void GenericActiveObjectFixture::testRunCount(void)
{
	int count = 0;
	GenericActiveObject object;
	for (int i = 0; i < 10; ++i) {
		object.schedule(boost::bind(&count_one, boost::ref(count)));
	}
	::Sleep(100); /* time to catch up */
	CPPUNIT_ASSERT(count == 10);
}

/**
 * @brief This tests that the timed call runs at least number of times
 * @return void
 */
void GenericActiveObjectFixture::testMultipleFunctions(void)
{
	int count = 0;
	GenericActiveObject object;
	object.schedule(boost::bind(&count_one, boost::ref(count)));
	object.schedule(boost::bind(&count_many, boost::ref(count), 10));
	object.schedule(boost::bind(&count_one, boost::ref(count)));
	::Sleep(100); /* time to catch up */
	CPPUNIT_ASSERT(count == 12);
}