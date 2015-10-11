#! make
CC=gcc
CFLAGS=-c -Wall -std=gnu99 
LDFLAGS= 
MAKE=make
#SOURCES=*.c
SOURCES=main.c nfa_test.c
 
#gcc -static main.c -L. -lmean -o statically_linked
#$ gcc -Wall -I/opt/gdbm-1.8.3/include 
#   -L/opt/gdbm-1.8.3/lib dbmain.c -lgdbm

OBJECTS=$(SOURCES:.c=.o)
EXECUTABLE=regex_test.exe

# library path
# 


all: $(SOURCES) $(EXECUTABLE) 
	
# todo: automake autoconf
$(EXECUTABLE): $(OBJECTS) 
	$(CC) $(LDFLAGS) $(OBJECTS) -o $@

run: $(EXECUTABLE)
	./$(EXECUTABLE)

clean: 
	rm $(OBJECTS) 
	rm $(EXECUTABLE)

.c.o:
	$(CC) $(CFLAGS) $< -o $@

