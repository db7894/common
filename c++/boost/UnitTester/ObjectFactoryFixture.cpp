/**
 * @file ObjectFactoryFixture.cpp
 * @brief Fixture to test functionality of the ObjectFactory class
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#include <boost/shared_ptr.hpp>
#include "ObjectFactory.hpp"

//---------------------------------------------------------------------------//
// Helper Types
//---------------------------------------------------------------------------//
struct message {
	std::string name;
};
typedef boost::shared_ptr<message> message_t;

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class ObjectFactoryFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(ObjectFactoryFixture);
		CPPUNIT_TEST(testGetAndStore);
		CPPUNIT_TEST(testCachedAddress);
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
	void testGetAndStore(void);
	void testCachedAddress(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(ObjectFactoryFixture);


//---------------------------------------------------------------------------//
// Test Suite Inteface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void ObjectFactoryFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void ObjectFactoryFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief This tests automatic returning to the pool
 * @return void
 */
void ObjectFactoryFixture::testGetAndStore(void)
{
	ObjectFactory<message> factory(1);
	message_t input;
	{ /* forced scope */
		message_t output(factory.create());
		input = output;
	}
	//CPPUNIT_ASSERT(input.get() == NULL);
	CPPUNIT_ASSERT(input.use_count() == 1);
}

/**
 * @brief This tests that the object pool caches entries
 * @return void
 *
 * Since the item returned to the pool will be put back into
 * use at the head of the pool, the next create should return
 * the very same object.
 */
void ObjectFactoryFixture::testCachedAddress(void)
{
	void *first, *second;
	ObjectFactory<message> factory(2);
	{ /* forced scope */
		message_t output(factory.create());
		first = output.get();
	}
	{ /* forced scope */
		message_t output(factory.create());
		second = output.get();
	}
	CPPUNIT_ASSERT(first == second);
}
