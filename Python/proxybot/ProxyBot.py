# Echo server program
import socket

HOST = ''                 # Symbolic name meaning all available interfaces
PORT = 13337              # Arbitrary non-privileged port
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
print 'Waiting for client...'
s.listen(1)
conn, addr = s.accept()
print 'Connected by', addr
data = conn.recv(1024)
if data: print 'Client sent: ', data
conn.close()
