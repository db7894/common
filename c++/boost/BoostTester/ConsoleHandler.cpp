/**
 * @file ConsoleHandler.cpp
 * @brief Simple runner that can be used to create a quick console interface
 */

//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include "stdafx.h"
#include <windows.h>
#include <conio.h>
#include <ctype.h>

#include <boost/bind.hpp>
#include "ConsoleHandler.h"

//---------------------------------------------------------------------------//
// Helpers
//---------------------------------------------------------------------------//
enum level {
	yellow	= FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY,
	white	= FOREGROUND_RED | FOREGROUND_BLUE | FOREGROUND_GREEN,
	red		= FOREGROUND_RED,
	blue	= FOREGROUND_BLUE,
	green	= FOREGROUND_GREEN,
	magenta	= FOREGROUND_RED | FOREGROUND_BLUE,
	cyan	= FOREGROUND_GREEN | FOREGROUND_BLUE,
};

/**
 * Uses the underlying windows console API
 * @param color The color enumeration value to set your text to
 * @return void
 */
static void set_color(int color)
{
	HANDLE hstdout = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(hstdout, color); 
}

/**
 * @brief Quick wrapper around printing with a color and back
 * @param input The text to colorize
 * @param color The color enumeration value to set your text to
 * @return void
 */
static void colorize(const char *input, int color)
{
	set_color(color);
	std::cout << input;
	set_color(white);
}

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 * @brief Connects the default signals to the handler
 */
ConsoleHandler::ConsoleHandler(void)
	: running_(false)
{
	connect('?', boost::bind(&ConsoleHandler::command_help, this),
		"Print the help information for all available commands");
	connect('Z', boost::bind(&ConsoleHandler::command_exit, this),
		"Exits from the terminal");
	connect('~', boost::bind(&ConsoleHandler::command_clear, this),
		"Clears the system terminal");
};

/**
 * Cleans out the handler structure
 */
ConsoleHandler::~ConsoleHandler(void)
{
	//std::for_each(handler_.begin(), handler_.end(),
	//	boost::bind(&handler_t::
	handler_.clear();
}

/**
 * @return void
 *
 * Note if the console is already running, another server
 * will not be started.
 */
void ConsoleHandler::start(void)
{
	if (!running_) {
		thread_ = boost::thread(
			boost::bind(&ConsoleHandler::runner, this));
	}
}

/**
 * @return void
 */
void ConsoleHandler::stop(void)
{
	stop_callback();
	running_ = false;
	thread_.join();
}

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//
/**
 * @param cmd The command the user will issue
 * @param function The command to connect to the command
 * @param help The help text for a command (default "")
 * @return void
 */
void ConsoleHandler::connect(char cmd, boost::function<void()> function, std::string help)
{
	command_t handle = {help, function};
	handler_[cmd] = handle;
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @return void
 */
void ConsoleHandler::runner(void)
{
	char input;
	running_ = true;

	while (running_) {
		input = (char)toupper(_getch());
		handle(input);
	}
};

/**
 * @param input The user input
 * @return void
 */
void ConsoleHandler::handle(char input)
{
	handler_t::iterator it = handler_.find(input);
	if (it != handler_.end()) {
		it->second.function();
	}
	else {
		colorize("Invalid Command. Please Enter A Valid Command\n", red);
		handler_['?'].function();
	}
}

//---------------------------------------------------------------------------//
// Private Commands
//---------------------------------------------------------------------------//
/**
 * @return void
 */
void ConsoleHandler::command_clear(void)
{
#ifdef WIN32
	system("cls");
#else
	std::cout << "\x1b[2J\x1b[1;1H" << std::flush;
#endif
}

/**
 * @return void
 */
void ConsoleHandler::command_help(void)
{
	handler_t::iterator end = handler_.end();
	set_color(yellow);
	std::cout << "----------------------------------------------------------------\n";
	std::cout << "-                       Console Help                           -\n";
	std::cout << "----------------------------------------------------------------\n";
	for (handler_t::iterator it = handler_.begin(); it != end; ++it) {
		std::cout << it->first << " -\t" << it->second.help << "\n";
	}
	std::cout << "----------------------------------------------------------------\n";
	set_color(white);
}

/**
 * @return void
 */
void ConsoleHandler::command_exit(void)
{
	colorize("Shutting Down...\n", red);
	running_ = false;
}

