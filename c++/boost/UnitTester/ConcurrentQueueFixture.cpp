/**
 * @file ConcurrentQueueFixture.cpp
 * @brief Fixture to test functionality of the ConcurrentQueue class
 */
#include "stdafx.h"
#include <cppunit/extensions/HelperMacros.h>
/* requirements */
#include <boost/bind.hpp>
#include <boost/thread.hpp>
#include "ConcurrentQueue.hpp"

//---------------------------------------------------------------------------//
// Helpers
//---------------------------------------------------------------------------//
template <typename Type>
void worker_thread(ConcurrentQueue<Type>& queue, int size)
{
	for (int i = 0; i < size; ++i) {
		Type output = queue.Consume();
	}
}

//---------------------------------------------------------------------------//
// Test Suite Interface
//---------------------------------------------------------------------------//
/**
 * @brief Test fixture for the request handler
 */
class ConcurrentQueueFixture : public CppUnit::TestFixture
{
	//-----------------------------------------------------------------------//
	// define the test suite
	//-----------------------------------------------------------------------//
	CPPUNIT_TEST_SUITE(ConcurrentQueueFixture);
		CPPUNIT_TEST(testProduceConsume);
		CPPUNIT_TEST(testClearingQueue);
		CPPUNIT_TEST(testThreadedConsume);
		CPPUNIT_TEST(testThreadedBlockedConsume);
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
	void testProduceConsume(void);
	void testClearingQueue(void);
	void testThreadedConsume(void);
	void testThreadedBlockedConsume(void);
};
/* auto register the test suite */
CPPUNIT_TEST_SUITE_REGISTRATION(ConcurrentQueueFixture);

//---------------------------------------------------------------------------//
// Test Suite Interface Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Used to perform any pre-test setup
 * @return void
 */
void ConcurrentQueueFixture::setUp(void)
{
	/* nothing */
}

/**
 * @brief Used to perform any post-test cleanup
 * @return void
 */
void ConcurrentQueueFixture::tearDown(void)
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
void ConcurrentQueueFixture::testProduceConsume(void)
{
	std::string input = "Test Message";
	ConcurrentQueue<std::string> queue;
	for (int i = 0; i < 20; ++i) {
		queue.Produce(input);
		CPPUNIT_ASSERT(queue.Size() == 1);
		std::string output = queue.Consume();
		CPPUNIT_ASSERT(queue.Size() == 0);
		CPPUNIT_ASSERT(input == output);
	}
	CPPUNIT_ASSERT(queue.Size() == 0);
}

/**
 * @brief This tests the queue clear method
 * @return void
 */
void ConcurrentQueueFixture::testClearingQueue(void)
{
	std::string input = "Test Message";
	ConcurrentQueue<std::string> queue;
	for (int i = 0; i < 20; ++i) {
		queue.Produce(input);
	}
	CPPUNIT_ASSERT(queue.Size() == 20);
	queue.Clear();
	CPPUNIT_ASSERT(queue.Size() == 0);
}

/**
 * @brief This tests working with a thread
 * @return void
 *
 * This tests to see if a separate thread consumes concurrently
 * with another producing thread and that the queue is empty
 * when it returns.
 */
void ConcurrentQueueFixture::testThreadedConsume(void)
{
	std::string input = "Test Message";
	ConcurrentQueue<std::string> queue;
	boost::thread worker(boost::bind(&worker_thread<std::string>,
		boost::ref(queue), 20));

	for (int i = 0; i < 20; ++i) {
		queue.Produce(input);
	}
	worker.join();
	CPPUNIT_ASSERT(queue.Size() == 0);
}

/**
 * @brief This tests working with a thread
 * @return void
 *
 * This tests to see if the thread blocks waiting for the
 * last message. It also checks if the queue is empty after
 * the thread returns.
 */
void ConcurrentQueueFixture::testThreadedBlockedConsume(void)
{
	std::string input = "Test Message";
	ConcurrentQueue<std::string> queue;
	boost::thread worker(boost::bind(&worker_thread<std::string>,
		boost::ref(queue), 20));

	for (int i = 0; i < 19; ++i) {
		queue.Produce(input);
	}
	CPPUNIT_ASSERT(worker.joinable() == true);
	queue.Produce(input);
	worker.join();
	CPPUNIT_ASSERT(queue.Size() == 0);
}