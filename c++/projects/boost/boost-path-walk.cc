#include <iostream>
#include <boost/filesystem.hpp>
#include "boost/program_options.hpp" 

namespace po = boost::program_options; 
namespace fs = boost::filesystem;

/**
 * @summary Example main runner program
 * @param argc The number of arguments supplied
 * @param argv The arguments supplied
 */
int main(int argc, char** argv) {

    po::variables_map options; 
    po::options_description description("Options"); 
    description.add_options() 
        ("help,h", "Print this help message") 
        ("path,p", po::value<std::string>()->required(), "Path to find images in");

    try {
        po::store(po::parse_command_line(argc, argv, description), options);

        if (options.count("help")) {
            std::cout << description << std::endl; 
            return 0;
        }
        po::notify(options);

    } catch (po::error& ex) { 
        std::cerr << "ERROR: " << ex.what() << std::endl << std::endl; 
        std::cerr << description << std::endl; 
        return -1;
    } 

    fs::path root(options["path"].as<std::string>());

    if (fs::exists(root)) {
        if (fs::is_directory(root)) {
            fs::directory_iterator it_end;
            for (auto it = fs::directory_iterator(root); it != it_end; ++ it) {
                if (it->path().extension() == ".jpg")
                    std::cout << it->path() << std::endl;
            }
        } else {
            std::cout << root << std::endl;
        }
    } else {
        std::cout << "File or path does not exist" << std::endl;
        return -1;
    }

    return 0;
}
