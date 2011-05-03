/**
 * @file RequestHandlerFixture.cpp
 * @brief Fixture to test functionality of the RequestHandler class
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#include <boost/shared_ptr.hpp>
#include "RequestHandler.hpp"

//---------------------------------------------------------------------------//
// Dummy Messages
//---------------------------------------------------------------------------//
struct Request { std::string message; };
struct Reply   { std::string message; };

//---------------------------------------------------------------------------//
// Helper Functions
//---------------------------------------------------------------------------//
static Reply* good_callback(const Request& request)
{
	Reply *result = new Reply();
	result->message = "[Good] " + request.message;
	return result;
};

static Reply* error_callback(const Request& request)
{
	Reply *result = new Reply();
	result->message = "[Error] " + request.message;
	return result;
};

typedef boost::shared_ptr<Reply> reply_t;
static reply_t good_shared_callback(const Request& request)
{
	reply_t result(new Reply());
	result->message = "[Good] " + request.message;
	return result;
};


//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class RequestHandlerFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(RequestHandlerFixture);
		CPPUNIT_TEST(testAddingCallbacks);
		CPPUNIT_TEST(testValidCallbacks);
		CPPUNIT_TEST(testInvalidCallbacks);
		CPPUNIT_TEST_EXCEPTION(testNoInvalidCallbacks, boost::bad_function_call);
		CPPUNIT_TEST(testMixedCallbacks);
		CPPUNIT_TEST(testSharedPointersCallbacks);
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
	void testAddingCallbacks(void);
	void testValidCallbacks(void);
	void testInvalidCallbacks(void);
	void testNoInvalidCallbacks(void);
	void testMixedCallbacks(void);
	void testSharedPointersCallbacks(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(RequestHandlerFixture);

//---------------------------------------------------------------------------//
// Test Suite Interface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void RequestHandlerFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void RequestHandlerFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief This tests adding and removing callback functions
 * @return void
 */
void RequestHandlerFixture::testAddingCallbacks(void)
{
	RequestHandler<Request, Reply*> handler;
	handler += std::make_pair(0, &good_callback);
	CPPUNIT_ASSERT(handler.Size() == 1);

	handler -= 0;
	CPPUNIT_ASSERT(handler.Size() == 0);
}

/**
 * @brief This tests calling valid handlers
 * @return void
 */
void RequestHandlerFixture::testValidCallbacks(void)
{
	RequestHandler<Request, Reply*> handler;
	handler += std::make_pair(0, &good_callback);
	handler += std::make_pair(1, &good_callback);
	handler += std::make_pair(2, &good_callback);
	Request request = {"Testing"};

	for (int i = 0; i < 10; ++i) {
		Reply* result = handler(request, i%3);
		CPPUNIT_ASSERT(result->message == "[Good] Testing");
		delete result;
	}
}

/**
 * @brief This tests calling an invalid handler
 * @return void
 */
void RequestHandlerFixture::testInvalidCallbacks(void)
{
	RequestHandler<Request, Reply*> handler;
	handler += std::make_pair(-1, &error_callback);
	Request request = {"Testing"};

	for (int i = 0; i < 10; ++i) {
		Reply* result = handler(request, i);
		CPPUNIT_ASSERT(result->message == "[Error] Testing");
		delete result;
	}
}

/**
 * @brief This tests calling an invalid handle without an error handler
 * @return void
 */
void RequestHandlerFixture::testNoInvalidCallbacks(void)
{
	RequestHandler<Request, Reply*> handler;
	handler += std::make_pair(0, &good_callback);
	Request request = {"Testing"};
	Reply* result = handler(request, 20);
}

/**
 * @brief This tests calling an both handlers
 * @return void
 */
void RequestHandlerFixture::testMixedCallbacks(void)
{
	RequestHandler<Request, Reply*> handler;
	handler += std::make_pair(-1, &error_callback);
	handler += std::make_pair(0, &good_callback);
	handler += std::make_pair(1, &good_callback);
	Request request = {"Testing"};

	for (int i = 0; i < 10; ++i) {
		Reply* result = handler(request, i%3);
		if (i%3 == 2)
			 CPPUNIT_ASSERT(result->message == "[Error] Testing");
		else CPPUNIT_ASSERT(result->message == "[Good] Testing");
		delete result;
	}
}

/**
 * @brief This tests using smart pointers as result types
 * @return void
 */
void RequestHandlerFixture::testSharedPointersCallbacks(void)
{
	RequestHandler<Request, reply_t> handler;
	handler += std::make_pair(0, &good_shared_callback);
	handler += std::make_pair(1, &good_shared_callback);
	Request request = {"Testing"};

	for (int i = 0; i < 10; ++i) {
		reply_t result(handler(request, i%2));
		CPPUNIT_ASSERT(result->message == "[Good] Testing");
	}
}
