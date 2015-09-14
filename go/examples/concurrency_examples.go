package main

import (
	"fmt"
	"math/rand"
	"time"
)

//
// An example channel generator
//
func channel(message string) <-chan string {
	c := make(chan string)
	go func() {
		for i := 0; ; i++ {
			c <- fmt.Sprintf("%s %d", msg, i)
			time.Sleep(time.Duration(rand.Intn(1e3)) * time.Millisecond)
		}
	}()
	return c
}

//
// An example of multiplexing channels using multiple
// go-routines.
//
// func main() {
// 	c := multiplex_1(channel("a"), channel("b"))
// 	for i := 0; i < 5; i++ {
// 		fmt.Printf("received: %q\n", <-c)
// 	}
// 	fmt.Println("finished")
// }
//
func multiplex_1(c1, c2 <-chan string) <-chan string {
	c := make(chan string)
	go func() {
		for {
			c <- <-c1
		}
	}()
	go func() {
		for {
			c <- <-c2
		}
	}()
	return c
}

//
// An example of multiplexing using select and a single
// go-routine.
//
// func main() {
// 	c := multiplex_2(channel("a"), channel("b"))
// 	for i := 0; i < 5; i++ {
// 		fmt.Printf("received: %q\n", <-c)
// 	}
// 	fmt.Println("finished")
// }
//
func multiplex_2(c1, c2 <-chan string) <-chan string {
	c := make(chan string)
	go func() {
		for {
			select {
			case s := <-c1:
				c <- s
			case s := <-c2:
				c <- s
			}
		}
	}()
	return c
}

//
// An example of waiting on a message or timing out
//
// func main() {
// 	c := timeout_example(channel("a"))
// 	fmt.Println("finished")
// }
//
func timeout_example(c <-chan string) {
	timeout := time.After(5 * time.Second)
	for {
		select {
		case msg := <-c:
			fmt.println(msg)
		case <-timeout:
			fmt.println("timed out")
			return
		}
	}
}

//
// An example of waiting on a message or timing out::
//
// func main() {
//     quit := make(chan bool)
//     c := quit_example("message", quit)
//     for i := rand.Intn(10); i >= 0; i-- {
//         fmt.Println(<-c)
//     }
//     quit <- true
//     fmt.Printf("finished: %q\n", <-quit)
// }
//
func quit_example(message string, quit <-chan string) {
	c := make(chan string)
	go func() {
		for i := 0; ; i++ {
			select {
			case c <- fmt.Sprintf("%s %d", message, i):
				time.Sleep(time.Duration(rand.Intn(1e3)) * time.Millisecond)
			case <-quit:
				quit <- "finished"
				return
			}
		}
	}()
	return c
}

type Result string
type Search func(query string) Result

//
// An example of sending multiple requests to various replicas
// and only keeping the first result.
//
func first_of_example(query string, replicas ...Search) Result {
	c := make(chan Result)
	replica := func(i int) { c <- replicas[i](query) }
	for i := range replicas {
		go replica(i)
	}
	return <-c
}

//
// An example of sending multiple requests to various endpoints
// and keeping all the results
//
func all_of_example(query string, services ...Search) (results []Result) {
	c := make(chan Result)
	for i := range services {
		go func() { c <- service[i](query) }()
	}

	for i := range services {
		result := <-c
		results = append(results, result)
	}
	return
}
