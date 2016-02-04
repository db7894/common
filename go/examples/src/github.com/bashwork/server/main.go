//
// An example of a pure golang RPC server
// https://golang.org/pkg/net/rpc/
//
package main

import (
	"os" // https://golang.org/pkg/flag/
	"server"
)

func main() {
	if len(os.Args) > 1 {
		start_http_client(os.Args[1])
	} else {
		start_http_server()
	}
}
