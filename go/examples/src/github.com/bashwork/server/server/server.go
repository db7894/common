//
// An example of a pure golang RPC server
// https://golang.org/pkg/net/rpc/
//
package server

import (
	"log"
	"net"
	"net/http"
	"net/rpc"
)

//
// Service interfaces
//

type Request struct {
	Query string
}

type Response struct {
	Result string
}

type Service int

//
// An example of a simple RPC service in go
//
func (s *Service) Search(request *Request, response *Response) error {
	log.Print("Searched for: " + request.Query)
	response.Result = "hello world"
	return nil
}

//
// Start the service as an HTTP RPC service
//
func start_http_server() {
	service := new(Service)
	rpc.Register(service)
	rpc.HandleHTTP()

	socket, err := net.Listen("tcp", ":8080")
	if err != nil {
		log.Fatal("failed to start server on the supplied port:", err)
	}
	log.Print("running server on localhost:8080")
	http.Serve(socket, nil)
	// go http.Serve(socket, nil)
}

//
// An example of a synchronous client
//
func start_http_client(host string) {
	client, err := rpc.DialHTTP("tcp", host+":8080")
	if err != nil {
		log.Fatal("failed to connect to server:", err)
	}

	request := Request{"home"}
	response := Response{"empty"}

	err = client.Call("Service.Search", request, &response)
	if err != nil {
		log.Fatal("service search error:", err)
	}
	log.Printf("Response: " + response.Result)
	client.Close()
}
