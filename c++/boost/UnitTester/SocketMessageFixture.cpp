/**
 * @file MessageFixture.cpp
 * @brief Fixture to test functionality of the Message class
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>

/* requirements */
/* break encapsulation to test message buffer */
#define protected public
#	include "SocketMessage.h"
#undef protected

//---------------------------------------------------------------------------//
// Helper Types
//---------------------------------------------------------------------------//
/* dummy data */
struct message_data {
	int number;
	char name[4];
};

/* header prefixed and encoded dummy data */
static char testpacket[] = {/* big endian */
	/* header */
	0x00,0x00,0x00,0x00,	/* type */
	0x08,0x00,0x00,0x00,	/* size */
	0x00,0x00,0x00,0x00,	/* encrypted */
	/* mesasge */
	0x01,0x00,0x00,0x00,	/* number */
	0x61,0x6f,0x62,0x00,	/* name */
};

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class SocketMessageFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(SocketMessageFixture);
		CPPUNIT_TEST(testConstructors);
		CPPUNIT_TEST(testAppenders);
		CPPUNIT_TEST(testReset);
		CPPUNIT_TEST(testPlusOperatorBuffer);
		CPPUNIT_TEST(testAppendBuffer);
		CPPUNIT_TEST(testCopyOperator);
		CPPUNIT_TEST(testAssignmentOperator);
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
	void testConstructors(void);
	void testAppenders(void);
	void testReset(void);
	void testPlusOperatorBuffer(void);
	void testAppendBuffer(void);
	void testCopyOperator(void);
	void testAssignmentOperator(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(SocketMessageFixture);


//---------------------------------------------------------------------------//
// Test Suite Inteface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void SocketMessageFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void SocketMessageFixture::tearDown(void)
{
	/* nothing */
}

//---------------------------------------------------------------------------//
// Test Suite Tests Implementation
//---------------------------------------------------------------------------//

/**
 * @brief This tests the various message constructors
 * @return void
 */
void SocketMessageFixture::testConstructors(void)
{
	message_data data = {1, "aob"};
	{ /* forced scope */
		SocketMessage message;
		CPPUNIT_ASSERT(message.Size() == 0);
	}
	{ /* forced scope */
		SocketMessage message(data);
		CPPUNIT_ASSERT(message.Size() == sizeof(message_data));
	}
	{ /* forced scope */
		SocketMessage message(data, sizeof(data));
		CPPUNIT_ASSERT(message.Size() == sizeof(message_data));
	}
}

/**
 * @brief This tests adding data to a message
 * @return void
 */
void SocketMessageFixture::testAppenders(void)
{
	message_data data = {1, "aob"};
	SocketMessage message;

	message += data;
	CPPUNIT_ASSERT(message.Size() == sizeof(data) * 1);
	message += data;
	CPPUNIT_ASSERT(message.Size() == sizeof(data) * 2);
	message.Append(data, sizeof(data));
	CPPUNIT_ASSERT(message.Size() == sizeof(data) * 3);
}

/**
 * @brief This tests adding data to a message
 * @return void
 */
void SocketMessageFixture::testReset(void)
{
	message_data data = {1, "aob"};
	SocketMessage message;

	message += data;
	message += data;
	message.Reset();
	CPPUNIT_ASSERT(message.Size() == 0);
}

/**
 * @brief This tests the resulting buffer from a +=
 * @return void
 */
void SocketMessageFixture::testPlusOperatorBuffer(void)
{
	message_data data = {1, "aob"};
	SocketMessage message;
	message += data;

	int size = sizeof(testpacket);
	for (int i = 0; i < size; ++i) {
		CPPUNIT_ASSERT(message.get_raw_header()[i] == testpacket[i]);
	}
}

/**
 * @brief This tests the resulting buffer from an append
 * @return void
 */
void SocketMessageFixture::testAppendBuffer(void)
{
	message_data data = {1, "aob"};
	SocketMessage message;
	message.Append(data, sizeof(data));

	int size = sizeof(testpacket);
	for (int i = 0; i < size; ++i) {
		CPPUNIT_ASSERT(message.get_raw_header()[i] == testpacket[i]);
	}
}

/**
 * @brief This tests the resulting buffer from an append
 * @return void
 */
void SocketMessageFixture::testCopyOperator(void)
{
	message_data data = {1, "aob"};
	SocketMessage expected;
	expected.Append(data, sizeof(data));
	SocketMessage message(expected);

	int size = sizeof(testpacket);
	for (int i = 0; i < size; ++i) {
		CPPUNIT_ASSERT(message.get_raw_header()[i] == testpacket[i]);
	}
}

/**
 * @brief This tests the resulting buffer from an append
 * @return void
 */
void SocketMessageFixture::testAssignmentOperator(void)
{
	message_data data = {1, "aob"};
	SocketMessage expected;
	expected.Append(data, sizeof(data));
	SocketMessage message = expected;

	int size = sizeof(testpacket);
	for (int i = 0; i < size; ++i) {
		CPPUNIT_ASSERT(message.get_raw_header()[i] == testpacket[i]);
	}
}
