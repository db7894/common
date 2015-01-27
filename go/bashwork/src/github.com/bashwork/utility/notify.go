// Package notify enables independent components of an application to
// observe notable events in a decoupled fashion.
//
// It generalizes the pattern of *multiple* consumers of an event (ie:
// the same message delivered to multiple channels) and obviates the need
// for components to have intimate knowledge of each other (only `import utility`
// and the name of the event are shared).
//
// Example:
//     // producer of "my_event"
//     go func() {
//         for {
//             time.Sleep(time.Duration(1) * time.Second):
//             notify.Publish("my_event", time.Now().Unix())
//         }
//     }()
//
//     // observer of "my_event" (normally some independent component that
//     // needs to be notified when "my_event" occurs)
//     output := make(chan interface{})
//     notify.Subscribe("my_event", output)
//     go func() {
//         for {
//             data := <- output
//             log.Printf("MY_EVENT: %#v", data)
//         }
//     }()
package utility

import (
	"errors"
	"sync"
	"time"
)

const ERROR_NOT_FOUND = "ERROR_NOT_FOUND"

// internal mapping of event names to observing channels
var events = make(map[string][]chan interface{})

// mutex for touching the event map
var mutex sync.RWMutex

// Start observing the specified event via provided output channel
func Subscribe(event string, output chan interface{}) {
	mutex.Lock()
	defer mutex.Unlock()

	events[event] = append(events[event], output)
}

// Stop observing the specified event on the provided output channel
func Unsubscribe(event string, output chan interface{}) error {
	mutex.Lock()
	defer mutex.Unlock()

	newArray := make([]chan interface{}, 0)
	channels, ok := events[event]
	if !ok {
		return errors.New(ERROR_NOT_FOUND)
	}
	for _, channel := range channels {
		if channel != output {
			newArray = append(newArray, channel)
		} else {
			close(channel)
		}
	}
	events[event] = newArray

	return nil
}

// Stop observing the specified event on all channels
func UnsubscribeAll(event string) error {
	mutex.Lock()
	defer mutex.Unlock()

	channels, ok := events[event]
	if !ok {
		return errors.New(ERROR_NOT_FOUND)
	}
	for _, channel := range channels {
		close(channel)
	}
	delete(events, event)

	return nil
}

// Post a notification to the specified event
func Publish(event string, data interface{}) error {
	mutex.RLock()
	defer mutex.RUnlock()

	channels, ok := events[event]
	if !ok {
		return errors.New(ERROR_NOT_FOUND)
	}
	for _, channel := range channels {
		channel <- data
	}

	return nil
}

// Post a notification to the specified event using the provided timeout for
// any output channels that are blocking
func PublishTimeout(event string, data interface{}, timeout time.Duration) error {
	mutex.RLock()
	defer mutex.RUnlock()

	channels, ok := events[event]
	if !ok {
		return errors.New(ERROR_NOT_FOUND)
	}
	for _, channel := range channels {
		select {
		  case channel <- data:
		  case <- time.After(timeout):
		}
	}

	return nil
}
