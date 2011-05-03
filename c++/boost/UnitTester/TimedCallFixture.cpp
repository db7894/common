/**
 * @file ObjectFactoryFixture.cpp
 * @brief Fixture to test functionality of Othe bjectFactory class
 * @todo Use sleep abstraction when c++0x adds threading
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#include <boost/bind.hpp>
#include "TimedCall.hpp"

//---------------------------------------------------------------------------//
// Helper Functions
//---------------------------------------------------------------------------//
void counter(int& count)
{
	++count;
}

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class TimedCallFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(TimedCallFixture);
		CPPUNIT_TEST(testRunCount);
		CPPUNIT_TEST(testAlteredRunCount);
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
	void testAlteredRunCount(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(TimedCallFixture);


//---------------------------------------------------------------------------//
// Test Suite Inteface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void TimedCallFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void TimedCallFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief This tests that the timed call runs at least number of times
 * @return void
 * 
 * I assume that with a delay of 10 ms, we can execute at least 10 iterations
 * in 120 ms (accounting for context switches)
 */
void TimedCallFixture::testRunCount(void)
{
	int count = 0;
	TimedCall call(boost::bind(&counter, boost::ref(count)), 10);
	::Sleep(120);
	CPPUNIT_ASSERT(count >= 10);
}

/**
 * @brief This tests that the timed call runs at least number of times
 * @return void
 * 
 * This uses a broad deadline to show that the time change works
 */
void TimedCallFixture::testAlteredRunCount(void)
{
	int count = 0;
	TimedCall call(boost::bind(&counter, boost::ref(count)), 100);
	::Sleep(10); /* should not trigger a count */
	CPPUNIT_ASSERT(count <= 10);
	call.SetDelay(10);
	::Sleep(250); /* in case we miss the next loop */
	CPPUNIT_ASSERT(count >= 10);
}

