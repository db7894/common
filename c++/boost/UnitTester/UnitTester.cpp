/**
 * @file UnitTester.cpp
 * @brief Main test runner for available fixtures
 */
#include "stdafx.h"
/* cppunit */
#include <cppunit/extensions/TestFactoryRegistry.h>
#include <cppunit/CompilerOutputter.h>
#include <cppunit/TestResult.h>
#include <cppunit/TestResultCollector.h>
#include <cppunit/TestRunner.h>
#include <cppunit/TextTestProgressListener.h>
#include <cppunit/BriefTestProgressListener.h>

/**
 * @brief Main test runner for available fixtures
 * @return True if all the tests were successfull, false otherwise
 */
int main(int argc, char *argv[])
{
	std::string path = (argc > 1) ? std::string(argv[1]) : "";

	/* create event manager and test controller */
	CppUnit::TestResult controller;

	/* add a listener that collects the results */
	CppUnit::TestResultCollector result;
	controller.addListener(&result);

	/* add a listener that prints dots while the test runs */
	CppUnit::TextTestProgressListener progress;
	controller.addListener( &progress );    

	/* add a listener that prints each running test */
	//CppUnit::BriefTestProgressListener progress;
	//controller.addListener( &progress );    

	/* add the suites to the test runner */
	CppUnit::TestRunner runner;
	runner.addTest(CppUnit::TestFactoryRegistry::getRegistry().makeTest());
	try {
		runner.run(controller, path);
		std::cerr << "\n";

		/* add compiler formatted output */
		CppUnit::CompilerOutputter outputter(&result, std::cerr);
		outputter.write();
	}
	/* invalid path */
	catch (std::invalid_argument &e) {
		std::cerr << "\n" << "ERROR: " << e.what() << "\n";
		return 0;
	}

	/* return result for autobuild tools */
	return result.wasSuccessful() ? 0 : 1;
}

