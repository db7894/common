/**
 * @file ConsoleHandler.h
 * @brief Simple runner that can be used to create a quick console interface
 */
#pragma once

#ifndef INPUT_HANDLER_H
#define INPUT_HANDLER_H

//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/thread.hpp>
#include <boost/function.hpp>
#include <boost/noncopyable.hpp>

/**
 * @brief Simple runner that can be used to create a quick console interface
 *
 * An example of RAII usage:
 * @include example_console.cpp
 */
class ConsoleHandler : private boost::noncopyable
{
public:
	/** 
	 * @brief Connects the default signals to the handler
	 */
	ConsoleHandler(void);

	/**
	 * @brief Cleans all commands out of the handler
	 */
	virtual ~ConsoleHandler(void);

	/**
	 * @brief Starts a thread to run the console
	 */
	void start(void);

	/**
	 * @brief Used to trigger the sentinal to stop the console
	 */
	void stop(void);

	/** 
	 * @brief Used query the current status of the console
	 * @return true if currently running, false otherwise
	 */
	bool running(void) { return running_; }

protected:
	/** 
	 * @brief Helper function to add a command to the handler map
	 */
	void connect(char cmd, boost::function<void()> function, std::string help);

	/** 
	 * @brief Implement any functionality to call on stop in derived class
	 */
	virtual void stop_callback(void) {};

private:
	/**
	 * @brief Structure for holding a command and its help text
	 */
	struct command_t {
		std::string help;
		boost::function<void()> function;
	};
	typedef std::map<char, command_t> handler_t;

	/**
	 * @brief Structure to maintain the command handler list
	 */
	handler_t handler_;

	/**
	 * @brief Sentinal to control if the console is running
	 */
	bool running_;

	/**
	 * @brief The current running console runner
	 */
	boost::thread thread_;

	/**
	 * @brief The main running function for the console
	 */
	void runner(void);

	/**
	 * @brief Used to handle a user input and issue the relevant command
	 */
	void handle(char input);

private:
	/** 
	 * @brief Used to stop the running console
	 */
	void command_clear(void);

	/**
	 * @brief Used to issue the help for all available commands
	 */
	void command_help(void);

	/**
	 * @brief Used to stop the running console
	 */
	void command_exit(void);
};

#endif
