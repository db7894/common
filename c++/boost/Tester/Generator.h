// generator/continuation for C++
// author: Andrew Fedoniouk @ terrainformatica.com
// idea borrowed from: "coroutines in C" Simon Tatham,
//                     http://www.chiark.greenend.org.uk/~sgtatham/coroutines.html
 

//----------------------------------------------------------------------------//
// Structure
//----------------------------------------------------------------------------//
template <typename Type>
class generator
{
public:
	/**
	 * @brief Default constructor
	 */
	generator() : _state(0)
	{}

	/**
	 * @brief The magic
	 */
	bool operator()(Type& input)
	{
		switch (_state) {
		case 0:
			do {
				_state = __LINE__; case __LINE__:
				if (r(input))
					return true;
				else break;
			} while (1);
		}
		_state = 0;
		return false;
	}

protected:
	/**
	 * @brief Overloaded generate function
	 */
	virtual bool r(Type&) { return false; }

private:
	int _state;
};


