all: example_module

example_module: swig_example.i
	swig -c++ -python -o example_module.cc swig_example.i
	python setup.py build_ext --inplace

clean:
	rm -f example_module.cc _example.so example.py
	rm -rf build *.pyc
