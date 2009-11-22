require 'socket'
include Socket::Constants
socket = Socket.new( AF_INET, SOCK_STREAM, 0 )
sockaddr = Socket.pack_sockaddr_in( 13337, '' )
socket.bind( sockaddr )
puts "Waiting for client..."
socket.listen( 5 )
client, client_sockaddr = socket.accept
puts "The client said, '#{client.readline.chomp}'"
#client.puts "Hello from script one!"
#socket.close